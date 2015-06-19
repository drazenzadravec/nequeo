/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContentEnabler.cpp
*  Purpose :       ContentEnabler class.
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

#include "ContentEnabler.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="hwndNotify">The handle to the hotifer.</param>
			ContentProtectionManager::ContentProtectionManager(HWND hNotify)
				: _nRefCount(0), _pMEG(NULL), _pResult(NULL), _pEnabler(NULL), _hwnd(hNotify),
				_state(Enabler_Ready), _hrStatus(S_OK)
			{
			}

			/// <summary>
			/// This destructor. call release to cleanup resources.
			/// </summary>
			ContentProtectionManager::~ContentProtectionManager()
			{
				SAFE_RELEASE(_pMEG);
				SAFE_RELEASE(_pResult);
				SAFE_RELEASE(_pEnabler);
			}

			/// <summary>
			/// Static class method to create the ContentProtectionManager object.
			/// </summary>
			/// <param name="hNotify">Handle to the application window to receive notifications.</param>
			/// <param name="ppManager">Receives an AddRef's pointer to the ContentProtectionManager object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::CreateInstance(HWND hNotify, ContentProtectionManager **ppManager)
			{
				// If nothing has been passed.
				if (hNotify == NULL || ppManager == NULL)
				{
					// Return invalid arguments.
					return E_INVALIDARG;
				}

				// All is good.
				HRESULT hr = S_OK;

				// Create the content protection manger.
				ContentProtectionManager *pManager = new ContentProtectionManager(hNotify);

				// If not created.
				if (pManager == NULL)
				{
					// Return out of memory.
					hr = E_OUTOFMEMORY;
				}

				// If created.
				if (SUCCEEDED(hr))
				{
					// Assign the manger.
					*ppManager = pManager;
					(*ppManager)->AddRef();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG ContentProtectionManager::AddRef()
			{
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG ContentProtectionManager::Release()
			{
				ULONG uCount = InterlockedDecrement(&_nRefCount);
				if (uCount == 0)
				{
					// Delete this class.
					delete this;
				}

				// For thread safety, return a temporary variable.
				return uCount;
			}

			/// <summary>
			/// Get the player reference for the reference id.
			/// </summary>
			/// <param name="iid">The player reference id.</param>
			/// <param name="ppv">The current player reference.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::QueryInterface(REFIID iid, void** ppv)
			{
				if (!ppv)
				{
					// Invalid pointer.
					return E_POINTER;
				}

				if (iid == IID_IUnknown)
				{
					// Return this player reference with async callback.
					*ppv = static_cast<IUnknown*>(static_cast<IMFAsyncCallback*>(this));
				}
				else if (iid == IID_IMFAsyncCallback)
				{
					// Return this player with default player reference.
					*ppv = static_cast<IMFAsyncCallback*>(this);
				}
				else if (iid == IID_IMFContentProtectionManager)
				{
					// Return this player reference with async callback.
					*ppv = static_cast<IMFContentProtectionManager*>(this);
				}
				else
				{
					// No such interface supported.
					return E_NOINTERFACE;
				}

				// Add a new player ref item.
				AddRef();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Callback for asynchronous BeginGetEvent method.
			/// </summary>
			/// <param name="pAsyncResult">The pointer to the result.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::Invoke(IMFAsyncResult *pAsyncResult)
			{
				HRESULT hr = S_OK;
				IMFMediaEvent* pEvent = NULL;
				MediaEventType meType = MEUnknown;  // Event type
				PROPVARIANT varEventData;	        // Event data

				PropVariantInit(&varEventData);

				// Get the event from the event queue.
				hr = _pMEG->EndGetEvent(pAsyncResult, &pEvent);

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Get the event type.
					hr = pEvent->GetType(&meType);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Get the event status. If the operation that triggered the event did
					// not succeed, the status is a failure code.
					hr = pEvent->GetStatus(&_hrStatus);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Get the event data.
					hr = pEvent->GetValue(&varEventData);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// For the MEEnablerCompleted action, notify the application.
					// Otehrwise, request another event.
					if (meType == MEEnablerCompleted)
					{
						// Post the message to trhe handler.
						PostMessage(_hwnd, WM_APP_CONTENT_ENABLER, 0, 0);
					}
					else
					{
						if (meType == MEEnablerProgress)
						{
							if (varEventData.vt == VT_LPWSTR)
							{
								TRACE((L"Progress: %s", varEventData.pwszVal));
							}
						}

						// End the event queue listener.
						_pMEG->BeginGetEvent(this, NULL);
					}
				}

				// Clean up.
				PropVariantClear(&varEventData);
				SAFE_RELEASE(pEvent);

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Called by the PMP session to start the enable action.
			/// </summary>
			/// <param name="pEnablerActivate">The player parameter.</param>
			/// <param name="pTopo">The player parameter.</param>
			/// <param name="pCallback">The player parameter.</param>
			/// <param name="punkState">The player parameter.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::BeginEnableContent(
				IMFActivate *pEnablerActivate,
				IMFTopology *pTopo,
				IMFAsyncCallback *pCallback,
				IUnknown *punkState)
			{
				HRESULT hr = S_OK;

				if (_pEnabler != NULL)
				{
					return E_FAIL;
				}

				// Create an async result for later use.
				hr = MFCreateAsyncResult(NULL, pCallback, punkState, &_pResult);

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Create the enabler from the IMFActivate pointer.
					hr = pEnablerActivate->ActivateObject(IID_IMFContentEnabler, (void**)&_pEnabler);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Notify the application. The application will call DoEnable from the app thread.
					_state = Enabler_Ready;
					PostMessage(_hwnd, WM_APP_CONTENT_ENABLER, 0, 0);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Completes the enable action.
			/// </summary>
			/// <param name="pResult">The media async result.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::EndEnableContent(IMFAsyncResult *pResult)
			{
				if (pResult == NULL)
				{
					// Invalid pointer.
					return E_POINTER;
				}

				// Release interfaces, so that we're ready to accept another call
				// to BeginEnableContent.
				SAFE_RELEASE(_pResult);
				SAFE_RELEASE(_pEnabler);
				SAFE_RELEASE(_pMEG);

				// Return the status.
				return _hrStatus;
			}

			/// <summary>
			/// On dispatch invoke handler.
			/// </summary>
			/// <param name="dispIdMember">The dispatch member id.</param>
			void ContentProtectionManager::OnDispatchInvoke(DISPID  dispIdMember)
			{
				if (dispIdMember == DISPID_ONQUIT)
				{
					// The user closed the browser window. Notify the application.
					PostMessage(_hwnd, WM_APP_BROWSER_DONE, 0, 0);
					_webDispatch.Exit();
				}
			}

			/// <summary>
			/// Does the enabler action.
			/// </summary>
			/// <param name="flags">If ForceNonSilent, then always use non-silent enable. Otherwise, use silent enable if possible.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::DoEnable(EnablerFlags flags)
			{
				HRESULT				hr = S_OK;
				BOOL				bAutomatic = FALSE;
				GUID				guidEnableType;

				// Get the enable type. (Just for logging. We don't use it.)
				hr = _pEnabler->GetEnableType(&guidEnableType);

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Log the enable type.
					LogEnableType(guidEnableType);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Query for the IMFMediaEventGenerator interface so that we can get the
					// enabler events.
					hr = _pEnabler->QueryInterface(IID_IMFMediaEventGenerator, (void**)&_pMEG);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Ask for the first event.
					hr = _pMEG->BeginGetEvent(this, NULL);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					if (flags == ForceNonSilent)
					{
						bAutomatic = FALSE;
					}
					else
					{
						// Decide whether to use silent or non-silent enabling. If flags is ForceNonSilent,
						// then we use non-silent. Otherwise, we query whether the enabler object supports 
						// silent enabling (also called "automatic" enabling).
						hr = _pEnabler->IsAutomaticSupported(&bAutomatic);
					}
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Start automatic or non-silent, depending.
					if (bAutomatic)
					{
						_state = Enabler_SilentInProgress;
						TRACE((L"Content enabler: Automatic is supported"));
						hr = _pEnabler->AutomaticEnable();
					}
					else
					{
						_state = Enabler_NonSilentInProgress;
						TRACE((L"Content enabler: Using non-silent enabling"));
						hr = DoNonSilentEnable();
					}
				}

				// If faild.
				if (FAILED(hr))
				{
					_hrStatus = hr;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Cancels the current action.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			/// During silent enable, this cancels the enable action in progress.
			/// During non-silent enable, this cancels the MonitorEnable thread.
			/// </remarks>
			HRESULT ContentProtectionManager::CancelEnable()
			{
				HRESULT hr = S_OK;
				if (_state != Enabler_Complete)
				{
					hr = _pEnabler->Cancel();
					
					// If failed.
					if (FAILED(hr))
					{
						// If Cancel fails for some reason, queue the MEEnablerCompleted
						// event ourselves. This will cause the current action to fail.
						_pMEG->QueueEvent(MEEnablerCompleted, GUID_NULL, hr, NULL);
					}
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Completes the current action.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			/// This method invokes the PMP session's callback.
			/// </remarks>
			HRESULT ContentProtectionManager::CompleteEnable()
			{
				_state = Enabler_Complete;

				// m_pResult can be NULL if the BeginEnableContent was not called.
				// This is the case when the application initiates the enable action, eg 
				// when MFCreatePMPMediaSession fails and returns an IMFActivate pointer.
				if (_pResult)
				{
					_pResult->SetStatus(_hrStatus);
					MFInvokeCallback(_pResult);
				}

				// All id good.
				return S_OK;
			}

			/// <summary>
			/// Performs non-silent enable.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT ContentProtectionManager::DoNonSilentEnable()
			{
				// Trust status for the URL.
				MF_URL_TRUST_STATUS	trustStatus = MF_LICENSE_URL_UNTRUSTED;

				WCHAR   *sURL = NULL;	    // Enable URL
				DWORD	cchURL = 0;         // Size of enable URL in characters.

				BYTE    *pPostData = NULL;  // Buffer to hold HTTP POST data.
				DWORD   cbPostDataSize = 0; // Size of buffer, in bytes.

				HRESULT hr = S_OK;

				// Get the enable URL. This is where we get the enable data for non-silent enabling.
				hr = _pEnabler->GetEnableURL(&sURL, &cchURL, &trustStatus);
				
				// If successful.
				if (SUCCEEDED(hr))
				{
					// Log trust status.
					LogTrustStatus(trustStatus);
				}

				// trust status.
				if (trustStatus != MF_LICENSE_URL_TRUSTED)
				{
					TRACE((L"The enabler URL is not trusted. Failing."));
					hr = E_FAIL;
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Start the thread that monitors the non-silent enable action. 
					hr = _pEnabler->MonitorEnable();
				}

				
				if (SUCCEEDED(hr))
				{
					// Get the HTTP POST data
					hr = _pEnabler->GetEnableData(&pPostData, &cbPostDataSize);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Initialize the browser control.
					hr = _webDispatch.Init((DispatchCallback*)this);
				}

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Open the URL and send the HTTP POST data.
					hr = _webDispatch.OpenURLWithData(sURL, pPostData, cbPostDataSize);
				}

				// Cleanup the resources.
				CoTaskMemFree(pPostData);
				CoTaskMemFree(sURL);

				// Return the result.
				return hr;
			}
		}
	}
}