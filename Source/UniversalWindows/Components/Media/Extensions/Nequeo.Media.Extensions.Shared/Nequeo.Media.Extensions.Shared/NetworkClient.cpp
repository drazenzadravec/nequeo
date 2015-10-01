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
#include "NetworkClient.h"
#include "StspDefs.h"

using namespace Nequeo::Media::Communication::Network;

INetworkClient ^Nequeo::Media::Communication::Network::CreateNetworkClient()
{
    return ref new CNetworkClient();
}

CNetworkClient::CNetworkClient()
{
    SetSocket(ref new StreamSocket());
}

CNetworkClient::~CNetworkClient()
{
}

Windows::Foundation::IAsyncAction ^CNetworkClient::ConnectAsync(String ^url, WORD wPort)
{
    if (url == nullptr)
    {
        throw ref new InvalidArgumentException();
    }

    AutoLock lock(_critSec);
    CheckClosed();
    WCHAR szPortNumber[6];
    HostName ^hostName;
    IStreamSocket ^socket = GetSocket();
    WORD port = wPort;
    HStringReference hostNameId(RuntimeClass_Windows_Networking_HostName);

    if (socket == nullptr)
    {
        Throw(MF_E_NOT_INITIALIZED);
    }

    if (port == 0)
    {
        port = c_wStspDefaultPort;
    }
    ThrowIfError(StringCchPrintf(szPortNumber, _countof(szPortNumber), L"%hu", wPort));

    hostName = ref new HostName(url);

    // Start asynchronous operation
    return socket->ConnectAsync(hostName, ref new String(szPortNumber));
}
