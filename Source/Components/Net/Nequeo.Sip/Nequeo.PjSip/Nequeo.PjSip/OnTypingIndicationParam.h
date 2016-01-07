/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnTypingIndicationParam.h
*  Purpose :       SIP OnTypingIndicationParam class.
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

#ifndef _ONTYPINGINDICATIONPARAM_H
#define _ONTYPINGINDICATIONPARAM_H

#include "stdafx.h"

#include "SipRxData.h"

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
			///	On typing indication paramters.
			///	</summary>
			public ref class OnTypingIndicationParam sealed
			{
			public:
				///	<summary>
				///	On typing indication paramters.
				///	</summary>
				OnTypingIndicationParam();

				/// <summary>
				/// Gets or sets Contact URI.
				/// </summary>
				property String^ ContactUri
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets sender from URI.
				/// </summary>
				property String^ FromUri
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets an indicator if sender is typing.
				/// </summary>
				property bool IsTyping
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets to URI.
				/// </summary>
				property String^ ToUri
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the incoming response that causes this callback to be called.
				/// If the transaction fails because of time out or transport error,
				/// the content will be empty.
				/// </summary>
				property SipRxData^ RxData
				{
					SipRxData^ get();
					void set(SipRxData^ value);
				}

			private:
				String^ _contactUri;
				String^ _fromUri;
				bool _isTyping;
				String^ _toUri;
				SipRxData^ _rxData;
			};
		}
	}
}
#endif