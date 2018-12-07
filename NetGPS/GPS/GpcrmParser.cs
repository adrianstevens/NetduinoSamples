using Microsoft.SPOT;
using System;

namespace GPS
{
    public class GprmcParser
    {
        public static GpsPoint Parse(string line)
        {
            // test data - $GPRMC,040302.663,A,3939.7,N,10506.6,W,0.27,358.86,200804,,*1A
            if (!IsCheckSumGood(line)) return null;
            
            try
            {
                var parts = line.Split(',');

                if (parts.Length != 13)  // This GPS has extra field
                    return null;
                
                //Debug.Print(parts[2]);
                if (parts[2] != "A")
                    return null;
                
                //Debug.Print(parts[9]);
                string date = parts[9]; // UTC Date DDMMYY
                if (date.Length != 6)
                    return null;

                var years = 2000 + int.Parse(date.Substring(4, 2));
                var months = int.Parse(date.Substring(2, 2));
                var days = int.Parse(date.Substring(0, 2));
                var time = parts[1]; // HHMMSS.XXX

                if (time.Length != 9)
                    return null;

                var hours = int.Parse(time.Substring(0, 2));
                var minutes = int.Parse(time.Substring(2, 2));
                var seconds = int.Parse(time.Substring(4, 2));
                var mseconds = int.Parse(time.Substring(7, 2));

                var utcTime = new DateTime(years, months, days, hours, minutes, seconds, mseconds);

                string lat = parts[3];  // HHMM.MMMM

                if (lat.Length != 10)
                    return null;

                double latHours = double.Parse(lat.Substring(0, 2));
                double latMins = double.Parse(lat.Substring(2));
                double latitude = latHours + latMins / 60.0;

                if (parts[4] == "S")       // N or S
                    latitude *= -1;

                string lng = parts[5];  // HHHMM.MMMMM

                if (lng.Length != 11)
                    return null;

                double lngHours = double.Parse(lng.Substring(0, 3));
                double lngMins = double.Parse(lng.Substring(3));
                double longitude = lngHours + lngMins / 60.0;

                if (parts[6] == "W")
                    longitude *= -1;

                double speed = double.Parse(parts[7]);
                double bearing = double.Parse(parts[8]);

                return new GpsPoint
                {
                    BearingInDegrees = bearing,
                    Latitude = latitude,
                    Longitude = longitude,
                    SpeedInKnots = speed,
                    Timestamp = utcTime
                };
            }
            catch (Exception)
            {
                // One of our parses failed...ignore.
                Debug.Print("parse failed exception");
            }
            return null;
        }

        private static bool IsCheckSumGood(string sentence)
        {
            int index1 = sentence.IndexOf("$");
            int index2 = sentence.LastIndexOf("*");

            if (index1 != 0 || index2 != sentence.Length - 3)
            {
                return false;
            }

            string checkSumString = sentence.Substring(index2 + 1, 2);
            int checkSum1 = Convert.ToInt32(checkSumString, 16);

            string valToCheck = sentence.Substring(index1 + 1, index2 - 1);
            char c = valToCheck[0];

            for (int i = 1; i < valToCheck.Length; i++)
            {
                c ^= valToCheck[i];
            }

            return checkSum1 == c;
        }
    }
}