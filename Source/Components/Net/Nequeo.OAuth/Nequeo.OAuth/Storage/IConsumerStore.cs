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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Storage
{
    /// <summary>
    /// Consumer store interface.
    /// </summary>
    public interface IConsumerStore
    {
        /// <summary>
        /// Is consumer
        /// </summary>
        /// <param name="consumer">The counsumer to examine.</param>
        /// <returns>True if consumer ; else false.</returns>
        bool IsConsumer(IConsumer consumer);

        /// <summary>
        /// Set consumer secret
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        void SetConsumerSecret(IConsumer consumer, string consumerSecret);

        /// <summary>
        /// Get consumer secret
        /// </summary>
        /// <param name="consumer">The OAuth context.</param>
        /// <returns>The consumer secret.</returns>
        string GetConsumerSecret(IOAuthContext consumer);

        /// <summary>
        /// Set consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <param name="certificate">The certificate.</param>
        void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate);

        /// <summary>
        /// Get consumer public key
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <returns>The asymmetric algorithm</returns>
        AsymmetricAlgorithm GetConsumerPublicKey(IConsumer consumer);

        /// <summary>
        /// Set the verification code for the current user identifier
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <param name="userID">The unique user identifier.</param>
        /// <returns>The verification code.</returns>
        string SetVerificationCode(IOAuthContext consumer, string userID);
    }
}
