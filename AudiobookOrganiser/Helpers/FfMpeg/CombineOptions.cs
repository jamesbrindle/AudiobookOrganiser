namespace FfMpeg
{
    public class CombineOptions
    {
        public bool ReplaceExistingAudioStream { get; set; } = true;

        public bool VideoLongerThanAudio { get; set; } = true;

        public bool HideBanner { get; set; } = true;

        public double? LengthInSeconds { get; set; } = null;
        public bool Overwrite { get; set; } = false;
    }
}
