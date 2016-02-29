/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreviewWindow.h
*  Purpose :       VideoPreviewWindow class.
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

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			namespace UI
			{

				using namespace System;
				using namespace System::ComponentModel;
				using namespace System::Collections;
				using namespace System::Windows::Forms;
				using namespace System::Data;
				using namespace System::Drawing;

				/// <summary>
				/// Summary for VideoPreviewWindow
				/// </summary>
				public ref class VideoPreviewWindow : public System::Windows::Forms::Form
				{
				public:
					/// <summary>
					/// Summary for VideoPreviewWindow
					/// </summary>
					/// <param name="videoCaptureIndex">The video capture index (default -1).</param>
					/// <param name="videoRenderIndex">The video render index (default -2).</param>
					VideoPreviewWindow(int videoCaptureIndex, int videoRenderIndex)
					{
						InitializeComponent();

						_videoCaptureIndex = videoCaptureIndex;
						_videoRenderIndex = videoRenderIndex;
					}
					/// <summary>
					/// Form is closing.
					/// </summary>
					event System::EventHandler^ OnVideoPreviewClosing;

				private:
					int _videoCaptureIndex;
					int _videoRenderIndex;

					/// <summary>
					/// Create the preview.
					/// </summary>
					void Create();

				protected:
					/// <summary>
					/// Clean up any resources being used.
					/// </summary>
					~VideoPreviewWindow()
					{
						if (components)
						{
							delete components;
						}
					}

				private:
					/// <summary>
					/// Required designer variable.
					/// </summary>
					System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
					/// <summary>
					/// Required method for Designer support - do not modify
					/// the contents of this method with the code editor.
					/// </summary>
					void InitializeComponent(void)
					{
						this->SuspendLayout();
						// 
						// VideoPreviewWindow
						// 
						this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
						this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
						this->ClientSize = System::Drawing::Size(276, 208);
						this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
						this->MaximizeBox = false;
						this->Name = L"VideoPreviewWindow";
						this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
						this->Text = L"Video Preview";
						this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &VideoPreviewWindow::VideoPreviewWindow_FormClosing);
						this->Load += gcnew System::EventHandler(this, &VideoPreviewWindow::VideoPreviewWindow_Load);
						this->ResumeLayout(false);

					}
#pragma endregion

				private: System::Void VideoPreviewWindow_Load(System::Object^  sender, System::EventArgs^  e);
				private: System::Void VideoPreviewWindow_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e);

				};
			}
		}
	}
}