using AudiobookOrganiser.Helpers.FfMpegWrapper;
using System;
using System.Collections.Concurrent;
using System.IO;
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

            Parallel.ForEach(m4bAudioFiles, new ParallelOptions { MaxDegreeOfParallelism = 2 }, audioFilePath =>
            {
                if (IsCorrupted(audioFilePath))
                    m_corruptedFiles.Add(audioFilePath);
            });

            if (m_corruptedFiles != null && m_corruptedFiles.Count > 0)
            {
                string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AudiobookOrganiser Corrupted Files.txt");

                try
                {
                    File.Delete(filepath);
                }
                catch { }

                File.WriteAllText(
                    filepath,
                    "The following files are found to be corrupt and need to be processed manually:\n\n" +
                    String.Join(Environment.NewLine, m_corruptedFiles));
            }
        }

        public static bool IsCorrupted(string filePath)
        {
            if (File.Exists(filePath) && !IsFileLocked(filePath))
            {
                var probe = new FfProbe(Program.FfProbePath);
                return probe.GetIsInvalidData(filePath);
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
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
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
