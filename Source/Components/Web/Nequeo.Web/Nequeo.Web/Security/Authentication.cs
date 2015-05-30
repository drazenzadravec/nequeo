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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Design;
using System.Web.Compilation;
using System.Web.Security;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;

namespace Nequeo.Web.Security
{
    /// <summary>
    /// Service for forms authentication.
    /// </summary>
    public class FormsAuthenticationService
    {
        /// <summary>
        /// Create the form authenticate cookie.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="createPersistentCookie">Create a persistent cookie</param>
        public static void SignIn(string userName, bool createPersistentCookie = false)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");

            // Create the form authenticate cookie.
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// Create the form authenticate cookie, and redirect from the login page to the requested resource.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="createPersistentCookie">Create a persistent cookie</param>
        public static void SignInRedirect(string userName, bool createPersistentCookie = false)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");

            // Create the form authenticate cookie.
            FormsAuthentication.RedirectFromLoginPage(userName, createPersistentCookie);
        }

        /// <summary>
        /// Redirect to user to the login page.
        /// </summary>
        public static void RedirectToLogin()
        {
            // Create the form authenticate cookie.
            FormsAuthentication.RedirectToLoginPage();
        }

        /// <summary>
        /// Get the form authenticate cookie.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="createPersistentCookie">Create a persistent cookie</param>
        /// <returns>The form authentication cookie.</returns>
        public static HttpCookie RedirectToLogin(string userName, bool createPersistentCookie = false)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");

            // Create the form authenticate cookie.
            return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// Reomve the form authenticate cookie.
        /// </summary>
        public static void SignOut()
        {
            // Sign the user out from the application.
            FormsAuthentication.SignOut();
        }
    }

    /// <summary>
    /// Interface to handle membership accounts.
    /// </summary>
    public interface IMembershipService
    {
        /// <summary>
        /// Gets, the minimum password length.
        /// </summary>
        int MinPasswordLength { get; }

        /// <summary>
        /// Validate the user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The user password.</param>
        /// <returns>True if validated else false.</returns>
        bool ValidateUser(string userName, string password);

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The user password.</param>
        /// <param name="email">The user email address.</param>
        /// <returns>The create user status.</returns>
        MembershipCreateStatus CreateUser(string userName, string password, string email);

        /// <summary>
        /// Change the user password.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="oldPassword">The user old password.</param>
        /// <param name="newPassword">The user new password.</param>
        /// <returns>True if the password was changed else false.</returns>
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    /// <summary>
    /// Membership service provider.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MembershipService()
            : this(null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The custom membership provider.</param>
        public MembershipService(MembershipProvider provider)
        {
            // Assign the current membership provider.
            _provider = provider ?? Membership.Provider;
        }

        /// <summary>
        /// Gets, the minimum password length.
        /// </summary>
        public int MinPasswordLength
        {
            get
            {
                // Get the MinRequiredPasswordLength from the current provider.
                return _provider.MinRequiredPasswordLength;
            }
        }

        /// <summary>
        /// Validate the user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The user password.</param>
        /// <returns>True if validated else false.</returns>
        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            // Validate the user with the current provider.
            return _provider.ValidateUser(userName, password);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The user password.</param>
        /// <param name="email">The user email address.</param>
        /// <returns>The create user status.</returns>
        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            MembershipCreateStatus status;

            // Create the new user using the current provider.
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        /// <summary>
        /// Change the user password.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="oldPassword">The user old password.</param>
        /// <param name="newPassword">The user new password.</param>
        /// <returns>True if the password was changed else false.</returns>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                // Get the curent user info using the current provider.
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }
}
