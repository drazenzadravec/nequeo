/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.com.au/
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Data.Enum;
using Nequeo.Data;
using Nequeo.Data.Provider;
using Nequeo.Xml.Authorisation.Configuration;

namespace Nequeo.Xml.Authorisation.Token
{
    /// <summary>
    /// Xml token provider.
    /// </summary>
    public class Provider : ITokenProvider
    {
        /// <summary>
        /// Xml token provider.
        /// </summary>
        public Provider()
        {
        }

        /// <summary>
        /// Xml token provider.
        /// </summary>
        /// <param name="tokenXmlPath">The token xml file path.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Provider(string tokenXmlPath)
        {
            if (String.IsNullOrEmpty(tokenXmlPath)) throw new ArgumentNullException("tokenXmlPath");
            
            // Load the Communication data.
            TokenReader.TokenXmlPath = tokenXmlPath;
        }

        private Action<string, Exception> _callback_Exception = null;
        private Nequeo.Xml.Authorisation.Token.Data.context _tokenData = null;

        /// <summary>
        /// Gets or sets the error callbak handler.
        /// </summary>
        public Action<string, Exception> ExceptionCallback
        {
            get { return _callback_Exception; }
            set { _callback_Exception = value; }
        }

        /// <summary>
        /// Commit provider data to the store.
        /// </summary>
        public void Commit()
        {
            // Save the data to the store.
            TokenReader.SaveTokenData(_tokenData);
        }

        /// <summary>
        /// Load provider data from the store.
        /// </summary>
        public void Load()
        {
            // Load the Communication data.
            _tokenData = TokenReader.LoadTokenData();
        }

        /// <summary>
        /// Create a new token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void Create(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var tokenData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return TokenReader.Create(uniqueIdentifier, serviceName, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("Create", exec);
            }
            if (callback != null)
                callback(tokenData, state);
        }

        /// <summary>
        /// Delete the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void Delete(string uniqueIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var tokenData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return TokenReader.Delete(uniqueIdentifier, serviceName, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("Delete", exec);
            }
            if (callback != null)
                callback(tokenData, state);
        }

        /// <summary>
        /// Get the current token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void Get(string uniqueIdentifier, string serviceName, Action<string, Nequeo.Security.IPermission, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var tokenData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<string>(() =>
                {
                    try
                    {
                        return TokenReader.Get(uniqueIdentifier, serviceName, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            var permission = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<Nequeo.Security.IPermission>(() =>
                {
                    try
                    {
                        return TokenReader.GetPermission(uniqueIdentifier, serviceName, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("Get", exec);
            }
            if (callback != null)
                callback(tokenData, permission, state);
        }

        /// <summary>
        /// Is the token valid
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="token">The token to validate.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void IsValid(string uniqueIdentifier, string serviceName, string token, Action<bool, Nequeo.Security.IPermission, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var tokenData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return TokenReader.IsValid(uniqueIdentifier, serviceName, token, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            var permission = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<Nequeo.Security.IPermission>(() =>
                {
                    try
                    {
                        return TokenReader.GetPermission(uniqueIdentifier, serviceName, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return null;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("IsValid", exec);
            }
            if (callback != null)
                callback(tokenData, permission, state);
        }

        /// <summary>
        /// Update the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="token">The token that will replace the cuurent token.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async void Update(string uniqueIdentifier, string serviceName, string token, Action<bool, object> callback, string actionName = "", object state = null)
        {
            Exception exec = null;
            var tokenData = await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask<bool>(() =>
                {
                    try
                    {
                        return TokenReader.Update(uniqueIdentifier, serviceName, token, _tokenData);
                    }
                    catch (Exception ex)
                    {
                        exec = ex;
                        return false;
                    }
                });

            if (exec != null)
            {
                if (_callback_Exception != null)
                    _callback_Exception("Update", exec);
            }
            if (callback != null)
                callback(tokenData, state);
        }
    }
}
