/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContactMapper.cpp
*  Purpose :       SIP Contact Mapper class.
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

#include "ContactMapper.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Contact callbacks.
///	</summary>
ContactMapper::ContactMapper() :
	_disposed(false)
{
}

///	<summary>
///	Contact callbacks.
///	</summary>
ContactMapper::~ContactMapper()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets specify whether presence subscription should start immediately.
///	</summary>
void ContactMapper::SetSubscribe(bool value)
{
	_subscribe = value;
}
bool ContactMapper::GetSubscribe()
{
	return _subscribe;
}

///	<summary>
///	Gets or sets the contact URL or name address (sip:[Name or IP Address]@[Provider Domain or IP Address]:[Optional port number]).
///	</summary>
void ContactMapper::SetUri(std::string value)
{
	_uri = value;
}
std::string ContactMapper::GetUri()
{
	return _uri;
}