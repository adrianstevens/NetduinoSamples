using Microsoft.SPOT;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Audio;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Displays;
using Netduino.Foundation.LEDs;
using System.Threading;
using Microsoft.SPOT.Hardware;
using System;

namespace BuzzerGame
{
    public class App
    {
        float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f };

        PushButton resetButton;
        InputPort tree;

        PwmLed redLed, greenLed;

        PiezoSpeaker speaker;
        GraphicsLibrary display;

        DateTime gameStart;
        TimeSpan timePlayed;


        //game variables
        int count;
        bool isPlaying = true;

        public void Run()
        {
            InitializePeripherals();

            UpdateDisplay();

            greenLed.StartPulse(1000, 0.8f, 0.05f);

            //thread to detect touches
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (isPlaying && tree.Read() == false)
                    {
                        OnTouched();
                    }
                    Thread.Sleep(10);
                }
            });
            thread.Start();

            ResetGame();
        }

        private void OnTouched ()
        {
            count++;
            PlayBuzzer();

            new Thread(() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    redLed.SetBrightness(0.8f);
                    Thread.Sleep(25);
                    redLed.SetBrightness(0);
                    Thread.Sleep(25);
                }
            }).Start();

            UpdateDisplay();
        }

        private void ResetGame ()
        {
            count = 0;
            gameStart = DateTime.Now;
        }

        private void InitializePeripherals()
        {
            resetButton = new PushButton(N.Pins.ONBOARD_BTN, Netduino.Foundation.CircuitTerminationType.Floating);
            resetButton.Clicked += OnResetButton;

            tree = new InputPort(N.Pins.GPIO_PIN_D10, true, Port.ResistorMode.PullUp);

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D3);
            greenLed = new PwmLed(N.PWMChannels.PWM_PIN_D5, 2.0f);
            redLed = new PwmLed(N.PWMChannels.PWM_PIN_D6, 2.0f);

            var ssd1306 = new SSD1306(0x3c, 400, SSD1306.DisplayType.OLED128x64);
            display = new GraphicsLibrary(ssd1306);
            display.CurrentFont = new Font8x12();
            
            display.Clear(true);

            PlayBuzzer();
        }

        private void OnResetButton(object sender, EventArgs e)
        {
            if (isPlaying == false)
                ResetGame();
            else
                timePlayed = (DateTime.Now - gameStart);

            isPlaying = !isPlaying;

            UpdateDisplay();
        }

        private void PlayBuzzer ()
        {
            speaker.PlayTone(notes[0], 50);
        }

        private void UpdateDisplay ()
        {
            display.Clear();

            if (isPlaying)
            {
                display.DrawText(0, 0, "XBuzzer - Play!");
                display.DrawText(0, 20, "Count: " + count);
            }
            else
            {
                display.DrawText(0, 20, "Count: " + count);

                string time = "Time:" + timePlayed.Minutes + ":";
                if (timePlayed.Seconds < 10)
                    time += "0";
                time += timePlayed.Seconds;

                display.DrawText(0, 0, time);
            }
            display.Show();
        }
    }
}