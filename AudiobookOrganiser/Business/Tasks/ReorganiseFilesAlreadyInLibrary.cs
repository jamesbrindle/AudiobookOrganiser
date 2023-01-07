using System;
using System.IO;
using System.Linq;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class ReorganiseFilesAlreadyInLibrary
    {
        internal static void Run(string libraryRootPath)
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "Renaming files already in library...\n\n");

            string outputDirectory = Path.Combine(libraryRootPath, Program.OutputDirectoryName);

            if (!File.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var audioFiles = Directory.GetFiles(libraryRootPath, "*.*", SearchOption.AllDirectories);
            foreach (var audioFilePath in audioFiles.Where(a => !a.ToLower().Contains(outputDirectory)))
            {
                if (Path.GetExtension(audioFilePath).ToLower().In(".mp3", ".m4a", ".m4b"))
                {
                    try
                    {
                        var metaData = MetaDataReader.GetMetaData(audioFilePath, true, false, libraryRootPath);

                        if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                        {
                            string newFilename = Path.Combine(
                                outputDirectory,
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
                                metaData = MetaDataReader.GetMetaData(audioFilePath, true, true, libraryRootPath);

                                newFilename = Path.Combine(
                                    outputDirectory,
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

            DeleteEmptyDirectoriesFromLibrary(libraryRootPath);
            MoveFromRenamedFolderToLibraryFolder(libraryRootPath);
        }

        private static void DeleteEmptyDirectoriesFromLibrary(string libraryRootPath)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Out.WriteLine("\n\nDeleting leftover empty directories from library...");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nDeleting leftover empty directories from library...");

            var directories = Directory.GetDirectories(libraryRootPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch { }
                }
            }
        }

        private static void MoveFromRenamedFolderToLibraryFolder(string libraryRootPath)
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nMoving from renamed temp folder to library...\n\n");

            string outputDirectory = Path.Combine(libraryRootPath, Program.OutputDirectoryName);

            var directories = Directory.GetDirectories(outputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directories)
            {
                Console.Out.WriteLine(Path.GetFileNameWithoutExtension(new DirectoryInfo(directory).Name));

                try
                {
                    Directory.Move(
                        directory,
                        Path.Combine(libraryRootPath, new DirectoryInfo(directory).Name));
                }
                catch
                {
                    ConsoleEx.WriteColouredLine(ConsoleColor.Red, $"Could not move file: {directory}");
                }
            }

            var renamedDirectories = Directory.GetDirectories(outputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var directory in renamedDirectories)
            {
                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                if (files.Length == 0)
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch { }
                }
            }

            var anyFilesInRenamed = Directory.GetFiles(outputDirectory, "*.*", SearchOption.AllDirectories);
            if (anyFilesInRenamed.Length == 0)
            {
                try
                {
                    Directory.Delete(outputDirectory, true);
                }
                catch { }
            }
        }
    }
}
