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

#include "pch.h"
#include "OAuth2.h"
#include "OAuth.h"
#include <agents.h>
#include <winerror.h>
#include <ppltasks.h>
#include <stdlib.h>

#include <string>
#include <sstream>
#include <vector>

using namespace Concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Web::Http;
using namespace Windows::Web::Http::Filters;


#include <collection.h>
#include <algorithm>
using namespace Platform;
using namespace Platform::Collections;

using namespace Windows::Security::Authentication::Web;
using namespace Windows::Security::Cryptography;

using namespace Nequeo::Auth;

//
// OAuth steps:
// GetRequestToken
//		- need nonce + timestamp + sign
//		+ PostAsync to "OauthTokenRequestUri"
//
// + ReadAsStringAsync + parse as form url encoded
//		= RequestToken + RequestTokenSecret
//
// Authenticate
//		+ WAB Async
//	get AccessToken from WAB results
//
// GetAccessToken
//		- neeed nonce + timestamp + sign
//		+ PostAsyn to the "OauthAccessTokenUri"
//
// + ReadAsString + parse as form url encoded
//		= oauth_token + oauth_token_secret
// SignHeader+SendRequest
//
OAuthFilter::OAuthFilter(IHttpFilter^ innerFilter)
{
	if (innerFilter == nullptr)
	{
		throw ref new Exception(E_INVALIDARG, "innerFilter cannot be null.");
	}
	m_InnerFilter = innerFilter;
	m_RoamingSettings = Windows::Storage::ApplicationData::Current->RoamingSettings;
}

OAuthFilter::~OAuthFilter()
{
}


void OAuthFilter::Clear()
{
	m_AccessToken = nullptr;

	try
	{
		m_RoamingSettings->Values->Remove(AuthConfiguration.TechnicalName+"access_token");
	}
	catch (Exception^)
	{
		// API throws where there is successfully nothing found.
	}
}


static Uri^ AppendQuery(Uri^ startingUri, Platform::String^ name, Platform::String^ value)
{
	Platform::String^ queryToAppend = (value == "") ? name : name + "=" + value;
	bool alreadyHasQuery = startingUri->Query != "";
	Platform::String^ queryChar = alreadyHasQuery ? "&" : "?";
	Platform::String^ newUriString = startingUri->AbsoluteUri + queryChar + queryToAppend;
	return ref new Uri(newUriString);
}

Platform::String^ OAuthFilter::GetNonce()
{
	auto rnd = CryptographicBuffer::GenerateRandomNumber() % 1000000000;
	wchar_t buffer[200];
	_itow_s(rnd, buffer, 200, 10);
	return ref new Platform::String(buffer);
}

Platform::String^ OAuthFilter::GetTimeStamp() 
{
	SYSTEMTIME nowSystemTime;
	GetSystemTime(&nowSystemTime);
	FILETIME nowFileTime;
	SystemTimeToFileTime(&nowSystemTime, &nowFileTime);
	ULARGE_INTEGER nowULarge;
	nowULarge.HighPart = nowFileTime.dwHighDateTime;
	nowULarge.LowPart = nowFileTime.dwLowDateTime;

	SYSTEMTIME startSystemTime;
	startSystemTime.wYear = 1970;
	startSystemTime.wMonth = 1; // Januaray
	startSystemTime.wDay = 1; // the first
	startSystemTime.wHour = 0;
	startSystemTime.wMinute = 0;
	startSystemTime.wSecond = 0;
	startSystemTime.wMilliseconds = 0;
	FILETIME startFileTime;
	SystemTimeToFileTime(&startSystemTime, &startFileTime);
	ULARGE_INTEGER startULarge;
	startULarge.HighPart = startFileTime.dwHighDateTime;
	startULarge.LowPart = startFileTime.dwLowDateTime;

	ULONGLONG delta100NS = nowULarge.QuadPart - startULarge.QuadPart;
	ULONGLONG convert100nsToSeconds = (10 * 1000 * 1000); // 100 ns*10=usec * 1000 = milliseconds * 1000=seconds
	ULONGLONG deltaSeconds = delta100NS / convert100nsToSeconds;

	wchar_t buffer[200];
	_itow_s((int) deltaSeconds, buffer, 200, 10);
	return ref new Platform::String(buffer);
}

IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ OAuthFilter::SendRequestAsync(
	HttpRequestMessage^ request
	)
{
	return create_async([=](progress_reporter<HttpProgress> reporter, cancellation_token token)
	{
		auto nonce = GetNonce();
		auto timestamp = GetTimeStamp();
		//std::map 
		auto content = ref new Map<Platform::String^, Platform::String^>();
		content->Insert(ref new Platform::String(L"oauth_consumer_key"), AuthConfiguration.ClientId);
		content->Insert(ref new Platform::String(L"oauth_nonce"), nonce);


		auto postContent = ref new HttpFormUrlEncodedContent(content);
		auto postUri = ref new Windows::Foundation::Uri(m_requestTokenUri);
		auto postRequest = ref new HttpRequestMessage(HttpMethod::Post, postUri);
		postRequest->Content = postContent;

		IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ operation = m_InnerFilter->SendRequestAsync(request);
		return create_task(operation, token).then([=](HttpResponseMessage^ response)
		{
			return create_task(response->Content->ReadAsStringAsync());
		}).then([=](Platform::String^ response)
		{
			IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ operation = m_InnerFilter->SendRequestAsync(request);
			return create_task(operation, token);
		});
	});
}


