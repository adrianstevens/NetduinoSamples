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
        static InterruptPort[] buttons = new InterruptPort[4];

        static bool isAnimating = false;

        static SimonGame game = new SimonGame();

        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            Initialize();

            game.OnGameStateChanged += OnGameStateChanged;

            game.Reset();

            while (true)
            {

            }
        }

        private static void OnGameStateChanged(object sender, SimonEventArgs e)
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
                    break;
                case GameState.Win:
                    ShowGameWonAnimation();
                    break;
            }
        }

        static void Initialize()
         { 
            leds[0] = new Led(N.Pins.GPIO_PIN_D0);
            leds[1] = new Led(N.Pins.GPIO_PIN_D1);
            leds[2] = new Led(N.Pins.GPIO_PIN_D2);
            leds[3] = new Led(N.Pins.GPIO_PIN_D3);

            buttons[0] = new InterruptPort(N.Pins.GPIO_PIN_D10, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[1] = new InterruptPort(N.Pins.GPIO_PIN_D11, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[2] = new InterruptPort(N.Pins.GPIO_PIN_D12, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[3] = new InterruptPort(N.Pins.GPIO_PIN_D13, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);

            buttons[0].OnInterrupt += OnButton0;
            buttons[1].OnInterrupt += OnButton1;
            buttons[2].OnInterrupt += OnButton2;
            buttons[3].OnInterrupt += OnButton3;

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
                Thread.Sleep(100);
                leds[steps[i]].IsOn = true;
                Thread.Sleep(400);
                SetAllLEDs(false);
            }

            isAnimating = false;
        }

        private static void ShowGameOverAnimation ()
        {
            if (isAnimating)
                return;

            isAnimating = true;

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

        private static void TurnOnLED(int index)
        {
            SetAllLEDs(false);
            leds[index].IsOn = true;
            Thread.Sleep(100);
            leds[index].IsOn = false;
        }


        static DateTime lastPressed;
        private static void OnButton(int buttonIndex)
        {
            foreach (var btn in buttons)
                btn.ClearInterrupt();

            if (DateTime.Now - lastPressed < TimeSpan.FromTicks(5000000)) //0.5s
                return;

            Debug.Print("Button tapped: " + buttonIndex);

            if (isAnimating == false)
            {
                lastPressed = DateTime.Now;
                game.EnterStep(buttonIndex);
                TurnOnLED(buttonIndex);
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