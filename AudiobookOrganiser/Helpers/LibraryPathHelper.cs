using AudiobookOrganiser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AudiobookOrganiser.Helpers
{
    internal class LibraryPathHelper
    {
        public static string DetermineLibraryPath(AudiobookMetaData metaData)
        {
            if (string.IsNullOrWhiteSpace(metaData.Genre))
                return Program.LibraryRootPaths.Where(l => l.Contains("Fiction") && !l.Contains("Non-Fiction")).FirstOrDefault();

            else if (metaData.Genre.StringContainsIn(
                "fiction",
                "mystery",
                "triller",
                "suspense",
                "crime",
                "romance",
                "classics",
                "fantasy"))
            {
                return Program.LibraryRootPaths.Where(l => l.Contains("\\Fiction")).FirstOrDefault();
            }

            if (metaData.Genre.StringContainsIn(
               "comedy",
               "stand-up",
               "stand up",
               "humour"))

            {
                return Program.LibraryRootPaths.Where(l => l.StringContainsIn("Comedy")).FirstOrDefault();
            }

            return Program.LibraryRootPaths.Where(l => l.StringContainsIn("Non-Fiction")).FirstOrDefault();
        }

        public static void MoveDirectory(string source, string target)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                }
            }
            Directory.Delete(source, true);
        }

        private class Folders
        {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target)
            {
                Source = source;
                Target = target;
            }
        }
    }
}
