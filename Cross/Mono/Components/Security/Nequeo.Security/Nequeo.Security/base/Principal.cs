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

namespace Nequeo.Security
{
    /// <summary>
    /// Represents the principal of a member. 
    /// </summary>
    public class PrincipalMember : IPrincipal
    {
        /// <summary>
        /// Represents the principal of a member.
        /// </summary>
        public PrincipalMember()
        {
        }

        /// <summary>
        /// Represents the principal of a member.
        /// </summary>
        /// <param name="identity">Represents the identity of a member.</param>
        /// <param name="roles">The collection of roles the identity is a member of.</param>
        public PrincipalMember(IIdentity identity, string[] roles)
		{
            _identity = identity;
            _roles = roles;
		}

        private IIdentity _identity = null;
        private string[] _roles = null;

        /// <summary>
        /// Gets or sets the identity of the current principal.
        /// </summary>
        public IIdentity Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }

        /// <summary>
        /// Gets or sets the roles for the current principal.
        /// </summary>
        public string[] Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
        public bool IsInRole(string role)
        {
            if (_roles != null)
            {
                return _roles.Contains(role);
            }
            else
                return false;
        }
    }

    /// <summary>
    /// Represents the Identity of a member. 
    /// </summary>
    public class IdentityMember : IIdentity
    {
        /// <summary>
        /// Represents the Identity of a member.
        /// </summary>
        public IdentityMember()
        {
        }

        /// <summary>
        /// Represents the Identity of a member.
        /// </summary>
        /// <param name="name">The unique identifier of the current user.</param>
        /// <param name="password">The password of the current user.</param>
        /// <param name="domain">The domain of the current user.</param>
        /// <param name="applicationName">The application name of the current user; else (All).</param>
        /// <param name="authenticationSchemes">The type of authentication schemes used.</param>
        public IdentityMember(string name, string password, string domain = "", string applicationName = "All",
            Nequeo.Security.AuthenticationType authenticationSchemes = Nequeo.Security.AuthenticationType.None)
		{
            _uniqueIdentifier = name;
            _password = password;
            _domain = domain;
            _applicationName = applicationName;
            _authenticationSchemes = authenticationSchemes;
		}

        private string _password = null;
        private string _domain = null;
        private string _applicationName = "All";
        private string _email = null;
        private string _fullName = null;
        private string _uniqueIdentifier = null;
        private bool _isAuthenticated = false;
        private Nequeo.Security.AuthenticationType _authenticationSchemes = Nequeo.Security.AuthenticationType.None;
        private Nequeo.Security.PermissionType _permission = Nequeo.Security.PermissionType.None;
        
        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        public string AuthenticationType
        {
            get { return _authenticationSchemes.ToString(); }
        }

        /// <summary>
        /// Gets or sets the type of authentication schemes used.
        /// </summary>
        public Nequeo.Security.AuthenticationType AuthenticationSchemes
        {
            get { return _authenticationSchemes; }
            set { _authenticationSchemes = value; }
        }

        /// <summary>
        /// Gets or sets the permission levels.
        /// </summary>
        public Nequeo.Security.PermissionType Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user has been authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set { _isAuthenticated = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the current user.
        /// </summary>
        public string Name
        {
            get { return _uniqueIdentifier; }
            set { _uniqueIdentifier = value; }
        }

        /// <summary>
        /// Gets or sets the full name of the current user.
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        /// <summary>
        /// Gets or sets the email address of the current user.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Gets or sets the domain of the current user.
        /// </summary>
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        /// <summary>
        /// Gets or sets the application name of the current user; else (All).
        /// </summary>
        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        /// <summary>
        /// Gets or sets the password of the current user.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Get the current user credentials.
        /// </summary>
        /// <returns>The user credentials.</returns>
        public Nequeo.Security.UserCredentials GetCredentials()
        {
            Nequeo.Security.UserCredentials credentials = new UserCredentials();
            credentials.ApplicationName = _applicationName;
            credentials.AuthenticationType = _authenticationSchemes;
            credentials.AuthorisationType = AuthorisationType.None;
            credentials.Domain = _domain;
            credentials.Password = _password;
            credentials.SecurePassword = new SecureText().GetSecureText(_password);
            credentials.Username = _uniqueIdentifier;
            credentials.Permission = _permission;
            return credentials;
        }
    }
}
