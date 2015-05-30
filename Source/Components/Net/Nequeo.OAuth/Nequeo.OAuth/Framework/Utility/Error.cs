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
using System.Net;

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// Error control helper.
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// Missing Required OAuth Parameter
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>The exception.</returns>
        public static OAuthException MissingRequiredOAuthParameter(IOAuthContext context, string parameterName)
        {
            var exception = new OAuthException(context, OAuthProblemParameters.ParameterAbsent,
                                               string.Format("Missing required parameter : {0}", parameterName));

            exception.Report.ParametersAbsent.Add(parameterName);
            return exception;
        }

        /// <summary>
        /// OAuth Authentication Failure
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The exception.</returns>
        public static Exception OAuthAuthenticationFailure(string errorMessage)
        {
            return new Exception(string.Format("OAuth authentication failed, message was: {0}", errorMessage));
        }

        /// <summary>
        /// Token Can No Longer Be Used
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The exception.</returns>
        public static Exception TokenCanNoLongerBeUsed(string token)
        {
            return new Exception(string.Format("Token \"{0}\" is no longer valid", token));
        }

        /// <summary>
        /// Failed To Parse Response
        /// </summary>
        /// <param name="parameters">The paramters.</param>
        /// <returns>The exception.</returns>
        public static Exception FailedToParseResponse(string parameters)
        {
            return new Exception(string.Format("Failed to parse response string \"{0}\"", parameters));
        }

        /// <summary>
        /// Unknown Signature Method
        /// </summary>
        /// <param name="signatureMethod">The signature Method</param>
        /// <returns>The exception.</returns>
        public static Exception UnknownSignatureMethod(string signatureMethod)
        {
            return new Exception(string.Format("Unknown signature method \"{0}\"", signatureMethod));
        }

        /// <summary>
        /// For Rsa Sha1 Signature Method You Must Supply Assymetric Key Parameter
        /// </summary>
        /// <returns>The exception.</returns>
        public static Exception ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter()
        {
            return
                new Exception(
                    "For the RSASSA-PKCS1-v1_5 signature method you must use the constructor which takes an additional AssymetricAlgorithm \"key\" parameter");
        }

        /// <summary>
        /// Request Failed
        /// </summary>
        /// <param name="innerException">The inner web exception.</param>
        /// <returns>The exception.</returns>
        public static Exception RequestFailed(WebException innerException)
        {
            var response = innerException.Response as HttpWebResponse;

            if (response != null)
            {
                using (var reader = new StreamReader(innerException.Response.GetResponseStream()))
                {
                    string body = reader.ReadToEnd();

                    return
                        new Exception(
                            string.Format(
                                "Request for uri: {0} failed.\r\nstatus code: {1}\r\nheaders: {2}\r\nbody:\r\n{3}",
                                response.ResponseUri, response.StatusCode, response.Headers, body), innerException);
                }
            }

            return innerException;
        }

        /// <summary>
        /// Empty Consumer Key
        /// </summary>
        /// <returns>The exception.</returns>
        public static Exception EmptyConsumerKey()
        {
            throw new Exception("Consumer key is null or empty");
        }

        /// <summary>
        /// Request Method Has Not Been Assigned
        /// </summary>
        /// <param name="parameter">The parameters</param>
        /// <returns>The exception.</returns>
        public static Exception RequestMethodHasNotBeenAssigned(string parameter)
        {
            return new Exception(string.Format("The RequestMethod parameter \"{0}\" is null or empty.", parameter));
        }

        /// <summary>
        /// Failed To Validate Signature
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <returns>The exception.</returns>
        public static Exception FailedToValidateSignature(IOAuthContext context)
        {
            return new OAuthException(context, OAuthProblemParameters.SignatureInvalid, "Failed to validate signature");
        }

        /// <summary>
        /// Failed To Validate Body Hash
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <returns>The exception.</returns>
        public static Exception FailedToValidateBodyHash(IOAuthContext context)
        {
            return new OAuthException(context, OAuthProblemParameters.BodyHashInvalid, "Failed to validate body hash");
        }

        /// <summary>
        /// Unknown Consumer Key
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <returns>The exception.</returns>
        public static Exception UnknownConsumerKey(IOAuthContext context)
        {
            return new OAuthException(context, OAuthProblemParameters.ConsumerKeyUnknown,
                                      string.Format("Unknown Consumer (Realm: {0}, Key: {1})", context.Realm,
                                                    context.ConsumerKey));
        }

        /// <summary>
        /// Algorithm Property Not Set On Signing Context
        /// </summary>
        /// <returns>The exception.</returns>
        public static Exception AlgorithmPropertyNotSetOnSigningContext()
        {
            return
                new Exception(
                    "Algorithm Property must be set on SingingContext when using an Assymetric encryption method such as RSA-SHA1");
        }

        /// <summary>
        /// Supplied Token Was Not Issued To This Consumer
        /// </summary>
        /// <param name="expectedConsumerKey">The expected consumer key</param>
        /// <param name="actualConsumerKey">The actual consumer key</param>
        /// <returns>The exception.</returns>
        public static Exception SuppliedTokenWasNotIssuedToThisConsumer(string expectedConsumerKey,
                                                                        string actualConsumerKey)
        {
            return
                new Exception(
                    string.Format("Supplied token was not issued to this consumer, expected key: {0}, actual key: {1}",
                                  expectedConsumerKey, actualConsumerKey));
        }

        /// <summary>
        /// Access Denied To Protected Resource
        /// </summary>
        /// <param name="outcome">The access outcome</param>
        /// <returns>The exception.</returns>
        public static Exception AccessDeniedToProtectedResource(AccessOutcome outcome)
        {
            Uri uri = outcome.Context.GenerateUri();

            if (string.IsNullOrEmpty(outcome.AdditionalInfo))
            {
                return new AccessDeniedException(outcome, string.Format("Access to resource \"{0}\" was denied", uri));
            }

            return new AccessDeniedException(outcome,
                                             string.Format("Access to resource: {0} was denied, additional info: {1}",
                                                           uri, outcome.AdditionalInfo));
        }

        /// <summary>
        /// Consumer Has Not Been Granted Access Yet
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <returns>The exception.</returns>
        public static Exception ConsumerHasNotBeenGrantedAccessYet(IOAuthContext context)
        {
            return new OAuthException(context, OAuthProblemParameters.PermissionUnknown,
                                      "The decision to give access to the consumer has yet to be made, please try again later.");
        }

        /// <summary>
        /// Consumer Has Been Denied Access
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <returns>The exception.</returns>
        public static Exception ConsumerHasBeenDeniedAccess(IOAuthContext context)
        {
            return new OAuthException(context, OAuthProblemParameters.PermissionDenied,
                                      "The consumer was denied access to this resource.");
        }

        /// <summary>
        /// Cant Build Problem Report When Problem Empty
        /// </summary>
        /// <returns>The exception.</returns>
        public static Exception CantBuildProblemReportWhenProblemEmpty()
        {
            return new Exception("Can't build problem report when \"Problem\" property is null or empty");
        }

        /// <summary>
        /// Nonce Has Already Been Used
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <returns>The exception.</returns>
        public static Exception NonceHasAlreadyBeenUsed(IOAuthContext context)
        {
            return new OAuthException(context, OAuthProblemParameters.NonceUsed,
                                      string.Format("The nonce value \"{0}\" has already been used", context.Nonce));
        }

        /// <summary>
        /// This Consumer Request Has Already Been Signed
        /// </summary>
        /// <returns>The exception.</returns>
        public static Exception ThisConsumerRequestHasAlreadyBeenSigned()
        {
            return new Exception("The consumer request for consumer \"{0}\" has already been signed");
        }

        /// <summary>
        /// Callback Was Not Confirmed
        /// </summary>
        /// <returns>The exception.</returns>
        public static Exception CallbackWasNotConfirmed()
        {
            return new Exception("Callback was not confirmed");
        }

        /// <summary>
        /// Rejected Required OAuth Parameter
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <param name="parameter">The paramter</param>
        /// <returns>The exception.</returns>
        public static Exception RejectedRequiredOAuthParameter(IOAuthContext context, string parameter)
        {
            return new OAuthException(context, OAuthProblemParameters.ParameterRejected, string.Format("The parameter \"{0}\" was rejected", parameter));
        }

        /// <summary>
        /// Unknown Token
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <param name="token">The token</param>
        /// <returns>The exception.</returns>
        public static Exception UnknownToken(IOAuthContext context, string token)
        {
            return new OAuthException(context, OAuthProblemParameters.TokenRejected, string.Format("Unknown or previously rejected token \"{0}\"", token));
        }

        /// <summary>
        /// Unknown Token
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <param name="token">The token</param>
        /// <param name="exception">The exception.</param>
        /// <returns>The exception.</returns>
        public static Exception UnknownToken(IOAuthContext context, string token, Exception exception)
        {
            return new OAuthException(context, OAuthProblemParameters.TokenRejected, string.Format("Unknown or previously rejected token \"{0}\"", token), exception);
        }

        /// <summary>
        /// Request For Token Must Not Include Token In Context
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception RequestForTokenMustNotIncludeTokenInContext(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterRejected, "When obtaining a request token, you must not supply the oauth_token parameter");
        }

        /// <summary>
        /// Experiencing Issue With Creating Uri Due To Missing App Config
        /// </summary>
        /// <param name="argumentException">The argument null exception.</param>
        /// <returns>The exception.</returns>
        public static Exception ExperiencingIssueWithCreatingUriDueToMissingAppConfig(ArgumentNullException argumentException)
        {
            return
                new Exception(
                    "It appears this may be the first Uri constructed by this AppDomain, and you have no App.config or Web.config file - which has triggered an unusual edge case: see this blog post from more details - http://ayende.com/Blog/archive/2010/03/04/is-select-system.uri-broken.aspx",
                    argumentException);
        }

        /// <summary>
        /// Encountered Unexpected Body Hash In Form Encoded Request
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception EncounteredUnexpectedBodyHashInFormEncodedRequest(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterRejected, "Encountered unexpected oauth_body_hash value in form-encoded request");
        }

        /// <summary>
        /// Empty XAuth Mode
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception EmptyXAuthMode(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterAbsent, "The x_auth_mode parameter must be present");
        }

        /// <summary>
        /// Invalid XAuth Mode
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception InvalidXAuthMode(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterRejected, "The x_auth_mode parameter is invalid");
        }

        /// <summary>
        /// Empty XAuth Username
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception EmptyXAuthUsername(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterAbsent, "The x_auth_username parameter must be present");
        }

        /// <summary>
        /// Empty XAuth Password
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception EmptyXAuthPassword(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterAbsent, "The x_auth_password parameter must be present");
        }

        /// <summary>
        /// Failed XAuth Authentication
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        /// <returns>The exception.</returns>
        public static Exception FailedXAuthAuthentication(IOAuthContext context)
        {
            throw new OAuthException(context, OAuthProblemParameters.ParameterRejected, "Authentication failed with the specified username and password");
        }
    }
}
