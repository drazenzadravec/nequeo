using System;

namespace Nequeo.Cryptography.Key.Utilities.IO.Pem
{
	public class PemGenerationException
		: Exception
	{
		public PemGenerationException()
			: base()
		{
		}

		public PemGenerationException(
			string message)
			: base(message)
		{
		}

		public PemGenerationException(
			string		message,
			Exception	exception)
			: base(message, exception)
		{
		}
	}
}
