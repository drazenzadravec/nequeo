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
    /// Specifies the zoom mode for the <see cref="PreviewControl"/> control.
    /// </summary>
    public enum ZoomMode
    {
        /// <summary>
        /// Show the preview in actual size.
        /// </summary>
        ActualSize,
        /// <summary>
        /// Show a full page.
        /// </summary>
        FullPage,
        /// <summary>
        /// Show a full page width.
        /// </summary>
        PageWidth,
        /// <summary>
        /// Show two full pages.
        /// </summary>
        TwoPages,
        /// <summary>
        /// Use the zoom factor specified by the <see cref="PreviewControl.Zoom"/> property.
        /// </summary>
        Custom
    }

#if false

    /// <summary>
    /// This version of the PageImageList stores images as byte arrays. It is a little
    /// more complex and slower than a simple list, but doesn't consume GDI resources.
    /// This is important when the list contains lots of images (Windows only supports
    /// 10,000 simultaneous GDI objects!)
    /// </summary>
    public class PageImageList
    {
        // ** fields
        List<byte[]> _list = new List<byte[]>();

        // ** object model
        public void Clear()
        {
            _list.Clear();
        }
        public int Count
        {
            get { return _list.Count; }
        }
        public void Add(Image img)
        {
            _list.Add(GetBytes(img));

            // stored image data, now dispose of original
            img.Dispose();
        }
        public Image this[int index]
        {
            get { return GetImage(_list[index]); }
            set { _list[index] = GetBytes(value); }
        }

        // implementation
        byte[] GetBytes(Image img)
        {
            // use interop to get the metafile bits
            Metafile mf = img as Metafile;
            var enhMetafileHandle = mf.GetHenhmetafile().ToInt32();
            var bufferSize = GetEnhMetaFileBits(enhMetafileHandle, 0, null);
            var buffer = new byte[bufferSize];
            GetEnhMetaFileBits(enhMetafileHandle, bufferSize, buffer);

            // return bits
            return buffer;
        }
        Image GetImage(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            return Image.FromStream(ms);
        }

        [System.Runtime.InteropServices.DllImport("gdi32")]
        static extern int GetEnhMetaFileBits(int hemf, int cbBuffer, byte[] lpbBuffer);
    }

#else

    /// <summary>
    /// This version of the PageImageList is a simple List Image. 
    /// It is simple, but caches one image (GDI object) per preview page.
    /// </summary>
    public class PageImageList : List<System.Drawing.Image>
    {
    }

#endif

    /// <summary>
    /// Preview document.
    /// </summary>
    public class Preview
    {
        /// <summary>
        /// Preview document.
        /// </summary>
        /// <param name="dialog">The print preview dialog.</param>
        public Preview(PreviewDialog dialog)
        {
            _previewDialog = dialog;
        }

        /// <summary>
        /// Preview document.
        /// </summary>
        /// <param name="dialog">The print preview dialog.</param>
        public Preview(PrintPreviewDialog dialog)
        {
            _printPreviewDialog = dialog;
        }

        private PrintDocument _printDocument = null;
        private PreviewDialog _previewDialog = null;
        private PrintPreviewDialog _printPreviewDialog = null;

        /// <summary>
        /// Gets or sets the print document.
        /// </summary>
        public PrintDocument Document
        {
            get { return _printDocument; }
            set { _printDocument = value; }
        }

        /// <summary>
        /// Shows the form as a modal dialog box.
        /// </summary>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        public DialogResult ShowDialog()
        {
            DialogResult result = DialogResult.None;

            if (_previewDialog != null)
            {
                _previewDialog.Document = _printDocument;
                result = _previewDialog.ShowDialog();
            }

            if (_printPreviewDialog != null)
            {
                _printPreviewDialog.Document = _printDocument;
                result = _printPreviewDialog.ShowDialog();
            }

            return result;
        }

        /// <summary>
        /// Shows the form as a modal dialog box.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents
        /// the top-level window that will own the modal dialog box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult result = DialogResult.None;

            if (_previewDialog != null)
            {
                _previewDialog.Document = _printDocument;
                result = _previewDialog.ShowDialog(owner);
            }

            if (_printPreviewDialog != null)
            {
                _printPreviewDialog.Document = _printDocument;
                result = _printPreviewDialog.ShowDialog(owner);
            }

            return result;
        }
    }
}
