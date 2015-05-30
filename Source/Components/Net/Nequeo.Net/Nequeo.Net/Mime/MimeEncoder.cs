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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Reflection;

namespace Nequeo.Net.Mime
{
    /// <summary>
    /// Mime encoding utility.
    /// </summary>
    public class MimeEncoder
    {
        /// <summary>
        /// Get the transfer encoding string value.
        /// </summary>
        /// <param name="transferEncoding">The transfer encoding.</param>
        /// <returns>The transfer encoding string.</returns>
        public static string GetTransferEncoding(System.Net.Mime.TransferEncoding transferEncoding)
        {
            switch (transferEncoding)
            {
                case System.Net.Mime.TransferEncoding.Base64:
                    return "base64";
                case System.Net.Mime.TransferEncoding.EightBit:
                    return "8bit";
                case System.Net.Mime.TransferEncoding.QuotedPrintable:
                    return "quoted-printable";
                case System.Net.Mime.TransferEncoding.SevenBit:
                    return "7bit";
                case System.Net.Mime.TransferEncoding.Unknown:
                default:
                    return "unknown";
            }
        }

        /// <summary>
        /// Get the transfer encoding.
        /// </summary>
        /// <param name="transferEncoding">The transfer encoding string value.</param>
        /// <returns>The transfer encoding.</returns>
        public static System.Net.Mime.TransferEncoding GetTransferEncoding(string transferEncoding)
        {
            switch (transferEncoding.ToLower())
            { 
                case "base64":
                    return System.Net.Mime.TransferEncoding.Base64;
                case "8bit":
                    return System.Net.Mime.TransferEncoding.EightBit;
                case "quoted-printable":
                    return System.Net.Mime.TransferEncoding.QuotedPrintable;
                case "7bit":
                    return System.Net.Mime.TransferEncoding.SevenBit;
                case "unknown":
                default:
                    return System.Net.Mime.TransferEncoding.Unknown;
            }
        }
    }
}
