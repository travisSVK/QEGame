﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client : MonoBehaviour
{
    public int clientId;
    private IPHostEntry _ipHost;
    private IPAddress _ipAddr;
    private IPEndPoint _localEndPoint;
    private ManualResetEvent _connectDone = new ManualResetEvent(false);
    private ManualResetEvent _sendDone = new ManualResetEvent(false);
    private ManualResetEvent _receiveDone = new ManualResetEvent(false);
    private Mutex _messageQueueMutex = new Mutex();
    private Queue<Message> _messageQueue = new Queue<Message>();
    private Socket _sender;
    private Thread _messageProcessingThread = null;
    private bool _otherClientConnected = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (_otherClientConnected)
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager)
            {
                menuManager.StartFirstScene();
            }
        }

        //if (_lastPosition != transform.position)
        //{
        //    Message msg = new Message();
        //    msg.clientId = clientId;
        //    _lastPosition = transform.position;
        //    msg.x = transform.position.x;
        //    msg.y = transform.position.y;
        //    msg.z = transform.position.z;
        //    msg.isInitialized = true;
        //    StateObject state = new StateObject();
        //    state.workSocket = _sender;
        //    Send<Message>(state, msg);
        //    _sendDone.WaitOne();
        //}
    }

    public void ConnectToServer()
    {
        _messageProcessingThread = new Thread(Connect);
        _messageProcessingThread.Start();
    }

    private void Connect()
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint remoteEndPoint = new IPEndPoint(ipAddr, 11111);
        _sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Connect to the remote endpoint.
            _sender.BeginConnect(remoteEndPoint,
                new AsyncCallback(ConnectCallback), _sender);
            _connectDone.WaitOne();

            _sendDone.Reset();
            _receiveDone.Reset();
            // TODO send ID here.
            MessageConnected msg = new MessageConnected();
            msg.connected = true;
            msg.clientId = clientId;
            StateObject state = new StateObject();
            state.workSocket = _sender;
            Send(state, msg);
            _sendDone.WaitOne();

            // Receive the ACK from the remote device.
            ReceiveAck(_sender);
            _receiveDone.WaitOne();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void ProcessMessages()
    {
        while (true)
        {
            // TODO add other message types processing here (that probably means adding wrappers around the
            // generic message stating which type of message it is (using enum) and adding a switch here).
            Message msg = GetMessage();
            if (msg.isInitialized)
            {
                if (!_otherClientConnected)
                {
                    _otherClientConnected = true;
                }
                
                Debug.Log("Client id: " + msg.clientId + " client position: " + new Vector3(msg.x, msg.y, msg.z));
            }
        }
    }

    private Message GetMessage()
    {
        Message msg = new Message();
        msg.isInitialized = false;
        _messageQueueMutex.WaitOne();
        if (_messageQueue.Count != 0)
        {
            msg = _messageQueue.Dequeue();
            msg.isInitialized = true;
        }
        _messageQueueMutex.ReleaseMutex();
        return msg;
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket sender = (Socket)ar.AsyncState;

            // Complete the connection.
            sender.EndConnect(ar);

            Debug.Log("Socket connected to " + sender.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            _connectDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void ReceiveAck(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveAckCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void ReceiveAckCallback(IAsyncResult ar)
    {
        String content = String.Empty;
        try
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket sender = state.workSocket;

            // Read data from the remote device.
            int bytesRead = sender.EndReceive(ar);

            if (bytesRead > 0)
            {
                MessageConnected msg = MessageUtils.Deserialize<MessageConnected>(state.buffer);
                if (msg.connected && (msg.clientId == clientId))
                {
                    _receiveDone.Set();
                    StateObject newState = new StateObject();
                    newState.workSocket = sender;

                    // Begin receiving the data from the remote device.
                    sender.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), newState);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket sender = state.workSocket;

            // Read data from the remote device.
            int bytesRead = sender.EndReceive(ar);

            if (bytesRead > 0)
            {
                Message msg = MessageUtils.Deserialize<Message>(state.buffer);
                _messageQueueMutex.WaitOne();
                _messageQueue.Enqueue(msg);
                _messageQueueMutex.ReleaseMutex();

                // Begin receiving the data from the remote device.
                sender.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void Send<MessageType>(StateObject state, MessageType message)
    {
        byte[] byteData = MessageUtils.Serialize<MessageType>(message);

        // Begin sending the data to the remote device.
        state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), state);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            StateObject state = (StateObject)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = state.workSocket.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to server.");

            // Signal that all bytes have been sent.
            _sendDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}