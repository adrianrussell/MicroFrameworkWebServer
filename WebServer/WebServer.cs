using System.IO;
using MicroFrameworkWebServer.WebServer;
using NetduinoPlusWebServer;

namespace Server
{
    public class WebServer
    {
        public string WebFolder { get; set; }
        public int Port { get; set; }

        public void Start() {
            var listener = new Listener(RequestReceived);
            listener.Start();
        }

        public void Stop() {
            
        }

        private void RequestReceived(Request request)
        {
            // Use this for a really basic check that it's working
            //request.SendResponse("<html><body><p>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL +"</p></body></html>");

            // Send a file
            TrySendFile(request);

        }

        private void TrySendFile(Request request)
        {
            // Replace / with \
            string filePath = WebFolder + request.URL.Replace('/', '\\');

            if (File.Exists(@"\SD\default.html"))
                request.SendFile(filePath);
            else
                request.Send404();
        }
    }
}