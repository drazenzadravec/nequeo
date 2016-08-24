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
using System.Collections.ObjectModel;

namespace Nequeo.Document
{
	/// <summary>
    /// Word file format enum.
    /// </summary>
	public enum WordFileFormat : long
	{
        /// <summary>
        /// The original format that the document was created.
        /// </summary>
        Original = 0,
        /// <summary>
        /// Portable document format
        /// </summary>
        PDF = 1,
        /// <summary>
        /// Hypertext markup langauge
        /// </summary>
        HTML = 2,
        /// <summary>
        /// Extensible markup language
        /// </summary>
        XML = 3,
        /// <summary>
        /// Rich text format
        /// </summary>
        RTF = 4,
        /// <summary>
        /// XML Paper Specification
        /// </summary>
        XPS = 5,
	}

    /// <summary>
    /// Excel file format enum.
    /// </summary>
    public enum ExcelFileFormat : long
    {
        /// <summary>
        /// The original format that the document was created.
        /// </summary>
        Original = 0,
        /// <summary>
        /// Portable document format
        /// </summary>
        PDF = 1,
        /// <summary>
        /// Hypertext markup langauge
        /// </summary>
        HTML = 2,
        /// <summary>
        /// Extensible markup language
        /// </summary>
        XML = 3,
        /// <summary>
        /// XML Paper Specification
        /// </summary>
        XPS = 4,
        /// <summary>
        /// Comma-separated values
        /// </summary>
        CSV = 5,
    }

    /// <summary>
    /// File format enum.
    /// </summary>
    public enum FileFormat : long
    {
        /// <summary>
        /// The original format that the document was created.
        /// </summary>
        Original = 0,
        /// <summary>
        /// Portable document format
        /// </summary>
        PDF = 1,
        /// <summary>
        /// Hypertext markup langauge
        /// </summary>
        HTML = 2,
        /// <summary>
        /// Extensible markup language
        /// </summary>
        XML = 3,
        /// <summary>
        /// Rich text format
        /// </summary>
        RTF = 4,
        /// <summary>
        /// XML Paper Specification
        /// </summary>
        XPS = 5,
    }

    /// <summary>
    /// Document file type enum.
    /// </summary>
    public enum FileType : long
    {
        /// <summary>
        /// Microsoft Word Document Type
        /// </summary>
        MicrosoftWord = 0,
        /// <summary>
        /// Microsoft Excel Document Type
        /// </summary>
        MicrosoftExcel = 1,
    }

    /// <summary>
    /// Converts system types to specific values.
    /// </summary>
    internal class SystemType
    {
        /// <summary>
        /// Convert the document mapping value.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="value">The value of the object.</param>
        /// <param name="fileType">The document type.</param>
        /// <returns>The string value.</returns>
        internal static string ConvertMapping(Type type, object value, FileType fileType)
        {
            switch (type.Name.ToLower())
            {
                case "system.guid":
                case "guid":
                    return new Guid(value.ToString()).ToString();

                case "system.boolean":
                case "boolean":
                case "bool":
                    // The value to return when mapping
                    // to a document with boolean type.
                    Boolean itemBoolean = (Boolean)value;
                    switch (fileType)
                    {
                        case FileType.MicrosoftExcel:
                        case FileType.MicrosoftWord:
                            if (itemBoolean)
                                return "X";
                            else
                                return "";
                        default:
                            return "";
                    }

                default:
                    // Return the original value if string.
                    return (value != null ? value.ToString() : "");
            }
        }

        /// <summary>
        /// Convert the document mapping value in an alternative format.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="value">The value of the object.</param>
        /// <param name="fileType">The document type.</param>
        /// <returns>The string value.</returns>
        internal static string ConvertMappingAlt1(Type type, object value, FileType fileType)
        {
            switch (type.Name.ToLower())
            {
                case "system.guid":
                case "guid":
                    return new Guid(value.ToString()).ToString();

                case "system.boolean":
                case "boolean":
                case "bool":
                    // The value to return when mapping
                    // to a document with boolean type.
                    Boolean itemBoolean = (Boolean)value;
                    switch (fileType)
                    {
                        case FileType.MicrosoftExcel:
                        case FileType.MicrosoftWord:
                            if (itemBoolean)
                                return "Y";
                            else
                                return "N";
                        default:
                            return "N";
                    }

                default:
                    // Return the original value if string.
                    return (value != null ? value.ToString() : "");
            }
        }
    }
}
