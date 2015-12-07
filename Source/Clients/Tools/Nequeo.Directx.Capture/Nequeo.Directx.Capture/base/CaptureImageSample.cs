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
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

using Nequeo.Directx.Utility;

namespace Nequeo.Directx
{
    /// <summary>
    /// Capture image sample provider.
    /// </summary>
    public class CaptureImageSample : ISampleGrabberCB, IDisposable
    {
        /// <summary>
        /// Create a new image capture object. 
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        internal CaptureImageSample(Capture capture)
        {
            if (capture == null)
                throw new ArgumentException("The capture parameter must be set to a valid Filter.\n");

            _capture = capture;
        }

        private Capture _capture = null;
        private ISampleGrabber _sampGrabber = null;
        private IBaseFilter _baseGrabFlt;

        private IntPtr _handleImage = IntPtr.Zero;
        private ManualResetEvent _pictureReadyImage = null;
        private volatile bool _gotOneImage = false;
        private int _videoWidthImage;
        private int _videoHeightImage;
        private int _strideImage;
        private int _droppedImage = 0;

        /// <summary>
        /// Gets the capture provider.
        /// </summary>
        public Capture Capture
        {
            get { return _capture; }
        }

        /// <summary>
        /// Gets the snapshot image width.
        /// </summary>
        public int Width
        {
            get { return _videoWidthImage; }
        }

        /// <summary>
        /// Gets the snapshot image height.
        /// </summary>
        public int Height
        {
            get { return _videoHeightImage; }
        }

        /// <summary>
        /// Gets the snapshot image stride.
        /// </summary>
        public int Stride
        {
            get { return _strideImage; }
        }

        /// <summary>
        /// Get the snap shot image pointer to the buffer.
        /// </summary>
        /// <returns>The point to the buffered image data.</returns>
        public IntPtr GetSnapshotImagePointer()
        {
            // Make sure the image captue has started.
            if (!_capture.IsCaptureToImage)
                throw new Exception("Start the snapshot image capture first.");

            _handleImage = Marshal.AllocCoTaskMem(_strideImage * _videoHeightImage);

            try
            {
                // Get ready to wait for new image
                _pictureReadyImage.Reset();
                _gotOneImage = false;

                // Start waiting
                if (!_pictureReadyImage.WaitOne(5000, false))
                    throw new Exception("Timeout waiting to get picture");

            }
            catch
            {
                Marshal.FreeCoTaskMem(_handleImage);
                throw;
            }

            // Got an image
            return _handleImage;
        }

        /// <summary>
        /// Capture data to a buffered image.
        /// </summary>
        public void WriteCaptureToImage()
        {
            // If capture should be written to a buffered image..
            if (_capture.IsCaptureToImage)
            {
                if (_sampGrabber == null)
                {
                    // Tell the callback to ignore new images
                    _pictureReadyImage = new ManualResetEvent(false);
                    _gotOneImage = true;

                    Guid cat;
                    Guid med;
                    int hr;

                    // Get the SampleGrabber interface
                    _sampGrabber = (ISampleGrabber)(new SampleGrabber());

                    _baseGrabFlt = (IBaseFilter)_sampGrabber;
                    ConfigureSampleGrabber(_sampGrabber);

                    // Add the frame grabber to the graph
                    hr = _capture.GraphBuilder.AddFilter(_baseGrabFlt, "Nequeo Image Grabber");
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                    // Render video (video -> mux)
                    if (_capture.VideoDevice != null)
                    {
                        // Render preview (video -> renderer)
                        cat = PinCategory.Capture;
                        med = MediaType.Video;
                        hr = _capture.CaptureGraphBuilder.RenderStream(cat, med, _capture.VideoDeviceFilter, null, _baseGrabFlt);
                        if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                    }

                    // Save size info.
                    SaveSizeInfo(_sampGrabber);
                }
            }
        }

        /// <summary>
        /// Configure the ample grabber.
        /// </summary>
        /// <param name="sampGrabber">The sanmple grabber.</param>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            AMMediaType media;
            int hr;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType(media);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback(this, 1);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Save the sample grabber size.
        /// </summary>
        /// <param name="sampGrabber">The sanmple grabber.</param>
        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
                throw new NotSupportedException("Unknown Grabber Media Format");

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            _videoWidthImage = videoInfoHeader.BmiHeader.Width;
            _videoHeightImage = videoInfoHeader.BmiHeader.Height;
            _strideImage = _videoWidthImage * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary>
        /// Buffer callback, COULD BE FROM FOREIGN THREAD.
        /// </summary>
        /// <param name="SampleTime">The sample time.</param>
        /// <param name="pBuffer">The buffer pointer.</param>
        /// <param name="BufferLen">The buffer length.</param>
        /// <returns>The result.</returns>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (!_gotOneImage)
            {
                // The buffer should be long enought
                if (BufferLen <= _strideImage * _videoHeightImage)
                {
                    // Copy the frame to the buffer
                    Devices.CopyMemory(_handleImage, pBuffer, _strideImage * _videoHeightImage);
                }
                else
                {
                    throw new Exception("Buffer is wrong size");
                }

                // Set bGotOne to prevent further calls until we
                // request a new bitmap.
                _gotOneImage = true;

                // Picture is ready.
                if (_pictureReadyImage != null)
                    _pictureReadyImage.Set();
            }
            else
            {
                _droppedImage++;
            }
            return 0;
        }

        /// <summary>
        /// Sample callback, NOT USED.
        /// </summary>
        /// <param name="SampleTime">The sample time.</param>
        /// <param name="pSample">The media sample.</param>
        /// <returns>The result.</returns>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            if (!_gotOneImage)
            {
                // Set bGotOne to prevent further calls until we
                // request a new bitmap.
                _gotOneImage = true;
                IntPtr pBuffer;

                pSample.GetPointer(out pBuffer);
                int iBufferLen = pSample.GetSize();

                if (pSample.GetSize() > _strideImage * _videoHeightImage)
                {
                    throw new Exception("Buffer is wrong size");
                }

                Devices.CopyMemory(_handleImage, pBuffer, _strideImage * _videoHeightImage);

                // Picture is ready.
                if (_pictureReadyImage != null)
                    _pictureReadyImage.Set();
            }

            Marshal.ReleaseComObject(pSample);
            return 0;
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
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    // Release the ManualResetEvent thread.
                    if (_pictureReadyImage != null)
                        _pictureReadyImage.Close();
                        
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pictureReadyImage = null;

                try
                {
                    // Remove the filter.
                    if (_baseGrabFlt != null)
                        if (_capture != null)
                            if (_capture.GraphBuilder != null)
                                _capture.GraphBuilder.RemoveFilter(_baseGrabFlt);
                }
                catch { }

                // Release the objects.
                if (_baseGrabFlt != null)
                    Marshal.ReleaseComObject(_baseGrabFlt); 

                // Release the objects.
                if (_sampGrabber != null)
                    Marshal.ReleaseComObject(_sampGrabber); 

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _baseGrabFlt = null;
                _sampGrabber = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~CaptureImageSample()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
