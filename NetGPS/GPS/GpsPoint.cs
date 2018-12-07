using System;

namespace GPS
{
    public class GpsPoint
    {
        public DateTime Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double SpeedInKnots { get; set; }
        public double BearingInDegrees { get; set; }
    }
}