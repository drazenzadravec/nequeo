using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsSignerCredentials : TlsCredentials
	{
		/// <exception cref="IOException"></exception>
		byte[] GenerateCertificateSignature(byte[] md5andsha1);
	}
}
