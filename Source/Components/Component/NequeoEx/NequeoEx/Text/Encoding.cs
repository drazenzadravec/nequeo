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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Text
{
    /// <summary>
    /// Encoding Type.
    /// </summary>
    public enum EncodingType
    {
        /// <summary>
        /// Cncoding for the operating system's.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Encoding for the ASCII (7-bit) character set.
        /// </summary>
        ASCII = 1,
        /// <summary>
        /// Encoding for the UTF-16 format that uses the big endian byte order.
        /// </summary>
        BigEndianUnicode = 2,
        /// <summary>
        /// Encoding for the UTF-16 format using the little endian byte order.
        /// </summary>
        Unicode = 3,
        /// <summary>
        /// Encoding for the UTF-32 format using the little endian byte order.
        /// </summary>
        UTF32 = 4,
        /// <summary>
        /// Encoding for the UTF-7 format.
        /// </summary>
        UTF7 = 5,
        /// <summary>
        /// Encoding for the UTF-8 format.
        /// </summary>
        UTF8 = 6,
    }

    /// <summary>
    /// Represents a character encoding.
    /// </summary>
    public class Encoding
    {
        /// <summary>
        /// Get the encoding from the type.
        /// </summary>
        /// <param name="encodingType">The encoding type.</param>
        /// <returns>The text encoding.</returns>
        public static System.Text.Encoding GetEncoder(EncodingType encodingType)
        {
            switch(encodingType)
            {
                case EncodingType.ASCII:
                    return System.Text.Encoding.ASCII;
                case EncodingType.BigEndianUnicode:
                    return System.Text.Encoding.BigEndianUnicode;
                case EncodingType.Unicode:
                    return System.Text.Encoding.Unicode;
                case EncodingType.UTF32:
                    return System.Text.Encoding.UTF32;
                case EncodingType.UTF7:
                    return System.Text.Encoding.UTF7;
                case EncodingType.UTF8:
                    return System.Text.Encoding.UTF8;
                case EncodingType.Default:
                default:
                    return System.Text.Encoding.Default;
            }
        }

        /// <summary>
        /// Detects the encoding of text provided on a StringReader.
        /// </summary>
        /// <param name="reader">The StringReader containing the text.</param>
        /// <returns>The encoding type.</returns>
        public static EncodingType DetectEncoding(System.IO.StreamReader reader)
        {
            // Select the encoding name.
            switch(reader.CurrentEncoding.EncodingName.ToLower())
            {
                case "ascii":
                    return EncodingType.ASCII;
                case "bigendianunicode":
                    return EncodingType.BigEndianUnicode;
                case "unicode":
                    return EncodingType.Unicode;
                case "utf32":
                    return EncodingType.UTF32;
                case "utf7":
                    return EncodingType.UTF7;
                case "utf8":
                    return EncodingType.UTF8;
                default:
                case "default":
                    return EncodingType.Default;
            }
        }

        /// <summary>
        /// Converts an entire byte array from one encoding to another.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="to">The encoding to convert to.</param>
        /// <param name="from">The encoding to convert from.</param>
        /// <returns>The converted encoding byte array.</returns>
        public static byte[] Convert(byte[] data, EncodingType to, EncodingType from = EncodingType.Default)
        {
            byte[] converted = null;
            switch (to)
            {
                case EncodingType.ASCII:
                    switch (from)
                    {
                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.ASCII, data);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.ASCII, data);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.ASCII, data);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.ASCII, data);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.ASCII, data);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.ASCII, data);
                            break;
                    }
                    break;

                case EncodingType.BigEndianUnicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.BigEndianUnicode, data);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.BigEndianUnicode, data);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.BigEndianUnicode, data);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.BigEndianUnicode, data);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.BigEndianUnicode, data);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.BigEndianUnicode, data);
                            break;
                    }
                    break;

                case EncodingType.Unicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.Unicode, data);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.Unicode, data);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.Unicode, data);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.Unicode, data);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Unicode, data);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.Unicode, data);
                            break;
                    }
                    break;

                case EncodingType.UTF32:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF32, data);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF32, data);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF32, data);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF32, data);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF32, data);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF32, data);
                            break;
                    }
                    break;

                case EncodingType.UTF7:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF7, data);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF7, data);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF7, data);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF7, data);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF7, data);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF7, data);
                            break;
                    }
                    break;

                case EncodingType.UTF8:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF8, data);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF8, data);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, data);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF8, data);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF8, data);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8, data);
                            break;
                    }
                    break;

                default:
                    converted = data;
                    break;
            }

            // Return the converted text.
            return converted;
        }

        /// <summary>
        /// Converts an entire string from one encoding to another.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="to">The encoding to convert to.</param>
        /// <param name="from">The encoding to convert from.</param>
        /// <returns>The converted encoding string.</returns>
        public static string Convert(string data, EncodingType to, EncodingType from = EncodingType.Default)
        {
            byte[] dataSource = null;
            byte[] converted = null;
            string convertedString = null;

            switch (to)
            {
                case EncodingType.ASCII:
                    switch (from)
                    {
                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.ASCII, dataSource);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.ASCII, dataSource);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.ASCII, dataSource);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.ASCII, dataSource);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.ASCII, dataSource);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.ASCII, dataSource);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.BigEndianUnicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.BigEndianUnicode, dataSource);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.BigEndianUnicode, dataSource);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.BigEndianUnicode, dataSource);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.BigEndianUnicode, dataSource);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.BigEndianUnicode, dataSource);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.BigEndianUnicode, dataSource);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.Unicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.Unicode, dataSource);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.Unicode, dataSource);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.Unicode, dataSource);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.Unicode, dataSource);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Unicode, dataSource);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.Unicode, dataSource);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.UTF32:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF32, dataSource);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF32, dataSource);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF32, dataSource);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF32, dataSource);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF32, dataSource);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF32, dataSource);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.UTF7:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF7, dataSource);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF7, dataSource);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF7, dataSource);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF7, dataSource);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF7, dataSource);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF7, dataSource);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.UTF8:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF8, dataSource);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF8, dataSource);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, dataSource);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF8, dataSource);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF8, dataSource);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8, dataSource);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;
                    }
                    break;

                default:
                    convertedString = data;
                    break;
            }

            // Return the converted text.
            return convertedString;
        }

        /// <summary>
        /// Converts an entire string from one encoding to another.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="to">The encoding to convert to.</param>
        /// <param name="from">The encoding to convert from.</param>
        /// <returns>The converted encoding string.</returns>
        public static byte[] ConvertGetBytes(string data, EncodingType to, EncodingType from = EncodingType.Default)
        {
            byte[] dataSource = null;
            byte[] converted = null;

            switch (to)
            {
                case EncodingType.ASCII:
                    switch (from)
                    {
                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.ASCII, dataSource);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.ASCII, dataSource);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.ASCII, dataSource);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.ASCII, dataSource);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.ASCII, dataSource);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.ASCII, dataSource);
                            break;
                    }
                    break;

                case EncodingType.BigEndianUnicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.BigEndianUnicode, dataSource);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.BigEndianUnicode, dataSource);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.BigEndianUnicode, dataSource);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.BigEndianUnicode, dataSource);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.BigEndianUnicode, dataSource);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.BigEndianUnicode, dataSource);
                            break;
                    }
                    break;

                case EncodingType.Unicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.Unicode, dataSource);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.Unicode, dataSource);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.Unicode, dataSource);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.Unicode, dataSource);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Unicode, dataSource);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.Unicode, dataSource);
                            break;
                    }
                    break;

                case EncodingType.UTF32:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF32, dataSource);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF32, dataSource);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF32, dataSource);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF32, dataSource);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF32, dataSource);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF32, dataSource);
                            break;
                    }
                    break;

                case EncodingType.UTF7:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF7, dataSource);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF7, dataSource);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF7, dataSource);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF7, dataSource);
                            break;

                        case EncodingType.UTF8:
                            dataSource = System.Text.Encoding.UTF8.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF7, dataSource);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF7, dataSource);
                            break;
                    }
                    break;

                case EncodingType.UTF8:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            dataSource = System.Text.Encoding.ASCII.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF8, dataSource);
                            break;

                        case EncodingType.BigEndianUnicode:
                            dataSource = System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF8, dataSource);
                            break;

                        case EncodingType.Unicode:
                            dataSource = System.Text.Encoding.Unicode.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, dataSource);
                            break;

                        case EncodingType.UTF32:
                            dataSource = System.Text.Encoding.UTF32.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF8, dataSource);
                            break;

                        case EncodingType.UTF7:
                            dataSource = System.Text.Encoding.UTF7.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF8, dataSource);
                            break;

                        default:
                            dataSource = System.Text.Encoding.Default.GetBytes(data);
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8, dataSource);
                            break;
                    }
                    break;

                default:
                    converted = System.Text.Encoding.Default.GetBytes(data);
                    break;
            }

            // Return the converted text.
            return converted;
        }

        /// <summary>
        /// Converts an entire byte array from one encoding to another.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="to">The encoding to convert to.</param>
        /// <param name="from">The encoding to convert from.</param>
        /// <returns>The converted encoding string.</returns>
        public static string ConvertGetString(byte[] data, EncodingType to, EncodingType from = EncodingType.Default)
        {
            byte[] converted = null;
            string convertedString = null;

            switch (to)
            {
                case EncodingType.ASCII:
                    switch (from)
                    {
                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.ASCII, data);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.ASCII, data);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.ASCII, data);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.ASCII, data);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.ASCII, data);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.ASCII, data);
                            convertedString = System.Text.Encoding.ASCII.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.BigEndianUnicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.BigEndianUnicode, data);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.BigEndianUnicode, data);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.BigEndianUnicode, data);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.BigEndianUnicode, data);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.BigEndianUnicode, data);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.BigEndianUnicode, data);
                            convertedString = System.Text.Encoding.BigEndianUnicode.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.Unicode:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.Unicode, data);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.Unicode, data);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.Unicode, data);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.Unicode, data);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Unicode, data);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.Unicode, data);
                            convertedString = System.Text.Encoding.Unicode.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.UTF32:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF32, data);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF32, data);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF32, data);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF32, data);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF32, data);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF32, data);
                            convertedString = System.Text.Encoding.UTF32.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.UTF7:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF7, data);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF7, data);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF7, data);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF7, data);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        case EncodingType.UTF8:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF7, data);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF7, data);
                            convertedString = System.Text.Encoding.UTF7.GetString(converted);
                            break;
                    }
                    break;

                case EncodingType.UTF8:
                    switch (from)
                    {
                        case EncodingType.ASCII:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF8, data);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.BigEndianUnicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.UTF8, data);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.Unicode:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, data);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.UTF32:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF32, System.Text.Encoding.UTF8, data);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        case EncodingType.UTF7:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.UTF7, System.Text.Encoding.UTF8, data);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;

                        default:
                            converted = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8, data);
                            convertedString = System.Text.Encoding.UTF8.GetString(converted);
                            break;
                    }
                    break;

                default:
                    convertedString = System.Text.Encoding.Default.GetString(data);
                    break;
            }

            // Return the converted text.
            return convertedString;
        }
    }
}
