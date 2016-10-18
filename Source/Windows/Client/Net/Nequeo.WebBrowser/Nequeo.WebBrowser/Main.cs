using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.WebBrowser
{
    /// <summary>
    /// Main.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Main.
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        private int _webBrowserIndex = 0;
        private string _homeUrl = "http://www.bing.com/";

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            webBrowser0.HomeUrl = _homeUrl;
            webBrowser0.DocumentComplete = (Nequeo.Forms.UI.Net.WebBrowser webBrowserSender, string address, string title) => AddressChanged(webBrowserSender, address, title);
            webBrowser0.AddressChanged = (Nequeo.Forms.UI.Net.WebBrowser webBrowserSender, string address, string title) => AddressChanged(webBrowserSender, address, title);
            webBrowser0.NewWindow = (Nequeo.Forms.UI.Net.WebBrowser webBrowserSender, string address, string title) => NewWindow(webBrowserSender, address, title);
        }

        /// <summary>
        /// The address has changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="address">The new address.</param>
        /// <param name="title">The document title.</param>
        private void AddressChanged(Nequeo.Forms.UI.Net.WebBrowser sender, string address, string title)
        {
            string webBrowserName = sender.Name;
            int webBrowserIndex = Int32.Parse(webBrowserName.Substring(10));

            // Assign the address to the tab control.
            tabControlMain.TabPages[webBrowserIndex].Text = (String.IsNullOrEmpty(title) ? address : title);
            tabControlMain.TabPages[webBrowserIndex].ToolTipText = address;
        }

        /// <summary>
        /// A new window event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="address">The new address.</param>
        /// <param name="title">The document title.</param>
        private void NewWindow(Nequeo.Forms.UI.Net.WebBrowser sender, string address, string title)
        {
            CreateNewWindow(address, title);
        }

        /// <summary>
        /// Create a new window.
        /// </summary>
        /// <param name="address">The new address.</param>
        /// <param name="title">The document title.</param>
        /// <param name="addAddress">Add address.</param>
        private void CreateNewWindow(string address, string title, bool addAddress = true)
        {
            _webBrowserIndex++;
            System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
            Nequeo.Forms.UI.Net.WebBrowser webBrowser = new Nequeo.Forms.UI.Net.WebBrowser();
            this.tabControlMain.Controls.Add(tabPage);

            // Add a new tab.
            tabPage.Controls.Add(webBrowser);
            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = "tabPage" + _webBrowserIndex.ToString();
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(989, 596);
            tabPage.TabIndex = 0;
            tabPage.UseVisualStyleBackColor = true;

            // Add a new web broswer.
            webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            webBrowser.HomeUrl = null;
            webBrowser.Location = new System.Drawing.Point(3, 3);
            webBrowser.MenuStripVisible = false;
            webBrowser.Name = "webBrowser" + _webBrowserIndex.ToString();
            webBrowser.Size = new System.Drawing.Size(983, 590);
            webBrowser.TabIndex = _webBrowserIndex;
            webBrowser.Url = null;
            webBrowser.HomeUrl = _homeUrl;
            webBrowser.DocumentComplete = (Nequeo.Forms.UI.Net.WebBrowser webBrowserSender, string addressNew, string titleNew) => AddressChanged(webBrowserSender, addressNew, titleNew);
            webBrowser.AddressChanged = (Nequeo.Forms.UI.Net.WebBrowser webBrowserSender, string addressNew, string titleNew) => AddressChanged(webBrowserSender, addressNew, titleNew);
            webBrowser.NewWindow = (Nequeo.Forms.UI.Net.WebBrowser webBrowserSender, string addressNew, string titleNew) => NewWindow(webBrowserSender, addressNew, titleNew);

            // If address are addded.
            if(addAddress)
            {
                tabPage.Text = (String.IsNullOrEmpty(title) ? address : title);
                tabPage.ToolTipText = address;
                webBrowser.SetUrl(new Uri(address));
            }
            else
            {
                tabPage.Text = (String.IsNullOrEmpty(title) ? address : title);
                tabPage.ToolTipText = address;
            }
        }

        /// <summary>
        /// Add new tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuNewTab_Click(object sender, EventArgs e)
        {
            CreateNewWindow("New Page", "New Page", false);
        }
    }
}
