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

            tagLib.Tag.Track = metaData.Track;

            if (!string.IsNullOrWhiteSpace(metaData.Overview))
            {
                if (metaData.Overview.ToLower().StartsWith("chapter"))
                {
                    tagLib.Tag.Comment = string.Empty;
                    tagLib.Tag.Description = string.Empty;
                }
                else
                {
                    tagLib.Tag.Comment = metaData.Overview;
                    tagLib.Tag.Description = metaData.Overview;
                }
            }

            if (!string.IsNullOrEmpty(metaData.Copyright))
                tagLib.Tag.Copyright = metaData.Copyright.Replace("&#169;", "©");

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
    }
}
