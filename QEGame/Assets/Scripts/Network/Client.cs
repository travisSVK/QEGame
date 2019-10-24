﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
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
    private bool _inputSent = false;
    private long _timeElapsedSec = 0;
    private Text _text = null;

    [SerializeField]
    private int _deadlineInSec = 100;
    private long _lastLevelElapsed = 0;
    private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
    private long _milisElapsedPrevious = 0;
    private int _completedStages = 0;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Screen.fullScreen = false;
    }

    private void Start()
    {
        _lastPosition = transform.position;
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

        ProcessMessage();
        if (!_text)
        {
            GameObject[] timerObj = GameObject.FindGameObjectsWithTag("Timer");
            if (timerObj.Length > 0)
            {
                _text = timerObj[0].GetComponent<Text>();
            }
        }
        if (_text)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                long elapsedTime = _stopwatch.ElapsedMilliseconds + _lastLevelElapsed;
                if (!_gameFinished && ((int)(elapsedTime / 1000) >= _deadlineInSec))
                {
                    _playerBase.InstantiateDeath();
                    _otherPlayerInput = Vector3.zero;
                    Destroy(_rigidbody.gameObject);
                    _rigidbody = null;
                    _gameFinished = true;
                    ShowHighScore();
                }
                _stopwatch.Start();
                if ((elapsedTime - _milisElapsedPrevious) >= 1000)
                {
                    _milisElapsedPrevious = elapsedTime;
                    _text.text = (_deadlineInSec - (elapsedTime / 1000)).ToString();
                }
            }
            else
            {
                _stopwatch.Start();
            }
        }
        if (_inputSent)
        {
            _inputSent = false;
        }
        else
        {
            _inputSent = true;
        }

        if (!_inputSent && !_gameFinished && _rigidbody && (_playerBase.movementIncrement != Vector3.zero)/*(_lastPosition != _rigidbody.transform.position)*/)
        {
            Move msg = new Move();
            msg.messageType = MessageType.Move;
            msg.clientId = clientId;
            //msg.timestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            msg.x = _playerBase.movementIncrement.x;
            msg.y = _playerBase.movementIncrement.y;
            msg.z = _playerBase.movementIncrement.z;
            _playerBase.movementIncrement = Vector3.zero;
            StateObject state = new StateObject();
            state.workSocket = _sender;
            Send(state, msg, false);
            //_sendDone.WaitOne();
        }

        if (_rigidbody && (_otherPlayerInput != Vector3.zero))
        {
            _rigidbody.MovePosition(_rigidbody.position + _otherPlayerInput);
            _otherPlayerInput = Vector3.zero;
        }

        if (_gameFinished && !_endSceneLoading)
        {
            HighscoreTable highscoreTable = FindObjectOfType<HighscoreTable>();
            if (highscoreTable && highscoreTable.isHighScoreClosed)
            {
                GameFinished(highscoreTable.lastName, highscoreTable.lastScore);
            }
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

    public void ShowHighScore()
    {
        EndScreen endScreen = FindObjectOfType<EndScreen>();
        if (endScreen)
        {
            endScreen.gameObject.SetActive(true);
            // TODO implement this
            _stopwatch.Stop();
            endScreen.SetCompletedStages(_completedStages);
            endScreen.SetRemainingTime(_deadlineInSec - _stopwatch.ElapsedMilliseconds / 1000);
            endScreen.ActivateScreen();
        }
    }

    public void GameFinished(string playerName, int lastScore)
    {
        Disconnect message = new Disconnect();
        message.messageType = MessageType.Disconnect;
        message.clientId = clientId;
        message.playerName = Encoding.UTF8.GetBytes(playerName);
        message.score = lastScore;
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
        Debug.Log(ipAddr.ToString());
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("fe80::2444:881b:bf8c:86ca"), 11111);
        //IPEndPoint remoteEndPoint = new IPEndPoint(ipAddr, 11111);
        _sender = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
            msg.messageType = MessageType.Connected;
            StateObject state = new StateObject();
            state.workSocket = _sender;
            Send(state, msg, false);
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
                Move move = (Move)msg;
                _otherPlayerInput = new Vector3(move.x, move.y, move.z);
                break;
            case MessageType.OtherPlayerConnected:
                Debug.Log("Other player connected received.");
                _messageProcessingThread.Join();
                LoadLevel menuManager = FindObjectOfType<LoadLevel>();
                if (menuManager)
                {
                    menuManager.StartFirstScene();
                    _stopwatch.Start();
                }
                break;
            case MessageType.NextLevel:
                CameraController cameraControlller = FindObjectOfType<CameraController>();
                if (cameraControlller)
                {
                    _playerBase.OnPlayerWin();
                    _otherPlayerInput = Vector3.zero;
                    Destroy(_rigidbody.gameObject);
                    _rigidbody = null;
                    if (cameraControlller.canMoveForward)
                    {
                        ++_completedStages;
                        _stopwatch.Reset();
                        NextLevel nl = (NextLevel)msg;
                        _lastLevelElapsed = nl.lastLevelElapsed;
                        cameraControlller.MoveForward();
                    }
                    else
                    {
                        _gameFinished = true;
                        ShowHighScore();
                    }
                }
                break;
            case MessageType.RestartLevel:
                CameraController cc = FindObjectOfType<CameraController>();
                if (cc)
                {
                    RestartLevel resLevel = (RestartLevel)msg;
                    if (resLevel.forced)
                    {
                        _lastLevelElapsed = resLevel.lastLevelElapsed;
                        _milisElapsedPrevious = _lastLevelElapsed;
                        _stopwatch.Reset();
                    }
                    _playerBase.InstantiateDeath();
                    _otherPlayerInput = Vector3.zero;
                    Destroy(_rigidbody.gameObject);
                    _rigidbody = null;
                    cc.RestartLevel();
                }
                break;
            case MessageType.TimeElapsed:
                //TimeElapsed timeElapsed = (TimeElapsed)msg;
                //long timeElapsedSec = timeElapsed.miliseconds / 1000;
                //Debug.Log((int)timeElapsedSec);
                //if (!_gameFinished && ((int)timeElapsedSec >= _deadlineInSec))
                //{
                //    _playerBase.InstantiateDeath();
                //    _otherPlayerInput = Vector3.zero;
                //    Destroy(_rigidbody.gameObject);
                //    _rigidbody = null;
                //    _gameFinished = true;
                //    ShowHighScore();
                //}
                //else
                //{
                //    if (!_text)
                //    {
                //        _text = GameObject.FindGameObjectWithTag("Timer").GetComponent<Text>();
                //    }
                //    _text.text = timeElapsedSec.ToString();
                //}
                break;
            case MessageType.Disconnect:
                Debug.Log("Disconnect response received.");
                Disconnect disconnect = (Disconnect)msg;
                if (disconnect.clientId == clientId)
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
        StateObject state = (StateObject)ar.AsyncState;
        Socket sender = state.workSocket;
        try
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.
            

            // Read data from the remote device.
            int bytesRead = sender.EndReceive(ar);

            if (bytesRead > 0)
            {
                Message msg = MessageUtils.Deserialize(state.buffer);
                if ((msg.messageType == MessageType.Connected) && (((MessageConnected)msg).clientId == clientId))
                {
                    _receiveDone.Set();
                    StateObject newState = new StateObject();
                    newState.workSocket = sender;

                    // Begin receiving the data from the remote device.
                    sender.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), newState);
                }
                else if ((msg.messageType == MessageType.Disconnect) && (((Disconnect)msg).clientId == clientId))
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
        // Retrieve the state object and the client socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket sender = state.workSocket;
        try
        {
            // Read data from the remote device.
            if (sender.Connected)
            {
                int bytesRead = sender.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Message msg = MessageUtils.Deserialize(state.buffer);
                    _messageQueueMutex.WaitOne();
                    _messageQueue.Enqueue(msg);
                    _messageQueueMutex.ReleaseMutex();

                    // Begin receiving the data from the remote device.
                    StateObject newState = new StateObject();
                    newState.workSocket = state.workSocket;
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

    private void Send(StateObject state, Message message, bool disconnect)
    {
        byte[] byteData = MessageUtils.Serialize(message);

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
