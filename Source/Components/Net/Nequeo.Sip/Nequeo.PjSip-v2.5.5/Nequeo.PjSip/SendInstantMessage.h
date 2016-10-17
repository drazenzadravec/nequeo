/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SendInstantMessage.h
*  Purpose :       SIP SendInstantMessage class.
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

#ifndef _SENDINSTANTMESSAGE_H
#define _SENDINSTANTMESSAGE_H

#include "stdafx.h"

#include"SipTxOption.h"

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
			/// <summary>
			/// This structure contains parameters for sending instance message methods,
			/// e.g: Contact::SendInstantMessage(), Call:SendInstantMessage().
			/// </summary>
			public ref class SendInstantMessageParam sealed
			{
			public:
				/// <summary>
				/// This structure contains parameters for sending instance message methods,
				/// e.g: Contact::SendInstantMessage(), Call:SendInstantMessage().
				/// </summary>
				SendInstantMessageParam();

				/// <summary>
				/// This structure contains parameters for sending instance message methods,
				/// e.g: Contact::SendInstantMessage(), Call:SendInstantMessage().
				/// </summary>
				~SendInstantMessageParam();

				/// <summary>
				/// Gets or sets the MIME type. Default is "text/plain".
				/// </summary>
				property String^ ContentType
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the message content.
				/// </summary>
				property String^ Content
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the list of headers etc to be included in outgoing request.
				/// </summary>
				property SipTxOption^ TxOption
				{
					SipTxOption^ get();
					void set(SipTxOption^ value);
				}

			private:
				bool _disposed;

				String^ _contentType;
				String^ _content;
				SipTxOption^ _txOption;
			};
		}
	}
}
#endif