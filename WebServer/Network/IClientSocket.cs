using System;
using System.Net;

namespace Server.Network
{
    public interface IClientSocket : IDisposable
    {
        int Send(string buffer);
        int Send(byte[] buffer, int size);
        void Close();
        IPEndPoint RemoteEndPoint { get; }
        int Available { get; }
        byte[] Receive(int bytesReceived);
    }
}