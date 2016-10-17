/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountConnection.h
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

#pragma once

#ifndef _ACCOUNTCONNECTION_H
#define _ACCOUNTCONNECTION_H

#include "stdafx.h"

#include "AuthenticateCredentials.h"
#include "AuthCredInfo.h"

#include "IPv6_Use.h"
#include "SRTP_SecureSignaling.h"
#include "SRTP_Use.h"
#include "TransportType.h"

#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Account connection configuration.
			///	</summary>
			public ref class AccountConnection sealed
			{
			public:
				///	<summary>
				///	Account connection configuration.
				///	</summary>
				AccountConnection();

				///	<summary>
				///	Account connection configuration.
				///	</summary>
				/// <param name="accountName">The account name or service phone number.</param>
				/// <param name="spHost">The service provider host name or IP address.</param>
				/// <param name="username">The sip username.</param>
				/// <param name="password">The sip password.</param>
				AccountConnection(String^ accountName, String^ spHost, String^ username, String^ password);

				///	<summary>
				///	Account connection configuration.
				///	</summary>
				~AccountConnection();

				///	<summary>
				///	Gets or sets the account name or service phone number.
				///	</summary>
				property String^ AccountName
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the service provider host name or IP address.
				///	</summary>
				property String^ SpHost
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the service provider host name or IP address.
				///	</summary>
				property int SpPort
				{
					int get();
					void set(int value);
				}

				///	<summary>
				///	Gets or sets the account priority.
				///	</summary>
				property int Priority
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets specify whether calls of the configured account should be dropped
				/// after registration failure and an attempt of re-registration has also failed.
				/// </summary>
				property bool DropCallsOnFail
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets specify whether the account should register as soon as it is
				/// added to the UA.Application can set this to false and control
				/// the registration manually with Account.Registration().
				/// </summary>
				property bool RegisterOnAdd
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets Specify interval of auto registration retry upon registration failure
				/// (including caused by transport problem), in second.Set to 0 to
				/// disable auto re-registration. Note that if the registration retry
				/// occurs because of transport failure, the first retry will be done
				/// after FirstRetryIntervalSec seconds instead.
				/// </summary>
				property unsigned RetryIntervalSec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets interval for registration, in seconds. If the value is zero,
				/// default interval will be used 300 seconds.
				/// </summary>
				property unsigned TimeoutSec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specifies the interval for the first registration retry. The
				/// registration retry is explained in RetryIntervalSec.
				/// </summary>
				property unsigned FirstRetryIntervalSec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specify the maximum time to wait for unregistration requests to
				/// complete during library shutdown sequence.
				/// </summary>
				property unsigned UnregWaitSec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specify the number of seconds to refresh the client registration
				/// before the registration expires.
				/// </summary>
				property unsigned DelayBeforeRefreshSec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specify minimum Session Timer expiration period, in seconds.
				/// Must not be lower than 90. Default is 90.
				/// </summary>
				property unsigned TimerMinSESec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specify Session Timer expiration period, in seconds.
				/// Must not be lower than timerMinSE.Default is 1800.
				/// </summary>
				property unsigned TimerSessExpiresSec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specify whether IPv6 should be used on media. Default is not used.
				/// </summary>
				property IPv6_Use IPv6Use
				{
					IPv6_Use get();
					void set(IPv6_Use value);
				}

				/// <summary>
				/// Gets or sets specify whether secure media transport should be used for this account.
				/// </summary>
				property SRTP_Use SRTPUse
				{
					SRTP_Use get();
					void set(SRTP_Use value);
				}

				/// <summary>
				/// Gets or sets specify whether SRTP requires secure signaling to be used. This option
				/// is only used when SRTPUse option is non-zero.
				/// </summary>
				property SRTP_SecureSignaling SRTPSecureSignaling
				{
					SRTP_SecureSignaling get();
					void set(SRTP_SecureSignaling value);
				}

				/// <summary>
				/// Gets or sets UDP port number to bind locally. This setting MUST be specified
				/// even when default port is desired.If the value is zero, the
				/// transport will be bound to any available port, and application
				/// can query the port by querying the transport info.
				/// </summary>
				property unsigned MediaTransportPort
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets specify the port range for socket binding, relative to the start
				/// port number specified in MediaTransportPort that this setting is only
				/// applicable when the start port number is non zero.
				/// </summary>
				property unsigned MediaTransportPortRange
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets true to subscribe to message waiting indication events (RFC 3842).
				/// </summary>
				property bool MessageWaitingIndication
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets specify the default expiration time (in seconds) for Message
				/// Waiting Indication(RFC 3842) event subscription.This must not be zero.
				/// </summary>
				property unsigned MWIExpirationSec
				{
					unsigned get();
					void set(unsigned value);
				}
				
				/// <summary>
				/// Gets or sets if this flag is set, the presence information of this account will
				/// be PUBLISH-ed to the server where the account belongs.
				/// </summary>
				property bool PublishEnabled
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets specify whether the client publication session should queue the
				/// PUBLISH request should there be another PUBLISH transaction still
				/// pending.If this is set to false, the client will return error
				/// on the PUBLISH request if there is another PUBLISH transaction still
				/// in progress.
				/// </summary>
				property bool PublishQueue
				{
					bool get();
					void set(bool value);
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
				property unsigned PublishShutdownWaitMsec
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the authentication credentials.
				/// </summary>
				property AuthenticateCredentials^ AuthCredentials
				{
					AuthenticateCredentials^ get();
					void set(AuthenticateCredentials^ value);
				}

				/// <summary>
				/// Gets or sets an indicator specifying this account is the default.
				/// </summary>
				property bool IsDefault
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets an indicator specifying that ice RTCP should not be used: default false.
				/// </summary>
				property bool NoIceRtcp
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets an indicator specifying that ice is enabled: default false.
				/// </summary>
				property bool IceEnabled
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets specify the Upstream/outgoing bandwidth. If this is set to zero, the video stream
				/// will use codec maximum bitrate setting. Default : 0.
				/// </summary>
				property unsigned VideoRateControlBandwidth
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets an indicator specifying that any video capture is done automatically.
				/// </summary>
				property bool VideoAutoTransmit
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets an indicator specifying that any video is shown automatically.
				/// </summary>
				property bool VideoAutoShow
				{
					bool get();
					void set(bool value);
				}

			private:
				bool _disposed;

				bool _isDefault;
				String^ _accountName;
				String^ _spHost;
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

				AuthenticateCredentials^ _authCreds;

			};
		}
	}
}
#endif