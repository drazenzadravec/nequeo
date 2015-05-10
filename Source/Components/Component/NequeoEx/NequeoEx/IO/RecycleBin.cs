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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Nequeo.IO
{
    /// <summary>
    /// Struct which contains information that the SHFileOperation function uses to perform file operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEOPSTRUCT
    {
        /// <summary>
        /// Handler.
        /// </summary>
        public IntPtr hwnd;

        /// <summary>
        /// The window function.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public int wFunc;

        /// <summary>
        /// The from path.
        /// </summary>
        public string pFrom;

        /// <summary>
        /// The to path.
        /// </summary>
        public string pTo;

        /// <summary>
        /// The operation flags.
        /// </summary>
        public short fFlags;

        /// <summary>
        /// The abort operation.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool fAnyOperationsAborted;

        /// <summary>
        /// The name mapping.
        /// </summary>
        public IntPtr hNameMappings;

        /// <summary>
        /// The progress title.
        /// </summary>
        public string lpszProgressTitle;
    }

    /// <summary>
    /// Recycle bin helper.
    /// </summary>
    public class RecycleBin
    {
        private const int FO_DELETE = 0x0003;
        private const int FOF_ALLOWUNDO = 0x0040;           // Preserve undo information, if possible. 
        private const int FOF_NOCONFIRMATION = 0x0010;      // Show no confirmation dialog box to the user

        /// <summary>
        /// The shell operation to execute.
        /// </summary>
        /// <param name="FileOp"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        /// <summary>
        /// Send the file or folder to the recycle bin.
        /// </summary>
        /// <param name="path">The path of the file or folder.</param>
        public static void DeleteFileOrFolder(string path)
        {
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.wFunc = FO_DELETE;
            fileop.pFrom = path + '\0' + '\0';
            fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            SHFileOperation(ref fileop);
        }
    }
}
