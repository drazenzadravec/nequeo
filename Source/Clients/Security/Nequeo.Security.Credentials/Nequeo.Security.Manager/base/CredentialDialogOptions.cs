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

using NequeoSecurity;

namespace Nequeo.Security
{
    /// <summary>
    /// The prompt for credentials dialog options.
    /// </summary>
	public class CredentialDialogOptions
	{
        /// <summary>
        /// Gets or sets Always Show UI
        /// </summary>
        public bool AlwaysShowUI { get; set; }

        /// <summary>
        /// Gets or sets the full path and file name of the banner.
        /// </summary>
        public string Banner { get; set; }

        /// <summary>
        /// Gets or sets Complete User Name
        /// </summary>
        public bool CompleteUserName { get; set; }

        /// <summary>
        /// Gets or sets Do Not Persist
        /// </summary>
        public bool DoNotPersist { get; set; }

        /// <summary>
        /// Gets or sets Error Code
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets Exclude Certificates
        /// </summary>
        public bool ExcludeCertificates { get; set; }

        /// <summary>
        /// Gets or sets Expect Confirmation
        /// </summary>
        public bool ExpectConfirmation { get; set; }

        /// <summary>
        /// Gets or sets Generic Credentials
        /// </summary>
        public bool GenericCredentials { get; set; }

        /// <summary>
        /// Gets or sets Incorrect Password
        /// </summary>
        public bool IncorrectPassword { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Persist
        /// </summary>
        public bool Persist { get; set; }

        /// <summary>
        /// Gets or sets Request Administrator
        /// </summary>
        public bool RequestAdministrator { get; set; }

        /// <summary>
        /// Gets or sets Require Certificate
        /// </summary>
        public bool RequireCertificate { get; set; }

        /// <summary>
        /// Gets or sets Require Smart Card
        /// </summary>
        public bool RequireSmartCard { get; set; }

        /// <summary>
        /// Gets or sets Save Checked
        /// </summary>
        public bool SaveChecked { get; set; }

        /// <summary>
        /// Gets or sets Show Save Check Box
        /// </summary>
        public bool ShowSaveCheckBox { get; set; }

        /// <summary>
        /// Gets or sets Target Name
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets User Name Read Only
        /// </summary>
        public bool UserNameReadOnly { get; set; }

        /// <summary>
        /// Gets or sets Validate User Name
        /// </summary>
        public bool ValidateUserName { get; set; }

        /// <summary>
        /// Create the default credential dialog.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <returns>The credential dialog.</returns>
        public static CredentialDialogOptions Create(string targetName)
        {
            CredentialDialogOptions dialog = new CredentialDialogOptions();
            dialog.AlwaysShowUI = false;
            dialog.Banner = null;
            dialog.CompleteUserName = false;
            dialog.DoNotPersist = false;
            dialog.ErrorCode = 0;
            dialog.ExcludeCertificates = false;
            dialog.ExpectConfirmation = false;
            dialog.GenericCredentials = false;
            dialog.IncorrectPassword = false;
            dialog.Message = string.Empty;
            dialog.Password = string.Empty;
            dialog.Persist = false;
            dialog.RequestAdministrator = false;
            dialog.RequireCertificate = false;
            dialog.RequireSmartCard = false;
            dialog.SaveChecked = false;
            dialog.ShowSaveCheckBox = false;
            dialog.Title = string.Empty;
            dialog.UserName = string.Empty;
            dialog.UserNameReadOnly = false;
            dialog.ValidateUserName = false;
            dialog.TargetName = targetName;
            return dialog;
        }
	}
}
