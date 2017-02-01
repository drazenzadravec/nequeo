/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaEncoder.h
*  Purpose :       MediaEncoder class.
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

#ifndef _MEDIAENCODER_H
#define _MEDIAENCODER_H

#include "MediaGlobal.h"
#include "CodecType.h"
#include "CriticalSectionHandler.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation encoder.
			/// </summary>
			class MediaEncoder : public IMFTransform
			{
			public:
				/// <summary>
				/// Static class method to create the MediaEncoder object.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="ppMediaEncoder">Receives an AddRef's pointer to the MediaEncoder object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static HRESULT CreateInstance(HWND hwnd, HWND hEvent, MediaEncoder **ppMediaEncoder);

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
				/// Retrieves the minimum and maximum number of input and output streams.
				/// </summary>
				/// <param name="pdwInputMinimum">Receives the minimum number of input streams.</param>
				/// <param name="pdwInputMaximum">Receives the maximum number of input streams. If there is no maximum, receives the value MFT_STREAMS_UNLIMITED. </param>
				/// <param name="pdwOutputMinimum">Receives the minimum number of output streams. </param>
				/// <param name="pdwOutputMaximum">Receives the maximum number of output streams. If there is no maximum, receives the value MFT_STREAMS_UNLIMITED. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetStreamLimits(
					DWORD   *pdwInputMinimum,
					DWORD   *pdwInputMaximum,
					DWORD   *pdwOutputMinimum,
					DWORD   *pdwOutputMaximum);

				/// <summary>
				/// Retrieves the current number of input and output streams on this MFT.
				/// </summary>
				/// <param name="pcInputStreams">Receives the number of input streams.</param>
				/// <param name="pcOutputStreams">Receives the number of output streams. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetStreamCount(
					DWORD   *pcInputStreams,
					DWORD   *pcOutputStreams);

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
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetStreamIDs(
					DWORD   dwInputIDArraySize,
					DWORD   *pdwInputIDs,
					DWORD   dwOutputIDArraySize,
					DWORD   *pdwOutputIDs);

				/// <summary>
				/// Retrieves the buffer requirements and other information for an input stream.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
				/// <param name="pStreamInfo">Pointer to an MFT_INPUT_STREAM_INFO structure. The method fills the structure with information about the input stream. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetInputStreamInfo(
					DWORD                     dwInputStreamID,
					MFT_INPUT_STREAM_INFO *   pStreamInfo);

				/// <summary>
				/// Retrieves the buffer requirements and other information for an output stream on this MFT.
				/// </summary>
				/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="pStreamInfo">Pointer to an MFT_OUTPUT_STREAM_INFO structure. The method fills the structure with information about the output stream. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetOutputStreamInfo(
					DWORD                     dwOutputStreamID,
					MFT_OUTPUT_STREAM_INFO *  pStreamInfo);

				/// <summary>
				/// Retrieves the attribute store for this MFT.
				/// </summary>
				/// <param name="pAttributes">Receives a pointer to the IMFAttributes interface. The caller must release the interface. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetAttributes(IMFAttributes** pAttributes);

				/// <summary>
				/// Retrieves the attribute store for an input stream on this MFT.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
				/// <param name="ppAttributes">Receives a pointer to the IMFAttributes interface. The caller must release the interface. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetInputStreamAttributes(
					DWORD           dwInputStreamID,
					IMFAttributes   **ppAttributes);

				/// <summary>
				/// Retrieves the attribute store for an output stream on this MFT.
				/// </summary>
				/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
				/// <param name="ppAttributes">Receives a pointer to the IMFAttributes interface. The caller must release the interface. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetOutputStreamAttributes(
					DWORD           dwOutputStreamID,
					IMFAttributes   **ppAttributes);

				/// <summary>
				/// Removes an input stream from this MFT.
				/// </summary>
				/// <param name="dwStreamID">Identifier of the input stream to remove.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) DeleteInputStream(DWORD dwStreamID);

				/// <summary>
				/// Adds one or more new input streams to this MFT.
				/// </summary>
				/// <param name="cStreams">Number of streams to add.</param>
				/// <param name="adwStreamIDs">Array of stream identifiers. The new stream identifiers must not match any existing input streams.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) AddInputStreams(
					DWORD   cStreams,
					DWORD   *adwStreamIDs);

				/// <summary>
				/// Retrieves a possible media type for an input stream on this MFT.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="dwTypeIndex">Index of the media type to retrieve. Media types are indexed from zero and returned in approximate order of preference.</param>
				/// <param name="ppType">Receives a pointer to the IMFMediaType interface. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetInputAvailableType(
					DWORD           dwInputStreamID,
					DWORD           dwTypeIndex, // 0-based
					IMFMediaType    **ppType);

				/// <summary>
				/// Retrieves an available media type for an output stream on this MFT.
				/// </summary>
				/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="dwTypeIndex">Index of the media type to retrieve. Media types are indexed from zero and returned in approximate order of preference. </param>
				/// <param name="ppType">Receives a pointer to the IMFMediaType interface. The caller must release the interface. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetOutputAvailableType(
					DWORD           dwOutputStreamID,
					DWORD           dwTypeIndex, // 0-based
					IMFMediaType    **ppType);

				/// <summary>
				/// Sets, tests, or clears the media type for an input stream on this MFT.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="pType">Pointer to the IMFMediaType interface, or NULL. </param>
				/// <param name="dwFlags">Zero or more flags from the _MFT_SET_TYPE_FLAGS enumeration. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) SetInputType(
					DWORD           dwInputStreamID,
					IMFMediaType    *pType,
					DWORD           dwFlags);

				/// <summary>
				/// Sets, tests, or clears the media type for an output stream on this MFT.
				/// </summary>
				/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="pType">Pointer to the IMFMediaType interface, or NULL. </param>
				/// <param name="dwFlags">Zero or more flags from the _MFT_SET_TYPE_FLAGS enumeration. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) SetOutputType(
					DWORD           dwOutputStreamID,
					IMFMediaType    *pType,
					DWORD           dwFlags);

				/// <summary>
				/// Retrieves the current media type for an input stream on this MFT.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="ppType">Receives a pointer to the IMFMediaType interface. The caller must release the interface.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetInputCurrentType(
					DWORD           dwInputStreamID,
					IMFMediaType    **ppType);

				/// <summary>
				/// Retrieves the current media type for an output stream on this MFT.
				/// </summary>
				/// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
				/// <param name="ppType">Receives a pointer to the IMFMediaType interface. The caller must release the interface.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetOutputCurrentType(
					DWORD           dwOutputStreamID,
					IMFMediaType    **ppType);

				/// <summary>
				/// Queries whether an input stream on this MFT can accept more data.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
				/// <param name="pdwFlags">Receives a member of the _MFT_INPUT_STATUS_FLAGS enumeration, or zero. If the value is MFT_INPUT_STATUS_ACCEPT_DATA, the stream specified in dwInputStreamID can accept more input data. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetInputStatus(
					DWORD           dwInputStreamID,
					DWORD           *pdwFlags);

				/// <summary>
				/// Queries whether the transform is ready to produce output data.
				/// </summary>
				/// <param name="pdwFlags">Receives a member of the _MFT_OUTPUT_STATUS_FLAGS enumeration, or zero. If the value is MFT_OUTPUT_STATUS_SAMPLE_READY, the MFT can produce an output sample. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetOutputStatus(DWORD *pdwFlags);

				/// <summary>
				/// Sets the range of timestamps the client needs for output.
				/// </summary>
				/// <param name="hnsLowerBound">Specifies the earliest time stamp. The Media Foundation transform (MFT) will accept input until it can produce an output sample that begins at this time; or until it can produce a sample that ends at this time or later. If there is no lower bound, use the value MFT_OUTPUT_BOUND_LOWER_UNBOUNDED. </param>
				/// <param name="hnsUpperBound">Specifies the latest time stamp. The MFT will not produce an output sample with time stamps later than this time. If there is no upper bound, use the value MFT_OUTPUT_BOUND_UPPER_UNBOUNDED. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) SetOutputBounds(
					LONGLONG        hnsLowerBound,
					LONGLONG        hnsUpperBound);

				/// <summary>
				/// Sends an event to an input stream on this MFT.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
				/// <param name="pEvent">Pointer to the IMFMediaEvent interface of an event object. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) ProcessEvent(
					DWORD              dwInputStreamID,
					IMFMediaEvent      *pEvent);

				/// <summary>
				/// Sends a message to the MFT.
				/// </summary>
				/// <param name="eMessage">The message to send, specified as a member of the MFT_MESSAGE_TYPE enumeration.</param>
				/// <param name="ulParam">Message parameter. The meaning of this parameter depends on the message type. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) ProcessMessage(
					MFT_MESSAGE_TYPE    eMessage,
					ULONG_PTR           ulParam);

				/// <summary>
				/// Delivers data to an input stream on this MFT.
				/// </summary>
				/// <param name="dwInputStreamID">Input stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
				/// <param name="pSample">Pointer to the IMFSample interface of the input sample. The sample must contain at least one media buffer that contains valid input data. </param>
				/// <param name="dwFlags">Reserved. Must be zero. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) ProcessInput(
					DWORD               dwInputStreamID,
					IMFSample           *pSample,
					DWORD               dwFlags);

				/// <summary>
				/// Generates output from the current input data.
				/// </summary>
				/// <param name="dwFlags">Bitwise OR of zero or more flags from the _MFT_PROCESS_OUTPUT_FLAGS enumeration. </param>
				/// <param name="cOutputBufferCount">Number of elements in the pOutputSamples array. The value must be at least 1.</param>
				/// <param name="pOutputSamples">Pointer to an array of MFT_OUTPUT_DATA_BUFFER structures, allocated by the caller. The MFT uses this array to return output data to the caller. </param>
				/// <param name="pdwStatus">Receives a bitwise OR of zero or more flags from the _MFT_PROCESS_OUTPUT_STATUS enumeration. </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) ProcessOutput(
					DWORD                   dwFlags,
					DWORD                   cOutputBufferCount,
					MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
					DWORD                   *pdwStatus);

				/// <summary>
				/// Initialise the encoder.
				/// </summary>
				/// <param name="encoder">The encoder to start.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT InitialiseEncoder(EncoderType encoder);

				/// <summary>
				/// Close a media encoder.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT Close();

				/// <summary>
				/// Setup initial encoder paramaters.
				/// </summary>
				/// <param name="input">The input encoder details.</param>
				/// <param name="output">The output encoder details.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetupInitialEncoder(IMFMediaType **input, IMFMediaType **output);

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				MediaEncoder(HWND hwnd, HWND hEvent);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~MediaEncoder();

			private:
				/// <summary>
				/// Initialise the encoder.
				/// </summary>
				/// <param name="encoder">The encoder to start.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT CreateEncoder(const CLSID encoder);

				/// <summary>
				/// Setup initial encoder paramaters.
				/// </summary>
				/// <param name="input">The input encoder details.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT SetupInitialInput(IMFMediaType *input);

				/// <summary>
				/// Setup initial encoder paramaters.
				/// </summary>
				/// <param name="output">The output encoder details.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT SetupInitialOutput(IMFMediaType *output);

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.
				bool					_isOpen;
				bool					_created;

				HWND				    _hwndApp;
				HWND				    _hwndEvent;			// App window to receive events.
				HANDLE				    _hCloseEvent;		// Event to wait on while closing.

				IUnknown				*_transformUnk;
				IMFTransform			*_encoder;
				EncoderType				_encoderType;

				CRITICAL_SECTION        _critsec;
			};
		}
	}
}
#endif