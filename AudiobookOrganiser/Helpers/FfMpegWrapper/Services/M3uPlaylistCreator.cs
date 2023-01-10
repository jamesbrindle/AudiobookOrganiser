using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Services
{
    /// <summary>
    /// Create an m3u playlist string contents for a file for a list of mp3 audio media files
    /// </summary>
    public class M3uPlaylistCreator : IPlaylistCreator
    {
        /// <summary>
        /// Create an m3u playlist string contents for a file for a list of mp3 audio media files
        /// </summary>
        public string Create(IList<MetaData> metaData)
        {
            if (metaData == null)
                throw new ArgumentException(nameof(metaData));

            var sb = new StringBuilder();
            sb.AppendLine("#EXTM3U");
            foreach (var meta in metaData)
            {
                sb.AppendLine($"#EXTINF:{(int)meta.Duration.TotalSeconds},{meta.FileInfo.Name}");
                sb.AppendLine(
                    $"file:///{meta.FileInfo.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}");
            }

            return sb.ToString();
        }
    }
}