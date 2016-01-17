/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallInfo.cpp
*  Purpose :       SIP CallInfo class.
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

#include "CallInfo.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call information. Application can query the call information by calling Call::getInfo().
///	</summary>
CallInfo::CallInfo() :
	_disposed(false)
{
}

///	<summary>
///	Call information. Application can query the call information by calling Call::getInfo().
///	</summary>
CallInfo::~CallInfo()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the call id.
///	</summary>
int CallInfo::Id::get()
{
	return _id;
}

///	<summary>
///	Gets or sets the call id.
///	</summary>
void CallInfo::Id::set(int value)
{
	_id = value;
}

///	<summary>
///	Gets or sets the account id.
///	</summary>
int CallInfo::AccountId::get()
{
	return _accountId;
}

///	<summary>
///	Gets or sets the account id.
///	</summary>
void CallInfo::AccountId::set(int value)
{
	_accountId = value;
}

///	<summary>
///	Gets or sets the call role (UAC == caller).
///	</summary>
CallRole CallInfo::Role::get()
{
	return _role;
}

///	<summary>
///	Gets or sets the call role (UAC == caller).
///	</summary>
void CallInfo::Role::set(CallRole value)
{
	_role = value;
}

///	<summary>
///	Gets or sets the local uri.
///	</summary>
String^ CallInfo::LocalUri::get()
{
	return _localUri;
}

///	<summary>
///	Gets or sets the local uri.
///	</summary>
void CallInfo::LocalUri::set(String^ value)
{
	_localUri = value;
}

///	<summary>
///	Gets or sets the local contact.
///	</summary>
String^ CallInfo::LocalContact::get()
{
	return _localContact;
}

///	<summary>
///	Gets or sets the local contact.
///	</summary>
void CallInfo::LocalContact::set(String^ value)
{
	_localContact = value;
}

///	<summary>
///	Gets or sets the remote uri.
///	</summary>
String^ CallInfo::RemoteUri::get()
{
	return _remoteUri;
}

///	<summary>
///	Gets or sets the remote uri.
///	</summary>
void CallInfo::RemoteUri::set(String^ value)
{
	_remoteUri = value;
}

///	<summary>
///	Gets or sets the remote contact.
///	</summary>
String^ CallInfo::RemoteContact::get()
{
	return _remoteContact;
}

///	<summary>
///	Gets or sets the remote contact.
///	</summary>
void CallInfo::RemoteContact::set(String^ value)
{
	_remoteContact = value;
}

///	<summary>
///	Gets or sets the dialog Call-ID string.
///	</summary>
String^ CallInfo::CallIdString::get()
{
	return _callIdString;
}

///	<summary>
///	Gets or sets the dialog Call-ID string.
///	</summary>
void CallInfo::CallIdString::set(String^ value)
{
	_callIdString = value;
}

///	<summary>
///	Gets or sets the call setting.
///	</summary>
CallSetting^ CallInfo::Setting::get()
{
	return _setting;
}

///	<summary>
///	Gets or sets the call setting.
///	</summary>
void CallInfo::Setting::set(CallSetting^ value)
{
	_setting = value;
}

///	<summary>
///	Gets or sets the call state.
///	</summary>
InviteSessionState CallInfo::State::get()
{
	return _state;
}

///	<summary>
///	Gets or sets the call state.
///	</summary>
void CallInfo::State::set(InviteSessionState value)
{
	_state = value;
}

///	<summary>
///	Gets or sets the text describing the state.
///	</summary>
String^ CallInfo::StateText::get()
{
	return _stateText;
}

///	<summary>
///	Gets or sets the text describing the state.
///	</summary>
void CallInfo::StateText::set(String^ value)
{
	_stateText = value;
}

///	<summary>
///	Gets or sets the last status code heard, which can be used as cause code.
///	</summary>
StatusCode CallInfo::LastStatusCode::get()
{
	return _lastStatusCode;
}

///	<summary>
///	Gets or sets the last status code heard, which can be used as cause code.
///	</summary>
void CallInfo::LastStatusCode::set(StatusCode value)
{
	_lastStatusCode = value;
}

///	<summary>
///	Gets or sets the reason phrase describing the last status.
///	</summary>
String^ CallInfo::LastReason::get()
{
	return _lastReason;
}

///	<summary>
///	Gets or sets the reason phrase describing the last status.
///	</summary>
void CallInfo::LastReason::set(String^ value)
{
	_lastReason = value;
}

///	<summary>
///	Gets or sets the array of active media information.
///	</summary>
array<CallMediaInfo^>^ CallInfo::Media::get()
{
	return _media;
}

///	<summary>
///	Gets or sets the array of active media information.
///	</summary>
void CallInfo::Media::set(array<CallMediaInfo^>^ value)
{
	_media = value;
}

///	<summary>
///	Gets or sets the Array of provisional media information. This contains the media info
/// in the provisioning state, that is when the media session is being
/// created / updated(SDP offer / answer is on progress).
///	</summary>
array<CallMediaInfo^>^ CallInfo::ProvMedia::get()
{
	return _provMedia;
}

///	<summary>
///	Gets or sets the Array of provisional media information. This contains the media info
/// in the provisioning state, that is when the media session is being
/// created / updated(SDP offer / answer is on progress).
///	</summary>
void CallInfo::ProvMedia::set(array<CallMediaInfo^>^ value)
{
	_provMedia = value;
}

///	<summary>
///	Gets or sets the Up-to-date call connected duration (zero when call is not established).
///	</summary>
TimeVal^ CallInfo::ConnectDuration::get()
{
	return _connectDuration;
}

///	<summary>
///	Gets or sets the Up-to-date call connected duration (zero when call is not established).
///	</summary>
void CallInfo::ConnectDuration::set(TimeVal^ value)
{
	_connectDuration = value;
}

///	<summary>
///	Gets or sets the total call duration, including set-up time.
///	</summary>
TimeVal^ CallInfo::TotalDuration::get()
{
	return _totalDuration;
}

///	<summary>
///	Gets or sets the total call duration, including set-up time.
///	</summary>
void CallInfo::TotalDuration::set(TimeVal^ value)
{
	_totalDuration = value;
}

///	<summary>
///	Gets or sets the flag if remote was SDP offerer.
///	</summary>
bool CallInfo::RemOfferer::get()
{
	return _remOfferer;
}

///	<summary>
///	Gets or sets the flag if remote was SDP offerer.
///	</summary>
void CallInfo::RemOfferer::set(bool value)
{
	_remOfferer = value;
}

///	<summary>
///	Gets or sets the number of audio streams offered by remote.
///	</summary>
unsigned CallInfo::RemAudioCount::get()
{
	return _remAudioCount;
}

///	<summary>
///	Gets or sets the number of audio streams offered by remote.
///	</summary>
void CallInfo::RemAudioCount::set(unsigned value)
{
	_remAudioCount = value;
}

///	<summary>
///	Gets or sets the number of video streams offered by remote.
///	</summary>
unsigned CallInfo::RemVideoCount::get()
{
	return _remVideoCount;
}

///	<summary>
///	Gets or sets the number of video streams offered by remote.
///	</summary>
void CallInfo::RemVideoCount::set(unsigned value)
{
	_remVideoCount = value;
}