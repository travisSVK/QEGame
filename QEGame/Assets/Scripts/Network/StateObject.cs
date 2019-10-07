﻿using System.Net.Sockets;

public class StateObject
{
    public Socket workSocket = null;
    public Socket sendSocket = null;

    public const int BUFFER_SIZE = 1024;

    // Receiving buffer.
    public byte[] buffer = new byte[BUFFER_SIZE];
}
