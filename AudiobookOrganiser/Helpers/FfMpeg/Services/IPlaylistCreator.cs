using System.Collections.Generic;

namespace FfMpeg.Services
{
    public interface IPlaylistCreator
    {
        string Create(IList<MetaData> metaData);
    }
}