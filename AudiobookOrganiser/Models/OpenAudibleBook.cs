namespace AudiobookOrganiser.Models
{
    public class OpenAudibleBook
    {
        public Property[] Properties { get; set; }

        public class Property
        {
            public string rating_average { get; set; }
            public string copyright { get; set; }
            public Chapter[] chapters { get; set; }
            public string series_link { get; set; }
            public string abridged { get; set; }
            public string description { get; set; }
            public string language { get; set; }
            public string title { get; set; }
            public string info_link { get; set; }
            public string duration { get; set; }
            public string author_link { get; set; }
            public int seconds { get; set; }
            public string narrated_by { get; set; }
            public string product_id { get; set; }
            public string genre { get; set; }
            public string series_name { get; set; }
            public string series_sequence { get; set; }
            public string summary { get; set; }
            public string author { get; set; }
            public string image_url { get; set; }
            public string title_short { get; set; }
            public string rating_count { get; set; }
            public string filename { get; set; }
            public string download_link { get; set; }
            public string release_date { get; set; }
            public string ayce { get; set; }
            public string publisher { get; set; }
            public string asin { get; set; }
            public string region { get; set; }
            public string purchase_date { get; set; }
            public string pdf_url { get; set; }
        }

        public class Chapter
        {
            public int start_offset_ms { get; set; }
            public int length_ms { get; set; }
            public string title { get; set; }
            public int start_offset_sec { get; set; }
        }
    }
}