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
using System.Drawing.Text;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Nequeo.Drawing.Text
{
    /// <summary>
    /// Font collection helper.
    /// </summary>
    public class FontCollection
    {
        /// <summary>
        /// Get the collection of all fonts.
        /// </summary>
        /// <returns>The font family collection.</returns>
        public static System.Drawing.FontFamily[] FontFamilies()
        {
            InstalledFontCollection installedFont = new InstalledFontCollection();
            return installedFont.Families;
        }

        /// <summary>
        /// Get the collection of font names.
        /// </summary>
        /// <returns>The font name collection.</returns>
        public static string[] FontFamilyNames()
        {
            List<string> _names = new List<string>();
            InstalledFontCollection installedFont = new InstalledFontCollection();
            System.Drawing.FontFamily[] fonts = installedFont.Families;

            // For each font add the name.
            for (int i = 0; i < fonts.Length; i++)
                _names.Add(fonts[i].Name);

            // Return the list of names.
            return _names.ToArray();
        }

        /// <summary>
        /// Get the collection of all fonts.
        /// </summary>
        /// <param name="names">The font family names.</param>
        /// <returns>The font family collection.</returns>
        public static System.Drawing.FontFamily[] FontFamilies(string[] names)
        {
            List<System.Drawing.FontFamily> _names = new List<System.Drawing.FontFamily>();

            // Get all installed fonts.
            InstalledFontCollection installedFont = new InstalledFontCollection();
            System.Drawing.FontFamily[] fonts = installedFont.Families;

            // For each font.
            for (int i = 0; i < fonts.Length; i++)
            {
                // For each name.
                for(int j = 0; j < names.Length; j++)
                {
                    // If the font names match.
                    if(fonts[i].Name.ToLower() == names[j].ToLower())
                    {
                        // Add the font.
                        _names.Add(fonts[i]);
                        break;
                    }
                }
            }

            // Return the list of names.
            return _names.ToArray();
        }

        /// <summary>
        /// Get the font family for the font name.
        /// </summary>
        /// <param name="name">The name of the font family.</param>
        /// <returns>The font family if found; else null.</returns>
        public static System.Drawing.FontFamily FontFamily(string name)
        {
            InstalledFontCollection installedFont = new InstalledFontCollection();
            System.Drawing.FontFamily[] fonts = installedFont.Families;
            IEnumerable< System.Drawing.FontFamily> families = fonts.Where(u => u.Name.ToLower() == name.ToLower());

            if (families.Count() > 0)
                return families.First();
            else
                return null;
        }
    }
}
