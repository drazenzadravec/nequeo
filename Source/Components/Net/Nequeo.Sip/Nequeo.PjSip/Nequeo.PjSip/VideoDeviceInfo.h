/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoDeviceInfo.h
*  Purpose :       SIP VideoDeviceInfo class.
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

#ifndef _VIDEODEVICEINFO_H
#define _VIDEODEVICEINFO_H

#include "stdafx.h"

#include "MediaFormat.h"
#include "MediaDirection.h"

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
			/// Video device info.
			/// </summary>
			public ref class VideoDeviceInfo sealed
			{
			public:
				/// <summary>
				/// Video device info.
				/// </summary>
				VideoDeviceInfo();

				/// <summary>
				/// Gets or sets the device id.
				/// </summary>
				property int Id
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the device name.
				/// </summary>
				property String^ Name
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the underlying driver name.
				/// </summary>
				property String^ Driver
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the supported direction of the video device, i.e. whether it supports
				/// capture only, render only, or both.
				/// </summary>
				property MediaDirection Direction
				{
					MediaDirection get();
					void set(MediaDirection value);
				}

				/// <summary>
				/// Gets or sets the device capabilities, as bitmask combination.
				/// </summary>
				property unsigned Caps
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the array of media formats.
				/// </summary>
				property array<MediaFormat^>^ MediaFormats
				{
					array<MediaFormat^>^ get();
					void set(array<MediaFormat^>^ value);
				}

			private:
				int _id;
				String^ _name;
				String^ _driver;
				unsigned _caps;
				MediaDirection _direction;
				array<MediaFormat^>^ _mediaFormats;
			};
		}
	}
}
#endif