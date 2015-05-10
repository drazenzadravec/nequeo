/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextEncoding.h
*  Purpose :       TextEncoding header.
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

#ifndef _STREAMCONVERTER_H
#define _STREAMCONVERTER_H

#include "GlobalText.h"
#include "TextEncoding.h"
#include "Poco/UnbufferedStreamBuf.h"
#include <istream>
#include <ostream>

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class StreamConverterBuf : public UnbufferedStreamBuf
				/// A StreamConverter converts streams from one encoding (inEncoding)
				/// into another (outEncoding).
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// If a byte sequence is not valid in inEncoding, defaultChar is used
				/// instead and the encoding error count is incremented.
			{
			public:
				StreamConverterBuf(std::istream& istr, const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				/// Creates the StreamConverterBuf and connects it
				/// to the given input stream.

				StreamConverterBuf(std::ostream& ostr, const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				/// Creates the StreamConverterBuf and connects it
				/// to the given output stream.

				~StreamConverterBuf();
				/// Destroys the StreamConverterBuf.

				int errors() const;
				/// Returns the number of encoding errors encountered.

			protected:
				int readFromDevice();
				int writeToDevice(char c);

			private:
				std::istream*       _pIstr;
				std::ostream*       _pOstr;
				const TextEncoding& _inEncoding;
				const TextEncoding& _outEncoding;
				int                 _defaultChar;
				unsigned char       _buffer[TextEncoding::MAX_SEQUENCE_LENGTH];
				int                 _sequenceLength;
				int                 _pos;
				int                 _errors;
			};


			class Foundation_API StreamConverterIOS : public virtual std::ios
				/// The base class for InputStreamConverter and OutputStreamConverter.
				///
				/// This class is needed to ensure the correct initialization
				/// order of the stream buffer and base classes.
			{
			public:
				StreamConverterIOS(std::istream& istr, const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				StreamConverterIOS(std::ostream& ostr, const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				~StreamConverterIOS();
				StreamConverterBuf* rdbuf();
				int errors() const;

			protected:
				StreamConverterBuf _buf;
			};


			class Foundation_API InputStreamConverter : public StreamConverterIOS, public std::istream
				/// This stream converts all characters read from the
				/// underlying istream from one character encoding into another.
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// If a byte sequence read from the underlying stream is not valid in inEncoding, 
				/// defaultChar is used instead and the encoding error count is incremented.
			{
			public:
				InputStreamConverter(std::istream& istr, const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				/// Creates the InputStreamConverter and connects it
				/// to the given input stream.

				~InputStreamConverter();
				/// Destroys the stream.
			};


			class Foundation_API OutputStreamConverter : public StreamConverterIOS, public std::ostream
				/// This stream converts all characters written to the
				/// underlying ostream from one character encoding into another.
				/// If a character cannot be represented in outEncoding, defaultChar
				/// is used instead.
				/// If a byte sequence written to the stream is not valid in inEncoding, 
				/// defaultChar is used instead and the encoding error count is incremented.
			{
			public:
				OutputStreamConverter(std::ostream& ostr, const TextEncoding& inEncoding, const TextEncoding& outEncoding, int defaultChar = '?');
				/// Creates the OutputStreamConverter and connects it
				/// to the given input stream.

				~OutputStreamConverter();
				/// Destroys the CountingOutputStream.
			};
		}
	}
}
#endif
