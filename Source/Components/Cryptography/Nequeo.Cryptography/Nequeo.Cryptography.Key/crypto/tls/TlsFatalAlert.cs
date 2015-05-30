using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Crypto.Tls
{
	public class TlsFatalAlert
		: IOException
	{
		private readonly AlertDescription alertDescription;

		public TlsFatalAlert(AlertDescription alertDescription)
		{
			this.alertDescription = alertDescription;
		}

		public AlertDescription AlertDescription
		{
			get { return alertDescription; }
		}
	}
}
