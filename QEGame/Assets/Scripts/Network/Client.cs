using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public int clientId;
    private IPHostEntry _ipHost;
    private IPAddress _ipAddr;
    private IPEndPoint _localEndPoint;
    private ManualResetEvent _connectDone = new ManualResetEvent(false);
    private ManualResetEvent _sendDone = new ManualResetEvent(false);
    private ManualResetEvent _receiveDone = new ManualResetEvent(false);
    private ManualResetEvent _disconnectDone = new ManualResetEvent(false);
    private Mutex _messageQueueMutex = new Mutex();
    private Queue<Message> _messageQueue = new Queue<Message>();
    private Socket _sender;
    private Thread _messageProcessingThread = null;
    private bool _endSceneLoading = false;
    private bool _gameFinished = false;
    private Vector3 _lastPosition;
    private Rigidbody _rigidbody;
    private Vector3 _otherPlayerInput = Vector3.zero;
    private PlayerControllerBase _playerBase;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        ProcessMessage();

        if (!_gameFinished && _rigidbody && (_playerBase.input != Vector3.zero)/*(_lastPosition != _rigidbody.transform.position)*/)
        {
            Message msg = new Message();
            msg.messageType = MessageType.Move;
            msg.move.clientId = clientId;
            //_lastPosition = _rigidbody.transform.position;
            //PlayerControllerBase playerBase = _rigidbody.GetComponent<PlayerControllerBase>();
            msg.move.x = _playerBase.input.x;
            msg.move.y = _playerBase.input.y;
            msg.move.z = _playerBase.input.z;
            StateObject state = new StateObject();
            state.workSocket = _sender;
            Send<Message>(state, msg, false);
            _sendDone.WaitOne();
        }

        if (_gameFinished && !_endSceneLoading)
        {
            GameFinished();
        }
    }

    private void FixedUpdate()
    {
        //if (!_gameFinished && _rigidbody && (_lastPosition != _rigidbody.transform.position))
        //{
        //    Message msg = new Message();
        //    msg.messageType = MessageType.Move;
        //    msg.move.clientId = clientId;
        //    _lastPosition = _rigidbody.transform.position;
        //    PlayerControllerBase playerBase = _rigidbody.GetComponent<PlayerControllerBase>();
        //    msg.move.x = playerBase.input.x;
        //    msg.move.y = playerBase.input.y;
        //    msg.move.z = playerBase.input.z;
        //    StateObject state = new StateObject();
        //    state.workSocket = _sender;
        //    Send<Message>(state, msg, false);
        //    _sendDone.WaitOne();
        //}

        if (_rigidbody && (_otherPlayerInput != Vector3.zero))
        {
            PlayerControllerBase playerBase = _rigidbody.GetComponent<PlayerControllerBase>();
            _rigidbody.MovePosition(_rigidbody.position + _otherPlayerInput * playerBase.MovementSpeed * Time.fixedDeltaTime);
            _otherPlayerInput = Vector3.zero;
        }
    }

    public Rigidbody rb
    {
        set
        {
            _rigidbody = value;
            _playerBase = FindObjectOfType<PlayerControllerBase>();
            _lastPosition = _rigidbody.position;
        }
    }

    public void GameFinished()
    {
        Message message = new Message();
        message.messageType = MessageType.Disconnect;
        message.disconnect = new Disconnect();
        message.disconnect.clientId = clientId;
        StateObject state = new StateObject();
        state.workSocket = _sender;
        _disconnectDone.Reset();
        Send(state, message, true);
        _disconnectDone.WaitOne();
        Debug.Log("Disconnect request sent.");
        _endSceneLoading = true;
        SceneManager.LoadScene("EndScene");
    }

    public void Restart()
    {
        _sender.Shutdown(SocketShutdown.Both);
        _sender.Close();
        Debug.Log("Game finished.");

        EndSceneManager endSceneManager = FindObjectOfType<EndSceneManager>();
        if (endSceneManager)
        {
            Debug.Log("Endscene manager called.");
            endSceneManager.Restart();
        }
        else
        {
            Debug.Log("Endscene manager not called.");
        }
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
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("fe80::11ec:b6d2:d1bf:e144"), 11111);
        _sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            _connectDone.Reset();
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
            Message message = new Message();
            message.messageType = MessageType.Connected;
            message.messageConnected = msg;
            Send(state, message, false);
            _sendDone.WaitOne();

            // Receive the ACK from the remote device.
            ReceiveAck(_sender);
            _receiveDone.WaitOne();
            Debug.Log("ACK received.");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void ProcessMessage()
    {
        Message msg = GetMessage();
        switch (msg.messageType)
        {
            case MessageType.Move:
                _otherPlayerInput = new Vector3(msg.move.x, msg.move.y, msg.move.z);
                break;
            case MessageType.OtherPlayerConnected:
                Debug.Log("Other player connected received.");
                _messageProcessingThread.Join();
                MenuManager menuManager = FindObjectOfType<MenuManager>();
                if (menuManager)
                {
                    menuManager.StartFirstScene();
                }
                break;
            case MessageType.NextLevel:
                _otherPlayerInput = Vector3.zero;
                _rigidbody = null;
                CameraController cameraControlller = FindObjectOfType<CameraController>();
                if (cameraControlller)
                {
                    if (cameraControlller.canMoveForward)
                    {
                        cameraControlller.MoveForward();
                    }
                    else
                    {
                        _gameFinished = true;
                    }
                }
                break;
            case MessageType.Disconnect:
                Debug.Log("Disconnect response received.");
                if (msg.disconnect.clientId == clientId)
                {
                    Debug.Log("Disconnect response received.");
                    Restart();
                }
                break;
            case MessageType.Uninitialized:
                break;
            default:
                break;
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
                Message msg = MessageUtils.Deserialize<Message>(state.buffer);
                if ((msg.messageType == MessageType.Connected) && (msg.messageConnected.clientId == clientId))
                {
                    _receiveDone.Set();
                    StateObject newState = new StateObject();
                    newState.workSocket = sender;

                    // Begin receiving the data from the remote device.
                    sender.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), newState);
                }
                else if ((msg.messageType == MessageType.Disconnect) && (msg.disconnect.clientId == clientId))
                {
                    _receiveDone.Set();
                }
                else
                {
                    StateObject newState = new StateObject();
                    newState.workSocket = sender;
                    sender.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveAckCallback), newState);
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
            if (sender.Connected)
            {
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
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void Send<MessageType>(StateObject state, MessageType message, bool disconnect)
    {
        byte[] byteData = MessageUtils.Serialize<MessageType>(message);

        if (state.workSocket.Connected)
        {
            if (disconnect)
            {
                state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                   new AsyncCallback(SendDisconnectCallback), state);
            }
            else
            {
                state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                   new AsyncCallback(SendCallback), state);
            }
        }
    }

    private void SendDisconnectCallback(IAsyncResult ar)
    {
        try
        {
            StateObject state = (StateObject)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = state.workSocket.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to server.");

            // Signal that all bytes have been sent.
            _disconnectDone.Set();
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
