/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CSession.h
*  Purpose :       CSession class.
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

#ifndef _CSESSION_H
#define _CSESSION_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Media
	{
		/// <summary>
		/// Session control.
		/// </summary>
		class CSession : public IMFAsyncCallback
		{
		public:
			/// <summary>
			/// Create the session.
			/// </summary>
			/// <param name="ppSession">The created session.</param>
			static HRESULT Create(CSession **ppSession);

			/// <summary>
			/// Query interface.
			/// </summary>
			/// <param name="riid">The reference id.</param>
			/// <param name="ppv">The pointer to the type.</param>
			/// <returns>The result.</returns>
			STDMETHODIMP QueryInterface(REFIID riid, void** ppv);

			/// <summary>
			/// Add the reference.
			/// </summary>
			/// <returns>The result.</returns>
			STDMETHODIMP_(ULONG) AddRef();

			/// <summary>
			/// Release the resource.
			/// </summary>
			/// <returns>The result.</returns>
			STDMETHODIMP_(ULONG) Release();

			/// <summary>
			/// Get parameters.
			/// </summary>
			/// <param name="pdwFlags">The flags.</param>
			/// <param name="pdwQueue">The queue.</param>
			/// <returns>The result.</returns>
			STDMETHODIMP GetParameters(DWORD* pdwFlags, DWORD* pdwQueue)
			{
				// Implementation of this method is optional.
				return E_NOTIMPL;
			}

			/// <summary>
			/// Invoke.
			/// </summary>
			/// <param name="pResult">The async result.</param>
			/// <returns>The result.</returns>
			STDMETHODIMP Invoke(IMFAsyncResult *pResult);

			/// <summary>
			/// Start encoding session.
			/// </summary>
			/// <param name="pTopology">The topology.</param>
			/// <returns>The result.</returns>
			HRESULT StartEncodingSession(IMFTopology *pTopology);

			/// <summary>
			/// Get encoding position.
			/// </summary>
			/// <param name="pTime">The time.</param>
			/// <returns>The result.</returns>
			HRESULT GetEncodingPosition(MFTIME *pTime);

			/// <summary>
			/// Wait.
			/// </summary>
			/// <param name="dwMsec">The wait time.</param>
			/// <returns>The result.</returns>
			HRESULT Wait(DWORD dwMsec);

		private:
			/// <summary>
			/// Session control.
			/// </summary>
			CSession() : m_cRef(1), m_pSession(NULL), m_pClock(NULL), m_hrStatus(S_OK), m_hWaitEvent(NULL) {}

			/// <summary>
			/// Session control destructor.
			/// </summary>
			virtual ~CSession()
			{
				// If Created shutdown.
				if (m_pSession)
				{
					m_pSession->Shutdown();
				}

				// Release resources.
				SafeRelease(&m_pClock);
				SafeRelease(&m_pSession);
				CloseHandle(m_hWaitEvent);
			}

			/// <summary>
			/// Initialize.
			/// </summary>
			/// <returns>The result.</returns>
			HRESULT Initialize();

		private:
			IMFMediaSession      *m_pSession;
			IMFPresentationClock *m_pClock;
			HRESULT m_hrStatus;
			HANDLE  m_hWaitEvent;
			long    m_cRef;
		};
	}
}
#endif