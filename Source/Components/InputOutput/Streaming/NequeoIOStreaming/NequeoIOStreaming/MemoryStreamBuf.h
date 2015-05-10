/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          MemoryStreamBuf.h
*  Purpose :       MemoryStreamBuf class.
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

#ifndef _MEMORYSTREAMBUF_H
#define _MEMORYSTREAMBUF_H

#include "GlobalStreaming.h"

namespace Nequeo {
	namespace IO
	{
		/// BasicMemoryStreamBuf is a simple implementation of a 
		/// stream buffer for reading and writing from a memory area.
		///
		/// This streambuf only supports unidirectional streams.
		/// In other words, the BasicBufferedStreamBuf can be
		/// used for the implementation of an istream or an
		/// ostream, but not for an iostream.
		template <typename ch, typename tr>
		class MemoryStreamBuffer : public std::basic_streambuf<ch, tr>
		{
		protected:
			typedef std::basic_streambuf<ch, tr> Base;
			typedef std::basic_ios<ch, tr> IOS;
			typedef ch char_type;
			typedef tr char_traits;
			typedef typename Base::int_type int_type;
			typedef typename Base::pos_type pos_type;
			typedef typename Base::off_type off_type;

		public:
			MemoryStreamBuffer(char_type* pBuffer, std::streamsize bufferSize) :
				_disposed(false),
				_pBuffer(pBuffer),
				_bufferSize(bufferSize)
			{
				this->setg(_pBuffer, _pBuffer, _pBuffer + _bufferSize);
				this->setp(_pBuffer, _pBuffer + _bufferSize);
			}

			~MemoryStreamBuffer()
			{
				// If not disposed.
				if (!_disposed)
				{
					_disposed = true;
				}
			}

			virtual int_type overflow(int_type c)
			{
				return char_traits::eof();
			}

			virtual int_type underflow()
			{
				return char_traits::eof();
			}

			virtual int sync()
			{
				return 0;
			}

			std::streamsize charsWritten() const
			{
				return static_cast<std::streamsize>(this->pptr() - this->pbase());
			}

			void reset()
				/// Resets the buffer so that current read and write positions
				/// will be set to the beginning of the buffer.
			{
				this->setg(_pBuffer, _pBuffer, _pBuffer + _bufferSize);
				this->setp(_pBuffer, _pBuffer + _bufferSize);
			}

		private:
			bool _disposed;

			char_type*      _pBuffer;
			std::streamsize _bufferSize;

			MemoryStreamBuffer();
			MemoryStreamBuffer(const MemoryStreamBuffer&);
			MemoryStreamBuffer& operator = (const MemoryStreamBuffer&);
		};
	}
}
#endif