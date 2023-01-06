using ATL;
using FfMpeg;
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

        internal static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "Converting and retagging existing MP3 to M4B...\n\n");

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
                            Console.WriteLine(Path.GetFileName(mp3MediaFile.FileInfo.FullName));

                            var conversionOptions = new ConversionOptions
                            {
                                Overwrite = false,
                                Codec = FfMpeg.Enums.Codec.aac,
                                Format = FfMpeg.Enums.Format.m4b,
                                RemoveVideo = true,
                                AudioBitRate = new Track(mp3AudioFile).Bitrate
                            };

                            var ffEngine = new Engine(Program.FfMpegPath);
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
    }
}
