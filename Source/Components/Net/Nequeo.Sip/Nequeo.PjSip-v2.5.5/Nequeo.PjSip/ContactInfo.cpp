/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContactInfo.cpp
*  Purpose :       SIP ContactInfo class.
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

#include "ContactInfo.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Contact information.
///	</summary>
ContactInfo::ContactInfo() :
	_disposed(false)
{
}

///	<summary>
///	Contact information.
///	</summary>
ContactInfo::~ContactInfo()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the full URI of the contact.
///	</summary>
String^ ContactInfo::Uri::get()
{
	return _uri;
}

///	<summary>
///	Gets or sets the full URI of the contact.
///	</summary>
void ContactInfo::Uri::set(String^ value)
{
	_uri = value;
}

/// <summary>
/// Gets or sets the contact info, only available when presence subscription has
/// been established to the buddy.
/// </summary>
String^ ContactInfo::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the contact info, only available when presence subscription has
/// been established to the buddy.
/// </summary>
void ContactInfo::Info::set(String^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets a flag to indicate that we should monitor the presence information for
/// this buddy(normally yes, unless explicitly disabled).
/// </summary>
bool ContactInfo::PresMonitorEnabled::get()
{
	return _presMonitorEnabled;
}

/// <summary>
/// Gets or sets a flag to indicate that we should monitor the presence information for
/// this buddy(normally yes, unless explicitly disabled).
/// </summary>
void ContactInfo::PresMonitorEnabled::set(bool value)
{
	_presMonitorEnabled = value;
}

/// <summary>
/// Gets or sets If PresMonitorEnabled is true, this specifies the last state of
/// the presence subscription. If presence subscription session is currently
/// active, the value will be EVSUB_STATE_ACTIVE.If presence
/// subscription request has been rejected, the value will be
/// EVSUB_STATE_TERMINATED, and the termination reason will be
/// specified in SubTermReason.
/// </summary>
SubscriptionState ContactInfo::SubState::get()
{
	return _subState;
}

/// <summary>
/// Gets or sets If PresMonitorEnabled is true, this specifies the last state of
/// the presence subscription. If presence subscription session is currently
/// active, the value will be EVSUB_STATE_ACTIVE.If presence
/// subscription request has been rejected, the value will be
/// EVSUB_STATE_TERMINATED, and the termination reason will be
/// specified in SubTermReason.
/// </summary>
void ContactInfo::SubState::set(SubscriptionState value)
{
	_subState = value;
}

/// <summary>
/// Gets or sets the representation of subscription state.
/// </summary>
String^ ContactInfo::SubStateName::get()
{
	return _subStateName;
}

/// <summary>
/// Gets or sets the representation of subscription state.
/// </summary>
void ContactInfo::SubStateName::set(String^ value)
{
	_subStateName = value;
}

/// <summary>
/// Gets or sets the specifies the last presence subscription termination code. This would
/// return the last status of the SUBSCRIBE request.If the subscription
/// is terminated with NOTIFY by the server, this value will be set to
/// 200, and subscription termination reason will be given in the
/// SubTermReason field.
/// </summary>
StatusCode ContactInfo::SubTermCode::get()
{
	return _subTermCode;
}

/// <summary>
/// Gets or sets the specifies the last presence subscription termination code. This would
/// return the last status of the SUBSCRIBE request.If the subscription
/// is terminated with NOTIFY by the server, this value will be set to
/// 200, and subscription termination reason will be given in the
/// SubTermReason field.
/// </summary>
void ContactInfo::SubTermCode::set(StatusCode value)
{
	_subTermCode = value;
}

/// <summary>
/// Gets or sets the Specifies the last presence subscription termination reason. If 
/// presence subscription is currently active, the value will be empty.
/// </summary>
String^ ContactInfo::SubTermReason::get()
{
	return _subTermReason;
}

/// <summary>
/// Gets or sets the Specifies the last presence subscription termination reason. If 
/// presence subscription is currently active, the value will be empty.
/// </summary>
void ContactInfo::SubTermReason::set(String^ value)
{
	_subTermReason = value;
}

/// <summary>
/// Gets or sets the presence status.
/// </summary>
PresenceState^ ContactInfo::PresenceStatus::get()
{
	return _presenceStatus;
}

/// <summary>
/// Gets or sets the presence status.
/// </summary>
void ContactInfo::PresenceStatus::set(PresenceState^ value)
{
	_presenceStatus = value;
}