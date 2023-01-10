using AudiobookOrganiser.Helpers.FfMpegWrapper.Enums;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    ///     FFmpeg wrapper set audio / video meta data options
    /// </summary>
    public class SetMetaDataOptions
    {
        /// <summary>
        ///     Hide Banner that Ffmpeg usually shows at the start of the process
        /// </summary>
        public bool HideBanner { get; set; } = true;

        /// <summary>
        ///     Hardware acceleration type (can usually keep as none, since setting meta data options is just a 'copy'
        /// </summary>
        public HWAccel HWAccel { get; set; } = HWAccel.None;

        /// <summary>
        /// Overwrite output file if it already exists
        /// </summary>
        public bool Overwrite { get; set; } = false;

        /// <summary>
        /// Path to text file containing meta data to give to output file (see https://wiki.multimedia.cx/index.php/FFmpeg_Metadata to give you an idea 
        /// of the text to use. Or if you just want to produce a chapter list from multiple input files, you can use method: FfMetaDataHelper.CreateAndOutputMetaDataAndChapterFileForConcatenation
        /// </summary>
        public string MetaDataFilePath { get; set; } = null;
    }
}
