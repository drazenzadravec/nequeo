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
    /// Web browser control.
    /// </summary>
    public partial class WebBrowser : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebBrowser()
        {
            InitializeComponent();

            // Set web broswer events
            wb.StatusTextChanged += wb_StatusTextChanged;
            wb.DocumentCompleted += wb_DocumentCompleted;
            wb.Navigating += wb_Navigating;
            wb.Navigated += wb_Navigated;
            wb.ProgressChanged += wb_ProgressChanged;
            wb.LocationChanged += wb_LocationChanged;
            wb.NewWindow += wb_NewWindow;
        }

        private string _homeUrl = null;
        private Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> _addressChanged = null;
        private Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> _newWindow = null;
        private Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> _documentComplete = null;
        private Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> _navigating = null;
        private Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> _navigated = null;
        private Action<Nequeo.Forms.UI.Net.WebBrowser> _locationChanged = null;

        /// <summary>
        /// Gets or sets the location changed action.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebBrowser> ControlLocationChanged
        {
            get { return _locationChanged; }
            set { _locationChanged = value; }
        }

        /// <summary>
        /// Gets or sets the address changed action.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> AddressChanged
        {
            get { return _addressChanged; }
            set { _addressChanged = value; }
        }

        /// <summary>
        /// Gets or sets the new window action.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> NewWindow
        {
            get { return _newWindow; }
            set { _newWindow = value; }
        }

        /// <summary>
        /// Gets or sets the document complete action.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> DocumentComplete
        {
            get { return _documentComplete; }
            set { _documentComplete = value; }
        }

        /// <summary>
        /// Gets or sets when navigated to
        /// a new document and has begun loading it.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> Navigated
        {
            get { return _navigated; }
            set { _navigated = value; }
        }

        /// <summary>
        /// Gets or sets before navigates to a new document.
        /// </summary>
        public Action<Nequeo.Forms.UI.Net.WebBrowser, string, string> Navigating
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
            get { return statusStrip.Visible; }
            set { statusStrip.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the menu strip visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Set the visibility of the menu strip.")]
        [NotifyParentProperty(true)]
        public bool MenuStripVisible
        {
            get { return menuStrip.Visible; }
            set { menuStrip.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the tool strip visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Set the visibility of the tool strip.")]
        [NotifyParentProperty(true)]
        public bool ToolStripVisible
        {
            get { return toolStrip.Visible; }
            set { toolStrip.Visible = value; }
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
        /// Gets or sets the home Url.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The URL of the current home page.")]
        [NotifyParentProperty(true)]
        public string HomeUrl
        {
            get
            {
                if (!String.IsNullOrEmpty(_homeUrl))
                    return _homeUrl;
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    _homeUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control can navigate to another
        /// page after its initial page has been loaded.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Specify whether the control can navigate to another page after its initial page has been loaded.")]
        [NotifyParentProperty(true)]
        public bool AllowNavigation
        {
            get { return wb.AllowNavigation; }
            set { wb.AllowNavigation = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WebBrowser
        /// control navigates to documents that are dropped onto it.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Specify indicating whether the WebBrowser control navigates to documents that are dropped onto it.")]
        [NotifyParentProperty(true)]
        public bool AllowWebBrowserDrop
        {
            get { return wb.AllowWebBrowserDrop; }
            set { wb.AllowWebBrowserDrop = value; }
        }

        /// <summary>
        /// Gets or a sets a value indicating whether the shortcut menu of the WebBrowser
        /// control is enabled.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Specify whether the shortcut menu of the WebBrowser control is enabled.")]
        [NotifyParentProperty(true)]
        public bool IsWebBrowserContextMenuEnabled
        {
            get { return wb.IsWebBrowserContextMenuEnabled; }
            set { wb.IsWebBrowserContextMenuEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WebBrowser
        /// displays dialog boxes such as script error messages.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Specify whether the WebBrowser displays dialog boxes such as script error messages.")]
        [NotifyParentProperty(true)]
        public bool ScriptErrorsSuppressed
        {
            get { return wb.ScriptErrorsSuppressed; }
            set { wb.ScriptErrorsSuppressed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether scroll bars are displayed in the
        /// WebBrowser control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Specify whether scroll bars are displayed in the WebBrowser control.")]
        [NotifyParentProperty(true)]
        public bool ScrollBarsEnabled
        {
            get { return wb.ScrollBarsEnabled; }
            set { wb.ScrollBarsEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether keyboard shortcuts are enabled within
        /// the WebBrowser control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Specify whether keyboard shortcuts are enabled within the WebBrowser control.")]
        [NotifyParentProperty(true)]
        public bool WebBrowserShortcutsEnabled
        {
            get { return wb.WebBrowserShortcutsEnabled; }
            set { wb.WebBrowserShortcutsEnabled = value; }
        }

        /// <summary>
        /// Gets the current web page html document.
        /// </summary>
        public System.Windows.Forms.HtmlDocument HtmlDocument
        {
            get { return this.wb.Document; }
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Validated(object sender, EventArgs e)
        {
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
        /// New browser window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_NewWindow(object sender, CancelEventArgs e)
        {
            // Cnacel new window open.
            e.Cancel = true;
            
            // Send new window opening.
            if (_newWindow != null)
                _newWindow(this, toolStripStatusText.Text, wb.DocumentTitle);
        }

        /// <summary>
        /// Document Completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripStatusText.Text = e.Url.ToString();
            toolStripButtonStop.Enabled = false;
            toolStripProgressBar.Value = 0;

            if (wb.CanGoBack)
                toolStripButtonBack.Enabled = true;
            else
                toolStripButtonBack.Enabled = false;

            if (wb.CanGoForward)
                toolStripButtonForward.Enabled = true;
            else
                toolStripButtonForward.Enabled = false;

            toolStripButtonSave.Enabled = true;
            toolStripButtonPrintPreview.Enabled = true;

            // Document complete action.
            if (_documentComplete != null)
                _documentComplete(this, e.Url.ToString(), wb.DocumentTitle);
        }

        /// <summary>
        /// Status Text Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_StatusTextChanged(object sender, EventArgs e)
        {
            toolStripStatusText.Text = wb.StatusText;
        }

        /// <summary>
        /// Navigated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (_navigated != null)
                _navigated(this, e.Url.ToString(), wb.DocumentTitle);
        }

        /// <summary>
        /// Navigating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripStatusText.Text = e.Url.ToString();
            toolStripButtonStop.Enabled = true;

            toolStripButtonSave.Enabled = false;
            toolStripButtonPrintPreview.Enabled = false;

            if (_navigating != null)
                _navigating(this, e.Url.ToString(), wb.DocumentTitle);
        }

        /// <summary>
        /// Progress Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (e.CurrentProgress > 0)
            {
                if (e.CurrentProgress <= e.MaximumProgress)
                {
                    // Change the progress bar.
                    toolStripProgressBar.Maximum = (int)e.MaximumProgress;
                    toolStripProgressBar.Value = (int)e.CurrentProgress;
                }
            }
        }

        /// <summary>
        /// Location changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wb_LocationChanged(object sender, EventArgs e)
        {
            if (_locationChanged != null)
                _locationChanged(this);
        }

        /// <summary>
        /// Load the Url.
        /// </summary>
        private void LoadUrl()
        {
            // If the item exists in the list.
            if (toolStripAddress.Items.Contains(toolStripAddress.Text))
            {
                // Get the Url.
                string url = toolStripAddress.Text;

                // Load the Url
                wb.Url = new Uri(url);
            }
            else
            {
                // Get the Url.
                string url = toolStripAddress.Text;

                // Add the url to the list.
                toolStripAddress.Items.Add(url);

                // Load the Url
                wb.Url = new Uri(url);
            }
        }

        /// <summary>
        /// Too strip address key pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                // Load the Url.
                LoadUrl();
            }
        }

        /// <summary>
        /// Url text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripAddress_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(toolStripAddress.Text))
                toolStripButtonRefresh.Enabled = false;
            else
                toolStripButtonRefresh.Enabled = true;

            // Send the new address.
            if (_addressChanged != null)
                _addressChanged(this, toolStripAddress.Text, wb.DocumentTitle);
        }

        /// <summary>
        /// Selected index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load the Url.
            LoadUrl();
        }

        /// <summary>
        /// Home Url button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonHome_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_homeUrl))
            {
                // Get the home Url.
                toolStripAddress.Text = _homeUrl;

                // Load the Url.
                LoadUrl();
            }
        }

        /// <summary>
        /// Refresh.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            // Load the Url.
            wb.Refresh();
        }

        /// <summary>
        /// Back.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonBack_Click(object sender, EventArgs e)
        {
            wb.GoBack();
        }

        /// <summary>
        /// Forward.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonForward_Click(object sender, EventArgs e)
        {
            wb.GoForward();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            wb.Stop();
        }

        /// <summary>
        /// Print preview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonPrintPreview_Click(object sender, EventArgs e)
        {
            wb.ShowPrintPreviewDialog();
        }

        /// <summary>
        /// Save the web page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            wb.ShowSaveAsDialog();
        }
    }
}
