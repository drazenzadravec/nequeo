/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ConfPortInfo.cpp
*  Purpose :       SIP ConfPortInfo class.
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

#include "ConfPortInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure descibes information about a particular media port that
/// has been registered into the conference bridge.
/// </summary>
ConfPortInfo::ConfPortInfo()
{
}

/// <summary>
/// Gets or sets the conference port number.
/// </summary>
int ConfPortInfo::PortId::get()
{
	return _portId;
}

/// <summary>
/// Gets or sets the conference port number.
/// </summary>
void ConfPortInfo::PortId::set(int value)
{
	_portId = value;
}

/// <summary>
/// Gets or sets the port name.
/// </summary>
String^ ConfPortInfo::Name::get()
{
	return _name;
}

/// <summary>
/// Gets or sets the port name.
/// </summary>
void ConfPortInfo::Name::set(String^ value)
{
	_name = value;
}

/// <summary>
/// Gets or sets the media audio format information.
/// </summary>
MediaFormatAudio^ ConfPortInfo::Format::get()
{
	return _format;
}

/// <summary>
/// Gets or sets the media audio format information.
/// </summary>
void ConfPortInfo::Format::set(MediaFormatAudio^ value)
{
	_format = value;
}

/// <summary>
/// Gets or sets the Tx level adjustment. Value 1.0 means no adjustment, value 0 means
/// the port is muted, value 2.0 means the level is amplified two times.
/// </summary>
float ConfPortInfo::TxLevelAdj::get()
{
	return _txLevelAdj;
}

/// <summary>
/// Gets or sets the Tx level adjustment. Value 1.0 means no adjustment, value 0 means
/// the port is muted, value 2.0 means the level is amplified two times.
/// </summary>
void ConfPortInfo::TxLevelAdj::set(float value)
{
	_txLevelAdj = value;
}

/// <summary>
/// Gets or sets the Rx level adjustment. Value 1.0 means no adjustment, value 0 means
/// the port is muted, value 2.0 means the level is amplified two times.
/// </summary>
float ConfPortInfo::RxLevelAdj::get()
{
	return _rxLevelAdj;
}

/// <summary>
/// Gets or sets the Rx level adjustment. Value 1.0 means no adjustment, value 0 means
/// the port is muted, value 2.0 means the level is amplified two times.
/// </summary>
void ConfPortInfo::RxLevelAdj::set(float value)
{
	_rxLevelAdj = value;
}

/// <summary>
/// Gets or sets the Array of listeners (in other words, ports where this port is transmitting to.
/// </summary>
array<int>^ ConfPortInfo::Listeners::get()
{
	return _listeners;
}

/// <summary>
/// Gets or sets the Array of listeners (in other words, ports where this port is transmitting to.
/// </summary>
void ConfPortInfo::Listeners::set(array<int>^ value)
{
	_listeners = value;
}