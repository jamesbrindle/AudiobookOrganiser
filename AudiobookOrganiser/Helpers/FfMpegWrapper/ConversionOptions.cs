using AudiobookOrganiser.Helpers.FfMpegWrapper.Enums;
using System;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    ///     FFmpeg wrapper audio / video conversion options.
    /// </summary>
    public class ConversionOptions
    {
        /// <summary>
        ///     Hide Banner that Ffmpeg usually shows at the start of the process
        /// </summary>
        public bool HideBanner { get; set; } = false;

        /// <summary>
        ///     Video Speed Up / Down using setpts filter
        /// </summary>
        private double? videoTimeScale;

        /// <summary>
        ///     Force overwrite if output file already exists
        /// </summary>
        public bool Overwrite { get; set; } = false;

        /// <summary>
        ///     Set the number of threads to be used, in case the selected codec implementation supports multi-threading.
        ///     Possible values:
        ///     - 0 - automatically select the number of threads to set
        ///     - integer to max of cpu cores
        ///     Default value is ‘0’.
        /// </summary>
        public int Threads { get; set; } = 0;

        /// <summary>
        ///     CRF (Constant Rate Factor) based quality (Lower is better. Recommended ranges: 17 - 40)
        /// </summary>
        public int? QualityCrf { get; set; } = null;

        /// <summary>
        ///     Hadware Acceleration type to use, if known.
        /// </summary>
        public HWAccel HWAccel { get; set; } = HWAccel.None;

        /// <summary>
        ///     Hardware Acceleration Output Format - Force HWAccel if selected
        /// </summary>
        public bool HWAccelOutputFormatCopy { get; set; } = true;

        /// <summary>
        ///     Copy source to output while only changing the container
        /// </summary>
        public bool Copy { get; set; } = false;

        /// <summary>
        ///     For decoding AAX (Audible) files
        /// </summary>
        public string ActivationBytes { get; set; } = null;

        /// <summary>
        ///     For decoding AAXC (Audible) files
        /// </summary>
        public string AudibleKey { get; set; } = null;

        /// <summary>
        ///     For decoding AAXC (Audible) files
        /// </summary>
        public string AudibleIv { get; set; } = null;

        /// <summary>
        ///    Output audio bit rate
        /// </summary>
        public int? AudioBitRate { get; set; } = null;

        /// <summary>
        ///     Remove Audio Stream
        /// </summary>
        public bool RemoveAudio { get; set; } = false;

        /// <summary>
        ///     Remove Video Stream
        /// </summary>
        public bool RemoveVideo { get; set; } = false;

        /// <summary>
        ///     Output audio sample rate
        /// </summary>
        public AudioSampleRate AudioSampleRate { get; set; } = AudioSampleRate.Default;

        /// <summary>
        ///     The maximum duration
        /// </summary>
        public TimeSpan? MaxVideoDuration { get; set; }

        /// <summary>
        ///     The frame to begin seeking from.
        /// </summary>
        public TimeSpan? Seek { get; set; }

        /// <summary>
        ///     Predefined audio and video options for various file formats,
        ///     <para>Can be used in conjunction with <see cref="TargetStandard" /> option</para>
        /// </summary>
        public Target Target { get; set; } = Target.Default;

        /// <summary>
        ///     Predefined standards to be used with the <see cref="Target" /> option
        /// </summary>
        public TargetStandard TargetStandard { get; set; } = TargetStandard.Default;

        /// <summary>
        ///     Output video aspect ratios
        /// </summary>
        public VideoAspectRatio VideoAspectRatio { get; set; } = VideoAspectRatio.Default;

        /// <summary>
        ///     Output video bit rate in kbit/s
        /// </summary>
        public int? VideoBitRate { get; set; } = null;

        /// <summary>
        ///     Chanel audio
        /// </summary>
        public int? AudioChanel { get; set; } = null;

        /// <summary>
        ///     Output video frame rate
        /// </summary>
        public double? VideoFps { get; set; } = null;

        /// <summary>
        ///     Pixel format. Available formats can be gathered via `ffmpeg -pix_fmts`.
        /// </summary>
        public string PixelFormat { get; set; } = null;

        /// <summary>
        ///     Video size
        /// </summary>
        public VideoSize VideoSize { get; set; } = VideoSize.Default;

        /// <summary>
        ///     Which encoder to use when converting. I.e. if the output file is .aac audio, you'd use the 'aac' codec.
        ///     Normally ffmpeg will try to decide, based on the output file type, however it's wise to specify to take 
        ///     advantage of non-default codes. For instance, in the example of 'aac' it would actually be better to 
        ///     use the 'LibFDK_AAC' codec when encoding smaller bit rates.
        /// </summary>
        public Codec Codec { get; set; } = Codec.Default;

        /// <summary>
        ///     Codec Preset (Tested for -vcodec libx264)
        /// </summary>
        public VideoCodecPreset VideoCodecPreset { get; set; } = VideoCodecPreset.Default;

        /// <summary>
        ///     Codec Profile (Tested for -vcodec libx264)
        ///     Specifies wheter or not to use a H.264 Profile
        /// </summary>
        public VideoCodecProfile VideoCodecProfile { get; set; } = VideoCodecProfile.Default;

        /// <summary>
        ///     Video sizes
        /// </summary>
        public Format Format { get; set; } = Format.Default;

        public double? VideoTimeScale
        {
            get => videoTimeScale;
            set => videoTimeScale = value > 0 ? value : 1;
        }

        /// <summary>
        ///     Map Metadata from Input to Output
        /// </summary>
        public bool MapMetadata { get; set; } = true;

        /// <summary>
        ///     Custom Width when VideoSize.Custom is set
        /// </summary>
        public int? CustomWidth { get; set; }

        /// <summary>
        ///     Custom Height when VideoSize.Custom is set
        /// </summary>
        public int? CustomHeight { get; set; }

        /// <summary>
        ///     Extra Arguments, such as  -movflags +faststart. Can be used to support missing features temporary
        /// </summary>
        public string ExtraArguments { get; set; }

        /// <summary>
        /// You can convert audio (i.e. Mp3) to Video (.Avi) - You may as well add an image to it to show... You need to if 
        /// the resulting file is an MP4
        /// </summary>
        public string ImagePathForWhenConvertingAudioToVideo { get; set; } = string.Empty;

        /// <summary>
        ///     Specifies an optional rectangle from the source video to crop
        /// </summary>
        public CropRectangle SourceCrop { get; set; }

        /// <summary>
        ///     <para> --- </para>
        ///     <para> Cut audio / video from existing media                </para>
        ///     <para> Example: To cut a 15 minute section                  </para>
        ///     <para> out of a 30 minute video starting                    </para>
        ///     <para> from the 5th minute:                                 </para>
        ///     <para> The start position would be: TimeSpan.FromMinutes(5) </para>
        ///     <para> The length would be:         TimeSpan.FromMinutes(15)</para>
        /// </summary>
        /// <param name="seekToPosition">
        ///     <para>Specify the position to seek to,                  </para>
        ///     <para>if you wish to begin the cut starting             </para>
        ///     <para>from the 5th minute, use: TimeSpan.FromMinutes(5);</para>
        /// </param>
        /// <param name="length">
        ///     <para>Specify the length of the video to cut,           </para>
        ///     <para>to cut out a 15 minute duration                   </para>
        ///     <para>simply use: TimeSpan.FromMinutes(15);             </para>
        /// </param>
        public void CutMedia(TimeSpan seekToPosition, TimeSpan length)
        {
            Seek = seekToPosition;
            MaxVideoDuration = length;
        }
    }
}