/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CMp4Encoder.cpp
*  Purpose :       CMp4Encoder class.
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

#include "VideoCodec.h"

using namespace Nequeo::Media;

/// <summary>
/// Encode the file.
/// </summary>
/// <param name="pszInput">The input file to convert.</param>
/// <param name="pszOutput">The mp4 encoded file.</param>
/// <param name="videoProfile">The video profile.</param>
/// <param name="audioProfile">The audio profile.</param>
/// <returns>The result.</returns>
HRESULT EncodeFile(PCWSTR pszInput, PCWSTR pszOutput, int videoProfile, int audioProfile)
{
	IMFTranscodeProfile *pProfile = NULL;
	IMFMediaSource *pSource = NULL;
	IMFTopology *pTopology = NULL;
	CSession *pSession = NULL;

	MFTIME duration = 0;

	HRESULT hr = CreateMediaSource(pszInput, &pSource);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = GetSourceDuration(pSource, &duration);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = CreateTranscodeProfile(&pProfile, videoProfile, audioProfile);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = MFCreateTranscodeTopology(pSource, pszOutput, pProfile, &pTopology);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = CSession::Create(&pSession);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pSession->StartEncodingSession(pTopology);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = RunEncodingSession(pSession, duration);

done:
	if (pSource)
	{
		pSource->Shutdown();
	}

	SafeRelease(&pSession);
	SafeRelease(&pProfile);
	SafeRelease(&pSource);
	SafeRelease(&pTopology);
	return hr;
}

/// <summary>
/// Encode the file.
/// </summary>
/// <param name="pszInput">The input file to convert.</param>
/// <param name="pszOutput">The mp4 encoded file.</param>
/// <param name="videoProfile">The video profile.</param>
/// <param name="audioProfile">The audio profile.</param>
/// <returns>The result.</returns>
extern HRESULT EncodeFile(PCWSTR pszInput, PCWSTR pszOutput, H264ProfileInfo& videoProfile, AACProfileInfo& audioProfile)
{
	IMFTranscodeProfile *pProfile = NULL;
	IMFMediaSource *pSource = NULL;
	IMFTopology *pTopology = NULL;
	CSession *pSession = NULL;

	MFTIME duration = 0;

	HRESULT hr = CreateMediaSource(pszInput, &pSource);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = GetSourceDuration(pSource, &duration);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = CreateTranscodeProfile(&pProfile, videoProfile, audioProfile);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = MFCreateTranscodeTopology(pSource, pszOutput, pProfile, &pTopology);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = CSession::Create(&pSession);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pSession->StartEncodingSession(pTopology);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = RunEncodingSession(pSession, duration);

done:
	if (pSource)
	{
		pSource->Shutdown();
	}

	SafeRelease(&pSession);
	SafeRelease(&pProfile);
	SafeRelease(&pSource);
	SafeRelease(&pTopology);
	return hr;
}

/// <summary>
/// Create media source.
/// </summary>
/// <param name="pszURL">The source input file URL.</param>
/// <param name="ppSource">The media source.</param>
/// <returns>The result.</returns>
HRESULT CreateMediaSource(PCWSTR pszURL, IMFMediaSource **ppSource)
{
	MF_OBJECT_TYPE ObjectType = MF_OBJECT_INVALID;

	IMFSourceResolver* pResolver = NULL;
	IUnknown* pSource = NULL;

	// Create the source resolver.
	HRESULT hr = MFCreateSourceResolver(&pResolver);
	if (FAILED(hr))
	{
		goto done;
	}

	// Use the source resolver to create the media source
	hr = pResolver->CreateObjectFromURL(pszURL, MF_RESOLUTION_MEDIASOURCE,
		NULL, &ObjectType, &pSource);
	if (FAILED(hr))
	{
		goto done;
	}

	// Get the IMFMediaSource interface from the media source.
	hr = pSource->QueryInterface(IID_PPV_ARGS(ppSource));

done:
	SafeRelease(&pResolver);
	SafeRelease(&pSource);
	return hr;
}

/// <summary>
/// Create media source duration.
/// </summary>
/// <param name="ppSource">The media source.</param>
/// <param name="pDuration">The media duration.</param>
/// <returns>The result.</returns>
HRESULT GetSourceDuration(IMFMediaSource *pSource, MFTIME *pDuration)
{
	*pDuration = 0;

	IMFPresentationDescriptor *pPD = NULL;

	HRESULT hr = pSource->CreatePresentationDescriptor(&pPD);
	if (SUCCEEDED(hr))
	{
		hr = pPD->GetUINT64(MF_PD_DURATION, (UINT64*)pDuration);
		pPD->Release();
	}
	return hr;
}

/// <summary>
/// Create transcode profile.
/// </summary>
/// <param name="ppProfile">The media source.</param>
/// <param name="videoProfile">The video profile.</param>
/// <param name="audioProfile">The audio profile.</param>
/// <returns>The result.</returns>
HRESULT CreateTranscodeProfile(IMFTranscodeProfile **ppProfile, int videoProfile, int audioProfile)
{
	IMFTranscodeProfile *pProfile = NULL;
	IMFAttributes *pAudio = NULL;
	IMFAttributes *pVideo = NULL;
	IMFAttributes *pContainer = NULL;

	HRESULT hr = MFCreateTranscodeProfile(&pProfile);
	if (FAILED(hr))
	{
		goto done;
	}

	// Audio attributes.
	hr = CreateAACProfile(audioProfile, &pAudio);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pProfile->SetAudioAttributes(pAudio);
	if (FAILED(hr))
	{
		goto done;
	}

	// Video attributes.
	hr = CreateH264Profile(videoProfile, &pVideo);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pProfile->SetVideoAttributes(pVideo);
	if (FAILED(hr))
	{
		goto done;
	}

	// Container attributes.
	hr = MFCreateAttributes(&pContainer, 1);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pContainer->SetGUID(MF_TRANSCODE_CONTAINERTYPE, MFTranscodeContainerType_MPEG4);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pProfile->SetContainerAttributes(pContainer);
	if (FAILED(hr))
	{
		goto done;
	}

	*ppProfile = pProfile;
	(*ppProfile)->AddRef();

done:
	SafeRelease(&pProfile);
	SafeRelease(&pAudio);
	SafeRelease(&pVideo);
	SafeRelease(&pContainer);
	return hr;
}

/// <summary>
/// Create transcode profile.
/// </summary>
/// <param name="ppProfile">The media source.</param>
/// <param name="videoProfile">The video profile.</param>
/// <param name="audioProfile">The audio profile.</param>
/// <returns>The result.</returns>
extern HRESULT CreateTranscodeProfile(IMFTranscodeProfile **ppProfile, H264ProfileInfo& videoProfile, AACProfileInfo& audioProfile)
{
	IMFTranscodeProfile *pProfile = NULL;
	IMFAttributes *pAudio = NULL;
	IMFAttributes *pVideo = NULL;
	IMFAttributes *pContainer = NULL;

	HRESULT hr = MFCreateTranscodeProfile(&pProfile);
	if (FAILED(hr))
	{
		goto done;
	}

	// Audio attributes.
	hr = CreateAACProfile(audioProfile, &pAudio);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pProfile->SetAudioAttributes(pAudio);
	if (FAILED(hr))
	{
		goto done;
	}

	// Video attributes.
	hr = CreateH264Profile(videoProfile, &pVideo);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pProfile->SetVideoAttributes(pVideo);
	if (FAILED(hr))
	{
		goto done;
	}

	// Container attributes.
	hr = MFCreateAttributes(&pContainer, 1);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pContainer->SetGUID(MF_TRANSCODE_CONTAINERTYPE, MFTranscodeContainerType_MPEG4);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pProfile->SetContainerAttributes(pContainer);
	if (FAILED(hr))
	{
		goto done;
	}

	*ppProfile = pProfile;
	(*ppProfile)->AddRef();

done:
	SafeRelease(&pProfile);
	SafeRelease(&pAudio);
	SafeRelease(&pVideo);
	SafeRelease(&pContainer);
	return hr;
}

/// <summary>
/// Create H264 profile.
/// </summary>
/// <param name="index">The profile index.</param>
/// <param name="ppAttributes">The H264 attributes.</param>
/// <returns>The result.</returns>
extern HRESULT CreateH264Profile(H264ProfileInfo& profile, IMFAttributes **ppAttributes)
{
	IMFAttributes *pAttributes = NULL;

	HRESULT hr = MFCreateAttributes(&pAttributes, 5);
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_H264);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(MF_MT_MPEG2_PROFILE, profile.profile);
	}
	if (SUCCEEDED(hr))
	{
		hr = MFSetAttributeSize(
			pAttributes, MF_MT_FRAME_SIZE,
			profile.frame_size.Numerator, profile.frame_size.Denominator);
	}
	if (SUCCEEDED(hr))
	{
		hr = MFSetAttributeRatio(
			pAttributes, MF_MT_FRAME_RATE,
			profile.fps.Numerator, profile.fps.Denominator);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(MF_MT_AVG_BITRATE, profile.bitrate);
	}
	if (SUCCEEDED(hr))
	{
		*ppAttributes = pAttributes;
		(*ppAttributes)->AddRef();
	}
	SafeRelease(&pAttributes);
	return hr;
}

/// <summary>
/// Create H264 profile.
/// </summary>
/// <param name="index">The profile index.</param>
/// <param name="ppAttributes">The H264 attributes.</param>
/// <returns>The result.</returns>
HRESULT CreateH264Profile(DWORD index, IMFAttributes **ppAttributes)
{
	if (index >= h264_profiles_array_size)
	{
		return E_INVALIDARG;
	}

	IMFAttributes *pAttributes = NULL;

	const H264ProfileInfo& profile = h264_profiles[index];

	HRESULT hr = MFCreateAttributes(&pAttributes, 5);
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_H264);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(MF_MT_MPEG2_PROFILE, profile.profile);
	}
	if (SUCCEEDED(hr))
	{
		hr = MFSetAttributeSize(
			pAttributes, MF_MT_FRAME_SIZE,
			profile.frame_size.Numerator, profile.frame_size.Denominator);
	}
	if (SUCCEEDED(hr))
	{
		hr = MFSetAttributeRatio(
			pAttributes, MF_MT_FRAME_RATE,
			profile.fps.Numerator, profile.fps.Denominator);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(MF_MT_AVG_BITRATE, profile.bitrate);
	}
	if (SUCCEEDED(hr))
	{
		*ppAttributes = pAttributes;
		(*ppAttributes)->AddRef();
	}
	SafeRelease(&pAttributes);
	return hr;
}

/// <summary>
/// Create ACC profile.
/// </summary>
/// <param name="index">The profile index.</param>
/// <param name="ppAttributes">The H264 attributes.</param>
/// <returns>The result.</returns>
extern HRESULT CreateAACProfile(AACProfileInfo& profile, IMFAttributes **ppAttributes)
{
	IMFAttributes *pAttributes = NULL;

	HRESULT hr = MFCreateAttributes(&pAttributes, 7);
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_AAC);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_BITS_PER_SAMPLE, profile.bitsPerSample);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_SAMPLES_PER_SECOND, profile.samplesPerSec);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_NUM_CHANNELS, profile.numChannels);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_AVG_BYTES_PER_SECOND, profile.bytesPerSec);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, 1);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION, profile.aacProfile);
	}
	if (SUCCEEDED(hr))
	{
		*ppAttributes = pAttributes;
		(*ppAttributes)->AddRef();
	}
	SafeRelease(&pAttributes);
	return hr;
}

/// <summary>
/// Create ACC profile.
/// </summary>
/// <param name="index">The profile index.</param>
/// <param name="ppAttributes">The H264 attributes.</param>
/// <returns>The result.</returns>
HRESULT CreateAACProfile(DWORD index, IMFAttributes **ppAttributes)
{
	if (index >= acc_profiles_array_size)
	{
		return E_INVALIDARG;
	}

	const AACProfileInfo& profile = aac_profiles[index];

	IMFAttributes *pAttributes = NULL;

	HRESULT hr = MFCreateAttributes(&pAttributes, 7);
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_AAC);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_BITS_PER_SAMPLE, profile.bitsPerSample);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_SAMPLES_PER_SECOND, profile.samplesPerSec);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_NUM_CHANNELS, profile.numChannels);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AUDIO_AVG_BYTES_PER_SECOND, profile.bytesPerSec);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, 1);
	}
	if (SUCCEEDED(hr))
	{
		hr = pAttributes->SetUINT32(
			MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION, profile.aacProfile);
	}
	if (SUCCEEDED(hr))
	{
		*ppAttributes = pAttributes;
		(*ppAttributes)->AddRef();
	}
	SafeRelease(&pAttributes);
	return hr;
}

/// <summary>
/// Run encoding session.
/// </summary>
/// <param name="pSession">The current session.</param>
/// <param name="duration">The duration.</param>
/// <returns>The result.</returns>
HRESULT RunEncodingSession(Nequeo::Media::CSession *pSession, MFTIME duration)
{
	const DWORD WAIT_PERIOD = 500;
	const int   UPDATE_INCR = 5;

	HRESULT hr = S_OK;
	MFTIME pos;
	LONGLONG prev = 0;
	while (1)
	{
		hr = pSession->Wait(WAIT_PERIOD);
		if (hr == E_PENDING)
		{
			hr = pSession->GetEncodingPosition(&pos);

			LONGLONG percent = (100 * pos) / duration;
			if (percent >= prev + UPDATE_INCR)
			{
				std::cout << percent << "% .. ";
				prev = percent;
			}
		}
		else
		{
			std::cout << std::endl;
			break;
		}
	}
	return hr;
}