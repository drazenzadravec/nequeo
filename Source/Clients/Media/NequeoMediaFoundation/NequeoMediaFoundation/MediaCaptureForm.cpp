/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaCaptureForm.cpp
*  Purpose :       MediaCaptureForm class.
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

#include "MediaCaptureForm.h"
#include "OpenNetworkUriForm.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::MediaCaptureForm, CDialog)
	ON_WM_CLOSE()
	ON_WM_DESTROY()
	ON_WM_CREATE()
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_MESSAGE(ON_WM_VIDEO_EVENT, &MediaCaptureForm::OnNotifyVideoState)
	ON_MESSAGE(ON_WM_AUDIO_EVENT, &MediaCaptureForm::OnNotifyAudioState)
	ON_MESSAGE(ON_WM_APP_NOTIFY, &MediaCaptureForm::OnNotifyState)
	ON_MESSAGE(ON_WM_APP_ERROR, &MediaCaptureForm::OnNotifyError)
	ON_BN_CLICKED(IDC_VIDEOPREVIEW_BUTTON, &MediaCaptureForm::OnBnClickedButtonVideoPreview)
	ON_BN_CLICKED(IDC_REFRESHDEVICE_BUTTON, &MediaCaptureForm::OnBnClickedButtonRefreshDevices)
	ON_CBN_SELCHANGE(IDC_VIDEODEVICE_COMBO, &MediaCaptureForm::OnCbnSelchangeCombVideo)
	ON_CBN_SELCHANGE(IDC_AUDIODEVICE_COMBO, &MediaCaptureForm::OnCbnSelchangeCombAudio)
	ON_NOTIFY(TCN_SELCHANGE, IDC_MEDIACAPTURE_TAB, &MediaCaptureForm::OnTcnSelchangeMediacaptureTab)
	ON_WM_SHOWWINDOW()
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			/// <param name="pParent">The parent window.</param>
			MediaCaptureForm::MediaCaptureForm(CWnd* pParent) : CDialog(MediaCaptureForm::IDD, pParent),
				_mediaCaptureVideo(NULL),
				_mediaCaptureAudio(NULL),
				_volume(NULL),
				_hEvent(NULL),
				_hCapture(NULL),
				_videoDevice(new CaptureDeviceParam()),
				_audioDevice(new CaptureDeviceParam()),
				_preview(NULL),
				_selectedIndexVideo(-1),
				_selectedIndexAudio(-1),
				_countVideo(0),
				_countAudio(0),
				_toolTip(NULL),
				_disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaCaptureForm::~MediaCaptureForm()
			{
				// If not disposed.
				if (!_disposed)
				{
					_disposed = true;

					// Clen up the tool tip.
					if (_toolTip != NULL)
						delete _toolTip;

					// If the media capture is not null.
					if (_mediaCaptureVideo != NULL)
					{
						// Release the media capture.
						_mediaCaptureVideo->Release();
						_mediaCaptureVideo = NULL;
					}

					// If the media capture is not null.
					if (_mediaCaptureAudio != NULL)
					{
						// Release the media capture.
						_mediaCaptureAudio->Release();
						_mediaCaptureAudio = NULL;
					}

					// If volume not null.
					if (_volume != NULL)
					{
						(void)_volume->EnableNotifications(FALSE);
						_volume->Release();
						_volume = NULL;
					}

					// If exists.
					if (_videoDevice)
					{
						// Safe release.
						Nequeo::Media::Foundation::MediaCapture::SafeReleaseCaptureDevices(_videoDevice);
						SAFE_DELETE(_videoDevice);
					}

					// If exists.
					if (_audioDevice)
					{
						// Safe release.
						Nequeo::Media::Foundation::MediaCapture::SafeReleaseCaptureDevices(_audioDevice);
						SAFE_DELETE(_audioDevice);
					}

					// If exists.
					if (_preview)
					{
						// Safe release.
						SAFE_DELETE(_preview);
						_preview = NULL;
					}
				}
			}

			/// <summary>
			/// On the show windows as a modal.
			/// </summary>
			INT_PTR MediaCaptureForm::DoModal()
			{
				HINSTANCE _hInstance = AfxGetResourceHandle();

				__try
				{
					HMODULE dllModule = ::GetModuleHandle(L"NequeoMediaFoundation.dll");
					AfxSetResourceHandle(dllModule);
					return CDialog::DoModal();
				}
				__finally
				{
					AfxSetResourceHandle(_hInstance);
				}
			}

			/// <summary>
			/// Data exchange.
			/// </summary>
			/// <param name="pDX">Data exchange instance.</param>
			void MediaCaptureForm::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
				DDX_Control(pDX, IDC_VIDEODEVICE_COMBO, _videoDeviceItem);
				DDX_Control(pDX, IDC_AUDIODEVICE_COMBO, _audioDeviceItem);
				DDX_Control(pDX, IDC_MEDIACAPTURE_TAB, _tab);
			}

			/// <summary>
			/// On initialize dialog.
			/// </summary>
			BOOL MediaCaptureForm::OnInitDialog()
			{
				CDialog::OnInitDialog();

				// Create the ToolTip control.
				_toolTip = new CToolTipCtrl();
				_toolTip->Create(this);

				// Assign the tool tip.
				_toolTip->AddTool(GetDlgItem(IDC_VIDEOPREVIEW_BUTTON), _T("Preview the capture video."));
				_toolTip->AddTool(GetDlgItem(IDC_REFRESHDEVICE_BUTTON), _T("Refresh the video and audio device list."));
				
				// Activate
				_toolTip->Activate(TRUE);

				// Create the tab control.
				_tab.InsertItem(0, L"Video");
				_tab.InsertItem(1, L"Audio");
				_tab.Init(*this, *this);

				// return TRUE  unless you set the focus to a control.
				return TRUE;  
			}

			/// <summary>
			/// Pre-translate message.
			/// </summary>
			/// <param name="pMsg">The message.</param>
			BOOL MediaCaptureForm::PreTranslateMessage(MSG* pMsg)
			{
				if (_toolTip != NULL)
					_toolTip->RelayEvent(pMsg);

				return CDialog::PreTranslateMessage(pMsg);
			}

			/// <summary>
			/// On close window.
			/// </summary>
			void MediaCaptureForm::OnClose()
			{
				// If the media capture is not null.
				if (_mediaCaptureVideo != NULL)
				{
					// Shut down the media capture.
					_mediaCaptureVideo->StopCapture();
				}

				// If the media capture is not null.
				if (_mediaCaptureAudio != NULL)
				{
					// Shut down the media capture.
					_mediaCaptureAudio->StopCapture();
				}

				CDialog::OnClose();
			}

			/// <summary>
			/// On destroy window.
			/// </summary>
			void MediaCaptureForm::OnDestroy()
			{
				CDialog::OnDestroy();

				// Post quit messsage. The will close the entire application.
				//PostQuitMessage(0);
			}

			/// <summary>
			/// On create window.
			/// </summary>
			/// <param name="lpCreateStruct">The create structure.</param>
			/// <returns>Zero if the window should be created; else -1 if the window should be distroyed</returns>
			int MediaCaptureForm::OnCreate(LPCREATESTRUCT lpCreateStruct)
			{
				if (CDialog::OnCreate(lpCreateStruct) == -1)
					return -1;

				// Get the window handlers where the video will be player
				// and the window the will handle the player evenets.
				_hCapture = lpCreateStruct->hwndParent;
				_hEvent = lpCreateStruct->hwndParent;

				// All is good create the window.
				return 0;
			}

			/// <summary>
			/// On show window.
			/// </summary>
			/// <param name="bShow">Show the window.</param>
			/// <param name="nStatus">The window status.</param>
			void MediaCaptureForm::OnShowWindow(BOOL bShow, UINT nStatus)
			{
				CDialog::OnShowWindow(bShow, nStatus);

				// Default is the main video.
				_hCapture = *this;
				_hEvent = *this;

				// Initialize the capture object.
				HRESULT hrVideo = MediaCapture::CreateInstance(_hCapture, _hEvent, &_mediaCaptureVideo);
				HRESULT hrAudio = MediaCapture::CreateInstance(_hCapture, _hEvent, &_mediaCaptureAudio);
			}

			/// <summary>
			/// On paint window.
			/// </summary>
			void MediaCaptureForm::OnPaint()
			{
				// Device context for painting.
				// Do not call CDialog::OnPaint() for painting messages.
				CPaintDC dc(this);
			}

			/// <summary>
			/// On size window.
			/// </summary>
			/// <param name="nType">The type.</param>
			/// <param name="cx">The width of the window.</param>
			/// <param name="cy">The height of the window.</param>
			void MediaCaptureForm::OnSize(UINT nType, int cx, int cy)
			{
				// On size the window.
				CDialog::OnSize(nType, cx, cy);
			}

			/// <summary>
			/// On notify video state.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaCaptureForm::OnNotifyVideoState(WPARAM wParam, LPARAM lParam)
			{
				// Get the state from the parameter.
				CaptureVideoState state = (CaptureVideoState)wParam;

				// Get the video page.
				const CaptureVideoPage& page = _tab.VideoPage();

				// Return all is good.
				return 0;
			}

			/// <summary>
			/// On notify audio state.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaCaptureForm::OnNotifyAudioState(WPARAM wParam, LPARAM lParam)
			{
				// Get the state from the parameter.
				CaptureAudioState state = (CaptureAudioState)wParam;

				// Get the audio page.
				const CaptureAudioPage& page = _tab.AudioPage();

				CButton *pCaptureChecked = (CButton*)page.GetDlgItem(IDC_CAPTURE_AUDIO_CHECK);
				if (pCaptureChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						int y = 0;
					}
					else if (checked == BST_CHECKED)
					{
						int x = 0;
					}
				}

				// Return all is good.
				return 0;
			}

			/// <summary>
			/// On notify state.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaCaptureForm::OnNotifyState(WPARAM wParam, LPARAM lParam)
			{
				// Get the state from the parameter.
				CaptureState state = (CaptureState)wParam;

				BOOL bWaiting = FALSE;
				BOOL bCapturingVideo = FALSE;
				BOOL bCapturingAudio = FALSE;

				// If the media capture is not null.
				if (_mediaCaptureVideo != NULL)
				{
					switch (state)
					{
					case CaptureNotReady:
						bWaiting = TRUE;
						bCapturingVideo = FALSE;
						break;

					case CaptureReady:
						bWaiting = FALSE;
						bCapturingVideo = FALSE;
						break;

					case CapturingVideo:
						bWaiting = FALSE;
						bCapturingVideo = TRUE;
						break;

					case NotCapturingVideo:
						bWaiting = TRUE;
						bCapturingVideo = FALSE;
						break;
					}

					// If capturing.
					if (bCapturingVideo == TRUE)
					{

					}
					else
					{
						// Not capturing.


					}
				}

				// If the media capture is not null.
				if (_mediaCaptureAudio != NULL)
				{
					switch (state)
					{
					case CaptureNotReady:
						bWaiting = TRUE;
						bCapturingAudio = FALSE;
						break;

					case CaptureReady:
						bWaiting = FALSE;
						bCapturingAudio = FALSE;
						break;

					case CapturingAudio:
						bWaiting = FALSE;
						bCapturingAudio = TRUE;
						break;

					case NotCapturingAudio:
						bWaiting = TRUE;
						bCapturingAudio = FALSE;
						break;
					}

					// If capturing.
					if (bCapturingAudio == TRUE)
					{

					}
					else
					{
						// Not capturing.


					}
				}

				// Return all is good.
				return 0;
			}

			/// <summary>
			/// On notify error.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaCaptureForm::OnNotifyError(WPARAM wParam, LPARAM lParam)
			{
				// Get the error from the parameter.
				HRESULT hrErr = (HRESULT)wParam;

				// Set the error message array.
				const size_t MESSAGE_LEN = 512;
				WCHAR message[MESSAGE_LEN];

				// Concantinate the error message.
				HRESULT hr = StringCchPrintf(message, MESSAGE_LEN, L"%s (HRESULT = 0x%X)", L"An error occurred.", hrErr);
				if (SUCCEEDED(hr))
				{
					// Display a message box with the error.
					MessageBox(message, (LPCWSTR)L"Error", MB_OK | MB_ICONERROR);
				}

				// Return all is good.
				return 0;
			}

			/// <summary>
			/// On video preview button clicked.
			/// </summary>
			void MediaCaptureForm::OnBnClickedButtonVideoPreview()
			{
				// Get the preview button handler.
				CWnd *pBtnPreview = GetDlgItem(IDC_VIDEOPREVIEW_BUTTON);
				if (pBtnPreview != NULL)
				{
					// If exists.
					if (_videoDevice)
					{
						// If video capture devices exists.
						if (_videoDevice->count > 0)
						{
							// Make sure the selection is valid.
							if (_selectedIndexVideo > -1 && _selectedIndexVideo < _countVideo)
							{
								// If exists.
								if (_preview)
								{
									// Safe release.
									SAFE_DELETE(_preview);
									_preview = NULL;
								}

								// Open the preview window.
								_preview = new MediaPreviewForm(_videoDevice->ppDevices[_selectedIndexVideo], L"Preview", this);
								_preview->ShowPreviewForm();
							}
						}
					}
				}
			}

			/// <summary>
			/// On refresh devices button clicked.
			/// </summary>
			void MediaCaptureForm::OnBnClickedButtonRefreshDevices()
			{
				// If exists.
				if (_videoDevice)
				{
					// Safe release.
					Nequeo::Media::Foundation::MediaCapture::SafeReleaseCaptureDevices(_videoDevice);
				}

				// If exists.
				if (_audioDevice)
				{
					// Safe release.
					Nequeo::Media::Foundation::MediaCapture::SafeReleaseCaptureDevices(_audioDevice);
				}

				// Clear the video and audio device list.
				// Delete every item from the combo box.
				for (int i = _videoDeviceItem.GetCount() - 1; i >= 0; i--)
				{
					_videoDeviceItem.DeleteString(i);
				}

				// Delete every item from the combo box.
				for (int i = _audioDeviceItem.GetCount() - 1; i >= 0; i--)
				{
					_audioDeviceItem.DeleteString(i);
				}

				// Load all video and audio devices.
				Nequeo::Media::Foundation::MediaCapture::GetVideoCaptureDevices(_videoDevice);
				Nequeo::Media::Foundation::MediaCapture::GetAudioCaptureDevices(_audioDevice);

				// If exists.
				if (_videoDevice)
				{
					// If video capture devices exists.
					if (_videoDevice->count > 0)
					{
						// Get the video device names.
						std::vector<std::wstring> videoNames = Nequeo::Media::Foundation::MediaCapture::GetDeviceNames(_videoDevice);

						// For each video device.
						for (UINT32 i = 0; i < _videoDevice->count; i++)
						{
							// Add the device name.
							_videoDeviceItem.AddString(videoNames[i].c_str());
						}
					}
				}

				// If exists.
				if (_audioDevice)
				{
					// If audio capture devices exists.
					if (_audioDevice->count > 0)
					{
						// Get the audio device names.
						std::vector<std::wstring> audioNames = Nequeo::Media::Foundation::MediaCapture::GetDeviceNames(_audioDevice);

						// For each audio device.
						for (UINT32 i = 0; i < _audioDevice->count; i++)
						{
							// Add the device name.
							_audioDeviceItem.AddString(audioNames[i].c_str());
						}
					}
				}

				// Enable or disable controls.
				EnableControls();
			}

			/// <summary>
			/// On selection changed video device.
			/// </summary>
			void MediaCaptureForm::OnCbnSelchangeCombVideo()
			{
				// Get the selected index.
				_selectedIndexVideo = _videoDeviceItem.GetCurSel();
				_countVideo = _videoDeviceItem.GetCount();

				// Get the preview button handler.
				CWnd *pBtnPreview = GetDlgItem(IDC_VIDEOPREVIEW_BUTTON);

				// Make sure the selection is valid.
				if (_selectedIndexVideo > -1 && _selectedIndexVideo < _countVideo)
				{
					// If exists.
					if (pBtnPreview != NULL)
					{
						// Enable.
						pBtnPreview->EnableWindow(true);
					}

					// If the media capture is not null.
					if (_mediaCaptureVideo != NULL)
					{
						// Set the video device.
						_mediaCaptureVideo->SetVideoDevice(_videoDevice->ppDevices[_selectedIndexVideo]);
					}
				}
				else
				{
					// If exists.
					if (pBtnPreview != NULL)
					{
						// Disable.
						pBtnPreview->EnableWindow(false);
					}

					// If the media capture is not null.
					if (_mediaCaptureVideo != NULL)
					{
						// Set the video device.
						_mediaCaptureVideo->SetVideoDevice(NULL);
					}
				}
			}

			/// <summary>
			/// On selection changed audio device.
			/// </summary>
			void MediaCaptureForm::OnCbnSelchangeCombAudio()
			{
				// Get the selected index.
				_selectedIndexAudio = _audioDeviceItem.GetCurSel();
				_countAudio = _audioDeviceItem.GetCount();

				// Make sure the selection is valid.
				if (_selectedIndexAudio > -1 && _selectedIndexAudio < _countAudio)
				{
					// If the media capture is not null.
					if (_mediaCaptureAudio != NULL)
					{
						// Set the audio device.
						_mediaCaptureAudio->SetAudioDevice(_audioDevice->ppDevices[_selectedIndexAudio]);
					}
				}
				else
				{
					// If the media capture is not null.
					if (_mediaCaptureAudio != NULL)
					{
						// Set the audio device.
						_mediaCaptureAudio->SetAudioDevice(NULL);
					}
				}
			}

			/// <summary>
			/// Enable or disable controls.
			/// </summary>
			void MediaCaptureForm::EnableControls()
			{
				// Get the preview button handler.
				CWnd *pBtnPreview = GetDlgItem(IDC_VIDEOPREVIEW_BUTTON);
				if (pBtnPreview != NULL)
					pBtnPreview->EnableWindow(false);
			}

			/// <summary>
			/// On tab page slection changed.
			/// </summary>
			void MediaCaptureForm::OnTcnSelchangeMediacaptureTab(NMHDR *pNMHDR, LRESULT *pResult)
			{
				// Get the selected index.
				int selectedIndex = _tab.GetCurSel();

				// Make sure the selection is valid.
				if (selectedIndex > -1 && selectedIndex < 2)
				{
					// Show current hide all others.
					_tab.ShowTabPage(selectedIndex);
				}

				// All is OK.
				*pResult = 0;
			}
		}
	}
}