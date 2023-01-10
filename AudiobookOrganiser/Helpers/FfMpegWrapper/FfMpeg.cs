using AudiobookOrganiser.Helpers.FfMpegWrapper.Events;
using AudiobookOrganiser.Helpers.FfMpegWrapper.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// An FFmpeg - https://www.ffmpeg.org/ Wrapper for converting and manipulating audio and video and getting media meta data.
/// 
/// You'll need to put the ffmpeg executable somewhere (it's a too large to embed). When you initialise the FFmpeg wrapper engine you
/// need to specify the location, i.e:: 
///   
///    var ffmpegEngine = new Engine(@"c:\ffmpeg\ffmpeg.exe");
/// 
/// </summary>
namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    ///     An FFmpeg - https://www.ffmpeg.org/ Wrapper for converting and manipulating audio and video and getting media meta
    ///     data.
    ///     Provides methods for audio and video conversion and manipulation and getting media meta data.
    ///     You'll need to put the ffmpeg executable somewhere (it's a too large to embed). When you initialise the FFmpeg
    ///     wrapper engine you need to specify the location, i.e: var ffmpegEngine = new Engine(@"c:\ffmpeg\ffmpeg.exe");
    /// </summary>
    public sealed class FfMpeg
    {
        private readonly string _ffmpegPath;
        private readonly bool _libFDK_AAC_EncodingEnabled;

        /// <summary>
        ///     Instantiate the FFmpeg engine by providing either the name of the executable or the path to the executable. If only
        ///     file name is provided, it must be found through the PATH variables.
        /// </summary>
        /// <param name="ffmpegExePath">
        ///     The path to the ffmpeg executable, or the executable if it is defined in PATH. If left
        ///     empty, it will try to find "ffmpeg.exe" from PATH.
        /// </param>
        public FfMpeg(string ffmpegExePath = null, bool? libFDK_AAC_EncodingEnabled = null)
        {
            ffmpegExePath = ffmpegExePath ?? "ffmpeg.exe";
            _libFDK_AAC_EncodingEnabled = libFDK_AAC_EncodingEnabled ?? false;

            if (!ffmpegExePath.TryGetFullPath(out _ffmpegPath))
                throw new ArgumentException(ffmpegExePath,
                    "FFmpeg executable could not be found neither in PATH nor in directory.");
        }

        /// <summary>
        /// Define the event handler method for dealing with invidivual processed events (optional)
        /// (usually for displaying progress percetage, current frame rate encoding speed etc)
        /// </summary>
        public event EventHandler<ConversionProgressEventArgs> Progress;

        /// <summary>
        /// Define the event handler method for dealing with errors (optional)
        /// </summary>
        public event EventHandler<ConversionErrorEventArgs> Error;

        /// <summary>
        /// Define the event handler method that is called upon completion (optional)
        /// </summary>
        public event EventHandler<ConversionCompleteEventArgs> Complete;

        /// <summary>
        /// Define the event handler method for dealing with incoming data (optional)
        /// </summary>
        public event EventHandler<ConversionDataEventArgs> Data;

        /// <summary>
        /// Get basic meta data of the media file
        /// </summary>
        /// <param name="mediaFile">Media file to get meta data for</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Basic meta data object</returns>
        public MetaData GetMetaData(
            MediaFile mediaFile,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await GetMetaDataAsync(mediaFile, cancellationToken)).Result;
        }

        /// <summary>
        /// Get basic meta data of the media file
        /// </summary>
        /// <param name="mediaFilePath">Media file to get meta data for</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process</param>
        /// <returns>Basic meta data object</returns>
        public async Task<MetaData> GetMetaDataAsync(
            MediaFile mediaFile,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.GetMetaData,
                InputFile = mediaFile,
                LibFDK_AAC_EncodingEnabled = _libFDK_AAC_EncodingEnabled
            };

            if (mediaFile.MetaData == null)
            {
                mediaFile.MetaData = new MetaData();
                mediaFile.MetaData.Tags = new System.Collections.Generic.Dictionary<string, string>();
            }

            try
            {
                string tempFilename = Path.GetTempFileName() + ".txt";
                ProcessExtensions.ExecuteProcessAndReadStdOut(
                    _ffmpegPath,
                    $"-i \"{mediaFile.FileInfo.FullName}\" -f ffmetadata \"{tempFilename}\" -y");

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

        /// <summary>
        /// Set basic meta data of the media file. Note FFMpeg won't set meta data in place, you have to output a seperate file. If you wish to replace, you need to perform a 'delete' and 'move'.
        /// </summary>
        /// <param name="input">Input file to set meta data for</param>
        /// <param name="output">Output file concatonated with given meta data</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Media file object</returns>
        public MediaFile SetMetaData(
              MediaFile input,
              MediaFile output,
              SetMetaDataOptions options,
              CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await SetMetaDataAsync(input, output, options, cancellationToken)).Result;
        }

        /// <summary>
        /// Set basic meta data of the media file. Note FFMpeg won't set meta data in place, you have to output a seperate file. If you wish to replace, you need to perform a 'delete' and 'move'.
        /// </summary>
        /// <param name="input">Input file to set meta data for</param>
        /// <param name="output">Output file concatonated with given meta data</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Media file object</returns>
        public async Task<MediaFile> SetMetaDataAsync(
           MediaFile input,
           MediaFile output,
           SetMetaDataOptions options,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.SetMetaData,
                InputFile = input,
                OutputFile = output,
                SetMetaDatOptions = options,
                LibFDK_AAC_EncodingEnabled = _libFDK_AAC_EncodingEnabled
            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        /// <summary>
        /// Get a thumnail of the media. For audio file this gives you the cover art, for video files this give you an image at the 1 second point. (Use GetThumbnailOptions to override this).
        /// </summary>
        /// <param name="input">Input file to get the thumnail for</param>
        /// <param name="input">Output file image</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Media file object</returns>
        public MediaFile GetThumbnail(
            MediaFile input,
            MediaFile output,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await GetThumbnailAsync(input, output, cancellationToken)).Result;
        }

        /// <summary>
        /// Get a thumnail of the media. For audio file this gives you the cover art, for video files this give you an image at the 1 second point. (Use GetThumbnailOptions to override this).
        /// </summary>
        /// <param name="input">Input file to get the thumnail for</param>
        /// <param name="input">Output file image</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Media file object</returns>
        public async Task<MediaFile> GetThumbnailAsync(
            MediaFile input,
            MediaFile output,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetThumbnailAsync(
                input,
                output,
                new GetThumbnailOptions
                {
                    HideBanner = true,
                },
                cancellationToken);
        }

        /// <summary>
        /// Get a thumnail of the media. For audio file this gives you the cover art, for video files you set the 'seak' paramter in the 'GetThumnailOptions'.
        /// </summary>
        /// <param name="input">Input file to get the thumnail for</param>
        /// <param name="output">Output file image</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Media file object</returns>
        public MediaFile GetThumbnail(
            MediaFile input,
            MediaFile output,
            GetThumbnailOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await GetThumbnailAsync(input, output, options, cancellationToken)).Result;
        }

        /// <summary>
        /// Get a thumnail of the media. For audio file this gives you the cover art, for video files you set the 'seak' paramter in the 'GetThumnailOptions'.
        /// </summary>
        /// <param name="input">Input file to get the thumnail for</param>
        /// <param name="output">Output file image</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Media file object</returns>
        public async Task<MediaFile> GetThumbnailAsync(
            MediaFile input,
            MediaFile output,
            GetThumbnailOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.GetThumbnail,
                InputFile = input,
                OutputFile = output,
                GetThumbnailOptions = options,
                LibFDK_AAC_EncodingEnabled = _libFDK_AAC_EncodingEnabled
            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        /// <summary>
        /// Converts a media file from one format (or encoding) to another. This can even include video to audio, or less commonly, audio to video. This will 
        /// use default options, for more granually control, include a 'ConversionOptions' object.
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Meda file object</returns>
        public MediaFile Convert(
            MediaFile input,
            MediaFile output,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await ConvertAsync(input, output, cancellationToken)).Result;
        }

        /// <summary>
        /// Converts a media file from one format (or encoding) to another. This can even include video to audio, or less commonly, audio to video. This will 
        /// use default options, for more granually control, include a 'ConversionOptions' object.
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Meda file object</returns>
        public async Task<MediaFile> ConvertAsync(
            MediaFile input,
            MediaFile output,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ConvertAsync(input, output, cancellationToken);
        }

        /// <summary>
        /// Converts a media file from one format (or encoding) to another. This can even include video to audio, or less commonly, audio to video.
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to convert</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Meda file object</returns>
        public MediaFile Convert(
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await ConvertAsync(input, output, options, cancellationToken)).Result;
        }

        /// <summary>
        /// Converts a media file from one format (or encoding) to another. This can even include video to audio, or less commonly, audio to video.
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to convert</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Meda file object</returns>
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
                ConversionOptions = options,
                LibFDK_AAC_EncodingEnabled = _libFDK_AAC_EncodingEnabled
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

        /// <summary>
        /// Concatonate multiple media files into one. Good for, for example, converting multiple MP3s to a single M4a or M4b (chapter meta data will be automatically included).
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to concatenate</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Meda file object</returns>
        public MediaFile Concatenate(
            MediaFile[] input,
            MediaFile output,
            ConcatenationOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await ConcatenateAsync(input, output, options, cancellationToken)).Result;
        }

        /// <summary>
        /// Concatonate multiple media files into one. Good for, for example, converting multiple MP3s to a single M4a or M4b (chapter meta data will be automatically included).
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to concatenate</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Meda file object</returns>
        public async Task<MediaFile> ConcatenateAsync(
            MediaFile[] input,
            MediaFile output,
            ConcatenationOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.Concatenate,
                InputFiles = input,
                OutputFile = output,
                ConcatenationOptions = options,
                LibFDK_AAC_EncodingEnabled = _libFDK_AAC_EncodingEnabled,
                ConversionOptions = new ConversionOptions
                {
                    AudioBitRate = options.AudioBitRate
                }
            };

            await ExecuteAsync(parameters, cancellationToken);
            return parameters.OutputFile;
        }

        /// <summary>
        /// Combine / merege an audio and video file into one (this is different to 'concatonate).
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to concatenate</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Meda file object</returns>
        public MediaFile Combine(
           MediaFile[] input,
           MediaFile output,
           CombineOptions options,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await CombineAsync(input, output, options, cancellationToken)).Result;
        }

        /// <summary>
        /// Combine / merge an audio and video file into one (this is different to 'concatonate).
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to concatenate</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        /// <returns>Meda file object</returns>
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

        /// <summary>
        /// Adds an audio steam to media if one doesn't exist
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to concatenate</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        /// <returns>Meda file object</returns>
        public MediaFile EnsureAudioStream(
           MediaFile input,
           MediaFile output,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(async () => await EnsureAudioStreamAsync(input, output, cancellationToken)).Result;
        }

        /// <summary>
        /// Adds an audio steam to media if one doesn't exist
        /// </summary>
        /// <param name="input">Input media file</param>
        /// <param name="output">Output media file</param>
        /// <param name="options">Granualar options for how to concatenate</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. </param>
        /// <returns>Meda file object</returns>
        public async Task<MediaFile> EnsureAudioStreamAsync(
           MediaFile input,
           MediaFile output,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters
            {
                Task = FfMpegTask.EnsureAudioStream,
                InputFile = input,
                OutputFile = output,
                LibFDK_AAC_EncodingEnabled = _libFDK_AAC_EncodingEnabled
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

        /// <summary>
        /// Execute the ffMpeg process with completely custom arguments. Nothing is returned for progress / errors
        /// </summary>
        /// <param name="arguments">Ffmpeg arugments</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process. Note, this method is synchronous, so you'd have to define your own threading to take advantage of this</param>
        public void Execute(
            string arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Task.Run(async () => await ExecuteAsync(arguments, cancellationToken)).Wait();
        }

        /// <summary>
        /// Execute the ffMpeg process with completely custom arguments. Nothing is returned for progress / errors
        /// </summary>
        /// <param name="arguments">Ffmpeg arugments</param>
        /// <param name="cancellationToken">For cancelling the ffmpeg process.</param>
        public async Task ExecuteAsync(
            string arguments, CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new FfMpegParameters { CustomArguments = arguments };
            await ExecuteAsync(parameters, cancellationToken);
        }

        private void OnProgress(ConversionProgressEventArgs e)
        {
            Progress?.Invoke(this, e);
        }

        private void OnError(ConversionErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        private void OnComplete(ConversionCompleteEventArgs e)
        {
            Complete?.Invoke(this, e);
        }

        private void OnData(ConversionDataEventArgs e)
        {
            Data?.Invoke(this, e);
        }
    }
}