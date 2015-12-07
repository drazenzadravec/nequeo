using System;

using Nequeo.Media.Vlc.Media;

namespace Nequeo.Media.Vlc.MediaLibrary
{
    public interface IMediaLibrary
    {
        void Load();
        IMediaList MediaList { get; }
    }
}
