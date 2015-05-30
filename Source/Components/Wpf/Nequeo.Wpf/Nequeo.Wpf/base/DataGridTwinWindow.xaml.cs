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
using Nequeo.Data.Custom;
using Nequeo.Data.Linq;

namespace Nequeo.Wpf
{
    /// <summary>
    /// Interaction logic for DataGridTwinWindow.xaml
    /// </summary>
    public partial class DataGridTwinWindow : Window
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataGridTwinWindow()
        {
            InitializeComponent();

            dataGridView.OnSelectedRecordChanged += new EventHandler(dataGridView_OnSelectedRecordChanged);
            dataGridViewRef.OnSelectedRecordChanged += new EventHandler(dataGridViewRef_OnSelectedRecordChanged);
        }

        private string _primarySearchPropertyName;
        private object _selectedReferenceRecord;
        private object _selectedRecord;
        private bool _apply = false;

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
                typeof(DataGridTwinWindow),
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
            typeof(DataGridTwinWindow));

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
            DataGridTwinWindow control = (DataGridTwinWindow)obj;

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
        /// Gets sets, the Sql order by clause for the reference data.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("The order by sql clause for the reference data. This is manditory.")]
        public string OrderByClauseReference
        {
            get { return (string)GetValue(OrderByClauseReferenceProperty); }
            set { SetValue(OrderByClauseReferenceProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty OrderByClauseReferenceProperty =
            DependencyProperty.Register(
                "OrderByClauseReference",
                typeof(string),
                typeof(DataGridTwinWindow),
                new FrameworkPropertyMetadata(string.Empty,
                    new PropertyChangedCallback(OnOrderByClauseReferenceChanged)));

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent OrderByClauseReferenceChangedEvent =
            EventManager.RegisterRoutedEvent(
            "OrderByClauseReferenceChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>),
            typeof(DataGridTwinWindow));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> OrderByClauseReferenceChanged
        {
            add { AddHandler(OrderByClauseReferenceChangedEvent, value); }
            remove { RemoveHandler(OrderByClauseReferenceChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnOrderByClauseReferenceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataGridTwinWindow control = (DataGridTwinWindow)obj;

            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(
                (string)args.OldValue, (string)args.NewValue, OrderByClauseReferenceChangedEvent);
            control.OnOrderByClauseReferenceChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnOrderByClauseReferenceChanged(RoutedPropertyChangedEventArgs<string> args)
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
                typeof(DataGridTwinWindow),
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
            typeof(DataGridTwinWindow));

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
            DataGridTwinWindow control = (DataGridTwinWindow)obj;

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
                typeof(DataGridTwinWindow),
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
            typeof(DataGridTwinWindow));

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
            DataGridTwinWindow control = (DataGridTwinWindow)obj;

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
                typeof(DataGridTwinWindow),
                new PropertyMetadata());

        /// <summary>
        /// Gets sets, connection informations and data obejct type.
        /// </summary>
        [Category("Data")]
        [DefaultValue(null)]
        [MergableProperty(false)]
        [Description("Connection informations and data obejct type.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public ConnectionTypeModel ConnectionTypeModelReference
        {
            get { return (ConnectionTypeModel)GetValue(ConnectionTypeModelReferenceProperty); }
            set { SetValue(ConnectionTypeModelReferenceProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectionTypeModelReferenceProperty =
            DependencyProperty.Register(
                "ConnectionTypeModelReference",
                typeof(ConnectionTypeModel),
                typeof(DataGridTwinWindow),
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
                typeof(DataGridTwinWindow),
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
            typeof(DataGridTwinWindow));

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
            DataGridTwinWindow control = (DataGridTwinWindow)obj;

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
        /// Gets sets, should the data be loaded on startup.
        /// </summary>
        [DefaultValue(false)]
        [Category("Data")]
        [Description("Should the data be loaded on startup.")]
        public bool LoadOnStart
        {
            get { return (bool)GetValue(LoadOnStartProperty); }
            set { SetValue(LoadOnStartProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty LoadOnStartProperty =
            DependencyProperty.Register(
                "LoadOnStart",
                typeof(bool),
                typeof(DataGridTwinWindow),
                new PropertyMetadata());

        /// <summary>
        /// Gets, the selected record.
        /// </summary>
        public object SelectedRecord
        {
            get { return _selectedRecord; }
            internal set { _selectedRecord = value; }
        }

        /// <summary>
        /// Gets, the selected reference record.
        /// </summary>
        public object SelectedReferenceRecord
        {
            get { return _selectedReferenceRecord; }
            internal set { _selectedReferenceRecord = value; }
        }

        /// <summary>
        /// Gets, the total number of records.
        /// </summary>
        public object TotalRecords
        {
            get { return dataGridView.TotalRecords; }
        }

        /// <summary>
        /// Gets sets, the key property name used to search for reference data.
        /// This value should be the primary key name of the top level data type model.
        /// </summary>
        public string PrimarySearchPropertyName
        {
            get { return _primarySearchPropertyName; }
            set { _primarySearchPropertyName = value; }
        }

        /// <summary>
        /// On closing window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_apply)
            {
                SelectedRecord = null;
                SelectedReferenceRecord = null;
            } 
        }

        /// <summary>
        /// Cancel the operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedRecord = null;
            SelectedReferenceRecord = null;
            this.Close();
        }

        /// <summary>
        /// Load the initial data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            dataGridView.OrderByClause = OrderByClause;
            dataGridView.Query = Query;
            dataGridView.WhereClause = WhereClause;
            dataGridView.ConnectionTypeModel = ConnectionTypeModel;
            dataGridView.MaxRecords = MaxRecords;
            dataGridView.LoadGrid();

            dataGridViewRef.OrderByClause = OrderByClauseReference;
            dataGridViewRef.ConnectionTypeModel = ConnectionTypeModelReference;
            dataGridViewRef.MaxRecords = MaxRecords;
        }

        /// <summary>
        /// Applies the selected record.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            _apply = true;
            SelectedRecord = dataGridView.SelectedRecord;
            SelectedReferenceRecord = dataGridViewRef.SelectedRecord;
            this.Close();
        }

        /// <summary>
        /// Edit where cluase expression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExpression_Click(object sender, RoutedEventArgs e)
        {
            DataGridSearchWindow input = new DataGridSearchWindow();
            input.SetSearchType(Type.GetType(ConnectionTypeModel.DataObjectTypeName, true, true));
            input.ShowDialog();
            Query = input.Query;
        }

        /// <summary>
        /// On selected records changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGridViewRef_OnSelectedRecordChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// On selected records changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGridView_OnSelectedRecordChanged(object sender, EventArgs e)
        {
            // Get the property information from the tyoe.
            Type dataType = Type.GetType(dataGridView.ConnectionTypeModel.DataObjectTypeName, true, true);
            PropertyInfo property = dataType.GetProperty(_primarySearchPropertyName);

            // Get the value of the property.
            object value = property.GetValue(dataGridView.SelectedRecord, null);

            // Contruct the where clause expression.
            dataGridViewRef.WhereClause = _primarySearchPropertyName + " = " + Nequeo.DataType.GetFormattedValue(property.PropertyType, value);
            dataGridViewRef.LoadGrid();
        }

        /// <summary>
        /// On window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadOnStart)
            {
                dataGridView.OrderByClause = OrderByClause;
                dataGridView.Query = Query;
                dataGridView.WhereClause = WhereClause;
                dataGridView.ConnectionTypeModel = ConnectionTypeModel;
                dataGridView.MaxRecords = MaxRecords;
                dataGridView.LoadGrid();

                dataGridViewRef.OrderByClause = OrderByClauseReference;
                dataGridViewRef.ConnectionTypeModel = ConnectionTypeModelReference;
                dataGridViewRef.MaxRecords = MaxRecords;
            }
        }
    }
}
