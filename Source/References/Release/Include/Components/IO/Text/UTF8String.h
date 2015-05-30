/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UTF8String.h
*  Purpose :       UTF8String header.
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

#ifndef _UTF8STRING_H
#define _UTF8STRING_H

#include "GlobalText.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			struct UTF8
				/// This class provides static methods that are UTF-8 capable variants
				/// of the same functions in Poco/String.h.
				///
				/// The various variants of icompare() provide case insensitive comparison
				/// for UTF-8 encoded strings.
				///
				/// toUpper(), toUpperInPlace(), toLower() and toLowerInPlace() provide
				/// Unicode-based character case transformation for UTF-8 encoded strings.
			{
				static int icompare(const std::string& str, std::string::size_type pos, std::string::size_type n, std::string::const_iterator it2, std::string::const_iterator end2);
				static int icompare(const std::string& str1, const std::string& str2);
				static int icompare(const std::string& str1, std::string::size_type n1, const std::string& str2, std::string::size_type n2);
				static int icompare(const std::string& str1, std::string::size_type n, const std::string& str2);
				static int icompare(const std::string& str1, std::string::size_type pos, std::string::size_type n, const std::string& str2);
				static int icompare(const std::string& str1, std::string::size_type pos1, std::string::size_type n1, const std::string& str2, std::string::size_type pos2, std::string::size_type n2);
				static int icompare(const std::string& str1, std::string::size_type pos1, std::string::size_type n, const std::string& str2, std::string::size_type pos2);
				static int icompare(const std::string& str, std::string::size_type pos, std::string::size_type n, const std::string::value_type* ptr);
				static int icompare(const std::string& str, std::string::size_type pos, const std::string::value_type* ptr);
				static int icompare(const std::string& str, const std::string::value_type* ptr);

				static std::string toUpper(const std::string& str);
				static std::string& toUpperInPlace(std::string& str);
				static std::string toLower(const std::string& str);
				static std::string& toLowerInPlace(std::string& str);
			};
		}
	}
}
#endif
