/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security;
using System.Net;
using System.Security.Authentication;

namespace Nequeo.Security
{
    /// <summary>
    /// Security authorisation provider.
    /// </summary>
    public interface IAuthorisationProvider
    {
        /// <summary>
        /// Authenticate user credentials.
        /// </summary>
        /// <param name="userCredentials">The user credentials.</param>
        /// <returns>True if authenticated; else false.</returns>
        bool AuthenticateUser(Nequeo.Security.UserCredentials userCredentials);
    }

    /// <summary>
    /// Authentication base provider.
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Gets or sets the client authentication validator. Used to over ride the ClientValidation method
        /// hehavior, when set the ClientValidation method default behavior is over written.
        /// </summary>
        ClientAuthenticationValidationSelector ClientAuthenticationValidator { get; set; }

        /// <summary>
        /// Gets or sets the client authentication schemes. Used to over ride the AuthenticationSchemeForClient method
        /// hehavior, when set the AuthenticationSchemeForClient method default behavior is over written.
        /// </summary>
        ClientAuthenticationSchemesSelector ClientAuthenticationSchemes { get; set; }

        /// <summary>
        /// Gets the Authentication Schemes for the client.
        /// </summary>
        Nequeo.Security.AuthenticationType AuthenticationSchemes { get; set; }

        /// <summary>
        /// Get specifies protocols for authentication.
        /// </summary>
        /// <returns>Specifies protocols for authentication.</returns>
        Nequeo.Security.AuthenticationType AuthenticationSchemeForClient();

        /// <summary>
        /// Validate the client. Client is only validated if IsAuthenticated = true; all other is false.
        /// Use the ClientAuthenticationValidationSelector delegate to over ride this behavior. Or override this
        /// method in a derived class to change the behavior
        /// </summary>
        /// <param name="user">Defines the basic functionality of a principal object.</param>
        /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        Boolean ClientValidation(IPrincipal user, Nequeo.Security.AuthenticationType authenticationSchemes);

    }
}
