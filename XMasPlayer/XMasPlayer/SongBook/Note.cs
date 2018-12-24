using System;

namespace XMasPlayer
{
    public static class Lengths
    {
        public static int ThirtySecond = 1;
        public static int Sixteeth = 2;
        public static int Eighth = 4;
        public static int Quarter = 8;
        public static int DottedQuarter = 12;
        public static int Half = 16;
        public static int DottedHalf = 24;
        public static int Whole = 32;
    }

    public static class Pitches
    {
        public static int Silence = 0;
        public static int B0 = 31;
        public static int C1  = 33;
        public static int CS1 = 35;
        public static int D1  = 37;
        public static int DS1 = 39;
        public static int E1  = 41;
        public static int F1  = 44;
        public static int FS1 = 46;
        public static int G1  = 49;
        public static int GS1 = 52;
        public static int A1  = 55;
        public static int AS1 = 58;
        public static int B1  = 62;
        public static int C2  = 65;
        public static int CS2 = 69;
        public static int D2  = 73;
        public static int DS2 = 78;
        public static int E2  = 82;
        public static int F2  = 87;
        public static int FS2 = 93;
        public static int G2  = 98;
        public static int GS2 = 104;
        public static int A2  = 110;
        public static int AS2 = 117;
        public static int B2  = 123;
        public static int C3  = 131;
        public static int CS3 = 139;
        public static int D3  = 147;
        public static int DS3 = 156;
        public static int E3  = 165;
        public static int F3  = 175;
        public static int FS3 = 185;
        public static int G3  = 196;
        public static int GS3 = 208;
        public static int A3  = 220;
        public static int AS3 = 233;
        public static int B3  = 247;
        public static int C4  = 262;
        public static int CS4 = 277;
        public static int D4  = 294;
        public static int DS4 = 311;
        public static int E4  = 330;
        public static int F4  = 349;
        public static int FS4 = 370;
        public static int G4  = 392;
        public static int GS4 = 415;
        public static int A4  = 440;
        public static int AS4 = 466;
        public static int B4  = 494;
        public static int C5  = 523;
        public static int CS5 = 554;
        public static int D5  = 587;
        public static int DS5 = 622;
        public static int E5  = 659;
        public static int F5  = 698;
        public static int FS5 = 740;
        public static int G5  = 784;
        public static int GS5 = 831;
        public static int A5  = 880;
        public static int AS5 = 932;
        public static int B5  = 988;
        public static int C6 = 1047;
        public static int CS6 = 1109;
        public static int D6  = 1175;
        public static int DS6 = 1245;
        public static int E6  = 1319;
        public static int F6  = 1397;
        public static int FS6 = 1480;
        public static int G6  = 1568;
        public static int GS6 = 1661;
        public static int A6  = 1760;
        public static int AS6 = 1865;
        public static int B6  = 1976;
        public static int C7  = 2093;
        public static int CS7 = 2217;
        public static int D7  = 2349;
        public static int DS7 = 2489;
        public static int E7  = 2637;
        public static int F7  = 2794;
        public static int FS7 = 2960;
        public static int G7  = 3136;
        public static int GS7 = 3322;
        public static int A7  = 3520;
        public static int AS7 = 3729;
        public static int B7  = 3951;
        public static int C8  = 4186;
        public static int CS8 = 4435;
        public static int D8  = 4699;
        public static int DS8 = 4978;
    }

    public class Note
    {
        public int Pitch { get; set; }
        public int Length { get; set; }

        public static Note GetNote (string pitch, string length)
        {
            var field = typeof(Pitches).GetField(pitch);
            int pitchValue = (int)field.GetValue(null);

            field = typeof(Lengths).GetField(length);
            int lenValue = (int)field.GetValue(null);

            return new Note() { Pitch = pitchValue, Length = lenValue };
        }
    }
}