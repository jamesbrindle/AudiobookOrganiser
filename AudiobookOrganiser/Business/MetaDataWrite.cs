using ATL;
using AudiobookOrganiser.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Business
{
    internal class MetaDataWriter
    {
        internal static void WriteMetaData(string audioPath, AudiobookMetaData metaData)
        {
            if (metaData == null)
                return;

            Track track = null;

            Thread thread = null;
            Task t = Task.Run(() =>
            {
                //Capture the thread
                thread = Thread.CurrentThread;
                track = new Track(audioPath);
            });

            Thread.Sleep(10);
            int count = 0;
            while (true)
            {
                if (count == 3)
                {
                    try
                    {

                        thread.Abort();
                    }
                    catch { }

                    Thread.Sleep(100);

                    try
                    {
                        thread = null;
                    }
                    catch { }

                    break;
                }

                if (track == null)
                {
                    Thread.Sleep(1000);
                    count++;
                }
                else
                    break;
            }

            if (track == null)
            {
                // Falback
                var tagLib = TagLib.File.Create(audioPath);

                if (!string.IsNullOrWhiteSpace(metaData.Title))
                    tagLib.Tag.Title = metaData.Title;

                if (!string.IsNullOrEmpty(metaData.Author))
                {
                    tagLib.Tag.AlbumArtists = metaData.Author?.Split(',');
                    if (tagLib.Tag.AlbumArtists != null && tagLib.Tag.AlbumArtists.Length > 0)
                        for (int i = 0; i < tagLib.Tag.AlbumArtists.Length; i++)
                            tagLib.Tag.AlbumArtists[0] = tagLib.Tag.AlbumArtists[0].Trim();

                    tagLib.Tag.Performers = metaData.Author?.Split(',');
                    if (tagLib.Tag.Performers != null && tagLib.Tag.Performers.Length > 0)
                        for (int i = 0; i < tagLib.Tag.Performers.Length; i++)
                            tagLib.Tag.Performers[0] = tagLib.Tag.Performers[0].Trim();
                }

                if (!string.IsNullOrEmpty(metaData.Narrator))
                {
                    tagLib.Tag.Composers = metaData.Narrator.Split(',');
                    if (tagLib.Tag.Composers != null && tagLib.Tag.Composers.Length > 0)
                        for (int i = 0; i < tagLib.Tag.Composers.Length; i++)
                            tagLib.Tag.Composers[0] = tagLib.Tag.Composers[0].Trim();

                }

                tagLib.Tag.Genres = new string[] { "Audiobook" };

                if (!string.IsNullOrWhiteSpace(metaData.Series))
                {
                    if (!string.IsNullOrWhiteSpace(metaData.SeriesPart) && metaData.SeriesPart != "0")
                        tagLib.Tag.Grouping = $"Book {metaData.SeriesPart}, {metaData.Series}";
                    else
                        tagLib.Tag.Grouping = $"{metaData.Series}";
                }

                if (!string.IsNullOrWhiteSpace(metaData.Year))
                {
                    try
                    {
                        tagLib.Tag.Year = (uint)Convert.ToInt32(metaData.Year);
                    }
                    catch { }
                }

                tagLib.Save();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(metaData.Title))
                    track.Title = metaData.Title;

                if (!string.IsNullOrEmpty(metaData.Author))
                {
                    track.AlbumArtist = metaData.Author;
                    track.Artist = metaData.Author;
                }

                if (!string.IsNullOrEmpty(metaData.Narrator))
                {
                    track.Composer = metaData.Narrator;
                    track.AdditionalFields["----:com.apple.iTunes:NRT"] = metaData.Narrator;
                    track.AdditionalFields["NRT"] = metaData.Narrator;
                }

                track.Genre = "Audiobook";

                if (!string.IsNullOrWhiteSpace(metaData.Series))
                {
                    track.AdditionalFields["----:com.apple.iTunes:SERIES"] = metaData.Series;
                    track.AdditionalFields["SERIES"] = metaData.Series;
                }

                if (!string.IsNullOrWhiteSpace(metaData.SeriesPart))
                {
                    track.AdditionalFields["----:com.apple.iTunes:SERIES-PART"] = metaData.SeriesPart;
                    track.AdditionalFields["SERIES-PART"] = metaData.SeriesPart;
                }

                if (!string.IsNullOrWhiteSpace(metaData.Series))
                {
                    if (!string.IsNullOrWhiteSpace(metaData.SeriesPart) && metaData.SeriesPart != "0")
                        track.Group = $"Book {metaData.SeriesPart}, {metaData.Series}";
                    else
                        track.Group = $"{metaData.Series}";
                }

                if (!string.IsNullOrWhiteSpace(metaData.Genre))
                {
                    track.AdditionalFields["----:com.apple.iTunes:book_genre"] = metaData.Genre;
                    track.AdditionalFields["book_genre"] = metaData.Genre;
                }

                if (!string.IsNullOrWhiteSpace(metaData.Asin))
                {
                    track.AdditionalFields["----:com.apple.iTunes:ASIN"] = metaData.Asin;
                    track.AdditionalFields["ASIN"] = metaData.Asin;
                }

                if (!string.IsNullOrWhiteSpace(metaData.Year))
                {
                    try
                    {
                        track.Year = Convert.ToInt32(metaData.Year);
                    }
                    catch { }
                }

                track.Save();
            }
        }

        internal static void WriteMetaData(string audioPath, AudibleLibrary.Property metaData)
        {
            if (metaData == null)
                return;

            Track track = null;

            Thread thread = null;
            Task t = Task.Run(() =>
            {
                //Capture the thread
                thread = Thread.CurrentThread;
                track = new Track(audioPath);
            });

            Thread.Sleep(10);
            int count = 0;
            while (true)
            {
                if (count == 3)
                {
                    try
                    {

                        thread.Abort();
                    }
                    catch { }

                    Thread.Sleep(100);

                    try
                    {
                        thread = null;
                    }
                    catch { }

                    break;
                }

                if (track == null)
                {
                    Thread.Sleep(1000);
                    count++;
                }
                else
                    break;
            }

            if (track == null)
            {
                // Falback
                var tagLib = TagLib.File.Create(audioPath);

                if (!string.IsNullOrWhiteSpace(metaData.title))
                    tagLib.Tag.Title = metaData.title;

                if (!string.IsNullOrEmpty(metaData.authors))
                {
                    tagLib.Tag.AlbumArtists = metaData.authors?.Split(',');
                    if (tagLib.Tag.AlbumArtists != null && tagLib.Tag.AlbumArtists.Length > 0)
                        for (int i = 0; i < tagLib.Tag.AlbumArtists.Length; i++)
                            tagLib.Tag.AlbumArtists[0] = tagLib.Tag.AlbumArtists[0].Trim();

                    tagLib.Tag.Performers = metaData.authors?.Split(',');
                    if (tagLib.Tag.Performers != null && tagLib.Tag.Performers.Length > 0)
                        for (int i = 0; i < tagLib.Tag.Performers.Length; i++)
                            tagLib.Tag.Performers[0] = tagLib.Tag.Performers[0].Trim();
                }

                if (!string.IsNullOrEmpty(metaData.narrators))
                {
                    tagLib.Tag.Composers = metaData.narrators.Split(',');
                    if (tagLib.Tag.Composers != null && tagLib.Tag.Composers.Length > 0)
                        for (int i = 0; i < tagLib.Tag.Composers.Length; i++)
                            tagLib.Tag.Composers[0] = tagLib.Tag.Composers[0].Trim();

                }

                tagLib.Tag.Genres = new string[] { "Audiobook" };

                if (!string.IsNullOrWhiteSpace(metaData.series_title))
                {
                    if (!string.IsNullOrWhiteSpace(metaData.series_sequence) && metaData.series_sequence != "0")
                        tagLib.Tag.Grouping = $"Book {metaData.series_sequence}, {metaData.series_title}";
                    else
                        tagLib.Tag.Grouping = $"{metaData.series_title}";
                }

                if (!string.IsNullOrWhiteSpace(metaData.release_date))
                {
                    try
                    {
                        tagLib.Tag.Year = (uint)Convert.ToInt32(Convert.ToDateTime(metaData.release_date));
                    }
                    catch { }
                }

                tagLib.Save();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(metaData.title))
                    track.Title = metaData.title;

                if (!string.IsNullOrEmpty(metaData.authors))
                {
                    track.AlbumArtist = metaData.authors;
                    track.Artist = metaData.authors;
                }

                if (!string.IsNullOrEmpty(metaData.narrators))
                {
                    track.Composer = metaData.narrators;
                    track.AdditionalFields["----:com.apple.iTunes:NRT"] = metaData.narrators;
                    track.AdditionalFields["NRT"] = metaData.narrators;
                }

                track.Genre = "Audiobook";

                if (!string.IsNullOrWhiteSpace(metaData.series_title))
                {
                    track.AdditionalFields["----:com.apple.iTunes:SERIES"] = metaData.series_title;
                    track.AdditionalFields["SERIES"] = metaData.series_title;
                }

                if (!string.IsNullOrWhiteSpace(metaData.series_sequence))
                {
                    track.AdditionalFields["----:com.apple.iTunes:SERIES-PART"] = metaData.series_sequence;
                    track.AdditionalFields["SERIES-PART"] = metaData.series_sequence;
                }

                if (!string.IsNullOrWhiteSpace(metaData.series_title))
                {
                    if (!string.IsNullOrWhiteSpace(metaData.series_sequence) && metaData.series_sequence != "0")
                        track.Group = $"Book {metaData.series_sequence}, {metaData.series_title}";
                    else
                        track.Group = $"{metaData.series_title}";
                }

                if (!string.IsNullOrWhiteSpace(metaData.genres))
                {
                    track.AdditionalFields["----:com.apple.iTunes:book_genre"] = metaData.genres;
                    track.AdditionalFields["book_genre"] = metaData.genres;
                }

                if (!string.IsNullOrWhiteSpace(metaData.asin))
                {
                    track.AdditionalFields["----:com.apple.iTunes:ASIN"] = metaData.asin;
                    track.AdditionalFields["ASIN"] = metaData.asin;
                }

                if (!string.IsNullOrWhiteSpace(metaData.release_date))
                {
                    try
                    {
                        track.Year = Convert.ToInt32(Convert.ToDateTime(metaData.release_date));
                    }
                    catch { }
                }

                track.Save();
            }
        }
    }
}
