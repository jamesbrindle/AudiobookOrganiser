using AudiobookOrganiser.Helpers.FfMpegWrapper.Extensions;
using System;
using System.IO;
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

        /// <summary>
        /// Get if the file contains invalid data and cannot be processed by Ffmpeg
        /// </summary>
        /// <param name="inputPath">Full path to media file</param>
        /// <returns>Bool - true if invalid</returns>
        public bool GetIsInvalidData(string inputPath)
        {
            if (!File.Exists(inputPath))
                throw new ApplicationException("File does not exists");

            try
            {
                string output = ProcessExtensions.ExecuteProcessAndReadStdOut(_ffProbeExePath, $"-i \"{inputPath}\"");

                if (output.ToLower().Contains("invalid data found when processing input"))
                    return true;
            }
            catch (Exception e)
            {
                var _ = e;
#if DEBUG
                Console.Out.WriteLine(e.Message);
#endif
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the output from FfProbe
        /// </summary>
        /// <param name="inputPath">Full path to media file</param>
        /// <returns>Output string</returns>
        public string GetInfo(string inputPath)
        {
            try
            {
                string output = ProcessExtensions.ExecuteProcessAndReadStdOut(_ffProbeExePath, $"-i \"{inputPath}\"");
                return output;
            }
            catch { }

            return string.Empty;
        }
    }
}