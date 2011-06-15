using System.Threading;
using MicroFrameworkWebServer.WebServer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetduinoPlusWebServer;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;
using Server;
using Server.Network;
using System.Net;

namespace MicroFrameworkWebServer
{
    public class Program
    {
        const string WebFolder = "\\SD";

        public static void Main() {


            //NetworkInterface.GetAllNetworkInterfaces()[0].EnableDhcp();
            NetworkInterface.GetAllNetworkInterfaces()[0].EnableStaticIP("192.168.0.16","255.255.255.0","192.168.0.1");
//            NetworkInterface.GetAllNetworkInterfaces()[0].RenewDhcpLease();

  //          var ipAddress = NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress;

            Thread.Sleep(1000);

            Debug.Print("Dirs: ");
            Debug.Print(Directory.GetCurrentDirectory());
            Directory.SetCurrentDirectory(@"\SD");
            string[] dirs = Directory.GetDirectories(@"\SD");

  
            var listener = new Listener(RequestReceived);
                listener.Start();
            

            var led = new OutputPort(Pins.ONBOARD_LED, false);
            while (true)
            {
                // Blink LED to show we're still responsive
                led.Write(!led.Read());
                Thread.Sleep(2000);
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

            if (File.Exists(@"\SD\default.html"))
                request.SendFile(filePath);
            else
                request.Send404();
        }

    }
}
