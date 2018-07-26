using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

//This next class defines how we interface with the Piezo speaker.

//    using SecretLabs.NETMF.Hardware;
//    using System.Threading;

public class PiezoSpeaker
{
    private SecretLabs.NETMF.Hardware.PWM _pin;

    // if the _busy flag is true we will just
    // ignore any request to make noise.
    private bool _busy = false;

    public PiezoSpeaker(Cpu.Pin pin)
    {
        _pin = new SecretLabs.NETMF.Hardware.PWM(pin);

        // take the pin low, so the speaker
        // doesn't make any noise until we
        // ask it to
        _pin.SetDutyCycle(0);
    }

    /// <summary>
    /// Play a particular frequency for a defined
    /// time period
    /// </summary>
    /// <param name="frequency">The frequency (in hertz) of the note to be played</param>
    /// <param name="duration">How long (in milliseconds: 1000 = 1 second) the note is to play for</param>
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