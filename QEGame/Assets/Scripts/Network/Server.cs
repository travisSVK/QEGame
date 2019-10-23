﻿using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server : MonoBehaviour
{
    //private AsyncListener _asyncListener;
    public int maxNumberOfClients = 2;
    private int _currentNumOfClients;
    private Mutex _messageQueueMutex = new Mutex();
    private Socket _listener;
    private List<Message> _messageQueue = new List<Message>();
    private ManualResetEvent _allDone = new ManualResetEvent(false);
    private ManualResetEvent _disconnected = new ManualResetEvent(false);
    private Thread _messageProcessingThread = null;
    private Dictionary<int, Vector3> _positions = new Dictionary<int, Vector3>();
    private Dictionary<int, StateObject> _states = new Dictionary<int, StateObject>();
    private Dictionary<int, Rigidbody> _rigidBodies = new Dictionary<int, Rigidbody>();
    private bool _rigidBodiesLoaded = false;
    private bool _clientsConnected = false;
    private int _numberOfFinishedPlayers = 0;
    private bool _joined = false;

    private string _playerNames = "";
    private int _score = 0;

    private void Start()
    {
        _currentNumOfClients = 0;
        _messageProcessingThread = new Thread(StartListening);
        _messageProcessingThread.Start();
    }

    private void Update()
    {
        // TODO update both players values depending on values in dictionary (in a
        // way which keeps Unity-specific values detached from our custom threads)
        if (!_clientsConnected)
        {
            return;
        }
        if (!_joined)
        {
            _messageProcessingThread.Join();
            _joined = true;
        }

        if (!_rigidBodiesLoaded)
        {
            Rigidbody[] rigidBodies = FindObjectsOfType<Rigidbody>();
            Debug.Log(rigidBodies.Length);
            if (rigidBodies.Length == maxNumberOfClients)
            {
                foreach (Rigidbody r in rigidBodies)
                {
                    r.useGravity = false;
                    PlayerControllerBase playerBase = r.GetComponent<PlayerControllerBase>();
                    if (playerBase)
                    {
                        _rigidBodies.Add(playerBase.ClientId, r);
                    }
                }
                _rigidBodiesLoaded = true;
            }
        }
        //else
        //{
        //    ProcessMessage();
        //    List<int> keys = new List<int>(_positions.Keys);
        //    foreach (int key in keys)
        //    {
        //        _rigidBodies[key].MovePosition(_rigidBodies[key].position + _positions[key]);
        //        _positions[key] = Vector3.zero;
        //    }
        //}

        if (_rigidBodiesLoaded)
        {
            List<int> keys = new List<int>(_positions.Keys);
            foreach (int key in keys)
            {
                _rigidBodies[key].MovePosition(_rigidBodies[key].position + _positions[key]);
                _positions[key] = Vector3.zero;
            }
        }
        ProcessMessage();
    }

    public void LevelFinishedRetracted()
    {
        --_numberOfFinishedPlayers;
    }

    public bool LevelFinished()
    {
        if (++_numberOfFinishedPlayers == _currentNumOfClients)
        {
            foreach (KeyValuePair<int, StateObject> entry in _states)
            {
                Message msg = new Message();
                msg.messageType = MessageType.NextLevel;
                Send(entry.Value, msg, false);
            }
            _rigidBodiesLoaded = false;
            foreach (Rigidbody rigidbody in _rigidBodies.Values)
            {
                PlayerControllerBase playerBase = rigidbody.GetComponent<PlayerControllerBase>();
                if (playerBase)
                {
                    playerBase.OnPlayerWin();
                }
                Destroy(rigidbody.gameObject);
            }
            _rigidBodies.Clear();
            _numberOfFinishedPlayers = 0;
            CameraController[] cameraControlllers = FindObjectsOfType<CameraController>();
            foreach (CameraController cameraController in cameraControlllers)
            {
                if (cameraController.canMoveForward)
                {
                    cameraController.MoveForward();
                }
            }
            _messageQueueMutex.WaitOne();
            {
                _messageQueue.Clear();
            }
            _messageQueueMutex.ReleaseMutex();
            return true;
        }
        return false;
    }

    // TODO
    public void RestartLevel()
    {
        foreach (KeyValuePair<int, StateObject> entry in _states)
        {
            Message msg = new Message();
            msg.messageType = MessageType.RestartLevel;
            Send(entry.Value, msg, false);
        }
        _rigidBodiesLoaded = false;
        foreach (Rigidbody rigidbody in _rigidBodies.Values)
        {
            Destroy(rigidbody.gameObject);
        }
        _rigidBodies.Clear();
        _numberOfFinishedPlayers = 0;

        List<int> keys = new List<int>(_positions.Keys);
        foreach (int key in keys)
        {
            _positions[key] = Vector3.zero;
        }
        CameraController[] cameraControlllers = FindObjectsOfType<CameraController>();
        foreach (CameraController cameraController in cameraControlllers)
        {
            cameraController.RestartLevel();
        }
        _messageQueueMutex.WaitOne();
        {
            _messageQueue.Clear();
        }
        _messageQueueMutex.ReleaseMutex();
    }

    private void StartListening()
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("fe80::11ec:b6d2:d1bf:e144"), 11111);
        //IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
        _listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            _listener.Bind(localEndPoint);
            _listener.Listen(2);

            while (_currentNumOfClients < maxNumberOfClients)
            {
                // Set the event to nonsignaled state.
                _allDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Debug.Log("Waiting for a connection...");
                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                // Wait until a connection is made before continuing.
                _allDone.WaitOne();
                ++_currentNumOfClients;
                Debug.Log("Player connected.");
            }
            _clientsConnected = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        foreach (KeyValuePair<int, StateObject> entry in _states)
        {
            OtherPlayerConnected opc = new OtherPlayerConnected();
            opc.connected = true;
            Message msg = new Message();
            msg.messageType = MessageType.OtherPlayerConnected;
            msg.otherPlayerConnected = opc;
            Debug.Log("Sending otherplayer connected: " + entry.Key);
            Send(entry.Value, msg, true);
        }
    }

    private void ProcessMessage()
    {
        Message msg = GetMessage();
        switch (msg.messageType)
        {
            case MessageType.Move:
                if (_currentNumOfClients == maxNumberOfClients)
                {
                    _positions[msg.move.clientId] = new Vector3(msg.move.x, msg.move.y, msg.move.z);
                    foreach (KeyValuePair<int, StateObject> entry in _states)
                    {
                        if (entry.Key != msg.move.clientId)
                        {
                            Send(entry.Value, msg, false);
                            _positions[entry.Key] = new Vector3(msg.move.x, msg.move.y, msg.move.z);
                        }
                    }
                }
                break;
            case MessageType.Disconnect:
                if (_playerNames.Length == 0)
                {
                    _playerNames += msg.disconnect.playerName;
                }
                else
                {
                    _playerNames += "+" + msg.disconnect.playerName;
                }
                _score = msg.disconnect.score;

                if (--_currentNumOfClients == 0)
                {
                    foreach (KeyValuePair<int, StateObject> entry in _states)
                    {
                        _disconnected.Reset();
                        Message message = new Message();
                        message.messageType = MessageType.Disconnect;
                        message.disconnect = new Disconnect();
                        message.disconnect.clientId = entry.Key;

                        Debug.Log("Sending disconnect: " + entry.Key);
                        SendDisconnect(entry.Value, message);
                        _disconnected.WaitOne();
                    }

                    HighscoreTable highscoreTable = FindObjectOfType<HighscoreTable>();
                    if (highscoreTable)
                    {
                        highscoreTable.AddHighscoreEntry(_score, _playerNames);
                    }

                    SpectatorView spectatorView = FindObjectOfType<SpectatorView>();
                    if (spectatorView)
                    {
                        spectatorView.Restart();
                        _listener.Close();
                    }
                }
                break;
            case MessageType.Uninitialized:
            default:
                break;
        }
    }

    private void DisconnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket sender = (Socket)ar.AsyncState;

            // Complete the disconnection.
            sender.EndDisconnect(ar);

            Debug.Log("Socket disconnected from " + sender.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            _disconnected.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private Message GetMessage()
    {
        Message msg = new Message();
        msg.messageType = MessageType.Uninitialized;
        _messageQueueMutex.WaitOne();
        if (_messageQueue.Count != 0)
        {
            msg = _messageQueue[0];
            _messageQueue.RemoveAt(0);
        }
        _messageQueueMutex.ReleaseMutex();
        return msg;
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        // Get the socket that handles the client request.
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        // Create the state object.
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveAckCallback), state);
    }

    private void ReceiveAckCallback(IAsyncResult ar)
    {
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            Message msg = MessageUtils.Deserialize(state.buffer);
            if (msg.messageType == MessageType.Connected)
            {
                _positions.Add(msg.messageConnected.clientId, Vector3.zero);
                _states.Add(msg.messageConnected.clientId, state);
                // Signal the main thread to continue.
                _allDone.Set();
                Send(state, msg, false);
            }
        }
    }

    private void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket.
        if (handler.Connected)
        {
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                Message msg = MessageUtils.Deserialize(state.buffer);
                _messageQueueMutex.WaitOne();
                _messageQueue.Add(msg);
                //_messageQueue.Sort((x, y) => x.timestamp.CompareTo(y.timestamp));
                _messageQueueMutex.ReleaseMutex();

                state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
        }
    }

    private void Send(StateObject state, Message message, bool startListening)
    {
        byte[] byteData = MessageUtils.Serialize(message);

        if (state.workSocket.Connected)
        {
            // Begin sending the data to the remote device.
            if (startListening)
            {
                state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallbackAndListen), state);
            }
            else
            {
                state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
            }
        }
    }

    private void SendCallbackAndListen(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            StateObject state = (StateObject)ar.AsyncState;
            // Complete sending the data to the remote device.
            int bytesSent = state.workSocket.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to client.");
            state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            StateObject state = (StateObject)ar.AsyncState;
            // Complete sending the data to the remote device.
            int bytesSent = state.workSocket.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to client.");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void SendDisconnect(StateObject state, Message message)
    {
        byte[] byteData = MessageUtils.Serialize(message);

        if (state.workSocket.Connected)
        {
            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallbackAndShutDown), state);
        }
    }

    private void SendCallbackAndShutDown(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            StateObject state = (StateObject)ar.AsyncState;
            // Complete sending the data to the remote device.
            int bytesSent = state.workSocket.EndSend(ar);
            Debug.Log("Closing client socket.");
            state.workSocket.Shutdown(SocketShutdown.Both);
            state.workSocket.Close();
            _disconnected.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
