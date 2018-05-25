using System;
using Microsoft.SPOT;
using Netduino.Foundation.Network;
using Maple;

namespace MapleImageServer
{
    public class Program
    {
        public static string ImageFileName = @"SD\StressTestSmall\Pic_9.jpg";
        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            Netduino.Foundation.Network.Initializer.NetworkConnected += Initializer_NetworkConnected;
            Netduino.Foundation.Network.Initializer.InitializeNetwork();
        }

        private static void Initializer_NetworkConnected(object sender, EventArgs e)
        {
            var mapleServer = new MapleServer();
            mapleServer.Start();

            Debug.Print("Maple started");
        }
    }
}
