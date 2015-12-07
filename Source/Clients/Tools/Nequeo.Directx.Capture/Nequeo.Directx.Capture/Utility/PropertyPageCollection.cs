/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Text;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

namespace Nequeo.Directx.Utility
{
    /// <summary>
    ///  A collection of available PropertyPages in a DirectShow
    ///  filter graph. It is up to the driver manufacturer to implement
    ///  a property pages on their drivers. The list of supported 
    ///  property pages will vary from driver to driver.
    /// </summary>
    public class PropertyPageCollection : CollectionBase, IDisposable
    {
        /// <summary>
        /// Initialize collection with no property pages
        /// </summary>
        internal PropertyPageCollection()
        {
            InnerList.Capacity = 1;
        }

        /// <summary>
        /// Initialize collection with property pages from existing graph
        /// </summary>
        /// <param name="graphBuilder"></param>
        /// <param name="videoDeviceFilter"></param>
        /// <param name="audioDeviceFilter"></param>
        /// <param name="videoCompressorFilter"></param>
        /// <param name="audioCompressorFilter"></param>
        /// <param name="videoSources"></param>
        /// <param name="audioSources"></param>
        internal PropertyPageCollection(
            ICaptureGraphBuilder2 graphBuilder,
            IBaseFilter videoDeviceFilter, IBaseFilter audioDeviceFilter,
            IBaseFilter videoCompressorFilter, IBaseFilter audioCompressorFilter,
            SourceCollection videoSources, SourceCollection audioSources)
        {
            addFromGraph(graphBuilder,
                videoDeviceFilter, audioDeviceFilter,
                videoCompressorFilter, audioCompressorFilter,
                videoSources, audioSources);
        }

        /// <summary>
        /// Get the filter at the specified index
        /// </summary>
        /// <param name="index">The index of the property page.</param>
        /// <returns>The current proprty page.</returns>
        public PropertyPage this[int index]
        {
            get { return ((PropertyPage)InnerList[index]); }
        }

        /// <summary>
        /// Removes all elements from the System.Collections.ArrayList.
        /// </summary>
        public new void Clear()
        {
            for (int c = 0; c < InnerList.Count; c++)
                this[c].Dispose();
            InnerList.Clear();
        }

        /// <summary>
        /// Populate the collection by looking for commonly implemented property pages
        /// </summary>
        /// <param name="graphBuilder"></param>
        /// <param name="videoDeviceFilter"></param>
        /// <param name="audioDeviceFilter"></param>
        /// <param name="videoCompressorFilter"></param>
        /// <param name="audioCompressorFilter"></param>
        /// <param name="videoSources"></param>
        /// <param name="audioSources"></param>
        protected void addFromGraph(
            ICaptureGraphBuilder2 graphBuilder,
            IBaseFilter videoDeviceFilter, IBaseFilter audioDeviceFilter,
            IBaseFilter videoCompressorFilter, IBaseFilter audioCompressorFilter,
            SourceCollection videoSources, SourceCollection audioSources)
        {
            object filter = null;
            Guid cat;
            Guid med;
            Guid iid;
            int hr;

            // 1. the video capture filter
            addIfSupported(videoDeviceFilter, "Video Capture Device");

            // 2. the video capture pin
            cat = PinCategory.Capture;
            med = MediaType.Interleaved;
            iid = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(
                cat, med, videoDeviceFilter, iid, out filter);
            if (hr != 0)
            {
                med = MediaType.Video;
                hr = graphBuilder.FindInterface(
                    cat, med, videoDeviceFilter, iid, out filter);
                if (hr != 0)
                    filter = null;
            }
            addIfSupported(filter, "Video Capture Pin");

            // 3. the video preview pin
            cat = PinCategory.Preview;
            med = MediaType.Interleaved;
            iid = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(
                cat, med, videoDeviceFilter, iid, out filter);
            if (hr != 0)
            {
                med = MediaType.Video;
                hr = graphBuilder.FindInterface(
                    cat, med, videoDeviceFilter, iid, out filter);
                if (hr != 0)
                    filter = null;
            }
            addIfSupported(filter, "Video Preview Pin");

            // 4. the video crossbar(s)
            ArrayList crossbars = new ArrayList();
            int num = 1;
            for (int c = 0; c < videoSources.Count; c++)
            {
                CrossbarSource s = videoSources[c] as CrossbarSource;
                if (s != null)
                {
                    if (crossbars.IndexOf(s.Crossbar) < 0)
                    {
                        crossbars.Add(s.Crossbar);
                        if (addIfSupported(s.Crossbar, "Video Crossbar " + (num == 1 ? "" : num.ToString())))
                            num++;
                    }
                }
            }
            crossbars.Clear();

            // 5. the video compressor
            addIfSupported(videoCompressorFilter, "Video Compressor");

            // 6. the video TV tuner
            cat = PinCategory.Capture;
            med = MediaType.Interleaved;
            iid = typeof(IAMTVTuner).GUID;
            hr = graphBuilder.FindInterface(
                cat, med, videoDeviceFilter, iid, out filter);
            if (hr != 0)
            {
                med = MediaType.Video;
                hr = graphBuilder.FindInterface(
                    cat, med, videoDeviceFilter, iid, out filter);
                if (hr != 0)
                    filter = null;
            }
            addIfSupported(filter, "TV Tuner");

            // 7. the video compressor (VFW)
            IAMVfwCompressDialogs compressDialog = videoCompressorFilter as IAMVfwCompressDialogs;
            if (compressDialog != null)
            {
                VfwCompressorPropertyPage page = new VfwCompressorPropertyPage("Video Compressor", compressDialog);
                InnerList.Add(page);
            }

            // 8. the audio capture filter
            addIfSupported(audioDeviceFilter, "Audio Capture Device");

            // 9. the audio capture pin
            cat = PinCategory.Capture;
            med = MediaType.Audio;
            iid = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(
                cat, med, audioDeviceFilter, iid, out filter);
            if (hr != 0)
            {
                filter = null;
            }
            addIfSupported(filter, "Audio Capture Pin");

            // 9. the audio preview pin
            cat = PinCategory.Preview;
            med = MediaType.Audio;
            iid = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(
                cat, med, audioDeviceFilter, iid, out filter);
            if (hr != 0)
            {
                filter = null;
            }
            addIfSupported(filter, "Audio Preview Pin");

            // 10. the audio crossbar(s)
            num = 1;
            for (int c = 0; c < audioSources.Count; c++)
            {
                CrossbarSource s = audioSources[c] as CrossbarSource;
                if (s != null)
                {
                    if (crossbars.IndexOf(s.Crossbar) < 0)
                    {
                        crossbars.Add(s.Crossbar);
                        if (addIfSupported(s.Crossbar, "Audio Crossbar " + (num == 1 ? "" : num.ToString())))
                            num++;
                    }
                }
            }
            crossbars.Clear();

            // 11. the audio compressor
            addIfSupported(audioCompressorFilter, "Audio Compressor");

        }

        /// <summary> 
        ///  Returns the object as an ISpecificPropertyPage
        ///  if the object supports the ISpecificPropertyPage
        ///  interface and has at least one property page.
        /// </summary>
        protected bool addIfSupported(object o, string name)
        {
            ISpecifyPropertyPages specifyPropertyPages = null;
            DsCAUUID cauuid = new DsCAUUID();
            bool wasAdded = false;

            // Determine if the object supports the interface
            // and has at least 1 property page
            try
            {
                specifyPropertyPages = o as ISpecifyPropertyPages;
                if (specifyPropertyPages != null)
                {
                    int hr = specifyPropertyPages.GetPages(out cauuid);
                    if ((hr != 0) || (cauuid.cElems <= 0))
                        specifyPropertyPages = null;
                }
            }
            finally
            {
                if (cauuid.pElems != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(cauuid.pElems);
            }

            // Add the page to the internal collection
            if (specifyPropertyPages != null)
            {
                DirectShowPropertyPage p = new DirectShowPropertyPage(name, specifyPropertyPages);
                InnerList.Add(p);
                wasAdded = true;
            }
            return (wasAdded);
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Clear();
                    InnerList.Capacity = 1;
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~PropertyPageCollection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
