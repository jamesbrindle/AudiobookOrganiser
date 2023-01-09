using AudiobookOrganiser.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AudiobookOrganiser.Business
{
    internal class MetaDataReader
    {
        static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        public static AudiobookMetaData GetMetaData(
            string audioFile,
            bool tryParseMetaFromPath,
            bool tryParseMetaFromReadarr,
            bool tryParseMetaFromOpenAudible,
            bool small,
            string libraryRootPath = null,
            bool getProperGenre = false)
        {
            var metaData = new AudiobookMetaData();

            metaData = GetMetaDataByTags(metaData, audioFile, small, getProperGenre: getProperGenre);

            if (tryParseMetaFromReadarr)
            {
                try
                {
                    metaData = GetMetaFromReadarr(metaData, audioFile);
                }
                catch { }
            }

            if (tryParseMetaFromOpenAudible)
            {
                try
                {
                    metaData = GetMetaFromOpenAudible(metaData, audioFile);
                }
                catch { }
            }

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

            if (metaData.Author == null)
                metaData.Author = string.Empty;

            if (metaData.Title == null)
                metaData.Title = string.Empty;

            if (metaData.Series == null)
                metaData.Series = string.Empty;

            if (metaData.SeriesPart == null)
                metaData.SeriesPart = string.Empty;

            if (metaData.Narrator == null)
                metaData.Narrator = string.Empty;

            return metaData;
        }

        public static string GetAudiobookTitle(string audioFilePath)
        {
            string newFilename = Path.GetFileName(audioFilePath);

            try
            {
                var metaData = GetMetaData(audioFilePath, true, true, true, false, null);

                if (!string.IsNullOrEmpty(metaData.Author) && !string.IsNullOrEmpty(metaData.Title))
                {
                    newFilename =
                        (string.IsNullOrEmpty(metaData.Author) ? "" : (metaData.Author.Split(',')?[0].Trim() + "\\")) +
                        (string.IsNullOrEmpty(metaData.Series) ? "" : (metaData.Series + "\\")) +
                        (string.IsNullOrEmpty(metaData.SeriesPart)
                                ? ""
                                : metaData.SeriesPart + ". ") +
                            metaData.Title +
                                (string.IsNullOrEmpty(metaData.SeriesPart)
                                    ? ""
                                    : " (Book " + metaData.SeriesPart + ")") +
                                 " - " +
                                (string.IsNullOrEmpty(metaData.Year)
                                    ? ""
                                    : metaData.Year) +
                                (string.IsNullOrEmpty(metaData.Narrator)
                                    ? ""
                                    : (string.IsNullOrEmpty(metaData.Year)
                                        ? ""
                                        : " ") +
                                        "(Narrated - " + metaData.Narrator + ")")
                            + Path.GetExtension(audioFilePath);

                    if ((Path.GetDirectoryName(audioFilePath) + "\\" + newFilename).Length > 255)
                    {
                        metaData = GetMetaData(audioFilePath, true, true, true, true, null);

                        newFilename =
                            (string.IsNullOrEmpty(metaData.Author) ? "" : (metaData.Author.Split(',')?[0].Trim() + "\\")) +
                            (string.IsNullOrEmpty(metaData.Series) ? "" : (metaData.Series + "\\")) +
                            (string.IsNullOrEmpty(metaData.SeriesPart)
                                    ? ""
                                    : metaData.SeriesPart + ". ") +
                                metaData.Title +
                                    (string.IsNullOrEmpty(metaData.SeriesPart)
                                        ? ""
                                        : " (Book " + metaData.SeriesPart + ")") +
                                     " - " +
                                    (string.IsNullOrEmpty(metaData.Year)
                                        ? ""
                                        : metaData.Year) +
                                    (string.IsNullOrEmpty(metaData.Narrator)
                                        ? ""
                                        : (string.IsNullOrEmpty(metaData.Year)
                                            ? ""
                                            : " ") +
                                            "(Narrated - " + metaData.Narrator + ")")
                                + Path.GetExtension(audioFilePath);

                        if ((Path.GetDirectoryName(audioFilePath) + "\\" + newFilename).Length > 255)
                            newFilename = Path.GetFileName(audioFilePath);
                    }
                }
            }
            catch { }

            return newFilename;
        }

        private static AudiobookMetaData GetMetaDataByTags(
            AudiobookMetaData metaData,
            string audioFilePath,
            bool small,
            bool getProperGenre = false)
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

            if (string.IsNullOrEmpty(metaData.Series))
            {
                string grouping = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Grouping"))?
                                        .Replace("  ", " ")
                                        .Trim();

                if (!string.IsNullOrEmpty(grouping))
                {
                    var match = Regex.Match(grouping, @"Book \d+");
                    if (!match.Success)
                        match = Regex.Match(grouping, @"book \d+");

                    if (match.Success)
                    {
                        var book = match.Value;

                        var series = grouping.Replace(book, "").Replace(",", "").Trim();
                        var bookNo = book.Replace("Book", "").Replace("book", "").Trim();

                        if (!string.IsNullOrEmpty(series))
                            metaData.Series = series;

                        if (!string.IsNullOrEmpty(bookNo))
                            metaData.SeriesPart = bookNo;
                    }
                }
            }

            /*
             * Genre
             */

            if (string.IsNullOrEmpty(metaData.Genre))
                metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "book_genre"))?
                                       .Replace("  ", " ")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Genre))
                metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "BOOK_GENRE"))?
                                       .Replace("  ", " ")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Genre))
                metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Book_Genre"))?
                                    .Replace("  ", " ")
                                    .Trim();

            if (getProperGenre)
            {
                if (string.IsNullOrEmpty(metaData.Genre))
                    metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "genre"))?
                                        .Replace("  ", " ")
                                        .Trim();

                if (string.IsNullOrEmpty(metaData.Genre))
                    metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Genre"))?
                                        .Replace("  ", " ")
                                        .Trim();

                if (string.IsNullOrEmpty(metaData.Genre))
                    metaData.Genre = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "GENRE"))?
                                        .Replace("  ", " ")
                                        .Trim();
            }
            else
            {
                if (metaData.Genre != null && metaData.Genre == "Audiobook")
                    metaData.Genre = string.Empty;
            }

            /*
             * Asin
             */

            if (string.IsNullOrEmpty(metaData.Asin))
                metaData.Asin = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "CDEK"))?
                                    .Replace("  ", " ")
                                    .Trim();

            if (string.IsNullOrEmpty(metaData.Asin))
                metaData.Asin = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "cdek"))?
                                    .Replace("  ", " ")
                                    .Trim();

            if (string.IsNullOrEmpty(metaData.Asin))
                metaData.Asin = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "asin"))?
                                       .Replace("  ", " ")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Asin))
                metaData.Asin = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Asin"))?
                                       .Replace("  ", " ")
                                       .Trim();

            if (string.IsNullOrEmpty(metaData.Asin))
                metaData.Asin = string.Join(", ", mediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "ASIN"))?
                                       .Replace("  ", " ")
                                       .Trim();

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

            mediaInfo.Close();

            return metaData;
        }

        private static AudiobookMetaData GetMetaDataByParsingFilePath(AudiobookMetaData metaData, string libraryRootPath, string audioFilePath)
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

            if (string.IsNullOrEmpty(metaData.SeriesPart))
            {
                var match = Regex.Match(audioFilePath, @"Book \d+");
                if (match.Success)
                {
                    string bookNoStr = match.Value.Trim();
                    string bookNo = bookNoStr.Replace("Book ", "").Replace("book", "").Trim();

                    metaData.SeriesPart = bookNo;
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

        private static AudiobookMetaData GetMetaFromReadarr(
            AudiobookMetaData metaData,
            string audioFilePath)
        {
            string readarrRoot = null;

            try
            {
                readarrRoot = ConfigurationManager.AppSettings["ReadarrAppDataRoute"];
            }
            catch { }

            if (string.IsNullOrWhiteSpace(readarrRoot))
                readarrRoot = @"C:\ProgramData\Readarr";

            var dbFiles = Directory.GetFiles(readarrRoot, "*.db", SearchOption.TopDirectoryOnly).ToList();
            dbFiles = dbFiles.OrderByDescending(m => new FileInfo(m).LastWriteTime).ToList();

            var dbFile = dbFiles.Where(m => Path.GetFileName(m).ToLower().Contains("readarr")).FirstOrDefault();

            using (var connection = new SQLiteConnection($"Data Source=\"{dbFile}\""))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT
	                    b.ReleaseDate,
	                    b.Genres,
	                    e.Isbn13,
	                    e.Asin,
	                    e.Title,
	                    e.Overview,
	                    e.Format,
	                    e.Images,
	                    bf.Path,
	                   	am.Name AS Author,
	                    am.NameLastFirst AS AuthorLastFirst,
	                    s.Title AS Series,
	                    sbl.Position As SeriesPart
                    FROM Books b
	                    LEFT JOIN Editions e
		                    ON e.BookId = b.Id
	                    LEFT JOIN BookFiles bf
		                    ON bf.EditionId = e.Id
	                    LEFT JOIN SeriesBookLink sbl
		                    ON sbl.BookId = b.Id
	                    LEFT JOIN Series s
		                    ON s.Id = sbl.SeriesId
	                    LEFT JOIN AuthorMetadata am
		                    ON am.Id = b.AuthorMetadataId
                    WHERE (IsEbook = 0
                    AND     Format <> 'Paperback'
                    AND     b.Title LIKE '%" + metaData.Title + @"%'
                    AND   (am.Name LIKE '%" + metaData.Author + @"%') 
	                    OR am.NameLastFirst LIKE '%" + metaData.Author + @"%')
                    OR bf.Path = '" + audioFilePath + @"
                    ORDER BY bf.Path DESC
                    LIMIT 1'
                ";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            if (string.IsNullOrEmpty(metaData.Author))
                                metaData.Author = reader["Author"].ToString()?.Trim();

                            if (string.IsNullOrEmpty(metaData.Title))
                                metaData.Title = reader["Title"].ToString();

                            if (string.IsNullOrEmpty(metaData.Genre))
                                metaData.Genre = FormatJsonGenres(reader["Genres"].ToString())?.Trim();

                            if (metaData.Genre != null && metaData.Genre == "Audiobook")
                                metaData.Genre = string.Empty;

                            if (string.IsNullOrEmpty(metaData.Series))
                                metaData.Series = reader["Series"].ToString()?.Trim();

                            if (string.IsNullOrEmpty(metaData.SeriesPart))
                                metaData.SeriesPart = reader["SeriesPart"].ToString()?.Trim();

                            if (string.IsNullOrEmpty(metaData.Year))
                            {
                                try
                                {
                                    metaData.Year = DateTime.Parse(reader["ReleaseDate"].ToString()).ToString("yyyy");
                                }
                                catch { }
                            }

                            if (string.IsNullOrEmpty(metaData.Asin))
                                metaData.Asin = reader["Asin"].ToString()?.Trim();
                        }
                    }
                }
            }

            return metaData;
        }

        private static AudiobookMetaData GetMetaFromOpenAudible(
            AudiobookMetaData metaData,
            string audioFilePath)
        {
            string openAudibleFilename = Path.Combine(Path.GetFileNameWithoutExtension(audioFilePath));

            if (File.Exists(Program.OpenAudibleBookListPath))
            {
                OpenAudibleBook.Property book = null;

                book = JsonConvert.DeserializeObject<OpenAudibleBook.Property[]>(File.ReadAllText(Program.OpenAudibleBookListPath))
                    .Where(m => m.filename == openAudibleFilename).FirstOrDefault();

                if (book == null && !string.IsNullOrEmpty(metaData.Asin))
                {
                    book = JsonConvert.DeserializeObject<OpenAudibleBook.Property[]>(File.ReadAllText(Program.OpenAudibleBookListPath))
                    .Where(m => m.asin == metaData.Asin).FirstOrDefault();
                }

                if (book != null)
                {
                    if (string.IsNullOrEmpty(metaData.Author))
                        metaData.Author = book.author?.Trim();

                    if (string.IsNullOrEmpty(metaData.Narrator))
                        metaData.Narrator = book.narrated_by?.Trim();

                    if (string.IsNullOrEmpty(metaData.Title))
                        metaData.Title = book.title_short?.Trim();

                    if (string.IsNullOrEmpty(metaData.Genre))
                        metaData.Genre = book.genre?.Trim();

                    if (metaData.Genre != null && metaData.Genre == "Audiobook")
                        metaData.Genre = string.Empty;

                    if (string.IsNullOrEmpty(metaData.Series))
                        metaData.Series = book.series_name?.Trim();

                    if (string.IsNullOrEmpty(metaData.SeriesPart))
                        metaData.SeriesPart = book.series_sequence?.Trim();

                    if (string.IsNullOrEmpty(metaData.Year))
                    {
                        try
                        {
                            metaData.Year = DateTime.Parse(book.release_date).ToString("yyyy");
                        }
                        catch { }
                    }

                    if (string.IsNullOrEmpty(metaData.Asin))
                        metaData.Asin = book.asin?.Trim();
                }
            }

            return metaData;
        }

        private static string FormatJsonGenres(string genres)
        {
            genres = genres.Replace("[", "")
                           .Replace("]", "")
                           .Replace("\r", "")
                           .Replace("\n", "")
                           .Replace("\"", "")
                           .Replace("-", " ")
                           .Replace("  ", " ");

            return genres;
        }
    }
}
