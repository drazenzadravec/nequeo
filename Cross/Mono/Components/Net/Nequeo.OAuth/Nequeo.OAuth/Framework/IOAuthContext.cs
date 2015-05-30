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
using System.Text;
using System.Web;

namespace Nequeo.Net.OAuth.Framework
{
    /// <summary>
    /// The base OAuth context interface.
    /// </summary>
    public interface IOAuthContext : IToken
    {
        /// <summary>
        /// Gets sets the http context headers.
        /// </summary>
        NameValueCollection Headers { get; set; }

        /// <summary>
        /// Gets sets the http context query parameters.
        /// </summary>
        NameValueCollection QueryParameters { get; set; }

        /// <summary>
        /// Gets sets the http context cookies.
        /// </summary>
        NameValueCollection Cookies { get; set; }

        /// <summary>
        /// Gets sets the http context form encoded parameters.
        /// </summary>
        NameValueCollection FormEncodedParameters { get; set; }

        /// <summary>
        /// Gets sets the http context authorization header parameters.
        /// </summary>
        NameValueCollection AuthorizationHeaderParameters { get; set; }

        /// <summary>
        /// Gets sets the http context raw content.
        /// </summary>
        byte[] RawContent { get; set; }

        /// <summary>
        /// Gets sets the http context raw content type.
        /// </summary>
        string RawContentType { get; set; }

        /// <summary>
        /// Gets sets the http context raw uri.
        /// </summary>
        Uri RawUri { get; set; }

        /// <summary>
        /// Gets sets the http context normalized request url.
        /// </summary>
        string NormalizedRequestUrl { get; }

        /// <summary>
        /// Gets sets the http context request method.
        /// </summary>
        string RequestMethod { get; set; }

        /// <summary>
        /// Gets sets the nonce.
        /// </summary>
        string Nonce { get; set; }

        /// <summary>
        /// Gets sets the signature.
        /// </summary>
        string Signature { get; set; }

        /// <summary>
        /// Gets sets the signature method
        /// </summary>
        string SignatureMethod { get; set; }

        /// <summary>
        /// Gets sets the timestamp.
        /// </summary>
        string Timestamp { get; set; }

        /// <summary>
        /// Gets sets the version.
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Gets sets the callback url.
        /// </summary>
        string CallbackUrl { get; set; }

        /// <summary>
        /// Gets sets the verifier.
        /// </summary>
        string Verifier { get; set; }

        /// <summary>
        /// Gets sets the body hash.
        /// </summary>
        string BodyHash { get; set; }

        /// <summary>
        /// Gets sets the XAuth mode.
        /// </summary>
        string XAuthMode { get; set; }

        /// <summary>
        /// Gets sets the XAuth username.
        /// </summary>
        string XAuthUsername { get; set; }

        /// <summary>
        /// Gets sets the XAuth password.
        /// </summary>
        string XAuthPassword { get; set; }

        /// <summary>
        /// Gets sets the use authorization header.
        /// </summary>
        bool UseAuthorizationHeader { get; set; }

        /// <summary>
        /// Gets sets the include OAuth request body hash in signature.
        /// </summary>
        bool IncludeOAuthRequestBodyHashInSignature { get; set; }

        /// <summary>
        /// Generate the uri.
        /// </summary>
        /// <returns>The URI.</returns>
        Uri GenerateUri();

        /// <summary>
        /// Generate the uri.
        /// </summary>
        /// <returns>The URI string.</returns>
        string GenerateUrl();

        /// <summary>
        /// Generate OAuth parameters for header.
        /// </summary>
        /// <returns>The generated value.</returns>
        string GenerateOAuthParametersForHeader();

        /// <summary>
        /// Generate uri without OAuth parameters.
        /// </summary>
        /// <returns>The generated URI.</returns>
        Uri GenerateUriWithoutOAuthParameters();

        /// <summary>
        /// Generate signature base.
        /// </summary>
        /// <returns>The generated value.</returns>
        string GenerateSignatureBase();

        /// <summary>
        /// Generate body hash.
        /// </summary>
        /// <returns>The generated value.</returns>
        string GenerateBodyHash();

        /// <summary>
        /// Generate and set body hash
        /// </summary>
        void GenerateAndSetBodyHash();

        /// <summary>
        /// Translate the signature method to the siganture type.
        /// </summary>
        /// <param name="signatureMethod">The signature method.</param>
        /// <returns>The siganture type.</returns>
        Nequeo.Net.OAuth.SignatureTypes TranslateSignatureType(string signatureMethod);

        /// <summary>
        /// Get the string value of the current OAuth contaxt.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
