using System;

namespace AudiobookOrganiser.Models
{
    internal class AudiobookMetaData
    {
        public string Author { get; set; }
        public string Narrator { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Series { get; set; }
        public string SeriesPart { get; set; }
        public string AlbumSort { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public string ProperGenre { get; set; }
        public string Asin { get; set; }
        public string Overview { get; set; }
        public TimeSpan? Duration { get; set; } = null;
        public uint Track { get; set; } = 1;
        public string Copyright { get; set; }
    }
}
