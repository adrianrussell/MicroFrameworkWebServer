using System.Threading;
using MicroFrameworkWebServer.WebServer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetduinoPlusWebServer;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;
using Server;

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


            var webServer = new Server.WebServer {WebFolder = WebFolder, Port = 80};

            webServer.Start();


            var led = new OutputPort(Pins.ONBOARD_LED, false);
            while (true)
            {
                // Blink LED to show we're still responsive
                led.Write(!led.Read());
                Thread.Sleep(2000);
            }

        }

    }
}
