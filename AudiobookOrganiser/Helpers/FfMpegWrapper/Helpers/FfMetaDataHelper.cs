using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Helpers
{
    /// <summary>
    /// Helper methods for dealing with media file 'meta data
    /// </summary>
    public static class FfMetaDataHelper
    {

        /// <summary>
        /// Given a list of media file inputs, it will produce the meta data including the chapter data for the output media
        /// file.It needs to output to a text file for this, which you would use in the concatonation options
        /// </summary>
        /// <param name="inputMediaFiles">List of media file objects to get the meta data for</param>
        /// <param name="outputMetaDataTextFilePath">Path to text file type file output</param>
        /// <exception cref="ApplicationException"></exception>
        public static void CreateAndOutputMetaDataAndChapterFileForConcatenation(
            List<MediaFile> inputMediiaFiles,
            ref string outputMetaDataTextFilePath)
        {
            if (inputMediiaFiles[0].MetaData == null)
                throw new ApplicationException("Your media files need to include meta data");

            var sb = new StringBuilder();
            sb.AppendLine(";FFMETADATA1");

            if (
                inputMediiaFiles != null &&
                inputMediiaFiles.Count > 0 &&
                inputMediiaFiles[0].MetaData != null &&
                inputMediiaFiles[0].MetaData.Tags != null &&
                inputMediiaFiles[0].MetaData.Tags.Count() > 0)
            {
                string composer = null;
                string narrator = null;
                string series = null;
                string seriesPart = null;

                foreach (var tag in inputMediiaFiles[0].MetaData.Tags)
                {
                    if (tag.Key.ToLower() == "track")
                    {
                        string title = string.Empty;
                        string album = string.Empty;
                        string track = string.Empty;

                        if (inputMediiaFiles[0].MetaData.Tags.ContainsKey("title"))
                            title = inputMediiaFiles[0].MetaData.Tags["title"];

                        if (inputMediiaFiles[0].MetaData.Tags.ContainsKey("album"))
                            album = inputMediiaFiles[0].MetaData.Tags["album"];

                        if (inputMediiaFiles[0].MetaData.Tags.ContainsKey("track"))
                            track = inputMediiaFiles[0].MetaData.Tags["track"];

                        if (!string.IsNullOrEmpty(title))
                        {
                            sb.AppendLine("title=" + title);
                            sb.AppendLine("track=" + title);
                        }
                        else if (!string.IsNullOrEmpty(album))
                        {
                            sb.AppendLine("title=" + album);
                            sb.AppendLine("track=" + album);
                        }
                        else if (!string.IsNullOrEmpty(track))
                        {
                            sb.AppendLine("title=" + track);
                            sb.AppendLine("track=" + track);
                        }
                        else
                        {
                            sb.AppendLine("title=" + Path.GetFileNameWithoutExtension(inputMediiaFiles[0].FileInfo.FullName));
                            sb.AppendLine("track=" + Path.GetFileNameWithoutExtension(inputMediiaFiles[0].FileInfo.FullName));
                        }
                    }
                    else if (tag.Key.ToLower() != "title")
                    {
                        if (!tag.Key.ToLower().In("timebase", "start", "end", "title", "[chapter]"))
                        {
                            if (tag.Key.ToLower() == "narrated_by")
                                narrator = tag.Key.ToLower();

                            else if (tag.Key.ToLower() == "composer")
                                composer = tag.Key.ToLower();

                            else if (tag.Key.ToLower() == "series")
                                series = tag.Key.ToLower();

                            else if (tag.Key.ToLower() == "series-part")
                                seriesPart = tag.Key.ToLower();

                            sb.AppendLine(tag.Key + "=" + tag.Value);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(series) && !string.IsNullOrEmpty(seriesPart))
                    sb.Append($"grouping=Book {seriesPart}, {series}");
                else if (!string.IsNullOrEmpty(series) && string.IsNullOrEmpty(seriesPart))
                    sb.Append($"grouping={series}");

                if (string.IsNullOrEmpty(composer) && !string.IsNullOrEmpty(narrator))
                    sb.Append($"composer={series}");

                int i = 1;
                long currentStart = 0;
                long currentEnd = 0;

                foreach (var file in inputMediiaFiles)
                {
                    currentEnd += Convert.ToInt64(file.MetaData.Duration.TotalMilliseconds);

                    sb.AppendLine("[CHAPTER]");
                    sb.AppendLine("TIMEBASE=1/1000");
                    sb.AppendLine($"START={currentStart}");
                    sb.AppendLine($"END={currentEnd}");

                    string track = string.Empty;

                    if (
                        file.MetaData != null &&
                        file.MetaData.Tags != null &&
                        file.MetaData.Tags.Count() > 0 &&
                        file.MetaData.Tags.ContainsKey("track"))
                    {
                        track = $"{i}/{inputMediiaFiles.Count}: {file.MetaData.Tags["track"]}";
                    }
                    else
                    {
                        track = i + "/" + (inputMediiaFiles.Count);
                    }

                    sb.AppendLine("title=" + track);

                    currentStart = currentEnd + 1;
                    i++;
                }
            }

            string path;
            if (!string.IsNullOrEmpty(outputMetaDataTextFilePath))
                path = outputMetaDataTextFilePath;
            else
                path = Path.GetTempPath() + ".txt";

            try
            {
                File.WriteAllText(path, sb.ToString());
            }
            catch { }

            if (File.Exists(path))
                outputMetaDataTextFilePath = path;
            else
                outputMetaDataTextFilePath = null;
        }
    }
}
