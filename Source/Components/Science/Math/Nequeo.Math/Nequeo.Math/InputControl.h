/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          InputControl.h
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

#include "micaut.h"
#include "micaut_i.c"

#include <atlbase.h>
#include <atlwin.h>

#include "InputControlEventHandler.h"

namespace Nequeo
{
	namespace Math
	{
		typedef std::function<void(BSTR)> OnInsertRecognitionResultHandler;
		typedef void(*OnInsertRecognitionResultFunction)(BSTR);
		delegate void OnInsertRecognitionResultCallback(BSTR);

		// Forward declare the class.
		class InputControl;

		// Setup the event handler.
		const _ATL_FUNC_INFO CMathInputControlEventHandler<InputControl>::OnMICInsertInfo = { CC_STDCALL, VT_I4, 1,{ VT_BSTR } };
		const _ATL_FUNC_INFO CMathInputControlEventHandler<InputControl>::OnMICCloseInfo = { CC_STDCALL, VT_I4, 0,{ VT_EMPTY } };

		///	<summary>
		///	Math Input Control.
		///	</summary>
		class InputControl : public CMathInputControlEventHandler<InputControl>
		{
		public:
			///	<summary>
			///	Math Input Control.
			///	</summary>
			InputControl();

			///	<summary>
			///	Math Input Control.
			///	</summary>
			virtual ~InputControl();

			///	<summary>
			///	Show the Math Input Control.
			///	</summary>
			void Show();

			///	<summary>
			///	Hide the Math Input Control.
			///	</summary>
			void Hide();

			///	<summary>
			///	Clear the Math Input Control.
			///	</summary>
			void Clear();

			///	<summary>
			///	Show extended button.
			///	</summary>
			/// <param name="extended">True to show; else false.</param>
			void EnableExtendedButtons(bool extended = true);

			///	<summary>
			///	Set caption text.
			///	</summary>
			/// <param name="caption">The caption.</param>
			void SetCaptionText(const std::wstring& caption);

			///	<summary>
			///	Set preview height
			///	</summary>
			/// <param name="height">The height.</param>
			void SetPreviewHeight(LONG height);

			///	<summary>
			///	Math Input Control Event Handler On Insert Recognition Result. 
			///	</summary>
			void OnInsertRecognitionResult(BSTR bstrRecoResult) override;

			///	<summary>
			///	Set the on insert recognition result function callback.
			///	</summary>
			/// <param name="handler">The on insert recognition result function callback.</param>
			void SetOnInsertRecognitionResultHandler(OnInsertRecognitionResultHandler handler);

			///	<summary>
			///	Set the on insert recognition result function callback.
			///	</summary>
			/// <param name="handler">The on insert recognition result function callback.</param>
			void SetOnInsertRecognitionResultFunction(OnInsertRecognitionResultFunction handler);

		private:
			bool _disposed;

			// Math Input Control
			CComPtr<IMathInputControl> _spMIC;
			HRESULT _hr;

			OnInsertRecognitionResultHandler _onInsertRecognitionResultHandler;
			OnInsertRecognitionResultFunction _onInsertRecognitionResultFunction;
		};

		///	<summary>
		///	Math Input Control.
		///	</summary>
		public ref class InputControlManaged sealed
		{
		public:
			///	<summary>
			///	Math Input Control.
			///	</summary>
			InputControlManaged();

			///	<summary>
			///	Math Input Control.
			///	</summary>
			~InputControlManaged();

			///	<summary>
			///	Math Input Control.
			///	</summary>
			!InputControlManaged();

			/// <summary>
			/// Notify application on incoming call.
			/// </summary>
			event System::EventHandler<System::String^>^ OnInsertRecognitionResult;

			///	<summary>
			///	Show the Math Input Control.
			///	</summary>
			void Show();

			///	<summary>
			///	Hide the Math Input Control.
			///	</summary>
			void Hide();

			///	<summary>
			///	Clear the Math Input Control.
			///	</summary>
			void Clear();

			///	<summary>
			///	Show extended button.
			///	</summary>
			/// <param name="extended">True to show; else false.</param>
			void EnableExtendedButtons(bool extended);

			///	<summary>
			///	Set caption text.
			///	</summary>
			/// <param name="caption">The caption.</param>
			void SetCaptionText(System::String^ caption);

			///	<summary>
			///	Set preview height
			///	</summary>
			/// <param name="height">The height.</param>
			void SetPreviewHeight(long height);

		private:
			bool _disposed;
			InputControl* _mathInputControl;

			System::Runtime::InteropServices::GCHandle _gchOnInsertRecognitionResult;
			void OnInsertRecognitionResult_Handler(BSTR prm);
		};

		///	<summary>
		///	Math Input Control.
		///	</summary>
		InputControlManaged::InputControlManaged() : _disposed(false), _mathInputControl(nullptr)
		{
			_mathInputControl = new InputControl();

			if (_mathInputControl != nullptr)
			{
				// Assign the handler and allocate memory.
				OnInsertRecognitionResultCallback^ onInsertRecognitionResultCallback = gcnew OnInsertRecognitionResultCallback(this, &InputControlManaged::OnInsertRecognitionResult_Handler);
				_gchOnInsertRecognitionResult = System::Runtime::InteropServices::GCHandle::Alloc(onInsertRecognitionResultCallback);

				// Get a CLS compliant pointer from our delegate
				System::IntPtr ipInsertRecognitionResult = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(onInsertRecognitionResultCallback);

				// Cast the pointer to the proper function ptr signature.
				OnInsertRecognitionResultFunction onInsertRecognitionResultFunction = static_cast<OnInsertRecognitionResultFunction>(ipInsertRecognitionResult.ToPointer());

				// Set the function handler.
				_mathInputControl->SetOnInsertRecognitionResultFunction(onInsertRecognitionResultFunction);
			}
		}

		///	<summary>
		///	Math Input Control.
		///	</summary>
		InputControlManaged::~InputControlManaged()
		{
			if (!_disposed)
			{
				// Cleanup the native classes.
				this->!InputControlManaged();

				_disposed = true;
				_gchOnInsertRecognitionResult.Free();
			}
		}

		///	<summary>
		///	Math Input Control.
		///	</summary>
		InputControlManaged::!InputControlManaged()
		{
			if (!_disposed)
			{
				// Clean-up.
				if (_mathInputControl != nullptr)
				{
					delete _mathInputControl;
					_mathInputControl = nullptr;
				}
			}
		}

		///	<summary>
		///	Show the Math Input Control.
		///	</summary>
		void InputControlManaged::Show()
		{
			if (_mathInputControl != nullptr)
			{
				_mathInputControl->Show();
			}
		}

		///	<summary>
		///	Hide the Math Input Control.
		///	</summary>
		void InputControlManaged::Hide()
		{
			if (_mathInputControl != nullptr)
			{
				_mathInputControl->Hide();
			}
		}

		///	<summary>
		///	Clear the Math Input Control.
		///	</summary>
		void InputControlManaged::Clear()
		{
			if (_mathInputControl != nullptr)
			{
				_mathInputControl->Clear();
			}
		}

		///	<summary>
		///	Show extended button.
		///	</summary>
		/// <param name="extended">True to show; else false.</param>
		void InputControlManaged::EnableExtendedButtons(bool extended)
		{
			if (_mathInputControl != nullptr)
			{
				_mathInputControl->EnableExtendedButtons(extended);
			}
		}

		///	<summary>
		///	Set caption text.
		///	</summary>
		/// <param name="caption">The caption.</param>
		void InputControlManaged::SetCaptionText(System::String^ caption)
		{
			if (_mathInputControl != nullptr)
			{
				using namespace System::Runtime::InteropServices;
				const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(caption)).ToPointer();
				std::wstring os = chars;
				Marshal::FreeHGlobal(System::IntPtr((void*)chars));

				// Set the caption.
				_mathInputControl->SetCaptionText(os);
			}
		}

		///	<summary>
		///	Set preview height
		///	</summary>
		/// <param name="height">The height.</param>
		void InputControlManaged::SetPreviewHeight(long height)
		{
			if (_mathInputControl != nullptr)
			{
				_mathInputControl->SetPreviewHeight((LONG)height);
			}
		}

		///	<summary>
		///	On insert recognition result handler.
		///	</summary>
		/// <param name="prm">The prm.</param>
		void InputControlManaged::OnInsertRecognitionResult_Handler(BSTR prm)
		{
			OnInsertRecognitionResult(this, gcnew System::String(prm));
		}
	}
}