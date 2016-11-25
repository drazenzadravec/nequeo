/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CaptureTabControl.h
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

#pragma once

#ifndef _CAPTURETABCONTROL_H
#define _CAPTURETABCONTROL_H

#include "MediaGlobal.h"
#include "CaptureAudioPage.h"
#include "CaptureVideoPage.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media capture tab control
			/// </summary>
			class CaptureTabControl : public CTabCtrl
			{
				DECLARE_DYNAMIC(CaptureTabControl)
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				CaptureTabControl();

				/// <summary>
				/// This destructor.
				/// </summary>
				~CaptureTabControl();

				/// <summary>
				/// Initialise.
				/// </summary>
				/// <param name="hwndEventAudio">The audio event handler window to get notifications.</param>
				/// <param name="hwndEventVideo">The video event handler window to get notifications.</param>
				void Init(HWND hwndEventAudio, HWND hwndEventVideo);

				/// <summary>
				/// Set the tab rectangle.
				/// </summary>
				void SetRectangle();

				/// <summary>
				/// Set the tab rectangle.
				/// </summary>
				/// <param name="index">The tab index to show all others are hidden.</param>
				void ShowTabPage(int index);

				/// <summary>
				/// Gets a reference to the video tab page.
				/// </summary>
				/// <returns>The video page reference.</returns>
				CaptureVideoPage& VideoPage() const;

				/// <summary>
				/// Gets a reference to the audio tab page.
				/// </summary>
				/// <returns>The audio page reference.</returns>
				CaptureAudioPage& AudioPage() const;

			private:
				bool _disposed;

				int _tabCurrent;
				int _numberOfPages;
				CDialog* _tabePages[2];

			public:
				/// <summary>
				/// Declare the message map.
				/// </summary>
				DECLARE_MESSAGE_MAP()
				
			};
		}
	}
}
#endif