/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContentEnabler.h
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

#pragma once

#ifndef _CONTENTENABLER_H
#define _CONTENTENABLER_H

#include "MediaGlobal.h"
#include "PlayerState.h"
#include "WebDispatch.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// This object implements IMFContentProtectionManager. The PMP media
			/// session uses this interface to pass a content enabler object back
			/// to the application. A content enabler in an object that performs some 
			/// action needed to play a protected file, such as license acquistion.
			/// For more information about content enablers, see IMFContentEnabler in
			/// the Media Foundation SDK documentation.
			/// </summary>
			class ContentProtectionManager :
				public IMFAsyncCallback,
				public IMFContentProtectionManager,
				public DispatchCallback // To get callbacks from the browser control.
			{
			public:
				/// <summary>
				/// Static class method to create the ContentProtectionManager object.
				/// </summary>
				/// <param name="hNotify">Handle to the application window to receive notifications.</param>
				/// <param name="ppManager">Receives an AddRef's pointer to the ContentProtectionManager object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				static HRESULT CreateInstance(HWND hNotify, ContentProtectionManager **ppManager);

				/// <summary>
				/// Add a new player ref item.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) AddRef();

				/// <summary>
				/// Release this player resources.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) Release();

				/// <summary>
				/// Get the player reference for the reference id.
				/// </summary>
				/// <param name="iid">The player reference id.</param>
				/// <param name="ppv">The current player reference.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP QueryInterface(REFIID iid, void** ppv);

				/// <summary>
				/// Get the player parameters.
				/// </summary>
				/// <param name="p1">The player parameter.</param>
				/// <param name="p2">The player parameter.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP GetParameters(DWORD*, DWORD*)
				{
					// Implementation of this method is optional.
					return E_NOTIMPL;
				}

				/// <summary>
				/// Callback for asynchronous BeginGetEvent method.
				/// </summary>
				/// <param name="pAsyncResult">The pointer to the result.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP Invoke(IMFAsyncResult* pAsyncResult);

				/// <summary>
				/// Called by the PMP session to start the enable action.
				/// </summary>
				/// <param name="pEnablerActivate">The player parameter.</param>
				/// <param name="pTopo">The player parameter.</param>
				/// <param name="pCallback">The player parameter.</param>
				/// <param name="punkState">The player parameter.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP BeginEnableContent(
					IMFActivate *pEnablerActivate,
					IMFTopology *pTopo,
					IMFAsyncCallback *pCallback,
					IUnknown *punkState);

				/// <summary>
				/// Completes the enable action.
				/// </summary>
				/// <param name="pResult">The media async result.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP EndEnableContent(IMFAsyncResult *pResult);

				/// <summary>
				/// On dispatch invoke handler.
				/// </summary>
				/// <param name="dispIdMember">The dispatch member id.</param>
				void OnDispatchInvoke(DISPID  dispIdMember);

				/// <summary>
				/// Does the enabler action.
				/// </summary>
				/// <param name="flags">If ForceNonSilent, then always use non-silent enable. Otherwise, use silent enable if possible.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT			DoEnable(EnablerFlags flags = SilentOrNonSilent);

				/// <summary>
				/// Cancels the current action.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				/// <remarks>
				/// During silent enable, this cancels the enable action in progress.
				/// During non-silent enable, this cancels the MonitorEnable thread.
				/// </remarks>
				HRESULT			CancelEnable();

				/// <summary>
				/// Completes the current action.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				/// <remarks>
				/// This method invokes the PMP session's callback.
				/// </remarks>
				HRESULT			CompleteEnable();

				/// <summary>
				/// Get the current state of the media protection.
				/// </summary>
				/// <returns>The media state.</returns>
				EnablerState	GetState() const { return _state; }

				/// <summary>
				/// Get the current status of the media protection.
				/// </summary>
				/// <returns>The medaia status.</returns>
				HRESULT			GetStatus() const { return _hrStatus; }

			private:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hwndNotify">The handle to the hotifer.</param>
				ContentProtectionManager(HWND hwndNotify);

				/// <summary>
				/// This destructor. call release to cleanup resources.
				/// </summary>
				virtual ~ContentProtectionManager();

				/// <summary>
				/// Performs non-silent enable.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT DoNonSilentEnable();


				long                    _nRefCount;        // Reference count.

				EnablerState			_state;
				HRESULT					_hrStatus;         // Status code from the most recent event.

				HWND					_hwnd;

				IMFContentEnabler		*_pEnabler;        // Content enabler.
				IMFMediaEventGenerator	*_pMEG;            // The content enabler's event generator interface.
				IMFAsyncResult          *_pResult;         // Asynchronus result object.

				WebDispatch				_webDispatch;      // For non-silent enable

			};
		}
	}
}
#endif