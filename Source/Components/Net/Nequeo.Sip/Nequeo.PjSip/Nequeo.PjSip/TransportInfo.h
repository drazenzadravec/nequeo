/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          TransportInfo.h
*  Purpose :       SIP TransportInfo class.
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

#ifndef _TRANSPORTINFO_H
#define _TRANSPORTINFO_H

#include "stdafx.h"

#include "TransportType.h"

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
			/// Transport info.
			///	</summary>
			public ref class TransportInfo sealed
			{
			public:
				///	<summary>
				/// Transport info.
				///	</summary>
				TransportInfo();

				/// <summary>
				/// Gets or sets the transport identification.
				/// </summary>
				property int TransportId
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the transport type.
				/// </summary>
				property TransportType Transport
				{
					TransportType get();
					void set(TransportType value);
				}

				/// <summary>
				/// Gets or sets the transport protocol.
				/// </summary>
				property String^ Protocol
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the transport type name.
				/// </summary>
				property String^ TypeName
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the transport info.
				/// </summary>
				property String^ Info
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the local socket address.
				/// </summary>
				property String^ LocalAddress
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the local socket address name.
				/// </summary>
				property String^ LocalName
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the transport flags.
				/// </summary>
				property unsigned Flags
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the current number of objects currently referencing this transport.
				/// </summary>
				property unsigned UsageCount
				{
					unsigned get();
					void set(unsigned value);
				}

			private:
				int _transportId;
				TransportType _transport;
				String^ _protocol;
				String^ _typeName;
				String^ _info;
				String^ _localAddress;
				String^ _localName;
				unsigned _flags;
				unsigned _usageCount;

			};
		}
	}
}
#endif