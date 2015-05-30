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
    #region Public Delegates
    /// <summary>
    /// The sender to client authenticate error delegate transport.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void AuthenticateErrorHandler(Object sender, AuthenticateErrorArgs e);

    /// <summary>
    /// The sender to client authenticate delegate transport.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void AuthenticateHandler(Object sender, AuthenticateArgs e);
    #endregion

    /// <summary>
    /// Windows authentication class.
    /// </summary>
    public class WindowsAuthentication : Nequeo.Security.IAuthorisationProvider, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public WindowsAuthentication()
        {
        }
        #endregion

        #region Private Constants
        private const int LOGON32_LOGON_NETWORK = 3;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        #endregion

        #region Private Fields
        private bool disposed = false;
        #endregion

        #region Private API Calls
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr Handle);

        [DllImport("advapi32.dll")]
        private static extern int LogonUser(String UserName, String Domain, String Password,
            int LogonType, int LogonProvider, ref IntPtr WindowToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int FormatMessage(int Flags, ref IntPtr MessageSource, int MessageID,
            int LanguageID, ref IntPtr Buffer, int BufferSize, int Arguments);
        #endregion

        #region Public Events
        /// <summary>
        /// Public authentication error event for the
        /// client through the delegate.
        /// </summary>
        public event AuthenticateErrorHandler OnError;

        /// <summary>
        /// Public authentication event for the
        /// client through the delegate.
        /// </summary>
        public event AuthenticateHandler OnAuthenticate;
        #endregion

        #region Public Methods
        /// <summary>
        /// Authenticate user credentials.
        /// </summary>
        /// <param name="userCredentials">The user credentials.</param>
        /// <returns>True if authenticated; else false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool AuthenticateUser(Security.UserCredentials userCredentials)
        {
            return AuthenticateUser(
                userCredentials.Username,
                userCredentials.Domain,
                userCredentials.Password);
        }

        /// <summary>
        /// Authenticate a user whin the domain.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <param name="domain">The domain where the account is held.</param>
        /// <param name="password">The password for the account.</param>
        /// <returns>True if the Authentication was successful else false.</returns>
        public virtual bool AuthenticateUser(String username, String domain, String password)
        {
            try
            {
                IntPtr windowsToken = IntPtr.Zero;

                // Attempt to authenticate a user.
                int logonRetVal = LogonUser(username, domain, password,
                    LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, ref windowsToken);

                // If the return value is not zero then
                // authentication was successful.
                if (logonRetVal != 0)
                {
                    // Return the windows identity token.
                    WindowsIdentity newWI = new WindowsIdentity(windowsToken);
                    CloseHandle(windowsToken);

                    // Send to the client a notification that
                    // authentication was successful through
                    // the delegate.
                    if (OnAuthenticate != null)
                        OnAuthenticate(this, new AuthenticateArgs(newWI.IsGuest, newWI.IsAnonymous,
                            newWI.IsAuthenticated, newWI.User, newWI.Owner, username, domain, password));

                    // Return successful authentication.
                    return true;
                }
                else
                {
                    // Throw an exception if authentication failed.
                    throw new Exception("\nError occurred when logging on the user. \nError number: " +
                        GetLastError() + ". \nError message: " + CreateLogonUserError(GetLastError()));
                }
            }
            catch (Exception ex)
            {
                // Send to the client a notification that
                // authentication was not successful through 
                // the delegate.
                if (OnError != null)
                    OnError(this, new AuthenticateErrorArgs(ex.Message, username, domain, password));

                return false;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the error message if authentication fails.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>The formatted error message.</returns>
        private String CreateLogonUserError(int errorCode)
        {
            IntPtr bufferPtr = IntPtr.Zero;
            IntPtr messageSource = IntPtr.Zero;

            // Formatt the error from the authentication attempt.
            int retVal = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER,
                ref messageSource, errorCode, 0, ref bufferPtr, 1, 0);

            // Return the message from the buffer.
            string strRetval = Marshal.PtrToStringAuto(bufferPtr, retVal);
            return strRetval;
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WindowsAuthentication()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Authenticate error arguments class, used when a logon
    /// has been authenticated. This is the client event.
    /// </summary>
    public class AuthenticateErrorArgs
    {
        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="errorMessage">The error message when authenticating.</param>
        /// <param name="username">The username of the account.</param>
        /// <param name="domain">The domain where the account is held.</param>
        /// <param name="password">The password of the account.</param>
        public AuthenticateErrorArgs(String errorMessage, String username,
            String domain, String password)
        {
            this.errorMessage = errorMessage;
            this.username = username;
            this.password = password;
            this.domain = domain;
        }
        #endregion

        #region Private Fields
        private String errorMessage = string.Empty;
        private String username = string.Empty;
        private String password = string.Empty;
        private String domain = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, The username of the account.
        /// </summary>
        public String UserName
        {
            get { return username; }
        }

        /// <summary>
        /// Get, The password of the account.
        /// </summary>
        public String Password
        {
            get { return password; }
        }


        /// <summary>
        /// Get, The domain where the account is held.
        /// </summary>
        public String Domain
        {
            get { return domain; }
        }

        /// <summary>
        /// Get, The error message when authenticating.
        /// </summary>
        public String ErrorMessage
        {
            get { return errorMessage; }
        }
        #endregion
    }

    /// <summary>
    /// Authenticate arguments class, used when a logon
    /// has been authenticated. This is the client event.
    /// </summary>
    public class AuthenticateArgs
    {
        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="isGuest">Get a value indicating if the user is an 
        /// guest account.</param>
        /// <param name="isAnonymous">Get a value indicating if the user 
        /// is an anonymous account.</param>
        /// <param name="isAuthenticated">Get a value indicating if the user is 
        /// authenticated as a valid account.</param>
        /// <param name="user">Get the security identifier (SID) for the user.</param>
        /// <param name="owner">Get the security identifier (SID) for the token owner.</param>
        /// <param name="username">The username of the account.</param>
        /// <param name="domain">The domain where the account is held.</param>
        /// <param name="password">The password of the account.</param>
        public AuthenticateArgs(bool isGuest, bool isAnonymous, bool isAuthenticated,
            SecurityIdentifier user, SecurityIdentifier owner, String username,
            String domain, String password)
        {
            this.isGuest = isGuest;
            this.isAnonymous = isAnonymous;
            this.isAuthenticated = isAuthenticated;
            this.user = user;
            this.owner = owner;
            this.username = username;
            this.password = password;
            this.domain = domain;
        }
        #endregion

        #region Private Fields
        private String username = string.Empty;
        private String password = string.Empty;
        private String domain = string.Empty;
        private bool isGuest = false;
        private bool isAnonymous = false;
        private bool isAuthenticated = false;
        private SecurityIdentifier user = null;
        private SecurityIdentifier owner = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, The username of the account.
        /// </summary>
        public String UserName
        {
            get { return username; }
        }

        /// <summary>
        /// Get, The password of the account.
        /// </summary>
        public String Password
        {
            get { return password; }
        }

        /// <summary>
        /// Get, The domain where the account is held.
        /// </summary>
        public String Domain
        {
            get { return domain; }
        }

        /// <summary>
        /// Get a value indicating if the user
        /// is authenticated as a valid account.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
        }

        /// <summary>
        /// Get a value indicating if the user
        /// is an guest account.
        /// </summary>
        public bool IsGuest
        {
            get { return isGuest; }
        }

        /// <summary>
        /// Get a value indicating if the user
        /// is an anonymous account.
        /// </summary>
        public bool IsAnonymous
        {
            get { return isAnonymous; }
        }

        /// <summary>
        /// Get the security identifier (SID) for the token owner.
        /// </summary>
        public SecurityIdentifier Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Get the security identifier (SID) for the user.
        /// </summary>
        public SecurityIdentifier User
        {
            get { return user; }
        }
        #endregion
    }
}
