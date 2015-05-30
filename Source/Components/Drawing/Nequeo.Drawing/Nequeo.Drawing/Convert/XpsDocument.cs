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
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Media.Imaging;   

namespace Nequeo.Drawing.Convert
{
    /// <summary>
    /// Convert XPS documents provider
    /// </summary>
    public class XpsDocument
    {
        /// <summary>
        /// Convert an XPS document to a Bitmap image
        /// </summary>
        /// <param name="xpsFileName">The path of the XPS document</param>
        /// <param name="bitmapFileName">The path of the bitmap image to create</param>
        static public void ToBitmap(string xpsFileName, string bitmapFileName)
        {
            // Create a new instance of the Xps documr
            System.Windows.Xps.Packaging.XpsDocument xpsDoc = 
                new System.Windows.Xps.Packaging.XpsDocument(xpsFileName, System.IO.FileAccess.Read);
            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();

            // Interate throught the total number of pages.
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {
                // Get the current document page.
                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);

                // Create a new bitmap image.
                BitmapImage bitmap = new BitmapImage();

                // Create a new bitmap render engine.
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based    
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                // Render the current page to the image
                renderTarget.Render(docPage.Visual);

                // Encode the image as a bitmap
                BitmapEncoder encoder = new BmpBitmapEncoder(); 
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                // Save the image to the file
                using (FileStream pageOutStream = new FileStream(bitmapFileName, FileMode.Create, FileAccess.Write))
                {
                    // Save the image to the image file.
                    encoder.Save(pageOutStream);
                    pageOutStream.Close();
                }
            }
        }

        /// <summary>
        /// Convert an XPS document to a TIFF image
        /// </summary>
        /// <param name="xpsFileName">The path of the XPS document</param>
        /// <param name="tiffFileName">The path of the tiff image to create</param>
        static public void ToTiff(string xpsFileName, string tiffFileName)
        {
            // Create a new instance of the Xps documr
            System.Windows.Xps.Packaging.XpsDocument xpsDoc =
                new System.Windows.Xps.Packaging.XpsDocument(xpsFileName, System.IO.FileAccess.Read);
            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();

            // Interate throught the total number of pages.
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {
                // Get the current document page.
                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);

                // Create a new bitmap image.
                BitmapImage bitmap = new BitmapImage();

                // Create a new bitmap render engine.
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based    
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                // Render the current page to the image
                renderTarget.Render(docPage.Visual);

                // Encode the image as a bitmap
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                // Save the image to the file
                using (FileStream pageOutStream = new FileStream(tiffFileName, FileMode.Create, FileAccess.Write))
                {
                    // Save the image to the image file.
                    encoder.Save(pageOutStream);
                    pageOutStream.Close();
                }
            }
        }

        /// <summary>
        /// Convert an XPS document to a PNG image
        /// </summary>
        /// <param name="xpsFileName">The path of the XPS document</param>
        /// <param name="pngFileName">The path of the png image to create</param>
        static public void ToPng(string xpsFileName, string pngFileName)
        {
            // Create a new instance of the Xps documr
            System.Windows.Xps.Packaging.XpsDocument xpsDoc =
                new System.Windows.Xps.Packaging.XpsDocument(xpsFileName, System.IO.FileAccess.Read);
            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();

            // Interate throught the total number of pages.
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {
                // Get the current document page.
                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);

                // Create a new bitmap image.
                BitmapImage bitmap = new BitmapImage();

                // Create a new bitmap render engine.
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based    
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                // Render the current page to the image
                renderTarget.Render(docPage.Visual);

                // Encode the image as a bitmap
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                // Save the image to the file
                using (FileStream pageOutStream = new FileStream(pngFileName, FileMode.Create, FileAccess.Write))
                {
                    // Save the image to the image file.
                    encoder.Save(pageOutStream);
                    pageOutStream.Close();
                }
            }
        }

        /// <summary>
        /// Convert an XPS document to a JPEG image
        /// </summary>
        /// <param name="xpsFileName">The path of the XPS document</param>
        /// <param name="jpegFileName">The path of the jpeg image to create</param>
        static public void ToJpeg(string xpsFileName, string jpegFileName)
        {
            // Create a new instance of the Xps documr
            System.Windows.Xps.Packaging.XpsDocument xpsDoc =
                new System.Windows.Xps.Packaging.XpsDocument(xpsFileName, System.IO.FileAccess.Read);
            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();

            // Interate throught the total number of pages.
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {
                // Get the current document page.
                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);

                // Create a new bitmap image.
                BitmapImage bitmap = new BitmapImage();

                // Create a new bitmap render engine.
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based    
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                // Render the current page to the image
                renderTarget.Render(docPage.Visual);

                // Encode the image as a bitmap
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                // Save the image to the file
                using (FileStream pageOutStream = new FileStream(jpegFileName, FileMode.Create, FileAccess.Write))
                {
                    // Save the image to the image file.
                    encoder.Save(pageOutStream);
                    pageOutStream.Close();
                }
            }
        }

        /// <summary>
        /// Convert an XPS document to a GIF image
        /// </summary>
        /// <param name="xpsFileName">The path of the XPS document</param>
        /// <param name="gifFileName">The path of the gif image to create</param>
        static public void ToGif(string xpsFileName, string gifFileName)
        {
            // Create a new instance of the Xps documr
            System.Windows.Xps.Packaging.XpsDocument xpsDoc =
                new System.Windows.Xps.Packaging.XpsDocument(xpsFileName, System.IO.FileAccess.Read);
            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();

            // Interate throught the total number of pages.
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {
                // Get the current document page.
                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);

                // Create a new bitmap image.
                BitmapImage bitmap = new BitmapImage();

                // Create a new bitmap render engine.
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based    
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                // Render the current page to the image
                renderTarget.Render(docPage.Visual);

                // Encode the image as a bitmap
                GifBitmapEncoder encoder = new GifBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                // Save the image to the file
                using (FileStream pageOutStream = new FileStream(gifFileName, FileMode.Create, FileAccess.Write))
                {
                    // Save the image to the image file.
                    encoder.Save(pageOutStream);
                    pageOutStream.Close();
                }
            }
        }
    }
}
