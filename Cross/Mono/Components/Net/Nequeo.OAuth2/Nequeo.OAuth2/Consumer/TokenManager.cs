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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

using Nequeo.Net.Core.Messaging;
using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
using Nequeo.Net.OAuth2.Consumer.Session;
using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth2.Consumer
{
    /// <summary>
    /// A token manager implemented by some clients to assist in tracking authorization state.
    /// </summary>
    public class TokenManager : IClientAuthorizationTracker
    {
        /// <summary>
        /// Gets the state of the authorization for a given callback URL and client state.
        /// </summary>
        /// <param name="callbackUrl">The callback URL.</param>
        /// <param name="clientState">State of the client stored at the beginning of an authorization request.</param>
        /// <returns>The authorization state; may be <c>null</c> if no authorization state matches.</returns>
        public IAuthorizationState GetAuthorizationState(Uri callbackUrl, string clientState)
        {
            return new AuthorizationState { Callback = callbackUrl };
        }
    }
}
