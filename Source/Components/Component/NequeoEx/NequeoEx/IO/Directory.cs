/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Nequeo.IO
{
    /// <summary>
    /// Directory helper class.
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// Get all directories in the source folder.
        /// </summary>
        /// <param name="sourceFolder">The source folder to search in.</param>
        /// <param name="searchPattern">The search pattern filter.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>The list of all directories including sub-folders.</returns>
        public string[] GetDirectories(string sourceFolder, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<string> directories = new List<string>();
            string[] patterns = searchPattern.Split(new char[] { ';' });

            foreach (string pattern in patterns)
            {
                // Get the collection of directories.
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder, pattern.Trim(), searchOption);
                directories.AddRange(folders);
            }

            // Return the folders.
            return directories.ToArray();
        }

        /// <summary>
        /// Get all files in the source folder.
        /// </summary>
        /// <param name="sourceFolder">The source folder to search in.</param>
        /// <param name="searchPattern">The search pattern filter.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>The list of all files including sub-folders.</returns>
        public string[] GetFiles(string sourceFolder, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<string> files = new List<string>();
            string[] patterns = searchPattern.Split(new char[] { ';' });

            foreach (string pattern in patterns)
            {
                // Get the collection of files.
                string[] file = System.IO.Directory.GetFiles(sourceFolder, pattern.Trim(), searchOption);
                files.AddRange(file);
            }

            // Return the files.
            return files.ToArray();
        }
    }
}
