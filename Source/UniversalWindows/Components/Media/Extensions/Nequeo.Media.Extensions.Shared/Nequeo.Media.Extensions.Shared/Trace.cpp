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
#include "Trace.h"

DWORD g_dwLogLevel = TRACE_LEVEL_NORMAL;

void Trace(DWORD dwLevel, LPCWSTR pszFormat, ...)
{
    if (g_dwLogLevel > dwLevel)
    {
        return;
    }
    WCHAR szTextBuf[256];
    va_list args;
    va_start(args, pszFormat);

    StringCchVPrintf(szTextBuf, _countof(szTextBuf), pszFormat, args);

    OutputDebugString(szTextBuf);
}

void TraceError(LPCSTR pszFile, long nLine, LPCSTR pszFunc, Object ^pThis, HRESULT hr)
{
    TraceError(pszFile, nLine, pszFunc, reinterpret_cast<IInspectable*>(pThis), hr);
}

void TraceError(LPCSTR pszFile, long nLine, LPCSTR pszFunc, const void * pThis, HRESULT hr)
{
    if (SUCCEEDED(hr))
    {
        return;
    }
    LPCSTR psz = pszFile + strlen(pszFile);
    for(;psz > pszFile; psz--)
    {
        if(*psz == '\\')
        {
            pszFile = psz+1;
            break;
        }             
    }
    WCHAR szTextBuf[256];
    szTextBuf[0] = 0;
    StringCchPrintf(szTextBuf, _countof(szTextBuf), L"%S:%d (%S@%p) FAILED hr=%x\n", pszFile, nLine, pszFunc, pThis, hr);
    OutputDebugString(szTextBuf);
}

void PrintSampleInfo(IMFSample *pSample)
{
    ComPtr<IMFMediaBuffer> spBuffer;
    pSample->GetBufferByIndex(0, &spBuffer);

    DWORD cbTotalLen;
    DWORD cbMaxLen;
    DWORD cbCurLen;
    LONGLONG llDur;
    LONGLONG llTs;
    pSample->GetTotalLength(&cbTotalLen);
    pSample->GetSampleDuration(&llDur);
    pSample->GetSampleTime(&llTs);
    
    Trace(TRACE_LEVEL_HIGH, L"Sample - Ts: %I64d Dur: %I64d Len: %d\n", llTs, llDur, cbTotalLen);
    BYTE *pByte;
    spBuffer->Lock(&pByte, &cbMaxLen, &cbCurLen);
    if (cbCurLen >= sizeof(LONGLONG))
    {
        Trace(TRACE_LEVEL_HIGH, L"Sample beginning - Ts: 0x%I64x\n", *(LONGLONG*)pByte);
    }

    PrintAttributes(pSample);
}

void PrintAttributes(IMFAttributes *pAttributes)
{
    UINT32 cAttr;
    pAttributes->GetCount(&cAttr);
    Trace(TRACE_LEVEL_HIGH, L"No attributes %d\n", cAttr);

    for (UINT32 i = 0; i < cAttr; ++i)
    {
        const DWORD CHARS_IN_GUID = 39;
        PROPVARIANT var = {0};
        GUID gui;
        OLECHAR szAttrId[CHARS_IN_GUID];

        pAttributes->GetItemByIndex(i, &gui, &var);
        StringFromGUID2(gui, szAttrId, CHARS_IN_GUID);

        switch (var.vt)
        {
        case MF_ATTRIBUTE_UINT32:
            Trace(TRACE_LEVEL_HIGH, L"Attr: %s - %d\n", szAttrId, var.uintVal);
            break;
        case MF_ATTRIBUTE_UINT64:
            Trace(TRACE_LEVEL_HIGH, L"Attr: %s - %I64d\n", szAttrId, var.uhVal.QuadPart);
            break;
        case MF_ATTRIBUTE_DOUBLE:
            Trace(TRACE_LEVEL_HIGH, L"Attr: %s - %f\n", szAttrId, var.dblVal);
            break;
        case MF_ATTRIBUTE_GUID:
            OLECHAR szAttrVal[CHARS_IN_GUID];
            StringFromGUID2(*var.puuid, szAttrVal, CHARS_IN_GUID);
            Trace(TRACE_LEVEL_HIGH, L"Attr: %s - %s\n", szAttrId, szAttrVal);
            break;
        case MF_ATTRIBUTE_BLOB:
            if (var.caub.cElems >= 8)
            {
                Trace(TRACE_LEVEL_HIGH, L"Attr: %s BLOB - %I64x\n", szAttrId, *(LONGLONG*)var.caub.pElems);
            }
            else if (var.caub.cElems > 0)
            {
                Trace(TRACE_LEVEL_HIGH, L"Attr: %s BLOB - %c\n", szAttrId, var.caub.pElems[0]);
            }
            else 
            {
                Trace(TRACE_LEVEL_HIGH, L"Attr: %s empty blob\n", szAttrId, var.caub.pElems[0]);
            }
            break;
        case MF_ATTRIBUTE_STRING:
            Trace(TRACE_LEVEL_HIGH, L"Attr: %s %s\n", szAttrId, var.pwszVal);
            break;
        default:
            Trace(TRACE_LEVEL_HIGH, L"Attr: %s TYPE %hd\n", szAttrId, var.vt);
            break;
        }

        PropVariantClear(&var);
    }
}
