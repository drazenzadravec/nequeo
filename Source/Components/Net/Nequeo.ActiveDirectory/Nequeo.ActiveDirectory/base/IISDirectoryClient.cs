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
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Nequeo.Net.ActiveDirectory
{
    /// <summary>
    /// Internet Information Service (IIS),
    /// iis machine directory services client.
    /// </summary>
    public partial class IISDirectoryClient : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>Context principal relates to the current iis machine.</remarks>
        public IISDirectoryClient()
        {
            OnCreated();
        }
        #endregion

        #region Private Fields
        private bool _disposed = false;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the current user name.
        /// </summary>
        /// <param name="includeDomain">Should the domain be included.</param>
        /// <returns>The current user name.</returns>
        public virtual string GetCurrentUser(bool includeDomain)
        {
            // Get the current login details.
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            // If the domain should be included.
            if (includeDomain)
                // Send the complete identity
                // including domain.
                return identity.Name;
            else
            {
                // Get the length of the domain.
                // Get the starting point do not
                // include the domain.
                int length = Environment.MachineName.Length + 1;
                int startIndex = identity.Name.IndexOf(Environment.MachineName) + length;

                // Get the user name of the
                // current account.
                return identity.Name.Substring(startIndex).Replace("\\", "");
            }
        }

        /// <summary>
        /// Create a new virtual directory on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="physicalPath">The physical path to the directory.</param>
        /// <param name="virtualDirectoryName">The virtual directory name.</param>
        /// <param name="defaultDocument">The defualt document to set.</param>
        /// <returns>True if the virtual directory was created else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// defaultDocument : [document] : default.aspx
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// <para>defaultDocument : [document] : default.aspx</para>
        /// </remarks>
        public virtual bool CreateVirtualDirectory(string iisHostPath, string physicalPath,
            string virtualDirectoryName, string defaultDocument)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(physicalPath))
                throw new System.ArgumentNullException("Physical can not be null.",
                    new System.Exception("A valid physical path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(virtualDirectoryName))
                throw new System.ArgumentNullException("Virtual directory name can not be null.",
                    new System.Exception("A valid virtual directory name should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(defaultDocument))
                throw new System.ArgumentNullException("Default document can not be null.",
                    new System.Exception("A valid default document should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Add the iis virtual directory
            // to the iis collection.
            DirectoryEntry virtName = localMachine.Children.Add(virtualDirectoryName, "IIsWebVirtualDir");

            // Commit the changes for the account.
            virtName.CommitChanges();

            // Assign default properties.
            virtName.Properties["Path"][0] = physicalPath;
            virtName.Properties["DefaultDoc"][0] = defaultDocument;
            virtName.Properties["AccessScript"][0] = true;

            // These properties are necessary for an application to be created.
            virtName.Properties["AppFriendlyName"][0] = virtualDirectoryName;
            virtName.Properties["AppIsolated"][0] = "1";
            virtName.Properties["AppRoot"][0] = "/LM/" + iisHostPath;

            // Commit the changes for the account.
            virtName.CommitChanges();

            // Close the connections.
            virtName.Close();
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Create a new web site on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="websiteID">The unique web site id.</param>
        /// <param name="websiteName">The name of the web site.</param>
        /// <param name="physicalPath">The physical path to the root directory.</param>
        /// <returns>True if the web site was created else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service] : localhost/W3SVC
        /// websiteID : [number] : 454354
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service] : localhost/W3SVC</para>
        /// <para>websiteID : [number] : 454354</para>
        /// </remarks>
        public virtual bool CreateWebSite(string iisHostPath,
            string websiteID, string websiteName, string physicalPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(websiteID))
                throw new System.ArgumentNullException("Web site id can not be null.",
                    new System.Exception("A valid web site id should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(websiteName))
                throw new System.ArgumentNullException("Web site name can not be null.",
                    new System.Exception("A valid web site name should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(physicalPath))
                throw new System.ArgumentNullException("Physical can not be null.",
                    new System.Exception("A valid physical path should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Add the iis web site
            // to the iis collection.
            DirectoryEntry siteName = localMachine.Children.Add(websiteID, "IIsWebServer");

            // Assign the web site properties.
            siteName.Properties["ServerComment"][0] = websiteName;
            siteName.CommitChanges();

            // Commit the changes for the account.
            siteName.CommitChanges();

            // Add the iis web site
            // to the iis collection.
            DirectoryEntry rootName = siteName.Children.Add("Root", "IIsWebVirtualDir");

            // Assign the web site properties.
            rootName.Properties["Path"][0] = physicalPath;
            rootName.Properties["AccessScript"][0] = true;

            // Commit the changes for the account.
            rootName.CommitChanges();

            // Close the connections.
            rootName.Close();
            siteName.Close();
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Set a port number to a web site on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="portNumber">The port number.</param>
        /// <returns>True if the port number was assigned else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID] : localhost/W3SVC/1
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID] : localhost/W3SVC/1</para>
        /// </remarks>
        public virtual bool SetWebSitePortNumber(string iisHostPath, int portNumber)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (portNumber < 1)
                throw new System.ArgumentNullException("Port number not valid.",
                    new System.Exception("The port number must be greater than zero."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Set the web site port number.
            localMachine.Properties["ServerBindings"][0] = ":" + portNumber + ":";

            // Commit the changes for the account.
            localMachine.CommitChanges();

            // Close the connections.
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Create a new web directory on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="webFolderName">The web directory name.</param>
        /// <returns>True if the web directory was created else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// </remarks>
        public virtual bool CreateWebDirectory(string iisHostPath, string webFolderName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(webFolderName))
                throw new System.ArgumentNullException("Web folder name can not be null.",
                    new System.Exception("A valid web folder name should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Add the iis web directory
            // to the iis collection.
            DirectoryEntry virtName = localMachine.Children.Add(webFolderName, "IIsWebDirectory");

            // Commit the changes for the account.
            virtName.CommitChanges();

            // Close the connections.
            virtName.Close();
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Create a new application pool on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="applicationPoolName">The application pool name.</param>
        /// <returns>True if the application pool was created else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools</para>
        /// </remarks>
        public virtual bool CreateApplicationPool(string iisHostPath, string applicationPoolName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(applicationPoolName))
                throw new System.ArgumentNullException("Application pool name can not be null.",
                    new System.Exception("A valid application pool name should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Add the iis virtual directory
            // to the iis collection.
            DirectoryEntry virtName = localMachine.Children.Add(applicationPoolName, "IIsApplicationPool");

            // Commit the changes for the account.
            virtName.CommitChanges();

            // Close the connections.
            virtName.Close();
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Assign the application pool to the virtual directory on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="virtualDirectoryName">The virtual directory name.</param>
        /// <param name="applicationPoolName">The application pool name.</param>
        /// <returns>True if the application pool was assigned.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// </remarks>
        public virtual bool AssignApplicationPool(string iisHostPath,
            string virtualDirectoryName, string applicationPoolName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(virtualDirectoryName))
                throw new System.ArgumentNullException("Virtual directory name can not be null.",
                    new System.Exception("A valid virtual directory name should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(applicationPoolName))
                throw new System.ArgumentNullException("Application pool name can not be null.",
                    new System.Exception("A valid application pool name should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Add the iis virtual directory
            // to the iis collection.
            DirectoryEntry virtName = localMachine.Children.Find(virtualDirectoryName, "IIsWebVirtualDir");

            // Assign the application pool.
            virtName.Invoke("AppCreate3", new object[] { 0, applicationPoolName, true });
            virtName.Properties["AppIsolated"][0] = "2";

            // Commit the changes for the account.
            virtName.CommitChanges();

            // Close the connections.
            virtName.Close();
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Get all applications that are in the pool on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="applicationPoolName">The application pool name.</param>
        /// <returns>Collection of applications in the pool.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools</para>
        /// </remarks>
        public virtual List<String> GetAllApplicationsInPool(
            string iisHostPath, string applicationPoolName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(applicationPoolName))
                throw new System.ArgumentNullException("Application pool name can not be null.",
                    new System.Exception("A valid application pool name should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath + "/" + applicationPoolName);

            // Create a new list collection instance
            // of application names.
            List<String> list = new List<String>();

            // Get all applications in the pool
            object appls = localMachine.Invoke("EnumAppsInPool", null);

            // For each application in the collection.
            foreach (object appl in (System.Collections.IEnumerable)appls)
                list.Add((String)appl);

            // Close the connections.
            localMachine.Close();

            // Return success.
            return list;
        }

        /// <summary>
        /// Get all the application pools on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <returns>Collection of entries with application pools.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools</para>
        /// </remarks>
        public virtual List<DirectoryEntry> GetAllApplicationPools(string iisHostPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Create a new list collection instance
            // of directory entries.
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // Get all the application pools.
            DirectoryEntries pools = localMachine.Children;

            // Get the enumerator of the account collection.
            System.Collections.IEnumerator col = pools.GetEnumerator();

            // For each account found.
            while (col.MoveNext())
            {
                // If the current schema class is 'IIsApplicationPool' then add
                // the entry to the collection, that is, if the current
                // account is of type 'IIsApplicationPool' then add to collection.
                if (((DirectoryEntry)col.Current).SchemaClassName.ToLower() == "IIsApplicationPool".ToLower())
                    list.Add((DirectoryEntry)col.Current);
            }

            // Close the entry to the local machine.
            localMachine.Close();

            // Return the collection of users.
            return list;
        }

        /// <summary>
        /// Get all the web directories on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <returns>Collection of entries with web directories.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// </remarks>
        public virtual List<DirectoryEntry> GetAllWebDirectories(string iisHostPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Create a new list collection instance
            // of directory entries.
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // Get all the application pools.
            DirectoryEntries pools = localMachine.Children;

            // Get the enumerator of the account collection.
            System.Collections.IEnumerator col = pools.GetEnumerator();

            // For each account found.
            while (col.MoveNext())
            {
                // If the current schema class is 'IIsApplicationPool' then add
                // the entry to the collection, that is, if the current
                // account is of type 'IIsApplicationPool' then add to collection.
                if (((DirectoryEntry)col.Current).SchemaClassName.ToLower() == "IIsWebDirectory".ToLower())
                    list.Add((DirectoryEntry)col.Current);
            }

            // Close the entry to the local machine.
            localMachine.Close();

            // Return the collection of users.
            return list;
        }

        /// <summary>
        /// Get all the virtual applications on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <returns>Collection of entries with virtual applications.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// </remarks>
        public virtual List<DirectoryEntry> GetAllVirtualApplications(string iisHostPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Create a new list collection instance
            // of directory entries.
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // Get all the application pools.
            DirectoryEntries pools = localMachine.Children;

            // Get the enumerator of the account collection.
            System.Collections.IEnumerator col = pools.GetEnumerator();

            // For each account found.
            while (col.MoveNext())
            {
                // If the current schema class is 'IIsApplicationPool' then add
                // the entry to the collection, that is, if the current
                // account is of type 'IIsApplicationPool' then add to collection.
                if (((DirectoryEntry)col.Current).SchemaClassName.ToLower() == "IIsWebVirtualDir".ToLower())
                    list.Add((DirectoryEntry)col.Current);
            }

            // Close the entry to the local machine.
            localMachine.Close();

            // Return the collection of users.
            return list;
        }

        /// <summary>
        /// Set IP security to configure IIS to restrict client access 
        /// based on IP addresses or DNS host names. Configuring IP 
        /// security modifies the IPSecurity metabase property. 
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="securityType">The security type to set.</param>
        /// <param name="item">The item value to use to set the security value.</param>
        /// <returns>True if the security was set else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID] : localhost/W3SVC/1
        /// iisHostPath : [servername]/[service] : localhost/W3SVC
        /// iisHostPath : [servername]/[service]/[smtpID] : localhost/SMTPSVC/1
        /// iisHostPath : [servername]/[service] : localhost/SMTPSVC
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID] : localhost/W3SVC/1</para>
        /// <para>iisHostPath : [servername]/[service] : localhost/W3SVC</para>
        /// <para>iisHostPath : [servername]/[service]/[smtpID] : localhost/SMTPSVC/1</para>
        /// <para>iisHostPath : [servername]/[service] : localhost/SMTPSVC</para>
        /// </remarks>
        public virtual bool SetSecurity(string iisHostPath,
            SecurityType securityType, string item)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(item))
                throw new System.ArgumentNullException("Item can not be null.",
                    new System.Exception("A valid itemshould be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath);

            // Loads the data into the cache.
            localMachine.RefreshCache();

            // Get the IPSecurity object, this object is a type
            // that contains fields, properties and methods.
            object ipSecurity = localMachine.Invoke("Get", new string[] { "IPSecurity" });

            // Get the IPSecurity type.
            Type type = ipSecurity.GetType();

            // Invoke the IPSecurity member, in this case the
            // member is a property with the securityType name.
            // Get this property in the IPSecurity object, in this
            // case the property is an array of data.
            Array data = (Array)type.InvokeMember(securityType.ToString(),
                BindingFlags.GetProperty, null, ipSecurity, null);

            // Search for the item in the array of data
            // indicate if the item already exists.
            bool exists = false;
            foreach (object dataItem in data)
                if (dataItem.ToString().StartsWith(item))
                    exists = true;

            // If the item already exists.
            if (exists)
                // Throw a new exception indicating that
                // the item already exists.
                throw new System.Exception(item + " already exists in " + securityType.ToString());
            else
            {
                // Create a new object array this array will
                // contain all the current data from the member
                // and one more.
                object[] newData = new object[data.Length + 1];

                // Copy the current member data to the new
                // data member object.
                data.CopyTo(newData, 0);

                // Set the last item in the new array
                // with the new value.
                newData.SetValue(item, data.Length);

                // Invoke the IPSecurity member, in this case the
                // member is a property with the securityType name.
                // Set this property in the IPSecurity object with the new data.
                type.InvokeMember(securityType.ToString(),
                    BindingFlags.SetProperty, null, ipSecurity, new object[] { newData });

                // Invoke the IP Security property, put
                // the new data set in the IPSecurity object
                // to the current iis host path.
                localMachine.Invoke("Put", new object[] { "IPSecurity", ipSecurity });

                // Commit the changes for the account.
                localMachine.CommitChanges();
            }

            // Close the connections.
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Deletes the specified virtual directory on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="virtualDirectoryName">The virtual directory name.</param>
        /// <returns>True if the virtual directory was deleted else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// </remarks>
        public virtual bool DeleteVirtualDirectory(string iisHostPath, string virtualDirectoryName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(virtualDirectoryName))
                throw new System.ArgumentNullException("Virtual directory name can not be null.",
                    new System.Exception("A valid virtual directory name should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath + "/" + virtualDirectoryName);

            // Delete the current virtual directory
            // and all the virtual directories within
            // the virtual directory.
            localMachine.DeleteTree();
            localMachine.CommitChanges();

            // Close the connections.
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Deletes the specified application pool on the iis host.
        /// </summary>
        /// <param name="iisHostPath">The iis host path.</param>
        /// <param name="applicationPoolName">The application pool name.</param>
        /// <returns>True if the application pool was deleted else false.</returns>
        /// <example>
        /// iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools
        /// </example>
        /// <remarks>
        /// <para>iisHostPath : [servername]/[service]/[AppPools] : localhost/W3SVC/AppPools</para>
        /// </remarks>
        public virtual bool DeleteApplicationPool(string iisHostPath, string applicationPoolName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(iisHostPath))
                throw new System.ArgumentNullException("IIS path can not be null.",
                    new System.Exception("A valid IIS path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(applicationPoolName))
                throw new System.ArgumentNullException("Application pool name can not be null.",
                    new System.Exception("A valid application pool name should be specified."));

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "IIS://" + iisHostPath + "/" + applicationPoolName);

            // Delete the current application pool
            // and all the application pools within
            // the virtual directory.
            localMachine.DeleteTree();
            localMachine.CommitChanges();

            // Close the connections.
            localMachine.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Copies a virtual directory from one location to another.
        /// </summary>
        /// <param name="iisHostPathOriginal">The iis host path containing the virtual directory.</param>
        /// <param name="virtualDirectoryNameOriginal">The virtual directory to copy.</param>
        /// <param name="iisHostPathNew">The iis host path where the virtual directory is to be copied.</param>
        /// <param name="virtualDirectoryNameNew">The new name of the virtual directory.</param>
        /// <returns>True if the virtual directory was copied else false.</returns>
        /// <example>
        /// iisHostPathOriginal : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// iisHostPathNew : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/6/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPathOriginal : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// <para>iisHostPathNew : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/6/Root</para>
        /// </remarks>
        public virtual bool CopyVirtualDirectory(string iisHostPathOriginal, string virtualDirectoryNameOriginal,
            string iisHostPathNew, string virtualDirectoryNameNew)
        {
            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachineOriginal = new DirectoryEntry(
                "IIS://" + iisHostPathOriginal + "/" + virtualDirectoryNameOriginal);

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachineNew = new DirectoryEntry(
                "IIS://" + iisHostPathNew);

            // If the schema class names are equal, that is,
            // each schema is the same type.
            if (localMachineOriginal.Parent.SchemaClassName.ToString() == localMachineNew.SchemaClassName.ToString())
            {
                // Copy the original virtual directory
                // to the new virtual directory.
                DirectoryEntry newPath = localMachineOriginal.CopyTo(localMachineNew, virtualDirectoryNameNew);

                // Commit the changes to the new metabase
                // and close the connection.
                localMachineNew.CommitChanges();
                newPath.Close();
            }
            else
                throw new Exception(String.Format("Failed in Copy Metabase Node; parent of {0} " +
                    "is not the same object as {1}.", iisHostPathOriginal, iisHostPathNew));

            // Close the connections.
            localMachineOriginal.Close();
            localMachineNew.Close();

            // Return success.
            return true;
        }

        /// <summary>
        /// Moves a virtual directory from one location to another.
        /// </summary>
        /// <param name="iisHostPathOriginal">The iis host path containing the virtual directory.</param>
        /// <param name="virtualDirectoryNameOriginal">The virtual directory to move.</param>
        /// <param name="iisHostPathNew">The iis host path where the virtual directory is to be moved.</param>
        /// <param name="virtualDirectoryNameNew">The new name of the virtual directory.</param>
        /// <returns>True if the virtual directory was moved else false.</returns>
        /// <example>
        /// iisHostPathOriginal : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root
        /// iisHostPathNew : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/6/Root
        /// </example>
        /// <remarks>
        /// <para>iisHostPathOriginal : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/1/Root</para>
        /// <para>iisHostPathNew : [servername]/[service]/[websiteID]/[Root] : localhost/W3SVC/6/Root</para>
        /// </remarks>
        public virtual bool MoveVirtualDirectory(string iisHostPathOriginal, string virtualDirectoryNameOriginal,
            string iisHostPathNew, string virtualDirectoryNameNew)
        {
            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachineOriginal = new DirectoryEntry(
                "IIS://" + iisHostPathOriginal + "/" + virtualDirectoryNameOriginal);

            // Create a new directory entry
            // instance to the iis machine.
            DirectoryEntry localMachineNew = new DirectoryEntry(
                "IIS://" + iisHostPathNew);

            // If the schema class names are equal, that is,
            // each schema is the same type.
            if (localMachineOriginal.Parent.SchemaClassName.ToString() == localMachineNew.SchemaClassName.ToString())
            {
                // Copy the original virtual directory
                // to the new virtual directory.
                localMachineOriginal.MoveTo(localMachineNew, virtualDirectoryNameNew);

                // Commit the changes to the new metabase
                // and close the connection.
                localMachineNew.CommitChanges();
            }
            else
                throw new Exception(String.Format("Failed in Move Metabase Node; parent of {0} " +
                    "is not the same object as {1}.", iisHostPathOriginal, iisHostPathNew));

            // Close the connections.
            localMachineOriginal.Close();
            localMachineNew.Close();

            // Return success.
            return true;
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~IISDirectoryClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
