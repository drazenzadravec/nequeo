/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ConfPortInfo.h
*  Purpose :       SIP ConfPortInfo class.
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

#ifndef _CONFPORTINFO_H
#define _CONFPORTINFO_H

#include "stdafx.h"

#include "MediaFormatAudio.h"
#include "MediaFormatVideo.h"

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
			/// This structure descibes information about a particular media port that
			/// has been registered into the conference bridge.
			/// </summary>
			public ref class ConfPortInfo sealed
			{
			public:
				/// <summary>
				/// This structure descibes information about a particular media port that
				/// has been registered into the conference bridge.
				/// </summary>
				ConfPortInfo();

				/// <summary>
				/// Gets or sets the conference port number.
				/// </summary>
				property int PortId
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the port name.
				/// </summary>
				property String^ Name
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the media audio format information.
				/// </summary>
				property MediaFormatAudio^ Format
				{
					MediaFormatAudio^ get();
					void set(MediaFormatAudio^ value);
				}

				/// <summary>
				/// Gets or sets the Tx level adjustment. Value 1.0 means no adjustment, value 0 means
				/// the port is muted, value 2.0 means the level is amplified two times.
				/// </summary>
				property float TxLevelAdj
				{
					float get();
					void set(float value);
				}

				/// <summary>
				/// Gets or sets the Rx level adjustment. Value 1.0 means no adjustment, value 0 means
				/// the port is muted, value 2.0 means the level is amplified two times.
				/// </summary>
				property float RxLevelAdj
				{
					float get();
					void set(float value);
				}

				/// <summary>
				/// Gets or sets the Array of listeners (in other words, ports where this port is transmitting to.
				/// </summary>
				property array<int>^ Listeners
				{
					array<int>^ get();
					void set(array<int>^ value);
				}

			private:
				int _portId;
				String^ _name;
				MediaFormatAudio^ _format;
				float _txLevelAdj;
				float _rxLevelAdj;
				array<int>^ _listeners;
			};
		}
	}
}
#endif