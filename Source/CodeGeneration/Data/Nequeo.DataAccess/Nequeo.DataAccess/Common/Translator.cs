using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// Encapsulates a method that has no parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult>();

    /// <summary>
    /// Encapsulates a method that has one parameter and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1>(T1 parameter1);

    /// <summary>
    /// Encapsulates a method that has two parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2>(T1 parameter1, T2 parameter2);

    /// <summary>
    /// Encapsulates a method that has three parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3);

    /// <summary>
    /// Class that contains members that control the
    /// convertion of object type data to strongly
    /// typed objects.
    /// </summary>
    public class AnonymousTypeFunction
    {
        /// <summary>
        /// Get all public properties within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive properties within.</param>
        /// <returns>The collection of all proerties within the type.</returns>
        private List<PropertyInfo> GetPublicProperties(Type t)
        {
            // Create a new instance of the property collection.
            List<PropertyInfo> properties = new List<PropertyInfo>();

            // Get the base type and the derived type.
            Type type = t;

            // Add the complete property range.
            properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            // Return all the properties within
            // the type.
            return properties;
        }

        /// <summary>
        /// Executes to delegate function for data table projection.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to examine.</typeparam>
        /// <param name="table">The table to project.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The array of data entities.</returns>
        private TEntity[] Projector<TEntity>(DataTable table, FunctionHandler<TEntity[], DataTable> function)
            where TEntity : class, new()
        {
            if (table == null)
                throw new ArgumentNullException("table");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(table);
        }

        /// <summary>
        /// Maps a data table to the corresponding data entities.
        /// </summary>
        /// <typeparam name="TEntity">The ntity type to examine.</typeparam>
        /// <param name="table">The table to map.</param>
        /// <returns>The array of data entities.</returns>
        private TEntity[] Mapper<TEntity>(DataTable table)
            where TEntity : class, new()
        {
            // Create a new instance for the generic data type.
            TEntity[] dataObjectCollection = new TEntity[table.Rows.Count];

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(TEntity));
            int i = 0;

            // For each row within the data collection.
            foreach (DataRow row in table.Rows)
            {
                // Create a new data business 
                // object for each row.
                TEntity data = new TEntity();

                // If properties exist within the data type.
                if (properties.Count > 0)
                {
                    // For each column within the data collection.
                    foreach (System.Data.DataColumn column in table.Columns)
                    {
                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = properties.Find(
                            new Predicate<PropertyInfo>(
                                delegate(PropertyInfo property)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (property.Name.ToLower().TrimStart('_') ==
                                        column.ColumnName.ToLower().TrimStart('_'))
                                    {
                                        // If the data within the current row and column
                                        // is 'NULL' then do not store the data.
                                        if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                            // Assign the current data for the current row
                                            // into the current data business object.
                                            property.SetValue(data, row[column.ColumnName.ToLower()], null);

                                        // Match found.
                                        return true;
                                    }
                                    else
                                        return false;
                                }
                            )
                        );
                    }
                }

                // Add the current data row to the
                // business object collection.
                dataObjectCollection[i++] = data;
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Executes to delegate function for data table projection.
        /// </summary>
        /// <param name="dataTable">The table to project.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The array of data entities.</returns>
        private Object ListGenericTypeProjector(
            DataTable dataTable, Type conversionType, FunctionHandler<Object, DataTable, Type> function)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(dataTable, conversionType);
        }

        /// <summary>
        /// Gets the list generic collection of the conversion type from the data table.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        private Object ListGenericTypeMapper(DataTable dataTable, Type conversionType)
        {
            // Create a new instance for the generic data type.
            Type listGenericType = typeof(List<>);
            Type[] typeArgs = { conversionType };
            Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);
            object listGeneric = Activator.CreateInstance(listGenericTypeConstructor);

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(conversionType);

            // For each row within the data collection.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data business 
                // object for each row.
                object data = Activator.CreateInstance(conversionType);

                // If properties exist within the data type.
                if (properties.Count > 0)
                {
                    // For each column within the data collection.
                    foreach (System.Data.DataColumn column in dataTable.Columns)
                    {
                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = properties.Find(
                            new Predicate<PropertyInfo>(
                                delegate(PropertyInfo property)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (property.Name.ToLower() == column.ColumnName.ToLower())
                                    {
                                        // If the data within the current row and column
                                        // is 'NULL' then do not store the data.
                                        if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                        {
                                            // Assign the current data for the current row
                                            // into the current data business object.
                                            object objResult = row[column.ColumnName.ToLower()];
                                            switch(property.PropertyType.Name.ToLower())
                                            {
                                                case "boolean":
                                                    objResult = Convert.ToBoolean(objResult);
                                                    break;
                                                case "int32":
                                                    objResult = Convert.ToInt32(objResult);
                                                    break;
                                                case "int64":
                                                    objResult = Convert.ToInt64(objResult);
                                                    break;
                                                case "nullable`1":
                                                    // Get the array of generic
                                                    // type parameters.
                                                    Type[] genericArguments = property.PropertyType.GetGenericArguments();
                                                    switch (genericArguments[0].Name.ToLower())
                                                    {
                                                        case "boolean":
                                                            objResult = Convert.ToBoolean(objResult);
                                                            break;
                                                        case "int32":
                                                            objResult = Convert.ToInt32(objResult);
                                                            break;
                                                        case "int64":
                                                            objResult = Convert.ToInt64(objResult);
                                                            break;
                                                    }
                                                    break;
                                            }
                                            // Assign the value to the object.
                                            property.SetValue(data, objResult, null);
                                        }
                                        // Match found.
                                        return true;
                                    }
                                    else
                                        return false;
                                }
                            )
                        );
                    }
                }

                // Get the current object.
                Object[] args = new Object[] { data };

                // Add the current data row to the
                // business object collection.
                listGeneric.GetType().InvokeMember("Add",
                    BindingFlags.DeclaredOnly | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, listGeneric, args);
            }

            // Return the collection.
            return listGeneric;
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to examine.</typeparam>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TEntity[] Translator<TEntity>(DataTable table)
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (table == null) throw new ArgumentNullException("table");

            // Create the delegate function.
            FunctionHandler<TEntity[], DataTable> function =
                (DataTable data) => this.Mapper<TEntity>(data);

            // Project the data table.
            return Projector<TEntity>(table, function);
        }

        /// <summary>
        /// Gets the list generic collection of the conversion type from the data table.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public object ListGenericTypeTranslator(DataTable dataTable, Type conversionType)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");
            if (conversionType == null) throw new ArgumentNullException("conversionType");

            // Create the delegate function.
            FunctionHandler<Object, DataTable, Type> function =
                (DataTable data, Type type) => this.ListGenericTypeMapper(data, type);

            // Project the data table.
            return ListGenericTypeProjector(dataTable, conversionType, function);
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateTable(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("TableName", typeof(String));
            newTable.Columns.Add("TableOwner", typeof(String));

            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data row.
                DataRow rowNew = null;
                rowNew = newTable.NewRow();

                rowNew["TableName"] = row["TABLE_NAME"];
                rowNew["TableOwner"] = row["TABLE_SCHEMA"];

                // Add the current row to the table.
                newTable.Rows.Add(rowNew);
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateTablePrimaryKey(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("PrimaryKeyName", typeof(String));
         
            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data row.
                DataRow rowNew = null;
                rowNew = newTable.NewRow();

                rowNew["PrimaryKeyName"] = row["COLUMN_NAME"];

                // Add the current row to the table.
                newTable.Rows.Add(rowNew);
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateTableColumns(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("ColumnName", typeof(String));
            newTable.Columns.Add("ColumnType", typeof(String));
            newTable.Columns.Add("ColumnNullable", typeof(Boolean));
            newTable.Columns.Add("IsComputed", typeof(Boolean));
            newTable.Columns.Add("ColumnOrder", typeof(Int32));
            newTable.Columns.Add("Length", typeof(Int64));
            newTable.Columns.Add("Precision", typeof(Int64));
            newTable.Columns.Add("PrimaryKeySeed", typeof(Boolean));

            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data row.
                DataRow rowNew = null;
                rowNew = newTable.NewRow();

                rowNew["ColumnName"] = row["COLUMN_NAME"];
                rowNew["ColumnType"] = LinqToDataTypes.GetOleDbType(row["DATA_TYPE"].ToString());
                rowNew["ColumnNullable"] = row["IS_NULLABLE"];
                rowNew["IsComputed"] = 0;
                rowNew["ColumnOrder"] = row["ORDINAL_POSITION"];
                rowNew["Length"] = row["CHARACTER_MAXIMUM_LENGTH"];
                rowNew["Precision"] = row["NUMERIC_PRECISION"];
                rowNew["PrimaryKeySeed"] = 0;

                // Add the current row to the table.
                newTable.Rows.Add(rowNew);
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="dataTableEx">The datatable containing the table data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateTableForeignKey(string tableName, string owner, DataTable dataTable, DataTable dataTableEx)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("TableName", typeof(String));
            newTable.Columns.Add("ColumnName", typeof(String));
            newTable.Columns.Add("ColumnType", typeof(String));
            newTable.Columns.Add("IsNullable", typeof(Boolean));
            newTable.Columns.Add("Length", typeof(Int64));
            newTable.Columns.Add("Precision", typeof(Int64));
            newTable.Columns.Add("ForeignKeyTable", typeof(String));
            newTable.Columns.Add("ForeignKeyColumnName", typeof(String));
            newTable.Columns.Add("ForeignKeyOwner", typeof(String));

            // For each row found.
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["FK_TABLE_NAME"].ToString() == tableName)
                {
                    DataRow columNames = dataTableEx.Select("COLUMN_NAME = '" + row["FK_COLUMN_NAME"] + "'").First();

                    // Create a new data row.
                    DataRow rowNew = null;
                    rowNew = newTable.NewRow();

                    rowNew["TableName"] = row["FK_TABLE_NAME"];
                    rowNew["ColumnName"] = row["FK_COLUMN_NAME"];
                    rowNew["ColumnType"] = LinqToDataTypes.GetOleDbType(columNames["DATA_TYPE"].ToString());
                    rowNew["IsNullable"] = columNames["IS_NULLABLE"];
                    rowNew["Length"] = columNames["CHARACTER_MAXIMUM_LENGTH"];
                    rowNew["Precision"] = columNames["NUMERIC_PRECISION"];
                    rowNew["ForeignKeyTable"] = row["PK_TABLE_NAME"].ToString();
                    rowNew["ForeignKeyColumnName"] = row["PK_COLUMN_NAME"];
                    rowNew["ForeignKeyOwner"] = row["PK_TABLE_SCHEMA"];

                    // Add the current row to the table.
                    newTable.Rows.Add(rowNew);
                }
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="dataTableEx">The datatable containing the table data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateTableReferenceKey(string tableName, string owner, DataTable dataTable, DataTable dataTableEx)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("TableName", typeof(String));
            newTable.Columns.Add("ColumnName", typeof(String));
            newTable.Columns.Add("ColumnType", typeof(String));
            newTable.Columns.Add("IsNullable", typeof(Boolean));
            newTable.Columns.Add("Length", typeof(Int64));
            newTable.Columns.Add("Precision", typeof(Int64));
            newTable.Columns.Add("ForeignKeyTable", typeof(String));
            newTable.Columns.Add("ForeignKeyColumnName", typeof(String));
            newTable.Columns.Add("ForeignKeyOwner", typeof(String));

            // For each row found.
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["PK_TABLE_NAME"].ToString() == tableName)
                {
                    DataRow columNames = null;
                    try{
                        columNames = dataTableEx.Select("COLUMN_NAME = '" + row["FK_COLUMN_NAME"] + "'").First();}
                    catch { }

                    if (columNames == null)
                        columNames = dataTableEx.Select("COLUMN_NAME = '" + row["PK_COLUMN_NAME"] + "'").First();

                    // Create a new data row.
                    DataRow rowNew = null;
                    rowNew = newTable.NewRow();

                    rowNew["TableName"] = row["FK_TABLE_NAME"];
                    rowNew["ColumnName"] = row["FK_COLUMN_NAME"];
                    rowNew["ColumnType"] = LinqToDataTypes.GetOleDbType(columNames["DATA_TYPE"].ToString());
                    rowNew["IsNullable"] = columNames["IS_NULLABLE"];
                    rowNew["Length"] = columNames["CHARACTER_MAXIMUM_LENGTH"];
                    rowNew["Precision"] = columNames["NUMERIC_PRECISION"];
                    rowNew["ForeignKeyTable"] = row["PK_TABLE_NAME"].ToString();
                    rowNew["ForeignKeyColumnName"] = row["PK_COLUMN_NAME"];
                    rowNew["ForeignKeyOwner"] = row["PK_TABLE_SCHEMA"];

                    // Add the current row to the table.
                    newTable.Rows.Add(rowNew);
                }
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateProcedures(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("ProcedureName", typeof(String));
            newTable.Columns.Add("ProcedureOwner", typeof(String));
            newTable.Columns.Add("PackageName", typeof(String));
            newTable.Columns.Add("FunctionRealName", typeof(String));

            char[] delimiter = new char[] { ';' };
            string[] split;

            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                split = row["PROCEDURE_NAME"].ToString().Split(delimiter);

                if (split[1] == "1")
                {
                    // Create a new data row.
                    DataRow rowNew = null;
                    rowNew = newTable.NewRow();
               
                    rowNew["ProcedureName"] = split[0];
                    rowNew["ProcedureOwner"] = row["PROCEDURE_SCHEMA"];
                    rowNew["PackageName"] = string.Empty;
                    rowNew["FunctionRealName"] = string.Empty;

                    // Add the current row to the table.
                    newTable.Rows.Add(rowNew);
                }
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateProcedureColumns(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("ColumnName", typeof(String));
            newTable.Columns.Add("ColumnType", typeof(String));
            newTable.Columns.Add("ColumnNullable", typeof(Boolean));
            newTable.Columns.Add("ColumnOrder", typeof(Int32));
            newTable.Columns.Add("Length", typeof(Int64));
            newTable.Columns.Add("Precision", typeof(Int64));
            newTable.Columns.Add("IsOutParameter", typeof(Boolean));

            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data row.
                DataRow rowNew = null;
                rowNew = newTable.NewRow();

                rowNew["ColumnName"] = row["PARAMETER_NAME"];
                rowNew["ColumnType"] = LinqToDataTypes.GetOleDbType(row["DATA_TYPE"].ToString());
                rowNew["ColumnNullable"] = row["IS_NULLABLE"];
                rowNew["ColumnOrder"] = row["ORDINAL_POSITION"];
                rowNew["Length"] = row["CHARACTER_MAXIMUM_LENGTH"];
                rowNew["Precision"] = row["NUMERIC_PRECISION"];
                rowNew["IsOutParameter"] = (row["PARAMETER_TYPE"].ToString() == "4" ? true : false);

                // Add the current row to the table.
                newTable.Rows.Add(rowNew);
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateFunctions(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("FunctionName", typeof(String));
            newTable.Columns.Add("FunctionOwner", typeof(String));
            newTable.Columns.Add("PackageName", typeof(String));
            newTable.Columns.Add("FunctionRealName", typeof(String));

            char[] delimiter = new char[] { ';' };
            string[] split;

            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                split = row["PROCEDURE_NAME"].ToString().Split(delimiter);

                if (split[1] == "0")
                {
                    // Create a new data row.
                    DataRow rowNew = null;
                    rowNew = newTable.NewRow();

                    rowNew["FunctionName"] = split[0];
                    rowNew["FunctionOwner"] = row["PROCEDURE_SCHEMA"];
                    rowNew["PackageName"] = string.Empty;
                    rowNew["FunctionRealName"] = string.Empty;

                    // Add the current row to the table.
                    newTable.Rows.Add(rowNew);
                }
            }

            // Return the translated table.
            return newTable;
        }

        /// <summary>
        /// Translates an OleDb data table data to table data.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The translated data table</returns>
        public DataTable TranslateFunctionColumns(DataTable dataTable)
        {
            DataTable newTable = new DataTable("TableName");
            newTable.Columns.Add("ColumnName", typeof(String));
            newTable.Columns.Add("ColumnType", typeof(String));
            newTable.Columns.Add("ColumnNullable", typeof(Boolean));
            newTable.Columns.Add("ColumnOrder", typeof(Int32));
            newTable.Columns.Add("Length", typeof(Int64));
            newTable.Columns.Add("Precision", typeof(Int64));
            newTable.Columns.Add("IsOutParameter", typeof(Boolean));

            // For each row founs.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data row.
                DataRow rowNew = null;
                rowNew = newTable.NewRow();

                rowNew["ColumnName"] = row["PARAMETER_NAME"];
                rowNew["ColumnType"] = LinqToDataTypes.GetOleDbType(row["DATA_TYPE"].ToString());
                rowNew["ColumnNullable"] = row["IS_NULLABLE"];
                rowNew["ColumnOrder"] = row["ORDINAL_POSITION"];
                rowNew["Length"] = row["CHARACTER_MAXIMUM_LENGTH"];
                rowNew["Precision"] = row["NUMERIC_PRECISION"];
                rowNew["IsOutParameter"] = (row["PARAMETER_TYPE"].ToString() == "4" ? true : false);

                // Add the current row to the table.
                newTable.Rows.Add(rowNew);
            }

            // Return the translated table.
            return newTable;
        }
    }
}
