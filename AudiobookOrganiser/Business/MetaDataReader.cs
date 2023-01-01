using AudiobookOrganiser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Business
{
    internal class MetaDataReader
    {
        public static MetaData GetMetaData(string audioFile, bool tryParseMetaFromPath)
        {
            var metaData = new MetaData();

            metaData = GetMetaDataByTags(metaData, audioFile);

            if (tryParseMetaFromPath)
                metaData = GetMetaDataByParsingFilePath(metaData, audioFile);

            return metaData;
        }

        private static MetaData GetMetaDataByTags(MetaData metaData, string audioFilePath)
        {
            var mediaInfo = new MediaInfoLib.MediaInfo();
            mediaInfo.Open(audioFilePath);

            metaData.Author = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "author"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Author"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "performer"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Performer"))?.Replace("  ", " ").Trim();

            metaData.Narrator = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "narrated_by"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Narrated_By"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "composer"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Composer"))?.Replace("  ", " ").Trim();

            metaData.Title = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "album"))?.Replace("Unabridged", "")?.Replace("  ", " ").Trim().Replace("()", "").Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Album"))?.Replace("Unabridged", "").Replace("  ", " ").Trim().Replace("()", "").Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "title"))?.Replace("Unabridged", "").Replace("  ", " ").Trim().Replace("()", "").Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Title"))?.Replace("Unabridged", "").Replace("  ", " ").Trim().Replace("()", "").Trim();

            metaData.Year = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "year"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Year"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Recorded_Date"))?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Tagged_Date"))?.Replace("  ", " ").Trim();

            metaData.Series = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "SERIES"))?.Replace("  ", " ").Trim();
            metaData.SeriesPart = String.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "SERIES-PART"))?.Replace("  ", " ").Trim();

            var tagLibInfo = TagLib.File.Create(audioFilePath);

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = String.Join(", ", tagLibInfo.Tag.Artists)?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = String.Join(", ", tagLibInfo.Tag.AlbumArtists)?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = String.Join(", ", tagLibInfo.Tag.Composers)?.Replace("  ", " ").Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = tagLibInfo.Tag.Title?.Replace("Unabridged", "").Trim().Replace("()", "");

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = tagLibInfo.Tag.Year.ToString();

            try
            {
                if (!string.IsNullOrEmpty(metaData.Year))
                    if (metaData.Year.Contains("-") || metaData.Year.Contains("/"))
                        metaData.Year = DateTime.Parse(metaData.Year.Replace("UTC ", "")).Year.ToString();
            }
            catch { }

            return metaData;
        }

        private static MetaData GetMetaDataByParsingFilePath(MetaData metaData, string audioFilePath)
        {
            string[] directories = audioFilePath.Replace(Program.LibraryRoot, "").Split(Path.DirectorySeparatorChar);

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = directories[0];

            if (string.IsNullOrEmpty(metaData.Narrator))
            {
                string narrator = string.Empty;

                foreach (var directory in directories)
                {
                    if (directory.ToLower().Contains("narrated"))
                    {
                        int nIndexStart = directory.IndexOf("(Narrated - ");
                        int nIndexEnd = directory.LastIndexOf(")");

                        if (nIndexStart != -1 && nIndexEnd != -1)
                            narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart).Trim();

                        if (!string.IsNullOrEmpty(narrator))
                            break;
                    }
                }

                if (string.IsNullOrEmpty(narrator))
                {
                    foreach (var directory in directories)
                    {
                        if (directory.ToLower().Contains("narrated"))
                        {
                            int nIndexStart = directory.IndexOf("(Narrated By ");
                            int nIndexEnd = directory.LastIndexOf(")");

                            if (nIndexStart != -1 && nIndexEnd != -1)
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart).Trim();

                            if (!string.IsNullOrEmpty(narrator))
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(narrator))
                {
                    foreach (var directory in directories)
                    {
                        if (directory.ToLower().Contains("narrated"))
                        {
                            int nIndexStart = directory.IndexOf("(Narrated by ");
                            int nIndexEnd = directory.LastIndexOf(")");

                            if (nIndexStart != -1 && nIndexEnd != -1)
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart).Trim();

                            if (!string.IsNullOrEmpty(narrator))
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(narrator))
                {
                    foreach (var directory in directories)
                    {
                        if (directory.ToLower().Contains("narrated"))
                        {
                            int nIndexStart = directory.IndexOf("(Narrated by - ");
                            int nIndexEnd = directory.LastIndexOf(")");

                            if (nIndexStart != -1 && nIndexEnd != -1)
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart).Trim();

                            if (!string.IsNullOrEmpty(narrator))
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(narrator))
                {
                    foreach (var directory in directories)
                    {
                        if (directory.ToLower().Contains("narrated"))
                        {
                            int nIndexStart = directory.IndexOf("(Narrated By - ");
                            int nIndexEnd = directory.LastIndexOf(")");

                            if (nIndexStart != -1 && nIndexEnd != -1)
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart).Trim();

                            if (!string.IsNullOrEmpty(narrator))
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(narrator))
                {
                    foreach (var directory in directories)
                    {
                        if (directory.ToLower().Contains("narrated"))
                        {
                            int nIndexStart = directory.IndexOf("Narrated By");

                            if (nIndexStart != -1)
                                narrator = directory.Substring(nIndexStart).Trim();

                            if (!string.IsNullOrEmpty(narrator))
                                break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(narrator))
                    metaData.Narrator = narrator;
            }

            if (string.IsNullOrEmpty(metaData.Year))
            {
                var matches = Regex.Matches(audioFilePath, @"\d+");

                foreach (Match match in matches)
                {
                    bool isNumeric = int.TryParse(match.Value, out int year);

                    if (isNumeric && year > 1900 && year < 2200)
                        metaData.Year = year.ToString();

                    if (!string.IsNullOrEmpty(metaData.Year))
                        break;
                }

            }

            return metaData;
        }
    }
}
