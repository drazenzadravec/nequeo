using System;

using Nequeo.Cryptography.Key.Crypto.Parameters;
using Nequeo.Cryptography.Key.Crypto.Signers;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	internal class TlsDssSigner
		: TlsDsaSigner
	{
		public override bool IsValidPublicKey(AsymmetricKeyParameter publicKey)
		{
			return publicKey is DsaPublicKeyParameters;
		}

	    protected override IDsa CreateDsaImpl()
	    {
			return new DsaSigner();
	    }
	}
}
