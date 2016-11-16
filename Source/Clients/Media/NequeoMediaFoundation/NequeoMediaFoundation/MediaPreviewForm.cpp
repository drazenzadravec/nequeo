/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPreviewForm.cpp
*  Purpose :       MediaPreviewForm class.
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

#include "MediaPreviewForm.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::MediaPreviewForm, CDialog)
	ON_WM_CLOSE()
	ON_WM_DESTROY()
	ON_WM_CREATE()
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_MESSAGE(ON_WM_APP_NOTIFY, &MediaPreviewForm::OnNotifyState)
	ON_MESSAGE(ON_WM_APP_ERROR, &MediaPreviewForm::OnNotifyError)
	ON_WM_SHOWWINDOW()
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			/// <param name="videoDevice">The media foundation device.</param>
			/// <param name="windowText">The widnow text.</param>
			/// <param name="pParent">The parent window.</param>
			MediaPreviewForm::MediaPreviewForm(IMFActivate* videoDevice, LPCTSTR windowText, CWnd* pParent) : CDialog(MediaPreviewForm::IDD, pParent),
				_mediaPreview(NULL), _hEvent(NULL), _repaintClient(FALSE), _videoDevice(videoDevice), _windowText(windowText),
				_disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaPreviewForm::~MediaPreviewForm()
			{
				// If not disposed.
				if (!_disposed)
				{
					_disposed = true;

					if (_toolTip != NULL)
						delete _toolTip;

					// If the media player is not null.
					if (_mediaPreview != NULL)
					{
						// Release the media player.
						_mediaPreview->Release();
						_mediaPreview = NULL;
					}
				}
			}

			/// <summary>
			/// On the show windows as a modal.
			/// </summary>
			INT_PTR MediaPreviewForm::DoModal()
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
			void MediaPreviewForm::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
			}

			/// <summary>
			/// On close window.
			/// </summary>
			void MediaPreviewForm::OnClose()
			{
				// If the media preview is not null.
				if (_mediaPreview != NULL)
				{
					// Shut down the media preview.
					_mediaPreview->Close();
				}

				CDialog::OnClose();
			}

			/// <summary>
			/// On destroy window.
			/// </summary>
			void MediaPreviewForm::OnDestroy()
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
			int MediaPreviewForm::OnCreate(LPCREATESTRUCT lpCreateStruct)
			{
				if (CDialog::OnCreate(lpCreateStruct) == -1)
					return -1;

				// Get the window handlers where the video will be player
				// and the window the will handle the player evenets.
				HWND hVideo = lpCreateStruct->hwndParent;
				_hEvent = lpCreateStruct->hwndParent;

				// Set the window text.
				CDialog::SetWindowTextW(_windowText);

				// All is good create the window.
				return 0;
			}

			/// <summary>
			/// On show window.
			/// </summary>
			/// <param name="bShow">Show the window.</param>
			/// <param name="nStatus">The window status.</param>
			void MediaPreviewForm::OnShowWindow(BOOL bShow, UINT nStatus)
			{
				CDialog::OnShowWindow(bShow, nStatus);

				// Create the tool tip control.
				_toolTip = new CToolTipCtrl();
				if (_toolTip != NULL)
				{

				}

				// Default is the main video.
				HWND hVideo = NULL;
				_hEvent = *this;

				// Get the video preview handler.
				CWnd *pVideoPreview = GetDlgItem(IDC_VIDEOPREVIEW);
				if (pVideoPreview != NULL)
				{
					// Set the video preview window handler.
					hVideo = pVideoPreview->m_hWnd;
				}

				// Initialize the player object.
				HRESULT hr = MediaPreview::CreateInstance(hVideo, _hEvent, &_mediaPreview);

				// If preview created.
				if (SUCCEEDED(hr))
				{
					// If the media preview is not null.
					if (_mediaPreview != NULL)
					{
						// Set the preview device.
						hr = _mediaPreview->SetDevice(_videoDevice);
					}
				}
			}

			/// <summary>
			/// On paint window.
			/// </summary>
			void MediaPreviewForm::OnPaint()
			{
				// Device context for painting.
				// Do not call CDialog::OnPaint() for painting messages.
				CPaintDC dc(this);

				// If the media player is not null.
				if (_mediaPreview != NULL)
				{
					// Repaint the client.
					if (!_repaintClient)
					{
						// Update video.
						_mediaPreview->UpdateVideo();
					}
				}
			}

			/// <summary>
			/// On size window.
			/// </summary>
			/// <param name="nType">The type.</param>
			/// <param name="cx">The width of the window.</param>
			/// <param name="cy">The height of the window.</param>
			void MediaPreviewForm::OnSize(UINT nType, int cx, int cy)
			{
				CDialog::OnSize(nType, cx, cy);

				// If the media player is not null.
				if (_mediaPreview != NULL)
				{
					// Since the media player width and height are type WORD (unsigned short).
					if (cx > 0 && cy > 0)
					{
						// Resize the video on the control.
						_mediaPreview->ResizeVideo((WORD)cx, (WORD)(cy));
					}
				}
			}

			/// <summary>
			/// On notify state.
			/// </summary>
			/// <param name="wParam">The message parameter.</param>
			/// <param name="lParam">The message parameter.</param>
			LRESULT MediaPreviewForm::OnNotifyState(WPARAM wParam, LPARAM lParam)
			{
				// Get the state from the parameter.
				PreviewState state = (PreviewState)wParam;

				BOOL bWaiting = FALSE;
				BOOL bPlayback = FALSE;

				// If the media player is not null.
				if (_mediaPreview != NULL)
				{
					switch (state)
					{
					case PreviewReady:
						bWaiting = FALSE;
						bPlayback = TRUE;

						// Preview panel client area.
						RECT lpRect;

						// Get the video preview handler.
						CWnd *pVideoPreview = GetDlgItem(IDC_VIDEOPREVIEW);
						if (pVideoPreview != NULL)
						{
							// Get the current preview size.
							pVideoPreview->GetClientRect(&lpRect);

							// Update the preview size in the wwindow.
							_mediaPreview->ResizeVideo((WORD)120, (WORD)120);
						}
						break;
					}

					// If in playback mode and has video.
					if (bPlayback && _mediaPreview->HasVideo())
					{
						// Do not repiant client.
						_repaintClient = FALSE;

						// Update video.
						_mediaPreview->UpdateVideo();
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
			LRESULT MediaPreviewForm::OnNotifyError(WPARAM wParam, LPARAM lParam)
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
		}
	}
}