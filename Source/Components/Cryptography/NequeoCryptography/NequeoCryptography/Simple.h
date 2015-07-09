/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Simple.h
*  Purpose :       Simple class.
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

#ifndef _SIMPLE_H
#define _SIMPLE_H

#include "GlobalCryptography.h"

namespace Nequeo {
	namespace Cryptography
	{


		/// <summary>
		/// Hash algorithm generation class.
		/// </summary>
		class Hashcode sealed
		{
		public:
			Hashcode();
			~Hashcode();

		private:
			bool _disposed;
		};

		/// <summary>
		/// Hashcode type.
		/// </summary>
		enum HashcodeType
		{
			/// <summary>
			/// MD5.
			/// </summary>
			MD5 = 0,
			/// <summary>
			/// SHA1.
			/// </summary>
			SHA1 = 1,
			/// <summary>
			/// SHA256.
			/// </summary>
			SHA256 = 2,
			/// <summary>
			/// SHA384.
			/// </summary>
			SHA384 = 3,
			/// <summary>
			/// SHA512.
			/// </summary>
			SHA512 = 4
		};

		/// <summary>
		/// Cryptography type.
		/// </summary>
		enum CryptographyType
		{
			/// <summary>
			/// No cryptography.
			/// </summary>
			None = 0,
			/// <summary>
			/// Encrypt.
			/// </summary>
			Encrypt = 1,
			/// <summary>
			/// Decrypt.
			/// </summary>
			Decrypt = 2,
		};

		/// <summary>
		/// Describes the encryption format for storing passwords.
		/// </summary>
		enum PasswordFormat
		{
			/// <summary>
			/// Passwords are not encrypted.
			/// </summary>
			Clear = 0,
			/// <summary>
			/// Passwords are encrypted one-way using the SHA hashing algorithm.
			/// </summary>
			Hashed = 1,
			/// <summary>
			/// Passwords are encrypted using the encryption settings determined.
			/// </summary>
			Encrypted = 2,
		};
	}
}
#endif