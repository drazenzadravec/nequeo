/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPreview.h
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

#pragma once

#ifndef _MEDIAPREVIEW_H
#define _MEDIAPREVIEW_H

#include "MediaGlobal.h"
#include "ContentEnabler.h"
#include "CriticalSectionHandler.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation preview.
			/// </summary>
			class MediaPreview : public IMFPMediaPlayerCallback
			{
			public:
				/// <summary>
				/// Static class method to create the MediaPreview object.
				/// </summary>
				/// <param name="hVideo">The handle to the video window.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaPreview object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static HRESULT CreateInstance(HWND hVideo, HWND hEvent, MediaPreview **ppPreview);

				/// <summary>
				/// Set the video capture device.
				/// </summary>
				/// <param name="pActivate">The media foundation device.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetDevice(IMFActivate *pActivate);

				/// <summary>
				/// Add a new player ref item.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) AddRef();

				/// <summary>
				/// Release this player resources.
				/// </summary>
				/// <returns>The result.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(ULONG) Release();

				/// <summary>
				/// Get the player reference for the reference id.
				/// </summary>
				/// <param name="iid">The player reference id.</param>
				/// <param name="ppv">The current player reference.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP QueryInterface(REFIID iid, void** ppv);

				/// <summary>
				/// Close the playback device.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT Close();

				/// <summary>
				/// Repaint the video window. The application should call this method when it receives a WM_SIZE message.
				/// </summary>
				/// <param name="width">The width of the player.</param>
				/// <param name="height">The height of the player.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT	ResizeVideo(WORD width, WORD height);

				/// <summary>
				/// Update the video.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT UpdateVideo();

				/// <summary>
				/// Get an indicator specifying if a video display control has been set.
				/// </summary>
				/// <returns>True if a video display control has ben set; else false.</returns>
				BOOL HasVideo() const { return _bHasVideo; }

				/// <summary>
				/// Get device lost in the collection.
				/// </summary>
				/// <param name="pHdr">The device broadcast handler.</param>
				/// <param name="pbDeviceLost">Is device lost.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT CheckDeviceLost(DEV_BROADCAST_HDR *pHdr, BOOL *pbDeviceLost);

				/// <summary>
				/// On media player event.
				/// </summary>
				/// <param name="pEventHeader">The event header.</param>
				void STDMETHODCALLTYPE OnMediaPlayerEvent(MFP_EVENT_HEADER * pEventHeader);

				/// <summary>
				/// Set the notify state event handler.
				/// </summary>
				/// <param name="stateEvent">The user defined event handler.</param>
				void SetNotifyStateEventHandler(HANDLE stateEvent)
				{
					// Assign the internal event.
					_hNotifyStateEvent = stateEvent;
				}

				/// <summary>
				/// Set the notify error event handler.
				/// </summary>
				/// <param name="errorEvent">The user defined event handler.</param>
				void SetNotifyErrorEventHandler(HANDLE errorEvent)
				{
					// Assign the internal event.
					_hNotifyErrorEvent = errorEvent;
				}

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hVideo">The handle to the video window.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="hr">The result reference.</param>
				MediaPreview(HWND hVideo, HWND hEvent, HRESULT &hr);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~MediaPreview();

				/// <summary>
				/// On media item created.
				/// </summary>
				/// <param name="pEvent">The event header.</param>
				void OnMediaItemCreated(MFP_MEDIAITEM_CREATED_EVENT *pEvent);

				/// <summary>
				/// On media item set.
				/// </summary>
				/// <param name="pEvent">The event header.</param>
				void OnMediaItemSet(MFP_MEDIAITEM_SET_EVENT *pEvent);

				/// <summary>
				/// Notifies the application when the state changes.
				/// </summary>
				void NotifyState()
				{
					// If posting the state messsage.
					if (_hNotifyStateEvent != NULL)
					{
						// Trigger the notify state event handler.
						SetEvent(_hNotifyStateEvent);
					}

					PostMessage(_hwndEvent, WM_APP_NOTIFY, (WPARAM)_previewState, (LPARAM)0);
				}

				/// <summary>
				/// Notifies the application when an error occurs.
				/// </summary>
				/// <param name="hr">The handler result.</param>
				void NotifyError(HRESULT hr)
				{
					_previewState = PreviewReady;

					// If posting the error messsage.
					if (_hNotifyErrorEvent != NULL)
					{
						// Trigger the notify error event handler.
						SetEvent(_hNotifyErrorEvent);
					}

					PostMessage(_hwndEvent, WM_APP_ERROR, (WPARAM)hr, 0);
				}

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.
				bool					_closed;

				IMFPMediaPlayer         *_pPreview;
				IMFMediaSource          *_pSource;
				HWND				    _hwndVideo;			// Video window.
				HWND				    _hwndEvent;			// App window to receive events.
				BOOL                    _bHasVideo;

				WCHAR                   *_pwszSymbolicLink;
				UINT32                  _cchSymbolicLink;

				HANDLE					_hNotifyStateEvent;
				HANDLE					_hNotifyErrorEvent;
				PreviewState			_previewState;		// Current state of the media session.
			};
		}
	}
}
#endif