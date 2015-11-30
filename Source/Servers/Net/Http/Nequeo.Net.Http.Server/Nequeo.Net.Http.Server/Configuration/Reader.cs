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
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Reflection;

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Invention;
using Nequeo.Serialisation;

namespace Nequeo.Net.Configuration
{
    /// <summary>
    /// Configuration element reader.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    internal class ElementReader
    {
        /// <summary>
        /// Gets, the http host collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The http host collection; else null.</returns>
        public static HttpHostElement[] HttpHostElements(string section = "HttpServerGroup/HttpServerHosts")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                HttpServerHosts baseHandler =
                    (HttpServerHosts)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                // Return the collection.
                HttpHostElement[] items = new HttpHostElement[baseHandler.HostSection.Count];
                baseHandler.HostSection.CopyTo(items, 0);
                return items.Where(q => (q.NameAttribute != "default")).ToArray();
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                return null;
            }
        }
    }

    /// <summary>
    /// Configuration reader
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    internal class ReaderHttp : ServiceBase, IDisposable
    {
        /// <summary>
        /// Get the path prefix.
        /// </summary>
        /// <param name="hostName">The host name to search for.</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The prefix path.</returns>
        public string GetPathPrefix(string hostName, string section = "HttpServerGroup/HttpServerHosts")
        {
            try
            {
                // If no host name set then
                // use the defualt values.
                if (String.IsNullOrEmpty(hostName))
                {
                    // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                    System.Configuration.ConfigurationManager.RefreshSection(section);

                    // Create a new default host type
                    // an load the values from the configuration
                    // file into the default host type.
                    HttpServerHosts defaultHost =
                        (HttpServerHosts)System.Configuration.ConfigurationManager.GetSection(section);

                    // The http html provider path prefix.
                    return defaultHost.HostSection[hostName].PathAttribute;
                }
            
                // Return null.
                return null;
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                return null;
            }
        }

        /// <summary>
        /// Gets the base directory path.
        /// </summary>
        /// <returns>The base path.</returns>
        public static string GetBaseDirectoryPath()
        {
            return Nequeo.Net.Common.Helper.BaseDirectory.TrimEnd('\\') + "\\";
        }

        /// <summary>
        /// Gets the provider prefix path
        /// </summary>
        /// <param name="providerName">The provider name</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The collection of prefix paths.</returns>
        public string[] GetProviderPathPrefix(string providerName, string section = "HttpServerGroup/HttpServerHosts")
        {
            List<string> prefixPaths = new List<string>();

            try
            {
                // Get the host collection elements.
                HttpHostElement[] httpHostElements = ElementReader.HttpHostElements(section);
                if (httpHostElements != null)
                {
                    // Get all provider hosts.
                    IEnumerable<HttpHostElement> hosts = httpHostElements.Where(u => u.HostAttribute.ToLower().Contains(providerName.ToLower()));

                    // If hosts have been found. Add each prefix path.
                    if (hosts.Count() > 0)
                        foreach (HttpHostElement item in hosts)
                            prefixPaths.Add(item.PathAttribute);

                    // Return the collection of hosts.
                    return prefixPaths.ToArray();
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                return null;
            }
        }

        /// <summary>
        /// Get the provider authentication mode.
        /// </summary>
        /// <param name="url">The Uri instance.</param>
        /// <param name="providerName">The provider name</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The authentication mode.</returns>
        public static string GetProviderAuthentication(Uri url, string providerName, string section = "HttpServerGroup/HttpServerHosts")
        {
            string authName = null;
            try
            {
                // Get the http host collection elements.
                HttpHostElement[] httpHostElements = ElementReader.HttpHostElements(section);
                if (httpHostElements != null)
                {
                    // Get all provider hosts.
                    IEnumerable<HttpHostElement> hosts = httpHostElements.Where(u => u.HostAttribute.ToLower().Contains(providerName));

                    // If hosts have been found.
                    if (hosts.Count() > 0)
                        foreach (HttpHostElement item in hosts)
                        {
                            // If the current url and path are similar.
                            if (url.ToString().ToLower().Contains(item.PathAttribute.ToLower()))
                            {
                                // Get the authentication type.
                                authName = item.AuthenticationAttribute.ToString().ToLower();
                                break;
                            }
                        }

                    // Return the collection of hosts.
                    return authName;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                return null;
            }
        }

        /// <summary>
        /// Get the mime type context data.
        /// </summary>
        /// <returns>The collection of mime types.</returns>
        public static Nequeo.Net.Data.context GetMimeType()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = Nequeo.Net.Common.Helper.MimeTypeXmlPath;
                string xsd = Nequeo.Net.Properties.Resources.MimeType;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                Nequeo.Net.Data.context mimeTypeData =
                    ((Nequeo.Net.Data.context)serial.Deserialise(typeof(Nequeo.Net.Data.context), xml));

                // Return the mime types.
                return mimeTypeData;
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);

                return null;
            }
        }
    }
}
