/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaCapture.cpp
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

#include "pch.h"

#include "MediaCapture.h"
#include "MediaByteStream.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			HRESULT CopyAttribute(IMFAttributes*, IMFAttributes*, const GUID&);

			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="hwnd">The handle to the application owner.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="hr">The result reference.</param>
			MediaCapture::MediaCapture(HWND hwnd, HWND hEvent, HRESULT &hr) :
				_nRefCount(1),
				_captureState(CaptureNotReady),
				_started(false),
				_disposed(false)
			{
				// Initialize critical section.
				InitializeCriticalSection(&_critsec);
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaCapture::~MediaCapture()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
					
					// Delete critical section.
					DeleteCriticalSection(&_critsec);
				}
			}

			/// <summary>
			/// Static class method to create the MediaCapture object.
			/// </summary>
			/// <param name="hwnd">The handle to the application owner.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaCapture object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::CreateInstance(HWND hwnd, HWND hEvent, MediaCapture **ppCapture)
			{
				// Make sure the a video and event handler exists.
				assert(hwnd != NULL);
				assert(hEvent != NULL);

				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// MediaCapture constructor sets the ref count to zero.
				// Create method calls AddRef.
				// Create a new medis player instance.
				MediaCapture *pCapture = new MediaCapture(hwnd, hEvent, hr);
				
				// If the preview was not created.
				if (pCapture == NULL)
				{
					// Out of memory result.
					hr = E_OUTOFMEMORY;
				}

				// If successful initialisation.
				if (SUCCEEDED(hr))
				{
					// Increment the reference count of the current player.
					*ppCapture = pCapture;
					(*ppCapture)->AddRef();
				}
				else
				{
					// Delete the instance of the preview
					// if not successful.
					delete pCapture;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaCapture::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaCapture::Release()
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
			HRESULT MediaCapture::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}
				return S_OK;
			}

			/// <summary>
			/// On event read sample override.
			/// </summary>
			/// <param name="hrStatus">The read status.</param>
			/// <param name="dwStreamIndex">The stream index number.</param>
			/// <param name="dwStreamFlags">The stream flag.</param>
			/// <param name="llTimestamp">The current time stamp.</param>
			/// <param name="pSample">The captured sample data.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::OnReadSample(
				HRESULT hrStatus,
				DWORD dwStreamIndex,
				DWORD dwStreamFlags,
				LONGLONG llTimestamp,
				IMFSample *pSample)
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Copy the attributes.
			/// </summary>
			/// <param name="pSrc">The attribute source.</param>
			/// <param name="pDest">The attribute destination.</param>
			/// <param name="key">The key GUID.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT CopyAttribute(IMFAttributes *pSrc, IMFAttributes *pDest, const GUID& key)
			{
				PROPVARIANT var;
				PropVariantInit(&var);

				HRESULT hr = S_OK;

				// Get the source.
				hr = pSrc->GetItem(key, &var);
				if (SUCCEEDED(hr))
				{
					// Set into destination.
					hr = pDest->SetItem(key, var);
				}

				PropVariantClear(&var);

				// Return the result.
				return hr;
			}
		}
	}
}