/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Advanced.h
*  Purpose :       Advanced class.
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

#ifndef _ADVANCED_H
#define _ADVANCED_H

#include "CryptoGlobal.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Advanced cryptography using cryptography next generation (CNG).
		/// </summary>
		class AdvancedCNG
		{
		public:
			/// <summary>
			///	Advanced cryptography using cryptography next generation (CNG).
			/// </summary>
			AdvancedCNG();

			/// <summary>
			/// This destructor.
			/// </summary>
			virtual ~AdvancedCNG();

			/// <summary>
			/// Create a hash code.
			/// </summary>
			/// <param name="data">The data to hash.</param>
			/// <returns>The resulting hash code.</returns>
			vector<BYTE> CreateHashCode(BYTE data[]);

			/// <summary>
			/// Create the signature for the data.
			/// </summary>
			/// <param name="data">The data to sign.</param>
			/// <returns>The resulting signature.</returns>
			vector<BYTE> SignData(BYTE data[]);

			/// <summary>
			/// Encrypt the data.
			/// </summary>
			/// <param name="data">The data to encrypt.</param>
			/// <param name="key">The key used to encrypt the data.</param>
			/// <param name="iv">The vector used in encryption.</param>
			/// <returns>The resulting encryption.</returns>
			vector<BYTE> Encrypt(BYTE data[], BYTE key[], BYTE iv[]);

			/// <summary>
			/// Decrypt the data.
			/// </summary>
			/// <param name="data">The data to decrypt.</param>
			/// <param name="key">The key used to decrypt the data.</param>
			/// <param name="iv">The vector used in decryption.</param>
			/// <returns>The resulting decryption.</returns>
			vector<BYTE> Decrypt(BYTE data[], BYTE key[], BYTE iv[]);

		};
	}
}
#endif