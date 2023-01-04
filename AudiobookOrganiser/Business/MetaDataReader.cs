using AudiobookOrganiser.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AudiobookOrganiser.Business
{
    internal class MetaDataReader
    {
        static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        public static MetaData GetMetaData(string audioFile, bool tryParseMetaFromPath, bool small, string libraryRootPath = null)
        {
            var metaData = new MetaData();

            metaData = GetMetaDataByTags(metaData, audioFile, small);

            if (tryParseMetaFromPath && !string.IsNullOrEmpty(libraryRootPath))
                metaData = GetMetaDataByParsingFilePath(metaData, libraryRootPath, audioFile);

            if (!string.IsNullOrEmpty(metaData.Author))
                metaData.Author = new string(metaData.Author.Select(ch => _invalidFileNameChars.Contains(ch) ? '-' : ch).ToArray());

            if (!string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = new string(metaData.Narrator.Select(ch => _invalidFileNameChars.Contains(ch) ? '-' : ch).ToArray());

            if (!string.IsNullOrEmpty(metaData.Title))
                metaData.Title = new string(metaData.Title.Select(ch => _invalidFileNameChars.Contains(ch) ? '-' : ch).ToArray());

            if (!string.IsNullOrEmpty(metaData.Series))
                metaData.Series = new string(metaData.Series.Select(ch => _invalidFileNameChars.Contains(ch) ? '-' : ch).ToArray());

            if (!string.IsNullOrEmpty(metaData.SeriesPart))
                metaData.SeriesPart = new string(metaData.SeriesPart.Select(ch => _invalidFileNameChars.Contains(ch) ? '-' : ch).ToArray());

            if (!string.IsNullOrEmpty(metaData.Year))
                metaData.Year = new string(metaData.Year.Select(ch => _invalidFileNameChars.Contains(ch) ? '-' : ch).ToArray());

            return metaData;
        }

        private static MetaData GetMetaDataByTags(MetaData metaData, string audioFilePath, bool small)
        {
            var mediaInfo = new MediaInfoLib.MediaInfo();
            mediaInfo.Open(audioFilePath);

            /*
             * Author
             */

            metaData.Author = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "author"))?
                                    .Replace("  ", " ")
                                    .Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Author"))?
                                        .Replace("  ", " ")
                                        .Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "performer"))?
                                        .Replace("  ", " ")
                                        .Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Performer"))?
                                        .Replace("  ", " ")
                                        .Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Album_Performer"))?
                                        .Replace("  ", " ")
                                        .Trim();

            if (string.IsNullOrEmpty(metaData.Author))
                metaData.Author = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "album_Performer"))?
                                        .Replace("  ", " ")
                                        .Trim();
            /*
            * Narrator
            */

            metaData.Narrator = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "narrated_by"))?
                                      .Replace("  ", " ")
                                      .Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Narrated_By"))?
                                          .Replace("  ", " ")
                                          .Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "composer"))?
                                          .Replace("  ", " ")
                                          .Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Composer"))?
                                          .Replace("  ", " ")
                                          .Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "nrt"))?
                                          .Replace("  ", " ")
                                          .Trim();

            if (string.IsNullOrEmpty(metaData.Narrator))
                metaData.Narrator = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "NRT"))?
                                          .Replace("  ", " ")
                                          .Trim();

            /*
             * Title
             */

            metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "track_name"))?.Replace("Unabridged", "")?
                                   .Replace("  ", " ")
                                   .Replace("()", "")
                                   .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Track_Name"))?.Replace("Unabridged", "")?
                                       .Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "track"))?.Replace("Unabridged", "")?
                                       .Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Track"))?.Replace("Unabridged", "")?
                                       .Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "album"))?.Replace("Unabridged", "")?
                                       .Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Album"))?
                                       .Replace("Unabridged", "")
                                       .Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "title"))?
                                       .Replace("Unabridged", "").Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Title))
                metaData.Title = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Title"))?.Replace("Unabridged", "")
                                       .Replace("  ", " ")
                                       .Replace("()", "")
                                       .Trim();

            /*
             * Year
             */

            metaData.Year = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "year"))?
                                  .Replace("  ", " ")
                                  .Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Year"))?
                                      .Replace("  ", " ")
                                      .Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Original_Released_Date"))?
                                      .Replace("  ", " ")
                                      .Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Recorded_Date"))?
                                      .Replace("  ", " ")
                                      .Trim();

            if (string.IsNullOrEmpty(metaData.Year))
                metaData.Year = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Tagged_Date"))?
                                      .Replace("  ", " ")
                                      .Trim();

            /*
             * Series
             */

            metaData.Series = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "SERIES"))?
                                    .Replace("  ", " ")
                                    .Trim();

            metaData.SeriesPart = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "SERIES-PART"))?
                                        .Replace("  ", " ")
                                        .Trim();

            /*
             * Genre
             */

            metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "book_genre"))?
                                   .Replace("  ", " ")
                                   .Trim();

            if (string.IsNullOrEmpty(metaData.Genre))
                metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Book_Genre"))?
                                    .Replace("  ", " ")
                                    .Trim();

            if (string.IsNullOrEmpty(metaData.Genre))
                metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "genre"))?
                                    .Replace("  ", " ")
                                    .Trim();

            if (string.IsNullOrEmpty(metaData.Genre))
                metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Genre"))?
                                    .Replace("  ", " ")
                                    .Trim();

            try
            {
                // Fallback

                var tagLibInfo = TagLib.File.Create(audioFilePath);

                if (string.IsNullOrEmpty(metaData.Author))
                    metaData.Author = string.Join(", ", tagLibInfo.Tag.Performers)?
                                            .Replace("  ", " ")
                                            .Trim();

                if (string.IsNullOrEmpty(metaData.Author))
                    metaData.Author = string.Join(", ", tagLibInfo.Tag.AlbumArtists)?
                                            .Replace("  ", " ")
                                            .Trim();

                if (string.IsNullOrEmpty(metaData.Narrator))
                    metaData.Narrator = string.Join(", ", tagLibInfo.Tag.Composers)?
                                              .Replace("  ", " ")
                                              .Trim();

                if (string.IsNullOrEmpty(metaData.Title))
                    metaData.Title = tagLibInfo.Tag.Title?.Replace("Unabridged", "")
                                                          .Trim()
                                                          .Replace("()", "");

                if (string.IsNullOrEmpty(metaData.Year))
                    metaData.Year = tagLibInfo.Tag.Year.ToString();

                if (string.IsNullOrEmpty(metaData.Genre))
                    metaData.Genre = string.Join(", ", tagLibInfo.Tag.Genres)?
                                            .Replace("  ", " ")
                                            .Trim();
               
            }
            catch { }

            // Cleaning up

            try
            {
                if (!string.IsNullOrEmpty(metaData.Title) &&
                    metaData.Title.ToLower().Contains("book"))
                {
                    metaData.Title = Regex.Replace(metaData.Title, @", Book \d+", "")
                                          .Trim();
                }
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(metaData.Title) &&
                    metaData.Title.Contains($" (Narrated by {metaData.Narrator ?? ""})"))
                {
                    metaData.Title = metaData.Title.Replace($" (Narrated by {metaData.Narrator ?? ""})", "")
                                                   .Trim();
                }
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(metaData.Title) &&
                    metaData.Title.Contains($" (Narrated By {metaData.Narrator ?? ""})"))
                {
                    metaData.Title = metaData.Title.Replace($" (Narrated By {metaData.Narrator ?? ""})", "")
                                                   .Trim();
                }
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(metaData.Year))
                    if (metaData.Year.Contains("-") || metaData.Year.Contains("/"))
                        metaData.Year = DateTime.Parse(metaData.Year.Replace("UTC ", "")).Year.ToString();
            }
            catch { }

            if (!string.IsNullOrEmpty(metaData.Year))
            {
                if (metaData.Year == "1" || metaData.Year == "0")
                    metaData.Year = null;
            }

            if (small)
            {
                if (!string.IsNullOrEmpty(metaData.Author) && metaData.Author.Contains(","))
                {
                    string[] authorParts = metaData.Author.Split(',');
                    if (authorParts.Length > 0)
                        metaData.Author = authorParts[0].Trim();
                }

                if (!string.IsNullOrEmpty(metaData.Narrator) && metaData.Narrator.Contains(","))
                {
                    string[] narratorParts = metaData.Narrator.Split(',');
                    if (narratorParts.Length > 0)
                        metaData.Narrator = narratorParts[0].Trim();
                }
            }

            return metaData;
        }

        private static MetaData GetMetaDataByParsingFilePath(MetaData metaData, string libraryRootPath, string audioFilePath)
        {
            string[] directories = audioFilePath.Replace(libraryRootPath, "").Split(Path.DirectorySeparatorChar);

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
                            narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart)
                                                .Replace("(Narrated - ", "")
                                                .Replace(")", "")
                                                .Replace("  ", " ")
                                                .Trim();

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
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart)
                                                    .Replace("(Narrated By ", "")
                                                    .Replace(")", "")
                                                    .Replace("  ", " ")
                                                    .Trim();

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
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart).Replace("(Narrated by", "")
                                                    .Replace(")", "")
                                                    .Replace("  ", " ")
                                                    .Trim();

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
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart)
                                                    .Replace("(Narrated by - ", "")
                                                    .Replace(")", "")
                                                    .Replace("  ", " ")
                                                    .Trim();

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
                                narrator = directory.Substring(nIndexStart, nIndexEnd - nIndexStart)
                                                    .Replace("(Narrated By -", "")
                                                    .Replace(")", "")
                                                    .Replace("  ", " ")
                                                    .Trim();

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
                            int nIndexStart = directory.IndexOf("Narrated By ");
                            if (nIndexStart != -1)
                                narrator = directory.Substring(nIndexStart)
                                                    .Replace("Narrated By", "")
                                                    .Replace("  ", " ")
                                                    .Trim();

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
                            int nIndexStart = directory.IndexOf("Narrated by ");
                            if (nIndexStart != -1)
                                narrator = directory.Substring(nIndexStart)
                                                    .Replace("Narrated by", "")
                                                    .Replace("  ", " ")
                                                    .Trim();

                            if (!string.IsNullOrEmpty(narrator))
                                break;
                        }
                    }
                }
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
