/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCreateMediaTransportParam.h
*  Purpose :       SIP OnCreateMediaTransportParam class.
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

#ifndef _ONCREATEMEDIATRANSPORTPARAM_H
#define _ONCREATEMEDIATRANSPORTPARAM_H

#include "stdafx.h"

#include "Call.h"
#include "CallInfo.h"
#include "SipEventType.h"

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
			///	This structure contains parameters for Call::onCreateMediaTransport() callback.
			///	</summary>
			public ref class OnCreateMediaTransportParam sealed
			{
			public:
				///	<summary>
				///	This structure contains parameters for Call::onCreateMediaTransport() callback.
				///	</summary>
				OnCreateMediaTransportParam();

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
				/// Gets or sets the media index in the SDP for which this media transport will be used.
				/// </summary>
				property unsigned MediaIndex
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the bitmask from pjsua_create_media_transport_flag.
				/// </summary>
				property unsigned Flags
				{
					unsigned get();
					void set(unsigned value);
				}

			private:
				Call^ _currentCall;
				CallInfo^ _info;
				unsigned _mediaIndex;
				unsigned _flags;
			};
		}
	}
}
#endif