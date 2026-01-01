using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Extensions;

namespace AudiobookOrganiser.Business.Tasks
{
    internal class CheckAndRewriteTags
    {
        internal static void Run()
        {
            ConsoleEx.WriteColouredLine(ConsoleColor.Yellow, "\n\nChecking and rewriting tags...\n\n");

            var m4bAudioFiles = Directory.GetFiles(
                new DirectoryInfo(Program.LibraryRootPaths[0]).Parent.FullName,
                "*.m4b",
                SearchOption.AllDirectories);

            //foreach (var audioFilePath in m4bAudioFiles)
            //    PerformCheckAndRewriteTags(audioFilePath);

            Parallel.ForEach(
                m4bAudioFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                audioFilePath =>
                {
                    PerformCheckAndRewriteTags(audioFilePath);
                });
        }

        private static void PerformCheckAndRewriteTags(string audioFilePath)
        {
            if (Path.GetExtension(audioFilePath).ToLower().In(".m4b"))
            {
                try
                {
                    var metaOnlyFromFile = MetaDataReader.GetMetaData(
                       audioFile: audioFilePath,
                       tryParseMetaFromPath: false,
                       tryParseMetaFromOpenAudible: false,
                       smallerFileName: false,
                       getProperGenre: true);

                    var metaFromOtherSources = MetaDataReader.GetMetaData(
                        audioFile: audioFilePath,
                        tryParseMetaFromPath: true,
                        tryParseMetaFromOpenAudible: true,
                        smallerFileName: false,
                        forOverwriting: true,
                        audibleLibrary: Program.AudiobookLibrary);

                    metaFromOtherSources.ProperGenre = "Audiobook";

                    bool hasChanged = false;
                    var changedList = new List<string>();

                    if (metaOnlyFromFile.Title != metaFromOtherSources.Title)
                    {
                        changedList.Add("Title");
                        hasChanged = true;
                    }

                    if (string.IsNullOrEmpty(metaOnlyFromFile.Album))
                    {
                        changedList.Add("Album");
                        hasChanged = true;
                    }

                    if (string.IsNullOrEmpty(metaOnlyFromFile.AlbumSort))
                    {
                        changedList.Add("Album Sort");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Author != metaFromOtherSources.Author)
                    {
                        changedList.Add("Author");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Narrator != metaFromOtherSources.Narrator)
                    {
                        changedList.Add("Narrator");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Overview != metaFromOtherSources.Overview)
                    {
                        if (metaOnlyFromFile.Overview
                            .Replace(" / ", "")
                            .Replace("\r", "")
                            .Replace("\n", "")
                            .Replace(" ", "")

                            !=

                            metaFromOtherSources.Overview
                            .Replace(" / ", "")
                            .Replace("\r", "")
                            .Replace("\n", "")
                            .Replace(" ", ""))
                        {
                            changedList.Add("Overview");
                            hasChanged = true;
                        }
                    }

                    if (metaOnlyFromFile.Copyright != metaFromOtherSources.Copyright || metaFromOtherSources.Copyright.Contains("&#169;"))
                    {
                        changedList.Add("Copyright");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Genre != metaFromOtherSources.Genre)
                    {
                        if (metaOnlyFromFile.Genre != "Audiobook")
                        {
                            changedList.Add("Genre");
                            hasChanged = true;
                        }
                    }

                    if (metaOnlyFromFile.ProperGenre != "Audiobook")
                    {
                        changedList.Add("Proper Genre");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Year != metaFromOtherSources.Year)
                    {
                        changedList.Add("Year");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Asin != metaFromOtherSources.Asin)
                    {
                        changedList.Add("Asin");
                        hasChanged = true;
                    }

                    if (hasChanged)
                    {
                        Console.Write($"{Path.GetFileName(audioFilePath)}: ({string.Join(", ", changedList)})\n");
                        MetaDataWriter.WriteMetaData(audioFilePath, metaFromOtherSources, iterationSleep: false);
                    }
                }
                catch { }
            }
        }
    }
}
