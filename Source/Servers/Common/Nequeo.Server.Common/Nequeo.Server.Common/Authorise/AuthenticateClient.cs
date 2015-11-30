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
using System.Threading.Tasks;

namespace Nequeo.Server.Authorise
{
    /// <summary>
    /// Common authentication client helper.
    /// </summary>
    public class AuthenticateClient : AuthenticateBase
    {
        /// <summary>
        /// Common authentication host helper.
        /// </summary>
        /// <param name="authenticationClient">The authentication client used to authorise clients.</param>
        public AuthenticateClient(Nequeo.Client.Manage.IAuthenticationClient authenticationClient)
            : base(authenticationClient)
        {
            if (authenticationClient == null) throw new ArgumentNullException("authenticationClient");

            _authenticationClient = authenticationClient;
            base.IsAuthenticationServer = false;
        }

        /// <summary>
        /// The client used to connection to the authentication server, this is used to
        /// authorise a connection and authentication any client connections.
        /// </summary>
        private Nequeo.Client.Manage.IAuthenticationClient _authenticationClient = null;

        /// <summary>
        /// Initialise authentication client, attempts to connect to the authentication server.
        /// </summary>
        /// <param name="useConfiguration">Is a configuration file used.</param>
        /// <param name="username">The authorise username credentials.</param>
        /// <param name="password">The authorise password credentials.</param>
        /// <param name="domain">The authorise domain credentials.</param>
        public void InitialiseAuthClient(bool useConfiguration, string username = null, string password = null, string domain = null)
        {
            try
            {
                // If not connected
                if (!_authenticationClient.Connected)
                {
                    // Connect to the authentication server.
                    _authenticationClient.Initialisation(useConfiguration);
                    _authenticationClient.Connect();
                    _authenticationClient.AuthoriseConnection(username, password, domain);

                    // If not connected
                    if (!_authenticationClient.Connected)
                        throw new Exception("Unable to connect to the remote authentication server.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
