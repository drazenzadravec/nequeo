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
			HRESULT ConfigureVideoAudioSourceEncoder(EncodingParameters&, IMFMediaType*, IMFMediaType*, IMFSinkWriter*, DWORD*, DWORD*);
			HRESULT CopyAttributeSource(IMFAttributes*, IMFAttributes*, const GUID&);

			// Gets an interface pointer from a Media Foundation collection.
			template <class IFACE>
			HRESULT GetCollectionObjectSource(IMFCollection *pCollection, DWORD index, IFACE **ppObject)
			{
				IUnknown *pUnk;
				HRESULT hr = pCollection->GetElement(index, &pUnk);
				if (SUCCEEDED(hr))
				{
					hr = pUnk->QueryInterface(IID_PPV_ARGS(ppObject));
					pUnk->Release();
				}
				return hr;
			}

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
				_pSourceWriter(NULL),
				_hCloseEvent(NULL),
				_hNotifyStateEvent(NULL),
				_hNotifyErrorEvent(NULL),
				_mediaSourceState(MediaClosed),
				_nRefCount(1),
				_isOpen(false),
				_isSourceReader(false),
				_isSourceWriter(false),
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
			/// Static class method to create the MediaSource object.
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

				// MediaSource constructor sets the ref count to zero.
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

				// If the writer exists. 
				if (_pSourceWriter)
				{
					// Finalise the writer.
					hr = _pSourceWriter->Finalize();
					_isSourceWriter = false;
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
				SafeRelease(&_pSourceWriter);

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

				// If not open.
				if (!_isOpen)
				{
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
			/// <param name="readFromFile">Read from file; else read from byte stream.</param>
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
			/// Open a write file stream.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to write data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::OpenWriteFile(const WCHAR *pwszFileName, const EncodingParameters& param)
			{
				HRESULT hr = S_OK;

				// Open the source.
				hr = OpenWriteMediaSource(pwszFileName, NULL, param, true);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open a write byte stream.
			/// </summary>
			/// <param name="pByteStream">The byte stream to write data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::OpenWriteStream(IMFByteStream *pByteStream, const EncodingParameters& param)
			{
				HRESULT hr = S_OK;

				// Open the source.
				hr = OpenWriteMediaSource(NULL, pByteStream, param, false);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open a write media source.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to write data to.</param>
			/// <param name="pByteStream">The byte stream to write data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <param name="writeToFile">Write to file; else write to byte stream.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::OpenWriteMediaSource(const WCHAR *pwszFileName, IMFByteStream *pByteStream, const EncodingParameters& param, bool writeToFile)
			{
				HRESULT hr = S_OK;

				IMFAttributes *pAttributesByteStream = NULL;

				// Initializes the media foundation.
				hr = Initialize();

				// If writing to file.
				if (writeToFile)
				{
					// Get this audio device media source.
					hr = MFCreateAttributes(&pAttributesByteStream, 1);

					// Create the sink writer 
					if (SUCCEEDED(hr))
					{
						// Set the MF_SINK_WRITER_DISABLE_THROTTLING.
						hr = pAttributesByteStream->SetUINT32(MF_SINK_WRITER_DISABLE_THROTTLING, TRUE);
					}

					// Create sink writer from URL.
					hr = MFCreateSinkWriterFromURL(
						pwszFileName,
						NULL,
						NULL,
						&_pSourceWriter
					);
				}
				else
				{
					// Get this audio device media source.
					hr = MFCreateAttributes(&pAttributesByteStream, 1);

					// Create the sink writer 
					if (SUCCEEDED(hr))
					{
						// Set the MF_TRANSCODE_CONTAINERTYPE.
						hr = pAttributesByteStream->SetGUID(
							MF_TRANSCODE_CONTAINERTYPE,
							param.transcode
						);
					}

					// Create the sink writer 
					if (SUCCEEDED(hr))
					{
						// Create sink writer from URL.
						hr = MFCreateSinkWriterFromURL(
							NULL,
							pByteStream,
							pAttributesByteStream,
							&_pSourceWriter
						);
					}
				}

				if (SUCCEEDED(hr))
					_isSourceWriter = true;
				else
					_isSourceWriter = false;

				// Safe release.
				SafeRelease(&pAttributesByteStream);

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
			/// Get the media types.
			/// </summary>
			/// <param name="videoType">The video type details.</param>
			/// <param name="audioType">The audio type details.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::MediaTypes(IMFMediaType **videoType, IMFMediaType **audioType)
			{
				HRESULT hr = S_OK;

				// If a source reader exists.
				if (_isSourceReader)
				{
					// Get the media type.
					hr = _pSourceReader->GetNativeMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						0,  // Type index
						videoType
					);

					if (SUCCEEDED(hr))
						hr = S_OK;

					// Get the media type.
					hr = _pSourceReader->GetNativeMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						0,  // Type index
						audioType
					);

					if (SUCCEEDED(hr))
						hr = S_OK;
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
			/// Write the sample for the stream to the source writer.
			/// </summary>
			/// <param name="streamIndex">The current sample stream index.</param>
			/// <param name="sample">The sample.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::WriteSample(DWORD streamIndex, IMFSample *sample)
			{
				HRESULT hr = S_OK;

				// If a source writer exists.
				if (_isSourceWriter)
				{
					// Write the sample.
					hr = _pSourceWriter->WriteSample(streamIndex, sample);
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
					var.vt = VT_I8;

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
					var.vt = VT_I8;

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
			/// Get the duration of the source (100-nanosecond).
			/// </summary>
			/// <param name="streamIndex">The stream to pull data from. The value can be any of the following.
			/// A zero-based index of a stream (e.g. 0, 1, etc. 0 could be video, 1 could be audio).
			/// The first video stream : MF_SOURCE_READER_FIRST_VIDEO_STREAM 0xFFFFFFFC.
			/// The first audio stream : MF_SOURCE_READER_FIRST_AUDIO_STREAM 0xFFFFFFFD.
			/// The media source : MF_SOURCE_READER_MEDIASOURCE 0xFFFFFFFF.
			/// </param>
			/// <param name="duration">The duration of the current stream index.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::GetDuration(DWORD streamIndex, MFTIME *duration)
			{
				HRESULT hr = S_OK;
				*duration = 0;

				// If a source reader exists.
				if (_isSourceReader)
				{
					PROPVARIANT prop;

					// Get the duration.
					hr = _pSourceReader->GetPresentationAttribute(streamIndex, MF_PD_DURATION, &prop);

					if (SUCCEEDED(hr))
					{
						// Assign the duration.
						UINT64 durationInt = prop.uhVal.QuadPart;
						*duration = durationInt;
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
			/// Copy the current sample data to the byte array.
			/// </summary>
			/// <param name="sample">The sample.</param>
			/// <param name="data">The sample byte array (caller must release the resource).</param>
			/// <param name="dataLength">The sample byte array size.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::SampleToBytes(IMFSample *sample, BYTE **data, DWORD *dataLength)
			{
				HRESULT hr = S_OK;
				*dataLength = 0;

				// Get the complete buffer.
				IMFMediaBuffer *pBuffer = NULL;
				hr = sample->ConvertToContiguousBuffer(&pBuffer);

				if (SUCCEEDED(hr))
				{
					// Lock the buffer.
					DWORD length;
					BYTE *pData = NULL;
					hr = pBuffer->Lock(&pData, NULL, &length);

					if (SUCCEEDED(hr))
					{
						// Set the data array length.
						*dataLength = length;

						// Create the byte[] that will contain
						// the current sample data.
						*data = new BYTE[length];

						// Copy the sample buffer to the byte[] data.
						memcpy_s(*data, length, pData, length);
					}

					// Unlock the buffer.
					if (pData)
					{
						hr = pBuffer->Unlock();
					}
				}

				// Safe release.
				SafeRelease(&pBuffer);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Create a bitmap from the sample byte array.
			/// </summary>
			/// <param name="data">The decompressed sample byte array.</param>
			/// <param name="dataLength">The sample byte array size.</param>
			/// <param name="bitmap">The sample bitmap.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::CreateBitmap(BYTE *data, DWORD dataLength, BITMAP *bitmap)
			{
				HRESULT hr = S_OK;

				// Assign the frame data.
				bitmap->bmBits = data;

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Create a bitmap from the sample byte array.
			/// </summary>
			/// <param name="data">The decompressed sample byte array.</param>
			/// <param name="dataLength">The sample byte array size.</param>
			/// <param name="pwszFileName">The path and name of the file.</param>
			/// <param name="param">The video frame size encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::CreateBitmap(BYTE *data, DWORD dataLength, const WCHAR *pwszFileName, const VideoFrameSize& param)
			{
				HRESULT hr = S_OK;

				LONG bmpWidth = (LONG)param.width;
				LONG bmpHeight = (LONG)param.height;

				BITMAPFILEHEADER   bmfHeader;
				BITMAPINFOHEADER   bi;

				bi.biSize = sizeof(BITMAPINFOHEADER);
				bi.biWidth = bmpWidth;
				bi.biHeight = bmpHeight;
				bi.biPlanes = 1;
				bi.biBitCount = 32;
				bi.biCompression = BI_RGB;
				bi.biSizeImage = 0;
				bi.biXPelsPerMeter = 0;
				bi.biYPelsPerMeter = 0;
				bi.biClrUsed = 0;
				bi.biClrImportant = 0;

				// A file is created, this is where we will save the screen capture.
				HANDLE hFile = CreateFile(
					pwszFileName,
					GENERIC_WRITE,
					0,
					NULL,
					CREATE_ALWAYS,
					FILE_ATTRIBUTE_NORMAL, NULL);

				// Add the size of the headers to the size of the bitmap to get the total file size
				DWORD dwSizeofDIB = dataLength + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);

				//Offset to where the actual bitmap bits start.
				bmfHeader.bfOffBits = (DWORD)sizeof(BITMAPFILEHEADER) + (DWORD)sizeof(BITMAPINFOHEADER);

				// Size of the file
				bmfHeader.bfSize = dwSizeofDIB;

				// bfType must always be BM for Bitmaps
				bmfHeader.bfType = 0x4D42; //BM   

				DWORD dwBytesWritten = 0;
				WriteFile(hFile, (LPSTR)&bmfHeader, sizeof(BITMAPFILEHEADER), &dwBytesWritten, NULL);
				WriteFile(hFile, (LPSTR)&bi, sizeof(BITMAPINFOHEADER), &dwBytesWritten, NULL);
				WriteFile(hFile, data, dataLength, &dwBytesWritten, NULL);

				// Close the handle for the file that was created
				CloseHandle(hFile);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initialise and start the source writer.
			/// </summary>
			/// <param name="videoType">The video type details.</param>
			/// <param name="audioType">The audio type details.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <param name="videoStreamIndex">The none negative video stream index; else -1 if none exists.</param>
			/// <param name="audioStreamIndex">The none negative audio stream index; else -1 if none exists.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaSource::StartSourceWriter(
				IMFMediaType *videoType, 
				IMFMediaType *audioType, 
				EncodingParameters& param,
				DWORD *videoStreamIndex,
				DWORD *audioStreamIndex)
			{
				HRESULT hr = S_OK;

				*videoStreamIndex = -1;
				*audioStreamIndex = -1;

				// If a source writer exists.
				if (_isSourceWriter)
				{
					DWORD sink_stream_video = 0;
					DWORD sink_stream_audio = 0;

					// Configure the media sink writer.
					hr = ConfigureVideoAudioSourceEncoder(param, videoType, audioType, _pSourceWriter, &sink_stream_video, &sink_stream_audio);

					if (SUCCEEDED(hr))
					{
						// Set the video stream index.
						if (sink_stream_video >= 0)
							*videoStreamIndex = sink_stream_video;

						// Set the audio stream index.
						if (sink_stream_audio >= 0)
							*audioStreamIndex = sink_stream_audio;
					}

					if (SUCCEEDED(hr))
					{
						// If a video type exists.
						if (videoType != NULL)
						{
							// Set the input media type.
							hr = _pSourceWriter->SetInputMediaType(sink_stream_video, videoType, NULL);
						}
					}

					if (SUCCEEDED(hr))
					{
						// If a audio type exists.
						if (audioType != NULL)
						{
							// Set the input media type.
							hr = _pSourceWriter->SetInputMediaType(sink_stream_audio, audioType, NULL);
						}
					}

					if (SUCCEEDED(hr))
					{
						// Begin writing.
						hr = _pSourceWriter->BeginWriting();
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
			/// Configure video and audio encoder.
			/// </summary>
			/// <param name="params">The encoding parameters.</param>
			/// <param name="pType">The media type.</param>
			/// <param name="pWriter">The sink writer.</param>
			/// <param name="pdwStreamIndex">The stream index.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ConfigureVideoAudioSourceEncoder(
				EncodingParameters& params,
				IMFMediaType *pVideoType,
				IMFMediaType *pAudioType,
				IMFSinkWriter *pWriter,
				DWORD *pdwVideoStreamIndex,
				DWORD *pdwAudioStreamIndex)
			{
				HRESULT hr = S_OK;

				IMFMediaType *pVideoType2 = NULL;
				IMFMediaType *pAudioType2 = NULL;
				IMFCollection *pAvailableTypes = NULL;
				IMFAttributes *pAttributes = NULL;

				// Get the media type.
				hr = MFCreateMediaType(&pVideoType2);
				hr = MFCreateMediaType(&pAudioType2);

				*pdwVideoStreamIndex = -1;
				*pdwAudioStreamIndex = -1;

				// If a video type exists or if a audio type exists.
				if (pVideoType != NULL || pAudioType != NULL)
				{
					// If a video type exists.
					if (pVideoType != NULL)
					{
						if (SUCCEEDED(hr))
						{
							// Set the media video
							hr = pVideoType2->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
						}

						if (SUCCEEDED(hr))
						{
							// Set the subtype.
							hr = pVideoType2->SetGUID(MF_MT_SUBTYPE, params.video.subtype);
						}

						if (SUCCEEDED(hr))
						{
							// Set the bit rate.
							if (params.video.bitRate > 0)
								hr = pVideoType2->SetUINT32(MF_MT_AVG_BITRATE, params.video.bitRate);
							else
							{
								hr = CopyAttributeSource(pVideoType, pVideoType2, MF_MT_AVG_BITRATE);
								if (SUCCEEDED(hr))
								{
									UINT32 bitRate;

									// Get the default bit rate.
									hr = pVideoType2->GetUINT32(MF_MT_AVG_BITRATE, &bitRate);
									if (SUCCEEDED(hr))
									{
										params.video.bitRate = bitRate;
									}
								}
							}
						}

						if (SUCCEEDED(hr))
						{
							// Set the frame size.
							if (params.video.frameSize.width > 0 && params.video.frameSize.height > 0)
								hr = MFSetAttributeSize(pVideoType2, MF_MT_FRAME_SIZE, params.video.frameSize.width, params.video.frameSize.height);
							else
							{
								hr = CopyAttributeSource(pVideoType, pVideoType2, MF_MT_FRAME_SIZE);
								if (SUCCEEDED(hr))
								{
									UINT32 width;
									UINT32 height;

									// Get the default frame size.
									hr = MFGetAttributeSize(pVideoType, MF_MT_FRAME_SIZE, &width, &height);
									if (SUCCEEDED(hr))
									{
										params.video.frameSize.width = width;
										params.video.frameSize.height = height;
									}
								}
							}
						}

						if (SUCCEEDED(hr))
						{
							// Set the frame rate.
							if (params.video.frameRate.numerator > 0 && params.video.frameRate.denominator > 0)
								hr = MFSetAttributeRatio(pVideoType2, MF_MT_FRAME_RATE, params.video.frameRate.numerator, params.video.frameRate.denominator);
							else
							{
								hr = CopyAttributeSource(pVideoType, pVideoType2, MF_MT_FRAME_RATE);
								if (SUCCEEDED(hr))
								{
									UINT32 numerator;
									UINT32 denominator;

									// Get the default frame rate.
									hr = MFGetAttributeRatio(pVideoType, MF_MT_FRAME_RATE, &numerator, &denominator);
									if (SUCCEEDED(hr))
									{
										params.video.frameRate.numerator = numerator;
										params.video.frameRate.denominator = denominator;
									}
								}
							}
						}

						if (SUCCEEDED(hr))
						{
							// Copy the apect ratio.
							if (params.video.aspectRatio.numerator > 0 && params.video.aspectRatio.denominator > 0)
								hr = MFSetAttributeRatio(pVideoType2, MF_MT_PIXEL_ASPECT_RATIO, params.video.aspectRatio.numerator, params.video.aspectRatio.denominator);
							else
							{
								hr = CopyAttributeSource(pVideoType, pVideoType2, MF_MT_PIXEL_ASPECT_RATIO);
								if (SUCCEEDED(hr))
								{
									UINT32 numerator;
									UINT32 denominator;

									// Get the default frame rate.
									hr = MFGetAttributeRatio(pVideoType, MF_MT_PIXEL_ASPECT_RATIO, &numerator, &denominator);
									if (SUCCEEDED(hr))
									{
										params.video.aspectRatio.numerator = numerator;
										params.video.aspectRatio.denominator = denominator;
									}
								}
							}
						}

						if (SUCCEEDED(hr))
						{
							// Copy the attribute interlace mode.
							hr = CopyAttributeSource(pVideoType, pVideoType2, MF_MT_INTERLACE_MODE);
						}

						if (SUCCEEDED(hr))
						{
							// Write to the stream.
							hr = pWriter->AddStream(pVideoType2, pdwVideoStreamIndex);
						}
					}

					// If a audio type exists.
					if (pAudioType != NULL)
					{
						if (SUCCEEDED(hr))
						{
							// Set the media audio
							hr = pAudioType2->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
						}

						if (SUCCEEDED(hr))
						{
							// Set the subtype.
							hr = pAudioType2->SetGUID(MF_MT_SUBTYPE, params.audio.subtype);
						}

						// Set the default colelction index for audio.
						if (params.audio.collectionIndex >= 0)
						{
							hr = MFCreateAttributes(&pAttributes, 1);
							if (SUCCEEDED(hr))
							{
								// Enumerate low latency media types
								hr = pAttributes->SetUINT32(MF_LOW_LATENCY, TRUE);
							}

							if (SUCCEEDED(hr))
							{
								// Get a list of encoded output formats that are supported by the encoder.
								hr = MFTranscodeGetAudioOutputAvailableTypes(params.audio.subtype, MFT_ENUM_FLAG_ALL | MFT_ENUM_FLAG_SORTANDFILTER,
									pAttributes, &pAvailableTypes);
							}

							if (SUCCEEDED(hr))
							{
								// Assgin the collection object.
								hr = GetCollectionObjectSource(pAvailableTypes, params.audio.collectionIndex, &pAudioType2);
							}

							// Get the collection index parameters that have been set.
							if (SUCCEEDED(hr))
							{
								UINT32 sampleRate;
								UINT32 channels;
								UINT32 bitsPerSample;
								UINT32 blockAlign;
								UINT32 bytesPerSecond;

								// Get the default sample rate.
								hr = pAudioType2->GetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, &sampleRate);
								if (SUCCEEDED(hr))
								{
									params.audio.sampleRate = sampleRate;
								}

								// Get the default channels.
								hr = pAudioType2->GetUINT32(MF_MT_AUDIO_NUM_CHANNELS, &channels);
								if (SUCCEEDED(hr))
								{
									params.audio.channels = channels;
								}

								// Get the default bits per sample.
								hr = pAudioType2->GetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, &bitsPerSample);
								if (SUCCEEDED(hr))
								{
									params.audio.bitsPerSample = bitsPerSample;
								}

								// Get the default block align.
								hr = pAudioType2->GetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, &blockAlign);
								if (SUCCEEDED(hr))
								{
									params.audio.blockAlign = blockAlign;
								}

								// Get the default bytes per second.
								hr = pAudioType2->GetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, &bytesPerSecond);
								if (SUCCEEDED(hr))
								{
									params.audio.bytesPerSecond = bytesPerSecond;
								}
							}
						}
						else
						{
							if (SUCCEEDED(hr))
							{
								// Set the sample rate.
								if (params.audio.sampleRate > 0)
									hr = pAudioType2->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, params.audio.sampleRate);
								else
								{
									hr = CopyAttributeSource(pAudioType, pAudioType2, MF_MT_AUDIO_SAMPLES_PER_SECOND);
									if (SUCCEEDED(hr))
									{
										UINT32 sampleRate;

										// Get the default sample rate.
										hr = pAudioType2->GetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, &sampleRate);
										if (SUCCEEDED(hr))
										{
											params.audio.sampleRate = sampleRate;
										}
									}
								}
							}

							if (SUCCEEDED(hr))
							{
								// Set the channels.
								if (params.audio.channels > 0)
									hr = pAudioType2->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, params.audio.channels);
								else
								{
									hr = CopyAttributeSource(pAudioType, pAudioType2, MF_MT_AUDIO_NUM_CHANNELS);
									if (SUCCEEDED(hr))
									{
										UINT32 channels;

										// Get the default channels.
										hr = pAudioType2->GetUINT32(MF_MT_AUDIO_NUM_CHANNELS, &channels);
										if (SUCCEEDED(hr))
										{
											params.audio.channels = channels;
										}
									}
								}
							}

							if (SUCCEEDED(hr))
							{
								// Set the bits per sample.
								if (params.audio.bitsPerSample > 0)
									hr = pAudioType2->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, params.audio.bitsPerSample);
								else
								{
									hr = CopyAttributeSource(pAudioType, pAudioType2, MF_MT_AUDIO_BITS_PER_SAMPLE);
									if (SUCCEEDED(hr))
									{
										UINT32 bitsPerSample;

										// Get the default bits per sample.
										hr = pAudioType2->GetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, &bitsPerSample);
										if (SUCCEEDED(hr))
										{
											params.audio.bitsPerSample = bitsPerSample;
										}
									}
								}
							}

							if (SUCCEEDED(hr))
							{
								// Set the block align.
								if (params.audio.blockAlign > 0)
									hr = pAudioType2->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, params.audio.blockAlign);
								else
								{
									hr = CopyAttributeSource(pAudioType, pAudioType2, MF_MT_AUDIO_BLOCK_ALIGNMENT);
									if (SUCCEEDED(hr))
									{
										UINT32 blockAlign;

										// Get the default block align.
										hr = pAudioType2->GetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, &blockAlign);
										if (SUCCEEDED(hr))
										{
											params.audio.blockAlign = blockAlign;
										}
									}
								}
							}

							if (SUCCEEDED(hr))
							{
								// Set the bytes per second.
								if (params.audio.bytesPerSecond > 0)
									hr = pAudioType2->SetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, params.audio.bytesPerSecond);
								else
								{
									hr = CopyAttributeSource(pAudioType, pAudioType2, MF_MT_AUDIO_AVG_BYTES_PER_SECOND);
									if (SUCCEEDED(hr))
									{
										UINT32 bytesPerSecond;

										// Get the default bytes per second.
										hr = pAudioType2->GetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, &bytesPerSecond);
										if (SUCCEEDED(hr))
										{
											params.audio.bytesPerSecond = bytesPerSecond;
										}
									}
								}
							}
						}

						if (SUCCEEDED(hr))
						{
							// Write to the stream.
							hr = pWriter->AddStream(pAudioType2, pdwAudioStreamIndex);
						}
					}
				}
				else
				{
					// Failed.
					hr = ((HRESULT)-1L);
				}

				// Safe release.
				SafeRelease(&pVideoType2);
				SafeRelease(&pAudioType2);
				SafeRelease(&pAvailableTypes);
				SafeRelease(&pAttributes);

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
			HRESULT CopyAttributeSource(IMFAttributes *pSrc, IMFAttributes *pDest, const GUID& key)
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