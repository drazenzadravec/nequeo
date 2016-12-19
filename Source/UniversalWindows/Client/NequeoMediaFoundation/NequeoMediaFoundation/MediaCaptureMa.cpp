/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaCaptureMa.cpp
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

#include "pch.h"

#include "MediaCaptureMa.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Media::Capture;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Media::MediaProperties;

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			MediaCaptureMa::MediaCaptureMa() :
				_mediaCapture(nullptr),
				_lowLagPhoto(nullptr),
				_lowLagRecord(nullptr),
				_settings(nullptr),
				_videoCollection(nullptr),
				_audioCollection(nullptr),
				_videoDevice(nullptr),
				_audioDevice(nullptr),
				_hasVideoCapture(false),
				_hasAudioCapture(false),
				_initilaised(false),
				_lowLagPrepared(false),
				_disposed(false)
			{
				auto mediaCapture = ref new MediaCapture();
				_mediaCapture = mediaCapture;
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaCaptureMa::~MediaCaptureMa()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					auto mediaCapture = _mediaCapture.Get();
					if (mediaCapture != nullptr)
					{
						delete mediaCapture;
						mediaCapture = nullptr;
					}

					if (_settings != nullptr)
					{
						delete _settings;
						_settings = nullptr;
					}
				}
			}

			/// <summary>
			/// Get the video capture devices.
			/// </summary>
			/// <param name="videoCollection">The callback to invoke when complete.</param>
			void MediaCaptureMa::GetVideoCaptureDevicesAsync(VideoCaptureDevicesCompleteCallback^ videoCollection)
			{
				_videoCollection = nullptr;

				// Create a new task.
				create_task(DeviceInformation::FindAllAsync(DeviceClass::VideoCapture))
					.then([this, videoCollection](task<DeviceInformationCollection^> findTask)
				{
					try
					{
						// Execute the task a wait until complete.
						_videoCollection = findTask.get();

						// If no video capture devices exist.
						if (_videoCollection == nullptr || _videoCollection->Size == 0)
						{
							// Send empty.
							videoCollection(nullptr);
						}
						else
						{
							auto items = ref new Vector<CaptureDevice^>();
							
							// For each device.
							for (unsigned int i = 0; i < _videoCollection->Size; i++)
							{
								auto device = ref new CaptureDevice();

								// Get the current device.
								auto devInfo = _videoCollection->GetAt(i);
								auto location = devInfo->EnclosureLocation;

								// Assign the details of the device.
								device->Index = i;
								device->ID = devInfo->Id;
								device->Name = devInfo->Name;
								device->IsDefault = devInfo->IsDefault;
								device->IsEnabled = devInfo->IsEnabled;

								// Location on device.
								if (location != nullptr)
								{
									// If location at front
									if (location->Panel == Windows::Devices::Enumeration::Panel::Front)
									{
										device->Location = "Front";
									}
									else if (location->Panel == Windows::Devices::Enumeration::Panel::Back)
									{
										device->Location = "Back";
									}
									else
									{
										device->Location = "None";
									}
								}

								// Add the device.
								items->Append(device);
							}

							// Call the callback.
							videoCollection(items);
						}
					}
					catch (Exception^ e)
					{
						// Send empty.
						videoCollection(nullptr);

						// Call the handler.
						OnException(this, e);
					}
				});
			}

			/// <summary>
			/// Get the audio capture devices.
			/// </summary>
			/// <param name="audioCollection">The callback to invoke when complete.</param>
			void MediaCaptureMa::GetAudioCaptureDevicesAsync(AudioCaptureDevicesCompleteCallback^ audioCollection)
			{
				_audioCollection = nullptr;

				// Create a new task.
				create_task(DeviceInformation::FindAllAsync(DeviceClass::AudioCapture))
					.then([this, audioCollection](task<DeviceInformationCollection^> findTask)
				{
					try
					{
						// Execute the task a wait until complete.
						_audioCollection = findTask.get();

						// If no audio capture devices exist.
						if (_audioCollection == nullptr || _audioCollection->Size == 0)
						{
							// Send empty.
							audioCollection(nullptr);
						}
						else
						{
							auto items = ref new Vector<CaptureDevice^>();

							// For each device.
							for (unsigned int i = 0; i < _audioCollection->Size; i++)
							{
								auto device = ref new CaptureDevice();

								// Get the current device.
								auto devInfo = _audioCollection->GetAt(i);
								auto location = devInfo->EnclosureLocation;

								// Assign the details of the device.
								device->Index = i;
								device->ID = devInfo->Id;
								device->Name = devInfo->Name;
								device->IsDefault = devInfo->IsDefault;
								device->IsEnabled = devInfo->IsEnabled;

								// Location on device.
								if (location != nullptr)
								{
									// If location at front
									if (location->Panel == Windows::Devices::Enumeration::Panel::Front)
									{
										device->Location = "Front";
									}
									else if (location->Panel == Windows::Devices::Enumeration::Panel::Back)
									{
										device->Location = "Back";
									}
									else
									{
										device->Location = "None";
									}
								}

								// Add the device.
								items->Append(device);
							}

							// Call the callback.
							audioCollection(items);
						}
					}
					catch (Exception^ e)
					{
						// Send empty.
						audioCollection(nullptr);

						// Call the handler.
						OnException(this, e);
					}
				});
			}

			/// <summary>
			/// Set the video capture device.
			/// </summary>
			/// <param name="device">The media foundation device.</param>
			void MediaCaptureMa::SetVideoDevice(CaptureDevice^ device)
			{
				// Has video.
				if (device != nullptr)
				{
					_videoDevice = device;
					_hasVideoCapture = true;
				}
				else
				{
					_videoDevice = nullptr;
					_hasVideoCapture = false;
				}

				// Initialization settings.
				InitializationSettings();
			}

			/// <summary>
			/// Set the audio capture device.
			/// </summary>
			/// <param name="device">The media foundation device.</param>
			void MediaCaptureMa::SetAudioDevice(CaptureDevice^ device)
			{
				// Has audio.
				if (device != nullptr)
				{
					_audioDevice = device;
					_hasAudioCapture = true;
				}
				else
				{
					_audioDevice = nullptr;
					_hasAudioCapture = false;
				}

				// Initialization settings.
				InitializationSettings();
			}

			/// <summary>
			/// Set custom setting.
			/// </summary>
			/// <param name="settings">Custom settings.</param>
			void MediaCaptureMa::SetCustomSettings(CustomSetting settings)
			{
				_customSettings = settings;
			}

			/// <summary>
			/// Initialization settings.
			/// </summary>
			void MediaCaptureMa::InitializationSettings()
			{
				// Has no video and audio.
				if (!_hasVideoCapture && !_hasAudioCapture)
				{
					_settings = nullptr;
				}
				else
				{
					// Create a new instance.
					if (_settings == nullptr)
					{
						// Capture settings.
						_settings = ref new Windows::Media::Capture::MediaCaptureInitializationSettings();
					}

					// Has video capture.
					if (_hasVideoCapture)
					{
						// Set the video id.
						_settings->VideoDeviceId = _videoDevice->ID;
					}

					// Has audio capture.
					if (_hasAudioCapture)
					{
						// Set the audio id.
						_settings->AudioDeviceId = _audioDevice->ID;
					}
				}
			}

			/// <summary>
			/// Initialization capture.
			/// </summary>
			void MediaCaptureMa::InitializationCapture()
			{
				// Not initilaised
				if (!_initilaised)
				{
					// Has video or audio capture.
					if (_hasVideoCapture || _hasAudioCapture)
					{
						// Get the media capture.
						auto mediaCapture = _mediaCapture.Get();

						// Initialise the capture.
						create_task(mediaCapture->InitializeAsync(_settings))
							.then([this](task<void> initTask)
						{
							try
							{
								initTask.get();
								auto mediaCapture = _mediaCapture.Get();

								mediaCapture->RecordLimitationExceeded += ref new Windows::Media::Capture::RecordLimitationExceededEventHandler(this, &MediaCaptureMa::OnRecordLimitationExceeded);
								mediaCapture->Failed += ref new Windows::Media::Capture::MediaCaptureFailedEventHandler(this, &MediaCaptureMa::OnFailed);

								// If concurrent record and photo support.
								if (_mediaCapture->MediaCaptureSettings->ConcurrentRecordAndPhotoSupported)
								{
									//prepare lowlag photo, then prepare lowlag record
									create_task(_mediaCapture->PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties::CreateJpeg()))
										.then([this](LowLagPhotoCapture ^photoCapture)
									{
										try
										{
											_lowLagPhoto = photoCapture;
											_lowLagPrepared = true;
										}
										catch (Exception ^e)
										{
											// Call the handler.
											OnException(this, e);
										}
									});
								}

								_initilaised = true;
								OnInitialiseCapture(this);
							}
							catch (Exception^ e)
							{
								// Call the handler.
								OnException(this, e);
							}
						});
					}
					else
					{
						// No video or audio capture.
						OnNotify(this, "No video or audio capture devices.");
					}
				}
			}

			void MediaCaptureMa::StartVideoAudioCapture()
			{
				// Has video and audio capture.
				if (_hasVideoCapture && _hasAudioCapture)
				{
					// Is initilaised
					if (_initilaised)
					{

					}
					else
					{
						// No video or audio capture.
						OnNotify(this, "Initialise the capture first.");
					}
				}
				else
				{
					// No video or audio capture.
					OnNotify(this, "No video and audio capture devices.");
				}
			}

			/// <summary>
			/// Record Limitation Exceeded.
			/// </summary>
			/// <param name="sender">Sender.</param>
			void MediaCaptureMa::OnRecordLimitationExceeded(MediaCapture ^sender)
			{
				
			}

			/// <summary>
			/// Failed.
			/// </summary>
			/// <param name="sender">Sender.</param>
			/// <param name="errorEventArgs">Failed arguments.</param>
			void MediaCaptureMa::OnFailed(MediaCapture ^sender, MediaCaptureFailedEventArgs ^errorEventArgs)
			{
				String^ message = "Fatal error, " + errorEventArgs->Message;
				OnException(this, ref new Exception(0, message));
			}
		}
	}
}




