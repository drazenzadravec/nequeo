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
using System.Collections.Generic;

using Nequeo.Data;
using Nequeo.Data.DataType;
using Nequeo.Handler;
using Nequeo.Data.Configuration;
using Nequeo.Web.Configuration;
using Nequeo.Web.Common;
using Nequeo.Invention;
using Nequeo.Cryptography;

namespace Nequeo.Web.HttpModule
{
    /// <summary>
    /// Current connection membership http module validation.
    /// </summary>
    public class Membership : IHttpModule
    {
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An HttpApplication that provides access to the methods, 
        /// properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            // Attach to the authenticate request event, attempts
            // to authenticate the current connection.
            context.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
        }

        /// <summary>
        /// Attempt to authenticate the current connection.
        /// </summary>
        /// <param name="sender">The current sender</param>
        /// <param name="e">The event argument</param>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            try
            {
                // Create a new member info type.
                MemberInfo memberInfo = new MemberInfo();

                // Has the user been authenticated
                if (request.IsAuthenticated)
                {
                    // Get the user identity name
                    string userIdentityName = context.User.Identity.Name;

                    // Generate the hash code from the user identity name.
                    memberInfo.UniqueHashcode = Hashcode.GetHashcode(userIdentityName, HashcodeType.SHA1);
                    memberInfo.UserIdentityName = userIdentityName;

                    // Add the member info to the context collection.
                    if (((MemberInfo)context.Items[memberInfo.UniqueHashcode]) == null)
                        context.Items.Add(memberInfo.UniqueHashcode, memberInfo);

                    // Add the new item or set the current member info.
                    Nequeo.Caching.RuntimeCache.Set(memberInfo.UniqueHashcode, memberInfo, (double)600.0);
                }
                else
                {
                    // The user has not been authenticated
                    // the connecting user is anonymous.
                    string userIdentityName = request.AnonymousID;

                    // If no anonymous id is found then
                    // generate a random user idnetity name.
                    if (String.IsNullOrEmpty(userIdentityName))
                    {
                        LowerUpperCaseGenerator password = new LowerUpperCaseGenerator();
                        userIdentityName = password.Random(10, 30);
                        memberInfo.IsAnonymousUser = true;
                        memberInfo.HasAnonymousID = false;
                    }
                    else
                        memberInfo.IsAnonymousUser = true;

                    // Generate the hash code from the user identity name.
                    memberInfo.UniqueHashcode = Hashcode.GetHashcode(userIdentityName, HashcodeType.SHA1);
                    memberInfo.UserIdentityName = userIdentityName;

                    // Add the member info to the context collection.
                    if (((MemberInfo)context.Items[memberInfo.UniqueHashcode]) == null)
                        context.Items.Add(memberInfo.UniqueHashcode, memberInfo);

                    // Add the new item or set the current member info.
                    Nequeo.Caching.RuntimeCache.Set(memberInfo.UniqueHashcode, memberInfo, (double)600.0);
                }
            }
            catch (Exception ex) 
            { 
                context.AddError(ex);
                LogHandler.WriteTypeMessage(ex.Message, typeof(Membership).GetMethod("OnAuthenticateRequest"));
            }
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements IHttpModule.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
