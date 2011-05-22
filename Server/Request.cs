#region Copyright and file header

// Copyright 2010 Adrian Russell - All Rights Reserved.

#endregion

using System;
using System.IO;
using System.Net;
using Server.Network;

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
        private IClientSocket _client;
        private IFileStreamFactory _fileStreamFactory;
        private string _method;
        private string _url;
        private char[] _data;

        public Request(IClientSocket client, char[] data) {
            _data = data;
            _client = client;
         
        }

        /// <summary>
        /// Request method
        /// </summary>
        public string Method {
            get { return _method; }
        }

        /// <summary>
        /// URL of request
        /// </summary>
        public string URL {
            get { return _url; }
        }

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

        public IFileStreamFactory FileStreamFactory {
            get {
                if (_fileStreamFactory == null)
                    _fileStreamFactory = new FileStreamFactory();

                return _fileStreamFactory;
            }
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
            // Map the file extension to a mime type
            string type = "";
            int dot = filePath.LastIndexOf('.');
            if (dot != 0)
                switch (filePath.Substring(dot + 1)) {
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


            using (Stream inputStream = FileStreamFactory.Create(filePath))
            {
                // Send the header
                string header = "HTTP/1.0 200 OK\r\nContent-Type: " + type + "; charset=utf-8\r\nContent-Length: " + inputStream.Length +
                                "\r\nConnection: close\r\n\r\n";

                _client.Send(header);

                var readBuffer = new byte[FileBufferSize];
                while (true) {
                    // Send the file a few bytes at a time
                    int bytesRead = inputStream.Read(readBuffer, 0, readBuffer.Length);
                    if (bytesRead == 0)
                        break;

                    _client.Send(readBuffer, bytesRead);
                 
                }
            }

         
        }

        /// <summary>
        /// Send a Not Found response
        /// </summary>
        public void Send404() {
            const string header = "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";
            if (_client != null)
                _client.Send(header);
         
        }

        public void ProcessRequest() {
            ProcessRequest(_data);
        }

        /// <summary>
        /// Process the request header
        /// </summary>
        /// <param name="data"></param>
        private void ProcessRequest(char[] data) {
            var content = new string(data);
            var firstLine = content.Substring(0, content.IndexOf('\n'));

            // Parse the first line of the request: "GET /path/ HTTP/1.1"
            var words = firstLine.Split(' ');
            _method = words[0];
            _url = words[1];

            // Could look for any further headers in other lines of the request if required (e.g. User-Agent, Cookie)
        }
    }
}