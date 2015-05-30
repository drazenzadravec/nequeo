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
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Net;

namespace Nequeo.Security
{
    /// <summary>
    /// Used to over ride the client validation hehavior, 
    /// when set the client validation default behavior is over written.
    /// </summary>
    /// <param name="user">Defines the basic functionality of a principal object.</param>
    /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
    /// <returns>True if the client has been validated; else false.</returns>
    public delegate Boolean ClientAuthenticationValidationSelector(IPrincipal user, Nequeo.Security.AuthenticationType authenticationSchemes);

    /// <summary>
    /// Used to over ride the authentication hehavior, 
    /// when set the authentication default behavior is over written.
    /// </summary>
    /// <returns>The specifies protocols for client authentication.</returns>
    public delegate Nequeo.Security.AuthenticationType ClientAuthenticationSchemesSelector();

    /// <summary>
    /// Authentication base provider.
    /// </summary>
    public abstract class Authentication : IAuthenticationProvider
    {
        /// <summary>
        /// Authentication base provider.
        /// </summary>
        public Authentication()
        {
        }

        private Nequeo.Security.AuthenticationType _authenticationSchemes = Nequeo.Security.AuthenticationType.None;
        private ClientAuthenticationValidationSelector _clientAuthenticationValidator = null;
        private ClientAuthenticationSchemesSelector _clientAuthenticationSchemes = null;

        /// <summary>
        /// Gets or sets the client authentication validator. Used to over ride the ClientValidation method
        /// hehavior, when set the ClientValidation method default behavior is over written.
        /// </summary>
        public ClientAuthenticationValidationSelector ClientAuthenticationValidator
        {
            get { return _clientAuthenticationValidator; }
            set { _clientAuthenticationValidator = value; }
        }

        /// <summary>
        /// Gets or sets the client authentication schemes. Used to over ride the AuthenticationSchemeForClient method
        /// hehavior, when set the AuthenticationSchemeForClient method default behavior is over written.
        /// </summary>
        public ClientAuthenticationSchemesSelector ClientAuthenticationSchemes
        {
            get { return _clientAuthenticationSchemes; }
            set { _clientAuthenticationSchemes = value; }
        }

        /// <summary>
        /// Gets the Authentication Schemes for the client.
        /// </summary>
        public Nequeo.Security.AuthenticationType AuthenticationSchemes
        {
            get { return _authenticationSchemes; }
            set { _authenticationSchemes = value; }
        }

        /// <summary>
        /// Get specifies protocols for authentication.
        /// </summary>
        /// <returns>Specifies protocols for authentication.</returns>
        public virtual Nequeo.Security.AuthenticationType AuthenticationSchemeForClient()
        {
            if (_clientAuthenticationSchemes != null)
            {
                _authenticationSchemes = _clientAuthenticationSchemes();

                // Return the schema.
                return _authenticationSchemes;
            }
            else
                // Return the schema.
                return _authenticationSchemes;
        }

        /// <summary>
        /// Validate the client. Client is only validated if IsAuthenticated = true; all other is false.
        /// Use the ClientAuthenticationValidationSelector delegate to over ride this behavior. Or override this
        /// method in a derived class to change the behavior
        /// </summary>
        /// <param name="user">Defines the basic functionality of a principal object.</param>
        /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        public virtual Boolean ClientValidation(IPrincipal user, Nequeo.Security.AuthenticationType authenticationSchemes)
        {
            // Does the user priciple exist.
            if (user != null)
            {
                // Does the user identity exist.
                if (user.Identity != null)
                {
                    // If the client was validated.
                    if (!user.Identity.IsAuthenticated)
                        return false;
                    else
                    {
                        if (_clientAuthenticationValidator != null)
                        {
                            // Custom client authentication validator.
                            return _clientAuthenticationValidator(user, authenticationSchemes);
                        }
                        else
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
