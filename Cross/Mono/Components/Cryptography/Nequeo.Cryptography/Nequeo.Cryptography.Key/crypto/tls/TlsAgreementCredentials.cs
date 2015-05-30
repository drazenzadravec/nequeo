using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsAgreementCredentials : TlsCredentials
	{
		/// <exception cref="IOException"></exception>
		byte[] GenerateAgreement(AsymmetricKeyParameter serverPublicKey);
	}
}
