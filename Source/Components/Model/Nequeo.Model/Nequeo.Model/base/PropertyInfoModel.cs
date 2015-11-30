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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Nequeo.Model
{
    /// <summary>
    /// Property info model container
    /// </summary>
    [Serializable()]
    public class PropertyInfoModel
    {
        #region Private Fields
        private String _propertyName = null;
        private Object _propertyValue = null;
        private Type _propertyType = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the property name.
        /// </summary>
        [XmlElement(ElementName = "PropertyName", IsNullable = false)]
        public String PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property value.
        /// </summary>
        [XmlElement(ElementName = "PropertyValue", IsNullable = false)]
        public Object PropertyValue
        {
            get { return _propertyValue; }
            set { _propertyValue = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [XmlElement(ElementName = "PropertyType", IsNullable = false)]
        public Type PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
        #endregion
    }

    /// <summary>
    /// Property model container
    /// </summary>
    [Serializable()]
    public class PropertyModel
    {
        #region Private Fields
        private String _propertyName = null;
        private Type _propertyType = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the property name.
        /// </summary>
        [XmlElement(ElementName = "PropertyName", IsNullable = false)]
        public String PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [XmlElement(ElementName = "PropertyType", IsNullable = false)]
        public Type PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
        #endregion
    }

    /// <summary>
    /// Property string model container
    /// </summary>
    [Serializable()]
    public class PropertyStringModel
    {
        #region Private Fields
        private String _propertyName = null;
        private String _propertyType = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the property name.
        /// </summary>
        [XmlElement(ElementName = "PropertyName", IsNullable = false)]
        public String PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [XmlElement(ElementName = "PropertyType", IsNullable = false)]
        public String PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
        #endregion
    }
}
