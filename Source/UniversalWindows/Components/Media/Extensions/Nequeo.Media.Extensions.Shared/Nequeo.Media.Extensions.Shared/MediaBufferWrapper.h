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
#include <StspNetwork.h>
#include <robuffer.h>
#include <windows.storage.streams.h>

namespace Nequeo { namespace Media { namespace Communication { namespace Network {

    class CMediaBufferWrapper : 
        public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,
        ABI::Windows::Storage::Streams::IBuffer,
        Microsoft::WRL::CloakedIid<Windows::Storage::Streams::IBufferByteAccess>,
        Microsoft::WRL::CloakedIid<IMarshal>,
        Microsoft::WRL::CloakedIid<IMediaBufferWrapper>,
        Microsoft::WRL::FtmBase>
    {
        InspectableClass(L"Microsoft.Samples.SimpleCommunication.Buffer", BaseTrust);

    public:
        static HRESULT CreateInstance(DWORD dwMaxLength, _Outptr_ IMediaBufferWrapper **ppMediaBufferWrapper);
        static HRESULT CreateInstance(_In_ IMFMediaBuffer *pMediaBuffer, _Outptr_ IMediaBufferWrapper **ppMediaBufferWrapper);

        CMediaBufferWrapper(void);
        ~CMediaBufferWrapper(void);

        HRESULT RuntimeClassInitialize(IMFMediaBuffer *pMediaBuffer);

        // IBuffer
        IFACEMETHOD (get_Capacity) (UINT *pcbCapacity);
        IFACEMETHOD (get_Length) (UINT *pcbLength);
        IFACEMETHOD (put_Length) (UINT cbLength);

        // IBufferByteAccess
        IFACEMETHOD (Buffer) (_Out_ BYTE **ppBuffer);

        // IMarshal
        IFACEMETHOD (GetUnmarshalClass) (REFIID riid, _In_opt_ void *pv, DWORD dwDestContext, 
            _Reserved_ void *pvDestContext, DWORD mshlflags, _Out_ CLSID *pclsid);
        IFACEMETHOD (GetMarshalSizeMax) (REFIID riid, _In_opt_ void *pv, DWORD dwDestContext,
            _Reserved_ void *pvDestContext, DWORD mshlflags, _Out_ DWORD *pcbSize);
        IFACEMETHOD (MarshalInterface) (_In_ IStream *pStm, REFIID riid, _In_opt_ void *pv, DWORD dwDestContext,
            _Reserved_ void *pvDestContext, DWORD mshlflags);
        IFACEMETHOD (UnmarshalInterface) (_In_ IStream *, _In_ REFIID, _Outptr_ void **);
        IFACEMETHOD (ReleaseMarshalData) (_In_ IStream *);
        IFACEMETHOD (DisconnectObject) (DWORD);

        // IMediaBufferWrapper
        IFACEMETHOD_(IMFMediaBuffer *, GetMediaBuffer) () const {return _spMediaBuffer.Get();}
        IFACEMETHOD_(BYTE *, GetBuffer) () const {return _pBuffer + _nOffset;}

        IFACEMETHOD (SetOffset) (DWORD nOffset);
        IFACEMETHOD_(DWORD, GetOffset) () const {return _nOffset;}

        IFACEMETHOD (GetCurrentLength) (DWORD *pcbCurrentLength);
        IFACEMETHOD (SetCurrentLength) (DWORD cbCurrentLength);

        IFACEMETHOD (TrimLeft) (DWORD cbSize);
        IFACEMETHOD (TrimRight) (DWORD cbSize, _Out_ IMediaBufferWrapper **pWrapper);

        IFACEMETHOD (Reset) ();

    protected:
        HRESULT CheckMarshal();

    private:
        ComPtr<IMFMediaBuffer>  _spMediaBuffer;
        ComPtr<IMF2DBuffer>     _sp2DBuffer;
        BYTE                    *_pBuffer;
        DWORD                   _nOffset;
        ComPtr<IMarshal>        _spBufferMarshal;
    };

}}}}
