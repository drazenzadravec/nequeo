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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nequeo.Media
{
    /// <summary>
    /// Provides a stream writer that can be used to write images as MJPEG 
    /// or (Motion JPEG) to any stream.
    /// </summary>
    public class MpegStreamer
    {
        /// <summary>
        /// Provides a stream writer that can be used to write images as MJPEG 
        /// or (Motion JPEG) to any stream.
        /// </summary>
        /// <param name="stream">The stream to write the output data to.</param>
        public MpegStreamer(Stream stream)
        {
            // Create a new response stream.
            _response = Nequeo.Net.WebResponse.Create(stream);
        }

        private Nequeo.Net.WebResponse _response = null;

        /// <summary>
        /// Write the images to the stream. This also writes the complete HTTP header information including boundaries.
        /// </summary>
        /// <param name="images">The images to write.</param>
        /// <param name="boundary">The HTTP header boundary name.</param>
        /// <param name="writeHeaders">Should the headers be written to the stream as well.</param>
        public void Write(Image[] images, string boundary = "987654321", bool writeHeaders = true)
        {
            string boundaryPrefix = "--";
            string boundarySuffix = "--";
            string deli = "\r\n";

            // Write headers.
            if (writeHeaders)
            {
                // Write the top level headers.
                _response.ContentType = "multipart/mixed; boundary=" + boundary;
                _response.WriteWebResponseHeaders();
            }

            // For each image.
            foreach (Image image in images)
            {
                // Save the image as a jpeg.
                MemoryStream imageStream = null;
                image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Create a memory stream for the current image.
                using (imageStream = new MemoryStream())
                {
                    // Write each header.
                    _response.Write(boundaryPrefix + boundary + deli);
                    _response.Write("Content-Type: image/jpeg" + deli);
                    _response.Write("Content-Length: " + imageStream.Length.ToString() + deli);
                    _response.Write(deli);

                    // Write the image to the stream.
                    _response.Write(imageStream.ToArray(), 0, (int)imageStream.Length);
                    _response.Write(deli + deli);
                    _response.Flush();
                }
            }

            // Write the end of the data.
            _response.Write(boundaryPrefix + boundary + boundarySuffix);
            _response.Flush();
        }

        /// <summary>
        /// Write the images to the stream constantly. This also writes the complete HTTP header information including boundaries.
        /// </summary>
        /// <param name="images">The images to write.</param>
        /// <param name="boundary">The HTTP header boundary name.</param>
        /// <param name="writeHeaders">Should the headers be written to the stream as well.</param>
        public void Write(Func<IEnumerable<Image>> images, string boundary = "987654321", bool writeHeaders = true)
        {
            string boundaryPrefix = "--";
            string boundarySuffix = "--";
            string deli = "\r\n";

            // Write headers.
            if (writeHeaders)
            {
                // Write the top level headers.
                _response.ContentType = "multipart/mixed; boundary=" + boundary;
                _response.WriteWebResponseHeaders();
            }

            // For each image.
            foreach (Image image in images())
            {
                // Save the image as a jpeg.
                MemoryStream imageStream = null;
                image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Create a memory stream for the current image.
                using (imageStream = new MemoryStream())
                {
                    // Write each header.
                    _response.Write(boundaryPrefix + boundary + deli);
                    _response.Write("Content-Type: image/jpeg" + deli);
                    _response.Write("Content-Length: " + imageStream.Length.ToString() + deli);
                    _response.Write(deli);

                    // Write the image to the stream.
                    _response.Write(imageStream.ToArray(), 0, (int)imageStream.Length);
                    _response.Write(deli + deli);
                    _response.Flush();
                }
            }

            // Write the end of the data.
            _response.Write(boundaryPrefix + boundary + boundarySuffix);
            _response.Flush();
        }
    }
}
