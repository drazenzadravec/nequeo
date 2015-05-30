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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Cryptography.Signing;
using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Provider.Inspectors
{
    /// <summary>
    /// This inspector implements additional behavior required by the 1.0a version of OAuth.
    /// </summary>
    public class OAuth10AInspector : IContextInspector
    {
        readonly ITokenStore _tokenStore;

        /// <summary>
        /// OAuth 10A Inspector
        /// </summary>
        /// <param name="tokenStore">The token store.</param>
        public OAuth10AInspector(ITokenStore tokenStore)
        {
            if (tokenStore == null) throw new ArgumentNullException("tokenStore");
            _tokenStore = tokenStore;
        }

        /// <summary>
        /// Inspect the current context.
        /// </summary>
        /// <param name="phase">The current provider phase.</param>
        /// <param name="context">OAuth context</param>
        public void InspectContext(ProviderPhase phase, IOAuthContext context)
        {
            if (phase == ProviderPhase.GrantRequestToken)
            {
                ValidateCallbackUrlIsPartOfRequest(context);
            }
            else if (phase == ProviderPhase.ExchangeRequestTokenForAccessToken)
            {
                ValidateVerifierMatchesStoredVerifier(context);
            }
        }

        /// <summary>
        /// Validate verifier matches stored verifier
        /// </summary>
        /// <param name="context">OAuth context</param>
        void ValidateVerifierMatchesStoredVerifier(IOAuthContext context)
        {
            string actual = context.Verifier;

            if (string.IsNullOrEmpty(actual))
            {
                throw Error.MissingRequiredOAuthParameter(context, Parameters.OAuth_Verifier);
            }

            string expected = _tokenStore.GetVerificationCodeForRequestToken(context);

            if (expected != actual.Trim())
            {
                throw Error.RejectedRequiredOAuthParameter(context, Parameters.OAuth_Verifier);
            }
        }

        /// <summary>
        /// Validate callback url is part of request
        /// </summary>
        /// <param name="context">OAuth context</param>
        static void ValidateCallbackUrlIsPartOfRequest(IOAuthContext context)
        {
            if (string.IsNullOrEmpty(context.CallbackUrl))
            {
                throw Error.MissingRequiredOAuthParameter(context, Parameters.OAuth_Callback);
            }
        }
    }
}
