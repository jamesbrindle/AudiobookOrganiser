using AudiobookOrganiser.Business;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace AudiobookOrganiser
{
    internal class Program
    {
        internal static string[] LibraryRoots { get; set; } = {
            @"D:\OneDrive\Books\Audio Books\Fiction",
            @"D:\OneDrive\Books\Audio Books\Non-Fiction",
            @"D:\OneDrive\Books\Audio Books\Stand Up",
        };

        internal static string LibraryRoot { get; set; }
        internal static string OutputRootPath { get; set; } = "__Renamed";

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Out.WriteLine("--------------------------------------");
            Console.Out.WriteLine("- Audiobook Organiser");
            Console.Out.WriteLine("--------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;

            foreach (var library in LibraryRoots)
            {
                LibraryRoot = library;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Out.WriteLine("\n\nLIBRARY: " + library + "\n\n");
                Console.ForegroundColor = ConsoleColor.White;

                RenameFilesAlreadyInLibrary();
                DeleteEmptyDirectoriesFromLibrary();
                MoveFromRenamedFolderToLibraryFolder();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Out.WriteLine("\n\nDONE!");
            Console.ForegroundColor = ConsoleColor.White;

            Thread.Sleep(3000);
        }

        private static void RenameFilesAlreadyInLibrary()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Out.WriteLine("Renaming files already in library...\n\n");
            Console.ForegroundColor = ConsoleColor.White;

            OutputRootPath = Path.Combine(LibraryRoot, OutputRootPath);

            if (!File.Exists(OutputRootPath))
                Directory.CreateDirectory(OutputRootPath);

            var audioFiles = Directory.GetFiles(LibraryRoot, "*.*", SearchOption.AllDirectories);

            foreach (var audioFilePath in audioFiles.Where(a => !a.ToLower().Contains(OutputRootPath)))
            {
                if (Path.GetExtension(audioFilePath.ToLower()) == ".mp3" ||
                    Path.GetExtension(audioFilePath.ToLower()) == ".m4a" ||
                    Path.GetExtension(audioFilePath.ToLower()) == ".m4b")
                {
                    var metaData = MetaDataReader.GetMetaData(audioFilePath, true, false);

                    if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                    {
                        string newFilename = Path.Combine(
                            OutputRootPath,
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
                                OutputRootPath,
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
                                Console.Out.WriteLine("Skipped: " + Path.GetFileNameWithoutExtension(Path.GetFileName(audioFilePath)));
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Out.WriteLine("Skipped: " + Path.GetFileNameWithoutExtension(Path.GetFileName(audioFilePath)));
                        Console.ForegroundColor = ConsoleColor.White;
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

        private static void DeleteEmptyDirectoriesFromLibrary()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Out.WriteLine("\n\nDeleting leftover empty directories from library...");
            Console.ForegroundColor = ConsoleColor.White;

            var directories = Directory.GetDirectories(LibraryRoot, "*.*", SearchOption.TopDirectoryOnly);

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

        private static void MoveFromRenamedFolderToLibraryFolder()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Out.WriteLine("\n\nMoving from renamed temp folder to library...\n\n");
            Console.ForegroundColor = ConsoleColor.White;

            var directories = Directory.GetDirectories(OutputRootPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var directory in directories)
            {
                Console.Out.WriteLine(Path.GetFileNameWithoutExtension(new DirectoryInfo(directory).Name));

                try
                {
                    Directory.Move(directory, Path.Combine(LibraryRoot, new DirectoryInfo(directory).Name));
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Out.WriteLine("Could not move file: " + directory);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            var renamedDirectories = Directory.GetDirectories(OutputRootPath, "*.*", SearchOption.TopDirectoryOnly);

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

            var anyFilesInRenamed = Directory.GetFiles(OutputRootPath, "*.*", SearchOption.AllDirectories);

            if (anyFilesInRenamed.Length == 0)
            {
                try
                {
                    Directory.Delete(OutputRootPath, true);
                }
                catch { }
            }
        }
    }
}
