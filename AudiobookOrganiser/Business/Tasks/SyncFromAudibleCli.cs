using AudiobookOrganiser.Helpers;
using AudiobookOrganiser.Helpers.FfMpegWrapper;
using AudiobookOrganiser.Helpers.FfMpegWrapper.Extensions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class SyncFromAudibleCli
    {
        public static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\nSyncing with audible-cli...\n");

            try
            {
                DownloadLibraryDataFromAudible();
            }
            catch
            {
                return;
            }

            if (!File.Exists(Program.LibraryExportJsonPath))
                return;

            try
            {
                DownloadBooksFromAudible();
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

            try
            {
                CopyBooksToLibraryFolder();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Thread.Sleep(5000);
                return;
            }
        }

        private static void DownloadLibraryDataFromAudible()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Getting library data from Audible...");

            ProcessHelper.ExecuteProcessAndReadStdOut(
                "audible.exe",
                out string _,
                $"library export --output \"{Program.LibraryExportJsonPath}\" --format json",
                throwOnError: true);
        }

        private static void DownloadBooksFromAudible()
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
            if (!File.Exists(Program.LibraryLastDownloadTxtPath))
                return null;

            string fileContents = File.ReadAllText(Program.LibraryLastDownloadTxtPath)?
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
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Logging last sync date...");

            File.WriteAllText(
                Program.LibraryLastDownloadTxtPath,
                DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private static void ConvertToM4bAndAlterMetaData()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Converting books to M4b...");

            var audibleLibrary = Program.AudiobookLibrary;
            if (audibleLibrary != null &&
                audibleLibrary != null)
            {
                foreach (var audioFile in Directory.GetFiles(
                    Program.AudibleCliSyncPath,
                    "*.*",
                    SearchOption.AllDirectories))
                {
                    if (Path.GetDirectoryName(audioFile) != Program.ConvertedAudiobooksPath)
                    {
                        if (Path.GetFileName(audioFile) == "audible-library-last-download.txt")
                            return;
                        else if (Path.GetFileName(audioFile) == "audible-library.json")
                            return;
                        else if (
                           !(Path.GetExtension(audioFile).ToLower() == ".aa" ||
                            Path.GetExtension(audioFile).ToLower() == ".aax" ||
                            Path.GetExtension(audioFile).ToLower() == ".aaxc"))
                        {
                            return;
                        }
                        else
                        {
                            string newPath = Path.Combine(
                                Program.ConvertedAudiobooksPath,
                                Path.GetFileNameWithoutExtension(audioFile) + ".m4b");

                            if (!File.Exists(newPath))
                            {
                                var cancelTokenSource = new CancellationTokenSource();
                                var cancelToken = cancelTokenSource.Token;
                                var conversionOptions = new ConversionOptions
                                {
                                    Format = Helpers.FfMpegWrapper.Enums.Format.m4b,
                                    HideBanner = true,
                                    Copy = true,
                                    HWAccelOutputFormatCopy = true,
                                    Codec = Helpers.FfMpegWrapper.Enums.Codec.copy,
                                    Overwrite = true
                                };

                                ResolveAudibleFileDecryptionCodes(audioFile, ref conversionOptions);

                                var metaData = MetaDataReader.GetMetaData(
                                    audioFile: audioFile,
                                    tryParseMetaFromPath: false,
                                    tryParseMetaFromReadarr: true,
                                    tryParseMetaFromOpenAudible: true,
                                    smallerFileName: false,
                                    audibleLibrary: audibleLibrary);

                                if (metaData != null)
                                {
                                    var engine = new FfMpeg(Program.FfMpegPath, Program.LibFDK_AAC_EncodingEnabled);
                                    Task.Run(() => engine.ConvertAsync(
                                        new MediaFile(audioFile),
                                        new MediaFile(newPath),
                                        conversionOptions,
                                        cancelToken)).Wait();

                                    MetaDataWriter.WriteMetaData(newPath, metaData);

                                    if (CheckForCorruptedFiles.IsCorrupted(newPath))
                                    {
                                        try
                                        {
                                            File.Delete(newPath);

                                            engine = new FfMpeg(Program.FfMpegPath, Program.LibFDK_AAC_EncodingEnabled);
                                            Task.Run(() => engine.ConvertAsync(
                                                new MediaFile(audioFile),
                                                new MediaFile(newPath),
                                                conversionOptions,
                                                cancelToken)).Wait();

                                            MetaDataWriter.WriteMetaData(newPath, metaData, true);
                                        }
                                        catch { }
                                    }

                                    CopyPdf(audioFile, newPath, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CopyPdf(
            string originalFilePath,
            string copyToFilePath,
            bool copyingToLibraryFolder)
        {
            try
            {
                string originalPdfName;
                string copyPdfName;

                if (!copyingToLibraryFolder)
                {
                    if (Path.GetFileNameWithoutExtension(originalFilePath).Contains("-AAX_"))
                    {
                        originalPdfName = Path.GetFileNameWithoutExtension(originalFilePath)
                                              .Substring(0, Path.GetFileNameWithoutExtension(originalFilePath)
                                              .IndexOf("-AAX_")) + ".pdf";

                        copyPdfName = Path.GetFileNameWithoutExtension(copyToFilePath)
                                          .Substring(0, Path.GetFileNameWithoutExtension(copyToFilePath)
                                          .IndexOf("-AAX_")) + ".pdf";
                    }
                    else
                    {
                        originalPdfName = Path.GetFileNameWithoutExtension(originalFilePath)
                                              .Substring(0, Path.GetFileNameWithoutExtension(originalFilePath)
                                              .IndexOf("-LC_")) + ".pdf";

                        copyPdfName = Path.GetFileNameWithoutExtension(copyToFilePath)
                                          .Substring(0, Path.GetFileNameWithoutExtension(copyToFilePath)
                                          .IndexOf("-LC_")) + ".pdf";
                    }
                }
                else
                {
                    if (Path.GetFileNameWithoutExtension(originalFilePath).Contains("-AAX_"))
                    {
                        originalPdfName = Path.GetFileNameWithoutExtension(originalFilePath)
                                              .Substring(0, Path.GetFileNameWithoutExtension(originalFilePath)
                                              .IndexOf("-AAX_")) + ".pdf";

                        copyPdfName = Path.GetFileNameWithoutExtension(copyToFilePath) + ".pdf";
                    }
                    else
                    {
                        originalPdfName = Path.GetFileNameWithoutExtension(originalFilePath)
                                              .Substring(0, Path.GetFileNameWithoutExtension(originalFilePath)
                                              .IndexOf("-LC_")) + ".pdf";

                        copyPdfName = Path.GetFileNameWithoutExtension(copyToFilePath) + ".pdf";
                    }
                }

                string originalPdfPath = Path.Combine(Path.GetDirectoryName(originalFilePath), originalPdfName);
                string copyPdfPath = Path.Combine(Path.GetDirectoryName(copyToFilePath), copyPdfName);

                if (File.Exists(originalPdfPath) &&
                    !File.Exists(copyPdfPath))
                {
                    File.Copy(originalPdfPath, copyPdfPath);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
            }
        }

        private static void ResolveAudibleFileDecryptionCodes(string filePath, ref ConversionOptions conversionOptions)
        {
            if (Path.GetExtension(filePath).ToLower() == ".aax") // Audible file 
            {
                conversionOptions.ActivationBytes = new AaxActivationClient(Program.FfProbePath).GetActivationBytes(filePath, out string _);
                conversionOptions.AudibleKey = null;
                conversionOptions.AudibleIv = null;
            }
            else if (Path.GetExtension(filePath).ToLower() == ".aaxc") // Audible file
            {
                var keyAndIv = new AaxActivationClient(Program.FfProbePath).GetAudibleKeyAndIv(Path.ChangeExtension(filePath, ".voucher"), out string _);

                conversionOptions.ActivationBytes = null;
                conversionOptions.AudibleKey = keyAndIv.Key;
                conversionOptions.AudibleIv = keyAndIv.Iv;
            }
        }

        private static void CopyBooksToLibraryFolder()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.White, "Copying to library folder...");

            var lockedInModel = LockInAudiobooks.GetLockedInModel();
            var audioFilesToLockIn = new ConcurrentBag<string>();

            Parallel.ForEach(
                Directory.GetFiles(
                    Program.ConvertedAudiobooksPath,
                    "*.m4b",
                    SearchOption.AllDirectories),
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                audioFile =>
            {
                string hash = LockInAudiobooks.ComputeFileHash(audioFile);
                if (lockedInModel.Any(m => m.Hash == hash))
                    return;

                try
                {
                    var metaData = MetaDataReader.GetMetaData(
                        audioFile: audioFile,
                        tryParseMetaFromPath: false,
                        tryParseMetaFromReadarr: true,
                        tryParseMetaFromOpenAudible: true,
                        smallerFileName: false,
                        audibleLibrary: Program.AudiobookLibrary);

                    string audioBookTitle = MetaDataReader.GetAudiobookTitle(audioFile, metaData);
                    string mp3Path = string.Empty;
                    string copyToPath = string.Empty;
                    bool fileAlreadyPresent = false;

                    foreach (string libraryPath in Program.LibraryRootPaths)
                    {
                        if (!string.IsNullOrEmpty(audioBookTitle))
                        {
                            string possiblePath = Path.Combine(libraryPath, audioBookTitle);
                            if (!string.IsNullOrEmpty(possiblePath))
                            {
                                mp3Path = Path.ChangeExtension(possiblePath, ".mp3");
                                if (!string.IsNullOrEmpty(mp3Path) && File.Exists(mp3Path))
                                    File.Delete(mp3Path);

                                if (File.Exists(possiblePath))
                                    fileAlreadyPresent = true;
                            }
                        }
                    }

                    foreach (string libraryPath in Program.LibraryRootPaths)
                    {
                        if (!string.IsNullOrEmpty(audioBookTitle))
                        {
                            string possiblePath = Path.Combine(libraryPath, audioBookTitle).Replace("...", "");
                            if (!string.IsNullOrEmpty(possiblePath))
                            {
                                mp3Path = Path.ChangeExtension(possiblePath, ".mp3");
                                if (!string.IsNullOrEmpty(mp3Path) && File.Exists(mp3Path))
                                    File.Delete(mp3Path);

                                if (File.Exists(possiblePath))
                                    fileAlreadyPresent = true;
                            }
                        }
                    }

                    if (!fileAlreadyPresent)
                    {
                        copyToPath = Path.Combine(
                            LibraryPathHelper.DetermineLibraryPath(metaData),
                            audioBookTitle).Replace("...", "");

                        mp3Path = Path.ChangeExtension(copyToPath, ".mp3");

                        if (File.Exists(mp3Path))
                            File.Delete(mp3Path);

                        if (!File.Exists(copyToPath))
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(copyToPath)))
                                Directory.CreateDirectory(Path.GetDirectoryName(copyToPath));

                            File.Copy(audioFile, copyToPath);
                        }

                        CopyPdf(audioFile, copyToPath, true);

                        audioFilesToLockIn.Add(copyToPath);
                    }

                    audioFilesToLockIn.Add(audioFile);
                }
                catch { }
            });

            LockInAudiobooks.LockInAudiobookFiles(audioFilesToLockIn.ToList());
        }
    }
}
