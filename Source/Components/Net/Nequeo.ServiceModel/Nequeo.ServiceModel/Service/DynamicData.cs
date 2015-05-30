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

using Nequeo.Data;
using Nequeo.Net.ServiceModel.Configuration;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.Configuration;
using Nequeo.Data.DataType;
using Nequeo.Handler.Global;
using Nequeo.Handler;
using Nequeo.Serialisation;

namespace Nequeo.Net.ServiceModel.Service
{
    /// <summary>
    /// Dynamic data type contract.
    /// </summary>
    [ServiceContract(Name = "DynamicData", SessionMode = SessionMode.NotAllowed)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DynamicData
    {
        /// <summary>
        /// Set which dynamic data return data.
        /// </summary>
        public object DynamicDataType = null;

        /// <summary>
        /// Get the user list data.
        /// </summary>
        /// <returns>The collection of data else null.</returns>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        [OperationContract(Name = "UserListService")]
        public string[] UserListService()
        {
            string[] userList = null;

            try
            {
                ConnectionStringExtensionElement[] items = ConnectionStringExtensionConfigurationManager.ConnectionStringExtensionElements();
                if (items != null)
                {
                    if (items.Count() > 0)
                    {
                        // For each service host  configuration find
                        // the corresponding service type.
                        foreach (ConnectionStringExtensionElement item in items)
                        {
                            if (item.ServiceMethodName.ToLower() == "userlistservice")
                            {
                                // Get the current type name
                                // and create a instance of the type.
                                Type typeName = Type.GetType(item.TypeName, true, true);
                                object typeNameInstance = Activator.CreateInstance(typeName);

                                if (DynamicDataType == null)
                                    DynamicDataType = this;

                                if (DynamicDataType != null)
                                {
                                    if (DynamicDataType.GetType().FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                                    {
                                        Type dataAccessProviderType = Type.GetType(item.DataAccessProvider, true, true);
                                        ConnectionContext.ConnectionType connectionType = ConnectionContext.ConnectionTypeConverter.GetConnectionType(item.ConnectionType);
                                        ConnectionContext.ConnectionDataType connectionDataType = ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(item.ConnectionDataType);

                                        // Data table containing the data.
                                        DataTable dataTable = null;
                                        string sql =
                                            "SELECT [" + item.IndicatorColumnName + "], [" + item.DataObjectPropertyName + "] " +
                                            "FROM [" + (String.IsNullOrEmpty(item.DatabaseOwner) ? "" : item.DatabaseOwner + "].[") + item.TableName.Replace(".", "].[") + "] ";

                                        if ((!String.IsNullOrEmpty(item.ComparerColumnName)) && (!String.IsNullOrEmpty(item.ComparerValue)))
                                            sql += "WHERE ([" + item.ComparerColumnName + "] = '" + item.ComparerValue + "')";

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
                                                List<string> cols = new List<string>();
                                                foreach (DataRow row in dataTable.Rows)
                                                    cols.Add("<a href=\"" + item.ServiceMethodRedirectionUrl + "?" + item.DataObjectPropertyName + "=" +
                                                        row[item.DataObjectPropertyName].ToString() + "\">" + row[item.IndicatorColumnName].ToString() + "</a>");

                                                // Assign the collection.
                                                userList = cols.ToArray();
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
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                LogHandler.WriteTypeMessage(errorMessage, typeof(DynamicData).GetMethod("UserListService"));
            }

            // Return the list of users.
            return (userList == null ? new string[0] : userList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textData"></param>
        /// <returns></returns>
        [OperationContract(Name = "XmlItemListService")]
        public string[] XmlItemListService(string textData)
        {
            string[] itemList = null;

            try
            {

                // Return the list of users.
                return itemList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Post the JSON serialised data fro the DataTable Service web control callback.
        /// </summary>
        /// <param name="iDisplayStart">Start index of the displayed records.</param>
        /// <param name="iDisplayLength">Number of records to display.</param>
        /// <param name="sSearch">The string to serach for.</param>
        /// <param name="bEscapeRegex">Global serach is regular expression of not.</param>
        /// <param name="iColumns">Number of columns</param>
        /// <param name="iSortingCols">The number of columns to sort on.</param>
        /// <param name="iSortCol_0">The index of the column to sort by.</param>
        /// <param name="sSortDir_0">The sorting direction.</param>
        /// <param name="sEcho">Information for data tables to use for rendering</param>
        /// <param name="extensionName">The configuration extension name used to query the database.</param>
        /// <returns>The JSON serialised data.</returns>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        [OperationContract(Name = "PostJSonDataTableService")]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST")]
        public string PostJSonDataTableService(int? iDisplayStart, int iDisplayLength, string sSearch, bool bEscapeRegex, int iColumns,
            int iSortingCols, int iSortCol_0, string sSortDir_0, string sEcho, string extensionName)
        {
            return GetJSonDataTableService(iDisplayStart, iDisplayLength, sSearch, bEscapeRegex, iColumns,
                iSortingCols, iSortCol_0, sSortDir_0, sEcho, extensionName);
        }

        /// <summary>
        /// Get the JSON serialised data fro the DataTable Service web control callback.
        /// </summary>
        /// <param name="iDisplayStart">Start index of the displayed records.</param>
        /// <param name="iDisplayLength">Number of records to display.</param>
        /// <param name="sSearch">The string to serach for.</param>
        /// <param name="bEscapeRegex">Global serach is regular expression of not.</param>
        /// <param name="iColumns">Number of columns</param>
        /// <param name="iSortingCols">The number of columns to sort on.</param>
        /// <param name="iSortCol_0">The index of the column to sort by.</param>
        /// <param name="sSortDir_0">The sorting direction.</param>
        /// <param name="sEcho">Information for data tables to use for rendering</param>
        /// <param name="extensionName">The configuration extension name used to query the database.</param>
        /// <returns>The JSON serialised data.</returns>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        [OperationContract(Name = "GetJSonDataTableService")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetJSonDataTableService(int? iDisplayStart, int iDisplayLength, string sSearch, bool bEscapeRegex, int iColumns,
            int iSortingCols, int iSortCol_0, string sSortDir_0, string sEcho, string extensionName)
        {
            // Create a new json datatable service object.
            JSonDataTableService dataTableObject = 
                new JSonDataTableService() { sEcho = Convert.ToInt32(sEcho).ToString() };

            // Default data to send back.
            string jSonData = "{ \"sEcho\": " + sEcho + ", \"iTotalRecords\": 0, \"iTotalDisplayRecords\": 0, \"aaData\": [] }";

            try
            {
                // Create a new json datatable service object.
                dataTableObject = new JSonDataTableService()
                    { sEcho = Convert.ToInt32(sEcho).ToString() };

                // Get the configuration data.
                ConnectionStringExtensionElement[] items = ConnectionStringExtensionConfigurationManager.ConnectionStringExtensionElements();
                if (items != null)
                {
                    if (items.Count() > 0)
                    {
                        // For each service host  configuration find
                        // the corresponding service type.
                        foreach (ConnectionStringExtensionElement item in items)
                        {
                            if (item.Name.ToLower() == extensionName.ToLower())
                            {
                                // Get the current type name
                                // and create a instance of the type.
                                Type typeName = Type.GetType(item.TypeName, true, true);
                                object typeNameInstance = Activator.CreateInstance(typeName);

                                if (DynamicDataType == null)
                                    DynamicDataType = this;

                                if (DynamicDataType != null)
                                {
                                    if (DynamicDataType.GetType().FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                                    {
                                        Type dataAccessProviderType = Type.GetType(item.DataAccessProvider, true, true);
                                        ConnectionContext.ConnectionType connectionType = ConnectionContext.ConnectionTypeConverter.GetConnectionType(item.ConnectionType);
                                        ConnectionContext.ConnectionDataType connectionDataType = ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(item.ConnectionDataType);

                                        // Build the current data object type and
                                        // the select data model generic type.
                                        Type dataType = Type.GetType(item.DataObjectTypeName, true, true);
                                        Type listGenericType = typeof(SelectDataGenericBase<>);

                                        // Create the generic type parameters
                                        // and create the genric type.
                                        Type[] typeArgs = { dataType };
                                        Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                                        // Create an instance of the data access provider
                                        Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                                        // Add the genric type contructor parameters
                                        // and create the generic type instance.
                                        object[] parameters = new object[] { item.ConnectionName, connectionType, connectionDataType, dataAccess };
                                        object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                                        // Get the current sorting column use this column as the
                                        // searc
                                        string[] columnNames = item.JsonDataTableColumnNames.Split(new char[] { '|' }, StringSplitOptions.None);
                                        PropertyInfo propertyInfo = dataType.GetProperty(columnNames[iSortCol_0]);
                                        string name = propertyInfo.Name;

                                        // Get the current object.
                                        Object[] args = null;
                                        Object[] countArgs = null;

                                        // If a search text exits.
                                        if (!String.IsNullOrEmpty(sSearch))
                                        {
                                            args = new Object[] 
                                            { 
                                                (iDisplayStart != null ? (int)iDisplayStart : 0), 
                                                iDisplayLength,
                                                name + (String.IsNullOrEmpty(sSortDir_0) ? " ASC" : " " + sSortDir_0.ToUpper()),
                                                "(SqlQueryMethods.Like(" + name + ", \"" + sSearch + "%\"))"
                                            };

                                            // Get the current object.
                                            countArgs = new Object[] 
                                            { 
                                                name + " LIKE '" + sSearch + "%'"
                                            };
                                        }
                                        else
                                        {
                                            args = new Object[] 
                                            { 
                                                (iDisplayStart != null ? (int)iDisplayStart : 0), 
                                                iDisplayLength,
                                                name + (String.IsNullOrEmpty(sSortDir_0) ? " ASC" : " " + sSortDir_0.ToUpper())
                                            };
                                        }

                                        // Add the current data row to the
                                        // business object collection.
                                        object retCount = listGeneric.GetType().InvokeMember("GetRecordCount",
                                            BindingFlags.DeclaredOnly | BindingFlags.Public |
                                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                                            null, listGeneric, countArgs);

                                        // Add the current data row to the
                                        // business object collection.
                                        object ret = listGeneric.GetType().InvokeMember("SelectData",
                                            BindingFlags.DeclaredOnly | BindingFlags.Public |
                                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                                            null, listGeneric, args);

                                        // Cast the data object type as an enumerable object,
                                        // get the enumerator.
                                        System.Collections.IEnumerable itemsRet = (System.Collections.IEnumerable)ret;
                                        System.Collections.IEnumerator dataObjects = itemsRet.GetEnumerator();

                                        // Create the data array.
                                        string[,] aaData = 
                                            new string[
                                                (iDisplayLength <= Convert.ToInt32((long)retCount) ? iDisplayLength : Convert.ToInt32((long)retCount)), 
                                                columnNames.Length];

                                        int z = 0;
                                        // Iterate through the collection.
                                        while (dataObjects.MoveNext())
                                        {
                                            // If the current index equals the
                                            // selected index then return
                                            // the data object type.
                                            // Get the property.
                                            for (int i = 0; i < columnNames.Length; i++)
                                            {
                                                // Get the property info the current column
                                                string columnValue = string.Empty;
                                                PropertyInfo property = dataObjects.Current.GetType().GetProperty(columnNames[i]);

                                                try
                                                {
                                                    // Get the current value of the property
                                                    columnValue = property.GetValue(dataObjects.Current, null).ToString();
                                                }
                                                catch { }
                                             
                                                // Add the value to the data two dimentinal array.
                                                aaData[z, i] = columnValue.Trim();
                                            }

                                            // Increamnt the row count.
                                            z++;
                                        }
                                        dataObjects.Reset();

                                        dataTableObject.iTotalRecords = Convert.ToInt32((long)retCount);
                                        dataTableObject.iTotalDisplayRecords = Convert.ToInt32((long)retCount);
                                        dataTableObject.aaData = aaData;

                                        // Serialse the data to a JSON format.
                                        jSonData = JavaObjectNotation.JSonCustomSerializer(dataTableObject);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                string errorMessage = ex.Message;
                LogHandler.WriteTypeMessage(errorMessage, typeof(DynamicData).GetMethod("GetJSonDataTableService"));
            }
            // Return serialised json data.
            return jSonData;
        }
    }
}
