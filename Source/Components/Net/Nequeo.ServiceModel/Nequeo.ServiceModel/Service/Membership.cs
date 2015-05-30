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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Compilation;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using Nequeo.Net.ServiceModel.Configuration;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.Configuration;
using Nequeo.Data.DataType;
using Nequeo.Handler.Global;

namespace Nequeo.Net.ServiceModel.Service
{
    /// <summary>
    /// Membership type contract.
    /// </summary>
    [ErrorBehavior(typeof(CustomErrorHandler), "MembershipErrorBehavior")]
    public class Membership : IMembership
    {
        /// <summary>
        /// Set which membership should return data.
        /// </summary>
        public object MembershipType = null;

        /// <summary>
        /// Validate the calling user.
        /// </summary>
        /// <param name="username">The calling username.</param>
        /// <param name="password">The calling password.</param>
        /// <returns>True if the user is valid else false.</returns>
        public bool Validate(string username, string password)
        {
            try
            {
                bool authResult = false;
                ConnectionStringExtensionElement[] items = ConnectionStringExtensionConfigurationManager.ConnectionStringExtensionElements();
                if (items != null)
                {
                    if (items.Count() > 0)
                    {
                        // For each service host  configuration find
                        // the corresponding service type.
                        foreach (ConnectionStringExtensionElement item in items)
                        {
                            // Get the current type name
                            // and create a instance of the type.
                            Type typeName = Type.GetType(item.TypeName, true, true);
                            object typeNameInstance = Activator.CreateInstance(typeName);

                            if (MembershipType == null)
                                MembershipType = this;

                            if (MembershipType != null)
                            {
                                if (MembershipType.GetType().FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                                {
                                    Type dataAccessProviderType = Type.GetType(item.DataAccessProvider, true, true);
                                    ConnectionContext.ConnectionType connectionType = ConnectionContext.ConnectionTypeConverter.GetConnectionType(item.ConnectionType);
                                    ConnectionContext.ConnectionDataType connectionDataType = ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(item.ConnectionDataType);

                                    // Data table containing the data.
                                    DataTable dataTable = null;
                                    string sql =
                                        "SELECT [" + item.IndicatorColumnName + "] " +
                                        "FROM [" + (String.IsNullOrEmpty(item.DatabaseOwner) ? "" : item.DatabaseOwner + "].[") + item.TableName.Replace(".", "].[") + "] " +
                                        "WHERE ([" + item.ComparerColumnName + "] = '" + username + "')";

                                    sql = Nequeo.Data.DataType.DataTypeConversion.
                                        GetSqlConversionDataTypeNoContainer(connectionDataType, sql);

                                    string providerName = null;
                                    string connection = string.Empty;
                                    string connectionString = string.Empty;

                                    // Get the current database connection string
                                    // from the configuration file through the
                                    // specified configuration key.
                                    using (DatabaseConnections databaseConnection = new DatabaseConnections())
                                        connection = databaseConnection.DatabaseConnection(item.ConnectionName, out providerName);

                                    // If empty string is returned then
                                    // value should be the connection string.
                                    if (String.IsNullOrEmpty(connection))
                                        connectionString = item.ConnectionName;
                                    else
                                        connectionString = connection;

                                    // Create an instance of the data access provider
                                    Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                                    // Get the connection type
                                    switch (connectionType)
                                    {
                                        // Get the permission data from the
                                        // database through the sql provider.
                                        case ConnectionContext.ConnectionType.SqlConnection:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;

                                        // Get the permission data from the
                                        // database through the oracle provider.
                                        case ConnectionContext.ConnectionType.PostgreSqlConnection:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;

                                        // Get the permission data from the
                                        // database through the oracle provider.
                                        case ConnectionContext.ConnectionType.OracleClientConnection:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;

                                        // Get the permission data from the
                                        // database through the oracle provider.
                                        case ConnectionContext.ConnectionType.OleDbConnection:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;

                                        // Get the permission data from the
                                        // database through the oracle provider.
                                        case ConnectionContext.ConnectionType.OdbcConnection:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;

                                        // Get the permission data from the
                                        // database through the oracle provider.
                                        case ConnectionContext.ConnectionType.MySqlConnection:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;

                                        default:
                                            dataAccess.ExecuteQuery(ref dataTable, sql,
                                                CommandType.Text, connectionString, true, null);
                                            break;
                                    }

                                    // Permission data exists.
                                    if (dataTable != null)
                                    {
                                        if (dataTable.Rows.Count > 0)
                                        {
                                            string permissionValueItem = dataTable.Rows[0][item.IndicatorColumnName].ToString();
                                            if (!String.IsNullOrEmpty(permissionValueItem))
                                            {
                                                if (password.ToLower() == permissionValueItem.ToLower())
                                                {
                                                    authResult = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // If the user has been validated
                // and autharised then allow connection.
                if (!authResult)
                    throw new FaultException("Unknown Username or Incorrect Password");

                return authResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Membership interface contract.
    /// </summary>
    [ServiceContract(Name = "Membership", SessionMode = SessionMode.NotAllowed)]
    public interface IMembership
    {
        /// <summary>
        /// Validate the calling user.
        /// </summary>
        /// <param name="username">The calling username.</param>
        /// <param name="password">The calling password.</param>
        /// <returns>True if the user is valid else false.</returns>
        [OperationContract(Name = "Validate")]
        bool Validate(string username, string password);
    }

    /// <summary>
    /// Membership service host
    /// </summary>
    public class MembershipServiceHost : ServiceManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MembershipServiceHost()
            : base(typeof(Membership))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public MembershipServiceHost(Uri[] baseAddresses)
            : base(typeof(Membership), baseAddresses)
        {
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        public void OpenServiceHost()
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.Open();
            }
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        /// <param name="binding">The specific binding instance.</param>
        public void OpenServiceHost(System.ServiceModel.Channels.Binding binding)
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.ServiceHost.AddServiceEndpoint(typeof(IMembership), binding, "");
                base.Open();
            }
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        /// <param name="binding">The specific binding instance.</param>
        /// <param name="address">The endpoint address.</param>
        public void OpenServiceHost(System.ServiceModel.Channels.Binding binding, string address)
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.ServiceHost.AddServiceEndpoint(typeof(IMembership), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
    }
}
