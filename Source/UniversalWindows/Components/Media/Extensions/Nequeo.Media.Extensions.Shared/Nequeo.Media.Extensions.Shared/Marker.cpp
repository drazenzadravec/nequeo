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

#include "Marker.h"

using namespace Nequeo::Media::Communication;

HRESULT Nequeo::Media::Communication::CreateMarker(    
    MFSTREAMSINK_MARKER_TYPE eMarkerType,
    const PROPVARIANT *pvarMarkerValue,     // Can be NULL.
    const PROPVARIANT *pvarContextValue,    // Can be NULL.
    IMarker **ppMarker
    )
{
    return CMarker::Create(eMarkerType, pvarMarkerValue, pvarContextValue, ppMarker);
}

CMarker::CMarker(MFSTREAMSINK_MARKER_TYPE eMarkerType) : _cRef(1), _eMarkerType(eMarkerType)
{
    ZeroMemory(&_varMarkerValue, sizeof(_varMarkerValue));
    ZeroMemory(&_varContextValue, sizeof(_varContextValue));
}

CMarker::~CMarker()
{
    assert(_cRef == 0);

    PropVariantClear(&_varMarkerValue);
    PropVariantClear(&_varContextValue);
}

/* static */
HRESULT CMarker::Create(
    MFSTREAMSINK_MARKER_TYPE eMarkerType,
    const PROPVARIANT *pvarMarkerValue,     // Can be NULL.
    const PROPVARIANT *pvarContextValue,    // Can be NULL.
    IMarker **ppMarker
    )
{
    if (ppMarker == nullptr)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    ComPtr<CMarker> spMarker;
        
    spMarker.Attach(new (std::nothrow) CMarker(eMarkerType));

    if (spMarker == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    // Copy the marker data.
    if (SUCCEEDED(hr))
    {
        if (pvarMarkerValue)
        {
            hr = PropVariantCopy(&spMarker->_varMarkerValue, pvarMarkerValue);
        }
    }

    if (SUCCEEDED(hr))
    {
        if (pvarContextValue)
        {
            hr = PropVariantCopy(&spMarker->_varContextValue, pvarContextValue);
        }
    }

    if (SUCCEEDED(hr))
    {
        *ppMarker = spMarker.Detach();
    }

    return hr;
}

// IUnknown methods.

IFACEMETHODIMP_(ULONG) CMarker::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CMarker::Release()
{
    ULONG cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }

    return cRef;
}

IFACEMETHODIMP CMarker::QueryInterface(REFIID riid, void **ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(IMarker))
    {
        (*ppv) = static_cast<IMarker*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

// IMarker methods
IFACEMETHODIMP CMarker::GetMarkerType(MFSTREAMSINK_MARKER_TYPE *pType)
{
    if (pType == NULL)
    {
        return E_POINTER;
    }

    *pType = _eMarkerType;
    return S_OK;
}

IFACEMETHODIMP CMarker::GetMarkerValue(PROPVARIANT *pvar)
{
    if (pvar == NULL)
    {
        return E_POINTER;
    }
    return PropVariantCopy(pvar, &_varMarkerValue);

}
IFACEMETHODIMP CMarker::GetContext(PROPVARIANT *pvar)
{
    if (pvar == NULL)
    {
        return E_POINTER;
    }
    return PropVariantCopy(pvar, &_varContextValue);
}
