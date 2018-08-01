using System.Threading;
using Microsoft.SPOT.Hardware;

public class PiezoSpeaker : IToneGenerator
{
    private SecretLabs.NETMF.Hardware.PWM _pin;

    private bool _isPlaying = false;

    public PiezoSpeaker(Cpu.Pin pin)
    {
        _pin = new SecretLabs.NETMF.Hardware.PWM(pin);

        // silence the piezo
        _pin.SetDutyCycle(0);
    }

    /// <summary>
    /// Play a frequency for a specified duration
    /// </summary>
    /// <param name="frequency">The frequency in hertz of the tone to be played</param>
    /// <param name="duration">How long the note is played in milliseconds</param>
    public void PlayTone(float frequency, int duration)
    {
        if (!_isPlaying)
        {
            _isPlaying = true;

            var period = (uint)(1000000 / frequency);
            _pin.SetPulse(period, period / 2);

            Thread.Sleep(duration);

            _pin.SetDutyCycle(0);
            _isPlaying = false;
        }
    }
}