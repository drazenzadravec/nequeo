/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallRole.h
*  Purpose :       SIP CallRole class.
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

#ifndef _CALLROLE_H
#define _CALLROLE_H

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
			/// Call transaction role.
			/// </summary>
			public enum class CallRole : unsigned
			{
				///	<summary>
				/// Role is UAC.
				///	</summary>
				PJSIP_ROLE_UAC,
				///	<summary>
				/// Role is UAS.
				///	</summary>
				PJSIP_ROLE_UAS,
				///	<summary>
				/// Role is UAC.
				///	</summary>
				PJSIP_UAC_ROLE = PJSIP_ROLE_UAC,
				///	<summary>
				/// Role is UAS.
				///	</summary>
				PJSIP_UAS_ROLE = PJSIP_ROLE_UAS

			};
		}
	}
}
#endif