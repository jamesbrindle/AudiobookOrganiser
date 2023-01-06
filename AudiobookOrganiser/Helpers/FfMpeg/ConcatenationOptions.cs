using FfMpeg.Enums;

namespace FfMpeg
{
    public class ConcatenationOptions
    {
        public ConcatenationType ConcatenationType { get; set; }

        public bool HideBanner { get; set; } = true;

        public HWAccel HWAccel { get; set; } = HWAccel.None;

        public Codec Codec { get; set; } = Codec.Default;

        public string OuputFileType { get; set; } = null;

        public int? AudioBitRate { get; set; } = null;

        public bool Overwrite { get; set; } = false;
    }
}
