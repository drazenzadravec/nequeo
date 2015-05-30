using System;

using Nequeo.Cryptography.Key.Asn1.X509;
using Nequeo.Cryptography.Key.Crypto.Parameters;

namespace Nequeo.Cryptography.Key.Cms
{
	internal interface CmsSecureReadable
	{
		AlgorithmIdentifier Algorithm { get; }
		object CryptoObject { get; }
		CmsReadable GetReadable(KeyParameter key);
	}
}
