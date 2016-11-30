/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          StoreByteStream.h
*  Purpose :       StoreByteStream class.
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

#ifndef _STOREBYTESTREAM_H
#define _STOREBYTESTREAM_H

#include "MediaGlobal.h"
#include "WriteByteContainer.h"
#include "ReadByteContainer.h"
#include "WriteByteAsyncCallback.h"
#include "ReadByteAsyncCallback.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Provides the base for a media foundation byte stream.
			/// </summary>
			class StoreByteStream : public IMFByteStream
			{
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="initialStreamSize">The number of bytes to reserve in the stream.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API StoreByteStream(QWORD initialStreamSize);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API virtual ~StoreByteStream();

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
				/// Begins an asynchronous read operation from the stream.
				/// </summary>
				/// <param name="pb">Pointer to a buffer that receives the data. The caller must allocate the buffer. [in]</param>
				/// <param name="cb">Size of the buffer in bytes. [in]</param>
				/// <param name="pCallback">Pointer to the IMFAsyncCallback interface of a callback object. The caller must implement this interface. [in]</param>
				/// <param name="punkState">Pointer to the IUnknown interface of a state object, defined by the caller. This parameter can be NULL. 
				/// You can use this object to hold state information. The object is returned to the caller when the callback is invoked. [in]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) BeginRead(
					BYTE             *pb,
					ULONG            cb,
					IMFAsyncCallback *pCallback,
					IUnknown         *punkState);

				/// <summary>
				/// Begins an asynchronous write operation to the stream.
				/// </summary>
				/// <param name="pb">Pointer to a buffer containing the data to write. [in]</param>
				/// <param name="cb">Size of the buffer in bytes. [in]</param>
				/// <param name="pCallback">Pointer to the IMFAsyncCallback interface of a callback object. The caller must implement this interface. [in]</param>
				/// <param name="punkState">Pointer to the IUnknown interface of a state object, defined by the caller. This parameter can be NULL. 
				/// You can use this object to hold state information. The object is returned to the caller when the callback is invoked. [in]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) BeginWrite(
					const BYTE       *pb,
					ULONG            cb,
					IMFAsyncCallback *pCallback,
					IUnknown         *punkState);

				/// <summary>
				/// Closes the stream and releases any resources associated with the stream, such as sockets or file handles. 
				/// This method also cancels any pending asynchronous I/O requests. 
				/// </summary>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) Close();

				/// <summary>
				/// Completes an asynchronous read operation.
				/// </summary>
				/// <param name="pResult">Pointer to the IMFAsyncResult interface. Pass in the same pointer that your callback object received in the IMFAsyncCallback::Invoke method. [in]</param>
				/// <param name="pcbRead">Receives the number of bytes that were read. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) EndRead(
					IMFAsyncResult *pResult,
					ULONG          *pcbRead);

				/// <summary>
				/// Completes an asynchronous write operation.
				/// </summary>
				/// <param name="pResult">Pointer to the IMFAsyncResult interface. Pass in the same pointer that your callback object received in the IMFAsyncCallback::Invoke method. [in]</param>
				/// <param name="pcbWritten">Receives the number of bytes that were written. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) EndWrite(
					IMFAsyncResult *pResult,
					ULONG          *pcbWritten);

				/// <summary>
				/// Clears any internal buffers used by the stream. If you are writing to the stream, the buffered data is written to the underlying file or device.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) Flush();

				/// <summary>
				/// Retrieves the characteristics of the byte stream.
				/// </summary>
				/// <param name="pdwCapabilities">Receives a bitwise OR of zero or more flags. The following flags are defined. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetCapabilities(
					DWORD *pdwCapabilities);

				/// <summary>
				/// Retrieves the current read or write position in the stream.
				/// </summary>
				/// <param name="pqwPosition">Receives the current position, in bytes. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetCurrentPosition(
					QWORD *pqwPosition);

				/// <summary>
				/// Retrieves the length of the stream.
				/// </summary>
				/// <param name="pqwLength">Receives the length of the stream, in bytes. If the length is unknown, this value is -1. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) GetLength(
					QWORD *pqwLength);

				/// <summary>
				/// Queries whether the current position has reached the end of the stream.
				/// </summary>
				/// <param name="pfEndOfStream">Receives the value TRUE if the end of the stream has been reached, or FALSE otherwise. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) IsEndOfStream(
					BOOL *pfEndOfStream);

				/// <summary>
				/// Reads data from the stream.
				/// </summary>
				/// <param name="pb">Pointer to a buffer that receives the data. The caller must allocate the buffer. [in]</param>
				/// <param name="cb">Size of the buffer in bytes. [in]</param>
				/// <param name="pcbRead">Receives the number of bytes that are copied into the buffer. This parameter cannot be NULL. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) Read(
					BYTE  *pb,
					ULONG cb,
					ULONG *pcbRead);

				/// <summary>
				/// Moves the current position in the stream by a specified offset.
				/// </summary>
				/// <param name="SeekOrigin">Specifies the origin of the seek as a member of the MFBYTESTREAM_SEEK_ORIGIN enumeration. The offset is calculated relative to this position. [in]</param>
				/// <param name="qwSeekOffset">Specifies the new position, as a byte offset from the seek origin. [in]</param>
				/// <param name="dwSeekFlags">Specifies zero or more flags. The following flags are defined. [in]</param>
				/// <param name="pqwCurrentPosition">Receives the new position after the seek. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) Seek(
					MFBYTESTREAM_SEEK_ORIGIN SeekOrigin,
					LONGLONG                 qwSeekOffset,
					DWORD                    dwSeekFlags,
					QWORD                    *pqwCurrentPosition);

				/// <summary>
				/// Sets the current read or write position.
				/// </summary>
				/// <param name="qwPosition">New position in the stream, as a byte offset from the start of the stream. [in]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) SetCurrentPosition(
					QWORD qwPosition);

				/// <summary>
				/// Sets the length of the stream.
				/// </summary>
				/// <param name="qwLength">Length of the stream in bytes. [in]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) SetLength(
					QWORD qwLength);

				/// <summary>
				/// Writes data to the stream.
				/// </summary>
				/// <param name="pb">Pointer to a buffer that contains the data to write. [in]</param>
				/// <param name="cb">Size of the buffer in bytes. [in]</param>
				/// <param name="pcbWritten">Receives the number of bytes that are written. [out]</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API STDMETHODIMP_(HRESULT) Write(
					const BYTE  *pb,
					ULONG       cb,
					ULONG       *pcbWritten);

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.

				QWORD					_position;
				QWORD					_initialStreamSize;
				std::vector<BYTE>		_streamData;

				CRITICAL_SECTION        _critsec;
			};
		}
	}
}
#endif