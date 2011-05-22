using System;
using System.Net.Sockets;

namespace Server.Network
{
    public interface IListeningSocket : IDisposable
    {
        Socket Accept();
    }
}