/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :
*  Purpose :
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
#include "OAuth2.h"
#include <ppltasks.h>

namespace Nequeo {
	namespace Auth
	{

		public ref class OAuthFilter sealed : public Windows::Web::Http::Filters::IHttpFilter, ISetInnerFilter
		{
		public:
			OAuthFilter(Windows::Web::Http::Filters::IHttpFilter^ innerFilter);
			virtual ~OAuthFilter();
			virtual Windows::Foundation::IAsyncOperationWithProgress <
				Windows::Web::Http::HttpResponseMessage^,
				Windows::Web::Http::HttpProgress > ^ SendRequestAsync(Windows::Web::Http::HttpRequestMessage^ request);

		private:
			AuthConfigurationData m_AuthConfigurationData;
			Platform::String^ m_requestTokenUri;

		public:
			property AuthConfigurationData AuthConfiguration
			{
				AuthConfigurationData get() { return m_AuthConfigurationData; }
				void set(AuthConfigurationData data) { m_AuthConfigurationData = data; }
			}

			property Platform::String^ RequestTokenUri
			{
				Platform::String^ get() { return m_requestTokenUri; }
				void set(Platform::String^ data) { m_requestTokenUri = data; }
			}
		private:
			Windows::Web::Http::Filters::IHttpFilter^ m_InnerFilter;

		public:
			virtual Windows::Web::Http::Filters::IHttpFilter^ SetInnerFilter(Windows::Web::Http::Filters::IHttpFilter^ newValue)
			{
				auto ReturnValue = m_InnerFilter;
				m_InnerFilter = newValue;
				return ReturnValue;
			}


		public:
			void Clear(); // Clears the login information i.e. the AccessToken.


		private:
			Platform::String^ m_RequestToken;
			Platform::String^ m_RequestSecret;
			Platform::String^ m_AccessToken;
			Platform::String^ m_OAuthToken;
			Platform::String^ m_OAuthTokenSecret;
			Windows::Storage::ApplicationDataContainer^ m_RoamingSettings;

			Platform::String^ GetNonce();
			Platform::String^ GetTimeStamp();

			/*
			virtual Windows::Foundation::IAsyncOperationWithProgress <
				Windows::Web::Http::HttpResponseMessage^,
				Windows::Web::Http::HttpProgress > ^ SendRequestWithAccessTokenAsync(Windows::Web::Http::HttpRequestMessage^ request);


			virtual Windows::Foundation::IAsyncOperationWithProgress <
				Windows::Web::Http::HttpResponseMessage^,
				Windows::Web::Http::HttpProgress > ^ CallWabAndSendRequestWithAccessTokenAsync(Windows::Web::Http::HttpRequestMessage^ request);
			*/
		};
	}
}