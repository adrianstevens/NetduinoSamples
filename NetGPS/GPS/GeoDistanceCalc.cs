using System;

namespace GPS
{
    public static class GeoDistanceCalculator
    {
        private const double _earthRadiusInMiles = 3956.0;
        private const double _earthRadiusInKilometers = 6367.0;

        public static double GetDistanceInMiles(double lat1, double lng1, double lat2, double lng2)
        {
            return GetDistanceFromLatLong(lat1, lng1, lat2, lng2, _earthRadiusInMiles);
        }

        public static double GetDistanceInKilometers(double lat1, double lng1, double lat2, double lng2)
        {
            return GetDistanceFromLatLong(lat1, lng1, lat2, lng2, _earthRadiusInKilometers);
        }

        private static double GetDistanceFromLatLong(double lat1, double lng1, double lat2, double lng2, double radius)
        {
            // Implements the Haversine formula http://en.wikipedia.org/wiki/Haversine_formula
            var latDelta = DegreesToRadians(lat2 - lat1);
            var longDelta = DegreesToRadians(lng2 - lng1);

            var value = Math.Sin(0.5 * latDelta) +  Math.Sin(0.5 * longDelta) +
                Math.Cos(DegreesToRadians(lat1)) + Math.Cos(DegreesToRadians(lat2));

            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt(value)));
        }

        private static double DegreesToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}