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
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Nequeo.Drawing
{
    /// <summary>
    /// Image Captcha provider.
    /// </summary>
    public class Captcha
    {
        /// <summary>
        /// Image Captcha provider.
        /// </summary>
        public Captcha() { }

        private Random _random = new Random();

        /// <summary>
        /// The collection of font families.
        /// </summary>
        public static readonly string[] FontFamilies = { 
                "Arial", "Comic Sans MS", "Courier New", "Georgia", "Lucida Console", "MS Sans Serif", 
                "Stencil", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana", "Mangneto", "Bauhaus 93",
                "Bernard MT Condensed", "DigifaceWide", "Impact", "Lucida Calligraphy", "Matura MT Script Capitals",
                "Pump Demi Bold LET", "Rockwell Condensed,", "Stencil", "Wide Latin", "Vrinda"};

        /// <summary>
        /// Generate the image from the form authentication encrypted method.
        /// </summary>
        /// <param name="text">The text to create.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="fontFamily">The image graphic font family of the text.</param>
        /// <param name="backgroundImage">File name and path of a background image (if set then the background image is used.</param>
        /// <returns>The bitmap image containg the captcha.</returns>
        public Bitmap GenerateImage(string text, int width, int height, FontFamily fontFamily, System.IO.Stream backgroundImage = null)
        {
            // Decrypt the demension data from the encyted text.
            Bitmap bitmap = null;

            try
            {
                // Create a new graphic.
                Graphics graphics = null;
                Brush brush = new SolidBrush(Color.LightGray);
                Brush brush1 = new SolidBrush(Color.Black);

                // If the back ground image has not been
                // set the create a new default image
                if (backgroundImage == null)
                    bitmap = CreateImage(width, height);
                else
                    // If the back ground image has been set
                    // then use this image as the back ground.
                    bitmap = (Bitmap)System.Convert.ChangeType(Bitmap.FromStream(backgroundImage), typeof(Bitmap));

                // Create the graphic from the image.
                graphics = Graphics.FromImage(bitmap);
                GraphicsPath textPath = CreateText(text, width, height, graphics, fontFamily);

                // If the back ground image has not been set
                // then use the first brush else use the second.
                if (backgroundImage == null)
                    graphics.FillPath(brush, textPath);
                else
                    graphics.FillPath(brush1, textPath);
            }
            catch (Exception) { throw; }

            // Return the new image created.
            return bitmap;
        }

        /// <summary>
        /// Create the bitmap size image, with a random backgroud gradient.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The image created.</returns>
        public Bitmap CreateImage(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics;

            // Create the rectangle objects
            Rectangle rectangle = new Rectangle(0, 0, width, height);
            RectangleF rectangleF = new RectangleF(0, 0, width, height);

            // Create the graphic
            graphics = Graphics.FromImage(bitmap);

            // Create the gradient color difference.
            Brush brush = new LinearGradientBrush(rectangle,
                        Color.FromArgb(_random.Next(192), _random.Next(192), _random.Next(192)),
                        Color.FromArgb(_random.Next(192), _random.Next(192), _random.Next(192)),
                        System.Convert.ToSingle(_random.NextDouble()) * 360, false);

            // Distort the image.
            if (_random.Next(2) == 1)
                DistortImage(ref bitmap, _random.Next(5, 10));
            else
                DistortImage(ref bitmap, -_random.Next(5, 10));

            // Return the bitmap image.
            graphics.FillRectangle(brush, rectangleF);
            return bitmap;
        }

        /// <summary>
        /// Distort the bitmap image.
        /// </summary>
        /// <param name="bitmap">The image to distort.</param>
        /// <param name="distortion">The distortion factor.</param>
        public void DistortImage(ref Bitmap bitmap, double distortion)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Clone the image
            Bitmap copy = (Bitmap)bitmap.Clone();

            // For each height 
            for (int y = 0; y < height; y++)
            {
                // For each width
                for (int x = 0; x < width; x++)
                {
                    // Distort the image for each pixel
                    // for the hieght and width.
                    int newX = System.Convert.ToInt32(x + (distortion * Math.Sin(Math.PI * y / 64.0)));
                    int newY = System.Convert.ToInt32(y + (distortion * Math.Cos(Math.PI * x / 64.0)));

                    if (newX < 0 || newX >= width) newX = 0;
                    if (newY < 0 || newY >= height) newY = 0;

                    // Apply the new pixel image.
                    bitmap.SetPixel(x, y, copy.GetPixel(newX, newY));
                }
            }
        }

        /// <summary>
        /// Create the text within the graphics path.
        /// </summary>
        /// <param name="text">The text to create.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text</param>
        /// <param name="graphics">The image graphic to draw the text on.</param>
        /// <param name="fontFamily">The image graphic font family of the text.</param>
        /// <returns>The new graphic path containing the text.</returns>
        public GraphicsPath CreateText(string text, int width, int height, Graphics graphics, FontFamily fontFamily)
        {
            GraphicsPath textPath = new GraphicsPath();
            int emSize = System.Convert.ToInt32(width * 2 / text.Length);
            Font font = null;

            try
            {
                // The starting and ending points of
                // the image.
                SizeF measured = new SizeF(0, 0);
                SizeF workingSize = new SizeF(width, height);

                // While the size of each letter is greater than two.
                // work out the size of the font to fit in the image.
                while (emSize > 2)
                {
                    // Create a new font type and size for each letter.
                    font = new Font(fontFamily, emSize);
                    measured = graphics.MeasureString(text, font);

                    // Make that each letter is within the image size.
                    if (!(measured.Width > workingSize.Width || measured.Height > workingSize.Height))
                        break;

                    font.Dispose();
                    emSize -= 2;
                }

                // Get the new font make sure the font fits in the image.
                emSize += 8;
                font = new Font(fontFamily, emSize);

                // Format the string in the center of the image.
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                // Add the text to the graphic path image.
                textPath.AddString(text, font.FontFamily, System.Convert.ToInt32(font.Style), font.Size, new RectangleF(0, 0, width, height), stringFormat);

                // Warp the text in the image.
                WarpText(ref textPath, new Rectangle(0, 0, width, height));
            }
            catch (Exception) { throw; }
            finally { font.Dispose(); }

            // Return the text path.
            return textPath;
        }

        /// <summary>
        /// Warp the text in the image.
        /// </summary>
        /// <param name="textPath">The graphic path containg the text.</param>
        /// <param name="rectangle">The rectangle that will contain the image.</param>
        public void WarpText(ref GraphicsPath textPath, Rectangle rectangle)
        {
            int warpDivisor = _random.Next(4, 8);
            RectangleF rectangleF = new RectangleF(0, 0, rectangle.Width, rectangle.Height);

            int hRange = System.Convert.ToInt32(rectangle.Height / warpDivisor);
            int wRange = System.Convert.ToInt32(rectangle.Width / warpDivisor);

            PointF pointF1 = RandomPoint(0, wRange, 0, hRange);
            PointF pointF2 = RandomPoint(rectangle.Width - (wRange - System.Convert.ToInt32(pointF1.X)), rectangle.Width, 0, hRange);
            PointF pointF3 = RandomPoint(0, wRange, rectangle.Height - (hRange - System.Convert.ToInt32(pointF1.Y)), rectangle.Height);
            PointF pointF4 = RandomPoint(rectangle.Width - (wRange - System.Convert.ToInt32(pointF3.X)), rectangle.Width, rectangle.Height - (hRange - System.Convert.ToInt32(pointF2.Y)), rectangle.Height);

            PointF[] points = new PointF[] { pointF1, pointF2, pointF3, pointF4 };
            Matrix matrix = new Matrix();
            matrix.Translate(0, 0);
            textPath.Warp(points, rectangleF, matrix, WarpMode.Perspective, 0);
        }

        /// <summary>
        /// Random point initiator.
        /// </summary>
        /// <param name="xMin">The minimun x co-ordinate.</param>
        /// <param name="xMax">The maximiun x co-ordinate.</param>
        /// <param name="yMin">The minimun y co-ordinate.</param>
        /// <param name="yMax">The maximun y co-ordinate.</param>
        /// <returns>Represents an ordered pair of floating-point x- and y-coordinates that defines
        /// a point in a two-dimensional plane.</returns>
        public PointF RandomPoint(int xMin, int xMax, int yMin, int yMax)
        {
            return new PointF(_random.Next(xMin, xMax), _random.Next(yMin, yMax));
        }
    }
}
