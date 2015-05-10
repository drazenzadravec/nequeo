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
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;

namespace Nequeo.Composite.Configuration
{
    /// <summary>
    /// Configuration reader
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Get the list of all composite service paths.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The list of paths else null.</returns>
        public string[] GetServicePaths(string section = "CompositeGroup/CompositeServices")
        {
            List<string> paths = new List<string>();

            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                CompositeServices defaultHost =
                    (CompositeServices)System.Configuration.ConfigurationManager.GetSection(section);

                // Get the list of assembly paths used to load the composite servers.
                CompositeServiceDirectoryCatalogCollection serviceCol = defaultHost.ServiceDirectoryCatalogSection;

                if (serviceCol != null)
                {
                    // For each service found
                    foreach (CompositeServiceDirectoryCatalogElement item in serviceCol)
                    {
                        if (!item.Path.Contains("C:\\TempDefault\\"))
                        {
                            // If a path has been specified.
                            if (!String.IsNullOrEmpty(item.Path))
                            {
                                // Get the list of paths for specific services.
                                string[] current = Directory.GetDirectories(item.Path, "*.*", SearchOption.AllDirectories);

                                // Join the current path with the default path.
                                if (current != null && current.Length > 0)
                                    paths.AddRange(current);
                            }
                        }
                    }
                }

                return paths.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the list of all composite service paths.
        /// </summary>
        /// <param name="name">The specific service name to search for the path.</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The list of paths else null.</returns>
        public string[] GetServicePaths(string name, string section = "CompositeGroup/CompositeServices")
        {
            List<string> paths = new List<string>();

            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                CompositeServices defaultHost =
                    (CompositeServices)System.Configuration.ConfigurationManager.GetSection(section);

                // Get the list of assembly paths used to load the composite servers.
                CompositeServiceDirectoryCatalogCollection serviceCol = defaultHost.ServiceDirectoryCatalogSection;

                if (serviceCol != null)
                {
                    // For each service found
                    foreach (CompositeServiceDirectoryCatalogElement item in serviceCol)
                    {
                        // If a name has been specified.
                        if (item.Name.ToLower() == name.ToLower())
                        {
                            if (!item.Path.Contains("C:\\TempDefault\\"))
                            {
                                // If a path has been specified.
                                if (!String.IsNullOrEmpty(item.Path))
                                {
                                    // Get the list of paths for specific services.
                                    string[] current = Directory.GetDirectories(item.Path, "*.*", SearchOption.AllDirectories);

                                    // Join the current path with the default path.
                                    if (current != null && current.Length > 0)
                                        paths.AddRange(current);
                                }
                            }
                        }
                    }
                }

                return paths.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the composite service.
        /// </summary>
        /// <param name="name">The specific service name to search for the path.</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The service; else null.</returns>
        public CompositeServiceDirectoryCatalogElement GetServiceDirectory(string name, string section = "CompositeGroup/CompositeServices")
        {
            CompositeServiceDirectoryCatalogElement service = null;

            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                CompositeServices defaultHost =
                    (CompositeServices)System.Configuration.ConfigurationManager.GetSection(section);

                // Get the list of assembly paths used to load the composite servers.
                CompositeServiceDirectoryCatalogCollection serviceCol = defaultHost.ServiceDirectoryCatalogSection;

                // If a service has not been found.
                if (service == null)
                {
                    if (serviceCol != null)
                    {
                        // For each service found
                        foreach (CompositeServiceDirectoryCatalogElement item in serviceCol)
                        {
                            // If a name has been specified.
                            if (item.Name.ToLower() == name.ToLower())
                            {
                                // Return this service.
                                service = item;
                                break;
                            }
                        }
                    }
                }

                return service;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the composite service.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The service; else null.</returns>
        public CompositeServiceDirectoryCatalogCollection GetServiceDirectories(string section = "CompositeGroup/CompositeServices")
        {
            CompositeServiceDirectoryCatalogCollection serviceCol = null;

            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                CompositeServices defaultHost =
                    (CompositeServices)System.Configuration.ConfigurationManager.GetSection(section);

                // Get the list of assembly paths used to load the composite servers.
                serviceCol = defaultHost.ServiceDirectoryCatalogSection;

                return serviceCol;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the composite service.
        /// </summary>
        /// <param name="name">The specific service name to search for the path.</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The service; else null.</returns>
        public CompositeServiceTypeCatalogElement GetServiceType(string name, string section = "CompositeGroup/CompositeServices")
        {
            CompositeServiceTypeCatalogElement service = null;

            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                CompositeServices defaultHost =
                    (CompositeServices)System.Configuration.ConfigurationManager.GetSection(section);

                // Get the list of assembly paths used to load the composite servers.
                CompositeServiceTypeCatalogCollection serviceCol = defaultHost.ServiceTypeCatalogSection;

                // If a service has not been found.
                if (service == null)
                {
                    if (serviceCol != null)
                    {
                        // For each service found
                        foreach (CompositeServiceTypeCatalogElement item in serviceCol)
                        {
                            // If a name has been specified.
                            if (item.Name.ToLower() == name.ToLower())
                            {
                                // Return this service.
                                service = item;
                                break;
                            }
                        }
                    }
                }

                return service;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the composite service.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The service; else null.</returns>
        public CompositeServiceTypeCatalogCollection GetServiceTypes(string section = "CompositeGroup/CompositeServices")
        {
            CompositeServiceTypeCatalogCollection serviceCol = null;

            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                CompositeServices defaultHost =
                    (CompositeServices)System.Configuration.ConfigurationManager.GetSection(section);

                // Get the list of assembly paths used to load the composite servers.
                serviceCol = defaultHost.ServiceTypeCatalogSection;

                return serviceCol;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
