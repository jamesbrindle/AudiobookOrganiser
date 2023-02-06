using System;
using System.Collections.Generic;
using System.IO;
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

            foreach (var audioFilePath in m4bAudioFiles)
                PerformCheckAndRewriteTags(audioFilePath);
        }

        private static void PerformCheckAndRewriteTags(string audioFilePath)
        {
            if (Path.GetExtension(audioFilePath).ToLower().In(".m4b"))
            {
                try
                {
                    var metaOnlyFromFile = MetaDataReader.GetMetaData(audioFilePath, false, false, false, false, null, true);
                    var metaFromOtherSources = MetaDataReader.GetMetaData(audioFilePath, true, true, true, false, forOverwriting: true);

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

                    if (metaOnlyFromFile.Series != metaFromOtherSources.Series)
                    {
                        changedList.Add("Series");
                        hasChanged = true;
                    }

                    if (metaOnlyFromFile.Overview != metaFromOtherSources.Overview)
                    {
                        changedList.Add("Overview");
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
                        MetaDataWriter.WriteMetaData(audioFilePath, metaFromOtherSources);
                    }
                }
                catch { }
            }
        }
    }
}
