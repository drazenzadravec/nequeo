/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Nequeo.Wpf.UI.Net
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
            wb.Navigated += Wb_Navigated;
            wb.Navigating += Wb_Navigating;
            wb.LoadCompleted += Wb_LoadCompleted;
        }

        private Action<Nequeo.Wpf.UI.Net.WebPage, string, System.Net.WebResponse> _documentComplete = null;
        private Action<Nequeo.Wpf.UI.Net.WebPage, string, string> _navigating = null;
        private Action<Nequeo.Wpf.UI.Net.WebPage, string, object> _navigated = null;

        /// <summary>
        /// Gets or sets the document complete action.
        /// </summary>
        public Action<Nequeo.Wpf.UI.Net.WebPage, string, System.Net.WebResponse> DocumentComplete
        {
            get { return _documentComplete; }
            set { _documentComplete = value; }
        }

        /// <summary>
        /// Gets or sets when navigated to
        /// a new document and has begun loading it.
        /// </summary>
        public Action<Nequeo.Wpf.UI.Net.WebPage, string, object> Navigated
        {
            get { return _navigated; }
            set { _navigated = value; }
        }

        /// <summary>
        /// Gets or sets before navigates to a new document.
        /// </summary>
        public Action<Nequeo.Wpf.UI.Net.WebPage, string, string> Navigating
        {
            get { return _navigating; }
            set { _navigating = value; }
        }

        /// <summary>
        /// Gets or sets the document Url.
        /// </summary>
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("The URL of the current document.")]
        public string Url
        {
            get
            {
                if (wb.Source != null)
                    return wb.Source.ToString();
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    wb.Source = new Uri(value);
            }
        }

        /// <summary>
        /// Gets the current web browser.
        /// </summary>
        public System.Windows.Controls.WebBrowser WebBrowser
        {
            get { return this.wb; }
        }

        /// <summary>
        /// Gets the current web page document.
        /// </summary>
        public object Document
        {
            get { return this.wb.Document; }
        }

        /// <summary>
        /// Set the document stream data to load into the web page.
        /// </summary>
        /// <param name="stream">The stream containing the page.</param>
        public void SetDocumentStream(System.IO.Stream stream)
        {
            wb.NavigateToStream(stream);
        }

        /// <summary>
        /// Set the document text data to load into the web page.
        /// </summary>
        /// <param name="text">The text containing the page.</param>
        public void SetDocumentText(string text)
        {
            wb.NavigateToString(text);
        }

        /// <summary>
        /// Set the URL of the current document.
        /// </summary>
        /// <param name="url">The URL of the current document.</param>
        public void SetUrl(Uri url)
        {
            wb.Source = url;
        }

        /// <summary>
        /// Loads the document at the location indicated by the specified System.Uri
        /// into a new browser window or into the System.Windows.Controls.WebBrowser control.
        /// </summary>
        /// <param name="url">A System.Uri representing the URL of the document to load
        /// into the System.Windows.Controls.WebBrowser control.</param>
        public void NavigateTo(Uri url)
        {
            wb.Navigate(url);
        }

        /// <summary>
        /// Occurs when the document being navigated to has finished downloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event argument.</param>
        private void Wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            // Document complete action.
            if (_documentComplete != null)
                _documentComplete(this, e.Uri.ToString(), e.WebResponse);
        }

        /// <summary>
        /// Occurs just before navigation to a document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event argument.</param>
        private void Wb_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_navigating != null)
                _navigating(this, e.Uri.ToString(), e.NavigationMode.ToString());
        }

        /// <summary>
        /// Occurs when the document being navigated to is located and has started downloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event argument.</param>
        private void Wb_Navigated(object sender, NavigationEventArgs e)
        {
            if (_navigated != null)
                _navigated(this, e.Uri.ToString(), e.Content);
        }
    }
}
