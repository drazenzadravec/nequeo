/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          NetRequestContent.h
*  Purpose :       Http net request content class.
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

namespace Nequeo {
	namespace Net {
		namespace Http
		{
			/// <summary>
			/// Http net request content.
			/// </summary>
			class EXPORT_NEQUEO_NET_BOOST_SERVER_API NetRequestContent
			{
			public:
				/// <summary>
				/// Http net request content.
				/// </summary>
				NetRequestContent() : _protocolVersion("HTTP/1.1"), _method("GET"), _path("/") {}

				/// <summary>
				/// Http net request content.
				/// </summary>
				~NetRequestContent() {}

				/// <summary>
				/// Get the accept encoding.
				/// </summary>
				/// <return>The accept encoding.</return>
				inline const std::string& GetAcceptEncoding() const
				{
					return _acceptEncoding;
				}

				/// <summary>
				/// Set the accept encoding.
				/// </summary>
				/// <param name="acceptEncoding">The accept encoding.</param>
				inline void SetAcceptEncoding(const std::string& acceptEncoding)
				{
					_acceptEncoding = acceptEncoding;
				}

				/// <summary>
				/// Get the content type.
				/// </summary>
				/// <return>The content type.</return>
				inline const std::string& GetContentType() const
				{
					return _contentType;
				}

				/// <summary>
				/// Set the content type.
				/// </summary>
				/// <param name="contentType">The content type.</param>
				inline void SetContentType(const std::string& contentType)
				{
					_contentType = contentType;
				}

				/// <summary>
				/// Get the content length.
				/// </summary>
				/// <return>The content length.</return>
				inline unsigned long long GetContentLength() const
				{
					return _contentLength;
				}

				/// <summary>
				/// Set the content length.
				/// </summary>
				/// <param name="contentLength">The content length.</param>
				inline void SetContentLength(unsigned long long contentLength)
				{
					_contentLength = contentLength;
				}

				/// <summary>
				/// Get the protocol version.
				/// </summary>
				/// <return>The protocol version.</return>
				inline const std::string& GetProtocolVersion() const
				{
					return _protocolVersion;
				}

				/// <summary>
				/// Set the protocol version.
				/// </summary>
				/// <param name="protocolVersion">The protocol version.</param>
				inline void SetProtocolVersion(std::string protocolVersion)
				{
					_protocolVersion = protocolVersion;
				}

				/// <summary>
				/// Get the method.
				/// </summary>
				/// <return>The method.</return>
				inline const std::string& GetMethod() const
				{
					return _method;
				}

				/// <summary>
				/// Set the method.
				/// </summary>
				/// <param name="method">The method.</param>
				inline void SetMethod(const std::string& method)
				{
					_method = method;
				}

				/// <summary>
				/// Get the path.
				/// </summary>
				/// <return>The path.</return>
				inline const std::string& GetPath() const
				{
					return _path;
				}

				/// <summary>
				/// Set the path.
				/// </summary>
				/// <param name="path">The path.</param>
				inline void SetPath(const std::string& path)
				{
					_path = path;
				}

				/// <summary>
				/// Get the host.
				/// </summary>
				/// <return>The host.</return>
				inline const std::string& GetHost() const
				{
					return _host;
				}

				/// <summary>
				/// Set the host.
				/// </summary>
				/// <param name="host">The host.</param>
				inline void SetHost(const std::string& host)
				{
					_host = host;
				}

				///	<summary>
				///	Add a new header or assign the value to existing key.
				///	</summary>
				/// <param name="key">The header key.</param>
				/// <param name="value">The header value.</param>
				void AddHeader(const std::string& key, const std::string& value);

				/// <summary>
				/// Get the headers.
				/// </summary>
				/// <return>The headers.</return>
				inline std::map<std::string, std::string>& GetHeaders()
				{
					return _headers;
				}

			private:
				std::map<std::string, std::string> _headers;
				std::string _protocolVersion;
				unsigned long long _contentLength;
				std::string _contentType;
				std::string _acceptEncoding;
				std::string _method;
				std::string _path;
				std::string _host;
			};
		}
	}
}