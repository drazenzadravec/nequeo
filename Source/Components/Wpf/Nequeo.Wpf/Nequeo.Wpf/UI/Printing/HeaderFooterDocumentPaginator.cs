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
using System.ComponentModel;
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
    /// Header footer document paginator, inserts a header and footer to each page.
    /// </summary>
    public class HeaderFooterDocumentPaginator : DocumentPaginator
    {
        /// <summary>
        /// Header footer document paginator, inserts a header and footer to each page.
        /// </summary>
        /// <param name="flowDocPaginator">The current flow document paginator.</param>
        /// <param name="header">The header string; can be null or empty.</param>
        /// <param name="footer">The footer string; can be null or empty.</param>
        /// <param name="height">The page height.</param>
        /// <param name="width">The page width.</param>
        public HeaderFooterDocumentPaginator(DocumentPaginator flowDocPaginator, string header, string footer, double height, double width)
        {
            this.flowDocPaginator = flowDocPaginator;
            this.headerText = header;
            this.footerText = footer;
            this.printableHeight = height;
            this.printableWidth = width;
            this.originalPageSize = new Size(width, height); // Size.Empty;

            // Register for events
            flowDocPaginator.GetPageCompleted += new GetPageCompletedEventHandler(HandleGetPageCompleted);
            flowDocPaginator.ComputePageCountCompleted += new AsyncCompletedEventHandler(HandleComputePageCountCompleted);
            flowDocPaginator.PagesChanged += new PagesChangedEventHandler(HandlePagesChanged);
        }

        private DocumentPaginator flowDocPaginator;
        private string headerText;
        private string footerText;
        private double printableHeight;
        private double printableWidth;
        private Size originalPageSize;

        /// <summary>
        /// Get the page from the document.
        /// </summary>
        /// <param name="pageNumber">The page in the document to return.</param>
        /// <returns>The document page.</returns>
        public override DocumentPage GetPage(int pageNumber)
        {

            DocumentPage documentPage = ConstructPageWithHeaderAndFooter(pageNumber);
            return documentPage;

        }

        /// <summary>
        /// get page async.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="userState">The user state.</param>
        public override void GetPageAsync(int pageNumber, object userState)
        {
            flowDocPaginator.GetPageAsync(pageNumber, userState);
        }

        /// <summary>
        /// Compute page count.
        /// </summary>
        public override void ComputePageCount()
        {
            flowDocPaginator.ComputePageCount();
        }

        /// <summary>
        /// Compute page count async.
        /// </summary>
        /// <param name="userState">The user state.</param>
        public override void ComputePageCountAsync(object userState)
        {
            flowDocPaginator.ComputePageCountAsync(userState);
        }

        /// <summary>
        /// Cancel async.
        /// </summary>
        /// <param name="userState">The user state.</param>
        public override void CancelAsync(object userState)
        {
            flowDocPaginator.CancelAsync(userState);
        }

        /// <summary>
        /// Override ToString method.
        /// </summary>
        /// <returns>The new string.</returns>
        public override string ToString()
        {
            return ((FlowDocument)flowDocPaginator.Source).Name;
        }

        /// <summary>
        /// Gets and indicator specifying if the page count is valid.
        /// </summary>
        public override bool IsPageCountValid
        {
            get
            {
                return flowDocPaginator.IsPageCountValid;
            }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public override int PageCount
        {
            get
            {
                return flowDocPaginator.PageCount;
            }
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public override Size PageSize
        {
            get
            {
                return flowDocPaginator.PageSize;
            }

            set
            {
                flowDocPaginator.PageSize = value;
            }
        }

        /// <summary>
        /// Gets the document paginator.
        /// </summary>
        public override IDocumentPaginatorSource Source
        {
            get
            {
                return flowDocPaginator.Source;
            }
        }

        /// <summary>
        /// Construct page with header and footer.
        /// </summary>
        /// <param name="pageNumber">The current page to transform.</param>
        /// <returns>The document page.</returns>
        private DocumentPage ConstructPageWithHeaderAndFooter(int pageNumber)
        {
            DocumentPage page0 = flowDocPaginator.GetPage(pageNumber);

            // Coming from WPFNotepad, the source document should always be a FlowDocument
            FlowDocument originalDocument = (FlowDocument)flowDocPaginator.Source;
            TextBlock headerBlock = null;
            TextBlock footerBlock = null;
            DocumentPage newPage = null;
            if (originalPageSize == Size.Empty)
            {
                originalPageSize = ((IDocumentPaginatorSource)originalDocument).DocumentPaginator.PageSize;
            }

            Size newPageSize = originalPageSize;

            // Decrease the top and/or bottom margins to account for headers/footers
            if ((headerText != null) && (headerText.Length > 0))
            {
                string expandedHeaderText = GetExpandedText(headerText, originalDocument, pageNumber + 1);
                headerBlock = new TextBlock();
                headerBlock.Text = expandedHeaderText;
                headerBlock.FontFamily = SystemFonts.MenuFontFamily;
                headerBlock.FontSize = 10;
                headerBlock.HorizontalAlignment = HorizontalAlignment.Center;

                headerBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                headerBlock.Arrange(new Rect(headerBlock.DesiredSize));
                headerBlock.UpdateLayout();
                if (headerBlock.DesiredSize.Width > 0 && headerBlock.DesiredSize.Height > 0)
                {
                    newPageSize.Height -= headerBlock.DesiredSize.Height;
                }
            }

            if ((footerText != null) && (footerText.Length > 0))
            {
                string expandedFooterText = GetExpandedText(footerText, originalDocument, pageNumber + 1);
                footerBlock = new TextBlock();
                footerBlock.Text = expandedFooterText;
                footerBlock.FontFamily = SystemFonts.MenuFontFamily;
                footerBlock.FontSize = 10;
                footerBlock.HorizontalAlignment = HorizontalAlignment.Center;

                footerBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                footerBlock.Arrange(new Rect(footerBlock.DesiredSize));
                footerBlock.UpdateLayout();
                if (footerBlock.DesiredSize.Width > 0 && footerBlock.DesiredSize.Height > 0)
                {
                    newPageSize.Height -= footerBlock.DesiredSize.Height;
                }

            }

            // Get the original page with its reduced size
            flowDocPaginator.PageSize = newPageSize;
            DocumentPage page = flowDocPaginator.GetPage(pageNumber);
            if (page != DocumentPage.Missing)
            {
                // Create a Grid that will hold the header, the original page, and the footer
                Grid grid = new Grid();
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(0, GridUnitType.Auto);
                grid.RowDefinitions.Add(rowDef);

                rowDef = new RowDefinition();
                rowDef.Height = new GridLength(0, GridUnitType.Star);
                grid.RowDefinitions.Add(rowDef);

                rowDef = new RowDefinition();
                rowDef.Height = new GridLength(0, GridUnitType.Auto);
                grid.RowDefinitions.Add(rowDef);

                ColumnDefinition columnDef = new ColumnDefinition();
                grid.ColumnDefinitions.Add(columnDef);

                // The header and footer TextBlocks can be added to the grid
                // directly.  The Visual from the original DocumentPage needs
                // to be hosted in a container that derives from UIElement.
                if (headerBlock != null)
                {
                    headerBlock.Margin = new Thickness(0, originalDocument.PagePadding.Top, 0, 0);
                    Grid.SetRow(headerBlock, 0);
                    grid.Children.Add(headerBlock);
                }

                VisualContainer container = new VisualContainer();
                container.PageVisual = page.Visual;
                container.PageSize = newPageSize;

                Grid.SetRow(container, 1);
                grid.Children.Add(container);

                if (footerBlock != null)
                {
                    footerBlock.Margin = new Thickness(0, 0, 0, originalDocument.PagePadding.Bottom);
                    Grid.SetRow(footerBlock, 2);
                    grid.Children.Add(footerBlock);
                }

                // Recalculate the children inside the Grid
                grid.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                grid.Arrange(new Rect(grid.DesiredSize));
                grid.UpdateLayout();


                // Return the new DocumentPage constructed from the Grid's Visual
                newPage = new DocumentPage(grid);
            }

            // Return the page.
            return newPage;
        }

        /// <summary>
        /// Get expanded text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="doc">The flow document.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <returns>The expanded text.</returns>
        private string GetExpandedText(string text, FlowDocument doc, int pageNumber)
        {
            // Replace instances of &f with the document's file name
            string expandedText = text.Replace("&f", GetDocumentFileName(doc));
            expandedText = expandedText.Replace("&F", GetDocumentFileName(doc));

            // Replace instances of &t with the current short time
            expandedText = expandedText.Replace("&t", DateTime.Now.ToShortTimeString());
            expandedText = expandedText.Replace("&T", DateTime.Now.ToShortTimeString());

            // Replace instances of &d with the current short date
            expandedText = expandedText.Replace("&d", DateTime.Now.ToShortDateString());
            expandedText = expandedText.Replace("&D", DateTime.Now.ToShortDateString());

            // Replace instances of &p with the current page number
            expandedText = expandedText.Replace("&p", pageNumber.ToString());
            expandedText = expandedText.Replace("&P", pageNumber.ToString());

            return expandedText;
        }

        /// <summary>
        /// Get the document file name.
        /// </summary>
        /// <param name="doc">The current flow document.</param>
        /// <returns>The name of the file.</returns>
        private string GetDocumentFileName(FlowDocument doc)
        {
            // Return the display name of the FlowDocument.
            return doc.Name;
        }

        /// <summary>
        /// We are being notified by the wrapped paginator.  If getting the page
        /// was successful, we use the resulting page to produce a new page that
        /// includes annotatons.  In either case, we fire an event from this instance.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">the args for this event</param>
        private void HandleGetPageCompleted(object sender, GetPageCompletedEventArgs e)
        {
            // If no errors, not cancelled, and page isn't missing, create a new page
            // with annotations and create a new event args for that page.
            if (!e.Cancelled && e.Error == null && e.DocumentPage != DocumentPage.Missing)
            {
                // Since we can't change the page the args is holding we create a new
                // args object with the page we produce.
                DocumentPage documentPage = ConstructPageWithHeaderAndFooter(e.PageNumber);

                e = new GetPageCompletedEventArgs(documentPage, e.PageNumber, e.Error, e.Cancelled, e.UserState);
            }

            // Fire the event
            OnGetPageCompleted(e);
        }

        /// <summary>
        /// We are notified by the wrapped paginator.  In response we fire
        /// an event from this instance.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">args for the event</param>
        private void HandleComputePageCountCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Fire the event
            OnComputePageCountCompleted(e);
        }

        /// <summary>
        /// We are notified by the wrapped paginator.  In response we fire
        /// an event from this instance.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">args for the event</param>
        private void HandlePagesChanged(object sender, PagesChangedEventArgs e)
        {
            // Fire the event
            OnPagesChanged(e);
        }
    }
}
