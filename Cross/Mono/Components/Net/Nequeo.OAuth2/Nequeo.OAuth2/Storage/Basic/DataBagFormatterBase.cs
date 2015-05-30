﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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

namespace Nequeo.Net.OAuth2.Storage.Basic
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using System.Web;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging.Reflection;

	/// <summary>
	/// A serializer for <see cref="DataBag"/>-derived types
	/// </summary>
	/// <typeparam name="T">The DataBag-derived type that is to be serialized/deserialized.</typeparam>
	internal abstract class DataBagFormatterBase<T> : IDataBagFormatter<T> where T : DataBag {
		/// <summary>
		/// The message description cache to use for data bag types.
		/// </summary>
		protected static readonly MessageDescriptionCollection MessageDescriptions = new MessageDescriptionCollection();

		/// <summary>
		/// The length of the nonce to include in tokens that can be decoded once only.
		/// </summary>
		private const int NonceLength = 40;

		/// <summary>
		/// The minimum allowable lifetime for the key used to encrypt/decrypt or sign this databag.
		/// </summary>
		private readonly TimeSpan minimumAge = TimeSpan.FromDays(1);

		/// <summary>
		/// The symmetric key store with the secret used for signing/encryption of verification codes and refresh tokens.
		/// </summary>
		private readonly ICryptoKeyStore cryptoKeyStore;

		/// <summary>
		/// The bucket for symmetric keys.
		/// </summary>
		private readonly string cryptoKeyBucket;

		/// <summary>
		/// The crypto to use for signing access tokens.
		/// </summary>
		private readonly RSACryptoServiceProvider asymmetricSigning;

		/// <summary>
		/// The crypto to use for encrypting access tokens.
		/// </summary>
		private readonly RSACryptoServiceProvider asymmetricEncrypting;

		/// <summary>
		/// A value indicating whether the data in this instance will be protected against tampering.
		/// </summary>
		private readonly bool signed;

		/// <summary>
		/// The nonce store to use to ensure that this instance is only decoded once.
		/// </summary>
		private readonly INonceStore decodeOnceOnly;

		/// <summary>
		/// The maximum age of a token that can be decoded; useful only when <see cref="decodeOnceOnly"/> is <c>true</c>.
		/// </summary>
		private readonly TimeSpan? maximumAge;

		/// <summary>
		/// A value indicating whether the data in this instance will be protected against eavesdropping.
		/// </summary>
		private readonly bool encrypted;

		/// <summary>
		/// A value indicating whether the data in this instance will be GZip'd.
		/// </summary>
		private readonly bool compressed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBagFormatterBase&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="signingKey">The crypto service provider with the asymmetric key to use for signing or verifying the token.</param>
		/// <param name="encryptingKey">The crypto service provider with the asymmetric key to use for encrypting or decrypting the token.</param>
		/// <param name="compressed">A value indicating whether the data in this instance will be GZip'd.</param>
		/// <param name="maximumAge">The maximum age of a token that can be decoded; useful only when <paramref name="decodeOnceOnly"/> is <c>true</c>.</param>
		/// <param name="decodeOnceOnly">The nonce store to use to ensure that this instance is only decoded once.</param>
		protected DataBagFormatterBase(RSACryptoServiceProvider signingKey = null, RSACryptoServiceProvider encryptingKey = null, bool compressed = false, TimeSpan? maximumAge = null, INonceStore decodeOnceOnly = null)
			: this(signingKey != null, encryptingKey != null, compressed, maximumAge, decodeOnceOnly) {
			this.asymmetricSigning = signingKey;
			this.asymmetricEncrypting = encryptingKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBagFormatterBase&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="cryptoKeyStore">The crypto key store used when signing or encrypting.</param>
		/// <param name="bucket">The bucket in which symmetric keys are stored for signing/encrypting data.</param>
		/// <param name="signed">A value indicating whether the data in this instance will be protected against tampering.</param>
		/// <param name="encrypted">A value indicating whether the data in this instance will be protected against eavesdropping.</param>
		/// <param name="compressed">A value indicating whether the data in this instance will be GZip'd.</param>
		/// <param name="minimumAge">The required minimum lifespan within which this token must be decodable and verifiable; useful only when <paramref name="signed"/> and/or <paramref name="encrypted"/> is true.</param>
		/// <param name="maximumAge">The maximum age of a token that can be decoded; useful only when <paramref name="decodeOnceOnly"/> is <c>true</c>.</param>
		/// <param name="decodeOnceOnly">The nonce store to use to ensure that this instance is only decoded once.</param>
		protected DataBagFormatterBase(ICryptoKeyStore cryptoKeyStore = null, string bucket = null, bool signed = false, bool encrypted = false, bool compressed = false, TimeSpan? minimumAge = null, TimeSpan? maximumAge = null, INonceStore decodeOnceOnly = null)
			: this(signed, encrypted, compressed, maximumAge, decodeOnceOnly) {

			this.cryptoKeyStore = cryptoKeyStore;
			this.cryptoKeyBucket = bucket;
			if (minimumAge.HasValue) {
				this.minimumAge = minimumAge.Value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBagFormatterBase&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="signed">A value indicating whether the data in this instance will be protected against tampering.</param>
		/// <param name="encrypted">A value indicating whether the data in this instance will be protected against eavesdropping.</param>
		/// <param name="compressed">A value indicating whether the data in this instance will be GZip'd.</param>
		/// <param name="maximumAge">The maximum age of a token that can be decoded; useful only when <paramref name="decodeOnceOnly"/> is <c>true</c>.</param>
		/// <param name="decodeOnceOnly">The nonce store to use to ensure that this instance is only decoded once.</param>
		private DataBagFormatterBase(bool signed = false, bool encrypted = false, bool compressed = false, TimeSpan? maximumAge = null, INonceStore decodeOnceOnly = null) {

			this.signed = signed;
			this.maximumAge = maximumAge;
			this.decodeOnceOnly = decodeOnceOnly;
			this.encrypted = encrypted;
			this.compressed = compressed;
		}

		/// <summary>
		/// Serializes the specified message, including compression, encryption, signing, and nonce handling where applicable.
		/// </summary>
		/// <param name="message">The message to serialize.  Must not be null.</param>
		/// <returns>A non-null, non-empty value.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "No apparent problem.  False positive?")]
		public string Serialize(T message) {

			message.UtcCreationDate = DateTime.UtcNow;

			if (this.decodeOnceOnly != null) {
				message.Nonce = MessagingUtilities.GetNonCryptoRandomData(NonceLength);
			}

			byte[] encoded = this.SerializeCore(message);

			if (this.compressed) {
				encoded = MessagingUtilities.Compress(encoded);
			}

			string symmetricSecretHandle = null;
			if (this.encrypted) {
				encoded = this.Encrypt(encoded, out symmetricSecretHandle);
			}

			if (this.signed) {
				message.Signature = this.CalculateSignature(encoded, symmetricSecretHandle);
			}

			int capacity = this.signed ? 4 + message.Signature.Length + 4 + encoded.Length : encoded.Length;
			using (var finalStream = new MemoryStream(capacity)) {
				var writer = new BinaryWriter(finalStream);
				if (this.signed) {
					writer.WriteBuffer(message.Signature);
				}

				writer.WriteBuffer(encoded);
				writer.Flush();

				string payload = MessagingUtilities.ConvertToBase64WebSafeString(finalStream.ToArray());
				string result = payload;
				if (symmetricSecretHandle != null && (this.signed || this.encrypted)) {
					result = MessagingUtilities.CombineKeyHandleAndPayload(symmetricSecretHandle, payload);
				}

				return result;
			}
		}

		/// <summary>
		/// Deserializes a <see cref="DataBag"/>, including decompression, decryption, signature and nonce validation where applicable.
		/// </summary>
		/// <param name="message">The instance to initialize with deserialized data.</param>
		/// <param name="containingMessage">The message that contains the <see cref="DataBag"/> serialized value.  Must not be null.</param>
		/// <param name="value">The serialized form of the <see cref="DataBag"/> to deserialize.  Must not be null or empty.</param>
		/// <param name="messagePartName">The name of the parameter whose value is to be deserialized.  Used for error message generation.</param>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "No apparent problem.  False positive?")]
		public void Deserialize(T message, IProtocolMessage containingMessage, string value, string messagePartName) {

			string symmetricSecretHandle = null;
			if (this.encrypted && this.cryptoKeyStore != null) {
				string valueWithoutHandle;
				MessagingUtilities.ExtractKeyHandleAndPayload(containingMessage, messagePartName, value, out symmetricSecretHandle, out valueWithoutHandle);
				value = valueWithoutHandle;
			}

			message.ContainingMessage = containingMessage;
			byte[] data = MessagingUtilities.FromBase64WebSafeString(value);

			byte[] signature = null;
			if (this.signed) {
				using (var dataStream = new MemoryStream(data)) {
					var dataReader = new BinaryReader(dataStream);
					signature = dataReader.ReadBuffer();
					data = dataReader.ReadBuffer();
				}

				// Verify that the verification code was issued by message authorization server.
				ErrorUtilities.VerifyProtocol(this.IsSignatureValid(data, signature, symmetricSecretHandle), MessagingStrings.SignatureInvalid);
			}

			if (this.encrypted) {
				data = this.Decrypt(data, symmetricSecretHandle);
			}

			if (this.compressed) {
				data = MessagingUtilities.Decompress(data);
			}

			this.DeserializeCore(message, data);
			message.Signature = signature; // TODO: we don't really need this any more, do we?

			if (this.maximumAge.HasValue) {
				// Has message verification code expired?
				DateTime expirationDate = message.UtcCreationDate + this.maximumAge.Value;
				if (expirationDate < DateTime.UtcNow) {
					throw new ExpiredMessageException(expirationDate, containingMessage);
				}
			}

			// Has message verification code already been used to obtain an access/refresh token?
			if (this.decodeOnceOnly != null) {
				ErrorUtilities.VerifyInternal(this.maximumAge.HasValue, "Oops!  How can we validate a nonce without a maximum message age?");
				string context = "{" + GetType().FullName + "}";

                if (message is Nequeo.Net.OAuth2.Framework.ChannelElements.AuthorizationDataBag)
                    context = message.GetType().GetProperty("ClientIdentifier").GetValue(message, null).ToString();

				if (!this.decodeOnceOnly.StoreNonce(message.UtcCreationDate, Convert.ToBase64String(message.Nonce))) {
					throw new ReplayedMessageException(containingMessage);
				}
			}

			((IMessage)message).EnsureValidMessage();
		}

		/// <summary>
		/// Serializes the <see cref="DataBag"/> instance to a buffer.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>The buffer containing the serialized data.</returns>
		protected abstract byte[] SerializeCore(T message);

		/// <summary>
		/// Deserializes the <see cref="DataBag"/> instance from a buffer.
		/// </summary>
		/// <param name="message">The message instance to initialize with data from the buffer.</param>
		/// <param name="data">The data buffer.</param>
		protected abstract void DeserializeCore(T message, byte[] data);

		/// <summary>
		/// Determines whether the signature on this instance is valid.
		/// </summary>
		/// <param name="signedData">The signed data.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="symmetricSecretHandle">The symmetric secret handle.  <c>null</c> when using an asymmetric algorithm.</param>
		/// <returns>
		///   <c>true</c> if the signature is valid; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "No apparent problem.  False positive?")]
		private bool IsSignatureValid(byte[] signedData, byte[] signature, string symmetricSecretHandle) {

			if (this.asymmetricSigning != null) {
				using (var hasher = new SHA1CryptoServiceProvider()) {
					return this.asymmetricSigning.VerifyData(signedData, hasher, signature);
				}
			} else {
				return MessagingUtilities.AreEquivalentConstantTime(signature, this.CalculateSignature(signedData, symmetricSecretHandle));
			}
		}

		/// <summary>
		/// Calculates the signature for the data in this verification code.
		/// </summary>
		/// <param name="bytesToSign">The bytes to sign.</param>
		/// <param name="symmetricSecretHandle">The symmetric secret handle.  <c>null</c> when using an asymmetric algorithm.</param>
		/// <returns>
		/// The calculated signature.
		/// </returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "No apparent problem.  False positive?")]
		private byte[] CalculateSignature(byte[] bytesToSign, string symmetricSecretHandle) {
		
			Contract.Ensures(Contract.Result<byte[]>() != null);

			if (this.asymmetricSigning != null) {
				using (var hasher = new SHA1CryptoServiceProvider()) {
					return this.asymmetricSigning.SignData(bytesToSign, hasher);
				}
			} else {
				var key = this.cryptoKeyStore.GetKey(this.cryptoKeyBucket, symmetricSecretHandle);
				ErrorUtilities.VerifyProtocol(key != null, MessagingStrings.MissingDecryptionKeyForHandle, this.cryptoKeyBucket, symmetricSecretHandle);
				using (var symmetricHasher = new HMACSHA256(key.Key)) {
					return symmetricHasher.ComputeHash(bytesToSign);
				}
			}
		}

		/// <summary>
		/// Encrypts the specified value using either the symmetric or asymmetric encryption algorithm as appropriate.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="symmetricSecretHandle">Receives the symmetric secret handle.  <c>null</c> when using an asymmetric algorithm.</param>
		/// <returns>
		/// The encrypted value.
		/// </returns>
		private byte[] Encrypt(byte[] value, out string symmetricSecretHandle) {

			if (this.asymmetricEncrypting != null) {
				symmetricSecretHandle = null;
				return this.asymmetricEncrypting.EncryptWithRandomSymmetricKey(value);
			} else {
				var cryptoKey = this.cryptoKeyStore.GetCurrentKey(this.cryptoKeyBucket, this.minimumAge);
				symmetricSecretHandle = cryptoKey.Key;
				return MessagingUtilities.Encrypt(value, cryptoKey.Value.Key);
			}
		}

		/// <summary>
		/// Decrypts the specified value using either the symmetric or asymmetric encryption algorithm as appropriate.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="symmetricSecretHandle">The symmetric secret handle.  <c>null</c> when using an asymmetric algorithm.</param>
		/// <returns>
		/// The decrypted value.
		/// </returns>
		private byte[] Decrypt(byte[] value, string symmetricSecretHandle) {

			if (this.asymmetricEncrypting != null) {
				return this.asymmetricEncrypting.DecryptWithRandomSymmetricKey(value);
			} else {
				var key = this.cryptoKeyStore.GetKey(this.cryptoKeyBucket, symmetricSecretHandle);
				ErrorUtilities.VerifyProtocol(key != null, MessagingStrings.MissingDecryptionKeyForHandle, this.cryptoKeyBucket, symmetricSecretHandle);
				return MessagingUtilities.Decrypt(value, key.Key);
			}
		}
	}
}
