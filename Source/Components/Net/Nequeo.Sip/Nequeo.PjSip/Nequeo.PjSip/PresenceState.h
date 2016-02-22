/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          PresenceState.h
*  Purpose :       SIP PresenceState class.
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

#ifndef _PRESENCESTATE_H
#define _PRESENCESTATE_H

#include "stdafx.h"

#include "BuddyStatus.h"
#include "RpidActivity.h"

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
			/// Presence state.
			/// </summary>
			public ref class PresenceState sealed
			{
			public:
				/// <summary>
				/// Presence state.
				/// </summary>
				PresenceState();

				/// <summary>
				/// Gets or sets the activity type.
				/// </summary>
				property RpidActivity Activity
				{
					RpidActivity get();
					void set(RpidActivity value);
				}

				/// <summary>
				/// Gets or sets the optional text describing the person/element.
				/// </summary>
				property String^ Note
				{
					String^ get();
					void set(String^ value);
				}
				
				/// <summary>
				/// Gets or sets the optional RPID ID string.
				/// </summary>
				property String^ RpidId
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the buddy's online status.
				/// </summary>
				property BuddyStatus Status
				{
					BuddyStatus get();
					void set(BuddyStatus value);
				}

				/// <summary>
				/// Gets or sets the text to describe buddy's online status.
				/// </summary>
				property String^ StatusText
				{
					String^ get();
					void set(String^ value);
				}

			private:
				RpidActivity _activity;
				String^ _note;
				String^ _rpidId;
				BuddyStatus _status;
				String^ _statusText;
			};
		}
	}
}
#endif