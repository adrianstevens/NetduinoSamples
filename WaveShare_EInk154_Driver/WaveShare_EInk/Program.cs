using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

// https://www.waveshare.com/wiki/1.54inch_e-Paper_Module
namespace WaveShare_EInk
{
    public class Program
    {
        static int COLORED = 0;
        static int UNCOLORED = 1;

        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            var epd = new Epd1in54(chipSelectPin: Pins.GPIO_PIN_D4,
                dcPin: Pins.GPIO_PIN_D7,
                resetPin: Pins.GPIO_PIN_D6,
                busyPin: Pins.GPIO_PIN_D5,
                spiModule: SPI.SPI_module.SPI1,
                speedKHz: 1500);

            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();
            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();

            var paint = new EpdPaint(new byte[1024], 0, 0);

            paint.SetRotate(0);

            paint.SetWidth(64);
            paint.SetHeight(64);

            paint.Clear(UNCOLORED);
            paint.DrawRectangle(0, 0, 40, 50, COLORED);
            paint.DrawLine(0, 0, 40, 50, COLORED);
            paint.DrawLine(40, 0, 0, 50, COLORED);
         //   paint.DrawStringAt(30, 4, "N3 WiFi", Fonts.Fon, UNCOLORED);
            epd.SetFrameMemory(paint.GetImage(), 16, 60, paint.GetWidth(), paint.GetHeight());



            epd.DisplayFrame();

            System.Threading.Thread.Sleep(2000);

            epd.Init(Epd1in54.LUT_Partial_Update);

            /*
            paint.SetWidth(200);
            paint.SetHeight(24);

            epd.Init(Epd1in54.LUT_Full_Update);

            epd.ClearFrameMemory(0);   // bit set = white, bit reset = black
            epd.DisplayFrame();
            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();

            /*
            paint.SetWidth(64);
            paint.SetHeight(64);
            paint.Clear(COLORED);
            paint.DrawRectangle(0, 0, 40, 50, COLORED);
            paint.DrawLine(0, 0, 40, 50, COLORED);
            paint.DrawLine(40, 0, 0, 50, COLORED);
            epd.SetFrameMemory(paint.GetImage(), 16, 60, paint.GetWidth(), paint.GetHeight());
            */
            System.Threading.Thread.Sleep(-1);

            /*    paint.SetRotate(EpdPaint.ROTATE_0);

                paint.SetWidth(64);
                paint.SetHeight(64);

                paint.Clear(UNCOLORED);
                paint.DrawRectangle(0, 0, 40, 50, COLORED);
                paint.DrawLine(0, 0, 40, 50, COLORED);
                paint.DrawLine(40, 0, 0, 50, COLORED);
                epd.SetFrameMemory(paint.GetImage(), 16, 60, paint.GetWidth(), paint.GetHeight());
                //       paint.SetWidth(64);
                //       paint.SetHeight(64);

                epd.DisplayFrame();

                epd.Init(Epd1in54.LUT_Partial_Update);


                while (true); */
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
