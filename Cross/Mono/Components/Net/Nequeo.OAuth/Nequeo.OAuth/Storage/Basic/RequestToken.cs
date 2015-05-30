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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Storage.Basic
{
    /// <summary>
    /// Simple request token model, this provides information about a request token which has been issued, including
    /// who it was issued to, if the token has been used up (a request token should only be presented once), and 
    /// the associated access token (if a user has granted access to a consumer i.e. given them access).
    /// </summary>
    public class RequestToken : TokenBase
    {
        /// <summary>
        /// Gets sets the access denied.
        /// </summary>
        public bool AccessDenied { get; set; }

        /// <summary>
        /// Gets sets the used up
        /// </summary>
        public bool UsedUp { get; set; }

        /// <summary>
        /// Gets sets the access token
        /// </summary>
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// Gets sets the callback url
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Gets sets the verifier
        /// </summary>
        public string Verifier { get; set; }

        /// <summary>
        /// Oner ride to string method.
        /// </summary>
        /// <returns>The new string.</returns>
        public override string ToString()
        {
            string formattedToken = base.ToString();

            formattedToken += "&" + Parameters.OAuth_Callback_Confirmed + "=true";

            return formattedToken;
        }
    }
}
