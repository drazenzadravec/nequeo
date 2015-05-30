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
using System.Web;
using System.Web.Management;
using System.Web.Hosting;

namespace Nequeo.Handler.Common
{
    /// <summary>
    /// Service web application information.
    /// </summary>
    public abstract class ServiceWebApplicationInformation : WebBaseEvent
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">The message string.</param>
        /// <param name="eventSource">The event source.</param>
        /// <param name="eventCode">The event code.</param>
        protected ServiceWebApplicationInformation(string message,
            object eventSource, int eventCode)
            : base(message, eventSource, eventCode) { }

        /// <summary>
        /// Get the web application domian.
        /// </summary>
        /// <returns>The web application domian.</returns>
        protected string GetApplicationDomain()
        {
            // Get the name of the application domain.
            return ApplicationInformation.ApplicationDomain;
        }

        /// <summary>
        /// Get the web application virtual path.
        /// </summary>
        /// <returns>The web application virtual path.</returns>
        protected string GetApplicationVirtualPath()
        {
            // Get the name of the application virtual path.
            return ApplicationInformation.ApplicationVirtualPath;
        }

        /// <summary>
        /// Get the web application physical path.
        /// </summary>
        /// <returns>The web application physical path.</returns>
        protected string GetApplicationPath()
        {
            // Get the name of the application path.
            return ApplicationInformation.ApplicationPath;
        }

        /// <summary>
        /// Get the web application machine name.
        /// </summary>
        /// <returns>The web application machine name.</returns>
        protected string GetApplicationMachineName()
        {
            // Get the name of the application machine name.
            return ApplicationInformation.MachineName;
        }

        /// <summary>
        /// Get the web application hosting physical path.
        /// </summary>
        /// <param name="virtualPath">The web application virtual path.</param>
        /// <returns></returns>
        protected string GetWebAppHostingPath(string virtualPath)
        {
            // Make sure the page reference exists.
            if (virtualPath == null) throw new ArgumentNullException("virtualPath");

            return HostingEnvironment.MapPath(virtualPath);
        }

        /// <summary>
        /// Get the web application information.
        /// </summary>
        /// <returns>The web application information.</returns>
        public WebApplicationInformation GetEventAppInfo()
        {
            // Get the event message.
            WebApplicationInformation appImfo =
                ApplicationInformation;
            return appImfo;
        }
    }
}
