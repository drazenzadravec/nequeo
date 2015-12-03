/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Data.Provider
{
    /// <summary>
    /// Token provider.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Create a new token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void Create(string uniqueIdentifier, string serviceName, Action<string, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Get the current token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void Get(string uniqueIdentifier, string serviceName, Action<string, Security.IPermission, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Delete the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void Delete(string uniqueIdentifier, string serviceName, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Update the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="token">The token that will replace the cuurent token.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void Update(string uniqueIdentifier, string serviceName, string token, Action<bool, object> callback, string actionName = "", object state = null);

        /// <summary>
        /// Is the token valid
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="token">The token to validate.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        void IsValid(string uniqueIdentifier, string serviceName, string token, Action<bool, Security.IPermission, object> callback, string actionName = "", object state = null);
    }
}
