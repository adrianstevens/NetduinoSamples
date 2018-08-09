using System.Threading;
using Microsoft.SPOT.Hardware;

public class PiezoSpeaker : IToneGenerator
{
    private PWM _pwm;
    private bool _isPlaying = false;

    public PiezoSpeaker(Cpu.PWMChannel pwmChannel)
    {
        _pwm = new PWM(pwmChannel, 100, 0, false);
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

            _pwm.Period = period;
            _pwm.Duration = period / 2;
            _pwm.Start();

            Thread.Sleep(duration);

            _pwm.Stop();

            _isPlaying = false;
        }
    }
}