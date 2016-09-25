/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MultiServerContainer.h
*  Purpose :       MultiServerContainer.
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

namespace Nequeo {
	namespace Net {
		namespace Http
		{
			/// <summary>
			/// Multi server container.
			/// </summary>
			class EXPORT_NEQUEO_NET_BOOST_SERVER_API MultiServerContainer
			{
			public:
				/// <summary>
				/// Multi server container.
				/// </summary>
				MultiServerContainer() {}

				/// <summary>
				/// Multi server container.
				/// </summary>
				virtual ~MultiServerContainer() {}

				/// <summary>
				/// Get the port.
				/// </summary>
				/// <return>The port.</return>
				inline unsigned short GetPort() const
				{
					return _port;
				}

				/// <summary>
				/// Set the port.
				/// </summary>
				/// <param name="port">The port.</param>
				inline void SetPort(unsigned short port)
				{
					_port = port;
				}

				/// <summary>
				/// Get the endpoint.
				/// </summary>
				/// <return>The endpoint.</return>
				inline const std::string& GetEndpoint() const
				{
					return _endpoint;
				}

				/// <summary>
				/// Set the endpoint.
				/// </summary>
				/// <param name="endpoint">The endpoint.</param>
				inline void SetEndpoint(const std::string& endpoint)
				{
					_endpoint = endpoint;
				}

				/// <summary>
				/// Get the ipv.
				/// </summary>
				/// <return>The ipv.</return>
				inline IPVersionType GetIPversion() const
				{
					return _ipv;
				}

				/// <summary>
				/// Set the ipv.
				/// </summary>
				/// <param name="ipv">The ipv.</param>
				inline void SetIPversion(IPVersionType ipv)
				{
					_ipv = ipv;
				}

			private:
				bool _disposed;
				unsigned short _port;
				IPVersionType _ipv;
				std::string _endpoint;
			};
		}
	}
}