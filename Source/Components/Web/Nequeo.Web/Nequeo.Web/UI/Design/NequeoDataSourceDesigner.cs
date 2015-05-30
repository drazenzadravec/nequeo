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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.Compilation;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Threading;

using Nequeo.Web.Common;
using Nequeo.Data;

namespace Nequeo.Web.UI.Design
{
    /// <summary>
    /// The data source designer
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class NequeoDataSourceDesigner : DataSourceDesigner
    {
        #region Nequeo Data Source Designer
        private NequeoDataSource _control;
        private string _defaultViewName = "NequeoBinding";
        private NequeoDesignDataSourceView _view = null;

        /// <summary>
        /// Initialize the designer
        /// </summary>
        /// <param name="component">The current component object.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _control = (NequeoDataSource)Component;
        }

        /// <summary>
        /// Get a view
        /// </summary>
        /// <param name="viewName">The view state name.</param>
        /// <returns>The data source view designer.</returns>
        public override DesignerDataSourceView GetView(string viewName)
        {
            if (!viewName.Equals(_defaultViewName))
                return null;

            if (_view == null)
            {
                _view = new NequeoDesignDataSourceView(this, _defaultViewName);
            }

            return _view;
        }

        /// <summary>
        /// Get a list of view names
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNames()
        {
            return new string[] { "NequeoBinding" };
        }

        /// <summary>
        /// Do not allow refreshing the schema
        /// </summary>
        public override bool CanRefreshSchema
        {
            get { return false; }
        }
        
        /// <summary>
        /// Do not allow resizing
        /// </summary>
        public override bool AllowResize
        {
            get { return false; }
        }

        /// <summary>
        /// Get the Nequeo Data Source component
        /// </summary>
        public NequeoDataSource Control
        {
            get { return _control; }
        }
        #endregion
    }

    /// <summary>
    /// A design-time data source view
    /// </summary>
    public class NequeoDesignDataSourceView : DesignerDataSourceView
    {
        #region Nequeo Design Data Source View
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">The data source designer</param>
        /// <param name="viewName">The view state name.</param>
        public NequeoDesignDataSourceView(
            NequeoDataSourceDesigner owner, string viewName)
            : base(owner, viewName)
        {
            _owner = owner;
        }

        private ArrayList _data = null;
        private readonly NequeoDataSourceDesigner _owner;

        /// <summary>
        /// Get data for design-time display
        /// </summary>
        /// <param name="minimumRows">The minimun number of rows to display.</param>
        /// <param name="isSampleData">Is the data sample only.</param>
        /// <returns>The enumerable data collection.</returns>
        public override IEnumerable GetDesignTimeData(
            int minimumRows, out bool isSampleData)
        {
            if (_data == null)
            {
                // Create a set of design-time fake data
                _data = new ArrayList();
                for (int i = 1; i <= minimumRows; i++)
                {
                    _data.Add(new BookItem("ID_" + i.ToString(),
                        "Design-Time Nequeo Binding_" + i.ToString()));
                }
            }
            isSampleData = true;
            return _data as IEnumerable;
        }

        /// <summary>
        /// Get the sample data schema.
        /// </summary>
        public override IDataSourceViewSchema Schema
        {
            get { return new NequeoObjectViewSchema(_owner); }
        }

        /// <summary>
        /// Allow getting the record count
        /// </summary>
        public override bool CanRetrieveTotalRowCount
        {
            get { return true; }
        }

        /// <summary>
        /// Do not allow deletions
        /// </summary>
        public override bool CanDelete
        {
            get { return false; }
        }
        
        /// <summary>
        /// Do not allow insertions
        /// </summary>
        public override bool CanInsert
        {
            get { return false; }
        }
        
        /// <summary>
        /// Do not allow updates
        /// </summary>
        public override bool CanUpdate
        {
            get { return false; }
        }
        
        /// <summary>
        /// Do not allow paging
        /// </summary>
        public override bool CanPage
        {
            get { return false; }
        }
        
        /// <summary>
        /// Do not allow sorting
        /// </summary>
        public override bool CanSort
        {
            get { return false; }
        }
        #endregion
    }

    /// <summary>
    /// The runtime data source view
    /// </summary>
    public class NequeoDataSourceView : ObjectDataSourceView
    {
        #region Nequeo Data Source View
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">The current data source object.</param>
        /// <param name="viewName">The view state name.</param>
        /// <param name="context">The current HttpContext.</param>
        public NequeoDataSourceView(NequeoDataSource owner,
            string viewName, HttpContext context)
            : base(owner, viewName, context)
        {
            owner.SelectCountMethod = "GetCount";
            _owner = owner;
        }

        private int _totalRowCount = 0;
        private NequeoDataSource _owner = null;

        /// <summary>
        /// Execute the select method.
        /// </summary>
        /// <param name="arguments">The select arguments.</param>
        /// <returns>The collection of data.</returns>
        protected override IEnumerable ExecuteSelect(
            DataSourceSelectArguments arguments)
        {
            try
            {
                // Build the current data object type and
                // the  select data model generic type.
                Type dataType = BuildManager.GetType(_owner.ConnectionTypeModel.DataObjectTypeName, true, true);
                Type dataAccessProviderType = BuildManager.GetType(_owner.ConnectionTypeModel.DataAccessProvider, true, true);
                Type listGenericType = typeof(SelectDataGenericBase<>);

                // Create the generic type parameters
                // and create the genric type.
                Type[] typeArgs = { dataType };
                Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                // Add the genric tyoe contructor parameters
                // and create the generic type instance.
                object[] parameters = new object[] { 
                    _owner.ConnectionTypeModel.DatabaseConnection, 
                    _owner.ConnectionTypeModel.ConnectionType, 
                    _owner.ConnectionTypeModel.ConnectionDataType,
                    ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType)) };
                object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                // Get all properites in the data object type.
                PropertyInfo[] properties = dataType.GetProperties();
                int skip = arguments.StartRowIndex;
                int take = arguments.MaximumRows;
                string where = _owner.WhereClause;
                string orderBy = _owner.OrderByClause;

                // Get the current object.
                Object[] args = new Object[] 
                { 
                    (skip > 0 ? skip : 0), 
                    (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy) 
                };

                // If a take count is set.
                if (take > 0)
                    args = new Object[] 
                { 
                    (skip > 0 ? skip : 0), 
                    take,
                    (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy) 
                };

                // If the where clause is set.
                if (!String.IsNullOrEmpty(where))
                    args = new Object[] 
                { 
                    (skip > 0 ? skip : 0), 
                    (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                    where
                };

                // If the take count and where clause are set.
                if ((take > 0) && (!String.IsNullOrEmpty(where)))
                    args = new Object[] 
                { 
                    (skip > 0 ? skip : 0), 
                    take,
                    (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                    where
                };

                // Get the total number of records.
                if (arguments.RetrieveTotalRowCount)
                {
                    // Get the current object.
                    Object[] argsRecords = new Object[] { };
                    if (!String.IsNullOrEmpty(where))
                        argsRecords = new Object[] { where };

                    // Add the current data row to the
                    // business object collection.
                    object count = listGeneric.GetType().InvokeMember("GetRecordCount",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, argsRecords);

                    // Assign the total number of records.
                    arguments.TotalRowCount = Convert.ToInt32(count); ;
                    _totalRowCount = arguments.TotalRowCount;
                }

                // Add the current data row to the
                // business object collection.
                object ret = listGeneric.GetType().InvokeMember("SelectData",
                    BindingFlags.DeclaredOnly | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, listGeneric, args);

                // Return the collection.
                return (IEnumerable)ret;
            }
            catch (Exception ex) 
            {
                _owner.OnErrorEx(new Nequeo.Handler.ErrorMessageArgs(ex.Message));
            }
            return null;
        }

        /// <summary>
        /// Execute the insert method.
        /// </summary>
        /// <param name="values">The collection of properties and values.</param>
        /// <returns>The number of rows affected else -1.</returns>
        protected override int ExecuteInsert(IDictionary values)
        {
            if (values != null)
            {
                try
                {
                    // Get the collection of property name keys.
                    ICollection keys = values.Keys;

                    // Build the current data object type and
                    // the  select data model generic type.
                    Type dataType = BuildManager.GetType(_owner.ConnectionTypeModel.DataObjectTypeName, true, true);
                    Type dataAccessProviderType = BuildManager.GetType(_owner.ConnectionTypeModel.DataAccessProvider, true, true);
                    Type listGenericType = typeof(InsertDataGenericBase<>);

                    // Create the generic type parameters
                    // and create the genric type.
                    Type[] typeArgs = { dataType };
                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                    // Add the genric tyoe contructor parameters
                    // and create the generic type instance.
                    object[] parameters = new object[] { 
                        _owner.ConnectionTypeModel.DatabaseConnection, 
                        _owner.ConnectionTypeModel.ConnectionType, 
                        _owner.ConnectionTypeModel.ConnectionDataType,
                        ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType))};
                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);
                    object dataObject = Activator.CreateInstance(dataType);
                    List<PropertyInfo> properties = dataObject.GetType().GetProperties().ToList();

                    // For each key property name in the collection.
                    foreach (object item in keys)
                    {
                        string propertyName = item.ToString();
                        object value = values[item];

                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = properties.First(p => p.Name.ToLower() == propertyName.ToLower().TrimStart('_'));
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the current property within the property collection
                            // is the current column within the data collection.
                            if (propertyInfo.Name.ToLower() == propertyName.ToLower())
                            {
                                // If the data within the current row and column
                                // is 'NULL' then do not store the data.
                                if (value != null)
                                    // Assign the current data for the current row
                                    // into the current data business object.
                                    propertyInfo.SetValue(dataObject, value, null);
                            }
                        }
                    }

                    // Get the current object.
                    Object[] args = new Object[] 
                    { 
                        dataObject 
                    };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("InsertItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);

                    // Return the number of records affected.
                    return (Convert.ToBoolean(ret) ? 1 : -1);
                }
                catch (Exception ex)
                {
                    _owner.OnErrorEx(new Nequeo.Handler.ErrorMessageArgs(ex.Message));
                }
            }
            return -1;
        }

        /// <summary>
        /// Execute the update method.
        /// </summary>
        /// <param name="keys">A IDictionary of primary keys to use with the 
        /// UpdateMethod property to perform the update database operation. If 
        /// there are no keys associated with the method, pass null reference </param>
        /// <param name="values">A IDictionary of values to be used with the UpdateMethod 
        /// to perform the update database operation. If there are no parameters associated 
        /// with the method, pass null reference </param>
        /// <param name="oldValues">A IDictionary that represents the original values in the 
        /// underlying data store. If there are no parameters associated with the query, 
        /// pass null reference </param>
        /// <returns>The number of rows affected else -1.</returns>
        protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            if ((values != null) && (keys != null))
            {
                try
                {
                    // Get the collection of property name keys.
                    ICollection keysProps = null;
                    if (values != null)
                        keysProps = values.Keys;

                    ICollection keysPropKeys = null;
                    if (keys != null)
                        keysPropKeys = keys.Keys;

                    // Build the current data object type and
                    // the  select data model generic type.
                    Type dataType = BuildManager.GetType(_owner.ConnectionTypeModel.DataObjectTypeName, true, true);
                    Type dataAccessProviderType = BuildManager.GetType(_owner.ConnectionTypeModel.DataAccessProvider, true, true);
                    Type listGenericType = typeof(UpdateDataGenericBase<>);

                    // Create the generic type parameters
                    // and create the genric type.
                    Type[] typeArgs = { dataType };
                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                    // Add the genric tyoe contructor parameters
                    // and create the generic type instance.
                    object[] parameters = new object[] { 
                        _owner.ConnectionTypeModel.DatabaseConnection, 
                        _owner.ConnectionTypeModel.ConnectionType, 
                        _owner.ConnectionTypeModel.ConnectionDataType,
                        ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType))};
                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);
                    object dataObject = Activator.CreateInstance(dataType);
                    List<PropertyInfo> properties = dataObject.GetType().GetProperties().ToList();

                    if ((values != null))
                    {
                        // For each key property name in the collection.
                        foreach (object item in keysProps)
                        {
                            string propertyName = item.ToString();
                            object value = values[item];

                            // Find in the propert collection the current propert that matches
                            // the current column. Use the Predicate delegate object to
                            // initiate a search for the specified match.
                            PropertyInfo propertyInfo = null;
                            try
                            {
                                propertyInfo = properties.First(p => p.Name.ToLower() == propertyName.ToLower().TrimStart('_'));
                            }
                            catch { }
                            if (propertyInfo != null)
                            {
                                // If the current property within the property collection
                                // is the current column within the data collection.
                                if (propertyInfo.Name.ToLower() == propertyName.ToLower())
                                {
                                    // If the data within the current row and column
                                    // is 'NULL' then do not store the data.
                                    if (value != null)
                                        // Assign the current data for the current row
                                        // into the current data business object.
                                        propertyInfo.SetValue(dataObject, value, null);
                                }
                            }
                        }
                    }

                    if ((keys != null))
                    {
                        // For each key property name in the collection.
                        foreach (object item in keysPropKeys)
                        {
                            string propertyName = item.ToString();
                            object value = keys[item];

                            // Find in the propert collection the current propert that matches
                            // the current column. Use the Predicate delegate object to
                            // initiate a search for the specified match.
                            PropertyInfo propertyInfo = null;
                            try
                            {
                                propertyInfo = properties.First(p => p.Name.ToLower() == propertyName.ToLower().TrimStart('_'));
                            }
                            catch { }
                            if (propertyInfo != null)
                            {
                                // If the current property within the property collection
                                // is the current column within the data collection.
                                if (propertyInfo.Name.ToLower() == propertyName.ToLower())
                                {
                                    // If the data within the current row and column
                                    // is 'NULL' then do not store the data.
                                    if (value != null)
                                        // Assign the current data for the current row
                                        // into the current data business object.
                                        propertyInfo.SetValue(dataObject, value, null);
                                }
                            }
                        }
                    }

                    // Get the current object.
                    Object[] args = new Object[] 
                    { 
                        dataObject 
                    };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("UpdateItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);

                    // Return the number of records affected.
                    return (Convert.ToBoolean(ret) ? 1 : -1);
                }
                catch (Exception ex)
                {
                    _owner.OnErrorEx(new Nequeo.Handler.ErrorMessageArgs(ex.Message));
                }
            }
            return -1;
        }

        /// <summary>
        /// Execute the delete method.
        /// </summary>
        /// <param name="keys">A IDictionary of parameters used with the DeleteMethod 
        /// property to perform the delete operation. If there are no parameters 
        /// associated with the method, pass null reference </param>
        /// <param name="oldValues">A IDictionary that contains row values that are 
        /// evaluated, only if the ConflictDetection property is set to the CompareAllValues field</param>
        /// <returns>The number of rows affected else -1.</returns>
        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
        {
            if (keys != null)
            {
                try
                {
                    // Get the collection of property name keys.
                    ICollection keysProps = oldValues.Keys;

                    // Build the current data object type and
                    // the  select data model generic type.
                    Type dataType = BuildManager.GetType(_owner.ConnectionTypeModel.DataObjectTypeName, true, true);
                    Type dataAccessProviderType = BuildManager.GetType(_owner.ConnectionTypeModel.DataAccessProvider, true, true);
                    Type listGenericType = typeof(DeleteDataGenericBase<>);

                    // Create the generic type parameters
                    // and create the genric type.
                    Type[] typeArgs = { dataType };
                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                    // Add the genric tyoe contructor parameters
                    // and create the generic type instance.
                    object[] parameters = new object[] { 
                        _owner.ConnectionTypeModel.DatabaseConnection, 
                        _owner.ConnectionTypeModel.ConnectionType, 
                        _owner.ConnectionTypeModel.ConnectionDataType,
                        ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType))};
                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);
                    object dataObject = Activator.CreateInstance(dataType);
                    List<PropertyInfo> properties = dataObject.GetType().GetProperties().ToList();

                    // For each key property name in the collection.
                    foreach (object item in keysProps)
                    {
                        string propertyName = item.ToString();
                        object value = keys[item];

                        // Find in the propert collection the current propert that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = properties.First(p => p.Name.ToLower() == propertyName.ToLower().TrimStart('_'));
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the current property within the property collection
                            // is the current column within the data collection.
                            if (propertyInfo.Name.ToLower() == propertyName.ToLower())
                            {
                                // If the data within the current row and column
                                // is 'NULL' then do not store the data.
                                if (value != null)
                                    // Assign the current data for the current row
                                    // into the current data business object.
                                    propertyInfo.SetValue(dataObject, value, null);
                            }
                        }
                    }

                    // Get the current object.
                    Object[] args = new Object[] 
                    { 
                        dataObject 
                    };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("DeleteItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);

                    // Return the number of records affected.
                    return (Convert.ToBoolean(ret) ? 1 : -1);
                }
                catch (Exception ex)
                {
                    _owner.OnErrorEx(new Nequeo.Handler.ErrorMessageArgs(ex.Message));
                }
            }
            return -1;
        }

        /// <summary>
        /// Allow getting the record count
        /// </summary>
        public override bool CanRetrieveTotalRowCount
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the number of records in the current set of data
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return _totalRowCount;
        }

        /// <summary>
        /// Allow deletions
        /// </summary>
        public override bool CanDelete
        {
            get { return true; }
        }
        
        /// <summary>
        /// Allow insertions
        /// </summary>
        public override bool CanInsert
        {
            get { return true; }
        }
        
        /// <summary>
        /// Allow paging
        /// </summary>
        public override bool CanPage
        {
            get { return true; }
        }

        /// <summary>
        /// Allow sorting
        /// </summary>
        public override bool CanSort
        {
            get { return true; }
        }
        
        /// <summary>
        /// Allow updating
        /// </summary>
        public override bool CanUpdate
        {
            get { return true; }
        }
        #endregion
    }

    /// <summary>
    /// A custom View Schema class
    /// </summary>
    internal class NequeoObjectViewSchema : IDataSourceViewSchema
    {
        #region Nequeo Object View Schema
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="owner">The current data source designer</param>
        public NequeoObjectViewSchema(NequeoDataSourceDesigner owner)
        {
            _owner = owner;
        }

        private readonly NequeoDataSourceDesigner _owner;

        /// <summary>
        /// The name of this View Schema
        /// </summary>
        public string Name
        {
            get { return "ObjectViewSchema"; }
        }

        /// <summary>
        /// Build a Field Schema array
        /// </summary>
        /// <returns>The collection of fields for the data object type.</returns>
        public IDataSourceFieldSchema[] GetFields()
        {
            PropertyDescriptorCollection fields =
                TypeDescriptor.GetProperties(_owner.Control.DataObjectTypeName); ;

            IDataSourceFieldSchema[] schema = new IDataSourceFieldSchema[fields.Count];

            for (int i = 0; i < schema.Length; i++)
                schema[i] = new ObjectFieldSchema(fields[i]);

            return schema;
        }

        /// <summary>
        /// There are no child views, so return null
        /// </summary>
        /// <returns>The collection of data source properties.</returns>
        public IDataSourceViewSchema[] GetChildren()
        {
            return null;
        }
        #endregion
    }
}
