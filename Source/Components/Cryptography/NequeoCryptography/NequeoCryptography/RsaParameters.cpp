/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RsaParameters.cpp
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

#include "stdafx.h"

#include "RsaParameters.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		RsaParameters::RsaParameters() : _disposed(false)
		{

		}

		/// <summary>
		/// This destructor.
		/// </summary>
		RsaParameters::~RsaParameters()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;

				_D.clear();
				_DP.clear();
				_DQ.clear();
				_InverseQ.clear();
				_P.clear();
				_Q.clear();
				_Modulus.clear();
				_Exponent.clear();
			}
		}

		/// <summary>
		/// Gets the D parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::D()
		{
			return _D;
		}

		/// <summary>
		/// Gets the DP parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::DP()
		{
			return _DP;
		}

		/// <summary>
		/// Gets the DQ parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::DQ()
		{
			return _DQ;
		}

		/// <summary>
		/// Gets the InverseQ parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::InverseQ()
		{
			return _InverseQ;
		}

		/// <summary>
		/// Gets the P parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::P()
		{
			return _P;
		}

		/// <summary>
		/// Gets the Q parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::Q()
		{
			return _Q;
		}

		/// <summary>
		/// Gets the Modulus parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::Modulus()
		{
			return _Modulus;
		}

		/// <summary>
		/// Gets the Exponent parameter.
		/// </summary>
		std::vector<unsigned char> RsaParameters::Exponent()
		{
			return _Exponent;
		}

		/// <summary>
		/// Sets the D parameter.
		/// </summary>
		/// <param name="key">The D parameter.</param>
		void RsaParameters::setD(std::vector<unsigned char> bytes)
		{
			_D = bytes;
		}

		/// <summary>
		/// Sets the DP parameter.
		/// </summary>
		/// <param name="key">The DP parameter.</param>
		void RsaParameters::setDP(std::vector<unsigned char> bytes)
		{
			_DP = bytes;
		}

		/// <summary>
		/// Sets the DQ parameter.
		/// </summary>
		/// <param name="key">The DQ parameter.</param>
		void RsaParameters::setDQ(std::vector<unsigned char> bytes)
		{
			_DQ = bytes;
		}

		/// <summary>
		/// Sets the InverseQ parameter.
		/// </summary>
		/// <param name="key">The InverseQ parameter.</param>
		void RsaParameters::setInverseQ(std::vector<unsigned char> bytes)
		{
			_InverseQ = bytes;
		}

		/// <summary>
		/// Sets the P parameter.
		/// </summary>
		/// <param name="key">The P parameter.</param>
		void RsaParameters::setP(std::vector<unsigned char> bytes)
		{
			_P = bytes;
		}

		/// <summary>
		/// Sets the Q parameter.
		/// </summary>
		/// <param name="key">The Q parameter.</param>
		void RsaParameters::setQ(std::vector<unsigned char> bytes)
		{
			_Q = bytes;
		}

		/// <summary>
		/// Sets the Modulus parameter.
		/// </summary>
		/// <param name="key">The Modulus parameter.</param>
		void RsaParameters::setModulus(std::vector<unsigned char> bytes)
		{
			_Modulus = bytes;
		}

		/// <summary>
		/// Sets the Exponent parameter.
		/// </summary>
		/// <param name="key">The Exponent parameter.</param>
		void RsaParameters::setExponent(std::vector<unsigned char> bytes)
		{
			_Exponent = bytes;
		}
	}
}