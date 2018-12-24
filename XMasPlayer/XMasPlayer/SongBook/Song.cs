namespace XMasPlayer
{
    public class Song
    {
        public string Title { get; set; }

        public Note[] Melody { get; set; }
        public Note[] Bass { get; set; }
    }
}