/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          WebDispatch.h
*  Purpose :       WebDispatch class.
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

#ifndef _WEBDISPATCH_H
#define _WEBDISPATCH_H

#include "MediaGlobal.h"
#include "PlayerState.h"

#include <exdisp.h>
#include <exdispid.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Web dispatch callback structure.
			/// DispatchCallback defines a callback for hooking the events from the
			/// browser control. (This is just to separate the browser code from the
			/// application logic.)
			/// </summary>
			struct DispatchCallback
			{
				/// <summary>
				/// On dispatch invoke handler.
				/// </summary>
				/// <param name="dispIdMember">The dispatch member id.</param>
				virtual void OnDispatchInvoke(DISPID  dispIdMember) = 0;
			};

			/// <summary>
			/// WebDispatch class hosts the InternetExplorer control and has a helper
			/// function for submitting HTTP POST data(see OpenURLWithData).
			/// The InternetExplorer controls sends events to the client through the
			/// DWebBrowserEvents2 dispinterface.We use this to catch the "browser
			/// window closed" event. 
			/// </summary>
			class WebDispatch : public IDispatch
			{
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				WebDispatch() : _pBrowser(NULL), _pCP(NULL), _hwnd(NULL), _pDispatchCB(NULL)
				{
				}

				/// <summary>
				/// This destructor.
				/// </summary>
				~WebDispatch()
				{
					Exit();
				}

				/// <summary>
				/// Initializes the InternetExplorer object.
				/// </summary>
				/// <param name="pCallback">The dispatch callback.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT Init(DispatchCallback *pCallback);

				/// <summary>
				/// Cleans up.
				/// </summary>
				void Exit();

				/// <summary>
				/// Navigates to a URL and POSTs the license acquisition data.
				/// </summary>
				/// <param name="pURL">The license acquisition URL.</param>
				/// <param name="pPostData">The license acquisition data.</param>
				/// <param name="cbData">The size of the data, in bytes..</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OpenURLWithData(const WCHAR *pURL, const BYTE *pPostData, DWORD cbData);

				/// <summary>
				/// Get the player reference for the reference id.
				/// </summary>
				/// <param name="riid">The player reference id.</param>
				/// <param name="ppv">The current player reference.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP QueryInterface(REFIID riid, void **ppv)
				{
					if (ppv == NULL)
					{
						return E_POINTER;
					}
					if (riid == IID_IUnknown)
					{
						// Assign the default handler.
						*ppv = (IUnknown*)this;
					}
					else if (riid == IID_IDispatch)
					{
						// Assign this dispatch handler.
						*ppv = (IDispatch*)this;
					}
					else
					{
						// Specifically do *not* expose DWebBrowserEvents2,
						// instead the caller must go through IDispatch.
						return E_NOINTERFACE;
					}

					// Add to the reference counter.
					AddRef();

					// Return all is good.
					return S_OK;
				}

				//// <summary>
				/// Add a new player ref item.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) AddRef() { return 1; }

				/// <summary>
				/// Release this player resources.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) Release() { return 2; }

				/// <summary>
				/// Get the dispatch id names.
				/// </summary>
				/// <param name="riid"></param>
				/// <param name="char"></param>
				/// <param name="int"></param>
				/// <param name="disp"></param>
				/// <returns>Not implemented.</returns>
				STDMETHODIMP GetIDsOfNames(REFIID, OLECHAR FAR* FAR*, unsigned int, LCID, DISPID FAR*)
				{
					return E_NOTIMPL;
				}

				/// <summary>
				/// Get the type information.
				/// </summary>
				/// <param name="int"></param>
				/// <param name="word"></param>
				/// <param name="type info"></param>
				/// <returns>Not implemented.</returns>
				STDMETHODIMP GetTypeInfo(unsigned int, LCID, ITypeInfo FAR* FAR*)
				{
					return E_NOTIMPL;
				}

				/// <summary>
				/// Get the type information count.
				/// </summary>
				/// <param name="pctinfo">The info pointer.</param>
				/// <returns>The result.</returns>
				STDMETHODIMP GetTypeInfoCount(unsigned int FAR* pctinfo)
				{
					// If pointer is null.
					if (pctinfo == NULL)
					{
						// Invalid pointer.
						return E_POINTER;
					}
					else
					{
						// Assign the pointer to zero.
						*pctinfo = 0;

						// Return all is good.
						return S_OK;
					}
				}

				/// <summary>
				/// Invoke the dispatch handler.
				/// </summary>
				/// <param name="dispIdMember">The dspatch member id.</param>
				/// <param name="refid"></param>
				/// <param name="id"></param>
				/// <param name="word"></param>
				/// <param name="parm"></param>
				/// <param name="variant"></param>
				/// <param name="info"></param>
				/// <param name="int"></param>
				/// <returns>The result.</returns>
				STDMETHODIMP Invoke(
					DISPID  dispIdMember, REFIID, LCID, WORD,
					DISPPARAMS FAR*, VARIANT FAR*, EXCEPINFO FAR*, unsigned int FAR*)
				{
					// If the dispatch exists.
					if (_pDispatchCB)
					{
						// Invoke the dispatch handler.
						_pDispatchCB->OnDispatchInvoke(dispIdMember);
					}

					// Return all is good.
					return S_OK;
				}

			private:
				IWebBrowser2		*_pBrowser;
				IConnectionPoint	*_pCP;				// Connection point to receive events from the control.
				DWORD				_dwCookie;			// Connection point identifier.

				DispatchCallback    *_pDispatchCB;		// Callback to handle browser events.
				HWND	_hwnd;
			};
		}
	}
}
#endif