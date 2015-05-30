using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsCipher
	{
		/// <exception cref="IOException"></exception>
		byte[] EncodePlaintext(ContentType type, byte[] plaintext, int offset, int len);

		/// <exception cref="IOException"></exception>
		byte[] DecodeCiphertext(ContentType type, byte[] ciphertext, int offset, int len);
	}
}
