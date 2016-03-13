/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountConnection.cpp
*  Purpose :       SIP Account Connection class.
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

#include "AccountConnection.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Account connection configuration.
///	</summary>
AccountConnection::AccountConnection() :
	_disposed(false), _isDefault(true), _accountName(nullptr), _spHost(nullptr), _spPort(5060), _priority(1), _dropCallsOnFail(true), _registerOnAdd(false), _retryIntervalSec(10),
	_timeoutSec(300), _firstRetryIntervalSec(10), _unregWaitSec(30), _delayBeforeRefreshSec(10), _timerMinSESec(90), _timerSessExpiresSec(1800), _noIceRtcp(false), _iceEnabled(false),
	_ipv6_Use(Nequeo::Net::PjSip::IPv6_Use::IPV6_DISABLED), _srtp_Use(Nequeo::Net::PjSip::SRTP_Use::SRTP_DISABLED), _srtp_SecureSignaling(Nequeo::Net::PjSip::SRTP_SecureSignaling::SRTP_SECURESIGNALING_DISABLED),
	_mediaTransportPort(0), _mediaTransportPortRange(0), _messageWaitingIndication(true), _mwiExpirationSec(3600), _publishEnabled(false), _publishQueue(true), _publishShutdownWaitMsec(2000),
	_videoAutoTransmit(false), _videoAutoShow(false), _videoRateControlBandwidth(0)
{
}

///	<summary>
///	Account connection configuration.
///	</summary>
/// <param name="accountName">The account name or service phone number.</param>
/// <param name="spHost">The service provider host name or IP address.</param>
/// <param name="username">The sip username.</param>
/// <param name="password">The sip password.</param>
AccountConnection::AccountConnection(String^ accountName, String^ spHost, String^ username, String^ password) :
	_disposed(false), _isDefault(true), _spPort(5060), _priority(1), _dropCallsOnFail(true), _registerOnAdd(false), _retryIntervalSec(10),
	_timeoutSec(300), _firstRetryIntervalSec(10), _unregWaitSec(30), _delayBeforeRefreshSec(10), _timerMinSESec(90), _timerSessExpiresSec(1800), _noIceRtcp(false), _iceEnabled(false),
	_ipv6_Use(Nequeo::Net::PjSip::IPv6_Use::IPV6_DISABLED), _srtp_Use(Nequeo::Net::PjSip::SRTP_Use::SRTP_DISABLED), _srtp_SecureSignaling(Nequeo::Net::PjSip::SRTP_SecureSignaling::SRTP_SECURESIGNALING_DISABLED),
	_mediaTransportPort(0), _mediaTransportPortRange(0), _messageWaitingIndication(true), _mwiExpirationSec(3600), _publishEnabled(false), _publishQueue(true), _publishShutdownWaitMsec(2000),
	_videoAutoTransmit(false), _videoAutoShow(false), _videoRateControlBandwidth(0)
{
	_accountName = accountName;
	_spHost = spHost;

	// Create a single credential.
	_authCreds = gcnew AuthenticateCredentials();
	AuthCredInfo^ authCredInfo = gcnew AuthCredInfo(username, password);
	_authCreds->AuthCredentials[0] = authCredInfo;
}

///	<summary>
///	Account connection configuration.
///	</summary>
AccountConnection::~AccountConnection()
{
	if (!_disposed)
	{
		_disposed = true;

		if (_authCreds != nullptr)
			delete _authCreds;
	}
}

///	<summary>
///	Gets the account name or service phone number.
///	</summary>
String^ AccountConnection::AccountName::get()
{
	return _accountName;
}

///	<summary>
///	Sets the account name or service phone number.
///	</summary>
void AccountConnection::AccountName::set(String^ value)
{
	_accountName = value;
}

///	<summary>
///	Gets the service provider host name or IP address.
///	</summary>
String^ AccountConnection::SpHost::get()
{
	return _spHost;
}

///	<summary>
///	Sets the service provider host name or IP address.
///	</summary>
void AccountConnection::SpHost::set(String^ value)
{
	_spHost = value;
}

///	<summary>
///	Gets the account priority.
///	</summary>
int AccountConnection::SpPort::get()
{
	return _spPort;
}

///	<summary>
///	Sets the account priority.
///	</summary>
void AccountConnection::SpPort::set(int value)
{
	_spPort = value;
}

///	<summary>
///	Gets the account priority.
///	</summary>
int AccountConnection::Priority::get()
{
	return _priority;
}

///	<summary>
///	Sets the account priority.
///	</summary>
void AccountConnection::Priority::set(int value)
{
	_priority = value;
}

/// <summary>
/// Gets specify whether calls of the configured account should be dropped
/// after registration failure and an attempt of re-registration has also failed.
/// </summary>
bool AccountConnection::DropCallsOnFail::get()
{
	return _dropCallsOnFail;
}

/// <summary>
/// Sets specify whether calls of the configured account should be dropped
/// after registration failure and an attempt of re-registration has also failed.
/// </summary>
void AccountConnection::DropCallsOnFail::set(bool value)
{
	_dropCallsOnFail = value;
}

/// <summary>
/// Gets specify whether the account should register as soon as it is
/// added to the UA.Application can set this to false and control
/// the registration manually with Account.Registration().
/// </summary>
bool AccountConnection::RegisterOnAdd::get()
{
	return _registerOnAdd;
}

/// <summary>
/// Sets specify whether the account should register as soon as it is
/// added to the UA.Application can set this to false and control
/// the registration manually with Account.Registration().
/// </summary>
void AccountConnection::RegisterOnAdd::set(bool value)
{
	_registerOnAdd = value;
}

/// <summary>
/// Gets specify interval of auto registration retry upon registration failure
/// (including caused by transport problem), in second.Set to 0 to
/// disable auto re-registration. Note that if the registration retry
/// occurs because of transport failure, the first retry will be done
/// after FirstRetryIntervalSec seconds instead.
/// </summary>
unsigned AccountConnection::RetryIntervalSec::get()
{
	return _retryIntervalSec;
}

/// <summary>
/// Sets specify interval of auto registration retry upon registration failure
/// (including caused by transport problem), in second.Set to 0 to
/// disable auto re-registration. Note that if the registration retry
/// occurs because of transport failure, the first retry will be done
/// after FirstRetryIntervalSec seconds instead.
/// </summary>
void AccountConnection::RetryIntervalSec::set(unsigned value)
{
	_retryIntervalSec = value;
}

/// <summary>
/// Gets interval for registration, in seconds. If the value is zero,
/// default interval will be used 300 seconds.
/// </summary>
unsigned AccountConnection::TimeoutSec::get()
{
	return _timeoutSec;
}

/// <summary>
/// Sets interval for registration, in seconds. If the value is zero,
/// default interval will be used 300 seconds.
/// </summary>
void AccountConnection::TimeoutSec::set(unsigned value)
{
	_timeoutSec = value;
}

/// <summary>
/// Gets specifies the interval for the first registration retry. The
/// registration retry is explained in RetryIntervalSec.
/// </summary>
unsigned AccountConnection::FirstRetryIntervalSec::get()
{
	return _firstRetryIntervalSec;
}

/// <summary>
/// Sets specifies the interval for the first registration retry. The
/// registration retry is explained in RetryIntervalSec.
/// </summary>
void AccountConnection::FirstRetryIntervalSec::set(unsigned value)
{
	_firstRetryIntervalSec = value;
}

/// <summary>
/// Gets specify the maximum time to wait for unregistration requests to
/// complete during library shutdown sequence.
/// </summary>
unsigned AccountConnection::UnregWaitSec::get()
{
	return _unregWaitSec;
}

/// <summary>
/// Sets specify the maximum time to wait for unregistration requests to
/// complete during library shutdown sequence.
/// </summary>
void AccountConnection::UnregWaitSec::set(unsigned value)
{
	_unregWaitSec = value;
}

/// <summary>
/// Gets specify the number of seconds to refresh the client registration
/// before the registration expires.
/// </summary>
unsigned AccountConnection::DelayBeforeRefreshSec::get()
{
	return _delayBeforeRefreshSec;
}

/// <summary>
/// Sets specify the number of seconds to refresh the client registration
/// before the registration expires.
/// </summary>
void AccountConnection::DelayBeforeRefreshSec::set(unsigned value)
{
	_delayBeforeRefreshSec = value;
}

/// <summary>
/// Gets specify minimum Session Timer expiration period, in seconds.
/// Must not be lower than 90. Default is 90.
/// </summary>
unsigned AccountConnection::TimerMinSESec::get()
{
	return _timerMinSESec;
}

/// <summary>
/// Sets specify minimum Session Timer expiration period, in seconds.
/// Must not be lower than 90. Default is 90.
/// </summary>
void AccountConnection::TimerMinSESec::set(unsigned value)
{
	_timerMinSESec = value;
}

/// <summary>
/// Gets specify Session Timer expiration period, in seconds.
/// Must not be lower than timerMinSE.Default is 1800.
/// </summary>
unsigned AccountConnection::TimerSessExpiresSec::get()
{
	return _timerSessExpiresSec;
}

/// <summary>
/// Sets specify Session Timer expiration period, in seconds.
/// Must not be lower than timerMinSE.Default is 1800.
/// </summary>
void AccountConnection::TimerSessExpiresSec::set(unsigned value)
{
	_timerSessExpiresSec = value;
}

/// <summary>
/// Gets specify whether IPv6 should be used on media. Default is not used.
/// </summary>
IPv6_Use AccountConnection::IPv6Use::get()
{
	return _ipv6_Use;
}

/// <summary>
/// Sets specify whether IPv6 should be used on media. Default is not used.
/// </summary>
void AccountConnection::IPv6Use::set(IPv6_Use value)
{
	_ipv6_Use = value;
}

/// <summary>
/// Gets specify whether secure media transport should be used for this account.
/// </summary>
SRTP_Use AccountConnection::SRTPUse::get()
{
	return _srtp_Use;
}

/// <summary>
/// Sets specify whether secure media transport should be used for this account.
/// </summary>
void AccountConnection::SRTPUse::set(SRTP_Use value)
{
	_srtp_Use = value;
}

/// <summary>
/// Gets specify whether SRTP requires secure signaling to be used. This option
/// is only used when SRTPUse option is non-zero.
/// </summary>
SRTP_SecureSignaling AccountConnection::SRTPSecureSignaling::get()
{
	return _srtp_SecureSignaling;
}

/// <summary>
/// Sets specify whether SRTP requires secure signaling to be used. This option
/// is only used when SRTPUse option is non-zero.
/// </summary>
void AccountConnection::SRTPSecureSignaling::set(SRTP_SecureSignaling value)
{
	_srtp_SecureSignaling = value;
}

/// <summary>
/// Gets UDP port number to bind locally. This setting MUST be specified
/// even when default port is desired.If the value is zero, the
/// transport will be bound to any available port, and application
/// can query the port by querying the transport info.
/// </summary>
unsigned AccountConnection::MediaTransportPort::get()
{
	return _mediaTransportPort;
}

/// <summary>
/// Sets UDP port number to bind locally. This setting MUST be specified
/// even when default port is desired.If the value is zero, the
/// transport will be bound to any available port, and application
/// can query the port by querying the transport info.
/// </summary>
void AccountConnection::MediaTransportPort::set(unsigned value)
{
	_mediaTransportPort = value;
}

/// <summary>
/// Gets specify the port range for socket binding, relative to the start
/// port number specified in MediaTransportPort that this setting is only
/// applicable when the start port number is non zero.
/// </summary>
unsigned AccountConnection::MediaTransportPortRange::get()
{
	return _mediaTransportPortRange;
}

/// <summary>
/// Sets specify the port range for socket binding, relative to the start
/// port number specified in MediaTransportPort that this setting is only
/// applicable when the start port number is non zero.
/// </summary>
void AccountConnection::MediaTransportPortRange::set(unsigned value)
{
	_mediaTransportPortRange = value;
}

/// <summary>
/// Gets if true to subscribe to message waiting indication events (RFC 3842).
/// </summary>
bool AccountConnection::MessageWaitingIndication::get()
{
	return _messageWaitingIndication;
}

/// <summary>
/// Sets if true to subscribe to message waiting indication events (RFC 3842).
/// </summary>
void AccountConnection::MessageWaitingIndication::set(bool value)
{
	_messageWaitingIndication = value;
}

/// <summary>
/// Gets specify the default expiration time (in seconds) for Message
/// Waiting Indication(RFC 3842) event subscription.This must not be zero.
/// </summary>
unsigned AccountConnection::MWIExpirationSec::get()
{
	return _mwiExpirationSec;
}

/// <summary>
/// Sets specify the default expiration time (in seconds) for Message
/// Waiting Indication(RFC 3842) event subscription.This must not be zero.
/// </summary>
void AccountConnection::MWIExpirationSec::set(unsigned value)
{
	_mwiExpirationSec = value;
}

/// <summary>
/// Gets if this flag is set, the presence information of this account will
/// be PUBLISH-ed to the server where the account belongs.
/// </summary>
bool AccountConnection::PublishEnabled::get()
{
	return _publishEnabled;
}

/// <summary>
/// Sets if this flag is set, the presence information of this account will
/// be PUBLISH-ed to the server where the account belongs.
/// </summary>
void AccountConnection::PublishEnabled::set(bool value)
{
	_publishEnabled = value;
}

/// <summary>
/// Gets specify whether the client publication session should queue the
/// PUBLISH request should there be another PUBLISH transaction still
/// pending.If this is set to false, the client will return error
/// on the PUBLISH request if there is another PUBLISH transaction still
/// in progress.
/// </summary>
bool AccountConnection::PublishQueue::get()
{
	return _publishQueue;
}

/// <summary>
/// Sets specify whether the client publication session should queue the
/// PUBLISH request should there be another PUBLISH transaction still
/// pending.If this is set to false, the client will return error
/// on the PUBLISH request if there is another PUBLISH transaction still
/// in progress.
/// </summary>
void AccountConnection::PublishQueue::set(bool value)
{
	_publishQueue = value;
}

/// <summary>
/// Gets maximum time to wait for unpublication transaction(s) to complete
/// during shutdown process, before sending unregistration.The library
/// tries to wait for the unpublication(un-PUBLISH) to complete before
/// sending REGISTER request to unregister the account, during library
/// shutdown process.If the value is set too short, it is possible that
/// the unregistration is sent before unpublication completes, causing
/// unpublication request to fail. Value is in milliseconds.
/// </summary>
unsigned AccountConnection::PublishShutdownWaitMsec::get()
{
	return _publishShutdownWaitMsec;
}

/// <summary>
/// Sets maximum time to wait for unpublication transaction(s) to complete
/// during shutdown process, before sending unregistration.The library
/// tries to wait for the unpublication(un-PUBLISH) to complete before
/// sending REGISTER request to unregister the account, during library
/// shutdown process.If the value is set too short, it is possible that
/// the unregistration is sent before unpublication completes, causing
/// unpublication request to fail. Value is in milliseconds.
/// </summary>
void AccountConnection::PublishShutdownWaitMsec::set(unsigned value)
{
	_publishShutdownWaitMsec = value;
}

/// <summary>
/// Gets the authentication credentials.
/// </summary>
AuthenticateCredentials^ AccountConnection::AuthCredentials::get()
{
	return _authCreds;
}

/// <summary>
/// Sets the authentication credentials.
/// </summary>
void AccountConnection::AuthCredentials::set(AuthenticateCredentials^ value)
{
	_authCreds = value;
}

/// <summary>
/// Gets or sets an indicator specifying this account is the default.
/// </summary>
bool AccountConnection::IsDefault::get()
{
	return _isDefault;
}

/// <summary>
/// Gets or sets an indicator specifying this account is the default.
/// </summary>
void AccountConnection::IsDefault::set(bool value)
{
	_isDefault = value;
}

/// <summary>
/// Gets or sets an indicator specifying that ice RTCP should not be used: default false.
/// </summary>
bool AccountConnection::NoIceRtcp::get()
{
	return _noIceRtcp;
}

/// <summary>
/// Gets or sets an indicator specifying that ice RTCP should not be used: default false.
/// </summary>
void AccountConnection::NoIceRtcp::set(bool value)
{
	_noIceRtcp = value;
}

/// <summary>
/// Gets or sets an indicator specifying that ice is enabled: default false.
/// </summary>
bool AccountConnection::IceEnabled::get()
{
	return _iceEnabled;
}

/// <summary>
/// Gets or sets an indicator specifying that ice is enabled: default false.
/// </summary>
void AccountConnection::IceEnabled::set(bool value)
{
	_iceEnabled = value;
}

/// <summary>
/// Gets or sets specify the Upstream/outgoing bandwidth. If this is set to zero, the video stream
/// will use codec maximum bitrate setting. Default : 0.
/// </summary>
unsigned AccountConnection::VideoRateControlBandwidth::get()
{
	return _videoRateControlBandwidth;
}

/// <summary>
/// Gets or sets specify the Upstream/outgoing bandwidth. If this is set to zero, the video stream
/// will use codec maximum bitrate setting. Default : 0.
/// </summary>
void AccountConnection::VideoRateControlBandwidth::set(unsigned value)
{
	_videoRateControlBandwidth = value;
}

/// <summary>
/// Gets or sets an indicator specifying that any video capture is done automatically.
/// </summary>
bool AccountConnection::VideoAutoTransmit::get()
{
	return _videoAutoTransmit;
}

/// <summary>
/// Gets or sets an indicator specifying that any video capture is done automatically.
/// </summary>
void AccountConnection::VideoAutoTransmit::set(bool value)
{
	_videoAutoTransmit = value;
}

/// <summary>
/// Gets or sets an indicator specifying that any video is shown automatically.
/// </summary>
bool AccountConnection::VideoAutoShow::get()
{
	return _videoAutoShow;
}

/// <summary>
/// Gets or sets an indicator specifying that any video is shown automatically.
/// </summary>
void AccountConnection::VideoAutoShow::set(bool value)
{
	_videoAutoShow = value;
}