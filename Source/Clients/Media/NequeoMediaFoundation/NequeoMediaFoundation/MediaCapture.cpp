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

#include "stdafx.h"

#include "MediaCapture.h"
#include "MediaByteStream.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			HRESULT ConfigureVideoSourceReader(IMFSourceReader*);
			HRESULT ConfigureVideoEncoder(EncodingParameters&, IMFMediaType*, IMFSinkWriter*, DWORD*);
			HRESULT ConfigureAudioSourceReader(IMFSourceReader*);
			HRESULT ConfigureAudioEncoder(EncodingParameters&, IMFMediaType*, IMFSinkWriter*, DWORD*);
			HRESULT ConfigureVideoAudioSourceReader(IMFSourceReader*, IMFSourceReader*, EncodingParameters&);
			HRESULT ConfigureVideoAudioEncoder(EncodingParameters&, IMFMediaType*, IMFMediaType*, IMFSinkWriter*, DWORD*, DWORD*);
			HRESULT CopyAttribute(IMFAttributes*, IMFAttributes*, const GUID&);
			

			// Gets an interface pointer from a Media Foundation collection.
			template <class IFACE>
			HRESULT GetCollectionObject(IMFCollection *pCollection, DWORD index, IFACE **ppObject)
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
			MediaCapture::MediaCapture(HWND hwnd, HWND hEvent, HRESULT &hr) :
				_hwndApp(hwnd), 
				_hwndEvent(hEvent), 
				_videoDevice(NULL), 
				_audioDevice(NULL),
				_nRefCount(1),
				_hNotifyStateEvent(NULL),
				_hNotifyErrorEvent(NULL),
				_captureState(CaptureNotReady),
				_pReader(NULL),
				_pReaderVideo(NULL),
				_pReaderAudio(NULL),
				_pVideoWriter(NULL),
				_pAudioWriter(NULL),
				_pVideoAudioWriter(NULL),
				_bFirstSample(FALSE),
				_llBaseTime(0),
				_pwszVideoSymbolicLink(NULL),
				_pwszAudioSymbolicLink(NULL),
				_hasVideoCapture(false),
				_hasAudioCapture(false),
				_hCloseEvent(NULL),
				_streamIndexVideo(0),
				_streamIndexAudio(0),
				_pSourceReaderVideo(NULL),
				_pSourceReaderAudio(NULL),
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

					// Stop capture.
					StopCapture();
					
					// Delete critical section.
					DeleteCriticalSection(&_critsec);
				}
			}

			/// <summary>
			/// Get the video capture devices.
			/// </summary>
			/// <param name="param">The capture device param.</param>
			void MediaCapture::GetVideoCaptureDevices(CaptureDeviceParam *param)
			{
				HRESULT hr = S_OK;
				IMFAttributes *pAttributes = NULL;

				// Initialize an attribute store to specify enumeration parameters.
				hr = MFCreateAttributes(&pAttributes, 1);

				// Ask for source type = video capture devices
				if (SUCCEEDED(hr))
				{
					// Set the device attribute.
					hr = pAttributes->SetGUID(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
					);
				}

				// Enumerate devices.
				if (SUCCEEDED(hr))
				{
					// Enumerate the device list.
					hr = MFEnumDeviceSources(pAttributes, &(*param).ppDevices, &(*param).count);
				}

				// Safe release.
				SafeRelease(&pAttributes);
			}

			/// <summary>
			/// Get the audio capture devices.
			/// </summary>
			/// <param name="param">The capture device param.</param>
			void MediaCapture::GetAudioCaptureDevices(CaptureDeviceParam *param)
			{
				HRESULT hr = S_OK;
				IMFAttributes *pAttributes = NULL;

				// Initialize an attribute store to specify enumeration parameters.
				hr = MFCreateAttributes(&pAttributes, 1);

				// Ask for source type = video capture devices
				if (SUCCEEDED(hr))
				{
					// Set the device attribute.
					hr = pAttributes->SetGUID(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID
					);
				}

				// Enumerate devices.
				if (SUCCEEDED(hr))
				{
					// Enumerate the device list.
					hr = MFEnumDeviceSources(pAttributes, &(*param).ppDevices, &(*param).count);
				}

				// Safe release.
				SafeRelease(&pAttributes);
			}

			/// <summary>
			/// Safely release all capture devices.
			/// </summary>
			/// <param name="param">The capture device param.</param>
			void MediaCapture::SafeReleaseCaptureDevices(CaptureDeviceParam *param)
			{
				// For each capture device.
				for (DWORD i = 0; i < param->count; i++)
				{
					// Release.
					SafeRelease(&param->ppDevices[i]);
				}

				// Release the device pointer.
				CoTaskMemFree(param->ppDevices);
			}

			/// <summary>
			/// Set the video capture device.
			/// </summary>
			/// <param name="device">The media foundation device.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::SetVideoDevice(IMFActivate *device)
			{
				HRESULT hr = S_OK;

				// Has video.
				if (device != NULL)
				{
					_videoDevice = device;
					_hasVideoCapture = true;
				}
				else
				{
					_videoDevice = NULL;
					_hasVideoCapture = false;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Set the audio capture device.
			/// </summary>
			/// <param name="device">The media foundation device.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::SetAudioDevice(IMFActivate *device)
			{
				HRESULT hr = S_OK;

				// Has audio.
				if (device != NULL)
				{
					_audioDevice = device;
					_hasAudioCapture = true;
				}
				else
				{
					_audioDevice = NULL;
					_hasAudioCapture = false;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Get the device name.
			/// </summary>
			/// <param name="device">The media foundation device.</param>
			/// <returns>The device name.</returns>
			std::wstring MediaCapture::GetDeviceName(IMFActivate *device)
			{
				HRESULT hr = S_OK;

				std::wstring name;
				WCHAR *pDeviceName = NULL;
				UINT32 namehLength = 0;

				// Get the name length.
				hr = device->GetStringLength(MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, &namehLength);

				if (SUCCEEDED(hr))
				{
					// Create char.
					pDeviceName = new WCHAR[namehLength + 1];
					if (pDeviceName == NULL)
					{
						hr = E_OUTOFMEMORY;
					}
				}

				if (SUCCEEDED(hr))
				{
					// Get the device name.
					hr = device->GetString(
						MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, pDeviceName, namehLength + 1, &namehLength);

					// If the name was returned.
					if (SUCCEEDED(hr))
					{
						// Assign the name.
						name = pDeviceName;
					}
				}

				// if created.
				if (pDeviceName)
				{
					// Delete.
					delete[] pDeviceName;
				}

				// Return the result.
				return name;
			}

			/// <summary>
			/// Get the device names.
			/// </summary>
			/// <param name="param">The media foundation device collection.</param>
			/// <returns>The device names.</returns>
			std::vector<std::wstring> MediaCapture::GetDeviceNames(CaptureDeviceParam *param)
			{
				std::vector<std::wstring> names;

				// For each device.
				for (size_t i = 0; i < param->count; i++)
				{
					// Get the name.
					std::wstring name = GetDeviceName(param->ppDevices[i]);

					// If name exists.
					if (name.length() > 0)
					{
						// Add the name.
						names.push_back(name);
					}
				}

				// Return the names.
				return names;
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

				// Attach MediaCapture to the interface.
				static const QITAB qit[] =
				{
					QITABENT(MediaCapture, IMFSourceReaderCallback),
					{ 0 },
				};
				return QISearch(this, qit, iid, ppv);
			}

			/// <summary>
			/// Is capturing.
			/// </summary>
			/// <returns>True if capturing; else false.</returns>
			BOOL MediaCapture::IsCapturing()
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);
				BOOL bIsCapturing = FALSE;

				// Has video and audio
				if (_hasVideoCapture && _hasAudioCapture)
				{
					// If the write sink exists
					// then capturing samples.
					bIsCapturing = (_pVideoAudioWriter != NULL);
				}
				// Has video capture.
				else if (_hasVideoCapture)
				{
					// If the write sink exists
					// then capturing samples.
					bIsCapturing = (_pVideoWriter != NULL);
				}
				// Has audio capture.
				else if (_hasAudioCapture)
				{
					// If the write sink exists
					// then capturing samples.
					bIsCapturing = (_pAudioWriter != NULL);
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return is capturing.
				return bIsCapturing;
			}

			/// <summary>
			/// Stop capture.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::StopCapture()
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// If is capturing.
				if (IsCapturing())
				{
					// End the capture session.
					EndCaptureSession();

					// If capture has started.
					if (_started)
					{
						// Shutdown the Media Foundation platform
						MFShutdown();
						_started = false;

						// Close the close event handler.
						CloseHandle(_hCloseEvent);
					}

					// Has video and audio
					if (_hasVideoCapture && _hasAudioCapture)
					{
						// Stop capturing.
						_captureState = NotCapturing;
						NotifyState();
					}
					// Has video capture.
					else if (_hasVideoCapture)
					{
						// Stop capturing.
						_captureState = NotCapturingVideo;
						NotifyState();
					}
					// Has audio capture.
					else if (_hasAudioCapture)
					{
						// Stop capturing.
						_captureState = NotCapturingAudio;
						NotifyState();
					}
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);
				
				// Return the result.
				return hr;
			}

			/// <summary>
			/// Start capture to file.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::StartCaptureToFile(const WCHAR *pwszFileName, EncodingParameters& param)
			{
				HRESULT hr = S_OK;
				_captureState = NotCapturing;

				// If capturing video and audio.
				if (_hasVideoCapture && _hasAudioCapture)
				{
					// If not capturing.
					if (_captureState != Capturing)
					{
						_captureState = Capturing;

						// Start video and audio capture.
						hr = StartVideoAudioCapture(pwszFileName, NULL, param, true);
					}
				}
				else if (_hasVideoCapture)
				{
					// If not capturing.
					if (_captureState != CapturingVideo)
					{
						_captureState = CapturingVideo;

						// Start video only capture.
						hr = StartVideoCapture(pwszFileName, NULL, param, true);
					}
				}
				else if (_hasAudioCapture)
				{
					// If not capturing.
					if (_captureState != CapturingAudio)
					{
						_captureState = CapturingAudio;

						// Start audio only capture.
						hr = StartAudioCapture(pwszFileName, NULL, param, true);
					}
				}
				else
				{
					// Not start notify error.
					hr = -1;
					NotifyError(hr);
				}

				// Start capturing.
				if (SUCCEEDED(hr))
				{
					// Send the notification.
					NotifyState();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Start capture to stream.
			/// </summary>
			/// <param name="pByteStream">The byte stream to write the capture data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::StartCaptureToStream(IMFByteStream *pByteStream, EncodingParameters& param)
			{
				HRESULT hr = S_OK;
				_captureState = NotCapturing;

				// If capturing video and audio.
				if (_hasVideoCapture && _hasAudioCapture)
				{
					// If not capturing.
					if (_captureState != Capturing)
					{
						_captureState = Capturing;

						// Start video and audio capture.
						hr = StartVideoAudioCapture(NULL, pByteStream, param, false);
					}
				}
				else if (_hasVideoCapture)
				{
					// If not capturing.
					if (_captureState != CapturingVideo)
					{
						_captureState = CapturingVideo;

						// Start video only capture.
						hr = StartVideoCapture(NULL, pByteStream, param, false);
					}
				}
				else if (_hasAudioCapture)
				{
					// If not capturing.
					if (_captureState != CapturingAudio)
					{
						_captureState = CapturingAudio;

						// Start audio only capture.
						hr = StartAudioCapture(NULL, pByteStream, param, false);
					}
				}
				else
				{
					// Not start notify error.
					hr = -1;
					NotifyError(hr);
				}

				// Start capturing.
				if (SUCCEEDED(hr))
				{
					// Send the notification.
					NotifyState();
				}
				
				/* Could be usefull later.
				IMFSourceResolver::CreateObjectFromByteStream();
				IMFByteStream* spMFByteStream = NULL;
				MFCreateMFByteStreamOnStreamEx((IUnknown*)streamHandle, &spMFByteStream);
				IMFSourceReader* _sourceReader = NULL;
				MFCreateSourceReaderFromByteStream(spMFByteStream, nullptr, &_sourceReader);
				
				IMFSourceResolver* resolver;
				HRESULT hr;
				hr = MFCreateSourceResolver(&resolver);
				*/

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Start video capture to file.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
			/// <param name="pByteStream">The byte stream to write the capture data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <param name="writeToFile">Write to file; else write to byte stream.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::StartVideoCapture(const WCHAR *pwszFileName, IMFByteStream *pByteStream, EncodingParameters& param, bool writeToFile)
			{
				HRESULT hr = S_OK;

				IMFAttributes *pAttributesByteStream = NULL;
				IMFMediaSource *pSource = NULL;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// Initializes the media foundation.
				hr = Initialize();

				// Create the media source for the device.
				hr = _videoDevice->ActivateObject(
					__uuidof(IMFMediaSource),
					(void**)&pSource
				);

				// Get the symbolic link. This is needed to handle device-
				// loss notifications.
				if (SUCCEEDED(hr))
				{
					// Get the name of the video symbolic link.
					hr = _videoDevice->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK,
						&_pwszVideoSymbolicLink,
						NULL
					);
				}

				if (SUCCEEDED(hr))
				{
					// Open media source.
					hr = OpenMediaSource(pSource);
				}

				// Create the sink writer 
				if (SUCCEEDED(hr))
				{
					// If writing to file.
					if (writeToFile)
					{
						// Create sink writer from URL.
						hr = MFCreateSinkWriterFromURL(
							pwszFileName,
							NULL,
							NULL,
							&_pVideoWriter
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
								param.video.transcode
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
								&_pVideoWriter
							);
						}
					}
				}

				// Set up the encoding parameters.
				if (SUCCEEDED(hr))
				{
					// Configure capture.
					hr = ConfigureVideoCapture(param);
				}

				if (SUCCEEDED(hr))
				{
					_bFirstSample = TRUE;
					_llBaseTime = 0;

					// Request the first video frame.
					hr = _pReader->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						0,
						NULL,
						NULL,
						NULL,
						NULL
					);
				}

				// Safe release.
				SafeRelease(&pAttributesByteStream);
				SafeRelease(&pSource);

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Start audio capture to file.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
			/// <param name="pByteStream">The byte stream to write the capture data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <param name="writeToFile">Write to file; else write to byte stream.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::StartAudioCapture(const WCHAR *pwszFileName, IMFByteStream *pByteStream, EncodingParameters& param, bool writeToFile)
			{
				HRESULT hr = S_OK;

				IMFAttributes *pAttributes = NULL;
				IMFAttributes *pAttributesByteStream = NULL;
				IMFMediaSource *pSource = NULL;

				// Get the name of the device.
				std::wstring name = GetDeviceName(_audioDevice);
				WCHAR* ednpointID;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// Initializes the media foundation.
				hr = Initialize();

				// Get this audio device media source.
				hr = MFCreateAttributes(&pAttributes, 2);
				
				// Set the device type to audio.
				if (SUCCEEDED(hr))
				{
					hr = pAttributes->SetGUID(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID
					);
				}

				// Set the endpoint ID.
				if (SUCCEEDED(hr))
				{
					// Get the endpoint id.
					hr = _audioDevice->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID,
						&ednpointID,
						NULL
					);

					// Set the endpoint id.
					hr = pAttributes->SetString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID,
						ednpointID
					);
				}

				if (SUCCEEDED(hr))
				{
					// Create the media source.
					hr = MFCreateDeviceSource(pAttributes, &pSource);
				}

				// Get the symbolic link. This is needed to handle device-
				// loss notifications.
				if (SUCCEEDED(hr))
				{
					// Get the name of the audio symbolic link.
					hr = _audioDevice->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK,
						&_pwszAudioSymbolicLink,
						NULL
					);
				}

				if (SUCCEEDED(hr))
				{
					// Open media source.
					hr = OpenMediaSource(pSource);
				}

				// Create the sink writer 
				if (SUCCEEDED(hr))
				{
					// If writing to file.
					if (writeToFile)
					{
						// Create sink writer from URL.
						hr = MFCreateSinkWriterFromURL(
							pwszFileName,
							NULL,
							NULL,
							&_pAudioWriter
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
								param.audio.transcode
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
								&_pAudioWriter
							);
						}
					}
				}

				// Set up the encoding parameters.
				if (SUCCEEDED(hr))
				{
					// Configure capture.
					hr = ConfigureAudioCapture(param);
				}

				if (SUCCEEDED(hr))
				{
					_bFirstSample = TRUE;
					_llBaseTime = 0;

					// Request the first audio frame.
					hr = _pReader->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						0,
						NULL,
						NULL,
						NULL,
						NULL
					);
				}

				// Safe release.
				SafeRelease(&pAttributesByteStream);
				SafeRelease(&pAttributes);
				SafeRelease(&pSource);

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Start video and audio capture.
			/// </summary>
			/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
			/// <param name="pByteStream">The byte stream to write the capture data to.</param>
			/// <param name="param">The encoding parameters.</param>
			/// <param name="writeToFile">Write to file; else write to byte stream.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::StartVideoAudioCapture(const WCHAR *pwszFileName, IMFByteStream *pByteStream, EncodingParameters& param, bool writeToFile)
			{
				HRESULT hr = S_OK;

				IMFCollection *pCollection = NULL;
				IMFAttributes *pAttributes = NULL;
				IMFAttributes *pAttributesByteStream = NULL;
				IMFMediaSource *pVideoSource = NULL;
				IMFMediaSource *pAudioSource = NULL;
				IMFMediaSource *pSource = NULL;

				// Get the name of the device.
				std::wstring name = GetDeviceName(_audioDevice);
				WCHAR* ednpointID;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// Initializes the media foundation.
				hr = Initialize();

				// Get this audio device media source.
				hr = MFCreateAttributes(&pAttributes, 2);

				// Create the media source for the device.
				hr = _videoDevice->ActivateObject(
					__uuidof(IMFMediaSource),
					(void**)&pVideoSource
				);

				// Get the symbolic link. This is needed to handle device-
				// loss notifications.
				if (SUCCEEDED(hr))
				{
					// Get the name of the video symbolic link.
					hr = _videoDevice->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK,
						&_pwszVideoSymbolicLink,
						NULL
					);
				}

				// Set the device type to audio.
				if (SUCCEEDED(hr))
				{
					hr = pAttributes->SetGUID(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID
					);
				}

				// Set the endpoint ID.
				if (SUCCEEDED(hr))
				{
					// Get the endpoint id.
					hr = _audioDevice->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID,
						&ednpointID,
						NULL
					);

					// Set the endpoint id.
					hr = pAttributes->SetString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID,
						ednpointID
					);
				}

				if (SUCCEEDED(hr))
				{
					// Create the media source.
					hr = MFCreateDeviceSource(pAttributes, &pAudioSource);
				}

				// Get the symbolic link. This is needed to handle device-
				// loss notifications.
				if (SUCCEEDED(hr))
				{
					// Get the name of the audio symbolic link.
					hr = _audioDevice->GetAllocatedString(
						MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK,
						&_pwszAudioSymbolicLink,
						NULL
					);
				}

				// Create the sink writer 
				if (SUCCEEDED(hr))
				{
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
							&_pVideoAudioWriter
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
								&_pVideoAudioWriter
							);
						}
					}
				}

				/*
				if (SUCCEEDED(hr))
				{
				// Create a video and audio collection.
				hr = MFCreateCollection(&pCollection);

				if (SUCCEEDED(hr))
				// Add the video source to the collection.
				hr = pCollection->AddElement(pVideoSource);

				if (SUCCEEDED(hr))
				// Add the audio source to the collection.
				hr = pCollection->AddElement(pAudioSource);

				if (SUCCEEDED(hr))
				// Create the video and audio combined collection.
				hr = MFCreateAggregateSource(pCollection, &pSource);
				}
				*/

				if (SUCCEEDED(hr))
				{
					// Open media source.
					hr = OpenMediaSourceVideoAudio(pVideoSource, pAudioSource);
				}


				// Set up the encoding parameters.
				if (SUCCEEDED(hr))
				{
					// Configure capture.
					hr = ConfigureVideoAudioCapture(param);
				}

				if (SUCCEEDED(hr))
				{
					_bFirstSample = TRUE;
					_llBaseTime = 0;

					// Request the first video frame.
					hr = _pReaderVideo->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						0,
						NULL,
						NULL,
						NULL,
						NULL
					);

					if (SUCCEEDED(hr))
					{
						// Request the first audio frame.
						hr = _pReaderAudio->ReadSample(
							(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
							0,
							NULL,
							NULL,
							NULL,
							NULL
						);
					}
				}

				// Safe release.
				SafeRelease(&pAttributesByteStream);
				SafeRelease(&pAttributes);
				SafeRelease(&pVideoSource);
				SafeRelease(&pAudioSource);
				SafeRelease(&pCollection);
				SafeRelease(&pSource);

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initializes the media foundation.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::Initialize()
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
					_started = true;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Check device lost.
			/// </summary>
			/// <param name="pHdr">The device broad cast hander.</param>
			/// <param name="pbDeviceLost">Is device lost.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::CheckDeviceLost(DEV_BROADCAST_HDR *pHdr, BOOL *pbDeviceLost)
			{
				// If lost device has reference.
				if (pbDeviceLost == NULL)
				{
					return E_POINTER;
				}

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				DEV_BROADCAST_DEVICEINTERFACE *pDi = NULL;

				// Initial false.
				*pbDeviceLost = FALSE;

				// If not capturing.
				if (!IsCapturing())
				{
					goto done;
				}

				// If broad cast handler null.
				if (pHdr == NULL)
				{
					goto done;
				}

				// If device type is not interface.
				if (pHdr->dbch_devicetype != DBT_DEVTYP_DEVICEINTERFACE)
				{
					goto done;
				}

				// Compare the device name with the symbolic link.
				pDi = (DEV_BROADCAST_DEVICEINTERFACE*)pHdr;

				// Has video capture.
				if (_hasVideoCapture)
				{
					// If symbolc link.
					if (_pwszVideoSymbolicLink)
					{
						// Compare the values. If the vlaues are equal.
						if (_wcsicmp(_pwszVideoSymbolicLink, pDi->dbcc_name) == 0)
						{
							// Device lost true.
							*pbDeviceLost = TRUE;
						}
					}
				}

				// Has audio capture.
				if (_hasAudioCapture)
				{
					// If symbolc link.
					if (_pwszAudioSymbolicLink)
					{
						// Compare the values. If the vlaues are equal.
						if (_wcsicmp(_pwszAudioSymbolicLink, pDi->dbcc_name) == 0)
						{
							// Device lost true.
							*pbDeviceLost = TRUE;
						}
					}
				}

			done:
				// Leave critical section.
				LeaveCriticalSection(&_critsec);
				return S_OK;
			}

			/// <summary>
			/// End capture session.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::EndCaptureSession()
			{
				// Enter critical section.
				EnterCriticalSection(&_critsec);

				HRESULT hr = S_OK;

				// If has video and audio.
				if (_hasVideoCapture && _hasAudioCapture)
				{
					// If the writer exists. 
					if (_pVideoAudioWriter)
					{
						// Finalise the writer.
						hr = _pVideoAudioWriter->Finalize();
					}
				}
				// Has video capture.
				else if (_hasVideoCapture)
				{
					// If the writer exists. 
					if (_pVideoWriter)
					{
						// Finalise the writer.
						hr = _pVideoWriter->Finalize();
					}
				}
				// Has audio capture.
				else if (_hasAudioCapture)
				{
					// If the writer exists. 
					if (_pAudioWriter)
					{
						// Finalise the writer.
						hr = _pAudioWriter->Finalize();
					}
				}

				// Safe release.
				SafeRelease(&_pVideoAudioWriter);
				SafeRelease(&_pVideoWriter);
				SafeRelease(&_pAudioWriter);
				SafeRelease(&_pReader);
				SafeRelease(&_pReaderVideo);
				SafeRelease(&_pReaderAudio);

				// If the source reader is not null.
				if (_pSourceReaderVideo != NULL)
				{
					// Release the source reader.
					_pSourceReaderVideo->Release();
					_pSourceReaderVideo = NULL;
				}

				// If the source reader is not null.
				if (_pSourceReaderAudio != NULL)
				{
					// Release the source reader.
					_pSourceReaderAudio->Release();
					_pSourceReaderAudio = NULL;
				}

				// Close;
				EndCaptureInternal();

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
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

				// If not capturing samples.
				if (!IsCapturing())
				{
					// Leave critical section.
					LeaveCriticalSection(&_critsec);
					return hr;
				}

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
					llTimestamp -= _llBaseTime;

					// Set the sample time stamp.
					hr = pSample->SetSampleTime(llTimestamp);

					// If failed.
					if (FAILED(hr)) { goto done; }

					// Select the stream index.
					switch (dwStreamIndex)
					{
					case (DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM:
						// Write the sample to the sink.
						hr = _pVideoWriter->WriteSample(0, pSample);
						break;

					case (DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM:
						// Write the sample to the sink.
						hr = _pAudioWriter->WriteSample(0, pSample);
						break;

					default:
						// If capturing video and audio.
						if (_hasVideoCapture && _hasAudioCapture)
						{
							// Write the sample.
							hr = _pVideoAudioWriter->WriteSample(dwStreamIndex, pSample);
						}
						else if (_hasVideoCapture)
						{
							// Write the sample to the sink.
							hr = _pVideoWriter->WriteSample(0, pSample);
						}
						else if (_hasAudioCapture)
						{
							// Write the sample to the sink.
							hr = _pAudioWriter->WriteSample(0, pSample);
						}
						break;
					}

					// If failed.
					if (FAILED(hr)) { goto done; }
				}

				// If capturing video and audio.
				if (_hasVideoCapture && _hasAudioCapture)
				{
					// Read another video and audio sample.
					hr = _pReaderVideo->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						0,
						NULL,   // actual
						NULL,   // flags
						NULL,   // timestamp
						NULL    // sample
					);

					// Read another video and audio sample.
					hr = _pReaderAudio->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						0,
						NULL,   // actual
						NULL,   // flags
						NULL,   // timestamp
						NULL    // sample
					);
				}
				else if (_hasVideoCapture)
				{
					// Read another video sample.
					hr = _pReader->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						0,
						NULL,   // actual
						NULL,   // flags
						NULL,   // timestamp
						NULL    // sample
					);
				}
				else if (_hasAudioCapture)
				{
					// Read another audio sample.
					hr = _pReader->ReadSample(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						0,
						NULL,   // actual
						NULL,   // flags
						NULL,   // timestamp
						NULL    // sample
					);
				}

			done:
				// If failed.
				if (FAILED(hr))
				{
					// Send a error notification.
					NotifyError(hr);
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// On event read sample override.
			/// </summary>
			/// <param name="hrStatus">The read status.</param>
			/// <param name="dwStreamIndex">The stream index number.</param>
			/// <param name="dwStreamFlags">The stream flag.</param>
			/// <param name="llTimestamp">The current time stamp.</param>
			/// <param name="pSample">The captured sample data.</param>
			/// <param name="llRebaseTimestamp">The rebased time stamp.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::VideoSampleComplete(
				HRESULT hrStatus,
				DWORD dwStreamIndex,
				DWORD dwStreamFlags,
				LONGLONG llTimestamp,
				IMFSample *pSample,
				LONGLONG llRebaseTimestamp)
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// If not capturing samples.
				if (!IsCapturing())
				{
					// Leave critical section.
					LeaveCriticalSection(&_critsec);
					return hr;
				}

				// If failed.
				if (FAILED(hrStatus))
				{
					hr = hrStatus;
					goto done;
				}

				// If a sample exists.
				if (pSample)
				{
					// Write the sample.
					hr = _pVideoAudioWriter->WriteSample(_streamIndexVideo, pSample);
				}

				// If failed.
				if (FAILED(hr)) { goto done; }

				// Read another video and audio sample.
				hr = _pReaderVideo->ReadSample(
					(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
					0,
					NULL,   // actual
					NULL,   // flags
					NULL,   // timestamp
					NULL    // sample
				);

			done:
				// If failed.
				if (FAILED(hr))
				{
					// Send a error notification.
					NotifyError(hr);
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// On event read sample override.
			/// </summary>
			/// <param name="hrStatus">The read status.</param>
			/// <param name="dwStreamIndex">The stream index number.</param>
			/// <param name="dwStreamFlags">The stream flag.</param>
			/// <param name="llTimestamp">The current time stamp.</param>
			/// <param name="pSample">The captured sample data.</param>
			/// <param name="llRebaseTimestamp">The rebased time stamp.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::AudioSampleComplete(
				HRESULT hrStatus,
				DWORD dwStreamIndex,
				DWORD dwStreamFlags,
				LONGLONG llTimestamp,
				IMFSample *pSample,
				LONGLONG llRebaseTimestamp)
			{
				HRESULT hr = S_OK;

				// Enter critical section.
				EnterCriticalSection(&_critsec);

				// If not capturing samples.
				if (!IsCapturing())
				{
					// Leave critical section.
					LeaveCriticalSection(&_critsec);
					return hr;
				}

				// If failed.
				if (FAILED(hrStatus))
				{
					hr = hrStatus;
					goto done;
				}

				// If a sample exists.
				if (pSample)
				{
					// Write the sample.
					hr = _pVideoAudioWriter->WriteSample(_streamIndexAudio, pSample);
				}

				// If failed.
				if (FAILED(hr)) { goto done; }

				// Read another video and audio sample.
				hr = _pReaderAudio->ReadSample(
					(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
					0,
					NULL,   // actual
					NULL,   // flags
					NULL,   // timestamp
					NULL    // sample
				);

			done:
				// If failed.
				if (FAILED(hr))
				{
					// Send a error notification.
					NotifyError(hr);
				}

				// Leave critical section.
				LeaveCriticalSection(&_critsec);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open media source.
			/// </summary>
			/// <param name="pSource">The media source.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::OpenMediaSource(IMFMediaSource *pSource)
			{
				HRESULT hr = S_OK;

				IMFAttributes *pAttributes = NULL;

				// Get the attributes.
				hr = MFCreateAttributes(&pAttributes, 2);

				if (SUCCEEDED(hr))
				{
					// Set the reader aync callback
					// is indicates that the read will be 
					// sampling in async mode.
					hr = pAttributes->SetUnknown(MF_SOURCE_READER_ASYNC_CALLBACK, this);
				}

				if (SUCCEEDED(hr))
				{
					// Create a new source reader.
					hr = MFCreateSourceReaderFromMediaSource(
						pSource,
						pAttributes,
						&_pReader
					);
				}

				// Safe release.
				SafeRelease(&pAttributes);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Open media source.
			/// </summary>
			/// <param name="pSourceVideo">The media source.</param>
			/// <param name="pSourceAudio">The media source.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::OpenMediaSourceVideoAudio(IMFMediaSource *pSourceVideo, IMFMediaSource *pSourceAudio)
			{
				HRESULT hr = S_OK;

				IMFAttributes *pAttributesVideo = NULL;
				IMFAttributes *pAttributesAudio = NULL;

				// Get the attributes.
				hr = MFCreateAttributes(&pAttributesVideo, 2);

				if (SUCCEEDED(hr))
				{
					// Create source reader.
					hr = SourceReader::CreateInstance(&_pSourceReaderVideo);

					if (SUCCEEDED(hr))
					{
						// Set the read sample callback.
						_pSourceReaderVideo->SetReadSampleCompleteHandler(
							[this](HRESULT hrStatus, DWORD dwStreamIndex, DWORD dwStreamFlags, LONGLONG llTimestamp, IMFSample *pSample, LONGLONG llRebaseTimestamp) ->HRESULT
						{
							return this->VideoSampleComplete(hrStatus, dwStreamIndex, dwStreamFlags, llTimestamp, pSample, llRebaseTimestamp);
						});

						// Set the reader aync callback
						// is indicates that the read will be 
						// sampling in async mode.
						hr = pAttributesVideo->SetUnknown(MF_SOURCE_READER_ASYNC_CALLBACK, _pSourceReaderVideo);
					}
				}

				if (SUCCEEDED(hr))
				{
					// Create a new source reader.
					hr = MFCreateSourceReaderFromMediaSource(
						pSourceVideo,
						pAttributesVideo,
						&_pReaderVideo
					);
				}

				if (SUCCEEDED(hr))
					// Get the attributes.
					hr = MFCreateAttributes(&pAttributesAudio, 2);

				if (SUCCEEDED(hr))
				{
					// Create source reader.
					hr = SourceReader::CreateInstance(&_pSourceReaderAudio);

					if (SUCCEEDED(hr))
					{
						// Set the sample reader.
						_pSourceReaderAudio->SetReadSampleCompleteHandler(
							[this](HRESULT hrStatus, DWORD dwStreamIndex, DWORD dwStreamFlags, LONGLONG llTimestamp, IMFSample *pSample, LONGLONG llRebaseTimestamp) ->HRESULT
						{
							return this->AudioSampleComplete(hrStatus, dwStreamIndex, dwStreamFlags, llTimestamp, pSample, llRebaseTimestamp);
						});

						// Set the reader aync callback
						// is indicates that the read will be 
						// sampling in async mode.
						hr = pAttributesAudio->SetUnknown(MF_SOURCE_READER_ASYNC_CALLBACK, _pSourceReaderAudio);
					}
				}

				if (SUCCEEDED(hr))
				{
					// Create a new source reader.
					hr = MFCreateSourceReaderFromMediaSource(
						pSourceAudio,
						pAttributesAudio,
						&_pReaderAudio
					);
				}

				// Safe release.
				SafeRelease(&pAttributesVideo);
				SafeRelease(&pAttributesAudio);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure video capture encoding.
			/// </summary>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::ConfigureVideoCapture(EncodingParameters& param)
			{
				HRESULT hr = S_OK;
				DWORD sink_stream = 0;

				IMFMediaType *pType = NULL;

				// Configure source reader.
				hr = ConfigureVideoSourceReader(_pReader);

				if (SUCCEEDED(hr))
				{
					// Get current media type.
					hr = _pReader->GetCurrentMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						&pType
					);
				}

				if (SUCCEEDED(hr))
				{
					// Configure the encoder.
					hr = ConfigureVideoEncoder(param, pType, _pVideoWriter, &sink_stream);
				}

				if (SUCCEEDED(hr))
				{
					// Register the color converter DSP for this process, in the video 
					// processor category. This will enable the sink writer to enumerate
					// the color converter when the sink writer attempts to match the
					// media types.
					/*hr = MFTRegisterLocalByCLSID(
						__uuidof(CColorConvertDMO),
						MFT_CATEGORY_VIDEO_PROCESSOR,
						L"",
						MFT_ENUM_FLAG_SYNCMFT,
						0,
						NULL,
						0,
						NULL
					);*/
				}

				if (SUCCEEDED(hr))
				{
					// Set the input media type.
					hr = _pVideoWriter->SetInputMediaType(sink_stream, pType, NULL);
				}

				if (SUCCEEDED(hr))
				{
					// Begin writing.
					hr = _pVideoWriter->BeginWriting();
				}

				// Safe release.
				SafeRelease(&pType);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure audio capture encoding.
			/// </summary>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::ConfigureAudioCapture(EncodingParameters& param)
			{
				HRESULT hr = S_OK;
				DWORD sink_stream = 0;

				IMFMediaType *pType = NULL;

				// Configure source reader.
				hr = ConfigureAudioSourceReader(_pReader);

				if (SUCCEEDED(hr))
				{
					// Get current media type.
					hr = _pReader->GetCurrentMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						&pType
					);
				}

				if (SUCCEEDED(hr))
				{
					// Configure the encoder.
					hr = ConfigureAudioEncoder(param, pType, _pAudioWriter, &sink_stream);
				}

				if (SUCCEEDED(hr))
				{
					// Register the color converter DSP for this process, in the video 
					// processor category. This will enable the sink writer to enumerate
					// the color converter when the sink writer attempts to match the
					// media types.
					/*hr = MFTRegisterLocalByCLSID(
						__uuidof(CResamplerMediaObject),
						MFT_CATEGORY_AUDIO_ENCODER,
						L"",
						MFT_ENUM_FLAG_SYNCMFT,
						0,
						NULL,
						0,
						NULL
					);*/
				}

				if (SUCCEEDED(hr))
				{
					// Set the input media type.
					hr = _pAudioWriter->SetInputMediaType(sink_stream, pType, NULL);
				}

				if (SUCCEEDED(hr))
				{
					// Begin writing.
					hr = _pAudioWriter->BeginWriting();
				}

				// Safe release.
				SafeRelease(&pType);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure video and audio capture encoding.
			/// </summary>
			/// <param name="param">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::ConfigureVideoAudioCapture(EncodingParameters& param)
			{
				HRESULT hr = S_OK;
				DWORD sink_stream_video = 0;
				DWORD sink_stream_audio = 0;

				IMFMediaType *pVideoType = NULL;
				IMFMediaType *pAudioType = NULL;

				// Configure source reader.
				hr = ConfigureVideoAudioSourceReader(_pReaderVideo, _pReaderAudio, param);

				if (SUCCEEDED(hr))
				{
					// Get current media type.
					hr = _pReaderVideo->GetCurrentMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
						&pVideoType
					);
				}

				if (SUCCEEDED(hr))
				{
					// Get current media type.
					hr = _pReaderAudio->GetCurrentMediaType(
						(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
						&pAudioType
					);
				}

				if (SUCCEEDED(hr))
				{
					// Configure the encoder.
					hr = ConfigureVideoAudioEncoder(param, pVideoType, pAudioType, _pVideoAudioWriter, &sink_stream_video, &sink_stream_audio);
					_streamIndexVideo = sink_stream_video;
					_streamIndexAudio = sink_stream_audio;
				}

				if (SUCCEEDED(hr))
				{
					// Set the input media type.
					hr = _pVideoAudioWriter->SetInputMediaType(sink_stream_video, pVideoType, NULL);
				}

				if (SUCCEEDED(hr))
				{
					// Set the input media type.
					hr = _pVideoAudioWriter->SetInputMediaType(sink_stream_audio, pAudioType, NULL);
				}

				if (SUCCEEDED(hr))
				{
					// Begin writing.
					hr = _pVideoAudioWriter->BeginWriting();
				}

				// Safe release.
				SafeRelease(&pVideoType);
				SafeRelease(&pAudioType);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// End capture internal.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaCapture::EndCaptureInternal()
			{
				HRESULT hr = S_OK;

				// Has video capture.
				if (_hasVideoCapture)
				{
					// Memory free.
					CoTaskMemFree(_pwszVideoSymbolicLink);
					_pwszVideoSymbolicLink = NULL;
				}

				// Has audio capture.
				if (_hasAudioCapture)
				{
					// Memory free.
					CoTaskMemFree(_pwszAudioSymbolicLink);
					_pwszAudioSymbolicLink = NULL;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure video source reader.
			/// </summary>
			/// <param name="pReader">The source reader.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ConfigureVideoSourceReader(IMFSourceReader *pReader)
			{
				// The list of acceptable types.
				GUID subtypes[] = {
					MFVideoFormat_NV12, MFVideoFormat_YUY2, MFVideoFormat_UYVY, MFVideoFormat_WMV3,
					MFVideoFormat_RGB32, MFVideoFormat_RGB24, MFVideoFormat_IYUV, MFVideoFormat_H264
				};

				HRESULT hr = S_OK;
				BOOL    bUseNativeType = FALSE;
				GUID subtype = { 0 };
				IMFMediaType *pType = NULL;

				// If the source's native format matches any of the formats in 
				// the list, prefer the native format.

				// Note: The camera might support multiple output formats, 
				// including a range of frame dimensions. The application could
				// provide a list to the user and have the user select the
				// camera's output format. 

				// Get the media type.
				hr = pReader->GetNativeMediaType(
					(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
					0,  // Type index
					&pType
				);

				if (FAILED(hr)) { goto done; }

				// Get the sub type.
				hr = pType->GetGUID(MF_MT_SUBTYPE, &subtype);

				if (FAILED(hr)) { goto done; }

				// For each sub type.
				for (UINT32 i = 0; i < ARRAYSIZE(subtypes); i++)
				{
					// If sub types match.
					if (subtype == subtypes[i])
					{
						// Set the current media type.
						hr = pReader->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
							NULL,
							pType
						);

						// Found native type.
						bUseNativeType = TRUE;
						break;
					}
				}

				// If not using native types.
				if (!bUseNativeType)
				{
					// None of the native types worked. The camera might offer 
					// output a compressed type such as MJPEG or DV.

					// Try adding a decoder.
					// For each sub type.
					for (UINT32 i = 0; i < ARRAYSIZE(subtypes); i++)
					{
						// Set the sub type.
						hr = pType->SetGUID(MF_MT_SUBTYPE, subtypes[i]);

						if (FAILED(hr)) { goto done; }

						// Set the current media type.
						hr = pReader->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
							NULL,
							pType
						);

						if (SUCCEEDED(hr))
						{
							// Exit.
							break;
						}
					}
				}

			done:
				// Safe release.
				SafeRelease(&pType);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure audio source reader.
			/// </summary>
			/// <param name="pReader">The source reader.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ConfigureAudioSourceReader(IMFSourceReader* pReader)
			{
				// The list of acceptable types.
				GUID subtypes[] = {
					MFAudioFormat_Base, MFAudioFormat_PCM, MFAudioFormat_Float, MFAudioFormat_DTS, MFAudioFormat_Dolby_AC3_SPDIF, MFAudioFormat_DRM,
					MFAudioFormat_WMAudioV8, MFAudioFormat_WMAudioV9, MFAudioFormat_WMAudio_Lossless, MFAudioFormat_WMASPDIF, MFAudioFormat_MSP1,
					MFAudioFormat_MP3, MFAudioFormat_MPEG, MFAudioFormat_AAC, MFAudioFormat_ADTS, MFAudioFormat_AMR_NB, MFAudioFormat_AMR_WB,
					MFAudioFormat_AMR_WP, MFAudioFormat_Dolby_AC3, MFAudioFormat_Dolby_DDPlus
				};

				HRESULT hr = S_OK;
				BOOL    bUseNativeType = FALSE;
				GUID subtype = { 0 };
				IMFMediaType *pType = NULL;

				// If the source's native format matches any of the formats in 
				// the list, prefer the native format.

				// Note: The camera might support multiple output formats, 
				// including a range of frame dimensions. The application could
				// provide a list to the user and have the user select the
				// camera's output format. 

				// Get the media type.
				hr = pReader->GetNativeMediaType(
					(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
					0,  // Type index
					&pType
				);

				if (FAILED(hr)) { goto done; }

				// Get the sub type.
				hr = pType->GetGUID(MF_MT_SUBTYPE, &subtype);

				if (FAILED(hr)) { goto done; }

				// For each sub type.
				for (UINT32 i = 0; i < ARRAYSIZE(subtypes); i++)
				{
					// If sub types match.
					if (subtype == subtypes[i])
					{
						// Set the current media type.
						hr = pReader->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
							NULL,
							pType
						);

						// Found native type.
						bUseNativeType = TRUE;
						break;
					}
				}

				// If not using native types.
				if (!bUseNativeType)
				{
					// None of the native types worked.

					// Try adding a decoder.
					// For each sub type.
					for (UINT32 i = 0; i < ARRAYSIZE(subtypes); i++)
					{
						// Set the sub type.
						hr = pType->SetGUID(MF_MT_SUBTYPE, subtypes[i]);

						if (FAILED(hr)) { goto done; }

						// Set the current media type.
						hr = pReader->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
							NULL,
							pType
						);

						if (SUCCEEDED(hr))
						{
							// Exit.
							break;
						}
					}
				}

			done:
				// Safe release.
				SafeRelease(&pType);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure video and audio source reader.
			/// </summary>
			/// <param name="pReaderVideo">The source reader.</param>
			/// <param name="pReaderAudio">The source reader.</param>
			/// <param name="ep">The encoding parameters.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ConfigureVideoAudioSourceReader(IMFSourceReader* pReaderVideo, IMFSourceReader* pReaderAudio, EncodingParameters& ep)
			{
				// The list of acceptable types.
				GUID subtypesAudio[] = {
					MFAudioFormat_Base, MFAudioFormat_PCM, MFAudioFormat_Float, MFAudioFormat_DTS, MFAudioFormat_Dolby_AC3_SPDIF, MFAudioFormat_DRM,
					MFAudioFormat_WMAudioV8, MFAudioFormat_WMAudioV9, MFAudioFormat_WMAudio_Lossless, MFAudioFormat_WMASPDIF, MFAudioFormat_MSP1,
					MFAudioFormat_MP3, MFAudioFormat_MPEG, MFAudioFormat_AAC, MFAudioFormat_ADTS, MFAudioFormat_AMR_NB, MFAudioFormat_AMR_WB,
					MFAudioFormat_AMR_WP, MFAudioFormat_Dolby_AC3, MFAudioFormat_Dolby_DDPlus
				};

				// The list of acceptable types.
				GUID subtypesVideo[] = {
					MFVideoFormat_NV12, MFVideoFormat_YUY2, MFVideoFormat_UYVY, MFVideoFormat_WMV3,
					MFVideoFormat_RGB32, MFVideoFormat_RGB24, MFVideoFormat_IYUV, MFVideoFormat_H264
				};

				HRESULT hr = S_OK;

				BOOL bVideoUseNativeType = FALSE;
				BOOL bAudioUseNativeType = FALSE;

				GUID subtypeVideo = { 0 };
				GUID subtypeAudio = { 0 };

				IMFMediaType *pVideoType = NULL;
				IMFMediaType *pAudioType = NULL;

				// If the source's native format matches any of the formats in 
				// the list, prefer the native format.

				// Note: The camera might support multiple output formats, 
				// including a range of frame dimensions. The application could
				// provide a list to the user and have the user select the
				// camera's output format. 

				// Get the media type.
				hr = pReaderVideo->GetNativeMediaType(
					(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
					0,  // Type index
					&pVideoType
				);

				if (FAILED(hr)) { goto done; }

				// Set as video input.
				hr = pVideoType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);

				// Get the sub type.
				hr = pVideoType->GetGUID(MF_MT_SUBTYPE, &subtypeVideo);

				if (FAILED(hr)) { goto done; }

				// If a subtype exists.
				if (ep.video.subtypeReader != GUID_NULL)
				{
					// Set the reader subtype.
					subtypeVideo = ep.video.subtypeReader;
					hr = pVideoType->SetGUID(MF_MT_SUBTYPE, subtypeVideo);
				}

				// Assign the default.
				ep.video.subtypeReader = subtypeVideo;

				// For each sub type.
				for (UINT32 i = 0; i < ARRAYSIZE(subtypesVideo); i++)
				{
					// If sub types match.
					if (subtypeVideo == subtypesVideo[i])
					{
						// Set the current media type.
						hr = pReaderVideo->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
							NULL,
							pVideoType
						);

						// Found native type.
						bVideoUseNativeType = TRUE;
						break;
					}
				}

				// If not using native types.
				if (!bVideoUseNativeType)
				{
					// None of the native types worked.

					// Try adding a decoder.
					// For each sub type.
					for (UINT32 i = 0; i < ARRAYSIZE(subtypesVideo); i++)
					{
						// Set the sub type.
						hr = pVideoType->SetGUID(MF_MT_SUBTYPE, subtypesVideo[i]);

						if (FAILED(hr)) { goto done; }

						// Set the current media type.
						hr = pReaderVideo->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM,
							NULL,
							pVideoType
						);

						if (SUCCEEDED(hr))
						{
							// Exit.
							break;
						}
					}
				}

				// If the source's native format matches any of the formats in 
				// the list, prefer the native format.

				// Note: The camera might support multiple output formats, 
				// including a range of frame dimensions. The application could
				// provide a list to the user and have the user select the
				// camera's output format. 

				// Get the media type.
				hr = pReaderAudio->GetNativeMediaType(
					(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
					0,  // Type index
					&pAudioType
				);

				if (FAILED(hr)) { goto done; }

				// Set as audio input.
				hr = pAudioType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);

				// Get the sub type.
				hr = pAudioType->GetGUID(MF_MT_SUBTYPE, &subtypeAudio);

				if (FAILED(hr)) { goto done; }

				// If a subtype exists.
				if (ep.audio.subtypeReader != GUID_NULL)
				{
					// Set the reader subtype.
					subtypeAudio = ep.audio.subtypeReader;
					pAudioType->SetGUID(MF_MT_SUBTYPE, subtypeAudio);
				}

				// Assign the default.
				ep.audio.subtypeReader = subtypeAudio;

				// For each sub type.
				for (UINT32 i = 0; i < ARRAYSIZE(subtypesAudio); i++)
				{
					// If sub types match.
					if (subtypeAudio == subtypesAudio[i])
					{
						// Set the current media type.
						hr = pReaderAudio->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
							NULL,
							pAudioType
						);

						// Found native type.
						bAudioUseNativeType = TRUE;
						break;
					}
				}

				// If not using native types.
				if (!bAudioUseNativeType)
				{
					// None of the native types worked.

					// Try adding a decoder.
					// For each sub type.
					for (UINT32 i = 0; i < ARRAYSIZE(subtypesAudio); i++)
					{
						// Set the sub type.
						hr = pAudioType->SetGUID(MF_MT_SUBTYPE, subtypesAudio[i]);

						if (FAILED(hr)) { goto done; }

						// Set the current media type.
						hr = pReaderAudio->SetCurrentMediaType(
							(DWORD)MF_SOURCE_READER_FIRST_AUDIO_STREAM,
							NULL,
							pAudioType
						);

						if (SUCCEEDED(hr))
						{
							// Exit.
							break;
						}
					}
				}

			done:
				// Safe release.
				SafeRelease(&pVideoType);
				SafeRelease(&pAudioType);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure video encoder.
			/// </summary>
			/// <param name="params">The encoding parameters.</param>
			/// <param name="pType">The media type.</param>
			/// <param name="pWriter">The sink writer.</param>
			/// <param name="pdwStreamIndex">The stream index.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ConfigureVideoEncoder(
				EncodingParameters& params,
				IMFMediaType *pType,
				IMFSinkWriter *pWriter,
				DWORD *pdwStreamIndex)
			{
				HRESULT hr = S_OK;

				IMFMediaType *pType2 = NULL;

				// Get the media type.
				hr = MFCreateMediaType(&pType2);

				if (SUCCEEDED(hr))
				{
					// Set the media video
					hr = pType2->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
				}

				if (SUCCEEDED(hr))
				{
					// Set the subtype.
					hr = pType2->SetGUID(MF_MT_SUBTYPE, params.video.subtype);
				}

				if (SUCCEEDED(hr))
				{
					// Set the bit rate.
					if (params.video.bitRate > 0)
						hr = pType2->SetUINT32(MF_MT_AVG_BITRATE, params.video.bitRate);
					else
					{
						hr = CopyAttribute(pType, pType2, MF_MT_AVG_BITRATE);
						if (SUCCEEDED(hr))
						{
							UINT32 bitRate;

							// Get the default bit rate.
							hr = pType2->GetUINT32(MF_MT_AVG_BITRATE, &bitRate);
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
						hr = MFSetAttributeSize(pType2, MF_MT_FRAME_SIZE, params.video.frameSize.width, params.video.frameSize.height);
					else
					{
						hr = CopyAttribute(pType, pType2, MF_MT_FRAME_SIZE);
						if (SUCCEEDED(hr))
						{
							UINT32 width;
							UINT32 height;

							// Get the default frame size.
							hr = MFGetAttributeSize(pType, MF_MT_FRAME_SIZE, &width, &height);
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
						hr = MFSetAttributeRatio(pType2, MF_MT_FRAME_RATE, params.video.frameRate.numerator, params.video.frameRate.denominator);
					else
					{
						hr = CopyAttribute(pType, pType2, MF_MT_FRAME_RATE);
						if (SUCCEEDED(hr))
						{
							UINT32 numerator;
							UINT32 denominator;

							// Get the default frame rate.
							hr = MFGetAttributeRatio(pType, MF_MT_FRAME_RATE, &numerator, &denominator);
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
						hr = MFSetAttributeRatio(pType2, MF_MT_PIXEL_ASPECT_RATIO, params.video.aspectRatio.numerator, params.video.aspectRatio.denominator);
					else
					{
						hr = CopyAttribute(pType, pType2, MF_MT_PIXEL_ASPECT_RATIO);
						if (SUCCEEDED(hr))
						{
							UINT32 numerator;
							UINT32 denominator;

							// Get the default frame rate.
							hr = MFGetAttributeRatio(pType, MF_MT_PIXEL_ASPECT_RATIO, &numerator, &denominator);
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
					hr = CopyAttribute(pType, pType2, MF_MT_INTERLACE_MODE);
				}

				if (SUCCEEDED(hr))
				{
					// Write to the stream.
					hr = pWriter->AddStream(pType2, pdwStreamIndex);
				}

				// Safe release.
				SafeRelease(&pType2);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Configure audio encoder.
			/// </summary>
			/// <param name="params">The encoding parameters.</param>
			/// <param name="pType">The media type.</param>
			/// <param name="pWriter">The sink writer.</param>
			/// <param name="pdwStreamIndex">The stream index.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ConfigureAudioEncoder(
				EncodingParameters& params,
				IMFMediaType *pType,
				IMFSinkWriter *pWriter,
				DWORD *pdwStreamIndex)
			{
				HRESULT hr = S_OK;
				IMFMediaType *pType2 = NULL;
				IMFCollection *pAvailableTypes = NULL;
				IMFAttributes *pAttributes = NULL;

				// Get the media type.
				hr = MFCreateMediaType(&pType2);

				if (SUCCEEDED(hr))
				{
					// Set the media audio
					hr = pType2->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
				}

				if (SUCCEEDED(hr))
				{
					// Set the subtype.
					hr = pType2->SetGUID(MF_MT_SUBTYPE, params.audio.subtype);
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
						hr = GetCollectionObject(pAvailableTypes, params.audio.collectionIndex, &pType2);
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
						hr = pType2->GetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, &sampleRate);
						if (SUCCEEDED(hr))
						{
							params.audio.sampleRate = sampleRate;
						}

						// Get the default channels.
						hr = pType2->GetUINT32(MF_MT_AUDIO_NUM_CHANNELS, &channels);
						if (SUCCEEDED(hr))
						{
							params.audio.channels = channels;
						}

						// Get the default bits per sample.
						hr = pType2->GetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, &bitsPerSample);
						if (SUCCEEDED(hr))
						{
							params.audio.bitsPerSample = bitsPerSample;
						}

						// Get the default block align.
						hr = pType2->GetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, &blockAlign);
						if (SUCCEEDED(hr))
						{
							params.audio.blockAlign = blockAlign;
						}

						// Get the default bytes per second.
						hr = pType2->GetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, &bytesPerSecond);
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
							hr = pType2->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, params.audio.sampleRate);
						else
						{
							hr = CopyAttribute(pType, pType2, MF_MT_AUDIO_SAMPLES_PER_SECOND);
							if (SUCCEEDED(hr))
							{
								UINT32 sampleRate;

								// Get the default sample rate.
								hr = pType2->GetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, &sampleRate);
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
							hr = pType2->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, params.audio.channels);
						else
						{
							hr = CopyAttribute(pType, pType2, MF_MT_AUDIO_NUM_CHANNELS);
							if (SUCCEEDED(hr))
							{
								UINT32 channels;

								// Get the default channels.
								hr = pType2->GetUINT32(MF_MT_AUDIO_NUM_CHANNELS, &channels);
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
							hr = pType2->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, params.audio.bitsPerSample);
						else
						{
							hr = CopyAttribute(pType, pType2, MF_MT_AUDIO_BITS_PER_SAMPLE);
							if (SUCCEEDED(hr))
							{
								UINT32 bitsPerSample;

								// Get the default bits per sample.
								hr = pType2->GetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, &bitsPerSample);
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
							hr = pType2->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, params.audio.blockAlign);
						else
						{
							hr = CopyAttribute(pType, pType2, MF_MT_AUDIO_BLOCK_ALIGNMENT);
							if (SUCCEEDED(hr))
							{
								UINT32 blockAlign;

								// Get the default block align.
								hr = pType2->GetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, &blockAlign);
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
							hr = pType2->SetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, params.audio.bytesPerSecond);
						else
						{
							hr = CopyAttribute(pType, pType2, MF_MT_AUDIO_AVG_BYTES_PER_SECOND);
							if (SUCCEEDED(hr))
							{
								UINT32 bytesPerSecond;

								// Get the default bytes per second.
								hr = pType2->GetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, &bytesPerSecond);
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
					hr = pWriter->AddStream(pType2, pdwStreamIndex);
				}

				// Safe release.
				SafeRelease(&pType2);
				SafeRelease(&pAvailableTypes);
				SafeRelease(&pAttributes);

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
			HRESULT ConfigureVideoAudioEncoder(
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
					// Set the media audio
					hr = pAudioType2->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
				}

				if (SUCCEEDED(hr))
				{
					// Set the subtype.
					hr = pAudioType2->SetGUID(MF_MT_SUBTYPE, params.audio.subtype);
				}

				if (SUCCEEDED(hr))
				{
					// Set the bit rate.
					if (params.video.bitRate > 0)
						hr = pVideoType2->SetUINT32(MF_MT_AVG_BITRATE, params.video.bitRate);
					else
					{
						hr = CopyAttribute(pVideoType, pVideoType2, MF_MT_AVG_BITRATE);
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
						hr = CopyAttribute(pVideoType, pVideoType2, MF_MT_FRAME_SIZE);
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
						hr = CopyAttribute(pVideoType, pVideoType2, MF_MT_FRAME_RATE);
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
						hr = CopyAttribute(pVideoType, pVideoType2, MF_MT_PIXEL_ASPECT_RATIO);
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
					hr = CopyAttribute(pVideoType, pVideoType2, MF_MT_INTERLACE_MODE);
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
						hr = GetCollectionObject(pAvailableTypes, params.audio.collectionIndex, &pAudioType2);
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
							hr = CopyAttribute(pAudioType, pAudioType2, MF_MT_AUDIO_SAMPLES_PER_SECOND);
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
							hr = CopyAttribute(pAudioType, pAudioType2, MF_MT_AUDIO_NUM_CHANNELS);
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
							hr = CopyAttribute(pAudioType, pAudioType2, MF_MT_AUDIO_BITS_PER_SAMPLE);
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
							hr = CopyAttribute(pAudioType, pAudioType2, MF_MT_AUDIO_BLOCK_ALIGNMENT);
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
							hr = CopyAttribute(pAudioType, pAudioType2, MF_MT_AUDIO_AVG_BYTES_PER_SECOND);
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
					hr = pWriter->AddStream(pVideoType2, pdwVideoStreamIndex);
				}

				if (SUCCEEDED(hr))
				{
					// Write to the stream.
					hr = pWriter->AddStream(pAudioType2, pdwAudioStreamIndex);
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