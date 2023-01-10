namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    internal class FfMpegParameters
    {
        internal bool HasCustomArguments => !string.IsNullOrWhiteSpace(CustomArguments);
        internal string CustomArguments { get; set; }
        internal ConversionOptions ConversionOptions { get; set; }
        internal CombineOptions CombineOptions { get; set; }
        internal ConcatenationOptions ConcatenationOptions { get; set; }
        internal SetMetaDataOptions SetMetaDatOptions { get; set; }
        internal GetThumbnailOptions GetThumbnailOptions { get; set; }
        internal FfMpegTask Task { get; set; }
        internal MediaFile OutputFile { get; set; }
        internal MediaFile InputFile { get; set; }
        internal MediaFile[] InputFiles { get; set; } = null;
        internal bool LibFDK_AAC_EncodingEnabled { get; set; } = false;
    }
}