/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          BufferAllocator.h
*  Purpose :       BufferAllocator class.
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

#ifndef _BUFFERALLOCATOR_H
#define _BUFFERALLOCATOR_H

#include "GlobalStreaming.h"

namespace Nequeo {
	namespace IO
	{
		/// The BufferAllocator used if no specific
		/// BufferAllocator has been specified.
		template <typename ch>
		class BufferAllocator
		{
		public:
			typedef ch char_type;

			static char_type* allocate(std::streamsize size)
			{
				return new char_type[static_cast<std::size_t>(size)];
			}

			static void deallocate(char_type* ptr, std::streamsize size)
			{
				delete[] ptr;
			}
		};
	}
}
#endif