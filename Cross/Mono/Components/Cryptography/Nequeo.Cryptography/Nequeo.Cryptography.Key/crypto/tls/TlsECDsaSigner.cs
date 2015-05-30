using System;

using Nequeo.Cryptography.Key.Crypto.Parameters;
using Nequeo.Cryptography.Key.Crypto.Signers;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	internal class TlsECDsaSigner
		: TlsDsaSigner
	{
		public override bool IsValidPublicKey(AsymmetricKeyParameter publicKey)
		{
			return publicKey is ECPublicKeyParameters;
		}

		protected override IDsa CreateDsaImpl()
		{
			return new ECDsaSigner();
		}
	}
}
