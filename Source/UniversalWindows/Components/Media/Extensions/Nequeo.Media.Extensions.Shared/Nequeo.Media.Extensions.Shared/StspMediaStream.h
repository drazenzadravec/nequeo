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
#include <CritSec.h>
#include <linklist.h>
#include <StspDefs.h>

namespace Nequeo { namespace Media { namespace Communication {
    class CMediaSource;
    namespace Network
    {
        interface IBufferPacket;
    }

    class CMediaStream : 
        public IMFMediaStream,
        public IMFQualityAdvise2,
        public IMFGetService
    {
    public:
        static HRESULT CreateInstance(StspStreamDescription *pStreamDescription, Network::IBufferPacket *pAttributesBuffer, CMediaSource *pSource, CMediaStream **ppStream);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID iid, void **ppv);
        IFACEMETHOD_ (ULONG, AddRef) ();
        IFACEMETHOD_ (ULONG, Release) ();

        // IMFMediaEventGenerator
        IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback *pCallback,IUnknown *punkState);
        IFACEMETHOD (EndGetEvent) (IMFAsyncResult *pResult, IMFMediaEvent **ppEvent);
        IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent **ppEvent);
        IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT *pvValue);

        // IMFMediaStream
        IFACEMETHOD (GetMediaSource) (IMFMediaSource **ppMediaSource);
        IFACEMETHOD (GetStreamDescriptor) (IMFStreamDescriptor **ppStreamDescriptor);
        IFACEMETHOD (RequestSample) (IUnknown *pToken);

        // IMFQualityAdvise
        IFACEMETHOD (SetDropMode ) (_In_ MF_QUALITY_DROP_MODE eDropMode);
        IFACEMETHOD (SetQualityLevel ) ( _In_ MF_QUALITY_LEVEL eQualityLevel);
        IFACEMETHOD (GetDropMode ) (_Out_ MF_QUALITY_DROP_MODE *peDropMode);
        IFACEMETHOD (GetQualityLevel )(_Out_ MF_QUALITY_LEVEL *peQualityLevel );
        IFACEMETHOD (DropTime) (_In_ LONGLONG hnsAmountToDrop);

        // IMFQualityAdvise2
        IFACEMETHOD (NotifyQualityEvent) (_In_opt_ IMFMediaEvent *pEvent, _Out_ DWORD *pdwFlags);

        // IMFGetService
        IFACEMETHOD (GetService) (_In_ REFGUID guidService,
                                 _In_ REFIID riid,
                                 _Out_ LPVOID *ppvObject);

        // Other public methods
        HRESULT Start();
        HRESULT Stop();
        HRESULT SetRate(float flRate);
        HRESULT Flush();
        HRESULT Shutdown();
        void ProcessSample(StspSampleHeader *pSampleHeader, IMFSample *pSample);
        void ProcessFormatChange(IMFMediaType *pMediaType);
        HRESULT SetActive(bool fActive);
        bool IsActive() const {return _fActive;}
        SourceState GetState() const {return _eSourceState;}

        DWORD GetId() const {return _dwId;}

    protected:
        CMediaStream(CMediaSource *pSource);
        ~CMediaStream(void);

    private:
        class CSourceLock;

    private:
        void Initialize(StspStreamDescription *pStreamDescription, Network::IBufferPacket *pAttributesBuffer);
        void DeliverSamples();
        void SetSampleAttributes(StspSampleHeader *pSampleHeader, IMFSample *pSample);
        void HandleError(HRESULT hErrorCode);

        HRESULT CheckShutdown() const
        {
            if (_eSourceState == SourceState_Shutdown)
            {
                return MF_E_SHUTDOWN;
            }
            else
            {
                return S_OK;
            }
        }

        bool ShouldDropSample(IMFSample *pSample);
        void CleanSampleQueue();
        void ResetDropTime();

    private:
        long                        _cRef;                      // reference count
        SourceState                 _eSourceState;              // Flag to indicate if Shutdown() method was called.
        ComPtr<CMediaSource>        _spSource;                  // Media source
        ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
        ComPtr<IMFStreamDescriptor> _spStreamDescriptor;        // Stream descriptor

        ComPtrList<IUnknown>        _samples;
        ComPtrList<IUnknown, true>  _tokens;

        DWORD                       _dwId;
        bool                        _fActive;
        bool                        _fVideo;
        float                       _flRate;

        MF_QUALITY_DROP_MODE        _eDropMode;
        bool                        _fDiscontinuity;
        bool                        _fDropTime;
        bool                        _fInitDropTime;
        bool                        _fWaitingForCleanPoint;
        LONGLONG                    _hnsStartDroppingAt;
        LONGLONG                    _hnsAmountToDrop;
    };

}}} // namespace Microsoft::Samples::SimpleCommunication
