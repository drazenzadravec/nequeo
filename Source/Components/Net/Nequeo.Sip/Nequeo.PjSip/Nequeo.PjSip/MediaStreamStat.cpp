/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaStreamStat.cpp
*  Purpose :       SIP MediaStreamStat class.
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

#include "MediaStreamStat.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Media stream statistic.
/// </summary>
MediaStreamStat::MediaStreamStat() :
	_disposed(false)
{
}

///	<summary>
///	Media stream statistic.
///	</summary>
MediaStreamStat::~MediaStreamStat()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets or sets the time when session was created.
/// </summary>
TimeVal^ MediaStreamStat::Start::get()
{
	return _start;
}

/// <summary>
/// Gets or sets the time when session was created.
/// </summary>
void MediaStreamStat::Start::set(TimeVal^ value)
{
	_start = value;
}

/// <summary>
/// Gets or sets the last TX RTP timestamp.
/// </summary>
unsigned int MediaStreamStat::RtpTxLastTs::get()
{
	return _rtpTxLastTs;
}

/// <summary>
/// Gets or sets the last TX RTP timestamp.
/// </summary>
void MediaStreamStat::RtpTxLastTs::set(unsigned int value)
{
	_rtpTxLastTs = value;
}

/// <summary>
/// Gets or sets the last TX RTP sequence. 
/// </summary>
unsigned short MediaStreamStat::RtpTxLastSeq::get()
{
	return _rtpTxLastSeq;
}

/// <summary>
/// Gets or sets the last TX RTP sequence. 
/// </summary>
void MediaStreamStat::RtpTxLastSeq::set(unsigned short value)
{
	_rtpTxLastSeq = value;
}

/// <summary>
/// Gets or sets the Individual frame size, in bytes. 
/// </summary>
unsigned MediaStreamStat::FrameSize::get()
{
	return _frameSize;
}

/// <summary>
/// Gets or sets the Individual frame size, in bytes. 
/// </summary>
void MediaStreamStat::FrameSize::set(unsigned value)
{
	_frameSize = value;
}

/// <summary>
/// Gets or sets the Minimum allowed prefetch, in frms. 
/// </summary>
unsigned MediaStreamStat::MinPrefetch::get()
{
	return _minPrefetch;
}

/// <summary>
/// Gets or sets the Minimum allowed prefetch, in frms. 
/// </summary>
void MediaStreamStat::MinPrefetch::set(unsigned value)
{
	_minPrefetch = value;
}

/// <summary>
/// Gets or sets the Maximum allowed prefetch, in frms.  
/// </summary>
unsigned MediaStreamStat::MaxPrefetch::get()
{
	return _maxPrefetch;
}

/// <summary>
/// Gets or sets the Maximum allowed prefetch, in frms. 
/// </summary>
void MediaStreamStat::MaxPrefetch::set(unsigned value)
{
	_maxPrefetch = value;
}

/// <summary>
/// Gets or sets the Current burst level, in frames.
/// </summary>
unsigned MediaStreamStat::Burst::get()
{
	return _burst;
}

/// <summary>
/// Gets or sets the Current burst level, in frames.
/// </summary>
void MediaStreamStat::Burst::set(unsigned value)
{
	_burst = value;
}

/// <summary>
/// Gets or sets the Current prefetch value, in frames.
/// </summary>
unsigned MediaStreamStat::Prefetch::get()
{
	return _prefetch;
}

/// <summary>
/// Gets or sets the Current prefetch value, in frames.
/// </summary>
void MediaStreamStat::Prefetch::set(unsigned value)
{
	_prefetch = value;
}

/// <summary>
/// Gets or sets the Current buffer size, in frames.
/// </summary>
unsigned MediaStreamStat::Size::get()
{
	return _size;
}

/// <summary>
/// Gets or sets the Current buffer size, in frames.
/// </summary>
void MediaStreamStat::Size::set(unsigned value)
{
	_size = value;
}

/// <summary>
/// Gets or sets the Average delay, in ms.
/// </summary>
unsigned MediaStreamStat::AvgDelayMsec::get()
{
	return _avgDelayMsec;
}

/// <summary>
/// Gets or sets the Average delay, in ms.
/// </summary>
void MediaStreamStat::AvgDelayMsec::set(unsigned value)
{
	_avgDelayMsec = value;
}

/// <summary>
/// Gets or sets the Minimum delay, in ms.
/// </summary>
unsigned MediaStreamStat::MinDelayMsec::get()
{
	return _minDelayMsec;
}

/// <summary>
/// Gets or sets the Minimum delay, in ms.
/// </summary>
void MediaStreamStat::MinDelayMsec::set(unsigned value)
{
	_minDelayMsec = value;
}

/// <summary>
/// Gets or sets the Maximum delay, in ms.
/// </summary>
unsigned MediaStreamStat::MaxDelayMsec::get()
{
	return _maxDelayMsec;
}

/// <summary>
/// Gets or sets the Maximum delay, in ms.
/// </summary>
void MediaStreamStat::MaxDelayMsec::set(unsigned value)
{
	_maxDelayMsec = value;
}

/// <summary>
/// Gets or sets the Standard deviation of delay, in ms.
/// </summary>
unsigned MediaStreamStat::DevDelayMsec::get()
{
	return _devDelayMsec;
}

/// <summary>
/// Gets or sets the Standard deviation of delay, in ms.
/// </summary>
void MediaStreamStat::DevDelayMsec::set(unsigned value)
{
	_devDelayMsec = value;
}

/// <summary>
/// Gets or sets the Average burst, in frames.
/// </summary>
unsigned MediaStreamStat::AvgBurst::get()
{
	return _avgBurst;
}

/// <summary>
/// Gets or sets the Average burst, in frames.
/// </summary>
void MediaStreamStat::AvgBurst::set(unsigned value)
{
	_avgBurst = value;
}

/// <summary>
/// Gets or sets the Number of lost frames.
/// </summary>
unsigned MediaStreamStat::Lost::get()
{
	return _lost;
}

/// <summary>
/// Gets or sets the Number of lost frames.
/// </summary>
void MediaStreamStat::Lost::set(unsigned value)
{
	_lost = value;
}

/// <summary>
/// Gets or sets the Number of discarded frames.
/// </summary>
unsigned MediaStreamStat::Discard::get()
{
	return _discard;
}

/// <summary>
/// Gets or sets the Number of discarded frames.
/// </summary>
void MediaStreamStat::Discard::set(unsigned value)
{
	_discard = value;
}

/// <summary>
/// Gets or sets the Number of empty on GET events.
/// </summary>
unsigned MediaStreamStat::Empty::get()
{
	return _empty;
}

/// <summary>
/// Gets or sets the Number of empty on GET events.
/// </summary>
void MediaStreamStat::Empty::set(unsigned value)
{
	_empty = value;
}