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
#include "StspMediaSinkProxy.h"
#include "StspDefs.h"
#include "StspMediaSink.h"

using namespace Nequeo::Media::Communication;
using namespace Nequeo::Media::Communication::Network;

IncomingConnectionEventArgs::IncomingConnectionEventArgs(IStspSinkInternal *pSink, DWORD connectionId, String ^remoteUrl)
    : _spSink(pSink)
    , _connectionId(connectionId)
    , _remoteUrl(remoteUrl)
{
}

// User accepted connection
void IncomingConnectionEventArgs::Accept()
{
    _spSink->TriggerAcceptConnection(_connectionId);
}

// User refused connection
void IncomingConnectionEventArgs::Refuse()
{
    _spSink->TriggerRefuseConnection(_connectionId);
}

StspMediaSinkProxy::StspMediaSinkProxy()
{
}

StspMediaSinkProxy::~StspMediaSinkProxy()
{
    AutoLock lock(_critSec);

    if (_spMediaSink != nullptr)
    {
        _spMediaSink->Shutdown();
        _spMediaSink = nullptr;
    }
}

Windows::Media::IMediaExtension ^StspMediaSinkProxy::GetMFExtensions()
{
    AutoLock lock(_critSec);

    if (_spMediaSink == nullptr)
    {
        Throw(MF_E_NOT_INITIALIZED);
    }

    ComPtr<IInspectable> spInspectable;
    ThrowIfError(_spMediaSink.As(&spInspectable));

    return safe_cast<IMediaExtension^>(reinterpret_cast<Object^>(spInspectable.Get()));
}


Windows::Foundation::IAsyncOperation<IMediaExtension^>^ StspMediaSinkProxy::InitializeAsync(
    Windows::Media::MediaProperties::IMediaEncodingProperties ^audioEncodingProperties,
    Windows::Media::MediaProperties::IMediaEncodingProperties ^videoEncodingProperties
    )
{
    return concurrency::create_async([this, videoEncodingProperties, audioEncodingProperties]()
    {
        AutoLock lock(_critSec);
        CheckShutdown();

        if (_spMediaSink != nullptr)
        {
            Throw(MF_E_ALREADY_INITIALIZED);
        }

        // Prepare the MF extension
        ThrowIfError(MakeAndInitialize<CMediaSink>(&_spMediaSink, ref new StspSinkCallback(this), audioEncodingProperties, videoEncodingProperties));

        ComPtr<IInspectable> spInspectable;
        ThrowIfError(_spMediaSink.As(&spInspectable));

        return safe_cast<IMediaExtension^>(reinterpret_cast<Object^>(spInspectable.Get()));
    });
}

void StspMediaSinkProxy::FireIncomingConnection(IncomingConnectionEventArgs ^args)
{
    IncomingConnectionEvent(this, args);
}

void StspMediaSinkProxy::OnShutdown()
{
    AutoLock lock(_critSec);
    if (_fShutdown)
    {
        return;
    }
    _fShutdown = true;
    _spMediaSink = nullptr;
}
