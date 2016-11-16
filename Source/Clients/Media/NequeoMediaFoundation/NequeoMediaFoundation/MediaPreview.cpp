/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPreview.cpp
*  Purpose :       MediaPreview class.
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

#include "MediaPreview.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="hVideo">The handle to the video window.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="hr">The result reference.</param>
			MediaPreview::MediaPreview(HWND hVideo, HWND hEvent, HRESULT &hr) :
				_pPreview(NULL),
				_pSource(NULL),
				_nRefCount(1),
				_hwndVideo(hVideo),
				_hwndEvent(hEvent),
				_bHasVideo(FALSE),
				_pwszSymbolicLink(NULL),
				_cchSymbolicLink(0),
				_hNotifyStateEvent(NULL),
				_hNotifyErrorEvent(NULL),
				_previewState(PreviewReady),
				_closed(false),
				_disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaPreview::~MediaPreview()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					Close();
				}
			}

			/// <summary>
			/// Static class method to create the MediaPreview object.
			/// </summary>
			/// <param name="hVideo">The handle to the video window.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaPreview object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPreview::CreateInstance(HWND hVideo, HWND hEvent, MediaPreview **ppPreview)
			{
				// Make sure the a video and event handler exists.
				assert(hVideo != NULL);
				assert(hEvent != NULL);

				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// MediaPreview constructor sets the ref count to zero.
				// Create method calls AddRef.
				// Create a new medis player instance.
				MediaPreview *pPreview = new MediaPreview(hVideo, hEvent, hr);

				// If the preview was not created.
				if (pPreview == NULL)
				{
					// Out of memory result.
					hr = E_OUTOFMEMORY;
				}

				// If successful initialisation.
				if (SUCCEEDED(hr))
				{
					// Increment the reference count of the current player.
					*ppPreview = pPreview;
					(*ppPreview)->AddRef();
				}
				else
				{
					// Delete the instance of the preview
					// if not successful.
					delete pPreview;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaPreview::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaPreview::Release()
			{
				// Decrement the player ref count.
				ULONG uCount = InterlockedDecrement(&_nRefCount);

				// If released.
				if (uCount == 0)
				{
					// Delete this players resources.
					delete this;
				}

				// For thread safety, return a temporary variable.
				return uCount;
			}

			/// <summary>
			/// Get the player reference for the reference id.
			/// </summary>
			/// <param name="iid">The player reference id.</param>
			/// <param name="ppv">The current player reference.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPreview::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}

				// Attach MediaPreview to the interface.
				static const QITAB qit[] =
				{
					QITABENT(MediaPreview, IMFPMediaPlayerCallback),
					{ 0 },
				};
				return QISearch(this, qit, iid, ppv);
			}

			/// <summary>
			/// Set the video capture device.
			/// </summary>
			/// <param name="pActivate">The media foundation device.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPreview::SetDevice(IMFActivate *pActivate)
			{
				HRESULT hr = S_OK;

				IMFMediaSource *pSource = NULL;

				// Release the current instance of the player (if any).
				Close();

				// Start a new operation.
				_closed = false;

				// Create a new instance of the player.
				hr = MFPCreateMediaPlayer(
					NULL,   // URL
					FALSE,
					0,      // Options
					this,   // Callback
					_hwndVideo,
					&_pPreview
				);

				// Create the media source for the device.
				if (SUCCEEDED(hr))
				{
					hr = pActivate->ActivateObject(
						__uuidof(IMFMediaSource),
						(void**)&pSource
					);
				}

				// Get the symbolic link. This is needed to handle device-
				// loss notifications. (See CheckDeviceLost.)
				if (SUCCEEDED(hr))
				{
					hr = pActivate->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK,
						&_pwszSymbolicLink,
						&_cchSymbolicLink
					);

				}

				// Create a new media item for this media source.
				if (SUCCEEDED(hr))
				{
					// Create a new media item for this media source.
					hr = _pPreview->CreateMediaItemFromObject(
						pSource,
						FALSE,  // FALSE = asynchronous call
						0,
						NULL
					);
				}

				// When the method completes, MFPlay will call OnMediaPlayerEvent
				// with the MFP_EVENT_TYPE_MEDIAITEM_CREATED event.
				if (SUCCEEDED(hr))
				{
					_pSource = pSource;
					_pSource->AddRef();
				}

				// If failed.
				if (FAILED(hr))
				{
					// Close.
					Close();
				}

				// If exists.
				if (pSource)
				{
					// release.
					pSource->Release();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Close the playback device.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPreview::Close()
			{
				HRESULT hr = S_OK;

				// If not closed.
				if (!_closed)
				{
					_closed = true;

					if (_pPreview)
					{
						// Shutdown the preview.
						_pPreview->Shutdown();
						_pPreview->Release();
						_pPreview = NULL;
					}

					if (_pSource)
					{
						// Shutdown the source.
						_pSource->Shutdown();
						_pSource->Release();
						_pSource = NULL;
					}

					_bHasVideo = FALSE;

					// Release the symbolic link.
					CoTaskMemFree(_pwszSymbolicLink);
					_pwszSymbolicLink = NULL;

					_cchSymbolicLink = 0;
				}

				return hr;
			}

			/// <summary>
			/// Repaint the video window. The application should call this method when it receives a WM_SIZE message.
			/// </summary>
			/// <param name="width">The width of the player.</param>
			/// <param name="height">The height of the player.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT	MediaPreview::ResizeVideo(WORD width, WORD height)
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				// If a video display control has been assigned.
				if (_pPreview)
				{
					MFVideoNormalizedRect nRect = { 0.0f, 0.0f, width, height };

					// Set the player position in the control.
					hr = _pPreview->SetVideoSourceRect(&nRect);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Update the video.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPreview::UpdateVideo()
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				if (_pPreview)
				{
					// Update the video.
					hr = _pPreview->UpdateVideo();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Get device lost in the collection.
			/// </summary>
			/// <param name="pHdr">The device broadcast handler.</param>
			/// <param name="pbDeviceLost">Is device lost.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPreview::CheckDeviceLost(DEV_BROADCAST_HDR *pHdr, BOOL *pbDeviceLost)
			{
				// Device
				DEV_BROADCAST_DEVICEINTERFACE *pDi = NULL;

				// If no reference.
				if (pbDeviceLost == NULL)
				{
					return E_POINTER;
				}

				// Initialy flase.
				*pbDeviceLost = FALSE;

				// If not exists.
				if (_pSource == NULL)
				{
					return S_OK;
				}

				// If not exists
				if (pHdr == NULL)
				{
					return S_OK;
				}

				// If not device interface.
				if (pHdr->dbch_devicetype != DBT_DEVTYP_DEVICEINTERFACE)
				{
					return S_OK;
				}

				// Compare the device name with the symbolic link.
				pDi = (DEV_BROADCAST_DEVICEINTERFACE*)pHdr;

				// If link
				if (_pwszSymbolicLink)
				{
					// If device not in list the lost.
					if (_wcsicmp(_pwszSymbolicLink, pDi->dbcc_name) == 0)
					{
						// Set to true.
						*pbDeviceLost = TRUE;
					}
				}

				return S_OK;
			}

			/// <summary>
			/// On media player event.
			/// </summary>
			/// <param name="pEventHeader">The event header.</param>
			void STDMETHODCALLTYPE MediaPreview::OnMediaPlayerEvent(MFP_EVENT_HEADER * pEventHeader)
			{
				if (FAILED(pEventHeader->hrEvent))
				{
					return;
				}

				// Send the event.
				switch (pEventHeader->eEventType)
				{
				case MFP_EVENT_TYPE_MEDIAITEM_CREATED:
					OnMediaItemCreated(MFP_GET_MEDIAITEM_CREATED_EVENT(pEventHeader));
					break;

				case MFP_EVENT_TYPE_MEDIAITEM_SET:
					OnMediaItemSet(MFP_GET_MEDIAITEM_SET_EVENT(pEventHeader));
					break;
				}

				// Send a message inticating the preview is ready.
				_previewState = PreviewReady;
				NotifyState();
			}

			/// <summary>
			/// On media item created.
			/// </summary>
			/// <param name="pEvent">The event header.</param>
			void MediaPreview::OnMediaItemCreated(MFP_MEDIAITEM_CREATED_EVENT *pEvent)
			{
				HRESULT hr = S_OK;

				// if exists.
				if (_pPreview)
				{
					// Check if there is video.
					BOOL bHasVideo = FALSE, bIsSelected = FALSE;

					// Has video.
					hr = pEvent->pMediaItem->HasVideo(&bHasVideo, &bIsSelected);

					// If ok
					if (SUCCEEDED(hr))
					{
						// Has video.
						_bHasVideo = bHasVideo && bIsSelected;

						// Set this media item on the player.
						hr = _pPreview->SetMediaItem(pEvent->pMediaItem);
					}
				}
			}

			/// <summary>
			/// On media item set.
			/// </summary>
			/// <param name="pEvent">The event header.</param>
			void MediaPreview::OnMediaItemSet(MFP_MEDIAITEM_SET_EVENT *pEvent)
			{
				HRESULT hr = S_OK;

				SIZE szVideo = { 0 };
				RECT rc = { 0 };

				// Adjust the preview window to match the native size
				// of the captured video.
				hr = _pPreview->GetNativeVideoSize(&szVideo, NULL);

				// If ok
				if (SUCCEEDED(hr))
				{
					// Set the rect sise.
					SetRect(&rc, 0, 0, szVideo.cx, szVideo.cy);

					// Adjust the window.
					AdjustWindowRect(&rc, GetWindowLong(_hwndVideo, GWL_STYLE), TRUE);

					// Set the window position.
					SetWindowPos(_hwndVideo, 0, 0, 0, rc.right - rc.left, rc.bottom - rc.top,
						SWP_NOZORDER | SWP_NOMOVE | SWP_NOOWNERZORDER);

					// Play the preview video.
					hr = _pPreview->Play();
				}
			}
		}
	}
}