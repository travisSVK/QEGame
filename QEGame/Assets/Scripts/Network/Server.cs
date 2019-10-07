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
    public int maxNumberOfCLients = 2;

    private int _currentNumOfClients;
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
        _currentNumOfClients = 0;
        StartListening();
    }

    private void Update()
    {
        // TODO update both players values depending on values in dictionary (do it in this fashion so unity specific values are detached from our custom threads)
        //_positionsMutex.WaitOne();
        //foreach (KeyValuePair<int, Vector3> entry in m_positions)
        //{
        //    Debug.Log("Client id: " + entry.Key + " client position: " + entry.Value);
        //}
        //_positionsMutex.ReleaseMutex();
    }

    private void StartListening()
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(2);

            while (_currentNumOfClients < maxNumberOfCLients)
            {
                // Set the event to nonsignaled state.  
                _allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                Debug.Log("Waiting for a connection...");
                //Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                ++_currentNumOfClients;
                _allDone.WaitOne();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        _messageProcessingThread = new Thread(ProcessMessages);
        _messageProcessingThread.Start();
    }

    private void ProcessMessages()
    {
        while (true)
        {
            // process messages here
            Message msg = GetMessage();
            if (msg.initialized)
            {
                // TODO have dictionary of clientIds and players locally
                _positionsMutex.WaitOne();
                _positions[msg.clientId] = new Vector3(msg.x, msg.y, msg.z);
                _positionsMutex.ReleaseMutex();
            }
        }
    }

    private Message GetMessage()
    {
        Message msg = new Message();
        msg.initialized = false;
        _messageQueueMutex.WaitOne();
        if (_messageQueue.Count != 0)
        {
            msg = _messageQueue.Dequeue();
            msg.initialized = true;
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
            MessageConnected msg = MessageUtils.Deserialize<MessageConnected>(state.buffer);
            if (msg.connected)
            {
                _positions.Add(msg.clientId, Vector3.zero);
                _states.Add(msg.clientId, state);
                // Signal the main thread to continue.  
                _allDone.Set();
                state.sendSocket = state.workSocket;
                Send(state, msg);
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
            int clientId = msg.clientId;
            _positionsMutex.WaitOne();
            foreach (KeyValuePair<int, Vector3> entry in _positions)
            {
                if (entry.Key != msg.clientId)
                {
                    clientId = entry.Key;
                    break;
                }
            }
            _positionsMutex.ReleaseMutex();
            state.sendSocket = _states[clientId].workSocket;
            Send(state, msg); 
            //Send(state, msg);
        }
    }
    
    private void Send<MessageType>(StateObject state, MessageType message)
    {
        byte[] byteData = MessageUtils.Serialize<MessageType>(message);

        // Begin sending the data to the remote device.  
        state.sendSocket.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), state);
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
            state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void SendAndClose(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendAndCloseCallback), handler);
    }

    private void SendAndCloseCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to client. Closing connection.");

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
