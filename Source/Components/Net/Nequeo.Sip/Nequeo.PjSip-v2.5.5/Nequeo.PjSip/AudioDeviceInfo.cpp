/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioDeviceInfo.cpp
*  Purpose :       SIP AudioDeviceInfo class.
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

#include "AudioDeviceInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Audio device info.
/// </summary>
AudioDeviceInfo::AudioDeviceInfo()
{
}

/// <summary>
/// Gets or sets the device name.
/// </summary>
String^ AudioDeviceInfo::Name::get()
{
	return _name;
}

/// <summary>
/// Gets or sets the device name.
/// </summary>
void AudioDeviceInfo::Name::set(String^ value)
{
	_name = value;
}

/// <summary>
/// Get or sets the maximum number of input channels supported by this device. If the
/// value is zero, the device does not support input operation (i.e. it is a playback only device).
/// </summary>
unsigned AudioDeviceInfo::InputCount::get()
{
	return _inputCount;
}

/// <summary>
/// Get or sets the maximum number of input channels supported by this device. If the
/// value is zero, the device does not support input operation (i.e. it is a playback only device).
/// </summary>
void AudioDeviceInfo::InputCount::set(unsigned value)
{
	_inputCount = value;
}

/// <summary>
/// Gets or sets the maximum number of output channels supported by this device. If the
/// value is zero, the device does not support output operation(i.e. it is an input only device).
/// </summary>
unsigned AudioDeviceInfo::OutputCount::get()
{
	return _outputCount;
}

/// <summary>
/// Gets or sets the maximum number of output channels supported by this device. If the
/// value is zero, the device does not support output operation(i.e. it is an input only device).
/// </summary>
void AudioDeviceInfo::OutputCount::set(unsigned value)
{
	_outputCount = value;
}

/// <summary>
/// Gets or sets the default sampling rate.
/// </summary>
unsigned AudioDeviceInfo::DefaultSamplesPerSec::get()
{
	return _defaultSamplesPerSec;
}

/// <summary>
/// Gets or sets the default sampling rate.
/// </summary>
void AudioDeviceInfo::DefaultSamplesPerSec::set(unsigned value)
{
	_defaultSamplesPerSec = value;
}

/// <summary>
/// Gets or sets the underlying driver name.
/// </summary>
String^ AudioDeviceInfo::Driver::get()
{
	return _driver;
}

/// <summary>
/// Gets or sets the underlying driver name.
/// </summary>
void AudioDeviceInfo::Driver::set(String^ value)
{
	_driver = value;
}

/// <summary>
/// Gets or sets the device capabilities, as bitmask combination.
/// </summary>
unsigned AudioDeviceInfo::Caps::get()
{
	return _caps;
}

/// <summary>
/// Gets or sets the device capabilities, as bitmask combination.
/// </summary>
void AudioDeviceInfo::Caps::set(unsigned value)
{
	_caps = value;
}

/// <summary>
/// Gets or sets the supported audio device routes, as bitmask combination
/// The value may be zero if the device does not support audio routing.
/// </summary>
unsigned AudioDeviceInfo::Routes::get()
{
	return _routes;
}

/// <summary>
/// Gets or sets the supported audio device routes, as bitmask combination
/// The value may be zero if the device does not support audio routing.
/// </summary>
void AudioDeviceInfo::Routes::set(unsigned value)
{
	_routes = value;
}

/// <summary>
/// Gets or sets the array of media formats.
/// </summary>
array<MediaFormat^>^ AudioDeviceInfo::MediaFormats::get()
{
	return _mediaFormats;
}

/// <summary>
/// Gets or sets the array of media formats.
/// </summary>
void AudioDeviceInfo::MediaFormats::set(array<MediaFormat^>^ value)
{
	_mediaFormats = value;
}