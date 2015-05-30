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
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Consumer.Session
{
    /// <summary>
    /// OAuth Consumer Context
    /// </summary>
    [Serializable]
    public class OAuthConsumerContext : IOAuthConsumerContext
    {
        private INonceGenerator _nonceGenerator = new GuidNonceGenerator();
        private IOAuthContextSigner _signer = new OAuthContextSigner();

        /// <summary>
        /// OAuth Consumer Context
        /// </summary>
        public OAuthConsumerContext()
        {
            SignatureMethod = Nequeo.Cryptography.Signing.SignatureMethod.PlainText;
        }

        /// <summary>
        /// Gets sets the OAuthContextSigner
        /// </summary>
        public IOAuthContextSigner Signer
        {
            get { return _signer; }
            set { _signer = value; }
        }

        /// <summary>
        /// Gets sets the
        /// </summary>
        public INonceGenerator NonceGenerator
        {
            get { return _nonceGenerator; }
            set { _nonceGenerator = value; }
        }

        /// <summary>
        /// Gets sets the realm
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Gets sets the consumer key
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets sets the cunsumer secret
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets sets the signature method
        /// </summary>
        public string SignatureMethod { get; set; }

        /// <summary>
        /// Gets sets the asymmetric algorithm key
        /// </summary>
        public AsymmetricAlgorithm Key { get; set; }

        /// <summary>
        /// Gets sets the use header for oauth parameters
        /// </summary>
        public bool UseHeaderForOAuthParameters { get; set; }

        /// <summary>
        /// Gets sets the user agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Sign Context
        /// </summary>
        /// <param name="context">The OAuth context.</param>
        public void SignContext(IOAuthContext context)
        {
            EnsureStateIsValid();

            context.UseAuthorizationHeader = UseHeaderForOAuthParameters;
            context.Nonce = _nonceGenerator.GenerateNonce(context);
            context.ConsumerKey = ConsumerKey;
            context.Realm = Realm;
            context.SignatureMethod = SignatureMethod;
            context.Timestamp = Clock.EpochString;
            context.Version = "1.0";

            context.Nonce = NonceGenerator.GenerateNonce(context);

            string signatureBase = context.GenerateSignatureBase();

            _signer.SignContext(context,
                                new SigningContext { AsymmetricAlgorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret });
        }

        /// <summary>
        /// Sign Context With Token
        /// </summary>
        /// <param name="context">>The OAuth context.</param>
        /// <param name="token">The token</param>
        public void SignContextWithToken(IOAuthContext context, IToken token)
        {
            context.Token = token.Token;
            context.TokenSecret = token.TokenSecret;

            SignContext(context);
        }

        /// <summary>
        /// Ensure State Is Valid
        /// </summary>
        void EnsureStateIsValid()
        {
            if (string.IsNullOrEmpty(ConsumerKey)) throw Error.EmptyConsumerKey();
            if (string.IsNullOrEmpty(SignatureMethod)) throw Error.UnknownSignatureMethod(SignatureMethod);
            if ((SignatureMethod == Nequeo.Cryptography.Signing.SignatureMethod.RsaSha1)
                && (Key == null)) throw Error.ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter();
        }
    }
}
