using FfMpeg.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FfMpeg.Extensions
{
    public class FormatHelper
    {
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

        public static Codec? GetAudioCodecFromFormat(string format)
        {
            switch (format)
            {
                case "mp3":
                    return Codec.libmp3lame;
                case "ra":
                    return Codec.real_144;
                case "aac":
                case "m4a":
                case "m4b":
                    return Codec.aac;
                case "flac":
                    return Codec.flac;
                case "wav":
                    return Codec.pcm_u8;
                case "wma":
                    return Codec.wmav2;
                case "wv":
                    return Codec.wavpack;
                case "ogg":
                case "oga":
                case "mogg":
                    return Codec.vorbis;
                case "opus":
                    return Codec.opus;
                default:
                    return null;
            }
        }

        public static List<Codec> GetVideoCodecsFromFormat(string format)
        {
            var codecs = new List<Codec>();

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

        public static Codec GetBestCodecFromFormat(string format)
        {
            switch (format)
            {
                case "flv":
                    return Codec.flashsv2;
                case "mpeg":
                    return Codec.mpeg4;
                default:
                    break;
            }

            return Codec.Default;
        }

        public static Format GetFormat(string format)
        {
            if (format == "3gp")
                return Format._3gp;

            return (Format)Enum.Parse(typeof(Format), format);
        }

        public static Codec GetCodec(string codec)
        {
            if (codec == "default")
                return Codec.Default;

            return (Codec)Enum.Parse(typeof(Codec), codec);
        }

        public static FormatType GetFormatMediaType(Format format)
        {
            if (AudioFormats.Contains(format.ToString().ToLower().Replace(".", "")))
                return FormatType.Audio;

            return FormatType.Video;
        }

        public static FormatType GetFormatMediaType(string format)
        {
            if (AudioFormats.Contains(format.ToLower().Replace(".", "")))
                return FormatType.Audio;

            return FormatType.Video;
        }

        public static HardwareEncoderType DetermineHardwareEncoderType(string ffmpegPath)
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
                              isNvenc = IsNvenc(ffmpegPath);
                              if (isNvenc)
                                  state.Break();
                          }
                          else if (index == 1)
                          {
                              isQsv = IsQsv(ffmpegPath);
                              if (isQsv)
                                  state.Break();
                          }
                          else if (index == 2)
                          {
                              isAmf = IsAmf(ffmpegPath);
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

        private static bool IsAmf(string ffmpegPath)
        {
            string tmpFile = Path.GetTempPath() + DateTime.Now.Ticks + ".mp4";
            bool error = false;

            var ffMpeg = new Engine(ffmpegPath);
            ffMpeg.Error += (sender, e) => { error = true; };

            Task.Run(async () =>
                await ffMpeg.ExecuteAsync($"-t 0.1 -f lavfi -i color=c=black:s=180x120 -c:v h264_amf -r 1 {tmpFile} -y -hide_banner -loglevel error"))
                            .Wait();

            if (error)
                return false;

            return true;
        }

        private static bool IsQsv(string ffmpegPath)
        {
            string tmpFile = Path.GetTempPath() + DateTime.Now.Ticks + ".mp4";
            bool error = false;

            var ffMpeg = new Engine(ffmpegPath);
            ffMpeg.Error += (sender, e) => { error = true; };

            Task.Run(async () => await
                ffMpeg.ExecuteAsync($"-t 0.1 -f lavfi -i color=c=black:s=40x20 -c:v h264_qsv -r 1 {tmpFile} -y -hide_banner -loglevel error"))
                      .Wait();

            if (error)
                return false;

            return true;
        }

        private static bool IsNvenc(string ffmpegPath)
        {
            string tmpFile = Path.GetTempPath() + DateTime.Now.Ticks + ".mp4";
            bool error = false;

            var ffMpeg = new Engine(ffmpegPath);
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
