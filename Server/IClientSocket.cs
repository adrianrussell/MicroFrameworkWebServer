using System;
using System.Net;

namespace Server
{
    public interface IClientSocket : IDisposable
    {
        void Send(string buffer);
        void Send(byte[] buffer,int size);
        void Close();
        IPEndPoint RemoteEndPoint { get; }
        int Available { get; }
    }
}