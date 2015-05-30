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
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Drawing.Pdf
{
    /// <summary>
    /// PDF document font.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Font encoding.
        /// </summary>
        public enum FontEncoding
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined,
            /// <summary>
            /// Cp1250
            /// </summary>
            CP1250,
            /// <summary>
            /// Cp1252
            /// </summary>
            CP1252,
            /// <summary>
            /// Cp1257
            /// </summary>
            CP1257,
            /// <summary>
            /// Cp1252
            /// </summary>
            WINANSI,
            /// <summary>
            /// MacRoman
            /// </summary>
            MACROMAN,
        }

        /// <summary>
        /// Font family.
        /// </summary>
        public enum FontFamily
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined,
            /// <summary>
            /// Courier
            /// </summary>
            Courier,
            /// <summary>
            /// Courier-Bold
            /// </summary>
            Courier_Bold,
            /// <summary>
            /// Courier-Oblique
            /// </summary>
            Courier_Oblique,
            /// <summary>
            /// Courier-BoldOblique
            /// </summary>
            Courier_BoldOblique,
            /// <summary>
            /// Helvetica
            /// </summary>
            Helvetica,
            /// <summary>
            /// Helvetica-Bold
            /// </summary>
            Helvetica_Bold,
            /// <summary>
            /// Helvetica-Oblique
            /// </summary>
            Helvetica_Oblique,
            /// <summary>
            /// Helvetica-BoldOblique
            /// </summary>
            Helvetica_BoldOblique,
            /// <summary>
            /// Symbol
            /// </summary>
            Symbol,
            /// <summary>
            /// Times-Roman
            /// </summary>
            Times_Roman,
            /// <summary>
            /// Times-Bold
            /// </summary>
            Times_Bold,
            /// <summary>
            /// Times-Italic
            /// </summary>
            Times_Italic,
            /// <summary>
            /// Times-BoldItalic
            /// </summary>
            Times_BoldItalic,
            /// <summary>
            /// ZapfDingbats
            /// </summary>
            ZapfDingbats,
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        public Font(FontFamily fontFamily)
        {
            _fontFamily = fontFamily;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="color">The font color.</param>
        public Font(FontFamily fontFamily, Color color) 
        {
            _fontFamily = fontFamily;
            _color = color;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="color">The font color.</param>
        /// <param name="style">The font style.</param>
        public Font(FontFamily fontFamily, Color color, FontStyle style)
        {
            _fontFamily = fontFamily;
            _color = color;
            _style = style;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="color">The font color.</param>
        /// <param name="style">The font style.</param>
        /// <param name="fontSize">The font size.</param>
        public Font(FontFamily fontFamily, Color color, FontStyle style, float fontSize)
        {
            _fontFamily = fontFamily;
            _color = color;
            _style = style;
            _size = fontSize;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontName">The font name or file name (e.g. Courier, ..\fonts\arial.ttf).</param>
        public Font(string fontName)
        {
            _fontName = fontName;
            _fontFamily = FontFamily.Undefined;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontName">The font name or file name (e.g. Courier, ..\fonts\arial.ttf).</param>
        /// <param name="encoding">The font encoding.</param>
        public Font(string fontName, string encoding)
        {
            _fontName = fontName;
            _encoding = encoding;
            _fontFamily = FontFamily.Undefined;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontName">The font name or file name (e.g. Courier, ..\fonts\arial.ttf).</param>
        /// <param name="encoding">The font encoding.</param>
        /// <param name="embedded">The font embedded.</param>
        public Font(string fontName, string encoding, bool embedded)
        {
            _fontName = fontName;
            _encoding = encoding;
            _embedded = embedded;
            _fontFamily = FontFamily.Undefined;
            _fontEncoding = FontEncoding.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontName">The font name or file name (e.g. Courier, ..\fonts\arial.ttf).</param>
        /// <param name="encoding">The font encoding.</param>
        public Font(string fontName, FontEncoding encoding)
        {
            _fontName = fontName;
            _fontEncoding = encoding;
            _fontFamily = FontFamily.Undefined;
        }

        /// <summary>
        /// PDF document font.
        /// </summary>
        /// <param name="fontName">The font name or file name (e.g. Courier, ..\fonts\arial.ttf).</param>
        /// <param name="encoding">The font encoding.</param>
        /// <param name="embedded">The font embedded.</param>
        public Font(string fontName, FontEncoding encoding, bool embedded)
        {
            _fontName = fontName;
            _fontEncoding = encoding;
            _embedded = embedded;
            _fontFamily = FontFamily.Undefined;
        }

        private string _fontName = "Courier";
        private string _encoding = "Cp1252";
        private FontEncoding _fontEncoding = FontEncoding.WINANSI;
        private bool _embedded = false;
        private FontFamily _fontFamily = FontFamily.Times_Roman;
        private Color _color = System.Drawing.Color.Black;
        private float _size = 12f;
        private FontStyle _style = FontStyle.Regular;

        /// <summary>
        /// Gets or sets the font color.
        /// </summary>
        public Color FontColor
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _style; }
            set { _style = value; }
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public float FontSize
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Gets or sets the font embedded mode.
        /// </summary>
        public bool Embedded
        {
            get { return _embedded; }
            set { _embedded = value; }
        }

        /// <summary>
        /// Get the converted font.
        /// </summary>
        /// <returns>The text font.</returns>
        internal iTextSharp.text.Font GetFont()
        {
            // Create the text font.
            iTextSharp.text.BaseColor color = new iTextSharp.text.BaseColor(_color);
            iTextSharp.text.Font font = new iTextSharp.text.Font(GetBaseFont(), _size, (int)_style, color);
            return font;
        }

        /// <summary>
        /// Get the converted font.
        /// </summary>
        /// <returns>The text font.</returns>
        private iTextSharp.text.pdf.BaseFont GetBaseFont()
        {
            iTextSharp.text.pdf.BaseFont font = null;

            // Select the font family.
            switch(_fontFamily)
            {
                case FontFamily.Courier:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.COURIER,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Courier_Bold:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.COURIER_BOLD,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Courier_BoldOblique:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.COURIER_BOLDOBLIQUE,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Courier_Oblique:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.COURIER_OBLIQUE,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Helvetica:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.HELVETICA, 
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Helvetica_Bold:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.HELVETICA_BOLD,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Helvetica_BoldOblique:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.HELVETICA_BOLDOBLIQUE,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Helvetica_Oblique:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.HELVETICA_OBLIQUE,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Symbol:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.SYMBOL,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Times_Bold:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.TIMES_BOLD,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Times_BoldItalic:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.TIMES_BOLDITALIC,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Times_Italic:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.TIMES_ITALIC,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Times_Roman:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.TIMES_ROMAN,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.ZapfDingbats:
                    font = iTextSharp.text.pdf.BaseFont.CreateFont(
                        iTextSharp.text.pdf.BaseFont.ZAPFDINGBATS,
                        iTextSharp.text.pdf.BaseFont.WINANSI,
                        _embedded);
                    break;

                case FontFamily.Undefined:
                default:
                    // Select the font encoding.
                    switch (_fontEncoding)
                    {
                        case FontEncoding.CP1250:
                            font = iTextSharp.text.pdf.BaseFont.CreateFont(
                                _fontName,
                                iTextSharp.text.pdf.BaseFont.CP1250,
                                _embedded);
                            break;

                        case FontEncoding.CP1252:
                            font = iTextSharp.text.pdf.BaseFont.CreateFont(
                                _fontName,
                                iTextSharp.text.pdf.BaseFont.CP1252,
                                _embedded);
                            break;

                        case FontEncoding.CP1257:
                            font = iTextSharp.text.pdf.BaseFont.CreateFont(
                                _fontName,
                                iTextSharp.text.pdf.BaseFont.CP1257,
                                _embedded);
                            break;

                        case FontEncoding.MACROMAN:
                            font = iTextSharp.text.pdf.BaseFont.CreateFont(
                                _fontName,
                                iTextSharp.text.pdf.BaseFont.MACROMAN,
                                _embedded);
                            break;

                        case FontEncoding.WINANSI:
                            font = iTextSharp.text.pdf.BaseFont.CreateFont(
                                _fontName,
                                iTextSharp.text.pdf.BaseFont.WINANSI,
                                _embedded);
                            break;

                        case FontEncoding.Undefined:
                        default:
                            font = iTextSharp.text.pdf.BaseFont.CreateFont(
                                _fontName,
                                _encoding,
                                _embedded);
                            break;
                    }
                    break;
            }

            // Return the base font;
            return font;
        }
    }
}
