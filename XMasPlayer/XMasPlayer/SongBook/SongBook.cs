using System;

namespace XMasPlayer
{
    public class SongBook
    {
        public Song[] Songs { get; private set; }

        public void LoadSongs ()
        {
            Songs = new Song[3];

            Songs[0] = LoadJingleBells();
        }

        private Note[] GetNotesFromStrings(string[] values)
        {
            if (values.Length % 2 != 0)
                throw new ArgumentException("SongBook: GetNotesFromStrings() String array for notes must be an even length");

            var len = values.Length/2;

            var notes = new Note[len];

            for (int i = 0; i < len; i++)
            {
                notes[i] = Note.GetNote(values[i * 2], values[i * 2 + 1]);
            }

            return notes;
        }

        private Song LoadJingleBells()
        {
            var melodyText = new string[]
            {
                "E5", "Quarter", "E5", "Quarter", "E5", "Half",
                "E5", "Quarter", "E5", "Quarter", "E5", "Half",
                "E5", "Quarter", "G5", "Quarter", "C5", "Quarter","D5", "Quarter",
                "E5", "Whole",

                "F5", "Quarter", "F5", "Quarter", "F5", "DottedQuarter", "F5", "Eighth",
                "F5", "Quarter", "E5", "Quarter", "E5", "Quarter", "E5", "Eighth", "E5", "Eighth",
                "E5", "Quarter", "D5", "Quarter", "D5", "Quarter", "E5", "Quarter",
                "D5", "Half", "G5", "Half",

                "E5", "Quarter", "E5", "Quarter","E5", "Half",
                "E5", "Quarter","E5", "Quarter","E5", "Half",
                "E5", "Quarter","G5", "Quarter","C5", "Quarter","D5", "Quarter",
                "E5", "Whole",

                "F5", "Quarter", "F5", "Quarter", "F5", "DottedQuarter", "F5", "Eighth",
                "F5", "Quarter", "E5", "Quarter", "E5", "Quarter", "E5", "Eighth", "E5", "Eighth",
                "G5", "Quarter", "G5", "Quarter", "F5", "Quarter", "D5", "Quarter",
                "C5", "Whole",
            };

            var bassText = new string[]
            {
                "C3", "Whole",
                "C3", "Whole",
                "C3", "Whole",
                "A3", "Quarter", "Silence", "Quarter" , "A3", "Quarter", "Silence", "Quarter",

                "E4", "Whole",
                "A3", "Whole",
                "E4", "Whole",
                "E4", "Half",  "Silence", "Half",

                "C3", "Whole",
                "C3", "Whole",
                "C3", "Whole",
                "A3", "Quarter", "Silence", "Quarter" , "A3", "Quarter", "Silence", "Quarter",

                "E4", "Whole",
                "A3", "Whole",
                "E4", "Whole",
                "A3", "Whole",
            };

            return new Song()
            {
                Title = "Jingle Bells",
                Bass = GetNotesFromStrings(bassText),
                Melody = GetNotesFromStrings(melodyText),
            };
        }
    }
}