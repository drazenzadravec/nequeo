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

#include "openssl\pem.h"
#include "openssl\evp.h"
#include "openssl\rsa.h"
#include "openssl\x509.h"

using Nequeo::Cryptography::Converter;

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

		RsaParameters& AdvancedRSA::GenerateKey(int keyBitSize, RsaExponent exponent)
		{
			RSA* rsa = RSA_generate_key(
				2048,   /* number of bits for the key - 2048 is a sensible value */
				RSA_3, /* exponent - RSA_3 is defined as 0x3L */
				NULL,   /* callback - can be NULL if we aren't displaying progress */
				NULL    /* callback argument - not needed in this case */
				);

			return _rsaParm;
		}

		void AdvancedRSA::GenerateCertificate(RsaParameters& key, string subject, string issuer)
		{
			
		}
	}
}