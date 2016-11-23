/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaCaptureForm.h
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

#pragma once

#ifndef _MEDIACAPTUREFORM_H
#define _MEDIACAPTUREFORM_H

#include "MediaGlobal.h"
#include "MediaCapture.h"
#include "MediaPreviewForm.h"
#include "Volume.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media capture window implementation. Capture a media resources from this window.
			/// </summary>
			class MediaCaptureForm : public CDialog
			{
				/// <summary>
				/// Media capture window enum.
				/// </summary>
				enum
				{
					/// <summary>
					/// The id of the media capture window.
					/// </summary>
					IDD = IDD_MEDIACAPTURECONTROL
				};

			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="pParent">The parent window.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API MediaCaptureForm(CWnd* pParent = NULL);

				/// <summary>
				/// This destructor.
				/// </summary>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API ~MediaCaptureForm();

				/// <summary>
				/// On the windows as a modal.
				/// </summary>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API INT_PTR DoModal() override;

			protected:
				/// <summary>
				/// Data exchange.
				/// </summary>
				/// <param name="pDX">Data exchange instance.</param>
				virtual void DoDataExchange(CDataExchange* pDX);

			private:
				/// <summary>
				/// Enable or diable controls.
				/// </summary>
				void EnableControls();

				/// <summary>
				/// On initialize dialog.
				/// </summary>
				BOOL OnInitDialog() override;

				/// <summary>
				/// Pre-translate message.
				/// </summary>
				/// <param name="pMsg">The message.</param>
				BOOL PreTranslateMessage(MSG* pMsg) override;

			private:
				bool _disposed;

				int _selectedIndexVideo;
				int _selectedIndexAudio;

				int _countVideo;
				int _countAudio;

				MediaCapture *_mediaCapture;
				Volume *_volume;
				CriticalSectionHandler	_critsec;

				HWND _hEvent;
				HWND _hCapture;

				// Mapped controls.
				CToolTipCtrl *_toolTip;
				CComboBox _videoDeviceItem;
				CComboBox _audioDeviceItem;

				CaptureDeviceParam *_videoDevice;
				CaptureDeviceParam *_audioDevice;

				MediaPreviewForm *_preview;

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
				afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);
				afx_msg LRESULT OnNotifyState(WPARAM wParam, LPARAM lParam);
				afx_msg LRESULT OnNotifyError(WPARAM wParam, LPARAM lParam);
				afx_msg void OnBnClickedButtonVideoPreview();
				afx_msg void OnBnClickedButtonRefreshDevices();
				afx_msg void OnCbnSelchangeCombVideo();
				afx_msg void OnCbnSelchangeCombAudio();
			};
		}
	}
}
#endif