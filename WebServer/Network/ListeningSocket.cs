using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Network
{
    public class ListeningSocket : IListeningSocket
    {
        private readonly Socket _socket;
        private readonly int _portNumber;

        public ListeningSocket(int portNumber) {
            _portNumber = portNumber;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.16"), _portNumber));
            _socket.Listen(10);
            Thread.Sleep(50);
        }


        public Socket Accept() {
            return
                _socket.Accept();
        }

        public void Dispose() {
            if (_socket != null) _socket.Close();
        }
    }
}
