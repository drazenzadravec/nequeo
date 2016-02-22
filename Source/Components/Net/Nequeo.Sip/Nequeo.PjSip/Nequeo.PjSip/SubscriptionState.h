/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SubscriptionState.h
*  Purpose :       SIP SubscriptionState class.
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

#ifndef _SUBSCRIPTIONSTATE_H
#define _SUBSCRIPTIONSTATE_H

#include "stdafx.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			/// <summary>
			/// This enumeration describes basic subscription state as described in the 
			/// RFC 3265. The standard specifies that extensions may define additional
			/// states.In the case where the state is not known, the subscription state
			/// will be set to PJSIP_EVSUB_STATE_UNKNOWN, and the token will be kept
			/// in state_str member of the susbcription structure.
			/// </summary>
			public enum class SubscriptionState : unsigned
			{
				/// <summary>
				/// State is NULL.
				/// </summary>
				EVSUB_STATE_NULL = 0,
				/// <summary>
				/// Client has sent SUBSCRIBE request.
				/// </summary>
				EVSUB_STATE_SENT = 1,
				/// <summary>
				/// 2xx response to SUBSCRIBE has been sent/received.
				/// </summary>
				EVSUB_STATE_ACCEPTED = 2,
				/// <summary>
				/// Subscription is pending.
				/// </summary>
				EVSUB_STATE_PENDING = 3,
				/// <summary>
				/// Subscription is active.
				/// </summary>
				EVSUB_STATE_ACTIVE = 4,
				/// <summary>
				/// Subscription is terminated.
				/// </summary>
				EVSUB_STATE_TERMINATED = 5,
				/// <summary>
				/// Subscription state can not be determined.
				/// </summary>
				EVSUB_STATE_UNKNOWN = 6
			};
		}
	}
}
#endif