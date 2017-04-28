/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OpenNetworkUriForm.cpp
*  Purpose :       OpenNetworkUriForm class.
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

#include "OpenNetworkUriForm.h"

/// <summary>
/// Begin messsage map.
/// </summary>
BEGIN_MESSAGE_MAP(Nequeo::Media::Foundation::OpenNetworkUriForm, CDialog)
	ON_WM_CREATE()
	ON_BN_CLICKED(IDCANCEL, &OpenNetworkUriForm::OnBnClickedCancel)
	ON_BN_CLICKED(IDOK, &OpenNetworkUriForm::OnBnClickedOk)
	ON_EN_CHANGE(IDC_EDITNETWORKURI, &OpenNetworkUriForm::OnEnChangeEditNetworUuri)
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
			OpenNetworkUriForm::OpenNetworkUriForm(CWnd* pParent) : CDialog(OpenNetworkUriForm::IDD, pParent),
				_disposed(false), _networkURL(""), _toolTip(NULL)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			OpenNetworkUriForm::~OpenNetworkUriForm()
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
			/// On create window.
			/// </summary>
			/// <param name="lpCreateStruct">The create structure.</param>
			/// <returns>Zero if the window should be created; else -1 if the window should be distroyed</returns>
			int OpenNetworkUriForm::OnCreate(LPCREATESTRUCT lpCreateStruct)
			{
				if (CDialog::OnCreate(lpCreateStruct) == -1)
					return -1;

				return 0;
			}

			/// <summary>
			/// On show window.
			/// </summary>
			/// <param name="bShow">Show the window.</param>
			/// <param name="nStatus">The window status.</param>
			void OpenNetworkUriForm::OnShowWindow(BOOL bShow, UINT nStatus)
			{
				// Show the window.
				CDialog::OnShowWindow(bShow, nStatus);

			}

			/// <summary>
			/// On initialize dialog.
			/// </summary>
			BOOL OpenNetworkUriForm::OnInitDialog()
			{
				CDialog::OnInitDialog();

				// Create the ToolTip control.
				_toolTip = new CToolTipCtrl();
				_toolTip->Create(this);

				// Activate
				_toolTip->Activate(TRUE);

				// return TRUE  unless you set the focus to a control.
				return TRUE;
			}

			/// <summary>
			/// Pre-translate message.
			/// </summary>
			/// <param name="pMsg">The message.</param>
			BOOL OpenNetworkUriForm::PreTranslateMessage(MSG* pMsg)
			{
				if (_toolTip != NULL)
					_toolTip->RelayEvent(pMsg);

				return CDialog::PreTranslateMessage(pMsg);
			}

			/// <summary>
			/// Gets the network URL.
			/// </summary>
			/// <returns>The URL text.</returns>
			string OpenNetworkUriForm::GetNetworkURL() const
			{
				return _networkURL;
			}

			/// <summary>
			/// Data exchange.
			/// </summary>
			/// <param name="pDX">Data exchange instance.</param>
			void OpenNetworkUriForm::DoDataExchange(CDataExchange* pDX)
			{
				CDialog::DoDataExchange(pDX);
				DDX_Control(pDX, IDC_EDITNETWORKURI, _textURL);
			}

			/// <summary>
			/// On button cancel clicked.
			/// </summary>
			void OpenNetworkUriForm::OnBnClickedCancel()
			{
				_networkURL.clear();
				CDialog::OnCancel();
			}

			/// <summary>
			/// On button ok clicked.
			/// </summary>
			void OpenNetworkUriForm::OnBnClickedOk()
			{
				// Get the text entered.
				CString text;
				_textURL.GetWindowTextW(text);

				// If text exists.
				if (text.GetLength() > 0)
				{
					CString newText = text.Trim();

					// Convert to the string.
					_networkURL.clear();
					_networkURL = string(CW2A(newText.GetString()));
				}
				else
				{
					// Clear the string.
					_networkURL.clear();
				}
				CDialog::OnOK();
			}

			/// <summary>
			/// On text changed.
			/// </summary>
			void OpenNetworkUriForm::OnEnChangeEditNetworUuri()
			{
				// TODO:  If this is a RICHEDIT control, the control will not
				// send this notification unless you override the CDialog::OnInitDialog()
				// function and call CRichEditCtrl().SetEventMask()
				// with the ENM_CHANGE flag ORed into the mask.

				// TODO:  Add your control notification handler code here
			}

		}
	}
}



