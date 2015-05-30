using System;

using Nequeo.Cryptography.Key.Asn1.Cms;
using Nequeo.Cryptography.Key.Crypto.Parameters;
using Nequeo.Cryptography.Key.Security;

namespace Nequeo.Cryptography.Key.Cms
{
	interface RecipientInfoGenerator
	{
		/// <summary>
		/// Generate a RecipientInfo object for the given key.
		/// </summary>
		/// <param name="contentEncryptionKey">
		/// A <see cref="KeyParameter"/>
		/// </param>
		/// <param name="random">
		/// A <see cref="SecureRandom"/>
		/// </param>
		/// <returns>
		/// A <see cref="RecipientInfo"/>
		/// </returns>
		/// <exception cref="GeneralSecurityException"></exception>
		RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random);
	}
}
