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
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;

namespace Nequeo.Reflection
{
    /// <summary>
    /// Non anonymous type container when using LINQ queries
    /// </summary>
    [Serializable]
    [DataContract]
    public class NonAnonymousType
    {
        #region Private Fields
        private Object _Property1 = null;
        private Object _Property2 = null;
        private Object _Property3 = null;
        private Object _Property4 = null;
        private Object _Property5 = null;
        private Object _Property6 = null;
        private Object _Property7 = null;
        private Object _Property8 = null;
        private Object _Property9 = null;
        private Object _Property10 = null;
        private Object _Property11 = null;
        private Object _Property12 = null;
        private Object _Property13 = null;
        private Object _Property14 = null;
        private Object _Property15 = null;
        private Object _Property16 = null;
        private Object _Property17 = null;
        private Object _Property18 = null;
        private Object _Property19 = null;
        private Object _Property20 = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the first property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property1
        {
            get { return _Property1; }
            set { _Property1 = value; }
        }

        /// <summary>
        /// Gets sets, the second property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property2
        {
            get { return _Property2; }
            set { _Property2 = value; }
        }

        /// <summary>
        /// Gets sets, the third property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property3
        {
            get { return _Property3; }
            set { _Property3 = value; }
        }

        /// <summary>
        /// Gets sets, the fourth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property4
        {
            get { return _Property4; }
            set { _Property4 = value; }
        }

        /// <summary>
        /// Gets sets, the fifth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property5
        {
            get { return _Property5; }
            set { _Property5 = value; }
        }

        /// <summary>
        /// Gets sets, the sixth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property6
        {
            get { return _Property6; }
            set { _Property6 = value; }
        }

        /// <summary>
        /// Gets sets, the seventh property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property7
        {
            get { return _Property7; }
            set { _Property7 = value; }
        }

        /// <summary>
        /// Gets sets, the eighth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property8
        {
            get { return _Property8; }
            set { _Property8 = value; }
        }

        /// <summary>
        /// Gets sets, the nineth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property9
        {
            get { return _Property9; }
            set { _Property9 = value; }
        }

        /// <summary>
        /// Gets sets, the tenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property10
        {
            get { return _Property10; }
            set { _Property10 = value; }
        }

        /// <summary>
        /// Gets sets, the eleventh property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property11
        {
            get { return _Property11; }
            set { _Property11 = value; }
        }

        /// <summary>
        /// Gets sets, the twelfth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property12
        {
            get { return _Property12; }
            set { _Property12 = value; }
        }

        /// <summary>
        /// Gets sets, the thirdteenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property13
        {
            get { return _Property13; }
            set { _Property13 = value; }
        }

        /// <summary>
        /// Gets sets, the fourthtenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property14
        {
            get { return _Property14; }
            set { _Property14 = value; }
        }

        /// <summary>
        /// Gets sets, the fifthtenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property15
        {
            get { return _Property15; }
            set { _Property15 = value; }
        }

        /// <summary>
        /// Gets sets, the sixteenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property16
        {
            get { return _Property16; }
            set { _Property16 = value; }
        }

        /// <summary>
        /// Gets sets, the seventeenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property17
        {
            get { return _Property17; }
            set { _Property17 = value; }
        }

        /// <summary>
        /// Gets sets, the eighteenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property18
        {
            get { return _Property18; }
            set { _Property18 = value; }
        }

        /// <summary>
        /// Gets sets, the nineteenth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property19
        {
            get { return _Property19; }
            set { _Property19 = value; }
        }

        /// <summary>
        /// Gets sets, the twentieth property.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = false)]
        public Object Property20
        {
            get { return _Property20; }
            set { _Property20 = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class that contains anonymous type data.
    /// </summary>
    [Serializable]
    [DataContract]
    public class NonAnonymousTypeCollection
    {
        #region Private Fields
        private String[] _propertyName = null;
        private Object[] _propertyValue = null;
        private Type[] _propertyType = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the property name.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public String[] PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property value.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public Object[] PropertyValue
        {
            get { return _propertyValue; }
            set { _propertyValue = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public Type[] PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class that contains anonymous type data.
    /// </summary>
    [Serializable]
    [DataContract]
    public class AnonymousType
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
        [DataMember]
        [XmlElement(IsNullable = true)]
        public String PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property value.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
        public Object PropertyValue
        {
            get { return _propertyValue; }
            set { _propertyValue = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [DataMember]
        [XmlElement(IsNullable = true)]
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
    public sealed class TypeConversion : ITypeConversion
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly TypeConversion Instance = new TypeConversion();

        /// <summary>
        /// Static constructor
        /// </summary>
        static TypeConversion() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public TypeConversion()
        {
        }
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

            // Return the collection.
            return listGeneric;
        }
        #endregion

        #region Public Methods
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
                (DataTable data) => this.Mapper<TEntity>(data);

            // Project the data table.
            return Projector<TEntity>(table, function);
        }

        /// <summary>
        /// Convert all the object data into a array of
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
                                    if (property.Name.ToLower() == property.Name.ToLower())
                                    {
                                        // Set the object type property
                                        // with the property found in the
                                        // current object collection.
                                        property.SetValue(data, property.GetValue(item, null), null);
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
        #endregion
    }

    /// <summary>
    /// Class that contains members that control the
    /// convertion of object type data to strongly
    /// typed objects.
    /// </summary>
    public interface ITypeConversion
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
        /// Convert all the object data into a array of
        /// anonymous types.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of anonymous type data.</returns>
        AnonymousType[] GetAnonymousTypeData(IQueryable query);

        /// <summary>
        /// Convert all the object data into a data table.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        DataTable GetDataTable(IQueryable query, string tableName);

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
        List<T> GetListCollection<T>(DataTable dataTable)
            where T : new();

        /// <summary>
        /// Gets the list generic collection of the conversion type from the data table.
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <param name="conversionType">The type to convert to.</param>
        /// <returns>The list generic conversion type object.</returns>
        /// <remarks>The return object is of type 'List[conversionType]'.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        object ListGenericTypeTranslator(DataTable dataTable, Type conversionType);

        #endregion
    }

    /// <summary>
    /// Class that extends the System.Ling.IQueryable type.
    /// </summary>
    internal static class IQueryableExtensions
    {
        /// <summary>
        /// Gets the count expression by dynamically creates a count query on the specified criteria.
        /// </summary>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The query expression from the dynamic creation.</returns>
        internal static int Count(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return (int)source.Provider.Execute(
                System.Linq.Expressions.Expression.Call(
                    typeof(Queryable), "Count",
                    new Type[] { source.ElementType }, source.Expression));
        }
    }
}
