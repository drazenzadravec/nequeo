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
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Data.Common;
using System.ComponentModel;

using Nequeo.Data.Custom;
using Nequeo.Linq.Extension;
using Nequeo.Data.DataType;

namespace Nequeo.Data.Control
{
    /// <summary>
    /// Abstract base class for all data objects.
    /// </summary>
    [Serializable]
    [DataContract(Name = "DataBase", IsReference = true)]
    public abstract class DataBase : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        #region DataBase Abstract Class

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected DataBase()
        {
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Property changed indicatror.
        /// </summary>
        [XmlIgnore()]
        protected System.Collections.Generic.List<string> _changedPropertyNames;
        #endregion

        #region Public Events And Methods
        /// <summary>
        /// Property changing event, triggered when a property is changing.
        /// </summary>
        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Property change event, triggered when a property changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Executes the property changing event handle for the attached event.
        /// </summary>
        protected virtual void SendPropertyChanging(string propertyName)
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Executes the property change event handle for the attached event.
        /// </summary>
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            if ((this._changedPropertyNames == null))
            {
                this._changedPropertyNames = new System.Collections.Generic.List<string>();
            }
            if ((!this._changedPropertyNames.Contains(propertyName)))
            {
                this._changedPropertyNames.Add(propertyName);
            }
        }

        /// <summary>
        /// Have any of the properties changed.
        /// </summary>
        /// <returns>True if at least one property has changed.</returns>
        public bool HasChanged()
        {
            return this._changedPropertyNames.Count > 0 ? true : false;
        }

        /// <summary>
        /// Has the specified property chamged.
        /// </summary>
        /// <param name="propertyName">The property name to examine.</param>
        /// <returns>True if the property has changed.</returns>
        public bool HasChanged(string propertyName)
        {
            return this._changedPropertyNames.Contains(propertyName);
        }
        #endregion

        #region Disposable class

        #region Private Fields
        private bool _disposed = false;
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
            if (!this._disposed)
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
        ~DataBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// Class that contains anonymous type data.
    /// </summary>
    [DataContract(Name = "AnonymousType", IsReference = true)]
    [Serializable()]
    [DataTable("dbo.AnonymousType")]
    public class AnonymousType : DataBase
    {
        #region Private Fields
        private String _propertyName = null;
        private Object _propertyValue = null;
        private Type _propertyType = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the property name.
        /// </summary>
        [DataMember(Name = "PropertyName")]
        [XmlElement(ElementName = "PropertyName", IsNullable = false)]
        [DataColumn("PropertyName", DbType = "varchar", Length = 8, IsNullable = false)]
        public String PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property value.
        /// </summary>
        [DataMember(Name = "PropertyValue")]
        [XmlElement(ElementName = "PropertyValue", IsNullable = false)]
        [DataColumn("PropertyName", DbType = "sql_variant", Length = 8, IsNullable = false)]
        public Object PropertyValue
        {
            get { return _propertyValue; }
            set { _propertyValue = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [DataMember(Name = "PropertyType")]
        [XmlElement(ElementName = "PropertyType", IsNullable = false)]
        [DataColumn("PropertyType", DbType = "sql_variant", Length = 8, IsNullable = false)]
        public Type PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class that contains members that control the
    /// convertion of object type data to strongly
    /// typed objects.
    /// </summary>
    public sealed class AnonymousTypeFunction : Nequeo.Handler.LogHandler, IAnonymousTypeFunction, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly AnonymousTypeFunction Instance = new AnonymousTypeFunction();

        /// <summary>
        /// Static constructor
        /// </summary>
        static AnonymousTypeFunction() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public AnonymousTypeFunction()
            : base(applicationName, eventNamespace)
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo.Data";
        private const string eventNamespace = "Nequeo.Data.Control";
        #endregion

        #region Private Methods
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
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc model type.</typeparam>
        /// <param name="mvcModel">The mvc model data.</param>
        /// <param name="function">The delegate that will execute the function.</param>
        /// <returns>The collection of translated data entities.</returns>
        private TDataEntity[] ProjectorMvcMetadataType<TDataEntity, TEntity>(TEntity[] mvcModel, Nequeo.Threading.FunctionHandler<TDataEntity[], TEntity[]> function)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            if (mvcModel == null)
                throw new ArgumentNullException("mvcModel");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(mvcModel);
        }

        /// <summary>
        /// Executes to delegate function for data table projection.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc model type.</typeparam>
        /// <param name="dataEntity">The data entity data.</param>
        /// <param name="function">The delegate that will execute the function.</param>
        /// <returns>The collection of translated data entities.</returns>
        private TEntity[] ProjectorMvcMetadataType<TDataEntity, TEntity>(TDataEntity[] dataEntity, Nequeo.Threading.FunctionHandler<TEntity[], TDataEntity[]> function)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            if (dataEntity == null)
                throw new ArgumentNullException("dataEntity");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(dataEntity);
        }

        /// <summary>
        /// Executes to delegate function for data table projection.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to examine.</typeparam>
        /// <param name="table">The table to project.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The array of data entities.</returns>
        private TEntity[] Projector<TEntity>(DataTable table, Nequeo.Threading.FunctionHandler<TEntity[], DataTable> function)
            where TEntity : class, new()
        {
            if (table == null)
                throw new ArgumentNullException("table");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(table);
        }

        /// <summary>
        /// Executes to delegate function for data entity projection.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to examine.</typeparam>
        /// <param name="items">The type items to convert.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The data table containing the data.</returns>
        private DataTable Projector<TEntity>(TEntity[] items, string tableName, Nequeo.Threading.FunctionHandler<DataTable, TEntity[], String> function)
            where TEntity : class, new()
        {
            if (items == null)
                throw new ArgumentNullException("items");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(items, tableName);
        }

        /// <summary>
        /// Maps mvc model data to the corresponding data entities.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc model type.</typeparam>
        /// <param name="mvcModel">The mvc model data.</param>
        /// <returns>The translated collection of mvc model data.</returns>
        private TDataEntity[] MapperMvcMetadataType<TDataEntity, TEntity>(TEntity[] mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Create a new instance for the generic data type.
            TDataEntity[] dataObjectCollection = new TDataEntity[mvcModel.Count()];
            int i = 0;

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(TEntity));
            List<PropertyInfo> entityProperties = GetPublicProperties(typeof(TDataEntity));

            // For each item within the data collection.
            foreach (TEntity item in mvcModel)
            {
                // Create a new data business 
                // object for each row.
                TDataEntity data = new TDataEntity();

                // If properties exist within the data type.
                if (properties.Count > 0)
                {
                    // For each property in the mvc model
                    foreach (PropertyInfo info in properties)
                    {
                        object value = info.GetValue(item, null);

                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = entityProperties.First(p => p.Name.ToLower() == info.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the data within the current row and column
                            // is 'NULL' then do not store the data.
                            if (value != null)
                                propertyInfo.SetValue(data, value, null);
                        }
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
        /// Maps mvc model data to the corresponding data entities.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc model type.</typeparam>
        /// <param name="dataEntity">The data entity data.</param>
        /// <returns>The translated collection of mvc model data.</returns>
        private TEntity[] MapperMvcMetadataType<TDataEntity, TEntity>(TDataEntity[] dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Create a new instance for the generic data type.
            TEntity[] dataObjectCollection = new TEntity[dataEntity.Count()];
            int i = 0;

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(TDataEntity));
            List<PropertyInfo> entityProperties = GetPublicProperties(typeof(TEntity));

            // For each item within the data collection.
            foreach (TDataEntity item in dataEntity)
            {
                // Create a new data business 
                // object for each row.
                TEntity data = new TEntity();

                // If properties exist within the data type.
                if (properties.Count > 0)
                {
                    // For each property in the mvc model
                    foreach (PropertyInfo info in properties)
                    {
                        object value = info.GetValue(item, null);

                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = entityProperties.First(p => p.Name.ToLower() == info.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the data within the current row and column
                            // is 'NULL' then do not store the data.
                            if (value != null)
                                propertyInfo.SetValue(data, value, null);
                        }
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
                    foreach (DataColumn column in table.Columns)
                    {
                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = properties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the current property within the property collection
                            // is the current column within the data collection.
                            if (propertyInfo.Name.ToLower().TrimStart('_') ==
                                column.ColumnName.ToLower().TrimStart('_'))
                            {
                                // If the data within the current row and column
                                // is 'NULL' then do not store the data.
                                if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                    // Assign the current data for the current row
                                    // into the current data business object.
                                    propertyInfo.SetValue(data, row[column.ColumnName.ToLower()], null);
                            }
                        }
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
        /// Maps a data table to the corresponding data entities.
        /// </summary>
        /// <typeparam name="TEntity">The ntity type to examine.</typeparam>
        /// <param name="items">The collection of type items</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The array of data entities.</returns>
        private DataTable Mapper<TEntity>(TEntity[] items, string tableName)
            where TEntity : class, new()
        {
            // Create a new data table instance.
            DataTable dataTable = new DataTable(tableName);

            // If data was returned.
            if (items != null)
            {
                // Get all the properties within the type.
                PropertyInfo[] properties = items.GetType().GetProperties();

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in properties)
                {
                    try
                    {
                        // Find nullable types
                        switch (propertyItem.PropertyType.Name.ToLower())
                        {
                            case "nullable`1":
                                // Get the array of generic
                                // type parameters.
                                Type[] genericArguments = propertyItem.PropertyType.GetGenericArguments();

                                // Create a new column and assign
                                // each of the properties on the column.
                                DataColumn columnGen = new DataColumn();
                                columnGen.DataType = genericArguments[0];
                                columnGen.ColumnName = propertyItem.Name;
                                dataTable.Columns.Add(columnGen);
                                break;

                            default:
                                // Create a new column and assign
                                // each of the properties on the column.
                                DataColumn column = new DataColumn();
                                column.DataType = propertyItem.PropertyType;
                                column.ColumnName = propertyItem.Name;
                                dataTable.Columns.Add(column);
                                break;
                        }
                    }
                    catch { }
                }

                // For each item in the collection.
                foreach (TEntity item in items)
                {
                    // Create a new data row.
                    DataRow row = null;
                    row = dataTable.NewRow();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in properties)
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(item, null);

                        try
                        {
                            // Assign the current row column
                            // value for the property.
                            row[propertyItem.Name] = value;
                        }
                        catch { }
                    }

                    // Add the current row to the table.
                    dataTable.Rows.Add(row);
                }
            }

            // Return the data table.
            return dataTable;
        }

        /// <summary>
        /// Executes to delegate function for data table projection.
        /// </summary>
        /// <param name="dataTable">The table to project.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The array of data entities.</returns>
        private Object ListGenericTypeProjector(
            DataTable dataTable, Type conversionType, Nequeo.Threading.FunctionHandler<Object, DataTable, Type> function)
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
                try
                {
                    // Create a new data business 
                    // object for each row.
                    object data = Activator.CreateInstance(conversionType);

                    // If properties exist within the data type.
                    if (properties.Count > 0)
                    {
                        // For each column within the data collection.
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            // Find in the propert collection the current propert that matches
                            // the current column. Use the Predicate delegate object to
                            // initiate a search for the specified match.
                            PropertyInfo propertyInfo = null;
                            try
                            {
                                propertyInfo = properties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                            }
                            catch { }
                            if (propertyInfo != null)
                            {
                                // If the current property within the property collection
                                // is the current column within the data collection.
                                if (propertyInfo.Name.ToLower() == column.ColumnName.ToLower())
                                {
                                    // If the data within the current row and column
                                    // is 'NULL' then do not store the data.
                                    if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                        // Assign the current data for the current row
                                        // into the current data business object.
                                        propertyInfo.SetValue(data, row[column.ColumnName.ToLower()], null);
                                }
                            }
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
                catch (System.Exception ex)
                {
                    // Throw a general exception.
                    throw new System.Exception(ex.Message, ex.InnerException);
                }
            }

            // Return the collection.
            return listGeneric;
        }

        /// <summary>
        /// Executes to delegate function for data row projection.
        /// </summary>
        /// <param name="dataRow">The row to project.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The array of data entities.</returns>
        private Object TypeProjector(
            DataRow dataRow, Type conversionType, Nequeo.Threading.FunctionHandler<Object, DataRow, Type> function)
        {
            if (dataRow == null)
                throw new ArgumentNullException("dataRow");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(dataRow, conversionType);
        }

        /// <summary>
        /// Gets the type of the conversion type from the data row.
        /// </summary>
        /// <param name="dataRow">The data row containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        private Object TypeMapper(DataRow dataRow, Type conversionType)
        {
            // Create a new instance for the generic data type.
            object data = Activator.CreateInstance(conversionType);

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(conversionType);

            // If properties exist within the data type.
            if (properties.Count > 0)
            {
                // For each column within the data collection.
                foreach (DataColumn column in dataRow.Table.Columns)
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
                                    if (dataRow[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                        // Assign the current data for the current row
                                        // into the current data business object.
                                        property.SetValue(data, dataRow[column.ColumnName.ToLower()], null);

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

            // Return the data.
            return data;
        }

        /// <summary>
        /// Executes to delegate function for data row projection.
        /// </summary>
        /// <typeparam name="TDestination">The destinamtion type.</typeparam>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <param name="destination">The destination instance</param>
        /// <param name="source">The source instamce.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The destination property mapped instance.</returns>
        private TDestination ProjectorPropertyMapperType<TDestination, TSource>(TDestination destination, TSource source,
            Nequeo.Threading.FunctionHandler<TDestination, TDestination, TSource> function)
            where TDestination : class, new()
            where TSource : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (source == null) throw new ArgumentNullException("source");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(destination, source);
        }

        /// <summary>
        /// Maps the source to the destination for each specifed property
        /// </summary>
        /// <typeparam name="TDestination">The destinamtion type.</typeparam>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <param name="destination">The destination instance</param>
        /// <param name="source">The source instamce.</param>
        /// <returns>The destination property mapped instance.</returns>
        private TDestination PropertyMapperType<TDestination, TSource>(TDestination destination, TSource source)
            where TDestination : class, new()
            where TSource : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (destination == null) throw new ArgumentNullException("destination");
            if (source == null) throw new ArgumentNullException("source");

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> destinationProperties = GetPublicProperties(typeof(TDestination));
            List<PropertyInfo> sourceProperties = GetPublicProperties(typeof(TSource));

            // If destination properties exist.
            if (destinationProperties.Count > 0)
            {
                // For each property in the destination type.
                foreach (PropertyInfo item in destinationProperties)
                {
                    try
                    {
                        bool propertyTypeMappingFound = false;
                        string sourcePropertyName = string.Empty;

                        // For each attribute on each property
                        // in the type.
                        foreach (object attribute in item.GetCustomAttributes(true))
                        {
                            // If the attribute is the
                            // linq column attribute.
                            if (attribute is Nequeo.Data.Custom.PropertyMappingAttribute)
                            {
                                // Cast the current attribute.
                                Nequeo.Data.Custom.PropertyMappingAttribute att =
                                    (Nequeo.Data.Custom.PropertyMappingAttribute)attribute;

                                // If the source type in the attribute has been found.
                                if (att.MappingSource == typeof(TSource))
                                {
                                    propertyTypeMappingFound = true;
                                    sourcePropertyName = att.Name;
                                    break;
                                }
                            }
                        }

                        // If the source type has been found.
                        if (propertyTypeMappingFound)
                        {
                            // If the source property name has been defined.
                            if (!String.IsNullOrEmpty(sourcePropertyName))
                            {
                                // Find in the propert collection the current propert that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                PropertyInfo propertyInfo = null;
                                try
                                {
                                    propertyInfo = sourceProperties.First(p => p.Name.ToLower() == sourcePropertyName.ToLower());
                                }
                                catch { }
                                if (propertyInfo != null)
                                {
                                    // Assign the source value to the destination.
                                    object sourceValue = propertyInfo.GetValue(source, null);
                                    item.SetValue(destination, sourceValue, null);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            // Return the original destination.
            return destination;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Maps the source to the destination for each specifed property
        /// </summary>
        /// <typeparam name="TDestination">The destinamtion type.</typeparam>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <param name="destination">The destination instance</param>
        /// <param name="source">The source instamce.</param>
        /// <returns>The destination property mapped instance.</returns>
        public TDestination PropertyMappingTranslator<TDestination, TSource>(TDestination destination, TSource source)
            where TDestination : class, new()
            where TSource : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (destination == null) throw new ArgumentNullException("destination");
            if (source == null) throw new ArgumentNullException("source");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TDestination, TDestination, TSource> function =
                ((TDestination des, TSource sou) => this.PropertyMapperType<TDestination, TSource>(des, sou));

            // Project the type.
            return ProjectorPropertyMapperType<TDestination, TSource>(destination, source, function);
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to examine.</typeparam>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public TEntity[] Translator<TEntity>(DataTable table)
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (table == null) throw new ArgumentNullException("table");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TEntity[], DataTable> function =
                ((DataTable data) => this.Mapper<TEntity>(data));

            // Project the data table.
            return Projector<TEntity>(table, function);
        }

        /// <summary>
        /// Translates the mvc model type to the data entity type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="mvcModel">The mvc data model</param>
        /// <returns>The translated data entity collection.</returns>
        public TDataEntity[] MvcMetadataTypeTranslator<TDataEntity, TEntity>(TEntity[] mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (mvcModel == null) throw new ArgumentNullException("mvcModel");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TDataEntity[], TEntity[]> function =
                ((TEntity[] data) => this.MapperMvcMetadataType<TDataEntity, TEntity>(data));

            // Project the data table.
            return ProjectorMvcMetadataType<TDataEntity, TEntity>(mvcModel, function);
        }

        /// <summary>
        /// Translates the mvc model type to the data entity type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="mvcModel">The mvc data model</param>
        /// <returns>The translated data entity collection.</returns>
        public TDataEntity MvcMetadataTypeTranslator<TDataEntity, TEntity>(TEntity mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (mvcModel == null) throw new ArgumentNullException("mvcModel");

            TEntity[] mvcModelACol = new TEntity[] { mvcModel };

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TDataEntity[], TEntity[]> function =
                ((TEntity[] data) => this.MapperMvcMetadataType<TDataEntity, TEntity>(data));

            // Project the data table.
            return ProjectorMvcMetadataType<TDataEntity, TEntity>(mvcModelACol, function).First();
        }

        /// <summary>
        /// Translates the data entity type to the mvc model type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        public TEntity[] MvcMetadataTypeTranslator<TDataEntity, TEntity>(TDataEntity[] dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TEntity[], TDataEntity[]> function =
                ((TDataEntity[] data) => this.MapperMvcMetadataType<TDataEntity, TEntity>(data));

            // Project the data table.
            return ProjectorMvcMetadataType<TDataEntity, TEntity>(dataEntity, function);
        }

        /// <summary>
        /// Translates the data entity type to the mvc model type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        public TEntity MvcMetadataTypeTranslator<TDataEntity, TEntity>(TDataEntity dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            TDataEntity[] mvcModelACol = new TDataEntity[] { dataEntity };

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TEntity[], TDataEntity[]> function =
                ((TDataEntity[] data) => this.MapperMvcMetadataType<TDataEntity, TEntity>(data));

            // Project the data table.
            return ProjectorMvcMetadataType<TDataEntity, TEntity>(mvcModelACol, function).First();
        }

        /// <summary>
        /// Translate a collection of data entities to the corresponding data table.
        /// </summary>
        /// <typeparam name="T">The entity type to examine.</typeparam>
        /// <param name="items">The collection of type items</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public DataTable Translator<T>(T[] items, string tableName)
            where T : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (items == null) throw new ArgumentNullException("items");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<DataTable, T[], String> function =
                (T[] itemsColl, string tableNameValue) => this.Mapper<T>(itemsColl, tableNameValue);

            // Project the data table.
            return Projector<T>(items, tableName, function);
        }

        /// <summary>
        /// Convert all the IQueryable data into a array of
        /// anonymous types.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of anonymous type data.</returns>
        public AnonymousType[] GetAnonymousTypeData(IQueryable query)
        {
            // Create a new anonymous type object.
            List<AnonymousType> anonymous = new List<AnonymousType>();

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get all the properties within the type.
                PropertyInfo[] properties = query.ElementType.GetProperties();

                // For each item in the collection.
                foreach (var result in query)
                {
                    // For each property found set the
                    // anonymous type data.
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            // Get the property information.
                            string propertyName = property.Name;
                            object propertyValue = property.GetValue(result, null);
                            Type propertyType = property.PropertyType;

                            // Create a new anonymous type
                            // and add the object data.
                            AnonymousType anonymousType = new AnonymousType()
                            {
                                PropertyName = propertyName,
                                PropertyValue = propertyValue,
                                PropertyType = propertyType
                            };

                            // Add the anonymous type data.
                            anonymous.Add(anonymousType);
                        }
                        catch { }
                    }
                }
            }

            // Return the anonymous type.
            return anonymous.ToArray();
        }

        /// <summary>
        /// Convert all the object data into a data table.
        /// </summary>
        /// <param name="data">The collection of objects to convert.</param>
        /// <param name="type">The data table with IQueryable anonymous types.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with anonymous types.</returns>
        public DataTable GetDataTable(object[] data, Type type, string tableName)
        {
            // Create a new data table instance.
            DataTable dataTable = new DataTable(tableName);

            // If data was returned.
            if (data != null)
            {
                // Get all the properties within the type.
                PropertyInfo[] properties = type.GetProperties();

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in properties)
                {
                    try
                    {
                        // Find nullable types
                        switch (propertyItem.PropertyType.Name.ToLower())
                        {
                            case "nullable`1":
                                // Get the array of generic
                                // type parameters.
                                Type[] genericArguments = propertyItem.PropertyType.GetGenericArguments();

                                // Create a new column and assign
                                // each of the properties on the column.
                                DataColumn columnGen = new DataColumn();
                                columnGen.DataType = genericArguments[0];
                                columnGen.ColumnName = propertyItem.Name;
                                dataTable.Columns.Add(columnGen);
                                break;

                            default:
                                // Create a new column and assign
                                // each of the properties on the column.
                                DataColumn column = new DataColumn();
                                column.DataType = propertyItem.PropertyType;
                                column.ColumnName = propertyItem.Name;
                                dataTable.Columns.Add(column);
                                break;
                        }
                    }
                    catch { }
                }

                // For each item in the collection.
                foreach (var result in data)
                {
                    // Create a new data row.
                    DataRow row = null;
                    row = dataTable.NewRow();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in properties)
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(result, null);

                        try
                        {
                            // Assign the current row column
                            // value for the property.
                            row[propertyItem.Name] = value;
                        }
                        catch { }
                    }

                    // Add the current row to the table.
                    dataTable.Rows.Add(row);
                }
            }

            // Return the data table.
            return dataTable;
        }

        /// <summary>
        /// Convert all the object data into a data table.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        public DataTable GetDataTable(IQueryable query, string tableName)
        {
            // Create a new data table instance.
            DataTable dataTable = new DataTable(tableName);

            // If data was returned.
            if (query != null)
            {
                // Get all the properties within the type.
                PropertyInfo[] properties = query.ElementType.GetProperties();

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in properties)
                {
                    try
                    {
                        // Find nullable types
                        switch (propertyItem.PropertyType.Name.ToLower())
                        {
                            case "nullable`1":
                                // Get the array of generic
                                // type parameters.
                                Type[] genericArguments = propertyItem.PropertyType.GetGenericArguments();

                                // Create a new column and assign
                                // each of the properties on the column.
                                DataColumn columnGen = new DataColumn();
                                columnGen.DataType = genericArguments[0];
                                columnGen.ColumnName = propertyItem.Name;
                                dataTable.Columns.Add(columnGen);
                                break;

                            default:
                                // Create a new column and assign
                                // each of the properties on the column.
                                DataColumn column = new DataColumn();
                                column.DataType = propertyItem.PropertyType;
                                column.ColumnName = propertyItem.Name;
                                dataTable.Columns.Add(column);
                                break;
                        }
                    }
                    catch { }
                }

                // For each item in the collection.
                foreach (var result in query)
                {
                    // Create a new data row.
                    DataRow row = null;
                    row = dataTable.NewRow();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in properties)
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(result, null);

                        try
                        {
                            // Assign the current row column
                            // value for the property.
                            row[propertyItem.Name] = value;
                        }
                        catch { }
                    }

                    // Add the current row to the table.
                    dataTable.Rows.Add(row);
                }
            }

            // Return the data table.
            return dataTable;
        }

        /// <summary>
        /// Convert all the data type into a data table.
        /// </summary>
        /// <param name="type">The type item schema to convert.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        public DataTable CreateDataTableSchema(Type type, string tableName)
        {
            // Create a new data table instance.
            DataTable dataTable = new DataTable(tableName);

            // Get all the properties within the type.
            PropertyInfo[] properties = type.GetProperties();

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                try
                {
                    // Find nullable types
                    switch (propertyItem.PropertyType.Name.ToLower())
                    {
                        case "nullable`1":
                            // Get the array of generic
                            // type parameters.
                            Type[] genericArguments = propertyItem.PropertyType.GetGenericArguments();

                            // Create a new column and assign
                            // each of the properties on the column.
                            DataColumn columnGen = new DataColumn();
                            columnGen.DataType = genericArguments[0];
                            columnGen.ColumnName = propertyItem.Name;
                            dataTable.Columns.Add(columnGen);
                            break;

                        default:
                            // Create a new column and assign
                            // each of the properties on the column.
                            DataColumn column = new DataColumn();
                            column.DataType = propertyItem.PropertyType;
                            column.ColumnName = propertyItem.Name;
                            dataTable.Columns.Add(column);
                            break;
                    }
                }
                catch { }
            }

            // Return the data table.
            return dataTable;
        }

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataObject">The object array containing the data.</param>
        /// <returns>The array of the type of object.</returns>
        public T[] GetTypeDataArray<T>(Object[] dataObject)
            where T : new()
        {
            int i = 0;
            T[] dataArrary = new T[dataObject.Length];

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // If properties exist within the data type.
            if (properties.Count > 0)
            {
                // Add the type to the list collection.
                foreach (Object item in dataObject)
                {
                    try
                    {
                        // Create a new instance of the
                        // object type.
                        T data = new T();

                        // Get all the properties within the type.
                        PropertyInfo[] propertyTypes = item.GetType().GetProperties();

                        // For each property found add the
                        // property information.
                        foreach (PropertyInfo property in propertyTypes)
                        {
                            try
                            {
                                // Find in the property collection the current property that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                PropertyInfo propertyInfo = null;
                                try
                                {
                                    propertyInfo = properties.First(p => p.Name.ToLower() == property.Name.ToLower());
                                }
                                catch { }
                                if (propertyInfo != null)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (propertyInfo.Name.ToLower() == property.Name.ToLower())
                                    {
                                        // Set the object type property
                                        // with the property found in the
                                        // current object collection.
                                        propertyInfo.SetValue(data, property.GetValue(item, null), null);
                                    }
                                }
                            }
                            catch { }
                        }

                        dataArrary[i++] = data;
                    }
                    catch (System.Exception ex)
                    {
                        // Throw a general exception.
                        throw new System.Exception(ex.Message, ex.InnerException);
                    }
                }
            }

            // Return the collection.
            return dataArrary;
        }

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of the type of object.</returns>
        public T[] GetTypeDataArray<T>(IQueryable query)
            where T : new()
        {
            int i = 0;
            T[] dataArrary = new T[query.Count()];

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // Get all the properties within the type.
            PropertyInfo[] propertyTypes = query.ElementType.GetProperties();

            // If properties exist within the data type.
            if (properties.Count > 0)
            {
                // Add the type to the list collection.
                foreach (var item in query)
                {
                    try
                    {
                        // Create a new instance of the
                        // object type.
                        T data = new T();

                        // For each property found add the
                        // property information.
                        foreach (PropertyInfo property in propertyTypes)
                        {
                            try
                            {
                                // Find in the property collection the current property that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                PropertyInfo propertyInfo = null;
                                try
                                {
                                    propertyInfo = properties.First(p => p.Name.ToLower() == property.Name.ToLower());
                                }
                                catch { }
                                if (propertyInfo != null)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (propertyInfo.Name.ToLower() == property.Name.ToLower())
                                    {
                                        // Set the object type property
                                        // with the property found in the
                                        // current object collection.
                                        propertyInfo.SetValue(data, property.GetValue(item, null), null);
                                    }
                                }
                            }
                            catch { }
                        }

                        dataArrary[i++] = data;
                    }
                    catch (System.Exception ex)
                    {
                        // Throw a general exception.
                        throw new System.Exception(ex.Message, ex.InnerException);
                    }
                }
            }

            // Return the collection.
            return dataArrary;
        }

        /// <summary>
        /// Convert all the object data into a list collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataObject">The object array containing the data.</param>
        /// <returns>The list collection of the type of object.</returns>
        public List<T> GetListTypeData<T>(Object[] dataObject)
            where T : new()
        {
            // Create a new instance for the generic data type.
            List<T> dataObjectCollection = new List<T>(dataObject.Length);

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // If properties exist within the data type.
            if (properties.Count > 0)
            {
                // Add the type to the list collection.
                foreach (Object item in dataObject)
                {
                    try
                    {
                        // Create a new instance of the
                        // object type.
                        T data = new T();

                        // Get all the properties within the type.
                        PropertyInfo[] propertyTypes = item.GetType().GetProperties();

                        // For each property found add the
                        // property information.
                        foreach (PropertyInfo property in propertyTypes)
                        {
                            try
                            {
                                // Find in the property collection the current property that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                PropertyInfo propertyInfo = null;
                                try
                                {
                                    propertyInfo = properties.First(p => p.Name.ToLower() == property.Name.ToLower());
                                }
                                catch { }
                                if (propertyInfo != null)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (propertyInfo.Name.ToLower() == property.Name.ToLower())
                                    {
                                        // Set the object type property
                                        // with the property found in the
                                        // current object collection.
                                        propertyInfo.SetValue(data, property.GetValue(item, null), null);
                                    }
                                }
                            }
                            catch { }
                        }

                        // Add the object type to the collection.
                        dataObjectCollection.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        // Throw a general exception.
                        throw new System.Exception(ex.Message, ex.InnerException);
                    }
                }
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Gets the data collection from the data table.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The list generic collection.</returns>
        public List<T> GetListCollection<T>(DataTable dataTable)
            where T : new()
        {
            // Create a new instance for the generic data type.
            List<T> dataObjectCollection = new List<T>();

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // For each row within the data collection.
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    // Create a new data business 
                    // object for each row.
                    T data = new T();

                    // If properties exist within the data type.
                    if (properties.Count > 0)
                    {
                        // For each column within the data collection.
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            // Find in the propert collection the current propert that matches
                            // the current column. Use the Predicate delegate object to
                            // initiate a search for the specified match.
                            PropertyInfo propertyInfo = null;
                            try
                            {
                                propertyInfo = properties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                            }
                            catch { }
                            if (propertyInfo != null)
                            {
                                // If the current property within the property collection
                                // is the current column within the data collection.
                                if (propertyInfo.Name.ToLower() == column.ColumnName.ToLower())
                                {
                                    // If the data within the current row and column
                                    // is 'NULL' then do not store the data.
                                    if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                        // Assign the current data for the current row
                                        // into the current data business object.
                                        propertyInfo.SetValue(data, row[column.ColumnName.ToLower()], null);
                                }
                            }
                        }
                    }

                    // Add the current data row to the
                    // business object collection.
                    dataObjectCollection.Add(data);
                }
                catch (System.Exception ex)
                {
                    // Throw a general exception.
                    throw new System.Exception(ex.Message, ex.InnerException);
                }
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Gets the data collection from the data table.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic convsersion type object collection.</returns>
        public List<object> GetListCollection(DataTable dataTable, Type conversionType)
        {
            // Create a new instance for the generic data type.
            List<object> dataObjectCollection = new List<object>();

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(conversionType);

            // For each row within the data collection.
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    // Create a new data business 
                    // object for each row.
                    object data = Activator.CreateInstance(conversionType);

                    // If properties exist within the data type.
                    if (properties.Count > 0)
                    {
                        // For each column within the data collection.
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            // Find in the propert collection the current propert that matches
                            // the current column. Use the Predicate delegate object to
                            // initiate a search for the specified match.
                            PropertyInfo propertyInfo = null;
                            try
                            {
                                propertyInfo = properties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                            }
                            catch { }
                            if (propertyInfo != null)
                            {
                                // If the current property within the property collection
                                // is the current column within the data collection.
                                if (propertyInfo.Name.ToLower() == column.ColumnName.ToLower())
                                {
                                    // If the data within the current row and column
                                    // is 'NULL' then do not store the data.
                                    if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                        // Assign the current data for the current row
                                        // into the current data business object.
                                        propertyInfo.SetValue(data, row[column.ColumnName.ToLower()], null);
                                }
                            }
                        }
                    }

                    // Add the current data row to the
                    // business object collection.
                    dataObjectCollection.Add(data);
                }
                catch (System.Exception ex)
                {
                    // Throw a general exception.
                    throw new System.Exception(ex.Message, ex.InnerException);
                }
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Get the matching datatable in the dataset for the matching type.
        /// </summary>
        /// <param name="dataSet">The dataset containing the collection of datatables.</param>
        /// <param name="matchingType">The object type to match against.</param>
        /// <returns>-1 if no match was found else the index number for the matching table in the dataset.</returns>
        public int GetMatchingDataTableSchema(System.Data.DataSet dataSet, Type matchingType)
        {
            int matchingTableIndex = -1;
            int currentIndex = -1;

            // For each table in the dataset.
            foreach (DataTable table in dataSet.Tables)
            {
                // Increment the current index.
                currentIndex++;

                try
                {
                    if (table.Columns.Count > 0)
                    {
                        // Get the list of all properties wthin the
                        // current business data type.
                        List<PropertyInfo> properties = GetPublicProperties(matchingType);

                        // If properties exist within the data type.
                        if (properties.Count > 0)
                        {
                            PropertyInfo propertyInfo = null;

                            // For each column within the data collection.
                            foreach (DataColumn column in table.Columns)
                            {
                                // Find in the propert collection the current propert that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                try
                                {
                                    propertyInfo = properties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                                }
                                catch { }

                                // If no column property match was
                                // found then this is not the data table.
                                if (propertyInfo == null)
                                    break;
                            }

                            // The matching index has been found.
                            if (propertyInfo != null)
                            {
                                matchingTableIndex = currentIndex;
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // Throw a general exception.
                    throw new System.Exception(ex.Message, ex.InnerException);
                }
            }

            // Return the matching table index for the type.
            return matchingTableIndex;
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
            Nequeo.Threading.FunctionHandler<Object, DataTable, Type> function =
                (DataTable data, Type type) => this.ListGenericTypeMapper(data, type);

            // Project the data table.
            return ListGenericTypeProjector(dataTable, conversionType, function);
        }

        /// <summary>
        /// Gets the type of the conversion type from the data row.
        /// </summary>
        /// <param name="dataRow">The datarow containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public object TypeTranslator(DataRow dataRow, Type conversionType)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");
            if (conversionType == null) throw new ArgumentNullException("conversionType");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<Object, DataRow, Type> function =
                (DataRow data, Type type) => this.TypeMapper(data, type);

            // Project the data table.
            return TypeProjector(dataRow, conversionType, function);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Maps the source to the destination for each specifed property
        /// </summary>
        /// <typeparam name="TDestination">The destinamtion type.</typeparam>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <param name="destination">The destination instance</param>
        /// <param name="source">The source instamce.</param>
        /// <returns>The destination property mapped instance.</returns>
        public static TDestination PropertyMapper<TDestination, TSource>(TDestination destination, TSource source)
            where TDestination : class, new()
            where TSource : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (destination == null) throw new ArgumentNullException("destination");
            if (source == null) throw new ArgumentNullException("source");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.PropertyMappingTranslator<TDestination, TSource>(destination, source);
        }

        /// <summary>
        /// Translates the mvc model type to the data entity type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="mvcModel">The mvc data model</param>
        /// <returns>The translated data entity collection.</returns>
        public static TDataEntity[] TranslateMvcMetadataType<TDataEntity, TEntity>(TEntity[] mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (mvcModel == null) throw new ArgumentNullException("mvcModel");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.MvcMetadataTypeTranslator<TDataEntity, TEntity>(mvcModel);
        }

        /// <summary>
        /// Translates the mvc model type to the data entity type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="mvcModel">The mvc data model</param>
        /// <returns>The translated data entity collection.</returns>
        public static TDataEntity TranslateMvcMetadataType<TDataEntity, TEntity>(TEntity mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (mvcModel == null) throw new ArgumentNullException("mvcModel");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.MvcMetadataTypeTranslator<TDataEntity, TEntity>(mvcModel);
        }

        /// <summary>
        /// Translates the data entity type to the mvc model type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        public static TEntity[] TranslateMvcMetadataType<TDataEntity, TEntity>(TDataEntity[] dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.MvcMetadataTypeTranslator<TDataEntity, TEntity>(dataEntity);
        }

        /// <summary>
        /// Translates the data entity type to the mvc model type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        public static TEntity TranslateMvcMetadataType<TDataEntity, TEntity>(TDataEntity dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.MvcMetadataTypeTranslator<TDataEntity, TEntity>(dataEntity);
        }

        /// <summary>
        /// Translates the data entity type to the model type.
        /// </summary>
        /// <typeparam name="T">The data entity type.</typeparam>
        /// <typeparam name="D">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        public static D TranslateType<T, D>(T dataEntity)
            where T : class, new()
            where D : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.MvcMetadataTypeTranslator<T, D>(dataEntity);
        }

        /// <summary>
        /// Translates the data entity type to the model type.
        /// </summary>
        /// <typeparam name="T">The data entity type.</typeparam>
        /// <typeparam name="D">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        public static D[] TranslateType<T, D>(T[] dataEntity)
            where T : class, new()
            where D : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            AnonymousTypeFunction func = new AnonymousTypeFunction();
            return func.MvcMetadataTypeTranslator<T, D>(dataEntity);
        }

        #endregion
    }

    /// <summary>
    /// Interface that contains members that control the
    /// convertion of object type data to strongly
    /// typed objects.
    /// </summary>
    public interface IAnonymousTypeFunction
    {
        #region Public Methods
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to examine.</typeparam>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TEntity[] Translator<TEntity>(DataTable table)
            where TEntity : class, new();

        /// <summary>
        /// Translates the mvc model type to the data entity type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="mvcModel">The mvc data model</param>
        /// <returns>The translated data entity collection.</returns>
        TDataEntity[] MvcMetadataTypeTranslator<TDataEntity, TEntity>(TEntity[] mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new();

        /// <summary>
        /// Translates the mvc model type to the data entity type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="mvcModel">The mvc data model</param>
        /// <returns>The translated data entity collection.</returns>
        TDataEntity MvcMetadataTypeTranslator<TDataEntity, TEntity>(TEntity mvcModel)
            where TDataEntity : class, new()
            where TEntity : class, new();

        /// <summary>
        /// Translates the data entity type to the mvc model type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        TEntity[] MvcMetadataTypeTranslator<TDataEntity, TEntity>(TDataEntity[] dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new();

        /// <summary>
        /// Translates the data entity type to the mvc model type.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type.</typeparam>
        /// <typeparam name="TEntity">The mvc entity type.</typeparam>
        /// <param name="dataEntity">The data entity type</param>
        /// <returns>The translated mvc model collection.</returns>
        TEntity MvcMetadataTypeTranslator<TDataEntity, TEntity>(TDataEntity dataEntity)
            where TDataEntity : class, new()
            where TEntity : class, new();

        /// <summary>
        /// Translate a collection of data entities to the corresponding data table.
        /// </summary>
        /// <typeparam name="T">The entity type to examine.</typeparam>
        /// <param name="items">The collection of type items</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable Translator<T>(T[] items, string tableName)
            where T : class, new();

        /// <summary>
        /// Convert all the object data into a data table.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        DataTable GetDataTable(IQueryable query, string tableName);

        /// <summary>
        /// Convert all the data type into a data table.
        /// </summary>
        /// <param name="type">The type item schema to convert.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        DataTable CreateDataTableSchema(Type type, string tableName);

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataObject">The object array containing the data.</param>
        /// <returns>The array of the type of object.</returns>
        T[] GetTypeDataArray<T>(Object[] dataObject)
            where T : new();

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of the type of object.</returns>
        T[] GetTypeDataArray<T>(IQueryable query)
            where T : new();

        /// <summary>
        /// Convert all the object data into a list collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataObject">The object array containing the data.</param>
        /// <returns>The list collection of the type of object.</returns>
        List<T> GetListTypeData<T>(Object[] dataObject)
            where T : new();

        /// <summary>
        /// Gets the data collection from the data table.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>The list generic collection.</returns>
        List<T> GetListCollection<T>(DataTable dataTable)
            where T : new();

        /// <summary>
        /// Gets the data collection from the data table.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic convsersion type object collection.</returns>
        List<object> GetListCollection(DataTable dataTable, Type conversionType);

        /// <summary>
        /// Get the matching datatable in the dataset for the matching type.
        /// </summary>
        /// <param name="dataSet">The dataset containing the collection of datatables.</param>
        /// <param name="matchingType">The object type to match against.</param>
        /// <returns>-1 if no match was found else the index number for the matching table in the dataset.</returns>
        int GetMatchingDataTableSchema(System.Data.DataSet dataSet, Type matchingType);

        /// <summary>
        /// Gets the list generic collection of the conversion type from the data table.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        object ListGenericTypeTranslator(DataTable dataTable, Type conversionType);

        /// <summary>
        /// Gets the type of the conversion type from the data row.
        /// </summary>
        /// <param name="dataRow">The datarow containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        object TypeTranslator(DataRow dataRow, Type conversionType);

        #endregion
    }

    /// <summary>
    /// Class that contains members that control the
    /// function routine execution of database 
    /// routines.
    /// </summary>
    public sealed class FunctionRountineHandler : Nequeo.Handler.LogHandler, 
        IFunctionRountineHandler, IExecuteFunctionResult, IFunctionMultipleResults, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly FunctionRountineHandler Instance = new FunctionRountineHandler();

        /// <summary>
        /// Static constructor
        /// </summary>
        static FunctionRountineHandler() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FunctionRountineHandler()
            : base(applicationName, eventNamespace)
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo.Data";
        private const string eventNamespace = "Nequeo.Data.Control";
        #endregion

        #region Private Fields
        private bool _isMultiRecordSet = false;
        private Type _returnType = null;
        private List<Type> _multiReturnType = new List<Type>();
        private Dictionary<int, string> _multiReturnTypeNameOrder = new Dictionary<int, string>();
        private Dictionary<int, Type> _multiReturnTypeOrder = new Dictionary<int, Type>();
        private System.Data.DataTable _dataTable = null;
        private System.Data.DataSet _dataSet = null;
        private System.Data.Common.DbCommand _functionResult = null;
        private FunctionRoutineType _functionType = FunctionRoutineType.None;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute the function.
        /// </summary>
        /// <param name="instance">The instance that implements the DataBase interface.</param>
        /// <param name="methodInfo">The curent method information.</param>
        /// <param name="parameters">The method parameters.</param>
        /// <returns>The execution function result interface implementation.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters)
        {
            string functionName = null;
            _functionType = FunctionRoutineType.None;

            // For each attribute on the method.
            foreach (object attribute in methodInfo.GetCustomAttributes(true))
            {
                // If the attribute is the
                // function column attribute.
                if (attribute is Nequeo.Data.Custom.FunctionRoutineAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.FunctionRoutineAttribute att =
                        (Nequeo.Data.Custom.FunctionRoutineAttribute)attribute;
                    
                    // Create the function and set the current
                    // function type.
                    functionName = att.Name;
                    _functionType = att.FunctionRoutineType;
                    _isMultiRecordSet = att.IsMultiResultSet;
                }

                // If the attribute is the
                // function column attribute.
                if (attribute is Nequeo.Data.Custom.FunctionMultiReturnTypeAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.FunctionMultiReturnTypeAttribute att =
                        (Nequeo.Data.Custom.FunctionMultiReturnTypeAttribute)attribute;

                    _multiReturnType.Add(att.ReturnType);
                    _multiReturnTypeNameOrder.Add(att.Order, att.ReturnType.Name);
                    _multiReturnTypeOrder.Add(att.Order, att.ReturnType);
                }
            }

            // If no function name or function type
            // has been set the throw exception.
            if (String.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("Name");

            if (_functionType == FunctionRoutineType.None)
                throw new Exception("Function rountine type can not be 'None'");

            // Get the method return type.
            _returnType = methodInfo.ReturnType;

            // Get the current common instance.
            // Create and execute the function query.
            CreateQuery(instance, functionName, _functionType, methodInfo, parameters);

            // Return the current object result.
            return this;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Create the function routine query.
        /// </summary>
        /// <param name="common">The current common data generic base instance.</param>
        /// <param name="functionName">The current function name.</param>
        /// <param name="functionType">The function type.</param>
        /// <param name="methodInfo">The current method information.</param>
        /// <param name="parameters">The function type parameters.</param>
        private void CreateQuery(IFunctionHandler common, string functionName, 
            FunctionRoutineType functionType, MethodInfo methodInfo, params Object[] parameters)
        {
            string functionParameters = string.Empty;
            string functionValues = string.Empty;
            _connectionType = common.FunctionConnectionType;
            _connectionDataType = common.FunctionConnectionDataType;

            switch (common.FunctionConnectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    DbParameter[] retSql = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retSql);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    DbParameter[] retClientOracle = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retClientOracle);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    DbParameter[] retClientPg = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retClientPg);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    DbParameter[] retClientMy = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retClientMy);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    DbParameter[] retOleDb = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, common.FunctionConnectionDataType, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retOleDb);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    DbParameter[] retOdbc = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, common.FunctionConnectionDataType, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retOdbc);
                    break;

                case ConnectionContext.ConnectionType.SqliteConnection:
                    DbParameter[] retSqlite = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retSqlite);
                    break;

                default:
                    DbParameter[] retDefault = common.FunctionDataAccessProvider.GetParameters(ref functionParameters, ref functionValues, common.FunctionConnectionDataType, methodInfo, parameters);
                    ExecuteQuery(common, "[" + functionName.Replace(".", "].[") + "]", functionParameters, functionValues, functionType, retDefault);
                    break;
            }
        }

        /// <summary>
        /// Execute the function routine query.
        /// </summary>
        /// <param name="common">The current common data generic base instance.</param>
        /// <param name="functionName">The current function name.</param>
        /// <param name="functionParameters">The function parameters.</param>
        /// <param name="functionValues">The function values.</param>
        /// <param name="functionType">The function type.</param>
        /// <param name="parameters">The function type database parameters.</param>
        private void ExecuteQuery(IFunctionHandler common, string functionName,
            string functionParameters, string functionValues, FunctionRoutineType functionType, params DbParameter[] parameters)
        {
            switch (functionType)
            {
                case FunctionRoutineType.StoredProcedure:
                    if (!_isMultiRecordSet)
                        _functionResult = common.ExecuteQuery(ref _dataTable, functionName, 
                            CommandType.StoredProcedure, common.FunctionDatabaseConnectionString,  true, parameters);
                    else
                        _functionResult = common.ExecuteQuery(ref _dataSet, GetTablesOrder(), functionName,
                            CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, parameters);
                    break;

                case FunctionRoutineType.ScalarFunction:
                    switch (common.FunctionConnectionType)
                    {
                        case ConnectionContext.ConnectionType.SqlConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT " + functionName +
                                "(" + functionParameters + ") AS 'FuncResult'", CommandType.Text,
                                common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        case ConnectionContext.ConnectionType.SqliteConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT " + functionName +
                                "(" + functionParameters + ") AS 'FuncResult'", CommandType.Text,
                                common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        case ConnectionContext.ConnectionType.OracleClientConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT " + functionName +
                                "(" + functionValues + ") AS \"FuncResult\" FROM DUAL", CommandType.Text,
                                common.FunctionDatabaseConnectionString, true, null);
                            break;

                        case ConnectionContext.ConnectionType.PostgreSqlConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT " + functionName +
                                "(" + functionValues + ") AS \"FuncResult\"", CommandType.Text,
                                common.FunctionDatabaseConnectionString, true, null);
                            break;

                        case ConnectionContext.ConnectionType.MySqlConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT " + functionName +
                                "(" + functionValues + ") AS \"FuncResult\"", CommandType.Text,
                                common.FunctionDatabaseConnectionString, true, null);
                            break;

                        case ConnectionContext.ConnectionType.OleDbConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, functionName,
                                CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        case ConnectionContext.ConnectionType.OdbcConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, functionName,
                                CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        default:
                             _functionResult = common.ExecuteQuery(ref _dataTable, functionName,
                                CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, true, parameters);
                            break;
                    }
                    break;

                case FunctionRoutineType.TableFunction:
                    switch (common.FunctionConnectionType)
                    {
                        case ConnectionContext.ConnectionType.SqlConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT * FROM " + functionName +
                                "(" + functionParameters + ")", CommandType.Text, common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        case ConnectionContext.ConnectionType.SqliteConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT * FROM " + functionName +
                                "(" + functionParameters + ")", CommandType.Text, common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        case ConnectionContext.ConnectionType.OracleClientConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT * FROM TABLE(" + functionName +
                                "(" + functionValues + "))", CommandType.Text, common.FunctionDatabaseConnectionString, true, null);
                            break;

                        case ConnectionContext.ConnectionType.PostgreSqlConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT * FROM " + functionName +
                                "(" + functionValues + ")", CommandType.Text, common.FunctionDatabaseConnectionString, true, null);
                            break;

                        case ConnectionContext.ConnectionType.MySqlConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, "SELECT * FROM " + functionName +
                                "(" + functionValues + ")", CommandType.Text, common.FunctionDatabaseConnectionString, true, null);
                            break;

                        case ConnectionContext.ConnectionType.OleDbConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, functionName,
                                CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        case ConnectionContext.ConnectionType.OdbcConnection:
                            _functionResult = common.ExecuteQuery(ref _dataTable, functionName,
                                CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, true, parameters);
                            break;

                        default:
                             _functionResult = common.ExecuteQuery(ref _dataTable, functionName,
                                CommandType.StoredProcedure, common.FunctionDatabaseConnectionString, true, parameters);
                            break;
                    }
                    break;

                default:
                    throw new Exception("No routine type has been specified.");
            }
        }

        /// <summary>
        /// Order the tables by the order key value.
        /// </summary>
        /// <returns>The ordered table names.</returns>
        private string[] GetTablesOrder()
        {
            IEnumerable<string> tables = _multiReturnTypeNameOrder.OrderBy(u => u.Key).Select(u => u.Value);
            return tables.ToArray();
        }

        /// <summary>
        /// Get all the datatable schemas
        /// </summary>
        /// <returns>The collection of datatables within the dataset schema.</returns>
        private DataTable[] GetDataTablesOrder()
        {
            int count = 0;
            IEnumerable<Type> tableTypes = _multiReturnTypeOrder.OrderBy(u => u.Key).Select(u => u.Value);
            DataTable[] tables = new DataTable[tableTypes.Count()];

            // Create a new instance of the type
            // conversion object.
            Control.AnonymousTypeFunction typeConversion =
                new Control.AnonymousTypeFunction();

            // Create a new datatable schema from the current type.
            foreach (Type item in tableTypes)
                tables[count++] = typeConversion.CreateDataTableSchema(item, item.Name);

            // Return the data connection.
            return tables;
        }

        /// <summary>
        /// Get the results of the function execution.
        /// </summary>
        /// <returns>The result of the function execution.</returns>
        private object GetReturnObject()
        {
            switch (_functionType)
            {
                case FunctionRoutineType.TableFunction:
                case FunctionRoutineType.StoredProcedure:
                    // Create a new instance of the type
                    // conversion object.
                    Control.AnonymousTypeFunction typeConversion =
                        new Control.AnonymousTypeFunction();

                    // If the return type is a generic type.
                    if (_returnType.IsGenericType)
                    {
                        // Get the array of generic
                        // type parameters.
                        Type[] genericArguments = _returnType.GetGenericArguments();

                        // Get the list of result generic objects,
                        // return the generic list of objects.
                        object results = typeConversion.ListGenericTypeTranslator(_dataTable, genericArguments[0]);
                        return results;
                    }
                        // If the return type is an interface.
                    else if (_returnType.IsInterface)
                    {
                        return this;
                    }
                    else
                    {
                        switch (_connectionDataType)
                        {
                            case ConnectionContext.ConnectionDataType.SqlDataType:
                                // Return the return parameter value.
                                return (_functionResult.Parameters[_functionResult.Parameters.Count - 1].Value);

                            default:
                                return ((object)(0));
                        }
                    }

                case FunctionRoutineType.ScalarFunction:
                    switch (_connectionType)
                    {
                        case ConnectionContext.ConnectionType.SqlConnection:
                        case ConnectionContext.ConnectionType.OracleClientConnection:
                        case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        case ConnectionContext.ConnectionType.MySqlConnection:
                        case ConnectionContext.ConnectionType.SqliteConnection:
                            // Return the scalar function result.
                            string returnValue = _dataTable.Rows[0]["FuncResult"].ToString();
                            object returnItem = null;

                            // If the return type is a generic type.
                            if (_returnType.IsGenericType)
                            {
                                // Get the array of generic
                                // type parameters.
                                Type[] genericArguments = _returnType.GetGenericArguments();

                                // Convert the return type.
                                returnItem = Nequeo.Data.DataType.DataTypeConversion.GetDataTypeValue(genericArguments[0], returnValue);
                            }
                            else
                            {
                                // Convert the return type.
                                returnItem = Nequeo.Data.DataType.DataTypeConversion.GetDataTypeValue(_returnType, returnValue);
                            }
                            return returnItem;
                            
                        default:
                            return ((object)(0));
                    }
            }

            // Return nothing.
            return null;
        }
        #endregion

        #region Public Execution Function Members Interface Implementation
        /// <summary>
        /// The execution result.
        /// </summary>
        public object ReturnValue
        {
            get { return GetReturnObject(); }
        }

        /// <summary>
        /// Gets the value of the parameter index.
        /// </summary>
        /// <param name="parameterIndex">The parameter index.</param>
        /// <returns>The object value.</returns>
        public object GetParameterValue(int parameterIndex)
        {
            return _functionResult.Parameters[parameterIndex].Value;
        }

        /// <summary>
        /// Gets, the datatable containing the result data.
        /// </summary>
        public System.Data.DataTable DataResult
        {
            get { return _dataTable; }
        }

        /// <summary>
        /// Gets, the dataset containing the result data.
        /// </summary>
        public System.Data.DataSet SetResult
        {
            get { return _dataSet; }
        }

        /// <summary>
        /// Gets, the database common function result.
        /// </summary>
        public System.Data.Common.DbCommand FunctionResult
        {
            get { return _functionResult; }
        }
        #endregion

        #region Public Function Multiple Results Interface Implementation

        /// <summary>
        /// Get the collection of types to return.
        /// </summary>
        /// <returns>The collection of types to return.</returns>
        public string[] GetTypeNames()
        {
            return GetTablesOrder();
        }

        /// <summary>
        /// Get the result for the current type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The list of generic types to return.</returns>
        public List<T> GetResult<T>()
        {
            // Create a new instance of the type
            // conversion object.
            Control.AnonymousTypeFunction typeConversion =
                new Control.AnonymousTypeFunction();

            // Get the array of generic
            // type parameters.
            Type genericArguments = typeof(T);
            int tableIndex = typeConversion.GetMatchingDataTableSchema(_dataSet, genericArguments);

            // If match was not found.
            if (tableIndex < 0)
                throw new Exception("The matching DataTable count not be found for the type " + genericArguments.Name);
            
            // Assign the table.
            _dataTable = _dataSet.Tables[tableIndex];

            // Get the list of result generic objects,
            // return the generic list of objects.
            object results = typeConversion.ListGenericTypeTranslator(_dataTable, genericArguments);
            return (List<T>)results;
        }

        #endregion
    }

    /// <summary>
    /// Interface when executing a database functions.
    /// </summary>
    public interface IFunctionHandler : IExecuteQuery, IExecuteCommand
    {
        #region Public Properties
        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        String FunctionDatabaseConnectionString { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType FunctionConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType FunctionConnectionDataType { get; set; }

        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess FunctionDataAccessProvider { get; set; }

        #endregion
    }

    /// <summary>
    /// Interface that contains members that control the
    /// function routine execution of database 
    /// routines.
    /// </summary>
    public interface IFunctionRountineHandler
    {
        #region Public Methods
        /// <summary>
        /// Execute the function.
        /// </summary>
        /// <param name="instance">The instance that implements the DataBase interface.</param>
        /// <param name="methodInfo">The curent method information.</param>
        /// <param name="parameters">The method parameters.</param>
        /// <returns>The execution function result interface implementation.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters);

        #endregion
    }

    /// <summary>
    /// Execution function result interface.
    /// </summary>
    public interface IExecuteFunctionResult
    {
        #region Execution Function Members.
        /// <summary>
        /// The execution result.
        /// </summary>
        object ReturnValue { get; }

        /// <summary>
        /// Gets the value of the parameter index.
        /// </summary>
        /// <param name="parameterIndex">The parameter index.</param>
        /// <returns>The object value.</returns>
        object GetParameterValue(int parameterIndex);

        /// <summary>
        /// Gets, the datatable containing the result data.
        /// </summary>
        System.Data.DataTable DataResult { get; }

        /// <summary>
        /// Gets, the dataset containing the result data.
        /// </summary>
        System.Data.DataSet SetResult { get; }

        /// <summary>
        /// Gets, the database common function result.
        /// </summary>
        System.Data.Common.DbCommand FunctionResult { get; }

        #endregion
    }

    /// <summary>
    /// Function multiple result interface.
    /// </summary>
    public interface IFunctionMultipleResults
    {
        #region Function Multiple Result Members.

        /// <summary>
        /// Get the collection of types to return.
        /// </summary>
        /// <returns>The collection of types to return.</returns>
        string[] GetTypeNames();

        /// <summary>
        /// Get the result for the current type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The list of generic types to return.</returns>
        List<T> GetResult<T>();

        #endregion
    }

    /// <summary>
    /// The function routine handler base..
    /// </summary>
    public abstract class FunctionBase : IFunctionHandler
    {
        #region FunctionBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected FunctionBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
        {
            _functionConnectionType = connectionType;
            _functionConnectionDataType = connectionDataType;
            _functionDatabaseConnectionString = configurationDatabaseConnection;
            _dataAccessProvider = dataAccessProvider;
        }
        #endregion

        #region Private Fields
        private string _functionDatabaseConnectionString = string.Empty;
        private ConnectionContext.ConnectionType _functionConnectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _functionConnectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the database connection string.
        /// </summary>
        public String FunctionDatabaseConnectionString
        {
            get { return DefaultConnection(_functionDatabaseConnectionString); }
            set { _functionDatabaseConnectionString = DefaultConnection(value); }
        }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        public ConnectionContext.ConnectionType FunctionConnectionType
        {
            get { return _functionConnectionType; }
            set { _functionConnectionType = value; }
        }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        public ConnectionContext.ConnectionDataType FunctionConnectionDataType
        {
            get { return _functionConnectionDataType; }
            set { _functionConnectionDataType = value; }
        }

        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        public IDataAccess FunctionDataAccessProvider
        {
            get { return _dataAccessProvider; }
            set { _dataAccessProvider = value; }
        }
        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        public string DefaultConnection(string configurationDatabaseConnection)
        {
            string providerName = null;
            string connection = string.Empty;

            // Get the current database connection string
            // from the configuration file through the
            // specified configuration key.
            using (Nequeo.Handler.Global.DatabaseConnections databaseConnection = new Nequeo.Handler.Global.DatabaseConnections())
                connection = databaseConnection.DatabaseConnection(configurationDatabaseConnection, out providerName);

            // If empty string is returned then
            // value should be the connection string.
            if (String.IsNullOrEmpty(connection))
                return configurationDatabaseConnection;
            else
                return connection;
        }

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        public string AlternativeConnection(string configurationDatabaseConnection)
        {
            string providerName = null;
            string connection = string.Empty;

            // Get the current database connection string
            // from the configuration file through the
            // specified configuration key.
            using (Nequeo.Handler.Global.DatabaseConnections databaseConnection = new Nequeo.Handler.Global.DatabaseConnections())
                connection = databaseConnection.DatabaseConnection(configurationDatabaseConnection, out providerName);

            // If empty string is returned then
            // value should be the connection string.
            if (String.IsNullOrEmpty(connection))
                return configurationDatabaseConnection;
            else
                return connection;
        }
        #endregion

        #region Public Execute Query Methods
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        public Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, params DbParameter[] values)
        {
            return ExecuteCommand(ref dbCommand, commandText, commandType,
                DefaultConnection(FunctionDatabaseConnectionString), values);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        public Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            dbCommand = null;
            Int32 returnValue = -1;

            try
            {
                switch (this.FunctionConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    default:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, _functionConnectionDataType, values);
                        break;
                }

                // Return true.
                return returnValue;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataTable, queryText, commandType,
                DefaultConnection(FunctionDatabaseConnectionString), getSchemaTable, values);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataSet, tables, queryText, commandType,
                DefaultConnection(FunctionDatabaseConnectionString), values);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataSet, tables, queryText, commandType,
                DefaultConnection(FunctionDatabaseConnectionString), values);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;

            try
            {
                switch (this.FunctionConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, _functionConnectionDataType, values);
                        break;
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;

            try
            {
                switch (this.FunctionConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _functionConnectionDataType, values);
                        break;
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;

            try
            {
                switch (this.FunctionConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _functionConnectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _functionConnectionDataType, values);
                        break;
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        public IExecuteFunctionResult ExecuteFunction(
            IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters)
        {
            return new FunctionRountineHandler().ExecuteFunction(instance, methodInfo, parameters);
        }
        #endregion

        #endregion

    }
}
