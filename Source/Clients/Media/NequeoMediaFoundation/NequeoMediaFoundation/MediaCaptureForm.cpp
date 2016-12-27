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
	ON_BN_CLICKED(IDC_CAPTURE_START_BUTTON, &MediaCaptureForm::OnBnClickedButtonStartCapture)
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
				_mediaCaptureVideoAudio(NULL),
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
				_captureVideo(false),
				_captureAudio(false),
				_captureVideoAudio(false),
				_capturing(false),
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

					// If the media capture is not null.
					if (_mediaCaptureVideoAudio != NULL)
					{
						// Release the media capture.
						_mediaCaptureVideoAudio->Release();
						_mediaCaptureVideoAudio = NULL;
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
				_tab.InsertItem(2, L"Screen");
				_tab.InsertItem(3, L"Video & Audio");
				_tab.Init(*this, *this, *this, *this);

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

				// If the media capture is not null.
				if (_mediaCaptureVideoAudio != NULL)
				{
					// Shut down the media capture.
					_mediaCaptureVideoAudio->StopCapture();
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
				HRESULT hrVideoAudio = MediaCapture::CreateInstance(_hCapture, _hEvent, &_mediaCaptureVideoAudio);
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

				// Enable capture.
				EnableCapture(state, DisallowAudioCapture, 0);

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

				// Enable capture.
				EnableCapture(DisallowVideoCapture, state, 1);

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
				// Capture stopped.
				_capturing = false;

				// Get the state from the parameter.
				CaptureState state = (CaptureState)wParam;

				BOOL bWaiting = FALSE;
				BOOL bCapturingVideo = FALSE;
				BOOL bCapturingAudio = FALSE;
				BOOL bCapturingVideoAudio = FALSE;

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
						_videoDeviceItem.EnableWindow(false);

						// Get the video page.
						CaptureVideoPage& pageVideo = _tab.VideoPage();
						pageVideo.EnableWindow(false);
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
						_audioDeviceItem.EnableWindow(false);

						// Get the audio page.
						CaptureAudioPage& pageAudio = _tab.AudioPage();
						pageAudio.EnableWindow(false);
					}
				}

				// If the media capture is not null.
				if (_mediaCaptureVideoAudio != NULL)
				{
					switch (state)
					{
					case CaptureNotReady:
						bWaiting = TRUE;
						bCapturingVideoAudio = FALSE;
						break;

					case CaptureReady:
						bWaiting = FALSE;
						bCapturingVideoAudio = FALSE;
						break;

					case Capturing:
						bWaiting = FALSE;
						bCapturingVideoAudio = TRUE;
						break;

					case NotCapturing:
						bWaiting = TRUE;
						bCapturingVideoAudio = FALSE;
						break;
					}

					// If capturing.
					if (bCapturingVideoAudio == TRUE)
					{
						_videoDeviceItem.EnableWindow(false);
						_audioDeviceItem.EnableWindow(false);

						// Get the video audio page.
						CaptureVideoAudioPage& pageVideoAudio = _tab.VideoAudioPage();
						pageVideoAudio.EnableWindow(false);
					}
				}

				// If capturing.
				if (bCapturingVideo == TRUE || bCapturingAudio == TRUE || bCapturingVideoAudio == TRUE)
				{
					_capturing = true;

					// Change text.
					// Get the start button handler.
					CWnd *pBtnStart = GetDlgItem(IDC_CAPTURE_START_BUTTON);
					if (pBtnStart != NULL)
					{
						// Capturing text.
						pBtnStart->SetWindowTextW(L"Stop Capture");
					}

					// Get the refresh button handler.
					CWnd *pRefresh = GetDlgItem(IDC_REFRESHDEVICE_BUTTON);
					if (pRefresh != NULL) pRefresh->EnableWindow(false);
				}
				else
				{
					// Change text.
					// Get the start button handler.
					CWnd *pBtnStart = GetDlgItem(IDC_CAPTURE_START_BUTTON);
					if (pBtnStart != NULL)
					{
						// Original text.
						pBtnStart->SetWindowTextW(L"Start Capture");
					}

					// Get the refresh button handler.
					CWnd *pRefresh = GetDlgItem(IDC_REFRESHDEVICE_BUTTON);
					if (pRefresh != NULL) pRefresh->EnableWindow(true);

					_videoDeviceItem.EnableWindow(true);
					_audioDeviceItem.EnableWindow(true);

					// Get the video page.
					CaptureVideoPage& pageVideo = _tab.VideoPage();
					pageVideo.EnableWindow(true);

					// Get the audio page.
					CaptureAudioPage& pageAudio = _tab.AudioPage();
					pageAudio.EnableWindow(true);

					// Get the video audio page.
					CaptureVideoAudioPage& pageVideoAudio = _tab.VideoAudioPage();
					pageVideoAudio.EnableWindow(true);
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
			/// On start capture button clicked.
			/// </summary>
			void MediaCaptureForm::OnBnClickedButtonStartCapture()
			{
				// If not capturing.
				if (!_capturing)
				{
					// Video and audio encoding configuration.
					EncodingParameters ep;

					// If capturing video.
					if (_captureVideo)
					{
						// Get the video page.
						const CaptureVideoPage& pageVideo = _tab.VideoPage();

						CString filenameVideo;
						CString bitRate;
						CString frameSizeW;
						CString frameSizeH;
						CString frameRateN;
						CString frameRateD;

						// Get the start button handler.
						CWnd *pFilename = pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_PATH_TEXT);
						CWnd *pBitRate = pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_BITRATE_TEXT);
						CWnd *pFrameSizeW = pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_FRAMESIZE_W_TEXT);
						CWnd *pFrameSizeH = pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_FRAMESIZE_H_TEXT);
						CWnd *pFrameRateN = pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_FRAMERATE_N_TEXT);
						CWnd *pFrameRateD = pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_FRAMERATE_D_TEXT);

						if (pFilename != NULL) pFilename->GetWindowTextW(filenameVideo);
						if (pBitRate != NULL) pBitRate->GetWindowTextW(bitRate);
						if (pFrameSizeW != NULL) pFrameSizeW->GetWindowTextW(frameSizeW);
						if (pFrameSizeH != NULL) pFrameSizeH->GetWindowTextW(frameSizeH);
						if (pFrameRateN != NULL) pFrameRateN->GetWindowTextW(frameRateN);
						if (pFrameRateD != NULL) pFrameRateD->GetWindowTextW(frameRateD);

						// Set video configuration.
						ep.video.subtype = MFVideoFormat_WMV3;
						ep.video.bitRate = _ttoi(bitRate);
						ep.video.frameSize.width = _ttoi(frameSizeW);
						ep.video.frameSize.height = _ttoi(frameSizeH);
						ep.video.frameRate.denominator = _ttoi(frameRateD);
						ep.video.frameRate.numerator = _ttoi(frameRateN);
						ep.video.aspectRatio.denominator = 0;
						ep.video.aspectRatio.numerator = 0;

						// If the media capture is not null.
						if (_mediaCaptureVideo != NULL)
						{
							HRESULT hrvideo = S_OK;

							// Start video capture.
							hrvideo = _mediaCaptureVideo->StartCaptureToFile(filenameVideo, ep);
							if (FAILED(hrvideo))
							{
								// Notify error.
								OnNotifyError((WPARAM)hrvideo, (LPARAM)0);
							}
						}
					}

					// If capturing audio.
					if (_captureAudio)
					{
						// Get the audio page.
						const CaptureAudioPage& pageAudio = _tab.AudioPage();

						CString filenameAudio;
						CString sampleRate;
						CString channels;
						CString bitsPerSample;

						// Get the start button handler.
						CWnd *pFilename = pageAudio.GetDlgItem(IDC_CAPTURE_AUDIO_PATH_TEXT);
						CWnd *pSampleRate = pageAudio.GetDlgItem(IDC_CAPTURE_AUDIO_SAMPLERATE_TEXT);
						CWnd *pChannels = pageAudio.GetDlgItem(IDC_CAPTURE_AUDIO_CHANNELS_TEXT);
						CWnd *pBitsPerSample = pageAudio.GetDlgItem(IDC_CAPTURE_AUDIO_BITSPERSAMPLE_TEXT);

						if (pFilename != NULL) pFilename->GetWindowTextW(filenameAudio);
						if (pSampleRate != NULL) pSampleRate->GetWindowTextW(sampleRate);
						if (pChannels != NULL) pChannels->GetWindowTextW(channels);
						if (pBitsPerSample != NULL) pBitsPerSample->GetWindowTextW(bitsPerSample);

						// Set audio configuration.
						ep.audio.collectionIndex = -1;
						ep.audio.subtype = MFAudioFormat_PCM;
						ep.audio.bitsPerSample = _ttoi(bitsPerSample);
						ep.audio.channels = _ttoi(channels);
						ep.audio.sampleRate = _ttoi(sampleRate);
						ep.audio.blockAlign = ep.audio.channels * (ep.audio.bitsPerSample / 8);
						ep.audio.bytesPerSecond = ep.audio.blockAlign * ep.audio.sampleRate;

						// If the media capture is not null.
						if (_mediaCaptureAudio != NULL)
						{
							HRESULT hraudio = S_OK;

							// Start audo capture.
							hraudio = _mediaCaptureAudio->StartCaptureToFile(filenameAudio, ep);
							if (FAILED(hraudio))
							{
								// Notify error.
								OnNotifyError((WPARAM)hraudio, (LPARAM)0);
							}
						}
					}

					// If capturing video and audio.
					if (_captureVideoAudio)
					{
						// Video and audio encoding configuration.
						EncodingParameters epVA;

						// Get the video audio page.
						const CaptureVideoAudioPage& pageVideoAudio = _tab.VideoAudioPage();

						CString filenameVideoAudio;
						CString bitRate;
						CString frameSizeW;
						CString frameSizeH;
						CString frameRateN;
						CString frameRateD;
						CString sampleRate;
						CString channels;
						CString bitsPerSample;
						CString bytesPerSecond;

						// Get the start button handler.
						CWnd *pFilename = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_PATH_TEXT);
						CWnd *pBitRate = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_BITRATE_TEXT);
						CWnd *pFrameSizeW = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_WIDTH_TEXT);
						CWnd *pFrameSizeH = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_HEIGHT_TEXT);
						CWnd *pFrameRateN = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_RATE_N_TEXT);
						CWnd *pFrameRateD = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_RATE_D_TEXT);
						CWnd *pSampleRate = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_SAMPLERATE_TEXT);
						CWnd *pChannels = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_CHANNELS_TEXT);
						CWnd *pBitsPerSample = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_BITSPERSAMPLE_TEXT);
						CWnd *pBytesPerSecond = pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_BYTESPERSECOND_TEXT);

						if (pFilename != NULL) pFilename->GetWindowTextW(filenameVideoAudio);
						if (pBitRate != NULL) pBitRate->GetWindowTextW(bitRate);
						if (pFrameSizeW != NULL) pFrameSizeW->GetWindowTextW(frameSizeW);
						if (pFrameSizeH != NULL) pFrameSizeH->GetWindowTextW(frameSizeH);
						if (pFrameRateN != NULL) pFrameRateN->GetWindowTextW(frameRateN);
						if (pFrameRateD != NULL) pFrameRateD->GetWindowTextW(frameRateD);
						if (pSampleRate != NULL) pSampleRate->GetWindowTextW(sampleRate);
						if (pChannels != NULL) pChannels->GetWindowTextW(channels);
						if (pBitsPerSample != NULL) pBitsPerSample->GetWindowTextW(bitsPerSample);
						if (pBytesPerSecond != NULL) pBytesPerSecond->GetWindowTextW(bytesPerSecond);

						// Set encoding configuration.
						epVA.transcode = MFTranscodeContainerType_FMPEG4;

						epVA.audio.collectionIndex = -1;
						epVA.audio.subtypeReader = { 0 };
						epVA.audio.subtype = MFAudioFormat_AAC;
						epVA.audio.transcode = MFTranscodeContainerType_WAVE;
						epVA.audio.bitsPerSample = _ttoi(bitsPerSample);
						epVA.audio.channels = _ttoi(channels);
						epVA.audio.sampleRate = _ttoi(sampleRate);
						epVA.audio.blockAlign = 0; // epVA.audio.channels * (epVA.audio.bitsPerSample / 8);
						epVA.audio.bytesPerSecond = _ttoi(bytesPerSecond); // epVA.audio.blockAlign * epVA.audio.sampleRate;

						epVA.video.subtype = MFVideoFormat_H264;
						epVA.video.subtypeReader = { 0 };
						epVA.video.transcode = MFTranscodeContainerType_FMPEG4;
						epVA.video.bitRate = _ttoi(bitRate);
						epVA.video.frameSize.width = _ttoi(frameSizeW);
						epVA.video.frameSize.height = _ttoi(frameSizeH);
						epVA.video.frameRate.denominator = _ttoi(frameRateD);
						epVA.video.frameRate.numerator = _ttoi(frameRateN);
						epVA.video.aspectRatio.denominator = 1;
						epVA.video.aspectRatio.numerator = 1;

						// If the media capture is not null.
						if (_mediaCaptureVideoAudio != NULL)
						{
							HRESULT hrvideoaudio = S_OK;

							// Start audo capture.
							hrvideoaudio = _mediaCaptureVideoAudio->StartCaptureToFile(filenameVideoAudio, epVA);
							if (FAILED(hrvideoaudio))
							{
								// Notify error.
								OnNotifyError((WPARAM)hrvideoaudio, (LPARAM)0);
							}
						}
					}
				}
				else
				{
					// If capturing video.
					if (_captureVideo)
					{
						// If the media capture is not null.
						if (_mediaCaptureVideo != NULL)
						{
							HRESULT hrvideo = S_OK;

							// Start video capture.
							hrvideo = _mediaCaptureVideo->StopCapture();
							if (FAILED(hrvideo))
							{
								// Notify error.
								OnNotifyError((WPARAM)hrvideo, (LPARAM)0);
							}
						}
					}

					// If capturing audio.
					if (_captureAudio)
					{
						// If the media capture is not null.
						if (_mediaCaptureAudio != NULL)
						{
							HRESULT hraudio = S_OK;

							// Start audo capture.
							hraudio = _mediaCaptureAudio->StopCapture();
							if (FAILED(hraudio))
							{
								// Notify error.
								OnNotifyError((WPARAM)hraudio, (LPARAM)0);
							}
						}
					}

					// If capturing video and audio.
					if (_captureVideoAudio)
					{
						// If the media capture is not null.
						if (_mediaCaptureVideoAudio != NULL)
						{
							HRESULT hrvideoaudio = S_OK;

							// Start video capture.
							hrvideoaudio = _mediaCaptureVideoAudio->StopCapture();
							if (FAILED(hrvideoaudio))
							{
								// Notify error.
								OnNotifyError((WPARAM)hrvideoaudio, (LPARAM)0);
							}
						}
					}
				}
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

					// If the media capture is not null.
					if (_mediaCaptureVideoAudio != NULL)
					{
						// Set the video device.
						_mediaCaptureVideoAudio->SetVideoDevice(_videoDevice->ppDevices[_selectedIndexVideo]);
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

					// If the media capture is not null.
					if (_mediaCaptureVideoAudio != NULL)
					{
						// Set the video device.
						_mediaCaptureVideoAudio->SetVideoDevice(NULL);
					}
				}

				// Enable capture.
				EnableCapture(AllowVideoCapture, DisallowAudioCapture, 0);
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

					// If the media capture is not null.
					if (_mediaCaptureVideoAudio != NULL)
					{
						// Set the audio device.
						_mediaCaptureVideoAudio->SetAudioDevice(_audioDevice->ppDevices[_selectedIndexAudio]);
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

					// If the media capture is not null.
					if (_mediaCaptureVideoAudio != NULL)
					{
						// Set the audio device.
						_mediaCaptureVideoAudio->SetAudioDevice(NULL);
					}
				}

				// Enable capture.
				EnableCapture(DisallowVideoCapture, AllowAudioCapture, 1);
			}

			/// <summary>
			/// On tab page slection changed.
			/// </summary>
			void MediaCaptureForm::OnTcnSelchangeMediacaptureTab(NMHDR *pNMHDR, LRESULT *pResult)
			{
				// Get the selected index.
				int selectedIndex = _tab.GetCurSel();

				// Make sure the selection is valid.
				if (selectedIndex > -1 && selectedIndex < _tab.GetNumberOfPages())
				{
					// Show current hide all others.
					_tab.ShowTabPage(selectedIndex);
				}

				// All is OK.
				*pResult = 0;
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

				_selectedIndexVideo = -1;
				_selectedIndexAudio = -1;

				_captureVideo = false;
				_captureAudio = false;
				_captureVideoAudio = false;

				// Get the video page.
				const CaptureVideoPage& pageVideo = _tab.VideoPage();

				// Get the audio page.
				const CaptureAudioPage& pageAudio = _tab.AudioPage();

				// Get the video audio page.
				const CaptureVideoAudioPage& pageVideoAudio = _tab.VideoAudioPage();

				// Get the video and audio check button.
				CButton *pCaptureAudioChecked = (CButton*)pageAudio.GetDlgItem(IDC_CAPTURE_AUDIO_CHECK);
				CButton *pCaptureVideoChecked = (CButton*)pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_CHECK);
				CButton *pCaptureVideoAudioChecked = (CButton*)pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_CHECK);

				// If audio.
				if (pCaptureAudioChecked != NULL)
				{
					// Set check box state.
					pCaptureAudioChecked->SetCheck(BST_UNCHECKED);
				}

				// If video.
				if (pCaptureVideoChecked != NULL)
				{
					// Set check box state.
					pCaptureVideoChecked->SetCheck(BST_UNCHECKED);
				}

				// If video.
				if (pCaptureVideoAudioChecked != NULL)
				{
					// Set check box state.
					pCaptureVideoAudioChecked->SetCheck(BST_UNCHECKED);
				}

				// Enable disable capture button.
				EnableCaptureButton(_captureVideo, _captureAudio, _captureVideoAudio);
			}

			/// <summary>
			/// Enable or disable capture button.
			/// </summary>
			/// <param name="videoState">The video state.</param>
			/// <param name="audioState">The audio state.</param>
			/// <param name="state">The state passed (0 = video, 1 = audio).</param>
			void MediaCaptureForm::EnableCapture(CaptureVideoState videoState, CaptureAudioState audioState, unsigned int state)
			{
				// Get the video page.
				const CaptureVideoPage& pageVideo = _tab.VideoPage();

				// Get the audio page.
				const CaptureAudioPage& pageAudio = _tab.AudioPage();

				// Get the video audio page.
				const CaptureVideoAudioPage& pageVideoAudio = _tab.VideoAudioPage();

				_captureVideo = false;
				_captureAudio = false;
				_captureVideoAudio = false;

				// Get the video and audio check button.
				CButton *pCaptureAudioChecked = (CButton*)pageAudio.GetDlgItem(IDC_CAPTURE_AUDIO_CHECK);
				CButton *pCaptureVideoChecked = (CButton*)pageVideo.GetDlgItem(IDC_CAPTURE_VIDEO_CHECK);
				CButton *pCaptureVideoAudioChecked = (CButton*)pageVideoAudio.GetDlgItem(IDC_CAPTURE_VIDEOAUDIO_CHECK);

				// If audio.
				if (pCaptureAudioChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureAudioChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						_captureAudio = false;
					}
					else if (checked == BST_CHECKED)
					{
						_captureAudio = true;
					}
				}

				// If video.
				if (pCaptureVideoChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureVideoChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						_captureVideo = false;
					}
					else if (checked == BST_CHECKED)
					{
						_captureVideo = true;
					}
				}

				// If video audio.
				if (pCaptureVideoAudioChecked != NULL)
				{
					// Get check box state.
					int checked = pCaptureVideoAudioChecked->GetCheck();

					// If un-checked.
					if (checked == BST_UNCHECKED)
					{
						_captureVideoAudio = false;
					}
					else if (checked == BST_CHECKED)
					{
						_captureVideoAudio = true;
					}
				}

				// If video capture disallowed.
				if (state == 0 && videoState == DisallowVideoCapture)
				{
					_captureVideo = false;
					_captureVideoAudio = false;
				}

				// If audio capture disallowed.
				if (state == 1 && audioState == DisallowAudioCapture)
				{
					_captureAudio = false;
					_captureVideoAudio = false;
				}

				// Make sure the selection is valid.
				if (_selectedIndexVideo < 0 || _selectedIndexVideo >= _countVideo)
				{
					_captureVideo = false;
					_captureVideoAudio = false;
				}

				// Make sure the selection is valid.
				if (_selectedIndexAudio < 0 || _selectedIndexAudio >= _countAudio)
				{
					_captureAudio = false;
					_captureVideoAudio = false;
				}

				// Enable disable capture button.
				EnableCaptureButton(_captureVideo, _captureAudio, _captureVideoAudio);
			}

			/// <summary>
			/// Enable or disable capture button.
			/// </summary>
			/// <param name="captureVideo">The video state.</param>
			/// <param name="captureAudio">The audio state.</param>
			/// <param name="captureVideoAudio">The video audio state.</param>
			void MediaCaptureForm::EnableCaptureButton(bool captureVideo, bool captureAudio, bool captureVideoAudio)
			{
				_captureVideo = captureVideo;
				_captureAudio = captureAudio;
				_captureVideoAudio = captureVideoAudio;

				// Get the start button handler.
				CWnd *pBtnStart = GetDlgItem(IDC_CAPTURE_START_BUTTON);
				if (pBtnStart != NULL)
				{
					// If capture video or audio
					if (captureVideo || captureAudio || captureVideoAudio)
					{
						// Enable start capture button.
						pBtnStart->EnableWindow(true);
					}
					else
					{
						// Enable start capture button.
						pBtnStart->EnableWindow(false);
					}
				}
			}
		}
	}
}