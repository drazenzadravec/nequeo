/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallSetting.h
*  Purpose :       SIP CallSetting class.
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

#ifndef _CALLSETTING_H
#define _CALLSETTING_H

#include "stdafx.h"

#include "CallFlag.h"
#include "VidReqKeyframeMethod.h"

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
			///	Call settings.
			///	</summary>
			public ref class CallSetting sealed
			{
			public:
				///	<summary>
				///	Call settings.
				///	</summary>
				CallSetting();

				///	<summary>
				///	Call settings.
				///	</summary>
				/// <param name="useDefaultValues">Use default values.</param>
				CallSetting(bool useDefaultValues);

				///	<summary>
				///	Call settings.
				///	</summary>
				~CallSetting();

				///	<summary>
				///	Gets or sets the bitmask of CallFlag constants.
				///	</summary>
				property CallFlag Flag
				{
					CallFlag get();
					void set(CallFlag value);
				}

				///	<summary>
				///	Gets or sets this flag controls what methods to request keyframe are allowed on
				/// the call. Value is bitmask of VidReqKeyframeMethod.
				///	</summary>
				property VidReqKeyframeMethod ReqKeyframeMethod
				{
					VidReqKeyframeMethod get();
					void set(VidReqKeyframeMethod value);
				}

				///	<summary>
				///	Gets or sets the number of simultaneous active audio streams for this call. Setting
				/// this to zero will disable audio in this call.
				///	</summary>
				property unsigned AudioCount
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the number of simultaneous active video streams for this call. Setting
				/// this to zero will disable video in this call.
				///	</summary>
				property unsigned VideoCount
				{
					unsigned get();
					void set(unsigned value);
				}

			internal:
				///	<summary>
				///	Gets the use default values.
				///	</summary>
				property bool UseDefaultValues
				{
					bool get();
				}

			private:
				bool _disposed;
				bool _useDefaultValues;
				
				CallFlag _flag;
				VidReqKeyframeMethod _reqKeyframeMethod;
				unsigned _audioCount;
				unsigned _videoCount;
			};
		}
	}
}
#endif