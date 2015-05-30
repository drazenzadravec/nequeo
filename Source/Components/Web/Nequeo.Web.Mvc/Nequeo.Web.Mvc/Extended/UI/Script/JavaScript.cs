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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Nequeo.Extension;
using Nequeo.Web.Mvc.Extended.Factory;
using Nequeo.Web.Mvc.Extended.WebAsset;
using Nequeo.Web.Mvc.Extended.UI.ControlBase;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// Javascript script Mvc writer.
    /// </summary>
    public class JavaScript : ScriptBase<string>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="viewContext">The current Mvc view context.</param>
        /// <param name="clientSideObjectWriterFactory">The client side object writer factory.</param>
        public JavaScript(ViewContext viewContext, IClientSideObjectWriterFactory clientSideObjectWriterFactory)
            : base(viewContext, clientSideObjectWriterFactory)
        {
        }

        /// <summary>
        /// Writes the Html data to the output stream.
        /// </summary>
        /// <param name="writer">The Html text writer to the current Mvc view context output stream.</param>
        protected override void WriteHtml(System.Web.UI.HtmlTextWriter writer)
        {
            IList<WebAssetItem> scripts = base.Scripts.Items;
            string defaultPath = base.Scripts.DefaultPath;

            // If scripts exist.
            if (scripts != null)
            {
                // For each script add to the curent page.
                foreach (WebAssetItem script in scripts)
                {
                    string path = (String.IsNullOrEmpty(defaultPath) ? "" : script.Source.Replace(WebAssetDefaultSettings.ScriptFilesPath, defaultPath));
                    writer.WriteLine("<script type=\"text/javascript\" src=\"{0}\"></script>".FormatWith(path));
                }
            }
        }
    }
}
