using System;
using System.IO;
using System.Net;
using Server.Network;
#if MF_FRAMEWORK_VERSION_V4_1
using Microsoft.SPOT;
#endif

namespace Server
{
    /// <summary>
    /// Holds information about a web request
    /// </summary>
    /// <remarks>
    /// Will expand as required, but stay simple until needed.
    /// </remarks>
    public class Request : IDisposable
    {
        private const int FileBufferSize = 256;
        private readonly char[] _data;
        private IClientSocket _client;
        private IStreamFactory _fileStreamFactory;

        public Request(IClientSocket client, char[] data) {
            _data = data;
            _client = client;
        }

        /// <summary>
        /// Request method
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// URL of request
        /// </summary>
        public string URL { get; private set; }

        /// <summary>
        /// Client IP address
        /// </summary>
        public IPAddress Client {
            get {
                IPEndPoint ip = _client.RemoteEndPoint;
                if (ip != null) return ip.Address;
                return null;
            }
        }

        /// <summary>
        /// Setup as property to support dependency injection
        /// </summary>
        public IStreamFactory FileStreamFactory {
            get { return _fileStreamFactory ?? (_fileStreamFactory = new FileStreamFactory()); }
        }

        #region IDisposable Members

        public void Dispose() {
            if (_client != null) {
                _client.Close();
                _client = null;
            }
        }

        #endregion

        /// <summary>
        /// Send a response back to the client
        /// </summary>
        /// <param name="response"></param>
        /// <param name="type"></param>
        public void SendResponse(string response, string type = "text/html") {
            if (_client != null) {
                string header = "HTTP/1.0 200 OK\r\nContent-Type: " + type + "; charset=utf-8\r\nContent-Length: " + response.Length +
                                "\r\nConnection: close\r\n\r\n";

                Log.Debug("header");
    
                _client.Send(header);
                _client.Send(response);
            }
        }

        /// <summary>
        /// Sends a file back to the client
        /// </summary>
        /// <remarks>
        /// Assumes the application using this has checked whether it exists
        /// </remarks>
        /// <param name="filePath"></param>
        public void SendFile(string filePath) {
            string type = "";
            if (DoesFileNameHaveExtentsion(filePath))
                type = MapMIMEType(filePath, type);


            using (Stream inputStream = FileStreamFactory.Create(filePath)) {
                // Send the header
                SendHeader(type, inputStream);

                SendResponseFromFile(inputStream);
            }
        }

        private static string MapMIMEType(string filePath, string type) {
            switch (GetFileExtentsion(filePath)) {
                case "css":
                    type = "text/css";
                    break;
                case "xml":
                case "xsl":
                    type = "text/xml";
                    break;
                case "jpg":
                case "jpeg":
                    type = "image/jpeg";
                    break;
                case "gif":
                    type = "image/gif";
                    break;
                    // Not exhaustive. Extend this list as required.
            }
            return type;
        }

        private static string GetFileExtentsion(string filePath) {
            return filePath.Substring(filePath.LastIndexOf('.') + 1);
        }

        private static bool DoesFileNameHaveExtentsion(string filePath) {
            return filePath.LastIndexOf('.') != 0;
        }

        private void SendHeader(string type, Stream inputStream) {
            string header = "HTTP/1.0 200 OK\r\nContent-Type: " + type + "; charset=utf-8\r\nContent-Length: " + inputStream.Length +
                            "\r\nConnection: close\r\n\r\n";

            _client.Send(header);
        }

        private void SendResponseFromFile(Stream inputStream) {
            var readBuffer = new byte[FileBufferSize];
            while (true) {
                // Send the file a few bytes at a time
                int bytesRead = inputStream.Read(readBuffer, 0, readBuffer.Length);
                if (NoBytesToRead(bytesRead))
                    break;

                _client.Send(readBuffer, bytesRead);
            }
        }

        private static bool NoBytesToRead(int bytesRead) {
            return bytesRead == 0;
        }

        /// <summary>
        /// Send a Not Found response
        /// </summary>
        public void Send404() {
            const string header = "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";
            if (_client != null)
                _client.Send(header);

        }

        public void ProcessRequestHeader() {
            ProcessRequestHeader(_data);
        }

        private void ProcessRequestHeader(char[] data) {
            var content = new string(data);
            var indexOf = content.IndexOf('\n');
            var firstLine = content.Substring(0, indexOf);

            // Parse the first line of the request: "GET /path/ HTTP/1.1"
            var words = firstLine.Split(' ');
            Method = words[0];
            URL = words[1];

            // Could look for any further headers in other lines of the request if required (e.g. User-Agent, Cookie)
        }
    }
}