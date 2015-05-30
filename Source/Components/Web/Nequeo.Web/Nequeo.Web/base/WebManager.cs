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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Design;
using System.Web.Compilation;
using System.Web.Security;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;

namespace Nequeo.Web
{
    /// <summary>
    /// Web implementation manger, contains members to access web functionality
    /// </summary>
    public sealed class WebManager
    {
        internal static string _separator = "|";
        private static Random _random = new Random();
        private static string[] _fontFamilies = { 
                "Arial", "Comic Sans MS", "Courier New", "Georgia", "Lucida Console", "MS Sans Serif", 
                "Stencil", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana", "Mangneto", "Bauhaus 93",
                "Bernard MT Condensed", "DigifaceWide", "Impact", "Lucida Calligraphy", "Matura MT Script Capitals",
                "Pump Demi Bold LET", "Rockwell Condensed,", "Stencil", "Wide Latin", "Vrinda"};

        /// <summary>
        /// Form authentication decryption.
        /// </summary>
        /// <param name="encryptedContent">The encrypted data.</param>
        /// <returns>The decrypted data else empty string.</returns>
        public static string Decrypt(string encryptedContent)
        {
            string decryptedText = string.Empty;
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedContent);
                if (!ticket.Expired)
                    decryptedText = ticket.UserData;
            }
            catch { }
            return decryptedText;
        }

        /// <summary>
        /// Form authentication encryption.
        /// </summary>
        /// <param name="content">The data to encrypt.</param>
        /// <param name="expiration">The expiration time from now.</param>
        /// <returns>The encrypted data.</returns>
        public static string Encrypt(string content, DateTime expiration)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1, HttpContext.Current.Request.UserHostAddress, DateTime.Now, expiration, false, content);
            return FormsAuthentication.Encrypt(ticket);
        }

        /// <summary>
        /// Create the bitmap size image, with a random backgroud gradient.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns></returns>
        public static Bitmap CreateImage(int width, int height)
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
                        Convert.ToSingle(_random.NextDouble()) * 360, false);

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
        public static void DistortImage(ref Bitmap bitmap, double distortion)
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
                    int newX = Convert.ToInt32(x + (distortion * System.Math.Sin(System.Math.PI * y / 64.0)));
                    int newY = Convert.ToInt32(y + (distortion * System.Math.Cos(System.Math.PI * x / 64.0)));

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
        /// <returns>The new graphic path containing the text.</returns>
        public static GraphicsPath CreateText(string text, int width, int height, Graphics graphics)
        {
            GraphicsPath textPath = new GraphicsPath();
            FontFamily fontFamily = GetFont();
            int emSize = Convert.ToInt32(width * 2 / text.Length);
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
                textPath.AddString(text, font.FontFamily, Convert.ToInt32(font.Style), font.Size, new RectangleF(0, 0, width, height), stringFormat);

                // Warp the text in the image.
                WarpText(ref textPath, new Rectangle(0, 0, width, height));
            }
            catch (Exception) { throw; }
            finally { font.Dispose(); }

            // Return the text path.
            return textPath;
        }

        /// <summary>
        /// Get the font family from the list.
        /// </summary>
        /// <returns></returns>
        public static FontFamily GetFont()
        {
            FontFamily font = null;
            while (font == null)
            {
                try
                {
                    // Randomly choose a font.
                    font = new FontFamily(_fontFamilies[_random.Next(_fontFamilies.Length)]);
                }
                catch { font = null; }
            }
            return font;
        }

        /// <summary>
        /// Generate the image from the form authentication encrypted method.
        /// </summary>
        /// <param name="encryptedText">The encypted text and dementions of the image to generate.</param>
        /// <returns>The bitmap image containg the captcha.</returns>
        public static Bitmap GenerateImage(string encryptedText)
        {
            // Decrypt the demension data from the encyted text.
            Bitmap bitmap = null;
            string encodedText = Decrypt(encryptedText);
            string[] settings = encodedText.Split(_separator.ToCharArray(), StringSplitOptions.None);

            try
            {
                // Get each value from the data.
                int width = Int32.Parse(settings[0]);
                int height = Int32.Parse(settings[1]);
                string text = settings[2];
                string backgroundImage = settings[3];

                // Create a new graphic.
                Graphics graphics = null;
                Brush brush = new SolidBrush(Color.LightGray);
                Brush brush1 = new SolidBrush(Color.Black);

                // If the back ground image has not been
                // set the create a new default image
                if (String.IsNullOrEmpty(backgroundImage))
                    bitmap = CreateImage(width, height);
                else
                    // If the back ground image has been set
                    // then use this image as the back ground.
                    bitmap = (Bitmap)Convert.ChangeType(Bitmap.FromFile(HttpContext.Current.Request.MapPath(backgroundImage)), typeof(Bitmap));

                // Create the graphic from the image.
                graphics = Graphics.FromImage(bitmap);
                GraphicsPath textPath = CreateText(text, width, height, graphics);

                // If the back ground image has not been set
                // then use the first brush else use the second.
                if (String.IsNullOrEmpty(backgroundImage))
                    graphics.FillPath(brush, textPath);
                else
                    graphics.FillPath(brush1, textPath);
            }
            catch (Exception) { throw; }

            // Return the new image created.
            return bitmap;
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
        public static PointF RandomPoint(int xMin, int xMax, int yMin, int yMax)
        {
            return new PointF(_random.Next(xMin, xMax), _random.Next(yMin, yMax));
        }

        /// <summary>
        /// Warp the text in the image.
        /// </summary>
        /// <param name="textPath">The graphic path containg the text.</param>
        /// <param name="rectangle">The rectangle that will contain the image.</param>
        public static void WarpText(ref GraphicsPath textPath, Rectangle rectangle)
        {
            int warpDivisor = _random.Next(4, 8);
            RectangleF rectangleF = new RectangleF(0, 0, rectangle.Width, rectangle.Height);

            int hRange = Convert.ToInt32(rectangle.Height / warpDivisor);
            int wRange = Convert.ToInt32(rectangle.Width / warpDivisor);

            PointF pointF1 = RandomPoint(0, wRange, 0, hRange);
            PointF pointF2 = RandomPoint(rectangle.Width - (wRange - Convert.ToInt32(pointF1.X)), rectangle.Width, 0, hRange);
            PointF pointF3 = RandomPoint(0, wRange, rectangle.Height - (hRange - Convert.ToInt32(pointF1.Y)), rectangle.Height);
            PointF pointF4 = RandomPoint(rectangle.Width - (wRange - Convert.ToInt32(pointF3.X)), rectangle.Width, rectangle.Height - (hRange - Convert.ToInt32(pointF2.Y)), rectangle.Height);

            PointF[] points = new PointF[] { pointF1, pointF2, pointF3, pointF4 };
            Matrix matrix = new Matrix();
            matrix.Translate(0, 0);
            textPath.Warp(points, rectangleF, matrix, WarpMode.Perspective, 0);
        }

        /// <summary>
        /// Add the control to the page.
        /// </summary>
        /// <param name="page">The page to add the control to.</param>
        /// <param name="control">The control to add.</param>
        public static void AddControl(System.Web.UI.Page page, System.Web.UI.Control control)
        {
            // Make sure the page reference exists.
            if (page == null) throw new ArgumentNullException("page");
            if (control == null) throw new ArgumentNullException("control");

            // Add the control to the page on the form.
            page.Form.Controls.Add(control);
        }

        /// <summary>
        /// Add the controls to the page.
        /// </summary>
        /// <param name="page">The page to add the control to.</param>
        /// <param name="controls">The controls to add.</param>
        public static void AddControls(System.Web.UI.Page page, IEnumerable<System.Web.UI.Control> controls)
        {
            // Make sure the page reference exists.
            if (page == null) throw new ArgumentNullException("page");
            if (controls == null) throw new ArgumentNullException("controls");

            // For each control found add to the page.
            foreach (System.Web.UI.Control control in controls)
                // Add the control to the page on the form.
                page.Form.Controls.Add(control);
        }

        /// <summary>
        /// Create a collection of calendar controls from the data object type.
        /// </summary>
        /// <param name="dataObjectType">The data object type to examine.</param>
        /// <param name="prefixControlName">The prefix control name</param>
        /// <returns>The collection of calendar controls.</returns>
        public static IEnumerable<Nequeo.Web.UI.ScriptControl.Calendar> CreateCalendarControls(object dataObjectType, string prefixControlName)
        {
            // Create a new calendar collection.
            List<Nequeo.Web.UI.ScriptControl.Calendar> calendars = 
                new List<Nequeo.Web.UI.ScriptControl.Calendar>();

            // For each property member in the current type.
            foreach (PropertyInfo member in dataObjectType.GetType().GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // If the current member is tyoe datetime.
                        if ((member.PropertyType == typeof(System.DateTime) ? true : false))
                        {
                            // Return the calendar control.
                            Nequeo.Web.UI.ScriptControl.Calendar calendar = 
                                new Nequeo.Web.UI.ScriptControl.Calendar()
                                {
                                    ID = (prefixControlName != null ? prefixControlName : "") + member.Name + "_Calendar",
                                    TargetControlID = (prefixControlName != null ? prefixControlName : "") + member.Name,
                                    PopupPosition = Nequeo.Web.Common.CalendarPosition.Right
                                };

                            // Add the calendar to the collection.
                            calendars.Add(calendar);
                        }
                    }
                }
            }

            // Return the collection of calendrs.
            return calendars.ToArray();
        }

        /// <summary>
        /// Add the client script block.
        /// </summary>
        /// <param name="type">The type of the client script to register.</param>
        /// <param name="page">The page to register the client script on.</param>
        /// <param name="key">The key of the client script to register.</param>
        /// <param name="script">The client script literal to register.</param>
        public static void AddClientScriptBlock(Type type, Page page, string key, StringBuilder script)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");
            if (page == null) throw new ArgumentNullException("page");
            if (key == null) throw new ArgumentNullException("key");
            if (script == null) throw new ArgumentNullException("script");

            // Get the client script collection on the current page.
            ClientScriptManager clientScript = page.ClientScript;

            // Check to see if the client script is already registered.
            if (!clientScript.IsClientScriptBlockRegistered(type, key))
            {
                // Register the client script.
                clientScript.RegisterClientScriptBlock(type, key,
                    "<script type=\"text/javascript\">" + "\r\n" +
                        script.ToString() + "\r\n" +
                    "</script>",
                    false);
            }
        }

        /// <summary>
        /// Add the client script source.
        /// </summary>
        /// <param name="type">The type of the client script to register.</param>
        /// <param name="page">The page to register the client script on.</param>
        /// <param name="key">The key of the client script to register.</param>
        /// <param name="source">The source relative url.</param>
        public static void AddClientScriptSource(Type type, Page page, string key, string source)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");
            if (page == null) throw new ArgumentNullException("page");
            if (key == null) throw new ArgumentNullException("key");
            if (source == null) throw new ArgumentNullException("source");

            // Get the client script collection on the current page.
            ClientScriptManager clientScript = page.ClientScript;

            // Check to see if the client script is already registered.
            if (!clientScript.IsClientScriptBlockRegistered(type, key))
            {
                // Register the client script.
                clientScript.RegisterClientScriptBlock(type, key,
                    "<script type=\"text/javascript\" src=\"" + source + "\"></script>",
                    false);
            }
        }

        /// <summary>
        /// Add the jQuery client script block and jQuery version source.
        /// </summary>
        /// <param name="type">The type of the client script to register.</param>
        /// <param name="page">The page to register the client script on.</param>
        /// <param name="key">The key of the client script to register.</param>
        /// <param name="source">The jQuery source relative url.</param>
        /// <param name="script">The jQuery client script initialisation code literal to register.</param>
        public static void AddJQueryClientScriptSource(Type type, Page page, string key, string source, StringBuilder script)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");
            if (page == null) throw new ArgumentNullException("page");
            if (key == null) throw new ArgumentNullException("key");
            if (source == null) throw new ArgumentNullException("source");

            // Get the client script collection on the current page.
            ClientScriptManager clientScript = page.ClientScript;

            // Check to see if the client script is already registered.
            if (!clientScript.IsClientScriptBlockRegistered(type, key))
            {
                // Register the client script.
                clientScript.RegisterClientScriptBlock(type, key,
                    "<script type=\"text/javascript\" src=\"" + source + "\"></script>" + "\r\n" +
                    "<script type=\"text/javascript\">" + "\r\n" +
                        "jQuery(document).ready(function () {" + "\r\n" +
                            script.ToString() + "\r\n" +
                        "});" + "\r\n" +
                    "</script>",
                    false);
            }
        }

        /// <summary>
        /// Add a link tag of type stylesheet to the page header.
        /// </summary>
        /// <param name="page">The page to register the style sheet on.</param>
        /// <param name="source">The source url refrence of the style sheet.</param>
        public static void AddLinkStylesheetSource(Page page, string source)
        {
            // Make sure the page reference exists.
            if (page == null) throw new ArgumentNullException("page");
            if (source == null) throw new ArgumentNullException("source");

            // Add the link to the page header instead 
            // of inside the body which is not xhtml compliant.
            HtmlHead header = page.Header;
            bool addIt = true;

            // looking for this stylesheet already in the header
            foreach (Control control in header.Controls)
            {
                // If the control type is a link control type.
                if(control.GetType() == typeof(HtmlLink))
                {
                    HtmlLink link = control as HtmlLink;

                    // If the link control does exist
                    // then do not add.
                    if (link != null && source.Equals(link.Href, StringComparison.OrdinalIgnoreCase))
                    {
                        addIt = false;
                        break;
                    }
                }
            }

            // If true add the link.
            if (addIt)
            {
                // Add the style sheet.
                HtmlLink link = new HtmlLink();
                link.Href = source;
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                header.Controls.Add(link);
            }
        }

        /// <summary>
        /// Add the client style block.
        /// </summary>
        /// <param name="type">The type of the client style to register.</param>
        /// <param name="page">The page to register the client style on.</param>
        /// <param name="key">The key of the client style to register.</param>
        /// <param name="style">The client style literal to register.</param>
        public static void AddStylesheetBlock(Type type, Page page, string key, StringBuilder style)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");
            if (page == null) throw new ArgumentNullException("page");
            if (key == null) throw new ArgumentNullException("key");
            if (style == null) throw new ArgumentNullException("style");

            // Get the client script collection on the current page.
            ClientScriptManager clientScript = page.ClientScript;

            // Check to see if the client script is already registered.
            if (!clientScript.IsClientScriptBlockRegistered(type, key))
            {
                // Register the client script.
                clientScript.RegisterClientScriptBlock(type, key,
                    "<style type=\"text/css\">" + "\r\n" +
                        style.ToString() + "\r\n" +
                    "</style>",
                    false);
            }
        }

        /// <summary>
        /// Add the style to the specified selector.
        /// </summary>
        /// <param name="page">The page to register the style sheet on.</param>
        /// <param name="style">The style rule to be added to the embedded style sheet.</param>
        /// <param name="selector">The part of the HTML page affected by the style (page tag name).</param>
        public static void AddStyle(Page page, Style style, string selector)
        {
            // Make sure the page reference exists.
            if (page == null) throw new ArgumentNullException("page");
            if (selector == null) throw new ArgumentNullException("selector");
            if (style == null) throw new ArgumentNullException("style");

            // Add the link to the page header instead 
            // of inside the body which is not xhtml compliant.
            HtmlHead header = page.Header;
            header.StyleSheet.CreateStyleRule(style, null, selector);
        }

        /// <summary>
        /// Parse the form body.
        /// </summary>
        /// <param name="body">The data to parse.</param>
        /// <returns>The name value collection.</returns>
        public static NameValueCollection ParseForm(byte[] body)
        {
            NameValueCollection nameValue = new NameValueCollection();

            // If data exists.
            if (body != null && body.Length > 0)
            {
                string bodyStr = Encoding.Default.GetString(body);
                string[] split = bodyStr.Split('&');
                foreach (string item in split)
                {
                    string[] equ = item.Split('=');
                    nameValue.Add(equ[0], equ[1]);
                }
            }
            return nameValue;
        }

        /// <summary>
        /// Parse the form body.
        /// </summary>
        /// <param name="body">The data to parse.</param>
        /// <returns>The name value collection.</returns>
        public static NameValueCollection ParseForm(string body)
        {
            NameValueCollection nameValue = new NameValueCollection();

            // If data exists.
            if (!String.IsNullOrEmpty(body))
            {
                string[] split = body.Split('&');
                foreach (string item in split)
                {
                    string[] equ = item.Split('=');
                    nameValue.Add(equ[0], equ[1]);
                }
            }
            return nameValue;
        }

        /// <summary>
        /// Create the query string from the collection.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <returns>The query string.</returns>
        public static string CreateQueryString(NameValueCollection collection)
        {
            string query = "";

            // Iterate through the collection.
            foreach (string item in collection.AllKeys)
            {
                query += item + "=" + collection[item] + "&";
            }

            // Return the query string.
            return query.TrimEnd('&');
        }
    }
}
