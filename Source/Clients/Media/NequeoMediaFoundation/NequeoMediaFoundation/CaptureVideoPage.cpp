/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CaptureVideoPage.cpp
*  Purpose :       CaptureVideoPage class.
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

#include "CaptureVideoPage.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::CaptureVideoPage, CDialog)
	ON_BN_CLICKED(IDC_CAPTURE_VIDEO_CHECK, &CaptureVideoPage::OnBnClickedCaptureVideoCheck)
	ON_BN_CLICKED(IDC_CAPTURE_VIDEO_PATH_SELECT, &CaptureVideoPage::OnBnClickedButtonSelectFile)
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			IMPLEMENT_DYNAMIC(CaptureVideoPage, CDialog)

			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			CaptureVideoPage::CaptureVideoPage(CWnd* pParent)
				: CDialog(CaptureVideoPage::IDD, pParent), 
				_disposed(false),
				_toolTip(NULL),
				_windowState(DisallowVideoCapture)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			CaptureVideoPage::~CaptureVideoPage()
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
			void CaptureVideoPage::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
			}

			/// <summary>
			/// On check box clicked.
			/// </summary>
			void CaptureVideoPage::OnBnClickedCaptureVideoCheck()
			{
				CButton *pCaptureChecked = (CButton*)GetDlgItem(IDC_CAPTURE_VIDEO_CHECK);
				if (pCaptureChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						_windowState = DisallowVideoCapture;
						NotifyState();
					}
					else if (checked == BST_CHECKED)
					{
						_windowState = AllowVideoCapture;
						NotifyState();
					}
				}
			}

			/// <summary>
			/// Set the event handler.
			/// </summary>
			/// <param name="hwndEvent">The handle to the window that gets the notification.</param>
			void CaptureVideoPage::SetEventHandler(HWND hwndEvent)
			{
				_hwndEvent = hwndEvent;
			}

			/// <summary>
			/// On initialize dialog.
			/// </summary>
			BOOL CaptureVideoPage::OnInitDialog()
			{
				CDialog::OnInitDialog();

				// Create the ToolTip control.
				_toolTip = new CToolTipCtrl();
				_toolTip->Create(this);

				// Assign the tool tip.
				_toolTip->AddTool(GetDlgItem(IDC_CAPTURE_VIDEO_PATH_SELECT), _T("Select the file and path of the video capture location."));

				// Activate
				_toolTip->Activate(TRUE);

				// Set the default audio configuration.
				CWnd *pBitRate = GetDlgItem(IDC_CAPTURE_VIDEO_BITRATE_TEXT);
				CWnd *pFrameSizeW = GetDlgItem(IDC_CAPTURE_VIDEO_FRAMESIZE_W_TEXT);
				CWnd *pFrameSizeH = GetDlgItem(IDC_CAPTURE_VIDEO_FRAMESIZE_H_TEXT);
				CWnd *pFrameRateN = GetDlgItem(IDC_CAPTURE_VIDEO_FRAMERATE_N_TEXT);
				CWnd *pFrameRateD = GetDlgItem(IDC_CAPTURE_VIDEO_FRAMERATE_D_TEXT);

				if (pBitRate != NULL)
					pBitRate->SetWindowTextW(L"0");

				if (pFrameSizeW != NULL)
					pFrameSizeW->SetWindowTextW(L"320");

				if (pFrameSizeH != NULL)
					pFrameSizeH->SetWindowTextW(L"240");

				if (pFrameRateN != NULL)
					pFrameRateN->SetWindowTextW(L"30");

				if (pFrameRateD != NULL)
					pFrameRateD->SetWindowTextW(L"1");

				// return TRUE  unless you set the focus to a control.
				return TRUE;
			}

			/// <summary>
			/// Pre-translate message.
			/// </summary>
			/// <param name="pMsg">The message.</param>
			BOOL CaptureVideoPage::PreTranslateMessage(MSG* pMsg)
			{
				if (_toolTip != NULL)
					_toolTip->RelayEvent(pMsg);

				return CDialog::PreTranslateMessage(pMsg);
			}

			/// <summary>
			/// On select file button clicked.
			/// </summary>
			void CaptureVideoPage::OnBnClickedButtonSelectFile()
			{
				HRESULT hr = S_OK;

				WCHAR path[MAX_PATH];
				path[0] = L'\0';

				// Show the File Save dialog.
				CFileDialog dlgFile(FALSE);
				OPENFILENAME& ofn = dlgFile.GetOFN();
				ofn.lpstrFilter = L"Video Media\0*.wmv\0";
				ofn.lpstrFile = path;
				ofn.nMaxFile = MAX_PATH;

				// If open ok.
				if (dlgFile.DoModal() == IDOK)
				{
					// Get the file name.
					LPWSTR fileName = ofn.lpstrFile;

					// Set the duration.
					// Get the duration button handler.
					CWnd *pPath = GetDlgItem(IDC_CAPTURE_VIDEO_PATH_TEXT);
					if (pPath != NULL)
						pPath->SetWindowTextW(fileName);

					// If the file has been created.
					if (SUCCEEDED(hr))
					{
						//EnableControls();
					}
					else
					{
						// Display a message box with the error.
						MessageBox((LPCWSTR)L"Unable to save the file.", (LPCWSTR)L"Error", MB_OK | MB_ICONERROR);
					}
				}
			}
		}
	}
}

