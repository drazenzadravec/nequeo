/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          BinaryReader.h
*  Purpose :       BinaryReader class.
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

#ifndef _BINARYREADER_H
#define _BINARYREADER_H

#include "GlobalStreaming.h"
#include "IO\Text\TextEncoding.h"
#include "IO\Text\TextConverter.h"
#include "Base\Types.h"
#include <vector>
#include <istream>

namespace Nequeo {
	namespace IO
	{
		/// This class reads basic types (and std::vectors thereof)
		/// in binary form into an input stream.
		/// It provides an extractor-based interface similar to istream.
		/// The reader also supports automatic conversion from big-endian
		/// (network byte order) to little-endian and vice-versa.
		/// Use a BinaryWriter to create a stream suitable for a BinaryReader.
		class BinaryReader
		{
		public:
			enum StreamByteOrder
			{
				NATIVE_BYTE_ORDER = 1,  /// the host's native byte-order
				BIG_ENDIAN_BYTE_ORDER = 2,  /// big-endian (network) byte-order
				NETWORK_BYTE_ORDER = 2,  /// big-endian (network) byte-order
				LITTLE_ENDIAN_BYTE_ORDER = 3,  /// little-endian byte-order
				UNSPECIFIED_BYTE_ORDER = 4   /// unknown, byte-order will be determined by reading the byte-order mark
			};

			BinaryReader(std::istream& istr, StreamByteOrder byteOrder = NATIVE_BYTE_ORDER);
			/// Creates the BinaryReader.

			BinaryReader(std::istream& istr, Nequeo::IO::Text::TextEncoding& encoding, StreamByteOrder byteOrder = NATIVE_BYTE_ORDER);
			/// Creates the BinaryReader using the given TextEncoding.
			///
			/// Strings will be converted from the specified encoding
			/// to the currently set global encoding (see Poco::TextEncoding::global()).

			~BinaryReader();
			/// Destroys the BinaryReader.

			BinaryReader& operator >> (bool& value);
			BinaryReader& operator >> (char& value);
			BinaryReader& operator >> (unsigned char& value);
			BinaryReader& operator >> (signed char& value);
			BinaryReader& operator >> (short& value);
			BinaryReader& operator >> (unsigned short& value);
			BinaryReader& operator >> (int& value);
			BinaryReader& operator >> (unsigned int& value);
			BinaryReader& operator >> (long& value);
			BinaryReader& operator >> (unsigned long& value);
			BinaryReader& operator >> (float& value);
			BinaryReader& operator >> (double& value);
			BinaryReader& operator >> (Nequeo::Int64& value);
			BinaryReader& operator >> (Nequeo::UInt64& value);

			BinaryReader& operator >> (std::string& value);

			template <typename T>
			BinaryReader& operator >> (std::vector<T>& value)
			{
				Nequeo::UInt32 size(0);
				T elem;

				*this >> size;
				if (!good()) return *this;
				value.reserve(size);
				while (this->good() && size-- > 0)
				{
					*this >> elem;
					value.push_back(elem);
				}
				return *this;
			}

			void read7BitEncoded(Nequeo::UInt32& value);
			/// Reads a 32-bit unsigned integer in compressed format.
			/// See BinaryWriter::write7BitEncoded() for a description
			/// of the compression algorithm.

			void read7BitEncoded(Nequeo::UInt64& value);
			/// Reads a 64-bit unsigned integer in compressed format.
			/// See BinaryWriter::write7BitEncoded() for a description
			/// of the compression algorithm.

			void readRaw(std::streamsize length, std::string& value);
			/// Reads length bytes of raw data into value.

			void readRaw(char* buffer, std::streamsize length);
			/// Reads length bytes of raw data into buffer.

			void readBOM();
			/// Reads a byte-order mark from the stream and configures
			/// the reader for the encountered byte order.
			/// A byte-order mark is a 16-bit integer with a value of 0xFEFF,
			/// written in host byte order.

			bool good();
			/// Returns _istr.good();

			bool fail();
			/// Returns _istr.fail();

			bool bad();
			/// Returns _istr.bad();

			bool eof();
			/// Returns _istr.eof();

			std::istream& stream() const;
			/// Returns the underlying stream.

			StreamByteOrder byteOrder() const;
			/// Returns the byte-order used by the reader, which is
			/// either BIG_ENDIAN_BYTE_ORDER or LITTLE_ENDIAN_BYTE_ORDER.

		private:
			std::istream&  _istr;
			bool           _flipBytes;
			Nequeo::IO::Text::TextConverter* _pTextConverter;
		};


		//
		// inlines
		//
		inline bool BinaryReader::good()
		{
			return _istr.good();
		}


		inline bool BinaryReader::fail()
		{
			return _istr.fail();
		}


		inline bool BinaryReader::bad()
		{
			return _istr.bad();
		}


		inline bool BinaryReader::eof()
		{
			return _istr.eof();
		}


		inline std::istream& BinaryReader::stream() const
		{
			return _istr;
		}


		inline BinaryReader::StreamByteOrder BinaryReader::byteOrder() const
		{
#if defined(NEQUEO_ARCH_BIG_ENDIAN)
			return _flipBytes ? LITTLE_ENDIAN_BYTE_ORDER : BIG_ENDIAN_BYTE_ORDER;
#else
			return _flipBytes ? BIG_ENDIAN_BYTE_ORDER : LITTLE_ENDIAN_BYTE_ORDER;
#endif
		}
	}
}
#endif
