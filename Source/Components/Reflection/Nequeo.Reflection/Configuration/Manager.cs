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
using System.Reflection;
using System.Data;

namespace Nequeo.Configuration
{
    /// <summary>
    /// Provides access to configuration files for client applications.
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// Assign the configuration element
        /// </summary>
        /// <typeparam name="T">The configuration section type.</typeparam>
        /// <typeparam name="E">The configuration element type.</typeparam>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <param name="sectionName">The path to the section to be returned.</param>
        /// <param name="elementCount">The number of elements in the collection.</param>
        /// <returns>The collection of element types.</returns>
        public static E[] AssignSectionElements<T, E>(string assemblyFilePath, string sectionName, int elementCount) 
            where T : System.Configuration.ConfigurationSection
            where E : System.Configuration.ConfigurationElement
        {
            T section = GetSection<T>(assemblyFilePath, sectionName);
            E[] items = new E[elementCount];

            // Return all element items.
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E">The configuration element type.</typeparam>
        /// <typeparam name="C">The configuration element collection.</typeparam>
        /// <param name="collection">The configuration element collection to copy.</param>
        /// <param name="sectionElements">The collection of element types.</param>
        /// <param name="collectionAction">The action to perform on the section elements in order to return the final element collection.</param>
        /// <returns>The collection of element types.</returns>
        public static E[] GetSectionElements<E, C>(C collection, E[] sectionElements, Func<E, bool> collectionAction) 
            where C : System.Configuration.ConfigurationElementCollection
            where E : System.Configuration.ConfigurationElement
        {
            collection.CopyTo(sectionElements, 0);
            return sectionElements.Where(collectionAction).ToArray();
        }

        /// <summary>
        /// Save the changes to the configuration file.
        /// </summary>
        /// <param name="configuration">The configuration that represents the file.</param>
        /// <param name="saveMode">The save mode.</param>
        public static void Save(System.Configuration.Configuration configuration, System.Configuration.ConfigurationSaveMode saveMode)
        {
            configuration.Save(saveMode);
        }

        /// <summary>
        /// Gets the path or UNC location of the loaded file that contains the manifest, loads the assembly.
        /// </summary>
        /// <returns>The location of the loaded file that contains the manifest. If the loaded
        /// file was shadow-copied, the location is that of the file after being shadow-copied.
        /// If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
        /// method overload, the value returned is an empty string ("").</returns>
        public static string GetExecutingAssemblyLocation()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// Get the configuration file instance for the assembly
        /// </summary>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <returns>Represents a configuration file that is applicable to a particular computer,
        /// application, or resource. This class cannot be inherited.</returns>
        public static System.Configuration.Configuration Configuration(string assemblyFilePath)
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config =
                System.Configuration.ConfigurationManager.OpenExeConfiguration(assemblyFilePath);

            // Return the configuration.
            return config;
        }

        /// <summary>
        /// Get the configuration file instance for the assembly
        /// </summary>
        /// <param name="userLevel">The System.Configuration.ConfigurationUserLevel for which you are opening the configuration.</param>
        /// <returns>Represents a configuration file that is applicable to a particular computer,
        /// application, or resource. This class cannot be inherited.</returns>
        public static System.Configuration.Configuration Configuration(System.Configuration.ConfigurationUserLevel userLevel)
        {
            // Get assembly configuration file.
            return System.Configuration.ConfigurationManager.OpenExeConfiguration(userLevel);
        }

        /// <summary>
        /// Opens the machine configuration file on the current computer.
        /// </summary>
        /// <returns>Represents a configuration file that is applicable to a particular computer,
        /// application, or resource. This class cannot be inherited.</returns>
        public static System.Configuration.Configuration OpenMachineConfiguration()
        {
            // Get assembly configuration file.
            return System.Configuration.ConfigurationManager.OpenMachineConfiguration();
        }

        /// <summary>
        /// Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
        /// </summary>
        /// <param name="sectionName">The path to the section to be returned.</param>
        public static void RefreshSection(string sectionName)
        {
            System.Configuration.ConfigurationManager.RefreshSection(sectionName);
        }

        /// <summary>
        /// Get the specified section type.
        /// </summary>
        /// <typeparam name="T">The configuration section type.</typeparam>
        /// <param name="sectionName">The path to the section to be returned.</param>
        /// <returns>The section type</returns>
        public static T GetSection<T>(string sectionName) where T : System.Configuration.ConfigurationSection
        {
            // Get the section settings.
            T configSetting = System.Configuration.ConfigurationManager.GetSection(sectionName) as T;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified section type.
        /// </summary>
        /// <typeparam name="T">The configuration section type.</typeparam>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <param name="sectionName">The path to the section to be returned.</param>
        /// <returns>The section type</returns>
        public static T GetSection<T>(string assemblyFilePath, string sectionName) where T : System.Configuration.ConfigurationSection
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config = Configuration(assemblyFilePath);

            // Get the section settings.
            T configSetting = config.GetSection(sectionName) as T;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified section group type.
        /// </summary>
        /// <typeparam name="T">The configuration section group type.</typeparam>
        /// <param name="sectionGroupName">The path name of the System.Configuration.ConfigurationSectionGroup to return.</param>
        /// <returns>The section group type.</returns>
        public static T GetSectionGroup<T>(string sectionGroupName) where T : System.Configuration.ConfigurationSectionGroup
        {
            // Get the section settings.
            T configSetting = System.Configuration.ConfigurationManager.GetSection(sectionGroupName) as T;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified section group type.
        /// </summary>
        /// <typeparam name="T">The configuration section group type.</typeparam>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <param name="sectionGroupName">The path name of the System.Configuration.ConfigurationSectionGroup to return.</param>
        /// <returns>The section group type.</returns>
        public static T GetSectionGroup<T>(string assemblyFilePath, string sectionGroupName) where T : System.Configuration.ConfigurationSectionGroup
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config = Configuration(assemblyFilePath);

            // Get the section settings.
            T configSetting = config.GetSectionGroup(sectionGroupName) as T;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the application settings configuration section.
        /// </summary>
        /// <returns>An System.Configuration.AppSettingsSection object representing the appSettings
        /// configuration section that applies to this System.Configuration.Configuration object.</returns>
        public static System.Collections.Specialized.NameValueCollection GetAppSettings()
        {
            // An System.Configuration.AppSettingsSection object representing the appSettings
            // configuration section that applies to this System.Configuration.Configuration object.
            return System.Configuration.ConfigurationManager.AppSettings;
        }

        /// <summary>
        /// Get the application settings configuration section.
        /// </summary>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <returns>An System.Configuration.AppSettingsSection object representing the appSettings
        /// configuration section that applies to this System.Configuration.Configuration object.</returns>
        public static System.Configuration.AppSettingsSection GetAppSettingsbyAssembly(string assemblyFilePath)
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config = Configuration(assemblyFilePath);

            // An System.Configuration.AppSettingsSection object representing the appSettings
            // configuration section that applies to this System.Configuration.Configuration object.
            return config.AppSettings;
        }

        /// <summary>
        /// Get the application settings configuration section.
        /// </summary>
        /// <param name="sectionName">The path to the section to be returned.</param>
        /// <returns>An System.Configuration.AppSettingsSection object representing the appSettings
        /// configuration section that applies to this System.Configuration.Configuration object.</returns>
        public static System.Configuration.AppSettingsSection GetAppSettings(string sectionName)
        {
            // Get the section settings.
            System.Configuration.AppSettingsSection configSetting =
                System.Configuration.ConfigurationManager.GetSection(sectionName) as System.Configuration.AppSettingsSection;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified application settings group.
        /// </summary>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <param name="sectionGroupName">The path name of the System.Configuration.ConfigurationSectionGroup to return.</param>
        /// <returns>Represents a grouping of related application settings sections within a configuration
        /// file. This class cannot be inherited.</returns>
        public static System.Configuration.ApplicationSettingsGroup GetApplicationSettings(string assemblyFilePath, string sectionGroupName)
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config = Configuration(assemblyFilePath);

            // Get the section settings.
            System.Configuration.ApplicationSettingsGroup configSetting = 
                config.GetSectionGroup(sectionGroupName) as System.Configuration.ApplicationSettingsGroup;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified application settings group.
        /// </summary>
        /// <param name="sectionGroupName">The path name of the System.Configuration.ConfigurationSectionGroup to return.</param>
        /// <returns>Represents a grouping of related application settings sections within a configuration
        /// file. This class cannot be inherited.</returns>
        public static System.Configuration.ApplicationSettingsGroup GetApplicationSettings(string sectionGroupName)
        {
            // Get the section settings.
            System.Configuration.ApplicationSettingsGroup configSetting = 
                System.Configuration.ConfigurationManager.GetSection(sectionGroupName) as System.Configuration.ApplicationSettingsGroup;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified client settings section.
        /// </summary>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <param name="sectionName">The path to the section to be returned.</param>
        /// <returns>Represents a group of user-scoped application settings in a configuration file.</returns>
        public static System.Configuration.ClientSettingsSection GetClientSettings(string assemblyFilePath, string sectionName)
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config = Configuration(assemblyFilePath);

            // Get the section settings.
            System.Configuration.ClientSettingsSection configSetting = config.GetSection(sectionName) as System.Configuration.ClientSettingsSection;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the specified client settings section.
        /// </summary>
        /// <param name="sectionName">The path to the section to be returned.</param>
        /// <returns>Represents a group of user-scoped application settings in a configuration file.</returns>
        public static System.Configuration.ClientSettingsSection GetClientSettings(string sectionName)
        {
            // Get the section settings.
            System.Configuration.ClientSettingsSection configSetting = 
                System.Configuration.ConfigurationManager.GetSection(sectionName) as System.Configuration.ClientSettingsSection;

            // Return the configuration type.
            return configSetting;
        }

        /// <summary>
        /// Get the connection string configuration section.
        /// </summary>
        /// <returns>A System.Configuration.ConnectionStringsSection configuration-section object
        /// representing the connectionStrings configuration section that applies to
        /// this System.Configuration.Configuration object.</returns>
        public static System.Configuration.ConnectionStringSettingsCollection GetConnectionStrings()
        {
            // A System.Configuration.ConnectionStringsSection configuration-section object
            // representing the connectionStrings configuration section that applies to
            // this System.Configuration.Configuration object.
            return System.Configuration.ConfigurationManager.ConnectionStrings;
        }

        /// <summary>
        /// Get the connection string configuration section.
        /// </summary>
        /// <param name="assemblyFilePath">The assembly file name and path assocciated with the configuration file.</param>
        /// <returns>A System.Configuration.ConnectionStringsSection configuration-section object
        /// representing the connectionStrings configuration section that applies to
        /// this System.Configuration.Configuration object.</returns>
        public static System.Configuration.ConnectionStringsSection GetConnectionStrings(string assemblyFilePath)
        {
            // Get assembly configuration file.
            System.Configuration.Configuration config = Configuration(assemblyFilePath);

            // A System.Configuration.ConnectionStringsSection configuration-section object
            // representing the connectionStrings configuration section that applies to
            // this System.Configuration.Configuration object.
            return config.ConnectionStrings;
        }
    }
}
