/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NetResponse.h
*  Purpose :       WebSocket net response class.
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

#include "stdafx.h"
#include "Global.h"

#include "NetResponseContent.h"

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// WebSocket net response.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKET_BOOST_SERVER_API NetResponse : public NetResponseContent
			{
			public:
				/// <summary>
				/// WebSocket net response.
				/// </summary>
				NetResponse();

				/// <summary>
				/// WebSocket net response.
				/// </summary>
				~NetResponse();

				/// <summary>
				/// Get the remote endpoint address.
				/// </summary>
				/// <return>The remote endpoint address.</return>
				inline const std::string& GetRemoteEndpointAddress() const
				{
					return _remoteEndpointAddress;
				}

				/// <summary>
				/// Set the remote endpoint address.
				/// </summary>
				/// <param name="remoteEndpointAddress">The remote endpoint address.</param>
				inline void SetRemoteEndpointAddress(const std::string& remoteEndpointAddress)
				{
					_remoteEndpointAddress = remoteEndpointAddress;
				}

				/// <summary>
				/// Get the remote endpoint port.
				/// </summary>
				/// <return>The remote endpoint port.</return>
				inline unsigned short GetRemoteEndpointPort() const
				{
					return _remoteEndpointPort;
				}

				/// <summary>
				/// Set the remote endpoint port.
				/// </summary>
				/// <param name="remoteEndpointPort">The remote endpoint port.</param>
				inline void SetRemoteEndpointPort(unsigned short remoteEndpointPort)
				{
					_remoteEndpointPort = remoteEndpointPort;
				}

			private:
				bool _disposed;

				std::string _remoteEndpointAddress;
				unsigned short _remoteEndpointPort;
			};
		}
	}
}