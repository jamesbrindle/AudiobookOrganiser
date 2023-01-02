using System;
using System.IO;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class DeleteEmptyDirectoriesFromLibrary
    {
        internal static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Out.WriteLine("\n\nDeleting leftover empty directories from library...");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nDeleting leftover empty directories from library...");

            var directories = Directory.GetDirectories(Program.CurrentLibraryRoot, "*.*", SearchOption.TopDirectoryOnly);
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
    }
}
