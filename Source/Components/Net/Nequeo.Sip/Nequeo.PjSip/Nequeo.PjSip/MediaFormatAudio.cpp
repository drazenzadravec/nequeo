/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaFormatAudio.cpp
*  Purpose :       SIP MediaFormatAudio class.
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

#include "MediaFormatAudio.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure describe detail information about an audio media.
/// </summary>
MediaFormatAudio::MediaFormatAudio()
{
}

/// <summary>
/// Gets or sets the audio clock rate in samples or Hz.
/// </summary>
unsigned MediaFormatAudio::ClockRate::get()
{
	return _clockRate;
}

/// <summary>
/// Gets or sets the audio clock rate in samples or Hz.
/// </summary>
void MediaFormatAudio::ClockRate::set(unsigned value)
{
	_clockRate = value;
}

/// <summary>
/// Gets or sets the number of channels.
/// </summary>
unsigned MediaFormatAudio::ChannelCount::get()
{
	return _channelCount;
}

/// <summary>
/// Gets or sets the number of channels.
/// </summary>
void MediaFormatAudio::ChannelCount::set(unsigned value)
{
	_channelCount = value;
}

/// <summary>
/// Gets or sets the frame interval, in microseconds.
/// </summary>
unsigned MediaFormatAudio::FrameTimeUsec::get()
{
	return _frameTimeUsec;
}

/// <summary>
/// Gets or sets the frame interval, in microseconds.
/// </summary>
void MediaFormatAudio::FrameTimeUsec::set(unsigned value)
{
	_frameTimeUsec = value;
}

/// <summary>
/// Gets or sets the number of bits per sample.
/// </summary>
unsigned MediaFormatAudio::BitsPerSample::get()
{
	return _bitsPerSample;
}

/// <summary>
/// Gets or sets the number of bits per sample.
/// </summary>
void MediaFormatAudio::BitsPerSample::set(unsigned value)
{
	_bitsPerSample = value;
}

/// <summary>
/// Gets or sets the average bitrate.
/// </summary>
unsigned MediaFormatAudio::AvgBps::get()
{
	return _avgBps;
}

/// <summary>
/// Gets or sets the average bitrate.
/// </summary>
void MediaFormatAudio::AvgBps::set(unsigned value)
{
	_avgBps = value;
}

/// <summary>
/// Gets or sets the maximum bitrate.
/// </summary>
unsigned MediaFormatAudio::MaxBps::get()
{
	return _maxBps;
}

/// <summary>
/// Gets or sets the maximum bitrate.
/// </summary>
void MediaFormatAudio::MaxBps::set(unsigned value)
{
	_maxBps = value;
}