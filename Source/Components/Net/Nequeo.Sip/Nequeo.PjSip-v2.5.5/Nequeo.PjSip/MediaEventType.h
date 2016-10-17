/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaEventType.h
*  Purpose :       SIP MediaEventType class.
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

#ifndef _MEDIAEVENTTYPE_H
#define _MEDIAEVENTTYPE_H

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
			/// <summary>
			/// This enumeration describes list of media events.
			/// </summary>
			public enum class MediaEventType
			{
				/// <summary>
				/// No event.
				/// </summary>
				PJMEDIA_EVENT_NONE,
				/// <summary>
				/// Media format has changed event.
				/// </summary>
				PJMEDIA_EVENT_FMT_CHANGED = PJMEDIA_FOURCC('F', 'M', 'C', 'H'),
				/// <summary>
				/// Video window is being closed.
				/// </summary>
				PJMEDIA_EVENT_WND_CLOSING = PJMEDIA_FOURCC('W', 'N', 'C', 'L'),
				/// <summary>
				/// Video window has been closed event.
				/// </summary>
				PJMEDIA_EVENT_WND_CLOSED = PJMEDIA_FOURCC('W', 'N', 'C', 'O'),
				/// <summary>
				/// Video window has been resized event.
				/// </summary>
				PJMEDIA_EVENT_WND_RESIZED = PJMEDIA_FOURCC('W', 'N', 'R', 'Z'),
				/// <summary>
				/// Mouse button has been pressed event.
				/// </summary>
				PJMEDIA_EVENT_MOUSE_BTN_DOWN = PJMEDIA_FOURCC('M', 'S', 'D', 'N'),
				/// <summary>
				/// Video keyframe has just been decoded event.
				/// </summary>
				PJMEDIA_EVENT_KEYFRAME_FOUND = PJMEDIA_FOURCC('I', 'F', 'R', 'F'),
				/// <summary>
				/// Video decoding error due to missing keyframe event.
				/// </summary>
				PJMEDIA_EVENT_KEYFRAME_MISSING = PJMEDIA_FOURCC('I', 'F', 'R', 'M'),
				/// <summary>
				/// Video orientation has been changed event.
				/// </summary>
				PJMEDIA_EVENT_ORIENT_CHANGED = PJMEDIA_FOURCC('O', 'R', 'N', 'T')
			};
		}
	}
}
#endif