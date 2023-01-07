using ATL;
using AudiobookOrganiser.Helpers;
using FfMpeg;
using FfMpeg.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class ConvertExistingMp3ToM4b
    {
        private static string _mainRootDir = null;
        private static double? _overrideTotalMediaSeconds = null;
        private static ProgressBar _progress = null;

        internal static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "Converting and retagging existing MP3 to M4B...\n");

            _mainRootDir = new DirectoryInfo(Program.LibraryRootPaths[0]).Parent.FullName;

            foreach (var mp3AudioFile in Directory.GetFiles(_mainRootDir, "*.mp3", SearchOption.AllDirectories))
            {
                var mp3MetaTags = MetaDataReader.GetMetaData(mp3AudioFile, true, false, Path.GetDirectoryName(mp3AudioFile));

                if (!string.IsNullOrEmpty(mp3MetaTags.Author) && !string.IsNullOrEmpty(mp3MetaTags.Title))
                {
                    var mp3MediaFile = new MediaFile(mp3AudioFile);
                    var m4bMediaFile = new MediaFile(
                        Path.Combine(
                            Path.GetDirectoryName(mp3AudioFile),
                            Path.GetFileNameWithoutExtension(mp3AudioFile) + ".m4b"));
                    try
                    {
                        if (!File.Exists(m4bMediaFile.FileInfo.FullName))
                        {
                            Console.Write($"\n{Path.GetFileName(mp3MediaFile.FileInfo.FullName)}   ");

                            var conversionOptions = new ConversionOptions
                            {
                                Overwrite = false,
                                Codec = FfMpeg.Enums.Codec.libfdk_aac,
                                Format = FfMpeg.Enums.Format.m4b,
                                RemoveVideo = true,
                                AudioBitRate = new Track(mp3AudioFile).Bitrate
                            };

                            _progress = new ProgressBar();

                            var ffEngine = new Engine(Program.FfMpegPath);
                            ffEngine.Progress += OnFfMpegProgress;
                            ffEngine.Error += OnFfMpegError;
                            ffEngine.Complete += OnFfMpegComplete;

                            var cancelTokenSource = new CancellationTokenSource();
                            var cancelToken = cancelTokenSource.Token;

                            Task.Run(async () => await
                                         ffEngine.ConvertAsync(mp3MediaFile, m4bMediaFile, conversionOptions, cancelToken))
                                                     .Wait();

                            if (File.Exists(m4bMediaFile.FileInfo.FullName) &&
                                new FileInfo(m4bMediaFile.FileInfo.FullName).Length > 10)
                            {
                                SetMetaDat(m4bMediaFile.FileInfo.FullName, mp3MetaTags);

                                try
                                {
                                    File.Delete(mp3AudioFile);
                                }
                                catch { }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ConsoleEx.WriteColouredLine(ConsoleColor.Red, $"Error: {e.Message}");

                        try
                        {
                            File.Delete(m4bMediaFile.FileInfo.FullName);
                        }
                        catch { }
                    }
                }
            }
        }

        private static void SetMetaDat(string filePath, Models.MetaData mp3MetaData)
        {
            Track track = new Track(filePath);

            track.Composer = mp3MetaData.Narrator;
            track.Artist = mp3MetaData.Author;
            track.AlbumArtist = mp3MetaData.Author;
            track.Genre = "Audiobook";

            if (!string.IsNullOrWhiteSpace(mp3MetaData.Series))
            {
                track.Group = $"Book {mp3MetaData.SeriesPart}, {mp3MetaData.Series}";
                track.AdditionalFields["----:com.apple.iTunes:SERIES"] = mp3MetaData.Series;
                track.AdditionalFields["----:com.apple.iTunes:SERIES-PART"] = mp3MetaData.SeriesPart;
            }

            track.Title = mp3MetaData.Title.Replace("(Unabridged)", "").Trim();
            track.AdditionalFields["----:com.apple.iTunes:NRT"] = mp3MetaData.Narrator;
            track.AdditionalFields["----:com.apple.iTunes:book_genre"] = mp3MetaData.Genre;
            track.AdditionalFields["ASIN"] = mp3MetaData.Asin;

            track.Save();
        }

        private static void OnFfMpegProgress(object sender, ConversionProgressEventArgs e)
        {
            double current = (double)e.ProcessedDuration.TotalSeconds;
            double total = ((double)e.TotalDuration.TotalSeconds) == 0 && _overrideTotalMediaSeconds != null
                ? (double)_overrideTotalMediaSeconds
                : (double)e.TotalDuration.TotalSeconds;

            double report = current / total;

            _progress.Report(report);
        }

        private static void OnFfMpegComplete(object sender, ConversionCompleteEventArgs e)
        {
            try
            {
                _progress.Dispose();
            }
            catch { }

            try
            {
                _progress = null;
            }
            catch { }
        }

        private static void OnFfMpegError(object sender, ConversionErrorEventArgs e)
        {
            try
            {
                _progress.Dispose();
            }
            catch { }

            try
            {
                _progress = null;
            }
            catch { }
        }
    }
}
