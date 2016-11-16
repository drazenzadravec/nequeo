/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPlayerForm.cpp
*  Purpose :       MediaPlayerForm class.
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

#include "MediaPlayerForm.h"
#include "OpenNetworkUriForm.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::MediaPlayerForm, CDialog)
	ON_WM_CLOSE()
	ON_WM_DESTROY()
	ON_WM_CREATE()
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_MESSAGE(ON_WM_APP_NOTIFY, &MediaPlayerForm::OnNotifyState)
	ON_MESSAGE(ON_WM_APP_ERROR, &MediaPlayerForm::OnNotifyError)
	ON_MESSAGE(ON_WM_AUDIO_EVENT, &MediaPlayerForm::OnVolumeChanged)
	ON_BN_CLICKED(IDC_BUTTONPLAY, &MediaPlayerForm::OnBnClickedButtonplay)
	ON_BN_CLICKED(IDC_BUTTONSTOP, &MediaPlayerForm::OnBnClickedButtonstop)
	ON_BN_CLICKED(IDC_BUTTONMUTE, &MediaPlayerForm::OnBnClickedButtonmute)
	ON_BN_CLICKED(IDC_BUTTONPAUSE, &MediaPlayerForm::OnBnClickedButtonpause)
	ON_BN_CLICKED(IDC_BUTTONCLOSE, &MediaPlayerForm::OnBnClickedButtonclose)
	ON_CBN_SELCHANGE(IDC_COMBOOPEN, &MediaPlayerForm::OnCbnSelchangeComboopen)
	ON_NOTIFY(TRBN_THUMBPOSCHANGING, IDC_SLIDERPOSITION, &MediaPlayerForm::OnTRBNThumbPosChangingSliderposition)
	ON_NOTIFY(NM_RELEASEDCAPTURE, IDC_SLIDERPOSITION, &MediaPlayerForm::OnNMReleasedcaptureSliderposition)
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
			MediaPlayerForm::MediaPlayerForm(CWnd* pParent) : CDialog(MediaPlayerForm::IDD, pParent), 
				_mediaPlayer(NULL), _repaintClient(TRUE), _volume(NULL), _toolTip(NULL), _hEvent(NULL),
				_disposed(false), _mute(false), _duration(0), _internalThread(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaPlayerForm::~MediaPlayerForm()
			{
				// If not disposed.
				if (!_disposed)
				{
					_disposed = true;

					// Stop the thread.
					StopGetCurrentPositionThread();

					if (_toolTip != NULL)
						delete _toolTip;

					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Release the media player.
						_mediaPlayer->Release();
						_mediaPlayer = NULL;
					}

					// If volume not null.
					if (_volume != NULL)
					{
						(void)_volume->EnableNotifications(FALSE);
						_volume->Release();
						_volume = NULL;
					}
				}
			}

			/// <summary>
			/// On the show windows as a modal.
			/// </summary>
			INT_PTR MediaPlayerForm::DoModal()
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
			void MediaPlayerForm::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
				DDX_Control(pDX, IDC_BUTTONMUTE, _muteButton);
				DDX_Control(pDX, IDC_COMBOOPEN, _openURL);
				DDX_Control(pDX, IDC_SLIDERPOSITION, _positionSlider);
			}

			/// <summary>
			/// On close window.
			/// </summary>
			void MediaPlayerForm::OnClose()
			{
				// If the media player is not null.
				if (_mediaPlayer != NULL)
				{
					// Shut down the media player.
					_mediaPlayer->Shutdown();
				}

				CDialog::OnClose();
			}

			/// <summary>
			/// On destroy window.
			/// </summary>
			void MediaPlayerForm::OnDestroy()
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
			int MediaPlayerForm::OnCreate(LPCREATESTRUCT lpCreateStruct)
			{
				if (CDialog::OnCreate(lpCreateStruct) == -1)
					return -1;

				// Get the window handlers where the video will be player
				// and the window the will handle the player evenets.
				HWND hVideo = lpCreateStruct->hwndParent;
				_hEvent = lpCreateStruct->hwndParent;

				// All is good create the window.
				return 0;
			}

			/// <summary>
			/// On show window.
			/// </summary>
			/// <param name="bShow">Show the window.</param>
			/// <param name="nStatus">The window status.</param>
			void MediaPlayerForm::OnShowWindow(BOOL bShow, UINT nStatus)
			{
				CDialog::OnShowWindow(bShow, nStatus);

				// Create the tool tip control.
				_toolTip = new CToolTipCtrl();
				if (_toolTip != NULL)
				{

				}

				// Set the slder position.
				_positionSlider.SetRange(0, 100);
				_positionSlider.SetPos(0);

				// Default is the main video.
				HWND hVideo = NULL;
				_hEvent = *this;

				// Get the play display handler.
				CWnd *pVideoDisplay = GetDlgItem(IDC_VIDEODISPLAY);
				if (pVideoDisplay != NULL)
				{
					hVideo = pVideoDisplay->m_hWnd;
				}

				// Initialize the player object.
				HRESULT hr = MediaPlayer::CreateInstance(hVideo, _hEvent, &_mediaPlayer);

				if (SUCCEEDED(hr))
				{
					// Also create the object that manages to audio session.
					// This can fail if the machine does not have an audio end-point.
					hr = Volume::CreateInstance(WM_AUDIO_EVENT, _hEvent, &_volume);

					if (SUCCEEDED(hr))
					{
						// Ask for audio session events.
						_volume->EnableNotifications(TRUE);
					}

					// All is good create the window.
					//return 0;
				}
				else
				{
					// Destroy the window.
					//return -1;
				}
			}

			/// <summary>
			/// On paint window.
			/// </summary>
			void MediaPlayerForm::OnPaint()
			{
				// Device context for painting.
				// Do not call CDialog::OnPaint() for painting messages.
				CPaintDC dc(this); 

				// If the media player is not null.
				if (_mediaPlayer != NULL)
				{
					// Repaint the client.
					if (!_repaintClient)
					{
						// If the video is playing. Ask the player to repaint.
						_mediaPlayer->Repaint();
					}
				}
			}

			/// <summary>
			/// On size window.
			/// </summary>
			/// <param name="nType">The type.</param>
			/// <param name="cx">The width of the window.</param>
			/// <param name="cy">The height of the window.</param>
			void MediaPlayerForm::OnSize(UINT nType, int cx, int cy)
			{
				CDialog::OnSize(nType, cx, cy);

				int playerControlSize = 80;

				// If the media player is not null.
				if (_mediaPlayer != NULL)
				{
					// Since the media player width and height are type WORD (unsigned short).
					if (cx > 0 && cy > playerControlSize)
					{
						// Resize the video on the control.
						_mediaPlayer->ResizeVideo((WORD)cx, (WORD)(cy - playerControlSize));
					}
				}
			}

			/// <summary>
			/// On notify state.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaPlayerForm::OnNotifyState(WPARAM wParam, LPARAM lParam)
			{
				// Get the state from the parameter.
				PlayerState state = (PlayerState)wParam;

				BOOL bWaiting = FALSE;
				BOOL bPlayback = FALSE;

				// If the media player is not null.
				if (_mediaPlayer != NULL)
				{
					switch (state)
					{
					case OpenPending:
						bWaiting = TRUE;
						break;

					case Started:
						bPlayback = TRUE;
						break;

					case Paused:
						bPlayback = TRUE;
						break;

					case PausePending:
						bWaiting = TRUE;
						bPlayback = TRUE;
						break;

					case StartPending:
						bWaiting = TRUE;
						bPlayback = TRUE;
						break;

					case Stopped:
						bWaiting = TRUE;
						bPlayback = FALSE;
						break;

					case Closed:
						bPlayback = FALSE;
						break;

					case Ready:
						bWaiting = TRUE;
						bPlayback = FALSE;
						break;

					case Ended:
						bWaiting = FALSE;
						bPlayback = FALSE;
						DisableControls();
						break;
					}

					UINT  uEnable = MF_BYCOMMAND | (bWaiting ? MF_GRAYED : MF_ENABLED);
					HCURSOR hCursor = LoadCursor(NULL, MAKEINTRESOURCE(bWaiting ? IDC_WAIT : IDC_ARROW));
					SetCursor(hCursor);

					// If in playback mode and has video.
					if (bPlayback && _mediaPlayer->HasVideo())
					{
						// Do not repiant client.
						_repaintClient = FALSE;
					}
					else
					{
						// Repiant client.
						_repaintClient = TRUE;
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
			LRESULT MediaPlayerForm::OnNotifyError(WPARAM wParam, LPARAM lParam)
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
			/// On volume changed.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaPlayerForm::OnVolumeChanged(WPARAM wParam, LPARAM lParam)
			{
				// Get the volume from the parameter.
				float newVolume = (float)wParam;
				BOOL newMute = (BOOL)lParam;

				// Return all is good.
				return 0;
			}

			/// <summary>
			/// On play button clicked.
			/// </summary>
			void MediaPlayerForm::OnBnClickedButtonplay()
			{
				PlayControls();

				// Get the play button handler.
				CWnd *pBtnPlay = GetDlgItem(IDC_BUTTONPLAY);
				if (pBtnPlay != NULL)
				{
					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Play the media.
						_mediaPlayer->Play();
					}
				}
			}

			/// <summary>
			/// On stop button clicked.
			/// </summary>
			void MediaPlayerForm::OnBnClickedButtonstop()
			{
				StopControls();

				// Get the stop button handler.
				CWnd *pBtnStop = GetDlgItem(IDC_BUTTONSTOP);
				if (pBtnStop != NULL)
				{
					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Stop the media.
						_mediaPlayer->Stop();
					}
				}
			}

			/// <summary>
			/// On pause button clicked.
			/// </summary>
			void MediaPlayerForm::OnBnClickedButtonpause()
			{
				PauseControls();

				// Get the pause button handler.
				CWnd *pBtnPause = GetDlgItem(IDC_BUTTONPAUSE);
				if (pBtnPause != NULL)
				{
					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Stop the media.
						_mediaPlayer->Pause();
					}
				}
			}

			/// <summary>
			/// On close button clicked.
			/// </summary>
			void MediaPlayerForm::OnBnClickedButtonclose()
			{
				DisableControls();

				// Get the pause button handler.
				CWnd *pBtnClose = GetDlgItem(IDC_BUTTONCLOSE);
				if (pBtnClose != NULL)
				{
					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Stop the media.
						_mediaPlayer->Close();
					}
				}
			}

			/// <summary>
			/// On mute button clicked.
			/// </summary>
			void MediaPlayerForm::OnBnClickedButtonmute()
			{
				// If not muted.
				if (!_mute)
				{
					// Mute.
					_mute = true;
					_muteButton.SetWindowTextW(L"Mute Off");

					// If volume not null.
					if (_volume != NULL)
						_volume->SetMute(true);
				}
				else
				{
					// Un-Mute.
					_mute = false;
					_muteButton.SetWindowTextW(L"Mute");

					// If volume not null.
					if (_volume != NULL)
						_volume->SetMute(false);
				}
			}

			/// <summary>
			/// On selection changed open URL.
			/// </summary>
			void MediaPlayerForm::OnCbnSelchangeComboopen()
			{
				// Get the selected index.
				const int selectedIndex = _openURL.GetCurSel();
				const int count = _openURL.GetCount();

				// Make sure the selection is valid.
				if (selectedIndex > -1 && selectedIndex < count)
				{
					// Make a selection.
					switch (selectedIndex)
					{
					case 0:
						// Open file.
						OpenFile();
						break;

					case 1:
						// Open Network URI.
						OpenNetworkURI();
						break;

					default:
						break;
					}
				}
			}

			/// <summary>
			/// On slider thumb position changing
			/// </summary>
			void MediaPlayerForm::OnTRBNThumbPosChangingSliderposition(NMHDR *pNMHDR, LRESULT *pResult)
			{
				*pResult = 0;
			}

			/// <summary>
			/// On slider released capture, position has changed.
			/// </summary>
			void MediaPlayerForm::OnNMReleasedcaptureSliderposition(NMHDR *pNMHDR, LRESULT *pResult)
			{
				HRESULT hr = S_OK;

				// Lock this critical section.
				AutoLockHandler lock(_critsec);

				// Get the slider position.
				int position = _positionSlider.GetPos();

				// Move the media to the new position.
				// This value must be a factor of the slider range.
				MFTIME newPosition = (MFTIME)((double)((double)position / (double)100) * (double)_duration);

				// If the media player is not null.
				if (_mediaPlayer != NULL)
				{
					// Set the position.
					hr = _mediaPlayer->SetPositionNoPending(newPosition);

					// Position has been set.
					if (SUCCEEDED(hr))
					{
						int positionSeconds = ((int)((double)((double)newPosition / (double)1000) / (double)100) / (double)100);
						int positionMinutes = (int)((double)positionSeconds / (double)60);
						int positionHours = (int)((double)positionMinutes / (double)60);

						// Get the position.
						CString stringPosition;
						stringPosition.Format(_T("%.2d:%.2d:%.2d"), positionHours, positionMinutes, positionSeconds);

						// Set the position.
						// Get the position button handler.
						CWnd *pPosition = GetDlgItem(IDC_DURATIONCURRENT);
						if (pPosition != NULL)
							pPosition->SetWindowTextW(stringPosition);
					}
				}
				
				*pResult = 0;

				/*CString str;
				str.Format(_T("Moved to: %d"), _duration);
				AfxMessageBox(str);*/
			}

			/// <summary>
			/// Open file.
			/// </summary>
			void MediaPlayerForm::OpenFile()
			{
				HRESULT hr = S_OK;

				// Show the File Open dialog.
				WCHAR path[MAX_PATH];
				path[0] = L'\0';

				CFileDialog dlgFile(TRUE);
				OPENFILENAME& ofn = dlgFile.GetOFN();
				ofn.lpstrFilter = L"Video Media\0*.mp4;*.m4a;*.m4v;*.mov;*.wmv;*.wma;*.asf;*.avi;*.3g2;*.3gp;*.3gp2;*.3gpp\0Audio Media\0*.mp3;*.wma;*.wav;*.aac;*.adts\0All files\0*.*\0";
				ofn.lpstrFile = path;
				ofn.nMaxFile = MAX_PATH;

				// If open ok.
				if (dlgFile.DoModal() == IDOK)
				{
					// Get the file name.
					LPWSTR fileName = ofn.lpstrFile;

					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Open the file.
						hr = _mediaPlayer->OpenURL(fileName);
					}

					// If the file has been opened.
					if (SUCCEEDED(hr))
					{
						EnableControls();
					}
					else
					{
						// Display a message box with the error.
						MessageBox((LPCWSTR)L"Unable to open the file.", (LPCWSTR)L"Error", MB_OK | MB_ICONERROR);
					}
				}
			}

			/// <summary>
			/// Open network URI form.
			/// </summary>
			void MediaPlayerForm::OpenNetworkURI()
			{
				HRESULT hr = S_OK;

				// Open the form.
				OpenNetworkUriForm networkURI(this);
				networkURI.DoModal();

				// Get the network url.
				string textURL = networkURI.GetNetworkURL();

				// If a newtork url exists.
				if (textURL.length() > 0)
				{
					// Get the file name.
					CString fileName(textURL.c_str());

					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Open the network URL.
						hr = _mediaPlayer->OpenURL(fileName);
					}

					// If the file has been opened.
					if (SUCCEEDED(hr))
					{
						EnableControls();
					}
					else
					{
						// Display a message box with the error.
						MessageBox((LPCWSTR)L"Unable to open the network URL.", (LPCWSTR)L"Error", MB_OK | MB_ICONERROR);
					}
				}
			}

			/// <summary>
			/// Enable controls.
			/// </summary>
			void MediaPlayerForm::EnableControls()
			{
				// Get the stop button handler.
				CWnd *pBtnStop = GetDlgItem(IDC_BUTTONSTOP);
				if (pBtnStop != NULL)
					pBtnStop->EnableWindow(true);

				// Get the pause button handler.
				CWnd *pBtnPause = GetDlgItem(IDC_BUTTONPAUSE);
				if (pBtnPause != NULL)
					pBtnPause->EnableWindow(true);

				// Enable the mute button.
				_muteButton.EnableWindow(true);
				_openURL.EnableWindow(false);
				_positionSlider.EnableWindow(true);

				MFTIME phnsDuration;
				HRESULT hr = _mediaPlayer->GetDuration(&phnsDuration);

				// Duration has been set.
				if (SUCCEEDED(hr))
				{
					_duration = phnsDuration;
					int durationSeconds = ((int)((double)((double)phnsDuration / (double)1000) / (double)100) / (double)100);
					int durationMinutes = (int)((double)durationSeconds / (double)60);
					int durationHours = (int)((double)durationMinutes / (double)60);

					// Get the position.
					CString stringDuration;
					stringDuration.Format(_T("%.2d:%.2d:%.2d"), durationHours, durationMinutes, durationSeconds);

					// Set the duration.
					// Get the duration button handler.
					CWnd *pDuration = GetDlgItem(IDC_DURATION);
					if (pDuration != NULL)
						pDuration->SetWindowTextW(stringDuration);

					// Start a new current position thread
					_internalThread = true;
					_threadGetCurrentPosition = std::thread(std::bind(&MediaPlayerForm::StartGetCurrentPositionThread, this));
					_threadGetCurrentPosition.detach();
				}
				else
				{
					// Set to no media.
					_duration = 0;
				}
			}

			/// <summary>
			/// Disable controls.
			/// </summary>
			void MediaPlayerForm::DisableControls()
			{
				// Stop the thread.
				StopGetCurrentPositionThread();

				// Get the stop button handler.
				CWnd *pBtnStop = GetDlgItem(IDC_BUTTONSTOP);
				if (pBtnStop != NULL)
					pBtnStop->EnableWindow(false);

				// Get the pause button handler.
				CWnd *pBtnPause = GetDlgItem(IDC_BUTTONPAUSE);
				if (pBtnPause != NULL)
					pBtnPause->EnableWindow(false);

				// Get the play button handler.
				CWnd *pBtnPlay = GetDlgItem(IDC_BUTTONPLAY);
				if (pBtnPlay != NULL)
					pBtnPlay->EnableWindow(false);

				// Enable the mute button.
				_muteButton.EnableWindow(false);
				_openURL.EnableWindow(true);
				_positionSlider.EnableWindow(false);
			}

			/// <summary>
			/// Pause controls.
			/// </summary>
			void MediaPlayerForm::PauseControls()
			{
				// Get the stop button handler.
				CWnd *pBtnStop = GetDlgItem(IDC_BUTTONSTOP);
				if (pBtnStop != NULL)
					pBtnStop->EnableWindow(false);

				// Get the pause button handler.
				CWnd *pBtnPause = GetDlgItem(IDC_BUTTONPAUSE);
				if (pBtnPause != NULL)
					pBtnPause->EnableWindow(false);

				// Get the play button handler.
				CWnd *pBtnPlay = GetDlgItem(IDC_BUTTONPLAY);
				if (pBtnPlay != NULL)
					pBtnPlay->EnableWindow(true);

				// Enable the mute button.
				_muteButton.EnableWindow(false);
			}

			/// <summary>
			/// Play controls.
			/// </summary>
			void MediaPlayerForm::PlayControls()
			{
				// Get the stop button handler.
				CWnd *pBtnStop = GetDlgItem(IDC_BUTTONSTOP);
				if (pBtnStop != NULL)
					pBtnStop->EnableWindow(true);

				// Get the pause button handler.
				CWnd *pBtnPause = GetDlgItem(IDC_BUTTONPAUSE);
				if (pBtnPause != NULL)
					pBtnPause->EnableWindow(true);

				// Get the play button handler.
				CWnd *pBtnPlay = GetDlgItem(IDC_BUTTONPLAY);
				if (pBtnPlay != NULL)
					pBtnPlay->EnableWindow(false);

				// Enable the mute button.
				_muteButton.EnableWindow(true);
			}

			/// <summary>
			/// Stop controls.
			/// </summary>
			void MediaPlayerForm::StopControls()
			{
				// Get the stop button handler.
				CWnd *pBtnStop = GetDlgItem(IDC_BUTTONSTOP);
				if (pBtnStop != NULL)
					pBtnStop->EnableWindow(false);

				// Get the pause button handler.
				CWnd *pBtnPause = GetDlgItem(IDC_BUTTONPAUSE);
				if (pBtnPause != NULL)
					pBtnPause->EnableWindow(false);

				// Get the play button handler.
				CWnd *pBtnPlay = GetDlgItem(IDC_BUTTONPLAY);
				if (pBtnPlay != NULL)
					pBtnPlay->EnableWindow(true);

				// Enable the mute button.
				_muteButton.EnableWindow(false);
			}

			/// <summary>
			/// Start get current position thread.
			/// </summary>
			void MediaPlayerForm::StartGetCurrentPositionThread()
			{
				// 4 second interval.
				Interval interval = chrono::milliseconds(4000);

				// While active.
				while (_internalThread)
				{
					// Sleep thread for 4 seconds.
					std::this_thread::sleep_for(interval);
					
					// If the media player is not null.
					if (_mediaPlayer != NULL)
					{
						// Lock this critical section.
						AutoLockHandler lock(_critsec);

						// Get the current position, and move the slider.
						MFTIME currentPosition;
						HRESULT hr = _mediaPlayer->GetCurrentPositionDirect(&currentPosition);

						// Current position has been set.
						if (SUCCEEDED(hr))
						{
							int secondsCount = 0;
							int positionSeconds = ((int)((double)((double)currentPosition / (double)1000) / (double)100) / (double)100);
							int positionMinutes = (int)((double)positionSeconds / (double)60);
							int positionHours = (int)((double)positionMinutes / (double)60);

							// Seconds counter.
							if (positionMinutes > 0)
								secondsCount = (positionSeconds - (positionMinutes * 60));
							else
								secondsCount = positionSeconds;

							// Seconds counter.
							if (positionHours > 0)
							    secondsCount = (secondsCount - (positionHours * 60));

							// Get the position.
							CString stringPosition;
							stringPosition.Format(_T("%.2d:%.2d:%.2d"), positionHours, positionMinutes, secondsCount);

							// Set the position.
							// Get the position button handler.
							CWnd *pPosition = GetDlgItem(IDC_DURATIONCURRENT);
							if (pPosition != NULL)
								pPosition->SetWindowTextW(stringPosition);

							// Set the slider position.
							int sliderPosition = (int)((double)((double)currentPosition / (double)_duration) * (double)100);
							_positionSlider.SetPos(sliderPosition);
						}
					}
				}
			}

			/// <summary>
			/// Stop get current position thread.
			/// </summary>
			void MediaPlayerForm::StopGetCurrentPositionThread()
			{
				// If an internal thread was created.
				if (_internalThread)
				{
					try
					{
						_internalThread = false;
						_threadGetCurrentPosition.join();
					}
					catch (...) {}
				}
			}
		}
	}
}