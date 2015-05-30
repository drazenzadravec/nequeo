using System;

using Nequeo.Cryptography.Key.Security;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public interface TlsClientContext
	{
		SecureRandom SecureRandom { get; }

		SecurityParameters SecurityParameters { get; }

		object UserObject { get; set; }
	}
}
