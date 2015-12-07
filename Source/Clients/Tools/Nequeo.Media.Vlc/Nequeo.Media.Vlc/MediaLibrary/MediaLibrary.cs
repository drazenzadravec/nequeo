using System;

using Nequeo.Media.Vlc;
using Nequeo.Media.Vlc.Enums;
using Nequeo.Media.Vlc.Media;
using Nequeo.Media.Vlc.Internal;
using Nequeo.Media.Vlc.Structures;
using Nequeo.Media.Vlc.MediaLibrary;

namespace Nequeo.Media.Vlc.MediaLibrary
{
    internal class MediaLibraryImpl : DisposableBase, IReferenceCount, INativePointer, IMediaLibrary
    {
        private IntPtr m_hMediaLib = IntPtr.Zero;

        public MediaLibraryImpl(IntPtr mediaLib)
        {
            m_hMediaLib = LibVlcMethods.libvlc_media_library_new(mediaLib);
        }

        protected override void Dispose(bool disposing)
        {
            Release();
        }

        public void Load()
        {
            int result = LibVlcMethods.libvlc_media_library_load(m_hMediaLib);
        }

        public IMediaList MediaList
        {
            get
            {
                return new MediaList(LibVlcMethods.libvlc_media_library_media_list(m_hMediaLib));
            }
        }

        public void AddRef()
        {
            LibVlcMethods.libvlc_media_library_retain(m_hMediaLib);
        }

        public void Release()
        {
            LibVlcMethods.libvlc_media_library_release(m_hMediaLib);
        }

        public IntPtr Pointer
        {
            get 
            {
                return m_hMediaLib;
            }
        }
    }
}
