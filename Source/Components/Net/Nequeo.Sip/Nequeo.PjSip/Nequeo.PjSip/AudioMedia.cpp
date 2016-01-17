/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMedia.cpp
*  Purpose :       SIP AudioMedia class.
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

#include "AudioMedia.h"
#include "MediaType.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Audio media.
/// </summary>
/// <param name="pjAudioMedia">The pj audio media.</param>
AudioMedia::AudioMedia(pj::AudioMedia& pjAudioMedia) :
	MediaBase(MediaType::PJMEDIA_TYPE_AUDIO), _disposed(false), _pjAudioMedia(pjAudioMedia)
{
}

///	<summary>
///	Sip media manager deconstructor.
///	</summary>
AudioMedia::~AudioMedia()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Get the pj audio media.
/// </summary>
/// <returns>The pj audio media.</returns>
pj::AudioMedia& AudioMedia::GetAudioMedia()
{
	return _pjAudioMedia;
}

/// <summary>
/// Gets or sets the conference port Id.
/// </summary>
int AudioMedia::Id::get()
{
	return _id;
}

/// <summary>
/// Gets or sets the conference port Id.
/// </summary>
void AudioMedia::Id::set(int value)
{
	_id = value;
}

/// <summary>
/// Get information about the specified conference port.
/// </summary>
/// <returns>The conference port.</returns>
ConfPortInfo^ AudioMedia::GetPortInfo()
{
	// Get the config
	pj::ConfPortInfo info = _pjAudioMedia.getPortInfo();

	ConfPortInfo^ confPortInfo = gcnew ConfPortInfo();
	confPortInfo->Format = gcnew MediaFormatAudio();
	confPortInfo->Format->AvgBps = info.format.avgBps;
	confPortInfo->Format->BitsPerSample = info.format.bitsPerSample;
	confPortInfo->Format->ChannelCount = info.format.channelCount;
	confPortInfo->Format->ClockRate = info.format.clockRate;
	confPortInfo->Format->FrameTimeUsec = info.format.frameTimeUsec;
	confPortInfo->Format->Id = info.format.id;
	confPortInfo->Format->MaxBps = info.format.maxBps;
	confPortInfo->Format->Type = MediaFormat::GetMediaTypeEx(info.format.type);

	confPortInfo->Name = gcnew String(info.name.c_str());
	confPortInfo->PortId = info.portId;
	confPortInfo->RxLevelAdj = info.rxLevelAdj;
	confPortInfo->TxLevelAdj = info.txLevelAdj;

	pj::IntVector ports = info.listeners;

	// Get the vector size.
	size_t vectorSize = ports.size();
	array<int>^ listeners = gcnew array<int>((int)vectorSize);

	// If devices exist.
	if (vectorSize > 0)
	{
		// For each code found.
		for (int i = 0; i < vectorSize; i++)
		{
			int port = ports[i];
			listeners[i] = port;
		}
	}

	confPortInfo->Listeners = listeners;

	// Return the config port info.
	return confPortInfo;
}

/// <summary>
/// Get port id.
/// </summary>
/// <returns>The port id.</returns>
int AudioMedia::GetPortId()
{
	return _pjAudioMedia.getPortId();
}

/// <summary>
/// Get information about the specified conference port.
/// </summary>
/// <param name="portId">The port id.</param>
/// <returns>The conference port.</returns>
ConfPortInfo^ AudioMedia::GetPortInfoFromId(int portId)
{
	// Get the config
	pj::ConfPortInfo info = pj::AudioMedia::getPortInfoFromId(portId);

	ConfPortInfo^ confPortInfo = gcnew ConfPortInfo();
	confPortInfo->Format = gcnew MediaFormatAudio();
	confPortInfo->Format->AvgBps = info.format.avgBps;
	confPortInfo->Format->BitsPerSample = info.format.bitsPerSample;
	confPortInfo->Format->ChannelCount = info.format.channelCount;
	confPortInfo->Format->ClockRate = info.format.clockRate;
	confPortInfo->Format->FrameTimeUsec = info.format.frameTimeUsec;
	confPortInfo->Format->Id = info.format.id;
	confPortInfo->Format->MaxBps = info.format.maxBps;
	confPortInfo->Format->Type = MediaFormat::GetMediaTypeEx(info.format.type);

	confPortInfo->Name = gcnew String(info.name.c_str());
	confPortInfo->PortId = info.portId;
	confPortInfo->RxLevelAdj = info.rxLevelAdj;
	confPortInfo->TxLevelAdj = info.txLevelAdj;

	pj::IntVector ports = info.listeners;

	// Get the vector size.
	size_t vectorSize = ports.size();
	array<int>^ listeners = gcnew array<int>((int)vectorSize);

	// If devices exist.
	if (vectorSize > 0)
	{
		// For each code found.
		for (int i = 0; i < vectorSize; i++)
		{
			int port = ports[i];
			listeners[i] = port;
		}
	}

	confPortInfo->Listeners = listeners;

	// Return the config port info.
	return confPortInfo;
}

/// <summary>
/// Establish unidirectional media flow to sink. This media port
/// will act as a source, and it may transmit to multiple destinations / sink.
/// And if multiple sources are transmitting to the same sink, the media
/// will be mixed together.Source and sink may refer to the same Media,
/// effectively looping the media.
///
/// If bidirectional media flow is desired, application needs to call
/// this method twice, with the second one called from the opposite source
/// media.
/// </summary>
/// <param name="sink">The destination media.</param>
void AudioMedia::StartTransmit(AudioMedia^ sink)
{
	pj::AudioMedia& media = sink->GetAudioMedia();
	_pjAudioMedia.startTransmit(media);
}

/// <summary>
/// Stop media flow to destination/sink port.
/// </summary>
/// <param name="sink">The destination media.</param>
void AudioMedia::StopTransmit(AudioMedia^ sink)
{
	pj::AudioMedia& media = sink->GetAudioMedia();
	_pjAudioMedia.stopTransmit(media);
}

/// <summary>
/// Adjust the signal level to be transmitted from the bridge to this
/// media port by making it louder or quieter.
/// </summary>
/// <param name="level">Signal level adjustment. Value 1.0 means no level 
/// adjustment, while value 0 means to mute the port.</param>
void AudioMedia::AdjustRxLevel(float level)
{
	_pjAudioMedia.adjustRxLevel(level);
}

/// <summary>
/// Adjust the signal level to be received from this media port (to
/// the bridge) by making it louder or quieter.
/// </summary>
/// <param name="level">Signal level adjustment. Value 1.0 means no level 
/// adjustment, while value 0 means to mute the port.</param>
void AudioMedia::AdjustTxLevel(float level)
{
	_pjAudioMedia.adjustTxLevel(level);
}

/// <summary>
/// Get the last received signal level.
/// </summary>
/// <returns>Signal level in percent.</returns>
unsigned AudioMedia::GetRxLevel()
{
	return _pjAudioMedia.getRxLevel();
}

/// <summary>
/// Get the last transmitted signal level.
/// </summary>
/// <returns>Signal level in percent.</returns>
unsigned AudioMedia::GetTxLevel()
{
	return _pjAudioMedia.getTxLevel();
}

/// <summary>
/// Typecast from base class MediaBase.
/// </summary>
/// <param name="media">The object to be downcasted.</param>
/// <returns>The object as AudioMedia instance.</returns>
AudioMedia^ AudioMedia::TypecastFromMedia(MediaBase^ media)
{
	// Upcast media.
	AudioMedia^ audio = safe_cast<AudioMedia^>(media);

	// Create the new call audio media.
	std::unique_ptr<CallAudioMedia> callAudioMedia = std::make_unique<CallAudioMedia>();

	// Assign the values.
	callAudioMedia->SetPortId(audio->GetPortId());
	callAudioMedia->adjustRxLevel(audio->GetRxLevel());
	callAudioMedia->adjustTxLevel(audio->GetTxLevel());
	pj::AudioMedia* audioMedia = pj::AudioMedia::typecastFromMedia(callAudioMedia.get());

	// Cleanup.
	delete audioMedia;
	
	// Return
	return audio;
}