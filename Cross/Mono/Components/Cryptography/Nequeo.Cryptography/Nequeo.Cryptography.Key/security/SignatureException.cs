using System;

namespace Nequeo.Cryptography.Key.Security
{
	public class SignatureException : GeneralSecurityException
	{
		public SignatureException() : base() { }
		public SignatureException(string message) : base(message) { }
		public SignatureException(string message, Exception exception) : base(message, exception) { }
	}
}
