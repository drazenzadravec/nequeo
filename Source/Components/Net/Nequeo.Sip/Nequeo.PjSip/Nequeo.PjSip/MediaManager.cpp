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
MediaManager::MediaManager(pj::AudDevManager& pjAudDevManager) : _disposed(false), _pjAudDevManager(pjAudDevManager)
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