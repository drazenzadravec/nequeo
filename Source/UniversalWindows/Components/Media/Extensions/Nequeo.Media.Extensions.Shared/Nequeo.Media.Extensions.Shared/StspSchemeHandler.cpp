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
#include <InitGuid.h>
#include "StspSchemeHandler.h"
#include "StspMediaSource.h"
#include <wrl/module.h>

using namespace Nequeo::Media::Communication;
using namespace Nequeo::Media::Communication::Network;

ActivatableClass(CSchemeHandler);

CSchemeHandler::CSchemeHandler(void)
{
}

CSchemeHandler::~CSchemeHandler(void)
{
}

// IMediaExtension methods
IFACEMETHODIMP CSchemeHandler::SetProperties (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    return S_OK;
}

// IMFSchemeHandler methods
IFACEMETHODIMP CSchemeHandler::BeginCreateObject( 
        _In_ LPCWSTR pwszURL,
        _In_ DWORD dwFlags,
        _In_ IPropertyStore *pProps,
        _COM_Outptr_opt_  IUnknown **ppIUnknownCancelCookie,
        _In_ IMFAsyncCallback *pCallback,
        _In_ IUnknown *punkState)
{
    HRESULT hr = S_OK;
    ComPtr<CMediaSource> spSource;

    try
    {
        if (ppIUnknownCancelCookie != nullptr)
        {
            *ppIUnknownCancelCookie = nullptr;
        }

        if (pwszURL == nullptr || pCallback == nullptr)
        {
            Throw(E_INVALIDARG);
        }

        if ((dwFlags & MF_RESOLUTION_MEDIASOURCE) == 0)
        {
            Throw(E_INVALIDARG);
        }

        ComPtr<IMFAsyncResult> spResult;
        ThrowIfError(CMediaSource::CreateInstance(&spSource));

        ComPtr<IUnknown> spSourceUnk;
        ThrowIfError(spSource.As(&spSourceUnk));
        ThrowIfError(MFCreateAsyncResult(spSourceUnk.Get(), pCallback, punkState, &spResult));

        ComPtr<CSchemeHandler> spThis = this;
        spSource->OpenAsync(pwszURL).then([this, spThis, spResult, spSource](concurrency::task<void>& openTask)
        {
            try
            {
                if (spResult == nullptr)
                {
                    ThrowIfError(MF_E_UNEXPECTED);
                }

                openTask.get();
            }
            catch(Exception ^exc)
            {
                if (spResult != nullptr)
                {
                    spResult->SetStatus(exc->HResult);
                }
            }

            if (spResult != nullptr)
            {
                MFInvokeCallback(spResult.Get());
            }
        });
    }
    catch(Exception ^exc)
    {
        if (spSource != nullptr)
        {
            spSource->Shutdown();
        }
        hr = exc->HResult;
    }

    TRACEHR_RET(hr);
}
        
IFACEMETHODIMP CSchemeHandler::EndCreateObject( 
        _In_ IMFAsyncResult *pResult,
        _Out_  MF_OBJECT_TYPE *pObjectType,
        _Out_  IUnknown **ppObject)
{
    if (pResult == nullptr || pObjectType == nullptr || ppObject == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = pResult->GetStatus();
    *pObjectType = MF_OBJECT_INVALID;
    *ppObject = nullptr;

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> punkSource;
        hr = pResult->GetObject(&punkSource);
        if (SUCCEEDED(hr))
        {
            *pObjectType = MF_OBJECT_MEDIASOURCE;
            *ppObject = punkSource.Detach();
        }
    }

    TRACEHR_RET(hr);
}
        
IFACEMETHODIMP CSchemeHandler::CancelObjectCreation( 
            _In_ IUnknown *pIUnknownCancelCookie)
{
    return E_NOTIMPL;
}
