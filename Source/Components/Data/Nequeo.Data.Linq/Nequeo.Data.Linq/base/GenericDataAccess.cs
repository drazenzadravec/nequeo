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
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;
using System.ComponentModel;
using System.Threading.Tasks;

using LinqTypes = Nequeo.Data.DataType.ProviderToDataTypes;

using Nequeo.ComponentModel;
using Nequeo.Linq.Extension;
using Nequeo.Data.DataType;
using Nequeo.Data.Linq;

namespace Nequeo.Data
{
    /// <summary>
    /// Generic data access CRUDE access.
    /// </summary>
    /// <typeparam name="T">The data model type to examine</typeparam>
    /// <typeparam name="D">The transform model type to examine</typeparam>
    public abstract class GenericDataAccess<T, D> : GenericDataAccess
        where T : class, new()
        where D : class, new()
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dataAccess">The data access implementation</param>
        protected GenericDataAccess(ICommonDataGenericBase<T> dataAccess)
        {
            this.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel<T, T>(dataAccess);

            this.OnLoad += new EventHandler(GenericDataAccess_OnLoad);
            this.OnUpdate += new EventHandler(GenericDataAccess_OnUpdate);
            this.OnInsert += new EventHandler(GenericDataAccess_OnInsert);
            this.OnDelete += new EventHandler(GenericDataAccess_OnDelete);

            this.OnLoadError += new EventHandler<Nequeo.Custom.MessageArgs>(GenericDataAccess_OnLoadError);
            this.OnUpdateError += new EventHandler<Nequeo.Custom.MessageArgs>(GenericDataAccess_OnUpdateError);
            this.OnInsertError += new EventHandler<Nequeo.Custom.MessageArgs>(GenericDataAccess_OnInsertError);
            this.OnDeleteError += new EventHandler<Nequeo.Custom.MessageArgs>(GenericDataAccess_OnDeleteError);
            this.OnTotalRecordsError += new EventHandler<Nequeo.Custom.MessageArgs>(GenericDataAccess_OnTotalRecordsError);

            this.OnBeforeLoad += new EventHandler<Nequeo.Custom.OperationArgs>(GenericDataAccess_OnBeforeLoad);
            this.OnBeforeUpdate += new EventHandler<Nequeo.Custom.OperationArgs>(GenericDataAccess_OnBeforeUpdate);
            this.OnBeforeInsert += new EventHandler<Nequeo.Custom.OperationArgs>(GenericDataAccess_OnBeforeInsert);
            this.OnBeforeDelete += new EventHandler<Nequeo.Custom.OperationArgs>(GenericDataAccess_OnBeforeDelete);
        }

        /// <summary>
        /// Gets sets, the data transform model.
        /// </summary>
        public Object TransformModel
        {
            get;
            set;
        }

        /// <summary>
        /// On before delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnBeforeDelete(object sender, Nequeo.Custom.OperationArgs e)
        {
            OnBeforeDeleteItem(e);
        }

        /// <summary>
        /// On before insert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnBeforeInsert(object sender, Nequeo.Custom.OperationArgs e)
        {
            OnBeforeInsertItem(e);
        }

        /// <summary>
        /// On before update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnBeforeUpdate(object sender, Nequeo.Custom.OperationArgs e)
        {
            OnBeforeUpdateItem(e);
        }

        /// <summary>
        /// On before load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnBeforeLoad(object sender, Nequeo.Custom.OperationArgs e)
        {
            OnBeforeLoadItem(e);
        }

        /// <summary>
        /// On delete error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnDeleteError(object sender, Nequeo.Custom.MessageArgs e)
        {
            OnDeleteErrorItem(e);
        }

        /// <summary>
        /// On insert error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnInsertError(object sender, Nequeo.Custom.MessageArgs e)
        {
            OnInsertErrorItem(e);
        }

        /// <summary>
        /// On update error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnUpdateError(object sender, Nequeo.Custom.MessageArgs e)
        {
            OnUpdateErrorItem(e);
        }

        /// <summary>
        /// On load error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnLoadError(object sender, Nequeo.Custom.MessageArgs e)
        {
            OnLoadErrorItem(e);
        }

        /// <summary>
        /// On total records error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnTotalRecordsError(object sender, Nequeo.Custom.MessageArgs e)
        {
            OnTotalRecordsErrorItem(e);
        }

        /// <summary>
        /// On delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnDelete(object sender, EventArgs e)
        {
            OnDeleteItem(e);
        }

        /// <summary>
        /// On insert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnInsert(object sender, EventArgs e)
        {
            OnInsertItem(e);
        }

        /// <summary>
        /// On update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnUpdate(object sender, EventArgs e)
        {
            OnUpdateItem(e);
        }

        /// <summary>
        /// On load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenericDataAccess_OnLoad(object sender, EventArgs e)
        {
            OnLoadItem(e);
        }

        /// <summary>
        /// On load item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLoadItem(EventArgs e)
        {
            // If the data model is enumerable
            if (DataModel is System.Collections.IEnumerable)
            {
                if (typeof(T) != typeof(D))
                {
                    TransformModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<T, D>((T[])DataModel);
                }
            }
            else
            {
                if (typeof(T) != typeof(D))
                {
                    TransformModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<T, D>((T)DataModel);
                }
            }
        }

        /// <summary>
        /// On update item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdateItem(EventArgs e)
        {
            if (typeof(T) != typeof(D))
            {
                if (TransformModel != null)
                {
                    // If the data model is enumerable
                    if (TransformModel is System.Collections.IEnumerable)
                    {
                        DataModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<D, T>((D[])TransformModel);
                    }
                    else
                    {
                        DataModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<D, T>((D)TransformModel);
                    }
                }
            }
            else
            {
                if (TransformModel != null)
                {
                    DataModel = TransformModel;
                }
            }
        }

        /// <summary>
        /// On insert item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnInsertItem(EventArgs e)
        {
            if (typeof(T) != typeof(D))
            {
                if (TransformModel != null)
                {
                    // If the data model is enumerable
                    if (TransformModel is System.Collections.IEnumerable)
                    {
                        DataModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<D, T>((D[])TransformModel);
                    }
                    else
                    {
                        DataModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<D, T>((D)TransformModel);
                    }
                }
            }
            else
            {
                if (TransformModel != null)
                {
                    DataModel = TransformModel;
                }
            }
        }

        /// <summary>
        /// On delete item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDeleteItem(EventArgs e)
        {
            if (typeof(T) != typeof(D))
            {
                if (TransformModel != null)
                {
                    // If the data model is enumerable
                    if (TransformModel is System.Collections.IEnumerable)
                    {
                        DataModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<D, T>((D[])TransformModel);
                    }
                    else
                    {
                        DataModel = Nequeo.Data.Control.AnonymousTypeFunction.TranslateType<D, T>((D)TransformModel);
                    }
                }
            }
            else
            {
                if (TransformModel != null)
                {
                    DataModel = TransformModel;
                }
            }
        }

        /// <summary>
        /// On total records error item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTotalRecordsErrorItem(Nequeo.Custom.MessageArgs e) { }

        /// <summary>
        /// On load error item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLoadErrorItem(Nequeo.Custom.MessageArgs e) { }

        /// <summary>
        /// On update error item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdateErrorItem(Nequeo.Custom.MessageArgs e) { }

        /// <summary>
        /// On insert error item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnInsertErrorItem(Nequeo.Custom.MessageArgs e) { }

        /// <summary>
        /// On delete error item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDeleteErrorItem(Nequeo.Custom.MessageArgs e) { }

        /// <summary>
        /// On before load item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeLoadItem(Nequeo.Custom.OperationArgs e) { }

        /// <summary>
        /// On before update item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeUpdateItem(Nequeo.Custom.OperationArgs e) { }

        /// <summary>
        /// On before insert item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeInsertItem(Nequeo.Custom.OperationArgs e) { }

        /// <summary>
        /// On before delete item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeDeleteItem(Nequeo.Custom.OperationArgs e) { }

    }

    /// <summary>
    /// Generic data access CRUDE access.
    /// </summary>
	public class GenericDataAccess
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        public GenericDataAccess()
        {
        }

        /// <summary>
        /// Gets sets, the Sql order by clause.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The order by sql cluase. This is manditory.")]
        public string OrderByClause
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, the Sql where clause
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The where sql clause.")]
        public string WhereClause
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, the number of records to skip
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The number of records to skip")]
        public int Skip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, the number of records to return
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The number of records to return")]
        public int Take
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, connection informations and data obejct type.
        /// </summary>
        [Category("Data")]
        [DefaultValue(null)]
        [MergableProperty(false)]
        [Description("Connection informations and data obejct type.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public ConnectionTypeModel ConnectionTypeModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, the sql where query.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The where sql clause search query.")]
        public QueryModel Query
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, the data model.
        /// </summary>
        public Object DataModel
        {
            get;
            set;
        }

        /// <summary>
        /// On load complete
        /// </summary>
        public event EventHandler OnLoad;

        /// <summary>
        /// On update complete
        /// </summary>
        public event EventHandler OnUpdate;

        /// <summary>
        /// On insert complete
        /// </summary>
        public event EventHandler OnInsert;

        /// <summary>
        /// On delete complete
        /// </summary>
        public event EventHandler OnDelete;

        /// <summary>
        /// On before load.
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeLoad;

        /// <summary>
        /// On before update.
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeUpdate;

        /// <summary>
        /// On before insert.
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeInsert;

        /// <summary>
        /// On before delete
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeDelete;

        /// <summary>
        /// On total records error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnTotalRecordsError;

        /// <summary>
        /// On load error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnLoadError;

        /// <summary>
        /// On update error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnUpdateError;

        /// <summary>
        /// On insert error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnInsertError;

        /// <summary>
        /// On delete error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnDeleteError;

        /// <summary>
        /// Get the total number of records for the where query.
        /// </summary>
        /// <returns>The total records found for the query; -1 if an error occurs.</returns>
        public int GetTotalRecords()
        {
            int totalRecods = -1;

            try
            {
                // Build the current data object type and
                // the  select data model generic type.
                Type dataType = Type.GetType(ConnectionTypeModel.DataObjectTypeName, true, true);
                Type dataAccessProviderType = Type.GetType(ConnectionTypeModel.DataAccessProvider, true, true);
                Type listGenericType = typeof(SelectDataGenericBase<>);

                // Create the generic type parameters
                // and create the genric type.
                Type[] typeArgs = { dataType };
                Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                // Add the genric tyoe contructor parameters
                // and create the generic type instance.
                object[] parameters = new object[] { 
                    ConnectionTypeModel.DatabaseConnection, 
                    ConnectionTypeModel.ConnectionType, 
                    ConnectionTypeModel.ConnectionDataType,
                    ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType)) };
                object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                // Get all properites in the data object type.
                PropertyInfo[] properties = dataType.GetProperties();

                // If the query exists.
                Object resultExpression = null;
                if (Query != null)
                {
                    // Create the expression constructor.
                    Nequeo.Data.DataType.DataTypeConversion dataTypeConversion =
                        new Nequeo.Data.DataType.DataTypeConversion(ConnectionTypeModel.ConnectionDataType);
                    Nequeo.Data.Linq.SqlStatementConstructor sql = new Nequeo.Data.Linq.SqlStatementConstructor(dataTypeConversion);

                    // Create the lambda expression.
                    resultExpression = sql.CreateLambdaExpression(Query.Queries, dataType, Query.Operand);
                }

                string where = WhereClause;
                Object[] argsRecords = new Object[] { };

                // Get the current object.
                if (resultExpression != null)
                {
                    // Add the query arguments
                    argsRecords = new Object[] { resultExpression };
                }
                else
                {
                    if (!String.IsNullOrEmpty(where))
                        argsRecords = new Object[] { where };
                }

                // Add the current data row to the
                // business object collection.
                object count = listGeneric.GetType().InvokeMember("GetRecordCount",
                    BindingFlags.DeclaredOnly | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, listGeneric, argsRecords);

                // Assign the total number of records.
                totalRecods = Convert.ToInt32(count);
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnTotalRecordsError != null)
                    OnTotalRecordsError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));

                // Indicate error.
                totalRecods = -1;
            }

            // Return the total number of recods.
            return totalRecods;
        }

        /// <summary>
        /// Load the selected data model data.
        /// </summary>
        public void Load()
        {
            try
            {
                // Should the load continue or be cancelled
                if (OnBeforeLoad != null)
                {
                    Nequeo.Custom.OperationArgs operation = new Nequeo.Custom.OperationArgs(false);
                    OnBeforeLoad(this, operation);

                    // Cancel operation if true.
                    if (operation.Cancel)
                        return;
                }

                // Build the current data object type and
                // the  select data model generic type.
                Type dataType = Type.GetType(ConnectionTypeModel.DataObjectTypeName, true, true);
                Type dataAccessProviderType = Type.GetType(ConnectionTypeModel.DataAccessProvider, true, true);
                Type listGenericType = typeof(SelectDataGenericBase<>);

                // Create the generic type parameters
                // and create the genric type.
                Type[] typeArgs = { dataType };
                Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                // Add the genric tyoe contructor parameters
                // and create the generic type instance.
                object[] parameters = new object[] { 
                    ConnectionTypeModel.DatabaseConnection, 
                    ConnectionTypeModel.ConnectionType, 
                    ConnectionTypeModel.ConnectionDataType,
                    ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType)) };
                object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                // Get all properites in the data object type.
                PropertyInfo[] properties = dataType.GetProperties();

                // If the query exists.
                Object resultExpression = null;
                if (Query != null)
                {
                    // Create the expression constructor.
                    Nequeo.Data.DataType.DataTypeConversion dataTypeConversion =
                        new Nequeo.Data.DataType.DataTypeConversion(ConnectionTypeModel.ConnectionDataType);
                    Nequeo.Data.Linq.SqlStatementConstructor sql = new Nequeo.Data.Linq.SqlStatementConstructor(dataTypeConversion);

                    // Create the lambda expression.
                    resultExpression = sql.CreateLambdaExpression(Query.Queries, dataType, Query.Operand);
                }

                string where = WhereClause;
                string orderBy = OrderByClause;

                // Get the current object.
                Object[] args = new Object[] 
                { 
                    (Skip > 0 ? Skip : 0), 
                    (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy) 
                };

                // If a take count is set.
                if (Take > 0)
                    args = new Object[] 
                { 
                    (Skip > 0 ? Skip : 0), 
                    Take,
                    (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy) 
                };

                // If an expression has been created.
                if (resultExpression != null)
                {
                    args = new Object[] 
                    { 
                        (Skip > 0 ? Skip : 0), 
                        (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                        resultExpression
                    };
                }
                else
                {
                    // If the where clause is set.
                    if (!String.IsNullOrEmpty(where))
                        args = new Object[] 
                        { 
                            (Skip > 0 ? Skip : 0), 
                            (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                            where
                        };
                }

                // If an expression has been created.
                if (resultExpression != null)
                {
                    args = new Object[] 
                    { 
                        (Skip > 0 ? Skip : 0), 
                        Take,
                        (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                        resultExpression
                    };
                }
                else
                {
                    // If the take count and where clause are set.
                    if ((Take > 0) && (!String.IsNullOrEmpty(where)))
                        args = new Object[] 
                        { 
                            (Skip > 0 ? Skip : 0), 
                            Take,
                            (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                            where
                        };
                }

                // Add the current data row to the
                // business object collection.
                object ret = listGeneric.GetType().InvokeMember("SelectData",
                    BindingFlags.DeclaredOnly | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, listGeneric, args);

                // Return the collection.
                DataModel = (IEnumerable)ret;

                if (OnLoad != null)
                    OnLoad(this, new EventArgs());
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnLoadError != null)
                    OnLoadError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
            }
        }

        /// <summary>
        /// Update the data model data.
        /// </summary>
        public void Update()
        {
            try
            {
                // Should the update continue or be cancelled
                if (OnBeforeUpdate != null)
                {
                    Nequeo.Custom.OperationArgs operation = new Nequeo.Custom.OperationArgs(false);
                    OnBeforeUpdate(this, operation);

                    // Cancel operation if true.
                    if (operation.Cancel)
                        return;
                }

                // Build the current data object type and
                // the  select data model generic type.
                Type dataType = Type.GetType(ConnectionTypeModel.DataObjectTypeName, true, true);
                Type dataAccessProviderType = Type.GetType(ConnectionTypeModel.DataAccessProvider, true, true);
                Type listGenericType = typeof(UpdateDataGenericBase<>);

                // Create the generic type parameters
                // and create the genric type.
                Type[] typeArgs = { dataType };
                Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                // Add the genric tyoe contructor parameters
                // and create the generic type instance.
                object[] parameters = new object[] { 
                        ConnectionTypeModel.DatabaseConnection, 
                        ConnectionTypeModel.ConnectionType, 
                        ConnectionTypeModel.ConnectionDataType,
                        ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType))};
                object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                // If the data model is enumerable
                if (DataModel is System.Collections.IEnumerable)
                {
                    // Cast the data object type as an enumerable object,
                    // get the enumerator.
                    System.Collections.IEnumerable items = (System.Collections.IEnumerable)DataModel;
                    System.Collections.IEnumerator dataObjects = items.GetEnumerator();
                    List<PropertyInfo> properties = dataType.GetProperties().ToList();

                    // Iterate through the collection.
                    while (dataObjects.MoveNext())
                    {
                        object currentDataObject = dataObjects.Current;

                        // Get the current object.
                        Object[] args = new Object[] 
                        { 
                            currentDataObject 
                        };

                        // Add the current data row to the
                        // business object collection.
                        object ret = listGeneric.GetType().InvokeMember("UpdateItem",
                            BindingFlags.DeclaredOnly | BindingFlags.Public |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, listGeneric, args);
                    }
                    dataObjects.Reset();
                }
                else
                {
                    // Get the current object.
                    Object[] args = new Object[] 
                        { 
                            DataModel 
                        };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("UpdateItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);
                }

                if (OnUpdate != null)
                    OnUpdate(this, new EventArgs());
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnUpdateError != null)
                    OnUpdateError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
            }
        }

        /// <summary>
        /// Insert the data model data.
        /// </summary>
        public void Insert()
        {
            try
            {
                // Should the insert continue or be cancelled
                if (OnBeforeInsert != null)
                {
                    Nequeo.Custom.OperationArgs operation = new Nequeo.Custom.OperationArgs(false);
                    OnBeforeInsert(this, operation);

                    // Cancel operation if true.
                    if (operation.Cancel)
                        return;
                }

                // Build the current data object type and
                // the  select data model generic type.
                Type dataType = Type.GetType(ConnectionTypeModel.DataObjectTypeName, true, true);
                Type dataAccessProviderType = Type.GetType(ConnectionTypeModel.DataAccessProvider, true, true);
                Type listGenericType = typeof(InsertDataGenericBase<>);

                // Create the generic type parameters
                // and create the genric type.
                Type[] typeArgs = { dataType };
                Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                // Add the genric tyoe contructor parameters
                // and create the generic type instance.
                object[] parameters = new object[] { 
                        ConnectionTypeModel.DatabaseConnection, 
                        ConnectionTypeModel.ConnectionType, 
                        ConnectionTypeModel.ConnectionDataType,
                        ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType))};
                object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                // If the data model is enumerable
                if (DataModel is System.Collections.IEnumerable)
                {
                    // Cast the data object type as an enumerable object,
                    // get the enumerator.
                    System.Collections.IEnumerable items = (System.Collections.IEnumerable)DataModel;
                    System.Collections.IEnumerator dataObjects = items.GetEnumerator();
                    List<PropertyInfo> properties = dataType.GetProperties().ToList();

                    // Iterate through the collection.
                    while (dataObjects.MoveNext())
                    {
                        object currentDataObject = dataObjects.Current;

                        // Get the current object.
                        Object[] args = new Object[] 
                        { 
                            currentDataObject 
                        };

                        // Add the current data row to the
                        // business object collection.
                        object ret = listGeneric.GetType().InvokeMember("InsertItem",
                            BindingFlags.DeclaredOnly | BindingFlags.Public |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, listGeneric, args);
                    }
                    dataObjects.Reset();
                }
                else
                {
                    // Get the current object.
                    Object[] args = new Object[] 
                        { 
                            DataModel 
                        };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("InsertItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);
                }

                if (OnInsert != null)
                    OnInsert(this, new EventArgs());
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnInsertError != null)
                    OnInsertError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
            }
        }

        /// <summary>
        /// Delete the data model data.
        /// </summary>
        public void Delete()
        {
            try
            {
                // Should the delete continue or be cancelled
                if (OnBeforeDelete != null)
                {
                    Nequeo.Custom.OperationArgs operation = new Nequeo.Custom.OperationArgs(false);
                    OnBeforeDelete(this, operation);

                    // Cancel operation if true.
                    if (operation.Cancel)
                        return;
                }

                // Build the current data object type and
                // the  select data model generic type.
                Type dataType = Type.GetType(ConnectionTypeModel.DataObjectTypeName, true, true);
                Type dataAccessProviderType = Type.GetType(ConnectionTypeModel.DataAccessProvider, true, true);
                Type listGenericType = typeof(DeleteDataGenericBase<>);

                // Create the generic type parameters
                // and create the genric type.
                Type[] typeArgs = { dataType };
                Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                // Add the genric tyoe contructor parameters
                // and create the generic type instance.
                object[] parameters = new object[] { 
                        ConnectionTypeModel.DatabaseConnection, 
                        ConnectionTypeModel.ConnectionType, 
                        ConnectionTypeModel.ConnectionDataType,
                        ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType))};
                object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                // If the data model is enumerable
                if (DataModel is System.Collections.IEnumerable)
                {
                    // Cast the data object type as an enumerable object,
                    // get the enumerator.
                    System.Collections.IEnumerable items = (System.Collections.IEnumerable)DataModel;
                    System.Collections.IEnumerator dataObjects = items.GetEnumerator();
                    List<PropertyInfo> properties = dataType.GetProperties().ToList();

                    // Iterate through the collection.
                    while (dataObjects.MoveNext())
                    {
                        object currentDataObject = dataObjects.Current;

                        // Get the current object.
                        Object[] args = new Object[] 
                        { 
                            currentDataObject 
                        };

                        // Add the current data row to the
                        // business object collection.
                        object ret = listGeneric.GetType().InvokeMember("DeleteItem",
                            BindingFlags.DeclaredOnly | BindingFlags.Public |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, listGeneric, args);
                    }
                    dataObjects.Reset();
                }
                else
                {
                    // Get the current object.
                    Object[] args = new Object[] 
                        { 
                            DataModel 
                        };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("DeleteItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);
                }

                if (OnDelete != null)
                    OnDelete(this, new EventArgs());
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnDeleteError != null)
                    OnDeleteError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
            }
        }
	}
}
