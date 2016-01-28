/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallOpParam.h
*  Purpose :       SIP CallOpParam class.
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

#ifndef _CALLOPPARAM_H
#define _CALLOPPARAM_H

#include "stdafx.h"

#include "CallSetting.h"
#include "SipTxOption.h"
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
			///	Call options.
			///	</summary>
			public ref class CallOpParam sealed
			{
			public:
				///	<summary>
				///	Call options.
				///	</summary>
				CallOpParam();

				///	<summary>
				///	Call options.
				///	</summary>
				/// <param name="useDefaultCallSetting">Use default call settings. Setting useDefaultCallSetting to true will initialize opt with default
				/// call setting values.</param>
				CallOpParam(bool useDefaultCallSetting);

				///	<summary>
				///	Call options.
				///	</summary>
				~CallOpParam();

				///	<summary>
				///	Gets or sets the call settings.
				///	</summary>
				property CallSetting^ Setting
				{
					CallSetting^ get();
					void set(CallSetting^ value);
				}
				
				///	<summary>
				///	Gets or sets the options.
				///	</summary>
				property unsigned Options
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the reason phrase.
				///	</summary>
				property String^ Reason
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the status code.
				///	</summary>
				property StatusCode Code
				{
					StatusCode get();
					void set(StatusCode value);
				}

				///	<summary>
				///	Gets or sets the list of headers etc to be added to outgoing response message.
				/// Note that this message data will be persistent in all next
				/// answers / responses for this INVITE request.
				///	</summary>
				property SipTxOption^ TxOption
				{
					SipTxOption^ get();
					void set(SipTxOption^ value);
				}

			internal:
				///	<summary>
				///	Gets the use default values.
				///	</summary>
				property bool UseDefaultCallSetting
				{
					bool get();
				}

			private:
				bool _disposed;
				bool _useDefaultCallSetting;

				CallSetting^ _setting;
				unsigned _options;
				String^ _reason;
				StatusCode _code;
				SipTxOption^ _txOption;
			};
		}
	}
}
#endif