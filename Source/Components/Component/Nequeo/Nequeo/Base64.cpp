/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Base64.cpp
*  Purpose :       Base64 class.
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

#include "Base64.h"

static const uint8_t SENTINEL_VALUE = 255;
static const char BASE64_ENCODING_TABLE_MIME[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

namespace Nequeo
{
	Base64::Base64(const char *encodingTable)
	{
		if (encodingTable == nullptr)
		{
			encodingTable = BASE64_ENCODING_TABLE_MIME;
		}

		size_t encodingTableLength = strlen(encodingTable);
		if (encodingTableLength != 64)
		{
			encodingTable = BASE64_ENCODING_TABLE_MIME;
			encodingTableLength = 64;
		}

		memcpy(m_mimeBase64EncodingTable, encodingTable, encodingTableLength);

		memset((void *)m_mimeBase64DecodingTable, 0, 256);

		for (uint32_t i = 0; i < encodingTableLength; ++i)
		{
			uint32_t index = static_cast<uint32_t>(m_mimeBase64EncodingTable[i]);
			m_mimeBase64DecodingTable[index] = static_cast<uint8_t>(i);
		}

		m_mimeBase64DecodingTable[(uint32_t)'='] = SENTINEL_VALUE;
	}

	Nequeo::String Base64::Encode(const Nequeo::ByteBuffer& buffer) const
	{
		size_t bufferLength = buffer.GetLength();
		size_t blockCount = (bufferLength + 2) / 3;
		size_t remainderCount = (bufferLength % 3);

		Nequeo::String outputString;
		outputString.reserve(CalculateBase64EncodedLength(buffer));

		for (size_t i = 0; i < bufferLength; i += 3)
		{
			uint32_t block = buffer[i];

			block <<= 8;
			if (i + 1 < bufferLength)
			{
				block = block | buffer[i + 1];
			}

			block <<= 8;
			if (i + 2 < bufferLength)
			{
				block = block | buffer[i + 2];
			}

			outputString.push_back(m_mimeBase64EncodingTable[(block >> 18) & 0x3F]);
			outputString.push_back(m_mimeBase64EncodingTable[(block >> 12) & 0x3F]);
			outputString.push_back(m_mimeBase64EncodingTable[(block >> 6) & 0x3F]);
			outputString.push_back(m_mimeBase64EncodingTable[block & 0x3F]);
		}

		if (remainderCount > 0)
		{
			outputString[blockCount * 4 - 1] = '=';
			if (remainderCount == 1)
			{
				outputString[blockCount * 4 - 2] = '=';
			}
		}

		return outputString;
	}

	Nequeo::ByteBuffer Base64::Decode(const Nequeo::String& str) const
	{
		size_t decodedLength = CalculateBase64DecodedLength(str);

		Nequeo::ByteBuffer buffer(decodedLength);

		const char* rawString = str.c_str();
		size_t blockCount = str.length() / 4;
		for (size_t i = 0; i < blockCount; ++i)
		{
			size_t stringIndex = i * 4;

			uint32_t value1 = m_mimeBase64DecodingTable[uint32_t(rawString[stringIndex])];
			uint32_t value2 = m_mimeBase64DecodingTable[uint32_t(rawString[++stringIndex])];
			uint32_t value3 = m_mimeBase64DecodingTable[uint32_t(rawString[++stringIndex])];
			uint32_t value4 = m_mimeBase64DecodingTable[uint32_t(rawString[++stringIndex])];

			size_t bufferIndex = i * 3;
			buffer[bufferIndex] = static_cast<uint8_t>((value1 << 2) | ((value2 >> 4) & 0x03));
			if (value3 != SENTINEL_VALUE)
			{
				buffer[++bufferIndex] = static_cast<uint8_t>(((value2 << 4) & 0xF0) | ((value3 >> 2) & 0x0F));
				if (value4 != SENTINEL_VALUE)
				{
					buffer[++bufferIndex] = static_cast<uint8_t>((value3 & 0x03) << 6 | value4);
				}
			}
		}

		return buffer;
	}

	size_t Base64::CalculateBase64DecodedLength(const Nequeo::String& b64input)
	{
		size_t len = b64input.length();
		if (len == 0)
		{
			return 0;
		}

		size_t padding = 0;

		if (b64input[len - 1] == '=' && b64input[len - 2] == '=') //last two chars are =
			padding = 2;
		else if (b64input[len - 1] == '=') //last char is =
			padding = 1;

		return (len * 3 / 4 - padding);
	}

	size_t Base64::CalculateBase64EncodedLength(const Nequeo::ByteBuffer& buffer)
	{
		return 4 * ((buffer.GetLength() + 2) / 3);
	}
}