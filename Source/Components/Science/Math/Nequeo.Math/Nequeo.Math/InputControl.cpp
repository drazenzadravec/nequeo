/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          InputControl.cpp
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

#include "stdafx.h"

#include "InputControl.h"

using namespace Nequeo::Math;

///	<summary>
///	Math Input Control.
///	</summary>
InputControl::InputControl() :
	_disposed(false), _hr(S_OK)
{
	// Init the COM.
	_hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	if (SUCCEEDED(_hr))
	{
		// Create the control instance.
		_hr = _spMIC.CoCreateInstance(CLSID_MathInputControl);
		if (SUCCEEDED(_hr))
		{
			// Initialize the event handler.
			_hr = CMathInputControlEventHandler<InputControl>::Initialize(_spMIC);
			if (SUCCEEDED(_hr)) 
			{
				// Start event dispatch.
				_hr = CMathInputControlEventHandler<InputControl>::DispEventAdvise(_spMIC);
			}
		}
	}
}

///	<summary>
///	Math Input Control.
///	</summary>
InputControl::~InputControl()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Show the Math Input Control.
///	</summary>
void InputControl::Show()
{
	if (SUCCEEDED(_hr))
	{
		_hr = _spMIC->Show();
	}
}

///	<summary>
///	Hide the Math Input Control.
///	</summary>
void InputControl::Hide()
{
	if (SUCCEEDED(_hr))
	{
		_hr = _spMIC->Hide();
	}
}

///	<summary>
///	Clear the Math Input Control.
///	</summary>
void InputControl::Clear()
{
	if (SUCCEEDED(_hr))
	{
		_spMIC->Clear();
	}
}

///	<summary>
///	Show extended button.
///	</summary>
/// <param name="extended">True to show; else false.</param>
void InputControl::EnableExtendedButtons(bool extended)
{
	if (SUCCEEDED(_hr))
	{
		if (extended)
		{
			_spMIC->EnableExtendedButtons(VARIANT_TRUE);
		}
		else
		{
			_spMIC->EnableExtendedButtons(VARIANT_FALSE);
		}
	}
}

///	<summary>
///	Set caption text.
///	</summary>
/// <param name="caption">The caption.</param>
void InputControl::SetCaptionText(const std::wstring& caption)
{
	if (SUCCEEDED(_hr))
	{
		CComBSTR cap(caption.c_str());
		_spMIC->SetCaptionText((BSTR)cap);
	}
}

///	<summary>
///	Set preview height
///	</summary>
/// <param name="height">The height.</param>
void InputControl::SetPreviewHeight(LONG height)
{
	if (SUCCEEDED(_hr))
	{
		_spMIC->SetPreviewHeight(height);
	}
}

///	<summary>
///	Math Input Control Event Handler On Insert Recognition Result. 
///	</summary>
void InputControl::OnInsertRecognitionResult(BSTR bstrRecoResult)
{
	if (_onInsertRecognitionResultHandler != nullptr)
	{
		_onInsertRecognitionResultHandler(bstrRecoResult);
	}
	else
	{
		_onInsertRecognitionResultFunction(bstrRecoResult);
	}
}

///	<summary>
///	Set the on insert recognition result function callback.
///	</summary>
/// <param name="handler">The on insert recognition result function callback.</param>
void InputControl::SetOnInsertRecognitionResultHandler(OnInsertRecognitionResultHandler handler)
{
	_onInsertRecognitionResultHandler = handler;
}

///	<summary>
///	Set the on insert recognition result function callback.
///	</summary>
/// <param name="handler">The on insert recognition result function callback.</param>
void InputControl::SetOnInsertRecognitionResultFunction(OnInsertRecognitionResultFunction handler)
{
	_onInsertRecognitionResultFunction = handler;
}