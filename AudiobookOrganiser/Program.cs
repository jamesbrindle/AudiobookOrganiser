using AudiobookOrganiser.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiobookOrganiser
{
    internal class Program
    {
        internal static string LibraryRoot { get; set; } = @"D:\OneDrive\Books\Audio Books\Fiction\";
        internal static string OpenAudibleDownloadsPath { get; set; } = @"D:\Other Storage\OpenAudible\mp3";
        internal static string OutputRootPath { get; set; } = "__Renamed";

        static void Main()
        {
            RenameFilesAlreadyInLibrary();
            CopyFromOpenAudibleDownloadsToLibrary();
            DeleteEmptyDirectoriesFromLibrary();
            MoveFromRenamedFolderToLibraryFolder();
        }

        private static void RenameFilesAlreadyInLibrary()
        {
            OutputRootPath = Path.Combine(LibraryRoot, OutputRootPath);

            if (!File.Exists(OutputRootPath))
                Directory.CreateDirectory(OutputRootPath);

            var audioFiles = Directory.GetFiles(LibraryRoot, "*.*", SearchOption.AllDirectories);

            foreach (var audioFilePath in audioFiles)
            {
                if (Path.GetExtension(audioFilePath.ToLower()) == ".mp3" ||
                    Path.GetExtension(audioFilePath.ToLower()) == ".m4a" ||
                    Path.GetExtension(audioFilePath.ToLower()) == ".m4b")
                {
                    var metaData = MetaDataReader.GetMetaData(audioFilePath, true);

                    if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                    {
                        string newFilename = Path.Combine(
                            OutputRootPath,
                            metaData.Author,
                            metaData.Series,
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
                                    : " (Narrated - " + metaData.Narrator + ")")
                                + Path.GetExtension(audioFilePath));

                        string replacementFilename = Path.Combine(
                            LibraryRoot,
                            metaData.Author,
                            metaData.Series,
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
                                    : " (Narrated - " + metaData.Narrator + ")")
                                + Path.GetExtension(audioFilePath));

                        if (!File.Exists(replacementFilename))
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(newFilename)))
                                Directory.CreateDirectory(Path.GetDirectoryName(newFilename));

                            File.Copy(audioFilePath, newFilename);
                        }
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

        private static void CopyFromOpenAudibleDownloadsToLibrary()
        {
            var audioFiles = Directory.GetFiles(OpenAudibleDownloadsPath, "*.*", SearchOption.AllDirectories);

            foreach (var audioFilePath in audioFiles)
            {
                if (Path.GetExtension(audioFilePath.ToLower()) == ".mp3" ||
                    Path.GetExtension(audioFilePath.ToLower()) == ".m4a" ||
                    Path.GetExtension(audioFilePath.ToLower()) == ".m4b")
                {
                    var metaData = MetaDataReader.GetMetaData(audioFilePath, false);

                    if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                    {
                        string newFilename = Path.Combine(
                            LibraryRoot,
                            metaData.Author,
                            metaData.Series,
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
                                    : " (Narrated - " + metaData.Narrator + ")")
                                + Path.GetExtension(audioFilePath));

                        if (!File.Exists(newFilename))
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(newFilename)))
                                Directory.CreateDirectory(Path.GetDirectoryName(newFilename));

                            File.Move(audioFilePath, newFilename);
                        }
                    }
                }
            }
        }

        private static void DeleteEmptyDirectoriesFromLibrary()
        {
            var directories = Directory.GetDirectories(OpenAudibleDownloadsPath, "*.*", SearchOption.TopDirectoryOnly);

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
            var directories = Directory.GetDirectories(OutputRootPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var directory in directories)
                Directory.Move(directory, LibraryRoot);

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
