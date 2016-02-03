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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using Nequeo.Extension;

namespace Nequeo.Html
{
    /// <summary>
    /// Html helper.
    /// </summary>
    public sealed class HtmlHelper
    {
        /// <summary>
        /// Parse the html data containing only the html elements.
        /// </summary>
        /// <param name="input">The stream containing the html data.</param>
        /// <returns>The stream containing only the html elements.</returns>
        public static System.IO.MemoryStream ParseHtml(System.IO.Stream input)
        {
            int bytesRead = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            byte[] html = null;
            int position = 0;

            // While the end of the header data
            // has not been found.
            while (!foundEndOfData)
            {
                // Read a single byte from the stream
                // add the data to the store and re-assign.
                bytesRead = input.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Store the temp bytes.
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > 4)
                    {
                        // Get the last five bytes.
                        html = new byte[] { store[position - 4], store[position - 3], store[position - 2], store[position - 1], store[position] };
                        string htmlValue = Encoding.ASCII.GetString(html).ToLower();

                        // If the end of the header data has been found '<html'.
                        if (htmlValue.Equals("<html"))
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfData)
            {
                // Create the memory stream.
                System.IO.MemoryStream htmlData = new MemoryStream();
                byte[] htmlBuffer = new byte[8192];

                // Write the '<html' data first.
                htmlData.Write(html, 0, html.Length);

                // Read and write the html document.
                do
                {
                    // Read data.
                    bytesRead = input.Read(htmlBuffer, 0, htmlBuffer.Length);

                    // If data exists.
                    if (bytesRead > 0)
                    {
                        // Write the data.
                        htmlData.Write(htmlBuffer, 0, bytesRead);
                    }
                }
                while (bytesRead != 0);

                // Set the begining of the stream.
                htmlData.Position = 0;

                // Return the stream.
                return htmlData;
            }
            else
                return null;
        }
    }
}
