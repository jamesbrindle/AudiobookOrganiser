namespace AudiobookOrganiser.Models
{
    public class LockInModel
    {
        public string Filename { get; set; }
        public string Hash { get; set; }

        public LockInModel(string filename, string hash)
        {
            Filename = filename;
            Hash = hash;
        }
    }
}
