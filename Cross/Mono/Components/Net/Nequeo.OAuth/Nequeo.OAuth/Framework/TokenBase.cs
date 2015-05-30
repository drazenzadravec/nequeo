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
using System.Text;
using System.Web;

using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Framework
{
    /// <summary>
    /// Token base
    /// </summary>
    [Serializable]
    public class TokenBase : IToken
    {
        /// <summary>
        /// Gets sets the token secret.
        /// </summary>
        public string TokenSecret { get; set; }

        /// <summary>
        /// Gets sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets sets the session handler.
        /// </summary>
        public string SessionHandle { get; set; }

        /// <summary>
        /// Gets sets the comsumer key.
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets sets the realm.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Over ride the to string method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return UriUtility.FormatTokenForResponse(this);
        }
    }
}
