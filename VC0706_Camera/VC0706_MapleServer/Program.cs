using System;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using Maple;

namespace VC0706_MapleServer
{
    public class Program
    {
        public static string ImageFileName = @"SD\StressTestSmall\Pic_7.jpg";

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

namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}
