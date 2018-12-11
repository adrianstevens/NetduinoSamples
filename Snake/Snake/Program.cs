using System;
using Microsoft.SPOT;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Audio;
using Netduino.Foundation.Displays;
using static Microsoft.SPOT.Hardware.Cpu;
using Netduino.Foundation;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace Snake
{
    public class Program
    {
        //Hardware
        static PushButton[] buttons = new PushButton[4];
        static PiezoSpeaker speaker;
        static GraphicsLibrary graphics;

        public static SnakeGame game;

        public static void Main()
        {
            Debug.Print(Resources.GetString(Resources.StringResources.String1));

            Initialize();

            graphics.DrawText(0,0, "Snake!!");
            graphics.Show();

            Thread.Sleep(1000);

            game = new SnakeGame(84, 48);

            StartGameLoop();
        }

        static void StartGameLoop ()
        {
            graphics.DrawRectangle(0, 0, 84, 48, false, true);

            while (true)
            {
                game.Update();
                //draw food
                graphics.DrawPixel(game.FoodPosition.X, game.FoodPosition.Y);
                
                //draw food
                for (int i = 0; i < game.SnakePosition.Count; i++)
                {
                    var point = (Point)game.SnakePosition[i];

                    graphics.DrawPixel(point.X, point.Y);
                }

                //show
                graphics.Show();

                Thread.Sleep(250 - game.Level * 5);
            }
        }

        //hardware init
        private static void Initialize ()
        {
            buttons[(int)SnakeDirection.Left] = new PushButton(Pins.GPIO_PIN_D6, CircuitTerminationType.CommonGround);
            buttons[(int)SnakeDirection.Right] = new PushButton(Pins.GPIO_PIN_D4, CircuitTerminationType.CommonGround);
            buttons[(int)SnakeDirection.Up] = new PushButton(Pins.GPIO_PIN_D5, CircuitTerminationType.CommonGround);
            buttons[(int)SnakeDirection.Down] = new PushButton(Pins.GPIO_PIN_D7, CircuitTerminationType.CommonGround);

            buttons[(int)SnakeDirection.Left].Clicked += LeftClicked;
            buttons[(int)SnakeDirection.Right].Clicked += RightClicked;
            buttons[(int)SnakeDirection.Up].Clicked += (s, e) => game.Direction = SnakeDirection.Up;
            buttons[(int)SnakeDirection.Down].Clicked += (s, e) => game.Direction = SnakeDirection.Down;

            speaker = new PiezoSpeaker(PWMChannel.PWM_4);

            var display = new PCD8544(chipSelectPin: Pins.GPIO_PIN_D9, dcPin: Pins.GPIO_PIN_D8,
                                      resetPin: Pins.GPIO_PIN_D10, spiModule: SPI.SPI_module.SPI1);

            graphics = new GraphicsLibrary(display);
            graphics.CurrentFont = new Font8x8();
        }

        private static void RightClicked(object sender, EventArgs e)
        {
            game.Direction = SnakeDirection.Right;
        }

        private static void LeftClicked(object sender, EventArgs e)
        {
            game.Direction = SnakeDirection.Left;
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