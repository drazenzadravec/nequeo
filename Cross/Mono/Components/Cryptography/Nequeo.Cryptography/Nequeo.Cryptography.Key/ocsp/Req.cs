using System;
using System.Collections;
using System.IO;

using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Asn1.Ocsp;
using Nequeo.Cryptography.Key.Asn1.X509;
using Nequeo.Cryptography.Key.X509;

namespace Nequeo.Cryptography.Key.Ocsp
{
	public class Req
		: X509ExtensionBase
	{
		private Request req;

		public Req(
			Request req)
		{
			this.req = req;
		}

		public CertificateID GetCertID()
		{
			return new CertificateID(req.ReqCert);
		}

		public X509Extensions SingleRequestExtensions
		{
			get { return req.SingleRequestExtensions; }
		}

		protected override X509Extensions GetX509Extensions()
		{
			return SingleRequestExtensions;
		}
	}
}
