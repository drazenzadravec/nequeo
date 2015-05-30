using System;
using Nequeo.Cryptography.Key.Crypto;

namespace Nequeo.Cryptography.Key.Crypto.Parameters
{
	/**
	* parameters for Key derivation functions for ISO-18033
	*/
	public class Iso18033KdfParameters
		: IDerivationParameters
	{
		byte[]  seed;

		public Iso18033KdfParameters(
			byte[]  seed)
		{
			this.seed = seed;
		}

		public byte[] GetSeed()
		{
			return seed;
		}
	}
}
