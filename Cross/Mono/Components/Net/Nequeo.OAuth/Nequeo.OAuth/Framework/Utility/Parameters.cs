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
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// OAuth Uri parameters.
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        /// application/x-www-form-urlencoded; charset=utf-8
        /// </summary>
        public const string HttpFormEncoded = "application/x-www-form-urlencoded; charset=utf-8";
        /// <summary>
        /// oauth_
        /// </summary>
        public const string OAuthParameterPrefix = "oauth_";
        /// <summary>
        /// oauth_acceptable_timestamps
        /// </summary>
        public const string OAuth_Acceptable_Timestamps = "oauth_acceptable_timestamps";
        /// <summary>
        /// oauth_acceptable_versions
        /// </summary>
        public const string OAuth_Acceptable_Versions = "oauth_acceptable_versions";
        /// <summary>
        /// oauth_authorization_expires_in
        /// </summary>
        public const string OAuth_Authorization_Expires_In = "oauth_authorization_expires_in";
        /// <summary>
        /// Authorization
        /// </summary>
        public const string OAuth_Authorization_Header = "Authorization";
        /// <summary>
        /// oauth_body_hash
        /// </summary>
        public const string OAuth_Body_Hash = "oauth_body_hash";
        /// <summary>
        /// oauth_callback
        /// </summary>
        public const string OAuth_Callback = "oauth_callback";
        /// <summary>
        /// oauth_callback_confirmed
        /// </summary>
        public const string OAuth_Callback_Confirmed = "oauth_callback_confirmed";
        /// <summary>
        /// oauth_consumer_key
        /// </summary>
        public const string OAuth_Consumer_Key = "oauth_consumer_key";
        /// <summary>
        /// oauth_expires_in
        /// </summary>
        public const string OAuth_Expires_In = "oauth_expires_in";
        /// <summary>
        /// oauth_nonce
        /// </summary>
        public const string OAuth_Nonce = "oauth_nonce";
        /// <summary>
        /// oauth_parameters_absent
        /// </summary>
        public const string OAuth_Parameters_Absent = "oauth_parameters_absent";
        /// <summary>
        /// oauth_parameters_rejected
        /// </summary>
        public const string OAuth_Parameters_Rejected = "oauth_parameters_rejected";
        /// <summary>
        /// oauth_problem
        /// </summary>
        public const string OAuth_Problem = "oauth_problem";
        /// <summary>
        /// oauth_problem_advice
        /// </summary>
        public const string OAuth_Problem_Advice = "oauth_problem_advice";
        /// <summary>
        /// oauth_session_handle
        /// </summary>
        public const string OAuth_Session_Handle = "oauth_session_handle";
        /// <summary>
        /// oauth_signature
        /// </summary>
        public const string OAuth_Signature = "oauth_signature";
        /// <summary>
        /// oauth_signature_method
        /// </summary>
        public const string OAuth_Signature_Method = "oauth_signature_method";
        /// <summary>
        /// oauth_timestamp
        /// </summary>
        public const string OAuth_Timestamp = "oauth_timestamp";
        /// <summary>
        /// oauth_token
        /// </summary>
        public const string OAuth_Token = "oauth_token";
        /// <summary>
        /// oauth_token_secret
        /// </summary>
        public const string OAuth_Token_Secret = "oauth_token_secret";
        /// <summary>
        /// oauth_verifier
        /// </summary>
        public const string OAuth_Verifier = "oauth_verifier";
        /// <summary>
        /// oauth_version
        /// </summary>
        public const string OAuth_Version = "oauth_version";
        /// <summary>
        /// realm
        /// </summary>
        public const string Realm = "realm";
        /// <summary>
        /// x_auth_
        /// </summary>
        public const string XAuthParameterPrefix = "x_auth_";
        /// <summary>
        /// x_auth_username
        /// </summary>
        public const string XAuthUsername = "x_auth_username";
        /// <summary>
        /// x_auth_password
        /// </summary>
        public const string XAuthPassword = "x_auth_password";
        /// <summary>
        /// x_auth_mode
        /// </summary>
        public const string XAuthMode = "x_auth_mode";
        /// <summary>
        /// The company unique user identifier.
        /// </summary>
        public const string Company_Unique_User_Identifier = "com_unique_uid";
    }
}
