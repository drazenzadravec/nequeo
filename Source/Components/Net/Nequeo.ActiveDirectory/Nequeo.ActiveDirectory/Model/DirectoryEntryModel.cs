/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          DomainDirectoryClient.cs
 *  Purpose :       This file contains classes that can be
 *                  used to access the domain machine
 *                  directory service.
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
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Nequeo.Net.ActiveDirectory.Model
{
    /// <summary>
    /// Directory entry model details.
    /// </summary>
    public class DirectoryEntryModel
    {
        /// <summary>
        /// Gets sets, the account username
        /// </summary>
        public string AccountUserName { get; set; }

        /// <summary>
        /// Gets sets, the full name/ display name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets sets, the user first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets sets, the user last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets sets, the description of the account
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets sets, the email address of the account
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets sets, the password of the account
        /// </summary>
        public string Password { get; set; }

    }
}
