/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CaptureTabControl.cpp
*  Purpose :       CaptureTabControl class.
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

#include "CaptureTabControl.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::CaptureTabControl, CTabCtrl)
END_MESSAGE_MAP()

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			IMPLEMENT_DYNAMIC(CaptureTabControl, CTabCtrl)

			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			CaptureTabControl::CaptureTabControl() :
				_disposed(false), _tabCurrent(0), _numberOfPages(0)
			{
				// Create the table pages.
				_tabePages[0] = new CaptureVideoPage();
				_tabePages[1] = new CaptureAudioPage();

				_numberOfPages = 2;
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			CaptureTabControl::~CaptureTabControl()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					// For each tab page.
					for (int i = 0; i < _numberOfPages; i++)
					{
						// Delete the page.
						delete _tabePages[i];
					}
				}
			}

			/// <summary>
			/// Initialise.
			/// </summary>
			/// <param name="hwndEventAudio">The audio event handler window to get notifications.</param>
			/// <param name="hwndEventVideo">The video event handler window to get notifications.</param>
			void CaptureTabControl::Init(HWND hwndEventAudio, HWND hwndEventVideo)
			{
				_tabCurrent = 0;

				// Create each tab page.
				_tabePages[0]->Create(IDD_MEDIACAPTURE_VIDEO_TAB, this);
				_tabePages[1]->Create(IDD_MEDIACAPTURE_AUDIO_TAB, this);

				// Show and hide.
				_tabePages[0]->ShowWindow(SW_SHOW);
				_tabePages[1]->ShowWindow(SW_HIDE);

				// Set the handlers.
				CaptureVideoPage* videopage = (CaptureVideoPage*)_tabePages[0];
				CaptureAudioPage* audiopage = (CaptureAudioPage*)_tabePages[1];
				videopage->SetEventHandler(hwndEventVideo);
				audiopage->SetEventHandler(hwndEventAudio);

				// Set the tab page rectangle.
				SetRectangle();
			}

			/// <summary>
			/// Set the tab rectangle.
			/// </summary>
			void CaptureTabControl::SetRectangle()
			{
				CRect tabRect;
				CRect itemRect;

				int x, y, xc, yc;

				// Get the client and item rectangle.
				GetClientRect(&tabRect);
				GetItemRect(0, &itemRect);

				// Set the tab page positions.
				x = itemRect.left;
				y = itemRect.bottom + 1;
				xc = tabRect.right - itemRect.left - 4;
				yc = tabRect.bottom - y - 3;

				// Set the ta page positions.
				_tabePages[0]->SetWindowPos(&wndTop, x, y, xc, yc, SWP_SHOWWINDOW);
				for (int i = 1; i < _numberOfPages; i++)
				{
					// Set the ta page positions.
					_tabePages[i]->SetWindowPos(&wndTop, x, y, xc, yc, SWP_HIDEWINDOW);
				}
			}

			/// <summary>
			/// Set the tab rectangle.
			/// </summary>
			/// <param name="index">The tab index to show all others are hidden.</param>
			void CaptureTabControl::ShowTabPage(int index)
			{
				// Make sure the selection is valid.
				if (index > -1 && index < _numberOfPages)
				{
					// Show current hide all others.
					_tabePages[index]->ShowWindow(SW_SHOW);
					for (int i = 0; i < _numberOfPages; i++)
					{
						// If not the current index hide.
						if (i != index)
						{
							// Hide all others.
							_tabePages[i]->ShowWindow(SW_HIDE);
						}
					}
				}
			}

			/// <summary>
			/// Gets a reference to the video tab page.
			/// </summary>
			/// <returns>The video page reference.</returns>
			CaptureVideoPage& CaptureTabControl::VideoPage() const
			{
				return *((CaptureVideoPage*)_tabePages[0]);
			}

			/// <summary>
			/// Gets a reference to the audio tab page.
			/// </summary>
			/// <returns>The audio page reference.</returns>
			CaptureAudioPage& CaptureTabControl::AudioPage() const
			{
				return *((CaptureAudioPage*)_tabePages[1]);
			}
		}
	}
}