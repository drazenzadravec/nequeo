/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnSelectAccountParam.cpp
*  Purpose :       SIP OnSelectAccountParam class.
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

#include "OnSelectAccountParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
/// Can be used by application to override the account
/// to be used to handle an incoming message.
///	</summary>
OnSelectAccountParam::OnSelectAccountParam()
{
}

/// <summary>
/// Gets or sets the account index to be used to handle the request.
/// Upon entry, this will be filled by the account index
/// chosen by the library.Application may change it to
/// another value to use another account.
/// </summary>
int OnSelectAccountParam::AccountIndex::get()
{
	return _accountIndex;
}

/// <summary>
/// Gets or sets the account index to be used to handle the request.
/// Upon entry, this will be filled by the account index
/// chosen by the library.Application may change it to
/// another value to use another account.
/// </summary>
void OnSelectAccountParam::AccountIndex::set(int value)
{
	_accountIndex = value;
}

/// <summary>
/// Gets or sets the incoming request.
/// </summary>
SipRxData^ OnSelectAccountParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming request.
/// </summary>
void OnSelectAccountParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}