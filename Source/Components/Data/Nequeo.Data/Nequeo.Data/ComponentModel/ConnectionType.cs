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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

using Nequeo.Data.DataType;

namespace Nequeo.ComponentModel
{
    /// <summary>
    /// Connection type data model.
    /// </summary>
    [Serializable()]
    [TypeConverter(typeof(ConnectionTypeModelConverter))]
    public class ConnectionTypeModel
    {
        private string _dataObjectTypeName = string.Empty;
        private string _configurationKeyDatabaseConnection = string.Empty;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private string _dataAccessProvider;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionTypeModel()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dataObjectTypeName">The data object type name.</param>
        /// <param name="configurationKeyDatabaseConnection">The database connection string or the connection configuration key.</param>
        /// <param name="connectionType">The database connection type.</param>
        /// <param name="connectionDataType">The database connection data type.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public ConnectionTypeModel(string dataObjectTypeName, string configurationKeyDatabaseConnection,
            ConnectionContext.ConnectionType connectionType, ConnectionContext.ConnectionDataType connectionDataType, string dataAccessProvider)
        {
            _dataObjectTypeName = dataObjectTypeName;
            _configurationKeyDatabaseConnection = configurationKeyDatabaseConnection;
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            _dataAccessProvider = dataAccessProvider;
        }

        /// <summary>
        /// Gets sets, the data object type name.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The data object type name.")]
        [NotifyParentProperty(true)]
        public string DataObjectTypeName
        {
            get { return _dataObjectTypeName; }
            set { _dataObjectTypeName = value; }
        }

        /// <summary>
        /// Gets sets, the database connection string or the connection configuration key.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The database connection string or the connection configuration key")]
        [NotifyParentProperty(true)]
        public string DatabaseConnection
        {
            get { return _configurationKeyDatabaseConnection; }
            set { _configurationKeyDatabaseConnection = value; }
        }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue(typeof(ConnectionContext.ConnectionType), "None")]
        [Description("The database connection type.")]
        [NotifyParentProperty(true)]
        public ConnectionContext.ConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue(typeof(ConnectionContext.ConnectionDataType), "None")]
        [Description("The database connection data type.")]
        [NotifyParentProperty(true)]
        public ConnectionContext.ConnectionDataType ConnectionDataType
        {
            get { return _connectionDataType; }
            set { _connectionDataType = value; }
        }

        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The data access provider.")]
        [NotifyParentProperty(true)]
        public string DataAccessProvider
        {
            get { return _dataAccessProvider; }
            set { _dataAccessProvider = value; }
        }

        /// <summary>
        /// Convert the model to a string format.
        /// </summary>
        /// <returns>The converted type.</returns>
        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert the model to a string format.
        /// </summary>
        /// <param name="culture">The current culture.</param>
        /// <returns>The converted type.</returns>
        public string ToString(CultureInfo culture)
        {
            return TypeDescriptor.GetConverter(
                GetType()).ConvertToString(null, culture, this);
        }
    }

    /// <summary>
    /// Connection type data model type converter.
    /// </summary>
    public class ConnectionTypeModelConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to
        /// the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type,
        /// using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A System.Type that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">The System.Globalization.CultureInfo to use as the current culture.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                // If the value is null then return
                // the ConnectionTypeModel type.
                return new ConnectionTypeModel();
            }

            // If the value is a string type
            // then attempt to convert to
            // a ConnectionTypeModel type.
            if (value is string)
            {
                string s = (string)value;
                if (s.Length == 0)
                {
                    // If no string value then return default
                    // ConnectionTypeModel type.
                    return new ConnectionTypeModel();
                }

                string[] parts = s.Split(',');

                // If there are no parts or more than 5 parts then
                // throw exception, can not convert to ConnectionTypeModel type.
                if ((parts.Length < 1) || (parts.Length > 5))
                {
                    throw new ArgumentException(
                        "Connection type must have 5 parts.", "value");
                }

                // Get the connection type enums from the string value passed.
                ConnectionContext.ConnectionType connectionType = ConnectionContext.ConnectionTypeConverter.GetConnectionType(parts[2]);
                ConnectionContext.ConnectionDataType connectionDataType = ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(parts[3]);
                string dataAccessProvider = parts[4];

                // Return the ConnectionTypeModel type with values.
                return new ConnectionTypeModel(parts[0], parts[1], connectionType, connectionDataType, dataAccessProvider);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                // Is the value of type ConnectionTypeModel
                if (!(value is ConnectionTypeModel))
                {
                    // Throw exception if type is not support.
                    throw new ArgumentException(
                        "Invalid Connection Type Model", "value");
                }
            }

            // If the destination type is a string type
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    // Return empty string.
                    return String.Empty;
                }

                // Cast the value to type to a ConnectionTypeModel type.
                ConnectionTypeModel model = (ConnectionTypeModel)value;

                // Format the model type to a string type form.
                return String.Format("{0}, {1}, {2}, {3}, {4}",
                        model.DataObjectTypeName,
                        model.DatabaseConnection,
                        model.ConnectionType,
                        model.ConnectionDataType,
                        model.DataAccessProvider);
            }

            return base.ConvertTo(context, culture, value,
                destinationType);
        }
    }
}
