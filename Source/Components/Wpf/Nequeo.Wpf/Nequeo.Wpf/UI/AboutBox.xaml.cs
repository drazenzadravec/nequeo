/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// About box.
    /// </summary>
    public partial class AboutBox : Window
    {
        /// <summary>
        /// Default constructor is protected so callers must use one with a parent.
        /// </summary>
        protected AboutBox(bool isAboutBoxIntegrated = true, string productTitle = "", 
            string version = "", string description = "", string product = "",
            string copyright = "", string company = "", string linkText = "", 
            string linkUri = "")
        {
            _isAboutBoxIntegrated = isAboutBoxIntegrated;
            if (!_isAboutBoxIntegrated)
            {
                _productTitle = productTitle;
                _version = version;
                _description = description;
                _product = product;
                _copyright = copyright;
                _company = company;
                _linkText = linkText;
                _linkUri = linkUri;
            }

            InitializeComponent();
        }

        /// <summary>
        /// About box.
        /// </summary>
        /// <param name="parent">Parent window for this dialog.</param>
        /// <param name="isAboutBoxIntegrated">True if the about box integrated into the application, else within a component base..</param>
        /// <param name="productTitle"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        /// <param name="product"></param>
        /// <param name="copyright"></param>
        /// <param name="company"></param>
        /// <param name="linkText"></param>
        /// <param name="linkUri"></param>
        public AboutBox(Window parent, bool isAboutBoxIntegrated = true, string productTitle = "",
            string version = "", string description = "", string product = "",
            string copyright = "", string company = "", string linkText = "",
            string linkUri = "")
            : this(isAboutBoxIntegrated, productTitle, version, description, product, copyright, company, linkText, linkUri)
        {
            this.Owner = parent;
        }

        private XmlDocument _xmlDoc = null;
        private bool _isAboutBoxIntegrated = true;

        private string _productTitle = string.Empty;
        private string _version = string.Empty;
        private string _description = string.Empty;
        private string _product = string.Empty;
        private string _copyright = string.Empty;
        private string _company = string.Empty;
        private string _linkText = string.Empty;
        private string _linkUri = string.Empty;

        private const string propertyNameTitle = "Title";
        private const string propertyNameDescription = "Description";
        private const string propertyNameProduct = "Product";
        private const string propertyNameCopyright = "Copyright";
        private const string propertyNameCompany = "Company";
        private const string xPathRoot = "ApplicationInfo/";
        private const string xPathTitle = xPathRoot + propertyNameTitle;
        private const string xPathVersion = xPathRoot + "Version";
        private const string xPathDescription = xPathRoot + propertyNameDescription;
        private const string xPathProduct = xPathRoot + propertyNameProduct;
        private const string xPathCopyright = xPathRoot + propertyNameCopyright;
        private const string xPathCompany = xPathRoot + propertyNameCompany;
        private const string xPathLink = xPathRoot + "Link";
        private const string xPathLinkUri = xPathRoot + "Link/@Uri";
        
        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        public string ProductTitle
        {
            get
            {
                string result = CalculatePropertyValue<AssemblyTitleAttribute>(propertyNameTitle, xPathTitle);
                if (string.IsNullOrEmpty(result))
                {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
                return _isAboutBoxIntegrated ? result : _productTitle;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        public string Version
        {
            get
            {
                string result = string.Empty;
                // first, try to get the version string from the assembly.
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version != null)
                {
                    result = version.ToString();
                }
                else
                {
                    // if that fails, try to get the version from a resource in the Application.
                    result = GetLogicalResourceString(xPathVersion);
                }
                return _isAboutBoxIntegrated ? result : _version;
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get
            {
                return _isAboutBoxIntegrated ?
                    CalculatePropertyValue<AssemblyDescriptionAttribute>(propertyNameDescription, xPathDescription) : _description;

            }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get
            {
                return _isAboutBoxIntegrated ?
                    CalculatePropertyValue<AssemblyProductAttribute>(propertyNameProduct, xPathProduct) : _product;
            }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get
            {
                return _isAboutBoxIntegrated ?
                    CalculatePropertyValue<AssemblyCopyrightAttribute>(propertyNameCopyright, xPathCopyright) : _copyright;
            }
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public string Company
        {
            get
            {
                return _isAboutBoxIntegrated ?
                    CalculatePropertyValue<AssemblyCompanyAttribute>(propertyNameCompany, xPathCompany) : _company;
            }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText
        {
            get
            {
                return _isAboutBoxIntegrated ?
                    GetLogicalResourceString(xPathLink) : _linkText;
            }
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUri
        {
            get
            {
                return _isAboutBoxIntegrated ?
                    GetLogicalResourceString(xPathLinkUri) : _linkUri;
            }
        }
        
        /// <summary>
        /// Gets the specified property value either from a specific attribute, or from a resource dictionary.
        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <param name="xpathQuery">XPath to the element in the XML data resource.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        private string CalculatePropertyValue<T>(string propertyName, string xpathQuery)
        {
            string result = string.Empty;
            // first, try to get the property value from an attribute.
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                T attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    result = property.GetValue(attributes[0], null) as string;
                }
            }

            // if the attribute wasn't found or it did not have a value, then look in an xml resource.
            if (result == string.Empty)
            {
                // if that fails, try to get it from a resource.
                result = GetLogicalResourceString(xpathQuery);
            }
            return result;
        }

        /// <summary>
        /// Handles click navigation on the hyperlink in the About dialog.
        /// </summary>
        /// <param name="sender">Object the sent the event.</param>
        /// <param name="e">Navigation events arguments.</param>
        private void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets the XmlDataProvider's document from the resource dictionary.
        /// </summary>
        protected virtual XmlDocument ResourceXmlDocument
        {
            get
            {
                if (_xmlDoc == null)
                {
                    // if we haven't already found the resource XmlDocument, then try to find it.
                    XmlDataProvider provider = this.TryFindResource("aboutProvider") as XmlDataProvider;
                    if (provider != null)
                    {
                        // save away the XmlDocument, so we don't have to get it multiple times.
                        _xmlDoc = provider.Document;
                    }
                }
                return _xmlDoc;
            }
        }

        /// <summary>
        /// Gets the specified data element from the XmlDataProvider in the resource dictionary.
        /// </summary>
        /// <param name="xpathQuery">An XPath query to the XML element to retrieve.</param>
        /// <returns>The resulting string value for the specified XML element. 
        /// Returns empty string if resource element couldn't be found.</returns>
        protected virtual string GetLogicalResourceString(string xpathQuery)
        {
            string result = string.Empty;
            // get the About xml information from the resources.
            XmlDocument doc = this.ResourceXmlDocument;
            if (doc != null)
            {
                // if we found the XmlDocument, then look for the specified data. 
                XmlNode node = doc.SelectSingleNode(xpathQuery);
                if (node != null)
                {
                    if (node is XmlAttribute)
                    {
                        // only an XmlAttribute has a Value set.
                        result = node.Value;
                    }
                    else
                    {
                        // otherwise, need to just return the inner text.
                        result = node.InnerText;
                    }
                }
            }
            return result;
        }
    }
}
