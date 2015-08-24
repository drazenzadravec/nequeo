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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace Nequeo.Drawing.Printing
{
    /// <summary>
    /// Printing document.
    /// </summary>
    public class Document : PrintDocument
    {
        /// <summary>
        /// Printing document.
        /// </summary>
        public Document()
        {
            _case = -1;
            _line = 0;
        }

        /// <summary>
        /// Printing document.
        /// </summary>
        /// <param name="reader">The stream reader containing the data to print.</param>
        /// <param name="font">The font to use when printing stream data.</param>
        /// <param name="brush">The font colour when printing stream data.</param>
        public Document(System.IO.StreamReader reader, System.Drawing.Font font, Brush brush)
        {
            _case = 0;
            _reader = reader;
            _font = font;
            _brush = brush;
        }

        /// <summary>
        /// Printing document.
        /// </summary>
        /// <param name="text">The array of text containing the data to print.</param>
        /// <param name="font">The font to use when printing stream data.</param>
        /// <param name="brush">The font colour when printing stream data.</param>
        public Document(string[] text, System.Drawing.Font font, Brush brush)
        {
            _case = 1;
            _line = 0;
            _text = text;
            _font = font;
            _brush = brush;
        }

        private int _case = -1;
        private int _line = 0;

        private string[] _text = null;
        private System.IO.StreamReader _reader = null;
        private System.Drawing.Font _font;
        private Brush _brush = Brushes.Black;

        /// <summary>
        /// Start printing the document
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            // fire event as usual
            base.OnBeginPrint(e);
        }

        /// <summary>
        /// Render a page into the PrintDocument
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            switch (_case)
            {
                case 0:
                    PrintFromStream(e);
                    break;
                case 1:
                    PrintFromArray(e);
                    break;
                default:
                    break;
            }

            // fire event as usual
            base.OnPrintPage(e);
        }

        /// <summary>
        /// Print from the stream.
        /// </summary>
        /// <param name="e"></param>
        private void PrintFromStream(PrintPageEventArgs e)
        {
            float yPos = 0f;
            int count = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            string line = null;
            float linesPerPage = e.MarginBounds.Height / _font.GetHeight(e.Graphics);

            // While lines exist.
            while (count < linesPerPage)
            {
                line = _reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                // Draw each line in the stream.
                yPos = topMargin + count * _font.GetHeight(e.Graphics);
                e.Graphics.DrawString(line, _font, _brush, leftMargin, yPos, new StringFormat());
                count++;
            }

            // More lines exist.
            if (line != null)
            {
                e.HasMorePages = true;
            }
        }

        /// <summary>
        /// Print from the stream.
        /// </summary>
        /// <param name="e"></param>
        private void PrintFromArray(PrintPageEventArgs e)
        {
            float yPos = 0f;
            int count = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            string line = null;
            float linesPerPage = e.MarginBounds.Height / _font.GetHeight(e.Graphics);

            // While lines exist.
            while (count < linesPerPage)
            {
                line = _text[_line];
                if (_text.Length >= _line)
                {
                    break;
                }

                // Draw each line in the stream.
                yPos = topMargin + count * _font.GetHeight(e.Graphics);
                e.Graphics.DrawString(line, _font, _brush, leftMargin, yPos, new StringFormat());
                count++;
                _line++;
            }

            // More lines exist.
            if (_text.Length < _line)
            {
                e.HasMorePages = true;
            }
        }
    }
}
