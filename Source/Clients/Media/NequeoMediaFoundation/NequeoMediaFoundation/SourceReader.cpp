/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SourceReader.cpp
*  Purpose :       SourceReader class.
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

#include "SourceReader.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="hr">The result reference.</param>
			SourceReader::SourceReader(HRESULT &hr) :
				_nRefCount(1),
				_bFirstSample(TRUE),
				_llBaseTime(0),
				_disposed(false)
			{
				// Initialize critical section.
				InitializeCriticalSection(&_critsec);
			}

			/// <summary>
			/// This destructor. Call release to cleanup resources.
			/// </summary>
			SourceReader::~SourceReader()
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
			/// <param name="pSourceReader">Receives an AddRef's pointer to the SourceReader object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT SourceReader::CreateInstance(SourceReader **ppSourceReader)
			{
				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// MediaCapture constructor sets the ref count to zero.
				// Create method calls AddRef.
				// Create a new medis player instance.
				SourceReader *pCapture = new SourceReader(hr);

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
					*ppSourceReader = pCapture;
					(*ppSourceReader)->AddRef();
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
			ULONG SourceReader::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG SourceReader::Release()
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
			HRESULT SourceReader::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}

				// Attach MediaCapture to the interface.
				static const QITAB qit[] =
				{
					QITABENT(SourceReader, IMFSourceReaderCallback),
					{ 0 },
				};
				return QISearch(this, qit, iid, ppv);
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
			HRESULT SourceReader::OnReadSample(
				HRESULT hrStatus,
				DWORD dwStreamIndex,
				DWORD dwStreamFlags,
				LONGLONG llTimestamp,
				IMFSample *pSample)
			{
				HRESULT hr = S_OK;
				LONGLONG rebaseTimestamp = llTimestamp;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// If failed.
				if (FAILED(hrStatus))
				{
					hr = hrStatus;
					goto done;
				}

				// If a sample exists.
				if (pSample)
				{
					// If first sample.
					if (_bFirstSample)
					{
						// Assign the time stamp.
						_llBaseTime = llTimestamp;
						_bFirstSample = FALSE;
					}

					// Rebase the time stamp
					rebaseTimestamp -= _llBaseTime;
				}

				// Call the handler.
				hr = _readSampleCompleteHandler(hrStatus, dwStreamIndex, dwStreamFlags, llTimestamp, pSample, rebaseTimestamp);

			done:
				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Set the read sample complete handler.
			/// </summary>
			/// <param name="handler">The function handler.</param>
			/// <param name="state">An object state.</param>
			void SourceReader::SetReadSampleCompleteHandler(ReadSampleCompleteHandler handler)
			{
				_readSampleCompleteHandler = handler;
			}
		}
	}
}