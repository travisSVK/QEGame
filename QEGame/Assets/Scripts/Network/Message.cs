﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum MessageType
{
    Connected,
    Move,
    OtherPlayerConnected,
    RestartLevel,
    Disconnect,
    Uninitialized,
    TimeElapsed,
    NextLevel
}

[Serializable]
public class Message
{
    public MessageType messageType;
}

[Serializable]
public class MessageConnected : Message
{
    public bool connected;
    public int clientId;
}

[Serializable]
public class Move : Message
{
    public int clientId;
    public float x;
    public float y;
    public float z;
    public float bx;
    public float by;
    public float bz;
}

[Serializable]
public class OtherPlayerConnected : Message
{
    public bool connected;
}

[Serializable]
public class Disconnect : Message
{
    public int clientId;
    public int score;
    public byte[] playerName = new byte[100];
}

[Serializable]
public class TimeElapsed : Message
{
    public long miliseconds;
}

public static class MessageUtils
{
    public static Message Deserialize(byte[] buffer)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        ms.Write(buffer, 0, buffer.Length);
        ms.Seek(0, SeekOrigin.Begin);
        return (Message)bf.Deserialize(ms);
        //using (var ms = new MemoryStream())
        //{
        //    var binForm = new BinaryFormatter();
        //    //ms.Seek(0, SeekOrigin.Begin);
        //    Message message = (Message)binForm.Deserialize(ms);
        //    return message;
        //}
    }

    public static byte[] Serialize(Message message)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, message);
        return ms.ToArray();
        //using (var ms = new MemoryStream())
        //{
        //    bf.Serialize(ms, message);
        //    return ms.ToArray();
        //}
    }
}
