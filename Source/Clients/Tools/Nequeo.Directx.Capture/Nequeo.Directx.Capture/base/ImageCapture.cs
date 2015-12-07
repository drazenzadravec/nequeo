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

namespace Nequeo.Directx
{
    /// <summary>
    /// Image capture provider.
    /// </summary>
    public partial class ImageCapture : IDisposable
	{
        /// <summary>
        /// Create a new image capture object. 
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        public ImageCapture(CaptureImageSample capture)
        {
            if (capture == null)
                throw new ArgumentException("The capture parameter must be set to a valid Filter.\n");

            _capture = capture;
            OnCreated();
        }

        private CaptureImageSample _capture = null;
        private bool _stop = true;
        
        /// <summary>
        /// Gets the capture provider.
        /// </summary>
        public CaptureImageSample Capture
        {
            get { return _capture; }
        }

        /// <summary>
        /// Gets the snapshot image width.
        /// </summary>
        public int Width
        {
            get { return _capture.Width; }
        }

        /// <summary>
        /// Gets the snapshot image height.
        /// </summary>
        public int Height
        {
            get { return _capture.Height; }
        }

        /// <summary>
        /// Gets the snapshot image stride.
        /// </summary>
        public int Stride
        {
            get { return _capture.Stride; }
        }

        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        public double FrameRate
        {
            get { return _capture.Capture.FrameRate; }
            set { _capture.Capture.FrameRate = value; }
        }

        /// <summary>
        /// Find the appropriate encoder.
        /// </summary>
        /// <param name="mimeType">The mime type to search for.</param>
        /// <returns>The image codec info.</returns>
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the frame size.
        /// </summary>
        public Size FrameSize
        {
            get { return _capture.Capture.FrameSize; }
            set { _capture.Capture.FrameSize = value; }
        }

        /// <summary>
        /// Start the capture single process.
        /// </summary>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="imageCaptureType">The image capture type.</param>
        public void StartSingle(Action<MemoryStream> captureHandler, ImageCaptureType imageCaptureType = ImageCaptureType.Bmp)
        {
            ImageCodecInfo imageCodecInfo = null;
            EncoderParameters encoderParameters = null;
            EncoderParameter encoderParameter;

            // Select the image capture type.
            switch (imageCaptureType)
            {
                case ImageCaptureType.Jpg:
                    imageCodecInfo = GetEncoderInfo("image/jpeg");
                    encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    break;

                case ImageCaptureType.Bmp:
                    imageCodecInfo = GetEncoderInfo("image/bmp");
                    encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)0);
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    break;
            }

            _stop = false;
            StartSampleImageCaptureSingle(_capture, captureHandler, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Start the capture continuous process.
        /// </summary>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="imageCodecInfo">The System.Drawing.Imaging.ImageCodecInfo class provides the necessary storage
        ///  members and methods to retrieve all pertinent information about the installed
        ///  image encoders and decoders (called codecs)</param>
        /// <param name="encoderParameters">Encapsulates an array of System.Drawing.Imaging.EncoderParameter objects.</param>
        public void StartSingle(Action<MemoryStream> captureHandler, ImageCodecInfo imageCodecInfo, EncoderParameters encoderParameters)
        {
            _stop = false;
            StartSampleImageCaptureSingle(_capture, captureHandler, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Start the capture single process.
        /// </summary>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="imageCaptureType">The image capture type.</param>
        public void StartContinuous(Action<MemoryStream> captureHandler, ImageCaptureType imageCaptureType = ImageCaptureType.Bmp)
        {
            ImageCodecInfo imageCodecInfo = null;
            EncoderParameters encoderParameters = null;
            EncoderParameter encoderParameter;

            // Select the image capture type.
            switch(imageCaptureType)
            {
                case ImageCaptureType.Jpg:
                    imageCodecInfo = GetEncoderInfo("image/jpeg");
                    encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    break;

                case ImageCaptureType.Bmp:
                    imageCodecInfo = GetEncoderInfo("image/bmp");
                    encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)0);
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    break;
            }

            _stop = false;
            StartSampleImageCaptureContinuous(_capture, captureHandler, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Start the capture continuous process.
        /// </summary>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="imageCodecInfo">The System.Drawing.Imaging.ImageCodecInfo class provides the necessary storage
        ///  members and methods to retrieve all pertinent information about the installed
        ///  image encoders and decoders (called codecs)</param>
        /// <param name="encoderParameters">Encapsulates an array of System.Drawing.Imaging.EncoderParameter objects.</param>
        public void StartContinuous(Action<MemoryStream> captureHandler, ImageCodecInfo imageCodecInfo, EncoderParameters encoderParameters)
        {
            _stop = false;
            StartSampleImageCaptureContinuous(_capture, captureHandler, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Stop the capture process
        /// </summary>
        public void Stop()
        {
            _stop = true;
        }

        /// <summary>
        /// Start sample image capture continuous.
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="imageCodecInfo">The System.Drawing.Imaging.ImageCodecInfo class provides the necessary storage
        ///  members and methods to retrieve all pertinent information about the installed
        ///  image encoders and decoders (called codecs)</param>
        /// <param name="encoderParameters">Encapsulates an array of System.Drawing.Imaging.EncoderParameter objects.</param>
        private void StartSampleImageCaptureContinuous(CaptureImageSample capture, Action<MemoryStream> captureHandler, ImageCodecInfo imageCodecInfo, EncoderParameters encoderParameters)
        {
            MemoryStream stream = new MemoryStream(20000);
            Bitmap image = null;
            IntPtr ip = IntPtr.Zero;

            // While not shutting down, and still capturing.
            while (!_stop)
            {
                try
                {
                    // capture image
                    ip = capture.GetSnapshotImagePointer();
                    image = new Bitmap(capture.Width, capture.Height, capture.Stride, PixelFormat.Format24bppRgb, ip);
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    image.Save(stream, imageCodecInfo, encoderParameters);

                    // Send the image to the client.
                    captureHandler(stream);

                    // Empty the stream
                    stream.SetLength(0);

                    // remove the image from memory
                    image.Dispose();
                    image = null;
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

                if (image != null)
                    image.Dispose();
            }
        }

        /// <summary>
        /// Start sample image capture only on request.
        /// </summary>
        /// <param name="capture">The capture provider.</param>
        /// <param name="captureHandler">The capture method where image data is to be written to.</param>
        /// <param name="imageCodecInfo">The System.Drawing.Imaging.ImageCodecInfo class provides the necessary storage
        ///  members and methods to retrieve all pertinent information about the installed
        ///  image encoders and decoders (called codecs)</param>
        /// <param name="encoderParameters">Encapsulates an array of System.Drawing.Imaging.EncoderParameter objects.</param>
        private void StartSampleImageCaptureSingle(CaptureImageSample capture, Action<MemoryStream> captureHandler, ImageCodecInfo imageCodecInfo, EncoderParameters encoderParameters)
        {
            MemoryStream stream = new MemoryStream(20000);
            Bitmap image = null;
            IntPtr ip = IntPtr.Zero;

            try
            {
                // capture image
                ip = capture.GetSnapshotImagePointer();
                image = new Bitmap(capture.Width, capture.Height, capture.Stride, PixelFormat.Format24bppRgb, ip);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                image.Save(stream, imageCodecInfo, encoderParameters);

                // Send the image to the client.
                captureHandler(stream);

                // Empty the stream
                stream.SetLength(0);

                // remove the image from memory
                image.Dispose();
                image = null;
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

                if (image != null)
                    image.Dispose();
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
                    if(_capture != null)
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
        ~ImageCapture()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}
}
