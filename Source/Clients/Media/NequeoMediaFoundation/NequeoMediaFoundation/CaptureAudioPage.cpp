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
	ON_BN_CLICKED(IDC_CAPTURE_AUDIO_PATH_SELECT, &CaptureAudioPage::OnBnClickedButtonSelectFile)
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

				// Set the default audio configuration.
				CWnd *pSampleRate = GetDlgItem(IDC_CAPTURE_AUDIO_SAMPLERATE_TEXT);
				CWnd *pChannels = GetDlgItem(IDC_CAPTURE_AUDIO_CHANNELS_TEXT);
				CWnd *pBitsPerSample = GetDlgItem(IDC_CAPTURE_AUDIO_BITSPERSAMPLE_TEXT);
				CWnd *pBytesPerSecond = GetDlgItem(IDC_CAPTURE_AUDIO_BYTESPERSECOND_TEXT);

				if (pSampleRate != NULL)
					pSampleRate->SetWindowTextW(L"44100");

				if (pChannels != NULL)
					pChannels->SetWindowTextW(L"2");

				if (pBitsPerSample != NULL)
					pBitsPerSample->SetWindowTextW(L"16");

				if (pBytesPerSecond != NULL)
					pBytesPerSecond->SetWindowTextW(L"0");

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

			/// <summary>
			/// On select file button clicked.
			/// </summary>
			void CaptureAudioPage::OnBnClickedButtonSelectFile()
			{
				HRESULT hr = S_OK;

				WCHAR path[MAX_PATH];
				path[0] = L'\0';

				// Show the File Save dialog.
				CFileDialog dlgFile(FALSE);
				OPENFILENAME& ofn = dlgFile.GetOFN();
				ofn.lpstrFilter = L"Audio Media\0*.wav\0";
				ofn.lpstrFile = path;
				ofn.nMaxFile = MAX_PATH;

				// If open ok.
				if (dlgFile.DoModal() == IDOK)
				{
					// Get the file name.
					LPWSTR fileName = ofn.lpstrFile;

					// Set the duration.
					// Get the duration button handler.
					CWnd *pPath = GetDlgItem(IDC_CAPTURE_AUDIO_PATH_TEXT);
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