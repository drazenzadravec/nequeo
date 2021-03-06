/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright � Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
#include "NetworkChannel.h"

using namespace Nequeo::Media::Communication::Network;

namespace
{
    ref class CSendOperationStatus
    {
    internal:
        CSendOperationStatus()
        {
        }

        void SetException(Exception ^error)
        {
            if (_error == nullptr)
            {
                _error = error;
            }
        }

        void CheckException()
        {
            if (_error != nullptr)
            {
                throw _error;
            }
        }

    private:
        Exception ^_error;
    };
}

CNetworkChannel::CNetworkChannel()
    : _isClosed(false)
{
}

CNetworkChannel::~CNetworkChannel()
{
    Disconnect();
}

Windows::Foundation::IAsyncAction ^CNetworkChannel::SendAsync(_In_ IBufferPacket *pPacket)
{
    if (pPacket == nullptr)
    {
        throw ref new InvalidArgumentException();
    }

    return concurrency::create_async([this, pPacket](){
        AutoLock lock(_critSec);
        CheckClosed();
        IStreamSocket ^socket = GetSocket();
        if (socket == nullptr)
        {
            throw ref new Exception(MF_E_NET_NOCONNECTION);
        }

        auto outputStream = socket->OutputStream;
        ComPtr<IBufferEnumerator> spEn;
        ThrowIfError(pPacket->GetEnumerator(&spEn));
        if (!spEn->IsValid())
        {
            // Packet is empty
            Throw(E_INVALIDARG);
        }

        std::list<concurrency::task<void>> tasks;
        CSendOperationStatus ^status = ref new CSendOperationStatus();

        for (; spEn->IsValid(); spEn->MoveNext())
        {
            ComPtr<IMediaBufferWrapper> spBuffer;
            ComPtr<IInspectable> spInspectable;
            DWORD dwLen = 0;

            ThrowIfError(spEn->GetCurrent(&spBuffer));

            ThrowIfError(spBuffer.As(&spInspectable));
            IBuffer ^buffer = safe_cast<IBuffer^>(reinterpret_cast<Object^>(spInspectable.Get()));

            ThrowIfError(spBuffer->GetCurrentLength(&dwLen));

            tasks.push_back(concurrency::create_task(outputStream->WriteAsync(buffer)).
                then([this, status](concurrency::task<unsigned int>& task)
                {
                    try
                    {
                        // We want when_all to finish when all task finish so remember error.
                        // Otherwise if any of the tasks throws when_all finishes earlier.
                        task.get();
                    }
                    catch(Exception ^exc)
                    {
                        status->SetException(exc);
                    }
                }));
        }

        return concurrency::when_all(tasks.begin(), tasks.end())
            .then([this, status](concurrency::task<void>& task)
        {
            status->CheckException();
        });
    });
}

Windows::Foundation::IAsyncAction ^CNetworkChannel::ReceiveAsync(_In_ IMediaBufferWrapper *pBuffer)
{
    if (pBuffer == nullptr)
    {
        throw ref new InvalidArgumentException();
    }
    ComPtr<IMediaBufferWrapper> spBuffer = pBuffer;

    return concurrency::create_async([this, spBuffer](){
        AutoLock lock(_critSec);
        CheckClosed();
        IStreamSocket ^socket = GetSocket();
        IInputStream ^inputStream;
        ComPtr<IInspectable> spInspectable;
        DWORD cbBufferLen = 0;

        if (socket == nullptr)
        {
            Throw(MF_E_NET_NOCONNECTION);
        }

        inputStream = socket->InputStream;

        ThrowIfError(spBuffer->GetMediaBuffer()->GetMaxLength(&cbBufferLen));

        ThrowIfError(spBuffer.As(&spInspectable));

        return concurrency::create_task(inputStream->ReadAsync(safe_cast<IBuffer^>(reinterpret_cast<Object^>(spInspectable.Get())), cbBufferLen, InputStreamOptions::Partial))
            .then([this](IBuffer ^buffer)
        {
            if (buffer->Length  == 0)
            {
                Throw(MF_E_NET_READ);
            }
        });
    });
}

void CNetworkChannel::Close()
{
    AutoLock lock(_critSec);

    Disconnect();

    OnClose();

    _isClosed = true;
}

void CNetworkChannel::Disconnect()
{
    AutoLock lock(_critSec);

    if (_socket != nullptr)
    {
        delete _socket;
        _socket = nullptr;
    }
}
