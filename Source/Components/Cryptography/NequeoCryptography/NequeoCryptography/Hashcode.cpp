/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Hashcode.cpp
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

#include "stdafx.h"

#include "Hashcode.h"
#include "RandomGenerator.h"
#include "PasswordType.h"

#include "sha.h"
#include "md5.h"
#include "hex.h"
#include "modes.h"
#include "files.h"
#include "filters.h"
#include "cryptlib.h"

#include <boost\functional\hash.hpp>
#include <boost\functional\hash\hash.hpp>

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		Hashcode::Hashcode() : _disposed(false)
		{
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		Hashcode::~Hashcode()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;
			}
		}

		/// <summary>
		/// Generate a random salt.
		/// </summary>
		/// <param name="minimum">The minimum length of the salt.</param>
		/// <param name="maximum">The minimum length of the salt.</param>
		/// <returns>The random salt value.</returns>
		string Hashcode::GenerateSalt(unsigned int minimum, unsigned int maximum)
		{
			RandomGenerator salt;
			return salt.Create(minimum, maximum, PasswordType::UpperCaseLetters_LowerCaseLetters_Number_Special);
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="value">The value to generate the hash code for.</param>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetHashcode(string value, HashcodeType hashcodeType)
		{
			switch (hashcodeType)
			{
			case Nequeo::Cryptography::MD5:
				return GetMd5(value);
			case Nequeo::Cryptography::SHA1:
				return GetSha1(value);
			case Nequeo::Cryptography::SHA256:
				return GetSha256(value);
			case Nequeo::Cryptography::SHA384:
				return GetSha384(value);
			case Nequeo::Cryptography::SHA512:
				return GetSha512(value);
			default:
				return GetSha1(value);
			}
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="filename">The path and file name to generate the hash code for.</param>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetHashcodeFile(const char *filename, HashcodeType hashcodeType)
		{
			switch (hashcodeType)
			{
			case Nequeo::Cryptography::MD5:
				return GetMd5File(filename);
			case Nequeo::Cryptography::SHA1:
				return GetSha1File(filename);
			case Nequeo::Cryptography::SHA256:
				return GetSha256File(filename);
			case Nequeo::Cryptography::SHA384:
				return GetSha384File(filename);
			case Nequeo::Cryptography::SHA512:
				return GetSha512File(filename);
			default:
				return GetSha1File(filename);
			}
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetMd5(string value)
		{
			std::string hashedText;
			CryptoPP::MD5 md5Hash;
			byte digest[CryptoPP::MD5::DIGESTSIZE];
			CryptoPP::HexEncoder encoder;

			// calculate the hash.
			md5Hash.CalculateDigest(digest, (byte*)value.c_str(), value.length());
			encoder.Attach(new CryptoPP::StringSink(hashedText));
			encoder.Put(digest, sizeof(digest));
			encoder.MessageEnd();

			// return the hashed text.
			return hashedText;
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha1(string value)
		{
			std::string hashedText;
			CryptoPP::SHA1 sha1Hash;
			byte digest[CryptoPP::SHA1::DIGESTSIZE];
			CryptoPP::HexEncoder encoder;

			// calculate the hash.
			sha1Hash.CalculateDigest(digest, (byte*)value.c_str(), value.length());
			encoder.Attach(new CryptoPP::StringSink(hashedText));
			encoder.Put(digest, sizeof(digest));
			encoder.MessageEnd();

			// return the hashed text.
			return hashedText;
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha256(string value)
		{
			std::string hashedText;
			CryptoPP::SHA256 sha256Hash;
			byte digest[CryptoPP::SHA256::DIGESTSIZE];
			CryptoPP::HexEncoder encoder;

			// calculate the hash.
			sha256Hash.CalculateDigest(digest, (byte*)value.c_str(), value.length());
			encoder.Attach(new CryptoPP::StringSink(hashedText));
			encoder.Put(digest, sizeof(digest));
			encoder.MessageEnd();

			// return the hashed text.
			return hashedText;
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha384(string value)
		{
			std::string hashedText;
			CryptoPP::SHA384 sha384Hash;
			byte digest[CryptoPP::SHA384::DIGESTSIZE];
			CryptoPP::HexEncoder encoder;

			// calculate the hash.
			sha384Hash.CalculateDigest(digest, (byte*)value.c_str(), value.length());
			encoder.Attach(new CryptoPP::StringSink(hashedText));
			encoder.Put(digest, sizeof(digest));
			encoder.MessageEnd();

			// return the hashed text.
			return hashedText;
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="hashcodeType">The hash name.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha512(string value)
		{
			std::string hashedText;
			CryptoPP::SHA512 sha512Hash;
			byte digest[CryptoPP::SHA512::DIGESTSIZE];
			CryptoPP::HexEncoder encoder;

			// calculate the hash.
			sha512Hash.CalculateDigest(digest, (byte*)value.c_str(), value.length());
			encoder.Attach(new CryptoPP::StringSink(hashedText));
			encoder.Put(digest, sizeof(digest));
			encoder.MessageEnd();

			// return the hashed text.
			return hashedText;
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="filename">The path and file name to generate the hash code for.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetMd5File(const char *filename)
		{
			std::string hashedText;
			CryptoPP::MD5 md5Hash;

			// Output size of the buffer
			// Note that the output is encoded as hex which is why the 
			// output buffer must be two times the size of the MD5 digest
			byte buffer[2 * CryptoPP::MD5::DIGESTSIZE];

			CryptoPP::FileSource file(filename, true,
				new CryptoPP::HashFilter(md5Hash,
				new CryptoPP::HexEncoder(new CryptoPP::ArraySink(buffer, 2 * CryptoPP::MD5::DIGESTSIZE))));

			// return the hashed text.
			hashedText = string((const char*)buffer, 2 * CryptoPP::MD5::DIGESTSIZE);
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="filename">The path and file name to generate the hash code for.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha1File(const char *filename)
		{
			std::string hashedText;
			CryptoPP::SHA1 sha1Hash;

			// Output size of the buffer
			// Note that the output is encoded as hex which is why the 
			// output buffer must be two times the size of the MD5 digest
			byte buffer[2 * CryptoPP::SHA1::DIGESTSIZE];

			CryptoPP::FileSource file(filename, true,
				new CryptoPP::HashFilter(sha1Hash,
				new CryptoPP::HexEncoder(new CryptoPP::ArraySink(buffer, 2 * CryptoPP::SHA1::DIGESTSIZE))));

			// return the hashed text.
			hashedText = string((const char*)buffer, 2 * CryptoPP::SHA1::DIGESTSIZE);
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="filename">The path and file name to generate the hash code for.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha256File(const char *filename)
		{
			std::string hashedText;
			CryptoPP::SHA256 sha256Hash;

			// Output size of the buffer
			// Note that the output is encoded as hex which is why the 
			// output buffer must be two times the size of the MD5 digest
			byte buffer[2 * CryptoPP::SHA256::DIGESTSIZE];

			CryptoPP::FileSource file(filename, true,
				new CryptoPP::HashFilter(sha256Hash,
				new CryptoPP::HexEncoder(new CryptoPP::ArraySink(buffer, 2 * CryptoPP::SHA256::DIGESTSIZE))));

			// return the hashed text.
			hashedText = string((const char*)buffer, 2 * CryptoPP::SHA256::DIGESTSIZE);
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="filename">The path and file name to generate the hash code for.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha384File(const char *filename)
		{
			std::string hashedText;
			CryptoPP::SHA384 sha384Hash;

			// Output size of the buffer
			// Note that the output is encoded as hex which is why the 
			// output buffer must be two times the size of the MD5 digest
			byte buffer[2 * CryptoPP::SHA384::DIGESTSIZE];

			CryptoPP::FileSource file(filename, true,
				new CryptoPP::HashFilter(sha384Hash,
				new CryptoPP::HexEncoder(new CryptoPP::ArraySink(buffer, 2 * CryptoPP::SHA384::DIGESTSIZE))));

			// return the hashed text.
			hashedText = string((const char*)buffer, 2 * CryptoPP::SHA384::DIGESTSIZE);
		}

		/// <summary>
		/// Get the hashcode from the value.
		/// </summary>
		/// <param name="filename">The path and file name to generate the hash code for.</param>
		/// <returns>The generated hash code.</returns>
		string Hashcode::GetSha512File(const char *filename)
		{
			std::string hashedText;
			CryptoPP::SHA512 sha512Hash;

			// Output size of the buffer
			// Note that the output is encoded as hex which is why the 
			// output buffer must be two times the size of the MD5 digest
			byte buffer[2 * CryptoPP::SHA512::DIGESTSIZE];

			CryptoPP::FileSource file(filename, true,
				new CryptoPP::HashFilter(sha512Hash,
				new CryptoPP::HexEncoder(new CryptoPP::ArraySink(buffer, 2 * CryptoPP::SHA512::DIGESTSIZE))));

			// return the hashed text.
			hashedText = string((const char*)buffer, 2 * CryptoPP::SHA512::DIGESTSIZE);
		}
	}
}