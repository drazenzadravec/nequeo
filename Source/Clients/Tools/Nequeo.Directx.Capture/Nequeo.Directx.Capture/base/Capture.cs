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

using Nequeo.Media;
using Nequeo.Directx.Utility;

namespace Nequeo.Directx
{
    /// <summary>
    /// Video and audio capture provider.
    /// </summary>
    public class Capture : IDisposable
	{
        /// <summary>
        /// Create a new capture object. 
        /// videoDevice and audioDevice can be null if you do not 
        /// wish to capture both audio and video. However at least
        /// one must be a valid device. Use the <see cref="Device"/> 
        /// class to list available devices.
        /// </summary>
        /// <param name="videoDevice">The video capture device.</param>
        /// <param name="audioDevice">The audio capture device.</param>
        public Capture(Device videoDevice, Device audioDevice)
        {
            if (videoDevice == null && audioDevice == null)
                throw new ArgumentException("The videoDevice and/or the audioDevice parameter must be set to a valid Filter.\n");

            _videoDevice = videoDevice;
            _audioDevice = audioDevice;

            // Create a new capture image sample.
            _captureImageSample = new CaptureImageSample(this);

            // Create a new capture sound sample.
            _captureSoundSample = new CaptureSoundSample(this);
        }

        private GraphState _graphState = GraphState.Null;		    // State of the internal filter graph
        private string _filename = null;
        private bool _isCaptureWrittingToFile = false;
        private MediaFormatType _mediaFormat = MediaFormatType.Wmv;

        private bool _isCaptureToImage = false;
        private CaptureImageSample _captureImageSample = null;
        private bool _isCaptureToSound = false;
        private CaptureSoundSample _captureSoundSample = null;

        private bool _isPreviewRendered = false;			        // When graphState==Rendered, have we rendered the preview stream?
        private bool _isCaptureRendered = false;			        // When graphState==Rendered, have we rendered the capture stream?
        private bool _wantPreviewRendered = false;		            // Do we need the preview stream rendered (VideoDevice and PreviewWindow != null)
        private bool _wantCaptureRendered = false;		            // Do we need the capture stream rendered

        private SourceCollection _videoSources = null;				// Property Backer: list of physical video sources
        private SourceCollection _audioSources = null;				// Property Backer: list of physical audio sources

        private Control _previewWindow = null;				        // Property Backer: Owner control for preview
        private VideoCapabilities _videoCaps = null;				// Property Backer: capabilities of video device
        private AudioCapabilities _audioCaps = null;				// Property Backer: capabilities of audio device
        private PropertyPageCollection _propertyPages = null;		// Property Backer: list of property pages exposed by filters
        private Tuner _tuner = null;		                        // Property Backer: TV Tuner

        private Device _videoDevice = null;
        private Device _audioDevice = null;
        private Device _videoCompressor = null;
        private Device _audioCompressor = null;

        private IMediaControl _mediaControl;						    // DShow Filter: Start/Stop the filter graph -> copy of graphBuilder
        private bool _isRunning = false;

        private IFilterGraph2 _graphBuilder;						    // DShow Filter: Graph builder 
        private IVideoWindow _videoWindow;						        // DShow Filter: Control preview window -> copy of graphBuilder
        private ICaptureGraphBuilder2 _captureGraphBuilder = null;	    // DShow Filter: building graphs for capturing video
        private IAMStreamConfig _videoStreamConfig = null;			    // DShow Filter: configure frame rate, size
        private IAMStreamConfig _audioStreamConfig = null;			    // DShow Filter: configure sample rate, sample size
        private IBaseFilter _videoDeviceFilter = null;			        // DShow Filter: selected video device
        private IBaseFilter _videoCompressorFilter = null;		        // DShow Filter: selected video compressor
        private IBaseFilter _audioDeviceFilter = null;			        // DShow Filter: selected audio device
        private IBaseFilter _audioCompressorFilter = null;		        // DShow Filter: selected audio compressor
        private IBaseFilter _muxFilter = null;					        // DShow Filter: multiplexor (combine video and audio streams)
        private IFileSinkFilter _fileWriterFilter = null;			    // DShow Filter: file writer

        /// <summary> 
        /// Fired when a capture is completed (manually or automatically). 
        /// </summary>
        public event EventHandler CaptureComplete;

        /// <summary>
        /// Gets the graph builder.
        /// </summary>
        internal IFilterGraph2 GraphBuilder
        {
            get { return _graphBuilder; }
        }

        /// <summary>
        /// Gets the capture graph builder.
        /// </summary>
        internal ICaptureGraphBuilder2 CaptureGraphBuilder
        {
            get { return _captureGraphBuilder; }
        }

        /// <summary>
        /// Gets the video device filter.
        /// </summary>
        internal IBaseFilter VideoDeviceFilter
        {
            get { return _videoDeviceFilter; }
        }

        /// <summary>
        /// Gets the audio device filter.
        /// </summary>
        internal IBaseFilter AudioDeviceFilter
        {
            get { return _audioDeviceFilter; }
        }

        /// <summary>
        /// Gets the image capture sample provider.
        /// </summary>
        public CaptureImageSample CaptureImageSample
        {
            get { return _captureImageSample; }
        }

        /// <summary>
        /// Gets the sound capture sample provider.
        /// </summary>
        public CaptureSoundSample CaptureSoundSample
        {
            get { return _captureSoundSample; }
        }

        /// <summary>
        /// Gets is the class currently capturing.
        /// </summary>
        public bool Capturing 
        {
            get { return (_graphState == GraphState.Capturing); } 
        }

        /// <summary>
        /// Gets has the class been cued to begin capturing.
        /// </summary>
        public bool Cued 
        {
            get { return (_isCaptureRendered && _graphState == GraphState.Rendered); } 
        }

        /// <summary>
        /// Gets is the class currently stopped.
        /// </summary>
        public bool Stopped 
        {
            get { return (_graphState != GraphState.Capturing); } 
        }

        /// <summary>
        /// Gets is capturing video data to file.
        /// </summary>
        public bool IsCaptureToFile
        {
            get { return _isCaptureWrittingToFile; }
        }

        /// <summary>
        /// Gets is capturing image snapshot.
        /// </summary>
        public bool IsCaptureToImage
        {
            get { return _isCaptureToImage; }
        }

        /// <summary>
        /// Gets is capturing sound snapshot.
        /// </summary>
        public bool IsCaptureToSound
        {
            get { return _isCaptureToSound; }
        }

        /// <summary>
        ///  Available property pages. 
        /// </summary>
        /// <remarks>
        ///  These are property pages exposed by the DirectShow filters. 
        ///  These property pages allow users modify settings on the 
        ///  filters directly. 
        /// 
        /// <para>
        ///  The information contained in this property is retrieved and
        ///  cached the first time this property is accessed. Future
        ///  calls to this property use the cached results. This was done 
        ///  for performance. </para>
        ///  
        /// <para>
        ///  However, this means <b>you may get different results depending 
        ///  on when you access this property first</b>. If you are experiencing 
        ///  problems, try accessing the property immediately after creating 
        ///  the Capture class or immediately after setting the video and 
        ///  audio compressors. Also, inform the developer. </para>
        /// </remarks>
        public PropertyPageCollection PropertyPages
        {
            get
            {
                if (_propertyPages == null)
                {
                    try
                    {
                        _propertyPages = new PropertyPageCollection(
                            _captureGraphBuilder,
                            _videoDeviceFilter, _audioDeviceFilter,
                            _videoCompressorFilter, _audioCompressorFilter,
                            _videoSources, _audioSources);
                    }
                    catch { }

                }
                return (_propertyPages);
            }
        }

        /// <summary>
        ///  The control that will host the preview window. 
        /// </summary>
        /// <remarks>
        ///  Setting this property will begin video preview
        ///  immediately. Set this property after setting all
        ///  other properties to avoid unnecessary changes
        ///  to the internal filter graph (some properties like
        ///  FrameSize require the internal filter graph to be 
        ///  stopped and disconnected before the property
        ///  can be retrieved or set).
        ///  
        /// <para>
        /// Gets or sets; to stop video preview, set this property to null. </para>
        /// </remarks>
        public Control PreviewWindow
        {
            get { return (_previewWindow); }
            set
            {
                AssertStopped();
                DerenderGraph();
                _previewWindow = value;
                _wantPreviewRendered = ((_previewWindow != null) && (_videoDevice != null));
                RenderGraph();
                StartPreviewIfNeeded();
            }
        }

        /// <summary>
        ///  The capabilities of the video device.
        /// </summary>
        /// <remarks>
        ///  It may be required to cue the capture (see <see cref="Cue"/>) 
        ///  before all capabilities are correctly reported. If you 
        ///  have such a device, the developer would be interested to
        ///  hear from you.
        /// 
        /// <para>
        ///  The information contained in this property is retrieved and
        ///  cached the first time this property is accessed. Future
        ///  calls to this property use the cached results. This was done 
        ///  for performance. </para>
        ///  
        /// <para>
        ///  However, this means <b>you may get different results depending 
        ///  on when you access this property first</b>. If you are experiencing 
        ///  problems, try accessing the property immediately after creating 
        ///  the Capture class or immediately after setting the video and 
        ///  audio compressors. Also, inform the developer. </para>
        /// </remarks>
        public VideoCapabilities VideoCaps
        {
            get
            {
                if (_videoCaps == null)
                {
                    if (_videoStreamConfig != null)
                    {
                        try
                        {
                            _videoCaps = new VideoCapabilities(_videoStreamConfig);
                        }
                        catch (Exception ex) { Debug.WriteLine("VideoCaps: unable to create videoCaps." + ex.ToString()); }
                    }
                }
                return (_videoCaps);
            }
        }

        /// <summary>
        ///  The capabilities of the audio device.
        /// </summary>
        /// <remarks>
        ///  It may be required to cue the capture (see <see cref="Cue"/>) 
        ///  before all capabilities are correctly reported. If you 
        ///  have such a device, the developer would be interested to
        ///  hear from you.
        /// 
        /// <para>
        ///  The information contained in this property is retrieved and
        ///  cached the first time this property is accessed. Future
        ///  calls to this property use the cached results. This was done 
        ///  for performance. </para>
        ///  
        /// <para>
        ///  However, this means <b>you may get different results depending 
        ///  on when you access this property first</b>. If you are experiencing 
        ///  problems, try accessing the property immediately after creating 
        ///  the Capture class or immediately after setting the video and 
        ///  audio compressors. Also, inform the developer. </para>
        /// </remarks>
        public AudioCapabilities AudioCaps
        {
            get
            {
                if (_audioCaps == null)
                {
                    if (_audioStreamConfig != null)
                    {
                        try
                        {
                            _audioCaps = new AudioCapabilities(_audioStreamConfig);
                        }
                        catch (Exception ex) { Debug.WriteLine("AudioCaps: unable to create audioCaps." + ex.ToString()); }
                    }
                }
                return (_audioCaps);
            }
        }

        /// <summary> 
        ///  Gets the video capture device filter. Read-only. To use a different 
        ///  device, dispose of the current Capture instance and create a new 
        ///  instance with the desired device. 
        /// </summary>
        public Device VideoDevice 
        { 
            get { return (_videoDevice); } 
        }

        /// <summary> 
        ///  Gets the audio capture device filter. Read-only. To use a different 
        ///  device, dispose of the current Capture instance and create a new 
        ///  instance with the desired device. 
        /// </summary>
        public Device AudioDevice 
        { 
            get { return (_audioDevice); } 
        }

        /// <summary> 
        ///  Gets or sets the video compression filter. When this property is changed 
        ///  the internal filter graph is rebuilt. This means that some properties
        ///  will be reset. Set this property as early as possible to avoid losing 
        ///  changes. This property cannot be changed while capturing.
        /// </summary>
        public Device VideoCompressor
        {
            get { return (_videoCompressor); }
            set
            {
                AssertStopped();
                DestroyGraph();
                _videoCompressor = value;
                RenderGraph();
                StartPreviewIfNeeded();
            }
        }

        /// <summary> 
        ///  Gets or sets the audio compression filter. 
        /// </summary>
        /// <remarks>
        ///  When this property is changed 
        ///  the internal filter graph is rebuilt. This means that some properties
        ///  will be reset. Set this property as early as possible to avoid losing 
        ///  changes. This property cannot be changed while capturing.
        /// </remarks>
        public Device AudioCompressor
        {
            get { return (_audioCompressor); }
            set
            {
                AssertStopped();
                DestroyGraph();
                _audioCompressor = value;
                RenderGraph();
                StartPreviewIfNeeded();
            }
        }

        /// <summary> 
        ///  Gets the collection of available video sources/physical connectors 
        ///  on the current video device. 
        /// </summary>
        /// <remarks>
        ///  In most cases, if the device has only one source, 
        ///  this collection will be empty. 
        /// 
        /// <para>
        ///  The information contained in this property is retrieved and
        ///  cached the first time this property is accessed. Future
        ///  calls to this property use the cached results. This was done 
        ///  for performance. </para>
        ///  
        /// <para>
        ///  However, this means <b>you may get different results depending 
        ///  on when you access this property first</b>. If you are experiencing 
        ///  problems, try accessing the property immediately after creating 
        ///  the Capture class or immediately after setting the video and 
        ///  audio compressors. Also, inform the developer. </para>
        /// </remarks>
        public SourceCollection VideoSources
        {
            get
            {
                if (_videoSources == null)
                {
                    try
                    {
                        if (_videoDevice != null)
                            _videoSources = new SourceCollection(_captureGraphBuilder, _videoDeviceFilter, true);
                        else
                            _videoSources = new SourceCollection();
                    }
                    catch (Exception ex) { Debug.WriteLine("VideoSources: unable to create VideoSources." + ex.ToString()); }
                }
                return (_videoSources);
            }
        }


        /// <summary> 
        ///  Gets the collection of available audio sources/physical connectors 
        ///  on the current audio device. 
        /// </summary>
        /// <remarks>
        ///  In most cases, if the device has only one source, 
        ///  this collection will be empty. For audio
        ///  there are 2 different methods for enumerating audio sources
        ///  an audio crossbar (usually TV tuners?) or an audio mixer 
        ///  (usually sound cards?). This class will first look for an 
        ///  audio crossbar. If no sources or only one source is available
        ///  on the crossbar, this class will then look for an audio mixer.
        ///  This class does not support both methods.
        /// 
        /// <para>
        ///  The information contained in this property is retrieved and
        ///  cached the first time this property is accessed. Future
        ///  calls to this property use the cached results. This was done 
        ///  for performance. </para>
        ///  
        /// <para>
        ///  However, this means <b>you may get different results depending 
        ///  on when you access this property first</b>. If you are experiencing 
        ///  problems, try accessing the property immediately after creating 
        ///  the Capture class or immediately after setting the video and 
        ///  audio compressors. Also, inform the developer. </para>
        ///  </remarks>
        public SourceCollection AudioSources
        {
            get
            {
                if (_audioSources == null)
                {
                    try
                    {
                        if (_audioDevice != null)
                            _audioSources = new SourceCollection(_captureGraphBuilder, _audioDeviceFilter, false);
                        else
                            _audioSources = new SourceCollection();
                    }
                    catch (Exception ex) { Debug.WriteLine("AudioSources: unable to create AudioSources." + ex.ToString()); }
                }
                return (_audioSources);
            }
        }

        /// <summary> 
        ///  Gets or sets the current video source. Use Capture.VideoSources to 
        ///  list available sources. Set to null to disable all 
        ///  sources (mute).
        /// </summary>
        public Source VideoSource
        {
            get { return (VideoSources.CurrentSource); }
            set { VideoSources.CurrentSource = value; }
        }

        /// <summary> 
        ///  Gets or sets the current audio source. Use Capture.AudioSources to 
        ///  list available sources. Set to null to disable all 
        ///  sources (mute).
        /// </summary>
        public Source AudioSource
        {
            get { return (AudioSources.CurrentSource); }
            set { AudioSources.CurrentSource = value; }
        }

        /// <summary>
        ///  Gets the TV Tuner or null if the current video device 
        ///  does not have a TV Tuner.
        /// </summary>
        public Tuner Tuner 
        { 
            get { return (_tuner); } 
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
        ///  
        /// <para>
        ///  Not all devices support getting/setting this property.
        ///  If this property is not supported, accessing it will
        ///  throw and exception. </para>
        ///  
        /// <para>
        ///  This property cannot be changed while capturing. Changing 
        ///  this property while preview is enabled will cause some 
        ///  fickering while the internal filter graph is partially
        ///  rebuilt. Changing this property while cued will cancel the
        ///  cue. Call Cue() again to re-cue the capture. </para>
        /// </remarks>
        public double FrameRate
        {
            get
            {
                long avgTimePerFrame = (long)GetStreamConfigSetting(_videoStreamConfig, "AvgTimePerFrame");
                return ((double)10000000 / avgTimePerFrame);
            }
            set
            {
                long avgTimePerFrame = (long)(10000000 / value);
                SetStreamConfigSetting(_videoStreamConfig, "AvgTimePerFrame", avgTimePerFrame);
            }
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
        ///  
        /// <para>
        ///  Not all devices support getting/setting this property.
        ///  If this property is not supported, accessing it will
        ///  throw and exception. </para>
        /// 
        /// <para> 
        ///  This property cannot be changed while capturing. Changing 
        ///  this property while preview is enabled will cause some 
        ///  fickering while the internal filter graph is partially
        ///  rebuilt. Changing this property while cued will cancel the
        ///  cue. Call Cue() again to re-cue the capture. </para>
        /// </remarks>
        public Size FrameSize
        {
            get
            {
                BitmapInfoHeader bmiHeader;
                bmiHeader = (BitmapInfoHeader)GetStreamConfigSetting(_videoStreamConfig, "BmiHeader");
                Size size = new Size(bmiHeader.Width, bmiHeader.Height);
                return (size);
            }
            set
            {
                BitmapInfoHeader bmiHeader;
                bmiHeader = (BitmapInfoHeader)GetStreamConfigSetting(_videoStreamConfig, "BmiHeader");
                bmiHeader.Width = value.Width;
                bmiHeader.Height = value.Height;
                SetStreamConfigSetting(_videoStreamConfig, "BmiHeader", bmiHeader);
            }
        }

        /// <summary>
        ///  Get or set the number of channels in the waveform-audio data. 
        /// </summary>
        /// <remarks>
        ///  Monaural data uses one channel and stereo data uses two channels. 
        ///  
        /// <para>
        ///  Not all devices support getting/setting this property.
        ///  If this property is not supported, accessing it will
        ///  throw and exception. </para>
        ///  
        /// <para>
        ///  This property cannot be changed while capturing. Changing 
        ///  this property while preview is enabled will cause some 
        ///  fickering while the internal filter graph is partially
        ///  rebuilt. Changing this property while cued will cancel the
        ///  cue. Call Cue() again to re-cue the capture. </para>
        /// </remarks>
        public short AudioChannels
        {
            get
            {
                short audioChannels = (short)GetStreamConfigSetting(_audioStreamConfig, "nChannels");
                return (audioChannels);
            }
            set
            {
                SetStreamConfigSetting(_audioStreamConfig, "nChannels", value);
            }
        }

        /// <summary>
        ///  Get or set the number of audio samples taken per second.
        /// </summary>
        /// <remarks>
        ///  Common sampling rates are 8.0 kHz, 11.025 kHz, 22.05 kHz, and 
        ///  44.1 kHz. Not all sampling rates are supported.
        ///  
        /// <para>
        ///  Not all devices support getting/setting this property.
        ///  If this property is not supported, accessing it will
        ///  throw and exception. </para>
        ///  
        /// <para>
        ///  This property cannot be changed while capturing. Changing 
        ///  this property while preview is enabled will cause some 
        ///  fickering while the internal filter graph is partially
        ///  rebuilt. Changing this property while cued will cancel the
        ///  cue. Call Cue() again to re-cue the capture. </para>
        /// </remarks>
        public int AudioSamplingRate
        {
            get
            {
                int samplingRate = (int)GetStreamConfigSetting(_audioStreamConfig, "nSamplesPerSec");
                return (samplingRate);
            }
            set
            {
                SetStreamConfigSetting(_audioStreamConfig, "nSamplesPerSec", value);
            }
        }

        /// <summary>
        ///  Get or set the number of bits recorded per sample. 
        /// </summary>
        /// <remarks>
        ///  Common sample sizes are 8 bit and 16 bit. Not all
        ///  samples sizes are supported.
        ///  
        /// <para>
        ///  Not all devices support getting/setting this property.
        ///  If this property is not supported, accessing it will
        ///  throw and exception. </para>
        ///  
        /// <para>
        ///  This property cannot be changed while capturing. Changing 
        ///  this property while preview is enabled will cause some 
        ///  fickering while the internal filter graph is partially
        ///  rebuilt. Changing this property while cued will cancel the
        ///  cue. Call Cue() again to re-cue the capture. </para>
        /// </remarks>
        public short AudioSampleSize
        {
            get
            {
                short sampleSize = (short)GetStreamConfigSetting(_audioStreamConfig, "wBitsPerSample");
                return (sampleSize);
            }
            set
            {
                SetStreamConfigSetting(_audioStreamConfig, "wBitsPerSample", value);
            }
        }

        /// <summary>
        ///  Prepare for capturing. Use this method when capturing 
        ///  must begin as quickly as possible. 
        /// </summary>
        /// <remarks>
        ///  This will create/overwrite a zero byte file with 
        ///  the name set in the Filename property. 
        ///  
        /// <para>
        ///  This will disable preview. Preview will resume
        ///  once capture begins. This problem can be fixed
        ///  if someone is willing to make the change. </para>
        ///  
        /// <para>
        ///  This method is optional. If Cue() is not called, 
        ///  Start() will call it before capturing. This method
        ///  cannot be called while capturing. </para>
        /// </remarks>
        public void Cue()
        {
            AssertStopped();

            // We want the capture stream rendered
            _wantCaptureRendered = true;

            // Re-render the graph (if necessary)
            RenderGraph();

            // Pause the graph
            int hr = _mediaControl.Pause();
            if (hr != 0) Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Begin capturing to file only.
        /// </summary>
        /// <param name="filename">The file name to write to.</param>
        /// <param name="mediaFormat">The media format.</param>
        public void StartToFile(string filename, MediaFormatType mediaFormat)
        {
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename");

            // If is running.
            if (!_isRunning)
            {
                // Assign the file name.
                _filename = filename;
                _mediaFormat = mediaFormat;
                _isCaptureWrittingToFile = true;
                _isRunning = true;

                AssertStopped();

                // We want the capture stream rendered
                _wantCaptureRendered = true;

                // Re-render the graph (if necessary)
                RenderGraph();

                // Start the filter graph: begin capturing
                int hr = _mediaControl.Run();
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }
            
            // Update the state
            _graphState = GraphState.Capturing;
        }

        /// <summary>
        /// Begin capture of image only data.
        /// </summary>
        public void StartSnapshotImage()
        {
            // If is running.
            if (!_isRunning)
            {
                // Assign the capture data.
                _isCaptureToImage = true;
                _isRunning = true;

                AssertStopped();

                // We want the capture stream rendered
                _wantCaptureRendered = true;

                // Re-render the graph (if necessary)
                RenderGraph();

                // Start the filter graph: begin capturing
                int hr = _mediaControl.Run();
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }

            // Update the state
            _graphState = GraphState.Capturing;
        }

        /// <summary>
        /// Begin capture of sound only data.
        /// </summary>
        public void StartSnapshotSound()
        {
            // If is running.
            if (!_isRunning)
            {
                // Assign the capture data.
                _isCaptureToSound = true;
                _isRunning = true;

                AssertStopped();

                // We want the capture stream rendered
                _wantCaptureRendered = true;

                // Re-render the graph (if necessary)
                RenderGraph();

                // Start the filter graph: begin capturing
                int hr = _mediaControl.Run();
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }

            // Update the state
            _graphState = GraphState.Capturing;
        }

        /// <summary>
        /// Begin capture of image and sound only data.
        /// </summary>
        public void StartSnapshotImageSound()
        {
            // If is running.
            if (!_isRunning)
            {
                // Assign the capture data.
                _isCaptureToSound = true;
                _isCaptureToImage = true;
                _isRunning = true;

                AssertStopped();

                // We want the capture stream rendered
                _wantCaptureRendered = true;

                // Re-render the graph (if necessary)
                RenderGraph();

                // Start the filter graph: begin capturing
                int hr = _mediaControl.Run();
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
            }

            // Update the state
            _graphState = GraphState.Capturing;
        }

        /// <summary> 
        ///  Stop the current capture capture. If there is no
        ///  current capture, this method will succeed.
        /// </summary>
        public void Stop()
        {
            _wantCaptureRendered = false;
            _isCaptureWrittingToFile = false;
            _isCaptureToImage = false;
            _isCaptureToSound = false;
            _isRunning = false;

            // Stop the graph if it is running
            // If we have a preview running we should only stop the
            // capture stream. However, if we have a preview stream
            // we need to re-render the graph anyways because we 
            // need to get rid of the capture stream. To re-render
            // we need to stop the entire graph
            if (_mediaControl != null)
            {
                _mediaControl.Stop();
            }

            // Update the state
            if (_graphState == GraphState.Capturing)
            {
                _graphState = GraphState.Rendered;
                if (CaptureComplete != null)
                    CaptureComplete(this, null);
            }

            // So we destroy the capture stream IF 
            // we need a preview stream. If we don't
            // this will leave the graph as it is.
            try { RenderGraph(); }
            catch { }
            try { StartPreviewIfNeeded(); }
            catch { }
        }

        /// <summary> 
		///  Create a new filter graph and add filters (devices, compressors, 
		///  misc), but leave the filters unconnected. Call renderGraph()
		///  to connect the filters.
		/// </summary>
        private void CreateGraph()
        {
            Guid cat;
            Guid med;
            int hr;

            // Ensure required properties are set
            if (_videoDevice == null && _audioDevice == null)
                throw new ArgumentException("The video and/or audio device have not been set. Please set one or both to valid capture devices.\n");

            // Skip if we are already created
            if ((int)_graphState < (int)GraphState.Created)
            {
                // Garbage collect, ensure that previous filters are released
                GC.Collect();

                // Make a new filter graph
                _graphBuilder = (IFilterGraph2)(new FilterGraph()); 

                // Get the Capture Graph Builder
                _captureGraphBuilder = (ICaptureGraphBuilder2)(new CaptureGraphBuilder2());

                // Link the CaptureGraphBuilder to the filter graph
                hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                // Get the video device and add it to the filter graph
                if (VideoDevice != null)
                {
                    hr = _graphBuilder.AddSourceFilterForMoniker(_videoDevice.DsDevice.Mon, null, _videoDevice.DsDevice.Name, out _videoDeviceFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Get the audio device and add it to the filter graph
                if (AudioDevice != null)
                {
                    hr = _graphBuilder.AddSourceFilterForMoniker(_audioDevice.DsDevice.Mon, null, _audioDevice.DsDevice.Name, out _audioDeviceFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Get the video compressor and add it to the filter graph
                if (VideoCompressor != null)
                {
                    hr = _graphBuilder.AddSourceFilterForMoniker(_videoCompressor.DsDevice.Mon, null, _videoCompressor.DsDevice.Name, out _videoCompressorFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Get the audio compressor and add it to the filter graph
                if (AudioCompressor != null)
                {
                    hr = _graphBuilder.AddSourceFilterForMoniker(_audioCompressor.DsDevice.Mon, null, _audioCompressor.DsDevice.Name, out _audioCompressorFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Retrieve the stream control interface for the video device
                // FindInterface will also add any required filters
                // (WDM devices in particular may need additional
                // upstream filters to function).

                // Try looking for an interleaved media type
                object o;
                cat = PinCategory.Capture;
                med = MediaType.Interleaved;
                Guid iid = typeof(IAMStreamConfig).GUID;
                hr = _captureGraphBuilder.FindInterface(cat, med, _videoDeviceFilter, iid, out o);

                if (hr != 0)
                {
                    // If not found, try looking for a video media type
                    med = MediaType.Video;
                    hr = _captureGraphBuilder.FindInterface(cat, med, _videoDeviceFilter, iid, out o);

                    if (hr != 0)
                        o = null;
                }

                _videoStreamConfig = o as IAMStreamConfig;

                // Retrieve the stream control interface for the audio device
                o = null;
                cat = PinCategory.Capture;
                med = MediaType.Audio;
                iid = typeof(IAMStreamConfig).GUID;
                hr = _captureGraphBuilder.FindInterface(cat, med, _audioDeviceFilter, iid, out o);
                if (hr != 0) o = null;

                _audioStreamConfig = o as IAMStreamConfig;

                // Retreive the media control interface (for starting/stopping graph)
                _mediaControl = (IMediaControl)_graphBuilder;

                // Reload any video crossbars
                if (_videoSources != null) _videoSources.Dispose(); _videoSources = null;

                // Reload any audio crossbars
                if (_audioSources != null) _audioSources.Dispose(); _audioSources = null;

                // Reload capabilities of video device
                _videoCaps = null;

                // Reload capabilities of video device
                _audioCaps = null;

                // Retrieve TV Tuner if available
                o = null;
                cat = PinCategory.Capture;
                med = MediaType.Interleaved;
                iid = typeof(IAMTVTuner).GUID;
                hr = _captureGraphBuilder.FindInterface(cat, med, _videoDeviceFilter, iid, out o);
                if (hr != 0)
                {
                    med = MediaType.Video;
                    hr = _captureGraphBuilder.FindInterface(cat, med, _videoDeviceFilter, iid, out o);
                    if (hr != 0)
                        o = null;
                }

                IAMTVTuner t = o as IAMTVTuner;
                if (t != null)
                    _tuner = new Tuner(t);

                // Update the state now that we are done
                _graphState = GraphState.Created;
            }
        }

        /// <summary>
		///  Connects the filters of a previously created graph 
		///  (created by createGraph()). Once rendered the graph
		///  is ready to be used. This method may also destroy
		///  streams if we have streams we no longer want.
		/// </summary>
        private void RenderGraph()
        {
            AssertStopped();

            // Stop the graph
            if (_mediaControl != null)
            {
                _mediaControl.Stop();
            }
                
            // Create the graph if needed (group should already be created)
            CreateGraph();

            // Derender the graph if we have a capture or preview stream
            // that we no longer want. We can't derender the capture and 
            // preview streams seperately. 
            // Notice the second case will leave a capture stream intact
            // even if we no longer want it. This allows the user that is
            // not using the preview to Stop() and Start() without
            // rerendering the graph.
            if (!_wantPreviewRendered && _isPreviewRendered)
                DerenderGraph();

            if (!_wantCaptureRendered && _isCaptureRendered)
                if (_wantPreviewRendered)
                    DerenderGraph();

            // Capture to preview.
            WriteCaptureToPreview();

            // If in the running state.
            if (_isRunning)
            {
                // Capture to file.
                WriteCaptureToFile();

                // Capture to image.
                if (_isCaptureToImage)
                    _captureImageSample.WriteCaptureToImage();

                // Capture to sound.
                if (_isCaptureToSound)
                    _captureSoundSample.WriteCaptureToSound();
            }
        }

        /// <summary>
        /// Capture data to a file.
        /// </summary>
        private void WriteCaptureToFile()
        {
            // If capture should be written to a file.
            if (_isCaptureWrittingToFile)
            {
                Guid cat;
                Guid med;
                int hr;
                bool didSomething = false;

                // Render capture stream (only if necessary)
                if (_wantCaptureRendered && !_isCaptureRendered)
                {
                    // Render the file writer portion of graph (mux -> file)
                    Guid mediaSubType = MediaSubType.Asf;

                    switch(_mediaFormat)
                    {
                        case MediaFormatType.Avi:
                            mediaSubType = MediaSubType.Avi;
                            break;

                        case MediaFormatType.Wav:
                            mediaSubType = MediaSubType.Avi;
                            break;

                        case MediaFormatType.Mpeg:
                            mediaSubType = MediaSubType.Mpeg2Video;
                            break;

                        case MediaFormatType.MpegAudio:
                            mediaSubType = MediaSubType.Mpeg2Audio;
                            break;

                        case MediaFormatType.Wma:
                        case MediaFormatType.Wmv:
                        default:
                            mediaSubType = MediaSubType.Asf;
                            break;
                    }

                    hr = _captureGraphBuilder.SetOutputFileName(mediaSubType, _filename, out _muxFilter, out _fileWriterFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                    // Render video (video -> mux)
                    if (VideoDevice != null)
                    {
                        // Try interleaved first, because if the device supports it,
                        // it's the only way to get audio as well as video
                        cat = PinCategory.Capture;
                        med = MediaType.Interleaved;
                        hr = _captureGraphBuilder.RenderStream(cat, med, _videoDeviceFilter, _videoCompressorFilter, _muxFilter);
                        if (hr < 0)
                        {
                            med = MediaType.Video;
                            hr = _captureGraphBuilder.RenderStream(cat, med, _videoDeviceFilter, _videoCompressorFilter, _muxFilter);
                            if (hr == -2147220969) throw new DeviceInUseException("Video device", hr);
                            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                        }
                    }

                    // Render audio (audio -> mux)
                    if (AudioDevice != null)
                    {
                        cat = PinCategory.Capture;
                        med = MediaType.Audio;
                        hr = _captureGraphBuilder.RenderStream(cat, med, _audioDeviceFilter, _audioCompressorFilter, _muxFilter);
                        if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                    }

                    _isCaptureRendered = true;
                    didSomething = true;

                    if (didSomething)
                        _graphState = GraphState.Rendered;
                }
            }
        }

        /// <summary>
        /// Capture data to a preview control.
        /// </summary>
        private void WriteCaptureToPreview()
        {
            Guid cat;
            Guid med;
            int hr;
            bool didSomething = false;

            // Render preview stream (only if necessary)
            if (_wantPreviewRendered && !_isPreviewRendered)
            {
                // Render video (video -> mux)
                if (VideoDevice != null)
                {
                    // Render preview (video -> renderer)
                    cat = PinCategory.Preview;
                    med = MediaType.Video;
                    hr = _captureGraphBuilder.RenderStream(cat, med, _videoDeviceFilter, null, null);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Render audio (audio -> mux)
                if (AudioDevice != null)
                {
                    // Render preview (audio -> renderer)
                    cat = PinCategory.Preview;
                    med = MediaType.Audio;
                    hr = _captureGraphBuilder.RenderStream(cat, med, _audioDeviceFilter, null, null);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Get the IVideoWindow interface
                _videoWindow = (IVideoWindow)_graphBuilder;

                // Set the video window to be a child of the main window
                hr = _videoWindow.put_Owner(_previewWindow.Handle);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                // Set video window style
                hr = _videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                // Position video window in client rect of owner window
                _previewWindow.Resize += new EventHandler(onPreviewWindowResize);
                onPreviewWindowResize(this, null);

                // Make the video window visible, now that it is properly positioned
                hr = _videoWindow.put_Visible(OABool.True);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                _isPreviewRendered = true;
                didSomething = true;

                if (didSomething)
                    _graphState = GraphState.Rendered;
            }
        }

        /// <summary>
        ///  Setup and start the preview window if the user has
        ///  requested it (by setting PreviewWindow).
        /// </summary>
        private void StartPreviewIfNeeded()
        {
            // Render preview 
            if (_wantPreviewRendered && _isPreviewRendered && !_isCaptureRendered)
            {
                // Run the graph (ignore errors)
                // We can run the entire graph becuase the capture
                // stream should not be rendered (and that is enforced
                // in the if statement above)
                _mediaControl.Run();
            }
        }

        /// <summary>
        /// Completely tear down a filter graph and 
        /// release all associated resources.
        /// </summary>
        private void DestroyGraph()
        {
            // Derender the graph (This will stop the graph
            // and release preview window. It also destroys
            // half of the graph which is unnecessary but
            // harmless here.) (ignore errors)
            try { DerenderGraph(); }
            catch { }

            // Update the state after derender because it
            // depends on correct status. But we also want to
            // update the state as early as possible in case
            // of error.
            _graphState = GraphState.Null;
            _isCaptureRendered = false;
            _isPreviewRendered = false;

            // Remove filters from the graph
            // This should be unnecessary but the Nvidia WDM
            // video driver cannot be used by this application 
            // again unless we remove it. Ideally, we should
            // simply enumerate all the filters in the graph
            // and remove them. (ignore errors)
            if (_muxFilter != null)
                _graphBuilder.RemoveFilter(_muxFilter);
            if (_videoCompressorFilter != null)
                _graphBuilder.RemoveFilter(_videoCompressorFilter);
            if (_audioCompressorFilter != null)
                _graphBuilder.RemoveFilter(_audioCompressorFilter);
            if (_videoDeviceFilter != null)
                _graphBuilder.RemoveFilter(_videoDeviceFilter);
            if (_audioDeviceFilter != null)
                _graphBuilder.RemoveFilter(_audioDeviceFilter);

            // Clean up properties
            if (_videoSources != null)
                _videoSources.Dispose(); _videoSources = null;
            if (_audioSources != null)
                _audioSources.Dispose(); _audioSources = null;
            if (_tuner != null)
                _tuner.Dispose(); _tuner = null;

            // Cleanup
            if (_graphBuilder != null)
                Marshal.ReleaseComObject(_graphBuilder); _graphBuilder = null;
            if (_captureGraphBuilder != null)
                Marshal.ReleaseComObject(_captureGraphBuilder); _captureGraphBuilder = null;
            if (_muxFilter != null)
                Marshal.ReleaseComObject(_muxFilter); _muxFilter = null;
            if (_fileWriterFilter != null)
                Marshal.ReleaseComObject(_fileWriterFilter); _fileWriterFilter = null;
            if (_videoDeviceFilter != null)
                Marshal.ReleaseComObject(_videoDeviceFilter); _videoDeviceFilter = null;
            if (_audioDeviceFilter != null)
                Marshal.ReleaseComObject(_audioDeviceFilter); _audioDeviceFilter = null;
            if (_videoCompressorFilter != null)
                Marshal.ReleaseComObject(_videoCompressorFilter); _videoCompressorFilter = null;
            if (_audioCompressorFilter != null)
                Marshal.ReleaseComObject(_audioCompressorFilter); _audioCompressorFilter = null;

            // These are copies of graphBuilder
            _mediaControl = null;
            _videoWindow = null;

            // For unmanaged objects we haven't released explicitly
            GC.Collect();
        }

        /// <summary>
        ///  Disconnect and remove all filters except the device
        ///  and compressor filters. This is the opposite of
        ///  renderGraph(). Soem properties such as FrameRate
        ///  can only be set when the device output pins are not
        ///  connected. 
        /// </summary>
        private void DerenderGraph()
        {
            // Stop the graph if it is running (ignore errors)
            if (_mediaControl != null)
            {
                _isRunning = false;
                _mediaControl.Stop();
            }

            // Free the preview window (ignore errors)
            if (_videoWindow != null)
            {
                _videoWindow.put_Visible(OABool.False);
                _videoWindow.put_Owner(IntPtr.Zero);
                _videoWindow = null;
            }

            // Remove the Resize event handler
            if (_previewWindow != null)
                _previewWindow.Resize -= new EventHandler(onPreviewWindowResize);

            if ((int)_graphState >= (int)GraphState.Rendered)
            {
                // Update the state
                _graphState = GraphState.Created;
                _isCaptureRendered = false;
                _isPreviewRendered = false;

                // Disconnect all filters downstream of the 
                // video and audio devices. If we have a compressor
                // then disconnect it, but don't remove it
                if (_videoDeviceFilter != null)
                    RemoveDownstream(_videoDeviceFilter, (_videoCompressor == null));
                if (_audioDeviceFilter != null)
                    RemoveDownstream(_audioDeviceFilter, (_audioCompressor == null));

                // These filters should have been removed by the
                // calls above. (Is there anyway to check?)
                _muxFilter = null;
                _fileWriterFilter = null;
            }
        }

        /// <summary>
        ///  Removes all filters downstream from a filter from the graph.
        ///  This is called only by derenderGraph() to remove everything
        ///  from the graph except the devices and compressors. The parameter
        ///  "removeFirstFilter" is used to keep a compressor (that should
        ///  be immediately downstream of the device) if one is begin used.
        /// </summary>
        private void RemoveDownstream(IBaseFilter filter, bool removeFirstFilter)
        {
            // Get a pin enumerator off the filter
            IEnumPins pinEnum;
            int hr = filter.EnumPins(out pinEnum);
            pinEnum.Reset();
            if ((hr == 0) && (pinEnum != null))
            {
                // Loop through each pin
                IPin[] pins = new IPin[1];
                IntPtr f = IntPtr.Zero;
                do
                {
                    // Get the next pin
                    hr = pinEnum.Next(1, pins, f);
                    if ((hr == 0) && (pins[0] != null))
                    {
                        // Get the pin it is connected to
                        IPin pinTo = null;
                        pins[0].ConnectedTo(out pinTo);
                        if (pinTo != null)
                        {
                            // Is this an input pin?
                            PinInfo info = new PinInfo();
                            hr = pinTo.QueryPinInfo(out info);
                            if ((hr == 0) && (info.dir == (PinDirection.Input)))
                            {
                                // Recurse down this branch
                                RemoveDownstream(info.filter, true);

                                // Disconnect 
                                _graphBuilder.Disconnect(pinTo);
                                _graphBuilder.Disconnect(pins[0]);

                                // Remove this filter
                                // but don't remove the video or audio compressors
                                if ((info.filter != _videoCompressorFilter) &&
                                     (info.filter != _audioCompressorFilter))
                                    _graphBuilder.RemoveFilter(info.filter);
                            }
                            Marshal.ReleaseComObject(info.filter);
                            Marshal.ReleaseComObject(pinTo);
                        }
                        Marshal.ReleaseComObject(pins[0]);
                    }
                }
                while (hr == 0);

                Marshal.ReleaseComObject(pinEnum); pinEnum = null;
            }
        }

        /// <summary>
        ///  Assert that the class is in a Stopped state.
        /// </summary>
        private void AssertStopped()
        {
            if (!Stopped)
                throw new InvalidOperationException("This operation not allowed while Capturing. Please Stop the current capture.");
        }

        /// <summary>
        /// Resize the preview when the PreviewWindow is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onPreviewWindowResize(object sender, EventArgs e)
        {
            if (_videoWindow != null)
            {
                if (_previewWindow != null)
                {
                    // Position video window in client rect of owner window
                    Rectangle rc = _previewWindow.ClientRectangle;
                    _videoWindow.SetWindowPosition(0, 0, rc.Right, rc.Bottom);
                }
            }
        }

        /// <summary>
        ///  Retrieves the value of one member of the IAMStreamConfig format block.
        ///  Helper function for several properties that expose
        ///  video/audio settings from IAMStreamConfig.GetFormat().
        ///  IAMStreamConfig.GetFormat() returns a AMMediaType struct.
        ///  AMMediaType.formatPtr points to a format block structure.
        ///  This format block structure may be one of several 
        ///  types, the type being determined by AMMediaType.formatType.
        /// </summary>
        private object GetStreamConfigSetting(IAMStreamConfig streamConfig, string fieldName)
        {
            if (streamConfig == null)
                throw new NotSupportedException();

            AssertStopped();
            DerenderGraph();

            object returnValue = null;
            AMMediaType mediaType = null;

            try
            {
                // Get the current format info
                int hr = streamConfig.GetFormat(out mediaType);
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);

                // The formatPtr member points to different structures
                // dependingon the formatType
                object formatStruct;
                if (mediaType.formatType == FormatType.WaveEx)
                    formatStruct = new WaveFormatEx();
                else if (mediaType.formatType == FormatType.VideoInfo)
                    formatStruct = new VideoInfoHeader();
                else if (mediaType.formatType == FormatType.VideoInfo2)
                    formatStruct = new VideoInfoHeader2();
                else
                    throw new NotSupportedException("This device does not support a recognized format block.");

                // Retrieve the nested structure
                Marshal.PtrToStructure(mediaType.formatPtr, formatStruct);

                // Find the required field
                Type structType = formatStruct.GetType();
                FieldInfo fieldInfo = structType.GetField(fieldName);
                if (fieldInfo == null)
                    throw new NotSupportedException("Unable to find the member '" + fieldName + "' in the format block.");

                // Extract the field's current value
                returnValue = fieldInfo.GetValue(formatStruct);

            }
            finally
            {
                DsUtils.FreeAMMediaType(mediaType);
            }

            RenderGraph();
            StartPreviewIfNeeded();

            return (returnValue);
        }

        /// <summary>
        ///  Set the value of one member of the IAMStreamConfig format block.
        ///  Helper function for several properties that expose
        ///  video/audio settings from IAMStreamConfig.GetFormat().
        ///  IAMStreamConfig.GetFormat() returns a AMMediaType struct.
        ///  AMMediaType.formatPtr points to a format block structure.
        ///  This format block structure may be one of several 
        ///  types, the type being determined by AMMediaType.formatType.
        /// </summary>
        private object SetStreamConfigSetting(IAMStreamConfig streamConfig, string fieldName, object newValue)
        {
            if (streamConfig == null)
                throw new NotSupportedException();

            AssertStopped();
            DerenderGraph();

            object returnValue = null;
            AMMediaType mediaType = null;

            try
            {
                // Get the current format info
                int hr = streamConfig.GetFormat(out mediaType);
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);

                // The formatPtr member points to different structures
                // dependingon the formatType
                object formatStruct;
                if (mediaType.formatType == FormatType.WaveEx)
                    formatStruct = new WaveFormatEx();
                else if (mediaType.formatType == FormatType.VideoInfo)
                    formatStruct = new VideoInfoHeader();
                else if (mediaType.formatType == FormatType.VideoInfo2)
                    formatStruct = new VideoInfoHeader2();
                else
                    throw new NotSupportedException("This device does not support a recognized format block.");

                // Retrieve the nested structure
                Marshal.PtrToStructure(mediaType.formatPtr, formatStruct);

                // Find the required field
                Type structType = formatStruct.GetType();
                FieldInfo fieldInfo = structType.GetField(fieldName);
                if (fieldInfo == null)
                    throw new NotSupportedException("Unable to find the member '" + fieldName + "' in the format block.");

                // Update the value of the field
                fieldInfo.SetValue(formatStruct, newValue);

                // PtrToStructure copies the data so we need to copy it back
                Marshal.StructureToPtr(formatStruct, mediaType.formatPtr, false);

                // Save the changes
                hr = streamConfig.SetFormat(mediaType);
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);
            }
            finally
            {
                DsUtils.FreeAMMediaType(mediaType);
            }

            RenderGraph();
            StartPreviewIfNeeded();

            return (returnValue);
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
                    // Dispose of the image capture sample.
                    if (_captureImageSample != null)
                        _captureImageSample.Dispose();
                    
                    // Dispose of the sound capture sample.
                    if (_captureSoundSample != null)
                        _captureSoundSample.Dispose();
                    
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _captureImageSample = null;
                _captureSoundSample = null;
                CaptureComplete = null;

                _wantPreviewRendered = false;
                _wantCaptureRendered = false;
                

                try { DestroyGraph(); }
                catch { }

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_videoSources != null)
                        _videoSources.Dispose(); 
                    if (_audioSources != null)
                        _audioSources.Dispose(); 
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _videoSources = null;
                _audioSources = null;

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
        ~Capture()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
