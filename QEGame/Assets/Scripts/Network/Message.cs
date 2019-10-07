using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct MessageConnected
{
    public bool connected;
    public int clientId;
}

[Serializable]
public struct Message
{
    public bool isInitialized;
    public float x;
    public float y;
    public float z;
    public int clientId;
}

public class MessageUtils
{
    public static MessageType Deserialize<MessageType>(byte[] buffer)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(buffer, 0, buffer.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            MessageType message = (MessageType)binForm.Deserialize(memStream);
            return message;
        }
    }

    public static byte[] Serialize<MessageType>(MessageType message)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, message);
            return ms.ToArray();
        }
    }
}
