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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Nequeo.Data.Custom;

namespace Nequeo.Wpf
{
    /// <summary>
    /// Interaction logic for DataGridSearchWindow.xaml
    /// </summary>
    public partial class DataGridSearchWindow : Window
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataGridSearchWindow()
        {
            InitializeComponent();
        }

        private object _searchType = null;
        private bool _isCancelled = true;
        private Nequeo.Data.Linq.QueryModel _model = null;

        /// <summary>
        /// Gets or sets the query model.
        /// </summary>
        public Nequeo.Data.Linq.QueryModel Query
        {
            get { return GetQuery(); }
            set { _model = value; }
        }

        /// <summary>
        /// Set the search data type to load into the data grid.
        /// </summary>
        /// <param name="dataType">The data type to create.</param>
        public void SetSearchType(Type dataType)
        {
            // Create the collection type.
            Type listGenericType = typeof(System.Collections.ObjectModel.Collection<>);

            // Create the generic type parameters
            // and create the genric type.
            Type[] typeArgs = { dataType };
            Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

            // Create the collection type instance.
            _searchType = Activator.CreateInstance(listGenericTypeConstructor);
        }

        /// <summary>
        /// Get the query.
        /// </summary>
        /// <returns>The new query model.</returns>
        private Nequeo.Data.Linq.QueryModel GetQuery()
        {
            if (_model != null)
            {
                Nequeo.Data.Linq.QueryModel model = new Nequeo.Data.Linq.QueryModel();
                model.Operand = _model.Operand;
                model.Queries = _model.Queries == null ? null : _model.Queries.Where(u => u.IncludeInSearch).ToArray();
                return model;
            }
            else
                return null;
        }

        /// <summary>
        /// Generate the query if not cancelled.
        /// </summary>
        /// <returns>The new query model.</returns>
        private Nequeo.Data.Linq.QueryModel GenerateQuery()
        {
            // If null the create the object.
            if (_model == null)
                _model = new Nequeo.Data.Linq.QueryModel();

            _model.Operand = Linq.ExpressionOperandType.OrElse;
            List<Nequeo.Data.Linq.SearchQueryModel> search = new List<Nequeo.Data.Linq.SearchQueryModel>();

            int columnIndex = 0;

            // For each data grid item
            // except the last add item.
            for (int i = 0; i < dataGrid.Items.Count - 1; i++)
            {
                // Get the current object.
                object item = dataGrid.Items[i];

                // For each column in the data grid.
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    columnIndex++;

                    try
                    {
                        // Get the column name, property info name value.
                        string columnName = column.Header.ToString();
                        System.Reflection.PropertyInfo property = item.GetType().GetProperty(columnName);
                        object value = property.GetValue(item);

                        // If a value has been set.
                        if (value != null)
                        {
                            // Create a new query.
                            Nequeo.Data.Linq.SearchQueryModel query = new Nequeo.Data.Linq.SearchQueryModel();
                            query.Index = columnIndex;
                            query.ColumnName = columnName;
                            query.Value = value;
                            query.ValueType = property.PropertyType;
                            query.Operand = Linq.ExpressionOperandType.Equal;
                            query.IncludeInSearch = true;

                            // If not null then set previously
                            // Search for changes.
                            if (_model.Queries != null)
                            {
                                try
                                {
                                    // Find the match.
                                    Nequeo.Data.Linq.SearchQueryModel queryCurrent = _model.Queries.First(u => (u.Index == columnIndex));
                                    query.Operand = queryCurrent.Operand;
                                    query.IncludeInSearch = queryCurrent.IncludeInSearch;
                                }
                                catch (Exception ex) { string ii = ex.Message; }
                            }

                            // Add the query.
                            search.Add(query);
                        }
                    }
                    catch { }
                }
            }

            // Get the list of queries.
            _model.Queries = search.ToArray();

            // Return null if cancelled.
            if (!_isCancelled)
                // Return the query.
                return _model;
            else
                return null;
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        /// <summary>
        /// Loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isCancelled = true;
            dataGrid.DataContext = (_model != null ? _model :_searchType);
        }

        /// <summary>
        /// AddingNewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
        }

        /// <summary>
        /// InitializingNewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _isCancelled = true;
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            _isCancelled = false;
            this.Close();
        }

        /// <summary>
        /// Change expression.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExpression_Click(object sender, RoutedEventArgs e)
        {
            // Generate the query.
            GenerateQuery();

            // If data exists.
            if (_model.Queries.Length > 0)
            {
                // Open the search view windows.
                Nequeo.Wpf.DataGridViewWindow view = new Nequeo.Wpf.DataGridViewWindow();
                view.Data = _model.Queries;
                view.ShowDialog();
                _model.Queries = (Nequeo.Data.Linq.SearchQueryModel[])view.Data;
            }
        }
    }
}
