/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.VoIP.PjSip.UI
{
    /// <summary>
    /// Video preview.
    /// </summary>
    public partial class VideoPreview : Form
    {
        /// <summary>
        /// Instant message.
        /// </summary>
        /// <param name="voipCall">VoIP call.</param>
        public VideoPreview(Nequeo.VoIP.PjSip.VoIPCall voipCall)
        {
            InitializeComponent();

            _voipCall = voipCall;
            Create();
        }

        private bool _disposed = false;
        private Nequeo.VoIP.PjSip.VoIPCall _voipCall = null;
        private Nequeo.Net.PjSip.VideoPreview _videoPreview = null;
        private Nequeo.Net.PjSip.VideoWindow _window = null;

        /// <summary>
        /// Form is closing.
        /// </summary>
        public event System.EventHandler OnVideoPreviewClosing;

        /// <summary>
        /// Dispose of the unmanaged resources.
        /// </summary>
        public void DisposeCall()
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                try
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (_window != null)
                        _window.Dispose();
                }
                catch { }

                try
                {
                    // The video preview.
                    if (_videoPreview != null)
                        _videoPreview.Dispose();
                }
                catch { }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _window = null;
                _videoPreview = null;
            }
        }

        /// <summary>
        /// Create the preview.
        /// </summary>
        private void Create()
        {
            try
            {
                // Get the current video capture device id.
                int videoCapID = _voipCall.VoIPManager.MediaManager.GetVideoCaptureDeviceID();
                int videoRenID = _voipCall.VoIPManager.MediaManager.GetVideoRenderDeviceID();

                // If a capture device exist.
                if (videoCapID >= -1)
                {
                    // Create.
                    _videoPreview = new Nequeo.Net.PjSip.VideoPreview(videoCapID);

                    // Configure.
                    Nequeo.Net.PjSip.VideoPreviewOpParam parm = new Nequeo.Net.PjSip.VideoPreviewOpParam();

                    Nequeo.Net.PjSip.VideoWindowHandle handle = new Nequeo.Net.PjSip.VideoWindowHandle();
                    handle.Type = Nequeo.Net.PjSip.VideoDeviceHandleType.PJMEDIA_VID_DEV_HWND_TYPE_WINDOWS;

                    Nequeo.Net.PjSip.MediaFormat format = new Nequeo.Net.PjSip.MediaFormat();
                    format.Type = Nequeo.Net.PjSip.MediaType.PJMEDIA_TYPE_VIDEO;

                    // Assign.
                    parm.Show = false;
                    parm.Format = format;
                    parm.WindowFlags = 0;
                    parm.RenderID = videoRenID;
                    parm.Window = handle;

                    // Show.
                    _videoPreview.Start(parm);
                    _window = _videoPreview.GetVideoWindow();
                }
            }
            catch { }
        }

        /// <summary>
        /// Form move.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPreview_Move(object sender, EventArgs e)
        {
            int top = this.Top + 43;
            int left = this.Left + 20;

            try
            {
                _window.SetPosition(
                    new Nequeo.Net.PjSip.MediaCoordinate()
                    {
                        X = left,
                        Y = top
                    });
            }
            catch { }
        }

        /// <summary>
        /// Form load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPreview_Load(object sender, EventArgs e)
        {
            panel1.Width = (int)_window.GetInfo().WindowSize.Width;
            panel1.Height = (int)_window.GetInfo().WindowSize.Height;

            this.Width = panel1.Width + 43;
            this.Height = panel1.Height + 60;

            try
            {
                // Show the preview.
                _window.Show(true);
            }
            catch { }
        }

        /// <summary>
        /// Closing the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _window.Show(false);
                _videoPreview.Stop();
            }
            catch { }
            DisposeCall();

            // Send the form closing event.
            OnVideoPreviewClosing?.Invoke(this, new EventArgs());
        }
    }
}
