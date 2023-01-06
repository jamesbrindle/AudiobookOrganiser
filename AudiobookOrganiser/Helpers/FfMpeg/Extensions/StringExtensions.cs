using System;
using System.IO;

namespace FfMpeg.Extensions
{
    public static class StringExtensions
    {
        public static bool TryGetFullPathIfFileExists(this string filePath, out string fullPath)
        {
            fullPath = string.Empty;

            if (!File.Exists(filePath)) return false;

            fullPath = Path.GetFullPath(filePath);
            return true;
        }

        public static bool TryGetFullPathIfPathEnvironmentExists(this string fileName, out string fullPath)
        {
            fullPath = string.Empty;
            string values = Environment.GetEnvironmentVariable("PATH");
            string[] pathElements = values?.Split(Path.PathSeparator);

            if (pathElements == null) return false;

            foreach (string path in pathElements)
            {
                string tempFullPath = Path.Combine(path, fileName);
                if (tempFullPath.TryGetFullPathIfFileExists(out fullPath))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetFullPath(this string file, out string fullPath)
        {
            if (file.TryGetFullPathIfFileExists(out fullPath)) return true;
            if (file.TryGetFullPathIfPathEnvironmentExists(out fullPath)) return true;

            return false;
        }
    }
}
