/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NetContext.h
*  Purpose :       WebSocket net context class.
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

#include "IPVersionType.h"
#include "NetResponse.h"

#include "Threading\Executor.h"
#include "Threading\ThreadTask.h"

namespace Nequeo {
	namespace Net {
		namespace WebSocket
		{
			/// <summary>
			/// WebSocket net context.
			/// </summary>
			class EXPORT_NEQUEO_WEBSOCKET_BOOST_SERVER_API NetContext
			{
			public:
				/// <summary>
				/// WebSocket net context.
				/// </summary>
				NetContext();

				/// <summary>
				/// WebSocket net context.
				/// </summary>
				~NetContext();

				/// <summary>
				/// Get the net response.
				/// </summary>
				/// <return>The net response.</return>
				NetResponse& Response() const;

				/// <summary>
				/// Is the server secure.
				/// </summary>
				/// <return>True if the server is secure; else false.</return>
				bool IsSecure() const;

				/// <summary>
				/// The IP version type.
				/// </summary>
				/// <return>The IP version type.</return>
				IPVersionType IPVersion() const;

				/// <summary>
				/// Get the port number.
				/// </summary>
				/// <return>The port number.</return>
				unsigned short GetPort() const;

				/// <summary>
				/// Set is server secure.
				/// </summary>
				/// <param name="isSecure">Is secure.</param>
				inline void SetIsSecure(bool isSecure)
				{
					_isSecure = isSecure;
				}
				/// <summary>
				/// Set ip version.
				/// </summary>
				/// <param name="ipv">IP version.</param>
				inline void SetIPVersionType(IPVersionType ipv)
				{
					_ipv = ipv;
				}

				/// <summary>
				/// Set port.
				/// </summary>
				/// <param name="port">Port.</param>
				inline void SetPort(unsigned short port)
				{
					_port = port;
				}

			private:
				bool _disposed;
				bool _isSecure;

				unsigned short _port;
				IPVersionType _ipv;

				std::shared_ptr<NetResponse> _response;
			};
		}
	}
}