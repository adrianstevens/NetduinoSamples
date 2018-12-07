using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;

namespace GPS
{
    public class Program
    {
        public static void Main()
        {
            var serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

            var gpsDataReader = new GpsReader(serialPort, 500, 0.0);

            gpsDataReader.GpsData += GpsDataReceived;
            gpsDataReader.Start();

            while (true)
            {
                Debug.Print("---");

                Thread.Sleep(10000);
            }
        }

        private static void GpsDataReceived(GpsPoint gpsPoint)
        {
            var lat = gpsPoint.Latitude.ToString("F4");
            var lon = gpsPoint.Longitude.ToString("F4");

            Debug.Print("Lat/long: " + lat + ", " + lon);
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