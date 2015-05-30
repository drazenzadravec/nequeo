using System;

namespace Nequeo.Cryptography.Key.Cms
{
	internal interface IDigestCalculator
	{
		byte[] GetDigest();
	}
}
