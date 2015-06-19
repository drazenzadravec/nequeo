/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPlayerForm.h
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

#pragma once

#ifndef _MEDIAPLAYERFORM_H
#define _MEDIAPLAYERFORM_H

#include "MediaGlobal.h"
#include "MediaPlayer.h"
#include "Volume.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media player window implementation. Player a media resources from this window.
			/// </summary>
			class MediaPlayerForm : public CDialog
			{
				/// <summary>
				/// Media player window enum.
				/// </summary>
				enum 
				{ 
					/// <summary>
					/// The id of the media player window.
					/// </summary>
					IDD = IDD_MEDIAPLAYERCONTROL 
				};

			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="pParent">The parent window.</param>
				MediaPlayerForm(CWnd* pParent = NULL);

				/// <summary>
				/// This destructor.
				/// </summary>
				~MediaPlayerForm();

			protected:
				/// <summary>
				/// Data exchange.
				/// </summary>
				/// <param name="pDX">Data exchange instance.</param>
				virtual void DoDataExchange(CDataExchange* pDX);

			private:
				/// <summary>
				/// Open file.
				/// </summary>
				void OpenFile();

				/// <summary>
				/// Open network URI.
				/// </summary>
				void OpenNetworkURI();

			private:
				bool _disposed;
				BOOL _repaintClient;
				bool _mute;

				MediaPlayer *_mediaPlayer;
				Volume *_volume;

				// Mapped controls.
				CToolTipCtrl* _toolTip;
				CButton _muteButton;
				CComboBox _openURL;

			public:
				/// <summary>
				/// Declare the message map.
				/// </summary>
				DECLARE_MESSAGE_MAP()
				afx_msg void OnClose();
				afx_msg void OnDestroy();
				afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
				afx_msg void OnPaint();
				afx_msg void OnSize(UINT nType, int cx, int cy);
				afx_msg LRESULT OnNotifyState(WPARAM wParam, LPARAM lParam);
				afx_msg LRESULT OnNotifyError(WPARAM wParam, LPARAM lParam);
				afx_msg LRESULT OnVolumeChanged(WPARAM wParam, LPARAM lParam);
				afx_msg void OnBnClickedButtonplay();
				afx_msg void OnBnClickedButtonstop();
				afx_msg void OnBnClickedButtonmute();
				afx_msg void OnCbnSelchangeComboopen();
			};
		}
	}
}
#endif
