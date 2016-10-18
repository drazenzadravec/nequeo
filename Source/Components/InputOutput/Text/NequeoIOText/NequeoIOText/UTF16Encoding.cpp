/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UTF16Encoding.cpp
*  Purpose :       UTF16Encoding header.
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

#include "UTF16Encoding.h"
#include "Base\ByteOrder.h"
#include "Base\StringEx.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			const char* UTF16Encoding::_names[] =
			{
				"UTF-16",
				"UTF16",
				NULL
			};


			const TextEncoding::CharacterMap UTF16Encoding::_charMap =
			{
				/* 00 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 10 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 20 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 30 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 40 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 50 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 60 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 70 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 80 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* 90 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* a0 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* b0 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* c0 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* d0 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* e0 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
				/* f0 */	-2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
			};


			UTF16Encoding::UTF16Encoding(ByteOrderType byteOrder)
			{
				setByteOrder(byteOrder);
			}


			UTF16Encoding::UTF16Encoding(int byteOrderMark)
			{
				setByteOrder(byteOrderMark);
			}


			UTF16Encoding::~UTF16Encoding()
			{
			}


			UTF16Encoding::ByteOrderType UTF16Encoding::getByteOrder() const
			{
#if defined(POCO_ARCH_BIG_ENDIAN)
				return _flipBytes ? LITTLE_ENDIAN_BYTE_ORDER : BIG_ENDIAN_BYTE_ORDER;
#else
				return _flipBytes ? BIG_ENDIAN_BYTE_ORDER : LITTLE_ENDIAN_BYTE_ORDER;
#endif
			}


			void UTF16Encoding::setByteOrder(ByteOrderType byteOrder)
			{
#if defined(POCO_ARCH_BIG_ENDIAN)
				_flipBytes = byteOrder == LITTLE_ENDIAN_BYTE_ORDER;
#else
				_flipBytes = byteOrder == BIG_ENDIAN_BYTE_ORDER;;
#endif
			}


			void UTF16Encoding::setByteOrder(int byteOrderMark)
			{
				_flipBytes = byteOrderMark != 0xFEFF;
			}


			const char* UTF16Encoding::canonicalName() const
			{
				return _names[0];
			}


			bool UTF16Encoding::isA(const std::string& encodingName) const
			{
				for (const char** name = _names; *name; ++name)
				{
					if (Nequeo::icompare(encodingName, *name) == 0)
						return true;
				}
				return false;
			}


			const TextEncoding::CharacterMap& UTF16Encoding::characterMap() const
			{
				return _charMap;
			}


			int UTF16Encoding::convert(const unsigned char* bytes) const
			{
				UInt16 uc;
				unsigned char* p = (unsigned char*)&uc;
				*p++ = *bytes++;
				*p++ = *bytes++;

				if (_flipBytes)
				{
					ByteOrder::flipBytes(uc);
				}

				if (uc >= 0xd800 && uc < 0xdc00)
				{
					UInt16 uc2;
					p = (unsigned char*)&uc2;
					*p++ = *bytes++;
					*p++ = *bytes++;

					if (_flipBytes)
					{
						ByteOrder::flipBytes(uc2);
					}
					if (uc2 >= 0xdc00 && uc2 < 0xe000)
					{
						return ((uc & 0x3ff) << 10) + (uc2 & 0x3ff) + 0x10000;
					}
					else
					{
						return -1;
					}
				}
				else
				{
					return uc;
				}
			}


			int UTF16Encoding::convert(int ch, unsigned char* bytes, int length) const
			{
				if (ch <= 0xFFFF)
				{
					if (bytes && length >= 2)
					{
						UInt16 ch1 = _flipBytes ? ByteOrder::flipBytes((UInt16)ch) : (UInt16)ch;
						unsigned char* p = (unsigned char*)&ch1;
						*bytes++ = *p++;
						*bytes++ = *p++;
					}
					return 2;
				}
				else
				{
					if (bytes && length >= 4)
					{
						int ch1 = ch - 0x10000;
						UInt16 w1 = 0xD800 + ((ch1 >> 10) & 0x3FF);
						UInt16 w2 = 0xDC00 + (ch1 & 0x3FF);
						if (_flipBytes)
						{
							w1 = ByteOrder::flipBytes(w1);
							w2 = ByteOrder::flipBytes(w2);
						}
						unsigned char* p = (unsigned char*)&w1;
						*bytes++ = *p++;
						*bytes++ = *p++;
						p = (unsigned char*)&w2;
						*bytes++ = *p++;
						*bytes++ = *p++;
					}
					return 4;
				}
			}


			int UTF16Encoding::queryConvert(const unsigned char* bytes, int length) const
			{
				int ret = -2;

				if (length >= 2)
				{
					UInt16 uc;
					unsigned char* p = (unsigned char*)&uc;
					*p++ = *bytes++;
					*p++ = *bytes++;
					if (_flipBytes)
						ByteOrder::flipBytes(ret);
					if (uc >= 0xd800 && uc < 0xdc00)
					{
						if (length >= 4)
						{
							UInt16 uc2;
							p = (unsigned char*)&uc2;
							*p++ = *bytes++;
							*p++ = *bytes++;
							if (_flipBytes)
								ByteOrder::flipBytes(ret);
							if (uc2 >= 0xdc00 && uc < 0xe000)
							{
								ret = ((uc & 0x3ff) << 10) + (uc2 & 0x3ff) + 0x10000;
							}
							else
							{
								ret = -1;	// Malformed sequence
							}
						}
						else
						{
							ret = -4;	// surrogate pair, four bytes needed
						}
					}
					else
					{
						ret = uc;
					}
				}

				return ret;
			}


			int UTF16Encoding::sequenceLength(const unsigned char* bytes, int length) const
			{
				int ret = -2;

				if (_flipBytes)
				{
					if (length >= 1)
					{
						unsigned char c = *bytes;
						if (c >= 0xd8 && c < 0xdc)
							ret = 4;
						else
							ret = 2;
					}
				}
				else
				{
					if (length >= 2)
					{
						UInt16 uc;
						unsigned char* p = (unsigned char*)&uc;
						*p++ = *bytes++;
						*p++ = *bytes++;
						if (uc >= 0xd800 && uc < 0xdc00)
							ret = 4;
						else
							ret = 2;
					}
				}
				return ret;
			}
		}
	}
}