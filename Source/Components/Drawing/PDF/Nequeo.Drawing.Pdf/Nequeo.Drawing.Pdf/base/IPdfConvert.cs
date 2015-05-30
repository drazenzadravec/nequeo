/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2011 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nequeo.Drawing;

namespace Nequeo.Drawing.Pdf
{
	/// <summary>
    /// PDF Conversion iterface
    /// </summary>
    public interface IPdfConvert
    {
        /// <summary>
        /// Save the image file to a Pdf file.
        /// </summary>
        /// <param name="sourceFile">The source file name to convert from.</param>
        /// <param name="destinationFile">The destination PDF file name.</param>
        /// <param name="conversionType">The type of image to convert to PDF</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        bool Save(string sourceFile, string destinationFile, ConversionType conversionType);

        /// <summary>
        /// Save the image file to a Pdf file.
        /// </summary>
        /// <param name="sourceFile">The source file name to convert from.</param>
        /// <param name="destinationFile">The destination PDF file name.</param>
        /// <returns>True if the conversion was successful; else false.</returns>
        bool Save(string sourceFile, string destinationFile);

        /// <summary>
        /// Save the image file to a Pdf file.
        /// </summary>
        /// <returns>True if the conversion was successful; else false.</returns>
        bool Save();

        /// <summary>
        /// Gets the list of errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        String[] GetErrors();

    }
}
