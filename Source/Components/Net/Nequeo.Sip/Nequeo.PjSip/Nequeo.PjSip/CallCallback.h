/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallCallback.h
*  Purpose :       SIP cALL Callback class.
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

#ifndef _CALLCALLBACK_H
#define _CALLCALLBACK_H

#include "stdafx.h"

#include "AccountCallback.h"

#include "pjsua2\call.hpp"
#include "pjsua2\presence.hpp"
#include "pjsua2\account.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Call callbacks.
			///	</summary>
			class CallCallback : public pj::Call
			{
			public:
				///	<summary>
				///	Call callbacks.
				///	</summary>
				/// <param name="account">The Sip account.</param>
				/// <param name="callId">An index call id (0 - 3).</param>
				CallCallback(AccountCallback& account, int callId);

				///	<summary>
				///	Call callbacks.
				///	</summary>
				virtual ~CallCallback();

				

			private:
				bool _disposed;

				
			};
		}
	}
}
#endif