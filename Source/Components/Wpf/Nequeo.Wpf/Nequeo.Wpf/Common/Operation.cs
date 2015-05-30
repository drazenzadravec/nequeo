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
using Nequeo.Data.Linq;

namespace Nequeo.Wpf.Common
{
    /// <summary>
    /// Common operation handler
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Get the selected index list bindings.
        /// </summary>
        /// <typeparam name="TItemModel">Item type with the collection.</typeparam>
        /// <typeparam name="TDataModel">The data model from the collection</typeparam>
        /// <param name="items">The collection of item model types</param>
        /// <param name="itemModelPropertyName">The item model property name to match</param>
        /// <param name="dataModel">The data model instance.</param>
        /// <param name="dataModelPropertyName">The data model property name to match.</param>
        /// <returns>The selected index from the item collection.</returns>
        public static int GetSelectedIndex<TItemModel, TDataModel>(
            ItemCollection items, string itemModelPropertyName, TDataModel dataModel, string dataModelPropertyName)
        {
            int selectedIndex = -1;
            if (items.Count > 0)
            {
                // Create the enumerator.
                System.Collections.IEnumerator dataObjects = items.SourceCollection.GetEnumerator();

                // Iterate through the collection.
                while (dataObjects.MoveNext())
                {
                    // Get the current object.
                    object currentDataObject = dataObjects.Current;
                    TItemModel itemType = (TItemModel)currentDataObject;

                    try
                    {
                        // Find the first occurence of the income type match.
                        if (itemType.GetType().GetProperty(itemModelPropertyName).GetValue(itemType, null).ToString().ToLower() ==
                            dataModel.GetType().GetProperty(dataModelPropertyName).GetValue(dataModel, null).ToString().ToLower())
                        {
                            // Set the index of the item
                            selectedIndex = items.IndexOf(currentDataObject);
                            break;
                        }
                    }
                    catch { }
                }
                dataObjects.Reset();
            }

            // Return the index.
            return selectedIndex;
        }

        /// <summary>
        /// Get the connection type model.
        /// </summary>
        /// <typeparam name="TDataModel">The data model type</typeparam>
        /// <param name="dataAccess">The data access instance.</param>
        /// <returns>The connection type model</returns>
        public static ConnectionTypeModel GetTypeModel<TDataModel>(Nequeo.Wpf.UI.DataAccess dataAccess)
        {
            ConnectionTypeModel connectionModel = new ConnectionTypeModel();
            connectionModel.ConnectionDataType = dataAccess.ConnectionTypeModel.ConnectionDataType;
            connectionModel.ConnectionType = dataAccess.ConnectionTypeModel.ConnectionType;
            connectionModel.DataAccessProvider = dataAccess.ConnectionTypeModel.DataAccessProvider;
            connectionModel.DatabaseConnection = dataAccess.ConnectionTypeModel.DatabaseConnection;
            connectionModel.DataObjectTypeName = typeof(TDataModel).AssemblyQualifiedName;
            return connectionModel;
        }

        /// <summary>
        /// Get the connection type model.
        /// </summary>
        /// <param name="dataObjectTypeName">The Assembly Qualified Name of the data type model.</param>
        /// <param name="dataAccess">The data access instance.</param>
        /// <returns>The connection type model</returns>
        public static ConnectionTypeModel GetTypeModel(string dataObjectTypeName, Nequeo.Wpf.UI.DataAccess dataAccess)
        {
            ConnectionTypeModel connectionModel = new ConnectionTypeModel();
            connectionModel.ConnectionDataType = dataAccess.ConnectionTypeModel.ConnectionDataType;
            connectionModel.ConnectionType = dataAccess.ConnectionTypeModel.ConnectionType;
            connectionModel.DataAccessProvider = dataAccess.ConnectionTypeModel.DataAccessProvider;
            connectionModel.DatabaseConnection = dataAccess.ConnectionTypeModel.DatabaseConnection;
            connectionModel.DataObjectTypeName = dataObjectTypeName;
            return connectionModel;
        }
    }
}
