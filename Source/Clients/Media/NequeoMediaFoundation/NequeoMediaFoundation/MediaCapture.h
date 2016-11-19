/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaCapture.h
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

#pragma once

#ifndef _MEDIACAPTURE_H
#define _MEDIACAPTURE_H

#include "MediaGlobal.h"
#include "ContentEnabler.h"
#include "CriticalSectionHandler.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Contains an array of IMFActivate pointers. Each pointer represents
			/// a audio or video capture device.
			/// </summary>
			struct CaptureDeviceParam
			{
				/// <summary>
				/// The array of devices.
				/// </summary>
				IMFActivate **ppDevices;
				/// <summary>
				/// Device count.
				/// </summary>
				UINT32      count;
				/// <summary>
				/// Device selection.
				/// </summary>
				UINT32      selection;
			};

			/// <summary>
			/// Contains the video frame rate encoding parameters
			/// </summary>
			struct VideoFrameRate
			{
				/// <summary>
				/// Numerator.
				/// </summary>
				UINT32  numerator;
				/// <summary>
				/// Denominator.
				/// </summary>
				UINT32  denominator;
			};

			/// <summary>
			/// Contains the video aspect ratio encoding parameters
			/// </summary>
			struct VideoAspectRatio
			{
				/// <summary>
				/// Numerator.
				/// </summary>
				UINT32  numerator;
				/// <summary>
				/// Denominator.
				/// </summary>
				UINT32  denominator;
			};

			/// <summary>
			/// Contains the video frame size encoding parameters
			/// </summary>
			struct VideoFrameSize
			{
				/// <summary>
				/// Width.
				/// </summary>
				UINT32  width;
				/// <summary>
				/// Height.
				/// </summary>
				UINT32  height;
			};

			/// <summary>
			/// Contains the video encoding parameters
			/// </summary>
			struct VideoEncodingParameters
			{
				/// <summary>
				/// Encoding subtype.
				/// </summary>
				GUID    subtype;
				/// <summary>
				/// Bitrate.
				/// </summary>
				UINT32  bitRate;
				/// <summary>
				/// Frame rate.
				/// </summary>
				VideoFrameRate  frameRate;
				/// <summary>
				/// Frame size.
				/// </summary>
				VideoFrameSize  frameSize;
				/// <summary>
				/// Aspect ratio.
				/// </summary>
				VideoAspectRatio aspectRatio;
				/// <summary>
				/// MF transcode container type.
				/// </summary>
				GUID transcode;
			};

			/// <summary>
			/// Contains the audio encoding parameters
			/// </summary>
			struct AudioEncodingParameters
			{
				/// <summary>
				/// Encoding subtype.
				/// </summary>
				GUID    subtype;
				/// <summary>
				/// Samplerate.
				/// </summary>
				UINT32  sampleRate;
				/// <summary>
				/// Channels.
				/// </summary>
				UINT32  channels;
				/// <summary>
				/// Channels.
				/// </summary>
				UINT32  bitsPerSample;
				/// <summary>
				/// Block align.
				/// </summary>
				UINT32 blockAlign;
				/// <summary>
				/// Bytes per second.
				/// </summary>
				UINT32 bytesPerSecond;
				/// <summary>
				/// MF transcode container type.
				/// </summary>
				GUID transcode;
			};

			/// <summary>
			/// Contains the encoding parameters
			/// </summary>
			struct EncodingParameters
			{
				/// <summary>
				/// Video encoding.
				/// </summary>
				VideoEncodingParameters video;
				/// <summary>
				/// Audio encoding.
				/// </summary>
				AudioEncodingParameters audio;
				/// <summary>
				/// MF transcode container type.
				/// </summary>
				GUID transcode;
			};

			/// <summary>
			/// Providers the base for a media foundation capture.
			/// </summary>
			class MediaCapture : public IMFSourceReaderCallback
			{
			public:
				/// <summary>
				/// Static class method to create the MediaCapture object.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaCapture object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static HRESULT CreateInstance(HWND hwnd, HWND hEvent, MediaCapture **ppCapture);

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
				/// Get the video capture devices.
				/// </summary>
				/// <param name="param">The capture device param.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static void GetVideoCaptureDevices(CaptureDeviceParam *param);

				/// <summary>
				/// Get the audio capture devices.
				/// </summary>
				/// <param name="param">The capture device param.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static void GetAudioCaptureDevices(CaptureDeviceParam *param);

				/// <summary>
				/// Safely release all capture devices.
				/// </summary>
				/// <param name="param">The capture device param.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static void SafeReleaseCaptureDevices(CaptureDeviceParam *param);

				/// <summary>
				/// Set the video capture device.
				/// </summary>
				/// <param name="device">The media foundation device.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetVideoDevice(IMFActivate *device);

				/// <summary>
				/// Set the audio capture device.
				/// </summary>
				/// <param name="device">The media foundation device.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetAudioDevice(IMFActivate *device);

				/// <summary>
				/// Get the device name.
				/// </summary>
				/// <param name="device">The media foundation device.</param>
				/// <returns>The device name.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API std::wstring GetDeviceName(IMFActivate *device);

				/// <summary>
				/// Get the device names.
				/// </summary>
				/// <param name="param">The media foundation device collection.</param>
				/// <returns>The device names.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API std::vector<std::wstring> GetDeviceNames(CaptureDeviceParam& param);

				/// <summary>
				/// Is capturing.
				/// </summary>
				/// <returns>True if capturing; else false.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API BOOL IsCapturing();

				/// <summary>
				/// Start capture to file.
				/// </summary>
				/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
				/// <param name="param">The encoding parameters.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT StartCaptureToFile(const WCHAR *pwszFileName, EncodingParameters& param);

				/// <summary>
				/// Start capture to stream.
				/// </summary>
				/// <param name="pByteStream">The byte stream to write the capture data to.</param>
				/// <param name="param">The encoding parameters.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT StartCaptureToStream(IMFByteStream *pByteStream, EncodingParameters& param);

				/// <summary>
				/// Stop capture.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT StopCapture();

				/// <summary>
				/// Check device lost.
				/// </summary>
				/// <param name="pHdr">The device broad cast hander.</param>
				/// <param name="pbDeviceLost">Is device lost.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT CheckDeviceLost(DEV_BROADCAST_HDR *pHdr, BOOL *pbDeviceLost);

				/// <summary>
				/// End capture session.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT EndCaptureSession();
				
				/// <summary>
				/// On event read sample override.
				/// </summary>
				/// <param name="hrStatus">The read status.</param>
				/// <param name="dwStreamIndex">The stream index number.</param>
				/// <param name="dwStreamFlags">The stream flag.</param>
				/// <param name="llTimestamp">The current time stamp.</param>
				/// <param name="pSample">The captured sample data.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnReadSample(
					HRESULT hrStatus,
					DWORD dwStreamIndex,
					DWORD dwStreamFlags,
					LONGLONG llTimestamp,
					IMFSample *pSample);

				/// <summary>
				/// On event MF override.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnEvent(DWORD, IMFMediaEvent*)
				{
					return S_OK;
				}

				/// <summary>
				/// On event flush override.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP OnFlush(DWORD)
				{
					return S_OK;
				}

				/// <summary>
				/// Set the notify state event handler.
				/// </summary>
				/// <param name="stateEvent">The user defined event handler.</param>
				void SetNotifyStateEventHandler(HANDLE stateEvent)
				{
					// Assign the internal event.
					_hNotifyStateEvent = stateEvent;
				}

				/// <summary>
				/// Set the notify error event handler.
				/// </summary>
				/// <param name="errorEvent">The user defined event handler.</param>
				void SetNotifyErrorEventHandler(HANDLE errorEvent)
				{
					// Assign the internal event.
					_hNotifyErrorEvent = errorEvent;
				}

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="hr">The result reference.</param>
				MediaCapture(HWND hwnd, HWND hEvent, HRESULT &hr);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~MediaCapture();

				/// <summary>
				/// Notifies the application when the state changes.
				/// </summary>
				void NotifyState()
				{
					// If posting the state messsage.
					if (_hNotifyStateEvent != NULL)
					{
						// Trigger the notify state event handler.
						SetEvent(_hNotifyStateEvent);
					}

					PostMessage(_hwndEvent, WM_APP_NOTIFY, (WPARAM)_captureState, (LPARAM)0);
				}

				/// <summary>
				/// Notifies the application when an error occurs.
				/// </summary>
				/// <param name="hr">The handler result.</param>
				void NotifyError(HRESULT hr)
				{
					_captureState = CaptureNotReady;

					// If posting the error messsage.
					if (_hNotifyErrorEvent != NULL)
					{
						// Trigger the notify error event handler.
						SetEvent(_hNotifyErrorEvent);
					}

					PostMessage(_hwndEvent, WM_APP_ERROR, (WPARAM)hr, 0);
				}

				/// <summary>
				/// Open media source.
				/// </summary>
				/// <param name="pSource">The media source.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OpenMediaSource(IMFMediaSource *pSource);

				/// <summary>
				/// Configure video capture encoding.
				/// </summary>
				/// <param name="param">The encoding parameters.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT ConfigureVideoCapture(EncodingParameters& param);

				/// <summary>
				/// Configure audio capture encoding.
				/// </summary>
				/// <param name="param">The encoding parameters.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT ConfigureAudioCapture(EncodingParameters& param);

				/// <summary>
				/// Configure video and audio capture encoding.
				/// </summary>
				/// <param name="param">The encoding parameters.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT ConfigureVideoAudioCapture(EncodingParameters& param);

				/// <summary>
				/// End capture internal.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT EndCaptureInternal();

				/// <summary>
				/// Start video and audio capture.
				/// </summary>
				/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
				/// <param name="pByteStream">The byte stream to write the capture data to.</param>
				/// <param name="param">The encoding parameters.</param>
				/// <param name="writeToFile">Write to file; else write to byte stream.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT StartVideoAudioCapture(const WCHAR *pwszFileName, IMFByteStream *pByteStream, EncodingParameters& param, bool writeToFile = true);

				/// <summary>
				/// Start video capture.
				/// </summary>
				/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
				/// <param name="pByteStream">The byte stream to write the capture data to.</param>
				/// <param name="param">The encoding parameters.</param>
				/// <param name="writeToFile">Write to file; else write to byte stream.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT StartVideoCapture(const WCHAR *pwszFileName, IMFByteStream *pByteStream, EncodingParameters& param, bool writeToFile = true);

				/// <summary>
				/// Start audio capture.
				/// </summary>
				/// <param name="pwszFileName">The path and file name to write the capture data to.</param>
				/// <param name="pByteStream">The byte stream to write the capture data to.</param>
				/// <param name="param">The encoding parameters.</param>
				/// <param name="writeToFile">Write to file; else write to byte stream.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT StartAudioCapture(const WCHAR *pwszFileName, IMFByteStream *pByteStream, EncodingParameters& param, bool writeToFile = true);

				/// <summary>
				/// Initializes the MediaPlayer object. This method is called by the
				/// CreateInstance method.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT Initialize();

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.
				bool					_started;

				HWND				    _hwndApp;
				HWND				    _hwndEvent;			// App window to receive events.
				CRITICAL_SECTION        _critsec;

				IMFActivate				*_videoDevice;
				IMFActivate				*_audioDevice;
				bool					_hasVideoCapture;
				bool					_hasAudioCapture;

				IMFSourceReader         *_pReader;
				IMFSinkWriter           *_pVideoWriter;
				IMFSinkWriter           *_pAudioWriter;
				IMFSinkWriter           *_pVideoAudioWriter;

				BOOL                    _bFirstSample;
				LONGLONG                _llBaseTime;

				WCHAR                   *_pwszVideoSymbolicLink;
				WCHAR                   *_pwszAudioSymbolicLink;

				HANDLE				    _hCloseEvent;		// Event to wait on while closing.
				HANDLE					_hNotifyStateEvent;
				HANDLE					_hNotifyErrorEvent;
				CaptureState			_captureState;		// Current state of the media session.
			};
		}
	}
}
#endif