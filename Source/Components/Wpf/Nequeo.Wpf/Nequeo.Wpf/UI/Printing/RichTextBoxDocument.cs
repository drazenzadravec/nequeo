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
using System.IO;
using System.IO.Packaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps;
using System.Windows.Xps.Serialization;
using System.Windows.Xps.Packaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;

namespace Nequeo.Wpf.UI.Printing
{
    /// <summary>
    /// Rich text box printable document.
    /// </summary>
    public class RichTextBoxDocument
    {
        /// <summary>
        /// Rich text box printable document.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <param name="pageSize">The page size the create.</param>
        /// <param name="margin">The page margins.</param>
        /// <param name="fontSize">The font size to use when printing stream data.</param>
        public RichTextBoxDocument(RichTextBox richTextBox, Size pageSize, Size margin, double fontSize = 24.0)
        {
            // store a reference to the RichTextBox to be rendered
            _richTextBox = richTextBox;
            _fontSize = fontSize;
            _margin = margin;
            _pageSize = pageSize;
        }

        private Size _pageSize;
        private Size _margin;
        private double _fontSize;
        private RichTextBox _richTextBox;

        /// <summary>
        /// Gets the document paginator.
        /// </summary>
        public IDocumentPaginatorSource Document
        {
            get
            {
                if(_richTextBox != null)
                {
                    // Get the document from the box.
                    FlowDocument flowDocument = _richTextBox.Document;
                    System.IO.MemoryStream stream = null;

                    try
                    {
                        // Clone the source document's content into a new FlowDocument.
                        // This is because the pagination for the printer needs to be
                        // done differently than the pagination for the displayed page.
                        // We print the copy, rather that the original FlowDocument.
                        stream = new System.IO.MemoryStream();
                        TextRange source = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                        source.Save(stream, DataFormats.Rtf);
                        FlowDocument copy = new FlowDocument();
                        TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
                        dest.Load(stream, DataFormats.Rtf);

                        // Used to set the correct page size.
                        // This needs some more work.
                        copy.FontSize = _fontSize;

                        // Create the fixed document from the flow document.
                        var paginator = new DocumentPaginatorWrapper(((IDocumentPaginatorSource)copy).DocumentPaginator, _pageSize, _margin);
                        var package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
                        var packUri = new Uri("pack://temprtf.xps");
                        PackageStore.RemovePackage(packUri);
                        PackageStore.AddPackage(packUri, package);
                        var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
                        XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
                        FixedDocument fixedDocument = xps.GetFixedDocumentSequence().References[0].GetDocument(true);

                        // Return the document source.
                        return fixedDocument;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
