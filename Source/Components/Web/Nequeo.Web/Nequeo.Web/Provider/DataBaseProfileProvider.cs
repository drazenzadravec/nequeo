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
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.Hosting;
using System.IO;
using System.Web.Compilation;

namespace Nequeo.Web.Provider
{
    /// <summary>
    /// Database source profile provider
    /// </summary>
    public sealed class DataBaseProfileProvider : ProfileProvider
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataBaseProfileProvider()
        {
        }

        private MachineKeySection _machineKey;
        private string _profileProviderType = null;
        private Nequeo.Data.DataType.IProfileProvider _profileProvider = null;

        /// <summary>
        /// Gets the profile provider type.
        /// </summary>
        /// <remarks>The profile provider type must implement 'Nequeo.Data.DataType.IProfileProvider'.</remarks>
        public string ProfileProviderType
        {
            get { return _profileProviderType; }
        }

        /// <summary>
        /// Gets or sets the current Nequeo.Data.DataType.IProfileProvider.
        /// </summary>
        public Nequeo.Data.DataType.IProfileProvider ProfileProviderTypeInstance
        {
            get { return _profileProvider; }
            set { _profileProvider = value; }
        }

        #region Abstract Property Overrides
        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        public override string ApplicationName
        {
            get { return _profileProvider.ApplicationName; }
            set { _profileProvider.ApplicationName = value; }
        }

        #endregion

        #region Abstract Method Overrides
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs 
        /// representing the provider-specific attributes specified 
        /// in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "DataBaseProfileProvider";

            // Get the profile application Name.
            if (String.IsNullOrEmpty(config["applicationName"]))
                throw new Exception("Attribute : applicationName, is missing this must be a valid application name.");

            // Get the profile provider type.
            if (String.IsNullOrEmpty(config["profileProviderType"]))
                throw new Exception("Attribute : profileProviderType, is missing (this type must implement : 'Nequeo.Data.DataType.IProfileProvider')");
            else
                _profileProviderType = config["profileProviderType"].ToString();

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Nequeo Pty Limited generic data access profile provider.");
            }

            // Get the current profile provider type
            // and create a instance of the type.
            Type profileProviderType = BuildManager.GetType(_profileProviderType, true, true);
            _profileProvider = (Nequeo.Data.DataType.IProfileProvider)Activator.CreateInstance(profileProviderType);

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _profileProvider.ProviderName = name;
            _profileProvider.ApplicationName = GetConfigValue(config["applicationName"], "ApplicationName");

            // Get encryption and decryption key information from the configuration.
            System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
        }

        /// <summary>
        /// When overridden in a derived class, deletes all user-profile data for profiles
        /// in which the last activity date occurred before the specified date.
        /// </summary>
        /// <param name="authenticationOption">One of the System.Web.Profile.ProfileAuthenticationOption values, specifying
        /// whether anonymous, authenticated, or both types of profiles are deleted.</param>
        /// <param name="userInactiveSinceDate">A System.DateTime that identifies which user profiles are considered inactive.
        /// If the System.Web.Profile.ProfileInfo.LastActivityDate value of a user profile
        /// occurs on or before this date and time, the profile is considered inactive.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            return _profileProvider.DeleteInactiveProfiles(authenticationOption, userInactiveSinceDate);
        }

        /// <summary>
        /// When overridden in a derived class, deletes profile properties and information
        /// for profiles that match the supplied list of user names.
        /// </summary>
        /// <param name="usernames">A string array of user names for profiles to be deleted.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        public override int DeleteProfiles(string[] usernames)
        {
            return _profileProvider.DeleteProfiles(usernames);
        }

        /// <summary>
        /// When overridden in a derived class, deletes profile properties and information
        /// for the supplied list of profiles.
        /// </summary>
        /// <param name="profiles">A System.Web.Profile.ProfileInfoCollection of information about profiles
        /// that are to be deleted.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            return _profileProvider.DeleteProfiles(profiles);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves profile information for profiles
        /// in which the last activity date occurred on or before the specified date
        /// and the user name matches the specified user name.
        /// </summary>
        /// <param name="authenticationOption">One of the System.Web.Profile.ProfileAuthenticationOption values, specifying
        /// whether anonymous, authenticated, or both types of profiles are returned.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="userInactiveSinceDate">A System.DateTime that identifies which user profiles are considered inactive.
        /// If the System.Web.Profile.ProfileInfo.LastActivityDate value of a user profile
        /// occurs on or before this date and time, the profile is considered inactive.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
        /// <returns>A System.Web.Profile.ProfileInfoCollection containing user profile information
        /// for inactive profiles where the user name matches the supplied usernameToMatch
        /// parameter.</returns>
        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, 
            int pageIndex, int pageSize, out int totalRecords)
        {
            return _profileProvider.FindInactiveProfilesByUserName(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves profile information for profiles
        /// in which the user name matches the specified user names.
        /// </summary>
        /// <param name="authenticationOption">One of the System.Web.Profile.ProfileAuthenticationOption values, specifying
        /// whether anonymous, authenticated, or both types of profiles are returned.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
        /// <returns>A System.Web.Profile.ProfileInfoCollection containing user-profile information
        /// for profiles where the user name matches the supplied usernameToMatch parameter.</returns>
        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return _profileProvider.FindProfilesByUserName(authenticationOption, usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves user-profile data from the
        /// data source for profiles in which the last activity date occurred on or before
        /// the specified date.
        /// </summary>
        /// <param name="authenticationOption">One of the System.Web.Profile.ProfileAuthenticationOption values, specifying
        /// whether anonymous, authenticated, or both types of profiles are returned.</param>
        /// <param name="userInactiveSinceDate">A System.DateTime that identifies which user profiles are considered inactive.
        /// If the System.Web.Profile.ProfileInfo.LastActivityDate of a user profile
        /// occurs on or before this date and time, the profile is considered inactive.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
        /// <returns>A System.Web.Profile.ProfileInfoCollection containing user-profile information
        /// about the inactive profiles.</returns>
        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            return _profileProvider.GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves user profile data for all profiles
        /// in the data source.
        /// </summary>
        /// <param name="authenticationOption">One of the System.Web.Profile.ProfileAuthenticationOption values, specifying
        /// whether anonymous, authenticated, or both types of profiles are returned.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
        /// <returns>A System.Web.Profile.ProfileInfoCollection containing user-profile information
        /// for all profiles in the data source.</returns>
        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            return _profileProvider.GetAllProfiles(authenticationOption, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, returns the number of profiles in which
        /// the last activity date occurred on or before the specified date.
        /// </summary>
        /// <param name="authenticationOption">One of the System.Web.Profile.ProfileAuthenticationOption values, specifying
        /// whether anonymous, authenticated, or both types of profiles are returned.</param>
        /// <param name="userInactiveSinceDate">A System.DateTime that identifies which user profiles are considered inactive.
        /// If the System.Web.Profile.ProfileInfo.LastActivityDate of a user profile
        /// occurs on or before this date and time, the profile is considered inactive.</param>
        /// <returns>The number of profiles in which the last activity date occurred on or before
        /// the specified date.</returns>
        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            return _profileProvider.GetNumberOfInactiveProfiles(authenticationOption, userInactiveSinceDate);
        }

        /// <summary>
        /// Returns the collection of settings property values for the specified application
        /// instance and settings property group.
        /// </summary>
        /// <param name="context">A System.Configuration.SettingsContext describing the current application use.</param>
        /// <param name="collection">A System.Configuration.SettingsPropertyCollection containing the settings
        /// property group whose values are to be retrieved.</param>
        /// <returns>A System.Configuration.SettingsPropertyValueCollection containing the values
        /// for the specified settings property group.</returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            return _profileProvider.GetPropertyValues(context, collection);
        }

        /// <summary>
        /// Sets the values of the specified group of property settings.
        /// </summary>
        /// <param name="context">A System.Configuration.SettingsContext describing the current application usage.</param>
        /// <param name="collection">A System.Configuration.SettingsPropertyValueCollection representing the group
        /// of property settings to set.</param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            _profileProvider.SetPropertyValues(context, collection);
        }
        #endregion

        /// <summary>
        /// Get the configration value.
        /// </summary>
        /// <param name="configValue">The configuration value.</param>
        /// <param name="defaultValue">The default value to set.</param>
        /// <returns>The string value.</returns>
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
    }
}
