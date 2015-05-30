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

using Nequeo.Cryptography.Signing;
using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Provider.Inspectors
{
    /// <summary>
    /// Signature Validation Inspector
    /// </summary>
    public class SignatureValidationInspector : IContextInspector
    {
        readonly IConsumerStore _consumerStore;
        readonly IOAuthContextSigner _signer;

        /// <summary>
        /// Signature Validation Inspector
        /// </summary>
        /// <param name="consumerStore">The consumer store.</param>
        public SignatureValidationInspector(IConsumerStore consumerStore)
            : this(consumerStore, new OAuthContextSigner())
        {
        }

        /// <summary>
        /// SignatureValidationInspector
        /// </summary>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="signer">The OAuth context signer</param>
        public SignatureValidationInspector(IConsumerStore consumerStore, IOAuthContextSigner signer)
        {
            _consumerStore = consumerStore;
            _signer = signer;
        }

        /// <summary>
        /// Inspect the current context.
        /// </summary>
        /// <param name="phase">The current provider phase.</param>
        /// <param name="context">OAuth context</param>
        public virtual void InspectContext(ProviderPhase phase, IOAuthContext context)
        {
            SigningContext signingContext = CreateSignatureContextForConsumer(context);

            if (!_signer.ValidateSignature(context, signingContext))
            {
                throw Error.FailedToValidateSignature(context);
            }
        }

        /// <summary>
        /// Signature method requires certificate
        /// </summary>
        /// <param name="signatureMethod">The signature method.</param>
        /// <returns>True is signature method requires certificate; else false.</returns>
        protected virtual bool SignatureMethodRequiresCertificate(string signatureMethod)
        {
            return ((signatureMethod != SignatureMethod.HmacSha1) && (signatureMethod != SignatureMethod.PlainText));
        }

        /// <summary>
        /// Create signature context for consumer
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The signing context.</returns>
        protected virtual SigningContext CreateSignatureContextForConsumer(IOAuthContext context)
        {
            var signingContext = new SigningContext { ConsumerSecret = _consumerStore.GetConsumerSecret(context) };

            if (SignatureMethodRequiresCertificate(context.SignatureMethod))
            {
                signingContext.AsymmetricAlgorithm = _consumerStore.GetConsumerPublicKey(context);
            }

            return signingContext;
        }
    }
}
