/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RandomStream.h
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

#pragma once

#ifndef _RANDOMSTREAM_H
#define _RANDOMSTREAM_H

#include "GlobalStreaming.h"
#include "BufferedStreamBuf.h"
#include <istream>

namespace Nequeo {
	namespace IO
	{
		class RandomBuf : public BufferedStreamBuf
			/// This streambuf generates random data.
			/// On Windows NT, the cryptographic API is used.
			/// On Unix, /dev/random is used, if available.
			/// Otherwise, a random number generator, some
			/// more-or-less random data and a SHA-1 digest
			/// is used to generate random data.
		{
		public:
			RandomBuf();
			~RandomBuf();
			int readFromDevice(char* buffer, std::streamsize length);
		};


		class RandomIOS : public virtual std::ios
			/// The base class for RandomInputStream.
			///
			/// This class is needed to ensure the correct initialization
			/// order of the stream buffer and base classes.
		{
		public:
			RandomIOS();
			~RandomIOS();
			RandomBuf* rdbuf();

		protected:
			RandomBuf _buf;
		};


		class RandomInputStream : public RandomIOS, public std::istream
			/// This istream generates random data
			/// using the RandomBuf.
		{
		public:
			RandomInputStream();
			~RandomInputStream();
		};
	}
}
#endif