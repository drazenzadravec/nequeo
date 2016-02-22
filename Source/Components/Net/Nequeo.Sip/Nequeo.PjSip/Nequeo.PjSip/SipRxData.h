/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SipRxData.h
*  Purpose :       SIP SipRxData class.
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

#ifndef _SIPRXDATA_H
#define _SIPRXDATA_H

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
			///	This structure describes an incoming SIP message. It corresponds to the
			/// rx data structure in SIP library.
			///	</summary>
			public ref class SipRxData sealed
			{
			public:
				///	<summary>
				///	This structure describes an incoming SIP message. It corresponds to the
				/// rx data structure in SIP library.
				///	</summary>
				SipRxData();

				/// <summary>
				/// Gets or sets a short info string describing the request, which normally contains
				/// the request method and its CSeq.
				/// </summary>
				property String^ Info
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the source address of the message.
				/// </summary>
				property String^ SrcAddress
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the whole message data as a string, containing both the header section
				/// and message body section.
				/// </summary>
				property String^ WholeMsg
				{
					String^ get();
					void set(String^ value);
				}

			private:
				String^ _info;
				String^ _srcAddress;
				String^ _wholeMsg;
			};
		}
	}
}
#endif