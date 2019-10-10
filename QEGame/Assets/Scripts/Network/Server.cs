using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server : MonoBehaviour
{
    //private AsyncListener _asyncListener;
    public int maxNumberOfClients = 2;
    private int _currentNumOfClients;
    private bool _gameFinished;
    private Mutex _messageQueueMutex = new Mutex();
    private Mutex _positionsMutex = new Mutex();
    private Socket _listener;
    private Queue<Message> _messageQueue = new Queue<Message>();
    private ManualResetEvent _allDone = new ManualResetEvent(false);
    private Thread _messageProcessingThread = null;
    private Dictionary<int, Vector3> _positions = new Dictionary<int, Vector3>();
    private Dictionary<int, StateObject> _states = new Dictionary<int, StateObject>();

    private void Start()
    {
        _gameFinished = false;
        _currentNumOfClients = 0;
        _messageProcessingThread = new Thread(StartListening);
        _messageProcessingThread.Start();
    }

    private void Update()
    {
        // TODO update both players values depending on values in dictionary (in a
        // way which keeps Unity-specific values detached from our custom threads)
        _positionsMutex.WaitOne();
        foreach (KeyValuePair<int, Vector3> entry in _positions)
        {
            //Debug.Log("Client id: " + entry.Key + " client position: " + entry.Value);
        }
        _positionsMutex.ReleaseMutex();
        if (_gameFinished)
        {
            _messageProcessingThread.Join();
            SpectatorView spectatorView = FindObjectOfType<SpectatorView>();
            if (spectatorView)
            {
                spectatorView.Restart();
            }
        }
    }

    private void StartListening()
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
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
                ++_currentNumOfClients;
                _allDone.WaitOne();
                Debug.Log("Player connected.");
            }
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
            Debug.Log("Sending otherplayer connected.");
            Send<Message>(entry.Value, msg, true);
        }
        ProcessMessages();
    }

    private void ProcessMessages()
    {
        while (true)
        {
            Message msg = GetMessage();
            switch (msg.messageType)
            {
                case MessageType.Move:
                    _positionsMutex.WaitOne();
                    _positions[msg.move.clientId] = new Vector3(msg.move.x, msg.move.y, msg.move.z);
                    _positionsMutex.ReleaseMutex();
                    break;
                case MessageType.Disconnect:
                    Socket socket = _states[msg.disconnect.clientId].workSocket;
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    --_currentNumOfClients;
                    break;
                case MessageType.Uninitialized:
                default:
                    break;
            }
            if (_currentNumOfClients == 0)
            {
                _listener.Close();
                _gameFinished = true;
                break;
            }
        }
    }

    private Message GetMessage()
    {
        Message msg = new Message();
        msg.messageType = MessageType.Uninitialized;
        _messageQueueMutex.WaitOne();
        if (_messageQueue.Count != 0)
        {
            msg = _messageQueue.Dequeue();
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
            Message msg = MessageUtils.Deserialize<Message>(state.buffer);
            if (msg.messageType == MessageType.Connected)
            {
                _positionsMutex.WaitOne();
                _positions.Add(msg.messageConnected.clientId, Vector3.zero);
                _positionsMutex.ReleaseMutex();
                _states.Add(msg.messageConnected.clientId, state);
                // Signal the main thread to continue.
                _allDone.Set();
                state.sendSocket = state.workSocket;
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
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            Message msg = MessageUtils.Deserialize<Message>(state.buffer);
            _messageQueueMutex.WaitOne();
            _messageQueue.Enqueue(msg);
            _messageQueueMutex.ReleaseMutex();

            state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
    }

    private void Send<MessageType>(StateObject state, MessageType message, bool startListening)
    {
        byte[] byteData = MessageUtils.Serialize<MessageType>(message);
        
        // Begin sending the data to the remote device.
        if (startListening)
        {
            state.sendSocket.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallbackAndListen), state);
        }
        else
        {
            state.sendSocket.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), state);
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
}
