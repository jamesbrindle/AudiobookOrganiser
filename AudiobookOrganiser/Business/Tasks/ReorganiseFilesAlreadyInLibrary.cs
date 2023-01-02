using System;
using System.IO;
using System.Linq;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class ReorganiseFilesAlreadyInLibrary
    {
        internal static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "Renaming files already in library...\n\n");

            Program.OutputDirectoryName = Path.Combine(Program.CurrentLibraryRoot, Program.OutputDirectoryName);

            if (!File.Exists(Program.OutputDirectoryName))
                Directory.CreateDirectory(Program.OutputDirectoryName);

            var audioFiles = Directory.GetFiles(Program.CurrentLibraryRoot, "*.*", SearchOption.AllDirectories);
            foreach (var audioFilePath in audioFiles.Where(a => !a.ToLower().Contains(Program.OutputDirectoryName)))
            {
                if (audioFilePath.In(".mp3", ".m4a", ".m4b"))
                {
                    try
                    {
                        var metaData = MetaDataReader.GetMetaData(audioFilePath, true, false);

                        if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                        {
                            string newFilename = Path.Combine(
                                Program.OutputDirectoryName,
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
                                    Program.OutputDirectoryName,
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

                                    continue;
                                }
                            }

                            Console.Out.WriteLine(Path.GetFileNameWithoutExtension(newFilename));

                            if (!Directory.Exists(Path.GetDirectoryName(newFilename)))
                                Directory.CreateDirectory(Path.GetDirectoryName(newFilename));

                            if (!File.Exists(newFilename))
                                File.Move(audioFilePath, newFilename);
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
                else
                {
                    try
                    {
                        File.Delete(audioFilePath);
                    }
                    catch { }
                }
            }
        }
    }
}
