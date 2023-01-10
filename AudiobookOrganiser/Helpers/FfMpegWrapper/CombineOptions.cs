namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    public class CombineOptions
    {
        /// <summary>
        ///     Hide Banner that Ffmpeg usually shows at the start of the process
        /// </summary>
        public bool HideBanner { get; set; } = true;

        /// <summary>
        /// When combining audio and video, are we also combining audio streams, or replacing the audio stream
        /// </summary>
        public bool ReplaceExistingAudioStream { get; set; } = true;

        /// <summary>
        /// If true, it will take the video length as the shortest output duration, otherwise it will take the audio length as the shortest duration
        /// </summary>
        public bool VideoLongerThanAudio { get; set; } = true;

        /// <summary>
        /// Overide the output playtime length
        /// </summary>
        public double? LengthInSeconds { get; set; } = null;

        /// <summary>
        /// Ovewrite the output media file if it already exists
        /// </summary>
        public bool Overwrite { get; set; } = false;
    }
}
