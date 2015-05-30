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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading;

using Nequeo.ComponentModel;
using Nequeo.Data;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Linq;

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// Generic data access user control.
    /// </summary>
    public partial class DataAccess : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataAccess()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets sets, the Sql order by clause.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The order by sql cluase. This is manditory.")]
        public string OrderByClause
        {
            get { return (string)GetValue(OrderByClauseProperty); }
            set { SetValue(OrderByClauseProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty OrderByClauseProperty =
            DependencyProperty.Register(
                "OrderByClause",
                typeof(string),
                typeof(DataAccess),
                new FrameworkPropertyMetadata(string.Empty,
                    new PropertyChangedCallback(OnOrderByClauseChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent OrderByClauseChangedEvent =
            EventManager.RegisterRoutedEvent(
            "OrderByClauseChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>),
            typeof(DataAccess));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> OrderByClauseChanged
        {
            add { AddHandler(OrderByClauseChangedEvent, value); }
            remove { RemoveHandler(OrderByClauseChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnOrderByClauseChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataAccess control = (DataAccess)obj;

            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(
                (string)args.OldValue, (string)args.NewValue, OrderByClauseChangedEvent);
            control.OnOrderByClauseChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnOrderByClauseChanged(RoutedPropertyChangedEventArgs<string> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Gets sets, the Sql where clause
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The where sql clause. This is manditory.")]
        public string WhereClause
        {
            get { return (string)GetValue(WhereClauseProperty); }
            set { SetValue(WhereClauseProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty WhereClauseProperty =
            DependencyProperty.Register(
                "WhereClause",
                typeof(string),
                typeof(DataAccess),
                new FrameworkPropertyMetadata(string.Empty,
                    new PropertyChangedCallback(OnWhereClauseChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent WhereClauseChangedEvent =
            EventManager.RegisterRoutedEvent(
            "WhereClauseChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>),
            typeof(DataAccess));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> WhereClauseChanged
        {
            add { AddHandler(WhereClauseChangedEvent, value); }
            remove { RemoveHandler(WhereClauseChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnWhereClauseChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataAccess control = (DataAccess)obj;

            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(
                (string)args.OldValue, (string)args.NewValue, WhereClauseChangedEvent);
            control.OnWhereClauseChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnWhereClauseChanged(RoutedPropertyChangedEventArgs<string> args)
        {
            RaiseEvent(args);
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
            get { return (ConnectionTypeModel)GetValue(ConnectionTypeModelProperty); }
            set { SetValue(ConnectionTypeModelProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectionTypeModelProperty =
            DependencyProperty.Register(
                "ConnectionTypeModel",
                typeof(ConnectionTypeModel),
                typeof(DataAccess),
                new PropertyMetadata());

        /// <summary>
        /// Gets sets, the data model.
        /// </summary>
        public Object DataModel
        {
            get { return (Object)GetValue(DataModelProperty); }
            set { SetValue(DataModelProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty DataModelProperty =
            DependencyProperty.Register(
                "DataModel",
                typeof(Object),
                typeof(DataAccess),
                new PropertyMetadata());

        /// <summary>
        /// Gets sets, the is load enabled.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("Is load enabled")]
        public bool IsLoadEnabled
        {
            get { return (bool)GetValue(IsLoadEnabledProperty); }
            set { SetValue(IsLoadEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadEnabledProperty =
            DependencyProperty.Register(
                "IsLoadEnabled",
                typeof(bool),
                typeof(DataAccess),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsLoadEnabledChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent IsLoadEnabledChangedEvent =
            EventManager.RegisterRoutedEvent(
            "IsLoadEnabledChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(DataAccess));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsLoadEnabledChanged
        {
            add { AddHandler(IsLoadEnabledChangedEvent, value); }
            remove { RemoveHandler(IsLoadEnabledChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnIsLoadEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataAccess control = (DataAccess)obj;
            control.EnableLoad();

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(
                (bool)args.OldValue, (bool)args.NewValue, IsLoadEnabledChangedEvent);
            control.OnIsLoadEnabledChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnIsLoadEnabledChanged(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Enable disable the load button
        /// </summary>
        private void EnableLoad()
        {
            btnLoad.IsEnabled = IsLoadEnabled;
        }

        /// <summary>
        /// Gets sets, the is update enabled.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("Is update enabled")]
        public bool IsUpdateEnabled
        {
            get { return (bool)GetValue(IsUpdateEnabledProperty); }
            set { SetValue(IsUpdateEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty IsUpdateEnabledProperty =
            DependencyProperty.Register(
                "IsUpdateEnabled",
                typeof(bool),
                typeof(DataAccess),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsUpdateEnabledChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent IsUpdateEnabledChangedEvent =
            EventManager.RegisterRoutedEvent(
            "IsUpdateEnabledChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(DataAccess));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsUpdateEnabledChanged
        {
            add { AddHandler(IsUpdateEnabledChangedEvent, value); }
            remove { RemoveHandler(IsUpdateEnabledChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnIsUpdateEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataAccess control = (DataAccess)obj;
            control.EnableUpdate();

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(
                (bool)args.OldValue, (bool)args.NewValue, IsUpdateEnabledChangedEvent);
            control.OnIsUpdateEnabledChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnIsUpdateEnabledChanged(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Enable disable the update button
        /// </summary>
        private void EnableUpdate()
        {
            btnUpdate.IsEnabled = IsUpdateEnabled;
        }

        /// <summary>
        /// Gets sets, the is insert enabled.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("Is insert enabled")]
        public bool IsInsertEnabled
        {
            get { return (bool)GetValue(IsInsertEnabledProperty); }
            set { SetValue(IsInsertEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInsertEnabledProperty =
            DependencyProperty.Register(
                "IsInsertEnabled",
                typeof(bool),
                typeof(DataAccess),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsInsertEnabledChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent IsInsertEnabledChangedEvent =
            EventManager.RegisterRoutedEvent(
            "IsInsertEnabledChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(DataAccess));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsInsertEnabledChanged
        {
            add { AddHandler(IsInsertEnabledChangedEvent, value); }
            remove { RemoveHandler(IsInsertEnabledChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnIsInsertEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataAccess control = (DataAccess)obj;
            control.EnableInsert();

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(
                (bool)args.OldValue, (bool)args.NewValue, IsInsertEnabledChangedEvent);
            control.OnIsInsertEnabledChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnIsInsertEnabledChanged(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Enable disable the insert button
        /// </summary>
        private void EnableInsert()
        {
            btnInsert.IsEnabled = IsInsertEnabled;
        }

        /// <summary>
        /// Gets sets, the is delete enabled.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("Is delete enabled")]
        public bool IsDeleteEnabled
        {
            get { return (bool)GetValue(IsDeleteEnabledProperty); }
            set { SetValue(IsDeleteEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDeleteEnabledProperty =
            DependencyProperty.Register(
                "IsDeleteEnabled",
                typeof(bool),
                typeof(DataAccess),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsDeleteEnabledChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent IsDeleteEnabledChangedEvent =
            EventManager.RegisterRoutedEvent(
            "IsDeleteEnabledChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(DataAccess));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsDeleteEnabledChanged
        {
            add { AddHandler(IsDeleteEnabledChangedEvent, value); }
            remove { RemoveHandler(IsDeleteEnabledChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnIsDeleteEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataAccess control = (DataAccess)obj;
            control.EnableDelete();

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(
                (bool)args.OldValue, (bool)args.NewValue, IsDeleteEnabledChangedEvent);
            control.OnIsDeleteEnabledChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnIsDeleteEnabledChanged(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Enable disable the delete button
        /// </summary>
        private void EnableDelete()
        {
            btnDelete.IsEnabled = IsDeleteEnabled;
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
        /// Load the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
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
                int skip = 0;
                int take = 1;
                string where = WhereClause;
                string orderBy = OrderByClause;

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
        /// Update the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
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
        /// Insert the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, RoutedEventArgs e)
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
        /// Delete the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
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
