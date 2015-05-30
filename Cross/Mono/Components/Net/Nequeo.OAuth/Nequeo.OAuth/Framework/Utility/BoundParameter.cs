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
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// Bound parameter
    /// </summary>
    internal class BoundParameter
    {
        readonly OAuthContext _context;
        readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        public BoundParameter(string name, OAuthContext context)
        {
            _name = name;
            _context = context;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                if (_context.AuthorizationHeaderParameters[_name] != null)
                    return _context.AuthorizationHeaderParameters[_name];

                if (_context.QueryParameters[_name] != null)
                    return _context.QueryParameters[_name];

                if (_context.FormEncodedParameters[_name] != null)
                    return _context.FormEncodedParameters[_name];

                return null;
            }
            set
            {
                if (value == null)
                {
                    Collection.Remove(_name);
                }
                else
                {
                    Collection[_name] = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>The collection.</value>
        NameValueCollection Collection
        {
            get
            {
                if (_context.UseAuthorizationHeader)
                    return _context.AuthorizationHeaderParameters;

                if (_context.RequestMethod == "GET")
                    return _context.QueryParameters;

                return _context.FormEncodedParameters;
            }
        }
    }
}
