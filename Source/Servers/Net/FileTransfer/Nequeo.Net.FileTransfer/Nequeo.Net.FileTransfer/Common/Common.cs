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
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Xml.Serialization;

namespace Nequeo.Net.FileTransfer.Common
{
    /// <summary>
    /// Helper class.
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// SQL Authentication Type Provider.
        /// </summary>
        public static string SQLAuthenticationTypeProvider = Nequeo.Net.FileTransfer.Properties.Settings.Default.SQLAuthenticationTypeProvider;

        /// <summary>
        /// SQL Composite Service.
        /// </summary>
        public static string SQLCompositeService = Nequeo.Net.FileTransfer.Properties.Settings.Default.SQLCompositeService;

        /// <summary>
        /// Get the authentication provider.
        /// </summary>
        public static string AuthenticationProvider = Nequeo.Net.FileTransfer.Properties.Settings.Default.AuthenticationProvider;

        /// <summary>
        /// Get the authentication provider.
        /// </summary>
        /// <returns>The authorisation provider.</returns>
        public static Nequeo.Security.IAuthorisationProvider Authenticate()
        {
            Nequeo.Security.IAuthorisationProvider authenticate = null;

            switch (AuthenticationProvider.ToLower())
            {
                case "sqlauthenticationtypeprovider":
                    // Get the provider type and create the provider instance.
                    string providerType = new Nequeo.Configuration.Reader().GetReflectionProvider(Common.Helper.SQLAuthenticationTypeProvider);
                    Type type = Type.GetType(providerType);
                    authenticate = (Nequeo.Security.IAuthorisationProvider)Nequeo.Reflection.TypeAccessor.CreateInstance(type);
                    break;

                case "sqlcompositeservice":
                    Nequeo.Composite.Composition<Nequeo.Security.IAuthorisationProvider, Nequeo.ComponentModel.Composition.IContentMetadata> compose = null;

                    try
                    {
                        // Create the composition instance.
                        compose = new Nequeo.Composite.Composition<Nequeo.Security.IAuthorisationProvider, Nequeo.ComponentModel.Composition.IContentMetadata>();

                        // Get the specified composite element.
                        Composite.Configuration.CompositeServiceDirectoryCatalogElement element =
                            new Nequeo.Composite.Configuration.Reader().GetServiceDirectory(Common.Helper.SQLCompositeService);

                        // Add the directory catalog items.
                        compose.AddCatalogItem(new string[] { element.Path }, element.SearchPattern);

                        // Compose the service.
                        compose.Compose();

                        // Get the composite service.
                        bool found = false;
                        authenticate = compose.FindCompositeContext(Common.Helper.SQLCompositeService, out found);
                    }
                    catch { throw; }
                    finally
                    {
                        try
                        {
                            // Release the services
                            // and release resources.
                            if (compose != null)
                            {
                                compose.Release();
                                compose.Dispose();
                            }
                        }
                        catch { }
                    }
                    break;

                default:
                    break;
            }

            // Return the authentication provider.
            return authenticate;
        }
    }
}
