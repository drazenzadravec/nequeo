/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnNatCheckStunServersCompleteParam.h
*  Purpose :       SIP OnNatCheckStunServersCompleteParam class.
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

#ifndef _ONNATCHECKSTUNSERVERSCOMPLETEPARAM_H
#define _ONNATCHECKSTUNSERVERSCOMPLETEPARAM_H

#include "stdafx.h"

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
			///	The Endpoint has finished performing STUN server
			/// checking that is initiated when calling libInit(), or by
			/// calling natCheckStunServers().
			///	</summary>
			public ref class OnNatCheckStunServersCompleteParam sealed
			{
			public:
				///	<summary>
				///	The Endpoint has finished performing STUN server
				/// checking that is initiated when calling libInit(), or by
				/// calling natCheckStunServers().
				///	</summary>
				OnNatCheckStunServersCompleteParam();

				/// <summary>
				/// Gets or sets the status of the detection process.
				/// </summary>
				property int Status
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the server name that yields successful result. This will only
				/// contain value if status is successful.
				/// </summary>
				property String^ Name
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the server IP address and port in "IP:port" format. This will only
				/// contain value if status is successful.
				/// </summary>
				property String^ SocketAddress
				{
					String^ get();
					void set(String^ value);
				}

			private:
				int _status;
				String^ _name;
				String^ _socketAddress;
			};
		}
	}
}
#endif