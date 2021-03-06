﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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

using Nequeo.Data.Control;
using Nequeo.DataAccess.NequeoCompany;

namespace Nequeo.Logic.NequeoCompany.User
{
    /// <summary>
    /// Complete data User logic control
    /// </summary>
    public partial interface IUser
    {
        /// <summary>
        /// Gets the user edm extension implementation.
        /// </summary>
        EdmUserExtender EdmUserExtension { get; }

        /// <summary>
        /// Gets the user data extension implementation.
        /// </summary>
        DataUserExtender Extension { get; }

        /// <summary>
        /// Gets the user type data extension implementation.
        /// </summary>
        DataUserTypeExtender Extension1 { get; }
    }

    /// <summary>
    /// Complete data user logic control
    /// </summary>
    public partial class User : DataUserExtender, Data.Control.IExtension<DataUserExtender, DataUserTypeExtender>
    {
        /// <summary>
        /// Gets the user data extension implementation.
        /// </summary>
        public virtual DataUserExtender Extension
        {
            get { return new DataUserExtender(); }
        }

        /// <summary>
        /// Gets the user type data extension implementation.
        /// </summary>
        public virtual DataUserTypeExtender Extension1
        {
            get { return new DataUserTypeExtender(); }
        }

        /// <summary>
        /// Gets the user edm extension implementation.
        /// </summary>
        public virtual EdmUserExtender EdmUserExtension
        {
            get { return new EdmUserExtender(); }
        }

        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="loginUserName">The login username</param>
        /// <param name="loginPassword">The login password</param>
        /// <returns>The validated user; else null</returns>
        public virtual DataAccess.NequeoCompany.Edm.User ValidateUser1(string loginUserName, string loginPassword)
        {
            return EdmUserExtension.Validate(loginUserName, loginPassword);
        }

        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="loginUserName">The login username</param>
        /// <param name="loginPassword">The login password</param>
        /// <returns>The validated user; else null</returns>
        public virtual DataAccess.NequeoCompany.Data.User ValidateUser(string loginUserName, string loginPassword)
        {
            return Extension.Validate(loginUserName, loginPassword);
        }
    }
}
