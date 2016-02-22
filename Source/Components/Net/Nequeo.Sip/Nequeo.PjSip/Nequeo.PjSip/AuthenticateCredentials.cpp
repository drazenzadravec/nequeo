/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AuthenticateCredentials.cpp
*  Purpose :       SIP AuthenticateCredentials class.
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

#include "AuthenticateCredentials.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// If registration is desired, normally there should
/// be at least one credential specified, to successfully authenticate
/// against the service provider. More credentials can be specified, for
/// example when the requests are expected to be challenged by the
/// proxies in the route set.
/// </summary>
AuthenticateCredentials::AuthenticateCredentials() : _disposed(false)
{
	_authCredentials = gcnew array<AuthCredInfo^>(1);
}

///	<summary>
///	Authenticate credentials deconstructor.
///	</summary>
AuthenticateCredentials::~AuthenticateCredentials()
{
	if (!_disposed)
	{
		_disposed = true;
		
		if (_authCredentials != nullptr)
			delete[] _authCredentials;
	}
}

/// <summary>
/// Gets the authenticate credentials.
/// </summary>
array<AuthCredInfo^>^ AuthenticateCredentials::AuthCredentials::get()
{
	return _authCredentials;
}

/// <summary>
/// Sets the authenticate credentials.
/// </summary>
void AuthenticateCredentials::AuthCredentials::set(array<AuthCredInfo^>^ value)
{
	_authCredentials = value;
}