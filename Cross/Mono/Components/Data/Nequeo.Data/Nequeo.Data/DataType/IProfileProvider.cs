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
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Configuration;
using System.Configuration.Provider;

namespace Nequeo.Data.DataType
{
    /// <summary>
    /// Custom profile provider.
    /// </summary>
    public interface IProfileProvider
    {
        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets sets the provider name.
        /// </summary>
        string ProviderName { get; set; }

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
        int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

        /// <summary>
        /// When overridden in a derived class, deletes profile properties and information
        /// for profiles that match the supplied list of user names.
        /// </summary>
        /// <param name="usernames">A string array of user names for profiles to be deleted.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        int DeleteProfiles(string[] usernames);

        /// <summary>
        /// When overridden in a derived class, deletes profile properties and information
        /// for the supplied list of profiles.
        /// </summary>
        /// <param name="profiles">A System.Web.Profile.ProfileInfoCollection of information about profiles
        /// that are to be deleted.</param>
        /// <returns>The number of profiles deleted from the data source.</returns>
        int DeleteProfiles(ProfileInfoCollection profiles);

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
        ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

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
        ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

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
        ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

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
        ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords);

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
        int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

        /// <summary>
        /// Returns the collection of settings property values for the specified application
        /// instance and settings property group.
        /// </summary>
        /// <param name="context">A System.Configuration.SettingsContext describing the current application use.</param>
        /// <param name="collection">A System.Configuration.SettingsPropertyCollection containing the settings
        /// property group whose values are to be retrieved.</param>
        /// <returns>A System.Configuration.SettingsPropertyValueCollection containing the values
        /// for the specified settings property group.</returns>
        SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection);

        /// <summary>
        /// Sets the values of the specified group of property settings.
        /// </summary>
        /// <param name="context">A System.Configuration.SettingsContext describing the current application usage.</param>
        /// <param name="collection">A System.Configuration.SettingsPropertyValueCollection representing the group
        /// of property settings to set.</param>
        void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection);

    }
}
