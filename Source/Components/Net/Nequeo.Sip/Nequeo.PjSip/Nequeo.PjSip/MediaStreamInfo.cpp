/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaStreamInfo.cpp
*  Purpose :       SIP MediaStreamInfo class.
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

#include "MediaStreamInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Media stream info.
/// </summary>
MediaStreamInfo::MediaStreamInfo() :
	_disposed(false)
{
}

///	<summary>
///	Media stream info.
///	</summary>
MediaStreamInfo::~MediaStreamInfo()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
MediaType MediaStreamInfo::Type::get()
{
	return _type;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
void MediaStreamInfo::Type::set(MediaType value)
{
	_type = value;
}

/// <summary>
/// Gets or sets the transport protocol (RTP/AVP, etc.)
/// </summary>
MediaTransportProtocol MediaStreamInfo::TransportProtocol::get()
{
	return _transportProtocol;
}

/// <summary>
/// Gets or sets the transport protocol (RTP/AVP, etc.)
/// </summary>
void MediaStreamInfo::TransportProtocol::set(MediaTransportProtocol value)
{
	_transportProtocol = value;
}

/// <summary>
/// Gets or sets the media direction.
/// </summary>
MediaDirection MediaStreamInfo::Direction::get()
{
	return _direction;
}

/// <summary>
/// Gets or sets the media direction.
/// </summary>
void MediaStreamInfo::Direction::set(MediaDirection value)
{
	_direction = value;
}

/// <summary>
/// Gets or sets the remote RTP address.
/// </summary>
String^ MediaStreamInfo::RemoteRtpAddress::get()
{
	return _remoteRtpAddress;
}

/// <summary>
/// Gets or sets the remote RTP address.
/// </summary>
void MediaStreamInfo::RemoteRtpAddress::set(String^ value)
{
	_remoteRtpAddress = value;
}

/// <summary>
/// Gets or sets the optional remote RTCP address.
/// </summary>
String^ MediaStreamInfo::RemoteRtcpAddress::get()
{
	return _remoteRtcpAddress;
}

/// <summary>
/// Gets or sets the optional remote RTCP address.
/// </summary>
void MediaStreamInfo::RemoteRtcpAddress::set(String^ value)
{
	_remoteRtcpAddress = value;
}

/// <summary>
/// Gets or sets the outgoing codec payload type.
/// </summary>
unsigned MediaStreamInfo::TxPayloadType::get()
{
	return _txPayloadType;
}

/// <summary>
/// Gets or sets the outgoing codec payload type.
/// </summary>
void MediaStreamInfo::TxPayloadType::set(unsigned value)
{
	_txPayloadType = value;
}

/// <summary>
/// Gets or sets the incoming codec payload type.
/// </summary>
unsigned MediaStreamInfo::RxPayloadType::get()
{
	return _rxPayloadType;
}

/// <summary>
/// Gets or sets the incoming codec payload type.
/// </summary>
void MediaStreamInfo::RxPayloadType::set(unsigned value)
{
	_rxPayloadType = value;
}

/// <summary>
/// Gets or sets the codec name.
/// </summary>
String^ MediaStreamInfo::CodecName::get()
{
	return _codecName;
}

/// <summary>
/// Gets or sets the codec name.
/// </summary>
void MediaStreamInfo::CodecName::set(String^ value)
{
	_codecName = value;
}

/// <summary>
/// Gets or sets the codec clock rate.
/// </summary>
unsigned MediaStreamInfo::CodecClockRate::get()
{
	return _codecClockRate;
}

/// <summary>
/// Gets or sets the codec clock rate.
/// </summary>
void MediaStreamInfo::CodecClockRate::set(unsigned value)
{
	_codecClockRate = value;
}