/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          BufferedBidirectionalStream.h
*  Purpose :       BufferedBidirectionalStream class.
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

#ifndef _BUFFEREDBIDIRECTIONALSTREAM_H
#define _BUFFEREDBIDIRECTIONALSTREAM_H

#include "GlobalStreaming.h"
#include "BufferAllocator.h"

namespace Nequeo {
	namespace IO
	{
		/// This is an implementation of a buffered bidirectional 
		/// streambuf that greatly simplifies the implementation of
		/// custom streambufs of various kinds.
		/// Derived classes only have to override the methods
		/// readFromDevice() or writeToDevice().
		///
		/// In contrast to BasicBufferedStreambuf, this class supports
		/// simultaneous read and write access, so in addition to
		/// istream and ostream this streambuf can also be used
		/// for implementing an iostream.
		template <typename ch, typename tr, typename ba = BufferAllocator<ch> >
		class BasicBufferedBidirectionalStream : public std::basic_streambuf<ch, tr>
		{
		protected:
			typedef std::basic_streambuf<ch, tr> Base;
			typedef std::basic_ios<ch, tr> IOS;
			typedef ch char_type;
			typedef tr char_traits;
			typedef ba Allocator;
			typedef typename Base::int_type int_type;
			typedef typename Base::pos_type pos_type;
			typedef typename Base::off_type off_type;
			typedef typename IOS::openmode openmode;

		public:
			BasicBufferedBidirectionalStream(std::streamsize bufferSize, openmode mode) :
				_bufsize(bufferSize),
				_pReadBuffer(Allocator::allocate(_bufsize)),
				_pWriteBuffer(Allocator::allocate(_bufsize)),
				_mode(mode)
			{
				resetBuffers();
			}

			~BasicBufferedBidirectionalStream()
			{
				Allocator::deallocate(_pReadBuffer, _bufsize);
				Allocator::deallocate(_pWriteBuffer, _bufsize);
			}

			virtual int_type overflow(int_type c)
			{
				if (!(_mode & IOS::out)) return char_traits::eof();

				if (c != char_traits::eof())
				{
					*this->pptr() = char_traits::to_char_type(c);
					this->pbump(1);
				}
				if (flushBuffer() == std::streamsize(-1)) return char_traits::eof();

				return c;
			}

			virtual int_type underflow()
			{
				if (!(_mode & IOS::in)) return char_traits::eof();

				if (this->gptr() && (this->gptr() < this->egptr()))
					return char_traits::to_int_type(*this->gptr());

				int putback = int(this->gptr() - this->eback());
				if (putback > 4) putback = 4;

				char_traits::move(_pReadBuffer + (4 - putback), this->gptr() - putback, putback);

				int n = readFromDevice(_pReadBuffer + 4, _bufsize - 4);
				if (n <= 0) return char_traits::eof();

				this->setg(_pReadBuffer + (4 - putback), _pReadBuffer + 4, _pReadBuffer + 4 + n);

				// return next character
				return char_traits::to_int_type(*this->gptr());
			}

			virtual int sync()
			{
				if (this->pptr() && this->pptr() > this->pbase())
				{
					if (flushBuffer() == -1) return -1;
				}
				return 0;
			}

		protected:
			void setMode(openmode mode)
			{
				_mode = mode;
			}

			openmode getMode() const
			{
				return _mode;
			}

			void resetBuffers()
			{
				this->setg(_pReadBuffer + 4, _pReadBuffer + 4, _pReadBuffer + 4);
				this->setp(_pWriteBuffer, _pWriteBuffer + (_bufsize - 1));
			}

		private:
			virtual int readFromDevice(char_type* buffer, std::streamsize length)
			{
				return 0;
			}

			virtual int writeToDevice(const char_type* buffer, std::streamsize length)
			{
				return 0;
			}

			int flushBuffer()
			{
				int n = int(this->pptr() - this->pbase());
				if (writeToDevice(this->pbase(), n) == n)
				{
					this->pbump(-n);
					return n;
				}
				return -1;
			}

			std::streamsize _bufsize;
			char_type*      _pReadBuffer;
			char_type*      _pWriteBuffer;
			openmode        _mode;

			BasicBufferedBidirectionalStream(const BasicBufferedBidirectionalStream&);
			BasicBufferedBidirectionalStream& operator = (const BasicBufferedBidirectionalStream&);
		};


		//
		// We provide an instantiation for char
		//
		typedef BasicBufferedBidirectionalStream<char, std::char_traits<char> > BufferedBidirectionalStream;
	}
}
#endif