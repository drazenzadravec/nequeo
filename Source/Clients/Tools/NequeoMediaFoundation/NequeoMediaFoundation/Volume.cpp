/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Volume.cpp
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

#include "stdafx.h"

#include "Volume.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="uNotificationMessage">The notification message.</param>
			/// <param name="hwndNotification">The handle to the window to receive notifications.</param>
			Volume::Volume(UINT uNotificationMessage, HWND hwndNotification) :
				_cRef(1),
				_uNotificationMessage(uNotificationMessage),
				_hwndNotification(hwndNotification),
				_bNotificationsEnabled(FALSE),
				_pAudioSession(NULL),
				_pSimpleAudioVolume(NULL),
				_disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			Volume::~Volume()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
				}

				EnableNotifications(FALSE);

				// Release the session.
				SAFE_RELEASE(_pAudioSession);
				SAFE_RELEASE(_pSimpleAudioVolume);
			}

			/// <summary>
			/// Static class method to create the Volume object.
			/// </summary>
			/// <param name="uNotificationMessage">The notification message.</param>
			/// <param name="hwndNotification">The handle to the window to receive notifications.</param>
			/// <param name="ppVolume">Receives an AddRef's pointer to the Volume object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::CreateInstance(UINT uNotificationMessage, HWND hwndNotification, Volume **ppAudioSessionVolume)
			{
				HRESULT hr = S_OK;

				// Create a new instance of the Volume object.
				Volume *pAudioSessionVolume = pAudioSessionVolume = new Volume(uNotificationMessage, hwndNotification);

				// If not created.
				if (pAudioSessionVolume == NULL)
				{
					hr = E_OUTOFMEMORY;

					// Delete the instance of the player
					// if not successful.
					delete pAudioSessionVolume;
				}
				else
				{
					// Initialse.
					hr = pAudioSessionVolume->Initialize();
					if (FAILED(hr)) 
					{ 
						// Delete the instance of the player
						// if not successful.
						delete pAudioSessionVolume;
					}
					else
					{
						// Assign the new Volume instance.
						*ppAudioSessionVolume = pAudioSessionVolume;
						(*ppAudioSessionVolume)->AddRef();
					}
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initializes the Volume object. This method is called by the
			/// CreateInstance method.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::Initialize()
			{
				HRESULT hr = S_OK;

				IMMDevice *pDevice = NULL;
				IMMDeviceEnumerator *pDeviceEnumerator = NULL;
				IAudioSessionManager *pAudioSessionManager = NULL;

				// Get the enumerator for the audio endpoint devices.
				hr = CoCreateInstance(
					__uuidof(MMDeviceEnumerator),
					NULL,
					CLSCTX_INPROC_SERVER,
					IID_PPV_ARGS(&pDeviceEnumerator));

				if (FAILED(hr)) { goto done; }

				// Get the default audio endpoint that the SAR will use.
				hr = pDeviceEnumerator->GetDefaultAudioEndpoint(
					eRender,
					eConsole,   // The SAR uses 'eConsole' by default.
					&pDevice);

				if (FAILED(hr)) { goto done; }

				// Get the session manager for this device.
				hr = pDevice->Activate(
					__uuidof(IAudioSessionManager),
					CLSCTX_INPROC_SERVER,
					NULL,
					(void**)&pAudioSessionManager);

				if (FAILED(hr)) { goto done; }

				// Get the audio session. 
				hr = pAudioSessionManager->GetAudioSessionControl(
					&GUID_NULL,     // Get the default audio session. 
					FALSE,          // The session is not cross-process.
					&_pAudioSession);

				if (FAILED(hr)) { goto done; }

				hr = pAudioSessionManager->GetSimpleAudioVolume(
					&GUID_NULL, 0, &_pSimpleAudioVolume);

			done:
				SAFE_RELEASE(pDeviceEnumerator);
				SAFE_RELEASE(pDevice);
				SAFE_RELEASE(pAudioSessionManager);
				return hr;
			}

			/// <summary>
			/// Get the volume reference for the reference id.
			/// </summary>
			/// <param name="iid">The volume reference id.</param>
			/// <param name="ppv">The current volume reference.</param>
			/// <returns>The result of the operation.</returns>
			STDMETHODIMP Volume::QueryInterface(REFIID riid, void **ppv)
			{
				static const QITAB qit[] =
				{
					QITABENT(Volume, IAudioSessionEvents),
					{ 0 },
				};
				return QISearch(this, qit, riid, ppv);
			}

			/// <summary>
			/// Add a new volume ref item.
			/// </summary>
			/// <returns>The result.</returns>
			STDMETHODIMP_(ULONG) Volume::AddRef()
			{
				return InterlockedIncrement(&_cRef);
			}

			/// <summary>
			/// Release this volume resources.
			/// </summary>
			/// <returns>The result.</returns>
			STDMETHODIMP_(ULONG) Volume::Release()
			{
				LONG c = InterlockedDecrement(&_cRef);
				if (c == 0)
				{
					delete this;
				}
				return c;
			}

			/// <summary>
			/// Enables or disables notifications from the audio session. For
			/// example, if the user mutes the audio through the system volume-
			/// control program (Sndvol), the application will be notified.
			/// </summary>
			/// <param name="bEnable">True to enable; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::EnableNotifications(BOOL bEnable)
			{
				HRESULT hr = S_OK;

				if (_hwndNotification == NULL || _pAudioSession == NULL)
				{
					return E_FAIL;
				}

				if (_bNotificationsEnabled == bEnable)
				{
					// No change.
					return S_OK;
				}

				if (bEnable)
				{
					hr = _pAudioSession->RegisterAudioSessionNotification(this);
				}
				else
				{
					hr = _pAudioSession->UnregisterAudioSessionNotification(this);
				}

				if (SUCCEEDED(hr))
				{
					_bNotificationsEnabled = bEnable;
				}

				return hr;
			}

			/// <summary>
			/// Get the volume.
			/// </summary>
			/// <param name="pflVolume">The current volume.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::GetVolume(float *pflVolume)
			{
				HRESULT hr = S_OK;

				if (_pSimpleAudioVolume == NULL)
				{
					hr = E_FAIL;
				}
				else
				{
					hr = _pSimpleAudioVolume->GetMasterVolume(pflVolume);
				}
				return hr;
			}

			/// <summary>
			/// Set the volume. Ranges from 0 (silent) to 1 (full volume).
			/// </summary>
			/// <param name="flVolume">The new volume.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::SetVolume(float flVolume)
			{
				HRESULT hr = S_OK;

				if (_pSimpleAudioVolume == NULL)
				{
					hr = E_FAIL;
				}
				else
				{
					hr = _pSimpleAudioVolume->SetMasterVolume(
						flVolume,
						&AudioSessionVolumeCtx);
				}
				return hr;
			}

			/// <summary>
			/// Get the mute state.
			/// </summary>
			/// <param name="pbMute">True if muted; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::GetMute(BOOL *pbMute)
			{
				HRESULT hr = S_OK;

				if (_pSimpleAudioVolume == NULL)
				{
					hr = E_FAIL;
				}
				else
				{
					hr = _pSimpleAudioVolume->GetMute(pbMute);
				}
				return hr;
			}

			/// <summary>
			/// Set the mute state.
			/// </summary>
			/// <param name="bMute">True if muted; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::SetMute(BOOL bMute)
			{
				HRESULT hr = S_OK;

				if (_pSimpleAudioVolume == NULL)
				{
					hr = E_FAIL;
				}
				else
				{
					hr = _pSimpleAudioVolume->SetMute(
						bMute,
						&AudioSessionVolumeCtx);
				}
				return hr;
			}

			/// <summary>
			/// Set the display name.
			/// </summary>
			/// <param name="wszName">The display name.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::SetDisplayName(const WCHAR *wszName)
			{
				HRESULT hr = S_OK;

				if (_pAudioSession == NULL)
				{
					hr = E_FAIL;
				}
				else
				{
					hr = _pAudioSession->SetDisplayName(wszName, NULL);
				}
				return hr;
			}

			/// <summary>
			/// On volume changed.
			/// </summary>
			/// <param name="newVolume">The new volume value.</param>
			/// <param name="newMute">The new mute value.</param>
			/// <param name="eventContext">The event context.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT Volume::OnSimpleVolumeChanged(float newVolume, BOOL newMute, LPCGUID eventContext)
			{
				// Check if we should post a message to the application.
				if (_bNotificationsEnabled &&
					(*eventContext != AudioSessionVolumeCtx) &&
					(_hwndNotification != NULL))
				{
					// Notifications are enabled, AND
					// We did not trigger the event ourselves, AND
					// We have a valid window handle.

					// Post the message.
					::PostMessage(
						_hwndNotification,
						_uNotificationMessage,
						*((WPARAM*)(&newVolume)),  // Coerce the float.
						(LPARAM)newMute);
				}
				return S_OK;
			}
		}
	}
}