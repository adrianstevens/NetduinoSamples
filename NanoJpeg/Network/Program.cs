using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using KeyJ;

namespace Network
{
    public class Program
    {
        public static int BlinkRate = 100;
        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            Netduino.Foundation.Network.Initializer.NetworkConnected += Initializer_NetworkConnected;

            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            var led = new OutputPort(Pins.ONBOARD_LED, false);

            Debug.Print("InitializeNetwork()");

            var nano = new NanoJPEG();
            nano.njDecode(new byte[4]);
            var img = nano.njGetImage();
            
                
            while(true)
            {
                led.Write(true);
                Thread.Sleep(BlinkRate);
                led.Write(false);
                Thread.Sleep(BlinkRate);
            }
        }

        private static void Initializer_NetworkConnected(object sender, EventArgs e)
        {
            Debug.Print("Connected! (do work)");

            BlinkRate = 1000;
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