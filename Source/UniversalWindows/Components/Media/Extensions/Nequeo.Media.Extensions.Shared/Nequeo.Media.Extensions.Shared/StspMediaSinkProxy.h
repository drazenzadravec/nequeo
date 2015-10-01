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
#include "StspDefs.h"
#include <queue>

namespace Nequeo { namespace Media { namespace Communication {
    ref class StspMediaSinkProxy;
    
    public ref class IncomingConnectionEventArgs sealed
    {
    public:
        property String ^RemoteUrl
        {
            String ^get() { return _remoteUrl; }
        }

        void Accept();
        void Refuse();

    internal:
        IncomingConnectionEventArgs(IStspSinkInternal *pSink, DWORD connectionId, String ^remoteUrl);

    private:
        String ^_remoteUrl;
        DWORD _connectionId;
        ComPtr<IStspSinkInternal> _spSink;
    };

    interface class ISinkCallback
    {
        void FireIncomingConnection(IncomingConnectionEventArgs ^args);
        void OnShutdown();
    };
    
    public ref class StspMediaSinkProxy sealed
    {
    public:
        StspMediaSinkProxy();
        virtual ~StspMediaSinkProxy();

        Windows::Media::IMediaExtension ^GetMFExtensions();

        Windows::Foundation::IAsyncOperation<Windows::Media::IMediaExtension^>^ InitializeAsync(
            Windows::Media::MediaProperties::IMediaEncodingProperties ^videoEncodingProperties,
            Windows::Media::MediaProperties::IMediaEncodingProperties ^audioEncodingProperties
            );

        event Windows::Foundation::EventHandler<Object^>^ IncomingConnectionEvent;

    internal:

        void SetMediaStreamProperties(
            Windows::Media::Capture::MediaStreamType MediaStreamType,
            _In_opt_ Windows::Media::MediaProperties::IMediaEncodingProperties ^mediaEncodingProperties
            );

    private:
        void FireIncomingConnection(IncomingConnectionEventArgs ^args);
        void OnShutdown();

        ref class StspSinkCallback sealed: ISinkCallback
        {
        public:
            virtual void FireIncomingConnection(IncomingConnectionEventArgs ^args)
            {
                _parent->FireIncomingConnection(args);
            }

            virtual void OnShutdown()
            {
                _parent->OnShutdown();
            }

        internal:
            StspSinkCallback(StspMediaSinkProxy ^parent)
                : _parent(parent)
            {
            }

        private:
            StspMediaSinkProxy ^_parent;
        };

        void CheckShutdown()
        {
            if (_fShutdown)
            {
                Throw(MF_E_SHUTDOWN);
            }
        }

    private:
        CritSec _critSec;
        ComPtr<IMFMediaSink> _spMediaSink;
        bool _fShutdown;
    };
} } }
