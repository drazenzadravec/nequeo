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
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace Nequeo.Data.DataType
{
    /// <summary>
    /// Custom membership provider.
    /// </summary>
    public interface IMembershipProvider
    {
        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets sets the provider name.
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// Gets sets enable password reset.
        /// </summary>
        bool EnablePasswordReset { get; set; }

        /// <summary>
        /// Gets sets enable password retrieval.
        /// </summary>
        bool EnablePasswordRetrieval { get; set; }

        /// <summary>
        /// Gets sets maximum invalid password attempts.
        /// </summary>
        int MaxInvalidPasswordAttempts { get; set; }

        /// <summary>
        /// Gets sets minimum required non alphanumeric characters
        /// </summary>
        int MinRequiredNonAlphanumericCharacters { get; set; }

        /// <summary>
        /// Gets sets minimum required password length.
        /// </summary>
        int MinRequiredPasswordLength { get; set; }

        /// <summary>
        /// Gets sets password attempt window.
        /// </summary>
        int PasswordAttemptWindow { get; set; }

        /// <summary>
        /// Gets sets password format.
        /// </summary>
        MembershipPasswordFormat PasswordFormat { get; set; }

        /// <summary>
        /// Gets sets password strength regular expression.
        /// </summary>
        string PasswordStrengthRegularExpression { get; set; }

        /// <summary>
        /// Gets sets requires question and answer.
        /// </summary>
        bool RequiresQuestionAndAnswer { get; set; }

        /// <summary>
        /// Gets sets requires unique email.
        /// </summary>
        bool RequiresUniqueEmail { get; set; }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if complete; else false.</returns>
        bool ChangePassword(string username, string oldPassword, string newPassword);

        /// <summary>
        /// Change password question and answer.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="newPasswordQuestion">The new password question.</param>
        /// <param name="newPasswordAnswer">The new password question.</param>
        /// <returns>True if complete; else false.</returns>
        bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer);

        /// <summary>
        /// Create user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email address.</param>
        /// <param name="passwordQuestion">The password question.</param>
        /// <param name="passwordAnswer">The password answer.</param>
        /// <param name="isApproved">Is approved.</param>
        /// <param name="providerUserKey">The provider key.</param>
        /// <param name="status">The status.</param>
        /// <returns>The membership user.</returns>
        MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status);

        /// <summary>
        /// Delete the user
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="deleteAllRelatedData">Delete all related data.</param>
        /// <returns>True if complete; else false.</returns>
        bool DeleteUser(string username, bool deleteAllRelatedData);

        /// <summary>
        /// Find users by email.
        /// </summary>
        /// <param name="emailToMatch">The email to match.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find users by name.
        /// </summary>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Get the number of users online.
        /// </summary>
        /// <returns>The number of users online.</returns>
        int GetNumberOfUsersOnline();

        /// <summary>
        /// Get password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>The password.</returns>
        string GetPassword(string username, string answer);

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userIsOnline">Is the user online.</param>
        /// <returns>The membership user.</returns>
        MembershipUser GetUser(string username, bool userIsOnline);

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="providerUserKey">Provider user key.</param>
        /// <param name="userIsOnline">Is the user online.</param>
        /// <returns>The membership user.</returns>
        MembershipUser GetUser(object providerUserKey, bool userIsOnline);

        /// <summary>
        /// Get username by email.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The username.</returns>
        string GetUserNameByEmail(string email);

        /// <summary>
        /// Reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>The new password.</returns>
        string ResetPassword(string username, string answer);

        /// <summary>
        /// Unlock the user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <returns>True if complete; else false.</returns>
        bool UnlockUser(string userName);

        /// <summary>
        /// Update the user.
        /// </summary>
        /// <param name="user">The membership user.</param>
        void UpdateUser(MembershipUser user);

        /// <summary>
        /// Validate the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if complete; else false.</returns>
        bool ValidateUser(string username, string password);

    }
}
