﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    private ManualResetEvent _timerSent = new ManualResetEvent(false);
    private Thread _messageProcessingThread = null;
    private Dictionary<int, Vector3> _positions = new Dictionary<int, Vector3>();
    private Dictionary<int, StateObject> _states = new Dictionary<int, StateObject>();
    private Dictionary<int, Rigidbody> _rigidBodies = new Dictionary<int, Rigidbody>();
    private bool _rigidBodiesLoaded = false;
    private bool _clientsConnected = false;
    private int _numberOfFinishedPlayers = 0;
    private bool _joined = false;
    private Text _text;
    [SerializeField]
    private int _deadlineInSec = 100;
    private string _playerNames = "";
    private int _score = 0;
    private long _milisElapsedPrevious = 0;
    private long _lastLevelElapsed = 0;
    private byte[] _leftOverMessage = new byte[0];
    private bool _messageSent = false;
    private bool _solarsEnabled = false;
    private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Screen.fullScreen = false;
    }

    private void Start()
    {
        _text = GameObject.FindGameObjectWithTag("Timer").GetComponent<Text>();
        _currentNumOfClients = 0;
        _messageProcessingThread = new Thread(StartListening);
        _messageProcessingThread.Start();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (Screen.fullScreen)
            {
                Screen.SetResolution(1280, 720, false);
            }
            else
            {
                Screen.SetResolution(1920, 1080, true);
            }
        }

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
        _messageSent = false;

        if (!_rigidBodiesLoaded)
        {
            Rigidbody[] rigidBodies = FindObjectsOfType<Rigidbody>();
            Debug.Log(rigidBodies.Length);
            if (rigidBodies.Length >= maxNumberOfClients)
            {
                foreach (Rigidbody r in rigidBodies)
                {
                    PlayerControllerBase playerBase = r.GetComponent<PlayerControllerBase>();
                    if (playerBase && !_rigidBodies.ContainsKey(playerBase.ClientId))
                    {
                        //r.useGravity = false;
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
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                long elapsedTime = _stopwatch.ElapsedMilliseconds + _lastLevelElapsed;
                //Debug.Log(_stopwatch.ElapsedMilliseconds + " " + _lastLevelElapsed);
                _stopwatch.Start();
                if ((elapsedTime - _milisElapsedPrevious) >= 1000)
                {
                    _milisElapsedPrevious = elapsedTime;
                    if (_deadlineInSec >= (elapsedTime / 1000))
                    {
                        _text.text = (_deadlineInSec - (elapsedTime / 1000)).ToString();

                        foreach (KeyValuePair<int, StateObject> entry in _states)
                        {
                            TimeElapsed timeElapsed = new TimeElapsed();
                            timeElapsed.messageType = MessageType.TimeElapsed;
                            timeElapsed.miliseconds = elapsedTime;
                            Send(entry.Value, timeElapsed, false, false);
                        }
                        SolarFlareMovement[] solarFlareMovements = FindObjectsOfType<SolarFlareMovement>();
                        foreach (SolarFlareMovement sm in solarFlareMovements)
                        {
                            sm.NewTime(elapsedTime / 1000.0f);
                        }
                    }
                }
            }
            else
            {
                _stopwatch.Start();
            }

            List<int> keys = new List<int>(_positions.Keys);
            foreach (int key in keys)
            {
                _rigidBodies[key].MovePosition(_rigidBodies[key].position + _positions[key]);
                _positions[key] = Vector3.zero;
            }
        }
        ProcessMessage();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel(true);
        }
    }

    public void LevelFinishedRetracted()
    {
        --_numberOfFinishedPlayers;
    }

    public bool LevelFinished()
    {
        if (++_numberOfFinishedPlayers == _currentNumOfClients)
        {
            _lastLevelElapsed = _stopwatch.ElapsedMilliseconds + _lastLevelElapsed;
            _milisElapsedPrevious = _lastLevelElapsed;
            _stopwatch.Reset();
            foreach (KeyValuePair<int, StateObject> entry in _states)
            {
                NextLevel msg = new NextLevel();
                msg.messageType = MessageType.NextLevel;
                msg.lastLevelElapsed = _lastLevelElapsed;
                StateObject state = new StateObject();
                state.workSocket = entry.Value.workSocket;
                Send(state, msg, false, false);
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
    
    public void RestartLevel(bool forced)
    {
        if (forced)
        {
            _milisElapsedPrevious = _lastLevelElapsed;
            _stopwatch.Reset();
        }
        foreach (KeyValuePair<int, StateObject> entry in _states)
        {
            RestartLevel msg = new RestartLevel();
            msg.messageType = MessageType.RestartLevel;
            msg.lastLevelElapsed = _lastLevelElapsed;
            msg.forced = forced;
            StateObject state = new StateObject();
            state.workSocket = entry.Value.workSocket;
            Send(state, msg, false, false);
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
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("fe80::2444:881b:bf8c:86ca"), 11111);
        //IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
        _listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
            OtherPlayerConnected msg = new OtherPlayerConnected();
            msg.connected = true;
            msg.messageType = MessageType.OtherPlayerConnected;
            Debug.Log("Sending otherplayer connected: " + entry.Key);
            StateObject state = new StateObject();
            state.workSocket = entry.Value.workSocket;
            Send(state, msg, true, false);
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
                    Move move = (Move)msg;
                    _positions[move.clientId] = new Vector3(move.x, move.y, move.z);
                    foreach (KeyValuePair<int, StateObject> entry in _states)
                    {
                        if (entry.Key != move.clientId)
                        {
                            Move newMove = new Move();
                            newMove.x = move.x;
                            newMove.y = move.y;
                            newMove.z = move.z;
                            newMove.messageType = MessageType.Move;
                            newMove.clientId = move.clientId;
                            Send(entry.Value, newMove, false, false);
                            _positions[entry.Key] = new Vector3(move.x, move.y, move.z);
                        }
                    }
                }
                break;
            case MessageType.SyncPosition:
                SyncPosition syncPosition = (SyncPosition)msg;
                _rigidBodies[syncPosition.clientId].position = new Vector3(syncPosition.x, syncPosition.y, syncPosition.z);
                break;
            case MessageType.Disconnect:
                Disconnect disconnect = (Disconnect)msg;
                if (_playerNames.Length == 0)
                {
                    _playerNames += Encoding.UTF8.GetString(disconnect.playerName);
                }
                else
                {
                    _playerNames += "+" + Encoding.UTF8.GetString(disconnect.playerName);
                }
                _score = disconnect.score;

                if (--_currentNumOfClients == 0)
                {
                    foreach (KeyValuePair<int, StateObject> entry in _states)
                    {
                        _disconnected.Reset();
                        Disconnect message = new Disconnect();
                        message.messageType = MessageType.Disconnect;
                        message.clientId = entry.Key;

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
                MessageConnected messageConnected = (MessageConnected)msg;
                _positions.Add(messageConnected.clientId, Vector3.zero);
                _states.Add(messageConnected.clientId, state);
                // Signal the main thread to continue.
                _allDone.Set();
                StateObject newState = new StateObject();
                newState.workSocket = state.workSocket;
                Send(newState, msg, false, true);
            }
        }
    }

    private void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;
        try
        {
            // Read data from the client socket.
            if (handler.Connected)
            {
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    int position = 0;
                    if (_leftOverMessage.Length != 0)
                    {
                        Debug.Log(_leftOverMessage.Length);
                        byte[] expectedLengthBytes = state.buffer.Take(4).ToArray();
                        int expectedLength = BitConverter.ToInt32(expectedLengthBytes, 0);
                        expectedLength = expectedLength - _leftOverMessage.Length - 4;
                        byte[] messagePart = state.buffer.Take(expectedLength).ToArray();
                        messagePart = _leftOverMessage.Skip(4).ToArray().Concat(messagePart).ToArray();
                        Message msg = MessageUtils.Deserialize(messagePart);
                        _messageQueueMutex.WaitOne();
                        _messageQueue.Add(msg);
                        _messageQueueMutex.ReleaseMutex();
                        position = expectedLength;
                        Array.Clear(_leftOverMessage, 0, _leftOverMessage.Length);
                    }
                    while (position != StateObject.BufferSize)
                    {
                        byte[] expectedLengthBytes = state.buffer.Skip(position).Take(position + 4).ToArray();
                        int expectedLength = BitConverter.ToInt32(expectedLengthBytes, 0);
                        if ((position + 4 + expectedLength) > StateObject.BufferSize)
                        {
                            _leftOverMessage = state.buffer.Skip(position).Take(StateObject.BufferSize - position).ToArray();
                            break;
                        }
                        Debug.Log(expectedLength);
                        byte[] messagePart = state.buffer.Skip(position + 4).Take(expectedLength).ToArray();
                        Message msg = MessageUtils.Deserialize(messagePart);
                        _messageQueueMutex.WaitOne();
                        _messageQueue.Add(msg);
                        _messageQueueMutex.ReleaseMutex();
                        position += 4 + expectedLength;
                        if (position >= bytesRead)
                        {
                            break;
                        }
                    }
                    // Begin receiving the data from the remote device.
                    StateObject newState = new StateObject();
                    newState.workSocket = state.workSocket;
                    state.workSocket.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), newState);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(state.buffer.Length);
            Debug.Log(e.ToString());
        }
    }
    
    private void SendTimer(StateObject state, Message message)
    {
        byte[] byteData = MessageUtils.Serialize(message);

        if (state.workSocket.Connected)
        {
            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendTimerCallback), state);
        }
    }

    private void Send(StateObject state, Message message, bool startListening, bool ack)
    {
        byte[] byteData = MessageUtils.Serialize(message);

        if (state.workSocket.Connected)
        {
            // Begin sending the data to the remote device.
            if (startListening)
            {
                if (!ack)
                {
                    byteData = BitConverter.GetBytes(byteData.Length).Concat(byteData).ToArray();
                }
                state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallbackAndListen), state);
            }
            else
            {
                if (!ack)
                {
                    Debug.Log(byteData.Length);
                    byteData = BitConverter.GetBytes(byteData.Length).Concat(byteData).ToArray();
                }
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
            StateObject newState = new StateObject();
            newState.workSocket = state.workSocket;
            state.workSocket.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), newState);
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

    private void SendTimerCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            StateObject state = (StateObject)ar.AsyncState;
            // Complete sending the data to the remote device.
            int bytesSent = state.workSocket.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to client.");
            _timerSent.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void SendDisconnect(StateObject state, Message message)
    {
        byte[] byteData = MessageUtils.Serialize(message);
        byteData = BitConverter.GetBytes(byteData.Length).Concat(byteData).ToArray();
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
