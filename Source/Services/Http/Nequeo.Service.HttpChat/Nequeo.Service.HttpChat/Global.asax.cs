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
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Nequeo.Service
{
    /// <summary>
    /// Global.
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Application start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            if (Common.Helper.Communication == null)
            {
                try
                {
                    Authorisation.Communication.Client communicationProvider = new Authorisation.Communication.Client();
                    Common.Helper.Communication = communicationProvider;

                    communicationProvider.ReconnectWhenNoConnection = true;
                    communicationProvider.Initialisation();
                    communicationProvider.Connect();
                }
                catch { }
            }

            if (Common.Helper.Token == null)
            {
                try
                {
                    Authorisation.Token.Client tokenProvider = new Authorisation.Token.Client();
                    Common.Helper.Token = tokenProvider;

                    tokenProvider.ReconnectWhenNoConnection = true;
                    tokenProvider.Initialisation();
                    tokenProvider.Connect();
                }
                catch { }
            }

            if (Common.Helper.IntegrationContext == null)
            {
                try
                {
                    Interact.IntegrationServer integrationServer = new Interact.IntegrationServer();
                    Common.Helper.IntegrationContext = integrationServer;

                    integrationServer.CommunicationProvider = Common.Helper.Communication;
                    integrationServer.Initialisation(true);
                    integrationServer.DirectConnection = true;
                    integrationServer.Receiver = (uid, sn, rsn, r, d) => Common.Helper.Receiver(uid, sn, rsn, r, d);
                    integrationServer.Start();
                }
                catch { }
            }

            if (Common.Helper.MemberContextManager == null)
            {
                try
                {
                    Server.MemberContextManager memberContextManager = new Server.MemberContextManager();
                    Common.Helper.MemberContextManager = memberContextManager;
                }
                catch { }
            }

            if (Common.Helper.TimeoutManager == null)
            {
                try
                {
                    Server.TimeoutManager timeoutManager = new Server.TimeoutManager();
                    Common.Helper.TimeoutManager = timeoutManager;
                    timeoutManager.Start(60);
                }
                catch { }
            }
        }

        /// <summary>
        /// Session start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Application begin request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Application authenticate request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Application error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Session end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Application end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(object sender, EventArgs e)
        {
            if (Common.Helper.Communication != null)
            {
                try
                {
                    // If server timeout manager.
                    if (Common.Helper.Communication is Authorisation.Communication.Client)
                    {
                        Authorisation.Communication.Client communicationProvider = (Authorisation.Communication.Client)Common.Helper.Communication;
                        communicationProvider.Close();
                    }
                }
                catch { }

                try
                {
                    // If the current state context
                    // implements IDisposable then
                    // dispose of the resources.
                    if (Common.Helper.Communication is IDisposable)
                    {
                        IDisposable disposable = (IDisposable)Common.Helper.Communication;
                        disposable.Dispose();
                    }
                }
                catch { }
                Common.Helper.Communication = null;
            }

            if (Common.Helper.Token != null)
            {
                try
                {
                    // If server timeout manager.
                    if (Common.Helper.Token is Authorisation.Token.Client)
                    {
                        Authorisation.Token.Client tokenProvider = (Authorisation.Token.Client)Common.Helper.Token;
                        tokenProvider.Close();
                    }
                }
                catch { }

                try
                {
                    // If the current state context
                    // implements IDisposable then
                    // dispose of the resources.
                    if (Common.Helper.Token is IDisposable)
                    {
                        IDisposable disposable = (IDisposable)Common.Helper.Token;
                        disposable.Dispose();
                    }
                }
                catch { }
                Common.Helper.Token = null;
            }

            if (Common.Helper.IntegrationContext != null)
            {
                try
                {
                    // If server timeout manager.
                    if (Common.Helper.IntegrationContext is Interact.IntegrationServer)
                    {
                        Interact.IntegrationServer integrationServer = (Interact.IntegrationServer)Common.Helper.IntegrationContext;
                        integrationServer.Stop();
                    }
                }
                catch { }

                try
                {
                    // If the current state context
                    // implements IDisposable then
                    // dispose of the resources.
                    if (Common.Helper.IntegrationContext is IDisposable)
                    {
                        IDisposable disposable = (IDisposable)Common.Helper.IntegrationContext;
                        disposable.Dispose();
                    }
                }
                catch { }
                Common.Helper.IntegrationContext = null;
            }

            if (Common.Helper.MemberContextManager != null)
            {
                try
                {
                    // If the current state context
                    // implements IDisposable then
                    // dispose of the resources.
                    if (Common.Helper.MemberContextManager is IDisposable)
                    {
                        IDisposable disposable = (IDisposable)Common.Helper.MemberContextManager;
                        disposable.Dispose();
                    }
                }
                catch { }
                Common.Helper.MemberContextManager = null;
            }

            if (Common.Helper.TimeoutManager != null)
            {
                try
                {
                    // If server timeout manager.
                    if (Common.Helper.TimeoutManager is Server.TimeoutManager)
                    {
                        Server.TimeoutManager timeoutManager = (Server.TimeoutManager)Common.Helper.TimeoutManager;
                        timeoutManager.Stop();
                    }
                }
                catch { }

                try
                {
                    // If the current state context
                    // implements IDisposable then
                    // dispose of the resources.
                    if (Common.Helper.TimeoutManager is IDisposable)
                    {
                        IDisposable disposable = (IDisposable)Common.Helper.TimeoutManager;
                        disposable.Dispose();
                    }
                }
                catch { }
                Common.Helper.TimeoutManager = null;
            }
        }
    }
}