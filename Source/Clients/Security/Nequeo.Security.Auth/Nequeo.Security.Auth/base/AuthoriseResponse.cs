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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Client;

namespace Nequeo.Security.Auth
{
    /// <summary>
    /// Authorise response.
    /// </summary>
    public class AuthoriseResponse
    {
        /// <summary>
        /// Response type.
        /// </summary>
        public enum ResponseTypes
        {
            /// <summary>
            /// Authorization code.
            /// </summary>
            AuthorizationCode,
            /// <summary>
            /// Token.
            /// </summary>
            Token,
            /// <summary>
            /// Form post.
            /// </summary>
            FormPost,
            /// <summary>
            /// Error.
            /// </summary>
            Error
        };

        /// <summary>
        /// Authorise response.
        /// </summary>
        /// <param name="url">The raw url response string.</param>
        public AuthoriseResponse(string url)
        {
            Raw = url;
            Values = new Dictionary<string, string>();
            ParseRaw();
        }

        /// <summary>
        /// Gets response type.
        /// </summary>
        public ResponseTypes ResponseType { get; protected set; }

        /// <summary>
        /// Gets raw url string.
        /// </summary>
        public string Raw { get; protected set; }

        /// <summary>
        /// Gets the vaues.
        /// </summary>
        public Dictionary<string, string> Values { get; protected set; }

        /// <summary>
        /// Get auth code.
        /// </summary>
        public string Code
        {
            get
            {
                return TryGet(OAuth2Constants.Code);
            }
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        public string AccessToken
        {
            get
            {
                return TryGet(OAuth2Constants.AccessToken);
            }
        }

        /// <summary>
        /// Gets the identity token.
        /// </summary>
        public string IdentityToken
        {
            get
            {
                return TryGet(OAuth2Constants.IdentityToken);
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error
        {
            get
            {
                return TryGet(OAuth2Constants.Error);
            }
        }

        /// <summary>
        /// Gets the expires in value.
        /// </summary>
        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OAuth2Constants.ExpiresIn);

                long longValue = 0;
                long.TryParse(value, out longValue);

                return longValue;
            }
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public string Scope
        {
            get
            {
                return TryGet(OAuth2Constants.Scope);
            }
        }

        /// <summary>
        /// Gets the token type.
        /// </summary>
        public string TokenType
        {
            get
            {
                return TryGet(OAuth2Constants.TokenType);
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public string State
        {
            get
            {
                return TryGet(OAuth2Constants.State);
            }
        }

        /// <summary>
        /// Parse raw data.
        /// </summary>
        private void ParseRaw()
        {
            var queryParameters = new Dictionary<string, string>();
            string[] fragments = null;

            // fragment encoded
            if (Raw.Contains("#"))
            {
                fragments = Raw.Split('#');
                ResponseType = ResponseTypes.Token;
            }
            // query string encoded
            else if (Raw.Contains("?"))
            {
                fragments = Raw.Split('?');
                ResponseType = ResponseTypes.AuthorizationCode;
            }
            // form encoded
            else
            {
                fragments = new string[] { "", Raw };
                ResponseType = ResponseTypes.FormPost;
            }

            if (Raw.Contains(OAuth2Constants.Error))
            {
                ResponseType = ResponseTypes.Error;
            }

            var qparams = fragments[1].Split('&');

            foreach (var param in qparams)
            {
                var parts = param.Split('=');

                if (parts.Length == 2)
                {
                    Values.Add(parts[0], parts[1]);
                }
                else
                {
                    throw new InvalidOperationException("Malformed callback URL.");
                }
            }
        }

        /// <summary>
        /// Try get the url value.
        /// </summary>
        /// <param name="type">The type to get.</param>
        /// <returns>The url value.</returns>
        private string TryGet(string type)
        {
            string value;
            if (Values.TryGetValue(type, out value))
            {
                return value;
            }

            return null;
        }
    }
}
