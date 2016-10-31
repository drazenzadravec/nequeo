/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Nequeo.IO.Audio.Wave;
using Nequeo.IO.Audio.Api;
using Nequeo.IO.Audio.Formats;
using Nequeo.IO.Audio.Utils;
using Nequeo.IO.Audio.Provider;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Controls recording of a sound stream.
    /// </summary>
    public class WaveRecorder : IDisposable
    {
        /// <summary>
        /// Controls recording of a sound stream.
        /// </summary>
        /// <param name="device">The recording device.</param>
        public WaveRecorder(Device device)
        {
            _device = device;
        }

        private Device _device;
        private MMDevice _mmDevice;
        private IWaveIn _waveIn;
        private WaveFileWriter _writer;

        private System.IO.Stream _audioStream = null;
        private AudioRecordingFormat _audioFormat = AudioRecordingFormat.WaveIn;
        private string _filename = null;
        private bool _internalStream = false;

        private object _waveInLock = new object();
        private bool _waveInCreated = false;
        private bool _recording = false;
        private bool _isBufferStream = false;
        private int _bufferMilliseconds = 100;

        /// <summary>
        /// Indicates recording has stopped automatically.
        /// </summary>
        public event EventHandler<StoppedEventArgs> RecordingStopped;

        /// <summary>
        /// Indicates recorded data is available 
        /// </summary>
        public event EventHandler<long> DataAvailable;

        /// <summary>
        /// Gets or sets milliseconds for the buffer. Recommended value is 100ms
        /// </summary>
        public int BufferMilliseconds
        {
            get{ return _bufferMilliseconds; }
            set{ _bufferMilliseconds = value; }
        }

        /// <summary>
        /// Gets or sets the microphone volume for this device 1.0 is full scale.
        /// </summary>
        public float MicrophoneVolume
        {
            get
            {
                if (_mmDevice != null)
                    return _mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                else
                    return 0.0f;
            }
            set
            {
                if (_mmDevice != null)
                    _mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
            }
        }

        /// <summary>
        /// Open the recorder.
        /// </summary>
        /// <param name="outStream">The stream to write the audio to.</param>
        /// <param name="format">The audio wave format.</param>
        /// <param name="audioRecordingFormat">The audio recording format.</param>
        public void Open(Nequeo.IO.Stream.StreamBufferBase outStream, WaveFormatProvider format = null, AudioRecordingFormat audioRecordingFormat = AudioRecordingFormat.WaveIn)
        {
            // If not created.
            if (!_waveInCreated)
            {
                _audioStream = outStream;
                _internalStream = false;
                _isBufferStream = true;

                // Initialise.
                Init(format, audioRecordingFormat);
                _waveInCreated = true;
            }
        }

        /// <summary>
        /// Open the recorder.
        /// </summary>
        /// <param name="outStream">The stream to write the audio to.</param>
        /// <param name="format">The audio wave format.</param>
        /// <param name="audioRecordingFormat">The audio recording format.</param>
        public void Open(System.IO.Stream outStream, WaveFormatProvider format = null, AudioRecordingFormat audioRecordingFormat = AudioRecordingFormat.WaveIn)
        {
            // If not created.
            if (!_waveInCreated)
            {
                _audioStream = outStream;
                _internalStream = false;
                _isBufferStream = false;

                // Initialise.
                Init(format, audioRecordingFormat);
                _waveInCreated = true;
            }
        }

        /// <summary>
        /// Open the recorder.
        /// </summary>
        /// <param name="filename">The path and filename of the file to write the audio to.</param>
        /// <param name="format">The audio wave format.</param>
        /// <param name="audioRecordingFormat">The audio recording format.</param>
        public void Open(string filename, WaveFormatProvider format = null, AudioRecordingFormat audioRecordingFormat = AudioRecordingFormat.WaveIn)
        {
            // If not created.
            if (!_waveInCreated)
            {
                _filename = filename;
                _audioStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                _internalStream = true;
                _isBufferStream = false;

                // Initialise.
                Init(format, audioRecordingFormat);
                _waveInCreated = true;
            }
        }

        /// <summary>
        /// Close the waveIn stream.
        /// </summary>
        public void Close()
        {
            // If created.
            if (_waveInCreated)
            {
                _waveInCreated = false;
                CleanUp();
            }
        }

        /// <summary>
        /// Stop and recording.
        /// </summary>
        public void Stop()
        {
            // If recording.
            if (_recording)
            {
                // If a waveIn stream has been created.
                if (_waveIn != null)
                {
                    // Stop recording.
                    _waveIn.StopRecording();
                    _recording = false;
                }
            }
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void Start()
        {
            // If created.
            if (_waveInCreated)
            {
                // If not recording.
                if (!_recording)
                {
                    // If a waveIn stream has been created.
                    if (_waveIn != null)
                    {
                        // If an output stream exists.
                        if (_audioStream != null)
                        {
                            // If not using buffer.
                            if (!_isBufferStream)
                            {
                                // Create the writer.
                                _writer = new WaveFileWriter(_audioStream, _waveIn.WaveFormat);
                            }

                            // Start capturing audio.
                            _waveIn.StartRecording();
                            _recording = true;
                        }
                    }
                }
            }
            else
                throw new Exception("Open a recording stream first.");
        }

        /// <summary>
        /// Initialise the waveIn stream.
        /// </summary>
        /// <param name="format">The audio wave format.</param>
        /// <param name="audioRecordingFormat">The audio recording format.</param>
        private void Init(WaveFormatProvider format, AudioRecordingFormat audioRecordingFormat)
        {
            _audioFormat = audioRecordingFormat;

            // Get all the catpure devices.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            // Select the device index.
            _mmDevice = devices[_device.Index];

            // If null the setup defaults.
            if (format == null)
            {
                // Select the provider.
                switch (audioRecordingFormat)
                {
                    case AudioRecordingFormat.WasapiLoopback:
                        _waveIn = new WasapiLoopbackCapture(_mmDevice);
                        break;

                    case AudioRecordingFormat.Wasapi:
                        _waveIn = new WasapiCapture(_mmDevice);
                        _waveIn.WaveFormat = new WaveFormatProvider(8000, 16, 1);
                        break;

                    case AudioRecordingFormat.WaveInEvent:
                        _waveIn = new WaveInEvent() { DeviceNumber = _device.Index, BufferMilliseconds = _bufferMilliseconds };
                        _waveIn.WaveFormat = new WaveFormatProvider(8000, 16, 1);
                        break;

                    case AudioRecordingFormat.WaveIn:
                    default:
                        _waveIn = new WaveIn() { DeviceNumber = _device.Index, BufferMilliseconds = _bufferMilliseconds };
                        _waveIn.WaveFormat = new WaveFormatProvider(8000, 16, 1);
                        break;
                }
            }
            else
            {
                // Select the provider.
                switch (audioRecordingFormat)
                {
                    case AudioRecordingFormat.WasapiLoopback:
                        _waveIn = new WasapiLoopbackCapture(_mmDevice);
                        break;

                    case AudioRecordingFormat.Wasapi:
                        _waveIn = new WasapiCapture(_mmDevice);
                        _waveIn.WaveFormat = format;
                        break;

                    case AudioRecordingFormat.WaveInEvent:
                        _waveIn = new WaveInEvent() { DeviceNumber = _device.Index, BufferMilliseconds = _bufferMilliseconds };
                        _waveIn.WaveFormat = format;
                        break;

                    case AudioRecordingFormat.WaveIn:
                    default:
                        _waveIn = new WaveIn() { DeviceNumber = _device.Index, BufferMilliseconds = _bufferMilliseconds };
                        _waveIn.WaveFormat = format;
                        break;
                }
            }

            // Set the capture.
            _waveIn.DataAvailable += _waveIn_DataAvailable;
            _waveIn.RecordingStopped += _waveIn_RecordingStopped;
        }

        /// <summary>
        /// Stop the recording.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            // Close the writer.
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }

            // If created.
            if (_waveInCreated)
            {
                // Stop recording.
                RecordingStopped?.Invoke(sender, new StoppedEventArgs(e.Exception, true));
            }
        }

        /// <summary>
        /// Audio sample is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // If not using buffer.
            if (!_isBufferStream)
            {
                if (_writer != null)
                {
                    // Write the audio data to the writer.
                    _writer.Write(e.Buffer, 0, e.BytesRecorded);
                }
            }
            else
            {
                // Write to buffer.
                _audioStream.Write(e.Buffer, 0, e.BytesRecorded);
                DataAvailable?.Invoke(this, (long)e.BytesRecorded);
            }
        }

        /// <summary>
        /// Cleanup.
        /// </summary>
        private void CleanUp()
        {
            // If recording.
            Stop();

            // Cleanup.
            if (_waveIn != null)
                _waveIn.Dispose();

            if (_writer != null)
                _writer.Dispose();

            // If the stream was created internally.
            if (_internalStream)
            {
                if (_audioStream != null)
                    _audioStream.Dispose();
            }

            // If the stream was created internally.
            if (_internalStream)
                _audioStream = null;

            _writer = null;
            _waveIn = null;
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
                    CleanUp();
                }

                // If the stream was created internally.
                if (_internalStream)
                    _audioStream = null;

                _writer = null;
                _waveIn = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WaveRecorder()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
