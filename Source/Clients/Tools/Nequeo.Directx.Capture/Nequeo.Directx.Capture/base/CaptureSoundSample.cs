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
using Nequeo.IO.Audio;
using Nequeo.IO;

namespace Nequeo.Directx
{
    /// <summary>
    /// Capture sound sample provider.
    /// </summary>
    public class CaptureSoundSample : ISampleGrabberCB, IDisposable
	{
        /// <summary>
        /// Create a new sound capture object. 
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        internal CaptureSoundSample(Capture capture)
        {
            if (capture == null)
                throw new ArgumentException("The capture parameter must be set to a valid Filter.\n");

            _capture = capture;
        }

        private Capture _capture = null;
        private ISampleGrabber _sampGrabber = null;
        private IBaseFilter _baseGrabFlt;

        private IntPtr _handleSound = IntPtr.Zero;
        private ManualResetEvent _audoReadySound = null;
        private volatile bool _gotOneSound = false;
        private int _droppedSound = 0;
        private int _soundMemorySize = 2;
        private int _sampleRate = 44100;
        private short _bitsPerSample = 16;
        private short _channels = 2;

        /// <summary>
        /// Gets the capture provider.
        /// </summary>
        public Capture Capture
        {
            get { return _capture; }
        }

        /// <summary>
        /// Gets samples per second
        /// </summary>
        public int SampleRate
        {
            get { return _sampleRate; }
        }

        /// <summary>
        /// Gets bits per sample.
        /// </summary>
        public short BitsPerSample
        {
            get { return _bitsPerSample; }
        }

        /// <summary>
        /// 1 - mono, 2 - stereo.
        /// </summary>
        public short Channels
        {
            get { return _channels; }
        }

        /// <summary>
        /// Gets the block align size.
        /// </summary>
        public short BlockAlign
        {
            get { return (short)(Channels * (BitsPerSample / 8)); }
        }

        /// <summary>
        /// Gets the average bytes per second.
        /// </summary>
        public int AverageBytesPerSecond
        {
            get { return BlockAlign * SampleRate; }
        }

        /// <summary>
        /// Get the snap shot sound pointer to the buffer.
        /// </summary>
        /// <returns>The point to the buffered sound data.</returns>
        public IntPtr GetSnapshotSoundPointer()
        {
            // Make sure the sound captue has started.
            if (!_capture.IsCaptureToSound)
                throw new Exception("Start the snapshot sound capture first.");

            _handleSound = Marshal.AllocCoTaskMem(_soundMemorySize);

            try
            {
                // Get ready to wait for new sound
                _audoReadySound.Reset();
                _gotOneSound = false;

                // Start waiting
                if (!_audoReadySound.WaitOne(5000, false))
                    throw new Exception("Timeout waiting to get sound");

            }
            catch
            {
                Marshal.FreeCoTaskMem(_handleSound);
                throw;
            }

            // Got an image
            return _handleSound;
        }

        /// <summary>
        /// Capture data to a buffered sound.
        /// </summary>
        public void WriteCaptureToSound()
        {
            // If capture should be written to a buffered image.
            if (_capture.IsCaptureToSound)
            {
                if (_sampGrabber == null)
                {
                    // Tell the callback to ignore new sound
                    _audoReadySound = new ManualResetEvent(false);
                    _gotOneSound = true;

                    Guid cat;
                    Guid med;
                    int hr;

                    // Get the SampleGrabber interface
                    _sampGrabber = (ISampleGrabber)(new SampleGrabber());

                    _baseGrabFlt = (IBaseFilter)_sampGrabber;
                    ConfigureSampleGrabber(_sampGrabber);

                    // Add the frame grabber to the graph
                    hr = _capture.GraphBuilder.AddFilter(_baseGrabFlt, "Nequeo Sound Grabber");
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                    // Render audio (audio -> mux)
                    if (_capture.AudioDevice != null)
                    {
                        // Render preview (audio -> renderer)
                        cat = PinCategory.Capture;
                        med = MediaType.Audio;
                        hr = _capture.CaptureGraphBuilder.RenderStream(cat, med, _capture.AudioDeviceFilter, null, _baseGrabFlt);
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

            // Set the media type to Audio/Wave
            media = new AMMediaType();
            media.majorType = MediaType.Audio;
            media.subType = MediaSubType.PCM;
            media.formatType = FormatType.WaveEx;
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

            if ((media.formatType != FormatType.WaveEx) || (media.formatPtr == IntPtr.Zero))
                throw new NotSupportedException("Unknown Grabber Media Format");

            // Grab the size info
            WaveFormatEx waveFormatEx = (WaveFormatEx)Marshal.PtrToStructure(media.formatPtr, typeof(WaveFormatEx));
            _channels = waveFormatEx.nChannels;
            _sampleRate = waveFormatEx.nSamplesPerSec;
            _bitsPerSample = waveFormatEx.wBitsPerSample;

            // Calculate the size of the sound data.
            _soundMemorySize = (int)_channels * _sampleRate;

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
            if (!_gotOneSound)
            {
                // The buffer should be long enought
                if (BufferLen <= _soundMemorySize)
                {
                    // Copy the frame to the buffer
                    Directx.Utility.Devices.CopyMemory(_handleSound, pBuffer, _soundMemorySize);
                }
                else
                {
                    throw new Exception("Buffer is wrong size");
                }

                // Set bGotOne to prevent further calls until we
                // request a new bitmap.
                _gotOneSound = true;

                // Picture is ready.
                if (_audoReadySound != null)
                    _audoReadySound.Set();
            }
            else
            {
                _droppedSound++;
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
            if (!_gotOneSound)
            {
                // Set bGotOne to prevent further calls until we
                // request a new bitmap.
                _gotOneSound = true;
                IntPtr pBuffer;

                pSample.GetPointer(out pBuffer);
                int iBufferLen = pSample.GetSize();

                if (pSample.GetSize() > _soundMemorySize)
                {
                    throw new Exception("Buffer is wrong size");
                }

                Directx.Utility.Devices.CopyMemory(_handleSound, pBuffer, _soundMemorySize);

                // Picture is ready.
                if (_audoReadySound != null)
                    _audoReadySound.Set();
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
                    if (_audoReadySound != null)
                        _audoReadySound.Close();
                   
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _audoReadySound = null;

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
        ~CaptureSoundSample()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}
}
