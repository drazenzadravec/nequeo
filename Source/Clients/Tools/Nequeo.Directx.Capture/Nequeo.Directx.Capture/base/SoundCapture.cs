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
using System.Drawing.Imaging;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

using Nequeo.Media;
using Nequeo.Directx.Utility;
using Nequeo.IO.Audio;
using Nequeo.IO;

namespace Nequeo.Directx
{
    /// <summary>
    /// Sound capture provider.
    /// </summary>
    public partial class SoundCapture : IDisposable
	{
        /// <summary>
        /// Create a new image capture object. 
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        public SoundCapture(CaptureSoundSample capture)
        {
            if (capture == null)
                throw new ArgumentException("The capture parameter must be set to a valid Filter.\n");

            _capture = capture;
            OnCreated();
        }

        private CaptureSoundSample _capture = null;
        private bool _stop = true;

        /// <summary>
        /// Gets the capture provider.
        /// </summary>
        public CaptureSoundSample Capture
        {
            get { return _capture; }
        }

        /// <summary>
        /// Gets samples per second
        /// </summary>
        public int SampleRate
        {
            get { return _capture.SampleRate; }
        }

        /// <summary>
        /// Gets bits per sample.
        /// </summary>
        public short BitsPerSample
        {
            get { return _capture.BitsPerSample; }
        }

        /// <summary>
        /// 1 - mono, 2 - stereo.
        /// </summary>
        public short Channels
        {
            get { return _capture.Channels; }
        }

        /// <summary>
        /// Gets the block align size.
        /// </summary>
        public short BlockAlign
        {
            get { return _capture.BlockAlign; }
        }

        /// <summary>
        /// Gets the average bytes per second.
        /// </summary>
        public int AverageBytesPerSecond
        {
            get { return _capture.AverageBytesPerSecond; }
        }

        /// <summary>
        /// Start the capture single process.
        /// </summary>
        /// <param name="captureHandler">The capture method where sound data is to be written to.</param>
        /// <param name="soundCaptureType">The sound capture type.</param>
        public void StartSingle(Action<MemoryStream> captureHandler, SoundCaptureType soundCaptureType = SoundCaptureType.Wav)
        {
            _stop = false;
            StartSampleSoundCaptureSingle(_capture, captureHandler, soundCaptureType);
        }

        /// <summary>
        /// Start the capture continuous process.
        /// </summary>
        /// <param name="captureHandler">The capture method where sound data is to be written to.</param>
        /// <param name="soundCaptureType">The sound capture type.</param>
        public void StartContinuous(Action<MemoryStream> captureHandler, SoundCaptureType soundCaptureType = SoundCaptureType.Wav)
        {
            _stop = false;
            StartSampleSoundCaptureContinuous(_capture, captureHandler, soundCaptureType);
        }

        /// <summary>
        /// Stop the capture process
        /// </summary>
        public void Stop()
        {
            _stop = true;
        }

        /// <summary>
        /// Start sample sound capture continuous.
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="soundCaptureType">The sound capture type.</param>
        private void StartSampleSoundCaptureContinuous(CaptureSoundSample capture, Action<MemoryStream> captureHandler, SoundCaptureType soundCaptureType = SoundCaptureType.Wav)
        {
            MemoryStream stream = new MemoryStream();
            byte[] buffer = null;
            int sampleSize = 0;
            IntPtr ip = IntPtr.Zero;

            // While not shutting down, and still capturing.
            while (!_stop)
            {
                try
                {
                    // capture image
                    ip = capture.GetSnapshotSoundPointer();

                    // Allocate the memory byte size.
                    sampleSize = (int)capture.Channels * capture.SampleRate;
                    buffer = new byte[sampleSize];

                    // Copy the sound data to the buffer.
                    Marshal.Copy(ip, buffer, 0, buffer.Length);

                    // Format the PCM raw sound data.
                    FormatSound(stream, capture, buffer, soundCaptureType);

                    // Send the sound to the client.
                    captureHandler(stream);

                    // Empty the stream
                    stream.SetLength(0);

                }
                catch (Exception) { }
                finally
                {
                    if (ip != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(ip);
                        ip = IntPtr.Zero;
                    }
                }
            }

            try { }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Start sample sound capture only on request.
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="soundCaptureType">The sound capture type.</param>
        private void StartSampleSoundCaptureSingle(CaptureSoundSample capture, Action<MemoryStream> captureHandler, SoundCaptureType soundCaptureType = SoundCaptureType.Wav)
        {
            MemoryStream stream = new MemoryStream();
            byte[] buffer = null;
            int sampleSize = 0;
            IntPtr ip = IntPtr.Zero;

            try
            {
                // capture image
                ip = capture.GetSnapshotSoundPointer();

                // Allocate the memory byte size.
                sampleSize = (int)capture.Channels * capture.SampleRate;
                buffer = new byte[sampleSize];

                // Copy the sound data to the buffer.
                Marshal.Copy(ip, buffer, 0, buffer.Length);

                // Format the PCM raw sound data.
                FormatSound(stream, capture, buffer, soundCaptureType);

                // Send the sound to the client.
                captureHandler(stream);

                // Empty the stream
                stream.SetLength(0);
                
            }
            catch (Exception) { }
            finally
            {
                if (ip != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(ip);
                    ip = IntPtr.Zero;
                }

                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Format the raw PCM sound data.
        /// </summary>
        /// <param name="stream">The memory stream that will contain the sound data.</param>
        /// <param name="capture">The capture provider.</param>
        /// <param name="rawPCMSound">The raw PCM sound data.</param>
        /// <param name="soundCaptureType">The sound capture type.</param>
        private void FormatSound(MemoryStream stream, CaptureSoundSample capture, byte[] rawPCMSound, SoundCaptureType soundCaptureType = SoundCaptureType.Wav)
        {
            switch (soundCaptureType)
            {
                case SoundCaptureType.Pcm:
                    // Write the raw PCm sound data.
                    stream.Write(rawPCMSound, 0, rawPCMSound.Length);
                    stream.Flush();
                    break;

                case SoundCaptureType.Wav:
                default:
                    // Write the wave formatted data from the raw PCM sound data.
                    WaveStructure waveStructure = WaveStructure.CreateDefaultStructure(capture.Channels, capture.SampleRate, capture.BitsPerSample, rawPCMSound);
                    WaveFormat waveFormat = new WaveFormat();
                    waveFormat.Write(stream, waveStructure);
                    break;
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_capture != null)
                        _capture.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _capture = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SoundCapture()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}
}
