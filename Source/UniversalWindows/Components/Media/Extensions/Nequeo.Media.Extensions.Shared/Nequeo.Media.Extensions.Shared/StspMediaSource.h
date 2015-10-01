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
#include <OpQueue.h>
#include <linklist.h>
#include <BaseAttributes.h>
#include <StspNetwork.h>
#include <StspDefs.h>

namespace Nequeo { namespace Media { namespace Communication {

class CMediaStream;

// Base class representing asyncronous source operation
class CSourceOperation : public IUnknown
{
public:
    enum Type
    {
        // Start the source
        Operation_Start,
        // Stop the source
        Operation_Stop,
        // Set rate
        Operation_SetRate,
    };

public:
    CSourceOperation(Type opType);
    virtual ~CSourceOperation();
    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void **ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    Type GetOperationType() const {return _opType;}
    const PROPVARIANT &GetData() const { return _data;}
    HRESULT SetData(const PROPVARIANT &varData);

private:
    long                        _cRef;                      // reference count
    Type                        _opType;
    PROPVARIANT                 _data;
};

// Start operation
class CStartOperation : public CSourceOperation
{
public:
    CStartOperation(IMFPresentationDescriptor *pPD);
    ~CStartOperation();

    IMFPresentationDescriptor *GetPresentationDescriptor() {return _spPD.Get();}

private:
    ComPtr<IMFPresentationDescriptor> _spPD;   // Presentation descriptor
};

// SetRate operation
class CSetRateOperation : public CSourceOperation
{
public:
    CSetRateOperation(BOOL fThin, float flRate);
    ~CSetRateOperation();

    BOOL IsThin() const {return _fThin;}
    float GetRate() const {return _flRate;}

private:
    BOOL _fThin;
    float _flRate;
};

class CMediaSource : 
    public OpQueue<CMediaSource, CSourceOperation>, 
    public IMFMediaSource,
	public IMFGetService,
    public IMFRateControl,
    public Nequeo::Media::Common::CBaseAttributes<>
{
public:
    static HRESULT CreateInstance(CMediaSource **ppNetSource);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void **ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    // IMFMediaEventGenerator
    IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback *pCallback,IUnknown *punkState);
    IFACEMETHOD (EndGetEvent) (IMFAsyncResult *pResult, IMFMediaEvent **ppEvent);
    IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent **ppEvent);
    IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT *pvValue);

    // IMFMediaSource
    IFACEMETHOD (CreatePresentationDescriptor) (IMFPresentationDescriptor **ppPresentationDescriptor);
    IFACEMETHOD (GetCharacteristics) (DWORD *pdwCharacteristics);
    IFACEMETHOD (Pause) ();
    IFACEMETHOD (Shutdown) ();
    IFACEMETHOD (Start) (
        IMFPresentationDescriptor *pPresentationDescriptor,
        const GUID *pguidTimeFormat,
        const PROPVARIANT *pvarStartPosition
    );
    IFACEMETHOD (Stop)();

	// IMFGetService
    IFACEMETHOD (GetService) ( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject);

    // IMFRateControl
    IFACEMETHOD (SetRate) (BOOL fThin, float flRate);        
    IFACEMETHOD (GetRate) (_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate);

    // OpQueue
    __override HRESULT DispatchOperation(_In_ CSourceOperation *pOp);
    __override HRESULT ValidateOperation(_In_ CSourceOperation *pOp);

    // Called by the byte stream handler.
    concurrency::task<void> OpenAsync(LPCWSTR pszUrl);

    // Other public methods
    _Acquires_lock_(_critSec)
    HRESULT Lock();
    _Releases_lock_(_critSec)
    HRESULT Unlock();

protected:
    CMediaSource(void);
    ~CMediaSource(void);

private:
    typedef ComPtrList<IMFMediaStream> StreamContainer;

private:
    void Initialize();
    
    void HandleError(HRESULT hResult);
    HRESULT GetStreamById(DWORD dwId, CMediaStream **ppStream);
    void ParseServerUrl(LPCWSTR pszUrl);
    void CompleteOpen(HRESULT hResult);
    concurrency::task<void> SendRequestAsync(StspOperation eOperation);
    void SendDescribeRequest();
    void SendStartRequest();
    void Receive();
    void ParseCurrentBuffer();
    void ProcessPacket(StspOperationHeader *pOpHeader, Network::IBufferPacket *pPacket);
    void ProcessServerDescription(Network::IBufferPacket *pPacket);
    void ProcessServerSample(Network::IBufferPacket *pPacket);
    void ProcessServerFormatChange(Network::IBufferPacket *pPacket);
    HRESULT AddStream(StspStreamDescription *pStreamDesc);
    void InitPresentationDescription();
    HRESULT ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD);
    void SelectStreams(IMFPresentationDescriptor *pPD);

    void DoStart(CStartOperation *pOp);
    void DoStop(CSourceOperation *pOp);
    void DoSetRate(CSetRateOperation *pOp);

    bool IsRateSupported(float flRate, float *pflAdjustedRate);

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

private:
    long                        _cRef;                      // reference count
    CritSec                     _critSec;                   // critical section for thread safety
    SourceState                 _eSourceState;              // Flag to indicate if Shutdown() method was called.
    ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
    Network::INetworkChannel^   _networkSender;             // Network sender
    String^                     _serverAddress;             // Address of a server
    WORD                        _serverPort;                // Port of a server
    
    ComPtr<Network::IBufferPacket> _spCurrentReceivePacket;    // Receive packet
    ComPtr<Network::IMediaBufferWrapper> _spCurrentReceiveBuffer;    // Current buffer 
    StspOperationHeader         _CurrentReceivedOperationHeader; // Header of the operation currently being received from the server.

    ComPtr<IMFPresentationDescriptor> _spPresentationDescriptor;

    StreamContainer             _streams;                   // Collection of streams associated with the source

    concurrency::task_completion_event<void> _openedEvent;

    float                       _flRate;
};

}}} // namespace Microsoft::Samples::SimpleCommunication
