using AudiobookOrganiser.Helpers;
using System;
using System.IO;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class SyncFromOpenAudibleDownloads
    {
        internal static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\nSyncing from OpenAudibleDownloads...\n\n");

            var mp3AudioFiles = Directory.GetFiles(Program.OpenAudibleDownloadsFolderMp3Path, "*.*", SearchOption.AllDirectories);
            foreach (var audioFilePath in mp3AudioFiles)
                PerformSyncOnFile(audioFilePath);
        }

        private static void PerformSyncOnFile(string audioFilePath)
        {
            if (Path.GetExtension(audioFilePath).ToLower().In(".mp3", ".m4a", ".m4b"))
            {
                try
                {
                    var metaData = MetaDataReader.GetMetaData(audioFilePath, false, false);

                    if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                    {
                        string newFilename = Path.Combine(
                            LibraryPathHelper.DetermineLibraryPath(metaData),
                            metaData.Author,
                            metaData.Series,
                            (string.IsNullOrEmpty(metaData.SeriesPart)
                                    ? ""
                                    : metaData.SeriesPart + ". ") +
                            metaData.Title +
                                (string.IsNullOrEmpty(metaData.SeriesPart)
                                    ? ""
                                    : " (Book " + metaData.SeriesPart + ")") +
                                 " - " +
                                (string.IsNullOrEmpty(metaData.Year)
                                    ? ""
                                    : metaData.Year) +
                                (string.IsNullOrEmpty(metaData.Narrator)
                                    ? ""
                                    : (string.IsNullOrEmpty(metaData.Year)
                                        ? ""
                                        : " ") +
                                        "(Narrated - " + metaData.Narrator + ")")
                                + Path.GetExtension(audioFilePath));

                        if (newFilename.Length > 255)
                        {
                            metaData = MetaDataReader.GetMetaData(audioFilePath, true, true);

                            newFilename = Path.Combine(
                                LibraryPathHelper.DetermineLibraryPath(metaData),
                                metaData.Author,
                                metaData.Series,
                                (string.IsNullOrEmpty(metaData.SeriesPart)
                                        ? ""
                                        : metaData.SeriesPart + ". ") +
                                metaData.Title +
                                    (string.IsNullOrEmpty(metaData.SeriesPart)
                                        ? ""
                                        : " (Book " + metaData.SeriesPart + ")") +
                                     " - " +
                                    (string.IsNullOrEmpty(metaData.Year)
                                        ? ""
                                        : metaData.Year) +
                                    (string.IsNullOrEmpty(metaData.Narrator)
                                        ? ""
                                        : (string.IsNullOrEmpty(metaData.Year)
                                            ? ""
                                            : " ") +
                                            "(Narrated - " + metaData.Narrator + ")")
                                    + Path.GetExtension(audioFilePath));

                            if (newFilename.Length > 255)
                            {
                                ConsoleEx.WriteColouredLine(
                                    ConsoleColor.Red,
                                    $"Skipped: {Path.GetFileNameWithoutExtension(Path.GetFileName(audioFilePath))}: " +
                                    $"Resulting path too large.");

                                return;
                            }
                        }

                        Console.Out.WriteLine(Path.GetFileNameWithoutExtension(newFilename));

                        if (!Directory.Exists(Path.GetDirectoryName(newFilename)))
                            Directory.CreateDirectory(Path.GetDirectoryName(newFilename));

                        if (!File.Exists(newFilename) && !File.Exists(Path.ChangeExtension(newFilename, ".m4b")))
                            File.Copy(audioFilePath, newFilename);
                    }
                    else
                    {
                        ConsoleEx.WriteColouredLine(
                            ConsoleColor.Red,
                            $"Skipped {Path.GetFileNameWithoutExtension(Path.GetFileName(audioFilePath))}: " +
                            $"No title or author.");
                    }
                }
                catch (Exception e)
                {
                    ConsoleEx.WriteColouredLine(
                        ConsoleColor.Red,
                        $"Skipped {Path.GetFileNameWithoutExtension(Path.GetFileName(audioFilePath))}: " +
                        $"{e.Message}");
                }
            }
        }     
    }
}
