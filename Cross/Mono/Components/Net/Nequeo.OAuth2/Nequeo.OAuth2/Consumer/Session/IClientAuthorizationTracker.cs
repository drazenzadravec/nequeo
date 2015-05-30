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

namespace Nequeo.Net.OAuth2.Consumer.Session
{
    using System;
    using System.Diagnostics.Contracts;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

    /// <summary>
    /// A token manager implemented by some clients to assist in tracking authorization state.
    /// </summary>
    [ContractClass(typeof(IClientAuthorizationTrackerContract))]
    public interface IClientAuthorizationTracker
    {
        /// <summary>
        /// Gets the state of the authorization for a given callback URL and client state.
        /// </summary>
        /// <param name="callbackUrl">The callback URL.</param>
        /// <param name="clientState">State of the client stored at the beginning of an authorization request.</param>
        /// <returns>The authorization state; may be <c>null</c> if no authorization state matches.</returns>
        IAuthorizationState GetAuthorizationState(Uri callbackUrl, string clientState);
    }

    /// <summary>
    /// Contract class for the <see cref="IClientAuthorizationTracker"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IClientAuthorizationTracker))]
    internal abstract class IClientAuthorizationTrackerContract : IClientAuthorizationTracker
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="IClientAuthorizationTrackerContract"/> class from being created.
        /// </summary>
        private IClientAuthorizationTrackerContract()
        {
        }

        #region IClientTokenManager Members

        /// <summary>
        /// Gets the state of the authorization for a given callback URL and client state.
        /// </summary>
        /// <param name="callbackUrl">The callback URL.</param>
        /// <param name="clientState">State of the client stored at the beginning of an authorization request.</param>
        /// <returns>
        /// The authorization state; may be <c>null</c> if no authorization state matches.
        /// </returns>
        IAuthorizationState IClientAuthorizationTracker.GetAuthorizationState(Uri callbackUrl, string clientState)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
