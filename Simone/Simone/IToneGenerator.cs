using System;
using Microsoft.SPOT;

interface IToneGenerator
{
    void PlayTone(float frequency, int duration);
}
