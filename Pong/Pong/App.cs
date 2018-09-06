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

namespace MoonLander
{
    public class App
    {
        int ANIMATION_DELAY = 50; //20 fps??

        Led[] leds = new Led[4];
        float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f };
        PushButton[] buttons = new PushButton[4];
        PiezoSpeaker speaker;
        SpdtSwitch spdt;

        GraphicsLibrary graphics;
        SSD1306 display;

        bool isAnimating = false;

        ArrayList messageQue = new ArrayList();

        public void Run()
        {
            InitializePeripherals();

            DrawText("NetPong!");

            SetAllLEDs(true);

            StartGameLoop();
        }

        int ballX = 64;
        int ballY = 30;
        int ballRadius = 3;
        int ballXVelocity = 5;
        int ballYVelocity = 2;

        int playerX = 4;
        int playerY = 32;
        int paddleWidth = 3;
        int paddleHeight = 14;

        int cpuX = 128 - 4 - 3;
        int cpuY = 32;

        int playerScore = 0;
        int cpuScore = 0;

        void StartGameLoop ()
        {
            while(spdt.IsOn)
            {
                graphics.Clear(false);
                graphics.DrawFilledCircle(ballX, ballY, ballRadius);
                graphics.DrawFilledRectangle(playerX, playerY, paddleWidth, paddleHeight);
                graphics.DrawFilledRectangle(cpuX, cpuY, paddleWidth, paddleHeight);

                graphics.DrawText(0, 0, 0, playerScore.ToString());
                graphics.DrawText(120, 0, 0, cpuScore.ToString());

                graphics.Show();

                //move player
                if(buttons[0].State == false)
                {
                    if (playerY > 0)
                        playerY-= 4;
                }
                else if(buttons[1].State == false)
                {
                    if (playerY - paddleHeight < 64)
                        playerY += 4;
                }

                //move cpu
                if (cpuY < ballY)
                    cpuY++;
                else if (cpuY > ballY)
                    cpuY--;

                ballX += ballXVelocity;
                if(ballY >= playerY &&
                    ballY <= playerY + paddleHeight &&
                    ballX - ballRadius <= playerX)
                {
                    ballXVelocity *= -1;
                    ballX += ballXVelocity;
                    speaker.PlayTone(300, 25);
                }
                else if (ballX - ballRadius < 0)
                {
                    cpuScore++;
                    ballXVelocity *= -1;
                    ballX += 64;
                    ballY = 32;
                    speaker.PlayTone(440, 25);
                    ShowGameOverAnimation();
                }
                else if (ballY >= cpuY &&
                    ballY <= cpuY + paddleHeight &&
                    ballX + ballRadius > cpuX)
                {
                    ballXVelocity *= -1;
                    ballX += ballXVelocity;
                    speaker.PlayTone(300, 25);
                }
                else if (ballX + ballRadius > 128)
                {
                    playerScore++;
                    ballXVelocity *= -1;
                    ballX = 64;
                    ballY = 32;
                    speaker.PlayTone(440, 25);
                    ShowGameOverAnimation();
                }
                
                ballY += ballYVelocity;
                if (ballY - ballRadius < 0)
                {
                    ballYVelocity *= -1;
                    ballY += ballYVelocity;
                    speaker.PlayTone(220, 25);
                }
                else if (ballY + ballRadius > 64)
                {
                    ballYVelocity *= -1;
                    ballY += ballYVelocity;
                    speaker.PlayTone(220, 25);
                }
                
                Thread.Sleep(25);
            }
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

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D3);
            spdt = new SpdtSwitch(N.Pins.GPIO_PIN_D2);
            graphics = new GraphicsLibrary(display = new SSD1306(0x3c, 400, SSD1306.DisplayType.OLED128x64));
            display.IgnoreOutOfBoundsPixels = true;

            SetAllLEDs(false);
        }

        private void SetAllLEDs(bool isOn)
        {
            leds[0].IsOn = isOn;
            leds[1].IsOn = isOn;
            leds[2].IsOn = isOn;
            leds[3].IsOn = isOn;
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

        private void DrawText(string message)
        {
            if (graphics != null)
            {
                graphics.Clear(true);
                graphics.CurrentFont = new Font8x8();
                graphics.DrawText(0, 0, 0, message);
                graphics.Show();
            }
            Debug.Print(message);
        }
    }
}