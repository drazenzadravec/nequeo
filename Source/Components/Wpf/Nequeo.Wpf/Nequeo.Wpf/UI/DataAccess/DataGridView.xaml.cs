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
using System.Linq.Expressions;
using System.Threading;

using Nequeo.ComponentModel;
using Nequeo.Data;
using Nequeo.Wpf.Common;
using Nequeo.Data.Custom;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Linq;

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView : UserControl
    {
        /// <summary>
        /// Selected operation
        /// </summary>
        internal enum SelectedOperation
        {
            First = 0,
            Previous = 1,
            Next = 2,
            Last = 3,
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataGridView()
        {
            InitializeComponent();
        }

        private object _selectedRecord;
        private int _currentIndex = 0;
        private int _totalRecords;

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
                typeof(DataGridView),
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
            typeof(DataGridView));

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
            DataGridView control = (DataGridView)obj;

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
                typeof(DataGridView),
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
            typeof(DataGridView));

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
            DataGridView control = (DataGridView)obj;

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
        /// Gets sets, the sql query
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The where sql clause search query.")]
        public QueryModel Query
        {
            get { return (QueryModel)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register(
                "Query",
                typeof(QueryModel),
                typeof(DataGridView),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnQueryChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent QueryChangedEvent =
            EventManager.RegisterRoutedEvent(
            "QueryChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<QueryModel>),
            typeof(DataGridView));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<QueryModel> QueryChanged
        {
            add { AddHandler(QueryChangedEvent, value); }
            remove { RemoveHandler(QueryChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnQueryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataGridView control = (DataGridView)obj;

            RoutedPropertyChangedEventArgs<QueryModel> e = new RoutedPropertyChangedEventArgs<QueryModel>(
                (QueryModel)args.OldValue, (QueryModel)args.NewValue, QueryChangedEvent);
            control.OnQueryChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnQueryChanged(RoutedPropertyChangedEventArgs<QueryModel> args)
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
                typeof(DataGridView),
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
                typeof(DataGridView),
                new PropertyMetadata());

        /// <summary>
        /// Gets sets, the maximum number of records to return in each batch.
        /// </summary>
        [DefaultValue(10)]
        [Category("Data")]
        [Description("The maximum number of records to return in each batch.")]
        public int MaxRecords
        {
            get { return (int)GetValue(MaxRecordsProperty); }
            set { SetValue(MaxRecordsProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxRecordsProperty =
            DependencyProperty.Register(
                "MaxRecords",
                typeof(int),
                typeof(DataGridView),
                new FrameworkPropertyMetadata(10,
                    new PropertyChangedCallback(OnMaxRecordsChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent MaxRecordsChangedEvent =
            EventManager.RegisterRoutedEvent(
            "MaxRecordsChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int>),
            typeof(DataGridView));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> MaxRecordsChanged
        {
            add { AddHandler(MaxRecordsChangedEvent, value); }
            remove { RemoveHandler(MaxRecordsChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnMaxRecordsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataGridView control = (DataGridView)obj;

            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(
                (int)args.OldValue, (int)args.NewValue, MaxRecordsChangedEvent);
            control.OnMaxRecordsChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnMaxRecordsChanged(RoutedPropertyChangedEventArgs<int> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Gets, the selected record.
        /// </summary>
        public object SelectedRecord
        {
            get { return _selectedRecord; }
        }

        /// <summary>
        /// Gets, the total number of records.
        /// </summary>
        public object TotalRecords
        {
            get { return _totalRecords; }
        }

        /// <summary>
        /// On selected recordf changed.
        /// </summary>
        public event EventHandler OnSelectedRecordChanged;

        /// <summary>
        /// On first complete
        /// </summary>
        public event EventHandler OnFirst;

        /// <summary>
        /// On previous complete
        /// </summary>
        public event EventHandler OnPrevious;

        /// <summary>
        /// On next complete
        /// </summary>
        public event EventHandler OnNext;

        /// <summary>
        /// On last complete
        /// </summary>
        public event EventHandler OnLast;

        /// <summary>
        /// On before first.
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeFirst;

        /// <summary>
        /// On before previous.
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforePrevious;

        /// <summary>
        /// On before next.
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeNext;

        /// <summary>
        /// On before last
        /// </summary>
        public event EventHandler<Nequeo.Custom.OperationArgs> OnBeforeLast;

        /// <summary>
        /// On first error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnFirstError;

        /// <summary>
        /// On previous error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnPreviousError;

        /// <summary>
        /// On next error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnNextError;

        /// <summary>
        /// On last error
        /// </summary>
        public event EventHandler<Nequeo.Custom.MessageArgs> OnLastError;

        /// <summary>
        /// User control loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _currentIndex = 0;
        }

        /// <summary>
        /// Data grid selected changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Create the enumerator.
            System.Collections.IEnumerator dataObjects = e.AddedItems.GetEnumerator();

            // Iterate through the collection.
            while (dataObjects.MoveNext())
            {
                object currentDataObject = dataObjects.Current;
                _selectedRecord = currentDataObject;
            }
            dataObjects.Reset();

            // Send a single that the slected record has changed.
            if (OnSelectedRecordChanged != null)
                OnSelectedRecordChanged(this, new EventArgs());
        }

        /// <summary>
        /// First operation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex = 0;
            LoadData(SelectedOperation.First, _currentIndex, MaxRecords);
            SetTotalRecordContent();
            EnableButton(_currentIndex);
            SetDetailsContent(_currentIndex);
        }

        /// <summary>
        /// Previous operation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex = _currentIndex - MaxRecords;
            LoadData(SelectedOperation.Previous, _currentIndex, MaxRecords);
            SetTotalRecordContent();
            EnableButton(_currentIndex);
            SetDetailsContent(_currentIndex);
        }

        /// <summary>
        /// Next operation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex = _currentIndex + MaxRecords;
            LoadData(SelectedOperation.Next, _currentIndex, MaxRecords);
            SetTotalRecordContent();
            EnableButton(_currentIndex);
            SetDetailsContent(_currentIndex);
        }

        /// <summary>
        /// Last operation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            LoadData(SelectedOperation.Last);
            SetTotalRecordContent();
            _currentIndex = _totalRecords;
            EnableButton(_currentIndex);
            SetDetailsContent(_currentIndex);
        }

        /// <summary>
        /// Load the data
        /// </summary>
        /// <param name="operation">The operation to perform</param>
        /// <param name="skip">The number of records to skip</param>
        /// <param name="take">The number of records to take</param>
        private void LoadData(SelectedOperation operation, int skip = 0, int take = 10)
        {
            try
            {
                switch(operation)
                {
                    case SelectedOperation.First:
                        // Should the load continue or be cancelled
                        if (OnBeforeFirst != null)
                        {
                            Nequeo.Custom.OperationArgs op = new Nequeo.Custom.OperationArgs(false);
                            OnBeforeFirst(this, op);

                            // Cancel operation if true.
                            if (op.Cancel)
                                return;
                        }
                        break;
                    case SelectedOperation.Previous:
                        // Should the load continue or be cancelled
                        if (OnBeforePrevious != null)
                        {
                            Nequeo.Custom.OperationArgs op = new Nequeo.Custom.OperationArgs(false);
                            OnBeforePrevious(this, op);

                            // Cancel operation if true.
                            if (op.Cancel)
                                return;
                        }
                        break;
                    case SelectedOperation.Next:
                        // Should the load continue or be cancelled
                        if (OnBeforeNext != null)
                        {
                            Nequeo.Custom.OperationArgs op = new Nequeo.Custom.OperationArgs(false);
                            OnBeforeNext(this, op);

                            // Cancel operation if true.
                            if (op.Cancel)
                                return;
                        }
                        break;
                    case SelectedOperation.Last:
                        // Should the load continue or be cancelled
                        if (OnBeforeLast != null)
                        {
                            Nequeo.Custom.OperationArgs op = new Nequeo.Custom.OperationArgs(false);
                            OnBeforeLast(this, op);

                            // Cancel operation if true.
                            if (op.Cancel)
                                return;
                        }
                        break;
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
                _totalRecords = Convert.ToInt32(count);

                switch (operation)
                {
                    case SelectedOperation.First:
                        skip = 0;
                        break;
                    case SelectedOperation.Previous:
                        skip = skip < 0 ? 0 : skip;
                        break;
                    case SelectedOperation.Next:
                        skip = skip > _totalRecords ? _totalRecords - take : skip;
                        break;
                    case SelectedOperation.Last:
                        skip = _totalRecords - take;
                        break;
                }

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

                // If an expression has been created.
                if (resultExpression != null)
                {
                    args = new Object[] 
                    { 
                        (skip > 0 ? skip : 0), 
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
                            (skip > 0 ? skip : 0), 
                            (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                            where
                        };
                }

                // If an expression has been created.
                if (resultExpression != null)
                {
                    args = new Object[] 
                    { 
                        (skip > 0 ? skip : 0), 
                        take,
                        (String.IsNullOrEmpty(orderBy) ? properties[0].Name : orderBy),
                        resultExpression
                    };
                }
                else
                {
                    // If the take count and where clause are set.
                    if ((take > 0) && (!String.IsNullOrEmpty(where)))
                        args = new Object[] 
                        { 
                            (skip > 0 ? skip : 0), 
                            take,
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
                dataGrid.ItemsSource = (IEnumerable)DataModel;
                
                switch (operation)
                {
                    case SelectedOperation.First:
                        if (OnFirst != null)
                            OnFirst(this, new EventArgs());
                        break;
                    case SelectedOperation.Previous:
                        if (OnPrevious != null)
                            OnPrevious(this, new EventArgs());
                        break;
                    case SelectedOperation.Next:
                        if (OnNext != null)
                            OnNext(this, new EventArgs());
                        break;
                    case SelectedOperation.Last:
                        if (OnLast != null)
                            OnLast(this, new EventArgs());
                        break;
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                switch (operation)
                {
                    case SelectedOperation.First:
                        if (OnFirstError != null)
                            OnFirstError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
                        break;
                    case SelectedOperation.Previous:
                        if (OnPreviousError != null)
                            OnPreviousError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
                        break;
                    case SelectedOperation.Next:
                        if (OnNextError != null)
                            OnNextError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
                        break;
                    case SelectedOperation.Last:
                        if (OnLastError != null)
                            OnLastError(this, new Nequeo.Custom.MessageArgs(ex.Message + " " + inner));
                        break;
                }
            }
        }

        /// <summary>
        /// Set the total record content.
        /// </summary>
        private void SetTotalRecordContent()
        {
            lblTotalRecords.Content = "Total number of records : " + _totalRecords.ToString();
        }

        /// <summary>
        /// Set the details content.
        /// </summary>
        /// <param name="currentIndex">The number to skip</param>
        private void SetDetailsContent(int currentIndex = 0)
        {
            int index = (currentIndex / MaxRecords) + 1;
            int total = (_totalRecords / MaxRecords) + 1;
            lblDetails.Content = "Index " + index.ToString() + " of " + total.ToString();
        }

        /// <summary>
        /// Enable or disable operation buttons.
        /// </summary>
        /// <param name="currentIndex">The number to skip</param>
        private void EnableButton(int currentIndex = 0)
        {
            if (currentIndex <= 0)
            {
                btnFirst.IsEnabled = false;
                btnPrevious.IsEnabled = false;

                if (_totalRecords <= MaxRecords)
                {
                    btnNext.IsEnabled = false;
                    btnLast.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                    btnLast.IsEnabled = true;
                }
            }
            else
            {
                btnFirst.IsEnabled = true;
                btnPrevious.IsEnabled = true;

                if (currentIndex >= _totalRecords)
                {
                    btnNext.IsEnabled = false;
                    btnLast.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                    btnLast.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Loads the first set of records into the grid.
        /// </summary>
        public void LoadGrid()
        {
            _currentIndex = 0;
            LoadData(SelectedOperation.First, 0, MaxRecords);
            SetTotalRecordContent();
            EnableButton(0);
            SetDetailsContent(0);
        }
    }
}
