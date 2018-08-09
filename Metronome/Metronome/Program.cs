using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation.LEDs;
using N = SecretLabs.NETMF.Hardware.Netduino;
using NF = Netduino.Foundation;

namespace Simone
{
    public class Program
    {
        static int ANIMATION_DELAY = 200;

        static Led[] leds = new Led[4];
        static float[] notes = new float[] {261.63f, 329.63f, 392, 523.25f };

        static InterruptPort[] buttons = new InterruptPort[4];
        static PiezoSpeaker speaker;

        static float MAX_BPM = 240;
        static float MIN_BPM = 60;
        static float BPM = 140;
        static float interval;
        static int step = 0;
        static DateTime lastTick = DateTime.Now;
        static bool isPlaying = false;
        static bool isThreadActive = false;

        static bool isAnimating = false;

        public static void Main()
        {
            Debug.Print("Welcome to Metronome");

            Initialize();

            while (true)
            {

            }
        }

        static void StartMetronome()
        {
            if (isThreadActive)
                return;

            isPlaying = true;

            interval = 60000f / BPM; //ms
            lastTick = DateTime.Now;

            var thread = new Thread(() =>
            {
                isThreadActive = true;
                while (isPlaying)
                {
                    lastTick = DateTime.Now;

                    leds[step].IsOn = true;
                    if (step == 3)
                        speaker.PlayTone(440, 50);
                    else
                        speaker.PlayTone(220, 50);
                    Thread.Sleep((int)interval);

                    if (step == 3)
                        SetAllLEDs(false);

                    step = (step + 1) % 4;
                }
                isThreadActive = false;
            });
            thread.Start();
        }

        static void StopMetronome()
        {
            isPlaying = false;
            Thread.Sleep(100);
            SetAllLEDs(false);
        }

         static void Initialize()
         { 
            leds[0] = new Led(N.Pins.GPIO_PIN_D10);
            leds[1] = new Led(N.Pins.GPIO_PIN_D11);
            leds[2] = new Led(N.Pins.GPIO_PIN_D12);
            leds[3] = new Led(N.Pins.GPIO_PIN_D13);

            buttons[0] = new InterruptPort(N.Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[1] = new InterruptPort(N.Pins.GPIO_PIN_D1, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[2] = new InterruptPort(N.Pins.GPIO_PIN_D2, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[3] = new InterruptPort(N.Pins.GPIO_PIN_D3, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            
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

        private static void ShowStartAnimation ()
        {
            if (isAnimating)
                return;

            isAnimating = true;

            SetAllLEDs(false);

            for(int i = 0; i < 4; i++)
            {
                leds[i].IsOn = true;
                Thread.Sleep(ANIMATION_DELAY);
            }

            for (int i = 0; i < 4; i++)
            {
                leds[3 - i].IsOn = false;
                Thread.Sleep(ANIMATION_DELAY);
            }

            isAnimating = false;
        }


        //ToDo remove
        private static void TurnOnLED(int index, int durration = 400)
        {
            leds[index].IsOn = true;

            speaker.PlayTone(notes[index], durration);
            leds[index].IsOn = false;
        }


        private static void OnButton0(uint data1, uint data2, DateTime time)
        {
            StartMetronome();
        }

        private static void OnButton1(uint data1, uint data2, DateTime time)
        {
            StopMetronome();
        }

        private static void OnButton2(uint data1, uint data2, DateTime time)
        {
            if (BPM == MAX_BPM)
                return;

            BPM++;
            interval = 60000f / BPM;

        }
        private static void OnButton3(uint data1, uint data2, DateTime time)
        {
            if (BPM == MIN_BPM)
                return;

            BPM--;
            interval = 60000f / BPM;
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