using System.IO;

namespace FfMpeg
{
    public class MediaFile
    {
        public MediaFile(string file) : this(new FileInfo(file))
        { }

        public MediaFile(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public MediaFile(FileInfo fileInfo, MetaData metaData)
        {
            FileInfo = fileInfo;
            MetaData = metaData;
        }

        public FileInfo FileInfo { get; }
        internal MetaData MetaData { get; set; }
    }
}