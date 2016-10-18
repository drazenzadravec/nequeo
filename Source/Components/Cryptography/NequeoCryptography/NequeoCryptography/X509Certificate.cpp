/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          X509Certificate.cpp
*  Purpose :       X509Certificate class.
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

#include "X509Certificate.h"
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

using Poco::Crypto::X509Certificate;
using Poco::Crypto::RSAKey;

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		X509Certificate::X509Certificate() : _disposed(false), _length(0), _rawCertificate(nullptr)
		{
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		X509Certificate::~X509Certificate()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;

				if (_rawCertificate != nullptr)
				{
					// Delete the resource.
					delete[] _rawCertificate;
					_rawCertificate = nullptr;
				}

			}
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		X509Certificate::X509Certificate(const X509Certificate& other) : 
			_disposed(false), _length(other._length), _rawCertificate(new unsigned char[other._length])
		{
			std::copy(other._rawCertificate, other._rawCertificate + _length, _rawCertificate);
		}

		/// <summary>
		/// Copy assignment operator.
		/// </summary>
		X509Certificate& X509Certificate::operator=(const X509Certificate& other)
		{
			// If the ojects are not the same object.
			if (this != &other)
			{
				// Free the existing resource.
				delete[] _rawCertificate;

				// Create and re-assign the resources.
				_length = other._length;
				_rawCertificate = new unsigned char[_length];
				std::copy(other._rawCertificate, other._rawCertificate + _length, _rawCertificate);
			}

			// Return the reference to this object.
			return *this;
		}

		/// <summary>
		/// Move constructor.
		/// </summary>
		X509Certificate::X509Certificate(X509Certificate&& other) :
			_disposed(false), _length(0), _rawCertificate(nullptr)
		{
			// Copy the data pointer and its length from the 
			// source object.
			_rawCertificate = other._rawCertificate;
			_length = other._length;

			// Release the data pointer from the source object so that
			// the destructor does not free the memory multiple times.
			other._rawCertificate = nullptr;
			other._length = 0;

			// Un-comment the code below for equivalent of above.
			//*this = std::move(other);
		}

		/// <summary>
		/// Move assignment operator.
		/// </summary>
		X509Certificate& X509Certificate::operator=(X509Certificate&& other)
		{
			// If the ojects are not the same object.
			if (this != &other)
			{
				// Free the existing resource.
				delete[] _rawCertificate;

				// Copy the data pointer and its length from the 
				// source object.
				_rawCertificate = other._rawCertificate;
				_length = other._length;

				// Release the data pointer from the source object so that
				// the destructor does not free the memory multiple times.
				other._rawCertificate = nullptr;
				other._length = 0;
			}

			// Return the reference to this object.
			return *this;
		}
	}
}