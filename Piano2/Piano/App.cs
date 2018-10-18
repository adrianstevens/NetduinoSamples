using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Audio;
using Netduino.Foundation.Sensors.Buttons;

namespace Piano
{
    public class App
    {
        
        float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f, 196f, 246.94f, 293.67f, 370 };

        int[] buttonSpeakerIndex = new int[8];
        PushButton[] buttons = new PushButton[8];
        PushButton buttonReset;
        
        PiezoSpeaker[] speakers = new PiezoSpeaker[3];
        bool[] isSpeakerPlaying = new bool[3];

        Led onboardLed;

        public void Run ()
        {
            DrawText("Welcome to Piano");

            InitializePeripherals();
        }

        private void InitializePeripherals()
        {
            buttons[0] = new PushButton(N.Pins.GPIO_PIN_D0, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[1] = new PushButton(N.Pins.GPIO_PIN_D1, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[2] = new PushButton(N.Pins.GPIO_PIN_D2, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[3] = new PushButton(N.Pins.GPIO_PIN_D3, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[4] = new PushButton(N.Pins.GPIO_PIN_D4, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[5] = new PushButton(N.Pins.GPIO_PIN_D5, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[6] = new PushButton(N.Pins.GPIO_PIN_D6, Netduino.Foundation.CircuitTerminationType.CommonGround);
            buttons[7] = new PushButton(N.Pins.GPIO_PIN_D7, Netduino.Foundation.CircuitTerminationType.CommonGround);

            buttonReset = new PushButton(N.Pins.ONBOARD_BTN, Netduino.Foundation.CircuitTerminationType.CommonGround);

            speakers[0] = new PiezoSpeaker(Cpu.PWMChannel.PWM_2);
            speakers[1] = new PiezoSpeaker(Cpu.PWMChannel.PWM_3);
            speakers[2] = new PiezoSpeaker(Cpu.PWMChannel.PWM_5);

            onboardLed = new Led(N.Pins.ONBOARD_LED);

            isSpeakerPlaying[0] = isSpeakerPlaying[1] = isSpeakerPlaying[2] = false;

            for (int i = 0; i < buttonSpeakerIndex.Length; i++)
                buttonSpeakerIndex[i] = -1;


            foreach (var button in buttons)
            {
                button.PressStarted += Button_PressStarted;
                button.PressEnded += Button_PressEnded;
            }

            buttonReset.Clicked += ButtonReset_Clicked;
        }

        private void ButtonReset_Clicked(object sender, EventArgs e)
        {
            for (int i = 0; i < speakers.Length; i++)
            {
                speakers[i].StopTone();
                isSpeakerPlaying[i] = false;
            }

            for (int j = 0; j < buttons.Length; j++)
            {
                buttonSpeakerIndex[j] = -1;
            }
        }

        private void Button_PressStarted(object sender, EventArgs e)
        {
            var button = (PushButton)sender;

            var bIndex = GetButtonIndex(button);

            DrawText("Button" + bIndex + "down");

            for (int sIndex = 0; sIndex < speakers.Length; sIndex++)
            {
                if (isSpeakerPlaying[sIndex] == false)
                {
                    speakers[sIndex].PlayTone(notes[bIndex]);
                    buttonSpeakerIndex[bIndex] = sIndex;
                    isSpeakerPlaying[sIndex] = true;
                    break;
                }   
            }

            onboardLed.IsOn = true;
        }

        private void Button_PressEnded(object sender, EventArgs e)
        {
            var button = (PushButton)sender;

            var bIndex = GetButtonIndex(button);

            DrawText("Button" + bIndex + "up");

            var sIndex = buttonSpeakerIndex[bIndex];
            speakers[sIndex].StopTone();
            isSpeakerPlaying[sIndex] = false;
  
            onboardLed.IsOn = false;
        }

        private int GetButtonIndex(PushButton button)
        {
            int index;

            for (index = 0; index < buttons.Length; index++)
            {
                if (button == buttons[index])
                    break;
            }
            return index;
        }

        private void DrawText (string message)
        {
            Debug.Print(message);
        }
    }
}