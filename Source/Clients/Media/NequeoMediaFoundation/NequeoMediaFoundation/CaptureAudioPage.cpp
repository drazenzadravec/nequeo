/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CaptureAudioPage.cpp
*  Purpose :       CaptureAudioPage class.
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

#include "CaptureAudioPage.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::CaptureAudioPage, CDialog)
	ON_BN_CLICKED(IDC_CAPTURE_AUDIO_CHECK, &CaptureAudioPage::OnBnClickedCaptureAudioCheck)
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			IMPLEMENT_DYNAMIC(CaptureAudioPage, CDialog)

			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			CaptureAudioPage::CaptureAudioPage(CWnd* pParent)
				: CDialog(CaptureAudioPage::IDD, pParent), 
				_disposed(false),
				_toolTip(NULL),
				_windowState(DisallowAudioCapture)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			CaptureAudioPage::~CaptureAudioPage()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					// Clen up the tool tip.
					if (_toolTip != NULL)
						delete _toolTip;
				}
			}

			/// <summary>
			/// Data exchange.
			/// </summary>
			/// <param name="pDX">Data exchange instance.</param>
			void CaptureAudioPage::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
			}

			/// <summary>
			/// On check box clicked.
			/// </summary>
			void CaptureAudioPage::OnBnClickedCaptureAudioCheck()
			{
				CButton *pCaptureChecked = (CButton*)GetDlgItem(IDC_CAPTURE_AUDIO_CHECK);
				if (pCaptureChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						_windowState = DisallowAudioCapture;
						NotifyState();
					}
					else if (checked == BST_CHECKED)
					{
						_windowState = AllowAudioCapture;
						NotifyState();
					}
				}
			}

			/// <summary>
			/// Set the event handler.
			/// </summary>
			/// <param name="hwndEvent">The handle to the window that gets the notification.</param>
			void CaptureAudioPage::SetEventHandler(HWND hwndEvent)
			{
				_hwndEvent = hwndEvent;
			}

			/// <summary>
			/// On initialize dialog.
			/// </summary>
			BOOL CaptureAudioPage::OnInitDialog()
			{
				CDialog::OnInitDialog();

				// Create the ToolTip control.
				_toolTip = new CToolTipCtrl();
				_toolTip->Create(this);

				// Assign the tool tip.
				_toolTip->AddTool(GetDlgItem(IDC_CAPTURE_AUDIO_PATH_SELECT), _T("Select the file and path of the audio capture location."));

				// Activate
				_toolTip->Activate(TRUE);

				// return TRUE  unless you set the focus to a control.
				return TRUE;
			}

			/// <summary>
			/// Pre-translate message.
			/// </summary>
			/// <param name="pMsg">The message.</param>
			BOOL CaptureAudioPage::PreTranslateMessage(MSG* pMsg)
			{
				if (_toolTip != NULL)
					_toolTip->RelayEvent(pMsg);

				return CDialog::PreTranslateMessage(pMsg);
			}
		}
	}
}