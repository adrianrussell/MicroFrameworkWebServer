using System;
using System.Net.Sockets;

namespace Server
{
    public interface IListeningSocket : IDisposable
    {
        Socket Accept();
    }
}