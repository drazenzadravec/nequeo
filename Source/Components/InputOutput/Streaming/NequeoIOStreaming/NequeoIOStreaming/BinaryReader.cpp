/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          BinaryReader.cpp
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

#include "stdafx.h"

#include "BinaryReader.h"
#include "Base\ByteOrder.h"
#include "IO\Text\TextEncoding.h"
#include "IO\Text\TextConverter.h"
#include <algorithm>

namespace Nequeo {
	namespace IO
	{
		BinaryReader::BinaryReader(std::istream& istr, StreamByteOrder byteOrder) :
			_istr(istr),
			_pTextConverter(0)
		{
#if defined(NEQUEO_ARCH_BIG_ENDIAN)
			_flipBytes = (byteOrder == LITTLE_ENDIAN_BYTE_ORDER);
#else
			_flipBytes = (byteOrder == BIG_ENDIAN_BYTE_ORDER);
#endif
		}


		BinaryReader::BinaryReader(std::istream& istr, Nequeo::IO::Text::TextEncoding& encoding, StreamByteOrder byteOrder) :
			_istr(istr),
			_pTextConverter(new Nequeo::IO::Text::TextConverter(encoding, Nequeo::IO::Text::TextEncoding::global()))
		{
#if defined(NEQUEO_ARCH_BIG_ENDIAN)
			_flipBytes = (byteOrder == LITTLE_ENDIAN_BYTE_ORDER);
#else
			_flipBytes = (byteOrder == BIG_ENDIAN_BYTE_ORDER);
#endif
		}


		BinaryReader::~BinaryReader()
		{
			delete _pTextConverter;
		}


		BinaryReader& BinaryReader::operator >> (bool& value)
		{
			_istr.read((char*)&value, sizeof(value));
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (char& value)
		{
			_istr.read((char*)&value, sizeof(value));
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (unsigned char& value)
		{
			_istr.read((char*)&value, sizeof(value));
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (signed char& value)
		{
			_istr.read((char*)&value, sizeof(value));
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (short& value)
		{
			_istr.read((char*)&value, sizeof(value));
			if (_flipBytes) value = ByteOrder::flipBytes(value);
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (unsigned short& value)
		{
			_istr.read((char*)&value, sizeof(value));
			if (_flipBytes) value = ByteOrder::flipBytes(value);
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (int& value)
		{
			_istr.read((char*)&value, sizeof(value));
			if (_flipBytes) value = ByteOrder::flipBytes(value);
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (unsigned int& value)
		{
			_istr.read((char*)&value, sizeof(value));
			if (_flipBytes) value = ByteOrder::flipBytes(value);
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (long& value)
		{
			_istr.read((char*)&value, sizeof(value));
#if defined(NEQUEO_LONG_IS_64_BIT)
			if (_flipBytes) value = ByteOrder::flipBytes((Int64) value);
#else
			if (_flipBytes) value = ByteOrder::flipBytes((Int32)value);
#endif
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (unsigned long& value)
		{
			_istr.read((char*)&value, sizeof(value));
#if defined(NEQUEO_LONG_IS_64_BIT)
			if (_flipBytes) value = ByteOrder::flipBytes((UInt64) value);
#else
			if (_flipBytes) value = ByteOrder::flipBytes((UInt32)value);
#endif
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (float& value)
		{
			if (_flipBytes)
			{
				char* ptr = (char*)&value;
				ptr += sizeof(value);
				for (unsigned i = 0; i < sizeof(value); ++i)
					_istr.read(--ptr, 1);
			}
			else
			{
				_istr.read((char*)&value, sizeof(value));
			}
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (double& value)
		{
			if (_flipBytes)
			{
				char* ptr = (char*)&value;
				ptr += sizeof(value);
				for (unsigned i = 0; i < sizeof(value); ++i)
					_istr.read(--ptr, 1);
			}
			else
			{
				_istr.read((char*)&value, sizeof(value));
			}
			return *this;
		}


#if defined(NEQUEO_HAVE_INT64) && !defined(NEQUEO_LONG_IS_64_BIT)


		BinaryReader& BinaryReader::operator >> (Int64& value)
		{
			_istr.read((char*) &value, sizeof(value));
			if (_flipBytes) value = ByteOrder::flipBytes(value);
			return *this;
		}


		BinaryReader& BinaryReader::operator >> (UInt64& value)
		{
			_istr.read((char*) &value, sizeof(value));
			if (_flipBytes) value = ByteOrder::flipBytes(value);
			return *this;
		}


#endif


		BinaryReader& BinaryReader::operator >> (std::string& value)
		{
			UInt32 size = 0;
			read7BitEncoded(size);
			value.clear();
			if (!_istr.good()) return *this;
			value.reserve(size);
			while (size--)
			{
				char c;
				if (!_istr.read(&c, 1).good()) break;
				value += c;
			}
			if (_pTextConverter)
			{
				std::string converted;
				_pTextConverter->convert(value, converted);
				std::swap(value, converted);
			}
			return *this;
		}


		void BinaryReader::read7BitEncoded(UInt32& value)
		{
			char c;
			value = 0;
			int s = 0;
			do
			{
				c = 0;
				_istr.read(&c, 1);
				UInt32 x = (c & 0x7F);
				x <<= s;
				value += x;
				s += 7;
			} while (c & 0x80);
		}


#if defined(NEQUEO_HAVE_INT64)


		void BinaryReader::read7BitEncoded(UInt64& value)
		{
			char c;
			value = 0;
			int s = 0;
			do
			{
				c = 0;
				_istr.read(&c, 1);
				UInt64 x = (c & 0x7F);
				x <<= s;
				value += x;
				s += 7;
			}
			while (c & 0x80);
		}


#endif


		void BinaryReader::readRaw(std::streamsize length, std::string& value)
		{
			value.clear();
			value.reserve(static_cast<std::string::size_type>(length));
			while (length--)
			{
				char c;
				if (!_istr.read(&c, 1).good()) break;
				value += c;
			}
		}


		void BinaryReader::readRaw(char* buffer, std::streamsize length)
		{
			_istr.read(buffer, length);
		}


		void BinaryReader::readBOM()
		{
			UInt16 bom;
			_istr.read((char*)&bom, sizeof(bom));
			_flipBytes = bom != 0xFEFF;
		}
	}
}
