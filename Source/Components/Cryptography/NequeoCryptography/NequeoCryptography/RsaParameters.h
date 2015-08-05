/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RsaParameters.h
*  Purpose :       RsaParameters class.
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

#ifndef _RSAPARAMETERS_H
#define _RSAPARAMETERS_H

#include "GlobalCryptography.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// RSA parameters.
		/// </summary>
		class RsaParameters
		{
		public:
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			RsaParameters();

			/// <summary>
			/// This destructor.
			/// </summary>
			~RsaParameters();

			/// <summary>
			/// Gets the D, the private exponent parameter.
			/// </summary>
			std::vector<unsigned char> D();

			/// <summary>
			/// Gets the DP, d mod (p - 1) parameter.
			/// </summary>
			std::vector<unsigned char> DP();

			/// <summary>
			/// Gets the DQ, d mod (q - 1) parameter.
			/// </summary>
			std::vector<unsigned char> DQ();

			/// <summary>
			/// Gets the InverseQ, (InverseQ)(q) = 1 mod p parameter.
			/// </summary>
			std::vector<unsigned char> InverseQ();

			/// <summary>
			/// Gets the P parameter.
			/// </summary>
			std::vector<unsigned char> P();

			/// <summary>
			/// Gets the Q parameter.
			/// </summary>
			std::vector<unsigned char> Q();

			/// <summary>
			/// Gets the Modulus, n parameter.
			/// </summary>
			std::vector<unsigned char> Modulus();

			/// <summary>
			/// Gets the Exponent, e the public exponent parameter.
			/// </summary>
			std::vector<unsigned char> Exponent();

			/// <summary>
			/// Sets the D, the private exponent parameter.
			/// </summary>
			/// <param name="key">The D parameter.</param>
			void setD(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the DP, d mod (p - 1) parameter.
			/// </summary>
			/// <param name="key">The DP parameter.</param>
			void setDP(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the DQ, d mod (q - 1) parameter.
			/// </summary>
			/// <param name="key">The DQ parameter.</param>
			void setDQ(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the InverseQ, (InverseQ)(q) = 1 mod p parameter.
			/// </summary>
			/// <param name="key">The InverseQ parameter.</param>
			void setInverseQ(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the P parameter.
			/// </summary>
			/// <param name="key">The P parameter.</param>
			void setP(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the Q parameter.
			/// </summary>
			/// <param name="key">The Q parameter.</param>
			void setQ(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the Modulus, n parameter.
			/// </summary>
			/// <param name="key">The Modulus parameter.</param>
			void setModulus(std::vector<unsigned char> bytes);

			/// <summary>
			/// Sets the Exponent, e the public exponent parameter.
			/// </summary>
			/// <param name="key">The Exponent parameter.</param>
			void setExponent(std::vector<unsigned char> bytes);

		private:
			bool _disposed;

			std::vector<unsigned char> _D;
			std::vector<unsigned char> _DP;
			std::vector<unsigned char> _DQ;
			std::vector<unsigned char> _InverseQ;
			std::vector<unsigned char> _P;
			std::vector<unsigned char> _Q;
			std::vector<unsigned char> _Modulus;
			std::vector<unsigned char> _Exponent;

		};
	}
}
#endif