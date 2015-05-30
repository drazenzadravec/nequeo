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

using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;

using Nequeo.Net.OAuth;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Framework
{
    /// <summary>
    /// OAuth context implementation
    /// </summary>
    [Serializable]
    public class OAuthContextProvider : IOAuthContext
    {
        string _bodyHash;
        string _callbackUrl;
        string _consumerKey;
        string _nonce;
        string _sessionHandle;
        string _signature;
        string _signatureMethod;
        string _timestamp;
        string _token;
        string _tokenSecret;
        string _verifier;
        string _version;
        string _xAuthMode;
        string _xAuthUsername;
        string _xAuthPassword;

        NameValueCollection _authorizationHeaderParameters;
        NameValueCollection _cookies;
        NameValueCollection _formEncodedParameters;
        NameValueCollection _headers;
        string _normalizedRequestUrl;
        NameValueCollection _queryParameters;
        Uri _rawUri;

        /// <summary>
        /// OAuth context
        /// </summary>
        public OAuthContextProvider()
        {
            FormEncodedParameters = new NameValueCollection();
            Cookies = new NameValueCollection();
            Headers = new NameValueCollection();
            AuthorizationHeaderParameters = new NameValueCollection();
        }

        /// <summary>
        /// Gets sets the http context headers.
        /// </summary>
        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null) _headers = new NameValueCollection();
                return _headers;
            }
            set { _headers = value; }
        }

        /// <summary>
        /// Gets sets the http context query parameters.
        /// </summary>
        public NameValueCollection QueryParameters
        {
            get
            {
                if (_queryParameters == null) _queryParameters = new NameValueCollection();
                return _queryParameters;
            }
            set { _queryParameters = value; }
        }

        /// <summary>
        /// Gets sets the http context cookies.
        /// </summary>
        public NameValueCollection Cookies
        {
            get
            {
                if (_cookies == null) _cookies = new NameValueCollection();
                return _cookies;
            }
            set { _cookies = value; }
        }

        /// <summary>
        /// Gets sets the http context form encoded parameters.
        /// </summary>
        public NameValueCollection FormEncodedParameters
        {
            get
            {
                if (_formEncodedParameters == null) _formEncodedParameters = new NameValueCollection();
                return _formEncodedParameters;
            }
            set { _formEncodedParameters = value; }
        }

        /// <summary>
        /// Gets sets the http context authorization header parameters.
        /// </summary>
        public NameValueCollection AuthorizationHeaderParameters
        {
            get
            {
                if (_authorizationHeaderParameters == null) _authorizationHeaderParameters = new NameValueCollection();
                return _authorizationHeaderParameters;
            }
            set { _authorizationHeaderParameters = value; }
        }

        /// <summary>
        /// Gets sets the http context raw content.
        /// </summary>
        public byte[] RawContent { get; set; }

        /// <summary>
        /// Gets sets the http context raw content type.
        /// </summary>
        public string RawContentType { get; set; }

        /// <summary>
        /// Gets sets the http context raw uri.
        /// </summary>
        public Uri RawUri
        {
            get { return _rawUri; }
            set
            {
                _rawUri = value;

                NameValueCollection newParameters = HttpUtility.ParseQueryString(_rawUri.Query);

                // TODO: tidy this up, bit clunky

                foreach (string parameter in newParameters)
                {
                    QueryParameters[parameter] = newParameters[parameter];
                }

                _normalizedRequestUrl = UriUtility.NormalizeUri(_rawUri);
            }
        }

        /// <summary>
        /// Gets sets the http context normalized request url.
        /// </summary>
        public string NormalizedRequestUrl
        {
            get { return _normalizedRequestUrl; }
        }

        /// <summary>
        /// Gets sets the http context request method.
        /// </summary>
        public string RequestMethod { get; set; }

        /// <summary>
        /// Gets sets the include oauth request body hash in signature
        /// </summary>
        public bool IncludeOAuthRequestBodyHashInSignature { get; set; }

        /// <summary>
        /// Gets sets the nonce.
        /// </summary>
        public string Nonce
        {
            get { return _nonce; }
            set { _nonce = value; }
        }

        /// <summary>
        /// Gets sets the verifier.
        /// </summary>
        public string Verifier
        {
            get { return _verifier; }
            set { _verifier = value; }
        }

        /// <summary>
        /// Gets sets the session handler
        /// </summary>
        public string SessionHandle
        {
            get { return _sessionHandle; }
            set { _sessionHandle = value; }
        }

        /// <summary>
        /// Gets sets the callback url.
        /// </summary>
        public string CallbackUrl
        {
            get { return _callbackUrl; }
            set { _callbackUrl = value; }
        }

        /// <summary>
        /// Gets sets the signature.
        /// </summary>
        public string Signature
        {
            get { return _signature; }
            set { _signature = value; }
        }

        /// <summary>
        /// Gets sets the signature method
        /// </summary>
        public string SignatureMethod
        {
            get { return _signatureMethod; }
            set { _signatureMethod = value; }
        }

        /// <summary>
        /// Gets sets the timestamp.
        /// </summary>
        public string Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        /// <summary>
        /// Gets sets the body hash.
        /// </summary>
        public string BodyHash
        {
            get { return _bodyHash; }
            set { _bodyHash = value; }
        }

        /// <summary>
        /// Gets sets the version.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Gets sets the use authorization header.
        /// </summary>
        public bool UseAuthorizationHeader { get; set; }

        /// <summary>
        /// Gets sets the realm
        /// </summary>
        public string Realm
        {
            get { return AuthorizationHeaderParameters[Parameters.Realm]; }
            set { AuthorizationHeaderParameters[Parameters.Realm] = value; }
        }

        /// <summary>
        /// Gets sets the consumer key
        /// </summary>
        public string ConsumerKey
        {
            get { return _consumerKey; }
            set { _consumerKey = value; }
        }

        /// <summary>
        /// Gets sets the token
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// Gets sets the token secret
        /// </summary>
        public string TokenSecret
        {
            get { return _tokenSecret; }
            set { _tokenSecret = value; }
        }

        /// <summary>
        /// Gets sets the XAuth mode.
        /// </summary>
        public string XAuthMode
        {
            get { return _xAuthMode; }
            set { _xAuthMode = value; }
        }

        /// <summary>
        /// Gets sets the XAuth username.
        /// </summary>
        public string XAuthUsername
        {
            get { return _xAuthUsername; }
            set { _xAuthUsername = value; }
        }

        /// <summary>
        /// Gets sets the XAuth password.
        /// </summary>
        public string XAuthPassword
        {
            get { return _xAuthPassword; }
            set { _xAuthPassword = value; }
        }

        /// <summary>
        /// Generate the uri.
        /// </summary>
        /// <returns>The URI.</returns>
        public Uri GenerateUri()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            IEnumerable<QueryParameter> parameters = QueryParameters.ToQueryParametersExcludingTokenSecret();

            builder.Query = UriUtility.FormatQueryString(parameters);

            return builder.Uri;
        }

        /// <summary>
        /// Generate the uri.
        /// </summary>
        /// <returns>The URI string.</returns>
        public string GenerateUrl()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            builder.Query = "";

            return builder.Uri + "?" + UriUtility.FormatQueryString(QueryParameters);
        }

        /// <summary>
        /// Generate OAuth parameters for header.
        /// </summary>
        /// <returns>The generated value.</returns>
        public string GenerateOAuthParametersForHeader()
        {
            var builder = new StringBuilder();

            if (Realm != null) builder.Append("realm=\"").Append(Realm).Append("\"");

            IEnumerable<QueryParameter> parameters = AuthorizationHeaderParameters.ToQueryParametersExcludingTokenSecret();

            foreach (
                var parameter in parameters.Where(p => p.Key != Parameters.Realm)
                )
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(UriUtility.UrlEncode(parameter.Key)).Append("=\"").Append(
                    UriUtility.UrlEncode(parameter.Value)).Append("\"");
            }

            builder.Insert(0, "OAuth ");

            return builder.ToString();
        }

        /// <summary>
        /// Generate uri without OAuth parameters.
        /// </summary>
        /// <returns>The generated URI.</returns>
        public Uri GenerateUriWithoutOAuthParameters()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            IEnumerable<QueryParameter> parameters = QueryParameters.ToQueryParameters()
                .Where(q => !q.Key.StartsWith(Parameters.OAuthParameterPrefix) && !q.Key.StartsWith(Parameters.XAuthParameterPrefix));

            builder.Query = UriUtility.FormatQueryString(parameters);

            return builder.Uri;
        }

        /// <summary>
        /// Generate and set body hash
        /// </summary>
        public void GenerateAndSetBodyHash()
        {
            BodyHash = GenerateBodyHash();
        }

        /// <summary>
        /// Generate body hash.
        /// </summary>
        /// <returns>The generated value.</returns>
        public string GenerateBodyHash()
        {
            byte[] hash = SHA1.Create().ComputeHash((RawContent ?? new byte[0]));
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generate signature base.
        /// </summary>
        /// <returns>The generated value.</returns>
        public string GenerateSignatureBase()
        {
            if (string.IsNullOrEmpty(ConsumerKey))
            {
                throw Error.MissingRequiredOAuthParameter(this, Parameters.OAuth_Consumer_Key);
            }

            if (string.IsNullOrEmpty(SignatureMethod))
            {
                throw Error.MissingRequiredOAuthParameter(this, Parameters.OAuth_Signature_Method);
            }

            if (string.IsNullOrEmpty(RequestMethod))
            {
                throw Error.RequestMethodHasNotBeenAssigned("RequestMethod");
            }

            if (IncludeOAuthRequestBodyHashInSignature)
            {
                GenerateAndSetBodyHash();
            }

            var allParameters = new List<QueryParameter>();

            //fix for issue: http://groups.google.com/group/oauth/browse_thread/thread/42ef5fecc54a7e9a/a54e92b13888056c?hl=en&lnk=gst&q=Signing+PUT+Request#a54e92b13888056c
            if (FormEncodedParameters != null && RequestMethod == "POST")
                allParameters.AddRange(FormEncodedParameters.ToQueryParametersExcludingTokenSecret());

            if (QueryParameters != null) allParameters.AddRange(QueryParameters.ToQueryParametersExcludingTokenSecret());

            if (Cookies != null) allParameters.AddRange(Cookies.ToQueryParametersExcludingTokenSecret());

            if (AuthorizationHeaderParameters != null)
                allParameters.AddRange(AuthorizationHeaderParameters.ToQueryParametersExcludingTokenSecret().Where(q => q.Key != Parameters.Realm));

            // patch from http://code.google.com/p/devdefined-tools/issues/detail?id=10
            //if(RawContent != null)
            //    allParameters.Add(new QueryParameter("raw", RawContent));

            allParameters.RemoveAll(param => param.Key == Parameters.OAuth_Signature);

            string signatureBase = UriUtility.FormatParameters(RequestMethod, new Uri(NormalizedRequestUrl), allParameters);

            return signatureBase;
        }

        /// <summary>
        /// Translate the signature method to the siganture type.
        /// </summary>
        /// <param name="signatureMethod">The signature method.</param>
        /// <returns>The siganture type.</returns>
        public Nequeo.Net.OAuth.SignatureTypes TranslateSignatureType(string signatureMethod)
        {
            switch (signatureMethod.ToLower())
            {
                case "hmac-sha1":
                    return SignatureTypes.HMAC_SHA1;
                case "plaintext":
                    return SignatureTypes.PLAIN_TEXT;
                case "rsa-sha1":
                    return SignatureTypes.RSA_SHA1;
                default:
                    return SignatureTypes.PLAIN_TEXT;
            }
        }
    }
}
