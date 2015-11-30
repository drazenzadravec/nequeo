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
    /// Common communication client helper.
    /// </summary>
    public class CommunicateClient : CommunicateBase
    {
        /// <summary>
        /// Common communication host helper.
        /// </summary>
        /// <param name="communicationClient">The communication client used to authorise clients.</param>
        public CommunicateClient(Nequeo.Client.Manage.ICommunicationClient communicationClient)
            : base(communicationClient)
        {
            if (communicationClient == null) throw new ArgumentNullException("communicationClient");

            _communicationClient = communicationClient;
            base.IsCommunicationServer = false;
        }

        /// <summary>
        /// The client used to connection to the communication server, this is used to
        /// authorise a connection and communication any client connections.
        /// </summary>
        private Nequeo.Client.Manage.ICommunicationClient _communicationClient = null;

        /// <summary>
        /// Initialise communication client, attempts to connect to the communication server.
        /// </summary>
        /// <param name="useConfiguration">Is a configuration file used.</param>
        /// <param name="username">The authorise username credentials.</param>
        /// <param name="password">The authorise password credentials.</param>
        /// <param name="domain">The authorise domain credentials.</param>
        public void InitialiseCommClient(bool useConfiguration, string username = null, string password = null, string domain = null)
        {
            try
            {
                // If not connected
                if (!_communicationClient.Connected)
                {
                    // Connect to the authentication server.
                    _communicationClient.Initialisation(useConfiguration);
                    _communicationClient.Connect();
                    _communicationClient.AuthoriseConnection(username, password, domain);

                    // If not connected
                    if (!_communicationClient.Connected)
                        throw new Exception("Unable to connect to the remote communication server.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
