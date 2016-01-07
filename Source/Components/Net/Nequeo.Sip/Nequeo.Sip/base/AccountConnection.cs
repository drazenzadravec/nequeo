/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Account connection configuration.
    /// </summary>
    public class AccountConnection
    {
        /// <summary>
        /// Account connection configuration.
        /// </summary>
        public AccountConnection() { }

        /// <summary>
        /// Account connection configuration.
        /// </summary>
        /// <param name="accountName">The account name or service phone number.</param>
        /// <param name="spHost">The service provider host name or IP address.</param>
        /// <param name="username">The sip username.</param>
        /// <param name="password">The sip password.</param>
        public AccountConnection(string accountName, string spHost, string username, string password)
        {
            _accountName = accountName;
            _spHost = spHost;

            // Create a single credential.
            _authCreds = new AuthenticateCredentials()
            {
                AuthCredentials = new AuthCredInfo[] 
                {
                    new AuthCredInfo(username, password),
                },
            };
        }

        private string _accountName = null;
        private string _spHost = null;
        private int _spPort = 5060;
        private int _priority = 1;

        private bool _dropCallsOnFail = true;
        private bool _registerOnAdd = false;
        private uint _retryIntervalSec = 10;
        private uint _timeoutSec = 300;
        private uint _firstRetryIntervalSec = 10;
        private uint _unregWaitSec = 30;
        private uint _delayBeforeRefreshSec = 10;

        private uint _timerMinSESec = 90;
        private uint _timerSessExpiresSec = 1800;

        private IPv6_Use _ipv6_Use = IPv6_Use.IPV6_DISABLED;
        private SRTP_Use _srtp_Use = SRTP_Use.SRTP_DISABLED;
        private SRTP_SecureSignaling _srtp_SecureSignaling = SRTP_SecureSignaling.SRTP_DISABLED;

        private uint _mediaTransportPort = 0;
        private uint _mediaTransportPortRange = 0;

        private AuthenticateCredentials _authCreds = null;

        private bool _messageWaitingIndication = true;
        private uint _mwiExpirationSec = 3600;

        private bool _publishEnabled = false;
        private bool _publishQueue = true;
        private uint _publishShutdownWaitMsec = 2000;

        /// <summary>
        /// Gets or sets the account name or service phone number.
        /// </summary>
        public string AccountName
        {
            get { return _accountName; }
            set { _accountName = value; }
        }

        /// <summary>
        /// Gets or sets the service provider host name or IP address.
        /// </summary>
        public string SpHost
        {
            get { return _spHost; }
            set { _spHost = value; }
        }

        /// <summary>
        /// Gets or sets the service provider port.
        /// </summary>
        public int SpPort
        {
            get { return _spPort; }
            set { _spPort = value; }
        }

        /// <summary>
        /// Gets or sets the account priority.
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// Gets or sets specify whether calls of the configured account should be dropped
        /// after registration failure and an attempt of re-registration has also failed.
        /// </summary>
        public bool DropCallsOnFail
        {
            get { return _dropCallsOnFail; }
            set { _dropCallsOnFail = value; }
        }

        /// <summary>
        /// Gets or sets specify whether the account should register as soon as it is
        /// added to the UA.Application can set this to false and control
        /// the registration manually with Account.Registration().
        /// </summary>
        public bool RegisterOnAdd
        {
            get { return _registerOnAdd; }
            set { _registerOnAdd = value; }
        }

        /// <summary>
        /// Gets or sets specify interval of auto registration retry upon registration failure
        /// (including caused by transport problem), in second.Set to 0 to
        /// disable auto re-registration. Note that if the registration retry
        /// occurs because of transport failure, the first retry will be done
        /// after FirstRetryIntervalSec seconds instead.
        /// </summary>
        public uint RetryIntervalSec
        {
            get { return _retryIntervalSec; }
            set { _retryIntervalSec = value; }
        }

        /// <summary>
        /// Gets or sets interval for registration, in seconds. If the value is zero,
        /// default interval will be used 300 seconds.
        /// </summary>
        public uint TimeoutSec
        {
            get { return _timeoutSec; }
            set { _timeoutSec = value; }
        }

        /// <summary>
        /// Gets or sets specifies the interval for the first registration retry. The
        /// registration retry is explained in RetryIntervalSec.
        /// </summary>
        public uint FirstRetryIntervalSec
        {
            get { return _firstRetryIntervalSec; }
            set { _firstRetryIntervalSec = value; }
        }

        /// <summary>
        /// Gets or sets specify the maximum time to wait for unregistration requests to
        /// complete during library shutdown sequence.
        /// </summary>
        public uint UnregWaitSec
        {
            get { return _unregWaitSec; }
            set { _unregWaitSec = value; }
        }

        /// <summary>
        /// Gets or sets specify the number of seconds to refresh the client registration
        /// before the registration expires.
        /// </summary>
        public uint DelayBeforeRefreshSec
        {
            get { return _delayBeforeRefreshSec; }
            set { _delayBeforeRefreshSec = value; }
        }

        /// <summary>
        /// Gets or sets specify minimum Session Timer expiration period, in seconds.
        /// Must not be lower than 90. Default is 90.
        /// </summary>
        public uint TimerMinSESec
        {
            get { return _timerMinSESec; }
            set { _timerMinSESec = value; }
        }

        /// <summary>
        /// Gets or sets specify Session Timer expiration period, in seconds.
        /// Must not be lower than timerMinSE.Default is 1800.
        /// </summary>
        public uint TimerSessExpiresSec
        {
            get { return _timerSessExpiresSec; }
            set { _timerSessExpiresSec = value; }
        }

        /// <summary>
        /// Gets or sets specify whether IPv6 should be used on media. Default is not used.
        /// </summary>
        public IPv6_Use IPv6Use
        {
            get { return _ipv6_Use; }
            set { _ipv6_Use = value; }
        }

        /// <summary>
        /// Gets or sets specify whether secure media transport should be used for this account.
        /// </summary>
        public SRTP_Use SRTPUse
        {
            get { return _srtp_Use; }
            set { _srtp_Use = value; }
        }

        /// <summary>
        /// Gets or sets specify whether SRTP requires secure signaling to be used. This option
        /// is only used when SRTPUse option is non-zero.
        /// </summary>
        public SRTP_SecureSignaling SRTPSecureSignaling
        {
            get { return _srtp_SecureSignaling; }
            set { _srtp_SecureSignaling = value; }
        }

        /// <summary>
        /// Gets or sets UDP port number to bind locally. This setting MUST be specified
        /// even when default port is desired.If the value is zero, the
        /// transport will be bound to any available port, and application
        /// can query the port by querying the transport info.
        /// </summary>
        public uint MediaTransportPort
        {
            get { return _mediaTransportPort; }
            set { _mediaTransportPort = value; }
        }

        /// <summary>
        /// Gets or sets specify the port range for socket binding, relative to the start
        /// port number specified in MediaTransportPort that this setting is only
        /// applicable when the start port number is non zero.
        /// </summary>
        public uint MediaTransportPortRange
        {
            get { return _mediaTransportPortRange; }
            set { _mediaTransportPortRange = value; }
        }

        /// <summary>
        /// Gets or sets the authentication credentials.
        /// </summary>
        public AuthenticateCredentials AuthenticateCredentials
        {
            get { return _authCreds; }
            set { _authCreds = value; }
        }

        /// <summary>
        /// Gets or sets if true to subscribe to message waiting indication events (RFC 3842).
        /// </summary>
        public bool MessageWaitingIndication
        {
            get { return _messageWaitingIndication; }
            set { _messageWaitingIndication = value; }
        }

        /// <summary>
        /// Gets or sets specify the default expiration time (in seconds) for Message
        /// Waiting Indication(RFC 3842) event subscription.This must not be zero.
        /// </summary>
        public uint MWIExpirationSec
        {
            get { return _mwiExpirationSec; }
            set { _mwiExpirationSec = value; }
        }

        /// <summary>
        /// Gets or sets if this flag is set, the presence information of this account will
        /// be PUBLISH-ed to the server where the account belongs.
        /// </summary>
        public bool PublishEnabled
        {
            get { return _publishEnabled; }
            set { _publishEnabled = value; }
        }

        /// <summary>
        /// Gets or sets specify whether the client publication session should queue the
        /// PUBLISH request should there be another PUBLISH transaction still
        /// pending.If this is set to false, the client will return error
        /// on the PUBLISH request if there is another PUBLISH transaction still
        /// in progress.
        /// </summary>
        public bool PublishQueue
        {
            get { return _publishQueue; }
            set { _publishQueue = value; }
        }

        /// <summary>
        /// Gets or sets maximum time to wait for unpublication transaction(s) to complete
        /// during shutdown process, before sending unregistration.The library
        /// tries to wait for the unpublication(un-PUBLISH) to complete before
        /// sending REGISTER request to unregister the account, during library
        /// shutdown process.If the value is set too short, it is possible that
        /// the unregistration is sent before unpublication completes, causing
        /// unpublication request to fail. Value is in milliseconds.
        /// </summary>
        public uint PublishShutdownWaitMsec
        {
            get { return _publishShutdownWaitMsec; }
            set { _publishShutdownWaitMsec = value; }
        }
    }
}
