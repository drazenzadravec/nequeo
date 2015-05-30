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
using System.Collections.Generic;

using Nequeo.Handler;
using Nequeo.Cryptography;
using Nequeo.Web.Common;
using Nequeo.Caching;

namespace Nequeo.Web
{
    /// <summary>
    /// Controls the caching and session state of all users currently online.
    /// </summary>
    [Serializable()]
    public class UserOnlineController
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="isActive">Is the user session enabled.</param>
        /// <param name="userIdentityName">The user identity name for the current user.</param>
        /// <param name="userSessionData">The session state data for the user.</param>
        public UserOnlineController(bool isActive, string userIdentityName, Object userSessionData)
		{
            if (userIdentityName == null) throw new ArgumentNullException("userIdentityName");

            _isActive = isActive;
            _userSessionData = userSessionData;
            _userIdentityName = userIdentityName;
            _uniqueHashcode = Hashcode.GetHashcode(userIdentityName, HashcodeType.SHA1);
        }
        #endregion

        #region Private Fields
        private bool _isActive = false;
        private Object _userSessionData = null;
        private string _uniqueHashcode = string.Empty;
        private string _userIdentityName = string.Empty;
        #endregion

        #region Private Constants
        private const string OnlineUserListKey = "OnlineUserList";
        private const string ApplicationName = "Nequeo";
        private const string EventNamespace = "Nequeo.Web";
        #endregion

        #region Session Properties
        /// <summary>
        /// Gets, is the user session enabled.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool IsActive
        {
            get { return _isActive; }
        }

        /// <summary>
        /// Gets, the hash code generated from the user identity name.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UniqueHashcode
        {
            get { return _uniqueHashcode; }
        }

        /// <summary>
        /// Gets, the user identity name for the current user.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UserIdentityName
        {
            get { return _userIdentityName; }
        }

        /// <summary>
        /// Gets, the session state data for the user.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public Object UserSessionData
        {
            get { return _userSessionData; }
        }
        #endregion

        #region Public Static Session Methods
        /// <summary>
        /// Set the user session enabled state.
        /// </summary>
        /// <param name="isActive">Is the user session enabled.</param>
        public static void SetIsActive(bool isActive)
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            // Create a new instance and assign the new data.
            UserOnlineController.CurrentUser =
                    new UserOnlineController(isActive,
                            UserOnlineController.CurrentUser.UserIdentityName, 
                            UserOnlineController.CurrentUser.UserSessionData);
        }

        /// <summary>
        /// Set the user session state data.
        /// </summary>
        /// <param name="userSessionData">The user session state data.</param>
        public static void SetUserSessionData(Object userSessionData)
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            // Create a new instance and assign the new data.
            UserOnlineController.CurrentUser =
                    new UserOnlineController(
                            UserOnlineController.CurrentUser.IsActive,
                            UserOnlineController.CurrentUser.UserIdentityName,
                            userSessionData);
        }

        /// <summary>
        /// Create a new instance of the user session controller
        /// </summary>
        /// <returns>The new instance else null.</returns>
        public static UserOnlineController CreateInstance()
        {
            // Get the current user identity name.
            UserOnlineController userOnlineController = null;
            string userIdentityName = HttpContext.Current.User.Identity.Name;

            // If the identity is null then the
            // user is anonymous
            if (String.IsNullOrEmpty(userIdentityName))
            {
                // Get the anonymous id assigned
                userIdentityName = HttpContext.Current.Request.AnonymousID;

                // If no anonymous id has been assigned
                // the throw exception.
                if (String.IsNullOrEmpty(userIdentityName)) {
                    throw new Exception("The current http context user identity is anonymous, unable to crate a online user controller.");}
                else {
                    // Assign the new controller with the anonymous id.
                    userOnlineController = new UserOnlineController(true, userIdentityName, null); }
            }
            else {
                // Assign the new controller with the user identity name.
                userOnlineController = new UserOnlineController(true, userIdentityName, null); }

            // Return the new user controller instance.
            return userOnlineController;
        }
        #endregion

        #region Public Static Session Controller
        /// <summary>
        /// Gets sets, the curent session state controller
        /// </summary>
        [XmlIgnore]
        public static UserOnlineController CurrentUser
        {
            // Get the current user session data.
            get { return HttpContext.Current.Session["UserOnlineController"] as UserOnlineController; }

            // the current user session data.
            set { HttpContext.Current.Session["UserOnlineController"] = value; }
        }
        #endregion

        #region Public Session State and Cache Methods
        /// <summary>
        /// Release the current session state data from memory.
        /// </summary>
        public static void ReleaseAll()
        {
            if (UserOnlineController.CurrentUser != null)
            {
                // Cleanup the cuurrent session state data
                // from the cache.
                MemberInfo memberInfo = new MemberInfo();
                memberInfo.UniqueHashcode = UserOnlineController.CurrentUser.UniqueHashcode;
                memberInfo.UserIdentityName = UserOnlineController.CurrentUser.UserIdentityName;
                RuntimeCache.Remove(UserOnlineController.CurrentUser.UniqueHashcode);
                RemoveUserFromCache(memberInfo);
            }

            // Cleanup the current session state.
            UserOnlineController.CurrentUser = null;
        }

        /// <summary>
        /// Get the current member data from the cache.
        /// </summary>
        /// <returns>The member information for the current session.</returns>
        public static MemberInfo GetMemberInfo()
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            MemberInfo memberInfo = ((MemberInfo)RuntimeCache.Get(UserOnlineController.CurrentUser.UniqueHashcode));
            return memberInfo;
        }

        /// <summary>
        /// Get the current user data from the cache.
        /// </summary>
        /// <returns>The user information for the cuurent session.</returns>
        public static UserInfo GetUserInfo()
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            UserInfo userInfo = ((UserInfo)RuntimeCache.Get(UserOnlineController.CurrentUser.UniqueHashcode));
            return userInfo;
        }

        /// <summary>
        /// Add the user info item to the cache.
        /// </summary>
        /// <param name="userInfo">The user info containing the data.</param>
        public static void SetUserInfo(UserInfo userInfo)
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            MemberInfo memberInfo = new MemberInfo();
            memberInfo.UniqueHashcode = UserOnlineController.CurrentUser.UniqueHashcode;
            memberInfo.UserIdentityName = UserOnlineController.CurrentUser.UserIdentityName;
            AddUserToCache(memberInfo, userInfo);
        }

        /// <summary>
        /// Get the user identity name.
        /// </summary>
        /// <returns>The hash code generated from the user identiy name.</returns>
        public static string GetUniqueHashcode()
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            return UserOnlineController.CurrentUser.UniqueHashcode;
        }

        /// <summary>
        /// Get the user identity name
        /// </summary>
        /// <returns>The user identity name.</returns>
        public static string GetUserIdentityName()
        {
            if (UserOnlineController.CurrentUser == null)
                throw new Exception("No current session data has been set.");

            return UserOnlineController.CurrentUser.UserIdentityName;
        }
        #endregion

        #region Internal Static Cache Control Methods
        /// <summary>
        /// Add the item to the runtime cache
        /// </summary>
        /// <param name="memberInfo">The member info type containing the data.</param>
        /// <param name="userInfo">The user info type containing the data.</param>
        internal static void AddUserToCache(MemberInfo memberInfo, UserInfo userInfo)
        {
            IHandler handler = new LogHandler(ApplicationName, EventNamespace);
            double cacheTimeOut = (double)handler.BaseHandlerConfigurationReader.MembershipCacheTimeOut;
            RuntimeCache.Add(memberInfo.UniqueHashcode, userInfo, cacheTimeOut);
        }

        /// <summary>
        /// Remove the item from the runtime cache.
        /// </summary>
        /// <param name="memberInfo">The member info type containing the data.</param>
        internal static void RemoveUserFromCache(MemberInfo memberInfo)
        {
            RuntimeCache.Remove(memberInfo.UniqueHashcode);
        }
        #endregion
    }
}
