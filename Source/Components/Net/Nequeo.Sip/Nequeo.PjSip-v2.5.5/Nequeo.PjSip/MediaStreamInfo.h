/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaStreamInfo.h
*  Purpose :       SIP MediaStreamInfo class.
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

#ifndef _MEDIASTREAMINFO_H
#define _MEDIASTREAMINFO_H

#include "stdafx.h"

#include "MediaType.h"
#include "MediaTransportProtocol.h"
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
			///	<summary>
			///	Media stream info.
			///	</summary>
			public ref class MediaStreamInfo sealed
			{
			public:
				///	<summary>
				///	Media stream info.
				///	</summary>
				MediaStreamInfo();

				///	<summary>
				///	Media stream info.
				///	</summary>
				~MediaStreamInfo();

				///	<summary>
				///	Gets or sets the media type.
				///	</summary>
				property MediaType Type
				{
					MediaType get();
					void set(MediaType value);
				}

				///	<summary>
				///	Gets or sets the transport protocol (RTP/AVP, etc.)
				///	</summary>
				property MediaTransportProtocol TransportProtocol
				{
					MediaTransportProtocol get();
					void set(MediaTransportProtocol value);
				}

				///	<summary>
				///	Gets or sets the media direction.
				///	</summary>
				property MediaDirection Direction
				{
					MediaDirection get();
					void set(MediaDirection value);
				}

				///	<summary>
				///	Gets or sets the remote RTP address.
				///	</summary>
				property String^ RemoteRtpAddress
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the optional remote RTCP address.
				///	</summary>
				property String^ RemoteRtcpAddress
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the outgoing codec payload type.
				///	</summary>
				property unsigned TxPayloadType
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the incoming codec payload type.
				///	</summary>
				property unsigned RxPayloadType
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the codec name.
				///	</summary>
				property String^ CodecName
				{
					String^ get();
					void set(String^ value);
				}

				///	<summary>
				///	Gets or sets the codec clock rate.
				///	</summary>
				property unsigned CodecClockRate
				{
					unsigned get();
					void set(unsigned value);
				}

			private:
				bool _disposed;

				MediaType _type;
				MediaTransportProtocol _transportProtocol;
				MediaDirection _direction;
				String^ _remoteRtpAddress;
				String^ _remoteRtcpAddress;
				unsigned _txPayloadType;
				unsigned _rxPayloadType;
				String^ _codecName;
				unsigned _codecClockRate;
			};
		}
	}
}
#endif