/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          MembershipProvider.cs
 *  Purpose :       
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration.Provider;
using System.Configuration;
using System.Web.Profile;

using Nequeo.ComponentModel.Composition;
using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;
using Nequeo.Web;

namespace Nequeo.DataAccess.CloudInteraction
{
    /// <summary>
    /// Cloud Interaction profile provider base.
    /// </summary>
    public class ProfileProviderBase
    {
        /// <summary>
        /// Gets the current ProfileProvider context
        /// </summary>
        public System.Web.Profile.ProfileProvider Context
        {
            get
            {
                // Create the profile provider instance.
                Nequeo.Web.Provider.DataBaseProfileProvider provider = new Web.Provider.DataBaseProfileProvider();
                Nequeo.Data.DataType.IProfileProvider cloudProvider = new ProfileProvider();

                // Assign the current cloud profile provider instance.
                provider.ProfileProviderTypeInstance = cloudProvider;
                return provider;
            }
        }
    }

    /// <summary>
    /// Cloud Interaction profile provider.
    /// </summary>
    [Export(typeof(Nequeo.Data.DataType.IProfileProvider))]
    [ContentMetadata(Name = "CloudInteractionProfileProvider", Index = 0)]
    public class ProfileProvider : Nequeo.Data.DataType.IProfileProvider
    {
        private string _providerName = string.Empty;
        private string _applicationName = string.Empty;

        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        /// <summary>
        /// Gets sets the provider name.
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
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
        public int DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            int deleteCount = 0;

            // Get all inactive profiles.
            Nequeo.DataAccess.CloudInteraction.Data.Profile[] profiles = GetInactiveProfiles(userInactiveSinceDate);
            IEnumerable<Nequeo.DataAccess.CloudInteraction.Data.Profile> profileCol = null;

            // Select the authentication option
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    profileCol = profiles.Where(u => u.IsAnonymous == true);
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    profileCol = profiles.Where(u => u.IsAnonymous == false);
                    break;
                default:
                    profileCol = profiles.AsEnumerable();
                    break;
            }

            List<string> usernames = new List<string>();

            // Return the username.
            if (profileCol != null)
                if (profileCol.Count() > 0)
                {
                    // Get the list of inactive user profiles.
                    foreach (Nequeo.DataAccess.CloudInteraction.Data.Profile profile in profileCol)
                        usernames.Add(profile.Username);

                    // Delete the specified user profiles.
                    if (usernames.Count > 0)
                        deleteCount = DeleteProfiles(usernames.ToArray());
                }

            // The number of profiles deleted.
            return deleteCount;
        }

        /// <summary>
        /// When overridden in a derived class, deletes profile properties and information
        /// for the supplied list of profiles.
        /// </summary>
        /// <param name="profiles">A System.Web.Profile.ProfileInfoCollection of information about profiles
        /// that are to be deleted.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        public int DeleteProfiles(System.Web.Profile.ProfileInfoCollection profiles)
        {
            int deleteCount = 0;

            // For each profile found delete the profile.
            foreach (ProfileInfo p in profiles)
            {
                // Get the profile for the current user name.
                Nequeo.DataAccess.CloudInteraction.Data.Profile profile = GetSpecificProfile(p.UserName);

                // If a profile exits.
                if (profile != null)
                {
                    // Delete all profile values and profiles.
                    if (DeleteProfileValue(profile.ProfileID))
                        if (DeleteProfile(p.UserName))
                            deleteCount++;
                }
            }

            // The number of profiles deleted.
            return deleteCount;
        }

        /// <summary>
        /// When overridden in a derived class, deletes profile properties and information
        /// for profiles that match the supplied list of user names.
        /// </summary>
        /// <param name="usernames">A string array of user names for profiles to be deleted.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        public int DeleteProfiles(string[] usernames)
        {
            int deleteCount = 0;

            // For each user name found delete the profile.
            foreach (string user in usernames)
            {
                // Get the profile for the current user name.
                Nequeo.DataAccess.CloudInteraction.Data.Profile profile = GetSpecificProfile(user);

                // If a profile exits.
                if (profile != null)
                {
                    // Delete all profile values and profiles.
                    if (DeleteProfileValue(profile.ProfileID))
                        if (DeleteProfile(user))
                            deleteCount++;
                }
            }

            // The number of profiles deleted.
            return deleteCount;
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
        public System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCol = new ProfileInfoCollection();
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile profile = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile();

            // Get all the profiles for the match.
            long profilesMatched = 0;
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.Profile> profileData = null;

            // Select the authentication option
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.IsAnonymous == true) && (u.LastActivityDate <= userInactiveSinceDate) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u =>
                        (u.ApplicationName == ApplicationName) && (u.IsAnonymous == true) && (u.LastActivityDate <= userInactiveSinceDate) &&
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%"))));

                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                case ProfileAuthenticationOption.Authenticated:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.IsAnonymous == false) && (u.LastActivityDate <= userInactiveSinceDate) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u =>
                        (u.ApplicationName == ApplicationName) && (u.IsAnonymous == false) && (u.LastActivityDate <= userInactiveSinceDate) &&
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%"))));

                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                default:
                    // Get all the profiles for the match.
                    profilesMatched = profile.Select.GetRecordCount(
                        u =>
                            (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%"))));

                    // Get the total number of uses.
                    totalRecords = Int32.Parse(profilesMatched.ToString());

                    // Get the current set on data.
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);
                    break;
            }

            // For each profile found.
            if (profileData != null)
                foreach (Data.Profile item in profileData)
                {
                    // Create the membership user.
                    System.Web.Profile.ProfileInfo profileUser =
                        new System.Web.Profile.ProfileInfo(
                            item.Username,
                            item.IsAnonymous.Value,
                            item.LastActivityDate.Value,
                            item.LastUpdatedDate.Value,
                            0);

                    // Add the profile to the collection.
                    profileInfoCol.Add(profileUser);
                }

            // Return the collection of profile users.
            return profileInfoCol;
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
        public System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption,
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCol = new ProfileInfoCollection();
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile profile = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile();

            // Get all the profiles for the match.
            long profilesMatched = 0;
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.Profile> profileData = null;

            // Select the authentication option
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.IsAnonymous == true) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u => 
                        (u.ApplicationName == ApplicationName) && (u.IsAnonymous == true) && 
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%"))));

                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                case ProfileAuthenticationOption.Authenticated:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.IsAnonymous == false) && 
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u => 
                        (u.ApplicationName == ApplicationName) && (u.IsAnonymous == false) && 
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%"))));

                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                default:
                    // Get all the profiles for the match.
                    profilesMatched = profile.Select.GetRecordCount(
                        u =>
                            (u.ApplicationName == ApplicationName) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%"))));

                    // Get the total number of uses.
                    totalRecords = Int32.Parse(profilesMatched.ToString());

                    // Get the current set on data.
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) &&
                            (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);
                    break;
            }

            // For each profile found.
            if (profileData != null)
                foreach (Data.Profile item in profileData)
                {
                    // Create the membership user.
                    System.Web.Profile.ProfileInfo profileUser =
                        new System.Web.Profile.ProfileInfo(
                            item.Username,
                            item.IsAnonymous.Value,
                            item.LastActivityDate.Value,
                            item.LastUpdatedDate.Value,
                            0);

                    // Add the profile to the collection.
                    profileInfoCol.Add(profileUser);
                }

            // Return the collection of profile users.
            return profileInfoCol;
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
        public System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCol = new ProfileInfoCollection();
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile profile = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile();

            // Get all the profiles for the match.
            long profilesMatched = 0;
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.Profile> profileData = null;

            // Select the authentication option
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate) && (u.IsAnonymous == true)).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u =>
                        (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate) && (u.IsAnonymous == true));
                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                case ProfileAuthenticationOption.Authenticated:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate) && (u.IsAnonymous == false)).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u =>
                        (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate) && (u.IsAnonymous == false));
                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                default:
                    // Get all the profiles for the match.
                    profilesMatched = profile.Select.GetRecordCount(
                        u =>
                            (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate));

                    // Get the total number of uses.
                    totalRecords = Int32.Parse(profilesMatched.ToString());

                    // Get the current set on data.
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.LastActivityDate <= userInactiveSinceDate)).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);
                    break;
            }

            // For each profile found.
            if (profileData != null)
                foreach (Data.Profile item in profileData)
                {
                    // Create the membership user.
                    System.Web.Profile.ProfileInfo profileUser =
                        new System.Web.Profile.ProfileInfo(
                            item.Username,
                            item.IsAnonymous.Value,
                            item.LastActivityDate.Value,
                            item.LastUpdatedDate.Value,
                            0);

                    // Add the profile to the collection.
                    profileInfoCol.Add(profileUser);
                }

            // Return the collection of profile users.
            return profileInfoCol;
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
        public System.Web.Profile.ProfileInfoCollection GetAllProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption,
            int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCol = new ProfileInfoCollection();
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile profile = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile();

            // Get all the profiles for the match.
            long profilesMatched = 0;
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.Profile> profileData = null;

            // Select the authentication option
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.IsAnonymous == true)).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u => (u.ApplicationName == ApplicationName) && (u.IsAnonymous == true));
                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                case ProfileAuthenticationOption.Authenticated:
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName) && (u.IsAnonymous == false)).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);

                    profilesMatched = profile.Select.GetRecordCount(u => (u.ApplicationName == ApplicationName) && (u.IsAnonymous == false));
                    totalRecords = Int32.Parse(profilesMatched.ToString());
                    break;

                default:
                    // Get all the profiles for the match.
                    profilesMatched = profile.Select.GetRecordCount(
                        u =>
                            (u.ApplicationName == ApplicationName));

                    // Get the total number of uses.
                    totalRecords = Int32.Parse(profilesMatched.ToString());

                    // Get the current set on data.
                    profileData = profile.Select.QueryableProvider().
                        Where(u =>
                            (u.ApplicationName == ApplicationName)).
                        OrderBy(u => u.Username).
                        Take(pageSize).
                        Skip(skipNumber);
                    break;
            }

            // For each profile found.
            if (profileData != null)
                foreach (Data.Profile item in profileData)
                {
                    // Create the membership user.
                    System.Web.Profile.ProfileInfo profileUser =
                        new System.Web.Profile.ProfileInfo(
                            item.Username,
                            item.IsAnonymous.Value,
                            item.LastActivityDate.Value,
                            item.LastUpdatedDate.Value,
                            0);

                    // Add the profile to the collection.
                    profileInfoCol.Add(profileUser);
                }

            // Return the collection of profile users.
            return profileInfoCol;
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
        public int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            int inactiveCount = 0;

            // Get all inactive profiles.
            Nequeo.DataAccess.CloudInteraction.Data.Profile[] profiles = GetInactiveProfiles(userInactiveSinceDate);
            IEnumerable<Nequeo.DataAccess.CloudInteraction.Data.Profile> profileCol = null;

            // Select the authentication option
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    profileCol = profiles.Where(u => u.IsAnonymous == true);
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    profileCol = profiles.Where(u => u.IsAnonymous == false);
                    break;
                default:
                    profileCol = profiles.AsEnumerable();
                    break;
            }

            // Return the inactive profiles..
            if (profileCol != null)
                inactiveCount = profileCol.Count();

            // The number of inactive profiles.
            return inactiveCount;
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
        public System.Configuration.SettingsPropertyValueCollection GetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection collection)
        {
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            // Create a new SettingsPropertyValueCollection instance.
            SettingsPropertyValueCollection svc = new SettingsPropertyValueCollection();

            // Get the profile for the current user name.
            Nequeo.DataAccess.CloudInteraction.Data.Profile profile = GetSpecificProfile(username);

            // If a profile exits.
            if (profile != null)
            {
                // For each property setting.
                foreach (SettingsProperty prop in collection)
                {
                    bool propertyFound = false;

                    // Get the value for the current property name.
                    object value = GetProfilePropertyValue(profile, isAuthenticated, prop.Name, out propertyFound);

                    // If the property has been found.
                    if (propertyFound)
                    {
                        // Add the value to the property collection.
                        SettingsPropertyValue pv = new SettingsPropertyValue(prop);
                        pv.PropertyValue = value;
                        svc.Add(pv);
                    }
                    else
                        throw new ProviderException("Unsupported property " + prop.Name);
                }

                // Updates the LastActivityDate and LastUpdatedDate values when profile properties
                UpdateActivityDates(username, isAuthenticated, true);
            }
            else
            {
                // Make sure that the profile exists for the username if authenticated.
                if (isAuthenticated)
                    throw new ProviderException("Profile username : " + username + " does not exist.");
            }

            // Return the SettingsPropertyValueCollection
            return svc;
        }

        /// <summary>
        /// Sets the values of the specified group of property settings.
        /// </summary>
        /// <param name="context">A System.Configuration.SettingsContext describing the current application usage.</param>
        /// <param name="collection">A System.Configuration.SettingsPropertyValueCollection representing the group
        /// of property settings to set.</param>
        public void SetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyValueCollection collection)
        {
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            // Get the profile for the current user name.
            Nequeo.DataAccess.CloudInteraction.Data.Profile profile = GetSpecificProfile(username);

            // If a profile exits.
            if (profile != null)
            {
                long profileID = profile.ProfileID;

                // For each property setting.
                foreach (SettingsPropertyValue pv in collection)
                {
                    bool propertyFound = false;
                    string propertyName = pv.Property.Name;

                    // Get the value for the current property name.
                    object value = GetProfilePropertyValue(profile, isAuthenticated, propertyName, out propertyFound);

                    // If the property has been found.
                    if (propertyFound)
                    {
                        // Update the property.
                        bool retUpdate = new Nequeo.DataAccess.CloudInteraction.Data.Extension.ProfileValue().
                            Update.UpdateItemPredicate(
                                new Data.ProfileValue()
                                {
                                    PropertyValue = pv.PropertyValue.ToString()
                                }, u =>
                                    (u.ProfileID == profileID) &&
                                    (u.PropertyName == propertyName)
                            );
                    }
                    else
                    {
                        // Insert the property.
                        bool retInsert = new Nequeo.DataAccess.CloudInteraction.Data.Extension.ProfileValue().
                            Insert.InsertItem(
                                new Data.ProfileValue()
                                {
                                    ProfileID = profileID,
                                    PropertyName = propertyName,
                                    PropertyType = "System.String",
                                    PropertyValue = pv.PropertyValue.ToString()
                                }
                            );
                    }
                }

                // Updates the LastActivityDate and LastUpdatedDate values when profile properties
                UpdateActivityDates(username, isAuthenticated, false);
            }
            else
            {
                // Make sure that the profile exists for the username if authenticated.
                if (isAuthenticated)
                    throw new ProviderException("Profile username : " + username + " does not exist.");
            }
        }

        /// <summary>
        /// Get the current property value.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="isAuthenticated">Is the user authenticated or anonymous.</param>
        /// <param name="propertyName">The name of the property to search for.</param>
        /// <param name="propertyFound">Has the property been found.</param>
        /// <returns>The property value.</returns>
        private object GetProfilePropertyValue(Nequeo.DataAccess.CloudInteraction.Data.Profile profile, bool isAuthenticated, string propertyName, out bool propertyFound)
        {
            propertyFound = false;

            // Is opposite; is anonymous then not authenticated.
            bool isAnonymous = !isAuthenticated;
            bool isAnonymousProfile = false;

            // Attempt to find the property value for the property name
            // within the current profile id.
            Nequeo.DataAccess.CloudInteraction.Data.ProfileValue profileValue = GetSpecificPropertyValue(profile.ProfileID, propertyName);
            if (profileValue != null)
                propertyFound = true;
            else
                return null;

            // If an anonymous value has been set.
            if (profile.IsAnonymous != null)
            {
                // Get the current is anonymous value.
                isAnonymousProfile = profile.IsAnonymous.Value;

                // If anonymous values match then return the property value
                if (isAnonymousProfile == isAnonymous)
                    return profileValue.PropertyValue;
                else
                    return null;
            }
            else
                return profileValue.PropertyValue;
        }

        /// <summary>
        /// Get the specific profile for the current application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The profile; else null.</returns>
        private Nequeo.DataAccess.CloudInteraction.Data.Profile GetSpecificProfile(string username)
        {
            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile profileExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile();
            Nequeo.DataAccess.CloudInteraction.Data.Profile profile =
                profileExt.Select.SelectDataEntity(
                    u =>
                        (u.Username == username) &&
                        (u.ApplicationName == ApplicationName)
                );

            // Return the profile.
            return profile;
        }

        /// <summary>
        /// Get the profile value for the property name.
        /// </summary>
        /// <param name="profileID">The profile id.</param>
        /// <param name="propertyName">The property name to serach for.</param>
        /// <returns>The profile value; else null.</returns>
        private Nequeo.DataAccess.CloudInteraction.Data.ProfileValue GetSpecificPropertyValue(long profileID, string propertyName)
        {
            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.ProfileValue profileExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.ProfileValue();
            Nequeo.DataAccess.CloudInteraction.Data.ProfileValue profile =
                profileExt.Select.SelectDataEntity(
                    u =>
                        (u.ProfileID == profileID) &&
                        (u.PropertyName == propertyName)
                );

            // Return the profile.
            return profile;
        }

        /// <summary>
        /// Updates the LastActivityDate and LastUpdatedDate values when profile properties are accessed by the
        /// GetPropertyValues and SetPropertyValues methods. Passing true as the activityOnly parameter will update
        /// only the LastActivityDate.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="isAuthenticated">Is the user authenticated or anonymous.</param>
        /// <param name="activityOnly">Update activity date only; else update activity and updated dates.</param>
        private void UpdateActivityDates(string username, bool isAuthenticated, bool activityOnly)
        {
            // Is opposite; is anonymous then not authenticated.
            bool isAnonymous = !isAuthenticated;
            DateTime activityDate = DateTime.Now;
            Data.Profile profile = null;

            // Update activity only
            if (activityOnly)
            {
                // Add profile data.
                profile = new Data.Profile()
                {
                    LastActivityDate = activityDate
                };
            }
            else
            {
                // Add profile data.
                profile = new Data.Profile()
                {
                    LastActivityDate = activityDate,
                    LastUpdatedDate = activityDate
                };
            }

            // Update the profile activity data.
            new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile().
                 Update.UpdateItemPredicate(
                     profile, 
                     u =>
                         (u.Username == username) &&
                         (u.ApplicationName == ApplicationName)
                 );
        }

        /// <summary>
        /// Delete the profile
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>True if complete; else false.</returns>
        private bool DeleteProfile(string username)
        {
            // Attempt to delete the user.
            bool ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile().
                Delete.DeleteItemPredicate(
                    u =>
                        (u.Username == username) &&
                        (u.ApplicationName == ApplicationName)
                );

            // Return the result of the deletion.
            return ret;
        }

        /// <summary>
        /// Delete the profile values
        /// </summary>
        /// <param name="profileID">The profile id..</param>
        /// <returns>True if complete; else false.</returns>
        private bool DeleteProfileValue(long profileID)
        {
            // Attempt to delete the user.
            bool ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.ProfileValue().
                Delete.DeleteItemPredicate(
                    u =>
                        (u.ProfileID == profileID)
                );

            // Return the result of the deletion.
            return ret;
        }

        /// <summary>
        /// Get all inactive profiles.
        /// </summary>
        /// <param name="lastActivityDate">The last activity date to go back to.</param>
        /// <returns>The array of profiles that are inactive.</returns>
        private Nequeo.DataAccess.CloudInteraction.Data.Profile[] GetInactiveProfiles(DateTime lastActivityDate)
        {
            // Get the profiles.
            Nequeo.DataAccess.CloudInteraction.Data.Profile[] profiles =
                new Nequeo.DataAccess.CloudInteraction.Data.Extension.Profile().Select.
                    SelectDataEntitiesPredicate(u =>
                        (u.ApplicationName == ApplicationName) &&
                        (u.LastActivityDate <= lastActivityDate)
                    );

            // Return the prifiles.
            return profiles;
        }
    }
}
