/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebResponseContent.cpp
*  Purpose :       Http web response content class.
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

#include "WebResponseContent.h"

using namespace Nequeo::Net::Http;

///	<summary>
///	Add a new header or assign the value to existing key.
///	</summary>
/// <param name="key">The header key.</param>
/// <param name="value">The header value.</param>
void WebResponseContent::AddHeader(const std::string& key, const std::string& value)
{
	// Find existing header.
	auto header_it = _headers.find(key);

	// If not found then add to the headers list.
	if (header_it == _headers.end())
	{
		// Add the header.
		typedef std::pair<std::string, std::string> headerPair;
		_headers.insert(headerPair(key, value));
	}
	else
	{
		// Header found, assign the new value.
		_headers[key] = value;
	}
}