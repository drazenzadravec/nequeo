/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright � Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Latin9Encoding.cpp
*  Purpose :       Latin9Encoding header.
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

#include "Latin9Encoding.h"
#include "Base\StringEx.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			const char* Latin9Encoding::_names[] =
			{
				"ISO-8859-15",
				"Latin9",
				"Latin-9",
				NULL
			};


			const TextEncoding::CharacterMap Latin9Encoding::_charMap =
			{
				/* 00 */	0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009, 0x000a, 0x000b, 0x000c, 0x000d, 0x000e, 0x000f,
				/* 10 */	0x0010, 0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017, 0x0018, 0x0019, 0x001a, 0x001b, 0x001c, 0x001d, 0x001e, 0x001f,
				/* 20 */	0x0020, 0x0021, 0x0022, 0x0023, 0x0024, 0x0025, 0x0026, 0x0027, 0x0028, 0x0029, 0x002a, 0x002b, 0x002c, 0x002d, 0x002e, 0x002f,
				/* 30 */	0x0030, 0x0031, 0x0032, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037, 0x0038, 0x0039, 0x003a, 0x003b, 0x003c, 0x003d, 0x003e, 0x003f,
				/* 40 */	0x0040, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004a, 0x004b, 0x004c, 0x004d, 0x004e, 0x004f,
				/* 50 */	0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 0x0058, 0x0059, 0x005a, 0x005b, 0x005c, 0x005d, 0x005e, 0x005f,
				/* 60 */	0x0060, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006a, 0x006b, 0x006c, 0x006d, 0x006e, 0x006f,
				/* 70 */	0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007a, 0x007b, 0x007c, 0x007d, 0x007e, 0x007f,
				/* 80 */	0x0080, 0x0081, 0x0082, 0x0083, 0x0084, 0x0085, 0x0086, 0x0087, 0x0088, 0x0089, 0x008a, 0x008b, 0x008c, 0x008d, 0x008e, 0x008f,
				/* 90 */	0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097, 0x0098, 0x0099, 0x009a, 0x009b, 0x009c, 0x009d, 0x009e, 0x009f,
				/* a0 */	0x00a0, 0x00a1, 0x00a2, 0x00a3, 0x20ac, 0x00a5, 0x0160, 0x00a7, 0x0161, 0x00a9, 0x00aa, 0x00ab, 0x00ac, 0x00ad, 0x00ae, 0x00af,
				/* b0 */	0x00b0, 0x00b1, 0x00b2, 0x00b3, 0x017d, 0x00b5, 0x00b6, 0x00b7, 0x017e, 0x00b9, 0x00ba, 0x00bb, 0x0152, 0x0153, 0x0178, 0x00bf,
				/* c0 */	0x00c0, 0x00c1, 0x00c2, 0x00c3, 0x00c4, 0x00c5, 0x00c6, 0x00c7, 0x00c8, 0x00c9, 0x00ca, 0x00cb, 0x00cc, 0x00cd, 0x00ce, 0x00cf,
				/* d0 */	0x00d0, 0x00d1, 0x00d2, 0x00d3, 0x00d4, 0x00d5, 0x00d6, 0x00d7, 0x00d8, 0x00d9, 0x00da, 0x00db, 0x00dc, 0x00dd, 0x00de, 0x00df,
				/* e0 */	0x00e0, 0x00e1, 0x00e2, 0x00e3, 0x00e4, 0x00e5, 0x00e6, 0x00e7, 0x00e8, 0x00e9, 0x00ea, 0x00eb, 0x00ec, 0x00ed, 0x00ee, 0x00ef,
				/* f0 */	0x00f0, 0x00f1, 0x00f2, 0x00f3, 0x00f4, 0x00f5, 0x00f6, 0x00f7, 0x00f8, 0x00f9, 0x00fa, 0x00fb, 0x00fc, 0x00fd, 0x00fe, 0x00ff,
			};


			Latin9Encoding::Latin9Encoding()
			{
			}


			Latin9Encoding::~Latin9Encoding()
			{
			}


			const char* Latin9Encoding::canonicalName() const
			{
				return _names[0];
			}


			bool Latin9Encoding::isA(const std::string& encodingName) const
			{
				for (const char** name = _names; *name; ++name)
				{
					if (Nequeo::icompare(encodingName, *name) == 0)
						return true;
				}
				return false;
			}


			const TextEncoding::CharacterMap& Latin9Encoding::characterMap() const
			{
				return _charMap;
			}


			int Latin9Encoding::convert(const unsigned char* bytes) const
			{
				return _charMap[*bytes];
			}


			int Latin9Encoding::convert(int ch, unsigned char* bytes, int length) const
			{
				if (ch >= 0 && ch <= 255 && _charMap[ch] == ch)
				{
					if (bytes && length >= 1)
						*bytes = ch;
					return 1;
				}
				else switch (ch)
				{
				case 0x0152: if (bytes && length >= 1) *bytes = 0xbc; return 1;
				case 0x0153: if (bytes && length >= 1) *bytes = 0xbd; return 1;
				case 0x0160: if (bytes && length >= 1) *bytes = 0xa6; return 1;
				case 0x0161: if (bytes && length >= 1) *bytes = 0xa8; return 1;
				case 0x017d: if (bytes && length >= 1) *bytes = 0xb4; return 1;
				case 0x017e: if (bytes && length >= 1) *bytes = 0xb8; return 1;
				case 0x0178: if (bytes && length >= 1) *bytes = 0xbe; return 1;
				case 0x20ac: if (bytes && length >= 1) *bytes = 0xa4; return 1;
				default: return 0;
				}
			}


			int Latin9Encoding::queryConvert(const unsigned char* bytes, int length) const
			{
				if (1 <= length)
					return _charMap[*bytes];
				else
					return -1;
			}


			int Latin9Encoding::sequenceLength(const unsigned char* bytes, int length) const
			{
				return 1;
			}
		}
	}
}