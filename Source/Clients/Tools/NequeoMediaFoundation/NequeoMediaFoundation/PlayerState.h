/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          PlayerState.h
*  Purpose :       PlayerState class.
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

#ifndef _PLAYERSTATE_H
#define _PLAYERSTATE_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media player current state.
			/// </summary>
			enum PlayerState
			{
				/// <summary>
				/// Ready.
				/// </summary>
				Ready = 0,
				/// <summary>
				/// Open pending.
				/// </summary>
				OpenPending,
				/// <summary>
				/// Started.
				/// </summary>
				Started,
				/// <summary>
				/// Pause pending.
				/// </summary>
				PausePending,
				/// <summary>
				/// Paused.
				/// </summary>
				Paused,
				/// <summary>
				/// Start pending.
				/// </summary>
				StartPending,
				/// <summary>
				/// Stopped.
				/// </summary>
				Stopped,
			};
		}
	}
}
#endif