namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    internal enum FfMpegTask
    {
        EnsureAudioStream,
        Convert,
        Combine,
        Concatenate,
        GetMetaData,
        SetMetaData,
        GetThumbnail
    }
}