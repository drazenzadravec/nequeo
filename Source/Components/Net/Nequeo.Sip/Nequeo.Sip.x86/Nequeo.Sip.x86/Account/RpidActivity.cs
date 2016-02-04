using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Rpid activity type.
    /// </summary>
    public enum RpidActivity
    {
        /// <summary>
        /// Activity is unknown. The activity would then be conceived in the "note" field.
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// The person is away.
        /// </summary>
        AWAY = 1,
        /// <summary>
        /// The person is busy.
        /// </summary>
        BUSY = 2
    }

    /// <summary>
    /// Contact status type.
    /// </summary>
    public enum ContactStatus
    {
        /// <summary>
        /// Online status is unknown (possibly because no presence subscription has been established).
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// Contact is known to be online.
        /// </summary>
        ONLINE = 1,
        /// <summary>
        /// Contact is offline.
        /// </summary>
        OFFLINE = 2
    }

    /// <summary>
    /// Use IP v6.
    /// </summary>
    public enum IPv6_Use
    {
        /// <summary>
        /// Disabled.
        /// </summary>
        IPV6_DISABLED = 0,
        /// <summary>
        /// Enabled.
        /// </summary>
        IPV6_ENABLED = 1
    }

    /// <summary>
    /// Secure media transport type.
    /// </summary>
    public enum SRTP_Use
    {
        /// <summary>
        /// Disabled.
        /// </summary>
        SRTP_DISABLED = 0,
        /// <summary>
        /// Optional.
        /// </summary>
        SRTP_OPTIONAL = 1,
        /// <summary>
        /// Mandatory.
        /// </summary>
        SRTP_MANDATORY = 2
    }

    /// <summary>
    /// Secure media transport signaling.
    /// </summary>
    public enum SRTP_SecureSignaling
    {
        /// <summary>
        /// SRTP does not require secure signaling.
        /// </summary>
        SRTP_DISABLED = 0,
        /// <summary>
        /// SRTP requires secure transport such as TLS.
        /// </summary>
        SRTP_REQUIRES = 1,
        /// <summary>
        /// SRTP requires secure end-to-end transport (SIPS).
        /// </summary>
        SRTP_REQUIRES_END_TO_END = 2,
    }

    /// <summary>
    /// This enumeration describes basic subscription state as described in the 
    /// RFC 3265. The standard specifies that extensions may define additional
    /// states.In the case where the state is not known, the subscription state
    /// will be set to PJSIP_EVSUB_STATE_UNKNOWN, and the token will be kept
    /// in state_str member of the susbcription structure.
    /// </summary>
    public enum SubscriptionState
    {
        /// <summary>
        /// State is NULL.
        /// </summary>
        EVSUB_STATE_NULL = 0,
        /// <summary>
        /// Client has sent SUBSCRIBE request.
        /// </summary>
        EVSUB_STATE_SENT = 1,
        /// <summary>
        /// 2xx response to SUBSCRIBE has been sent/received.
        /// </summary>
        EVSUB_STATE_ACCEPTED = 2,
        /// <summary>
        /// Subscription is pending.
        /// </summary>
        EVSUB_STATE_PENDING = 3,
        /// <summary>
        /// Subscription is active.
        /// </summary>
        EVSUB_STATE_ACTIVE = 4,
        /// <summary>
        /// Subscription is terminated.
        /// </summary>
        EVSUB_STATE_TERMINATED = 5,
        /// <summary>
        /// Subscription state can not be determined.
        /// </summary>
        EVSUB_STATE_UNKNOWN = 6
    }

    /// <summary>
    /// Status code type.
    /// </summary>
    /// <remarks>
    /// This enumeration lists standard SIP status codes according to RFC 3261.
    /// In addition, it also declares new status class 7xx for errors generated
    /// by the stack.This status class however should not get transmitted on the wire.
    /// </remarks>
    public enum StatusCode
    {
        /// <summary>
        /// 
        /// </summary>
        SC_TRYING = 100,
        /// <summary>
        /// 
        /// </summary>
        SC_RINGING = 180,
        /// <summary>
        /// 
        /// </summary>
        SC_CALL_BEING_FORWARDED = 181,
        /// <summary>
        /// 
        /// </summary>
        SC_QUEUED = 182,
        /// <summary>
        /// 
        /// </summary>
        SC_PROGRESS = 183,
        /// <summary>
        /// 
        /// </summary>
        SC_OK = 200,
        /// <summary>
        /// 
        /// </summary>
        SC_ACCEPTED = 202,
        /// <summary>
        /// 
        /// </summary>
        SC_MULTIPLE_CHOICES = 300,
        /// <summary>
        /// 
        /// </summary>
        SC_MOVED_PERMANENTLY = 301,
        /// <summary>
        /// 
        /// </summary>
        SC_MOVED_TEMPORARILY = 302,
        /// <summary>
        /// 
        /// </summary>
        SC_USE_PROXY = 305,
        /// <summary>
        /// 
        /// </summary>
        SC_ALTERNATIVE_SERVICE = 380,
        /// <summary>
        /// 
        /// </summary>
        SC_BAD_REQUEST = 400,
        /// <summary>
        /// 
        /// </summary>
        SC_UNAUTHORIZED = 401,
        /// <summary>
        /// 
        /// </summary>
        SC_PAYMENT_REQUIRED = 402,
        /// <summary>
        /// 
        /// </summary>
        SC_FORBIDDEN = 403,
        /// <summary>
        /// 
        /// </summary>
        SC_NOT_FOUND = 404,
        /// <summary>
        /// 
        /// </summary>
        SC_METHOD_NOT_ALLOWED = 405,
        /// <summary>
        /// 
        /// </summary>
        SC_NOT_ACCEPTABLE = 406,
        /// <summary>
        /// 
        /// </summary>
        SC_PROXY_AUTHENTICATION_REQUIRED = 407,
        /// <summary>
        /// 
        /// </summary>
        SC_REQUEST_TIMEOUT = 408,
        /// <summary>
        /// 
        /// </summary>
        SC_TSX_TIMEOUT = 409,
        /// <summary>
        /// 
        /// </summary>
        SC_GONE = 410,
        /// <summary>
        /// 
        /// </summary>
        SC_REQUEST_ENTITY_TOO_LARGE = 413,
        /// <summary>
        /// 
        /// </summary>
        SC_REQUEST_URI_TOO_LONG = 414,
        /// <summary>
        /// 
        /// </summary>
        SC_UNSUPPORTED_MEDIA_TYPE = 415,
        /// <summary>
        /// 
        /// </summary>
        SC_UNSUPPORTED_URI_SCHEME = 416,
        /// <summary>
        /// 
        /// </summary>
        SC_BAD_EXTENSION = 420,
        /// <summary>
        /// 
        /// </summary>
        SC_EXTENSION_REQUIRED = 421,
        /// <summary>
        /// 
        /// </summary>
        SC_SESSION_TIMER_TOO_SMALL = 422,
        /// <summary>
        /// 
        /// </summary>
        SC_INTERVAL_TOO_BRIEF = 423,
        /// <summary>
        /// 
        /// </summary>
        SC_TEMPORARILY_UNAVAILABLE = 480,
        /// <summary>
        /// 
        /// </summary>
        SC_CALL_TSX_DOES_NOT_EXIST = 481,
        /// <summary>
        /// 
        /// </summary>
        SC_LOOP_DETECTED = 482,
        /// <summary>
        /// 
        /// </summary>
        SC_TOO_MANY_HOPS = 483,
        /// <summary>
        /// 
        /// </summary>
        SC_ADDRESS_INCOMPLETE = 484,
        /// <summary>
        /// 
        /// </summary>
        AC_AMBIGUOUS = 485,
        /// <summary>
        /// 
        /// </summary>
        SC_BUSY_HERE = 486,
        /// <summary>
        /// 
        /// </summary>
        SC_REQUEST_TERMINATED = 487,
        /// <summary>
        /// 
        /// </summary>
        SC_NOT_ACCEPTABLE_HERE = 488,
        /// <summary>
        /// 
        /// </summary>
        SC_BAD_EVENT = 489,
        /// <summary>
        /// 
        /// </summary>
        SC_REQUEST_UPDATED = 490,
        /// <summary>
        /// 
        /// </summary>
        SC_REQUEST_PENDING = 491,
        /// <summary>
        /// 
        /// </summary>
        SC_UNDECIPHERABLE = 493,
        /// <summary>
        /// 
        /// </summary>
        SC_INTERNAL_SERVER_ERROR = 500,
        /// <summary>
        /// 
        /// </summary>
        SC_NOT_IMPLEMENTED = 501,
        /// <summary>
        /// 
        /// </summary>
        SC_BAD_GATEWAY = 502,
        /// <summary>
        /// 
        /// </summary>
        SC_SERVICE_UNAVAILABLE = 503,
        /// <summary>
        /// 
        /// </summary>
        SC_TSX_TRANSPORT_ERROR = 533,
        /// <summary>
        /// 
        /// </summary>
        SC_SERVER_TIMEOUT = 504,
        /// <summary>
        /// 
        /// </summary>
        SC_VERSION_NOT_SUPPORTED = 505,
        /// <summary>
        /// 
        /// </summary>
        SC_MESSAGE_TOO_LARGE = 513,
        /// <summary>
        /// 
        /// </summary>
        SC_PRECONDITION_FAILURE = 580,
        /// <summary>
        /// 
        /// </summary>
        SC_BUSY_EVERYWHERE = 600,
        /// <summary>
        /// 
        /// </summary>
        SC_DECLINE = 603,
        /// <summary>
        /// 
        /// </summary>
        SC_DOES_NOT_EXIST_ANYWHERE = 604,
        /// <summary>
        /// 
        /// </summary>
        SC_NOT_ACCEPTABLE_ANYWHERE = 606,
        /// <summary>
        /// 
        /// </summary>
        SC__force_32bit = int.MaxValue
    }
}
