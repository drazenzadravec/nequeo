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
    /// Account information.
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// Account information.
        /// </summary>
        public AccountInfo() { }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a flag to indicate whether this is the default account.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the presence online status for this account.
        /// </summary>
        public bool OnlineStatus { get; set; }

        /// <summary>
        /// Gets or sets the presence online status text.
        /// </summary>
        public string OnlineStatusText { get; set; }

        /// <summary>
        /// Gets or sets an up to date expiration interval for account registration session.
        /// </summary>
        public int RegExpiresSec { get; set; }

        /// <summary>
        /// Gets or sets a flag to tell whether this account is currently registered (has active registration session).
        /// </summary>
        public bool RegIsActive { get; set; }

        /// <summary>
        /// Gets or sets a flag to tell whether this account has registration setting (reg_uri is not empty).
        /// </summary>
        public bool RegIsConfigured { get; set; }

        /// <summary>
        /// Gets or sets the Last registration error code. When the status field contains a SIP
        /// status code that indicates a registration failure, last registration
        /// error code contains the error code that causes the failure.In any
        /// other case, its value is zero.
        /// </summary>
        public int RegLastErr { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public StatusCode RegStatus { get; set; }

        /// <summary>
        /// Gets or sets a describing the registration status.
        /// </summary>
        public string RegStatusText { get; set; }

        /// <summary>
        /// Gets or sets the account URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Get ip v6 use.
        /// </summary>
        /// <param name="ipv6Use">The current ipv6 use.</param>
        /// <returns>The ipv6 use.</returns>
        internal static pjsua2.pjsua_ipv6_use GetIPv6UseEx(IPv6_Use ipv6Use)
        {
            // Select the ipv6 use
            switch (ipv6Use)
            {
                case IPv6_Use.IPV6_ENABLED:
                    return pjsua2.pjsua_ipv6_use.PJSUA_IPV6_ENABLED;
                default:
                    return pjsua2.pjsua_ipv6_use.PJSUA_IPV6_DISABLED;
            }
        }

        /// <summary>
        /// Get srtp use.
        /// </summary>
        /// <param name="srtpUse">The current srtp use.</param>
        /// <returns>The srtp use.</returns>
        internal static pjsua2.pjmedia_srtp_use GetSrtpUseEx(SRTP_Use srtpUse)
        {
            // Select the srtp use
            switch (srtpUse)
            {
                case SRTP_Use.SRTP_MANDATORY:
                    return pjsua2.pjmedia_srtp_use.PJMEDIA_SRTP_MANDATORY;
                case SRTP_Use.SRTP_OPTIONAL:
                    return pjsua2.pjmedia_srtp_use.PJMEDIA_SRTP_OPTIONAL;
                default:
                    return pjsua2.pjmedia_srtp_use.PJMEDIA_SRTP_DISABLED;
            }
        }

        /// <summary>
        /// Get srtp secure signaling.
        /// </summary>
        /// <param name="srtpSecureSignaling">The current srtp secure signaling.</param>
        /// <returns>The srtp secure signaling.</returns>
        internal static int GetSRTPSecureSignalingEx(SRTP_SecureSignaling srtpSecureSignaling)
        {
            // Select the srtp signaling.
            switch (srtpSecureSignaling)
            {
                case SRTP_SecureSignaling.SRTP_REQUIRES:
                    return 1;
                case SRTP_SecureSignaling.SRTP_REQUIRES_END_TO_END:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the status code.
        /// </summary>
        /// <param name="statusCode">The current status code.</param>
        /// <returns>The status code.</returns>
        internal static StatusCode GetStatusCodeEx(pjsua2.pjsip_status_code statusCode)
        {
            if (statusCode == pjsua2.pjsip_status_code.PJSIP_SC_TSX_TIMEOUT)
                return StatusCode.SC_DOES_NOT_EXIST_ANYWHERE;

            if (statusCode == pjsua2.pjsip_status_code.PJSIP_SC_TSX_TRANSPORT_ERROR)
                return StatusCode.SC_DOES_NOT_EXIST_ANYWHERE;

            // Select the sttaus code.
            switch (statusCode)
            {
                case pjsua2.pjsip_status_code.PJSIP_AC_AMBIGUOUS:
                    return StatusCode.AC_AMBIGUOUS;
                case pjsua2.pjsip_status_code.PJSIP_SC_ACCEPTED:
                    return StatusCode.SC_ACCEPTED;
                case pjsua2.pjsip_status_code.PJSIP_SC_ADDRESS_INCOMPLETE:
                    return StatusCode.SC_ADDRESS_INCOMPLETE;
                case pjsua2.pjsip_status_code.PJSIP_SC_ALTERNATIVE_SERVICE:
                    return StatusCode.SC_ALTERNATIVE_SERVICE;
                case pjsua2.pjsip_status_code.PJSIP_SC_BAD_EVENT:
                    return StatusCode.SC_BAD_EVENT;
                case pjsua2.pjsip_status_code.PJSIP_SC_BAD_EXTENSION:
                    return StatusCode.SC_BAD_EXTENSION;
                case pjsua2.pjsip_status_code.PJSIP_SC_BAD_GATEWAY:
                    return StatusCode.SC_BAD_GATEWAY;
                case pjsua2.pjsip_status_code.PJSIP_SC_BAD_REQUEST:
                    return StatusCode.SC_BAD_REQUEST;
                case pjsua2.pjsip_status_code.PJSIP_SC_BUSY_EVERYWHERE:
                    return StatusCode.SC_BUSY_EVERYWHERE;
                case pjsua2.pjsip_status_code.PJSIP_SC_BUSY_HERE:
                    return StatusCode.SC_BUSY_HERE;
                case pjsua2.pjsip_status_code.PJSIP_SC_CALL_BEING_FORWARDED:
                    return StatusCode.SC_CALL_BEING_FORWARDED;
                case pjsua2.pjsip_status_code.PJSIP_SC_CALL_TSX_DOES_NOT_EXIST:
                    return StatusCode.SC_CALL_TSX_DOES_NOT_EXIST;
                case pjsua2.pjsip_status_code.PJSIP_SC_DECLINE:
                    return StatusCode.SC_DECLINE;
                case pjsua2.pjsip_status_code.PJSIP_SC_DOES_NOT_EXIST_ANYWHERE:
                    return StatusCode.SC_DOES_NOT_EXIST_ANYWHERE;
                case pjsua2.pjsip_status_code.PJSIP_SC_EXTENSION_REQUIRED:
                    return StatusCode.SC_EXTENSION_REQUIRED;
                case pjsua2.pjsip_status_code.PJSIP_SC_FORBIDDEN:
                    return StatusCode.SC_FORBIDDEN;
                case pjsua2.pjsip_status_code.PJSIP_SC_GONE:
                    return StatusCode.SC_GONE;
                case pjsua2.pjsip_status_code.PJSIP_SC_INTERNAL_SERVER_ERROR:
                    return StatusCode.SC_INTERNAL_SERVER_ERROR;
                case pjsua2.pjsip_status_code.PJSIP_SC_INTERVAL_TOO_BRIEF:
                    return StatusCode.SC_INTERVAL_TOO_BRIEF;
                case pjsua2.pjsip_status_code.PJSIP_SC_LOOP_DETECTED:
                    return StatusCode.SC_LOOP_DETECTED;
                case pjsua2.pjsip_status_code.PJSIP_SC_MESSAGE_TOO_LARGE:
                    return StatusCode.SC_MESSAGE_TOO_LARGE;
                case pjsua2.pjsip_status_code.PJSIP_SC_METHOD_NOT_ALLOWED:
                    return StatusCode.SC_METHOD_NOT_ALLOWED;
                case pjsua2.pjsip_status_code.PJSIP_SC_MOVED_PERMANENTLY:
                    return StatusCode.SC_MOVED_PERMANENTLY;
                case pjsua2.pjsip_status_code.PJSIP_SC_MOVED_TEMPORARILY:
                    return StatusCode.SC_MOVED_TEMPORARILY;
                case pjsua2.pjsip_status_code.PJSIP_SC_MULTIPLE_CHOICES:
                    return StatusCode.SC_MULTIPLE_CHOICES;
                case pjsua2.pjsip_status_code.PJSIP_SC_NOT_ACCEPTABLE:
                    return StatusCode.SC_NOT_ACCEPTABLE;
                case pjsua2.pjsip_status_code.PJSIP_SC_NOT_ACCEPTABLE_ANYWHERE:
                    return StatusCode.SC_NOT_ACCEPTABLE_ANYWHERE;
                case pjsua2.pjsip_status_code.PJSIP_SC_NOT_ACCEPTABLE_HERE:
                    return StatusCode.SC_NOT_ACCEPTABLE_HERE;
                case pjsua2.pjsip_status_code.PJSIP_SC_NOT_FOUND:
                    return StatusCode.SC_NOT_FOUND;
                case pjsua2.pjsip_status_code.PJSIP_SC_NOT_IMPLEMENTED:
                    return StatusCode.SC_NOT_IMPLEMENTED;
                case pjsua2.pjsip_status_code.PJSIP_SC_OK:
                    return StatusCode.SC_OK;
                case pjsua2.pjsip_status_code.PJSIP_SC_PAYMENT_REQUIRED:
                    return StatusCode.SC_PAYMENT_REQUIRED;
                case pjsua2.pjsip_status_code.PJSIP_SC_PRECONDITION_FAILURE:
                    return StatusCode.SC_PRECONDITION_FAILURE;
                case pjsua2.pjsip_status_code.PJSIP_SC_PROGRESS:
                    return StatusCode.SC_PROGRESS;
                case pjsua2.pjsip_status_code.PJSIP_SC_PROXY_AUTHENTICATION_REQUIRED:
                    return StatusCode.SC_PROXY_AUTHENTICATION_REQUIRED;
                case pjsua2.pjsip_status_code.PJSIP_SC_QUEUED:
                    return StatusCode.SC_QUEUED;
                case pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_ENTITY_TOO_LARGE:
                    return StatusCode.SC_REQUEST_ENTITY_TOO_LARGE;
                case pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_PENDING:
                    return StatusCode.SC_REQUEST_PENDING;
                case pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_TERMINATED:
                    return StatusCode.SC_REQUEST_TERMINATED;
                case pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_TIMEOUT:
                    return StatusCode.SC_REQUEST_TIMEOUT;
                case pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_UPDATED:
                    return StatusCode.SC_REQUEST_UPDATED;
                case pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_URI_TOO_LONG:
                    return StatusCode.SC_REQUEST_URI_TOO_LONG;
                case pjsua2.pjsip_status_code.PJSIP_SC_RINGING:
                    return StatusCode.SC_RINGING;
                case pjsua2.pjsip_status_code.PJSIP_SC_SERVER_TIMEOUT:
                    return StatusCode.SC_SERVER_TIMEOUT;
                case pjsua2.pjsip_status_code.PJSIP_SC_SERVICE_UNAVAILABLE:
                    return StatusCode.SC_SERVICE_UNAVAILABLE;
                case pjsua2.pjsip_status_code.PJSIP_SC_SESSION_TIMER_TOO_SMALL:
                    return StatusCode.SC_SESSION_TIMER_TOO_SMALL;
                case pjsua2.pjsip_status_code.PJSIP_SC_TEMPORARILY_UNAVAILABLE:
                    return StatusCode.SC_TEMPORARILY_UNAVAILABLE;
                case pjsua2.pjsip_status_code.PJSIP_SC_TOO_MANY_HOPS:
                    return StatusCode.SC_TOO_MANY_HOPS;
                case pjsua2.pjsip_status_code.PJSIP_SC_TRYING:
                    return StatusCode.SC_TRYING;
                case pjsua2.pjsip_status_code.PJSIP_SC_UNAUTHORIZED:
                    return StatusCode.SC_UNAUTHORIZED;
                case pjsua2.pjsip_status_code.PJSIP_SC_UNDECIPHERABLE:
                    return StatusCode.SC_UNDECIPHERABLE;
                case pjsua2.pjsip_status_code.PJSIP_SC_UNSUPPORTED_MEDIA_TYPE:
                    return StatusCode.SC_UNSUPPORTED_MEDIA_TYPE;
                case pjsua2.pjsip_status_code.PJSIP_SC_UNSUPPORTED_URI_SCHEME:
                    return StatusCode.SC_UNSUPPORTED_URI_SCHEME;
                case pjsua2.pjsip_status_code.PJSIP_SC_USE_PROXY:
                    return StatusCode.SC_USE_PROXY;
                case pjsua2.pjsip_status_code.PJSIP_SC_VERSION_NOT_SUPPORTED:
                    return StatusCode.SC_VERSION_NOT_SUPPORTED;
                default:
                    return StatusCode.SC__force_32bit;
            }
        }

        /// <summary>
        /// Get the status code.
        /// </summary>
        /// <param name="statusCode">The current status code.</param>
        /// <returns>The status code.</returns>
        internal static pjsua2.pjsip_status_code GetStatusCode(StatusCode statusCode)
        {
            // Select the sttaus code.
            switch (statusCode)
            {
                case StatusCode.AC_AMBIGUOUS:
                    return pjsua2.pjsip_status_code.PJSIP_AC_AMBIGUOUS;
                case StatusCode.SC_ACCEPTED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_ACCEPTED;
                case StatusCode.SC_ADDRESS_INCOMPLETE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_ADDRESS_INCOMPLETE;
                case StatusCode.SC_ALTERNATIVE_SERVICE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_ALTERNATIVE_SERVICE;
                case StatusCode.SC_BAD_EVENT:
                    return pjsua2.pjsip_status_code.PJSIP_SC_BAD_EVENT;
                case StatusCode.SC_BAD_EXTENSION:
                    return pjsua2.pjsip_status_code.PJSIP_SC_BAD_EXTENSION;
                case StatusCode.SC_BAD_GATEWAY:
                    return pjsua2.pjsip_status_code.PJSIP_SC_BAD_GATEWAY;
                case StatusCode.SC_BAD_REQUEST:
                    return pjsua2.pjsip_status_code.PJSIP_SC_BAD_REQUEST;
                case StatusCode.SC_BUSY_EVERYWHERE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_BUSY_EVERYWHERE;
                case StatusCode.SC_BUSY_HERE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_BUSY_HERE;
                case StatusCode.SC_CALL_BEING_FORWARDED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_CALL_BEING_FORWARDED;
                case StatusCode.SC_CALL_TSX_DOES_NOT_EXIST:
                    return pjsua2.pjsip_status_code.PJSIP_SC_CALL_TSX_DOES_NOT_EXIST;
                case StatusCode.SC_DECLINE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_DECLINE;
                case StatusCode.SC_DOES_NOT_EXIST_ANYWHERE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_DOES_NOT_EXIST_ANYWHERE;
                case StatusCode.SC_EXTENSION_REQUIRED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_EXTENSION_REQUIRED;
                case StatusCode.SC_FORBIDDEN:
                    return pjsua2.pjsip_status_code.PJSIP_SC_FORBIDDEN;
                case StatusCode.SC_GONE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_GONE;
                case StatusCode.SC_INTERNAL_SERVER_ERROR:
                    return pjsua2.pjsip_status_code.PJSIP_SC_INTERNAL_SERVER_ERROR;
                case StatusCode.SC_INTERVAL_TOO_BRIEF:
                    return pjsua2.pjsip_status_code.PJSIP_SC_INTERVAL_TOO_BRIEF;
                case StatusCode.SC_LOOP_DETECTED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_LOOP_DETECTED;
                case StatusCode.SC_MESSAGE_TOO_LARGE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_MESSAGE_TOO_LARGE;
                case StatusCode.SC_METHOD_NOT_ALLOWED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_METHOD_NOT_ALLOWED;
                case StatusCode.SC_MOVED_PERMANENTLY:
                    return pjsua2.pjsip_status_code.PJSIP_SC_MOVED_PERMANENTLY;
                case StatusCode.SC_MOVED_TEMPORARILY:
                    return pjsua2.pjsip_status_code.PJSIP_SC_MOVED_TEMPORARILY;
                case StatusCode.SC_MULTIPLE_CHOICES:
                    return pjsua2.pjsip_status_code.PJSIP_SC_MULTIPLE_CHOICES;
                case StatusCode.SC_NOT_ACCEPTABLE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_NOT_ACCEPTABLE;
                case StatusCode.SC_NOT_ACCEPTABLE_ANYWHERE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_NOT_ACCEPTABLE_ANYWHERE;
                case StatusCode.SC_NOT_ACCEPTABLE_HERE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_NOT_ACCEPTABLE_HERE;
                case StatusCode.SC_NOT_FOUND:
                    return pjsua2.pjsip_status_code.PJSIP_SC_NOT_FOUND;
                case StatusCode.SC_NOT_IMPLEMENTED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_NOT_IMPLEMENTED;
                case StatusCode.SC_OK:
                    return pjsua2.pjsip_status_code.PJSIP_SC_OK;
                case StatusCode.SC_PAYMENT_REQUIRED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_PAYMENT_REQUIRED;
                case StatusCode.SC_PRECONDITION_FAILURE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_PRECONDITION_FAILURE;
                case StatusCode.SC_PROGRESS:
                    return pjsua2.pjsip_status_code.PJSIP_SC_PROGRESS;
                case StatusCode.SC_PROXY_AUTHENTICATION_REQUIRED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_PROXY_AUTHENTICATION_REQUIRED;
                case StatusCode.SC_QUEUED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_QUEUED;
                case StatusCode.SC_REQUEST_ENTITY_TOO_LARGE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_ENTITY_TOO_LARGE;
                case StatusCode.SC_REQUEST_PENDING:
                    return pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_PENDING;
                case StatusCode.SC_REQUEST_TERMINATED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_TERMINATED;
                case StatusCode.SC_REQUEST_TIMEOUT:
                    return pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_TIMEOUT;
                case StatusCode.SC_REQUEST_UPDATED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_UPDATED;
                case StatusCode.SC_REQUEST_URI_TOO_LONG:
                    return pjsua2.pjsip_status_code.PJSIP_SC_REQUEST_URI_TOO_LONG;
                case StatusCode.SC_RINGING:
                    return pjsua2.pjsip_status_code.PJSIP_SC_RINGING;
                case StatusCode.SC_SERVER_TIMEOUT:
                    return pjsua2.pjsip_status_code.PJSIP_SC_SERVER_TIMEOUT;
                case StatusCode.SC_SERVICE_UNAVAILABLE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_SERVICE_UNAVAILABLE;
                case StatusCode.SC_SESSION_TIMER_TOO_SMALL:
                    return pjsua2.pjsip_status_code.PJSIP_SC_SESSION_TIMER_TOO_SMALL;
                case StatusCode.SC_TEMPORARILY_UNAVAILABLE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_TEMPORARILY_UNAVAILABLE;
                case StatusCode.SC_TOO_MANY_HOPS:
                    return pjsua2.pjsip_status_code.PJSIP_SC_TOO_MANY_HOPS;
                case StatusCode.SC_TRYING:
                    return pjsua2.pjsip_status_code.PJSIP_SC_TRYING;
                case StatusCode.SC_TSX_TIMEOUT:
                    return pjsua2.pjsip_status_code.PJSIP_SC_TSX_TIMEOUT;
                case StatusCode.SC_TSX_TRANSPORT_ERROR:
                    return pjsua2.pjsip_status_code.PJSIP_SC_TSX_TRANSPORT_ERROR;
                case StatusCode.SC_UNAUTHORIZED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_UNAUTHORIZED;
                case StatusCode.SC_UNDECIPHERABLE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_UNDECIPHERABLE;
                case StatusCode.SC_UNSUPPORTED_MEDIA_TYPE:
                    return pjsua2.pjsip_status_code.PJSIP_SC_UNSUPPORTED_MEDIA_TYPE;
                case StatusCode.SC_UNSUPPORTED_URI_SCHEME:
                    return pjsua2.pjsip_status_code.PJSIP_SC_UNSUPPORTED_URI_SCHEME;
                case StatusCode.SC_USE_PROXY:
                    return pjsua2.pjsip_status_code.PJSIP_SC_USE_PROXY;
                case StatusCode.SC_VERSION_NOT_SUPPORTED:
                    return pjsua2.pjsip_status_code.PJSIP_SC_VERSION_NOT_SUPPORTED;
                default:
                    return pjsua2.pjsip_status_code.PJSIP_SC__force_32bit;
            }
        }
    }
}
