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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// If registration is desired, normally there should
    /// be at least one credential specified, to successfully authenticate
    /// against the service provider. More credentials can be specified, for
    /// example when the requests are expected to be challenged by the
    /// proxies in the route set.
    /// </summary>
    public class AuthenticateCredentials
    {
        /// <summary>
        /// Gets or sets the authenticate credentials.
        /// </summary>
        public AuthCredInfo[] AuthCredentials { get; set; }

        /// <summary>
        /// Get the credential list from the auth credetial info.
        /// </summary>
        /// <returns>The credetials</returns>
        internal pjsua2.AuthCredInfoVector GetAuthCreds()
        {
            pjsua2.AuthCredInfoVector authCredVector = new pjsua2.AuthCredInfoVector();

            // If credetials exist.
            if (AuthCredentials != null && AuthCredentials.Length > 0)
            {
                // For each credetial.
                for (int i = 0; i < AuthCredentials.Length; i++)
                {
                    // Create the credetial.
                    AuthCredInfo current = AuthCredentials[i];
                    pjsua2.AuthCredInfo authCredInfo = new pjsua2.AuthCredInfo(current.Scheme, current.Realm, current.Username, current.DataType, current.Data);

                    // Add the credential to the list.
                    authCredVector.Add(authCredInfo);
                }
            }

            // Return the credentials.
            return authCredVector;
        }
    }

    /// <summary>
    /// Credential information. Credential contains information to authenticate against a service.
    /// </summary>
    public class AuthCredInfo
    {
        /// <summary>
        /// Credential information. Credential contains information to authenticate against a service.
        /// </summary>
        public AuthCredInfo() { }

        /// <summary>
        /// Credential information. Credential contains information to authenticate against a service.
        /// </summary>
        /// <param name="username">The sip username.</param>
        /// <param name="password">The sip password.</param>
        /// <param name="scheme">The authentication scheme (e.g. "digest").</param>
        /// <param name="realm">Realm on which this credential is to be used. Use "*" to make a credential that can be used to authenticate against any challenges.</param>
        /// <param name="dataType">Type of data that is contained in the "data" field. Use 0 if the data contains plain text password.</param>
        public AuthCredInfo(string username, string password, string scheme = "plain", string realm = "*", int dataType = 0)
        {
            Username = username;
            Data = password;
            Scheme = scheme;
            Realm = realm;
            DataType = dataType;
        }

        /// <summary>
        /// Gets or sets the the data, which can be a plain text password or a hashed digest.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the type of data that is contained in the "data" field. Use 0 if the data
        /// contains plain text password.
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Gets or sets the Realm on which this credential is to be used. Use "*" to make
        /// a credential that can be used to authenticate against any challenges.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Gets or sets the authentication scheme (e.g. "digest").
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// Gets or sets the authentication user name.
        /// </summary>
        public string Username { get; set; }
    }
}
