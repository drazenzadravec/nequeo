/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          PresenceState.cpp
*  Purpose :       SIP PresenceState class.
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

#include "PresenceState.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Presence state.
/// </summary>
PresenceState::PresenceState()
{
}

/// <summary>
/// Gets or sets the activity type.
/// </summary>
RpidActivity PresenceState::Activity::get()
{
	return _activity;
}

/// <summary>
/// Gets or sets the activity type.
/// </summary>
void PresenceState::Activity::set(RpidActivity value)
{
	_activity = value;
}

/// <summary>
/// Gets or sets the optional text describing the person/element.
/// </summary>
String^ PresenceState::Note::get()
{
	return _note;
}

/// <summary>
/// Gets or sets the optional text describing the person/element.
/// </summary>
void PresenceState::Note::set(String^ value)
{
	_note = value;
}

/// <summary>
/// Gets or sets the optional RPID ID string.
/// </summary>
String^ PresenceState::RpidId::get()
{
	return _rpidId;
}

/// <summary>
/// Gets or sets the optional RPID ID string.
/// </summary>
void PresenceState::RpidId::set(String^ value)
{
	_rpidId = value;
}

/// <summary>
/// Gets or sets the buddy's online status.
/// </summary>
BuddyStatus PresenceState::Status::get()
{
	return _status;
}

/// <summary>
/// Gets or sets the buddy's online status.
/// </summary>
void PresenceState::Status::set(BuddyStatus value)
{
	_status = value;
}

/// <summary>
/// Gets or sets the text to describe buddy's online status.
/// </summary>
String^ PresenceState::StatusText::get()
{
	return _statusText;
}

/// <summary>
/// Gets or sets the text to describe buddy's online status.
/// </summary>
void PresenceState::StatusText::set(String^ value)
{
	_statusText = value;
}