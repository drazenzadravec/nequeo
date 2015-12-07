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

using Nequeo.Media;
using Nequeo.IO.Compression;
using Nequeo.Directx.Utility;

namespace Nequeo.Directx
{
    /// <summary>
    /// Image and sound sample capture provider.
    /// </summary>
    public class SampleCapture : IDisposable
    {
        /// <summary>
        /// Image and sound sample capture provider.
        /// </summary>
        /// <param name="videoDevice">The video capture device.</param>
        /// <param name="audioDevice">The audio capture device.</param>
        /// <param name="mux">Video audio encoder.</param>
        public SampleCapture(Device videoDevice, Device audioDevice, VideoAudioMux mux)
        {
            if (videoDevice == null && audioDevice == null)
                throw new ArgumentException("The videoDevice and/or the audioDevice parameter must be set to a valid Filter.\n");

            if (mux == null)
                throw new ArgumentException("The mux parameter must be set to a valid Filter.\n");

            _capture = new Capture(videoDevice, audioDevice);
            _mux = mux;

            if (videoDevice != null) _hasVideoDevice = true;
            if (audioDevice != null) _hasAudioDevice = true;

            // What active type is present.
            if (_hasVideoDevice && _hasAudioDevice)
                _active = MediaActiveType.Video | MediaActiveType.Audio;
            else if (_hasVideoDevice)
                _active = MediaActiveType.Video;
            else if (_hasAudioDevice)
                _active = MediaActiveType.Audio;
        }

        private object _lockObject = new object();

        private Capture _capture = null;
        private VideoAudioMux _mux = null;
        private VideoAudio _block = new VideoAudio();
        private MediaActiveType _active = MediaActiveType.Video | MediaActiveType.Audio;
        private CompressionAlgorithmStreaming _compressionAlgorithm = CompressionAlgorithmStreaming.None;

        private bool _isPaused = false;
        private bool _hasHeaders = false;
        private bool _isLiveStreaming = false;
        private bool _hasVideoDevice = false;
        private bool _hasAudioDevice = false;

        private long _currentVideoFrame = 0;

        private ImageCaptureType _imageType = ImageCaptureType.Jpg;
        private SoundCaptureType _soundType = SoundCaptureType.Wav;

        private Nequeo.Directx.ImageCapture _imageCapture = null;
        private Nequeo.Directx.SoundCapture _soundCapture = null;

        private Action _writeImageComplete = null;
        private Action _writeSoundComplete = null;

        /// <summary>
        /// Gets the video and audio devices collection.
        /// </summary>
        public static Nequeo.Directx.Utility.Devices Devices
        {
            get { return new Nequeo.Directx.Utility.Devices(); }
        }

        /// <summary>
        /// Gets the video audio mux encoder.
        /// </summary>
        public VideoAudioMux Mux
        {
            get { return _mux; }
        }

        /// <summary>
        /// Gets or sets the compression algorithm.
        /// </summary>
        public CompressionAlgorithmStreaming CompressionAlgorithm
        {
            get { return _compressionAlgorithm; }
            set { _compressionAlgorithm = value; }
        }

        /// <summary>
        /// Gets or sets the action when the image has been written is complete.
        /// </summary>
        public Action WriteImageComplete
        {
            get { return _writeImageComplete; }
            set { _writeImageComplete = value; }
        }

        /// <summary>
        /// Gets or sets the action when the sound has been written is complete.
        /// </summary>
        public Action WriteSoundComplete
        {
            get { return _writeSoundComplete; }
            set { _writeSoundComplete = value; }
        }

        /// <summary>
        /// Gets or sets the media active type.
        /// </summary>
        public MediaActiveType MediaActive
        {
            get { return _active; }
            set
            {
                if (_hasVideoDevice && _hasAudioDevice)
                    _active = value;
                else if (_hasVideoDevice)
                    _active = MediaActiveType.Video;
                else if (_hasAudioDevice)
                    _active = MediaActiveType.Audio;
            }
        }

        /// <summary>
        /// Gets or sets live streaming; if true then the captured data
        /// is sent to a live stream; else false captured data is sent
        /// to a file or memory stream.
        /// </summary>
        public bool IsLiveStreaming
        {
            get { return _isLiveStreaming; }
            set { _isLiveStreaming = value; }
        }

        /// <summary>
        /// Gets or sets the image capture type.
        /// </summary>
        public ImageCaptureType ImageCaptureType
        {
            get { return _imageType; }
            set { _imageType = value; }
        }

        /// <summary>
        /// Gets or sets the sound capture type.
        /// </summary>
        public SoundCaptureType SoundCaptureType
        {
            get { return _soundType; }
            set { _soundType = value; }
        }

        /// <summary>
        ///  Gets and sets the frame rate used to capture video.
        /// </summary>
        /// <remarks>
        ///  Common frame rates: 24 fps for film, 25 for PAL, 29.997
        ///  for NTSC. Not all NTSC capture cards can capture at 
        ///  exactly 29.997 fps. Not all frame rates are supported. 
        ///  When changing the frame rate, the closest supported 
        ///  frame rate will be used. 
        /// </remarks>
        public double VideoFrameRate
        {
            get { return _capture.FrameRate; }
            set { _capture.FrameRate = value; }
        }

        /// <summary>
        ///  Gets and sets the frame size used to capture video.
        /// </summary>
        /// <remarks>
        ///  To change the frame size, assign a new Size object 
        ///  to this property <code>capture.Size = new Size( w, h );</code>
        ///  rather than modifying the size in place 
        ///  (capture.Size.Width = w;). Not all frame
        ///  rates are supported.
        /// </remarks>
        public Size VideoFrameSize
        {
            get { return _capture.FrameSize; }
            set { _capture.FrameSize = value; }
        }

        /// <summary>
        ///  Get or set the number of channels in the waveform-audio data. 
        /// </summary>
        /// <remarks>
        ///  Monaural data uses one channel and stereo data uses two channels. 
        /// </remarks>
        public short AudioChannels
        {
            get { return _capture.AudioChannels; }
            set { _capture.AudioChannels = value; }
        }

        /// <summary>
        ///  Get or set the number of audio samples taken per second.
        /// </summary>
        /// <remarks>
        ///  Common sampling rates are 8.0 kHz, 11.025 kHz, 22.05 kHz, and 
        ///  44.1 kHz. Not all sampling rates are supported.
        /// </remarks>
        public int AudioSamplingRate
        {
            get { return _capture.AudioSamplingRate; }
            set { _capture.AudioSamplingRate = value; }
        }

        /// <summary>
        ///  Get or set the number of bits recorded per sample. 
        /// </summary>
        /// <remarks>
        ///  Common sample sizes are 8 bit and 16 bit. Not all
        ///  samples sizes are supported.
        /// </remarks>
        public short AudioSampleSize
        {
            get { return _capture.AudioSampleSize; }
            set { _capture.AudioSampleSize = value; }
        }

        /// <summary>
        /// Initialise the capture devices.
        /// </summary>
        public void Initialise()
        {
            try
            {
                // If not capturing.
                if (!_capture.Capturing)
                {
                    // Build the graph.
                    if (!_capture.Cued)
                        _capture.Cue();
                }
            }
            catch { }
        }

        /// <summary>
        /// Start the capture process.
        /// </summary>
        public void Start()
        {
            try
            {
                // If not capturing.
                if (!_capture.Capturing)
                {
                    // If headers have not been written.
                    if (!_hasHeaders)
                    {
                        // If in live streaming mode then
                        // there is no need to set the duration.
                        if (!_isLiveStreaming)
                        {
                            // Write the header initially.
                            _hasHeaders = true;

                            VideoHeader videoHeader = new VideoHeader();
                            AudioHeader audioHeader = new AudioHeader();
                            VideoAudioHeader headers = new VideoAudioHeader();

                            // Select what needs to be captured.
                            switch (_active)
                            {
                                case MediaActiveType.Video | MediaActiveType.Audio:
                                    // Video and audio capture.
                                    videoHeader.ContainsVideo = true;
                                    videoHeader.Duration = 0.0;
                                    videoHeader.FrameRate = VideoFrameRate;
                                    videoHeader.FrameSizeHeight = VideoFrameSize.Height;
                                    videoHeader.FrameSizeWidth = VideoFrameSize.Width;
                                    videoHeader.ImageType = _imageType;
                                    videoHeader.CompressionAlgorithm = _compressionAlgorithm;

                                    audioHeader.ContainsAudio = true;
                                    audioHeader.Channels = AudioChannels;
                                    audioHeader.Duration = 0.0;
                                    audioHeader.SampleSize = AudioSampleSize;
                                    audioHeader.SamplingRate = AudioSamplingRate;
                                    audioHeader.SoundType = _soundType;
                                    audioHeader.CompressionAlgorithm = _compressionAlgorithm;
                                    break;

                                case MediaActiveType.Video:
                                    // Video capture.
                                    videoHeader.ContainsVideo = true;
                                    videoHeader.Duration = 0.0;
                                    videoHeader.FrameRate = VideoFrameRate;
                                    videoHeader.FrameSizeHeight = VideoFrameSize.Height;
                                    videoHeader.FrameSizeWidth = VideoFrameSize.Width;
                                    videoHeader.ImageType = _imageType;
                                    videoHeader.CompressionAlgorithm = _compressionAlgorithm;
                                    break;

                                case MediaActiveType.Audio:
                                    // Audio capture.
                                    audioHeader.ContainsAudio = true;
                                    audioHeader.Channels = AudioChannels;
                                    audioHeader.Duration = 0.0;
                                    audioHeader.SampleSize = AudioSampleSize;
                                    audioHeader.SamplingRate = AudioSamplingRate;
                                    audioHeader.SoundType = _soundType;
                                    audioHeader.CompressionAlgorithm = _compressionAlgorithm;
                                    break;
                            }

                            // Add the header.
                            headers.MediaFormat = Nequeo.Media.Streaming.MediaFormat;
                            headers.Video = videoHeader;
                            headers.Audio = audioHeader;
                            _mux.WriteHeader(headers);
                        }
                    }

                    // Build the graph.
                    if (!_capture.Cued)
                        _capture.Cue();

                    // Select what needs to be captured.
                    switch (_active)
                    {
                        case MediaActiveType.Video | MediaActiveType.Audio:
                            // Video and audio capture.
                            // Start sample capture.
                            _capture.StartSnapshotImageSound();

                            // Create the samplers.
                            ImageSampler();
                            SoundSampler();
                            break;

                        case MediaActiveType.Video:
                            // Video capture.
                            // Start sample capture.
                            _capture.StartSnapshotImage();

                            // Create the samplers.
                            ImageSampler();
                            break;

                        case MediaActiveType.Audio:
                            // Audio capture.
                            // Start sample capture.
                            _capture.StartSnapshotSound();

                            // Create the samplers.
                            SoundSampler();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    // If the engine has been created.
                    if (_capture != null)
                        _capture.Stop();

                    if (_imageCapture != null)
                    {
                        _imageCapture.Stop();
                        _imageCapture.Dispose();
                    }

                    if (_soundCapture != null)
                    {
                        _soundCapture.Stop();
                        _soundCapture.Dispose();
                    }

                    _imageCapture = null;
                    _soundCapture = null;
                }
                catch { }
            }

            // Un pause.
            _isPaused = false;
        }

        /// <summary>
        /// Stop the capture process.
        /// </summary>
        public void Stop()
        {
            try
            {
                // If capturing.
                if (_capture.Capturing)
                {
                    // Stop the capture.
                    _capture.Stop();

                    if (_imageCapture != null)
                        _imageCapture.Stop();

                    if (_soundCapture != null)
                        _soundCapture.Stop();

                    // If not paused then stop the capture.
                    if(!_isPaused)
                    {
                        if (_imageCapture != null)
                            _imageCapture.Dispose();

                        if (_soundCapture != null)
                            _soundCapture.Dispose();

                        _imageCapture = null;
                        _soundCapture = null;

                        // Un pause.
                        _isPaused = false;

                        // If in live streaming mode then
                        // there is no need to set the duration.
                        if (!_isLiveStreaming)
                        {
                            // Read the header and set the duration of the video and audio.
                            VideoAudioHeader headers = _mux.ReadHeader();
                            VideoHeader videoHeader = (headers.Video.HasValue ? headers.Video.Value : new VideoHeader());
                            AudioHeader audioHeader = (headers.Audio.HasValue ? headers.Audio.Value : new AudioHeader());

                            // Get the video and audio duration.
                            double videoDuration = _mux.VideoDuration;
                            double audioDuration = _mux.AudioDuration;

                            // Select what needs to be captured.
                            switch (_active)
                            {
                                case MediaActiveType.Video | MediaActiveType.Audio:
                                    // Video and audio capture.
                                    videoHeader.Duration = videoDuration;
                                    audioHeader.Duration = audioDuration;
                                    break;

                                case MediaActiveType.Video:
                                    // Video capture.
                                    videoHeader.Duration = videoDuration;
                                    break;

                                case MediaActiveType.Audio:
                                    // Audio capture.
                                    audioHeader.Duration = audioDuration;
                                    break;
                            }

                            // Write the header.
                            headers.Video = videoHeader;
                            headers.Audio = audioHeader;
                            _mux.WriteHeader(headers);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Pause the capture process.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
            Stop();
        }

        /// <summary>
        /// Create the image sampler.
        /// </summary>
        private void ImageSampler()
        {
            // Get the capture sampler.
            if (_imageCapture == null)
                _imageCapture = new ImageCapture(_capture.CaptureImageSample);

            // Set the method that is used for sampling.
            _imageCapture.ImageCaptureThreadContext.Execute(a => a.StartContinuous(u => GetImageData(u), _imageType));
        }

        /// <summary>
        /// Create the sound sampler.
        /// </summary>
        private void SoundSampler()
        {
            // Get the capture sampler.
            if (_soundCapture == null)
                _soundCapture = new SoundCapture(_capture.CaptureSoundSample);

            // Set the method that is used for sampling.
            _soundCapture.SoundCaptureThreadContext.Execute(a => a.StartContinuous(u => GetSoundData(u), _soundType));
        }

        /// <summary>
        /// Get image sample.
        /// </summary>
        /// <param name="stream">The stream containg the image data.</param>
        private void GetImageData(System.IO.MemoryStream stream)
        {
            // Only one thread can write to
            // the dtream at one time.
            lock (_lockObject)
            {
                MemoryStream zip = null;
                byte[] data = null;

                try
                {
                    _block.Video = null;
                    _block.Audio = null;

                    // Get the current video frame.
                    _currentVideoFrame = _mux.TotalVideoFrames;

                    // If compression is used.
                    switch(_compressionAlgorithm)
                    {
                        case CompressionAlgorithmStreaming.GZip:
                        case CompressionAlgorithmStreaming.ZLib:
                            // Zip stream
                            zip = new MemoryStream();
                            Nequeo.IO.Compression.Compresss.Compress(stream, zip, _compressionAlgorithm);
                            data = zip.ToArray();
                            break;
                        default:
                            data = stream.ToArray();
                            break;
                    }
                    
                    // Create the image sample.
                    _block.Video = new ImageModel[1] 
                    { 
                        // Add the image block.
                        new ImageModel()
                        {
                            Size = 1,
                            Data = data
                        }
                    };

                    // Write the audio block.
                    _mux.Write(_block);

                    // Write complete.
                    if (_writeImageComplete != null)
                        _writeImageComplete();
                }
                catch { }
                finally
                {
                    if (zip != null)
                    {
                        zip.Close();
                        zip.Dispose();
                    }
                }

                // Release the data.
                data = null;
            }
        }

        /// <summary>
        /// Get sound sample.
        /// </summary>
        /// <param name="stream">The stream containg the sound data.</param>
        private void GetSoundData(System.IO.MemoryStream stream)
        {
            // Only one thread can write to
            // the dtream at one time.
            lock (_lockObject)
            {
                MemoryStream zip = null;
                byte[] data = null;

                try
                {
                    _block.Video = null;
                    _block.Audio = null;

                    // If compression is used.
                    switch (_compressionAlgorithm)
                    {
                        case CompressionAlgorithmStreaming.GZip:
                        case CompressionAlgorithmStreaming.ZLib:
                            // Zip stream
                            zip = new MemoryStream();
                            Nequeo.IO.Compression.Compresss.Compress(stream, zip, _compressionAlgorithm);
                            data = zip.ToArray();
                            break;
                        default:
                            data = stream.ToArray();
                            break;
                    }

                    // Create the sound sample.
                    _block.Audio = new SoundModel[1] 
                    { 
                        // Add the sound block.
                        new SoundModel()
                        {
                            StartAtFrameIndex = (int)_currentVideoFrame,
                            Size = 1,
                            Data = data
                        }
                    };

                    // Write the audio block.
                    _mux.Write(_block);

                    // Write complete.
                    if (_writeSoundComplete != null)
                        _writeSoundComplete();
                }
                catch { }
                finally
                {
                    if (zip != null)
                    {
                        zip.Close();
                        zip.Dispose();
                    }
                }

                // Release the data.
                data = null;
            }
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_capture != null)
                        _capture.Dispose();

                    if (_imageCapture != null)
                        _imageCapture.Dispose();

                    if (_soundCapture != null)
                        _soundCapture.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _lockObject = null;
                _capture = null;
                _imageCapture = null;
                _soundCapture = null;

                try
                {
                    _block.Audio = null;
                    _block.Video = null;
                }
                catch { }
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SampleCapture()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
