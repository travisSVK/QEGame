using System;
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
public struct OtherPlayerConnected
{
    public bool connected;
}

[Serializable]
public struct Disconnect
{
    public int clientId;
    public int score;
    public byte[] playerName;
}

[Serializable]
public struct TimeElapsed
{
    public long miliseconds;
}

[Serializable]
public struct Move
{
    public int clientId;
    public float x;
    public float y;
    public float z;
}

[Serializable]
public struct MessageConnected
{
    public bool connected;
    public int clientId;
}

[Serializable]
public struct Message
{
    public MessageType messageType;
    public MessageConnected messageConnected;
    public OtherPlayerConnected otherPlayerConnected;
    public Disconnect disconnect;
    public TimeElapsed timeElapsed;
    public Move move;
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
