/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaManager.cpp
*  Purpose :       SIP MediaManager class.
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

#include "stdafx.h"

#include "MediaManager.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Sip media manager.
/// </summary>
/// <param name="pjAudDevManager">Audio device manager.</param>
/// <param name="pjVidDevManager">Video device manager.</param>
/// <param name="videoConfig">Video configuration.</param>
MediaManager::MediaManager(pj::AudDevManager& pjAudDevManager, pj::VidDevManager& pjVidDevManager, pj::AccountVideoConfig& videoConfig) :
	_disposed(false), _pjAudDevManager(pjAudDevManager), _pjVidDevManager(pjVidDevManager), _videoConfig(videoConfig),
	_videoCaptureID(-1), _videoRenderID(-2), _videoWindowFlag(1)
{
}

///	<summary>
///	Sip media manager deconstructor.
///	</summary>
MediaManager::~MediaManager()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get all audio devices installed in the system.
/// </summary>
/// <returns>The array of audio devices installed in the system.</returns>
array<AudioDeviceInfo^>^ MediaManager::GetAllAudioDevices()
{
	List<AudioDeviceInfo^>^ audioDeviceInfo = gcnew List<AudioDeviceInfo^>();
	const pj::AudioDevInfoVector& audioDevices = _pjAudDevManager.enumDev();
	
	// Get the vector size.
	size_t vectorSize = audioDevices.size();

	// If devices exist.
	if (vectorSize > 0)
	{
		// For each device.
		for (int i = 0; i < vectorSize; i++)
		{
			AudioDeviceInfo^ audoDevice = gcnew AudioDeviceInfo();
			audoDevice->Caps = audioDevices[i]->caps;
			audoDevice->DefaultSamplesPerSec = audioDevices[i]->defaultSamplesPerSec;
			audoDevice->Driver = gcnew String(audioDevices[i]->driver.c_str());
			audoDevice->InputCount = audioDevices[i]->inputCount;
			audoDevice->Name = gcnew String(audioDevices[i]->name.c_str());
			audoDevice->OutputCount = audioDevices[i]->outputCount;
			audoDevice->Routes = audioDevices[i]->routes;

			// Get the media format list.
			pj::MediaFormatVector mediaFormats = audioDevices[i]->extFmt;

			// Get the vector size format.
			size_t vectorSizeFormat = mediaFormats.size();

			// if media format exists.
			if (vectorSizeFormat > 0)
			{
				List<MediaFormat^>^ formats = gcnew List<MediaFormat^>();

				// For each format.
				for (int j = 0; j < vectorSizeFormat; j++)
				{
					MediaFormat^ mediaFormat = gcnew MediaFormat();
					mediaFormat->Id = mediaFormats[j]->id;
					mediaFormat->Type = MediaFormat::GetMediaTypeEx(mediaFormats[j]->type);

					// Add the media formats.
					formats->Add(mediaFormat);
				}

				// Add the list of media formats.
				audoDevice->MediaFormats = formats->ToArray();
			}

			// Add the audio device.
			audioDeviceInfo->Add(audoDevice);
		}
	}

	// Return the list of devices.
	return audioDeviceInfo->ToArray();
}

/// <summary>
/// Get all video devices installed in the system.
/// </summary>
/// <returns>The array of video devices installed in the system.</returns>
array<VideoDeviceInfo^>^ MediaManager::GetAllVideoDevices()
{
	List<VideoDeviceInfo^>^ videoDeviceInfo = gcnew List<VideoDeviceInfo^>();
	const pj::VideoDevInfoVector& videoDevices = _pjVidDevManager.enumDev();
	
	// Get the vector size.
	size_t vectorSize = videoDevices.size();

	// If devices exist.
	if (vectorSize > 0)
	{
		// For each device.
		for (int i = 0; i < vectorSize; i++)
		{
			VideoDeviceInfo^ videoDevice = gcnew VideoDeviceInfo();
			videoDevice->Id = videoDevices[i]->id;
			videoDevice->Caps = videoDevices[i]->caps;
			videoDevice->Driver = gcnew String(videoDevices[i]->driver.c_str());
			videoDevice->Name = gcnew String(videoDevices[i]->name.c_str());
			videoDevice->Direction = CallMapper::GetMediaDirectionEx(videoDevices[i]->dir);

			// Get the media format list.
			pj::MediaFormatVector mediaFormats = videoDevices[i]->fmt;

			// Get the vector size format.
			size_t vectorSizeFormat = mediaFormats.size();

			// if media format exists.
			if (vectorSizeFormat > 0)
			{
				List<MediaFormat^>^ formats = gcnew List<MediaFormat^>();

				// For each format.
				for (int j = 0; j < vectorSizeFormat; j++)
				{
					MediaFormat^ mediaFormat = gcnew MediaFormat();
					mediaFormat->Id = mediaFormats[j]->id;
					mediaFormat->Type = MediaFormat::GetMediaTypeEx(mediaFormats[j]->type);

					// Add the media formats.
					formats->Add(mediaFormat);
				}

				// Add the list of media formats.
				videoDevice->MediaFormats = formats->ToArray();
			}

			// Add the video device.
			videoDeviceInfo->Add(videoDevice);
		}
	}

	// Return the list of devices.
	return videoDeviceInfo->ToArray();
}

/// <summary>
/// Get the number of video devices installed in the system.
/// </summary>
/// <returns>The number of video devices installed in the system.</returns>
int MediaManager::GetVideoDeviceCount()
{
	return _pjVidDevManager.getDevCount();
}

/// <summary>
/// Is the video capture active.
/// </summary>
/// <param name="deviceID">Device ID of the capture device.</param>
/// <returns>True if the video capture is active: else false.</returns>
bool MediaManager::IsVideoCaptureActive(int deviceID)
{
	return _pjVidDevManager.isCaptureActive(deviceID);
}

/// <summary>
/// Get device index based on the driver and device name.
/// </summary>
/// <param name="driverName">The driver name.</param>
/// <param name="deviceName">The device name.</param>
/// <returns>The device ID. If the device is not found, error will be thrown.</returns>
int MediaManager::GetAudioDeviceID(String^ driverName, String^ deviceName)
{
	std::string driver_Name = "";
	MarshalString(driverName, driver_Name);

	std::string device_Name = "";
	MarshalString(deviceName, device_Name);

	return _pjAudDevManager.lookupDev(driver_Name, device_Name);
}

/// <summary>
/// Get device index based on the driver and device name.
/// </summary>
/// <param name="driverName">The driver name.</param>
/// <param name="deviceName">The device name.</param>
/// <returns>The device ID. If the device is not found, error will be thrown.</returns>
int MediaManager::GetVideoDeviceID(String^ driverName, String^ deviceName)
{
	std::string driver_Name = "";
	MarshalString(driverName, driver_Name);

	std::string device_Name = "";
	MarshalString(deviceName, device_Name);

	return _pjVidDevManager.lookupDev(driver_Name, device_Name);
}

/// <summary>
/// Get currently active capture sound devices. If sound devices has not been
/// created, it is possible that the function returns -1 as device IDs.
/// </summary>
/// <returns>Device ID of the capture device.</returns>
int MediaManager::GetCaptureDevice()
{
	return _pjAudDevManager.getCaptureDev();
}

/// <summary>
/// Set or change capture sound device. Application may call this
/// function at any time to replace current sound device.
/// </summary>
/// <param name="deviceID">Device ID of the capture device.</param>
void MediaManager::SetCaptureDevice(int deviceID)
{
	_pjAudDevManager.setCaptureDev(deviceID);
}

/// <summary>
/// Get currently active playback sound devices. If sound devices has not
/// been created, it is possible that the function returns -1 as device IDs.
/// </summary>
/// <returns>Device ID of the playback device.</returns>
int MediaManager::GetPlaybackDevice()
{
	return _pjAudDevManager.getPlaybackDev();
}

/// <summary>
/// Set or change playback sound device. Application may call this
/// function at any time to replace current sound device.
/// </summary>
/// <param name="deviceID">Device ID of the playback device.</param>
void MediaManager::SetPlaybackDevice(int deviceID)
{
	_pjAudDevManager.setPlaybackDev(deviceID);
}

/// <summary>
/// Get the AudioMedia of the capture audio device.
/// </summary>
/// <returns>Audio media for the capture device.</returns>
AudioMedia^ MediaManager::GetCaptureDeviceMedia()
{
	return gcnew AudioMedia(_pjAudDevManager.getCaptureDevMedia());
}

/// <summary>
/// Get the AudioMedia of the speaker/playback audio device.
/// </summary>
/// <returns>Audio media for the speaker/playback device.</returns>
AudioMedia^ MediaManager::GetPlaybackDeviceMedia()
{
	return gcnew AudioMedia(_pjAudDevManager.getPlaybackDevMedia());
}

/// <summary>
/// Set the video capture device.
/// </summary>
/// <param name="deviceID">Device ID of the capture device.</param>
void MediaManager::SetVideoCaptureDeviceID(int deviceID)
{
	if (_videoCaptureID >= 0)
	{
		_videoConfig.defaultCaptureDevice = deviceID;
		_videoCaptureID = deviceID;
	}
	else
	{
		_videoConfig.defaultCaptureDevice = pjmedia_vid_dev_std_index::PJMEDIA_VID_DEFAULT_CAPTURE_DEV;
		_videoCaptureID = -1;
	}
}

/// <summary>
/// Set the video render device.
/// </summary>
/// <param name="deviceID">Device ID of the render device.</param>
void MediaManager::SetVideoRenderDeviceID(int deviceID)
{
	if (_videoRenderID >= 0)
	{
		_videoConfig.defaultRenderDevice = deviceID;
		_videoRenderID = deviceID;
	}
	else
	{
		_videoConfig.defaultRenderDevice = pjmedia_vid_dev_std_index::PJMEDIA_VID_DEFAULT_RENDER_DEV;
		_videoRenderID = -2;
	}
}

/// <summary>
/// Get the video capture device.
/// </summary>
/// <returns>The device ID.</returns>
int MediaManager::GetVideoCaptureDeviceID()
{
	return _videoCaptureID;
}

/// <summary>
/// Get the video render device.
/// </summary>
/// <returns>The device ID.</returns>
int MediaManager::GetVideoRenderDeviceID()
{
	return _videoRenderID;
}

/// <summary>
/// Gets or sets an indicator specifying that any video capture is done automatically.
/// </summary>
/// <param name="value">True to enable video capture is done automatically.</param>
void MediaManager::SetVideoAutoTransmit(bool value)
{
	_videoConfig.autoTransmitOutgoing = value;
}

/// <summary>
/// Gets or sets an indicator specifying that any video is shown automatically.
/// </summary>
/// <param name="value">True to enable video is shown automatically.</param>
void MediaManager::SetVideoAutoShow(bool value)
{
	_videoConfig.autoShowIncoming = value;
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void MediaManager::MarshalString(String^ s, std::string& os)
{
	using namespace Runtime::InteropServices;
	const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
	os = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void MediaManager::MarshalString(String^ s, std::wstring& os)
{
	using namespace Runtime::InteropServices;
	const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
	os = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}

/// <summary>
/// Start conference call between remote parties; allow each party to talk to each other.
/// </summary>
/// <param name="conferenceCalls">Array of remote conference calls.</param>
void MediaManager::StartConferenceCall(array<AudioMedia^>^ conferenceCalls)
{
	// For each call.
	for (int i = 0; i < conferenceCalls->Length; i++)
	{
		// Get first group.
		pj::AudioMedia& mediaCall_1 = conferenceCalls[i]->GetAudioMedia();

		// For each call.
		for (int j = 0; j < conferenceCalls->Length; j++)
		{
			// Get second group.
			pj::AudioMedia& mediaCall_2 = conferenceCalls[j]->GetAudioMedia();

			// If the two audio media are not equal.
			if (mediaCall_1.getPortId() != mediaCall_2.getPortId())
			{
				// Allow these two calls to communicate.
				mediaCall_1.startTransmit(mediaCall_2);
			}
		}
	}
}

/// <summary>
/// Stop conference call between remote parties; allow each party to talk to each other.
/// </summary>
/// <param name="conferenceCalls">Array of remote conference calls.</param>
void MediaManager::StopConferenceCall(array<AudioMedia^>^ conferenceCalls)
{
	// For each call.
	for (int i = 0; i < conferenceCalls->Length; i++)
	{
		// Get first group.
		pj::AudioMedia& mediaCall_1 = conferenceCalls[i]->GetAudioMedia();

		// For each call.
		for (int j = 0; j < conferenceCalls->Length; j++)
		{
			// Get second group.
			pj::AudioMedia& mediaCall_2 = conferenceCalls[j]->GetAudioMedia();

			// If the two audio media are not equal.
			if (mediaCall_1.getPortId() != mediaCall_2.getPortId())
			{
				// Stop these two calls from communicating.
				mediaCall_1.stopTransmit(mediaCall_2);
			}
		}
	}
}

/// <summary>
/// Set the video window flag.
/// </summary>
/// <param name="withBorder">Window with border.</param>
/// <param name="resizable">Window is resizable.</param>
void MediaManager::SetVideoWindowFlag(bool withBorder, bool resizable)
{
	// If no border.
	if (!withBorder)
	{
		// Video window without border and not resizable.
		_videoConfig.windowFlags = 0;
		_videoWindowFlag = 0;
	}
	else if(withBorder && !resizable)
	{
		// Video window with border and not resizable.
		_videoConfig.windowFlags = pjmedia_vid_dev_wnd_flag::PJMEDIA_VID_DEV_WND_BORDER;
		_videoWindowFlag = 1;
	}
	else
	{
		// Video window with border and resizable.
		_videoConfig.windowFlags = pjmedia_vid_dev_wnd_flag::PJMEDIA_VID_DEV_WND_BORDER | pjmedia_vid_dev_wnd_flag::PJMEDIA_VID_DEV_WND_RESIZABLE;
		_videoWindowFlag = 3;
	}
}

/// <summary>
/// Set the video window flag.
/// </summary>
/// <returns>
/// 0 - No border.
/// 1 - With border.
/// 3 - With border and resizable.
/// </returns>
int MediaManager::SetVideoWindowFlag()
{
	return _videoWindowFlag;
}