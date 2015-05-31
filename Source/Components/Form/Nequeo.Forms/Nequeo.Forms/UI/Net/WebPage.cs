/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Forms.UI.Net
{
    /// <summary>
    /// Web page user control.
    /// </summary>
    public partial class WebPage : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebPage()
        {
            InitializeComponent();

            // Set web broswer events
            wb.StatusTextChanged += WebBrowser_StatusTextChanged;
            wb.DocumentCompleted += WebBrowser_DocumentCompleted;
            wb.Navigating += WebBrowser_Navigating;
            wb.Navigated += WebBrowser_Navigated;
            wb.LocationChanged += WebBrowser_LocationChanged;
        }

        private Action<Nequeo.Forms.UI.Net.WebPage, string, string> _documentComplete = null;
        private Action<Nequeo.Forms.UI.Net.WebPage, string, string> _navigating = null;
        private Action<Nequeo.Forms.UI.Net.WebPage, string, string> _navigated = null;
        private Action<Nequeo.Forms.UI.Net.WebPage> _locationChanged = null;

        /// <summary>
        /// Gets or sets the location changed action.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebPage> ControlLocationChanged
        {
            get { return _locationChanged; }
            set { _locationChanged = value; }
        }

        /// <summary>
        /// Gets or sets the document complete action.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebPage, string, string> DocumentComplete
        {
            get { return _documentComplete; }
            set { _documentComplete = value; }
        }

        /// <summary>
        /// Gets or sets when navigated to
        /// a new document and has begun loading it.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebPage, string, string> Navigated
        {
            get { return _navigated; }
            set { _navigated = value; }
        }

        /// <summary>
        /// Gets or sets before navigates to a new document.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebPage, string, string> Navigating
        {
            get { return _navigating; }
            set { _navigating = value; }
        }

        /// <summary>
        /// Gets or sets the status strip visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Set the visibility of the status strip.")]
        [NotifyParentProperty(true)]
        public bool StatusStripVisible
        {
            get { return statusStripWebBrowser.Visible; }
            set { statusStripWebBrowser.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the document Url.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The URL of the current document.")]
        [NotifyParentProperty(true)]
        public string Url
        {
            get 
            {
                if (wb.Url != null)
                    return wb.Url.ToString();
                else
                    return null;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    wb.Url = new Uri(value); 
            }
        }

        /// <summary>
        /// Gets the current web browser.
        /// </summary>
        public System.Windows.Forms.WebBrowser WebBrowser
        {
            get { return this.wb; }
        }

        /// <summary>
        /// Gets the current web page html document.
        /// </summary>
        public System.Windows.Forms.HtmlDocument HtmlDocument
        {
            get { return this.wb.Document; }
        }

        /// <summary>
        /// Set the document stream data to load into the web page.
        /// </summary>
        /// <param name="stream">The stream containing the page.</param>
        public void SetDocumentStream(System.IO.Stream stream)
        {
            wb.DocumentStream = stream;
        }

        /// <summary>
        /// Set the document text data to load into the web page.
        /// </summary>
        /// <param name="text">The text containing the page.</param>
        public void SetDocumentText(string text)
        {
            wb.DocumentText = text;
        }

        /// <summary>
        /// Set the URL of the current document.
        /// </summary>
        /// <param name="url">The URL of the current document.</param>
        public void SetUrl(Uri url)
        {
            wb.Url = url;
        }

        /// <summary>
        /// Loads the document at the location indicated by the specified System.Uri
        /// into a new browser window or into the System.Windows.Forms.WebBrowser control.
        /// </summary>
        /// <param name="url">A System.Uri representing the URL of the document to load.</param>
        /// <param name="newWindow">True to load the document into a new browser window; false to load the document
        /// into the System.Windows.Forms.WebBrowser control.</param>
        public void NavigateTo(Uri url, bool newWindow = false)
        {
            wb.Navigate(url, newWindow);
        }

        /// <summary>
        /// Document Completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripStatusText.Text = e.Url.ToString();

            // Document complete action.
            if (_documentComplete != null)
                _documentComplete(this, e.Url.ToString(), wb.DocumentTitle);
        }

        /// <summary>
        /// Status Text Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            toolStripStatusText.Text = wb.StatusText;
        }

        /// <summary>
        /// Navigating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripStatusText.Text = e.Url.ToString();

            if (_navigating != null)
                _navigating(this, e.Url.ToString(), wb.DocumentTitle);
        }

        /// <summary>
        /// Navigated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (_navigated != null)
                _navigated(this, e.Url.ToString(), wb.DocumentTitle);
        }

        /// <summary>
        /// Location changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebBrowser_LocationChanged(object sender, EventArgs e)
        {
            if (_locationChanged != null)
                _locationChanged(this);
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebPage_Load(object sender, EventArgs e)
        {
        }
    }
}
