/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          StoreByteStream.cpp
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

#include "pch.h"

#include "StoreByteStream.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			/// <param name="initialStreamSize">The number of bytes to reserve in the stream.</param>
			StoreByteStream::StoreByteStream(QWORD initialStreamSize) :
				_nRefCount(1),
				_position(0),
				_initialStreamSize(initialStreamSize),
				_writePriority(0),
				_readPriority(0),
				_disposed(false)
			{
				// Reserve some memeory space.
				_streamData.resize(initialStreamSize);

				// Initialize critical section.
				InitializeCriticalSection(&_critsec);
			}

			/// <summary>
			/// This destructor. Call release to cleanup resources.
			/// </summary>
			StoreByteStream::~StoreByteStream()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					// Delete critical section.
					DeleteCriticalSection(&_critsec);

					// Clear the contents.
					_streamData.clear();
				}
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG StoreByteStream::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG StoreByteStream::Release()
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
			HRESULT StoreByteStream::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}
				return S_OK;
			}

			/// <summary>
			/// Begins an asynchronous read operation from the stream.
			/// </summary>
			/// <param name="pb">Pointer to a buffer that receives the data. The caller must allocate the buffer. [in]</param>
			/// <param name="cb">Size of the buffer in bytes. [in]</param>
			/// <param name="pCallback">Pointer to the IMFAsyncCallback interface of a callback object. The caller must implement this interface. [in]</param>
			/// <param name="punkState">Pointer to the IUnknown interface of a state object, defined by the caller. This parameter can be NULL. 
			/// You can use this object to hold state information. The object is returned to the caller when the callback is invoked. [in]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::BeginRead(
				BYTE             *pb,
				ULONG            cb,
				IMFAsyncCallback *pCallback,
				IUnknown         *punkState)
			{
				HRESULT hr = S_OK;

				// Increment the read priority.
				IncrementReadPriority();

				// Create a new read byte container.
				ReadByteContainer* readBytes = new ReadByteContainer(pb, cb);
				ReadByteAsyncCallback* readCallback = new ReadByteAsyncCallback(this);

				// If not created.
				if (readBytes == NULL)
				{
					return E_OUTOFMEMORY;
				}

				// If not created.
				if (readCallback == NULL)
				{
					return E_OUTOFMEMORY;
				}

				IMFAsyncResult *pResult = NULL;
				readBytes->_readCallback = readCallback;

				// Creates an asynchronous result object. Use this function if you are implementing an asynchronous method.
				hr = MFCreateAsyncResult(readBytes, pCallback, punkState, &pResult);

				if (SUCCEEDED(hr))
				{
					// Start a new work item thread.
					hr = MFPutWorkItem2(MFASYNC_CALLBACK_QUEUE_STANDARD, _readPriority, readCallback, pResult);
					pResult->Release();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Begins an asynchronous write operation to the stream.
			/// </summary>
			/// <param name="pb">Pointer to a buffer containing the data to write. [in]</param>
			/// <param name="cb">Size of the buffer in bytes. [in]</param>
			/// <param name="pCallback">Pointer to the IMFAsyncCallback interface of a callback object. The caller must implement this interface. [in]</param>
			/// <param name="punkState">Pointer to the IUnknown interface of a state object, defined by the caller. This parameter can be NULL. 
			/// You can use this object to hold state information. The object is returned to the caller when the callback is invoked. [in]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::BeginWrite(
				const BYTE       *pb,
				ULONG            cb,
				IMFAsyncCallback *pCallback,
				IUnknown         *punkState)
			{
				HRESULT hr = S_OK;

				// Increment the write priority.
				IncrementWritePriority();

				// Create a new write byte container.
				WriteByteContainer* writeBytes = new WriteByteContainer(pb, cb);
				WriteByteAsyncCallback* writeCallback = new WriteByteAsyncCallback(this);

				// If not created.
				if (writeBytes == NULL)
				{
					return E_OUTOFMEMORY;
				}

				if (writeCallback == NULL)
				{
					return E_OUTOFMEMORY;
				}

				IMFAsyncResult *pResult = NULL;
				writeBytes->_writeCallback = writeCallback;

				// Creates an asynchronous result object. Use this function if you are implementing an asynchronous method.
				hr = MFCreateAsyncResult(writeBytes, pCallback, punkState, &pResult);

				if (SUCCEEDED(hr))
				{
					// Start a new work item thread.
					hr = MFPutWorkItem2(MFASYNC_CALLBACK_QUEUE_STANDARD, _writePriority, writeCallback, pResult);
					pResult->Release();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Closes the stream and releases any resources associated with the stream, such as sockets or file handles. 
			/// This method also cancels any pending asynchronous I/O requests. 
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::Close()
			{
				HRESULT hr = S_OK;

				// Delete the stream data.
				_streamData.clear();

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Completes an asynchronous read operation.
			/// </summary>
			/// <param name="pResult">Pointer to the IMFAsyncResult interface. Pass in the same pointer that your callback object received in the IMFAsyncCallback::Invoke method. [in]</param>
			/// <param name="pcbRead">Receives the number of bytes that were read. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::EndRead(
				IMFAsyncResult *pResult,
				ULONG          *pcbRead)
			{
				HRESULT hr = S_OK;
				IUnknown *pUnk = NULL;
				ReadByteContainer* readBytes = NULL;

				// Get the status.
				hr = pResult->GetStatus();

				if (FAILED(hr))
				{
					goto done;
				}

				// Get the read container.
				hr = pResult->GetObject(&pUnk);
				if (FAILED(hr))
				{
					goto done;
				}

				// Cast the read byte container.
				readBytes = static_cast<ReadByteContainer*>(pUnk);

				// Assign the read bytes.
				*pcbRead = readBytes->_pcbRead;

			done:
				// Clean-up.
				SafeRelease(&pUnk);

				// If read is not null.
				if (readBytes != NULL)
				{
					SAFE_RELEASE(readBytes->_readCallback);
					SAFE_RELEASE(readBytes);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Completes an asynchronous write operation.
			/// </summary>
			/// <param name="pResult">Pointer to the IMFAsyncResult interface. Pass in the same pointer that your callback object received in the IMFAsyncCallback::Invoke method. [in]</param>
			/// <param name="pcbWritten">Receives the number of bytes that were written. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::EndWrite(
				IMFAsyncResult *pResult,
				ULONG          *pcbWritten)
			{
				HRESULT hr = S_OK;
				IUnknown *pUnk = NULL;
				WriteByteContainer* writeBytes = NULL;

				// Get the status.
				hr = pResult->GetStatus();

				if (FAILED(hr))
				{
					goto done;
				}

				// Get the write container.
				hr = pResult->GetObject(&pUnk);
				if (FAILED(hr))
				{
					goto done;
				}

				// Cast the write byte container.
				writeBytes = static_cast<WriteByteContainer*>(pUnk);

				// Assign the write bytes.
				*pcbWritten = writeBytes->_pcbWritten;

			done:
				// Clean-up.
				SafeRelease(&pUnk);

				// If write is not null.
				if (writeBytes != NULL)
				{
					SAFE_RELEASE(writeBytes->_writeCallback);
					SAFE_RELEASE(writeBytes);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Clears any internal buffers used by the stream. If you are writing to the stream, the buffered data is written to the underlying file or device.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::Flush()
			{
				HRESULT hr = S_OK;

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the characteristics of the byte stream.
			/// </summary>
			/// <param name="pdwCapabilities">Receives a bitwise OR of zero or more flags. The following flags are defined. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::GetCapabilities(
				DWORD *pdwCapabilities)
			{
				HRESULT hr = S_OK;

				// Stream can read, can write, can seek.
				*pdwCapabilities = MFBYTESTREAM_IS_READABLE | MFBYTESTREAM_IS_WRITABLE | MFBYTESTREAM_IS_SEEKABLE;

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the current read or write position in the stream.
			/// </summary>
			/// <param name="pqwPosition">Receives the current position, in bytes. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::GetCurrentPosition(
				QWORD *pqwPosition)
			{
				HRESULT hr = S_OK;

				// Get the position in the stream.
				*pqwPosition = _position;

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Retrieves the length of the stream.
			/// </summary>
			/// <param name="pqwLength">Receives the length of the stream, in bytes. If the length is unknown, this value is -1. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::GetLength(
				QWORD *pqwLength)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// The length of the stream;
				size_t size = _streamData.size();
				*pqwLength = size;

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Queries whether the current position has reached the end of the stream.
			/// </summary>
			/// <param name="pfEndOfStream">Receives the value TRUE if the end of the stream has been reached, or FALSE otherwise. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::IsEndOfStream(
				BOOL *pfEndOfStream)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// if the position is larger than or same as the size
				// then this is the end of the stream.
				if (_position >= _streamData.size())
				{
					// At the end of the stream.
					*pfEndOfStream = TRUE;
				}
				else
				{
					// If data.
					*pfEndOfStream = FALSE;
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Reads data from the stream.
			/// </summary>
			/// <param name="pb">Pointer to a buffer that receives the data. The caller must allocate the buffer. [in]</param>
			/// <param name="cb">Size of the buffer in bytes. [in]</param>
			/// <param name="pcbRead">Receives the number of bytes that are copied into the buffer. This parameter cannot be NULL. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::Read(
				BYTE  *pb,
				ULONG cb,
				ULONG *pcbRead)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// Get the size.
				size_t size = _streamData.size();
				ULONG numberToRead = cb;
				ULONG numberRead = 0;

				// If reading more then exists.
				if (cb >= (ULONG)size)
				{
					// Set the new number to read.
					numberToRead = (ULONG)size;
				}

				// If not at the end of the stream.
				// then read data.
				if (_position < size)
				{
					// Read the data.
					for (ULONG i = 0; i < numberToRead; i++)
					{
						// If the current position is less than
						// the size of the stream.
						if (_position < size)
						{
							// Write to the buffer.
							pb[i] = _streamData[_position];

							// Increment the current position.
							_position++;
							numberRead++;
						}
						else
						{
							// No more to read.
							break;
						}
					}
				}

				// Set the number read.
				*pcbRead = numberRead;

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Moves the current position in the stream by a specified offset.
			/// </summary>
			/// <param name="SeekOrigin">Specifies the origin of the seek as a member of the MFBYTESTREAM_SEEK_ORIGIN enumeration. The offset is calculated relative to this position. [in]</param>
			/// <param name="qwSeekOffset">Specifies the new position, as a byte offset from the seek origin. [in]</param>
			/// <param name="dwSeekFlags">Specifies zero or more flags. The following flags are defined. [in]</param>
			/// <param name="pqwCurrentPosition">Receives the new position after the seek. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::Seek(
				MFBYTESTREAM_SEEK_ORIGIN SeekOrigin,
				LONGLONG                 qwSeekOffset,
				DWORD                    dwSeekFlags,
				QWORD                    *pqwCurrentPosition)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// Get the size.
				size_t size = _streamData.size();

				// Select the seek origin.
				switch (SeekOrigin)
				{
				case MFBYTESTREAM_SEEK_ORIGIN::msoCurrent:
					// If the buffer is less or same.
					if ((qwSeekOffset + _position) < size)
						_position += qwSeekOffset;
					else
						_position = size;

					break;

				case MFBYTESTREAM_SEEK_ORIGIN::msoBegin:
				default:
					// If the buffer is less or same.
					if (qwSeekOffset < size)
						_position = qwSeekOffset;
					else
						_position = size;

					break;
				}

				// Get the current position in the stream.
				*pqwCurrentPosition = _position;

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sets the current read or write position.
			/// </summary>
			/// <param name="qwPosition">New position in the stream, as a byte offset from the start of the stream. [in]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::SetCurrentPosition(
				QWORD qwPosition)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// Get the size.
				size_t size = _streamData.size();

				if (qwPosition >= size)
				{
					// Set the position in the stream, the end of the stream.
					_position = size;
				}
				else
				{
					// Set the position in the stream.
					_position = qwPosition;
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Sets the length of the stream.
			/// </summary>
			/// <param name="qwLength">Length of the stream in bytes. [in]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::SetLength(
				QWORD qwLength)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// Reserve some memeory space.
				_streamData.resize(qwLength);
				_initialStreamSize = _streamData.size();

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Writes data to the stream.
			/// </summary>
			/// <param name="pb">Pointer to a buffer that contains the data to write. [in]</param>
			/// <param name="cb">Size of the buffer in bytes. [in]</param>
			/// <param name="pcbWritten">Receives the number of bytes that are written. [out]</param>
			/// <returns>The result of the operation.</returns>
			HRESULT StoreByteStream::Write(
				const BYTE  *pb,
				ULONG       cb,
				ULONG       *pcbWritten)
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				HRESULT hr = S_OK;

				// Write the bytes.
				for (ULONG i = 0; i < cb; i++)
				{
					// If the new position is the same as the length
					// then add the new element.
					if (_position >= _initialStreamSize)
					{
						// Add to the stream data.
						_streamData.push_back(pb[i]);
					}
					else
					{
						// Add to index from the current position.
						_streamData[_position] = pb[i];
					}

					// Increment the current position.
					_position++;
				}

				// All bytes are written.
				*pcbWritten = cb;

				// Get the size, set the new capacity.
				size_t size = _streamData.size();
				_initialStreamSize = (QWORD)size;

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Set or reset the write priority.
			/// </summary>
			/// <param name="priority">The starting priority.</param>
			void StoreByteStream::SetWritePriority(LONG priority)
			{
				InterlockedExchange(&_writePriority, priority);
			}

			/// <summary>
			/// Set or reset the read priority.
			/// </summary>
			/// <param name="priority">The starting priority.</param>
			void StoreByteStream::SetReadPriority(LONG priority)
			{
				InterlockedExchange(&_readPriority, priority);
			}

			/// <summary>
			/// Increment the write priority.
			/// </summary>
			void StoreByteStream::IncrementWritePriority()
			{
				InterlockedIncrement(&_writePriority);
			}

			/// <summary>
			/// Increment the read priority.
			/// </summary>
			void StoreByteStream::IncrementReadPriority()
			{
				InterlockedIncrement(&_readPriority);
			}
		}
	}
}