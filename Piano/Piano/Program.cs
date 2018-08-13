using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation.LEDs;
using N = SecretLabs.NETMF.Hardware.Netduino;
using NF = Netduino.Foundation;
using Netduino.Foundation.Piezos;

namespace Piano
{
    public class Program
    {
        static Led[] leds = new Led[4];
        static float[] notes = new float[] {261.63f, 329.63f, 392, 523.25f };

        static InterruptPort[] buttons = new InterruptPort[4];
        static PiezoSpeaker speaker;

        public static void Main()
        {
            Debug.Print("Welcome to Piano");

            Initialize();

            while (true)
            {

            }
        }

         static void Initialize()
         { 
            leds[0] = new Led(N.Pins.GPIO_PIN_D10);
            leds[1] = new Led(N.Pins.GPIO_PIN_D11);
            leds[2] = new Led(N.Pins.GPIO_PIN_D12);
            leds[3] = new Led(N.Pins.GPIO_PIN_D13);

            buttons[0] = new InterruptPort(N.Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[1] = new InterruptPort(N.Pins.GPIO_PIN_D1, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[2] = new InterruptPort(N.Pins.GPIO_PIN_D2, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[3] = new InterruptPort(N.Pins.GPIO_PIN_D3, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            
            buttons[0].OnInterrupt += OnButton0;
            buttons[1].OnInterrupt += OnButton1;
            buttons[2].OnInterrupt += OnButton2;
            buttons[3].OnInterrupt += OnButton3;

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D5);

            SetAllLEDs(false);
        }

        private static void SetAllLEDs(bool isOn)
        {
            leds[0].IsOn = isOn;
            leds[1].IsOn = isOn;
            leds[2].IsOn = isOn;
            leds[3].IsOn = isOn;
        }

        private static void ToggleNote(int index, bool play)
        {
            if (play)
            {
                speaker.PlayTone(notes[index]);
                leds[index].IsOn = true;
            }
            else
            {
                speaker.StopTone();
                leds[index].IsOn = false;
            }
        }

        private static void OnButton0(uint data1, uint data2, DateTime time)
        {
            ToggleNote(0, data2 == 1);
        }

        private static void OnButton1(uint data1, uint data2, DateTime time)
        {
            ToggleNote(1, data2 == 1);
        }

        private static void OnButton2(uint data1, uint data2, DateTime time)
        {
            ToggleNote(2, data2 == 1);
        }
        private static void OnButton3(uint data1, uint data2, DateTime time)
        {
            ToggleNote(3, data2 == 1);
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