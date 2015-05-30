using System;
using System.IO;

using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Asn1.X509;
using Nequeo.Cryptography.Key.Crypto;
using Nequeo.Cryptography.Key.Crypto.Parameters;
using Nequeo.Cryptography.Key.Security;
using Nequeo.Cryptography.Key.Utilities.Date;
using Nequeo.Cryptography.Key.Utilities.IO;

namespace Nequeo.Cryptography.Key.Cms
{
	public class CmsAuthenticatedGenerator
		: CmsEnvelopedGenerator
	{
		/**
		* base constructor
		*/
		public CmsAuthenticatedGenerator()
		{
		}

		/**
		* constructor allowing specific source of randomness
		*
		* @param rand instance of SecureRandom to use
		*/
		public CmsAuthenticatedGenerator(
			SecureRandom rand)
			: base(rand)
		{
		}
	}
}
