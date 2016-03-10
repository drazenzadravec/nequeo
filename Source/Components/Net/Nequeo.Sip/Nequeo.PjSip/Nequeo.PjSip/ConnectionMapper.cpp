/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ConnectionMapper.cpp
*  Purpose :       SIP Connectio nMapper class.
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

#include "ConnectionMapper.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Account callbacks.
///	</summary>
ConnectionMapper::ConnectionMapper() :
	_disposed(false)
{
}

///	<summary>
///	Account callbacks.
///	</summary>
ConnectionMapper::~ConnectionMapper()
{
	if (!_disposed)
	{
		_disposed = true;
		_authCreds.clear();
	}
}

///	<summary>
///	Gets or sets the account name or service phone number.
///	</summary>
void ConnectionMapper::SetAccountName(std::string value)
{
	_accountName = value;
}
std::string ConnectionMapper::GetAccountName()
{
	return _accountName;
}

///	<summary>
///	Gets or sets the service provider host name or IP address.
///	</summary>
void ConnectionMapper::SetSpHost(std::string value)
{
	_spHost = value;
}
std::string ConnectionMapper::GetSpHost()
{
	return _spHost;
}

///	<summary>
///	Gets or sets the service provider host name or IP address.
///	</summary>
void ConnectionMapper::SetSpPort(int value)
{
	_spPort = value;
}
int ConnectionMapper::GetSpPort()
{
	return _spPort;
}

///	<summary>
///	Gets or sets the account priority.
///	</summary>
void ConnectionMapper::SetPriority(int value) 
{
	_priority = value;
}
int ConnectionMapper::GetPriority()
{
	return _priority;
}

/// <summary>
/// Gets or sets specify whether calls of the configured account should be dropped
/// after registration failure and an attempt of re-registration has also failed.
/// </summary>
void ConnectionMapper::SetDropCallsOnFail(bool value)
{
	_dropCallsOnFail = value;
}
bool ConnectionMapper::GetDropCallsOnFail()
{
	return _dropCallsOnFail;
}

/// <summary>
/// Gets or sets specify whether the account should register as soon as it is
/// added to the UA.Application can set this to false and control
/// the registration manually with Account.Registration().
/// </summary>
void ConnectionMapper::SetRegisterOnAdd(bool value)
{
	_registerOnAdd = value;
}
bool ConnectionMapper::GetRegisterOnAdd()
{
	return _registerOnAdd;
}

/// <summary>
/// Gets or sets Specify interval of auto registration retry upon registration failure
/// (including caused by transport problem), in second.Set to 0 to
/// disable auto re-registration. Note that if the registration retry
/// occurs because of transport failure, the first retry will be done
/// after FirstRetryIntervalSec seconds instead.
/// </summary>
void ConnectionMapper::SetRetryIntervalSec(unsigned value)
{
	_retryIntervalSec = value;
}
unsigned ConnectionMapper::GetRetryIntervalSec()
{
	return _retryIntervalSec;
}

/// <summary>
/// Gets or sets interval for registration, in seconds. If the value is zero,
/// default interval will be used 300 seconds.
/// </summary>
void ConnectionMapper::SetTimeoutSec(unsigned value)
{
	_timeoutSec = value;
}
unsigned ConnectionMapper::GetTimeoutSec()
{
	return _timeoutSec;
}

/// <summary>
/// Gets or sets specifies the interval for the first registration retry. The
/// registration retry is explained in RetryIntervalSec.
/// </summary>
void ConnectionMapper::SetFirstRetryIntervalSec(unsigned value)
{
	_firstRetryIntervalSec = value;
}
unsigned ConnectionMapper::GetFirstRetryIntervalSec()
{
	return _firstRetryIntervalSec;
}

/// <summary>
/// Gets or sets specify the maximum time to wait for unregistration requests to
/// complete during library shutdown sequence.
/// </summary>
void ConnectionMapper::SetUnregWaitSec(unsigned value)
{
	_unregWaitSec = value;
}
unsigned ConnectionMapper::GetUnregWaitSec()
{
	return _unregWaitSec;
}

/// <summary>
/// Gets or sets specify the number of seconds to refresh the client registration
/// before the registration expires.
/// </summary>
void ConnectionMapper::SetDelayBeforeRefreshSec(unsigned value)
{
	_delayBeforeRefreshSec = value;
}
unsigned ConnectionMapper::GetDelayBeforeRefreshSec()
{
	return _delayBeforeRefreshSec;
}

/// <summary>
/// Gets or sets specify minimum Session Timer expiration period, in seconds.
/// Must not be lower than 90. Default is 90.
/// </summary>
void ConnectionMapper::SetTimerMinSESec(unsigned value)
{
	_timerMinSESec = value;
}
unsigned ConnectionMapper::GetTimerMinSESec()
{
	return _timerMinSESec;
}

/// <summary>
/// Gets or sets specify Session Timer expiration period, in seconds.
/// Must not be lower than timerMinSE.Default is 1800.
/// </summary>
void ConnectionMapper::SetTimerSessExpiresSec(unsigned value)
{
	_timerSessExpiresSec = value;
}
unsigned ConnectionMapper::GetTimerSessExpiresSec()
{
	return _timerSessExpiresSec;
}

/// <summary>
/// Gets or sets specify whether IPv6 should be used on media. Default is not used.
/// </summary>
void ConnectionMapper::SetIPv6Use(IPv6_Use value)
{
	_ipv6_Use = value;
}
IPv6_Use ConnectionMapper::GetIPv6Use()
{
	return _ipv6_Use;
}

/// <summary>
/// Gets or sets specify whether secure media transport should be used for this account.
/// </summary>
void ConnectionMapper::SetSRTPUse(SRTP_Use value)
{
	_srtp_Use = value;
}
SRTP_Use ConnectionMapper::GetSRTPUse()
{
	return _srtp_Use;
}

/// <summary>
/// Gets or sets specify whether SRTP requires secure signaling to be used. This option
/// is only used when SRTPUse option is non-zero.
/// </summary>
void ConnectionMapper::SetSRTPSecureSignaling(SRTP_SecureSignaling value)
{
	_srtp_SecureSignaling = value;
}
SRTP_SecureSignaling ConnectionMapper::GetSRTPSecureSignaling()
{
	return _srtp_SecureSignaling;
}

/// <summary>
/// Gets or sets UDP port number to bind locally. This setting MUST be specified
/// even when default port is desired.If the value is zero, the
/// transport will be bound to any available port, and application
/// can query the port by querying the transport info.
/// </summary>
void ConnectionMapper::SetMediaTransportPort(unsigned value)
{
	_mediaTransportPort = value;
}
unsigned ConnectionMapper::GetMediaTransportPort()
{
	return _mediaTransportPort;
}

/// <summary>
/// Gets or sets specify the port range for socket binding, relative to the start
/// port number specified in MediaTransportPort that this setting is only
/// applicable when the start port number is non zero.
/// </summary>
void ConnectionMapper::SetMediaTransportPortRange(unsigned value)
{
	_mediaTransportPortRange = value;
}
unsigned ConnectionMapper::GetMediaTransportPortRange()
{
	return _mediaTransportPortRange;
}

/// <summary>
/// Gets or sets true to subscribe to message waiting indication events (RFC 3842).
/// </summary>
void ConnectionMapper::SetMessageWaitingIndication(bool value)
{
	_messageWaitingIndication = value;
}
bool ConnectionMapper::GetMessageWaitingIndication()
{
	return _messageWaitingIndication;
}

/// <summary>
/// Gets or sets specify the default expiration time (in seconds) for Message
/// Waiting Indication(RFC 3842) event subscription.This must not be zero.
/// </summary>
void ConnectionMapper::SetMWIExpirationSec(unsigned value)
{
	_mwiExpirationSec = value;
}
unsigned ConnectionMapper::GetMWIExpirationSec()
{
	return _mwiExpirationSec;
}

/// <summary>
/// Gets or sets if this flag is set, the presence information of this account will
/// be PUBLISH-ed to the server where the account belongs.
/// </summary>
void ConnectionMapper::SetPublishEnabled(bool value)
{
	_publishEnabled = value;
}
bool ConnectionMapper::GetPublishEnabled()
{
	return _publishEnabled;
}

/// <summary>
/// Gets or sets specify whether the client publication session should queue the
/// PUBLISH request should there be another PUBLISH transaction still
/// pending.If this is set to false, the client will return error
/// on the PUBLISH request if there is another PUBLISH transaction still
/// in progress.
/// </summary>
void ConnectionMapper::SetPublishQueue(bool value)
{
	_publishQueue = value;
}
bool ConnectionMapper::GetPublishQueue()
{
	return _publishQueue;
}

/// <summary>
/// Gets or sets Maximum time to wait for unpublication transaction(s) to complete
/// during shutdown process, before sending unregistration.The library
/// tries to wait for the unpublication(un-PUBLISH) to complete before
/// sending REGISTER request to unregister the account, during library
/// shutdown process.If the value is set too short, it is possible that
/// the unregistration is sent before unpublication completes, causing
/// unpublication request to fail. Value is in milliseconds.
/// </summary>
void ConnectionMapper::SetPublishShutdownWaitMsec(unsigned value)
{
	_publishShutdownWaitMsec = value;
}
unsigned ConnectionMapper::GetPublishShutdownWaitMsec()
{
	return _publishShutdownWaitMsec;
}

/// <summary>
/// Gets or sets the authentication credentials.
/// </summary>
void ConnectionMapper::SetAuthCredentials(pj::AuthCredInfoVector value)
{
	_authCreds = value;
}
pj::AuthCredInfoVector ConnectionMapper::GetAuthCredentials()
{
	return _authCreds;
}

/// <summary>
/// Gets or sets an indicator specifying this account is the default.
/// </summary>
void ConnectionMapper::SetIsDefault(bool value)
{
	_isDefault = value;
}
bool ConnectionMapper::GetIsDefault()
{
	return _isDefault;
}

/// <summary>
/// Gets or sets an indicator specifying that ice RTCP should not be used: default false.
/// </summary>
void ConnectionMapper::SetNoIceRtcp(bool value)
{
	_noIceRtcp = value;
}
bool ConnectionMapper::GetNoIceRtcp()
{
	return _noIceRtcp;
}

/// <summary>
/// Gets or sets an indicator specifying that ice is enabled: default false.
/// </summary>
void ConnectionMapper::SetIceEnabled(bool value)
{
	_iceEnabled = value;
}
bool ConnectionMapper::GetIceEnabled()
{
	return _iceEnabled;
}

/// <summary>
/// Gets or sets specify the Upstream/outgoing bandwidth. If this is set to zero, the video stream
/// will use codec maximum bitrate setting. Default : 0.
/// </summary>
void ConnectionMapper::SetVideoRateControlBandwidth(unsigned value)
{
	_videoRateControlBandwidth = value;
}
unsigned ConnectionMapper::GetVideoRateControlBandwidth()
{
	return _videoRateControlBandwidth;
}

/// <summary>
/// Gets or sets an indicator specifying that any video capture is done automatically.
/// </summary>
void ConnectionMapper::SetVideoAutoTransmit(bool value)
{
	_videoAutoTransmit = value;
}
bool ConnectionMapper::GetVideoAutoTransmit()
{
	return _videoAutoTransmit;
}

/// <summary>
/// Gets or sets an indicator specifying that any video is shown automatically.
/// </summary>
void ConnectionMapper::SetVideoAutoShow(bool value)
{
	_videoAutoShow = value;
}
bool ConnectionMapper::GetVideoAutoShow()
{
	return _videoAutoShow;
}

/// <summary>
/// Gets or sets the transport type.
/// </summary>
void ConnectionMapper::SetTransportType(TransportType value)
{
	_transportType = value;
}
TransportType ConnectionMapper::GetTransportType()
{
	return _transportType;
}

/// <summary>
/// Get ip v6 use.
/// </summary>
/// <param name="ipv6Use">The current ipv6 use.</param>
/// <returns>The ipv6 use.</returns>
pjsua_ipv6_use ConnectionMapper::GetIPv6UseEx(IPv6_Use ipv6Use)
{
	switch (ipv6Use)
	{
	case Nequeo::Net::PjSip::IPv6_Use::IPV6_DISABLED:
		return pjsua_ipv6_use::PJSUA_IPV6_DISABLED;
	case Nequeo::Net::PjSip::IPv6_Use::IPV6_ENABLED:
		return pjsua_ipv6_use::PJSUA_IPV6_ENABLED;
	default:
		return pjsua_ipv6_use::PJSUA_IPV6_DISABLED;
	}
}

/// <summary>
/// Get srtp use.
/// </summary>
/// <param name="srtpUse">The current srtp use.</param>
/// <returns>The srtp use.</returns>
pjmedia_srtp_use ConnectionMapper::GetSrtpUseEx(SRTP_Use srtpUse)
{
	switch (srtpUse)
	{
	case Nequeo::Net::PjSip::SRTP_Use::SRTP_DISABLED:
		return pjmedia_srtp_use::PJMEDIA_SRTP_DISABLED;
	case Nequeo::Net::PjSip::SRTP_Use::SRTP_OPTIONAL:
		return pjmedia_srtp_use::PJMEDIA_SRTP_OPTIONAL;
	case Nequeo::Net::PjSip::SRTP_Use::SRTP_MANDATORY:
		return pjmedia_srtp_use::PJMEDIA_SRTP_MANDATORY;
	default:
		return pjmedia_srtp_use::PJMEDIA_SRTP_DISABLED;
	}
}

/// <summary>
/// Get srtp secure signaling.
/// </summary>
/// <param name="srtpSecureSignaling">The current srtp secure signaling.</param>
/// <returns>The srtp secure signaling.</returns>
int ConnectionMapper::GetSRTPSecureSignalingEx(SRTP_SecureSignaling srtpSecureSignaling)
{
	switch (srtpSecureSignaling)
	{
	case Nequeo::Net::PjSip::SRTP_SecureSignaling::SRTP_SECURESIGNALING_DISABLED:
		return 0;
	case Nequeo::Net::PjSip::SRTP_SecureSignaling::SRTP_SECURESIGNALING_REQUIRES:
		return 1;
	case Nequeo::Net::PjSip::SRTP_SecureSignaling::SRTP_SECURESIGNALING_REQUIRES_END_TO_END:
		return 2;
	default:
		return 0;
	}
}

/// <summary>
/// Get the activity.
/// </summary>
/// <param name="activity">The current activity.</param>
/// <returns>The activity.</returns>
pjrpid_activity ConnectionMapper::GetActivityEx(RpidActivity activity)
{
	switch (activity)
	{
	case Nequeo::Net::PjSip::RpidActivity::UNKNOWN:
		return pjrpid_activity::PJRPID_ACTIVITY_UNKNOWN;
	case Nequeo::Net::PjSip::RpidActivity::AWAY:
		return pjrpid_activity::PJRPID_ACTIVITY_AWAY;
	case Nequeo::Net::PjSip::RpidActivity::BUSY:
		return pjrpid_activity::PJRPID_ACTIVITY_BUSY;
	default:
		return pjrpid_activity::PJRPID_ACTIVITY_UNKNOWN;
	}
}

/// <summary>
/// Get the activity.
/// </summary>
/// <param name="activity">The current activity.</param>
/// <returns>The activity.</returns>
RpidActivity ConnectionMapper::GetActivityEx(pjrpid_activity activity)
{
	switch (activity)
	{
	case PJRPID_ACTIVITY_UNKNOWN:
		return RpidActivity::UNKNOWN;
	case PJRPID_ACTIVITY_AWAY:
		return RpidActivity::AWAY;
	case PJRPID_ACTIVITY_BUSY:
		return RpidActivity::BUSY;
	default:
		return RpidActivity::UNKNOWN;
	}
}

/// <summary>
/// Get the buddy status.
/// </summary>
/// <param name="status">The current buddy status.</param>
/// <returns>The buddy status.</returns>
pjsua_buddy_status ConnectionMapper::GetBuddyStatusEx(BuddyStatus status)
{
	switch (status)
	{
	case Nequeo::Net::PjSip::BuddyStatus::UNKNOWN:
		return pjsua_buddy_status::PJSUA_BUDDY_STATUS_UNKNOWN;
	case Nequeo::Net::PjSip::BuddyStatus::ONLINE:
		return pjsua_buddy_status::PJSUA_BUDDY_STATUS_ONLINE;
	case Nequeo::Net::PjSip::BuddyStatus::OFFLINE:
		return pjsua_buddy_status::PJSUA_BUDDY_STATUS_OFFLINE;
	default:
		return pjsua_buddy_status::PJSUA_BUDDY_STATUS_UNKNOWN;
	}
}

/// <summary>
/// Get the buddy status.
/// </summary>
/// <param name="status">The current buddy status.</param>
/// <returns>The buddy status.</returns>
BuddyStatus ConnectionMapper::GetBuddyStatusEx(pjsua_buddy_status status)
{
	switch (status)
	{
	case PJSUA_BUDDY_STATUS_UNKNOWN:
		return BuddyStatus::UNKNOWN;
	case PJSUA_BUDDY_STATUS_ONLINE:
		return BuddyStatus::ONLINE;
	case PJSUA_BUDDY_STATUS_OFFLINE:
		return BuddyStatus::OFFLINE;
	default:
		return BuddyStatus::UNKNOWN;
	}
}

/// <summary>
/// Get subscription state.
/// </summary>
/// <param name="subscriptionState">The current subscription state.</param>
/// <returns>The subscription state.</returns>
SubscriptionState ConnectionMapper::GetSubscriptionStateEx(pjsip_evsub_state subscriptionState)
{
	switch (subscriptionState)
	{
	case PJSIP_EVSUB_STATE_NULL:
		return SubscriptionState::EVSUB_STATE_NULL;
	case PJSIP_EVSUB_STATE_SENT:
		return SubscriptionState::EVSUB_STATE_SENT;
	case PJSIP_EVSUB_STATE_ACCEPTED:
		return SubscriptionState::EVSUB_STATE_ACCEPTED;
	case PJSIP_EVSUB_STATE_PENDING:
		return SubscriptionState::EVSUB_STATE_PENDING;
	case PJSIP_EVSUB_STATE_ACTIVE:
		return SubscriptionState::EVSUB_STATE_ACTIVE;
	case PJSIP_EVSUB_STATE_TERMINATED:
		return SubscriptionState::EVSUB_STATE_TERMINATED;
	case PJSIP_EVSUB_STATE_UNKNOWN:
		return SubscriptionState::EVSUB_STATE_UNKNOWN;
	default:
		return SubscriptionState::EVSUB_STATE_UNKNOWN;
	}
}

/// <summary>
/// Get the status code.
/// </summary>
/// <param name="statusCode">The current status code.</param>
/// <returns>The status code.</returns>
StatusCode ConnectionMapper::GetStatusCodeEx(pjsip_status_code statusCode)
{
	switch (statusCode)
	{
	case PJSIP_SC_TRYING:
		return StatusCode::SC_TRYING;
	case PJSIP_SC_RINGING:
		return StatusCode::SC_RINGING;
	case PJSIP_SC_CALL_BEING_FORWARDED:
		return StatusCode::SC_CALL_BEING_FORWARDED;
	case PJSIP_SC_QUEUED:
		return StatusCode::SC_QUEUED;
	case PJSIP_SC_PROGRESS:
		return StatusCode::SC_PROGRESS;
	case PJSIP_SC_OK:
		return StatusCode::SC_OK;
	case PJSIP_SC_ACCEPTED:
		return StatusCode::SC_ACCEPTED;
	case PJSIP_SC_MULTIPLE_CHOICES:
		return StatusCode::SC_MULTIPLE_CHOICES;
	case PJSIP_SC_MOVED_PERMANENTLY:
		return StatusCode::SC_MOVED_PERMANENTLY;
	case PJSIP_SC_MOVED_TEMPORARILY:
		return StatusCode::SC_MOVED_TEMPORARILY;
	case PJSIP_SC_USE_PROXY:
		return StatusCode::SC_USE_PROXY;
	case PJSIP_SC_ALTERNATIVE_SERVICE:
		return StatusCode::SC_ALTERNATIVE_SERVICE;
	case PJSIP_SC_BAD_REQUEST:
		return StatusCode::SC_BAD_REQUEST;
	case PJSIP_SC_UNAUTHORIZED:
		return StatusCode::SC_UNAUTHORIZED;
	case PJSIP_SC_PAYMENT_REQUIRED:
		return StatusCode::SC_PAYMENT_REQUIRED;
	case PJSIP_SC_FORBIDDEN:
		return StatusCode::SC_FORBIDDEN;
	case PJSIP_SC_NOT_FOUND:
		return StatusCode::SC_NOT_FOUND;
	case PJSIP_SC_METHOD_NOT_ALLOWED:
		return StatusCode::SC_METHOD_NOT_ALLOWED;
	case PJSIP_SC_NOT_ACCEPTABLE:
		return StatusCode::SC_NOT_ACCEPTABLE;
	case PJSIP_SC_PROXY_AUTHENTICATION_REQUIRED:
		return StatusCode::SC_PROXY_AUTHENTICATION_REQUIRED;
	case PJSIP_SC_REQUEST_TIMEOUT:
		return StatusCode::SC_REQUEST_TIMEOUT;
	case PJSIP_SC_GONE:
		return StatusCode::SC_GONE;
	case PJSIP_SC_REQUEST_ENTITY_TOO_LARGE:
		return StatusCode::SC_REQUEST_ENTITY_TOO_LARGE;
	case PJSIP_SC_REQUEST_URI_TOO_LONG:
		return StatusCode::SC_REQUEST_URI_TOO_LONG;
	case PJSIP_SC_UNSUPPORTED_MEDIA_TYPE:
		return StatusCode::SC_UNSUPPORTED_MEDIA_TYPE;
	case PJSIP_SC_UNSUPPORTED_URI_SCHEME:
		return StatusCode::SC_UNSUPPORTED_URI_SCHEME;
	case PJSIP_SC_BAD_EXTENSION:
		return StatusCode::SC_BAD_EXTENSION;
	case PJSIP_SC_EXTENSION_REQUIRED:
		return StatusCode::SC_EXTENSION_REQUIRED;
	case PJSIP_SC_SESSION_TIMER_TOO_SMALL:
		return StatusCode::SC_SESSION_TIMER_TOO_SMALL;
	case PJSIP_SC_INTERVAL_TOO_BRIEF:
		return StatusCode::SC_INTERVAL_TOO_BRIEF;
	case PJSIP_SC_TEMPORARILY_UNAVAILABLE:
		return StatusCode::SC_TEMPORARILY_UNAVAILABLE;
	case PJSIP_SC_CALL_TSX_DOES_NOT_EXIST:
		return StatusCode::SC_CALL_TSX_DOES_NOT_EXIST;
	case PJSIP_SC_LOOP_DETECTED:
		return StatusCode::SC_LOOP_DETECTED;
	case PJSIP_SC_TOO_MANY_HOPS:
		return StatusCode::SC_TOO_MANY_HOPS;
	case PJSIP_SC_ADDRESS_INCOMPLETE:
		return StatusCode::SC_ADDRESS_INCOMPLETE;
	case PJSIP_AC_AMBIGUOUS:
		return StatusCode::AC_AMBIGUOUS;
	case PJSIP_SC_BUSY_HERE:
		return StatusCode::SC_BUSY_HERE;
	case PJSIP_SC_REQUEST_TERMINATED:
		return StatusCode::SC_REQUEST_TERMINATED;
	case PJSIP_SC_NOT_ACCEPTABLE_HERE:
		return StatusCode::SC_NOT_ACCEPTABLE_HERE;
	case PJSIP_SC_BAD_EVENT:
		return StatusCode::SC_BAD_EVENT;
	case PJSIP_SC_REQUEST_UPDATED:
		return StatusCode::SC_REQUEST_UPDATED;
	case PJSIP_SC_REQUEST_PENDING:
		return StatusCode::SC_REQUEST_PENDING;
	case PJSIP_SC_UNDECIPHERABLE:
		return StatusCode::SC_UNDECIPHERABLE;
	case PJSIP_SC_INTERNAL_SERVER_ERROR:
		return StatusCode::SC_INTERNAL_SERVER_ERROR;
	case PJSIP_SC_NOT_IMPLEMENTED:
		return StatusCode::SC_NOT_IMPLEMENTED;
	case PJSIP_SC_BAD_GATEWAY:
		return StatusCode::SC_BAD_GATEWAY;
	case PJSIP_SC_SERVICE_UNAVAILABLE:
		return StatusCode::SC_SERVICE_UNAVAILABLE;
	case PJSIP_SC_SERVER_TIMEOUT:
		return StatusCode::SC_SERVER_TIMEOUT;
	case PJSIP_SC_VERSION_NOT_SUPPORTED:
		return StatusCode::SC_VERSION_NOT_SUPPORTED;
	case PJSIP_SC_MESSAGE_TOO_LARGE:
		return StatusCode::SC_MESSAGE_TOO_LARGE;
	case PJSIP_SC_PRECONDITION_FAILURE:
		return StatusCode::SC_PRECONDITION_FAILURE;
	case PJSIP_SC_BUSY_EVERYWHERE:
		return StatusCode::SC_BUSY_EVERYWHERE;
	case PJSIP_SC_DECLINE:
		return StatusCode::SC_DECLINE;
	case PJSIP_SC_DOES_NOT_EXIST_ANYWHERE:
		return StatusCode::SC_DOES_NOT_EXIST_ANYWHERE;
	case PJSIP_SC_NOT_ACCEPTABLE_ANYWHERE:
		return StatusCode::SC_NOT_ACCEPTABLE_ANYWHERE;
	case PJSIP_SC__force_32bit:
		return StatusCode::SC__force_32bit;
	default:
		return StatusCode::SC__force_32bit;
	}
}

/// <summary>
/// Get the status code.
/// </summary>
/// <param name="statusCode">The current status code.</param>
/// <returns>The status code.</returns>
pjsip_status_code ConnectionMapper::GetStatusCodeEx(StatusCode statusCode)
{
	switch (statusCode)
	{
	case Nequeo::Net::PjSip::StatusCode::SC_TRYING:
		return pjsip_status_code::PJSIP_SC_TRYING;
	case Nequeo::Net::PjSip::StatusCode::SC_RINGING:
		return pjsip_status_code::PJSIP_SC_RINGING;
	case Nequeo::Net::PjSip::StatusCode::SC_CALL_BEING_FORWARDED:
		return pjsip_status_code::PJSIP_SC_CALL_BEING_FORWARDED;
	case Nequeo::Net::PjSip::StatusCode::SC_QUEUED:
		return pjsip_status_code::PJSIP_SC_QUEUED;
	case Nequeo::Net::PjSip::StatusCode::SC_PROGRESS:
		return pjsip_status_code::PJSIP_SC_PROGRESS;
	case Nequeo::Net::PjSip::StatusCode::SC_OK:
		return pjsip_status_code::PJSIP_SC_OK;
	case Nequeo::Net::PjSip::StatusCode::SC_ACCEPTED:
		return pjsip_status_code::PJSIP_SC_ACCEPTED;
	case Nequeo::Net::PjSip::StatusCode::SC_MULTIPLE_CHOICES:
		return pjsip_status_code::PJSIP_SC_MULTIPLE_CHOICES;
	case Nequeo::Net::PjSip::StatusCode::SC_MOVED_PERMANENTLY:
		return pjsip_status_code::PJSIP_SC_MOVED_PERMANENTLY;
	case Nequeo::Net::PjSip::StatusCode::SC_MOVED_TEMPORARILY:
		return pjsip_status_code::PJSIP_SC_MOVED_TEMPORARILY;
	case Nequeo::Net::PjSip::StatusCode::SC_USE_PROXY:
		return pjsip_status_code::PJSIP_SC_USE_PROXY;
	case Nequeo::Net::PjSip::StatusCode::SC_ALTERNATIVE_SERVICE:
		return pjsip_status_code::PJSIP_SC_ALTERNATIVE_SERVICE;
	case Nequeo::Net::PjSip::StatusCode::SC_BAD_REQUEST:
		return pjsip_status_code::PJSIP_SC_BAD_REQUEST;
	case Nequeo::Net::PjSip::StatusCode::SC_UNAUTHORIZED:
		return pjsip_status_code::PJSIP_SC_UNAUTHORIZED;
	case Nequeo::Net::PjSip::StatusCode::SC_PAYMENT_REQUIRED:
		return pjsip_status_code::PJSIP_SC_PAYMENT_REQUIRED;
	case Nequeo::Net::PjSip::StatusCode::SC_FORBIDDEN:
		return pjsip_status_code::PJSIP_SC_FORBIDDEN;
	case Nequeo::Net::PjSip::StatusCode::SC_NOT_FOUND:
		return pjsip_status_code::PJSIP_SC_NOT_FOUND;
	case Nequeo::Net::PjSip::StatusCode::SC_METHOD_NOT_ALLOWED:
		return pjsip_status_code::PJSIP_SC_METHOD_NOT_ALLOWED;
	case Nequeo::Net::PjSip::StatusCode::SC_NOT_ACCEPTABLE:
		return pjsip_status_code::PJSIP_SC_NOT_ACCEPTABLE;
	case Nequeo::Net::PjSip::StatusCode::SC_PROXY_AUTHENTICATION_REQUIRED:
		return pjsip_status_code::PJSIP_SC_PROXY_AUTHENTICATION_REQUIRED;
	case Nequeo::Net::PjSip::StatusCode::SC_REQUEST_TIMEOUT:
		return pjsip_status_code::PJSIP_SC_REQUEST_TIMEOUT;
	case Nequeo::Net::PjSip::StatusCode::SC_TSX_TIMEOUT:
		return pjsip_status_code::PJSIP_SC_TSX_TIMEOUT;
	case Nequeo::Net::PjSip::StatusCode::SC_GONE:
		return pjsip_status_code::PJSIP_SC_GONE;
	case Nequeo::Net::PjSip::StatusCode::SC_REQUEST_ENTITY_TOO_LARGE:
		return pjsip_status_code::PJSIP_SC_REQUEST_ENTITY_TOO_LARGE;
	case Nequeo::Net::PjSip::StatusCode::SC_REQUEST_URI_TOO_LONG:
		return pjsip_status_code::PJSIP_SC_REQUEST_URI_TOO_LONG;
	case Nequeo::Net::PjSip::StatusCode::SC_UNSUPPORTED_MEDIA_TYPE:
		return pjsip_status_code::PJSIP_SC_UNSUPPORTED_MEDIA_TYPE;
	case Nequeo::Net::PjSip::StatusCode::SC_UNSUPPORTED_URI_SCHEME:
		return pjsip_status_code::PJSIP_SC_UNSUPPORTED_URI_SCHEME;
	case Nequeo::Net::PjSip::StatusCode::SC_BAD_EXTENSION:
		return pjsip_status_code::PJSIP_SC_BAD_EXTENSION;
	case Nequeo::Net::PjSip::StatusCode::SC_EXTENSION_REQUIRED:
		return pjsip_status_code::PJSIP_SC_EXTENSION_REQUIRED;
	case Nequeo::Net::PjSip::StatusCode::SC_SESSION_TIMER_TOO_SMALL:
		return pjsip_status_code::PJSIP_SC_SESSION_TIMER_TOO_SMALL;
	case Nequeo::Net::PjSip::StatusCode::SC_INTERVAL_TOO_BRIEF:
		return pjsip_status_code::PJSIP_SC_INTERVAL_TOO_BRIEF;
	case Nequeo::Net::PjSip::StatusCode::SC_TEMPORARILY_UNAVAILABLE:
		return pjsip_status_code::PJSIP_SC_TEMPORARILY_UNAVAILABLE;
	case Nequeo::Net::PjSip::StatusCode::SC_CALL_TSX_DOES_NOT_EXIST:
		return pjsip_status_code::PJSIP_SC_CALL_TSX_DOES_NOT_EXIST;
	case Nequeo::Net::PjSip::StatusCode::SC_LOOP_DETECTED:
		return pjsip_status_code::PJSIP_SC_LOOP_DETECTED;
	case Nequeo::Net::PjSip::StatusCode::SC_TOO_MANY_HOPS:
		return pjsip_status_code::PJSIP_SC_TOO_MANY_HOPS;
	case Nequeo::Net::PjSip::StatusCode::SC_ADDRESS_INCOMPLETE:
		return pjsip_status_code::PJSIP_SC_ADDRESS_INCOMPLETE;
	case Nequeo::Net::PjSip::StatusCode::AC_AMBIGUOUS:
		return pjsip_status_code::PJSIP_AC_AMBIGUOUS;
	case Nequeo::Net::PjSip::StatusCode::SC_BUSY_HERE:
		return pjsip_status_code::PJSIP_SC_BUSY_HERE;
	case Nequeo::Net::PjSip::StatusCode::SC_REQUEST_TERMINATED:
		return pjsip_status_code::PJSIP_SC_REQUEST_TERMINATED;
	case Nequeo::Net::PjSip::StatusCode::SC_NOT_ACCEPTABLE_HERE:
		return pjsip_status_code::PJSIP_SC_NOT_ACCEPTABLE_HERE;
	case Nequeo::Net::PjSip::StatusCode::SC_BAD_EVENT:
		return pjsip_status_code::PJSIP_SC_BAD_EVENT;
	case Nequeo::Net::PjSip::StatusCode::SC_REQUEST_UPDATED:
		return pjsip_status_code::PJSIP_SC_REQUEST_UPDATED;
	case Nequeo::Net::PjSip::StatusCode::SC_REQUEST_PENDING:
		return pjsip_status_code::PJSIP_SC_REQUEST_PENDING;
	case Nequeo::Net::PjSip::StatusCode::SC_UNDECIPHERABLE:
		return pjsip_status_code::PJSIP_SC_UNDECIPHERABLE;
	case Nequeo::Net::PjSip::StatusCode::SC_INTERNAL_SERVER_ERROR:
		return pjsip_status_code::PJSIP_SC_INTERNAL_SERVER_ERROR;
	case Nequeo::Net::PjSip::StatusCode::SC_NOT_IMPLEMENTED:
		return pjsip_status_code::PJSIP_SC_NOT_IMPLEMENTED;
	case Nequeo::Net::PjSip::StatusCode::SC_BAD_GATEWAY:
		return pjsip_status_code::PJSIP_SC_BAD_GATEWAY;
	case Nequeo::Net::PjSip::StatusCode::SC_SERVICE_UNAVAILABLE:
		return pjsip_status_code::PJSIP_SC_SERVICE_UNAVAILABLE;
	case Nequeo::Net::PjSip::StatusCode::SC_TSX_TRANSPORT_ERROR:
		return pjsip_status_code::PJSIP_SC_TSX_TRANSPORT_ERROR;
	case Nequeo::Net::PjSip::StatusCode::SC_SERVER_TIMEOUT:
		return pjsip_status_code::PJSIP_SC_SERVER_TIMEOUT;
	case Nequeo::Net::PjSip::StatusCode::SC_VERSION_NOT_SUPPORTED:
		return pjsip_status_code::PJSIP_SC_VERSION_NOT_SUPPORTED;
	case Nequeo::Net::PjSip::StatusCode::SC_MESSAGE_TOO_LARGE:
		return pjsip_status_code::PJSIP_SC_MESSAGE_TOO_LARGE;
	case Nequeo::Net::PjSip::StatusCode::SC_PRECONDITION_FAILURE:
		return pjsip_status_code::PJSIP_SC_PRECONDITION_FAILURE;
	case Nequeo::Net::PjSip::StatusCode::SC_BUSY_EVERYWHERE:
		return pjsip_status_code::PJSIP_SC_BUSY_EVERYWHERE;
	case Nequeo::Net::PjSip::StatusCode::SC_DECLINE:
		return pjsip_status_code::PJSIP_SC_DECLINE;
	case Nequeo::Net::PjSip::StatusCode::SC_DOES_NOT_EXIST_ANYWHERE:
		return pjsip_status_code::PJSIP_SC_DOES_NOT_EXIST_ANYWHERE;
	case Nequeo::Net::PjSip::StatusCode::SC_NOT_ACCEPTABLE_ANYWHERE:
		return pjsip_status_code::PJSIP_SC_NOT_ACCEPTABLE_ANYWHERE;
	case Nequeo::Net::PjSip::StatusCode::SC__force_32bit:
		return pjsip_status_code::PJSIP_SC__force_32bit;
	default:
		return pjsip_status_code::PJSIP_SC__force_32bit;
	}
}