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
#include <ppltasks.h>
#include <map>
#include "OAuth2.h"

namespace Nequeo {
	namespace Auth
	{

		// SwitchableAuthFilter picks one (or none) from a set of OAuth2Filters based on matching
		// the incoming request Uri against a template in the Oauth2Filter->AuthConfiguration.ApiUriPrefix
		// 
		// This way, the SwitchableAuthFilter can be pre-populated with a set of filters which can
		// authorize against different web services.  App requests to different web services will
		// automatically be routed to the correct sub-filter.
		//
		// Note that every time the SendRequest is called, the sub-filter's InnerFilter will be re-set
		// to the current SwitchableAuthFilter's innerFilter.
		public ref class SwitchableAuthFilter sealed : public Windows::Web::Http::Filters::IHttpFilter, ISetInnerFilter
		{
		public:
			SwitchableAuthFilter(Windows::Web::Http::Filters::IHttpFilter^ innerFilter);
			virtual ~SwitchableAuthFilter();
			virtual Windows::Foundation::IAsyncOperationWithProgress <
				Windows::Web::Http::HttpResponseMessage^,
				Windows::Web::Http::HttpProgress > ^ SendRequestAsync(Windows::Web::Http::HttpRequestMessage^ request);
			void AddOAuthFilter(Nequeo::Auth::OAuthFilter^ newFilter);
			void AddOAuth2Filter(Nequeo::Auth::OAuth2Filter^ newFilter);
			void AddFilter(Platform::String^ uriToMatch, Windows::Web::Http::Filters::IHttpFilter^ newFilter);
			void ClearAll();

		private:
			Windows::Web::Http::Filters::IHttpFilter^ m_InnerFilter;
		public:
			virtual Windows::Web::Http::Filters::IHttpFilter^ SetInnerFilter(Windows::Web::Http::Filters::IHttpFilter^ newValue)
			{
				auto ReturnValue = m_InnerFilter;
				m_InnerFilter = newValue;
				return ReturnValue;
			}

		private:
			//std::vector<AuthFilters::OAuth2Filter^> m_FilterVector;
			std::map<Platform::String^, Windows::Web::Http::Filters::IHttpFilter^> m_FilterMap;

		};
	}
}