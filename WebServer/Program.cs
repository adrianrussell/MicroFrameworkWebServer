using System.Threading;
using MicroFrameworkWebServer.WebServer;
using Microsoft.SPOT.Hardware;
using NetduinoPlusWebServer;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;

namespace MicroFrameworkWebServer
{
    public class Program
    {
        const string WebFolder = "\\SD\\Web";

        public static void Main()
        {

            Listener webServer = new Listener(RequestReceived);

            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            while (true)
            {
                // Blink LED to show we're still responsive
                led.Write(!led.Read());
                Thread.Sleep(500);
            }

        }


        private static void RequestReceived(Request request)
        {
            // Use this for a really basic check that it's working
            //request.SendResponse("<html><body><p>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL +"</p></body></html>");

            // Send a file
            TrySendFile(request);

        }

        /// <summary>
        /// Look for a file on the SD card and send it back if it exists
        /// </summary>
        /// <param name="request"></param>
        private static void TrySendFile(Request request)
        {
            // Replace / with \
            string filePath = WebFolder + request.URL.Replace('/', '\\');

            if (File.Exists(filePath))
                request.SendFile(filePath);
            else
                request.Send404();
        }

    }
}
