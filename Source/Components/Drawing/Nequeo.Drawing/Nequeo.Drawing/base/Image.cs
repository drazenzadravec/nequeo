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
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Nequeo.Drawing
{
    /// <summary>
    /// Image drawing provider.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Image drawing provider.
        /// </summary>
        public Image() { }

        private Random _random = new Random();

        /// <summary>
        /// Capture the primary screen image.
        /// </summary>
        /// <param pixelFormat="image">Specifies the format of the color data for each pixel in the image.</param>
        /// <returns>The image containing the screen.</returns>
        public System.Drawing.Bitmap CaptureScreen(PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            // Create the image.
            Rectangle screenSize = Screen.PrimaryScreen.Bounds;
            Bitmap target = new Bitmap(screenSize.Width, screenSize.Height, pixelFormat);

            // Create the graphics on the bitmap image.
            using (Graphics graphics = Graphics.FromImage(target))
            {
                // Copy the screen to the target.
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(screenSize.Width, screenSize.Height));
            }

            // Return the image.
            return target;
        }

        /// <summary>
        /// Capture all screens image.
        /// </summary>
        /// <param pixelFormat="image">Specifies the format of the color data for each pixel in the image.</param>
        /// <returns>The image containing the screen.</returns>
        public System.Drawing.Bitmap CaptureAllScreens(PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            Rectangle screenSize = Rectangle.Empty;

            // For each creen.
            foreach (Screen s in Screen.AllScreens)
                screenSize = Rectangle.Union(screenSize, s.Bounds);

            // Get the image.
            Bitmap target = new Bitmap(screenSize.Width, screenSize.Height, pixelFormat);

            // Create the graphics on the bitmap image.
            using (Graphics graphics = Graphics.FromImage(target))
            {
                // Copy the screen to the target.
                graphics.CopyFromScreen(screenSize.X, screenSize.Y, 0, 0, screenSize.Size, CopyPixelOperation.SourceCopy);
            }

            // Return the image.
            return target;
        }

        /// <summary>
        /// Capture the primary screen image.
        /// </summary>
        /// <param name="width">The scaled width.</param>
        /// <param name="height">The scaled height.</param>
        /// <param name="showCursor">Draw the cursor on the image.</param>
        /// <param name="pixelFormat">Specifies the format of the color data for each pixel in the image.</param>
        /// <returns>The image containing the screen.</returns>
        public System.Drawing.Bitmap CaptureScreenScaled(int width = -1, int height = -1, bool showCursor = true, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            bool scaled = false;

            // Create the image.
            Rectangle screenSize = Screen.PrimaryScreen.Bounds;
            Bitmap target = new Bitmap(screenSize.Width, screenSize.Height, pixelFormat);

            // If valid image size.
            if (width >= 0 && height >= 0)
                scaled = (width != screenSize.Width || height != screenSize.Height);

            // Create the graphics on the bitmap image.
            using (Graphics graphics = Graphics.FromImage(target))
            {
                // Copy the screen to the target.
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(screenSize.Width, screenSize.Height));

                // Show the cursor be drawn.
                if (showCursor)
                    Cursors.Default.Draw(graphics, new Rectangle(Cursor.Position, new Size(32, 32)));

                // If scaled.
                if (scaled)
                {
                    // Get the scaled image.
                    Bitmap imageScaled = new Bitmap(width, height);
                    Rectangle sizeScaled = new Rectangle(0, 0, width, height);

                    // Create the graphics on the bitmap image.
                    using (Graphics graphicsScaled = Graphics.FromImage(imageScaled))
                    {
                        // Draw the scaled image.
                        graphicsScaled.DrawImage(target, sizeScaled, screenSize, GraphicsUnit.Pixel);
                    }

                    // Assign the new image.
                    target = imageScaled;
                }
            }

            // Return the image.
            return target;
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified file.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file from which to create the System.Drawing.Image.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Image Load(string filename)
        {
            return System.Drawing.Image.FromFile(filename);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified file.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file from which to create the System.Drawing.Image.</param>
        /// <param name="useEmbeddedColorManagement">Set to true to use color management information embedded in the image file; otherwise, false.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Image Load(string filename, bool useEmbeddedColorManagement)
        {
            return System.Drawing.Image.FromFile(filename, useEmbeddedColorManagement);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified data stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains the data for this System.Drawing.Image.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Image Load(System.IO.Stream stream)
        {
            return System.Drawing.Image.FromStream(stream);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified data stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains the data for this System.Drawing.Image.</param>
        /// <param name="useEmbeddedColorManagement">True to use color management information embedded in the data stream; otherwise, false.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Image Load(System.IO.Stream stream, bool useEmbeddedColorManagement)
        {
            return System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified data stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains the data for this System.Drawing.Image.</param>
        /// <param name="useEmbeddedColorManagement">True to use color management information embedded in the data stream; otherwise, false.</param>
        /// <param name="validateImageData">True to validate the image data; otherwise, false.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Image Load(System.IO.Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
        {
            return System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement);
        }
        
        /// <summary>
        /// Creates an System.Drawing.Image from the specified file.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file from which to create the System.Drawing.Image.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Bitmap Create(string filename)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(filename);
            return new Bitmap(image);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified file.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file from which to create the System.Drawing.Image.</param>
        /// <param name="useEmbeddedColorManagement">Set to true to use color management information embedded in the image file; otherwise, false.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Bitmap Create(string filename, bool useEmbeddedColorManagement)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(filename, useEmbeddedColorManagement);
            return new Bitmap(image);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified data stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains the data for this System.Drawing.Image.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Bitmap Create(System.IO.Stream stream)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            return new Bitmap(image);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified data stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains the data for this System.Drawing.Image.</param>
        /// <param name="useEmbeddedColorManagement">True to use color management information embedded in the data stream; otherwise, false.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Bitmap Create(System.IO.Stream stream, bool useEmbeddedColorManagement)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement);
            return new Bitmap(image);
        }

        /// <summary>
        /// Creates an System.Drawing.Image from the specified data stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains the data for this System.Drawing.Image.</param>
        /// <param name="useEmbeddedColorManagement">True to use color management information embedded in the data stream; otherwise, false.</param>
        /// <param name="validateImageData">True to validate the image data; otherwise, false.</param>
        /// <returns>The System.Drawing.Image this method creates.</returns>
        public System.Drawing.Bitmap Create(System.IO.Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement);
            return new Bitmap(image);
        }
        
        /// <summary>
        /// Save the bitmap image.
        /// </summary>
        /// <param name="image">The bitmap image to save.</param>
        /// <param name="stream">The stream to write the image to.</param>
        /// <param name="imageFormat">The image format to create.</param>
        public void Save(Bitmap image, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            // Save the image.
            image.Save(stream, imageFormat);
        }

        /// <summary>
        /// Create a new image.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="backgroundImage">The back ground image to add to this image.</param>
        /// <returns>The new bitmap image.</returns>
        public Bitmap Create(int width, int height, System.IO.Stream backgroundImage = null)
        {
            Bitmap bitmap = null;

            try
            {
                // If the back ground image has not been
                // set the create a new default image
                if (backgroundImage == null)
                    bitmap = new Bitmap(width, height);
                else
                    // If the back ground image has been set
                    // then use this image as the back ground.
                    bitmap = (Bitmap)System.Convert.ChangeType(Bitmap.FromStream(backgroundImage), typeof(Bitmap));
            }
            catch (Exception) { throw; }

            // Return the new image created.
            return bitmap;
        }

        /// <summary>
        /// Create the graphics.
        /// </summary>
        /// <param name="image">The image to draw graphics on.</param>
        /// <param name="rectangle">Stores a set of four integers that represent the location and size of a rectangle.</param>
        /// <param name="rectangleF">Stores a set of four floating-point numbers that represent the location and
        /// size of a rectangle. For more advanced region functions, use a System.Drawing.Region object.</param>
        /// <returns>Encapsulates a GDI+ drawing surface.</returns>
        public Graphics CreateGraphics(ref Bitmap image, out Rectangle rectangle, out RectangleF rectangleF)
        {
            // Create the graphic.
            Graphics graphics = Graphics.FromImage(image);

            // Create the rectangle objects
            rectangle = new Rectangle(0, 0, image.Width, image.Height);
            rectangleF = new RectangleF(0, 0, image.Width, image.Height);

            // Return the graphic.
            return graphics;
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
        /// <param name="graphics">The image graphic to draw the text on.</param>
        /// <param name="text">The text to create.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text</param>
        /// <param name="stringFormat">Text layout information (such as alignment, orientation and
        /// tab stops) display manipulations (such as ellipsis insertion and national
        /// digit substitution) and OpenType features</param>
        /// <param name="fontFamily">The image graphic font family of the text.</param>
        /// <returns>The new graphic path containing the text.</returns>
        public GraphicsPath CreateText(ref Graphics graphics, string text, float x, float y, 
            float width, float height, StringFormat stringFormat, FontFamily fontFamily)
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

                // Add the text to the graphic path image.
                textPath.AddString(text, font.FontFamily, System.Convert.ToInt32(font.Style), font.Size, new RectangleF(x, y, width, height), stringFormat);

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
        private PointF RandomPoint(int xMin, int xMax, int yMin, int yMax)
        {
            return new PointF(_random.Next(xMin, xMax), _random.Next(yMin, yMax));
        }
    }
}
