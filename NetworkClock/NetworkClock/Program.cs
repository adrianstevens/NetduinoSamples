using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Displays;
using Netduino.Foundation;
using Network;

namespace NetworkClock
{
    public class Program
    {
        static SSD1306 oledDisplay;
        static ILI9163 tftDisplay;

        static GraphicsLibrary tftGraphics, oledGraphics;

        static Font4x8 font4x8;
        static Font8x12 font8x12;

        static byte[] meadowLogo;

        static int count;

        static string updName = "NetClock";
        static bool isConnected = false;

        public static int BlinkRate = 500;
        public static void Main()
        {
            meadowLogo = Resources.GetBytes(Resources.BinaryResources.meadow_logo);

            InitDisplays();

            UpdateDisplays();

            Debug.Print("Network Clock");

            UpdateOled("No IP address", "Connecting ...");

            Thread.Sleep(200);

            Netduino.Foundation.Network.Initializer.NetworkConnected += Initializer_NetworkConnected;

            Netduino.Foundation.Network.Initializer.InitializeNetwork(updName);

            var led = new OutputPort(Pins.ONBOARD_LED, false);

            Debug.Print("InitializeNetwork()");

            while(true)
            {
                count++;
                led.Write(true);
                Thread.Sleep(BlinkRate);
                led.Write(false);
                Thread.Sleep(BlinkRate);
                UpdateDisplays();
            }
                      
            Thread.Sleep(-1);
        }

        static void UpdateDisplays ()
        {
            tftGraphics.Clear();


            if(isConnected == false)
                DrawClock(-1, 0, 0);
            else
                DrawClock(second: count%60);
            tftGraphics.Show();
        }

        static void UpdateOled(string message1, string message2)
        {
            oledGraphics.Clear();
            oledGraphics.DrawText(2, 2, message1);
            oledGraphics.DrawText(2, 18, message2);
            oledGraphics.Show(); 
        }

        static void InitDisplays ()
        {
            tftDisplay = new ILI9163(chipSelectPin: Pins.GPIO_PIN_D4,
                dcPin: Pins.GPIO_PIN_D7,
                resetPin: Pins.GPIO_PIN_D6,
                width: 128,
                height: 160,
                spiModule: SPI.SPI_module.SPI1,
                speedKHz: 25000);

            oledDisplay = new SSD1306(0x3C, 400, SSD1306.DisplayType.OLED128x32);

            font4x8 = new Font4x8();
            font8x12 = new Font8x12();

            tftGraphics = new GraphicsLibrary(tftDisplay) { CurrentFont = font4x8 };
            oledGraphics = new GraphicsLibrary(oledDisplay) { CurrentFont = font8x12 };
        }

        static private void DrawClock (int hour = 6, int minute = 20, int second = 30)
        {
            int xCenter = 64;
            int yCenter = 80;

            int x, y;

            Draw24bppBitmap(24, 60, meadowLogo, tftDisplay);

            //ticks - let's do 60
            for (int i = 0; i < 60; i++)
            {
                x = (int)(xCenter + 50 * System.Math.Sin(i * System.Math.PI / 30));
                y = (int)(yCenter - 50 * System.Math.Cos(i * System.Math.PI / 30));

                if (i % 5 == 0)
                    tftGraphics.DrawCircle(x, y, 2, Color.White);
                else
                    tftDisplay.DrawPixel(x, y, true);
            }

            if (hour == -1)
            {
                tftGraphics.DrawText(44, 64, "Wilderness", Color.Gray);
                return;
            }

            int xT, yT;

            //hour
            x = (int)(xCenter + 23 * System.Math.Sin(hour * System.Math.PI / 6));
            y = (int)(yCenter - 23 * System.Math.Cos(hour * System.Math.PI / 6));
            xT = (int)(xCenter + 3 * System.Math.Sin((hour - 3) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((hour - 3) * System.Math.PI / 6));
            tftGraphics.DrawLine(xT, yT, x, y, Color.LawnGreen);
            xT = (int)(xCenter + 3 * System.Math.Sin((hour + 3) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((hour + 3) * System.Math.PI / 6));
            tftGraphics.DrawLine(xT, yT, x, y, Color.LawnGreen);

            //minute
            x = (int)(xCenter + 35 * System.Math.Sin(minute * System.Math.PI / 30));
            y = (int)(yCenter - 35 * System.Math.Cos(minute * System.Math.PI / 30));
            xT = (int)(xCenter + 3 * System.Math.Sin((minute - 15) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((minute - 15) * System.Math.PI / 6));
            tftGraphics.DrawLine(xT, yT, x, y, Color.LawnGreen);
            xT = (int)(xCenter + 3 * System.Math.Sin((minute + 15) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((minute + 15) * System.Math.PI / 6));
            tftGraphics.DrawLine(xT, yT, x, y, Color.LawnGreen);

            //second
            x = (int)(xCenter + 40 * System.Math.Sin(second * System.Math.PI / 30));
            y = (int)(yCenter - 40 * System.Math.Cos(second * System.Math.PI / 30));
            tftGraphics.DrawLine(xCenter, yCenter, x, y, Color.Red);
        }

        static void Draw24bppBitmap(int x, int y, byte[] data, ILI9163 display)
        {
            byte r, g, b;

            int offset = 14 + data[14];

            int width = data[18];
            int height = data[22];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    b = data[i * 3 + j * width * 3 + offset];
                    g = data[i * 3 + j * width * 3 + offset + 1];
                    r = data[i * 3 + j * width * 3 + offset + 2];

                    display.DrawPixel(x + i, y + height - j, r, g, b);
                }
            }
        }

        private static void Initializer_NetworkConnected(object sender, EventArgs e)
        {
            Debug.Print("Connected! (do work)");

            var ip = Netduino.Foundation.Network.Initializer.CurrentNetworkInterface.IPAddress;

            // UpdateOled(updName, ip);
            UpdateOled(updName, ip);

            isConnected = true;
            BlinkRate = 0;
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