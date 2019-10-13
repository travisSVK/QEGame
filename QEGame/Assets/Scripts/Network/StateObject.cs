using System.Net.Sockets;

public class StateObject
{
    public Socket workSocket = null;

    public const int BufferSize = 1024;

    // Receiving buffer.
    public byte[] buffer = new byte[BufferSize];
}
