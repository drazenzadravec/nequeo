/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Volume.h
*  Purpose :       Volume class.
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

#ifndef _VOLUME_H
#define _VOLUME_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for volume control.
			/// </summary>
			class Volume : public IAudioSessionEvents
			{
			public:
				/// <summary>
				/// Static class method to create the Volume object.
				/// </summary>
				/// <param name="uNotificationMessage">The notification message.</param>
				/// <param name="hwndNotification">The handle to the window to receive notifications.</param>
				/// <param name="ppVolume">Receives an AddRef's pointer to the Volume object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				static HRESULT CreateInstance(UINT uNotificationMessage, HWND hwndNotification, Volume **ppVolume);

				/// <summary>
				/// Get the volume reference for the reference id.
				/// </summary>
				/// <param name="iid">The volume reference id.</param>
				/// <param name="ppv">The current volume reference.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP QueryInterface(REFIID iid, void** ppv);

				/// <summary>
				/// Add a new volume ref item.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) AddRef();

				/// <summary>
				/// Release this volume resources.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) Release();

				/// <summary>
				/// On volume changed.
				/// </summary>
				/// <param name="newVolume">The new volume value.</param>
				/// <param name="newMute">The new mute value.</param>
				/// <param name="eventContext">The event context.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnSimpleVolumeChanged(float newVolume, BOOL newMute, LPCGUID eventContext);

				/// <summary>
				/// On display name changed.
				/// </summary>
				/// <param name="newDisplayName">The new display name.</param>
				/// <param name="eventContext">The event context.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnDisplayNameChanged(LPCWSTR /*NewDisplayName*/, LPCGUID /*EventContext*/)
				{
					return S_OK;
				}

				/// <summary>
				/// On icon path changed.
				/// </summary>
				/// <param name="newIconPath">The new icon path.</param>
				/// <param name="eventContext">The event context.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnIconPathChanged(LPCWSTR /*NewIconPath*/, LPCGUID /*EventContext*/)
				{
					return S_OK;
				}

				/// <summary>
				/// On channel volume changed.
				/// </summary>
				/// <param name="channelCount">The channel count.</param>
				/// <param name="newChannelVolumeArray">The new channel volume array.</param>
				/// <param name="changedChannel">The changed channel.</param>
				/// <param name="eventContext">The event context.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnChannelVolumeChanged(DWORD /*ChannelCount*/, float[] /*NewChannelVolumeArray*/, DWORD /*ChangedChannel*/, LPCGUID /*EventContext*/)
				{
					return S_OK;
				}

				/// <summary>
				/// On grouping parameter changed.
				/// </summary>
				/// <param name="newGroupingParam">The grouping parameter.</param>
				/// <param name="eventContext">The event context.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnGroupingParamChanged(LPCGUID /*NewGroupingParam*/, LPCGUID /*EventContext*/)
				{
					return S_OK;
				}

				/// <summary>
				/// On state changed.
				/// </summary>
				/// <param name="newState">The new audio session state.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnStateChanged(AudioSessionState /*NewState*/)
				{
					return S_OK;
				}

				/// <summary>
				/// On session disconnected.
				/// </summary>
				/// <param name="disconnectReason">The disconnection reason.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnSessionDisconnected(AudioSessionDisconnectReason /*DisconnectReason*/)
				{
					return S_OK;
				}

				/// <summary>
				/// Enables or disables notifications from the audio session. For
				/// example, if the user mutes the audio through the system volume-
				/// control program (Sndvol), the application will be notified.
				/// </summary>
				/// <param name="bEnable">True to enable; else false.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT EnableNotifications(BOOL bEnable);

				/// <summary>
				/// Get the volume.
				/// </summary>
				/// <param name="pflVolume">The current volume.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT GetVolume(float *pflVolume);

				/// <summary>
				/// Set the volume. Ranges from 0 (silent) to 1 (full volume).
				/// </summary>
				/// <param name="flVolume">The new volume.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT SetVolume(float flVolume);

				/// <summary>
				/// Get the mute state.
				/// </summary>
				/// <param name="pbMute">True if muted; else false.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT GetMute(BOOL *pbMute);

				/// <summary>
				/// Set the mute state.
				/// </summary>
				/// <param name="bMute">True if muted; else false.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT SetMute(BOOL bMute);

				/// <summary>
				/// Set the display name.
				/// </summary>
				/// <param name="wszName">The display name.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT SetDisplayName(const WCHAR *wszName);

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="uNotificationMessage">The notification message.</param>
				/// <param name="hwndNotification">The handle to the window to receive notifications.</param>
				Volume(UINT uNotificationMessage, HWND hwndNotification);

				/// <summary>
				/// This destructor. call release to cleanup resources.
				/// </summary>
				virtual ~Volume();

				/// <summary>
				/// Initializes the Volume object. This method is called by the
				/// CreateInstance method.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				virtual HRESULT Initialize();

			protected:
				bool _disposed;

				LONG _cRef;                        // Reference count.
				UINT _uNotificationMessage;        // Window message to send when an audio event occurs.
				HWND _hwndNotification;            // Window to receives messages.
				BOOL _bNotificationsEnabled;       // Are audio notifications enabled.

				IAudioSessionControl    *_pAudioSession;
				ISimpleAudioVolume      *_pSimpleAudioVolume;

			};
		}
	}
}
#endif