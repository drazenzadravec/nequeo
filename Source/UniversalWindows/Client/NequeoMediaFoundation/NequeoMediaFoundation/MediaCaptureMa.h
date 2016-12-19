/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaCaptureMa.h
*  Purpose :       MediaCapture Managed class.
*
*/

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

#pragma once

using namespace concurrency;
using namespace Windows::Foundation::Collections;
using namespace Windows::Media::Capture;
using namespace Windows::Devices::Enumeration;

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Contains the video or audio details.
			/// </summary>
			public ref class CaptureDevice sealed
			{
			public:
				/// <summary>
				/// Gets or sets the device index.
				/// </summary>
				property int Index
				{
					int get() { return _index; }
					void set(int value) { _index = value; }
				}

				/// <summary>
				/// Gets or sets the ID.
				/// </summary>
				property Platform::String^ ID 
				{ 
					Platform::String^ get() { return _id; } 
					void set(Platform::String^ value) { _id = value; }
				}

				/// <summary>
				/// Gets or sets the Name.
				/// </summary>
				property Platform::String^ Name 
				{ 
					Platform::String^ get() { return _name; } 
					void set(Platform::String^ value) { _name = value; }
				}

				/// <summary>
				/// Gets or sets an indicator specifying if the device is the default.
				/// </summary>
				property bool IsDefault 
				{ 
					bool get() { return _isDefault; } 
					void set(bool value) { _isDefault = value; }
				}

				/// <summary>
				/// Gets or sets and indicator specifying if the device is enabled.
				/// </summary>
				property bool IsEnabled 
				{ 
					bool get() { return _isEnabled; } 
					void set(bool value) { _isEnabled = value; }
				}

				/// <summary>
				/// Gets or sets the location of the device (e.g. Front, Back, ...).
				/// </summary>
				property Platform::String^ Location 
				{ 
					Platform::String^ get() { return _location; } 
					void set(Platform::String^ value) { _location = value; }
				}

			private:
				int _index;
				Platform::String^ _id;
				Platform::String^ _name;
				bool _isDefault;
				bool _isEnabled;
				Platform::String^ _location;
			};

			/// <summary>
			/// Contains the video and audio settings.
			/// </summary>
			public value struct CustomSetting
			{
				bool RotateVideoOnOrientationChange;
				bool ReversePreviewRotation;
			};

			public delegate void VideoCaptureDevicesCompleteCallback(IVector<CaptureDevice^>^);
			public delegate void AudioCaptureDevicesCompleteCallback(IVector<CaptureDevice^>^);

			ref class MediaCaptureMa;

			public delegate void InitialiseCaptureHandler(MediaCaptureMa^);
			public delegate void NotifyHandler(MediaCaptureMa^, Platform::String^);
			public delegate void ExceptionHandler(MediaCaptureMa^, Platform::Exception^);

			/// <summary>
			/// Providers the base for a media foundation capture.
			/// </summary>
			public ref class MediaCaptureMa sealed
			{
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				MediaCaptureMa();

				/// <summary>
				/// This destructor.
				/// </summary>
				virtual ~MediaCaptureMa();

				/// <summary>
				/// Any internal exception handler.
				/// </summary>
				event ExceptionHandler^ OnException;

				/// <summary>
				/// On notify handler.
				/// </summary>
				event NotifyHandler^ OnNotify;

				/// <summary>
				/// On initialise capture handler.
				/// </summary>
				event InitialiseCaptureHandler^ OnInitialiseCapture;

				/// <summary>
				/// Get the video capture devices.
				/// </summary>
				/// <param name="videoCollection">The callback to invoke when complete.</param>
				void GetVideoCaptureDevicesAsync(VideoCaptureDevicesCompleteCallback^ videoCollection);

				/// <summary>
				/// Get the audio capture devices.
				/// </summary>
				/// <param name="audioCollection">The callback to invoke when complete.</param>
				void GetAudioCaptureDevicesAsync(AudioCaptureDevicesCompleteCallback^ audioCollection);

				/// <summary>
				/// Set the video capture device.
				/// </summary>
				/// <param name="device">The media foundation device.</param>
				void SetVideoDevice(CaptureDevice^ device);

				/// <summary>
				/// Set the audio capture device.
				/// </summary>
				/// <param name="device">The media foundation device.</param>
				void SetAudioDevice(CaptureDevice^ device);

				/// <summary>
				/// Set custom setting.
				/// </summary>
				/// <param name="settings">Custom settings.</param>
				void SetCustomSettings(CustomSetting settings);

				/// <summary>
				/// Initialization capture.
				/// </summary>
				void InitializationCapture();

			private:
				bool _disposed;
				bool _initilaised;
				bool _lowLagPrepared;

				CustomSetting _customSettings;
				MediaCaptureInitializationSettings^ _settings;

				Platform::Agile<MediaCapture> _mediaCapture;
				Platform::Agile<LowLagPhotoCapture> _lowLagPhoto;
				Platform::Agile<LowLagMediaRecording> _lowLagRecord;

				DeviceInformationCollection^ _videoCollection;
				DeviceInformationCollection^ _audioCollection;

				CaptureDevice^ _videoDevice;
				CaptureDevice^ _audioDevice;

				bool _hasVideoCapture;
				bool _hasAudioCapture;

				void InitializationSettings();
				void StartVideoAudioCapture();
				void OnRecordLimitationExceeded(MediaCapture ^sender);
				void OnFailed(MediaCapture ^sender, MediaCaptureFailedEventArgs ^errorEventArgs);
			};
		}
	}
}
