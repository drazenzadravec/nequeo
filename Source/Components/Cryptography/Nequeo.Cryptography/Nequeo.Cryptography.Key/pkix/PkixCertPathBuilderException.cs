using System;

using Nequeo.Cryptography.Key.Security;

namespace Nequeo.Cryptography.Key.Pkix
{
	/// <summary>
	/// Summary description for PkixCertPathBuilderException.
	/// </summary>
	public class PkixCertPathBuilderException : GeneralSecurityException
	{
		public PkixCertPathBuilderException() : base() { }
		
		public PkixCertPathBuilderException(string message) : base(message)	{ }  

		public PkixCertPathBuilderException(string message, Exception exception) : base(message, exception) { }
		
	}
}
