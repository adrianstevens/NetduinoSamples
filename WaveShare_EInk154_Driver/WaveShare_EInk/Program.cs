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

            var epd = new Epd1in54();
            var paint = new EpdPaint(new byte[1024], 200, 200);

            epd.BusyPin = new OutputPort(Cpu.Pin.GPIO_Pin7, false);
            epd.ResetPin = new OutputPort(Cpu.Pin.GPIO_Pin6, false);
            epd.DCPin = new OutputPort(Cpu.Pin.GPIO_Pin5, false);
            epd.CSPin = new OutputPort(Cpu.Pin.GPIO_Pin10, false); 
            
            epd.Init(Epd1in54.LUT_Full_Update);

            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();
            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();

            paint.SetWidth(64);
            paint.SetHeight(64);
            paint.Clear(COLORED);
            paint.DrawRectangle(0, 0, 40, 50, COLORED);
            paint.DrawLine(0, 0, 40, 50, COLORED);
            paint.DrawLine(40, 0, 0, 50, COLORED);
            epd.SetFrameMemory(paint.GetImage(), 16, 60, paint.GetWidth(), paint.GetHeight());


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
