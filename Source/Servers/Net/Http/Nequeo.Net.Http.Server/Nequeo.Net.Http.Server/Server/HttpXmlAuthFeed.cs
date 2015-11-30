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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Principal;

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Net.Configuration;
using Nequeo.ComponentModel.Composition;
using Nequeo.Composite.Configuration;

namespace Nequeo.Net.Server
{
    /// <summary>
    /// Http Xml authorisation feed server, user must authenticate to connect to this server.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class HttpXmlAuthFeed : HttpXmlFeed
    {
        /// <summary>
        /// Http html authorisation server
        /// </summary>
        /// <param name="uriList">The url prefix listening list</param>
        /// <param name="providerName">The provider name (host="xmlauthprovider") in the configuration file.</param>
        public HttpXmlAuthFeed(string[] uriList = null, string providerName = "xmlauthprovider")
            : base(uriList, providerName)
        {
            base.AuthenticationSchemeSelector = new AuthenticationSchemeSelector(AuthenticationSchemeForClient);
        }

        /// <summary>
        /// Get specifies protocols for authentication.
        /// </summary>
        /// <param name="request">Describes an incoming HTTP request to an 
        /// System.Net.HttpListener object. This class cannot be inherited.</param>
        /// <returns>Specifies protocols for authentication.</returns>
        private AuthenticationSchemes AuthenticationSchemeForClient(HttpListenerRequest request)
        {
            // Do not authenticate local machine requests.
            if (request.RemoteEndPoint.Address.Equals(IPAddress.Loopback))
                base.AuthenticationSchemes = System.Net.AuthenticationSchemes.None;
            else
                // Basic authentication is required.
                base.AuthenticationSchemes =
                    System.Net.AuthenticationSchemes.Basic | System.Net.AuthenticationSchemes.IntegratedWindowsAuthentication;

            // Return the schema.
            return base.AuthenticationSchemes;
        }

        /// <summary>
        /// Validate the client.
        /// </summary>
        /// <param name="user">Defines the basic functionality of a principal object.</param>
        /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        protected override bool ClientValidation(System.Security.Principal.IPrincipal user, AuthenticationSchemes authenticationSchemes)
        {
            // Does the user priciple exist.
            if (user != null)
            {
                // Does the user identity exist.
                if (user.Identity != null)
                {
                    // If the client was not validated.
                    if (!user.Identity.IsAuthenticated)
                        return false;
                    else
                    {
                        // Select the curent Authentication Schemes
                        switch (authenticationSchemes)
                        {
                            case System.Net.AuthenticationSchemes.Basic | System.Net.AuthenticationSchemes.IntegratedWindowsAuthentication:
                            case System.Net.AuthenticationSchemes.Basic:
                            case System.Net.AuthenticationSchemes.IntegratedWindowsAuthentication:
                                // Specifies Windows authentication. Is the user in the roles
                                // then the users has been valiadted.

                                // If the authentication type is 'IntegratedWindowsAuthentication'
                                if (user.Identity is System.Security.Principal.WindowsIdentity)
                                {
                                    WindowsIdentity windowsIdentity = (WindowsIdentity)user.Identity;
                                    if (user.IsInRole("Administrators") || user.IsInRole("Users"))
                                        return true;
                                    else
                                        return false;
                                }
                                // If the authentication type is 'Basic'
                                else if (user.Identity is HttpListenerBasicIdentity)
                                {
                                    // The username and password are passed for
                                    // Basic authentication type.
                                    HttpListenerBasicIdentity httpListenerBasicIdentity = (HttpListenerBasicIdentity)user.Identity;
                                    string userName = httpListenerBasicIdentity.Name;
                                    string password = httpListenerBasicIdentity.Password;
                                    return true;
                                }
                                else
                                    return false;

                            default:
                                return false;
                        }
                    }
                }
            }
            return false;
        }
    }
}
