using System;
using Microsoft.SPOT;

namespace WaveShare_EInk
{
    public class sFONT
    {
        public byte[] Table { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

    }

    public partial class Fonts
    {
        public static int MAX_HEIGHT_FONT = 24;
        public static int MAX_WIDTH_FONT = 17;
        public static int OFFSET_BITMAP = 54;

        public sFONT Font24 { get; set; }
        public sFONT Font20 { get; set; }
        public sFONT Font16 { get; set; }
        public sFONT Font12 { get; set; }
        public sFONT Font8 { get; set; }

        public Fonts ()
        {
            Font12 = new sFONT()
            {
                Table = this.Font12Data
            };
        }
    }
}