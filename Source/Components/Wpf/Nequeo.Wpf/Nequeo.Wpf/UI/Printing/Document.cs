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
    /// Printable document.
    /// </summary>
    public class PrintableDocument
    {
        /// <summary>
        /// Printable text document.
        /// </summary>
        /// <param name="reader">The stream reader containing the data to print.</param>
        /// <param name="pageSize">The page size the create.</param>
        /// <param name="margin">The page margins.</param>
        /// <param name="fontSize">The font size to use when printing stream data.</param>
        public PrintableDocument(System.IO.Stream reader, Size pageSize, Size margin, double fontSize = 24.0)
        {
            _case = 1;

            // store a reference to the RichTextBox to be rendered
            _reader = reader;
            _margin = margin;
            _pageSize = pageSize;
            _fontSize = fontSize;
        }

        /// <summary>
        /// Printable text document.
        /// </summary>
        /// <param name="reader">The stream reader containing the data to print.</param>
        /// <param name="pageSize">The page size the create.</param>
        /// <param name="margin">The page margins.</param>
        /// <param name="font">The font to use when printing stream data.</param>
        /// <param name="fontSize">The font size to use when printing stream data.</param>
        /// <param name="brush">The font colour when printing stream data.</param>
        public PrintableDocument(System.IO.Stream reader, Size pageSize, Size margin, Typeface font, double fontSize, Brush brush)
        {
            _case = 1;

            // store a reference to the RichTextBox to be rendered
            _reader = reader;
            _font = font;
            _margin = margin;
            _brush = brush;
            _pageSize = pageSize;
            _fontSize = fontSize;
        }

        /// <summary>
        /// Printable document.
        /// </summary>
        /// <param name="reader">The stream reader containing the data to print.</param>
        /// <param name="dataFormat">A data format to load the data as. Currently supported data formats are System.Windows.DataFormats.Rtf,
        /// System.Windows.DataFormats.Text, System.Windows.DataFormats.Xaml, and System.Windows.DataFormats.XamlPackage.</param>
        /// <param name="pageSize">The page size the create.</param>
        /// <param name="margin">The page margins.</param>
        /// <param name="fontSize">The font size to use when printing stream data.</param>
        public PrintableDocument(System.IO.Stream reader, string dataFormat, Size pageSize, Size margin, double fontSize = 24.0)
        {
            _case = 0;

            // store a reference to the RichTextBox to be rendered
            _reader = reader;
            _dataFormat = dataFormat;
            _margin = margin;
            _pageSize = pageSize;
            _fontSize = fontSize;
        }

        /// <summary>
        /// Printable document.
        /// </summary>
        /// <param name="reader">The stream reader containing the data to print.</param>
        /// <param name="dataFormat">A data format to load the data as. Currently supported data formats are System.Windows.DataFormats.Rtf,
        /// System.Windows.DataFormats.Text, System.Windows.DataFormats.Xaml, and System.Windows.DataFormats.XamlPackage.</param>
        /// <param name="pageSize">The page size the create.</param>
        /// <param name="margin">The page margins.</param>
        /// <param name="font">The font to use when printing stream data.</param>
        /// <param name="fontSize">The font size to use when printing stream data.</param>
        /// <param name="brush">The font colour when printing stream data.</param>
        public PrintableDocument(System.IO.Stream reader, string dataFormat, Size pageSize, Size margin, Typeface font, double fontSize, Brush brush)
        {
            _case = 0;

            // store a reference to the RichTextBox to be rendered
            _reader = reader;
            _dataFormat = dataFormat;
            _font = font;
            _margin = margin;
            _brush = brush;
            _pageSize = pageSize;
            _fontSize = fontSize;
        }

        private int _case = -1;
        private Size _pageSize;
        private Size _margin;
        private double _fontSize;
        private Typeface _font = null;
        private Brush _brush = Brushes.Black;
        private string _dataFormat = DataFormats.Text;
        private System.IO.Stream _reader = null;

        /// <summary>
        /// Gets the document paginator.
        /// </summary>
        public IDocumentPaginatorSource Document
        {
            get
            {
                if (_reader != null)
                {
                    try
                    {
                        // Select the case to print.
                        switch (_case)
                        {
                            case 0:
                                return PrintFromStream();
                            case 1:
                                return PrintTextFromStream();
                            default:
                                return null;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Print from the stream.
        /// </summary>
        private IDocumentPaginatorSource PrintFromStream()
        {
            // Load the text data,
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(_reader, _dataFormat);

            // Set the font of the text in the stream.
            if (_font != null)
            {
                copy.Foreground = _brush;
                copy.FontFamily = _font.FontFamily;
                copy.FontStretch = _font.Stretch;
                copy.FontStyle = _font.Style;
                copy.FontWeight = _font.Weight;
            }

            // Used to set the correct page size.
            // This needs some more work.
            copy.FontSize = _fontSize;

            // Create the fixed document from the flow document.
            var paginator = new DocumentPaginatorWrapper(((IDocumentPaginatorSource)copy).DocumentPaginator, _pageSize, _margin);
            var package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
            var packUri = new Uri("pack://tempstream.xps");
            PackageStore.RemovePackage(packUri);
            PackageStore.AddPackage(packUri, package);
            var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
            XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
            FixedDocument fixedDocument = xps.GetFixedDocumentSequence().References[0].GetDocument(true);

            // Return the document source.
            return fixedDocument;
        }

        /// <summary>
        /// Print text from the stream.
        /// </summary>
        private IDocumentPaginatorSource PrintTextFromStream()
        {
            // Load the text data,
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(_reader, DataFormats.Text);

            // Set the font of the text in the stream.
            if (_font != null)
            {
                copy.Foreground = _brush;
                copy.FontFamily = _font.FontFamily;
                copy.FontStretch = _font.Stretch;
                copy.FontStyle = _font.Style;
                copy.FontWeight = _font.Weight;
            }

            // Used to set the correct page size.
            // This needs some more work.
            copy.FontSize = _fontSize;

            // Create the fixed document from the flow document.
            var paginator = new DocumentPaginatorWrapper(((IDocumentPaginatorSource)copy).DocumentPaginator, _pageSize, _margin);
            var package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
            var packUri = new Uri("pack://temptextstream.xps");
            PackageStore.RemovePackage(packUri);
            PackageStore.AddPackage(packUri, package);
            var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
            XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
            FixedDocument fixedDocument = xps.GetFixedDocumentSequence().References[0].GetDocument(true);

            // Return the document source.
            return fixedDocument;
        }
    }
}
