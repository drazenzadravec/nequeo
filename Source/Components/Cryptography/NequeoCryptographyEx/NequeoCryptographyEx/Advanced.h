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
			/// <param name="hashSize">The hash size.</param>
			/// <returns>The resulting hash code.</returns>
			PBYTE CreateHashCode(BYTE data[], unsigned long* hashSize);

			/// <summary>
			/// Create the signature for the data.
			/// </summary>
			/// <param name="data">The data to sign.</param>
			/// <param name="signatureSize">The signature size.</param>
			/// <returns>The resulting signature.</returns>
			PBYTE SignData(BYTE data[], unsigned long* signatureSize);

			/// <summary>
			/// Encrypt the data.
			/// </summary>
			/// <param name="data">The data to encrypt.</param>
			/// <param name="key">The key used to encrypt the data.</param>
			/// <param name="iv">The vector used in encryption.</param>
			/// <param name="encryptSize">The encrypted data size.</param>
			/// <returns>The resulting encryption.</returns>
			PBYTE Encrypt(BYTE data[], BYTE key[], BYTE iv[], unsigned long* encryptSize);

			/// <summary>
			/// Decrypt the data.
			/// </summary>
			/// <param name="data">The data to decrypt.</param>
			/// <param name="key">The key used to decrypt the data.</param>
			/// <param name="iv">The vector used in decryption.</param>
			/// <param name="decryptSize">The decrypted data size.</param>
			/// <returns>The resulting decryption.</returns>
			PBYTE Decrypt(BYTE data[], BYTE key[], BYTE iv[], unsigned long* decryptSize);

		private:
			PBYTE _hashCode;
			PBYTE _signedData;
			PBYTE _encryptData;
			PBYTE _decryptData;
		};
	}
}
#endif