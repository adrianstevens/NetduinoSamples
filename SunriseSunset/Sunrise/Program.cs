using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace Sunrise
{
    public class Program
    {
        public static void Main()
        {
            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            Thread.Sleep(4000);

            var sunrise = new SunriseService();

            var data = sunrise.GetSunriseSunset(49.246, -123.1162);

            Debug.Print(data.ToString());
        }
    }
}
