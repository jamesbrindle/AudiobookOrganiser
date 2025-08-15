using AudiobookOrganiser.Business.Tasks;
using AudiobookOrganiser.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using static System.Extensions;

namespace AudiobookOrganiser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Green, "--------------------------------------");
            ConsoleEx.WriteColouredLine(ConsoleColor.Green, "- Audiobook Organiser -");
            ConsoleEx.WriteColouredLine(ConsoleColor.Green, "--------------------------------------");

            CleanUpTempFiles();

            if (args.Contains("-audible-sync"))
            {
                SyncFromAudibleCli.Run();
                ConsoleEx.WriteColoured(ConsoleColor.Green, "\n\nDONE!");
            }
            else if (args.Contains("-update-tags-only"))
            {
                CheckAndRewriteTags.Run();
                LockInAudiobooks.LockInAll();
                ConsoleEx.WriteColoured(ConsoleColor.Green, "\n\nDONE!");
            }
            else if (args.Contains("-lock-in-only"))
            {
                LockInAudiobooks.LockInAll();
                ConsoleEx.WriteColoured(ConsoleColor.Green, "\n\nDONE!");
            }
            else if (args.Contains("-corruption-check-only"))
            {
                CheckForCorruptedFiles.Run();
                ConsoleEx.WriteColoured(ConsoleColor.Green, "\n\nDONE!");
            }
            else
            {
                foreach (var library in LibraryRootPaths)
                {
                    ConsoleEx.WriteColouredLine(ConsoleColor.Cyan, $"\n\nLIBRARY {library}\n\n");
                    ReorganiseFilesAlreadyInLibrary.Run(library);
                }

                SyncFromOpenAudibleDownloads.Run();
                ConvertExistingMp3ToM4b.Run();
                CheckAndRewriteTags.Run();
                CheckForCorruptedFiles.Run();

                ConsoleEx.WriteColoured(ConsoleColor.Green, "\n\nDONE!");

                // Give time to see output before closing console
                Thread.Sleep(3000);
            }

            Thread.Sleep(1000);
            CleanUpTempFiles();
        }

        internal static string[] LibraryRootPaths { get; }
            = ConfigurationManager.AppSettings["LibraryRootPaths"].Trim().Split(';');
        internal static string OpenAudibleDownloadsFolderMp3Path { get; }
            = ConfigurationManager.AppSettings["OpenAudibleDownloadsFolderMp3Path"].Trim();
        internal static string OpenAudibleDownloadsFolderM4bPath { get; }
            = ConfigurationManager.AppSettings["OpenAudibleDownloadsFolderM4bPath"].Trim();
        internal static string OpenAudibleBookListPath { get; }
            = ConfigurationManager.AppSettings["OpenAudibleBookListPath"].Trim();
        internal static bool SyncFromOpenAudibleDownloadsFolder { get; }
            = ConfigurationManager.AppSettings["SyncFromOpenAudibleDownloadsFolder"].Trim().ToLower().In("yes", "true", "1");
        internal static string OutputDirectoryName { get; }
            = ConfigurationManager.AppSettings["OutputDirectoryName"].Trim();
        internal static string FfMpegPath { get; }
            = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg.exe");
        internal static string FfProbePath { get; }
            = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffprobe.exe");
        internal static bool LibFDK_AAC_EncodingEnabled { get; }
           = ConfigurationManager.AppSettings["LibFDK_AAC_EncodingEnabled"].Trim().ToLower().In("yes", "true", "1");

        internal static string AudibleCliSyncPath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["AudibleCliSyncPath"].Trim();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        internal static AudibleLibrary.Property[] AudiobookLibrary
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<AudibleLibrary.Property[]>
                        (File.ReadAllText(LibraryExportJsonPath));
                }
                catch
                {
                    return null;
                }
            }
        }

        internal static string ConvertedAudiobooksPath
        {
            get
            {
                string path = Path.Combine(AudibleCliSyncPath, "Converted");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
        internal static string LibraryExportJsonPath { get; } = Path.Combine(AudibleCliSyncPath, "audible-library.json");
        internal static string LibraryLastDownloadTxtPath { get; } = Path.Combine(Program.AudibleCliSyncPath, "audible-library-last-download.txt");
        internal static string LockedInAudiobooksJsonPath { get; } = Path.Combine(Program.AudibleCliSyncPath, "locked-in.json");

        private static string _tempReadarrDbPath = null;

        internal static string ReadarrDbPath
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_tempReadarrDbPath) && File.Exists(_tempReadarrDbPath))
                        return _tempReadarrDbPath;

                    var dbFiles = Directory.GetFiles(ConfigurationManager.AppSettings["ReadarrAppDataRoute"].Trim(), "*.db", SearchOption.TopDirectoryOnly)
                                            .ToList();

                    dbFiles = dbFiles.OrderByDescending(m => new FileInfo(m).LastWriteTime).ToList();

                    var originalDbFilePath = dbFiles.Where(m => Path.GetFileName(m).ToLower().Contains("readarr"))
                                                    .FirstOrDefault();

                    _tempReadarrDbPath = originalDbFilePath;

                    string tempDbFileDir = Path.Combine(Path.GetTempPath(), "AudioBookOrganiser");

                    if (!Directory.Exists(tempDbFileDir))
                        Directory.CreateDirectory(tempDbFileDir);

                    string tempDbFilePath = Path.Combine(tempDbFileDir, Guid.NewGuid().ToString() + ".db");

                    Helpers.DbHelper.BackupDatabase(originalDbFilePath, tempDbFilePath);

                    _tempReadarrDbPath = tempDbFilePath;
                }
                catch
                { }

                if (string.IsNullOrEmpty(_tempReadarrDbPath))
                    _tempReadarrDbPath = @"C:\ProgramData\Readarr\readarr.db";

                return _tempReadarrDbPath;
            }
            set
            {
                _tempReadarrDbPath = value;
            }
        }

        private static void CleanUpTempFiles()
        {
            try
            {
                foreach (var file in Directory.GetFiles(Path.Combine(Path.GetTempPath(), "AudioBookOrganiser"), "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
            catch { }

            try
            {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "AudioBookOrganiser"), true);
            }
            catch { }
        }
    }
}
