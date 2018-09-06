using System;
using System.Threading;
using Microsoft.SPOT;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Audio;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Displays;
using Netduino.Foundation.Sensors.Switches;
using System.Collections;

namespace Simone
{
    public class App
    {
        int ANIMATION_DELAY = 200;

        Led[] leds = new Led[4];
        float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f };
        PushButton[] buttons = new PushButton[4];
        PiezoSpeaker speaker;
        SpdtSwitch spdt;
        GraphicsLibrary display;

        bool isAnimating = false;

        SimonGame game = new SimonGame();

        ArrayList messageQue = new ArrayList();

        public void Run()
        {
            InitializePeripherals();

            DisplayMessage("Simone!");

            SetAllLEDs(true);

            game.OnGameStateChanged += OnGameStateChanged;

            game.Reset();
        }

        private void InitializePeripherals()
        {
            leds[0] = new Led(N.Pins.GPIO_PIN_D4);
            leds[1] = new Led(N.Pins.GPIO_PIN_D5);
            leds[2] = new Led(N.Pins.GPIO_PIN_D6);
            leds[3] = new Led(N.Pins.GPIO_PIN_D7);

            buttons[0] = new PushButton(N.Pins.GPIO_PIN_D10, Netduino.Foundation.CircuitTerminationType.High);
            buttons[1] = new PushButton(N.Pins.GPIO_PIN_D11, Netduino.Foundation.CircuitTerminationType.High);
            buttons[2] = new PushButton(N.Pins.GPIO_PIN_D12, Netduino.Foundation.CircuitTerminationType.High);
            buttons[3] = new PushButton(N.Pins.GPIO_PIN_D13, Netduino.Foundation.CircuitTerminationType.High);

            buttons[0].PressStarted += OnButton0;
            buttons[1].PressStarted += OnButton1;
            buttons[2].PressStarted += OnButton2;
            buttons[3].PressStarted += OnButton3;

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D3);
            spdt = new SpdtSwitch(N.Pins.GPIO_PIN_D2);
            display = new GraphicsLibrary(new SSD1306(0x3c, 400, SSD1306.DisplayType.OLED128x64));

            SetAllLEDs(false);
        }

        private void OnGameStateChanged(object sender, SimonEventArgs e)
        {
            var th = new Thread(() =>
            {
                switch (e.GameState)
                {
                    case GameState.Start:
                        DisplayMessage("Are you ready?");
                        break;
                    case GameState.NextLevel:
                        if(game.Level > 2)
                            DisplayMessage("Nice work!");
                        ShowStartAnimation();
                        ShowNextLevelAnimation(game.Level);
                        DisplayMessage("Level " + (game.Level - 1));
                        ShowSequenceAnimation(game.Level);
                        break;
                    case GameState.GameOver:
                        DisplayMessage("Game over :(");
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
        
        private void SetAllLEDs(bool isOn)
        {
            leds[0].IsOn = isOn;
            leds[1].IsOn = isOn;
            leds[2].IsOn = isOn;
            leds[3].IsOn = isOn;
        }

        private void ShowStartAnimation()
        {
            if (isAnimating)
                return;

            isAnimating = true;

            SetAllLEDs(false);

            for (int i = 0; i < 4; i++)
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

        private void ShowNextLevelAnimation(int level)
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

        private void ShowSequenceAnimation(int level)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            var steps = game.GetStepsForLevel();

            SetAllLEDs(false);

            for (int i = 0; i < level; i++)
            {
                Thread.Sleep(200);
                ShowGameStep(steps[i], 400);
            }

            isAnimating = false;
        }

        private void ShowGameOverAnimation()
        {
            if (isAnimating)
                return;

            isAnimating = true;

            if(spdt.IsOn)
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

        private void ShowGameWonAnimation()
        {
            ShowStartAnimation();
            ShowStartAnimation();
            ShowStartAnimation();
            ShowStartAnimation();
        }

        private void ShowGameStep(int index, int durration = 400)
        {
            leds[index].IsOn = true;

            if (spdt.IsOn)
                speaker.PlayTone(notes[index], durration);
            else
                Thread.Sleep(durration);
            leds[index].IsOn = false;
        }
        
         DateTime lastPressed;
        private void OnButton(int buttonIndex)
        {
            if (DateTime.Now - lastPressed < TimeSpan.FromTicks(5000000)) //0.5s
                return;

            Debug.Print("Button tapped: " + buttonIndex);

            if (isAnimating == false)
            {
                lastPressed = DateTime.Now;

                ShowGameStep(buttonIndex);

                game.EnterStep(buttonIndex);
            }
        }

        private void OnButton0(object sender, EventArgs e)
        {
            OnButton(0);
        }
        private void OnButton1(object sender, EventArgs e)
        {
            OnButton(1);
        }
        private void OnButton2(object sender, EventArgs e)
        {
            OnButton(2);
        }
        private void OnButton3(object sender, EventArgs e)
        {
            OnButton(3);
        }

        private void DisplayMessage(string message)
        {
            messageQue.Add(message);

            if (messageQue.Count != 1)
                return;

            while (messageQue.Count > 0)
            {
                DrawText((string)messageQue[messageQue.Count - 1]);
                messageQue.RemoveAt(0);
                Thread.Sleep(1000);
            }
        }

        private void DrawText(string message)
        {
            if (display != null)
            {
                display.Clear(true);
                display.CurrentFont = new Font8x8();
                display.DrawText(0, 0, 0, message);
                display.Show();
            }

            Debug.Print(message);
        }
    }
}