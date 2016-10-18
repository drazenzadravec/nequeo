/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Nequeo.Drawing.Pdf
{
    /// <summary>
    /// PDF document creator.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// PDF document creator.
        /// </summary>
        public Document() { }

        /// <summary>
        /// Read the stream containing the PDF data.
        /// </summary>
        /// <param name="pdf">The PDf stream.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <returns>The PDF document.</returns>
        public iTextSharp.text.pdf.PdfReader ReadPdf(Stream pdf, string password = "")
        {
            byte[] pass = null;
            iTextSharp.text.pdf.PdfReader pdfReader = null;

            // If no password.
            if (String.IsNullOrEmpty(password))
            {
                pdfReader = new iTextSharp.text.pdf.PdfReader(pdf);
            }
            else
            {
                pass = Encoding.Default.GetBytes(password);
                pdfReader = new iTextSharp.text.pdf.PdfReader(pdf, pass);
            }

            // Return the reader.
            return pdfReader;
        }

        /// <summary>
        /// Extract all the text from each page within the PDF document.
        /// </summary>
        /// <param name="pdf">The PDf stream.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <param name="encoding">The encoding the extracted text should be converted to; the default is UTF8</param>
        /// <returns>The complete text extracted.</returns>
        public StringBuilder ExtractText(Stream pdf, string password = "", Nequeo.Text.EncodingType encoding = Nequeo.Text.EncodingType.UTF8)
        {
            return ExtractText(pdf, 1, Int32.MaxValue, password, encoding);
        }

        /// <summary>
        /// Extract all the text from the specified page interval within the PDF document.
        /// </summary>
        /// <param name="pdf">The PDf stream.</param>
        /// <param name="fromPage">From page (must be greater than zero).</param>
        /// <param name="toPage">To page (must be greater than zero).</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <param name="encoding">The encoding the extracted text should be converted to; the default is UTF8</param>
        /// <returns>The complete text extracted.</returns>
        public StringBuilder ExtractText(Stream pdf, int fromPage, int toPage = Int32.MaxValue, string password = "", Nequeo.Text.EncodingType encoding = Nequeo.Text.EncodingType.UTF8)
        {
            byte[] pass = null;
            StringBuilder text = new StringBuilder();
            iTextSharp.text.pdf.PdfReader pdfReader = null;

            try
            {
                // If no password.
                if (String.IsNullOrEmpty(password))
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf);
                }
                else
                {
                    pass = Encoding.Default.GetBytes(password);
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf, pass);
                }

                // Get the page list.
                int toPageInt = (toPage > pdfReader.NumberOfPages ? pdfReader.NumberOfPages : (toPage < 1 ? 1 : toPage));
                int fromPageInt = (fromPage < 1 ? 1 : (toPage < fromPage ? toPage : fromPage));

                // For each page.
                for (int page = fromPageInt; page <= toPageInt; page++)
                {
                    // Create the PDF text extractor.
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    // Convert the text into UTF8.
                    currentText = Nequeo.Text.Encoding.Convert(currentText + "\r\n", encoding);
                    text.Append(currentText);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();
            }

            // Return the text.
            return text;
        }

        /// <summary>
        /// Extract all the text from the specified pages within the PDF document.
        /// </summary>
        /// <param name="pdf">The PDf stream.</param>
        /// <param name="pages">The list of page numbers to extract text from.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <param name="encoding">The encoding the extracted text should be converted to; the default is UTF8</param>
        /// <returns>The complete text extracted.</returns>
        public StringBuilder ExtractText(Stream pdf, int[] pages, string password = "", Nequeo.Text.EncodingType encoding = Nequeo.Text.EncodingType.UTF8)
        {
            byte[] pass = null;
            StringBuilder text = new StringBuilder();
            iTextSharp.text.pdf.PdfReader pdfReader = null;

            try
            {
                // If no password.
                if (String.IsNullOrEmpty(password))
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf);
                }
                else
                {
                    pass = Encoding.Default.GetBytes(password);
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf, pass);
                }

                // For each page.
                foreach (int page in pages)
                {
                    // If within page interval the extract text.
                    if (page >= 1 || page <= pdfReader.NumberOfPages)
                    {
                        // Create the PDF text extractor.
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                        // Convert the text into UTF8.
                        currentText = Nequeo.Text.Encoding.Convert(currentText + "\r\n", encoding);
                        text.Append(currentText);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();
            }

            // Return the text.
            return text;
        }

        /// <summary>
        /// Create a new PDF text document.
        /// </summary>
        /// <param name="pdf">The PDF file stream.</param>
        /// <param name="text">The text to add to the document.</param>
        /// <param name="font">The text font to create.</param>
        /// <exception cref="System.Exception"></exception>
        public void CreateText(Stream pdf, string text, Nequeo.Drawing.Pdf.Font font)
        {
            iTextSharp.text.Document document = null;

            try
            {
                // Create the document.
                document = new iTextSharp.text.Document();
                iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfAWriter.GetInstance(document, pdf);
                document.Open();

                // Add the text.
                iTextSharp.text.Font fontText = font.GetFont();
                document.Add(new iTextSharp.text.Paragraph(text, fontText));

                // Close the document.
                document.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (document != null)
                    document.Dispose();
            }
        }

        /// <summary>
        /// Create a new PDF text document.
        /// </summary>
        /// <param name="pdf">The PDF file stream.</param>
        /// <param name="text">The text to add to the document.</param>
        /// <param name="font">The text font to create.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <exception cref="System.Exception"></exception>
        public void CreateText(Stream pdf, string text, Nequeo.Drawing.Pdf.Font font, string password)
        {
            iTextSharp.text.Document document = null;

            try
            {
                // Create the document.
                document = new iTextSharp.text.Document();
                iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfAWriter.GetInstance(document, pdf);
                pdfWriter.SetEncryption(iTextSharp.text.pdf.PdfWriter.ENCRYPTION_AES_256, password, password, 0);
                document.Open();

                // Add the text.
                iTextSharp.text.Font fontText = font.GetFont();
                document.Add(new iTextSharp.text.Paragraph(text, fontText));

                // Close the document.
                document.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (document != null)
                    document.Dispose();
            }
        }

        /// <summary>
        /// Checks whether a specified page of a PDF file contains images.
        /// </summary> 
        /// <param name="pdf">The PDF stream.</param>
        /// <param name="pageNumber">The page number to look for images.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <returns>True if the page contains at least one image; false otherwise.</returns> 
        public bool PageContainsImages(Stream pdf, int pageNumber, string password = "")
        {
            bool result = false;
            ImageRenderListener listener = null;

            byte[] pass = null;
            iTextSharp.text.pdf.PdfReader pdfReader = null;

            try
            {
                // If no password.
                if (String.IsNullOrEmpty(password))
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf);
                }
                else
                {
                    pass = Encoding.Default.GetBytes(password);
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf, pass);
                }

                // Parse the stream.
                var parser = new PdfReaderContentParser(pdfReader);
                parser.ProcessContent(pageNumber, (listener = new ImageRenderListener()));
                result = (listener.Images.Count > 0 ? true : false);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();
            }

            return result;
        }

        /// <summary>
        /// Extracts all images from a specified page of a PDF file.
        /// </summary> 
        /// <param name="pdf">The PDF stream.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <returns>Returns an array of images
        /// where the key is a suggested file name, in the format: PDF filename without extension,  
        /// page number and image index in the page.</returns> 
        public Dictionary<string, System.Drawing.Image> ExtractImages(Stream pdf, string password = "")
        {
            byte[] pass = null;
            iTextSharp.text.pdf.PdfReader pdfReader = null;
            var images = new Dictionary<string, System.Drawing.Image>();

            try
            {
                // If no password.
                if (String.IsNullOrEmpty(password))
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf);
                }
                else
                {
                    pass = Encoding.Default.GetBytes(password);
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf, pass);
                }

                // Create the pdf parser.
                var parser = new PdfReaderContentParser(pdfReader);
                ImageRenderListener listener = null;

                for (var i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    // Parse the pdf stream.
                    parser.ProcessContent(i, (listener = new ImageRenderListener()));
                    var index = 1;

                    // If images exist.
                    if (listener.Images.Count > 0)
                    {
                        // For each image extracted.
                        foreach (var pair in listener.Images)
                        {
                            // Add the image.
                            images.Add(string.Format("Page_{ 1} Image_{ 2} { 3}", i.ToString("D4"), index.ToString("D4"), pair.Value), pair.Key);
                            index++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();
            }

            // Return the images.
            return images;
        }

        /// <summary>
        /// Extracts all images from a specified page of a PDF file.
        /// </summary> 
        /// <param name="pdf">The PDF stream.</param>
        /// <param name="pageNumber">The page number to look for images.</param>
        /// <param name="password">The password used to protect the document.</param>
        /// <returns>Returns an array of images
        /// where the key is a suggested file name, in the format: PDF filename without extension,  
        /// page number and image index in the page.</returns> 
        public Dictionary<string, System.Drawing.Image> ExtractImages(Stream pdf, int pageNumber, string password = "")
        {
            byte[] pass = null;
            iTextSharp.text.pdf.PdfReader pdfReader = null;
            Dictionary<string, System.Drawing.Image> images = new Dictionary<string, System.Drawing.Image>();
            
            try
            {
                // If no password.
                if (String.IsNullOrEmpty(password))
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf);
                }
                else
                {
                    pass = Encoding.Default.GetBytes(password);
                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdf, pass);
                }

                // Create the content reader.
                PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);
                ImageRenderListener listener = null;

                // Parse the content.
                parser.ProcessContent(pageNumber, (listener = new ImageRenderListener()));
                int index = 1;

                // If images have been found.
                if (listener.Images.Count > 0)
                {
                    // Add each image to the list.
                    foreach (KeyValuePair<System.Drawing.Image, string> pair in listener.Images)
                    {
                        // Add the image.
                        images.Add(string.Format("Page_{ 1} Image_{ 2} { 3}", pageNumber.ToString("D4"), index.ToString("D4"), pair.Value), pair.Key);
                        index++;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();
            }

            // Return the images.
            return images;
        }

        /// <summary>
        /// Create pdf table.
        /// </summary>
        /// <returns>The PDF table.</returns>
        private iTextSharp.text.pdf.PdfPTable CreateTable()
        {
            // A table with three colums
            iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(3);

            // The cell
            iTextSharp.text.pdf.PdfPCell cell = null;

            // Add a cell with colspan 3
            cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cell with colspan 3"));
            cell.Colspan = 3;
            table.AddCell(cell);

            // Add a cell with colspan 2
            cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cell with colspan 2"));
            cell.Colspan = 2;
            table.AddCell(cell);

            // Add the four remaining cells.
            table.AddCell("row 1; cell 1");
            table.AddCell("row 1; cell 2");
            table.AddCell("row 2; cell 1");
            table.AddCell("row 2; cell 2");

            // Return the table.
            return table;
        }
    }

    /// <summary>
    /// Image Render Listener Helper.
    /// </summary>
    internal class ImageRenderListener : IRenderListener
    {
        #region Fields 

        Dictionary<System.Drawing.Image, string> images = new Dictionary<System.Drawing.Image, string>();
        #endregion Fields 

        #region Properties 

        public Dictionary<System.Drawing.Image, string> Images
        {
            get { return images; }
        }
        #endregion Properties 

        #region Methods 

        #region Public Methods 

        public void BeginTextBlock() { }

        public void EndTextBlock() { }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            PdfImageObject image = renderInfo.GetImage();
            PdfName filter = (PdfName)image.Get(PdfName.FILTER);

            //int width = Convert.ToInt32(image.Get(PdfName.WIDTH).ToString()); 
            //int bitsPerComponent = Convert.ToInt32(image.Get(PdfName.BITSPERCOMPONENT).ToString()); 
            //string subtype = image.Get(PdfName.SUBTYPE).ToString(); 
            //int height = Convert.ToInt32(image.Get(PdfName.HEIGHT).ToString()); 
            //int length = Convert.ToInt32(image.Get(PdfName.LENGTH).ToString()); 
            //string colorSpace = image.Get(PdfName.COLORSPACE).ToString(); 

            /* It appears to be safe to assume that when filter == null, PdfImageObject  
             * does not know how to decode the image to a System.Drawing.Image. 
             *  
             * Uncomment the code above to verify, but when I’ve seen this happen,  
             * width, height and bits per component all equal zero as well. */
            if (filter != null)
            {
                System.Drawing.Image drawingImage = image.GetDrawingImage();

                string extension = ".";

                if (filter == PdfName.DCTDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.JPG.FileExtension;
                }
                else if (filter == PdfName.JPXDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.JP2.FileExtension;
                }
                else if (filter == PdfName.FLATEDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.PNG.FileExtension;
                }
                else if (filter == PdfName.LZWDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.CCITT.FileExtension;
                }

                /* Rather than struggle with the image stream and try to figure out how to handle  
                 * BitMapData scan lines in various formats (like virtually every sample I’ve found  
                 * online), use the PdfImageObject.GetDrawingImage() method, which does the work for us. */
                this.Images.Add(drawingImage, extension);
            }
        }
        public void RenderText(TextRenderInfo renderInfo) { }

        #endregion Public Methods 

        #endregion Methods 
    }
}
