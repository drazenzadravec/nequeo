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
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Compilation;

using Nequeo.Data;
using Nequeo.Net.ServiceModel.Configuration;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.Configuration;
using Nequeo.Data.DataType;
using Nequeo.Handler.Global;
using Nequeo.Handler;
using Nequeo.Serialisation;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Custom username and password validator.
    /// </summary>
    public class UsernameSecurity : UserNamePasswordValidator
    {
        /// <summary>
        /// Set which user name password validator should return data.
        /// </summary>
        public object UserNamePasswordValidatorType = null;

        /// <summary>
        /// Custom validation method implementation.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user password.</param>
        public override void Validate(string userName, string password)
        {
            ValidateUser(userName, password);
        }

        /// <summary>
        /// Validates a user to a SQL or integrated account.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <param name="password">The user password.</param>
        private void ValidateUser(string username, string password)
        {
            try
            {
                bool authResult = false;
                ConnectionStringExtensionElement[] items = ConnectionStringExtensionConfigurationManager.ConnectionStringExtensionElements();
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

                        if (UserNamePasswordValidatorType == null)
                            UserNamePasswordValidatorType = this;

                        if (UserNamePasswordValidatorType != null)
                        {
                            if (UserNamePasswordValidatorType.GetType().FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
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

                // If the user has been validated
                // and autharised then allow connection.
                if (!authResult)
                    throw new FaultException("Unknown Username or Incorrect Password");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
