using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Network
{
    public class Program
    {
        public static int BlinkRate = 250;
        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            Netduino.Foundation.Network.Initializer.NetworkConnected += Initializer_NetworkConnected;

            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            var led = new OutputPort(Pins.ONBOARD_LED, false);

            Debug.Print("InitializeNetwork()");

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
            Debug.Print("InitializeNetwork()");

            BlinkRate = 250;
        }
    }
}
