using System;

using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Asn1.Cms;
using Nequeo.Cryptography.Key.Asn1.X509;

namespace Nequeo.Cryptography.Key.Cms
{
	internal interface SignerInfoGenerator
	{
		SignerInfo Generate(DerObjectIdentifier contentType, AlgorithmIdentifier digestAlgorithm,
        	byte[] calculatedDigest);
	}
}
