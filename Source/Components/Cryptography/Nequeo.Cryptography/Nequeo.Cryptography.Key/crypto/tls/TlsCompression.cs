using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsCompression
	{
		Stream Compress(Stream output);

		Stream Decompress(Stream output);
	}
}
