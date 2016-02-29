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
#include "VideoDeviceInfo.h"
#include "AudioMedia.h"
#include "CallMapper.h"

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
				/// Get all video devices installed in the system.
				/// </summary>
				/// <returns>The array of video devices installed in the system.</returns>
				array<VideoDeviceInfo^>^ GetAllVideoDevices();

				/// <summary>
				/// Get the number of video devices installed in the system.
				/// </summary>
				/// <returns>The number of video devices installed in the system.</returns>
				int GetVideoDeviceCount();

				/// <summary>
				/// Is the video capture active.
				/// </summary>
				/// <param name="deviceID">Device ID of the capture device.</param>
				/// <returns>True if the video capture is active: else false.</returns>
				bool IsVideoCaptureActive(int deviceID);

				/// <summary>
				/// Get device index based on the driver and device name.
				/// </summary>
				/// <param name="driverName">The driver name.</param>
				/// <param name="deviceName">The device name.</param>
				/// <returns>The device ID. If the device is not found, error will be thrown.</returns>
				int GetAudioDeviceID(String^ driverName, String^ deviceName);

				/// <summary>
				/// Get device index based on the driver and device name.
				/// </summary>
				/// <param name="driverName">The driver name.</param>
				/// <param name="deviceName">The device name.</param>
				/// <returns>The device ID. If the device is not found, error will be thrown.</returns>
				int GetVideoDeviceID(String^ driverName, String^ deviceName);

				/// <summary>
				/// Set the video capture device.
				/// </summary>
				/// <param name="deviceID">Device ID of the capture device.</param>
				void SetVideoCaptureDeviceID(int deviceID);

				/// <summary>
				/// Set the video render device.
				/// </summary>
				/// <param name="deviceID">Device ID of the render device.</param>
				void SetVideoRenderDeviceID(int deviceID);

				/// <summary>
				/// Get the video capture device.
				/// </summary>
				/// <returns>The device ID.</returns>
				int GetVideoCaptureDeviceID();

				/// <summary>
				/// Get the video render device.
				/// </summary>
				/// <returns>The device ID.</returns>
				int GetVideoRenderDeviceID();

				/// <summary>
				/// Gets or sets an indicator specifying that any video capture is done automatically.
				/// </summary>
				/// <param name="value">True to enable video capture is done automatically.</param>
				void SetVideoAutoTransmit(bool value);

				/// <summary>
				/// Gets or sets an indicator specifying that any video is shown automatically.
				/// </summary>
				/// <param name="value">True to enable video is shown automatically.</param>
				void SetVideoAutoShow(bool value);

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

				/// <summary>
				/// Start conference call between remote parties; allow each party to talk to each other.
				/// </summary>
				/// <param name="conferenceCalls">Array of remote conference calls.</param>
				void StartConferenceCall(array<AudioMedia^>^ conferenceCalls);

				/// <summary>
				/// Stop conference call between remote parties; allow each party to talk to each other.
				/// </summary>
				/// <param name="conferenceCalls">Array of remote conference calls.</param>
				void StopConferenceCall(array<AudioMedia^>^ conferenceCalls);

				/// <summary>
				/// Set the video window flag.
				/// </summary>
				/// <param name="withBorder">Window with border.</param>
				/// <param name="resizable">Window is resizable.</param>
				void SetVideoWindowFlag(bool withBorder, bool resizable);

				/// <summary>
				/// Set the video window flag.
				/// </summary>
				/// <returns>
				/// 0 - No border.
				/// 1 - With border.
				/// 3 - With border and resizable.
				/// </returns>
				int SetVideoWindowFlag();

			internal:
				/// <summary>
				/// Sip media manager.
				/// </summary>
				/// <param name="pjAudDevManager">Audio device manager.</param>
				/// <param name="pjVidDevManager">Video device manager.</param>
				/// <param name="videoConfig">Video configuration.</param>
				MediaManager(pj::AudDevManager& pjAudDevManager, pj::VidDevManager& pjVidDevManager, pj::AccountVideoConfig& videoConfig);

			private:
				bool _disposed;
				int _videoCaptureID;
				int _videoRenderID;
				int _videoWindowFlag;

				pj::AudDevManager& _pjAudDevManager;
				pj::VidDevManager& _pjVidDevManager;
				pj::AccountVideoConfig& _videoConfig;

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);
			};
		}
	}
}
#endif