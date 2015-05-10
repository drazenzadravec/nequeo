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
using System.Runtime.Remoting.Activation;

namespace Nequeo.Reflection
{
    /// <summary>
    /// App domain include flag.
    /// </summary>
    [Flags]
    internal enum AppDomainFlag : long
    {
        /// <summary>
        /// None included.
        /// </summary>
        None = 0,
        /// <summary>
        /// App Domain Setup included.
        /// </summary>
        AppDomainSetup = 1,
        /// <summary>
        /// Full Trust Assemblies included.
        /// </summary>
        FullTrustAssemblies = 2,
        /// <summary>
        /// Permission Set included.
        /// </summary>
        PermissionSet = 4,
        /// <summary>
        /// Security Info included.
        /// </summary>
        SecurityInfo = 8,
        /// <summary>
        /// Domain Friendly Name included.
        /// </summary>
        DomainFriendlyName = 16,
    }

    /// <summary>
    /// Application domain loader.
    /// </summary>
    public sealed class AppDomianLoader
    {
        /// <summary>
        /// Application domain load.
        /// </summary>
        public AppDomianLoader()
        {
        }

        private AppDomainFlag _appDomainFlag = AppDomainFlag.None;
        private AppDomainSetup _appDomainSetup = null;
        private StrongName[] _fullTrustAssemblies = null;
        private PermissionSet _permissionSet = null;
        private Evidence _securityInfo = null;
        private string _domainFriendlyName = null;

        /// <summary>
        /// Set the app domain setup.
        /// </summary>
        /// <param name="appDomainSetup">The app domain setup.</param>
        /// <returns>The application loader.</returns>
        public AppDomianLoader AppDomainSetup(AppDomainSetup appDomainSetup)
        {
            _appDomainSetup = appDomainSetup;
            if (_appDomainSetup != null)
                _appDomainFlag = _appDomainFlag | AppDomainFlag.AppDomainSetup;

            return this;
        }

        /// <summary>
        /// Set the strong names.
        /// </summary>
        /// <param name="fullTrustAssemblies">The strong names.</param>
        /// <returns>The application loader.</returns>
        public AppDomianLoader StrongNames(params StrongName[] fullTrustAssemblies)
        {
            _fullTrustAssemblies = fullTrustAssemblies;
            if (_fullTrustAssemblies != null)
                _appDomainFlag = _appDomainFlag | AppDomainFlag.FullTrustAssemblies;

            return this;
        }

        /// <summary>
        /// Set the permission set.
        /// </summary>
        /// <param name="permissionSet">The permission set.</param>
        /// <returns>The application loader.</returns>
        public AppDomianLoader PermissionSet(PermissionSet permissionSet)
        {
            _permissionSet = permissionSet;
            if (_permissionSet != null)
                _appDomainFlag = _appDomainFlag | AppDomainFlag.PermissionSet;

            return this;
        }

        /// <summary>
        /// Set the security info.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns>The application loader.</returns>
        public AppDomianLoader Evidence(Evidence securityInfo)
        {
            _securityInfo = securityInfo;
            if (_securityInfo != null)
                _appDomainFlag = _appDomainFlag | AppDomainFlag.SecurityInfo;

            return this;
        }

        /// <summary>
        /// Set the domain friendly name.
        /// </summary>
        /// <param name="domainFriendlyName">The domain friendly name.</param>
        /// <returns>The application loader.</returns>
        public AppDomianLoader DomainFriendlyName(string domainFriendlyName)
        {
            _domainFriendlyName = domainFriendlyName;
            if (_domainFriendlyName != null)
                _appDomainFlag = _appDomainFlag | AppDomainFlag.DomainFriendlyName;

            return this;
        }

        /// <summary>
        /// Create a new application domain from the set paramaters.
        /// </summary>
        /// <returns>The create application domian; else null;</returns>
        public AppDomain CreateAppDomain()
        {
            AppDomain appDomain = null;

            // If the domain friendly name has been set,
            // if not then do not create the domain.
            if(_appDomainFlag.HasFlag(AppDomainFlag.DomainFriendlyName))
            {
                if (_appDomainFlag == 
                    (AppDomainFlag.AppDomainSetup |
                     AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies |
                     AppDomainFlag.PermissionSet |
                     AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo, _appDomainSetup, _permissionSet, _fullTrustAssemblies);
                }
                else if(_appDomainFlag == 
                    (AppDomainFlag.AppDomainSetup |
                     AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies |
                     AppDomainFlag.PermissionSet))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, null, _appDomainSetup, _permissionSet, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies |
                     AppDomainFlag.PermissionSet |
                     AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo, null, _permissionSet, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.AppDomainSetup |
                     AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies |
                     AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo, _appDomainSetup, null, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.AppDomainSetup |
                     AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.PermissionSet |
                     AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo, _appDomainSetup, _permissionSet);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies |
                     AppDomainFlag.PermissionSet))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, null, null, _permissionSet, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.AppDomainSetup |
                     AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, null, _appDomainSetup, null, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies |
                     AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo, null, null, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.AppDomainSetup |
                    AppDomainFlag.DomainFriendlyName |
                    AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo, _appDomainSetup);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.FullTrustAssemblies))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, null, null, null, _fullTrustAssemblies);
                }
                else if (_appDomainFlag ==
                (AppDomainFlag.AppDomainSetup |
                 AppDomainFlag.DomainFriendlyName))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, null, _appDomainSetup);
                }
                else if (_appDomainFlag ==
                    (AppDomainFlag.DomainFriendlyName |
                     AppDomainFlag.SecurityInfo))
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName, _securityInfo);
                }
                else
                {
                    // Create the domain.
                    appDomain = AppDomain.CreateDomain(_domainFriendlyName);
                }
            }

            // Return the domian.
            return appDomain;
        }

        /// <summary>
        /// Load the type 'T'.
        /// </summary>
        /// <typeparam name="T">The type to load.</typeparam>
        /// <param name="appDomain">The application domain to use.</param>
        /// <param name="arguments">The arguments used to create the instance.</param>
        /// <param name="activationAttributes">The activation attributes.</param>
        /// <returns>The instance of the type 'T'.</returns>
        public static T Load<T>(AppDomain appDomain, object[] arguments = null, object[] activationAttributes = null)
        {
            // Use CreateInstanceFrom to load an instance of 
            // the T class into the new AppDomain. 
            ObjectHandle handle = null;
            if (arguments == null)
            {
                // Use CreateInstanceFrom to load an instance of 
                // the T class into the new AppDomain. 
                handle = Activator.CreateInstanceFrom(
                    appDomain, typeof(T).Assembly.ManifestModule.FullyQualifiedName,
                    typeof(T).FullName);
            }
            else
            {
                // Use CreateInstanceFrom to load an instance of 
                // the T class into the new AppDomain. 
                handle = Activator.CreateInstanceFrom(
                    appDomain, typeof(T).Assembly.ManifestModule.FullyQualifiedName,
                    typeof(T).FullName, true, BindingFlags.CreateInstance, null, arguments, null, activationAttributes);
            }

            // Unwrap the new domain instance into a reference in 
            // this domain and use it to execute the untrusted code.
            return (T)handle.Unwrap();
        }

        /// <summary>
        /// Load the type.
        /// </summary>
        /// <param name="appDomain">The application domain to use.</param>
        /// <param name="fullyQualifiedName">The string representing the fully qualified name and path to this module.</param>
        /// <param name="fullName">The fully qualified name of the System.Type, including the namespace of the System.Type but not the assembly.</param>
        /// <param name="arguments">The arguments used to create the instance.</param>
        /// <param name="activationAttributes">The activation attributes.</param>
        /// <returns>The instance of the type 'T'.</returns>
        public static object Load(AppDomain appDomain, string fullyQualifiedName, string fullName, object[] arguments = null, object[] activationAttributes = null)
        {
            // Use CreateInstanceFrom to load an instance of 
            // the T class into the new AppDomain. 
            ObjectHandle handle = null;
            if (arguments == null)
            {
                // Use CreateInstanceFrom to load an instance of 
                // the T class into the new AppDomain. 
                handle = Activator.CreateInstanceFrom(appDomain, fullyQualifiedName, fullName);
            }
            else
            {
                // Use CreateInstanceFrom to load an instance of 
                // the T class into the new AppDomain. 
                handle = Activator.CreateInstanceFrom(appDomain, fullyQualifiedName, fullName, 
                    true, BindingFlags.CreateInstance, null, arguments, null, activationAttributes);
            }

            // Unwrap the new domain instance into a reference in 
            // this domain and use it to execute the untrusted code.
            return handle.Unwrap();
        }

        /// <summary>
        /// Unload the application domain.
        /// </summary>
        /// <param name="appDomain">The application domian to unload.</param>
        public void Unload(AppDomain appDomain)
        {
            AppDomain.Unload(appDomain);
        }
    }
}
