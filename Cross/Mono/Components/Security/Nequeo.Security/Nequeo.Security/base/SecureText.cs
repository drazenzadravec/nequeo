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
using System.Threading;
using System.IO;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace Nequeo.Security
{
    /// <summary>
    /// Represents text that should be kept confidential. The text is encrypted for
    /// privacy when being used, and deleted from computer memory when no longer
    /// needed. This class cannot be inherited.
    /// </summary>
    public sealed class SecureText
    {
        /// <summary>
        /// Initializes a new instance of the SecureText class.
        /// </summary>
        public SecureText() { }

        /// <summary>
        /// Get the secure string from the text.
        /// </summary>
        /// <param name="text">The text to secure.</param>
        /// <returns>The secure string of the text.</returns>
        public SecureString GetSecureText(string text)
        {
            // Construct the secure string.
            SecureString textSecure = new SecureString();

            // Append the secure text for each character.
            foreach (char element in text)
                textSecure.AppendChar(element);

            // Return the secure text.
            return textSecure;
        }

        /// <summary>
        /// Get the text from the secure string.
        /// </summary>
        /// <param name="secureString">The secure string containing the text.</param>
        /// <returns>The extracted text.</returns>
        public string GetText(SecureString secureString)
        {
            string text = null;
            IntPtr pointerText = IntPtr.Zero;

            try
            {
                // Get the pointer to where the secure text is stored.
                // Convert the pointer text to the string.
                pointerText = Marshal.SecureStringToBSTR(secureString);
                text = Marshal.PtrToStringBSTR(pointerText);
            }
            finally
            {
                if (IntPtr.Zero != pointerText)
                    Marshal.FreeBSTR(pointerText);
            }

            // Return the text.
            return text;
        }
    }
}
