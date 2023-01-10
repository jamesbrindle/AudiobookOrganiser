using AudiobookOrganiser.Helpers.FfMpegWrapper.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Helpers
{
    /// <summary>
    ///     Extension method for help with media format (file type) and encoding (codec) choices.
    /// </summary>
    public class FormatHelper
    {
        /// <summary>
        /// Give all possible media formats as a string list.
        /// </summary>
        public static List<string> AllFormats
        {
            get
            {
                var allFormats = Enum.GetValues(typeof(Format))
                         .Cast<Format>()
                         .OrderBy(v => v.ToString())
                         .Select(v => v.ToString())
                         .ToList();

                allFormats.Remove("Default");
                allFormats.Remove("matroska");
                allFormats.Remove("_3gp");
                allFormats.Insert(0, "3gp");

                return allFormats;
            }
        }

        /// <summary>
        /// Give all possible 'video' media formats as a string list.
        /// </summary>
        public static List<string> VideoFormats
        {
            get
            {
                var audioFormats = Enum.GetValues(typeof(AudioFormat))
                         .Cast<AudioFormat>()
                         .OrderBy(v => v.ToString())
                         .Select(v => v.ToString())
                         .ToList();

                var allFormats = AllFormats;

                foreach (string audioFormat in audioFormats)
                    allFormats.Remove(audioFormat);

                return allFormats;
            }
        }

        /// <summary>
        /// Give all possible 'audio' media formats as a string list.
        /// </summary>
        public static List<string> AudioFormats
        {
            get
            {
                return Enum.GetValues(typeof(AudioFormat))
                         .Cast<AudioFormat>()
                         .OrderBy(v => v.ToString())
                         .Select(v => v.ToString())
                         .ToList();
            }
        }

        /// <summary>
        /// Give all possible media codecs as a string list.
        /// </summary>
        public static List<string> AllCodecs
        {
            get
            {
                var allCodecs = Enum.GetValues(typeof(Codec))
                                         .Cast<Codec>()
                                         .OrderBy(v => v.ToString())
                                         .Select(v => v.ToString())
                                         .ToList();

                allCodecs.Remove("copy");
                allCodecs.Remove("Default");
                allCodecs.Insert(0, "default");

                return allCodecs;
            }
        }

        /// <summary>
        /// Give all possible 'video' codecs as a string list.
        /// </summary>
        public static List<string> VideoCodecs
        {
            get
            {
                var audioCodecs = Enum.GetValues(typeof(AudioCodec))
                                         .Cast<AudioCodec>()
                                         .OrderBy(v => v.ToString())
                                         .Select(v => v.ToString())
                                         .ToList();

                var allCodes = AllCodecs;

                foreach (string audioFormat in audioCodecs)
                    allCodes.Remove(audioFormat);

                return allCodes;
            }
        }

        /// <summary>
        /// Give all possible media codecs that could be audio or video.
        /// </summary>
        public static List<string> NonSpecificVideoCodecs
        {
            get
            {
                var audioCodecs = Enum.GetValues(typeof(AudioCodec))
                                         .Cast<AudioCodec>()
                                         .OrderBy(v => v.ToString())
                                         .Select(v => v.ToString())
                                         .ToList();

                var allCodecs = AllCodecs;

                foreach (string audioFormat in audioCodecs)
                    allCodecs.Remove(audioFormat);

                allCodecs.Remove("flashsv");
                allCodecs.Remove("flashsv2");
                allCodecs.Remove("flv");
                allCodecs.Remove("mpeg4");
                allCodecs.Remove("mpeg1video");
                allCodecs.Remove("mp2");
                allCodecs.Remove("mp2fixed");
                allCodecs.Remove("mpeg2video");
                allCodecs.Remove("mpeg2_qsv");
                allCodecs.Remove("libtheora");
                allCodecs.Remove("wmv1");
                allCodecs.Remove("wmv2");

                return allCodecs;
            }
        }

        /// <summary>
        /// Give all possible 'audio' codecs as a string list.
        /// </summary>
        public static List<string> AudioCodecs
        {
            get
            {
                return Enum.GetValues(typeof(AudioCodec))
                         .Cast<AudioCodec>()
                         .OrderBy(v => v.ToString())
                         .Select(v => v.ToString())
                         .ToList();
            }
        }

        /// <summary>
        /// Given a specifc format (i.e. mp3), give me all possible audio codecs that a valid for encoding
        /// </summary>
        /// <param name="format">The media format (usually the file extension, i.e. mp3)</param>
        /// <param name="libFDK_AAC_EncodingEnabled">If your ffmpeg.exe has been built with the including of the the libFdk_aac code,
        /// includ that as well.</param>
        /// <returns>A list of valid codecs you can use for encoding</returns>
        public static List<Codec> GetAudioCodecFromFormat(string format, bool libFDK_AAC_EncodingEnabled = false)
        {
            var codecs = new List<Codec>();

            format = format.Replace(".", "").ToLower();

            switch (format)
            {
                case "mp3":
                    codecs.Add(Codec.libmp3lame);
                    break;
                case "ra":
                    codecs.Add(Codec.real_144);
                    break;
                case "aac":
                case "m4a":
                case "m4b":
                    if (libFDK_AAC_EncodingEnabled)
                        codecs.Add(Codec.libfdk_aac);
                    codecs.Add(Codec.aac);
                    break;
                case "flac":
                    codecs.Add(Codec.flac);
                    break;
                case "alac":
                    codecs.Add(Codec.alac);
                    break;
                case "wav":
                    codecs.Add(Codec.pcm_u8);
                    break;
                case "wma":
                    codecs.Add(Codec.wmav2);
                    break;
                case "wv":
                    codecs.Add(Codec.wavpack);
                    break;
                case "ogg":
                case "oga":
                case "mogg":
                    codecs.Add(Codec.vorbis);
                    break;
                case "opus":
                    codecs.Add(Codec.opus);
                    break;
                default:
                    return null;
            }

            return codecs;
        }

        /// <summary>
        /// Given a specifc format (i.e. mp3), give me all possible video codecs that a valid for encoding
        /// </summary>
        /// <param name="format">The media format (usually the file extension, i.e. mp3)</param>
        /// <param name="libFDK_AAC_EncodingEnabled"></param>
        /// <returns>A list of valid codecs you can use for encoding</returns>
        public static List<Codec> GetVideoCodecsFromFormat(string format)
        {
            var codecs = new List<Codec>();

            format = format.Replace(".", "").ToLower();

            switch (format)
            {
                case "flv":
                    codecs.Add(Codec.Default);
                    codecs.Add(Codec.flashsv);
                    codecs.Add(Codec.flashsv2);
                    codecs.Add(Codec.flv);
                    break;
                case "mpeg":
                    codecs.Add(Codec.Default);
                    codecs.Add(Codec.mpeg4);
                    codecs.Add(Codec.mpeg1video);
                    codecs.Add(Codec.mp2);
                    codecs.Add(Codec.mp2fixed);
                    codecs.Add(Codec.mpeg2video);
                    codecs.Add(Codec.mpeg2_qsv);
                    break;
                case "ogv":
                    codecs.Add(Codec.Default);
                    codecs.Add(Codec.libtheora);
                    break;
                default:
                    if (FfMpegArgumentBuilder.IgnoreCustomCodec(GetFormat(format)))
                    {
                        codecs.Add(Codec.Default);
                        break;
                    }
                    else
                        return null;
            }

            return codecs;
        }

        /// <summary>
        /// Given a specifc format (i.e. mp3), give the best (or 'default') single codec to use for encoding
        /// </summary>
        /// <param name="format">The media format (usually the file extension, i.e. mp3)</param>
        /// <param name="libFDK_AAC_EncodingEnabled">If your ffmpeg.exe has been built with the including of the the libFdk_aac code,
        /// includ that as well.</param>
        /// <returns>A list of valid codecs you can use for encoding</returns>
        public static Codec GetBestCodecFromFormat(string format, bool libFDK_AAC_EncodingEnabled = false)
        {
            format = format.Replace(".", "").ToLower();

            switch (format)
            {
                case "aac":
                    if (libFDK_AAC_EncodingEnabled)
                        return Codec.libfdk_aac;
                    else
                        return Codec.Default;
                case "flv":
                    return Codec.flashsv2;
                case "mpeg":
                    return Codec.mpeg4;
                default:
                    break;
            }

            return Codec.Default;
        }

        /// <summary>
        /// Returns the Format (enum) from a string
        /// </summary>
        /// <param name="format">Format (string) </param>
        /// <returns>Format (enum) </returns>
        public static Format GetFormat(string format)
        {
            format = format.Replace(".", "").ToLower();

            if (format == "3gp")
                return Format._3gp;

            return (Format)Enum.Parse(typeof(Format), format);
        }

        /// <summary>
        /// Returns the Codec (enum) from a string
        /// </summary>
        /// <param name="format">Codec (string) </param>
        /// <returns>Codec (enum) </returns>
        public static Codec GetCodec(string codec)
        {
            if (codec == "default")
                return Codec.Default;

            return (Codec)Enum.Parse(typeof(Codec), codec);
        }

        /// <summary>
        /// Return the 'media type' (i.e. audio video) enum) from a Format (enum)
        /// </summary>
        /// <param name="format">Format (enum)</param>
        /// <returns>FormatType (i.e. audio or video)</returns>
        public static FormatType GetFormatMediaType(Format format)
        {
            if (AudioFormats.Contains(format.ToString().ToLower().Replace(".", "")))
                return FormatType.Audio;

            return FormatType.Video;
        }

        /// <summary>
        /// Return the 'media type' (i.e. audio video) enum) from a Format (string)
        /// </summary>
        /// <param name="format">Format (string)</param>
        /// <returns>FormatType (i.e. audio or video)</returns>
        public static FormatType GetFormatMediaType(string format)
        {
            if (AudioFormats.Contains(format.ToLower().Replace(".", "")))
                return FormatType.Audio;

            return FormatType.Video;
        }

        /// <summary>
        /// This will try automatically determine the hardware acceleration codec based on the current system.
        /// It does this by simple generating a small, blank video file and atemping to use each codec. if it's successful, it
        /// will give you the best hardware acceleration codec to use. (i.e. nvenc (NVidia), qsv (Intel) or amf (AMD)
        /// </summary>
        /// <param name="ffMpegExePath">Path to ffmpeg.exe executable</param>
        /// <returns>HardwareEncoderType (enum), used to determine hardware acceleration codec</returns>
        public static HardwareEncoderType DetermineHardwareEncoderType(string ffMpegExePath)
        {
            try
            {
                bool isAmf = false;
                bool isQsv = false;
                bool isNvenc = false;

                Parallel.For(0, 3,
                      (index, state) =>
                      {
                          if (index == 0)
                          {
                              isNvenc = IsNvenc(ffMpegExePath);
                              if (isNvenc)
                                  state.Break();
                          }
                          else if (index == 1)
                          {
                              isQsv = IsQsv(ffMpegExePath);
                              if (isQsv)
                                  state.Break();
                          }
                          else if (index == 2)
                          {
                              isAmf = IsAmf(ffMpegExePath);
                              if (isAmf)
                                  state.Break();
                          }
                      });

                if (isAmf)
                    return HardwareEncoderType.amf;
                else if (isQsv)
                    return HardwareEncoderType.qsv;
                else if (isNvenc)
                    return HardwareEncoderType.nvenc;
                else
                    return HardwareEncoderType.none;
            }
            catch
            {
                return HardwareEncoderType.none;
            }
        }

        private static bool IsAmf(string ffmpegPath, bool libFDK_AAC_EncodingEnabled = false)
        {
            string tmpFile = Path.GetTempPath() + DateTime.Now.Ticks + ".mp4";
            bool error = false;

            var ffMpeg = new FfMpeg(ffmpegPath, libFDK_AAC_EncodingEnabled);
            ffMpeg.Error += (sender, e) => { error = true; };

            Task.Run(async () =>
                await ffMpeg.ExecuteAsync($"-t 0.1 -f lavfi -i color=c=black:s=180x120 -c:v h264_amf -r 1 {tmpFile} -y -hide_banner -loglevel error"))
                            .Wait();

            if (error)
                return false;

            return true;
        }

        private static bool IsQsv(string ffmpegPath, bool libFDK_AAC_EncodingEnabled = false)
        {
            string tmpFile = Path.GetTempPath() + DateTime.Now.Ticks + ".mp4";
            bool error = false;

            var ffMpeg = new FfMpeg(ffmpegPath, libFDK_AAC_EncodingEnabled);
            ffMpeg.Error += (sender, e) => { error = true; };

            Task.Run(async () => await
                ffMpeg.ExecuteAsync($"-t 0.1 -f lavfi -i color=c=black:s=40x20 -c:v h264_qsv -r 1 {tmpFile} -y -hide_banner -loglevel error"))
                      .Wait();

            if (error)
                return false;

            return true;
        }

        private static bool IsNvenc(string ffmpegPath, bool libFDK_AAC_EncodingEnabled = false)
        {
            string tmpFile = Path.GetTempPath() + DateTime.Now.Ticks + ".mp4";
            bool error = false;

            var ffMpeg = new FfMpeg(ffmpegPath, libFDK_AAC_EncodingEnabled);
            ffMpeg.Error += (sender, e) => { error = true; };

            Task.Run(async () => await
                ffMpeg.ExecuteAsync($"-t 0.1 -f lavfi -i color=c=black:s=180x120 -c:v h264_nvenc -r 1 {tmpFile} -y -hide_banner -loglevel error"))
                      .Wait();

            if (error)
                return false;

            return true;
        }
    }
}