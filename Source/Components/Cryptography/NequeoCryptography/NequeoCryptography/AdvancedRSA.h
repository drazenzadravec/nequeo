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

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Advanced encryption decryption of data using RSA.
		/// </summary>
		class AdvancedRSA
		{
		public:
			AdvancedRSA();
			~AdvancedRSA();

			RsaParameters& GenerateKey(int keyBitSize = 4096, RsaExponent exponent = RsaExponent::RSA_Exp_3);
			void GenerateCertificate(RsaParameters& key, string subject, string issuer);

		private:
			bool _disposed;
			RsaParameters _rsaParm;
		};
	}
}
#endif