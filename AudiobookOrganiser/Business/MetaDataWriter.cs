using AudiobookOrganiser.Models;
using System;
using System.Threading;

namespace AudiobookOrganiser.Business
{
    internal class MetaDataWriter
    {
        internal static void WriteMetaData(string audioPath, AudiobookMetaData metaData)
        {
            try
            {
                WriteMetaDataActual(audioPath, metaData);
            }
            catch
            {
                Thread.Sleep(1000);
            }
        }

        private static void WriteMetaDataActual(string audioPath, AudiobookMetaData metaData)
        {
            if (metaData == null)
                return;

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

            if (string.IsNullOrWhiteSpace(metaData.Album))
            {
                if (string.IsNullOrEmpty(metaData.Series))
                {
                    tagLib.Tag.Album = metaData.Title;
                }
                else
                {
                    if (!string.IsNullOrEmpty(metaData.SeriesPart))
                        tagLib.Tag.Album = $"{metaData.Title}: {metaData.Series}, Book {metaData.SeriesPart}";
                    else
                        tagLib.Tag.Album = $"{metaData.Title}: {metaData.Series}";
                }
            }
            else
                tagLib.Tag.Album = metaData.Album;

            if (string.IsNullOrWhiteSpace(metaData.AlbumSort))
            {
                if (string.IsNullOrEmpty(metaData.Series))
                {
                    tagLib.Tag.AlbumSort = metaData.Title;
                }
                else
                {
                    if (!string.IsNullOrEmpty(metaData.SeriesPart))
                        tagLib.Tag.AlbumSort = $"{metaData.Series}, Book {metaData.SeriesPart} - {metaData.Title}";
                    else
                        tagLib.Tag.AlbumSort = $"{metaData.Series} - {metaData.Title}";
                }
            }
            else
                tagLib.Tag.AlbumSort = metaData.AlbumSort;

            if (!string.IsNullOrWhiteSpace(metaData.Asin))
                tagLib.Tag.AmazonId = metaData.Asin;

            if (!string.IsNullOrWhiteSpace(metaData.Overview))
            {
                tagLib.Tag.Comment = metaData.Overview;
                tagLib.Tag.Description = metaData.Overview;
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
            tagLib.Dispose();
        }
        internal static void WriteMetaData(string audioPath, AudibleLibrary.Property metaData)
        {
            try
            {
                WriteMetaDataActual(audioPath, metaData);
            }
            catch
            {
                Thread.Sleep(1000);
            }
        }

        private static void WriteMetaDataActual(string audioPath, AudibleLibrary.Property metaData)
        {
            if (metaData == null)
                return;

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

            if (!string.IsNullOrWhiteSpace(metaData.asin))
            {
                tagLib.Tag.AmazonId = metaData.asin;
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
            tagLib.Dispose();
        }
    }
}
