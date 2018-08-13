using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation.LEDs;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Piezos;

namespace Simone
{
    public class Program
    {
        static int ANIMATION_DELAY = 200;

        static Led[] leds = new Led[4];
        static float[] notes = new float[] {261.63f, 329.63f, 392, 523.25f };
        static InterruptPort[] buttons = new InterruptPort[4];
        static PiezoSpeaker speaker;

        static bool isAnimating = false;

        static SimonGame game = new SimonGame();

        public static void Main()
        {
            Debug.Print("Welcome to Simon");

            Initialize();

            SetAllLEDs(true);

            game.OnGameStateChanged += OnGameStateChanged;

            game.Reset();

            while (true)
            {

            }
        }

        private static void OnGameStateChanged(object sender, SimonEventArgs e)
        {
            var th = new Thread(() =>
            {
                switch (e.GameState)
                {
                    case GameState.Start:
                        break;
                    case GameState.NextLevel:
                        ShowStartAnimation();
                        ShowNextLevelAnimation(game.Level);
                        ShowSequenceAnimation(game.Level);
                        break;
                    case GameState.GameOver:
                        ShowGameOverAnimation();
                        game.Reset();
                        break;
                    case GameState.Win:
                        ShowGameWonAnimation();
                        break;
                }
            });
            th.Start();
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

        private static void ShowNextLevelAnimation (int level)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            SetAllLEDs(false);

            for (int i = 0; i < level; i++)
            {
                Thread.Sleep(ANIMATION_DELAY);
                SetAllLEDs(true);
                Thread.Sleep(ANIMATION_DELAY * 3);
                SetAllLEDs(false);
            }

            isAnimating = false;
        }

        private static void ShowSequenceAnimation (int level)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            var steps = game.GetStepsForLevel();

            SetAllLEDs(false);

            for (int i = 0; i < level; i++)
            {
                Thread.Sleep(200);
                TurnOnLED(steps[i], 400);
            }

            isAnimating = false;
        }

        private static void ShowGameOverAnimation ()
        {
            if (isAnimating)
                return;

            isAnimating = true;

            speaker.PlayTone(123.47f, 750);

            for (int i = 0; i < 20; i++)
            {
                SetAllLEDs(false);
                Thread.Sleep(50);
                SetAllLEDs(true);
                Thread.Sleep(50);
            }

            isAnimating = false;
        }

        private static void ShowGameWonAnimation ()
        {
            ShowStartAnimation();
            ShowStartAnimation();
            ShowStartAnimation();
            ShowStartAnimation();
        }

        private static void TurnOnLED(int index, int durration = 400)
        {
            leds[index].IsOn = true;

            speaker.PlayTone(notes[index], durration);
            leds[index].IsOn = false;
        }


        static DateTime lastPressed;
        private static void OnButton(int buttonIndex)
        {
            if (DateTime.Now - lastPressed < TimeSpan.FromTicks(5000000)) //0.5s
                return;

            Debug.Print("Button tapped: " + buttonIndex);

            if (isAnimating == false)
            {
                lastPressed = DateTime.Now;

                TurnOnLED(buttonIndex);

                game.EnterStep(buttonIndex);
            }
        }

        private static void OnButton0(uint data1, uint data2, DateTime time)
        {
            OnButton(0);
        }
        private static void OnButton1(uint data1, uint data2, DateTime time)
        {
            OnButton(1);
        }
        private static void OnButton2(uint data1, uint data2, DateTime time)
        {
            OnButton(2);
        }
        private static void OnButton3(uint data1, uint data2, DateTime time)
        {
            OnButton(3);
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