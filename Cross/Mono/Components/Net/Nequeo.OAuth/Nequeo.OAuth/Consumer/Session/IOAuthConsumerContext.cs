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

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Consumer.Session
{
    /// <summary>
    /// A consumer context is used to identify a consumer, and to sign a context on behalf 
    /// of a consumer using an optional supplied token.
    /// </summary>
    public interface IOAuthConsumerContext
    {
        /// <summary>
        /// Gets sets the realm
        /// </summary>
        string Realm { get; set; }

        /// <summary>
        /// Gets sets the consumer key
        /// </summary>
        string ConsumerKey { get; set; }

        /// <summary>
        /// Gets sets the cunsumer secret
        /// </summary>
        string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets sets the signature method
        /// </summary>
        string SignatureMethod { get; set; }

        /// <summary>
        /// Gets sets the user agent
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// Gets sets the asymmetric algorithm key
        /// </summary>
        AsymmetricAlgorithm Key { get; set; }

        /// <summary>
        /// Gets sets the use header for oauth parameters
        /// </summary>
        bool UseHeaderForOAuthParameters { get; set; }

        /// <summary>
        /// Sign Context
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        void SignContext(IOAuthContext context);

        /// <summary>
        /// Sign Context With Token
        /// </summary>
        /// <param name="context">>The OAuth context.</param>
        /// <param name="token">The token</param>
        void SignContextWithToken(IOAuthContext context, IToken token);
    }
}
