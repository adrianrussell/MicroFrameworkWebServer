using System;
using System.Text;
using System.Threading;
using Server;
using Server.Network;

namespace MicroFrameworkWebServer.WebServer
{
    public delegate void RequestReceivedDelegate(Request request);

    public class Listener : IDisposable
    {
        const int MaxRequestSize = 1024;
        readonly int _portNumber = 80;

        private IListeningSocket _listeningSocket;
        private readonly RequestReceivedDelegate _requestReceived;

        public Listener(RequestReceivedDelegate requestReceived)
            : this(requestReceived, 80) { }

        public Listener(RequestReceivedDelegate requestReceived, int portNumber)
        {
            _portNumber = portNumber;
            _requestReceived = requestReceived;


        }

        public void Start() {
            new Thread(StartListening).Start();
        }

        ~Listener()
        {
            Dispose();
        }

        public IListeningSocket ListeningSocket {
            get { return _listeningSocket ?? (_listeningSocket = new ListeningSocket(_portNumber)); }
        }

        private void StartListening()
        {

            while (true)
            {
                using (IClientSocket clientSocket = new ClientSocket(ListeningSocket.Accept()))
                {
                    Log.Debug("Received request from " + clientSocket.RemoteEndPoint);

                    Log.Debug(DateTime.Now + " " + clientSocket.Available + " request bytes available");

                    int bytesReceived = DetermineBytesReceived(clientSocket.Available);
                    if (bytesReceived > 0)
                    {
                        byte[] buffer = clientSocket.Receive(bytesReceived); // Buffer probably should be larger than this.
                        
                        using (var r = new Request(clientSocket, Encoding.UTF8.GetChars(buffer)))
                        {
                            r.ProcessRequestHeader();
                            Log.Debug(DateTime.Now + " " + r.URL);
                            if (_requestReceived != null) _requestReceived(r);
                        }
                    }
                }

                // I always like to have this in a continuous loop. Helps prevent lock-ups
                Thread.Sleep(10);
            }

        }

        private int DetermineBytesReceived(int availableBytes) {
            return (availableBytes > MaxRequestSize ? MaxRequestSize : availableBytes);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (ListeningSocket != null) ListeningSocket.Dispose();

        }

        #endregion
    }
}
