using System.Threading;
using Microsoft.SPOT;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Audio;
using Netduino.Foundation.Displays;
using System.Collections;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation;

namespace Breakout
{
    public class App
    {
        float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f };
        PiezoSpeaker speaker;
        GraphicsLibrary graphics;

        ILI9163 display;

        BreakoutGame game;
        PushButton buttonLeft;
        PushButton buttonRight;

        public void Run()
        {
            InitializePeripherals();

            game = new BreakoutGame(128, 160);

            DrawText("Breakout!");

            while (true)
               GameLoop();
        }

        void GameLoop ()
        {
            game.Update();

            graphics.Clear(false);

            graphics.DrawCircle(game.Ball.X, game.Ball.Y, game.Ball.Radius,
                Color.White);

            graphics.DrawRectangle(game.Paddle.X, game.Paddle.Y, game.Paddle.Width, game.Paddle.Height,
                Color.LightGray);

            for (int i = 0; i < game.Blocks.Length; i++)
            {
                if (game.Blocks[i].IsVisible)
                {
                    graphics.DrawRectangle(game.Blocks[i].X, game.Blocks[i].Y, game.Blocks[i].Width, game.Blocks[i].Height,
                        GetColorForRow(i/5));
                }
            } 
         
            graphics.Show();
        }

        Color GetColorForRow(int row)
        {
            if (row == 0)
                return Color.Orange;
            if (row == 1)
                return Color.Red;
            if (row == 2)
                return Color.Purple;
            return Color.Blue;
        }

        private void InitializePeripherals()
        {
            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D3);

            buttonLeft = new PushButton(N.Pins.GPIO_PIN_D2, CircuitTerminationType.CommonGround);
            buttonRight = new PushButton(N.Pins.GPIO_PIN_D5, CircuitTerminationType.CommonGround);

            buttonLeft.Clicked += ButtonLeft_Clicked;
            buttonRight.Clicked += ButtonRight_Clicked;

            display = new ILI9163(chipSelectPin: N.Pins.GPIO_PIN_D4,
                dcPin: N.Pins.GPIO_PIN_D7,
                resetPin: N.Pins.GPIO_PIN_D6,
                spiModule: SPI.SPI_module.SPI1,
                speedKHz: 15000);

            graphics = new GraphicsLibrary(display);
        }

        private void ButtonLeft_Clicked(object sender, EventArgs e)
        {
            game.Left();
        }

        private void ButtonRight_Clicked(object sender, EventArgs e)
        {
            game.Right();
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