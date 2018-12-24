using Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Audio;
using System.Threading;
using Netduino.Foundation.Sensors.Buttons;

namespace XMasPlayer
{
    public class App
    {
        Led ledMelody;
        Led ledBass;

        PiezoSpeaker speakerMelody;
        PiezoSpeaker speakerBass;

        PushButton buttonPlay;

        SongBook songBook = new SongBook();

        public void Run ()
        {
            songBook.LoadSongs();

            InitializePeripherals();
        }

        private void PlaySong(Song song)
        {
            //smallest note length is a 32nd note 
            //change value to adjust tempo 
            var len32Note = 1500 / 32;

            //index of the currently playing note
            int melodyIndex = 0;
            int bassIndex = 0;

            //remaining steps for the currently playing note
            int melodyRemaining = 0;
            int bassRemaining = 0;
           
            //loop until we've played every melody and bass note
            while (melodyIndex < song.Melody.Length && 
                   bassIndex < song.Bass.Length)
            {
                if (melodyRemaining == 0 && melodyIndex < song.Melody.Length)
                {
                    speakerMelody.StopTone();
                    ledMelody.IsOn = false;

                    //get the length of the next note
                    melodyRemaining = song.Melody[melodyIndex].Length;

                    //if the note isn't silence (i.e. don't play rests)
                    if (song.Melody[melodyIndex].Pitch != 0)
                    {
                        speakerMelody.PlayTone(song.Melody[melodyIndex].Pitch);
                        ledMelody.IsOn = true;
                    }
                    melodyIndex++;
                }
                melodyRemaining--;

                if (bassRemaining == 0 && bassIndex < song.Bass.Length)
                {
                    speakerBass.StopTone();
                    ledBass.IsOn = false;

                    bassRemaining = song.Bass[bassIndex].Length;

                    if (song.Bass[bassIndex].Pitch != 0)
                    {
                        speakerBass.PlayTone(song.Bass[bassIndex].Pitch);
                        ledBass.IsOn = true;
                    }
                    bassIndex++;    
                }
                bassRemaining--;

                Thread.Sleep(len32Note);
            }

            Thread.Sleep(len32Note * 32);

            ledMelody.IsOn = false;
            ledBass.IsOn = false;

            speakerMelody.StopTone();
            speakerBass.StopTone();
        }

        private void InitializePeripherals()
        {
            ledMelody = new Led(N.Pins.GPIO_PIN_D4);
            ledBass = new Led(N.Pins.GPIO_PIN_D2);

            speakerMelody = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D9);
            speakerBass = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D11);

            buttonPlay = new PushButton(N.Pins.ONBOARD_BTN, Netduino.Foundation.CircuitTerminationType.Floating);
            buttonPlay.Clicked += OnButtonPlay;
        }

        private void OnButtonPlay(object sender, Microsoft.SPOT.EventArgs e)
        {
            PlaySong(songBook.Songs[0]);
        }
    }
}