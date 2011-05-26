using System.Net;
using System.Net.Sockets;

namespace Server.Network
{
    public class ListeningSocket : IListeningSocket
    {
        private readonly Socket _listeningSocket;
        private readonly int _portNumber;

        public ListeningSocket(int portNumber) {
            _portNumber = portNumber;

            _listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listeningSocket.Bind(new IPEndPoint(IPAddress.Any, _portNumber));
            _listeningSocket.Listen(10);
        }


        public Socket Accept() {
            return _listeningSocket.Accept();
        }

        public void Dispose() {
            if (_listeningSocket != null) _listeningSocket.Close();
        }
    }
}
