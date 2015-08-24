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
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Nequeo.Drawing.Printing
{
    /// <summary>
    /// Rich text box printable document.
    /// </summary>
    public class RichTextBoxDocument : PrintDocument
    {
        /// <summary>
        /// Rich text box printable document.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        public RichTextBoxDocument(RichTextBox richTextBox)
        {
            // store a reference to the RichTextBox to be rendered
            _richTextBox = richTextBox;

            // initialize header and footer
            Header = Footer = string.Empty;
            HeaderFont = FooterFont = new Font("Verdana", 9, FontStyle.Bold);
        }

        private int _firstChar;
        private int _currentPage;
        private int _pageCount;
        private RichTextBox _richTextBox;

        // special tags for headers/footers
        private const string PAGE = "[page]";
        private const string PAGES = "[pages]";

        /// <summary>
        /// Gets or sets the page header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the page footer.
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets the page header font.
        /// </summary>
        public Font HeaderFont { get; set; }

        /// <summary>
        /// Gets or sets the page footer font.
        /// </summary>
        public Font FooterFont { get; set; }

        /// <summary>
        /// Start printing the document
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            // we haven't printed anything yet
            _firstChar = 0;
            _currentPage = 0;

            // check whether we need a page count
            _pageCount = Header.IndexOf(PAGES) > -1 || Footer.IndexOf(PAGES) > -1
                ? -1
                : 0;

            // fire event as usual
            base.OnBeginPrint(e);
        }

        /// <summary>
        /// Render a page into the PrintDocument
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            // get a page count if that is required
            if (_pageCount < 0)
            {
                _pageCount = GetPageCount(e);
            }

            // update current page
            _currentPage++;

            // render text
            FORMATRANGE fmt = GetFormatRange(e, _firstChar);
            int nextChar = FormatRange(_richTextBox, true, ref fmt);
            e.Graphics.ReleaseHdc(fmt.hdc);

            // render header
            if (!string.IsNullOrEmpty(Header))
            {
                var rc = e.MarginBounds;
                rc.Y = 0;
                rc.Height = e.MarginBounds.Top;
                RenderHeaderFooter(e, Header, HeaderFont, rc);
                e.Graphics.DrawLine(Pens.Black, rc.X, rc.Bottom, rc.Right, rc.Bottom);
            }

            // render footer
            if (!string.IsNullOrEmpty(Footer))
            {
                var rc = e.MarginBounds;
                rc.Y = rc.Bottom;
                rc.Height = e.PageBounds.Bottom - rc.Y;
                RenderHeaderFooter(e, Footer, FooterFont, rc);
                e.Graphics.DrawLine(Pens.Black, rc.X, rc.Y, rc.Right, rc.Y);
            }

            // check whether we're done
            e.HasMorePages = nextChar > _firstChar && nextChar < _richTextBox.TextLength;

            // save start char for next time
            _firstChar = nextChar;

            // fire event as usual
            base.OnPrintPage(e);
        }

        /// <summary>
        /// Render a header or a footer on the current page
        /// </summary>
        /// <param name="e"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="rc"></param>
        void RenderHeaderFooter(PrintPageEventArgs e, string text, Font font, Rectangle rc)
        {
            var parts = text.Split('\t');
            if (parts.Length > 0)
            {
                RenderPart(e, parts[0], font, rc, StringAlignment.Near);
            }
            if (parts.Length > 1)
            {
                RenderPart(e, parts[1], font, rc, StringAlignment.Center);
            }
            if (parts.Length > 2)
            {
                RenderPart(e, parts[2], font, rc, StringAlignment.Far);
            }
        }

        /// <summary>
        /// Render a part of a header or footer on the page
        /// </summary>
        /// <param name="e"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="rc"></param>
        /// <param name="align"></param>
        void RenderPart(PrintPageEventArgs e, string text, Font font, Rectangle rc, StringAlignment align)
        {
            // replace wildcards
            text = text.Replace(PAGE, _currentPage.ToString());
            text = text.Replace(PAGES, _pageCount.ToString());

            // prepare string format
            StringFormat fmt = new StringFormat();
            fmt.Alignment = align;
            fmt.LineAlignment = StringAlignment.Center;

            // render footer
            e.Graphics.DrawString(text, font, Brushes.Black, rc, fmt);
        }

        /// <summary>
        /// build a FORMATRANGE structure with the proper page size and hdc
        /// (the hdc must be released after the FORMATRANGE is used)
        /// </summary>
        /// <param name="e"></param>
        /// <param name="firstChar"></param>
        /// <returns></returns>
        FORMATRANGE GetFormatRange(PrintPageEventArgs e, int firstChar)
        {
            // get page rectangle in twips
            var rc = e.MarginBounds;
            rc.X = (int)(rc.X * 14.4 + .5);
            rc.Y = (int)(rc.Y * 14.4 + .5);
            rc.Width = (int)(rc.Width * 14.4 + .5);
            rc.Height = (int)(rc.Height * 14.40 + .5);

            // set up FORMATRANGE structure
            var hdc = e.Graphics.GetHdc();
            var fmt = new FORMATRANGE();
            fmt.hdc = fmt.hdcTarget = hdc;
            fmt.rc.SetRect(rc);
            fmt.rcPage = fmt.rc;
            fmt.cpMin = firstChar;
            fmt.cpMax = -1;

            // done
            return fmt;
        }

        /// <summary>
        /// Get a page count by using FormatRange to measure the content
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        int GetPageCount(PrintPageEventArgs e)
        {
            int pageCount = 0;

            // count the pages using FormatRange
            FORMATRANGE fmt = GetFormatRange(e, 0);
            for (int firstChar = 0; firstChar < _richTextBox.TextLength;)
            {
                fmt.cpMin = firstChar;
                firstChar = FormatRange(_richTextBox, false, ref fmt);
                pageCount++;
            }
            e.Graphics.ReleaseHdc(fmt.hdc);

            // done
            return pageCount;
        }

        // messages used by RichEd20.dll
        const int
            WM_USER = 0x400,
            EM_FORMATRANGE = WM_USER + 0x39;

        // Win32 RECT
        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left, top, right, bottom;
            public void SetRect(Rectangle rc)
            {
                left = rc.Left;
                top = rc.Top;
                right = rc.Right;
                bottom = rc.Bottom;
            }
        }

        // FORMATRANGE is used by RichEd20.dll to render RTF
        [StructLayout(LayoutKind.Sequential)]
        struct FORMATRANGE
        {
            public IntPtr hdc, hdcTarget;
            public RECT rc, rcPage;
            public int cpMin, cpMax;
        }

        // send the EM_FORMATRANGE message to the RichTextBox to render or measure
        // a range of the document into a target specified by a FORMATRANGE structure.
        int FormatRange(RichTextBox rtb, bool render, ref FORMATRANGE fmt)
        {
            // render
            int nextChar = SendMessageFormatRange(
                rtb.Handle,
                EM_FORMATRANGE,
                render ? 1 : 0,
                ref fmt);

            // reset
            SendMessage(rtb.Handle, EM_FORMATRANGE, 0, 0);

            // return next character to print
            return nextChar;
        }

        // two flavors of SendMessage
        [DllImport("USER32.DLL", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hWnd, uint wMsg, int wParam, int lParam);

        [DllImport("USER32.DLL", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern int SendMessageFormatRange(IntPtr hWnd, uint wMsg, int wParam, ref FORMATRANGE lParam);
    }
}
