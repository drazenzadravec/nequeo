/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          InputControlEventHandler.h
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

namespace Nequeo
{
	namespace Math
	{
		// Math input control id.
		static const int MATHINPUTCONTROL_SINK_ID = 1;

		///	<summary>
		///	Math Input Control Event Handler.
		///	</summary>
		template <class T>
		class ATL_NO_VTABLE CMathInputControlEventHandler :
			public IDispEventSimpleImpl<MATHINPUTCONTROL_SINK_ID, CMathInputControlEventHandler<T>, &__uuidof(_IMathInputControlEvents)>
		{
		private:
			IUnknown* _pUnknown;
			ULONG _ulAdviseCount;

		public:
			///	<summary>
			///	Math Input Control Event Handler on insert.
			///	</summary>
			static const _ATL_FUNC_INFO OnMICInsertInfo; // = {CC_STDCALL, VT_I4, 1, {VT_BSTR}};

			///	<summary>
			///	Math Input Control Event Handler on close.
			///	</summary>
			static const _ATL_FUNC_INFO OnMICCloseInfo;  // = {CC_STDCALL, VT_I4, 0, {VT_EMPTY}};

			// Event method dispatcher.
			BEGIN_SINK_MAP(CMathInputControlEventHandler)
				SINK_ENTRY_INFO(MATHINPUTCONTROL_SINK_ID, __uuidof(_IMathInputControlEvents), DISPID_MICInsert, OnMICInsert, const_cast<_ATL_FUNC_INFO*>(&OnMICInsertInfo))
				SINK_ENTRY_INFO(MATHINPUTCONTROL_SINK_ID, __uuidof(_IMathInputControlEvents), DISPID_MICClose, OnMICClose, const_cast<_ATL_FUNC_INFO*>(&OnMICCloseInfo))
			END_SINK_MAP()

			///	<summary>
			///	Math Input Control Event Handler Initialize. 
			///	</summary>
			HRESULT Initialize(IUnknown *pUnknown)
			{
				_pUnknown = pUnknown;
				_ulAdviseCount = 0;
				return S_OK;
			}

			///	<summary>
			///	Math Input Control Event Handler On Insert. 
			///	</summary>
			HRESULT __stdcall OnMICInsert(BSTR bstrRecoResult)
			{
				CComQIPtr<IMathInputControl> spMIC(_pUnknown);
				HRESULT hr = S_OK;
				if (spMIC)
				{
					OnInsertRecognitionResult(bstrRecoResult);
					hr = spMIC->Hide();
					return hr;
				}
				return E_FAIL;
			}

			///	<summary>
			///	Math Input Control Event Handler On Close. 
			///	</summary>
			HRESULT __stdcall OnMICClose()
			{
				CComPtr<IMathInputControl> spMIC;
				HRESULT hr = _pUnknown->QueryInterface<IMathInputControl>(&spMIC);
				if (SUCCEEDED(hr))
				{
					hr = spMIC->Hide();
					return hr;
				}
				return hr;
			}

			///	<summary>
			///	Math Input Control Event Handler On Insert Recognition Result. 
			///	</summary>
			virtual void OnInsertRecognitionResult(BSTR bstrRecoResult);
		};
	}
}
