/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CSession.cpp
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

#include "stdafx.h"

#include "CSession.h"

using namespace Nequeo::Media;

/// <summary>
/// Create the session.
/// </summary>
/// <param name="ppSession">The created session.</param>
HRESULT CSession::Create(CSession **ppSession)
{
	*ppSession = NULL;

	CSession *pSession = new (std::nothrow) CSession();
	if (pSession == NULL)
	{
		return E_OUTOFMEMORY;
	}

	HRESULT hr = pSession->Initialize();
	if (FAILED(hr))
	{
		pSession->Release();
		return hr;
	}
	*ppSession = pSession;
	return S_OK;
}

/// <summary>
/// Query interface.
/// </summary>
/// <param name="riid">The reference id.</param>
/// <param name="ppv">The pointer to the type.</param>
/// <returns>The result.</returns>
STDMETHODIMP CSession::QueryInterface(REFIID riid, void** ppv)
{
	static const QITAB qit[] =
	{
		QITABENT(CSession, IMFAsyncCallback),
		{ 0 }
	};
	return QISearch(this, qit, riid, ppv);
}

/// <summary>
/// Add the reference.
/// </summary>
/// <returns>The result.</returns>
STDMETHODIMP_(ULONG) CSession::AddRef()
{
	return InterlockedIncrement(&m_cRef);
}

/// <summary>
/// Release the resource.
/// </summary>
/// <returns>The result.</returns>
STDMETHODIMP_(ULONG) CSession::Release()
{
	long cRef = InterlockedDecrement(&m_cRef);
	if (cRef == 0)
	{
		delete this;
	}
	return cRef;
}

/// <summary>
/// Initialize.
/// </summary>
/// <returns>The result.</returns>
HRESULT CSession::Initialize()
{
	IMFClock *pClock = NULL;

	HRESULT hr = MFCreateMediaSession(NULL, &m_pSession);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = m_pSession->GetClock(&pClock);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pClock->QueryInterface(IID_PPV_ARGS(&m_pClock));
	if (FAILED(hr))
	{
		goto done;
	}

	hr = m_pSession->BeginGetEvent(this, NULL);
	if (FAILED(hr))
	{
		goto done;
	}

	m_hWaitEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	if (m_hWaitEvent == NULL)
	{
		hr = HRESULT_FROM_WIN32(GetLastError());
	}
done:
	SafeRelease(&pClock);
	return hr;
}

/// <summary>
/// Invoke.
/// </summary>
/// <param name="pResult">The async result.</param>
/// <returns>The result.</returns>
STDMETHODIMP CSession::Invoke(IMFAsyncResult *pResult)
{
	IMFMediaEvent* pEvent = NULL;
	MediaEventType meType = MEUnknown;
	HRESULT hrStatus = S_OK;

	HRESULT hr = m_pSession->EndGetEvent(pResult, &pEvent);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pEvent->GetType(&meType);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pEvent->GetStatus(&hrStatus);
	if (FAILED(hr))
	{
		goto done;
	}

	if (FAILED(hrStatus))
	{
		hr = hrStatus;
		goto done;
	}

	switch (meType)
	{
	case MESessionEnded:
		hr = m_pSession->Close();
		if (FAILED(hr))
		{
			goto done;
		}
		break;

	case MESessionClosed:
		SetEvent(m_hWaitEvent);
		break;
	}

	if (meType != MESessionClosed)
	{
		hr = m_pSession->BeginGetEvent(this, NULL);
	}

done:
	if (FAILED(hr))
	{
		m_hrStatus = hr;
		m_pSession->Close();
	}

	SafeRelease(&pEvent);
	return hr;
}

/// <summary>
/// Start encoding session.
/// </summary>
/// <param name="pTopology">The topology.</param>
/// <returns>The result.</returns>
HRESULT CSession::StartEncodingSession(IMFTopology *pTopology)
{
	HRESULT hr = m_pSession->SetTopology(0, pTopology);
	if (SUCCEEDED(hr))
	{
		PROPVARIANT varStart;
		PropVariantClear(&varStart);
		hr = m_pSession->Start(&GUID_NULL, &varStart);
	}
	return hr;
}

/// <summary>
/// Get encoding position.
/// </summary>
/// <param name="pTime">The time.</param>
/// <returns>The result.</returns>
HRESULT CSession::GetEncodingPosition(MFTIME *pTime)
{
	return m_pClock->GetTime(pTime);
}

/// <summary>
/// Wait.
/// </summary>
/// <param name="dwMsec">The wait time.</param>
/// <returns>The result.</returns>
HRESULT CSession::Wait(DWORD dwMsec)
{
	HRESULT hr = S_OK;

	DWORD dwTimeoutStatus = WaitForSingleObject(m_hWaitEvent, dwMsec);
	if (dwTimeoutStatus != WAIT_OBJECT_0)
	{
		hr = E_PENDING;
	}
	else
	{
		hr = m_hrStatus;
	}
	return hr;
}