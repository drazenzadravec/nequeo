/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
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
using Nequeo.IO.Audio.Formats;
using Nequeo.IO.Audio.Utils;
using Nequeo.IO.Audio.Provider;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Controls playback of a sound stream.
    /// </summary>
    public class WavePlayer : IDisposable
    {
        /// <summary>
        /// Controls playback of a sound stream.
        /// </summary>
        /// <param name="device">The playback device.</param>
        public WavePlayer(Device device)
        {
            _device = device;
            _syncContext = SynchronizationContext.Current;
            _callback = new WaveInterop.WaveCallback(Callback);
            _callbackInfo = WaveCallbackInfo.NewWindow();
            _callbackInfo.Connect(_callback);
        }

        private AudioReader _audioReader = null;
        private System.IO.Stream _audioStream = null;
        private AudioFormat _audioFormat = AudioFormat.Wav;
        private string _filename = null;
        private bool _internalStream = false;
        private TimeSpan _duration = new TimeSpan(0, 0, 0);

        private IntPtr _hWaveOut;
        private object _waveOutLock = new object();
        private WaveOutBuffer[] _buffers = null;
        private WaveCallbackInfo _callbackInfo = null;
        private SynchronizationContext _syncContext;
        private WaveInterop.WaveCallback _callback;
        private IWaveProvider _waveProvider;
        private int _queuedBuffers = 0;

        private int _desiredLatency = 300;
        private int _numberOfBuffers = 2;
        private float _volume = 1.0F;
        private Device _device;
        private volatile PlaybackState _playbackState = PlaybackState.Stopped;
        private volatile bool _pressStop = true;

        /// <summary>
        /// Indicates playback has stopped automatically
        /// </summary>
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        /// <summary>
        /// Gets or sets the desired latency in milliseconds
        /// Should be set before a call to Init
        /// </summary>
        public int DesiredLatency 
        {
            get { return _desiredLatency; }
            set { _desiredLatency = value; }
        }

        /// <summary>
        /// Gets or sets the number of buffers used
        /// Should be set before a call to Init
        /// </summary>
        public int NumberOfBuffers 
        {
            get { return _numberOfBuffers; }
            set { _numberOfBuffers = value; }
        }

        /// <summary>
        /// Gets the playback State.
        /// </summary>
        public PlaybackState PlaybackState
        {
            get { return _playbackState; }
        }

        /// <summary>
        /// Gets the total audio duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
        }

        /// <summary>
        /// Gets os sets the current position in the stream.
        /// </summary>
        public virtual TimeSpan CurrentTime
        {
            get 
            {
                if (_audioReader != null)
                    return _audioReader.CurrentTime;
                else
                    return new TimeSpan(0, 0, 0);
            }
            set 
            {
                if (_audioReader != null)
                    _audioReader.CurrentTime = value; 
            }
        }

        /// <summary>
        /// Gets or sets the volume for this device 1.0 is full scale
        /// </summary>
        public float Volume
        {
            get { return _volume; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
                if (value > 1) throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
                _volume = value;
                float left = _volume;
                float right = _volume;

                int stereoVolume = (int)(left * 0xFFFF) + ((int)(right * 0xFFFF) << 16);
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutSetVolume(_hWaveOut, stereoVolume);
                }
                MmException.Try(result, "waveOutSetVolume");
            }
        }

        /// <summary>
        /// Gets the current position in bytes from the wave output device.
        /// (n.b. this is not the same thing as the position within your reader
        /// stream - it calls directly into waveOutGetPosition)
        /// </summary>
        /// <returns>Position in bytes</returns>
        public long GetPosition()
        {
            lock (_waveOutLock)
            {
                MmTime mmTime = new MmTime();
                mmTime.wType = MmTime.TIME_BYTES; // request results in bytes, TODO: perhaps make this a little more flexible and support the other types?
                MmException.Try(WaveInterop.waveOutGetPosition(_hWaveOut, out mmTime, Marshal.SizeOf(mmTime)), "waveOutGetPosition");

                if (mmTime.wType != MmTime.TIME_BYTES)
                    throw new Exception(string.Format("waveOutGetPosition: wType -> Expected {0}, Received {1}", MmTime.TIME_BYTES, mmTime.wType));

                return mmTime.cb;
            }
        }

        /// <summary>
        /// Stop and reset the WaveOut device.
        /// </summary>
        public void Stop()
        {
            if (_playbackState != PlaybackState.Stopped)
            {
                _pressStop = true;

                // in the call to waveOutReset with function callbacks
                // some drivers will block here until OnDone is called
                // for every buffer
                _playbackState = PlaybackState.Stopped; // set this here to avoid a problem with some drivers whereby 

                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutReset(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutReset");
                }

                // With function callbacks, waveOutReset will call OnDone,
                // and so PlaybackStopped must not be raised from the handler
                // we know playback has definitely stopped now, so raise callback
                if (_callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback)
                {
                    RaisePlaybackStoppedEvent(null);
                }
            }
        }

        /// <summary>
        /// Resume playing after a pause from the same position
        /// </summary>
        public void Resume()
        {
            _pressStop = false;

            if (_playbackState == PlaybackState.Paused)
            {
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutRestart(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutRestart");
                }
                _playbackState = PlaybackState.Playing;
            }
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        public void Pause()
        {
            _pressStop = false;

            if (_playbackState == PlaybackState.Playing)
            {
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutPause(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutPause");
                }
                _playbackState = PlaybackState.Paused;
            }
        }

        /// <summary>
        /// Start playing the audio from the WaveStream.
        /// </summary>
        public void Play()
        {
            PlayEx();
        }

        /// <summary>
        /// Play
        /// </summary>
        private void PlayEx()
        {
            _pressStop = false;

            if (_playbackState == PlaybackState.Stopped)
            {
                // Open the audio reader.
                OpenReader();

                // Init the wave provider.
                Init();

                _playbackState = PlaybackState.Playing;
                EnqueueBuffers();
            }
            else if (_playbackState == PlaybackState.Paused)
            {
                EnqueueBuffers();
                Resume();
                _playbackState = PlaybackState.Playing;
            }
        }

        /// <summary>
        /// Open file or URL (supports .wav, .mp3, .aiff).
        /// </summary>
        /// <param name="filename">The file or network URL to open (e.g. http://, mms://, file://).</param>
        public void Open(string filename)
        {
            _internalStream = true;
            _filename = filename;
        }

        /// <summary>
        /// Open the audio stream for the specified format.
        /// </summary>
        /// <param name="audioStream">The audio stream.</param>
        /// <param name="audioFormat">The audio format of the stream.</param>
        public void Open(System.IO.Stream audioStream, AudioFormat audioFormat)
        {
            _internalStream = false;
            _audioStream = audioStream;
            _audioFormat = audioFormat;
        }

        /// <summary>
        /// Open the audio reader.
        /// </summary>
        private void OpenReader()
        {
            if (!_internalStream)
                _audioReader = new AudioReader(_audioStream, _audioFormat);
            else
                _audioReader = new AudioReader(_filename);

            // Get the initial values of the stream.
            _duration = _audioReader.TotalTime; 
        }

        /// <summary>
        /// Init the wave provider.
        /// </summary>
        private void Init()
        {
            // Create the channels
            SampleChannel channel = new SampleChannel(_audioReader, true);
            MeteringSampleProvider provider = new MeteringSampleProvider(channel);
            
            // Create the wave provider.
            _waveProvider = new SampleToWaveProvider(provider);

            // calculate the buffer size.
            int bufferSize = _waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);

            MmResult result;
            lock (_waveOutLock)
            {
                result = _callbackInfo.WaveOutOpen(out _hWaveOut, _device.Index, _waveProvider.WaveFormat, _callback);
            }
            MmException.Try(result, "waveOutOpen");

            _buffers = new WaveOutBuffer[NumberOfBuffers];
            _playbackState = PlaybackState.Stopped;

            for (int n = 0; n < NumberOfBuffers; n++)
            {
                _buffers[n] = new WaveOutBuffer(_hWaveOut, bufferSize, _waveProvider, _waveOutLock);
            }
        }

        /// <summary>
        /// Enqueue Buffers.
        /// </summary>
        private void EnqueueBuffers()
        {
            for (int n = 0; n < NumberOfBuffers; n++)
            {
                if (!_buffers[n].InQueue)
                {
                    if (_buffers[n].OnDone())
                    {
                        Interlocked.Increment(ref _queuedBuffers);
                    }
                    else
                    {
                        _playbackState = PlaybackState.Stopped;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Made non-static so that playing can be stopped here
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="uMsg"></param>
        /// <param name="dwInstance"></param>
        /// <param name="wavhdr"></param>
        /// <param name="dwReserved"></param>
        private void Callback(IntPtr hWaveOut, WaveMessage uMsg, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved)
        {
            if (uMsg == WaveMessage.WaveOutDone)
            {
                GCHandle hBuffer = (GCHandle)wavhdr.userData;
                WaveOutBuffer buffer = (WaveOutBuffer)hBuffer.Target;
                Interlocked.Decrement(ref _queuedBuffers);
                Exception exception = null;
                // check that we're not here through pressing stop
                if (PlaybackState == PlaybackState.Playing)
                {
                    // to avoid deadlocks in Function callback mode,
                    // we lock round this whole thing, which will include the
                    // reading from the stream.
                    // this protects us from calling waveOutReset on another 
                    // thread while a WaveOutWrite is in progress
                    lock (_waveOutLock)
                    {
                        try
                        {
                            if (buffer.OnDone())
                            {
                                Interlocked.Increment(ref _queuedBuffers);
                            }
                        }
                        catch (Exception e)
                        {
                            // one likely cause is soundcard being unplugged
                            exception = e;
                        }
                    }
                }
                if (_queuedBuffers == 0)
                {
                    if (_callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback && _playbackState == PlaybackState.Stopped)
                    {
                        // the user has pressed stop
                        // DO NOT raise the playback stopped event from here
                        // since on the main thread we are still in the waveOutReset function
                        // Playback stopped will be raised elsewhere
                    }
                    else
                    {
                        // set explicitly for when we reach the end of the audio.
                        _playbackState = PlaybackState.Stopped; 
                        RaisePlaybackStoppedEvent(exception, (_pressStop ? false : true));
                    }
                }
            }
        }

        /// <summary>
        /// Raise playback stopped event.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="audioComplete"></param>
        private void RaisePlaybackStoppedEvent(Exception e, bool audioComplete = false)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (_syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e, audioComplete));
                }
                else
                {
                    _syncContext.Post(state => handler(this, new StoppedEventArgs(e, audioComplete)), null);
                }
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
                Stop();

                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_buffers != null)
                    {
                        for (int n = 0; n < _buffers.Length; n++)
                        {
                            if (_buffers[n] != null)
                            {
                                _buffers[n].Dispose();
                            }
                        }
                    }

                    lock (_waveOutLock)
                    {
                        WaveInterop.waveOutClose(_hWaveOut);
                    }

                    if (_audioReader != null)
                        _audioReader.Dispose();

                    // If the stream was created internally.
                    if (_internalStream)
                    {
                        if (_audioStream != null)
                            _audioStream.Dispose();
                    }

                    // Close the callback.
                    _callbackInfo.Disconnect();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If the stream was created internally.
                _buffers = null;
                _audioReader = null;

                // If the stream was created internally.
                if (_internalStream)
                {
                    _audioStream = null;
                }
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WavePlayer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
