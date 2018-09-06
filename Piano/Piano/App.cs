using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Audio;
using Netduino.Foundation.Sensors.Switches;
using Netduino.Foundation.Displays;

namespace Piano
{
    public class App
    {
        Led[] leds = new Led[4];
        float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f, 196f, 246.94f, 293.67f, 370 };

        InterruptPort[] buttons = new InterruptPort[4];
        PiezoSpeaker speaker;
        SpdtSwitch spdt;
        GraphicsLibrary display;

        public void Run ()
        {
            DrawText("Welcome to Piano");

            InitializePeripherals();
        }

        private void InitializePeripherals()
        {
            leds[0] = new Led(N.Pins.GPIO_PIN_D4);
            leds[1] = new Led(N.Pins.GPIO_PIN_D5);
            leds[2] = new Led(N.Pins.GPIO_PIN_D6);
            leds[3] = new Led(N.Pins.GPIO_PIN_D7);

            buttons[0] = new InterruptPort(N.Pins.GPIO_PIN_D10, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[1] = new InterruptPort(N.Pins.GPIO_PIN_D11, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[2] = new InterruptPort(N.Pins.GPIO_PIN_D12, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[3] = new InterruptPort(N.Pins.GPIO_PIN_D13, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);

            buttons[0].OnInterrupt += OnButton0;
            buttons[1].OnInterrupt += OnButton1;
            buttons[2].OnInterrupt += OnButton2;
            buttons[3].OnInterrupt += OnButton3;

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D3);
            spdt = new SpdtSwitch(N.Pins.GPIO_PIN_D2);
            display = new GraphicsLibrary(new SSD1306(0x3c, 400, SSD1306.DisplayType.OLED128x64));

            SetAllLEDs(false);
        }

        private void SetAllLEDs(bool isOn)
        {
            leds[0].IsOn = isOn;
            leds[1].IsOn = isOn;
            leds[2].IsOn = isOn;
            leds[3].IsOn = isOn;
        }

        private void ToggleNote(int index, bool play, bool shift)
        {
            if (play)
            {
                var frequency = notes[index + (shift ? 4 : 0)];
                speaker.PlayTone(frequency);
                leds[index].IsOn = true;
                DrawText(frequency + " hz");
            }
            else
            {
                speaker.StopTone();
                leds[index].IsOn = false;
                display.Clear(true);
            }
        }

        private void OnButton0(uint data1, uint data2, DateTime time)
        {
            ToggleNote(0, data2 == 1, spdt.IsOn);
        }

        private void OnButton1(uint data1, uint data2, DateTime time)
        {
            ToggleNote(1, data2 == 1, spdt.IsOn);
        }

        private void OnButton2(uint data1, uint data2, DateTime time)
        {
            ToggleNote(2, data2 == 1, spdt.IsOn);
        }

        private void OnButton3(uint data1, uint data2, DateTime time)
        {
            ToggleNote(3, data2 == 1, spdt.IsOn);
        }

        private void DrawText (string message)
        {
            if(display != null)
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
