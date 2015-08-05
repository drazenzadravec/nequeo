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

using Nequeo::Cryptography::Converter;
using Poco::Crypto::X509Certificate;
using Poco::Crypto::RSAKey;

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Constructor for the current class.
		/// </summary>
		X509Certificate::X509Certificate() : _disposed(false)
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
			}
		}
	}
}