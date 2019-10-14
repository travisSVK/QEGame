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
    NextLevel
}

[Serializable]
public struct MessageConnected
{
    public bool connected;
    public int clientId;
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
public struct OtherPlayerConnected
{
    public bool connected;
}

[Serializable]
public struct Disconnect
{
    public int clientId;
}


[Serializable]
public struct Message
{
    public MessageType messageType;
    public int timestamp;
    public MessageConnected messageConnected;
    public OtherPlayerConnected otherPlayerConnected;
    public Move move;
    public Disconnect disconnect;
}

public class MessageUtils
{
    public static Message Deserialize(byte[] buffer)
    {
        using (var ms = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            ms.Write(buffer, 0, buffer.Length);
            ms.Seek(0, SeekOrigin.Begin);
            Message message = (Message)binForm.Deserialize(ms);
            ms.Flush();
            ms.Close();
            ms.Dispose();
            return message;
        }
    }

    public static byte[] Serialize(Message message)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, message);
            byte[] array = ms.ToArray();
            ms.Flush();
            ms.Close();
            ms.Dispose();
            return array;
        }
    }
}
