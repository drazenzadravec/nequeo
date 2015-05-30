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

using Nequeo.Net.OAuth.Provider;
using Nequeo.Net.OAuth.Provider.Inspectors;
using Nequeo.Net.OAuth.Consumer;
using Nequeo.Net.OAuth.Consumer.Session;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework.Utility;

using Nequeo.Net.OAuth.Storage.Basic;
using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth
{
    /// <summary>
    /// OAuth provider
    /// </summary>
    public class AuthProvider
	{
        /// <summary>
        /// OAuth provider.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        public AuthProvider(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore)
        {
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;

            ValidateEx();

            // Create a new OAuth provider.
            _oAuthProvider = new OAuthProvider(tokenStore, 
                new SignatureValidationInspector(consumerStore),
                new NonceStoreInspector(nonceStore),
                new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                new ConsumerValidationInspector(consumerStore),
                new XAuthValidationInspector(ValidateXAuthMode, AuthenticateXAuthUsernameAndPassword));
        }

        /// <summary>
        /// OAuth provider.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        /// <param name="inspectors">The collection of validation inspectors.</param>
        public AuthProvider(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore, params IContextInspector[] inspectors)
        {
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;

            ValidateEx();

            // Create a new OAuth provider.
            _oAuthProvider = new OAuthProvider(tokenStore, inspectors);
        }

        private OAuthProvider _oAuthProvider = null;

        private ITokenStore _tokenStore = null;
        private IConsumerStore _consumerStore = null;
        private INonceStore _nonceStore = null;

        private string _tokenError = string.Empty;

        /// <summary>
        /// Create a request token from the request.
        /// </summary>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <returns>The token if successful; else null.</returns>
        public string CreateRequestToken(Uri rawUri, NameValueCollection queryString, 
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies)
        {
            try
            {
                // Make sure that all the passed parameters are valid.
                if (rawUri == null) throw new ArgumentNullException("rawUri");
                if (queryString == null) throw new ArgumentNullException("queryString");
                if (form == null) throw new ArgumentNullException("form");
                if (headers == null) throw new ArgumentNullException("headers");
                if (cookies == null) throw new ArgumentNullException("cookies");

                // Make sure that all the maditory OAuth parameters
                // have been passed to the provider from the consumer
                OAuthProblemReport validate = new OAuthProblemReport(queryString);
                validate.ValidateRequestParametersAbsent(queryString);
                string validationError = validate.ToString();

                // If any of the maditory OAuth parameters are missing.
                if(!String.IsNullOrEmpty(validationError))
                    throw new OAuthException(OAuthProblemParameters.ParameterAbsent, "Absent Parameters", new Exception(validationError));

                // Create an assign each manditory parameter.
                IOAuthContext context = new OAuthContextProvider();
                context.RawUri = rawUri;
                context.RequestMethod = "GET";
                context.Headers = headers;
                context.QueryParameters = queryString;
                context.FormEncodedParameters = form;
                context.CallbackUrl = queryString[Parameters.OAuth_Callback];
                context.Nonce = queryString[Parameters.OAuth_Nonce];
                context.ConsumerKey = queryString[Parameters.OAuth_Consumer_Key];
                context.SignatureMethod = queryString[Parameters.OAuth_Signature_Method];
                context.Timestamp = queryString[Parameters.OAuth_Timestamp];
                context.Signature = queryString[Parameters.OAuth_Signature];

                // Assign each optional parameter
                GetOptionalRequestParameters(context, queryString);

                // Create the request token from the stores.
                IToken token = _oAuthProvider.GrantRequestToken(context);
                RequestToken requestToken = new RequestToken()
                {
                    Token = token.Token,
                    TokenSecret = token.TokenSecret,
                    ConsumerKey = token.ConsumerKey,
                    CallbackUrl = context.CallbackUrl,
                    SessionHandle = token.SessionHandle,
                    Realm = token.Realm
                };

                // Return the request token string.
                return requestToken.ToString();
            }
            catch (OAuthException aex)
            {
                // Get the current token errors.
                _tokenError = aex.Report.ToString();
                return null;
            }
            catch (Exception ex)
            {
                // Transform the execption.
                OAuthException OAuthException =
                    new OAuthException(OAuthProblemParameters.ParameterRejected, ex.Message, ex);

                // Get the current token errors.
                _tokenError = OAuthException.Report.ToString();
                return null;
            }
        }

        /// <summary>
        /// Create a access token from the request.
        /// </summary>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <returns>The token if successful; else null.</returns>
        public string CreateAccessToken(Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies)
        {
            try
            {
                // Make sure that all the passed parameters are valid.
                if (rawUri == null) throw new ArgumentNullException("rawUri");
                if (queryString == null) throw new ArgumentNullException("queryString");
                if (form == null) throw new ArgumentNullException("form");
                if (headers == null) throw new ArgumentNullException("headers");
                if (cookies == null) throw new ArgumentNullException("cookies");

                // Make sure that all the maditory OAuth parameters
                // have been passed to the provider from the consumer
                OAuthProblemReport validate = new OAuthProblemReport(queryString);
                validate.ValidateAccessParametersAbsent(queryString);
                string validationError = validate.ToString();

                // If any of the maditory OAuth parameters are missing.
                if (!String.IsNullOrEmpty(validationError))
                    throw new OAuthException(OAuthProblemParameters.ParameterAbsent, "Absent Parameters", new Exception(validationError));

                // Create an assign each manditory parameter.
                IOAuthContext context = new OAuthContextProvider();
                context.RawUri = rawUri;
                context.RequestMethod = "GET";
                context.Headers = headers;
                context.QueryParameters = queryString;
                context.FormEncodedParameters = form;
                context.Token = queryString[Parameters.OAuth_Token];
                context.Nonce = queryString[Parameters.OAuth_Nonce];
                context.ConsumerKey = queryString[Parameters.OAuth_Consumer_Key];
                context.SignatureMethod = queryString[Parameters.OAuth_Signature_Method];
                context.Timestamp = queryString[Parameters.OAuth_Timestamp];
                context.Signature = queryString[Parameters.OAuth_Signature];
                context.Verifier = queryString[Parameters.OAuth_Verifier];

                // Assign each optional parameter
                GetOptionalAccessParameters(context, queryString);

                // Create the access token from the stores.
                IToken token = _oAuthProvider.ExchangeRequestTokenForAccessToken(context);
                return UriUtility.FormatTokenForResponse(token);
            }
            catch (OAuthException aex)
            {
                // Get the current token errors.
                _tokenError = aex.Report.ToString();
                return null;
            }
            catch (Exception ex)
            {
                // Transform the execption.
                OAuthException OAuthException =
                    new OAuthException(OAuthProblemParameters.ParameterRejected, ex.Message, ex);

                // Get the current token errors.
                _tokenError = OAuthException.Report.ToString();
                return null;
            }
        }

        /// <summary>
        /// Create a authorise token from the request.
        /// </summary>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <param name="isApprovedByUser">Has the user approved the client to access the resources.</param>
        /// <returns>The formatted redirect url; else null.</returns>
        public string CreateAuthoriseToken(Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies, bool isApprovedByUser = false)
        {
            try
            {
                // Make sure that all the passed parameters are valid.
                if (rawUri == null) throw new ArgumentNullException("rawUri");
                if (queryString == null) throw new ArgumentNullException("queryString");
                if (form == null) throw new ArgumentNullException("form");
                if (headers == null) throw new ArgumentNullException("headers");
                if (cookies == null) throw new ArgumentNullException("cookies");

                // Only process if the user has approved the request.
                if (isApprovedByUser)
                {
                    // Make sure that all the maditory OAuth parameters
                    // have been passed to the provider from the consumer
                    OAuthProblemReport validate = new OAuthProblemReport(queryString);
                    validate.ValidateAuthoriseParametersAbsent(queryString);
                    string validationError = validate.ToString();

                    // If any of the maditory OAuth parameters are missing.
                    if (!String.IsNullOrEmpty(validationError))
                        throw new OAuthException(OAuthProblemParameters.ParameterAbsent, "Absent Parameters", new Exception(validationError));

                    // Create an assign each manditory parameter.
                    IOAuthContext context = new OAuthContextProvider();
                    context.RawUri = rawUri;
                    context.RequestMethod = "GET";
                    context.Headers = headers;
                    context.QueryParameters = queryString;
                    context.FormEncodedParameters = form;
                    context.Token = queryString[Parameters.OAuth_Token];
                    context.CallbackUrl = queryString[Parameters.OAuth_Callback];
                    string companyUniqueUserID = queryString[Parameters.Company_Unique_User_Identifier];

                    // Assign each optional parameter
                    GetOptionalAuthoriseParameters(context, queryString);

                    // Create a new OAuth provider.
                    _oAuthProvider = new OAuthProvider(_tokenStore,
                        new NonceStoreInspector(_nonceStore),
                        new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                        new ConsumerValidationInspector(_consumerStore),
                        new XAuthValidationInspector(ValidateXAuthMode, AuthenticateXAuthUsernameAndPassword));

                    // Create the access token from the stores, and create a new verification code.
                    string verifier = _consumerStore.SetVerificationCode(context, companyUniqueUserID);
                    IToken token = _oAuthProvider.CreateAccessToken(context);

                    // Create the parameter response.
                    NameValueCollection parameters = new NameValueCollection();
                    parameters[Parameters.OAuth_Token] = token.Token;
                    parameters[Parameters.OAuth_Verifier] = verifier;

                    // Return the token callback query string..
                    return context.CallbackUrl + "?" + UriUtility.FormatQueryString(parameters);
                }
                else
                    throw new OAuthException(OAuthProblemParameters.PermissionDenied, "Authorisation Denied", new Exception("User has denied access"));
            }
            catch (OAuthException aex)
            {
                // Get the current token errors.
                _tokenError = aex.Report.ToString();
                return null;
            }
            catch (Exception ex)
            {
                // Transform the execption.
                OAuthException OAuthException =
                    new OAuthException(OAuthProblemParameters.ParameterRejected, ex.Message, ex);

                // Get the current token errors.
                _tokenError = OAuthException.Report.ToString();
                return null;
            }
        }

        /// <summary>
        /// Get the current OAuth provider error.
        /// </summary>
        /// <returns>The current OAuth provider error.</returns>
        public string GetTokenError()
        {
            return _tokenError;
        }

        /// <summary>
        /// Authenticate XAuth
        /// </summary>
        /// <param name="authMode">The authentication mode</param>
        /// <returns>True if the XAuth mode match; else false.</returns>
        internal bool ValidateXAuthMode(string authMode)
        {
            return authMode == "client_auth";
        }

        /// <summary>
        /// Authenticate XAuth
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The password.</param>
        /// <returns>True if user is authenticated; else false.</returns>
        internal bool AuthenticateXAuthUsernameAndPassword(string username, string password)
        {
            return username == "username" && password == "password";
        }

        /// <summary>
        /// Validate all the parameters
        /// </summary>
        private void ValidateEx()
        {
            if (_tokenStore == null) throw new ArgumentNullException("tokenStore");
            if (_consumerStore == null) throw new ArgumentNullException("consumerStore");
            if (_nonceStore == null) throw new ArgumentNullException("nonceStore");
        }

        /// <summary>
        /// Get all the optional request parameters
        /// </summary>
        /// <param name="context">The current provider context.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        private void GetOptionalRequestParameters(IOAuthContext context, NameValueCollection queryString)
        {
            if (queryString[Parameters.OAuth_Version] != null)
                context.Version = queryString[Parameters.OAuth_Version];

            if (queryString[Parameters.OAuth_Body_Hash] != null)
                context.BodyHash = queryString[Parameters.OAuth_Body_Hash];

            if (queryString[Parameters.OAuth_Session_Handle] != null)
                context.SessionHandle = queryString[Parameters.OAuth_Session_Handle];

            if (queryString[Parameters.OAuth_Token] != null)
                context.Token = queryString[Parameters.OAuth_Token];

            if (queryString[Parameters.OAuth_Token_Secret] != null)
                context.TokenSecret = queryString[Parameters.OAuth_Token_Secret];

            if (queryString[Parameters.OAuth_Verifier] != null)
                context.Verifier = queryString[Parameters.OAuth_Verifier];

            if (queryString[Parameters.XAuthMode] != null)
                context.XAuthMode = queryString[Parameters.XAuthMode];

            if (queryString[Parameters.XAuthUsername] != null)
                context.XAuthUsername = queryString[Parameters.XAuthUsername];

            if (queryString[Parameters.XAuthPassword] != null)
                context.XAuthPassword = queryString[Parameters.XAuthPassword];
        }

        /// <summary>
        /// Get all the optional access parameters
        /// </summary>
        /// <param name="context">The current provider context.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        private void GetOptionalAccessParameters(IOAuthContext context, NameValueCollection queryString)
        {
            if (queryString[Parameters.OAuth_Version] != null)
                context.Version = queryString[Parameters.OAuth_Version];

            if (queryString[Parameters.OAuth_Body_Hash] != null)
                context.BodyHash = queryString[Parameters.OAuth_Body_Hash];

            if (queryString[Parameters.OAuth_Session_Handle] != null)
                context.SessionHandle = queryString[Parameters.OAuth_Session_Handle];

            if (queryString[Parameters.OAuth_Callback] != null)
                context.CallbackUrl = queryString[Parameters.OAuth_Callback];

            if (queryString[Parameters.OAuth_Token_Secret] != null)
                context.TokenSecret = queryString[Parameters.OAuth_Token_Secret];

            if (queryString[Parameters.XAuthMode] != null)
                context.XAuthMode = queryString[Parameters.XAuthMode];

            if (queryString[Parameters.XAuthUsername] != null)
                context.XAuthUsername = queryString[Parameters.XAuthUsername];

            if (queryString[Parameters.XAuthPassword] != null)
                context.XAuthPassword = queryString[Parameters.XAuthPassword];
        }

        /// <summary>
        /// Get all the optional access parameters
        /// </summary>
        /// <param name="context">The current provider context.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        private void GetOptionalAuthoriseParameters(IOAuthContext context, NameValueCollection queryString)
        {
            if (queryString[Parameters.OAuth_Nonce] != null)
                context.Nonce = queryString[Parameters.OAuth_Nonce];

            if (queryString[Parameters.OAuth_Consumer_Key] != null)
                context.ConsumerKey = queryString[Parameters.OAuth_Consumer_Key];

            if (queryString[Parameters.OAuth_Signature_Method] != null)
                context.SignatureMethod = queryString[Parameters.OAuth_Signature_Method];

            if (queryString[Parameters.OAuth_Version] != null)
                context.Version = queryString[Parameters.OAuth_Version];

            if (queryString[Parameters.OAuth_Timestamp] != null)
                context.Timestamp = queryString[Parameters.OAuth_Timestamp];

            if (queryString[Parameters.OAuth_Signature] != null)
                context.Signature = queryString[Parameters.OAuth_Signature];

            if (queryString[Parameters.OAuth_Body_Hash] != null)
                context.BodyHash = queryString[Parameters.OAuth_Body_Hash];

            if (queryString[Parameters.OAuth_Session_Handle] != null)
                context.SessionHandle = queryString[Parameters.OAuth_Session_Handle];

            if (queryString[Parameters.OAuth_Token_Secret] != null)
                context.TokenSecret = queryString[Parameters.OAuth_Token_Secret];

            if (queryString[Parameters.OAuth_Verifier] != null)
                context.Verifier = queryString[Parameters.OAuth_Verifier];

            if (queryString[Parameters.XAuthMode] != null)
                context.XAuthMode = queryString[Parameters.XAuthMode];

            if (queryString[Parameters.XAuthUsername] != null)
                context.XAuthUsername = queryString[Parameters.XAuthUsername];

            if (queryString[Parameters.XAuthPassword] != null)
                context.XAuthPassword = queryString[Parameters.XAuthPassword];
        }
	}
}
