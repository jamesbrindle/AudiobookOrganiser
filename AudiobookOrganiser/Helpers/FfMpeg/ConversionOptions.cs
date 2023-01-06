using FfMpeg.Enums;
using System;

namespace FfMpeg
{
    public class ConversionOptions
    {
        public bool HideBanner { get; set; } = false;

        public int Threads { get; set; } = 0;

        public int? QualityCrf { get; set; } = null;

        public HWAccel HWAccel { get; set; } = HWAccel.None;

        public bool HWAccelOutputFormatCopy { get; set; } = true;

        public bool Copy { get; set; } = false;

        public int? AudioBitRate { get; set; } = null;

        public bool RemoveAudio { get; set; } = false;

        public bool RemoveVideo { get; set; } = false;

        public AudioSampleRate AudioSampleRate { get; set; } = AudioSampleRate.Default;

        public TimeSpan? MaxVideoDuration { get; set; }

        public TimeSpan? Seek { get; set; }

        public Target Target { get; set; } = Target.Default;

        public TargetStandard TargetStandard { get; set; } = TargetStandard.Default;

        public VideoAspectRatio VideoAspectRatio { get; set; } = VideoAspectRatio.Default;

        public int? VideoBitRate { get; set; } = null;

        public int? AudioChanel { get; set; } = null;

        public double? VideoFps { get; set; } = null;

        public string PixelFormat { get; set; } = null;

        public string ActivationBytes { get; set; } = null;

        public string AudibleKey { get; set; } = null;

        public string AudibleIv { get; set; } = null;

        public VideoSize VideoSize { get; set; } = VideoSize.Default;

        public Codec Codec { get; set; } = Codec.Default;

        public VideoCodecPreset VideoCodecPreset { get; set; } = VideoCodecPreset.Default;

        public VideoCodecProfile VideoCodecProfile { get; set; } = VideoCodecProfile.Default;

        public Format Format { get; set; } = Format.Default;

        private double? videoTimeScale = null;
        public double? VideoTimeScale { get => videoTimeScale; set => videoTimeScale = (value > 0) ? value : 1; }

        public bool MapMetadata { get; set; } = true;

        public int? CustomWidth { get; set; }

        public int? CustomHeight { get; set; }

        public string ExtraArguments { get; set; }

        public CropRectangle SourceCrop { get; set; }

        public bool Overwrite { get; set; } = false;

        public void CutMedia(TimeSpan seekToPosition, TimeSpan length)
        {
            Seek = seekToPosition;
            MaxVideoDuration = length;
        }
    }
}
