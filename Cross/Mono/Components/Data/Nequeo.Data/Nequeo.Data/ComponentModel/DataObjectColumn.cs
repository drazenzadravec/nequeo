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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nequeo.ComponentModel
{
    /// <summary>
    /// Data object type column model.
    /// </summary>
    [Serializable()]
    public class DataObjectColumn
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataObjectColumn()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fieldName">The data object field name.</param>
        /// <param name="fieldText">The data object field text.</param>
        public DataObjectColumn(string fieldName, string fieldText)
        {
            _fieldName = fieldName;
            _fieldText = fieldText;
        }

        private string _fieldName = string.Empty;
        private string _fieldText = string.Empty;

        /// <summary>
        /// Gets sets, the data object field name.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The data object field name.")]
        [NotifyParentProperty(true)]
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        /// <summary>
        /// Gets sets, the data object field text.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The data object field text.")]
        [NotifyParentProperty(true)]
        public string FieldText
        {
            get { return _fieldText; }
            set { _fieldText = value; }
        }
    }

    /// <summary>
    /// Data object type column collection.
    /// </summary>
    public class DataObjectColumnCollection : Nequeo.Collections.Collection<DataControlField>, IList, IStateManager
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dataObjectTypeName">The data object type name.</param>
        public DataObjectColumnCollection(string dataObjectTypeName)
        {
            _dataObjectTypeName = dataObjectTypeName;
            _viewstate = new StateBag();
            _viewstate.Add("dataObjectTypeName", dataObjectTypeName);
        }

        private string _dataObjectTypeName = string.Empty;
        // The StateBag object that allows you to save
        // and restore view-state information.
        private StateBag _viewstate;

        /// <summary>
        /// Gets sets, the data object type name to build.
        /// </summary>
        public string DataObjectTypeName
        {
            get { return _dataObjectTypeName; }
            set { _dataObjectTypeName = value; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public new object this[int index]
        {
            get { return base[index]; }
            set { base[index] = (DataControlField)value; }
        }

        /// <summary>
        /// Adds an item to the System.Collections.IList.
        /// </summary>
        /// <param name="value">The System.Object to add to the System.Collections.IList.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        new public int Add(object value)
        {
            base.Add((DataControlField)value);
            return 1;
        }

        /// <summary>
        /// Determines whether the System.Collections.IList contains a specific value.
        /// </summary>
        /// <param name="value">The System.Object to locate in the System.Collections.IList.</param>
        /// <returns>true if the System.Object is found in the System.Collections.IList; otherwise, false.</returns>
        new public bool Contains(object value)
        {
            return base.Contains((DataControlField)value);
        }

        /// <summary>
        /// Determines the index of a specific item in the System.Collections.IList.
        /// </summary>
        /// <param name="value">The System.Object to locate in the System.Collections.IList.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        new public int IndexOf(object value)
        {
            return base.IndexOf((DataControlField)value);
        }

        /// <summary>
        /// Inserts an item to the System.Collections.IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The System.Object to insert into the System.Collections.IList.</param>
        new public void Insert(int index, object value)
        {
            base.Insert(index, (DataControlField)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.IList.
        /// </summary>
        /// <param name="value">The System.Object to remove from the System.Collections.IList.</param>
        new public void Remove(object value)
        {
            base.Remove((DataControlField)value);
        }

        /// <summary>
        /// Copies the elements of the System.Collections.ICollection 
        /// to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.ICollection. The System.Array must have zero-based
        /// indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        new public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be 
        /// used to iterate through the collection.</returns>
        public new IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether a server control is tracking its view state changes.
        /// </summary>
        public bool IsTrackingViewState
        {
            get { return ((IStateManager)_viewstate).IsTrackingViewState; }
        }

        /// <summary>
        ///  When implemented by a class, loads the server control's 
        ///  previously saved view state to the control.
        /// </summary>
        /// <param name="state">An System.Object that contains the saved view state values for the control.</param>
        public void LoadViewState(object state)
        {
            _dataObjectTypeName = (string)_viewstate["dataObjectTypeName"];
            if (state != null)
                ((IStateManager)_viewstate).LoadViewState(state);

        }

        /// <summary>
        /// When implemented by a class, saves the changes to a 
        /// server control's view state to an System.Object.
        /// </summary>
        /// <returns>The System.Object that contains the view state changes.</returns>
        public object SaveViewState()
        {
            // Check whether the message property exists in 
            // the ViewState property, and if it does, check
            // whether it has changed since the most recent
            // TrackViewState method call.
            if (!((IDictionary)_viewstate).Contains("dataObjectTypeName") || _viewstate.IsItemDirty("dataObjectTypeName"))
            {
                _viewstate.Clear();
                // Add the _message property to the StateBag.
                _viewstate.Add("dataObjectTypeName", _dataObjectTypeName);
            }
            return ((IStateManager)_viewstate).SaveViewState();

        }

        /// <summary>
        /// When implemented by a class, instructs the server 
        /// control to track changes to its view state.
        /// </summary>
        public void TrackViewState()
        {
            ((IStateManager)_viewstate).TrackViewState();
        }
    }
}
