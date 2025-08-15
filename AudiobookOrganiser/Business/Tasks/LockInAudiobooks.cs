using AudiobookOrganiser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class LockInAudiobooks
    {
        internal static void LockInAll()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nLocking-in Audiobook Files...\n\n");

            var libraryPathFiles = Directory.GetFiles(
                new DirectoryInfo(Program.LibraryRootPaths[0]).Parent.FullName,
                "*.m4b",
                SearchOption.AllDirectories);

            var hashes = new ConcurrentBag<LockInModel>();

            Parallel.ForEach(
                libraryPathFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 8 },
                audioFilePath =>
            {
                Console.Write($"{Path.GetFileName(audioFilePath)}:\n");
                hashes.Add(new LockInModel(Path.GetFileName(audioFilePath), ComputeFileHash(audioFilePath)));
            });

            var audibleCliSyncConvertedFiles = Directory.GetFiles(
               Program.ConvertedAudiobooksPath,
               "*.m4b",
               SearchOption.AllDirectories);

            Parallel.ForEach(
                audibleCliSyncConvertedFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 8 },
                audioFilePath =>
            {
                Console.Write($"{Path.GetFileName(audioFilePath)}:\n");
                hashes.Add(new LockInModel(Path.GetFileName(audioFilePath), ComputeFileHash(audioFilePath)));
            });

            File.WriteAllText(Program.LockedInAudiobooksJsonPath, JsonConvert.SerializeObject(hashes, Formatting.Indented));
        }

        internal static void LockInAudiobookFiles(List<string> audioFilePaths)
        {
            var lockedInModel = GetLockedInModel();

            foreach (var audioFilePath in audioFilePaths)
                lockedInModel.Add(new LockInModel(Path.GetFileName(audioFilePath), ComputeFileHash(audioFilePath)));

            File.WriteAllText(Program.LockedInAudiobooksJsonPath, JsonConvert.SerializeObject(lockedInModel, Formatting.Indented));
        }

        internal static List<LockInModel> GetLockedInModel()
        {
            if (File.Exists(Program.LockedInAudiobooksJsonPath))
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<LockInModel>>(File.ReadAllText(Program.LockedInAudiobooksJsonPath));
                }
                catch { }
            }

            return new List<LockInModel>();
        }

        internal static string ComputeFileHash(string filePath)
        {
            HashAlgorithm algorithm = MD5.Create();

            using (algorithm)
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = algorithm.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
