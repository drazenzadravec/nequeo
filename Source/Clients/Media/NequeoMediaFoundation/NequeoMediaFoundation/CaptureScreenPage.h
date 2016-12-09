/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CaptureScreenPage.h
*  Purpose :       CaptureScreenPage class.
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

#include "MediaGlobal.h"
#include "PlayerState.h"
#include "ScreenCapture.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media capture screen tab.
			/// </summary>
			class CaptureScreenPage : public CDialog
			{
				DECLARE_DYNAMIC(CaptureScreenPage)

				/// <summary>
				/// Media capture screen enum.
				/// </summary>
				enum { IDD = IDD_MEDIACAPTURE_SCREEN_TAB };

			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				CaptureScreenPage(CWnd* pParent = NULL);

				/// <summary>
				/// This destructor.
				/// </summary>
				virtual ~CaptureScreenPage();

				/// <summary>
				/// Set the event handler.
				/// </summary>
				/// <param name="hwndEvent">The handle to the window that gets the notification.</param>
				void SetEventHandler(HWND hwndEvent);

				/// <summary>
				/// On initialize dialog.
				/// </summary>
				BOOL OnInitDialog() override;

				/// <summary>
				/// Pre-translate message.
				/// </summary>
				/// <param name="pMsg">The message.</param>
				BOOL PreTranslateMessage(MSG* pMsg) override;

			protected:
				/// <summary>
				/// Notifies the application when the state changes.
				/// </summary>
				void NotifyState()
				{
					// Send state info.
					::PostMessage(_hwndEvent, WM_AUDIO_EVENT, (WPARAM)_windowState, (LPARAM)0);
				}

				/// <summary>
				/// Data exchange.
				/// </summary>
				/// <param name="pDX">Data exchange instance.</param>
				virtual void DoDataExchange(CDataExchange* pDX);

				/// <summary>
				/// Declare the message map.
				/// </summary>
				DECLARE_MESSAGE_MAP()
				afx_msg void OnBnClickedButtonSelectFile();
				afx_msg void OnBnClickedButtonStartCapture();
				afx_msg void OnBnClickedCaptureScreenCheck();

			private:
				bool _disposed;
				int _imageIndex;

				CToolTipCtrl *_toolTip;

				HWND _hwndEvent;
				HWND _hwndScalling;
				CaptureScreenState _windowState;

				void EnableControls();
			};
		}
	}
}