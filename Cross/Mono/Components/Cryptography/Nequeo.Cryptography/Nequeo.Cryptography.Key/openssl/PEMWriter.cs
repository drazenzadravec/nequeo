using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Asn1.CryptoPro;
using Nequeo.Cryptography.Key.Asn1.Pkcs;
using Nequeo.Cryptography.Key.Asn1.X509;
using Nequeo.Cryptography.Key.Asn1.X9;
using Nequeo.Cryptography.Key.Crypto;
using Nequeo.Cryptography.Key.Crypto.Generators;
using Nequeo.Cryptography.Key.Crypto.Parameters;
using Nequeo.Cryptography.Key.Math;
using Nequeo.Cryptography.Key.Pkcs;
using Nequeo.Cryptography.Key.Security;
using Nequeo.Cryptography.Key.Security.Certificates;
using Nequeo.Cryptography.Key.Utilities.Encoders;
using Nequeo.Cryptography.Key.Utilities.IO.Pem;
using Nequeo.Cryptography.Key.X509;

namespace Nequeo.Cryptography.Key.OpenSsl
{
	/// <remarks>General purpose writer for OpenSSL PEM objects.</remarks>
	public class PemWriter
		: Nequeo.Cryptography.Key.Utilities.IO.Pem.PemWriter
	{
		/// <param name="writer">The TextWriter object to write the output to.</param>
		public PemWriter(
			TextWriter writer)
			: base(writer)
		{
		}

		public void WriteObject(
			object obj) 
		{
			try
			{
				base.WriteObject(new MiscPemGenerator(obj));
			}
			catch (PemGenerationException e)
			{
				if (e.InnerException is IOException)
					throw (IOException)e.InnerException;

				throw e;
			}
		}

		public void WriteObject(
			object			obj,
			string			algorithm,
			char[]			password,
			SecureRandom	random)
		{
			base.WriteObject(new MiscPemGenerator(obj, algorithm, password, random));
		}
	}
}
