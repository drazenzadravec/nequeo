using System;

using Nequeo.Cryptography.Key.Asn1.Nist;

namespace Nequeo.Cryptography.Key.Cms
{
	internal class CmsAuthEnvelopedGenerator
	{
		public static readonly string Aes128Ccm = NistObjectIdentifiers.IdAes128Ccm.Id;
		public static readonly string Aes192Ccm = NistObjectIdentifiers.IdAes192Ccm.Id;
		public static readonly string Aes256Ccm = NistObjectIdentifiers.IdAes256Ccm.Id;
		public static readonly string Aes128Gcm = NistObjectIdentifiers.IdAes128Gcm.Id;
		public static readonly string Aes192Gcm = NistObjectIdentifiers.IdAes192Gcm.Id;
		public static readonly string Aes256Gcm = NistObjectIdentifiers.IdAes256Gcm.Id;
	}
}
