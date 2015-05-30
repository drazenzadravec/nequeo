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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nequeo.ComponentModel.Design
{
    /// <summary>
    /// Data object type column list user control designer.
    /// </summary>
    public partial class DataObjectColumnListControlDesigner : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataObjectColumnListControlDesigner()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dataObjectColumnCollection">The data object column collection container.</param>
        public DataObjectColumnListControlDesigner(DataObjectColumnCollection dataObjectColumnCollection)
        {
            InitializeComponent();
            _dataObjectColumnCollection = dataObjectColumnCollection;
        }

        private Type _dataObject;
        private string _dataObjectTypeName = string.Empty;
        private DataObjectColumnCollection _dataObjectColumnCollection = null;

        /// <summary>
        /// Gets sets, the data object column collection container.
        /// </summary>
        public DataObjectColumnCollection DataObjectColumnCollection
        {
            get { return _dataObjectColumnCollection; }
            set { _dataObjectColumnCollection = value; }
        }

        /// <summary>
        /// Gets sets, the data object type name to build.
        /// </summary>
        internal string DataObjectTypeName
        {
            get { return _dataObjectTypeName; }
            set { _dataObjectTypeName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataObjectColumnListControlDesigner_Load(object sender, EventArgs e)
        {
            if (_dataObjectColumnCollection != null)
            {
                _dataObjectTypeName = _dataObjectColumnCollection.DataObjectTypeName;
                _dataObject = Type.GetType(_dataObjectTypeName, true, true);
                propertyGridType.SelectedObject = Nequeo.Reflection.TypeAccessor.CreateInstance(_dataObject);
            }
            else
            {
                // Create a new instance and assign the data types.
                _dataObjectColumnCollection = new DataObjectColumnCollection(string.Empty);
                //propertyGridType.SelectedObject = TypeAccessor.CreateInstance(_dataObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {

        }
    }
}
