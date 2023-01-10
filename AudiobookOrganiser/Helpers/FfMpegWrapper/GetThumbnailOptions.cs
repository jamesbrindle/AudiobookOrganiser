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
    }
}
