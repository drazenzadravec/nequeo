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
    /// OAuth Uri problem parameters.
    /// </summary>
    public static class OAuthProblemParameters
	{
        /// <summary>
        /// additional_authorization_required
        /// </summary>
		public const string AdditionalAuthorizationRequired = "additional_authorization_required";
        /// <summary>
        /// body_hash_invalid
        /// </summary>
		public const string BodyHashInvalid = "body_hash_invalid";
        /// <summary>
        /// consumer_key_refused
        /// </summary>
		public const string ConsumerKeyRefused = "consumer_key_refused";
        /// <summary>
        /// consumer_key_rejected
        /// </summary>
		public const string ConsumerKeyRejected = "consumer_key_rejected";
        /// <summary>
        /// consumer_key_unknown
        /// </summary>
		public const string ConsumerKeyUnknown = "consumer_key_unknown";
        /// <summary>
        /// nonce_used
        /// </summary>
		public const string NonceUsed = "nonce_used";
        /// <summary>
        /// parameter_absent
        /// </summary>
		public const string ParameterAbsent = "parameter_absent";
        /// <summary>
        /// parameter_rejected
        /// </summary>
		public const string ParameterRejected = "parameter_rejected";
        /// <summary>
        /// permission_denied
        /// </summary>
		public const string PermissionDenied = "permission_denied";
        /// <summary>
        /// permission_unknown
        /// </summary>
		public const string PermissionUnknown = "permission_unknown";
        /// <summary>
        /// signature_invalid
        /// </summary>
		public const string SignatureInvalid = "signature_invalid";
        /// <summary>
        /// signature_method_rejected
        /// </summary>
		public const string SignatureMethodRejected = "signature_method_rejected";
        /// <summary>
        /// timestamp_refused
        /// </summary>
		public const string TimestampRefused = "timestamp_refused";
        /// <summary>
        /// token_expired
        /// </summary>
		public const string TokenExpired = "token_expired";
        /// <summary>
        /// token_rejected
        /// </summary>
		public const string TokenRejected = "token_rejected";
        /// <summary>
        /// token_revoked
        /// </summary>
		public const string TokenRevoked = "token_revoked";
        /// <summary>
        /// token_used
        /// </summary>
		public const string TokenUsed = "token_used";
        /// <summary>
        /// user_refused
        /// </summary>
		public const string UserRefused = "user_refused";
        /// <summary>
        /// version_rejected
        /// </summary>
		public const string VersionRejected = "version_rejected";
	}
}
