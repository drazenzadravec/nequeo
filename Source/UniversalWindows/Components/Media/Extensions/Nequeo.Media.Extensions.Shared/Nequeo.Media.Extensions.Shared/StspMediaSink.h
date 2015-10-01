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
#include <BaseAttributes.h>
#include <StspDefs.h>
#include <StspNetwork.h>

namespace Nequeo { namespace Media { namespace Communication {
interface class ISinkCallback;
class CStreamSink;

class CMediaSink
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           FtmBase,
           IMFMediaSink,
           IMFClockStateSink,
           IStspSinkInternal>
    , public Nequeo::Media::Common::CBaseAttributes<>
{
    InspectableClass(L"Nequeo::Media::Communication::StspMediaSink",BaseTrust)

public:
    CMediaSink();
    ~CMediaSink();

    HRESULT RuntimeClassInitialize(
        ISinkCallback ^callback,
        Windows::Media::MediaProperties::IMediaEncodingProperties ^audioEncodingProperties,
        Windows::Media::MediaProperties::IMediaEncodingProperties ^videoEncodingProperties
        );

    // IStspSinkInternal
    void TriggerAcceptConnection(DWORD connectionId);
    void TriggerRefuseConnection(DWORD connectionId);

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration) {return S_OK;}

    // IMFMediaSink methods
    IFACEMETHOD (GetCharacteristics) (DWORD *pdwCharacteristics);

    IFACEMETHOD (AddStreamSink)(
        /* [in] */ DWORD dwStreamSinkIdentifier,
        /* [in] */ IMFMediaType *pMediaType,
        /* [out] */ IMFStreamSink **ppStreamSink);

    IFACEMETHOD (RemoveStreamSink) (DWORD dwStreamSinkIdentifier);
    IFACEMETHOD (GetStreamSinkCount) (_Out_ DWORD *pcStreamSinkCount);
    IFACEMETHOD (GetStreamSinkByIndex) (DWORD dwIndex, _Outptr_ IMFStreamSink **ppStreamSink);
    IFACEMETHOD (GetStreamSinkById) (DWORD dwIdentifier, IMFStreamSink **ppStreamSink);
    IFACEMETHOD (SetPresentationClock) (IMFPresentationClock *pPresentationClock);
    IFACEMETHOD (GetPresentationClock) (IMFPresentationClock **ppPresentationClock);
    IFACEMETHOD (Shutdown) ();

    // IMFClockStateSink methods
    IFACEMETHOD (OnClockStart) (MFTIME hnsSystemTime, LONGLONG llClockStartOffset);
    IFACEMETHOD (OnClockStop) (MFTIME hnsSystemTime);
    IFACEMETHOD (OnClockPause) (MFTIME hnsSystemTime);
    IFACEMETHOD (OnClockRestart) (MFTIME hnsSystemTime);
    IFACEMETHOD (OnClockSetRate) (MFTIME hnsSystemTime, float flRate);

    LONGLONG GetStartTime() const {return _llStartTime;}

    void ReportEndOfStream();

private:
    typedef ComPtrList<IMFStreamSink> StreamContainer;

private:
    void StartListening();
    void StartReceiving(Network::IMediaBufferWrapper *pReceiveBuffer);
    concurrency::task<void> SendPacket(Network::IBufferPacket *pPacket);
    String ^PrepareRemoteUrl(StreamSocketInformation ^info);
    void SendDescription();

    ComPtr<Network::IMediaBufferWrapper> FillStreamDescription(CStreamSink *pStream, StspStreamDescription *pStreamDescription);

    void HandleError(HRESULT hr);

    void SetMediaStreamProperties(
        Windows::Media::Capture::MediaStreamType MediaStreamType,
        _In_opt_ Windows::Media::MediaProperties::IMediaEncodingProperties ^mediaEncodingProperties);

    HRESULT CheckShutdown() const
    {
        if (_IsShutdown)
        {
            return MF_E_SHUTDOWN;
        }
        else
        {
            return S_OK;
        }
    }

private:
    long                            _cRef;                      // reference count
    CritSec                         _critSec;                   // critical section for thread safety

    bool                            _IsShutdown;                // Flag to indicate if Shutdown() method was called.
    bool                            _IsConnected;
    LONGLONG                        _llStartTime;

    ComPtr<IMFPresentationClock>    _spClock;                   // Presentation clock.
    Network::INetworkChannel^       _networkSender;  
    ISinkCallback^                  _callback;
    ComPtr<Network::IMediaBufferWrapper> _spReceiveBuffer;
    StreamContainer                 _streams;
    long                            _cStreamsEnded;
    String^                         _remoteUrl;

    DWORD                           _waitingConnectionId;
};

}}} // namespace Microsoft::Samples::SimpleCommunication
