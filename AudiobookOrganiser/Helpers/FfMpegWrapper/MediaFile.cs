using System.IO;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    /// Defines a file object to be used throughout the FfMpeg wrapper. The file object contains a System.IO.FileInfo object 
    /// and optionally a meta data object (useful for concatonation). Note that the file path doesn't have to exist yet, such
    /// as in the case when you're defining the 'output' object
    /// </summary>
    public class MediaFile
    {
        /// <summary>
        /// New MediaFile contructor
        /// </summary>
        /// <param name="filePath">File path of existing or future (i.e. output) media file</param>
        public MediaFile(string filePath) : this(new FileInfo(filePath))
        { }

        /// <summary>
        /// New MediaFile contructor
        /// </summary>
        /// <param name="filePath">FileInfo object of existing or future (i.e. output) media file</param>
        public MediaFile(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        /// <summary>
        /// New MediaFile contructor
        /// </summary>
        /// <param name="filePath">FileInfo object of existing or future (i.e. output) media file</param>
        /// <param name="metaData">Initialised meta data objet</param>
        public MediaFile(FileInfo fileInfo, MetaData metaData)
        {
            FileInfo = fileInfo;
            MetaData = metaData;
        }

        /// <summary>
        /// FileInfo object of existing or future (i.e. output) media file
        /// </summary>
        public FileInfo FileInfo { get; }

        /// <summary>
        /// Meta data object of media file
        /// </summary>
        public MetaData MetaData { get; set; }
    }
}