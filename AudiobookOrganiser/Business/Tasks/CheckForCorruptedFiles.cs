using AudiobookOrganiser.Helpers.FfMpegWrapper;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class CheckForCorruptedFiles
    {
        private static ConcurrentBag<string> m_corruptedFiles = new ConcurrentBag<string>();

        internal static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nChecking for corrupted files...\n\n");

            var m4bAudioFiles = Directory.GetFiles(
                new DirectoryInfo(Program.LibraryRootPaths[0]).Parent.FullName,
                "*.m4b",
                SearchOption.AllDirectories);

            Parallel.ForEach(
                m4bAudioFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                audioFilePath =>
            {
                if (IsCorrupted(audioFilePath))
                    m_corruptedFiles.Add(audioFilePath);
            });

            if (m_corruptedFiles != null && m_corruptedFiles.Count > 0)
            {
                string filepath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "AudiobookOrganiser Corrupted Files.txt");

                try
                {
                    File.Delete(filepath);
                }
                catch { }

                File.WriteAllText(
                    filepath,
                    "The following files are found to be corrupt and need to be processed manually:\n\n" +
                    string.Join(Environment.NewLine, m_corruptedFiles));
            }
        }

        public static bool IsCorrupted(string filePath)
        {
            if (File.Exists(filePath) && !IsFileLocked(filePath))
            {
                var probe = new FfProbe(Program.FfProbePath);
                bool valid = probe.GetIsInvalidData(filePath);

                // Maybe file path too long, copy to a temp path and try again

                if (!valid)
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp4");
                    File.Copy(filePath, tempPath);

                    Thread.Sleep(1000);
                    valid = probe.GetIsInvalidData(tempPath);
                    Thread.Sleep(1000);

                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch { }

                    return valid;
                }
            }

            return false;
        }

        public static bool IsFileLocked(string path)
        {
            // it must exist for it to be locked
            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                FileStream stream = null;

                try
                {
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    // The file is unavailable because it is:
                    // still being written to
                    // or being processed by another thread
                    // or does not exist (has already been processed)
                    return true;
                }
                finally
                {
                    stream?.Close();
                }
            }

            //file is not locked
            return false;
        }
    }
}
