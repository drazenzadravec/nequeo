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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Nequeo.Extension;

namespace Nequeo.ComponentModel.Design
{
    /// <summary>
    /// Connection type user control designer.
    /// </summary>
    public partial class ConnectionTypeControlDesigner : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConnectionTypeControlDesigner()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionTypeModel">The connection type data model container.</param>
        public ConnectionTypeControlDesigner(ConnectionTypeModel connectionTypeModel)
        {
            InitializeComponent();
            _connectionTypeModel = connectionTypeModel;
        }

        private ConnectionTypeModel _connectionTypeModel = null;

        /// <summary>
        /// Gets sets, the connection type data model container.
        /// </summary>
        public ConnectionTypeModel ConnectionTypeModel
        {
            get { return _connectionTypeModel; }
            set { _connectionTypeModel = value; }
        }

        /// <summary>
        /// The control starts to load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionTypeControlDesigner_Load(object sender, EventArgs e)
        {
            // Get the enum values list items for connection type
            // and connection data type.
            string[] connectionType = Enum.GetNames(typeof(Nequeo.Data.DataType.ConnectionContext.ConnectionType));
            string[] connectionDataType = Enum.GetNames(typeof(Nequeo.Data.DataType.ConnectionContext.ConnectionDataType));

            // Add the collection.
            cboConnectionType.Items.AddRange(connectionType);
            cboConnectionDataType.Items.AddRange(connectionDataType);

            // If the connection type model
            // is not null
            if (_connectionTypeModel != null)
            {
                // Assign the selected value.
                cboConnectionType.SelectedValue = _connectionTypeModel.ConnectionType.ToString();
                cboConnectionDataType.SelectedValue = _connectionTypeModel.ConnectionDataType.ToString();

                // Set the selected index for each type.
                cboConnectionType.SelectedIndex = connectionType.FindIndex(u => u == _connectionTypeModel.ConnectionType.ToString());
                cboConnectionDataType.SelectedIndex = connectionDataType.FindIndex(u => u == _connectionTypeModel.ConnectionDataType.ToString());

                // Assign the text values.
                txtDatabaseConnection.Text = _connectionTypeModel.DatabaseConnection;
                txtDataObjectTypeName.Text = _connectionTypeModel.DataObjectTypeName;
                txtDataAccessProvider.Text = _connectionTypeModel.DataAccessProvider;
            }
            else
            {
                // Create a new instance and assign the data types.
                _connectionTypeModel = new ConnectionTypeModel();
                cboConnectionType.SelectedValue = (_connectionTypeModel.ConnectionType = Nequeo.Data.DataType.ConnectionContext.ConnectionType.None).ToString();
                cboConnectionDataType.SelectedValue = (_connectionTypeModel.ConnectionDataType = Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.None).ToString();
                cboConnectionType.SelectedIndex = 0;
                cboConnectionDataType.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Connection type index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _connectionTypeModel.ConnectionType =
                Nequeo.Data.DataType.ConnectionContext.ConnectionTypeConverter.GetConnectionType(cboConnectionType.Text);
        }

        /// <summary>
        /// Connection data type index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboConnectionDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _connectionTypeModel.ConnectionDataType =
                Nequeo.Data.DataType.ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(cboConnectionDataType.Text);
        }

        /// <summary>
        /// Database connection text changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDatabaseConnection_TextChanged(object sender, EventArgs e)
        {
            _connectionTypeModel.DatabaseConnection = txtDatabaseConnection.Text;
        }

        /// <summary>
        /// Data object type name text changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDataObjectTypeName_TextChanged(object sender, EventArgs e)
        {
            _connectionTypeModel.DataObjectTypeName = txtDataObjectTypeName.Text;
        }

        /// <summary>
        /// Data access provider text changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDataAccessProvider_TextChanged(object sender, EventArgs e)
        {
            _connectionTypeModel.DataAccessProvider = txtDataAccessProvider.Text;
        }
    }
}
