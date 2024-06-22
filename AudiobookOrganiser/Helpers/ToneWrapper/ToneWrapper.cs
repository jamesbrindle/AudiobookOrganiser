using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace AudiobookOrganiser.Helpers.ToneWrapper
{
    public class ToneWrapper
    {
        public class PropertyAttribute : Attribute
        {
            public string Name { get; set; }

            public PropertyAttribute(string propertyString)
            {
                Name = propertyString;
            }
        }

        public string AudioFilePath { get; set; }
        public string ToneExecutablePath { get; set; } = null;

        public class AdditionalField
        {
            public string FieldName { get; set; }
            public string Value { get; set; }

            public AdditionalField(string fieldName, string value)
            {
                FieldName = fieldName;
                Value = value;

            }
        }

        public class PropertyToRemove
        {
            public string PropertyName { get; set; }

            public PropertyToRemove(string propertyName)
            {
                PropertyName = propertyName;

            }
        }

        public class AdditionalFieldToRemove
        {
            public string FieldName { get; set; }

            public AdditionalFieldToRemove(string fieldName)
            {
                FieldName = fieldName;
            }
        }

        [Description("--meta-remove-property")]
        public List<PropertyToRemove> PropertiesToRemove { get; set; } = new List<PropertyToRemove>();


        [Description("--meta-additional-field")]
        public List<AdditionalField> AdditionalFieldsToAdd { get; set; } = new List<AdditionalField>();


        [Description("--meta-remove-additional-field")]
        public List<AdditionalFieldToRemove> AdditionalFieldsToRemove { get; set; } = new List<AdditionalFieldToRemove>();


        [Description("--meta-artist")]
        [Property("Artist")]
        public string Artist { get; set; }


        [Description("--meta-album-artist")]
        [Property("AlbumArtist")]
        public string AlbumArtist { get; set; }


        [Description("--meta-bpm")]
        [Property("BPM")]
        public string Bpm { get; set; }


        [Description("--meta-chapters-table-description")]
        [Property("ChaptersTableDescription")]
        public string ChaptersTableDescription { get; set; }


        [Description("--meta-comment")]
        [Property("Comment")]
        public string Comment { get; set; }


        [Description("--meta-composer")]
        [Property("Composer")]
        public string Composer { get; set; }


        [Description("--meta-conductor")]
        [Property("Conductor")]
        public string Conductor { get; set; }


        [Description("--meta-copyright")]
        [Property("Copyright")]
        public string Copyright { get; set; }


        [Description("--meta-description")]
        [Property("Description")]
        public string Description { get; set; }


        [Description("--meta-disc-number")]
        [Property("DiscNumber")]
        public string DiscNumber { get; set; }


        [Description("--meta-disc-total")]
        [Property("DiscTotal")]
        public string DiscTotal { get; set; }


        [Description("--meta-encoded-by")]
        [Property("EncodedBy")]
        public string EncodedBy { get; set; }


        [Description("--meta-encoding-tool")]
        [Property("EncodingTool")]
        public string EncodingTool { get; set; }


        [Description("--meta-genre")]
        [Property("Genre")]
        public string Genre { get; set; }


        [Description("--meta-group")]
        [Property("Group")]
        public string Group { get; set; }


        [Description("--meta-itunes-compilation")]
        [Property("ItunesCompilation")]
        public string ItunesCompilation { get; set; }


        [Description("--meta-itunes-media-type")]
        [Property("ItunesMediaType")]
        public string ItunesMediaType { get; set; }


        [Description("--meta-itunes-play-gap")]
        [Property("ItunesPlayGap")]
        public string ItunesPlayGap { get; set; }


        [Description("--meta-long-description")]
        [Property("LongDescription")]
        public string LongDescription { get; set; }


        [Description("--meta-part")]
        [Property("Part")]
        public string Part { get; set; }


        [Description("--meta-movement")]
        [Property("Movement")]
        public string Movement { get; set; }


        [Description("--meta-movement-name")]
        [Property("MovementName")]
        public string MovementName { get; set; }


        [Description("--meta-narrator")]
        [Property("Narrator")]
        public string Narrator { get; set; }


        [Description("--meta-original-album")]
        [Property("OriginalAlbum")]
        public string OriginalAlbum { get; set; }


        [Description("--meta-original-artist")]
        [Property("OriginalArtist")]
        public string OriginalArtist { get; set; }


        [Description("--meta-popularity")]
        [Property("Popularity")]
        public string Popularity { get; set; }


        [Description("--meta-publisher")]
        [Property("Publisher")]
        public string Publisher { get; set; }


        [Description("--meta-publishing-date")]
        [Property("PublishingDate")]
        public string PublishingDate { get; set; }


        [Description("--meta-purchase-date")]
        [Property("PurchaseDate")]
        public string PurchaseDate { get; set; }


        [Description("--meta-recording-date")]
        [Property("RecordingDate")]
        public string RecordingDate { get; set; }


        [Description("--meta-sort-album")]
        [Property("SortAlbum")]
        public string SortAlbum { get; set; }


        [Description("--meta-sort-album-artist")]
        [Property("SortAlbumArtist")]
        public string SortAlbumArtist { get; set; }


        [Description("--meta-sort-artist")]
        [Property("SortArtist")]
        public string SortArtist { get; set; }


        [Description("--meta-sort-composer")]
        [Property("SortComposer")]
        public string SortComposer { get; set; }


        [Description("--meta-sort-title")]
        [Property("SortTitle")]
        public string SortTitle { get; set; }


        [Description("--meta-subtitle")]
        [Property("Subtitle")]
        public string Subtitle { get; set; }


        [Description("--meta-title")]
        [Property("Title")]
        public string Title { get; set; }


        [Description("--meta-track-number")]
        [Property("TrackNumber")]
        public string TrackNumber { get; set; }


        [Description("--meta-track-total")]
        [Property("TrackTotal")]
        public string TrackTotal { get; set; }


        [Description("--meta-chapters-file")]
        [Property("ChaptersFile")]
        public string ChaptersFile { get; set; }


        [Description("--meta-cover-file")]
        [Property("CoverFile")]
        public string CoverFile { get; set; }


        public ToneWrapper(string audioFilePath, string toneExecutablePath = null)
        {
            AudioFilePath = audioFilePath;

            if (!string.IsNullOrEmpty(toneExecutablePath))
                ToneExecutablePath = toneExecutablePath;

            if (string.IsNullOrEmpty(toneExecutablePath))
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string path = Path.GetDirectoryName(asm.Location);

                if (File.Exists(Path.Combine(path, "tone.exe")))
                    ToneExecutablePath = Path.Combine(path, "tone.exe");
                else
                    throw new ApplicationException("Unable to locate tone.exe executable");
            }
        }

        public void WriteMetaData()
        {
            string command = $"tag \"{AudioFilePath}\" --assume-yes ";

            if (!string.IsNullOrEmpty(Artist))
                command += "";

            if (!string.IsNullOrEmpty(Artist))
                command += $"{this.Description("Artist")}=\"{Artist}\" ";

            if (!string.IsNullOrEmpty(AlbumArtist))
                command += $"{this.Description("AlbumArtist")}=\"{AlbumArtist}\" ";

            if (!string.IsNullOrEmpty(Bpm))
                command += $"{this.Description("Bpm")}=\"{Bpm}\" ";

            if (!string.IsNullOrEmpty(ChaptersTableDescription))
                command += $"{this.Description("ChaptersTableDescription")}=\"{ChaptersTableDescription}\" ";

            if (!string.IsNullOrEmpty(Comment))
                command += $"{this.Description("Comment")}=\"{Comment.Replace("\"", "'").Replace("\n", " ").Replace("\r", "")}\" ";

            if (!string.IsNullOrEmpty(Composer))
                command += $"{this.Description("Composer")}=\"{Composer}\" ";

            if (!string.IsNullOrEmpty(Conductor))
                command += $"{this.Description("Conductor")}=\"{Conductor}\" ";

            if (!string.IsNullOrEmpty(Copyright))
                command += $"{this.Description("Copyright")}=\"{Copyright}\" ";

            if (!string.IsNullOrEmpty(Description))
                command += $"{this.Description("Description")}=\"{Description.Replace("\"", "'").Replace("\n", " ").Replace("\r", "")}\" ";

            if (!string.IsNullOrEmpty(DiscNumber))
                command += $"{this.Description("DiscNumber")}=\"{DiscNumber}\" ";

            if (!string.IsNullOrEmpty(DiscTotal))
                command += $"{this.Description("DiscTotal")}=\"{DiscTotal}\" ";

            if (!string.IsNullOrEmpty(EncodedBy))
                command += $"{this.Description("EncodedBy")}=\"{EncodedBy}\" ";

            if (!string.IsNullOrEmpty(EncodingTool))
                command += $"{this.Description("EncodingTool")}=\"{EncodingTool}\" ";

            if (!string.IsNullOrEmpty(Genre))
                command += $"{this.Description("Genre")}=\"{Genre}\" ";

            if (!string.IsNullOrEmpty(Group))
                command += $"{this.Description("Group")}=\"{Group}\" ";

            if (!string.IsNullOrEmpty(ItunesCompilation))
                command += $"{this.Description("ArItunesCompilationtist")}=\"{ItunesCompilation}\" ";

            if (!string.IsNullOrEmpty(ItunesMediaType))
                command += $"{this.Description("ItunesMediaType")}=\"{ItunesMediaType}\" ";

            if (!string.IsNullOrEmpty(ItunesPlayGap))
                command += $"{this.Description("ItunesPlayGap")}=\"{ItunesPlayGap}\" ";

            if (!string.IsNullOrEmpty(LongDescription))
                command += $"{this.Description("LongDescription")}=\"{LongDescription.Replace("\"", "'").Replace("\n", " ").Replace("\r", "")}\" ";

            if (!string.IsNullOrEmpty(Part))
                command += $"{this.Description("Part")}=\"{Part}\" ";

            if (!string.IsNullOrEmpty(Movement))
                command += $"{this.Description("Movement")}=\"{Movement}\" ";

            if (!string.IsNullOrEmpty(MovementName))
                command += $"{this.Description("MovementName")}=\"{MovementName}\" ";

            if (!string.IsNullOrEmpty(Narrator))
                command += $"{this.Description("Narrator")}=\"{Narrator}\" ";

            if (!string.IsNullOrEmpty(OriginalAlbum))
                command += $"{this.Description("OriginalAlbum")}=\"{OriginalAlbum}\" ";

            if (!string.IsNullOrEmpty(OriginalArtist))
                command += $"{this.Description("OriginalArtist")}=\"{OriginalArtist}\" ";

            if (!string.IsNullOrEmpty(Popularity))
                command += $"{this.Description("Popularity")}=\"{Popularity}\" ";

            if (!string.IsNullOrEmpty(Publisher))
                command += $"{this.Description("Publisher")}=\"{Publisher}\" ";

            if (!string.IsNullOrEmpty(PublishingDate))
            {
                if (PublishingDate.Length == 4)
                    PublishingDate = "01/01/" + PublishingDate;

                command += $"{this.Description("PublishingDate")}=\"{PublishingDate}\" ";
            }

            if (!string.IsNullOrEmpty(PurchaseDate))
            {
                if (PurchaseDate.Length == 4)
                    PurchaseDate = "01/01/" + PurchaseDate;

                command += $"{this.Description("PurchaseDate")}=\"{PurchaseDate}\" ";
            }

            if (!string.IsNullOrEmpty(RecordingDate))
            {
                if (RecordingDate.Length == 4)
                    RecordingDate = "01/01/" + RecordingDate;

                command += $"{this.Description("RecordingDate")}=\"{RecordingDate}\" ";

            }

            if (!string.IsNullOrEmpty(SortAlbum))
                command += $"{this.Description("SortAlbum")}=\"{SortAlbum}\" ";

            if (!string.IsNullOrEmpty(SortAlbumArtist))
                command += $"{this.Description("SortAlbumArtist")}=\"{SortAlbumArtist}\" ";

            if (!string.IsNullOrEmpty(SortArtist))
                command += $"{this.Description("SortArtist")}=\"{SortArtist}\" ";

            if (!string.IsNullOrEmpty(SortComposer))
                command += $"{this.Description("SortComposer")}=\"{SortComposer}\" ";

            if (!string.IsNullOrEmpty(SortTitle))
                command += $"{this.Description("SortTitle")}=\"{SortTitle.Replace("\"", "")}\" ";

            if (!string.IsNullOrEmpty(Subtitle))
                command += $"{this.Description("Subtitle")}=\"{Subtitle.Replace("\"", "")}\" ";

            if (!string.IsNullOrEmpty(Title))
                command += $"{this.Description("Title")}=\"{Title.Replace("\"", "")}\" ";

            if (!string.IsNullOrEmpty(TrackNumber))
                command += $"{this.Description("TrackNumber")}=\"{TrackNumber}\" ";

            if (!string.IsNullOrEmpty(TrackTotal))
                command += $"{this.Description("TrackTotal")}=\"{TrackTotal}\" ";

            if (!string.IsNullOrEmpty(ChaptersFile))
                command += $"{this.Description("ChaptersFile")}=\"{ChaptersFile}\" ";

            if (!string.IsNullOrEmpty(CoverFile))
                command += $"{this.Description("CoverFile")}=\"{CoverFile}\" ";

            if (PropertiesToRemove != null && PropertiesToRemove.Count > 0)
            {
                foreach (var property in PropertiesToRemove)
                {
                    command += $"--meta-remove-property=\"{property.PropertyName}\" ";
                }
            }

            if (AdditionalFieldsToRemove != null && AdditionalFieldsToRemove.Count > 0)
            {
                foreach (var additionalField in AdditionalFieldsToRemove)
                {
                    command += $"--meta-remove-additional-field=\"{additionalField.FieldName}\" ";
                }
            }

            if (AdditionalFieldsToAdd != null && AdditionalFieldsToAdd.Count > 0)
            {
                foreach (var additionalField in AdditionalFieldsToAdd)
                {
                    command += $"--meta-additional-field=\"{additionalField.FieldName}={additionalField.Value}\" ";
                }
            }

            ProcessHelper.ExecuteProcessAndReadStdOut(
                ToneExecutablePath,
                out string errorOutput,
                command,
                timeoutSeconds: 120,
                throwOnError: false);

            if (!string.IsNullOrEmpty(errorOutput))
                throw new ApplicationException(errorOutput);
        }
    }
}
