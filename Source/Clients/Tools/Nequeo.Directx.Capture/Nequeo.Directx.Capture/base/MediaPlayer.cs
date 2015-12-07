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
    /// Video and audio media play provider.
    /// </summary>
    public class MediaPlayer : IDisposable
	{
        /// <summary>
        /// Video and audio media play.
        /// </summary>
        public MediaPlayer()
        {
        }

        private const int WMGraphNotify = 0x0400 + 13;
        private const int VolumeFull = 0;
        private const int VolumeSilence = -10000;

        private IGraphBuilder _graphBuilder = null;
        private IMediaControl _mediaControl = null;
        private IMediaEventEx _mediaEventEx = null;
        private IVideoWindow _videoWindow = null;
        private IBasicAudio _basicAudio = null;
        private IBasicVideo _basicVideo = null;
        private IMediaSeeking _mediaSeeking = null;
        private IMediaPosition _mediaPosition = null;
        private IVideoFrameStep _frameStep = null;

        private IDvdControl2 _dvdControl;

        private string _filename = null;
        private bool _isAudioOnly = false;
        private bool _isFullScreen = false;
        private int _currentVolume = VolumeFull;
        private PlayState _currentState = PlayState.Init;
        private double _currentPlaybackRate = 1.0;

        private IntPtr _pDrain = IntPtr.Zero;
        private Control _playWindow = null;
        private int _videoHeight;
        private int _videoWidth;
        private int _multiplier;
        private int _divider;

        private static string _videoFiles = @"Video Files (*.avi; *.wmv; *.qt; *.mov; *.mpg; *.mpeg; *.m1v)|*.avi; *.wmv; *.qt; *.mov; *.mpg; *.mpeg; *.m1v|";
        private static string _audioFiles = @"Audio files (*.wav; *.wma; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd)|*.wav; *.wma; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd|";
        private static string _midiFiles = @"MIDI Files (*.mid, *.midi, *.rmi)|*.mid; *.midi; *.rmi|";
        private static string _imageFiles = @"Image Files (*.jpg, *.bmp, *.gif, *.tga)|*.jpg; *.bmp; *.gif; *.tga|";

        /// <summary>
        /// Gets or sets the window control where the media will be displayed.
        /// </summary>
        public Control PlayerWindow
        {
            get { return _playWindow; }
            set { _playWindow = value; }
        }

        /// <summary>
        /// Gets the current video height after opening the media stream.
        /// </summary>
        public int VideoHeight
        {
            get { return _videoHeight; }
        }

        /// <summary>
        /// Gets the current video width after opening the media stream.
        /// </summary>
        public int VideoWidth
        {
            get { return _videoWidth; }
        }

        /// <summary>
        /// Gets the current supported media file types.
        /// </summary>
        public static string VideoFileTypes
        {
            get
            {
                return _videoFiles;
            }
        }

        /// <summary>
        /// Gets the current supported media file types.
        /// </summary>
        public static string AudioFileTypes
        {
            get
            {
                return _audioFiles;
            }
        }

        /// <summary>
        /// Gets the current supported media file types.
        /// </summary>
        public static string MidiFileTypes
        {
            get
            {
                return _midiFiles;
            }
        }

        /// <summary>
        /// Gets the current supported media file types.
        /// </summary>
        public static string ImageFileTypes
        {
            get
            {
                return _imageFiles;
            }
        }

        /// <summary>
        /// Gets an indicator for full screen mode.
        /// </summary>
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
        }

        /// <summary>
        /// Gets the playback rate.
        /// </summary>
        public double PlaybackRate
        {
            get { return _currentPlaybackRate; }
        }

        /// <summary>
        /// Gets the total duration of the media.
        /// </summary>
        public double Duration
        {
            get
            {
                if (_mediaPosition != null)
                {
                    // Get the total duration of the media.
                    double duration = 0.0;
                    int hr = _mediaPosition.get_Duration(out duration);
                    if (hr == 0)
                    {
                        // Return the duration.
                        return duration;
                    }
                    else
                        return 0.0;
                }
                else
                    return 0.0;
            }
        }

        /// <summary>
        /// Get the current position of the media.
        /// </summary>
        public double CurrentPosition
        {
            get
            {
                if (_mediaPosition != null)
                {
                    // Get the current position of the media.
                    double pllTime = 0.0;
                    int hr = _mediaPosition.get_CurrentPosition(out pllTime);
                    if (hr == 0)
                    {
                        // Return the position.
                        return pllTime;
                    }
                    else
                        return 0.0;
                }
                else
                    return 0.0;
            }
        }

        /// <summary>
        /// Open a new media stream.
        /// </summary>
        /// <param name="filename">The file name where the media stream is stored.</param>
        public void Open(string filename)
        {
            if (_playWindow == null) throw new Exception("The media player window must be set");
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename");

            _filename = filename;

            // Create a new graph.
            if (_currentState == PlayState.Init)
                CreateGraph();
        }

        /// <summary>
        /// Open a new DVD media stream.
        /// </summary>
        private void OpenDvd()
        {
            if (_playWindow == null) throw new Exception("The media player window must be set");

            // Create a new graph.
            if (_currentState == PlayState.Init)
                CreateDvdGraph();
        }

        /// <summary>
        /// Play the media stream.
        /// </summary>
        public void Play()
        {
            int hr = 0;

            // Current state is stopped.
            if ((_currentState == PlayState.Stopped) || (_currentState == PlayState.Paused))
            {
                // Run the graph to play the media file
                hr = _mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);
                _currentState = PlayState.Running;
            }
        }

        /// <summary>
        /// Pause the media stream.
        /// </summary>
        public void Pause()
        {
            int hr = 0;

            // Current state is ruiing.
            if (_currentState == PlayState.Running)
            {
                // Run the graph to play the media file
                hr = _mediaControl.Pause();
                DsError.ThrowExceptionForHR(hr);
                _currentState = PlayState.Paused;
            }
        }

        /// <summary>
        /// Stop the media stream.
        /// </summary>
        public void Stop()
        {
            int hr = 0;

            // Stop and reset postion to beginning
            if ((_currentState == PlayState.Paused) || (_currentState == PlayState.Running))
            {
                DsLong pos = new DsLong(0);

                hr = _mediaControl.Stop();
                DsError.ThrowExceptionForHR(hr);
                _currentState = PlayState.Stopped;

                // Seek to the beginning
                hr = _mediaSeeking.SetPositions(pos, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
                DsError.ThrowExceptionForHR(hr);

                // Display the first frame to indicate the reset condition
                hr = _mediaControl.Pause();
                DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Mute or unmute the sound.
        /// </summary>
        public void Mute()
        {
            int hr = 0;

            // Stop and reset postion to beginning
            if ((_currentState == PlayState.Paused) || (_currentState == PlayState.Running))
            {
                // Read current volume
                hr = _basicAudio.get_Volume(out _currentVolume);

                //E_NOTIMPL
                if (hr == -1) 
                {
                    // Fail quietly if this is a video-only media file
                    return;
                }
                else if (hr < 0)
                {
                    return;
                }

                // Switch volume levels
                if (_currentVolume == VolumeFull)
                    _currentVolume = VolumeSilence;
                else
                    _currentVolume = VolumeFull;

                // Set new volume
                hr = _basicAudio.put_Volume(_currentVolume);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Full screen mode or un-do full screen mode.
        /// </summary>
        public void FullScreenMode()
        {
            int hr = 0;

            // Stop and reset postion to beginning
            if ((_currentState == PlayState.Paused) || (_currentState == PlayState.Running) || (_currentState == PlayState.Stopped))
            {
                OABool mode;

                // Don't bother with full-screen for audio-only files
                if ((_isAudioOnly) || (_videoWindow == null))
                    return;

                // Read current state
                hr = _videoWindow.get_FullScreenMode(out mode);
                DsError.ThrowExceptionForHR(hr);

                if (mode == OABool.False)
                {
                    // Save current message drain
                    hr = _videoWindow.get_MessageDrain(out _pDrain);
                    DsError.ThrowExceptionForHR(hr);

                    // Set message drain to application main window
                    hr = _videoWindow.put_MessageDrain(_playWindow.Handle);
                    DsError.ThrowExceptionForHR(hr);

                    // Switch to full-screen mode
                    mode = OABool.True;
                    hr = _videoWindow.put_FullScreenMode(mode);
                    DsError.ThrowExceptionForHR(hr);
                    _isFullScreen = true;
                }
                else
                {
                    // Switch back to windowed mode
                    mode = OABool.False;
                    hr = _videoWindow.put_FullScreenMode(mode);
                    DsError.ThrowExceptionForHR(hr);

                    // Undo change of message drain
                    hr = _videoWindow.put_MessageDrain(_pDrain);
                    DsError.ThrowExceptionForHR(hr);

                    // Reset video window
                    hr = _videoWindow.SetWindowForeground(OABool.True);
                    DsError.ThrowExceptionForHR(hr);

                    // Reclaim keyboard focus for player application
                    //this.Focus();
                    _isFullScreen = false;
                }
            }
        }

        /// <summary>
        /// Set to single frame step.
        /// </summary>
        public void SetSingleFrameStep()
        {
            int hr = 0;

            // Stop and reset postion to beginning
            if ((_currentState == PlayState.Running) || (_currentState == PlayState.Stopped))
            {
                // Pause the video.
                Pause();

                // Step the requested number of frames, if supported
                hr = _frameStep.Step(1, null);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Set the video size.
        /// </summary>
        /// <param name="videoSize">The video size required.</param>
        public void SetVideoSize(MediaPlayerVideoSize videoSize)
        {
            // If not in the initialisation state.
            if ((_currentState != PlayState.Init))
            {
                // Select video size.
                switch(videoSize)
                {
                    case MediaPlayerVideoSize.Half:
                        InitVideoWindow(1, 2);
                        break;

                    case MediaPlayerVideoSize.Double:
                        InitVideoWindow(2, 1);
                        break;

                    case MediaPlayerVideoSize.ThreeQuarter:
                        InitVideoWindow(3, 4);
                        break;

                    case MediaPlayerVideoSize.Normal:
                    default:
                        InitVideoWindow(1, 1);
                        break;
                }
            }
        }

        /// <summary>
        /// Set the playback rate.
        /// </summary>
        /// <param name="playbackRate">The playback rate required.</param>
        public void SetPlaybackRate(MediaPlayerPlaybackRate playbackRate)
        {
            // If not in the initialisation state.
            if ((_currentState != PlayState.Init))
            {
                // Select playback rate.
                switch (playbackRate)
                {
                    case MediaPlayerPlaybackRate.Half:
                        SetRate(0.5);
                        break;

                    case MediaPlayerPlaybackRate.Double:
                        SetRate(2.0);
                        break;

                    case MediaPlayerPlaybackRate.Increase:
                        ModifyRate(+0.25);
                        break;

                    case MediaPlayerPlaybackRate.Decrease:
                        ModifyRate(-0.25);
                        break;

                    case MediaPlayerPlaybackRate.Normal:
                    default:
                        SetRate(1.0);
                        break;
                }
            }
        }

        /// <summary>
        /// Closes the current media stream.
        /// </summary>
        public void Close()
        {
            int hr = 0;

            // No current media state
            _currentState = PlayState.Init;

            // Stop media playback
            if (_mediaControl != null)
                hr = _mediaControl.Stop();

            // Clear global flags
            _isAudioOnly = true;
            _isFullScreen = false;

            // Free DirectShow interfaces
            CloseInterfaces();

            // Clear file name.
            _filename = string.Empty;
        }

        /// <summary>
        /// Create a new DVD graph.
        /// </summary>
        private void CreateDvdGraph()
        {
            try
            {
                 _currentVolume = VolumeFull;

                int hr = 0;
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Create a new graph.
        /// </summary>
        private void CreateGraph()
        {
            try
            {
                _currentVolume = VolumeFull;

                int hr = 0;

                if (String.IsNullOrEmpty(_filename))
                    return;

                _graphBuilder = (IGraphBuilder)new FilterGraph();

                // Have the graph builder construct its the appropriate graph automatically
                hr = _graphBuilder.RenderFile(_filename, null);
                DsError.ThrowExceptionForHR(hr);

                // QueryInterface for DirectShow interfaces
                _mediaControl = (IMediaControl)_graphBuilder;
                _mediaEventEx = (IMediaEventEx)_graphBuilder;
                _mediaSeeking = (IMediaSeeking)_graphBuilder;
                _mediaPosition = (IMediaPosition)_graphBuilder;

                // Query for video interfaces, which may not be relevant for audio files
                _videoWindow = _graphBuilder as IVideoWindow;
                _basicVideo = _graphBuilder as IBasicVideo;

                // Query for audio interfaces, which may not be relevant for video-only files
                _basicAudio = _graphBuilder as IBasicAudio;

                // Is this an audio-only file (no video component)?
                CheckVisibility();

                // Have the graph signal event via window callbacks for performance
                hr = _mediaEventEx.SetNotifyWindow(_playWindow.Handle, WMGraphNotify, IntPtr.Zero);
                DsError.ThrowExceptionForHR(hr);

                if (!_isAudioOnly)
                {
                    // Setup the video window
                    hr = _videoWindow.put_Owner(_playWindow.Handle);
                    DsError.ThrowExceptionForHR(hr);

                    hr = _videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings | WindowStyle.ClipChildren);
                    DsError.ThrowExceptionForHR(hr);

                    hr = InitVideoWindow(1, 1);
                    DsError.ThrowExceptionForHR(hr);

                    // Position video window in client rect of owner window
                    _playWindow.Resize += new EventHandler(onPlayerWindowResize);
                    onPlayerWindowResize(this, null);

                    GetFrameStepInterface();
                }

                // Complete window initialization
                _isFullScreen = false;
                _currentPlaybackRate = 1.0;
                _currentState = PlayState.Stopped;
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Make the video visible.
        /// </summary>
        private void CheckVisibility()
        {
            int hr = 0;
            OABool lVisible;

            if ((_videoWindow == null) || (_basicVideo == null))
            {
                // Audio-only files have no video interfaces.  This might also
                // be a file whose video component uses an unknown video codec.
                _isAudioOnly = true;
                return;
            }
            else
            {
                // Clear the global flag
                _isAudioOnly = false;
            }

            hr = _videoWindow.get_Visible(out lVisible);
            if (hr < 0)
            {
                // If this is an audio-only clip, get_Visible() won't work.
                //
                // Also, if this video is encoded with an unsupported codec,
                // we won't see any video, although the audio will work if it is
                // of a supported format.
                if (hr == unchecked((int)0x80004002)) //E_NOINTERFACE
                {
                    _isAudioOnly = true;
                }
                else
                    DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Initialise the video display window.
        /// </summary>
        /// <param name="multiplier">The window multiplier size.</param>
        /// <param name="divider">The window divider size.</param>
        /// <returns>Zero if the window position has been set.</returns>
        private int InitVideoWindow(int multiplier, int divider)
        {
            _multiplier = multiplier;
            _divider = divider;

            int hr = 0;

            if (_basicVideo == null)
                return 0;

            // Read the default video size
            hr = _basicVideo.GetVideoSize(out _videoWidth, out _videoHeight);
            if (hr == DsResults.E_NoInterface)
                return 0;

            // Account for requests of normal, half, or double size
            _videoWidth = _videoWidth * _multiplier / _divider;
            _videoHeight = _videoHeight * _multiplier / _divider;

            return hr;
        }

        /// <summary>
        /// Some video renderers support stepping media frame by frame with the
        /// IVideoFrameStep interface.  See the interface documentation for more
        /// details on frame stepping.
        /// </summary>
        /// <returns>True if frame step found; else false.</returns>
        private bool GetFrameStepInterface()
        {
            int hr = 0;

            IVideoFrameStep frameStepTest = null;

            // Get the frame step interface, if supported
            frameStepTest = (IVideoFrameStep)_graphBuilder;

            // Check if this decoder can step
            hr = frameStepTest.CanStep(0, null);
            if (hr == 0)
            {
                _frameStep = frameStepTest;
                return true;
            }
            else
            {
                // BUG 1560263 found by husakm (thanks)...
                // Marshal.ReleaseComObject(frameStepTest);
                _frameStep = null;
                return false;
            }
        }

        /// <summary>
        /// Modify the playback rate.
        /// </summary>
        /// <param name="dRateAdjust">The rate to adjust by.</param>
        private void ModifyRate(double dRateAdjust)
        {
            int hr = 0;
            double dRate;

            // If the IMediaPosition interface exists, use it to set rate
            if ((_mediaPosition != null) && (dRateAdjust != 0.0))
            {
                hr = _mediaPosition.get_Rate(out dRate);
                if (hr == 0)
                {
                    // Add current rate to adjustment value
                    double dNewRate = dRate + dRateAdjust;
                    hr = _mediaPosition.put_Rate(dNewRate);

                    // Save global rate
                    if (hr == 0)
                    {
                        _currentPlaybackRate = dNewRate;
                    }
                }
            }
        }

        /// <summary>
        /// Set the playback rate.
        /// </summary>
        /// <param name="rate">The rate to set.</param>
        private void SetRate(double rate)
        {
            int hr = 0;

            // If the IMediaPosition interface exists, use it to set rate
            if (_mediaPosition != null)
            {
                hr = _mediaPosition.put_Rate(rate);
                if (hr >= 0)
                {
                    _currentPlaybackRate = rate;
                }
            }
        }

        /// <summary>
        /// Resize the player when the player window is resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onPlayerWindowResize(object sender, EventArgs e)
        {
            if (_videoWindow != null)
            {
                // Position video window in client rect of owner window
                Rectangle rc = _playWindow.ClientRectangle;
                _videoWindow.SetWindowPosition(0, 0, rc.Right, rc.Bottom);
            }
        }


        /// <summary>
        /// Close all interfaces.
        /// </summary>
        private void CloseInterfaces()
        {
            int hr = 0;

            try
            {
                lock (this)
                {
                    // Remove the dvd control.
                    if (_dvdControl != null)
                        hr = _dvdControl.SetOption(DvdOptionFlag.ResetOnStop, true);

                    // Free the preview window (ignore errors)
                    if (_videoWindow != null)
                    {
                        _videoWindow.put_Visible(OABool.False);
                        _videoWindow.put_Owner(IntPtr.Zero);
                    }

                    // Remove the Resize event handler
                    if (_playWindow != null)
                        _playWindow.Resize -= new EventHandler(onPlayerWindowResize);

                    if (_mediaEventEx != null)
                    {
                        hr = _mediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
                        DsError.ThrowExceptionForHR(hr);
                    }

                    // Release and zero DirectShow interfaces
                    if (_mediaEventEx != null)
                        _mediaEventEx = null;

                    if (_mediaSeeking != null)
                        _mediaSeeking = null;

                    if (_mediaPosition != null)
                        _mediaPosition = null;

                    if (_mediaControl != null)
                        _mediaControl = null;

                    if (_basicAudio != null)
                        _basicAudio = null;

                    if (_basicVideo != null)
                        _basicVideo = null;

                    if (_videoWindow != null)
                        _videoWindow = null;

                    if (_frameStep != null)
                        _frameStep = null;

                    if (_graphBuilder != null)
                        Marshal.ReleaseComObject(_graphBuilder); _graphBuilder = null;

                    GC.Collect();
                }
            }
            catch
            {
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
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // Free DirectShow interfaces
                CloseInterfaces();

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
