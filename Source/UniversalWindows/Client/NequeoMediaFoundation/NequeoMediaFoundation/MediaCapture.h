/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaCapture.h
*  Purpose :       MediaCapture class.
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

#ifndef _MEDIACAPTURE_H
#define _MEDIACAPTURE_H

#include "MediaGlobal.h"
#include "PlayerState.h"
#include "CriticalSectionHandler.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation capture.
			/// </summary>
			class MediaCapture : public IMFSourceReaderCallback
			{
			public:
				/// <summary>
				/// Static class method to create the MediaCapture object.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaCapture object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static HRESULT CreateInstance(HWND hwnd, HWND hEvent, MediaCapture **ppCapture);

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
				virtual STDMETHODIMP QueryInterface(REFIID iid, void** ppv);
				
				/// <summary>
				/// On event read sample override.
				/// </summary>
				/// <param name="hrStatus">The read status.</param>
				/// <param name="dwStreamIndex">The stream index number.</param>
				/// <param name="dwStreamFlags">The stream flag.</param>
				/// <param name="llTimestamp">The current time stamp.</param>
				/// <param name="pSample">The captured sample data.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnReadSample(
					HRESULT hrStatus,
					DWORD dwStreamIndex,
					DWORD dwStreamFlags,
					LONGLONG llTimestamp,
					IMFSample *pSample);

				/// <summary>
				/// On event MF override.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnEvent(DWORD, IMFMediaEvent*)
				{
					return S_OK;
				}

				/// <summary>
				/// On event flush override.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnFlush(DWORD)
				{
					return S_OK;
				}

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="hr">The result reference.</param>
				MediaCapture(HWND hwnd, HWND hEvent, HRESULT &hr);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~MediaCapture();

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.
				bool					_started;

				CRITICAL_SECTION        _critsec;
				CaptureState			_captureState;		// Current state of the media session.
			};
		}
	}
}
#endif