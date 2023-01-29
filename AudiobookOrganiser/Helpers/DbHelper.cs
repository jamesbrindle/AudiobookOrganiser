using System.Data.SQLite;
using System.IO;

namespace AudiobookOrganiser.Helpers
{
    internal class DbHelper
    {
        public static void BackupDatabase(string sourceFile, string destFile)
        {
            try
            {
                var tempDbBytes = Helpers.SafeFileStream.GetBytes(sourceFile);
                File.WriteAllBytes(destFile, tempDbBytes);

                if (File.Exists(destFile))
                    return;
            }
            catch { }

            if (sourceFile.StartsWith("@\\"))
                sourceFile = @"\\" + sourceFile;

            using (SQLiteConnection source = new SQLiteConnection($"Data Source=\"{sourceFile}\""))
            using (SQLiteConnection destination = new SQLiteConnection($"Data Source=\"{destFile}\""))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }
        }
    }
}
