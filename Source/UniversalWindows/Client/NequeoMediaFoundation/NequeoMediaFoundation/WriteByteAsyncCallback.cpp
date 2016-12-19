/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WriteByteAsyncCallback.cpp
*  Purpose :       WriteByteAsyncCallback class.
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

#include "WriteByteAsyncCallback.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			/// <param name="byteStream">The byte stream to invoke write implementation on.</param>
			WriteByteAsyncCallback::WriteByteAsyncCallback(IMFByteStream* byteStream) :
				_nRefCount(1),
				_byteStream(byteStream),
				_disposed(false)
			{
				// Initialize critical section.
				InitializeCriticalSection(&_critsec);
			}

			/// <summary>
			/// This destructor. Call release to cleanup resources.
			/// </summary>
			WriteByteAsyncCallback::~WriteByteAsyncCallback()
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
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG WriteByteAsyncCallback::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG WriteByteAsyncCallback::Release()
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
			HRESULT WriteByteAsyncCallback::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}
				return S_OK;
			}

			/// <summary>
			/// Called when an asynchronous operation is completed.
			/// </summary>
			/// <param name="pAsyncResult">Pointer to the IMFAsyncResult interface. 
			/// Pass this pointer to the asynchronous End... method to complete the asynchronous call.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT WriteByteAsyncCallback::Invoke(IMFAsyncResult* pAsyncResult)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				ULONG pcbWritten;
				IUnknown *pState = NULL;
				IUnknown *pUnk = NULL;
				IMFAsyncResult *pCallerResult = NULL;
				WriteByteContainer *pWriter = NULL;

				// Get the asynchronous result object for the application callback. 
				hr = pAsyncResult->GetState(&pState);
				if (FAILED(hr))
				{
					goto done;
				}

				// Get the caller result interface.
				hr = pState->QueryInterface(IID_PPV_ARGS(&pCallerResult));
				if (FAILED(hr))
				{
					goto done;
				}

				// Get the object that holds the state information for the asynchronous method.
				hr = pCallerResult->GetObject(&pUnk);
				if (FAILED(hr))
				{
					goto done;
				}

				// Get the write byte container.
				pWriter = static_cast<WriteByteContainer*>(pUnk);

				// Write the bytes.
				_byteStream->Write(pWriter->_pb, pWriter->_cb, &pcbWritten);

				// Assign the number of bytes writen.
				pWriter->_pcbWritten = pcbWritten;

			done:
				// Set the result.
				// Signal the application.
				if (pCallerResult)
				{
					pCallerResult->SetStatus(hr);
					MFInvokeCallback(pCallerResult);
				}

				// Clean-up.
				SafeRelease(&pState);
				SafeRelease(&pUnk);
				SafeRelease(&pCallerResult);

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}
		}
	}
}