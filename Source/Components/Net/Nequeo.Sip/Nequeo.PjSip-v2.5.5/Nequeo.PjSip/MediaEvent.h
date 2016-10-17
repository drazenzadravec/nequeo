/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaEvent.h
*  Purpose :       SIP MediaEvent class.
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

#ifndef _MEDIAEVENT_H
#define _MEDIAEVENT_H

#include "stdafx.h"

#include "MediaEventType.h"
#include "MediaEventData.h"

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
			///	This structure describes a media event.
			///	</summary>
			public ref class MediaEvent sealed
			{
			public:
				///	<summary>
				///	This structure describes a media event.
				///	</summary>
				MediaEvent();

				/// <summary>
				/// Gets or sets the Additional data/parameters about the event. The type of data
				/// will be specific to the event type being reported.
				/// </summary>
				property MediaEventData^ Data
				{
					MediaEventData^ get();
					void set(MediaEventData^ value);
				}

				/// <summary>
				/// Gets or sets the event type.
				/// </summary>
				property MediaEventType Type
				{
					MediaEventType get();
					void set(MediaEventType value);
				}

			private:
				MediaEventData^ _data;
				MediaEventType _type;
			};
		}
	}
}
#endif