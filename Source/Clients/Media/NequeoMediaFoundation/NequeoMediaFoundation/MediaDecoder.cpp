/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaDecoder.cpp
*  Purpose :       MediaDecoder class.
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

#include "MediaDecoder.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="hwnd">The handle to the application owner.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="hr">The result reference.</param>
			MediaDecoder::MediaDecoder(HWND hwnd, HWND hEvent) :
				_hwndApp(hwnd),
				_hwndEvent(hEvent),
				_hCloseEvent(NULL),
				_decoder(NULL),
				_transformUnk(NULL),
				_nRefCount(1),
				_isOpen(false),
				_created(false),
				_decoderType(DecoderType::H264),
				_disposed(false)
			{
				// Initialize critical section.
				InitializeCriticalSection(&_critsec);
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaDecoder::~MediaDecoder()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					// Close a media decoder.
					Close();

					// Delete critical section.
					DeleteCriticalSection(&_critsec);
				}
			}

			/// <summary>
			/// Static class method to create the MediaDecoder object.
			/// </summary>
			/// <param name="hwnd">The handle to the application owner.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="ppMediaDecoder">Receives an AddRef's pointer to the MediaDecoder object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::CreateInstance(HWND hwnd, HWND hEvent, MediaDecoder **ppMediaDecoder)
			{
				// Make sure the a video and event handler exists.
				assert(hwnd != NULL);
				assert(hEvent != NULL);

				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// MediaDecoder constructor sets the ref count to zero.
				// Create method calls AddRef.
				// Create a new medis source instance.
				MediaDecoder *pDecoder = new MediaDecoder(hwnd, hEvent);

				// If the preview was not created.
				if (pDecoder == NULL)
				{
					// Out of memory result.
					hr = E_OUTOFMEMORY;
				}

				// If successful initialisation.
				if (SUCCEEDED(hr))
				{
					// Increment the reference count of the current player.
					*ppMediaDecoder = pDecoder;
					(*ppMediaDecoder)->AddRef();
				}
				else
				{
					// Delete the instance of the source
					// if not successful.
					delete pDecoder;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaDecoder::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaDecoder::Release()
			{
				// Decrement the player ref count.
				ULONG uCount = InterlockedDecrement(&_nRefCount);

				// If released.
				if (uCount == 0)
				{
					// Delete this media resources.
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
			HRESULT MediaDecoder::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}

				// Attach MediaCapture to the interface.
				static const QITAB qit[] =
				{
					QITABENT(MediaDecoder, IMFTransform),
					{ 0 },
				};
				return QISearch(this, qit, iid, ppv);
			}

			/// <summary>
			/// Retrieves the minimum and maximum number of input and output streams.
			/// </summary>
			/// <param name="pdwInputMinimum">Receives the minimum number of input streams.</param>
			/// <param name="pdwInputMaximum">Receives the maximum number of input streams. If there is no maximum, receives the value MFT_STREAMS_UNLIMITED. </param>
			/// <param name="pdwOutputMinimum">Receives the minimum number of output streams. </param>
			/// <param name="pdwOutputMaximum">Receives the maximum number of output streams. If there is no maximum, receives the value MFT_STREAMS_UNLIMITED. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetStreamLimits(
				DWORD   *pdwInputMinimum,
				DWORD   *pdwInputMaximum,
				DWORD   *pdwOutputMinimum,
				DWORD   *pdwOutputMaximum)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetStreamLimits(
						pdwInputMinimum,
						pdwInputMaximum,
						pdwOutputMinimum,
						pdwOutputMaximum);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the current number of input and output streams on this MFT.
			/// </summary>
			/// <param name="pcInputStreams">Receives the number of input streams.</param>
			/// <param name="pcOutputStreams">Receives the number of output streams. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetStreamCount(
				DWORD   *pcInputStreams,
				DWORD   *pcOutputStreams)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetStreamCount(
						pcInputStreams,
						pcOutputStreams);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the stream identifiers for the input and output streams on this MFT.
			/// </summary>
			/// <param name="dwInputIDArraySize">Number of elements in the pdwInputIDs array</param>
			/// <param name="pdwInputIDs">Pointer to an array allocated by the caller. The method fills the array with the input stream identifiers. The array size must be at least equal to the number of input streams. To get the number of input streams, call IMFTransform::GetStreamCount. 
			/// If the caller passes an array that is larger than the number of input streams, the MFT must not write values into the extra array entries.</param>
			/// <param name="dwOutputIDArraySize">Number of elements in the pdwOutputIDs array.</param>
			/// <param name="pdwOutputIDs">Pointer to an array allocated by the caller. The method fills the array with the output stream identifiers. The array size must be at least equal to the number of output streams. To get the number of output streams, call GetStreamCount. 
			/// If the caller passes an array that is larger than the number of output streams, the MFT must not write values into the extra array entries.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetStreamIDs(
				DWORD   dwInputIDArraySize,
				DWORD   *pdwInputIDs,
				DWORD   dwOutputIDArraySize,
				DWORD   *pdwOutputIDs)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetStreamIDs(
						dwInputIDArraySize,
						pdwInputIDs,
						dwOutputIDArraySize,
						pdwOutputIDs);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the buffer requirements and other information for an input stream.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
			/// <param name="pStreamInfo">Pointer to an MFT_INPUT_STREAM_INFO structure. The method fills the structure with information about the input stream. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetInputStreamInfo(
				DWORD                     dwInputStreamID,
				MFT_INPUT_STREAM_INFO *   pStreamInfo)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetInputStreamInfo(
						dwInputStreamID,
						pStreamInfo);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the buffer requirements and other information for an output stream on this MFT.
			/// </summary>
			/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="pStreamInfo">Pointer to an MFT_OUTPUT_STREAM_INFO structure. The method fills the structure with information about the output stream. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetOutputStreamInfo(
				DWORD                     dwOutputStreamID,
				MFT_OUTPUT_STREAM_INFO *  pStreamInfo)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetOutputStreamInfo(
						dwOutputStreamID,
						pStreamInfo);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the attribute store for this MFT.
			/// </summary>
			/// <param name="pAttributes">Receives a pointer to the IMFAttributes interface. The caller must release the interface. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetAttributes(IMFAttributes** pAttributes)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetAttributes(
						pAttributes);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the attribute store for an input stream on this MFT.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
			/// <param name="ppAttributes">Receives a pointer to the IMFAttributes interface. The caller must release the interface. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetInputStreamAttributes(
				DWORD           dwInputStreamID,
				IMFAttributes   **ppAttributes)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetInputStreamAttributes(
						dwInputStreamID,
						ppAttributes);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the attribute store for an output stream on this MFT.
			/// </summary>
			/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
			/// <param name="ppAttributes">Receives a pointer to the IMFAttributes interface. The caller must release the interface. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetOutputStreamAttributes(
				DWORD           dwOutputStreamID,
				IMFAttributes   **ppAttributes)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetOutputStreamAttributes(
						dwOutputStreamID,
						ppAttributes);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Removes an input stream from this MFT.
			/// </summary>
			/// <param name="dwStreamID">Identifier of the input stream to remove.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::DeleteInputStream(DWORD dwStreamID)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->DeleteInputStream(
						dwStreamID);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Adds one or more new input streams to this MFT.
			/// </summary>
			/// <param name="cStreams">Number of streams to add.</param>
			/// <param name="adwStreamIDs">Array of stream identifiers. The new stream identifiers must not match any existing input streams.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::AddInputStreams(
				DWORD   cStreams,
				DWORD   *adwStreamIDs)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->AddInputStreams(
						cStreams,
						adwStreamIDs);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves a possible media type for an input stream on this MFT.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="dwTypeIndex">Index of the media type to retrieve. Media types are indexed from zero and returned in approximate order of preference.</param>
			/// <param name="ppType">Receives a pointer to the IMFMediaType interface. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetInputAvailableType(
				DWORD           dwInputStreamID,
				DWORD           dwTypeIndex,
				IMFMediaType    **ppType)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetInputAvailableType(
						dwInputStreamID,
						dwTypeIndex,
						ppType);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves an available media type for an output stream on this MFT.
			/// </summary>
			/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="dwTypeIndex">Index of the media type to retrieve. Media types are indexed from zero and returned in approximate order of preference. </param>
			/// <param name="ppType">Receives a pointer to the IMFMediaType interface. The caller must release the interface. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetOutputAvailableType(
				DWORD           dwOutputStreamID,
				DWORD           dwTypeIndex,
				IMFMediaType    **ppType)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetOutputAvailableType(
						dwOutputStreamID,
						dwTypeIndex,
						ppType);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sets, tests, or clears the media type for an input stream on this MFT.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="pType">Pointer to the IMFMediaType interface, or NULL. </param>
			/// <param name="dwFlags">Zero or more flags from the _MFT_SET_TYPE_FLAGS enumeration. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::SetInputType(
				DWORD           dwInputStreamID,
				IMFMediaType    *pType,
				DWORD           dwFlags)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->SetInputType(
						dwInputStreamID,
						pType,
						dwFlags);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sets, tests, or clears the media type for an output stream on this MFT.
			/// </summary>
			/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="pType">Pointer to the IMFMediaType interface, or NULL. </param>
			/// <param name="dwFlags">Zero or more flags from the _MFT_SET_TYPE_FLAGS enumeration. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::SetOutputType(
				DWORD           dwOutputStreamID,
				IMFMediaType    *pType,
				DWORD           dwFlags)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->SetOutputType(
						dwOutputStreamID,
						pType,
						dwFlags);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the current media type for an input stream on this MFT.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="ppType">Receives a pointer to the IMFMediaType interface. The caller must release the interface.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetInputCurrentType(
				DWORD           dwInputStreamID,
				IMFMediaType    **ppType)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetInputCurrentType(
						dwInputStreamID,
						ppType);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the current media type for an output stream on this MFT.
			/// </summary>
			/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
			/// <param name="ppType">Receives a pointer to the IMFMediaType interface. The caller must release the interface.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetOutputCurrentType(
				DWORD           dwOutputStreamID,
				IMFMediaType    **ppType)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetOutputCurrentType(
						dwOutputStreamID,
						ppType);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Queries whether an input stream on this MFT can accept more data.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
			/// <param name="pdwFlags">Receives a member of the _MFT_INPUT_STATUS_FLAGS enumeration, or zero. If the value is MFT_INPUT_STATUS_ACCEPT_DATA, the stream specified in dwInputStreamID can accept more input data. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetInputStatus(
				DWORD           dwInputStreamID,
				DWORD           *pdwFlags)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetInputStatus(
						dwInputStreamID,
						pdwFlags);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Queries whether the transform is ready to produce output data.
			/// </summary>
			/// <param name="pdwFlags">Receives a member of the _MFT_OUTPUT_STATUS_FLAGS enumeration, or zero. If the value is MFT_OUTPUT_STATUS_SAMPLE_READY, the MFT can produce an output sample. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::GetOutputStatus(DWORD *pdwFlags)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->GetOutputStatus(
						pdwFlags);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sets the range of timestamps the client needs for output.
			/// </summary>
			/// <param name="hnsLowerBound">Specifies the earliest time stamp. The Media Foundation transform (MFT) will accept input until it can produce an output sample that begins at this time; or until it can produce a sample that ends at this time or later. If there is no lower bound, use the value MFT_OUTPUT_BOUND_LOWER_UNBOUNDED. </param>
			/// <param name="hnsUpperBound">Specifies the latest time stamp. The MFT will not produce an output sample with time stamps later than this time. If there is no upper bound, use the value MFT_OUTPUT_BOUND_UPPER_UNBOUNDED. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::SetOutputBounds(
				LONGLONG        hnsLowerBound,
				LONGLONG        hnsUpperBound)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->SetOutputBounds(
						hnsLowerBound,
						hnsUpperBound);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sends an event to an input stream on this MFT.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
			/// <param name="pEvent">Pointer to the IMFMediaEvent interface of an event object. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::ProcessEvent(
				DWORD              dwInputStreamID,
				IMFMediaEvent      *pEvent)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->ProcessEvent(
						dwInputStreamID,
						pEvent);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sends a message to the MFT.
			/// </summary>
			/// <param name="eMessage">The message to send, specified as a member of the MFT_MESSAGE_TYPE enumeration.</param>
			/// <param name="ulParam">Message parameter. The meaning of this parameter depends on the message type. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::ProcessMessage(
				MFT_MESSAGE_TYPE    eMessage,
				ULONG_PTR           ulParam)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->ProcessMessage(
						eMessage,
						ulParam);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Delivers data to an input stream on this MFT.
			/// </summary>
			/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
			/// <param name="pSample">Pointer to the IMFSample interface of the input sample. The sample must contain at least one media buffer that contains valid input data. </param>
			/// <param name="dwFlags">Reserved. Must be zero. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::ProcessInput(
				DWORD               dwInputStreamID,
				IMFSample           *pSample,
				DWORD               dwFlags)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->ProcessInput(
						dwInputStreamID,
						pSample,
						dwFlags);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Generates output from the current input data.
			/// </summary>
			/// <param name="dwFlags">Bitwise OR of zero or more flags from the _MFT_PROCESS_OUTPUT_FLAGS enumeration. </param>
			/// <param name="cOutputBufferCount">Number of elements in the pOutputSamples array. The value must be at least 1.</param>
			/// <param name="pOutputSamples">Pointer to an array of MFT_OUTPUT_DATA_BUFFER structures, allocated by the caller. The MFT uses this array to return output data to the caller. </param>
			/// <param name="pdwStatus">Receives a bitwise OR of zero or more flags from the _MFT_PROCESS_OUTPUT_STATUS enumeration. </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::ProcessOutput(
				DWORD                   dwFlags,
				DWORD                   cOutputBufferCount,
				MFT_OUTPUT_DATA_BUFFER  *pOutputSamples,
				DWORD                   *pdwStatus)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					hr = _decoder->ProcessOutput(
						dwFlags,
						cOutputBufferCount,
						pOutputSamples,
						pdwStatus);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Close a media decoder.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::Close()
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// Is open.
				if (_isOpen)
				{
					// Shutdown the Media Foundation platform
					MFShutdown();
					_isOpen = false;

					// Close the close event handler.
					CloseHandle(_hCloseEvent);
				}

				// Safe release.
				SafeRelease(&_decoder);
				SafeRelease(&_transformUnk);
				
				// Creation closed.
				_created = false;

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initialise the decoder.
			/// </summary>
			/// <param name="decoder">The decoder to start.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::InitialiseDecoder(DecoderType decoder)
			{
				HRESULT hr = S_OK;

				// Has the decoder been init.
				if (!_isOpen)
				{
					_decoderType = decoder;

					// Init the COM.
					CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

					// Create a new close player event handler.
					_hCloseEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

					// If event was not created.
					if (_hCloseEvent == NULL)
					{
						// Get the result value.
						hr = __HRESULT_FROM_WIN32(GetLastError());
					}

					// If successful creation of the close event.
					if (SUCCEEDED(hr))
					{
						// Start up Media Foundation platform.
						hr = MFStartup(MF_VERSION);
						_isOpen = true;
					}

					if (SUCCEEDED(hr))
					{
						// Select the decoder.
						switch (decoder)
						{
						case Nequeo::Media::Foundation::DecoderType::H264:
							// Create the H264 decoder.
							hr = CreateDecoder(CLSID_CMSH264DecoderMFT);
							break;

						case Nequeo::Media::Foundation::DecoderType::AAC:
							// Create the AAC decoder.
							hr = CreateDecoder(CLSID_CMSAACDecMFT);
							break;

						case Nequeo::Media::Foundation::DecoderType::MP3:
							// Create the MP3 decoder.
							hr = CreateDecoder(CLSID_CMP3DecMediaObject);
							break;

						case Nequeo::Media::Foundation::DecoderType::MPEG4:
							// Create the MPEG4 decoder.
							hr = CreateDecoder(CLSID_CMpeg4sDecMFT);
							break;

						default:
							hr = ((HRESULT)-1L);
							break;
						}
					}

					if (SUCCEEDED(hr))
					{
						// Query for the IMFTransform interface 
						hr = _transformUnk->QueryInterface(IID_PPV_ARGS(&_decoder));

						// Decoder has been created.
						_created = true;
					}
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initialise the decoder.
			/// </summary>
			/// <param name="decoder">The decoder to start.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::CreateDecoder(const CLSID decoder)
			{
				HRESULT hr = S_OK;

				// Create the decoder.
				hr = CoCreateInstance(decoder, NULL, CLSCTX_INPROC_SERVER, IID_IUnknown, (void**)&_transformUnk);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Setup initial decoder paramaters.
			/// </summary>
			/// <param name="input">The input decoder details.</param>
			/// <param name="output">The output decoder details.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::SetupInitialDecoder(IMFMediaType **input, IMFMediaType **output)
			{
				HRESULT hr = S_OK;

				// If a decoder has been created.
				if (_created)
				{
					IMFMediaType *inputMedia = NULL;
					IMFMediaType *outputMedia = NULL;

					// Create the input and output media types.
					MFCreateMediaType(&inputMedia);
					MFCreateMediaType(&outputMedia);

					// If the was not created.
					if (inputMedia == NULL)
					{
						// Out of memory result.
						hr = E_OUTOFMEMORY;
					}
					else
					{
						// Setup input media type.
						hr = SetupInitialInput(inputMedia);
					}

					// If successful initialisation.
					if (SUCCEEDED(hr))
					{
						// Increment the reference count of the current.
						*input = inputMedia;
						(*input)->AddRef();
					}
					else
					{
						// Delete the instance of the source
						// if not successful.
						delete inputMedia;
					}

					// If the was not created.
					if (outputMedia == NULL)
					{
						// Out of memory result.
						hr = E_OUTOFMEMORY;
					}
					else
					{
						// Setup output media type.
						hr = SetupInitialOutput(outputMedia);
					}

					// If successful initialisation.
					if (SUCCEEDED(hr))
					{
						// Increment the reference count of the current.
						*output = outputMedia;
						(*output)->AddRef();
					}
					else
					{
						// Delete the instance of the source
						// if not successful.
						delete outputMedia;
					}
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Setup initial decoder paramaters.
			/// </summary>
			/// <param name="input">The input decoder details.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::SetupInitialInput(IMFMediaType *input)
			{
				HRESULT hr = S_OK;

				// Default user data.
				UINT8 userData[] = { 0x00, 0x00, 0x2a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0xb0 };

				// Select the decoder.
				switch (_decoderType)
				{
				case Nequeo::Media::Foundation::DecoderType::H264:
					// Setup input media type.
					input->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
					input->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_H264);
					MFSetAttributeSize(input, MF_MT_FRAME_SIZE, 640, 480);
					MFSetAttributeRatio(input, MF_MT_FRAME_RATE, 30, 1);
					MFSetAttributeRatio(input, MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
					input->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_MixedInterlaceOrProgressive);
					input->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, TRUE);
					break;

				case Nequeo::Media::Foundation::DecoderType::AAC:
					// Setup input media type.
					input->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
					input->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_AAC);
					input->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, 44100);
					input->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, 2);
					input->SetUINT32(MF_MT_AAC_PAYLOAD_TYPE, 0);
					input->SetBlob(MF_MT_USER_DATA, userData, ARRAYSIZE(userData));
					break;

				case Nequeo::Media::Foundation::DecoderType::MP3:
					// Setup input media type.
					input->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
					input->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_MP3);
					input->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, 44100);
					input->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, 2);
					input->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, 16);
					break;

				case Nequeo::Media::Foundation::DecoderType::MPEG4:
					// Setup input media type.
					input->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
					input->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_MP4V);
					MFSetAttributeSize(input, MF_MT_FRAME_SIZE, 640, 480);
					MFSetAttributeRatio(input, MF_MT_FRAME_RATE, 30, 1);
					MFSetAttributeRatio(input, MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
					break;

				default:
					hr = ((HRESULT)-1L);
					break;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Setup initial decoder paramaters.
			/// </summary>
			/// <param name="output">The output decoder details.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaDecoder::SetupInitialOutput(IMFMediaType *output)
			{
				HRESULT hr = S_OK;

				// Select the decoder.
				switch (_decoderType)
				{
				case Nequeo::Media::Foundation::DecoderType::H264:
					// Setup output media type.
					output->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
					output->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_YUY2);
					MFSetAttributeSize(output, MF_MT_FRAME_SIZE, 640, 480);
					MFSetAttributeRatio(output, MF_MT_FRAME_RATE, 30, 1);
					MFSetAttributeRatio(output, MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
					break;

				case Nequeo::Media::Foundation::DecoderType::AAC:
					// Setup output media type.
					output->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
					output->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM);
					output->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, 44100);
					output->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, 2);
					output->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, 16);
					break;

				case Nequeo::Media::Foundation::DecoderType::MP3:
					// Setup output media type.
					output->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
					output->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM);
					output->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, 44100);
					output->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, 2);
					output->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, 16);
					break;

				case Nequeo::Media::Foundation::DecoderType::MPEG4:
					// Setup output media type.
					output->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
					output->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_YV12);
					MFSetAttributeSize(output, MF_MT_FRAME_SIZE, 640, 480);
					MFSetAttributeRatio(output, MF_MT_FRAME_RATE, 30, 1);
					MFSetAttributeRatio(output, MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
					break;

				default:
					hr = ((HRESULT)-1L);
					break;
				}

				// Return the result.
				return hr;
			}
		}
	}
}