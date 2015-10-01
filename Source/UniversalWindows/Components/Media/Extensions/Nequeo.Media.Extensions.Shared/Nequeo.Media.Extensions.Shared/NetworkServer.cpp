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
#include "NetworkServer.h"
#include "StspDefs.h"

using namespace Nequeo::Media::Communication::Network;

INetworkServer ^Nequeo::Media::Communication::Network::CreateNetworkServer(unsigned short listeningPort)
{
    return ref new CNetworkServer(listeningPort);
}

CNetworkServer::CNetworkServer(unsigned short listeningPort)
    : _listeningPort(listeningPort)
{
}

CNetworkServer::~CNetworkServer()
{
    OnClose();
}

Windows::Foundation::IAsyncOperation<Windows::Networking::Sockets::StreamSocketInformation^>^ CNetworkServer::AcceptAsync()
{
    return concurrency::create_async([this](){
        AutoLock lock(_critSec);

        try
        {
            if (_acceptOperation != nullptr)
            {
                // We only support accepting one connection.
                Throw(MF_E_INVALIDREQUEST);
            }

            _acceptOperation = ref new CAcceptOperation(this, _critSec);
            return concurrency::create_task(_acceptOperation->AcceptAsync(_listeningPort)).then([this](concurrency::task<IStreamSocket^>& acceptTask)
            {
                try
                {
                    auto clientSocket = acceptTask.get();                        

                    SetSocket(clientSocket);

                    return clientSocket->Information;
                }
                catch (Exception ^exc)
                {
                    if (_acceptOperation != nullptr)
                    {
                        delete _acceptOperation;
                        _acceptOperation = nullptr;
                    }
                    throw;
                }
            });
        }
        catch (Exception ^exc)
        {
            if (_acceptOperation != nullptr)
            {
                delete _acceptOperation;
                _acceptOperation = nullptr;
            }
            throw;
        }
    });
}

void CNetworkServer::OnClose()
{
    if (_acceptOperation != nullptr)
    {
        delete _acceptOperation;
        _acceptOperation = nullptr;
    }
}

CAcceptOperation::CAcceptOperation(CNetworkServer ^parent, CritSec &critSec)
    : _parent(parent)
    , _critSec(critSec)
    , _fAsyncOperationInProgress(false)
{
}

CAcceptOperation::~CAcceptOperation()
{
    AutoLock lock(_critSec);
    Detach();
    if (_fAsyncOperationInProgress && !_completionEvent._IsTriggered())
    {
        _completionEvent.set_exception(ref new DisconnectedException());
    }
}

concurrency::task<IStreamSocket^> CAcceptOperation::AcceptAsync(unsigned short port)
{
    AutoLock lock(_critSec);
    if (_listener != nullptr)
    {
        Throw(MF_E_INVALIDREQUEST);
    }
    WCHAR szPortNumber[6];
    if (port == 0)
    {
        port = c_wStspDefaultPort;
    }

    ThrowIfError(StringCchPrintf(szPortNumber, _countof(szPortNumber), L"%hu", port));
    _listener = ref new StreamSocketListener();
    _connectionReceivedToken = _listener->ConnectionReceived += ref new Windows::Foundation::TypedEventHandler<Windows::Networking::Sockets::StreamSocketListener ^, Windows::Networking::Sockets::StreamSocketListenerConnectionReceivedEventArgs ^>(this, &Nequeo::Media::Communication::Network::CAcceptOperation::OnConnectionReceived);
    concurrency::create_task(_listener->BindServiceNameAsync(ref new String(szPortNumber)));
    auto &result = concurrency::create_task(_completionEvent);
    _fAsyncOperationInProgress = true;
    return result;
}

void CAcceptOperation::OnConnectionReceived(Windows::Networking::Sockets::StreamSocketListener ^sender, Windows::Networking::Sockets::StreamSocketListenerConnectionReceivedEventArgs ^args)
{
    AutoLock lock(_critSec);
    Detach();
    if (!_completionEvent.set(args->Socket))
    {
        delete args->Socket;
    }
}

void CAcceptOperation::Detach()
{
    _parent = nullptr;
    delete _listener;
    _listener = nullptr;
}
