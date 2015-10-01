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

enum
{
    TRACE_LEVEL_LOW, 
    TRACE_LEVEL_NORMAL, 
    TRACE_LEVEL_HIGH,
};

extern DWORD g_dwTraceLevel;
void Trace(DWORD dwLevel, LPCWSTR pszFormat, ...);
void TraceError(LPCSTR pszFile, long nLine, LPCSTR pszFunc, const void *pThis, HRESULT hr);
void TraceError(LPCSTR pszFile, long nLine, LPCSTR pszFunc, Object ^pThis, HRESULT hr);
void PrintSampleInfo(IMFSample *pSample);
void PrintAttributes(IMFAttributes *pAttributes);

#if 0
#define TRACE Trace

#define TRACEHR(hrVal) \
{ \
    __if_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, this, (hrVal)); } \
    __if_not_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, NULL, (hrVal)); } \
}

#define TRACEHR_RET(hrVal) \
{ \
    __if_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, this, (hrVal)); } \
    __if_not_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, NULL, (hrVal)); } \
    return hrVal; \
}

#define ThrowIfError(hrVal) \
{ \
    HRESULT hrInternal = (hrVal); \
    if (FAILED(hrInternal)) \
    { \
        __if_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, this, (hrInternal)); } \
        __if_not_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, NULL, (hrInternal)); } \
        throw ref new Exception(hrInternal); \
    } \
}

#define Throw(hrVal) \
{ \
    HRESULT hrInternal = (hrVal); \
    assert(FAILED(hrInternal)); \
    __if_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, this, (hrInternal)); } \
    __if_not_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, NULL, (hrInternal)); } \
    throw ref new Exception(hrInternal); \
}

#else
#define TRACE
#define TRACEHR(hrVal)
#define TRACEHR_RET(hrVal) {return hrVal;}
inline void ThrowIfError(HRESULT hr)
{
    if (FAILED(hr))
    {
        throw ref new Exception(hr);
    }
}

inline void Throw(HRESULT hr)
{
    assert(FAILED(hr));
    throw ref new Exception(hr);
}
#endif
