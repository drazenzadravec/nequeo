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

namespace Nequeo.Net.OAuth
{
    /// <summary>
    /// OAuth common errors.
    /// </summary>
    public class OAuthError
    {
        /// <summary>
        /// Invalid request.
        /// </summary>
        public const string InvalidRequest = "invalid_request";

        /// <summary>
        /// Unauthorized client.
        /// </summary>
        public const string UnauthorizedClient = "unauthorized_client";

        /// <summary>
        /// Access denied.
        /// </summary>
        public const string AccessDenied = "access_denied";

        /// <summary>
        /// Unsupported response type.
        /// </summary>
        public const string UnsupportedResponseType = "unsupported_response_type";

        /// <summary>
        /// Invalid scope.
        /// </summary>
        public const string InvalidScope = "invalid_scope";

        /// <summary>
        /// Invalid client.
        /// </summary>
        public const string InvalidClient = "invalid_client";

        /// <summary>
        /// Unsupported grant type.
        /// </summary>
        public const string UnsupportedGrantType = "unsupported_grant_type";
    }
}
