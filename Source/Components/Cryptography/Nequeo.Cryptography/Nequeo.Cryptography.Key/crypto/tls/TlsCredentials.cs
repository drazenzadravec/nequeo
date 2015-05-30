using System;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsCredentials
	{
		Certificate Certificate { get; }
	}
}
