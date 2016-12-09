/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CaptureScreenPage.cpp
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

#include "stdafx.h"

#include "CaptureScreenPage.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::CaptureScreenPage, CDialog)
	ON_BN_CLICKED(IDC_CAPTURE_SCREEN_PATH_SELECT, &CaptureScreenPage::OnBnClickedButtonSelectFile)
	ON_BN_CLICKED(IDC_CAPTURE_SCREEN_START, &CaptureScreenPage::OnBnClickedButtonStartCapture)
	ON_BN_CLICKED(IDC_CAPTURE_SCREEN_CHECK, &CaptureScreenPage::OnBnClickedCaptureScreenCheck)
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			IMPLEMENT_DYNAMIC(CaptureScreenPage, CDialog)

				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				CaptureScreenPage::CaptureScreenPage(CWnd* pParent)
				: CDialog(CaptureScreenPage::IDD, pParent),
				_disposed(false),
				_toolTip(NULL),
				_hwndEvent(NULL),
				_hwndScalling(NULL),
				_windowState(DisallowScreenCapture),
				_imageIndex(0)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			CaptureScreenPage::~CaptureScreenPage()
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
			void CaptureScreenPage::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
			}


			/// <summary>
			/// Set the event handler.
			/// </summary>
			/// <param name="hwndEvent">The handle to the window that gets the notification.</param>
			void CaptureScreenPage::SetEventHandler(HWND hwndEvent)
			{
				_hwndEvent = hwndEvent;
			}

			/// <summary>
			/// On initialize dialog.
			/// </summary>
			BOOL CaptureScreenPage::OnInitDialog()
			{
				CDialog::OnInitDialog();

				// Create the ToolTip control.
				_toolTip = new CToolTipCtrl();
				_toolTip->Create(this);

				// Assign the tool tip.
				_toolTip->AddTool(GetDlgItem(IDC_CAPTURE_SCREEN_PATH_SELECT), _T("Select the file and path of the screen capture location."));
				_toolTip->AddTool(GetDlgItem(IDC_CAPTURE_SCREEN_START), _T("Capture the screen."));

				// Activate
				_toolTip->Activate(TRUE);

				// Set the default audio configuration.
				CWnd *pSourceX = GetDlgItem(IDC_CAPTURE_SCREEN_SOURCE_X_TEXT);
				CWnd *pSourceY = GetDlgItem(IDC_CAPTURE_SCREEN_SOURCE_Y_TEXT);
				CWnd *pWidth = GetDlgItem(IDC_CAPTURE_SCREEN_WIDTH_TEXT);
				CWnd *pHeight = GetDlgItem(IDC_CAPTURE_SCREEN_HIEGHT_TEXT);

				if (pSourceX != NULL)
					pSourceX->SetWindowTextW(std::to_wstring(GetSystemMetrics(SM_XVIRTUALSCREEN)).c_str());

				if (pSourceY != NULL)
					pSourceY->SetWindowTextW(std::to_wstring(GetSystemMetrics(SM_YVIRTUALSCREEN)).c_str());

				if (pWidth != NULL)
					pWidth->SetWindowTextW(std::to_wstring(GetSystemMetrics(SM_CXSCREEN)).c_str());

				if (pHeight != NULL)
					pHeight->SetWindowTextW(std::to_wstring(GetSystemMetrics(SM_CYSCREEN)).c_str());

				CButton *pCaptureChecked = (CButton*)GetDlgItem(IDC_CAPTURE_SCREEN_CHECK);
				if (pCaptureChecked != NULL) pCaptureChecked->SetCheck(BST_CHECKED);

				CWnd *pScaleWidth = GetDlgItem(IDC_CAPTURE_SCREEN_SCALE_WIDTH_TEXT);
				CWnd *pScaleHeight = GetDlgItem(IDC_CAPTURE_SCREEN_SCALE_HEIGHT_TEXT);

				if (pScaleWidth != NULL)
					pScaleWidth->SetWindowTextW(std::to_wstring(GetSystemMetrics(SM_CXSCREEN)).c_str());

				if (pScaleHeight != NULL)
					pScaleHeight->SetWindowTextW(std::to_wstring(GetSystemMetrics(SM_CYSCREEN)).c_str());

				CButton *pScaleChecked = (CButton*)GetDlgItem(IDC_CAPTURE_SCREEN_SCALLING_CHECK);
				if (pScaleChecked != NULL) pScaleChecked->SetCheck(BST_UNCHECKED);

				// Assign the scalling hwnd.
				CWnd *pScallingPicture = GetDlgItem(IDC_CAPTURE_SCREEN_HWND);
				if (pScallingPicture != NULL) _hwndScalling = pScallingPicture->m_hWnd;

				// return TRUE  unless you set the focus to a control.
				return TRUE;
			}

			/// <summary>
			/// Pre-translate message.
			/// </summary>
			/// <param name="pMsg">The message.</param>
			BOOL CaptureScreenPage::PreTranslateMessage(MSG* pMsg)
			{
				if (_toolTip != NULL)
					_toolTip->RelayEvent(pMsg);

				return CDialog::PreTranslateMessage(pMsg);
			}

			/// <summary>
			/// On select file button clicked.
			/// </summary>
			void CaptureScreenPage::OnBnClickedButtonSelectFile()
			{
				HRESULT hr = S_OK;

				WCHAR path[MAX_PATH];
				path[0] = L'\0';

				// Show the File Save dialog.
				CFileDialog dlgFile(FALSE);
				OPENFILENAME& ofn = dlgFile.GetOFN();
				ofn.lpstrFilter = L"Screen Media\0*.bmp\0";
				ofn.lpstrFile = path;
				ofn.nMaxFile = MAX_PATH;

				// If open ok.
				if (dlgFile.DoModal() == IDOK)
				{
					// Get the file name.
					LPWSTR fileName = ofn.lpstrFile;

					// Set the duration.
					// Get the duration button handler.
					CWnd *pPath = GetDlgItem(IDC_CAPTURE_SCREEN_PATH_TEXT);
					if (pPath != NULL)
						pPath->SetWindowTextW(fileName);

					// If the file has been created.
					if (SUCCEEDED(hr))
					{
						EnableControls();
					}
					else
					{
						// Display a message box with the error.
						MessageBox((LPCWSTR)L"Unable to save the file.", (LPCWSTR)L"Error", MB_OK | MB_ICONERROR);
					}
				}
			}

			/// <summary>
			/// On start capture button clicked.
			/// </summary>
			void CaptureScreenPage::OnBnClickedButtonStartCapture()
			{
				CString filePath;

				// Get the file path.
				CWnd *pPath = GetDlgItem(IDC_CAPTURE_SCREEN_PATH_TEXT);
				if (pPath != NULL)
				{
					pPath->GetWindowTextW(filePath);
					std::wstring file = filePath;

					// Default extension.
					std::wstring extension = L".bmp";

					// If an extension has been found.
					if (file.find_last_of(L".") != std::wstring::npos)
					{
						// Get the extension.
						extension = L"." + file.substr(file.find_last_of(L".") + 1);
					}

					// Set the extension name.
					extension = L"_" + std::to_wstring(_imageIndex++) + extension;

					// Get the file name without extension.
					size_t lastdot = file.find_last_of(L".");
					std::wstring fileNew = file + extension;

					// Extension exists.
					if (lastdot != std::wstring::npos)
					{
						// New file name.
						fileNew = file.substr(0, lastdot) + extension;
					}

					// Capture to file.
					ScreenCapture captureScreen(this->m_hWnd);

					CButton *pCaptureChecked = (CButton*)GetDlgItem(IDC_CAPTURE_SCREEN_CHECK);
					CButton *pScaleChecked = (CButton*)GetDlgItem(IDC_CAPTURE_SCREEN_SCALLING_CHECK);

					if (pCaptureChecked != NULL)
					{
						// Get check box state.
						int checked = pCaptureChecked->GetCheck();

						// If checked.
						if (checked == BST_CHECKED)
						{
							// If scalling.
							if (pScaleChecked != NULL)
							{
								// Get check box state.
								int checkedScale = pScaleChecked->GetCheck();

								// If checked.
								if (checkedScale == BST_CHECKED)
								{
									CString widthScale;
									CString heightScale;

									CWnd *pWidth = GetDlgItem(IDC_CAPTURE_SCREEN_SCALE_WIDTH_TEXT);
									CWnd *pHeight = GetDlgItem(IDC_CAPTURE_SCREEN_SCALE_HEIGHT_TEXT);

									if (pWidth != NULL) pWidth->GetWindowTextW(widthScale);
									if (pHeight != NULL) pHeight->GetWindowTextW(heightScale);

									// Enable scalling
									captureScreen.SetIsScalling(true);
									captureScreen.Scale(_ttoi(widthScale), _ttoi(heightScale));
								}
								else if (checkedScale == BST_UNCHECKED)
								{
									// No scalling
									captureScreen.SetIsScalling(false);
								}
							}

							// Capture all the screens.
							captureScreen.ImageToFile(fileNew.c_str());
						}
						else if (checked == BST_UNCHECKED)
						{
							CString sourceX;
							CString sourceY;
							CString width;
							CString height;

							// Get the start button handler.
							CWnd *pSourceX = GetDlgItem(IDC_CAPTURE_SCREEN_SOURCE_X_TEXT);
							CWnd *pSourceY = GetDlgItem(IDC_CAPTURE_SCREEN_SOURCE_Y_TEXT);
							CWnd *pWidth = GetDlgItem(IDC_CAPTURE_SCREEN_WIDTH_TEXT);
							CWnd *pHeight = GetDlgItem(IDC_CAPTURE_SCREEN_HIEGHT_TEXT);

							if (pSourceX != NULL) pSourceX->GetWindowTextW(sourceX);
							if (pSourceY != NULL) pSourceY->GetWindowTextW(sourceY);
							if (pWidth != NULL) pWidth->GetWindowTextW(width);
							if (pHeight != NULL) pHeight->GetWindowTextW(height);

							// If scalling.
							if (pScaleChecked != NULL)
							{
								// Get check box state.
								int checkedScale = pScaleChecked->GetCheck();

								// If checked.
								if (checkedScale == BST_CHECKED)
								{
									CString widthScale;
									CString heightScale;

									CWnd *pWidth = GetDlgItem(IDC_CAPTURE_SCREEN_SCALE_WIDTH_TEXT);
									CWnd *pHeight = GetDlgItem(IDC_CAPTURE_SCREEN_SCALE_HEIGHT_TEXT);

									if (pWidth != NULL) pWidth->GetWindowTextW(widthScale);
									if (pHeight != NULL) pHeight->GetWindowTextW(heightScale);

									// Enable scalling
									captureScreen.SetIsScalling(true);
									captureScreen.Scale(_ttoi(widthScale), _ttoi(heightScale));
								}
								else if (checkedScale == BST_UNCHECKED)
								{
									// No scalling
									captureScreen.SetIsScalling(false);
								}
							}

							// Capture specific screen.
							captureScreen.ImageToFile(fileNew.c_str(), _ttoi(width), _ttoi(height), _ttoi(sourceX), _ttoi(sourceY));
						}
					}
				}
			}

			/// <summary>
			/// Enable controls.
			/// </summary>
			void CaptureScreenPage::EnableControls()
			{
				// Get the file path.
				CWnd *pStart = GetDlgItem(IDC_CAPTURE_SCREEN_START);
				if (pStart != NULL) 
					pStart->EnableWindow(true);
				else
					pStart->EnableWindow(false);
			}

			/// <summary>
			/// On check box clicked.
			/// </summary>
			void CaptureScreenPage::OnBnClickedCaptureScreenCheck()
			{
				CButton *pCaptureChecked = (CButton*)GetDlgItem(IDC_CAPTURE_SCREEN_CHECK);
				if (pCaptureChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						_windowState = DisallowScreenCapture;
						NotifyState();
					}
					else if (checked == BST_CHECKED)
					{
						_windowState = AllowScreenCapture;
						NotifyState();
					}
				}
			}
		}
	}
}