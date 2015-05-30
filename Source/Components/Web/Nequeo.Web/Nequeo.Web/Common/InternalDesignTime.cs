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
using System.Data;
using System.Security.Permissions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Nequeo.Web.Common
{
    /// <summary>
    /// The generic object field builder.
    /// </summary>
    internal class ObjectFieldSchema : IDataSourceFieldSchema
    {
        #region Object Field Schema Builder
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="propertyDescriptor">the property descriptor.</param>
        public ObjectFieldSchema(PropertyDescriptor propertyDescriptor)
        {
            _propertyDescriptor = propertyDescriptor;

            DataObjectFieldAttribute attr =
                (DataObjectFieldAttribute)_propertyDescriptor.Attributes[typeof(DataObjectFieldAttribute)];

            if (attr != null)
            {
                _length = attr.Length;
                _primaryKey = attr.PrimaryKey;
                _isIdentity = attr.IsIdentity;
                _isNullable = attr.IsNullable;
            }
        }

        private readonly PropertyDescriptor _propertyDescriptor;
        private readonly int _length = -1;
        private readonly bool _isIdentity;
        private readonly bool _isNullable;
        private readonly bool _primaryKey;

        public Type DataType { get { return _propertyDescriptor.PropertyType; } }
        public bool Identity { get { return _isIdentity; } }
        public bool IsReadOnly { get { return _propertyDescriptor.IsReadOnly; } }
        public bool IsUnique { get { return false; } }
        public int Length { get { return _length; } }
        public string Name { get { return _propertyDescriptor.Name; } }
        public int Precision { get { return -1; } }
        public bool PrimaryKey { get { return _primaryKey; } }
        public int Scale { get { return -1; } }

        /// <summary>
        /// Gets, the nullable type.
        /// </summary>
        public bool Nullable
        {
            get
            {
                Type type = _propertyDescriptor.PropertyType;
                Type underlyingType = System.Nullable.GetUnderlyingType(type);

                return underlyingType != null ? true : _isNullable;
            }
        }
        #endregion
    }

    /// <summary>
    /// The initial data type to display.
    /// </summary>
    internal class BookItem
    {
        #region Initial Data Object Type
        private string _id;
        private string _sample;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="id">The initial id.</param>
        /// <param name="sample">The initial sample data.</param>
        public BookItem(string id, string sample)
        {
            _id = id;
            _sample = sample;
        }

        /// <summary>
        /// Gets, the initial id.
        /// </summary>
        public string ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets, the initial sample data.
        /// </summary>
        public string Sample
        {
            get { return _sample; }
            set { _sample = value; }
        }
        #endregion
    }
}
