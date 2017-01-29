/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaSource.h
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

#pragma once

#ifndef _MEDIASOURCE_H
#define _MEDIASOURCE_H

#include "MediaGlobal.h"
#include "MediaCapture.h"
#include "SourceReader.h"
#include "CriticalSectionHandler.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			class MediaSource : public IUnknown
			{
			public:
				/// <summary>
				/// Static class method to create the MediaCapture object.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="ppMediaSource">Receives an AddRef's pointer to the MediaSource object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API static HRESULT CreateInstance(HWND hwnd, HWND hEvent, MediaSource **ppMediaSource);

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
				/// Open a file stream.
				/// </summary>
				/// <param name="pwszFileName">The path and file name to read data from.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT OpenFile(const WCHAR *pwszFileName);

				/// <summary>
				/// Open a byte stream.
				/// </summary>
				/// <param name="pByteStream">The byte stream to read data from.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT OpenStream(IMFByteStream *pByteStream);

				/// <summary>
				/// Close a media source.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT Close();

				/// <summary>
				/// Get the media details.
				/// </summary>
				/// <param name="param">The encoding parameters.</param>
				/// <param name="hasVideo">True if the source contains video; else false.</param>
				/// <param name="hasAudio">True if the source contains audio; else false.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT MediaDetails(EncodingParameters& param, bool *hasVideo, bool *hasAudio);

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
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT ReadSample(
					DWORD streamIndex,
					bool *endOfSamples, 
					DWORD *pdwActualStreamIndex, 
					DWORD *pdwStreamFlags, 
					LONGLONG *pllTimestamp, 
					IMFSample **ppSample);

				/// <summary>
				/// Seeks to a new position in the media source.
				/// </summary>
				/// <param name="position">The position from which playback will be started. The units are specified by the timeFormat parameter. If the timeFormat parameter is GUID_NULL, set the variant type to VT_I8.</param>
				/// <param name="timeFormat">A GUID that specifies the time format. The time format defines the units for the varPosition parameter. The following value is defined for all media sources: 
				/// GUID_NULL : 100-nanosecond units.
				/// Some media sources might support additional values.
				/// </param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetPosition(REFPROPVARIANT position, REFGUID timeFormat);

				/// <summary>
				/// Seeks to a new position in the media source.
				/// </summary>
				/// <param name="position">The position from which playback will be started.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetPosition(LARGE_INTEGER position);

				/// <summary>
				/// Seeks to a new position in the media source.
				/// </summary>
				/// <param name="position">The position from which playback will be started.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SetPosition(LONGLONG position);

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
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT GetDuration(DWORD streamIndex, MFTIME *duration);

				/// <summary>
				/// Copy the current sample data to the byte array.
				/// </summary>
				/// <param name="sample">The sample.</param>
				/// <param name="data">The sample byte array.</param>
				/// <param name="dataLength">The sample byte array size.</param>
				/// <returns>The result of the operation.</returns>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API HRESULT SampleToBytes(IMFSample *sample, BYTE **data, DWORD *dataLength);

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="hr">The result reference.</param>
				MediaSource(HWND hwnd, HWND hEvent, HRESULT &hr);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				virtual ~MediaSource();

				/// <summary>
				/// Open a media source.
				/// </summary>
				/// <param name="pwszFileName">The path and file name to read data from.</param>
				/// <param name="pByteStream">The byte stream to read data from.</param>
				/// <param name="readFromFile">Read to file; else read from byte stream.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OpenMediaSource(const WCHAR *pwszFileName, IMFByteStream *pByteStream, bool readFromFile = true);

				/// <summary>
				/// Initializes the MediaPlayer object. This method is called by the
				/// CreateInstance method.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT Initialize();

			private:
				bool					_disposed;
				long                    _nRefCount;			// Reference count.
				bool					_isOpen;
				bool					_isSourceReader;

				IMFSourceReader			*_pSourceReader;

				HWND				    _hwndApp;
				HWND				    _hwndEvent;			// App window to receive events.
				HANDLE				    _hCloseEvent;		// Event to wait on while closing.
				CRITICAL_SECTION        _critsec;
			};
		}
	}
}
#endif