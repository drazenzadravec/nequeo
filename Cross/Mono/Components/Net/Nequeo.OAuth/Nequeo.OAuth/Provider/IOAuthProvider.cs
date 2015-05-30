﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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

using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Provider
{
    /// <summary>
    /// OAuth provider interface.
    /// </summary>
    public interface IOAuthProvider
    {
        /// <summary>
        /// Grant request token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        IToken GrantRequestToken(IOAuthContext context);

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        IToken ExchangeRequestTokenForAccessToken(IOAuthContext context);

        /// <summary>
        /// Access protected resource request
        /// </summary>
        /// <param name="context">OAuth context</param>
        void AccessProtectedResourceRequest(IOAuthContext context);

        /// <summary>
        /// Renew access token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        IToken RenewAccessToken(IOAuthContext context);

        /// <summary>
        /// Create access token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        IToken CreateAccessToken(IOAuthContext context);
    }
}
