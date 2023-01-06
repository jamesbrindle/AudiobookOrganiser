using FfMpeg.Enums;
using FfMpeg.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FfMpeg
{
    internal static class FfMpegArgumentBuilder
    {
        public static string Build(FfMpegParameters parameters)
        {
            if (parameters.HasCustomArguments)
                return parameters.CustomArguments;

            switch (parameters.Task)
            {
                case FfMpegTask.EnsureAudioStream:
                    return EnsureAudioStream(parameters.InputFile, parameters.OutputFile);

                case FfMpegTask.Convert:
                    return Convert(parameters.InputFile, parameters.OutputFile, parameters.ConversionOptions);

                case FfMpegTask.Concatenate:
                    return Concatenate(parameters.InputFiles, parameters.OutputFile, parameters.ConcatenationOptions, parameters.TempPath);

                case FfMpegTask.Combine:
                    return Combine(parameters.InputFiles, parameters.OutputFile, parameters.CombineOptions);

                case FfMpegTask.GetMetaData:
                    return GetMetadata(parameters.InputFile);

                case FfMpegTask.GetThumbnail:
                    return GetThumbnail(parameters.InputFile, parameters.OutputFile, parameters.ConversionOptions);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetMetadata(MediaFile inputFile) => $"-i \"{inputFile.FileInfo.FullName}\" -f ffmetadata -";

        private static string GetThumbnail(
            MediaFile inputFile,
            MediaFile outputFile,
            ConversionOptions conversionOptions)
        {
            var defaultTimeSpan = TimeSpan.FromSeconds(1);
            var commandBuilder = new StringBuilder();

            commandBuilder.AppendFormat(
                CultureInfo.InvariantCulture,
                " -ss {0} ",
                conversionOptions?.Seek.GetValueOrDefault(defaultTimeSpan).TotalSeconds ?? defaultTimeSpan.TotalSeconds);

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.FileInfo.FullName);
            commandBuilder.AppendFormat(" -vframes {0} ", 1);

            // Video size / resolution
            commandBuilder = AppendVideoSize(commandBuilder, conversionOptions);

            // Video aspect ratio
            commandBuilder = AppendVideoAspectRatio(commandBuilder, conversionOptions);

            // Video cropping
            commandBuilder = AppendVideoCropping(commandBuilder, conversionOptions);

            return commandBuilder.AppendFormat(" {0}\"{1}\" ", conversionOptions.Overwrite ? "-y " : "", outputFile.FileInfo.FullName).ToString();
        }

        private static string Convert(
            MediaFile inputFile,
            MediaFile outputFile,
            ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            if (conversionOptions.Format == Format._3gp)
                conversionOptions.ExtraArguments = "-vcodec h263 -ar 8000 -b:a 12.20k -ac 1 -s 704x576";

            // Default conversion
            if (conversionOptions == null)
                return commandBuilder.AppendFormat(
                    " -i \"{0}\" \"{1}\" ",
                    inputFile.FileInfo.FullName,
                    outputFile.FileInfo.FullName).ToString();

            if (conversionOptions.HideBanner) commandBuilder.Append(" -hide_banner ");

            if (conversionOptions.Threads != 0)
            {
                commandBuilder.AppendFormat(" -threads {0} ", conversionOptions.Threads);
            }

            // HW Accel
            if (conversionOptions.HWAccel != HWAccel.None)
            {
                commandBuilder.AppendFormat(" -hwaccel {0} ", conversionOptions.HWAccel);
                AppendHWAccelOutputFormat(commandBuilder, conversionOptions);
            }

            // Media seek position
            if (conversionOptions.Seek != null)
                commandBuilder.AppendFormat(
                    CultureInfo.InvariantCulture,
                    " -ss {0} ",
                    conversionOptions.Seek.Value.TotalSeconds);

            // For decoding AAX (audible) files
            if (!string.IsNullOrEmpty(conversionOptions.ActivationBytes))
            {
                commandBuilder.Append($" -activation_bytes {conversionOptions.ActivationBytes} ");
            }

            // For decoding AAXC (audible) files
            if (!string.IsNullOrEmpty(conversionOptions.AudibleKey))
            {
                commandBuilder.Append($" -audible_key {conversionOptions.AudibleKey} ");
            }

            // For decoding AAXC (audible) files
            if (!string.IsNullOrEmpty(conversionOptions.AudibleIv))
            {
                commandBuilder.Append($" -audible_iv {conversionOptions.AudibleIv} ");
            }

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.FileInfo.FullName);

            // Physical media conversion (DVD etc)
            if (conversionOptions.Target != Target.Default)
            {
                commandBuilder.Append(" -target ");
                if (conversionOptions.TargetStandard != TargetStandard.Default)
                {
                    commandBuilder.AppendFormat(
                        " {0}-{1} \"{2}\" ",
                        conversionOptions.TargetStandard.ToString().ToLowerInvariant(),
                        conversionOptions.Target.ToString().ToLowerInvariant(),
                        outputFile.FileInfo.FullName);

                    return commandBuilder.ToString();
                }

                commandBuilder.AppendFormat(
                    "{0} \"{1}\" ",
                    conversionOptions.Target.ToString().ToLowerInvariant(),
                    outputFile.FileInfo.FullName);

                return commandBuilder.ToString();
            }

            #region Video

            // Remove Video
            if (conversionOptions.RemoveVideo && FormatHelper.GetFormatMediaType(conversionOptions.Format) != FormatType.Video)
                commandBuilder.Append(" -vn");

            // Video Format
            commandBuilder = AppendFormat(commandBuilder, conversionOptions);

            // Video Codec
            commandBuilder = AppendCodec(commandBuilder, conversionOptions);

            // Video Codec Preset
            if (conversionOptions.VideoCodecPreset != VideoCodecPreset.Default && !IgnoreCustomPreset(conversionOptions.Format))
                commandBuilder.AppendFormat(" -preset {0} ", conversionOptions.VideoCodecPreset);

            // Video Codec Profile
            if (conversionOptions.VideoCodecProfile != VideoCodecProfile.Default)
                commandBuilder.AppendFormat(" -profile:v {0} ", conversionOptions.VideoCodecProfile);

            // Video Time Scale
            if (conversionOptions.VideoTimeScale != null && conversionOptions.VideoTimeScale != 1)
                commandBuilder.AppendFormat(
                    " -filter:v \"setpts = {0} * PTS\" ",
                    conversionOptions.VideoTimeScale.ToString().Replace(",", "."));

            // Maximum video duration
            if (conversionOptions.MaxVideoDuration != null)
                commandBuilder.AppendFormat(" -t {0} ", conversionOptions.MaxVideoDuration);

            // Video bit rate
            if (conversionOptions.VideoBitRate != null && !IgnoreCustomFps(conversionOptions.Format))
                commandBuilder.AppendFormat(" -b:v {0}k ", conversionOptions.VideoBitRate);

            // Video frame rate
            if (conversionOptions.VideoFps != null && !IgnoreCustomFps(conversionOptions.Format))
                commandBuilder.AppendFormat(" -r {0} ", conversionOptions.VideoFps);

            // Video pixel format
            if (conversionOptions.PixelFormat != null)
                commandBuilder.AppendFormat(" -pix_fmt {0} ", conversionOptions.PixelFormat);

            // CRF quality value
            if (conversionOptions.QualityCrf != null && !IgnoreCustomCrfQuality(conversionOptions.Format))
                commandBuilder.AppendFormat(" -crf {0} ", conversionOptions.QualityCrf);

            if (!IgnoreCustomResolution(conversionOptions.Format))
            {
                // Video size / resolution
                commandBuilder = AppendVideoSize(commandBuilder, conversionOptions);

                // Video aspect ratio
                commandBuilder = AppendVideoAspectRatio(commandBuilder, conversionOptions);

                // Video cropping
                commandBuilder = AppendVideoCropping(commandBuilder, conversionOptions);
            }

            #endregion

            #region Audio

            if (!IgnoreCustomAudioBps(conversionOptions.Format))
            {
                // Audio bit rate
                if (conversionOptions.AudioBitRate != null && !IgnoreCustomAudioBps(conversionOptions.Format))
                    commandBuilder.AppendFormat(" -ab {0}k ", conversionOptions.AudioBitRate);

                // Audio sample rate
                if (conversionOptions.AudioSampleRate != AudioSampleRate.Default && !IgnoreCustomAudioBps(conversionOptions.Format))
                    commandBuilder.AppendFormat(" -ar {0} ", conversionOptions.AudioSampleRate.ToString().Replace("Hz", ""));

                // AudioChannel
                if (conversionOptions.AudioChanel != null)
                    commandBuilder.AppendFormat(" -ac {0} ", conversionOptions.AudioChanel);
            }

            // Remove Audio
            if (conversionOptions.RemoveAudio && !IgnoreRemoveAudio(conversionOptions.Format))
                commandBuilder.Append(" -an ");

            #endregion

            if (conversionOptions.MapMetadata) 
                commandBuilder.Append(" -map_metadata 0 ");

            // Extra arguments
            if (conversionOptions.ExtraArguments != null)
                commandBuilder.AppendFormat(" {0} ", conversionOptions.ExtraArguments);

            return commandBuilder.AppendFormat(" {0}\"{1}\" ", conversionOptions.Overwrite ? "-y " : "", outputFile.FileInfo.FullName).ToString();
        }

        private static string Concatenate(
            MediaFile[] inputFiles,
            MediaFile outputFile,
            ConcatenationOptions concatenationOptions,
            string tempMetaDataPath)
        {
            var commandBuilder = new StringBuilder();

            if (concatenationOptions.HideBanner)
                commandBuilder.Append("-hide_banner ");

            if (concatenationOptions.HWAccel != HWAccel.None && concatenationOptions.ConcatenationType == ConcatenationType.Video)
                commandBuilder.Append($"-hwaccel {concatenationOptions.HWAccel} ");

            foreach (var inputFile in inputFiles)
                commandBuilder.Append($"-i \"{inputFile.FileInfo.FullName}\" ");

            if (concatenationOptions.OuputFileType.ToLower().In(".m4a", ".m4b"))
            {
                string metaDataFile = BuildM4bMetaDataFile(inputFiles, tempMetaDataPath);
                if (!string.IsNullOrEmpty(metaDataFile))
                    commandBuilder.Append($"-i \"{metaDataFile}\" -map_metadata 1 ");
            }

            commandBuilder.Append("-filter_complex \"");

            if (concatenationOptions.ConcatenationType == ConcatenationType.Audio)
            {
                for (int i = 0; i < inputFiles.Length; i++)
                    commandBuilder.Append($"[{i}:0]");

                commandBuilder.Append($"concat=n={inputFiles.Length}:v=0:a=1[outa]\" ");

                // Audio bit rate
                if (concatenationOptions.AudioBitRate != null)
                    commandBuilder.AppendFormat(" -b:a {0}k ", concatenationOptions.AudioBitRate);

                commandBuilder.Append($"-map \"[outa]\" {(concatenationOptions.Overwrite ? "-y" : "")} \"{outputFile.FileInfo.FullName}\"");
            }
            else if (concatenationOptions.ConcatenationType == ConcatenationType.Video)
            {
                for (int i = 0; i < inputFiles.Length; i++)
                    commandBuilder.Append($"[{i}:v:0][{i}:a:0]");

                commandBuilder.Append($"concat=n={inputFiles.Length}:v=1:a=1[outv][outa]\" ");

                // Audio bit rate
                if (concatenationOptions.AudioBitRate != null)
                    commandBuilder.AppendFormat(" -b:a {0}k ", concatenationOptions.AudioBitRate);

                commandBuilder.Append($"-vcodec {concatenationOptions.Codec} ");
                commandBuilder.Append($"-map \"[outv]\" -map \"[outa]\" {(concatenationOptions.Overwrite ? "-y" : "")} \"{outputFile.FileInfo.FullName}\"");
            }

            return commandBuilder.ToString();
        }

        private static string BuildM4bMetaDataFile(MediaFile[] inputFiles, string tempPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine(";FFMETADATA1");

            if (
                inputFiles != null &&
                inputFiles.Length > 0 &&
                inputFiles[0].MetaData != null &&
                inputFiles[0].MetaData.Tags != null &&
                inputFiles[0].MetaData.Tags.Count() > 0)
            {
                foreach (var tag in inputFiles[0].MetaData.Tags)
                {
                    if (tag.Key.ToLower() == "track")
                    {
                        string title = string.Empty;
                        string album = string.Empty;
                        string track = string.Empty;

                        if (inputFiles[0].MetaData.Tags.ContainsKey("title"))
                            title = inputFiles[0].MetaData.Tags["title"];

                        if (inputFiles[0].MetaData.Tags.ContainsKey("album"))
                            album = inputFiles[0].MetaData.Tags["album"];

                        if (inputFiles[0].MetaData.Tags.ContainsKey("track"))
                            track = inputFiles[0].MetaData.Tags["track"];

                        if (!string.IsNullOrEmpty(title))
                        {
                            sb.AppendLine("title=" + title);
                            sb.AppendLine("track=" + title);
                        }
                        else if (!string.IsNullOrEmpty(album))
                        {
                            sb.AppendLine("title=" + album);
                            sb.AppendLine("track=" + album);
                        }
                        else if (!string.IsNullOrEmpty(track))
                        {
                            sb.AppendLine("title=" + track);
                            sb.AppendLine("track=" + track);
                        }
                        else
                        {
                            sb.AppendLine("title=" + Path.GetFileNameWithoutExtension(inputFiles[0].FileInfo.FullName));
                            sb.AppendLine("track=" + Path.GetFileNameWithoutExtension(inputFiles[0].FileInfo.FullName));
                        }
                    }
                    else if (tag.Key.ToLower() != "title")
                    {
                        sb.AppendLine(tag.Key + "=" + tag.Value);
                    }
                }

                int i = 1;
                long currentStart = 1;
                long currentEnd = 0;

                foreach (var file in inputFiles)
                {
                    currentEnd += System.Convert.ToInt64(file.MetaData.Duration.TotalMilliseconds);

                    sb.AppendLine("[CHAPTER]");
                    sb.AppendLine("TIMEBASE=1/1000");
                    sb.AppendLine($"START={currentStart}");
                    sb.AppendLine($"END={currentEnd}");

                    string track = string.Empty;

                    if (
                        file.MetaData != null &&
                        file.MetaData.Tags != null &&
                        file.MetaData.Tags.Count() > 0 &&
                        file.MetaData.Tags.ContainsKey("track"))
                    {
                        track = $"{i}/{inputFiles.Length}: {file.MetaData.Tags["track"]}";
                    }
                    else
                    {
                        track = i + "/" + inputFiles.Length;
                    }

                    sb.AppendLine("title=" + track);

                    currentStart = currentEnd + 1;
                    i++;
                }
            }

            string path;
            if (!string.IsNullOrEmpty(tempPath))
                path = tempPath;
            else
                path = Path.GetTempPath() + ".txt";

            try
            {
                File.WriteAllText(path, sb.ToString());
            }
            catch { }

            if (File.Exists(path))
                return path;
            else
                return null;
        }

        private static string Combine(
            MediaFile[] inputFiles,
            MediaFile outputFile,
            CombineOptions combineOptions)
        {
            var commandBuilder = new StringBuilder();

            if (combineOptions.HideBanner)
                commandBuilder.Append("-hide_banner ");

            int index = 0;
            foreach (var inputFile in inputFiles)
            {
                commandBuilder.Append($"-i \"{inputFile.FileInfo.FullName}\" ");
                if (index == 1)
                {
                    if (!combineOptions.VideoLongerThanAudio && combineOptions.LengthInSeconds != null)
                        commandBuilder.Append($"-t {combineOptions.LengthInSeconds} ");
                }

                index++;
            }

            if (combineOptions.ReplaceExistingAudioStream)
                commandBuilder.Append("-map 0:v:0 -map 1:a:0 ");
            else
                commandBuilder.Append("-filter_complex amix -map 0:v -map 0:a -map 1:a ");

            commandBuilder.Append("-c:v copy -c:a aac ");
            commandBuilder.Append($"{(combineOptions.Overwrite ? "-y " : "")}\"{outputFile.FileInfo.FullName}\"");

            return commandBuilder.ToString();
        }

        private static string EnsureAudioStream(
            MediaFile inputFile,
            MediaFile outputFile)
        {
            var commandBuilder = new StringBuilder();
            commandBuilder.Append("-hide_banner ");
            commandBuilder.Append($"-i \"{inputFile.FileInfo.FullName}\" -f lavfi -i anullsrc -c:v copy -map 0:v -map 0:a? -map 1:a -shortest ");
            commandBuilder.Append($"\"{outputFile.FileInfo.FullName}\"");

            return commandBuilder.ToString();
        }

        private static void AppendHWAccelOutputFormat(
        StringBuilder commandBuilder,
        ConversionOptions conversionOptions)
        {
            if (conversionOptions.HWAccel != HWAccel.None && conversionOptions.HWAccelOutputFormatCopy)
            {
                var accel = conversionOptions.HWAccel;
                bool add = false;
                switch (conversionOptions.HWAccel)
                {
                    case HWAccel.cuda:
                    case HWAccel.cuvid:
                        add = true;
                        accel = HWAccel.cuda;
                        break;
                    case HWAccel.dxva2:     //Not tested
                    case HWAccel.qsv:       //Not tested
                    case HWAccel.d3d11va:   //Not tested
                    default:
                        break;
                }
                if (add) commandBuilder.AppendFormat(" -hwaccel_output_format {0} ", accel);
            }
        }

        private static StringBuilder AppendVideoCropping(
            StringBuilder commandBuilder,
            ConversionOptions conversionOptions)
        {
            if (conversionOptions.SourceCrop != null)
            {
                var crop = conversionOptions.SourceCrop;
                commandBuilder.AppendFormat(" -filter:v \"crop={0}:{1}:{2}:{3}\" ", crop.Width, crop.Height, crop.X, crop.Y);
            }
            return commandBuilder;
        }

        private static StringBuilder AppendVideoAspectRatio(
            StringBuilder commandBuilder,
            ConversionOptions conversionOptions)
        {
            if (conversionOptions.VideoAspectRatio != VideoAspectRatio.Default)
            {
                string ratio = conversionOptions.VideoAspectRatio.ToString();
                ratio = ratio.Substring(1);
                ratio = ratio.Replace("_", ":");

                commandBuilder.AppendFormat(" -aspect {0} ", ratio);
            }
            return commandBuilder;
        }

        private static StringBuilder AppendVideoSize(
            StringBuilder commandBuilder,
            ConversionOptions conversionOptions)
        {
            if (conversionOptions.VideoSize == VideoSize.Custom)
            {
                commandBuilder.AppendFormat(
                    " -vf \"scale={0}:{1}\" ",
                    conversionOptions.CustomWidth ?? -2,
                    conversionOptions.CustomHeight ?? -2);
            }
            else if (conversionOptions.VideoSize != VideoSize.Default)
            {
                string size = conversionOptions.VideoSize.ToString().ToLowerInvariant();
                if (size.StartsWith("_"))
                    size = size.Replace("_", "");
                if (size.Contains("_"))
                    size = size.Replace("_", "-");

                commandBuilder.AppendFormat(" -s {0} ", size);
            }
            return commandBuilder;
        }

        private static StringBuilder AppendCodec(
            StringBuilder commandBuilder,
            ConversionOptions conversionOptions)
        {
            if (!IgnoreCustomCodec(conversionOptions.Format))
            {
                string codec = conversionOptions.Codec.ToString().ToLowerInvariant();
                if (FormatHelper.GetFormatMediaType(conversionOptions.Format) != FormatType.Audio)
                {
                    if (conversionOptions.Codec != Codec.copy && !conversionOptions.Copy && conversionOptions.Codec != Codec.Default)
                        commandBuilder.AppendFormat(" -vcodec {0}", codec);
                }
                else
                {
                    if (conversionOptions.Codec != Codec.copy && !conversionOptions.Copy && conversionOptions.Codec != Codec.Default)
                        commandBuilder.AppendFormat(" -acodec {0}", codec);
                }
            }

            if ((conversionOptions.Codec == Codec.copy || conversionOptions.Copy) && !IgnoreCopy(conversionOptions.Format))
                commandBuilder.AppendFormat(" -c copy");

            return commandBuilder;
        }

        private static StringBuilder AppendFormat(
            StringBuilder commandBuilder,
            ConversionOptions conversionOptions)
        {
            if (!IgnoreCustomCodec(conversionOptions.Format))
            {
                string format = conversionOptions.Format.ToString().ToLowerInvariant();

                if (format == "mkv")
                    format = "matroska";
                if (format == "ts")
                    format = "mpegts";

                if (format.StartsWith("_"))
                    format = format.Replace("_", "");

                if (FormatHelper.GetFormatMediaType(conversionOptions.Format) != FormatType.Audio)
                    commandBuilder.AppendFormat(" -f {0} ", format);
            }
            return commandBuilder;
        }

        public static bool IgnoreCustomCodec(Format format)
        {
            switch (format)
            {
                case Format.Default:
                case Format._3gp:
                case Format.mov:
                case Format.ts:
                case Format.vob:
                case Format.wmv:
                case Format.webm:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreCopy(Format format)
        {
            switch (format)
            {
                case Format._3gp:
                case Format.mpeg:
                case Format.webm:
                case Format.ogv:
                case Format.wmv:
                case Format.ts:
                case Format.vob:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreCustomPreset(Format format)
        {
            switch (format)
            {
                case Format._3gp:
                case Format.mpeg:
                case Format.ts:
                case Format.vob:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreCustomAudioBps(Format format)
        {
            switch (format)
            {
                case Format._3gp:
                case Format.vob:
                case Format.ts:
                case Format.mpeg:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreCustomResolution(Format format)
        {
            switch (format)
            {
                case Format._3gp:
                case Format.mpeg:
                case Format.ts:
                case Format.vob:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreCustomFps(Format format)
        {
            switch (format)
            {
                case Format._3gp:
                case Format.mpeg:
                case Format.ts:
                case Format.vob:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreRemoveAudio(Format format)
        {
            switch (format)
            {
                case Format.vob:
                case Format.ts:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IgnoreCustomCrfQuality(Format format)
        {
            switch (format)
            {
                case Format.vob:
                case Format._3gp:
                case Format.ts:
                    return true;
                default:
                    break;
            }

            return false;
        }
    }
}
