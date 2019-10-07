using System.Text;
using System.Net.Sockets;

public class StateObject
{
    // Client  socket.  
    public Socket workSocket = null;
    public Socket sendSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 1024;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
}
