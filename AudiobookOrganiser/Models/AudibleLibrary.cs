using System;

namespace AudiobookOrganiser.Models
{
    public class AudibleLibrary
    {
        public Property[] Properties { get; set; }

        public class Property
        {
            public string asin { get; set; }
            public string authors { get; set; }
            public string genres { get; set; }
            public bool is_finished { get; set; }
            public DateTime date_added { get; set; }
            public string narrators { get; set; }
            public float percent_complete { get; set; }
            public string cover_url { get; set; }
            public DateTime purchase_date { get; set; }
            public string rating { get; set; }
            public int num_ratings { get; set; }
            public string release_date { get; set; }
            public int runtime_length_min { get; set; }
            public string series_title { get; set; }
            public string series_sequence { get; set; }
            public string subtitle { get; set; }
            public string title { get; set; }
        }
    }
}
