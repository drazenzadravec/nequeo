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
using System.IO;
using System.Text;
using System.Security;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Policy;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Remoting;
using System.Reflection.Emit;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Remoting.Activation;

namespace Nequeo.Reflection
{
    /// <summary>
    /// Application domain host.
    /// </summary>
    public abstract class AppDomianHost<T>
    {
        /// <summary>
        /// Application domain host.
        /// </summary>
        /// <param name="basePath">The base path to the assemblies.</param>
        /// <param name="configurationFile">The configuration file name.</param>
        protected AppDomianHost(string basePath, string configurationFile = null)
        {
            if (String.IsNullOrEmpty(basePath)) throw new ArgumentNullException("basePath");
            Initialise(basePath, configurationFile);
        }

        private T _instance = default(T);
        private AppDomain _appDomain = null;

        /// <summary>
        /// Gets the new host instance.
        /// </summary>
        public T Instance
        {
            get { return _instance; }
            protected set { _instance = value; }
        }

        /// <summary>
        /// Gets the application domain.
        /// </summary>
        public AppDomain AppDomain
        {
            get { return _appDomain; }
            protected set { _appDomain = value; }
        }

        /// <summary>
        /// Unload the application domain.
        /// </summary>
        public virtual void Unload()
        {
            AppDomain.Unload(_appDomain);
        }

        /// <summary>
        /// Initialise the app domain.
        /// </summary>
        /// <param name="path">The path to the assemblies.</param>
        /// <param name="configurationFile">The configuration file name.</param>
        protected virtual void Initialise(string path, string configurationFile = null)
        {
            // Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
            // other than the one in which the sandboxer resides.
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = Path.GetFullPath(path);

            // Get the configuration file.
            if (!String.IsNullOrEmpty(configurationFile))
                adSetup.ConfigurationFile = configurationFile;

            // Create the application loader.
            AppDomianLoader loader = new AppDomianLoader()
                .DomainFriendlyName(DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo).GetHashCode().ToString("x"))
                .AppDomainSetup(adSetup);

            // Create the app domain.
            _appDomain = loader.CreateAppDomain();

            // Load the domain.
            _instance = AppDomianLoader.Load<T>(_appDomain);
        }
    }

    /// <summary>
    /// Application domain host.
    /// </summary>
    public class AppDomianHost : AppDomianHost<AppDomianMarshal>
    {
        /// <summary>
        /// Application domain host.
        /// </summary>
        /// <param name="basePath">The base path to the assemblies.</param>
        /// <param name="configurationFile">The configuration file name.</param>
        public AppDomianHost(string basePath, string configurationFile = null)
            : base(basePath, configurationFile)
        {
        }
    }
}
