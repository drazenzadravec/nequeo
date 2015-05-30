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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Web;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Security.Principal;

namespace Nequeo.Net.Security
{
    /// <summary>
    /// Authentication.
    /// </summary>
    public class Authentication : Nequeo.Security.Authentication
    {
        /// <summary>
        /// Authentication.
        /// </summary>
        /// <param name="provider">The authorisation provider use to authorisation the user.</param>
        public Authentication(Nequeo.Security.IAuthorisationProvider provider)
        {
            _provider = provider;
        }

        private Nequeo.Security.IAuthorisationProvider _provider = null;
        private HttpContext _httpContext = null;
        private HttpListenerContext _httpListenerContext = null;
        private Nequeo.Net.WebContext _webContext = null;

        /// <summary>
        /// Gets or sets the http context.
        /// </summary>
        public HttpContext HttpContext
        {
            get { return _httpContext; }
            set { _httpContext = value; }
        }

        /// <summary>
        /// Gets or sets the http listener context.
        /// </summary>
        public HttpListenerContext HttpListenerContext
        {
            get { return _httpListenerContext; }
            set { _httpListenerContext = value; }
        }

        /// <summary>
        /// Gets or sets the web context.
        /// </summary>
        public Nequeo.Net.WebContext WebContext
        {
            get { return _webContext; }
            set { _webContext = value; }
        }

        /// <summary>
        /// Get specifies protocols for authentication.
        /// </summary>
        /// <returns>Specifies protocols for authentication.</returns>
        public override Nequeo.Security.AuthenticationType AuthenticationSchemeForClient()
        {
            // If the HttpListenerContext
            if (_httpListenerContext != null)
            {
                // Do not authenticate local machine requests.
                if (_httpListenerContext.Request.RemoteEndPoint.Address.Equals(IPAddress.Loopback))
                    base.AuthenticationSchemes = Nequeo.Security.AuthenticationType.None;
            }
            else if (_httpContext != null)
            {
                // Do not authenticate local machine requests.
                if (IPAddress.Parse(_httpContext.Request.UserHostAddress).Equals(IPAddress.Loopback))
                    base.AuthenticationSchemes = Nequeo.Security.AuthenticationType.None;
            }
            else if (_webContext != null)
            {
                // Do not authenticate local machine requests.
                if (_webContext.RemoteEndPoint.Address.Equals(IPAddress.Loopback))
                    base.AuthenticationSchemes = Nequeo.Security.AuthenticationType.None;
            }

            // Return the schema.
            return base.AuthenticationSchemes;
        }

        /// <summary>
        /// Validate the current client.
        /// </summary>
        /// <param name="user">The current user principal.</param>
        /// <param name="authenticationSchemes">The authentication type.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        public override bool ClientValidation(System.Security.Principal.IPrincipal user, Nequeo.Security.AuthenticationType authenticationSchemes)
        {
            // Does the user priciple exist.
            if (user != null)
            {
                // Does the user identity exist.
                if (user.Identity != null)
                {
                    // Select the curent Authentication Schemes
                    switch (authenticationSchemes)
                    {
                        case Nequeo.Security.AuthenticationType.User:
                        case Nequeo.Security.AuthenticationType.Basic:
                            // If the authentication type is 'Basic'

                            // If the identity is IIdentityMember.
                            if (user.Identity is Nequeo.Security.IdentityMember)
                            {
                                // The username and password are passed for
                                // Basic authentication type.
                                Nequeo.Security.IdentityMember identityMember = (Nequeo.Security.IdentityMember)user.Identity;

                                // Return the result of the authentication.
                                return _provider.AuthenticateUser(identityMember.GetCredentials());
                            }

                            // If the identity is HttpListenerBasicIdentity.
                            if (user.Identity is HttpListenerBasicIdentity)
                            {
                                // The username and password are passed for
                                // Basic authentication type.
                                HttpListenerBasicIdentity httpListenerBasicIdentity = (HttpListenerBasicIdentity)user.Identity;
                                string userName = httpListenerBasicIdentity.Name;
                                string password = httpListenerBasicIdentity.Password;

                                // Create the user credentials.
                                Nequeo.Security.UserCredentials credentials = new Nequeo.Security.UserCredentials();
                                credentials.Username = userName;
                                credentials.Password = password;

                                // Return the result of the authentication.
                                return _provider.AuthenticateUser(credentials);
                            }
                            return false;

                        case Nequeo.Security.AuthenticationType.Integrated:
                            // If the authentication type is WindowsIdentity
                            if (user.Identity is System.Security.Principal.WindowsIdentity)
                            {
                                WindowsIdentity windowsIdentity = (WindowsIdentity)user.Identity;
                                if (user.IsInRole("Administrators") || user.IsInRole("Users"))
                                    return true;
                                else
                                    return false;
                            }
                            break;

                        case Nequeo.Security.AuthenticationType.None:
                        case Nequeo.Security.AuthenticationType.Anonymous:
                            return true;

                        default:
                            return false;
                    }
                }
            }
            return false;
        }
    }
}

