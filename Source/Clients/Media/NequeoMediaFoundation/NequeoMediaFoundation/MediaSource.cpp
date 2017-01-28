/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaSource.cpp
*  Purpose :       MediaSource class.
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

#include "MediaSource.h"

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
			MediaSource::MediaSource(HWND hwnd, HWND hEvent, HRESULT &hr) :
				_hwndApp(hwnd),
				_hwndEvent(hEvent),
				_pSourceReader(NULL),
				_hCloseEvent(NULL),
				_nRefCount(1),
				_isOpen(false),
				_isSourceReader(false),
				_disposed(false)
			{
				// Initialize critical section.
				InitializeCriticalSection(&_critsec);
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaSource::~MediaSource()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					// Close a media source.
					Close();

					// Delete critical section.
					DeleteCriticalSection(&_critsec);
				}
			}

			/// <summary>
			/// Static class method to create the MediaCapture object.
			/// </summary>
			/// <param name="hwnd">The handle to the application owner.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="ppMediaSource">Receives an AddRef's pointer to the MediaSource object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::CreateInstance(HWND hwnd, HWND hEvent, MediaSource **ppMediaSource)
			{
				// Make sure the a video and event handler exists.
				assert(hwnd != NULL);
				assert(hEvent != NULL);

				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// MediaCapture constructor sets the ref count to zero.
				// Create method calls AddRef.
				// Create a new medis source instance.
				MediaSource *pSource = new MediaSource(hwnd, hEvent, hr);

				// If the preview was not created.
				if (pSource == NULL)
				{
					// Out of memory result.
					hr = E_OUTOFMEMORY;
				}

				// If successful initialisation.
				if (SUCCEEDED(hr))
				{
					// Increment the reference count of the current player.
					*ppMediaSource = pSource;
					(*ppMediaSource)->AddRef();
				}
				else
				{
					// Delete the instance of the source
					// if not successful.
					delete pSource;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaSource::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaSource::Release()
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
			HRESULT MediaSource::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}

				// Attach MediaCapture to the interface.
				static const QITAB qit[] =
				{
					QITABENT(MediaSource, IUnknown),
					{ 0 },
				};
				return QISearch(this, qit, iid, ppv);
			}

			/// <summary>
			/// Close a media source.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::Close()
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// If the reader exists. 
				if (_pSourceReader)
				{
					// Finalise the reader.
					hr = _pSourceReader->Release();
					_isSourceReader = false;
				}

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
				SafeRelease(&_pSourceReader);

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initializes the media foundation.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::Initialize()
			{
				// Initialse the result to all is good.
				HRESULT hr = S_OK;

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

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open a file stream.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to read data from.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::OpenFile(const WCHAR *pwszFileName)
			{
				HRESULT hr = S_OK;

				// Open the source.
				hr = OpenMediaSource(pwszFileName, NULL, true);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open a byte stream.
			/// </summary>
			/// <param name="pByteStream">The byte stream to read data from.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::OpenStream(IMFByteStream *pByteStream)
			{
				HRESULT hr = S_OK;

				// Open the source.
				hr = OpenMediaSource(NULL, pByteStream, false);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open a media source.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to read data from.</param>
			/// <param name="pByteStream">The byte stream to read data from.</param>
			/// <param name="readFromFile">Read to file; else read from byte stream.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::OpenMediaSource(const WCHAR *pwszFileName, IMFByteStream *pByteStream, bool readFromFile)
			{
				HRESULT hr = S_OK;

				// Initializes the media foundation.
				hr = Initialize();
				
				// If reading from file.
				if (readFromFile)
				{
					// Open the file URL source.
					hr = MFCreateSourceReaderFromURL(
						pwszFileName, 
						NULL, 
						&_pSourceReader);
				}
				else
				{
					// Open the byte stream source.
					hr = MFCreateSourceReaderFromByteStream(
						pByteStream,
						NULL,
						&_pSourceReader);
				}

				if (SUCCEEDED(hr))
					_isSourceReader = true;
				else
					_isSourceReader = false;

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Get the media details.
			/// </summary>
			/// <param name="param">The encoding parameters.</param>
			/// <param name="hasVideo">True if the source contains video; else false.</param>
			/// <param name="hasAudio">True if the source contains audio; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::MediaDetails(EncodingParameters& param, bool *hasVideo, bool *hasAudio)
			{
				HRESULT hr = S_OK;

				*hasVideo = false;
				*hasAudio = false;

				// If a source reader exists.
				if (_isSourceReader)
				{
					IMFMediaType *pVideoType = NULL;
					IMFMediaType *pAudioType = NULL;

					// Get the media type.
					hr = _pSourceReader->GetNativeMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						0,  // Type index
						&pVideoType
					);

					// If a video source exists.
					if (SUCCEEDED(hr))
					{
						*hasVideo = true;

						GUID majorType;
						hr = pVideoType->GetGUID(MF_MT_MAJOR_TYPE, &majorType);

						if (SUCCEEDED(hr))
							// Get the major type.
							param.video.transcode = majorType;

						GUID subTypeType;
						hr = pVideoType->GetGUID(MF_MT_SUBTYPE, &subTypeType);

						if (SUCCEEDED(hr))
							// Get the sub type.
							param.video.subtype = subTypeType;

						UINT32 bitRate;
						hr = pVideoType->GetUINT32(MF_MT_AVG_BITRATE, &bitRate);

						if (SUCCEEDED(hr))
						{
							// Get the default bit rate.
							param.video.bitRate = bitRate;
						}

						UINT32 width;
						UINT32 height;
						hr = MFGetAttributeSize(pVideoType, MF_MT_FRAME_SIZE, &width, &height);

						if (SUCCEEDED(hr))
						{
							// Get the default frame size.
							param.video.frameSize.width = width;
							param.video.frameSize.height = height;
						}

						UINT32 numeratorRate;
						UINT32 denominatorRate;
						hr = MFGetAttributeRatio(pVideoType, MF_MT_FRAME_RATE, &numeratorRate, &denominatorRate);

						if (SUCCEEDED(hr))
						{
							// Get the default frame rate.
							param.video.frameRate.numerator = numeratorRate;
							param.video.frameRate.denominator = denominatorRate;
						}

						UINT32 numeratorRatio;
						UINT32 denominatorRatio;
						hr = MFGetAttributeRatio(pVideoType, MF_MT_PIXEL_ASPECT_RATIO, &numeratorRatio, &denominatorRatio);

						if (SUCCEEDED(hr))
						{
							// Get the default frame rate.
							param.video.aspectRatio.numerator = numeratorRatio;
							param.video.aspectRatio.denominator = denominatorRatio;
						}
					}

					// Get the media type.
					hr = _pSourceReader->GetNativeMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						0,  // Type index
						&pAudioType
					);

					// If a audio source exists.
					if (SUCCEEDED(hr))
					{
						*hasAudio = true;

						GUID majorType;
						hr = pAudioType->GetGUID(MF_MT_MAJOR_TYPE, &majorType);

						if (SUCCEEDED(hr))
							// Get the major type.
							param.audio.transcode = majorType;

						GUID subTypeType;
						hr = pAudioType->GetGUID(MF_MT_SUBTYPE, &subTypeType);

						if (SUCCEEDED(hr))
							// Get the sub type.
							param.audio.subtype = subTypeType;

						UINT32 sampleRate;
						hr = pAudioType->GetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, &sampleRate);

						if (SUCCEEDED(hr))
						{
							// Get the default sample rate.
							param.audio.sampleRate = sampleRate;
						}

						UINT32 channels;
						hr = pAudioType->GetUINT32(MF_MT_AUDIO_NUM_CHANNELS, &channels);

						if (SUCCEEDED(hr))
						{
							// Get the default channels.
							param.audio.channels = channels;
						}

						UINT32 bitsPerSample;
						hr = pAudioType->GetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, &bitsPerSample);

						if (SUCCEEDED(hr))
						{
							// Get the default bits per sample.
							param.audio.bitsPerSample = bitsPerSample;
						}

						UINT32 blockAlign;
						hr = pAudioType->GetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, &blockAlign);

						if (SUCCEEDED(hr))
						{
							// Get the default block align.
							param.audio.blockAlign = blockAlign;
						}

						UINT32 bytesPerSecond;
						hr = pAudioType->GetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, &bytesPerSecond);

						if (SUCCEEDED(hr))
						{
							// Get the default bytes per second.
							param.audio.bytesPerSecond = bytesPerSecond;
						}
					}

					// Safe release.
					SafeRelease(&pVideoType);
					SafeRelease(&pAudioType);
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
			/// Read the current sample.
			/// </summary>
			/// <param name="streamIndex">The stream to pull data from. The value can be any of the following.
			/// A zero-based index of a stream (e.g. 0, 1, etc. 0 could be video, 1 could be audio).
			/// The first video stream : MF_SOURCE_READER_FIRST_VIDEO_STREAM 0xFFFFFFFC.
			/// The first audio stream : MF_SOURCE_READER_FIRST_AUDIO_STREAM 0xFFFFFFFD.
			/// Get the next available sample, regardless of which stream :  MF_SOURCE_READER_ANY_STREAM 0xFFFFFFFE.
			/// </param>
			/// <param name="endOfSamples">True if no more samples exist.</param>
			/// <param name="pdwActualStreamIndex">Receives the zero-based index of the stream.</param>
			/// <param name="pdwStreamFlags">Receives a bitwise OR of zero or more flags from the MF_SOURCE_READER_FLAG enumeration.</param>
			/// <param name="pllTimestamp">Receives the time stamp of the sample, or the time of the stream event indicated in pdwStreamFlags. The time is given in 100-nanosecond units.</param>
			/// <param name="ppSample">Receives a pointer to the IMFSample interface or the value NULL (see Remarks). If this parameter receives a non-NULL pointer, the caller must release the interface.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::ReadSample(
				DWORD streamIndex,
				bool *endOfSamples, 
				DWORD *pdwActualStreamIndex, 
				DWORD *pdwStreamFlags, 
				LONGLONG *pllTimestamp, 
				IMFSample **ppSample)
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				*endOfSamples = true;

				// If a source reader exists.
				if (_isSourceReader)
				{
					// Get the next sample, either video or audio.
					hr = _pSourceReader->ReadSample(
						streamIndex,
						0, 
						pdwActualStreamIndex, 
						pdwStreamFlags, 
						pllTimestamp, 
						ppSample);

					if (SUCCEEDED(hr))
					{
						// If samples is NULL, then no more data.
						if (ppSample == NULL)
						{
							// No more samples.
							*endOfSamples = true;
						}
						else
						{
							// More samples.
							*endOfSamples = false;
						}
					}
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Seeks to a new position in the media source.
			/// </summary>
			/// <param name="position">The position from which playback will be started. The units are specified by the timeFormat parameter. If the timeFormat parameter is GUID_NULL, set the variant type to VT_I8.</param>
			/// <param name="timeFormat">A GUID that specifies the time format. The time format defines the units for the varPosition parameter. The following value is defined for all media sources: 
			/// GUID_NULL : 100-nanosecond units.
			/// Some media sources might support additional values.
			/// </param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::SetPosition(REFPROPVARIANT position, REFGUID timeFormat)
			{
				HRESULT hr = S_OK;

				// If a source reader exists.
				if (_isSourceReader)
				{
					// Set the new position.
					hr = _pSourceReader->SetCurrentPosition(timeFormat, position);
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
			/// Seeks to a new position in the media source.
			/// </summary>
			/// <param name="position">The position from which playback will be started.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::SetPosition(LARGE_INTEGER position)
			{
				HRESULT hr = S_OK;

				// If a source reader exists.
				if (_isSourceReader)
				{
					// If the timeFormat parameter is GUID_NULL, set the variant type to VT_I8.
					REFGUID timeFormat = GUID_NULL;

					// Variant type init.
					PROPVARIANT var;
					PropVariantInit(&var);

					// Set the position.
					var.hVal = position;

					// Set the new position.
					hr = _pSourceReader->SetCurrentPosition(timeFormat, var);

					// Clear the variant.
					PropVariantClear(&var);
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
			/// Seeks to a new position in the media source.
			/// </summary>
			/// <param name="position">The position from which playback will be started.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::SetPosition(LONGLONG position)
			{
				HRESULT hr = S_OK;

				// If a source reader exists.
				if (_isSourceReader)
				{
					// If the timeFormat parameter is GUID_NULL, set the variant type to VT_I8.
					REFGUID timeFormat = GUID_NULL;

					// Variant type init.
					PROPVARIANT var;
					PropVariantInit(&var);

					// Set the position.
					var.hVal.QuadPart = position;

					// Set the new position.
					hr = _pSourceReader->SetCurrentPosition(timeFormat, var);

					// Clear the variant.
					PropVariantClear(&var);
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Return the result.
				return hr;
			}
		}
	}
}