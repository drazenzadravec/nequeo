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
#include "NetworkChannel.h"

namespace Nequeo { namespace Media { namespace Communication { namespace Network {

    ref class CAcceptOperation;

    ref class CNetworkServer sealed: 
        public CNetworkChannel, 
        public INetworkServer
    {
    public:
        virtual ~CNetworkServer();

        // INetworkServer
        virtual Windows::Foundation::IAsyncOperation<Windows::Networking::Sockets::StreamSocketInformation^>^ AcceptAsync();

    internal:
        CNetworkServer(unsigned short listeningPort);

    protected:
        virtual void OnClose() override;

    private:
        unsigned short _listeningPort;
        CAcceptOperation ^_acceptOperation;
    };

    ref class CAcceptOperation sealed
    {
    public:
        virtual ~CAcceptOperation();

    internal:
        CAcceptOperation(CNetworkServer ^parent, CritSec &critSec);
        
        concurrency::task<IStreamSocket^> AcceptAsync(unsigned short port);

    private:
        void Detach();
        void OnConnectionReceived(Windows::Networking::Sockets::StreamSocketListener ^sender, Windows::Networking::Sockets::StreamSocketListenerConnectionReceivedEventArgs ^args);

        CritSec &_critSec;
        IStreamSocketListener ^_listener;
        concurrency::task_completion_event<IStreamSocket^>  _completionEvent;
        Windows::Foundation::EventRegistrationToken _connectionReceivedToken;
        CNetworkServer ^_parent;
        bool _fAsyncOperationInProgress;
    };
}}}}