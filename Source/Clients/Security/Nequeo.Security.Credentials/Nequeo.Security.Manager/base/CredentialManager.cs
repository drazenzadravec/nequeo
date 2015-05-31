/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

using NequeoSecurity;

namespace Nequeo.Security
{
    /// <summary>
    /// Credential manager provider.
    /// </summary>
	public class CredentialManager
	{
        /// <summary>
        /// Gets the credentials for the current target name.
        /// </summary>
        /// <param name="targetName">The target name to filter by.</param>
        /// <returns>The list of credentials for the target name filter.</returns>
        public List<Credential> GetCredentials(string targetName)
        {
            List<Credential> col = new List<Credential>();

            // Create a new credential set for the target name.
            using (CredentialSet credentials = new CredentialSet(targetName))
            {
                // For each target name found.
                foreach (Credential credential in credentials)
                    col.Add(credential);
            }

            // Return the collection of credentials.
            return col;
        }

        /// <summary>
        /// Gets the credentials for the target name and type.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="type">The credential type.</param>
        /// <returns>The credentials; else null.</returns>
        public Credential GetCredential(string targetName, CredentialType type)
        {
            Credential credential = null;
            
            try
            {
                // Load the credentials.
                credential = new Credential(targetName, type);
                credential.Load();
                return credential;
            }
            catch (Exception)
            {
                if (credential != null)
                    credential.Dispose();

                throw;
            }
        }

        /// <summary>
        /// Does the credentials exist for the target name and type.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="type">The credential type.</param>
        /// <returns>True if the credential exists; else false.</returns>
        public bool CredentialsExist(string targetName, CredentialType type)
        {
            return Credential.Exists(targetName, type);
        }

        /// <summary>
        /// Delete the credentials for the target and type.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="type">The credential type.</param>
        /// <returns>True if deleted; else false.</returns>
        public bool DeleteCredentials(string targetName, CredentialType type)
        {
            Credential credential = null;

            try
            {
                // Load the credentials.
                credential = new Credential(targetName, type);
                credential.Delete();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (credential != null)
                    credential.Dispose();
            }
        }

        /// <summary>
        /// Change the credential target name.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="type">The credential type.</param>
        /// <param name="newTargetName">The new target name.</param>
        /// <returns>True if changed; else false.</returns>
        public bool ChangeCredentialTargetName(string targetName, CredentialType type, string newTargetName)
        {
            Credential credential = null;

            try
            {
                // Load the credentials.
                credential = new Credential(targetName, type);
                credential.ChangeTargetName(newTargetName);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (credential != null)
                    credential.Dispose();
            }
        }

        /// <summary>
        /// Add credentials.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="description">The description.</param>
        /// <param name="type">The credential type.</param>
        /// <param name="persistence">The credential persistence.</param>
        /// <returns>True if the credential was added; else false.</returns>
        public bool AddCredentials(string targetName, string username, string password, 
            string description, CredentialType type, CredentialPersistence persistence)
        {
            Credential credential = null;

            try
            {
                // If the credentials already exist.
                if (Credential.Exists(targetName, type))
                    throw new Exception("Credentials with the given target name and type already exist.");

                // Construct the secure string password.
                SecureString passwordSecure = new SecureString();

                // Append the secure password for each character.
                foreach (char element in password)
                    passwordSecure.AppendChar(element);

                // Create the new credentials
                credential = new Credential(targetName, type, username, passwordSecure, persistence, description);

                // Save the credentials to the store.
                credential.Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (credential != null)
                    credential.Dispose();
            }
        }

        /// <summary>
        /// Try to get the secure password from the password string.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The secure string password.</returns>
        public SecureString TryGetSecurePassword(string password)
        {
            // Construct the secure string password.
            SecureString passwordSecure = new SecureString();

            // Append the secure password for each character.
            foreach (char element in password)
                passwordSecure.AppendChar(element);

            // Return the secure password.
            return passwordSecure;
        }

        /// <summary>
        /// Try to get the password from the credential secure password.
        /// </summary>
        /// <param name="credential">The credentials.</param>
        /// <returns>The password; else null.</returns>
        public string TryGetPassword(Credential credential)
        {
            string password = null;
            IntPtr pointerPassword = IntPtr.Zero;

            try
            {
                // Get the pointer to where the secure password is stored.
                // Convert the pointer password to the password string.
                pointerPassword = Marshal.SecureStringToBSTR(credential.Password);
                password = Marshal.PtrToStringBSTR(pointerPassword);
            }
            finally
            {
                if (IntPtr.Zero != pointerPassword)
                    Marshal.FreeBSTR(pointerPassword);
            }

            // Return the password.
            return password;
        }

        /// <summary>
        /// Show the prompt for credentials dialog window.
        /// </summary>
        /// <param name="options">The prompt options.</param>
        /// <returns>The prompt credential result: else null;</returns>
        public CredentialDialogResult ShowPromptForCredentialsDialog(CredentialDialogOptions options)
        {
            return ShowPromptForCredentialsDialog(null, options);
        }

        /// <summary>
        /// Show the prompt for credentials dialog window.
        /// </summary>
        /// <param name="owner">The windows form prompt dialog owner.</param>
        /// <param name="options">The prompt options.</param>
        /// <returns>The prompt credential result: else null;</returns>
        public CredentialDialogResult ShowPromptForCredentialsDialog(System.Windows.Forms.IWin32Window owner, CredentialDialogOptions options)
        {
            CredentialDialogResult result = null;

            try
            {
                // Create a new credential dialog window.
                using (PromptForCredential prompt = new PromptForCredential())
                {
                    prompt.TargetName = options.TargetName;
                    prompt.UserName = options.UserName;

                    prompt.Password = new SecureString();
                    foreach (char element in options.Password)
                        prompt.Password.AppendChar(element);

                    prompt.Title = options.Title;
                    prompt.Message = options.Message;
                    prompt.ErrorCode = options.ErrorCode;

                    if (!String.IsNullOrEmpty(options.Banner))
                        prompt.Banner = new Bitmap(options.Banner);

                    prompt.SaveChecked = options.SaveChecked;
                    prompt.AlwaysShowUI = options.AlwaysShowUI;
                    prompt.CompleteUserName = options.CompleteUserName;
                    prompt.DoNotPersist = options.DoNotPersist;
                    prompt.ExcludeCertificates = options.ExcludeCertificates;
                    prompt.ExpectConfirmation = options.ExpectConfirmation;
                    prompt.GenericCredentials = options.GenericCredentials;
                    prompt.IncorrectPassword = options.IncorrectPassword;
                    prompt.Persist = options.Persist;
                    prompt.RequestAdministrator = options.RequestAdministrator;
                    prompt.RequireCertificate = options.RequireCertificate;
                    prompt.RequireSmartCard = options.RequireSmartCard;
                    prompt.ShowSaveCheckBox = options.ShowSaveCheckBox;
                    prompt.UserNameReadOnly = options.UserNameReadOnly;
                    prompt.ValidateUserName = options.ValidateUserName;

                    DialogResult dialogResult;
                    if (owner == null)
                        dialogResult = prompt.ShowDialog();
                    else
                        dialogResult = prompt.ShowDialog(owner);

                    // Show the dialog.
                    if (DialogResult.OK == dialogResult)
                    {
                        // Create the result.
                        result = new CredentialDialogResult();

                        result.SaveChecked = prompt.SaveChecked;
                        result.UserName = prompt.UserName;

                        IntPtr passwordBstr = IntPtr.Zero;

                        try
                        {
                            // Convert the password pointer data.
                            passwordBstr = Marshal.SecureStringToBSTR(prompt.Password);
                            result.Password = Marshal.PtrToStringBSTR(passwordBstr);
                        }
                        finally
                        {
                            if (IntPtr.Zero != passwordBstr)
                                Marshal.FreeBSTR(passwordBstr);
                        }

                        // Confirm the credentials.
                        if (prompt.ExpectConfirmation && prompt.SaveChecked)
                            prompt.ConfirmCredentials();
                    }
                }

                // Return the result.
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
	}
}
