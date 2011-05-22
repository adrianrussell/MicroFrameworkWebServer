using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Network
{
    public class ClientSocket : IClientSocket
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        private Socket _socket;

        public ClientSocket(Socket socket) {
            _socket = socket;
        }

        public void Send(string buffer) {
            _socket.Send(_encoding.GetBytes(buffer), buffer.Length, SocketFlags.None);
        }

        public void Send(byte[] buffer,int size)
        {
            _socket.Send(buffer, size, SocketFlags.None);
        }

        public IPEndPoint RemoteEndPoint { get { return _socket.RemoteEndPoint as IPEndPoint; }
        }

        public int Available {
            get { return _socket.Available; }
        }


        public void Close() {
            if (_socket == null) return;

            _socket.Close();
            _socket = null;
        }

        public void Dispose() {
            Close();
        }
    }
}
