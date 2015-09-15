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
using System.Drawing.Printing;

namespace Nequeo.Wpf.UI.Printing
{
    /// <summary>
    /// Preview control.
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        /// <summary>
        /// Preview control.
        /// </summary>
        public PreviewControl()
        {
            InitializeComponent();
        }

        private IDocumentPaginatorSource _document = null;

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        public IDocumentPaginatorSource Document
        {
            get
            {
                _document = documentViewer.Document;
                return _document;
            }
            set
            {
                _document = value;
                documentViewer.Document = _document;
            }
        }
    }

    /// <summary>
    /// Document paginator wrapper.
    /// </summary>
    internal class DocumentPaginatorWrapper : DocumentPaginator
    {
        /// <summary>
        /// Document paginator wrapper.
        /// </summary>
        /// <param name="paginator">The current document paginator.</param>
        /// <param name="pageSize">The page size the create.</param>
        /// <param name="margin">The page margins.</param>
        public DocumentPaginatorWrapper(DocumentPaginator paginator, Size pageSize, Size margin)

        {
            // Assign the values.
            _pageSize = pageSize;
            _margin = margin;
            _paginator = paginator;

            // Set the page size.
            _paginator.PageSize = new Size(_pageSize.Width - (_margin.Width * 2), _pageSize.Height - (_margin.Height * 2));
        }

        private Size _pageSize;
        private Size _margin;
        private DocumentPaginator _paginator;

        /// <summary>
        /// Move to the next rect.
        /// </summary>
        /// <param name="rect">The current rect.</param>
        /// <returns>The new rect.</returns>
        private Rect Move(Rect rect)
        {
            // If empty.
            if (rect.IsEmpty)
            {
                return rect;
            }
            else
            {
                // Set the rect.
                return new Rect(rect.Left + _margin.Width, rect.Top + _margin.Height, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// Get the page.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <returns>The document page.</returns>
        public override DocumentPage GetPage(int pageNumber)
        {
            // Get the document page.
            DocumentPage page = _paginator.GetPage(pageNumber);

            // Create a wrapper visual for transformation and add extras
            ContainerVisual newpage = new ContainerVisual();

            // Container.
            ContainerVisual smallerPage = new ContainerVisual();
            smallerPage.Children.Add(page.Visual);

            // Add the created page to the page.
            newpage.Children.Add(smallerPage);
            newpage.Transform = new TranslateTransform(_margin.Width, _margin.Height);

            // Return the new page.
            return new DocumentPage(newpage, _pageSize, Move(page.BleedBox), Move(page.ContentBox));
        }

        /// <summary>
        /// Gets the is page count valid indicator.
        /// </summary>
        public override bool IsPageCountValid
        {
            get
            {
                return _paginator.IsPageCountValid;
            }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public override int PageCount
        {
            get
            {
                return _paginator.PageCount;
            }
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public override Size PageSize
        {
            get
            {
                return _paginator.PageSize;
            }
            set
            {
                _paginator.PageSize = value;
            }
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public override IDocumentPaginatorSource Source
        {
            get
            {
                return _paginator.Source;
            }
        }
    }
}
