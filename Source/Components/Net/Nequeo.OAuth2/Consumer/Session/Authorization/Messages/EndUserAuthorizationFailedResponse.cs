/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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

namespace Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

    /// <summary>
    /// The message that an Authorization Server responds to a Client with when the user denies a user authorization request.
    /// </summary>
    public class EndUserAuthorizationFailedResponse : MessageBase, IMessageWithClientState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndUserAuthorizationFailedResponse"/> class.
        /// </summary>
        /// <param name="clientCallback">The URL to redirect to so the client receives the message. This may not be built into the request message if the client pre-registered the URL with the authorization server.</param>
        /// <param name="version">The protocol version.</param>
        internal EndUserAuthorizationFailedResponse(Uri clientCallback, Version version)
            : base(version, MessageTransport.Indirect, clientCallback)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndUserAuthorizationFailedResponse"/> class.
        /// </summary>
        /// <param name="clientCallback">The URL to redirect to so the client receives the message. This may not be built into the request message if the client pre-registered the URL with the authorization server.</param>
        /// <param name="request">The authorization request from the user agent on behalf of the client.</param>
        internal EndUserAuthorizationFailedResponse(Uri clientCallback, EndUserAuthorizationRequest request)
            : base(((IProtocolMessage)request).Version, MessageTransport.Indirect, clientCallback)
        {
            ((IMessageWithClientState)this).ClientState = request.ClientState;
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// One of the values given in <see cref="Protocol.EndUserAuthorizationRequestErrorCodes"/>.
        /// OR a numerical HTTP status code from the 4xx or 5xx
        /// range, with the exception of the 400 (Bad Request) and
        /// 401 (Unauthorized) status codes.  For example, if the
        /// service is temporarily unavailable, the authorization
        /// server MAY return an error response with "error" set to
        /// "503".
        /// </value>
        [MessagePart(Protocol.error, IsRequired = true)]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets a human readable description of the error.
        /// </summary>
        /// <value>Human-readable text providing additional information, used to assist in the understanding and resolution of the error that occurred.</value>
        [MessagePart(Protocol.error_description, IsRequired = false)]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the location of the web page that describes the error and possible resolution.
        /// </summary>
        /// <value>A URI identifying a human-readable web page with information about the error, used to provide the end-user with additional information about the error.</value>
        [MessagePart(Protocol.error_uri, IsRequired = false)]
        public Uri ErrorUri { get; set; }

        /// <summary>
        /// Gets or sets some state as provided by the client in the authorization request.
        /// </summary>
        /// <value>An opaque value defined by the client.</value>
        /// <remarks>
        /// REQUIRED if the Client sent the value in the <see cref="EndUserAuthorizationRequest"/>.
        /// </remarks>
        [MessagePart(Protocol.state, IsRequired = false)]
        string IMessageWithClientState.ClientState { get; set; }
    }
}
