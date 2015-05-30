/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Hashcode.h
*  Purpose :       Hashcode class.
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

#ifndef _HASHCODE_H
#define _HASHCODE_H

#include "GlobalCryptography.h"

namespace Nequeo {
	namespace Cryptography
	{
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

		/// <summary>
		/// Hash algorithm generation class.
		/// </summary>
		class Hashcode
		{
		public:
			Hashcode();
			~Hashcode();

			/// <summary>
			/// Get the hashcode from the value.
			/// </summary>
			/// <param name="value">The value to generate the hash code for.</param>
			/// <param name="hashcodeType">The hash name.</param>
			/// <returns>The generated hash code.</returns>
			string GetHashcode(string value, HashcodeType hashcodeType);

			/// <summary>
			/// Get the hashcode from the value.
			/// </summary>
			/// <param name="filename">The path and file name to generate the hash code for.</param>
			/// <param name="hashcodeType">The hash name.</param>
			/// <returns>The generated hash code.</returns>
			string GetHashcodeFile(const char *filename, HashcodeType hashcodeType);

			/// <summary>
			/// Generate a random salt.
			/// </summary>
			/// <param name="minimum">The minimum length of the salt.</param>
			/// <param name="maximum">The minimum length of the salt.</param>
			/// <returns>The random salt value.</returns>
			string GenerateSalt(unsigned int minimum = 15, unsigned int maximum = 15);

		private:
			bool _disposed;

			string GetMd5(string value);
			string GetSha1(string value);
			string GetSha256(string value);
			string GetSha384(string value);
			string GetSha512(string value);

			string GetMd5File(const char *filename);
			string GetSha1File(const char *filename);
			string GetSha256File(const char *filename);
			string GetSha384File(const char *filename);
			string GetSha512File(const char *filename);
		};
	}
}
#endif