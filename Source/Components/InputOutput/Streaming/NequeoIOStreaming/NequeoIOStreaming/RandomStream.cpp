/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RandomStream.cpp
*  Purpose :       RandomStream class.
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

#include "RandomStream.h"
#include "Base\UnWindows.h"
#include <wincrypt.h>
#include <ctime>

namespace Nequeo {
	namespace IO
	{
		RandomBuf::RandomBuf() : BufferedStreamBuf(256, std::ios::in)
		{
		}

		RandomBuf::~RandomBuf()
		{
		}

		int RandomBuf::readFromDevice(char* buffer, std::streamsize length)
		{
			int n = 0;

			HCRYPTPROV hProvider = 0;
			CryptAcquireContext(&hProvider, 0, 0, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT);
			CryptGenRandom(hProvider, (DWORD)length, (BYTE*)buffer);
			CryptReleaseContext(hProvider, 0);
			n = static_cast<int>(length);

			return n;
		}


		RandomIOS::RandomIOS()
		{
			nequeo_ios_init(&_buf);
		}


		RandomIOS::~RandomIOS()
		{
		}


		RandomBuf* RandomIOS::rdbuf()
		{
			return &_buf;
		}


		RandomInputStream::RandomInputStream() : std::istream(&_buf)
		{
		}


		RandomInputStream::~RandomInputStream()
		{
		}
	}
}