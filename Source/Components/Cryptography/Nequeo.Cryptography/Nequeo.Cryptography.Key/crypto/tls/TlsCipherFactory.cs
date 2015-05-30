using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsCipherFactory
	{
		/// <exception cref="IOException"></exception>
		TlsCipher CreateCipher(TlsClientContext context, EncryptionAlgorithm encryptionAlgorithm,
			DigestAlgorithm digestAlgorithm);
	}
}
