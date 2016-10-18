/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Web;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nequeo.Web.Download.Common
{
    /// <summary>
    /// Token state object.
    /// </summary>
    internal class TokenState
    {
        /// <summary>
        /// Gets or sets an indicator if the token is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the user permission.
        /// </summary>
        public Nequeo.Security.IPermission Permission { get; set; }
    }

    /// <summary>
    /// Token provider
    /// </summary>
    internal class TokenProvider : Nequeo.Data.ITokenProvider
    {
        /// <summary>
        /// Validate the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="token">The token.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="actionName">The unique action name.</param>
        /// <param name="state">The object state.</param>
        public async void IsValid(string uniqueIdentifier, string serviceName, string token, Action<bool, Nequeo.Security.IPermission, object> callback, string actionName = "", object state = null)
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.RunTask<bool>(() =>
            {
                return false;
            });

            // Check the credentials.
            bool allow = false;
            if (!String.IsNullOrEmpty(uniqueIdentifier) && !String.IsNullOrEmpty(serviceName) && !String.IsNullOrEmpty(token))
            {
                // Make sure the credentials match.
                if (uniqueIdentifier.ToLower().Equals("drazen") && serviceName.ToLower().Equals(Common.Helper.ServiceName.ToLower()) && token.ToLower().Equals("allow"))
                {
                    // Allow access.
                    allow = true;
                }
            }

            // Send the result and permission to the client.
            if (callback != null)
                callback(allow,
                    new Nequeo.Security.PermissionSource(
                        Nequeo.Security.PermissionType.Download |
                        Nequeo.Security.PermissionType.Upload |
                        Nequeo.Security.PermissionType.List |
                        Nequeo.Security.PermissionType.Delete |
                        Nequeo.Security.PermissionType.Move |
                        Nequeo.Security.PermissionType.Rename |
                        Nequeo.Security.PermissionType.Create |
                        Nequeo.Security.PermissionType.Copy),
                        state);
        }

        #region Private Action Members
        /// <summary>
        /// 
        /// </summary>
        public Action<string, Exception> ExceptionCallback
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Create(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Delete(string uniqueIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Get(string uniqueIdentifier, string serviceName, Action<string, Nequeo.Security.IPermission, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="serviceName"></param>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        /// <param name="actionName"></param>
        /// <param name="state"></param>
        public void Update(string uniqueIdentifier, string serviceName, string token, Action<bool, object> callback, string actionName = "", object state = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}