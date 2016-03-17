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
    /// Continuous screen capture.
    /// </summary>
    public class ScreenCapture : IDisposable
    {
        /// <summary>
        /// Continuous screen capture.
        /// </summary>
        public ScreenCapture()
        {
            // Set the default cursor size.
            _cursorSize = new Size(32, 32);
        }

        private volatile bool _enabledPrimaryScreen = true;
        private Size _cursorSize;

        private int _width = -1;
        private int _height = -1;
        private bool _showCursor = true;
        private PixelFormat _pixelFormat = PixelFormat.Format32bppArgb;

        /// <summary>
        /// Gets or sets an indicator specifying that capture should be enabled. False to stop capture.
        /// </summary>
        public bool Enabled
        {
            get { return _enabledPrimaryScreen; }
            set { _enabledPrimaryScreen = value; }
        }

        /// <summary>
        /// Gets or sets the cursor size.
        /// </summary>
        public Size CursorSize
        {
            get { return _cursorSize; }
            set { _cursorSize = value; }
        }

        /// <summary>
        /// Gets or sets the image scaled width, default is -1, no scaling is applied.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Gets or sets the image scaled height, default is -1, no scaling is applied.
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the cursor should be drawn on the image.
        /// </summary>
        public bool ShowCursor
        {
            get { return _showCursor; }
            set { _showCursor = value; }
        }

        /// <summary>
        /// Gets or sets the pixel format.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
            set { _pixelFormat = value; }
        }

        /// <summary>
        /// Capture primary screen and then yield.
        /// </summary>
        /// <returns>The captured screen.</returns>
        public IEnumerable<System.Drawing.Image> PrimaryScreen()
        {
            bool scaled = false;
            Graphics graphicsScaled = null;

            // Create the image.
            Rectangle screenSize = Screen.PrimaryScreen.Bounds;
            Size primaryScreen = new Size(screenSize.Width, screenSize.Height);
            Bitmap target = new Bitmap(screenSize.Width, screenSize.Height, _pixelFormat);

            // If valid image size.
            if (_width >= 0 && _height >= 0)
                scaled = (_width != screenSize.Width || _height != screenSize.Height);

            // Create the graphics on the bitmap image.
            using (Graphics graphics = Graphics.FromImage(target))
            {
                // Scaled image.
                Bitmap imageScaled = target;

                // If scaled.
                if (scaled)
                {
                    // Get the scaled image.
                    imageScaled = new Bitmap(_width, _height);
                    graphicsScaled = Graphics.FromImage(imageScaled);
                }

                // While enabled.
                while (_enabledPrimaryScreen)
                {
                    // Copy the screen to the target.
                    graphics.CopyFromScreen(0, 0, 0, 0, primaryScreen);

                    // Show the cursor be drawn.
                    if (_showCursor)
                        Cursors.Default.Draw(graphics, new Rectangle(Cursor.Position, _cursorSize));

                    // If scaled.
                    if (scaled)
                        graphicsScaled.DrawImage(target, new Rectangle(0, 0, _width, _height), screenSize, GraphicsUnit.Pixel);

                    // Yield the image.
                    yield return imageScaled;
                }

                // Cleaup.
                if(graphicsScaled != null)
                    graphicsScaled.Dispose();
            }

            yield break;
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ScreenCapture()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
