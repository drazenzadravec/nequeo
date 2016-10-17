/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallTransferRequestParam.h
*  Purpose :       SIP OnCallTransferRequestParam class.
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

#ifndef _ONCALLTRANSFERREQUESTPARAM_H
#define _ONCALLTRANSFERREQUESTPARAM_H

#include "stdafx.h"

#include "Call.h"
#include "CallInfo.h"
#include "CallSetting.h"

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
			///	This structure contains parameters for Call::onCallTransferRequest() callback.
			///	</summary>
			public ref class OnCallTransferRequestParam sealed
			{
			public:
				///	<summary>
				///	This structure contains parameters for Call::onCallTransferRequest() callback.
				///	</summary>
				OnCallTransferRequestParam();

				/// <summary>
				/// Gets or sets the current call.
				/// </summary>
				property Call^ CurrentCall
				{
					Call^ get();
					void set(Call^ value);
				}

				/// <summary>
				/// Gets or sets the call information.
				/// </summary>
				property CallInfo^ Info
				{
					CallInfo^ get();
					void set(CallInfo^ value);
				}

				/// <summary>
				/// Gets or sets the status progress of the transfer request.
				/// </summary>
				property StatusCode Code
				{
					StatusCode get();
					void set(StatusCode value);
				}

				/// <summary>
				/// Gets or sets the destination where the call will be transferred to.
				/// </summary>
				property String^ DestinationUri
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the current call setting, application can update this setting
				/// for the call being transferred.
				/// </summary>
				property CallSetting^ Setting
				{
					CallSetting^ get();
					void set(CallSetting^ value);
				}

			private:
				Call^ _currentCall;
				CallInfo^ _info;
				StatusCode _code;
				String^ _destinationUri;
				CallSetting^ _setting;
			};
		}
	}
}
#endif