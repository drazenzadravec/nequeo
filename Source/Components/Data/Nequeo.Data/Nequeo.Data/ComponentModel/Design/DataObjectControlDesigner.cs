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
using System.Reflection;

namespace Nequeo.ComponentModel.Design
{
    /// <summary>
    /// The data object designer control.
    /// </summary>
    public partial class DataObjectControlDesigner : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataObjectControlDesigner()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dataObject">The data object type.</param>
        public DataObjectControlDesigner(Object dataObject)
        {
            InitializeComponent();
            _dataObject = dataObject;
        }

        private Object _dataObject = null;
        private PropertyDescriptor _property = null;

        /// <summary>
        /// Gets sets, the data object type.
        /// </summary>
        public Object DataObject
        {
            get { return _dataObject; }
            set { _dataObject = value; }
        }

        /// <summary>
        /// Sets, the data object type property.
        /// </summary>
        public PropertyDescriptor Property
        {
            set { _property = value; }
        }

        /// <summary>
        /// The data object designer is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataObjectControlDesigner_Load(object sender, EventArgs e)
        {
            // Assign the current data object.
            propertyGridMain.SelectedObject = _dataObject;

            // If data object is null then use property descriptor.
            if (_dataObject == null && _property != null)
                btnCreateNew.Enabled = true;
            else
                btnApplyChanges.Enabled = true;
        }

        /// <summary>
        /// Selected grid item changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyGridMain_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
        }

        /// <summary>
        /// Property value changes.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGridMain_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }

        /// <summary>
        /// Apply changes is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyChanges_Click(object sender, EventArgs e)
        {
            // Get all the changes and return data object.
            _dataObject = propertyGridMain.SelectedObject;
        }

        /// <summary>
        /// Create a new instance of a sub object type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            // Create a new reference object
            // from the property descriptor.
            _dataObject = Nequeo.Reflection.TypeAccessor.CreateInstance(_property.PropertyType);
            propertyGridMain.SelectedObject = _dataObject;

            btnCreateNew.Enabled = false;
            btnApplyChanges.Enabled = true;
        }
    }
}
