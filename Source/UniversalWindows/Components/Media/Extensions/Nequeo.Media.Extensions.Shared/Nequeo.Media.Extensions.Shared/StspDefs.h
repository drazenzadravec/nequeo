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
#ifndef DECLSPEC_UUID
#define DECLSPEC_UUID(x)    __declspec(uuid(x))
#endif

#ifndef DECLSPEC_NOVTABLE
#define DECLSPEC_NOVTABLE   __declspec(novtable)
#endif

namespace Nequeo { namespace Media { namespace Communication {

interface DECLSPEC_UUID("09CFFBBF-67AC-4AD6-8463-9CF82CA3E52F") DECLSPEC_NOVTABLE IStspSinkInternal : public IUnknown
{
    virtual void TriggerAcceptConnection(DWORD connectionId) = 0;
    virtual void TriggerRefuseConnection(DWORD connectionId) = 0;
};

interface DECLSPEC_UUID("3AC82233-933C-43a9-AF3D-ADC94EABF406") DECLSPEC_NOVTABLE IMarker : public IUnknown
{
    IFACEMETHOD (GetMarkerType) (MFSTREAMSINK_MARKER_TYPE *pType) = 0;
    IFACEMETHOD (GetMarkerValue) (PROPVARIANT *pvar) = 0;
    IFACEMETHOD (GetContext) (PROPVARIANT *pvar) = 0;
};

enum StspOperation
{
    StspOperation_Unknown,
    StspOperation_ClientRequestDescription,
    StspOperation_ClientRequestStart,
    StspOperation_ClientRequestStop,
    StspOperation_ServerDescription,
    StspOperation_ServerSample,
    StspOperation_ServerFormatChange,
    StspOperation_Last,
};

struct StspOperationHeader
{
    DWORD cbDataSize;
    StspOperation eOperation;
};

struct StspStreamDescription
{
    GUID guiMajorType;
    GUID guiSubType;
    DWORD dwStreamId;
    UINT32 cbAttributesSize;
};

struct StspDescription
{
    UINT32 cNumStreams;
    StspStreamDescription aStreams[1];
};

enum StspSampleFlags
{
    StspSampleFlag_BottomFieldFirst,
    StspSampleFlag_CleanPoint,
    StspSampleFlag_DerivedFromTopField,
    StspSampleFlag_Discontinuity,
    StspSampleFlag_Interlaced,
    StspSampleFlag_RepeatFirstField,
    StspSampleFlag_SingleField,
};

struct StspSampleHeader
{
    DWORD dwStreamId;
    LONGLONG ullTimestamp;
    LONGLONG ullDuration;
    DWORD dwFlags;
    DWORD dwFlagMasks;
};

enum StspNetworkType
{
    StspNetworkType_IPv4,
    StspNetworkType_IPv6,
};

// Possible states of the stsp source object
enum SourceState
{
    // Invalid state, source cannot be used 
    SourceState_Invalid,
    // Opening the connection
    SourceState_Opening,
    // Streaming started
    SourceState_Starting,
    // Streaming started
    SourceState_Started,
    // Streanung stopped
    SourceState_Stopped,
    // Source is shut down
    SourceState_Shutdown,
};

extern wchar_t const __declspec(selectany) c_szStspScheme[] = L"stsp";
extern wchar_t const __declspec(selectany) c_szStspSchemeWithColon[] = L"stsp:";
unsigned short const c_wStspDefaultPort = 10010;

void FilterOutputMediaType(IMFMediaType *pSourceMediaType, IMFMediaType *pDestinationMediaType);
void ValidateInputMediaType(REFGUID guidMajorType, REFGUID guidSubtype, IMFMediaType *pMediaType);
HRESULT CreateMarker(    
    MFSTREAMSINK_MARKER_TYPE eMarkerType,
    const PROPVARIANT *pvarMarkerValue,     // Can be NULL.
    const PROPVARIANT *pvarContextValue,    // Can be NULL.
    IMarker **ppMarker
    );
}}} // namespace Microsoft::Samples::SimpleCommunication
