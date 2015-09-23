/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// Dialog result.
    /// </summary>
    public enum DialogResult : int
    {
        /// <summary>
        /// Abort.
        /// </summary>
        Abort = 1,
        /// <summary>
        /// Cancel.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// Ignore.
        /// </summary>
        Ignore = 3,
        /// <summary>
        /// No.
        /// </summary>
        No = 4,
        /// <summary>
        /// None.
        /// </summary>
        None = 5,
        /// <summary>
        /// OK.
        /// </summary>
        OK = 6,
        /// <summary>
        /// Retry.
        /// </summary>
        Retry = 7,
        /// <summary>
        /// Yes.
        /// </summary>
        Yes = 8
    }

    /// <summary>
    /// Special folders.
    /// </summary>
    public enum SpecialFolders : int
    {
        /// <summary>
        /// Desktop
        /// </summary>
        Desktop = 0,
        /// <summary>
        /// Documents
        /// </summary>
        Documents = 1,
        /// <summary>
        /// Pictures
        /// </summary>
        Pictures = 2,
        /// <summary>
        /// Videos
        /// </summary>
        Videos = 3,
        /// <summary>
        /// Computer
        /// </summary>
        Computer = 4,
        /// <summary>
        /// Music
        /// </summary>
        Music = 5,
        /// <summary>
        /// StartMenu
        /// </summary>
        StartMenu = 6,
        /// <summary>
        /// ProgramFiles
        /// </summary>
        ProgramFiles = 7,
        /// <summary>
        /// ProgramFilesX86
        /// </summary>
        ProgramFilesX86 = 8
    }
}
