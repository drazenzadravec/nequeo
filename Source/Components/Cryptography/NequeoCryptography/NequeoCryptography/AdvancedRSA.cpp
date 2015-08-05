/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AdvancedRSA.cpp
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

#include "stdafx.h"

#include "AdvancedRSA.h"
#include "Converter.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

#include "openssl\pem.h"
#include "openssl\evp.h"
#include "openssl\rsa.h"
#include "openssl\x509.h"
#if OPENSSL_VERSION_NUMBER >= 0x00908000L
#include <openssl/bn.h>
#endif

#include <Poco\Crypto\RSAKey.h>
#include <Poco\Crypto\X509Certificate.h>

using Nequeo::Cryptography::Converter;
using Poco::Crypto::RSAKey;

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		AdvancedRSA::AdvancedRSA() : _disposed(false)
		{
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		AdvancedRSA::~AdvancedRSA()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;
			}
		}

		/// <summary>
		/// Generate the public and private key RSA parameters.
		/// </summary>
		/// <param name="keyBitSize">The key bit size.</param>
		/// <param name="exponent">The RSA exponent size.</param>
		/// <returns>The RSA parameters.</returns>
		RsaParameters AdvancedRSA::GenerateKey(int keyBitSize, RsaExponent exponent)
		{
			RSA* rsa;
			unsigned long exp = (exponent == RsaExponent::RSA_Exp_3 ? RSA_3 : RSA_F4);

#if OPENSSL_VERSION_NUMBER >= 0x00908000L

			int ret = 0;
			BIGNUM* bn = 0;

			// Initialise.
			rsa = RSA_new();

			try
			{
				// Create a new big number.
				bn = BN_new();
				BN_set_word(bn, exp);

				// Create the RSA parameters.
				ret = RSA_generate_key_ex(rsa, keyBitSize, bn, 0);

				// Release the big number.
				BN_free(bn);
			}
			catch (...)
			{
				// Release the big number.
				BN_free(bn);
				throw;
			}

			// The RSA creation failed.
			if (!ret) throw Nequeo::Exceptions::InvalidArgumentException("Failed to create RSA context.");
#else
			// Create the RSA parameters.
			rsa = RSA_generate_key(keyBitSize, exp, 0, 0);

			// The RSA creation failed.
			if (!rsa) throw Nequeo::Exceptions::InvalidArgumentException("Failed to create RSA context.");
#endif

			// Create the RSA parameters
			RsaParameters rsaParm;
			rsaParm.setD(ConvertToByteArray(rsa->d));
			rsaParm.setDP(ConvertToByteArray(rsa->dmp1));
			rsaParm.setDQ(ConvertToByteArray(rsa->dmq1));
			rsaParm.setInverseQ(ConvertToByteArray(rsa->iqmp));
			rsaParm.setP(ConvertToByteArray(rsa->p));
			rsaParm.setQ(ConvertToByteArray(rsa->q));
			rsaParm.setModulus(ConvertToByteArray(rsa->n));
			rsaParm.setExponent(ConvertToByteArray(rsa->e));
			
			// Return the RSA parameters.
			return rsaParm;
		}

		/// <summary>
		/// Generate the public and private keys.
		/// </summary>
		/// <param name="publicKeyFile">The public key path and file name.</param>
		/// <param name="privateKeyFile">The private key path and file name.</param>
		/// <param name="privateKeyPassphrase">The private key pass phrase</param>
		/// <param name="keyBitSize">The key bit size.</param>
		/// <param name="exponent">The RSA exponent size.</param>
		void AdvancedRSA::GenerateKey(string& publicKeyFile, string& privateKeyFile, const std::string& privateKeyPassphrase, int keyBitSize, RsaExponent exponent)
		{
			RSAKey::KeyLength length;
			RSAKey::Exponent exp = (exponent == RsaExponent::RSA_Exp_3 ? RSAKey::EXP_SMALL : RSAKey::EXP_LARGE);

			// Select the key size.
			if (keyBitSize <= 512)
			{
				length = RSAKey::KeyLength::KL_512;
			}
			else if (keyBitSize <= 1024)
			{
				length = RSAKey::KeyLength::KL_1024;
			}
			else if (keyBitSize <= 2048)
			{
				length = RSAKey::KeyLength::KL_2048;
			}
			else
			{
				length = RSAKey::KeyLength::KL_4096;
			}

			// Create a new RSA key.
			RSAKey key(length, exp);
			key.save(publicKeyFile, privateKeyFile, privateKeyPassphrase);
		}

		/// <summary>
		/// Generate the public and private keys.
		/// </summary>
		/// <param name="publicKeyStream">The public key output stream.</param>
		/// <param name="privateKeyStream">The private key output stream.</param>
		/// <param name="privateKeyPassphrase">The private key pass phrase</param>
		/// <param name="keyBitSize">The key bit size.</param>
		/// <param name="exponent">The RSA exponent size.</param>
		void AdvancedRSA::GenerateKey(std::ostream* publicKeyStream, std::ostream* privateKeyStream, const std::string& privateKeyPassphrase, int keyBitSize, RsaExponent exponent)
		{
			RSAKey::KeyLength length;
			RSAKey::Exponent exp = (exponent == RsaExponent::RSA_Exp_3 ? RSAKey::EXP_SMALL : RSAKey::EXP_LARGE);

			// Select the key size.
			if (keyBitSize <= 512)
			{
				length = RSAKey::KeyLength::KL_512;
			}
			else if (keyBitSize <= 1024)
			{
				length = RSAKey::KeyLength::KL_1024;
			}
			else if (keyBitSize <= 2048)
			{
				length = RSAKey::KeyLength::KL_2048;
			}
			else
			{
				length = RSAKey::KeyLength::KL_4096;
			}

			// Create a new RSA key.
			RSAKey key(length, exp);
			key.save(publicKeyStream, privateKeyStream, privateKeyPassphrase);
		}

		/// <summary>
		/// Generate the certificate from the key RSA parameters.
		/// </summary>
		/// <param name="key">The key RSA parameters.</param>
		/// <param name="subject">The certificate subject.</param>
		/// <param name="issuer">The certificate issuer.</param>
		void AdvancedRSA::GenerateCertificate(RsaParameters& key, string subject, string issuer)
		{
			
		}

		/// <summary>
		/// Convert the big number to an array of bytes.
		/// </summary>
		/// <param name="bn">The big number.</param>
		/// <returns>The array of bytes.</returns>
		std::vector<unsigned char> AdvancedRSA::ConvertToByteArray(const BIGNUM* bn)
		{
			// Create the array.
			int numBytes = BN_num_bytes(bn);
			std::vector<unsigned char> byteVector(numBytes);

			// Create the buffer.
			std::vector<unsigned char>::value_type* buffer = new std::vector<unsigned char>::value_type[numBytes];
			BN_bn2bin(bn, buffer);

			// Copy the buffer to the array.
			for (int i = 0; i < numBytes; ++i)
				byteVector[i] = buffer[i];

			// Release the buffer.
			delete[] buffer;

			// Return the array.
			return byteVector;
		}
	}
}