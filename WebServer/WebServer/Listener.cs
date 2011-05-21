using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace MicroFrameworkWebServer.WebServer
{
    public delegate void RequestReceivedDelegate(Request request);

    public class Listener : IDisposable
    {
        const int MaxRequestSize = 1024;
        readonly int _portNumber = 80;

        private readonly Socket _listeningSocket;
        private readonly RequestReceivedDelegate _requestReceived;

        public Listener(RequestReceivedDelegate requestReceived)
            : this(requestReceived, 80) { }

        public Listener(RequestReceivedDelegate requestReceived, int portNumber)
        {
            _portNumber = portNumber;
            _requestReceived = requestReceived;
            _listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listeningSocket.Bind(new IPEndPoint(IPAddress.Any, _portNumber));
            _listeningSocket.Listen(10);

            new Thread(StartListening).Start();

        }

        ~Listener()
        {
            Dispose();
        }

        public void StartListening()
        {

            while (true)
            {
                using (Socket clientSocket = _listeningSocket.Accept())
                {
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    Debug.Print("Received request from " + clientIP);

                    int availableBytes = clientSocket.Available;
                    Debug.Print(DateTime.Now + " " + availableBytes + " request bytes available");

                    int bytesReceived = (availableBytes > MaxRequestSize ? MaxRequestSize : availableBytes);
                    if (bytesReceived > 0)
                    {
                        byte[] buffer = new byte[bytesReceived]; // Buffer probably should be larger than this.
                        
                        using (Request r = new Request(clientSocket, Encoding.UTF8.GetChars(buffer)))
                        {
                            Debug.Print(DateTime.Now + " " + r.URL);
                            if (_requestReceived != null) _requestReceived(r);

                        }


                    }
                }

                // I always like to have this in a continuous loop. Helps prevent lock-ups
                Thread.Sleep(10);
            }

        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_listeningSocket != null) _listeningSocket.Close();

        }

        #endregion
    }
}
