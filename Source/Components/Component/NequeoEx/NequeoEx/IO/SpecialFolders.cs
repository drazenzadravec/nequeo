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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Nequeo.IO
{
    /// <summary>
    /// Special folders.
    /// </summary>
    public class SpecialFolders
    {
        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration.
        /// </summary>
        /// <param name="folder">An enumerated constant that identifies a system special folder.</param>
        /// <returns>The path to the specified system special folder, if that folder physically
        /// exists on your computer; otherwise, an empty string ("").A folder will not
        /// physically exist if the operating system did not create it, the existing
        /// folder was deleted, or the folder is a virtual directory, such as My Computer,
        /// which does not correspond to a physical path.</returns>
        public static string GetFolderPath(Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration.
        /// </summary>
        /// <param name="folder">An enumerated constant that identifies a system special folder.</param>
        /// <param name="option">Specifies options to use for accessing a special folder.</param>
        /// <returns>The path to the specified system special folder, if that folder physically
        /// exists on your computer; otherwise, an empty string ("").A folder will not
        /// physically exist if the operating system did not create it, the existing
        /// folder was deleted, or the folder is a virtual directory, such as My Computer,
        /// which does not correspond to a physical path.</returns>
        public static string GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
        {
            return Environment.GetFolderPath(folder, option);
        }
    }
}
