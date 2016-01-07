/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SipTxOption.h
*  Purpose :       SIP SipTxOption class.
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

#ifndef _SIPTXOPTION_H
#define _SIPTXOPTION_H

#include "stdafx.h"

#include "SipMediaType.h"

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
			/// This structure describes an incoming SIP message. It corresponds to the
			/// rx data structure in SIP library.
			/// </summary>
			public ref class SipTxOption sealed
			{
			public:
				/// <summary>
				/// This structure describes an incoming SIP message. It corresponds to the
				/// rx data structure in SIP library.
				/// </summary>
				SipTxOption();

				/// <summary>
				/// Gets or sets MIME type of the message body, if application specifies the messageBody
				/// in this structure.
				/// </summary>
				property String^ ContentType
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets Optional message body to be added to the message, only when the
				/// message doesn't have a body.
				/// </summary>
				property String^ MsgBody
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets optional remote target URI (i.e. Target header). If empty (""), the
				/// target will be set to the remote URI(To header). At the moment this
				/// field is only used when sending initial INVITE and MESSAGE requests.
				/// </summary>
				property String^ TargetUri
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets content type of the multipart body. If application wants to send
				/// multipart message bodies, it puts the parts in multipartParts and set
				/// the content type in multipartContentType.If the message already
				/// contains a body, the body will be added to the multipart bodies.
				/// </summary>
				property SipMediaType^ MultipartContentType
				{
					SipMediaType^ get();
					void set(SipMediaType^ value);
				}

			private:
				String^ _contentType;
				String^ _msgBody;
				String^ _targetUri;
				SipMediaType^ _multipartContentType;
			};
		}
	}
}
#endif