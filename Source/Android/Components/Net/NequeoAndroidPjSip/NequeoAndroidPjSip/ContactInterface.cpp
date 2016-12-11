/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ContactInterface.cpp
*  Purpose :       SIP Contact Interface class.
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

#include "pch.h"

#include "ContactInterface.h"

using namespace Nequeo::Net::Android::PjSip;

///	<summary>
///	Contact callbacks.
///	</summary>
ContactInterface::ContactInterface() :
	_disposed(false)
{
}

///	<summary>
///	Contact callbacks.
///	</summary>
ContactInterface::~ContactInterface()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Create buddy and register the buddy to PJSUA-LIB.
///	</summary>
/// <param name="pjAccount">The account for this buddy.</param>
/// <param name="contactMapper">The contact mapper config.</param>
void ContactInterface::Create(AccountInterface& pjAccount)
{
	pj::BuddyConfig buddyConfig;
	//buddyConfig.subscribe = contactMapper.GetSubscribe();
	//buddyConfig.uri = contactMapper.GetUri();

	// Create the contact.
	create(pjAccount, buddyConfig);
}

///	<summary>
///	Set the on buddy state function callback.
///	</summary>
/// <param name="onBuddyStateCallBack">The on buddy state function callback.</param>
void ContactInterface::Set_OnBuddyState_Function(OnBuddyState_Function onBuddyStateCallBack)
{
	_onBuddyState_function_internal = onBuddyStateCallBack;
}

///	<summary>
/// Notify application when the buddy state has changed.
/// Application may then query the buddy info to get the details.
///	</summary>
void ContactInterface::onBuddyState()
{
	_onBuddyState_function_internal();
}