/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnNatDetectionCompleteParam.h
*  Purpose :       SIP OnNatDetectionCompleteParam class.
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

#ifndef _ONNATDETECTIONCOMPLETEPARAM_H
#define _ONNATDETECTIONCOMPLETEPARAM_H

#include "stdafx.h"

#include "StunNatType.h"

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
			///	The Endpoint has finished performing NAT type
			/// detection that is initiated.
			///	</summary>
			public ref class OnNatDetectionCompleteParam sealed
			{
			public:
				///	<summary>
				///	The Endpoint has finished performing NAT type
				/// detection that is initiated.
				///	</summary>
				OnNatDetectionCompleteParam();

				/// <summary>
				/// Gets or sets the status of the detection process.
				/// </summary>
				property int Status
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the text describing the status, if the status is not PJ_SUCCESS.
				/// </summary>
				property String^ Reason
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets this contains the NAT type as detected by the detection procedure.
				/// </summary>
				property StunNatType NatType
				{
					StunNatType get();
					void set(StunNatType value);
				}

				/// <summary>
				/// Gets or sets the text describing that NAT type.
				/// </summary>
				property String^ NatTypeName
				{
					String^ get();
					void set(String^ value);
				}

			private:
				int _status;
				String^ _reason;
				StunNatType _natType;
				String^ _natTypeName;
			};
		}
	}
}
#endif