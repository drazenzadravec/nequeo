/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContactCallback.h
*  Purpose :       SIP Contact Callback class.
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

#ifndef _CONTACTCALLBACK_H
#define _CONTACTCALLBACK_H

#include "stdafx.h"

#include "AccountCallback.h"
#include "ContactMapper.h"

#include "pjsua2\presence.hpp"
#include "pjsua2\account.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			typedef void(*OnBuddyState_Function)();

			///	<summary>
			///	Contact callbacks.
			///	</summary>
			class ContactCallback : public pj::Buddy
			{
			public:
				///	<summary>
				///	Contact callbacks.
				///	</summary>
				ContactCallback();

				///	<summary>
				///	Contact callbacks.
				///	</summary>
				virtual ~ContactCallback();

				///	<summary>
				///	Create buddy and register the buddy to PJSUA-LIB.
				///	</summary>
				/// <param name="pjAccount">The account for this buddy.</param>
				/// <param name="contactMapper">The contact mapper config.</param>
				void Create(AccountCallback& pjAccount, ContactMapper& contactMapper);

				///	<summary>
				/// Notify application when the buddy state has changed.
				/// Application may then query the buddy info to get the details.
				///	</summary>
				void onBuddyState();

				///	<summary>
				///	Set the on buddy state function callback.
				///	</summary>
				/// <param name="onBuddyStateCallBack">The on buddy state function callback.</param>
				void Set_OnBuddyState_Function(OnBuddyState_Function onBuddyStateCallBack);

			private:
				bool _disposed;

				OnBuddyState_Function _onBuddyState_function_internal;
			};
		}
	}
}
#endif