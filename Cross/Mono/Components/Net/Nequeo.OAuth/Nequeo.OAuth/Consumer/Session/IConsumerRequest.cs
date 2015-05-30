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
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Xml.Linq;

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Consumer.Session
{
    /// <summary>
    /// Consumer request interface
    /// </summary>
    public interface IConsumerRequest
    {
        /// <summary>
        /// Gets the consumer context
        /// </summary>
        IOAuthConsumerContext ConsumerContext { get; }

        /// <summary>
        /// Gets the OAuth context
        /// </summary>
        IOAuthContext Context { get; }

        /// <summary>
        /// Gets sets the proxy server URI
        /// </summary>
        Uri ProxyServerUri { get; set; }

        /// <summary>
        /// Gets sets the response body action
        /// </summary>
        Action<string> ResponseBodyAction { get; set; }

        /// <summary>
        /// Gets sets the accepts type
        /// </summary>
        string AcceptsType { get; set; }

        /// <summary>
        /// Gets sets the request body
        /// </summary>
        string RequestBody { get; set; }

        /// <summary>
        /// Get the xdocument
        /// </summary>
        /// <returns>The xdocument</returns>
        XDocument ToDocument();

        /// <summary>
        /// Convert to byte array.
        /// </summary>
        /// <returns>The byte array</returns>
        byte[] ToBytes();

        /// <summary>
        /// To web response.
        /// </summary>
        /// <returns>The http web response</returns>
        HttpWebResponse ToWebResponse();

        /// <summary>
        /// To body parameters
        /// </summary>
        /// <returns>The name value collection.</returns>
        NameValueCollection ToBodyParameters();

        /// <summary>
        /// Get the request description
        /// </summary>
        /// <returns>The name value collection.</returns>
        RequestDescription GetRequestDescription();

        /// <summary>
        /// Sign Without Token
        /// </summary>
        /// <returns>The name value collection.</returns>
        IConsumerRequest SignWithoutToken();

        /// <summary>
        /// Sign With Token
        /// </summary>
        /// <returns>The name value collection.</returns>
        IConsumerRequest SignWithToken();

        /// <summary>
        /// Sign With Token
        /// </summary>
        /// <param name="token">The token</param>
        /// <returns>The name value collection.</returns>
        IConsumerRequest SignWithToken(IToken token);
    }
}
