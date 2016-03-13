/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ConnectionMapper.h
*  Purpose :       SIP ConnectionMapper class.
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

#pragma once

#ifndef _CONNECTIONMAPPER_H
#define _CONNECTIONMAPPER_H

#include "stdafx.h"

#include "IPv6_Use.h"
#include "SRTP_SecureSignaling.h"
#include "SRTP_Use.h" 
#include "StatusCode.h"
#include "RpidActivity.h"
#include "BuddyStatus.h"
#include "SubscriptionState.h"
#include "TransportType.h"

#include "pjsua2\account.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Account connection mapper.
			///	</summary>
			class ConnectionMapper
			{
			public:
				///	<summary>
				///	Account connection mapper.
				///	</summary>
				ConnectionMapper();

				///	<summary>
				///	Account connection mapper.
				///	</summary>
				~ConnectionMapper();

				///	<summary>
				///	Gets or sets the account name or service phone number.
				///	</summary>
				void SetAccountName(std::string value);
				std::string GetAccountName();

				///	<summary>
				///	Gets or sets the service provider host name or IP address.
				///	</summary>
				void SetSpHost(std::string value);
				std::string GetSpHost();

				///	<summary>
				///	Gets or sets the service provider host name or IP address.
				///	</summary>
				void SetSpPort(int value);
				int GetSpPort();

				///	<summary>
				///	Gets or sets the account priority.
				///	</summary>
				void SetPriority(int value);
				int GetPriority();

				/// <summary>
				/// Gets or sets specify whether calls of the configured account should be dropped
				/// after registration failure and an attempt of re-registration has also failed.
				/// </summary>
				void SetDropCallsOnFail(bool value);
				bool GetDropCallsOnFail();

				/// <summary>
				/// Gets or sets specify whether the account should register as soon as it is
				/// added to the UA.Application can set this to false and control
				/// the registration manually with Account.Registration().
				/// </summary>
				void SetRegisterOnAdd(bool value);
				bool GetRegisterOnAdd();

				/// <summary>
				/// Gets or sets Specify interval of auto registration retry upon registration failure
				/// (including caused by transport problem), in second.Set to 0 to
				/// disable auto re-registration. Note that if the registration retry
				/// occurs because of transport failure, the first retry will be done
				/// after FirstRetryIntervalSec seconds instead.
				/// </summary>
				void SetRetryIntervalSec(unsigned value);
				unsigned GetRetryIntervalSec();

				/// <summary>
				/// Gets or sets interval for registration, in seconds. If the value is zero,
				/// default interval will be used 300 seconds.
				/// </summary>
				void SetTimeoutSec(unsigned value);
				unsigned GetTimeoutSec();

				/// <summary>
				/// Gets or sets specifies the interval for the first registration retry. The
				/// registration retry is explained in RetryIntervalSec.
				/// </summary>
				void SetFirstRetryIntervalSec(unsigned value);
				unsigned GetFirstRetryIntervalSec();

				/// <summary>
				/// Gets or sets specify the maximum time to wait for unregistration requests to
				/// complete during library shutdown sequence.
				/// </summary>
				void SetUnregWaitSec(unsigned value);
				unsigned GetUnregWaitSec();

				/// <summary>
				/// Gets or sets specify the number of seconds to refresh the client registration
				/// before the registration expires.
				/// </summary>
				void SetDelayBeforeRefreshSec(unsigned value);
				unsigned GetDelayBeforeRefreshSec();

				/// <summary>
				/// Gets or sets specify minimum Session Timer expiration period, in seconds.
				/// Must not be lower than 90. Default is 90.
				/// </summary>
				void SetTimerMinSESec(unsigned value);
				unsigned GetTimerMinSESec();

				/// <summary>
				/// Gets or sets specify Session Timer expiration period, in seconds.
				/// Must not be lower than timerMinSE.Default is 1800.
				/// </summary>
				void SetTimerSessExpiresSec(unsigned value);
				unsigned GetTimerSessExpiresSec();

				/// <summary>
				/// Gets or sets specify whether IPv6 should be used on media. Default is not used.
				/// </summary>
				void SetIPv6Use(IPv6_Use value);
				IPv6_Use GetIPv6Use();

				/// <summary>
				/// Gets or sets specify whether secure media transport should be used for this account.
				/// </summary>
				void SetSRTPUse(SRTP_Use value);
				SRTP_Use GetSRTPUse();

				/// <summary>
				/// Gets or sets specify whether SRTP requires secure signaling to be used. This option
				/// is only used when SRTPUse option is non-zero.
				/// </summary>
				void SetSRTPSecureSignaling(SRTP_SecureSignaling value);
				SRTP_SecureSignaling GetSRTPSecureSignaling();

				/// <summary>
				/// Gets or sets UDP port number to bind locally. This setting MUST be specified
				/// even when default port is desired.If the value is zero, the
				/// transport will be bound to any available port, and application
				/// can query the port by querying the transport info.
				/// </summary>
				void SetMediaTransportPort(unsigned value);
				unsigned GetMediaTransportPort();

				/// <summary>
				/// Gets or sets specify the port range for socket binding, relative to the start
				/// port number specified in MediaTransportPort that this setting is only
				/// applicable when the start port number is non zero.
				/// </summary>
				void SetMediaTransportPortRange(unsigned value);
				unsigned GetMediaTransportPortRange();

				/// <summary>
				/// Gets or sets true to subscribe to message waiting indication events (RFC 3842).
				/// </summary>
				void SetMessageWaitingIndication(bool value);
				bool GetMessageWaitingIndication();

				/// <summary>
				/// Gets or sets specify the default expiration time (in seconds) for Message
				/// Waiting Indication(RFC 3842) event subscription.This must not be zero.
				/// </summary>
				void SetMWIExpirationSec(unsigned value);
				unsigned GetMWIExpirationSec();

				/// <summary>
				/// Gets or sets if this flag is set, the presence information of this account will
				/// be PUBLISH-ed to the server where the account belongs.
				/// </summary>
				void SetPublishEnabled(bool value);
				bool GetPublishEnabled();

				/// <summary>
				/// Gets or sets specify whether the client publication session should queue the
				/// PUBLISH request should there be another PUBLISH transaction still
				/// pending.If this is set to false, the client will return error
				/// on the PUBLISH request if there is another PUBLISH transaction still
				/// in progress.
				/// </summary>
				void SetPublishQueue(bool value);
				bool GetPublishQueue();

				/// <summary>
				/// Gets or sets Maximum time to wait for unpublication transaction(s) to complete
				/// during shutdown process, before sending unregistration.The library
				/// tries to wait for the unpublication(un-PUBLISH) to complete before
				/// sending REGISTER request to unregister the account, during library
				/// shutdown process.If the value is set too short, it is possible that
				/// the unregistration is sent before unpublication completes, causing
				/// unpublication request to fail. Value is in milliseconds.
				/// </summary>
				void SetPublishShutdownWaitMsec(unsigned value);
				unsigned GetPublishShutdownWaitMsec();

				/// <summary>
				/// Gets or sets the authentication credentials.
				/// </summary>
				void SetAuthCredentials(pj::AuthCredInfoVector value);
				pj::AuthCredInfoVector GetAuthCredentials();

				/// <summary>
				/// Gets or sets an indicator specifying this account is the default.
				/// </summary>
				void SetIsDefault(bool value);
				bool GetIsDefault();

				/// <summary>
				/// Gets or sets an indicator specifying that ice RTCP should not be used: default false.
				/// </summary>
				void SetNoIceRtcp(bool value);
				bool GetNoIceRtcp();

				/// <summary>
				/// Gets or sets an indicator specifying that ice is enabled: default false.
				/// </summary>
				void SetIceEnabled(bool value);
				bool GetIceEnabled();

				/// <summary>
				/// Gets or sets specify the Upstream/outgoing bandwidth. If this is set to zero, the video stream
				/// will use codec maximum bitrate setting. Default : 0.
				/// </summary>
				void SetVideoRateControlBandwidth(unsigned value);
				unsigned GetVideoRateControlBandwidth();

				/// <summary>
				/// Gets or sets an indicator specifying that any video capture is done automatically.
				/// </summary>
				void SetVideoAutoTransmit(bool value);
				bool GetVideoAutoTransmit();

				/// <summary>
				/// Gets or sets an indicator specifying that any video is shown automatically.
				/// </summary>
				void SetVideoAutoShow(bool value);
				bool GetVideoAutoShow();

				/// <summary>
				/// Get ip v6 use.
				/// </summary>
				/// <param name="ipv6Use">The current ipv6 use.</param>
				/// <returns>The ipv6 use.</returns>
				static pjsua_ipv6_use GetIPv6UseEx(IPv6_Use ipv6Use);

				/// <summary>
				/// Get srtp use.
				/// </summary>
				/// <param name="srtpUse">The current srtp use.</param>
				/// <returns>The srtp use.</returns>
				static pjmedia_srtp_use GetSrtpUseEx(SRTP_Use srtpUse);

				/// <summary>
				/// Get srtp secure signaling.
				/// </summary>
				/// <param name="srtpSecureSignaling">The current srtp secure signaling.</param>
				/// <returns>The srtp secure signaling.</returns>
				static int GetSRTPSecureSignalingEx(SRTP_SecureSignaling srtpSecureSignaling);

				/// <summary>
				/// Get the status code.
				/// </summary>
				/// <param name="statusCode">The current status code.</param>
				/// <returns>The status code.</returns>
				static StatusCode GetStatusCodeEx(pjsip_status_code statusCode);

				/// <summary>
				/// Get the status code.
				/// </summary>
				/// <param name="statusCode">The current status code.</param>
				/// <returns>The status code.</returns>
				static pjsip_status_code GetStatusCodeEx(StatusCode statusCode);

				/// <summary>
				/// Get the activity.
				/// </summary>
				/// <param name="activity">The current activity.</param>
				/// <returns>The activity.</returns>
				static pjrpid_activity GetActivityEx(RpidActivity activity);

				/// <summary>
				/// Get the activity.
				/// </summary>
				/// <param name="activity">The current activity.</param>
				/// <returns>The activity.</returns>
				static RpidActivity GetActivityEx(pjrpid_activity activity);

				/// <summary>
				/// Get the buddy status.
				/// </summary>
				/// <param name="status">The current buddy status.</param>
				/// <returns>The buddy status.</returns>
				static pjsua_buddy_status GetBuddyStatusEx(BuddyStatus status);

				/// <summary>
				/// Get the buddy status.
				/// </summary>
				/// <param name="status">The current buddy status.</param>
				/// <returns>The buddy status.</returns>
				static BuddyStatus GetBuddyStatusEx(pjsua_buddy_status status);

				/// <summary>
				/// Get subscription state.
				/// </summary>
				/// <param name="subscriptionState">The current subscription state.</param>
				/// <returns>The subscription state.</returns>
				static SubscriptionState GetSubscriptionStateEx(pjsip_evsub_state subscriptionState);

			private:
				bool _disposed;

				bool _isDefault;
				std::string _accountName;
				std::string _spHost;
				int _spPort;
				int _priority;
				bool _noIceRtcp;
				bool _iceEnabled;
				unsigned _videoRateControlBandwidth;
				bool _videoAutoTransmit;
				bool _videoAutoShow;

				bool _dropCallsOnFail;
				bool _registerOnAdd;
				unsigned _retryIntervalSec;
				unsigned _timeoutSec;
				unsigned _firstRetryIntervalSec;
				unsigned _unregWaitSec;
				unsigned _delayBeforeRefreshSec;

				unsigned _timerMinSESec;
				unsigned _timerSessExpiresSec;

				IPv6_Use _ipv6_Use;
				SRTP_Use _srtp_Use;
				SRTP_SecureSignaling _srtp_SecureSignaling;

				unsigned _mediaTransportPort;
				unsigned _mediaTransportPortRange;

				bool _messageWaitingIndication;
				unsigned _mwiExpirationSec;

				bool _publishEnabled;
				bool _publishQueue;
				unsigned _publishShutdownWaitMsec;

				pj::AuthCredInfoVector _authCreds;
			};
		}
	}
}
#endif