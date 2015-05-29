/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          DataPoint.h
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

#include "stdafx.h"

using namespace System;
using namespace System::Numerics;

/*
namespace Nequeo 
{
	namespace Math 
	{
		class CompletionPort : public CHandle
{
public:

    CompletionPort() :
        m_closeHandle(true)
    {
        // Do nothing
    }

    explicit CompletionPort(__in bool closeHandle) :
        m_closeHandle(closeHandle)
    {
        // Do nothing
    }

    CompletionPort(__in bool closeHandle,
                   __in_opt HANDLE handle) :
        m_closeHandle(closeHandle),
        CHandle(handle)
    {
        // Do nothing
    }

    ~CompletionPort()
    {
        if (!m_closeHandle)
        {
            Detach();
        }
    }

    __checkReturn HRESULT Create(__in DWORD threadCount)
    {
        Attach(::CreateIoCompletionPort(INVALID_HANDLE_VALUE,
                                        0, // no existing port
                                        0, // ignored
                                        threadCount));

        if (0 == m_h)
        {
            return HRESULT_FROM_WIN32(::GetLastError());
        }

        return S_OK;
    }

    __checkReturn HRESULT AssociateFile(__in HANDLE file,
                                        __in ULONG_PTR completionKey)
    {
        ASSERT(0 != file && INVALID_HANDLE_VALUE != file);
        ASSERT(0 != m_h);

        if (0 == ::CreateIoCompletionPort(file,
                                          m_h,
                                          completionKey,
                                          0)) // ignored
        {
            return HRESULT_FROM_WIN32(::GetLastError());
        }

        return S_OK;
    }

    __checkReturn HRESULT QueuePacket(__in DWORD bytesCopied,
                                      __in ULONG_PTR completionKey,
                                      __in OVERLAPPED* overlapped)
    {
        ASSERT(0 != m_h);

        if (!::PostQueuedCompletionStatus(m_h,
                                          bytesCopied,
                                          completionKey,
                                          overlapped))
        {
            return HRESULT_FROM_WIN32(::GetLastError());
        }

        return S_OK;
    }

    __checkReturn HRESULT DequeuePacket(__in DWORD milliseconds,
                                        __out DWORD& bytesCopied,
                                        __out ULONG_PTR& completionKey,
                                        __out OVERLAPPED*& overlapped)
    {
        ASSERT(0 != m_h);

        if (!::GetQueuedCompletionStatus(m_h,
                                         &bytesCopied,
                                         &completionKey,
                                         &overlapped,
                                         milliseconds))
        {
            return HRESULT_FROM_WIN32(::GetLastError());
        }

        return S_OK;
    }

private:

    CompletionPort(CompletionPort&);
    CompletionPort& operator=(CompletionPort&);

    bool m_closeHandle;

};
	}
}
*/