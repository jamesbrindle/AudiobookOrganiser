using System;
using System.IO;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class MoveFromRenamedFolderToLibraryFolder
    {
        internal static void Run(string libraryPath)
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nMoving from renamed temp folder to library...\n\n");

            string outputDirectory = Path.Combine(libraryPath, Program.OutputDirectoryName);

            var directories = Directory.GetDirectories(outputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directories)
            {
                Console.Out.WriteLine(Path.GetFileNameWithoutExtension(new DirectoryInfo(directory).Name));

                try
                {
                    Directory.Move(
                        directory,
                        Path.Combine(libraryPath, new DirectoryInfo(directory).Name));
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
