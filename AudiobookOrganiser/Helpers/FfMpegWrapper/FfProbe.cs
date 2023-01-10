using AudiobookOrganiser.Helpers.FfMpegWrapper.Extensions;
using System.Text.RegularExpressions;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    /// FFprobe just reads and output data of an object. I.e. meta data, audio specs (bit-rate, FTP, scale etc)
    /// </summary>
    public class FfProbe
    {
        private readonly string _ffProbeExePath = string.Empty;

        public FfProbe(string ffProbeExePath)
        {
            this._ffProbeExePath = ffProbeExePath;
        }

        /// <summary>
        /// Get an FFMpeg standard 'checksum' of a media file
        /// </summary>
        /// <param name="inputPath">Full path to media file</param>
        /// <returns>Checksum string</returns>
        public string GetChecksum(string inputPath)
        {
            try
            {
                string output = ProcessExtensions.ExecuteProcessAndReadStdOut(_ffProbeExePath, $"-i \"{inputPath}\"");
                string[] parts = Regex.Split(output, "file checksum == ");
                string[] parts2 = Regex.Split(parts[1], @"\[");

                return parts2[0].Trim();
            }
            catch { }

            return string.Empty;
        }
    }
}