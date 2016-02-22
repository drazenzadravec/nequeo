/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoDeviceInfo.cpp
*  Purpose :       SIP VideoDeviceInfo class.
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

#include "VideoDeviceInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Video device info.
/// </summary>
VideoDeviceInfo::VideoDeviceInfo()
{
}

/// <summary>
/// Gets or sets the device id.
/// </summary>
int VideoDeviceInfo::Id::get()
{
	return _id;
}

/// <summary>
/// Gets or sets the device id.
/// </summary>
void VideoDeviceInfo::Id::set(int value)
{
	_id = value;
}

/// <summary>
/// Gets or sets the device name.
/// </summary>
String^ VideoDeviceInfo::Name::get()
{
	return _name;
}

/// <summary>
/// Gets or sets the device name.
/// </summary>
void VideoDeviceInfo::Name::set(String^ value)
{
	_name = value;
}

/// <summary>
/// Gets or sets the underlying driver name.
/// </summary>
String^ VideoDeviceInfo::Driver::get()
{
	return _driver;
}

/// <summary>
/// Gets or sets the underlying driver name.
/// </summary>
void VideoDeviceInfo::Driver::set(String^ value)
{
	_driver = value;
}

/// <summary>
/// Gets or sets the supported direction of the video device, i.e. whether it supports
/// capture only, render only, or both.
/// </summary>
MediaDirection VideoDeviceInfo::Direction::get()
{
	return _direction;
}

/// <summary>
/// Gets or sets the supported direction of the video device, i.e. whether it supports
/// capture only, render only, or both.
/// </summary>
void VideoDeviceInfo::Direction::set(MediaDirection value)
{
	_direction = value;
}

/// <summary>
/// Gets or sets the device capabilities, as bitmask combination.
/// </summary>
unsigned VideoDeviceInfo::Caps::get()
{
	return _caps;
}

/// <summary>
/// Gets or sets the device capabilities, as bitmask combination.
/// </summary>
void VideoDeviceInfo::Caps::set(unsigned value)
{
	_caps = value;
}

/// <summary>
/// Gets or sets the array of media formats.
/// </summary>
array<MediaFormat^>^ VideoDeviceInfo::MediaFormats::get()
{
	return _mediaFormats;
}

/// <summary>
/// Gets or sets the array of media formats.
/// </summary>
void VideoDeviceInfo::MediaFormats::set(array<MediaFormat^>^ value)
{
	_mediaFormats = value;
}