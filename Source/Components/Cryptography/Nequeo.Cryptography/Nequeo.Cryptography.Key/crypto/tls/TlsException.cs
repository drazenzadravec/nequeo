using System;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public class TlsException : Exception
	{
		public TlsException() : base() { }
		public TlsException(string message) : base(message) { }
		public TlsException(string message, Exception exception) : base(message, exception) { }
	}
}
