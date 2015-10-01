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
#include "StspNetwork.h"

namespace Nequeo { namespace Media { namespace Communication { namespace Network {
    
    ref class CNetworkChannel: public INetworkChannel
    {
    public:
        virtual ~CNetworkChannel();

    internal:
        CNetworkChannel();

        // INetworkChannel
        virtual Windows::Foundation::IAsyncAction ^SendAsync(_In_ IBufferPacket *pPacket) override;
        virtual Windows::Foundation::IAsyncAction ^ReceiveAsync(_In_ IMediaBufferWrapper *pBuffer) override;
        virtual void Close() override;
        virtual void Disconnect() override;

    private protected:

        virtual void OnClose() {}

        void SetSocket(IStreamSocket ^socket) { _socket = socket; }
        IStreamSocket ^GetSocket() { return _socket; }

        void CheckClosed()
        {
            if (_isClosed)
            {
                throw ref new Exception(MF_E_SHUTDOWN);
            }
        }

        CritSec _critSec;                // critical section for thread safety

    private:
        bool _isClosed;
        IStreamSocket ^_socket;
    };
}}}}
