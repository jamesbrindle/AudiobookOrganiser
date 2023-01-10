using AudiobookOrganiser.Helpers.FfMpegWrapper.Enums;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    public class ConcatenationOptions
    {
        public ConcatenationType ConcatenationType { get; set; }

        /// <summary>
        ///     Hide Banner that Ffmpeg usually shows at the start of the process
        /// </summary>
        public bool HideBanner { get; set; } = true;

        /// <summary>
        ///     Hadware Acceleration type to use, if known.
        /// </summary>
        public HWAccel HWAccel { get; set; } = HWAccel.None;

        /// <summary>
        ///     Which encoder to use when converting. I.e. if the output file is .aac audio, you'd use the 'aac' codec.
        ///     Normally ffmpeg will try to decide, based on the output file type, however it's wise to specify to take 
        ///     advantage of non-default codes. For instance, in the example of 'aac' it would actually be better to 
        ///     use the 'LibFDK_AAC' codec when encoding smaller bit rates.
        /// </summary>
        public Codec Codec { get; set; } = Codec.Default;

        /// <summary>
        ///     Output Audio bit rate
        /// </summary>
        public int? AudioBitRate { get; set; } = null;

        public bool Overwrite { get; set; } = false;

        /// <summary>
        ///     Overwrite media if output file already exists
        /// </summary>
        public string ChaptersMapTextFilePath { get; set; } = null;

        public string OuputFileExtension { get; set; } = null;
    }
}
