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
using System.Data;
using System.Security.Permissions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace Nequeo.Web.Common
{
    /// <summary>
    /// Member info type used to store users currently connected.
    /// </summary>
    [Serializable()]
    public class MemberInfo
    {
        #region Member Info
        private bool _isAnonymousUser = false;
        private bool _hasAnonymousID = false;
        private string _userIdentityName = string.Empty;
        private string _uniqueHashcode = string.Empty;

        /// <summary>
        /// Gets sets, is the user identity an anonymous connection.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool IsAnonymousUser
        {
            get { return _isAnonymousUser; }
            internal set { _isAnonymousUser = value; }
        }

        /// <summary>
        /// Gets sets, is the user identity an anonymous id.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool HasAnonymousID
        {
            get { return _hasAnonymousID; }
            internal set { _hasAnonymousID = value; }
        }

        /// <summary>
        /// Gets sets, the user identity name currently authenticated.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UserIdentityName
        {
            get { return _userIdentityName; }
            internal set { _userIdentityName = value; }
        }

        /// <summary>
        /// Gets sets, the hash code generated from the user identity.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UniqueHashcode
        {
            get { return _uniqueHashcode; }
            internal set { _uniqueHashcode = value; }
        }
        #endregion
    }

    /// <summary>
    /// User info type used to store current user data.
    /// </summary>
    [Serializable()]
    public class UserInfo
    {
        #region User Info
        private string _userID = string.Empty;
        private DateTime _lastActiveDate;
        private DateTime _creationDate;
        private Object _userData = null;

        /// <summary>
        /// Gets sets, the unique userid for the current session user.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        /// <summary>
        /// Gets sets, the last active date time for the user.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public DateTime LastActiveDate
        {
            get { return _lastActiveDate; }
            set { _lastActiveDate = value; }
        }

        /// <summary>
        /// Gets sets, the creation date time of the user.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; }
        }

        /// <summary>
        /// Gets sets, the object containing user information.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public Object UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }
        #endregion
    }
}
