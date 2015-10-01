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
#include <list>
#include <StspNetwork.h>

namespace Nequeo { namespace Media { namespace Communication { namespace Network {

    class CBufferPacket : public IBufferPacket
    {
    public:
        typedef std::list<ComPtr<IMediaBufferWrapper> > Container;
        typedef std::list<ComPtr<IMediaBufferWrapper> >::iterator Iterator;

        static HRESULT FromMFSample(IMFSample *pSample, IBufferPacket **ppBufferPackage);
        static HRESULT CreateInstance(IBufferPacket **ppBufferPackage);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID riid, void **ppv);
        IFACEMETHOD_(ULONG, AddRef) ();
        IFACEMETHOD_(ULONG, Release) ();

        // IBufferPacket
        IFACEMETHOD (AddBuffer) (IMediaBufferWrapper *pBuffer);
        IFACEMETHOD (InsertBuffer) (unsigned int nIndex, IMediaBufferWrapper *pBuffer);
        IFACEMETHOD (RemoveBuffer) (unsigned int nIndex, IMediaBufferWrapper **ppBuffer);
        IFACEMETHOD (Clear) ();
        IFACEMETHOD_(size_t, GetBufferCount) () const;

        IFACEMETHOD (GetTotalLength) (_Out_ DWORD *pcbTotalLength);
        IFACEMETHOD (CopyTo) (DWORD nOffset, DWORD cbSize, _In_reads_bytes_(cbSize) void *pDest, _Out_ DWORD *pcbCopied);
        IFACEMETHOD (MoveLeft) (DWORD cbSize, _Out_writes_bytes_(cbSize) void *pDest);
        IFACEMETHOD (TrimLeft) (DWORD cbSize);
        IFACEMETHOD (ToMFSample) (IMFSample **ppSample);

        IFACEMETHOD (GetEnumerator) (_Out_ IBufferEnumerator **ppEnumerator);

    #if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
        IFACEMETHOD (FillWSABUF) (WSABUF *pBuffers, size_t cBuffers);
        IFACEMETHOD (SaveToFile) (LPCWSTR pszFileName);
    #endif
    protected:
        CBufferPacket(void);
        ~CBufferPacket(void);

    private:
        long                        _cRef;                      // reference count
        Container                   _buffers;                   // List of packets
    };

    class CBufferEnumerator: public IBufferEnumerator
    {
    public:
        static HRESULT CreateInstance(CBufferPacket::Container &container, IUnknown *pParent, IBufferEnumerator **ppEnumerator);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID riid, void **ppv);
        IFACEMETHOD_(ULONG, AddRef) ();
        IFACEMETHOD_(ULONG, Release) ();

        // IBufferEnumerator
        IFACEMETHOD_ (bool, IsValid) ();
        IFACEMETHOD (GetCurrent) (_Out_ IMediaBufferWrapper **ppBuffer);
        IFACEMETHOD (MoveNext) ();
        IFACEMETHOD (Reset) ();

    protected:
        CBufferEnumerator(CBufferPacket::Container &container, IUnknown *pParent);
        ~CBufferEnumerator();

    private:
        long                        _cRef;                      // reference count
        CBufferPacket::Container    &_buffers;                  // List of packets
        CBufferPacket::Iterator     _currentIt;                 // Current position
        ComPtr<IUnknown>            _spParent;                  // Parent object, we hold reference to it to make sure the container is valid all the time.
    };

}}}}
