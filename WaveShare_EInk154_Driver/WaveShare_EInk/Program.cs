using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

// https://www.waveshare.com/wiki/1.54inch_e-Paper_Module
namespace WaveShare_EInk
{
    public class Program
    {
        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            var epd = new Epd1in54(); //gross name

            epd.BusyPin = new OutputPort(Cpu.Pin.GPIO_Pin7, false);
            epd.ResetPin = new OutputPort(Cpu.Pin.GPIO_Pin6, false);
            epd.DCPin = new OutputPort(Cpu.Pin.GPIO_Pin5, false);
            epd.CSPin = new OutputPort(Cpu.Pin.GPIO_Pin3, false);
            

            epd.Init(Epd1in54.LUT_Full_Update);

            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();
            epd.ClearFrameMemory(0xFF);   // bit set = white, bit reset = black
            epd.DisplayFrame();


            while (true);
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
