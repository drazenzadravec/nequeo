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

namespace Nequeo.Reflection
{
    /// <summary>
    /// Sandbox for executing code.
    /// </summary>
    public class Sandbox : AppDomianHost<AppDomianMarshal>
    {
        /// <summary>
        /// Sandbox for executing code.
        /// </summary>
        /// <param name="basePath">The base path to the assemblies.</param>
        /// <param name="configurationFile">The configuration file name.</param>
        public Sandbox(string basePath, string configurationFile = null)
            : base(basePath, configurationFile)
        { }

        /// <summary>
        /// Initialise the app domain.
        /// </summary>
        /// <param name="path">The path to the assemblies.</param>
        /// <param name="configurationFile">The configuration file name.</param>
        protected override void Initialise(string path, string configurationFile = null)
        {
            // Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
            // other than the one in which the sandboxer resides.
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = Path.GetFullPath(path);

            // Get the configuration file.
            if (!String.IsNullOrEmpty(configurationFile))
                adSetup.ConfigurationFile = configurationFile;

            // Setting the permissions for the AppDomain. We give the permission to execute and to 
            // read/discover the location where the untrusted code is loaded.
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            // We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            //StrongName fullTrustAssembly = typeof(AppDomianMarshal).Assembly.Evidence.GetHostEvidence<StrongName>();

            // Create the application loader.
            AppDomianLoader loader = new AppDomianLoader()
                .DomainFriendlyName("SandboxModule")
                .AppDomainSetup(adSetup)
                .PermissionSet(permSet);

            // Create the app domain.
            base.AppDomain = loader.CreateAppDomain();

            // Load the domain.
            base.Instance = AppDomianLoader.Load<AppDomianMarshal>(base.AppDomain);
        }
    }
}
