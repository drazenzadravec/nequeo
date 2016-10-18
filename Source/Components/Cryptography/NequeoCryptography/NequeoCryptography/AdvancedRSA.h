/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AdvancedRSA.h
*  Purpose :       AdvancedRSA class.
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#pragma once

#ifndef _ADVANCEDRSA_H
#define _ADVANCEDRSA_H

#include "GlobalCryptography.h"
#include "RsaParameters.h"
#include "RsaExponent.h"
#include "CipherMode.h"
#include "PaddingMode.h"
#include "X509Certificate.h"

struct bignum_st;
typedef struct bignum_st BIGNUM;
typedef struct X509_name_st X509_NAME;

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Advanced encryption decryption of data using RSA.
		/// </summary>
		class AdvancedRSA
		{
		public:
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			AdvancedRSA();

			/// <summary>
			/// This destructor.
			/// </summary>
			~AdvancedRSA();

			/// <summary>
			/// Generate the public and private key RSA parameters.
			/// </summary>
			/// <param name="keyBitSize">The key bit size.</param>
			/// <param name="exponent">The RSA exponent size.</param>
			/// <returns>The RSA parameters.</returns>
			RsaParameters GenerateKey(int keyBitSize = 4096, RsaExponent exponent = RsaExponent::RSA_Exp_3);

			/// <summary>
			/// Generate the public and private keys.
			/// </summary>
			/// <param name="publicKeyFile">The public key path and file name.</param>
			/// <param name="privateKeyFile">The private key path and file name.</param>
			/// <param name="privateKeyPassphrase">The private key pass phrase</param>
			/// <param name="keyBitSize">The key bit size.</param>
			/// <param name="exponent">The RSA exponent size.</param>
			void GenerateKey(string& publicKeyFile, string& privateKeyFile, const std::string& privateKeyPassphrase = "", int keyBitSize = 4096, RsaExponent exponent = RsaExponent::RSA_Exp_3);

			/// <summary>
			/// Generate the public and private keys.
			/// </summary>
			/// <param name="publicKeyStream">The public key output stream.</param>
			/// <param name="privateKeyStream">The private key output stream.</param>
			/// <param name="privateKeyPassphrase">The private key pass phrase</param>
			/// <param name="keyBitSize">The key bit size.</param>
			/// <param name="exponent">The RSA exponent size.</param>
			void GenerateKey(std::ostream* publicKeyStream, std::ostream* privateKeyStream, const std::string& privateKeyPassphrase = "", int keyBitSize = 4096, RsaExponent exponent = RsaExponent::RSA_Exp_3);
			
			/// <summary>
			/// Generate the certificate from the parameters.
			/// </summary>
			/// <param name="subject">The certificate subject.</param>
			/// <param name="keyBitSize">The key bit size.</param>
			/// <param name="exponent">The RSA exponent size.</param>
			/// <returns>The X509 Certificate.</returns>
			X509Certificate& GenerateCertificate(RSACertificateIssuer& caIssuer, Subject& subject, long serialNumber, tm& notBefore, tm& notAfter, int keyBitSize = 4096, RsaExponent exponent = RsaExponent::RSA_Exp_3);

		private:
			bool _disposed;

			/// <summary>
			/// Convert the big number to an array of bytes.
			/// </summary>
			/// <param name="bn">The big number.</param>
			/// <returns>The array of bytes.</returns>
			std::vector<unsigned char> ConvertToByteArray(const BIGNUM* bn);

			/// <summary>
			/// Create certificate subject entry.
			/// </summary>
			/// <param name="subject">The subject.</param>
			/// <param name="entryKey">The entry name.</param>
			/// <param name="entryVal">The entry value.</param>
			void CreateCertificateEntry(X509_NAME* subject, char* entryKey, std::string entryVal);
		};
	}
}
#endif