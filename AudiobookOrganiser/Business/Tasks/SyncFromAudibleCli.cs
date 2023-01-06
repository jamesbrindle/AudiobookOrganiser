using AudiobookOrganiser.Helpers;
using AudiobookOrganiser.Models;
using FfMpeg;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Extensions;
using static FfMpeg.Extensions.AaxActivationClient;
using ATL;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class SyncFromAudibleCli
    {
        private static readonly string _libraryExportName = "audible-library.json";
        private static readonly string _libraryLastDownloadName = "audible-library-last-download.txt";
        private static string _libraryExportPath = string.Empty;

        public static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\nSyncing with audible-cli...\n");

            if (!Directory.Exists(Program.AudibleCliSyncPath))
                Directory.CreateDirectory(Program.AudibleCliSyncPath);

            _libraryExportPath = Path.Combine(Program.AudibleCliSyncPath, _libraryExportName);

            try
            {
                ExportLibrary();
            }
            catch
            {
                return;
            }

            if (!File.Exists(_libraryExportPath))
                return;

            try
            {
                DownloadBooks();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Thread.Sleep(5000);
                return;
            }

            try
            {
                ConvertToM4bAndAlterMetaData();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Thread.Sleep(5000);
                return;
            }
        }

        private static void ExportLibrary()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Exporing library...");

            ProcessHelper.ExecuteProcessAndReadStdOut(
                "audible.exe",
                out string _,
                $"library export --output \"{_libraryExportPath}\" --format json",
                throwOnError: true);
        }

        private static void DownloadBooks()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Downloading books...");
            var lastLibraryDownloadDate = GetLastLibraryDownloadDate();
            string fromDate = GetLastLibraryDownloadDate() == null
                ? null
                : ((DateTime)lastLibraryDownloadDate).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            string command =
                $"download " +
                $"--all " +
                $"--output-dir \"{Program.AudibleCliSyncPath}\" " +
                $"--aax-fallback " +
                $"--quality best " +
                $"--pdf " +
                $"-y " +
                $"--timeout 0 " +
                $"--ignore-errors " +
                (fromDate == null
                    ? ""
                    : $"--start-date {fromDate}");

            ProcessHelper.ExecuteProcessAndReadStdOut(
                "audible.exe",
                out string _,
                command,
                timeoutSeconds: 14400, // 4 hours
                throwOnError: true);

            WriteLastDownloadDate();
        }

        private static DateTime? GetLastLibraryDownloadDate()
        {
            string libraryLastDownloadPath = Path.Combine(Program.AudibleCliSyncPath, _libraryLastDownloadName);

            if (!File.Exists(libraryLastDownloadPath))
                return null;

            string fileContents = File.ReadAllText(libraryLastDownloadPath)?
                                       .Replace(" ", "")?.Replace("\r", "")?
                                       .Replace("\n", "")?
                                       .Trim();

            if (!string.IsNullOrEmpty(fileContents))
            {
                if (DateTime.TryParseExact(
                    fileContents,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime result))
                {
                    return result;
                }
            }

            return null;
        }

        private static void WriteLastDownloadDate()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Loggin last sync date...");

            string libraryLastDownloadPath = Path.Combine(Program.AudibleCliSyncPath, _libraryLastDownloadName);

            File.WriteAllText(
                libraryLastDownloadPath,
                DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private static void ConvertToM4bAndAlterMetaData()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Converting books to M4b...");

            var audibleLibrary = JsonConvert.DeserializeObject<AudibleLibrary.Property[]>
                (File.ReadAllText(_libraryExportPath));

            if (audibleLibrary != null &&
                audibleLibrary != null)
            {
                string convertedPath = Path.Combine(Program.AudibleCliSyncPath, "Converted");

                if (!Directory.Exists(convertedPath))
                    Directory.CreateDirectory(convertedPath);

                foreach (var file in Directory.GetFiles(Program.AudibleCliSyncPath))
                {
                    if (Path.GetFileName(file) == "audible-library-last-download.txt")
                        continue;
                    else if (Path.GetFileName(file) == "audible-library.json")
                        continue;
                    else if (!(Path.GetExtension(file).ToLower() == ".aa" ||
                        Path.GetExtension(file).ToLower() == ".aax" ||
                        Path.GetExtension(file).ToLower() == ".aaxc"))
                    {
                        continue;
                    }
                    else
                    {
                        string newPath = Path.Combine(convertedPath, Path.GetFileNameWithoutExtension(file) + ".m4b");

                        if (!File.Exists(newPath))
                        {
                            var cancelTokenSource = new CancellationTokenSource();
                            var cancelToken = cancelTokenSource.Token;
                            var conversionOptions = new ConversionOptions
                            {
                                Format = FfMpeg.Enums.Format.m4b,
                                HideBanner = true,
                                Copy = true,
                                HWAccelOutputFormatCopy = true,
                                Codec = FfMpeg.Enums.Codec.copy,
                                Overwrite = true
                            };

                            if (Path.GetExtension(file).ToLower() == ".aax") // Audible file 
                            {
                                conversionOptions.ActivationBytes = GetActivationBytes(file, out string _);
                                conversionOptions.AudibleKey = null;
                                conversionOptions.AudibleIv = null;
                            }
                            else if (Path.GetExtension(file).ToLower() == ".aaxc") // Audible file
                            {
                                var keyAndIv = GetAudibleKeyAndIv(Path.ChangeExtension(file, ".voucher"), out string _);

                                conversionOptions.ActivationBytes = null;
                                conversionOptions.AudibleKey = keyAndIv.Key;
                                conversionOptions.AudibleIv = keyAndIv.Iv;
                            }

                            var metaData = MetaDataReader.GetMetaData(file, false, false, null);
                            var audibleBookProperties = audibleLibrary.Where(m => m.asin == metaData.Asin)?.FirstOrDefault();

                            if (audibleBookProperties != null)
                            {
                                var engine = new Engine(Program.FfMpegPath);
                                Task.Run(() => engine.ConvertAsync(
                                    new MediaFile(file),
                                    new MediaFile(newPath),
                                    conversionOptions,
                                    cancelToken)).Wait();

                                Track track = new Track(newPath);

                                track.Composer = audibleBookProperties.narrators;
                                track.Genre = "Audiobook";                               
                                track.AdditionalFields["----:com.apple.iTunes:SERIES"] = audibleBookProperties.series_title;
                                track.AdditionalFields["----:com.apple.iTunes:SERIES-PART"] = audibleBookProperties.series_sequence;
                                track.AdditionalFields["----:com.apple.iTunes:NRT"] = audibleBookProperties.narrators;
                                track.AdditionalFields["----:com.apple.iTunes:book_genre"] = audibleBookProperties.narrators;
                                track.Group = $"Book {audibleBookProperties.series_sequence}, {audibleBookProperties.series_title}";
                                track.AdditionalFields["ASIN"] = audibleBookProperties.asin;

                                bool isReadonly = track.AdditionalFields.IsReadOnly;

                                track.Save();
                            }
                        }
                    }
                }
            }
        }

        private static void ComplimentMetaDataFile(string metaDataFilePath, Models.MetaData metaData)
        {
          
        }
    }
}
