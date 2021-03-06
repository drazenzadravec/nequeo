/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright � Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          X509Certificate.h
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

#pragma once

#ifndef _X509CERTIFICATE_H
#define _X509CERTIFICATE_H

#include "GlobalCryptography.h"
#include "RsaParameters.h"
#include "RsaExponent.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// X509 certificate type.
		/// </summary>
		class X509Certificate
		{
		public:
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			X509Certificate();

			/// <summary>
			/// This destructor.
			/// </summary>
			~X509Certificate();

			/// <summary>
			/// Copy constructor.
			/// </summary>
			X509Certificate(const X509Certificate& other);

			/// <summary>
			/// Copy assignment operator.
			/// </summary>
			X509Certificate& operator=(const X509Certificate& other);

			/// <summary>
			/// Move constructor.
			/// </summary>
			X509Certificate(X509Certificate&& other);

			/// <summary>
			/// Move assignment operator.
			/// </summary>
			X509Certificate& operator=(X509Certificate&& other);

		private:
			bool _disposed;

			// The length of the raw certificate.
			size_t _length;
			unsigned char* _rawCertificate;

			std::string _issuerName;
			std::string _subjectName;
		};
	}
}
#endif