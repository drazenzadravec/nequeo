/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaTransportInfo.h
*  Purpose :       SIP MediaTransportInfo class.
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

#ifndef _MEDIATRANSPORTINFO_H
#define _MEDIATRANSPORTINFO_H

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
			///	<summary>
			///	This structure describes media transport informations.
			///	</summary>
			public ref class MediaTransportInfo sealed
			{
			public:
				///	<summary>
				///	This structure describes media transport informations.
				///	</summary>
				MediaTransportInfo();

				///	<summary>
				///	This structure describes media transport informations.
				///	</summary>
				~MediaTransportInfo();

				///	<summary>
				///	Gets or sets the remote address where RTP originated from.
				///	</summary>
				property String^ RemoteRtpName
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the remote address where RTCP originated from.
				///	</summary>
				property String^ RemoteRtcpName
				{
					String^ get();
					void set(String^ value);
				}

			private:
				bool _disposed;

				String^ _remoteRtpName;
				String^ _remoteRtcpName;
			};
		}
	}
}
#endif