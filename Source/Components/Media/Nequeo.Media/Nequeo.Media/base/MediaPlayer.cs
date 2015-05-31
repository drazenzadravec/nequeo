/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Media
{
    /// <summary>
    /// Video audio media player.
    /// </summary>
    public class MediaPlayer : IDisposable
    {
        /// <summary>
        /// Video audio media player.
        /// </summary>
        public MediaPlayer()
        { }

        private Nequeo.IO.Audio.WavePlayer _audioPlayer = null;
        private Nequeo.IO.Audio.Device _device;

        private VideoAudioMux _mux = null;
        private BinaryReader _binaryReader = null;

        private Control _videoWindow = null;
        private Stream _mediaStream = null;
        private bool _isInternalStream = false;

        private TimeSpan _duration;
        private TimeSpan _position;
        private ushort _volume = 0;

        /// <summary>
        /// Gets or sets the control where the video is to be displayed.
        /// </summary>
        public Control VideoWindow
        {
            get { return _videoWindow; }
            set { _videoWindow = value; }
        }

        /// <summary>
        /// Gets the total duration of the media.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
        }

        /// <summary>
        /// Gets or sets the position within the media.
        /// </summary>
        public TimeSpan Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Gets or sets the system volume (between 0 and 10);
        /// </summary>
        public ushort Volume
        {
            get 
            {
                // Get the volume.
                _volume = Nequeo.IO.Audio.Volume.GetVolume();
                return _volume; 
            }
            set 
            {
                Nequeo.IO.Audio.Volume.SetVolume(value);
                _volume = Nequeo.IO.Audio.Volume.GetVolume(); 
            }
        }

        /// <summary>
        /// Close and release all resources of the media.
        /// </summary>
        public void Close()
        {
            // If the stream was created internally.
            if (_isInternalStream)
            {
                // Dispose managed resources.
                if (_mediaStream != null)
                    _mediaStream.Dispose();
            }

            // Cleanup
            if (_binaryReader != null)
                _binaryReader.Dispose();

            // Cleanup
            if (_audioPlayer != null)
                _audioPlayer.Dispose();

            if (_isInternalStream)
            {
                _mediaStream = null;
            }

            _binaryReader = null;
            _audioPlayer = null;
        }

        /// <summary>
        /// Stop playing the media.
        /// </summary>
        public void Stop()
        {


            if (_audioPlayer != null)
                _audioPlayer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaFilename"></param>
        public void Open(string mediaFilename)
        {
            _isInternalStream = true;

            // Create the binary reader.
            _mediaStream = new FileStream(mediaFilename, FileMode.Open, FileAccess.Read);
            _binaryReader = new BinaryReader(_mediaStream);
            _mux = new VideoAudioMux(_binaryReader);

            // Create the audio player.
            _device = Nequeo.IO.Audio.Devices.GetDevice(0);
            _audioPlayer = new IO.Audio.WavePlayer(_device);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaStream"></param>
        public void Open(Stream mediaStream)
        {
            _isInternalStream = false;
            _mediaStream = mediaStream;

            // Create the binary reader.
            _binaryReader = new BinaryReader(_mediaStream);
            _mux = new VideoAudioMux(_binaryReader);

            // Create the audio player.
            _device = Nequeo.IO.Audio.Devices.GetDevice(0);
            _audioPlayer = new IO.Audio.WavePlayer(_device);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {


            
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {

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
                    // If the stream was created internally.
                    if (_isInternalStream)
                    {
                        // Dispose managed resources.
                        if (_mediaStream != null)
                            _mediaStream.Dispose();
                    }

                    if (_binaryReader != null)
                        _binaryReader.Dispose();

                    if (_audioPlayer != null)
                        _audioPlayer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If the stream was created internally.
                if (_isInternalStream)
                {
                    _mediaStream = null;
                }

                _binaryReader = null;
                _audioPlayer = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MediaPlayer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
