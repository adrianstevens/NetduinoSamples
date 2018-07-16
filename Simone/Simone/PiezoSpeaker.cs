using System.Threading;
using SecretLabs.NETMF.Hardware;

public class PiezoSpeaker
{
    private PWM _pin;

    private bool _busy = false;

    public PiezoSpeaker(Microsoft.SPOT.Hardware.Cpu.Pin pin)
    {
        _pin = new PWM(pin);

        _pin.SetDutyCycle(0);
    }

    public void Play(float frequency, int duration)
    {
        if (!_busy)
        {
            _busy = true;

            // calculate the actual period and turn the
            // speaker on for the defined period of time
            uint period = (uint)(1000000 / frequency);
            _pin.SetPulse(period, period / 2);

            Thread.Sleep(duration);

            // turn the speaker off
            _pin.SetDutyCycle(0);
            _busy = false;
        }
    }
}