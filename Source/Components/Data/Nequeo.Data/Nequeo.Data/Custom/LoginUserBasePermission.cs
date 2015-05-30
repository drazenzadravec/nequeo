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
using System.Collections;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Data;

using Nequeo.Data.DataType;

namespace Nequeo.Data.Custom
{
    /// <summary>
    /// Login user base security permission, code access class.
    /// </summary>
    [Serializable]
    public sealed class LoginUserBaseSecurityPermission : CodeAccessPermission
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="loginUsername">The login username.</param>
        public LoginUserBaseSecurityPermission(string loginUsername)
        {
            _loginUsername = loginUsername;
        }
        #endregion

        #region Private Fields
        private string _loginUsername = null;
        private string _databaseConnection = null;
        private string _table = null;
        private string _loginUsernameColumn = null;
        private string _permissionColumn = null;
        private string _permissionValue = null;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the login username for security permission.
        /// </summary>
        public string LoginUsername
        {
            get { return _loginUsername; }
            set { _loginUsername = value; }
        }

        /// <summary>
        /// Gets sets, the database connection for security permission.
        /// </summary>
        public string DatabaseConnection
        {
            get { return _databaseConnection; }
            set { _databaseConnection = value; }
        }

        /// <summary>
        /// Gets sets, the database table for security permission.
        /// </summary>
        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        /// <summary>
        /// Gets sets, the table login username column for security permission.
        /// </summary>
        public string LoginUsernameColumn
        {
            get { return _loginUsernameColumn; }
            set { _loginUsernameColumn = value; }
        }

        /// <summary>
        /// Gets sets, the table permission column for security permission.
        /// </summary>
        public string PermissionColumn
        {
            get { return _permissionColumn; }
            set { _permissionColumn = value; }
        }

        /// <summary>
        /// Gets sets, the table permission value for security permission.
        /// </summary>
        public string PermissionValue
        {
            get { return _permissionValue; }
            set { _permissionValue = value; }
        }

        /// <summary>
        /// Gets sets, the connection type. for security permission.
        /// </summary>
        public ConnectionContext.ConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        /// <summary>
        /// Gets sets, the connection data type. for security permission.
        /// </summary>
        public ConnectionContext.ConnectionDataType ConnectionDataType
        {
            get { return _connectionDataType; }
            set { _connectionDataType = value; }
        }

        /// <summary>
        /// Gets sets, the current data access provider
        /// </summary>
        public IDataAccess DataAccessProvider
        {
            get { return _dataAccessProvider; }
            set { _dataAccessProvider = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a code access permission security validater.
        /// </summary>
        /// <returns>The code access permission.</returns>
        new public IPermission Demand()
        {
            if (String.IsNullOrEmpty(_loginUsername))
            {
                throw new ArgumentException("No login username has been specified, execution " +
                    "permission is denied.");
            }
            else
            {
                if (_connectionType == ConnectionContext.ConnectionType.None)
                    throw new ArgumentException("No connection type has been specified, execution " +
                        "permission is denied.");

                if (_connectionDataType == ConnectionContext.ConnectionDataType.None)
                    throw new ArgumentException("No connection data type has been specified, execution " +
                        "permission is denied.");

                if (String.IsNullOrEmpty(_databaseConnection))
                    throw new ArgumentException("No database connection has been specified, execution " +
                        "permission is denied.");

                if (String.IsNullOrEmpty(_table))
                    throw new ArgumentException("No database table has been specified, execution " +
                        "permission is denied.");

                if (String.IsNullOrEmpty(_loginUsernameColumn))
                    throw new ArgumentException("No login username table column has been specified, execution " +
                        "permission is denied.");

                if (String.IsNullOrEmpty(_permissionColumn))
                    throw new ArgumentException("No permission table column has been specified, execution " +
                        "permission is denied.");

                // Look for user permission.
                return UserPermission();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Verifies that the target is the current class type.
        /// </summary>
        /// <param name="target">The target class.</param>
        /// <returns>True if the target is the class type.</returns>
        private bool VerifyType(IPermission target)
        {
            return (target is LoginUserBaseSecurityPermission);
        }

        /// <summary>
        /// Get the login username code access permission.
        /// </summary>
        /// <returns>The code access permission.</returns>
        private IPermission UserPermission()
        {
            // Data table containing the data.
            DataTable dataTable = null;
            string sql = "SELECT [" + _permissionColumn + "] " +
                         "FROM [" + _table.Replace(".", "].[") + "] " +
                         "WHERE ([" + _loginUsernameColumn + "] = '" + _loginUsername + "')";

            sql = Nequeo.Data.DataType.DataTypeConversion.
                GetSqlConversionDataTypeNoContainer(_connectionDataType, sql);

            string providerName = null;
            string connection = string.Empty;
            string connectionString = string.Empty;

            // Get the current database connection string
            // from the configuration file through the
            // specified configuration key.
            using (Nequeo.Handler.Global.DatabaseConnections databaseConnection = new Nequeo.Handler.Global.DatabaseConnections())
                connection = databaseConnection.DatabaseConnection(_databaseConnection, out providerName);

            // If empty string is returned then
            // value should be the connection string.
            if (String.IsNullOrEmpty(connection))
                connectionString = _databaseConnection;
            else
                connectionString = connection;

            // Get the type og connection.
            switch (_connectionType)
            {
                // Get the permission data from the
                // database through the sql provider.
                case ConnectionContext.ConnectionType.SqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, null);
                    break;

                // Get the permission data from the
                // database through the sql provider.
                case ConnectionContext.ConnectionType.SqliteConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, null);
                    break;

                // Get the permission data from the
                // database through the oracle provider.
                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, null);
                    break;

                // Get the permission data from the
                // database through the oracle provider.
                case ConnectionContext.ConnectionType.OracleClientConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, null);
                    break;
                
                // Get the permission data from the
                // database through the oracle provider.
                case ConnectionContext.ConnectionType.OleDbConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, _connectionDataType, null);
                    break;

                // Get the permission data from the
                // database through the oracle provider.
                case ConnectionContext.ConnectionType.OdbcConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, _connectionDataType, null);
                    break;

                // Get the permission data from the
                // database through the oracle provider.
                case ConnectionContext.ConnectionType.MySqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, null);
                    break;

                default:
                    _dataAccessProvider.ExecuteQuery(ref dataTable, sql,
                        CommandType.Text, connectionString, true, _connectionDataType, null);
                    break;
            }

            // Permission data exists.
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    string permissionValueItem = dataTable.Rows[0][_permissionColumn].ToString();
                    if (!String.IsNullOrEmpty(permissionValueItem))
                    {
                        if (_permissionValue.ToLower() == permissionValueItem.ToLower())
                        {
                            return this;
                        }
                    }
                }
            }

            // The current user is not in the group.
            throw new SecurityException("No permission data has been found, " +
                "execution permission is denied.");
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Copy the current object.
        /// </summary>
        /// <returns>The current permission object.</returns>
        public override IPermission Copy()
        {
            string loginUsername = _loginUsername;
            return new LoginUserBaseSecurityPermission(loginUsername);
        }

        /// <summary>
        /// The current union of the current object and the target.
        /// </summary>
        /// <param name="target">The target permission object.</param>
        /// <returns>The current permission object.</returns>
        public override IPermission Union(IPermission target)
        {
            return base.Union(target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem"></param>
        public override void FromXml(SecurityElement elem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override IPermission Intersect(IPermission target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool IsSubsetOf(IPermission target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override SecurityElement ToXml()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// Login user base security permission attribute, code access security class.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor |
        AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class LoginUserBaseSecurityPermissionAttribute : CodeAccessSecurityAttribute
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="action">The SecurityAction action applied.</param>
        public LoginUserBaseSecurityPermissionAttribute(SecurityAction action)
            : base(action)
        {
        }
        #endregion

        #region Private Fields
        private string _loginUsername = null;
        private string _databaseConnection = null;
        private string _table = null;
        private string _loginUsernameColumn = null;
        private string _permissionColumn = null;
        private string _permissionValue = null;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the login username for security permission.
        /// </summary>
        public string LoginUsername
        {
            get { return _loginUsername; }
            set { _loginUsername = value; }
        }

        /// <summary>
        /// Gets sets, the database connection for security permission.
        /// </summary>
        public string DatabaseConnection
        {
            get { return _databaseConnection; }
            set { _databaseConnection = value; }
        }

        /// <summary>
        /// Gets sets, the database table for security permission.
        /// </summary>
        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        /// <summary>
        /// Gets sets, the table login username column for security permission.
        /// </summary>
        public string LoginUsernameColumn
        {
            get { return _loginUsernameColumn; }
            set { _loginUsernameColumn = value; }
        }

        /// <summary>
        /// Gets sets, the table permission column for security permission.
        /// </summary>
        public string PermissionColumn
        {
            get { return _permissionColumn; }
            set { _permissionColumn = value; }
        }

        /// <summary>
        /// Gets sets, the table permission value for security permission.
        /// </summary>
        public string PermissionValue
        {
            get { return _permissionValue; }
            set { _permissionValue = value; }
        }

        /// <summary>
        /// Gets sets, the connection type. for security permission.
        /// </summary>
        public ConnectionContext.ConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        /// <summary>
        /// Gets sets, the connection data type. for security permission.
        /// </summary>
        public ConnectionContext.ConnectionDataType ConnectionDataType
        {
            get { return _connectionDataType; }
            set { _connectionDataType = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a code access permission security validater.
        /// </summary>
        /// <returns>The code access permission.</returns>
        public override IPermission CreatePermission()
        {
            // Create a new instance of the
            // group security permission.
            LoginUserBaseSecurityPermission userPermission =
                new LoginUserBaseSecurityPermission(_loginUsername);

            userPermission.ConnectionType = _connectionType;
            userPermission.ConnectionDataType = _connectionDataType;
            userPermission.DatabaseConnection = _databaseConnection;
            userPermission.LoginUsernameColumn = _loginUsernameColumn;
            userPermission.PermissionColumn = _permissionColumn;
            userPermission.PermissionValue = _permissionValue;
            userPermission.Table = _table;

            // Return the new permission interface.
            return userPermission.Demand();
        }
        #endregion
    }
}
