/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          WebDispatch.cpp
*  Purpose :       WebDispatch class.
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

#include "stdafx.h"

#include "WebDispatch.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Initializes the InternetExplorer object.
			/// </summary>
			/// <param name="pCallback">The dispatch callback.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT WebDispatch::Init(DispatchCallback *pCallback)
			{
				_pDispatchCB = pCallback;

				// Container.
				IConnectionPointContainer *pCPContainer = NULL;
				HWND hwndBrowser = NULL;

				// Create the InternetExplorer object.
				HRESULT hr = CoCreateInstance(CLSID_InternetExplorer, NULL,
					CLSCTX_ALL, IID_IWebBrowser2, (void**)&_pBrowser);

				// If successful created object.
				if (SUCCEEDED(hr))
				{
					// Set up the connection point so that we receive events.
					hr = _pBrowser->QueryInterface(IID_IConnectionPointContainer, (void**)&pCPContainer);
				}

				// If successful query interface.
				if (SUCCEEDED(hr))
				{
					// Find connection point.
					hr = pCPContainer->FindConnectionPoint(DIID_DWebBrowserEvents2, &_pCP);
				}

				// If successful connection point.
				if (SUCCEEDED(hr))
				{
					// Get cookie info.
					hr = _pCP->Advise(this, &_dwCookie);
				}

				// If successful advise.
				if (SUCCEEDED(hr))
				{
#ifdef _WIN64
					// Get the browser handler.
					hr = _pBrowser->get_HWND((__int64*)&hwndBrowser);
#else
					// Get the browser handler.
					hr = _pBrowser->get_HWND((long*)&hwndBrowser);
#endif
				}

				// If successful advise.
				if (SUCCEEDED(hr))
				{
					// Move the browser window to the front.
					SetWindowPos(hwndBrowser, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
				}

				// Clean up.
				SAFE_RELEASE(pCPContainer);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Cleans up.
			/// </summary>
			void WebDispatch::Exit()
			{
				// Release the connection point. 
				if (_pCP)
				{
					// Unadvice the browser of connection with the specified cookie hander.
					_pCP->Unadvise(_dwCookie);
				}

				// Clean up.
				SAFE_RELEASE(_pBrowser);
				SAFE_RELEASE(_pCP);

				_pDispatchCB = NULL;
			}

			/// <summary>
			/// Navigates to a URL and POSTs the license acquisition data.
			/// </summary>
			/// <param name="pURL">The license acquisition URL.</param>
			/// <param name="pPostData">The license acquisition data.</param>
			/// <param name="cbData">The size of the data, in bytes..</param>
			/// <returns>The result of the operation.</returns>
			HRESULT WebDispatch::OpenURLWithData(const WCHAR *wszURL, const BYTE *pbPostData, DWORD cbData)
			{
				// This string is the header needed for HTTP POST actions.
				const LPWSTR POST_HEADER_DATA = L"Content-Type: application/x-www-form-urlencoded\r\n";

				if (!wszURL)
				{
					return E_INVALIDARG;
				}

				if (!_pBrowser)
				{
					return E_UNEXPECTED;
				}

				HRESULT hr = S_OK;
				BSTR    bstrURL = NULL;
				VARIANT vtEmpty;
				VARIANT vtHeader;
				VARIANT vtPostData;

				VariantInit(&vtEmpty);
				VariantInit(&vtHeader);
				VariantInit(&vtPostData);

				// Allocate a BSTR for the URL.
				bstrURL = SysAllocString(wszURL);
				if (bstrURL == NULL)
				{
					hr = E_OUTOFMEMORY;
				}

				// Allocate a BSTR for the header.
				if (SUCCEEDED(hr))
				{
					vtHeader.bstrVal = SysAllocString(POST_HEADER_DATA);
					if (vtHeader.bstrVal == NULL)
					{
						hr = E_OUTOFMEMORY;
					}
					else
					{
						vtHeader.vt = VT_BSTR;
					}
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					if (pbPostData)
					{
						// Convert the POST data to a safe array in a variant. The safe array type is VT_UI1.

						void *pvData = NULL;
						SAFEARRAY *saPostData = SafeArrayCreateVector(VT_UI1, 0, cbData);
						if (saPostData == NULL)
						{
							hr = E_OUTOFMEMORY;
						}

						if (SUCCEEDED(hr))
						{
							hr = SafeArrayAccessData(saPostData, &pvData);
						}

						if (SUCCEEDED(hr))
						{
							CopyMemory((BYTE*)pvData, pbPostData, cbData);
							hr = SafeArrayUnaccessData(saPostData);
						}

						if (SUCCEEDED(hr))
						{
							vtPostData.vt = VT_ARRAY | VT_UI1;
							vtPostData.parray = saPostData;
						}
					}
				}

				// Make the IE window visible.
				if (SUCCEEDED(hr))
				{
					hr = _pBrowser->put_Visible(VARIANT_TRUE);
				}

				// Navigate to the URL.
				if (SUCCEEDED(hr))
				{
					hr = _pBrowser->Navigate(bstrURL, &vtEmpty, &vtEmpty, &vtPostData, &vtHeader);
				}

				SysFreeString(bstrURL);

				VariantClear(&vtEmpty);
				VariantClear(&vtHeader);
				VariantClear(&vtPostData);

				// Return the result.
				return hr;
			}
		}
	}
}