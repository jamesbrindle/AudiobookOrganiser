using AudiobookOrganiser;
using AudiobookOrganiser.Helpers;
using FfMpeg.Events;
using FfMpeg.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FfMpeg
{
    public sealed class Engine
    {
        private readonly string _ffmpegPath;
        public Engine(string ffmpegPath = null)
        {
            ffmpegPath = ffmpegPath ?? Program.FfMpegPath;

            if (!ffmpegPath.TryGetFullPath(out _ffmpegPath))
                throw new ArgumentException(ffmpegPath, "FFmpeg executable could not be found neither in PATH nor in directory.");
        }

        public event EventHandler<ConversionProgressEventArgs> Progress;
        public event EventHandler<ConversionErrorEventArgs> Error;
        public event EventHandler<ConversionCompleteEventArgs> Complete;
        public event EventHandler<ConversionDataEventArgs> Data;

        public async Task<MetaData> GetMetaDataAsync(
            MediaFile mediaFile,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.GetMetaData,
                InputFile = mediaFile
            };

            if (mediaFile.MetaData == null)
            {
                mediaFile.MetaData = new MetaData();
                mediaFile.MetaData.Tags = new System.Collections.Generic.Dictionary<string, string>();
            }

            try
            {
                string tempFilename = Path.GetTempFileName() + ".txt";
                ProcessHelper.ExecuteProcessAndReadStdOut(
                    _ffmpegPath,
                    out string _,
                    $"-i \"{mediaFile.FileInfo.FullName}\" -f ffmetadata \"{tempFilename}\" -y",
                    throwOnError: false);

                if (File.Exists(tempFilename))
                {
                    string[] tagsRaw = File.ReadAllLines(tempFilename);

                    try
                    {
                        File.Delete(tempFilename);
                    }
                    catch { }

                    if (tagsRaw != null && tagsRaw.Length > 0)
                    {
                        foreach (var line in tagsRaw)
                        {
                            if (line.Contains("="))
                            {
                                string[] parts = line.Split('=');

                                try
                                {
                                    mediaFile.MetaData.Tags.Add(parts[0].Trim(), parts[1].Trim());
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.InputFile.MetaData;
        }

        public async Task<MediaFile> GetThumbnailAsync(
            MediaFile input,
            MediaFile output,
            CancellationToken cancellationToken = default(CancellationToken))
            => await GetThumbnailAsync(input, output, default(ConversionOptions), cancellationToken);

        public async Task<MediaFile> GetThumbnailAsync(
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.GetThumbnail,
                InputFile = input,
                OutputFile = output,
                ConversionOptions = options
            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        public async Task<MediaFile> ConvertAsync(
            MediaFile input,
            MediaFile output,
            CancellationToken cancellationToken = default(CancellationToken))
            => await ConvertAsync(input, output, default(ConversionOptions), cancellationToken);

        public async Task<MediaFile> ConvertAsync(
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.Convert,
                InputFile = input,
                OutputFile = output,
                ConversionOptions = options
            };

            await ExecuteAsync(parameters, cancellationToken);

            if (options.Overwrite && Path.GetExtension(input.FileInfo.FullName).ToLower() == Path.GetExtension(output.FileInfo.FullName).ToLower())
            {
                try
                {
                    File.Delete(input.FileInfo.FullName);
                    File.Move(output.FileInfo.FullName, input.FileInfo.FullName);
                }
                catch { }
            }

            return parameters.OutputFile;
        }

        public async Task<MediaFile> ConcatenationAsync(
            MediaFile[] input,
            MediaFile output,
            ConcatenationOptions options,
            CancellationToken cancellationToken = default(CancellationToken),
            string tempPath = null)
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.Concatenate,
                InputFiles = input,
                OutputFile = output,
                ConcatenationOptions = options,
                TempPath = tempPath,
                ConversionOptions = new ConversionOptions
                {
                    AudioBitRate = options.AudioBitRate
                }
            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        public async Task<MediaFile> CombineAsync(
            MediaFile[] input,
            MediaFile output,
            CombineOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.Combine,
                InputFiles = input,
                OutputFile = output,
                CombineOptions = options

            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        public async Task<MediaFile> EnsureAudioStreamAsync(
           MediaFile input,
           MediaFile output,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.EnsureAudioStream,
                InputFile = input,
                OutputFile = output
            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        private async Task ExecuteAsync(
            FfMpegParameters parameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            var ffmpegProcess = new FfMpegProcess(parameters, _ffmpegPath, cancellationToken);
            ffmpegProcess.Progress += OnProgress;
            ffmpegProcess.Completed += OnComplete;
            ffmpegProcess.Error += OnError;
            ffmpegProcess.Data += OnData;
            await ffmpegProcess.ExecuteAsync();

            ffmpegProcess.Progress -= OnProgress;
            ffmpegProcess.Completed -= OnComplete;
            ffmpegProcess.Error -= OnError;
            ffmpegProcess.Data -= OnData;
        }

        public async Task ExecuteAsync(
            string arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters { CustomArguments = arguments };
            await ExecuteAsync(parameters, cancellationToken);
        }

        private void OnProgress(ConversionProgressEventArgs e) => Progress?.Invoke(this, e);

        private void OnError(ConversionErrorEventArgs e) => Error?.Invoke(this, e);

        private void OnComplete(ConversionCompleteEventArgs e) => Complete?.Invoke(this, e);

        private void OnData(ConversionDataEventArgs e) => Data?.Invoke(this, e);
    }
}