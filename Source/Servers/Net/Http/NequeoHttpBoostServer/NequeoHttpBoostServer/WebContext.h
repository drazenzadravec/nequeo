/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          WebContext.h
*  Purpose :       Http web context class.
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
#include "WebRequest.h"
#include "WebResponse.h"

namespace Nequeo {
	namespace Net {
		namespace Http
		{
			/// <summary>
			/// Http web context.
			/// </summary>
			class EXPORT_NEQUEO_NET_BOOST_SERVER_API WebContext
			{
			public:
				/// <summary>
				/// Http web context.
				/// </summary>
				/// <param name="request">The web request.</param>
				/// <param name="response">The web response.</param>
				WebContext(std::shared_ptr<WebRequest>& request, std::shared_ptr<WebResponse>& response);

				/// <summary>
				/// Http web context.
				/// </summary>
				~WebContext();

				/// <summary>
				/// Get the web request.
				/// </summary>
				/// <return>The web request.</return>
				WebRequest& Request();

				/// <summary>
				/// Get the web response.
				/// </summary>
				/// <return>The web response.</return>
				WebResponse& Response();

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
				/// Get the server name.
				/// </summary>
				/// <return>The servername.</return>
				std::string GetServerName() const;

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
				/// Set server name.
				/// </summary>
				/// <param name="serverName">Server name.</param>
				inline void SetServerName(std::string serverName)
				{
					_serverName = serverName;
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
				std::string _serverName;

				std::shared_ptr<WebRequest> _request;
				std::shared_ptr<WebResponse> _response;
			};
		}
	}
}