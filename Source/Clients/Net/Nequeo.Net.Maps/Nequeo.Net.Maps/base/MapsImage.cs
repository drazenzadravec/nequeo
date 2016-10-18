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
using System.Drawing;

namespace Nequeo.Net.Maps
{
    /// <summary>
    /// Image map provider.
    /// </summary>
    public sealed class MapsImage
    {
        /// <summary>
        /// Get the bitmap image from the map data.
        /// </summary>
        /// <param name="mapData">The byte array containing the image.</param>
        /// <returns>The new bitmap image.</returns>
        public static Bitmap GetBitmap(byte[] mapData)
        {
            Bitmap bitmap = null;
            Nequeo.Drawing.Image image = new Nequeo.Drawing.Image();

            // Load the data into the image.
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(mapData))
            {
                // Get the image.
                bitmap = image.Create(stream);
            }

            // Return the image.
            return bitmap;
        }

        /// <summary>
        /// Save the bitmap image.
        /// </summary>
        /// <param name="bitmap">The bitmap image to save.</param>
        /// <param name="stream">The stream to write the image to.</param>
        /// <param name="imageFormat">The image format to create.</param>
        public static void SaveBitmap(Bitmap bitmap, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            Nequeo.Drawing.Image image = new Nequeo.Drawing.Image();
            image.Save(bitmap, stream, imageFormat);
        }
    }
}
