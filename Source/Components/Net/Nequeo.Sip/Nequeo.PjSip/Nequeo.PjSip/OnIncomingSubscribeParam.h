/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnIncomingSubscribeParam.h
*  Purpose :       SIP OnIncomingSubscribeParam class.
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

#ifndef _ONINCOMINGSUBSCRIBEPARAM_H
#define _ONINCOMINGSUBSCRIBEPARAM_H

#include "stdafx.h"

#include "SipTxOption.h"
#include "SipRxData.h"
#include "StatusCode.h"

#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	On incoming subscribe paramters.
			///	</summary>
			public ref class OnIncomingSubscribeParam sealed
			{
			public:
				///	<summary>
				///	On incoming subscribe paramters.
				///	</summary>
				OnIncomingSubscribeParam();

				/// <summary>
				/// Gets or sets the status code.
				/// </summary>
				property StatusCode Code
				{
					StatusCode get();
					void set(StatusCode value);
				}

				/// <summary>
				/// Gets or sets the sender URI.
				/// </summary>
				property String^ FromUri
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the reason phrase to respond to the request.
				/// </summary>
				property String^ Reason
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the incoming INVITE request.
				/// </summary>
				property SipRxData^ RxData
				{
					SipRxData^ get();
					void set(SipRxData^ value);
				}

				/// <summary>
				/// Gets or sets additional data to be sent with the response, if any.
				/// </summary>
				property SipTxOption^ TxOption
				{
					SipTxOption^ get();
					void set(SipTxOption^ value);
				}

			private:
				StatusCode _code;
				String^ _fromUri;
				SipRxData^ _rxData;
				SipTxOption^ _txOption;
				String^ _reason;
			};
		}
	}
}
#endif