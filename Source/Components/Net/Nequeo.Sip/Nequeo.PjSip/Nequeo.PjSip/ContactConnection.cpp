/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContactConnection.cpp
*  Purpose :       SIP ContactConnection class.
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

#include "ContactConnection.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Contact connection configuration.
///	</summary>
ContactConnection::ContactConnection() :
	_disposed(false)
{
}

///	<summary>
///	Contact connection configuration.
///	</summary>
/// <param name="subscribe">Specify whether presence subscription should start immediately.</param>
/// <param name="uri">The contact URL or name address (sip:[Name or IP Address]@[Provider Domain or IP Address]:[Optional port number]).</param>
ContactConnection::ContactConnection(bool subscribe, String^ uri) :
	_disposed(false)
{
	_subscribe = subscribe;
	_uri = uri;
}

///	<summary>
///	Contact connection configuration.
///	</summary>
ContactConnection::~ContactConnection()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the contact URL or name address (sip:[Name or IP Address]@[Provider Domain or IP Address]:[Optional port number]).
///	</summary>
String^ ContactConnection::Uri::get()
{
	return _uri;
}

///	<summary>
///	Gets or sets the contact URL or name address (sip:[Name or IP Address]@[Provider Domain or IP Address]:[Optional port number]).
///	</summary>
void ContactConnection::Uri::set(String^ value)
{
	_uri = value;
}

///	<summary>
///	Gets or sets specify whether presence subscription should start immediately.
///	</summary>
bool ContactConnection::Subscribe::get()
{
	return _subscribe;
}

///	<summary>
///	Gets or sets specify whether presence subscription should start immediately.
///	</summary>
void ContactConnection::Subscribe::set(bool value)
{
	_subscribe = value;
}