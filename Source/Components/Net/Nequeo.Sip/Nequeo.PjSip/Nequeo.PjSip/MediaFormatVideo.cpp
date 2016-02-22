/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaFormatVideo.cpp
*  Purpose :       SIP MediaFormatVideo class.
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

#include "MediaFormatVideo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure describe detail information about an video media.
/// </summary>
MediaFormatVideo::MediaFormatVideo()
{
}

/// <summary>
/// Gets or sets the video width.
/// </summary>
unsigned MediaFormatVideo::Width::get()
{
	return _width;
}

/// <summary>
/// Gets or sets the video width.
/// </summary>
void MediaFormatVideo::Width::set(unsigned value)
{
	_width = value;
}

/// <summary>
/// Gets or sets the video height.
/// </summary>
unsigned MediaFormatVideo::Height::get()
{
	return _height;
}

/// <summary>
/// Gets or sets the video height.
/// </summary>
void MediaFormatVideo::Height::set(unsigned value)
{
	_height = value;
}

/// <summary>
/// Gets or sets the frames per second numerator.
/// </summary>
int MediaFormatVideo::FpsNum::get()
{
	return _fpsNum;
}

/// <summary>
/// Gets or sets the frames per second numerator.
/// </summary>
void MediaFormatVideo::FpsNum::set(int value)
{
	_fpsNum = value;
}

/// <summary>
/// Gets or sets the frames per second denumerator.
/// </summary>
int MediaFormatVideo::FpsDenum::get()
{
	return _fpsDenum;
}

/// <summary>
/// Gets or sets the frames per second denumerator.
/// </summary>
void MediaFormatVideo::FpsDenum::set(int value)
{
	_fpsDenum = value;
}

/// <summary>
/// Gets or sets the average bitrate.
/// </summary>
unsigned MediaFormatVideo::AvgBps::get()
{
	return _avgBps;
}

/// <summary>
/// Gets or sets the average bitrate.
/// </summary>
void MediaFormatVideo::AvgBps::set(unsigned value)
{
	_avgBps = value;
}

/// <summary>
/// Gets or sets the maximum bitrate.
/// </summary>
unsigned MediaFormatVideo::MaxBps::get()
{
	return _maxBps;
}

/// <summary>
/// Gets or sets the maximum bitrate.
/// </summary>
void MediaFormatVideo::MaxBps::set(unsigned value)
{
	_maxBps = value;
}