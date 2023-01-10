using System.Collections.Generic;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Services
{
    public interface IPlaylistCreator
    {
        string Create(IList<MetaData> metaData);
    }
}