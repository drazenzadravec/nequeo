/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaManager.h
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

#pragma once

#ifndef _MEDIAMANAGER_H
#define _MEDIAMANAGER_H

#include "stdafx.h"

#include "MediaType.h"
#include "AudioDeviceInfo.h"
#include "AudioMedia.h"

#include "pjsua2\media.hpp"
#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			/// <summary>
			/// Sip media manager.
			/// </summary>
			public ref class MediaManager sealed
			{
			public:
				///	<summary>
				///	Sip media manager deconstructor.
				///	</summary>
				~MediaManager();

				/// <summary>
				/// Get all audio devices installed in the system.
				/// </summary>
				/// <returns>The array of audio devices installed in the system.</returns>
				array<AudioDeviceInfo^>^ GetAllAudioDevices();

				/// <summary>
				/// Get device index based on the driver and device name.
				/// </summary>
				/// <param name="driverName">The driver name.</param>
				/// <param name="deviceName">The device name.</param>
				/// <returns>The device ID. If the device is not found, error will be thrown.</returns>
				int GetAudioDeviceID(String^ driverName, String^ deviceName);

				/// <summary>
				/// Get currently active capture sound devices. If sound devices has not been
				/// created, it is possible that the function returns -1 as device IDs.
				/// </summary>
				/// <returns>Device ID of the capture device.</returns>
				int GetCaptureDevice();

				/// <summary>
				/// Set or change capture sound device. Application may call this
				/// function at any time to replace current sound device.
				/// </summary>
				/// <param name="deviceID">Device ID of the capture device.</param>
				void SetCaptureDevice(int deviceID);

				/// <summary>
				/// Get currently active playback sound devices. If sound devices has not
				/// been created, it is possible that the function returns -1 as device IDs.
				/// </summary>
				/// <returns>Device ID of the playback device.</returns>
				int GetPlaybackDevice();

				/// <summary>
				/// Set or change playback sound device. Application may call this
				/// function at any time to replace current sound device.
				/// </summary>
				/// <param name="deviceID">Device ID of the playback device.</param>
				void SetPlaybackDevice(int deviceID);

				/// <summary>
				/// Get the AudioMedia of the capture audio device.
				/// </summary>
				/// <returns>Audio media for the capture device.</returns>
				AudioMedia^ GetCaptureDeviceMedia();

				/// <summary>
				/// Get the AudioMedia of the speaker/playback audio device.
				/// </summary>
				/// <returns>Audio media for the speaker/playback device.</returns>
				AudioMedia^ GetPlaybackDeviceMedia();

			internal:
				/// <summary>
				/// Sip media manager.
				/// </summary>
				/// <param name="pjAudDevManager">Audio device manager.</param>
				MediaManager(pj::AudDevManager& pjAudDevManager);

			private:
				bool _disposed;
				pj::AudDevManager& _pjAudDevManager;

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);
			};
		}
	}
}
#endif