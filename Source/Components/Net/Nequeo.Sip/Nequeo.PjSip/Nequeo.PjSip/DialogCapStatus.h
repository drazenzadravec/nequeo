/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          DialogCapStatus.h
*  Purpose :       SIP DialogCapStatus class.
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

#ifndef _DIALOGCAPSTATUS_H
#define _DIALOGCAPSTATUS_H

#include "stdafx.h"

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
			/// Dialog capability status.
			/// </summary>
			public enum class DialogCapStatus : unsigned
			{
				///	<summary>
				/// Capability is unsupported.
				///	</summary>
				PJSIP_DIALOG_CAP_UNSUPPORTED = 0,
				///	<summary>
				/// Capability is supported.
				///	</summary>
				PJSIP_DIALOG_CAP_SUPPORTED = 1,
				///	<summary>
				/// Unknown capability status. This is usually because we lack the 
				/// capability info which is retrieved from capability header specified
				/// in the dialog messages.
				///	</summary>
				PJSIP_DIALOG_CAP_UNKNOWN = 2
			};
		}
	}
}
#endif