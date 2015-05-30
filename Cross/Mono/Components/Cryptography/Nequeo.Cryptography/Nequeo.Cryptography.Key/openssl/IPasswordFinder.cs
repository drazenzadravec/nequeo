using System;

namespace Nequeo.Cryptography.Key.OpenSsl
{
	public interface IPasswordFinder
	{
		char[] GetPassword();
	}
}
