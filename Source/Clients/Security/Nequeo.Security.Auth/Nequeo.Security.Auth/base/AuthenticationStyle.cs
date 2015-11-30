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

namespace Nequeo.Security.Auth
{
    /// <summary>
    /// Authentication style.
    /// </summary>
    public enum AuthenticationStyle
    {
        /// <summary>
        /// Basic authentication.
        /// </summary>
        BasicAuthentication = 0,
        /// <summary>
        /// Post values.
        /// </summary>
        PostValues = 1,
        /// <summary>
        /// None.
        /// </summary>
        None = 2
    }

    /// <summary>
    /// Authentication style helper.
    /// </summary>
    internal class AuthenticationStyleHelper
    {
        /// <summary>
        /// Get Nequeo Authentication Style.
        /// </summary>
        /// <param name="style">Client style.</param>
        /// <returns>The style.</returns>
        public static AuthenticationStyle GetNequeoAuthenticationStyle(Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle style)
        {
            switch(style)
            {
                case Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle.BasicAuthentication:
                    return AuthenticationStyle.BasicAuthentication;
                case Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle.PostValues:
                    return AuthenticationStyle.PostValues;
                case Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle.None:
                default:
                    return AuthenticationStyle.None;
            }
        }

        /// <summary>
        /// Get Authentication Style.
        /// </summary>
        /// <param name="style">Client style.</param>
        /// <returns>The style.</returns>
        public static Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle GetAuthenticationStyle(AuthenticationStyle style)
        {
            switch (style)
            {
                case AuthenticationStyle.BasicAuthentication:
                    return Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle.BasicAuthentication;
                case AuthenticationStyle.PostValues:
                    return Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle.PostValues;
                case AuthenticationStyle.None:
                default:
                    return Thinktecture.IdentityModel.Client.OAuth2Client.ClientAuthenticationStyle.None;
            }
        }
    }
}
