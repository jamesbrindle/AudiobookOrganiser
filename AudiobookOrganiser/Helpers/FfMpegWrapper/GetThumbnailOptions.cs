using AudiobookOrganiser.Helpers.FfMpegWrapper.Enums;
using System;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    public class GetThumbnailOptions
    {
        /// <summary>
        ///     Hide Banner that Ffmpeg usually shows at the start of the process
        /// </summary>
        public bool HideBanner { get; set; } = true;

        /// <summary>
        /// <summary>
        ///     The frame to begin seeking from.
        /// </summary>
        public TimeSpan? Seek { get; set; }

        /// <summary>
        ///     Output video aspect ratios
        /// </summary>
        public VideoAspectRatio VideoAspectRatio { get; set; } = VideoAspectRatio.Default;

        /// <summary>
        ///     Thumbnail size
        /// </summary>
        public VideoSize VideoSize { get; set; } = VideoSize.Default;

        /// <summary>
        ///     Custom Width when VideoSize.Custom is set
        /// </summary>
        public int? CustomWidth { get; set; }

        /// <summary>
        ///     Custom Height when VideoSize.Custom is set
        /// </summary>
        public int? CustomHeight { get; set; }

        /// <summary>
        ///     Specifies an optional rectangle from the source video to crop
        /// </summary>
        public CropRectangle SourceCrop { get; set; }

        /// <summary>
        ///     Force overwrite if output file already exists
        /// </summary>
        public bool Overwrite { get; set; } = false;
    }
}
