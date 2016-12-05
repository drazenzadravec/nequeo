/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AccountInterface.h
*  Purpose :       SIP Account Interface class.
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

#include <pjsua2\account.hpp>

namespace Nequeo {
	namespace Net {
		namespace Uwp {
			namespace PjSip
			{
				typedef void(*OnIncomingCall_Function)(pj::OnIncomingCallParam&);
				typedef void(*OnIncomingSubscribe_Function)(pj::OnIncomingSubscribeParam&);
				typedef void(*OnInstantMessage_Function)(pj::OnInstantMessageParam&);
				typedef void(*OnInstantMessageStatus_Function)(pj::OnInstantMessageStatusParam&);
				typedef void(*OnMwiInfo_Function)(pj::OnMwiInfoParam&);
				typedef void(*OnTypingIndication_Function)(pj::OnTypingIndicationParam&);
				typedef void(*OnRegStarted_Function)(pj::OnRegStartedParam&);
				typedef void(*OnRegState_Function)(pj::OnRegStateParam&);

				///	<summary>
				///	Account callbacks.
				///	</summary>
				class AccountInterface : public pj::Account
				{
				public:
					///	<summary>
					///	Account callbacks.
					///	</summary>
					AccountInterface();

					///	<summary>
					///	Account callbacks.
					///	</summary>
					virtual ~AccountInterface();
				};
			}
		}
	}
}

