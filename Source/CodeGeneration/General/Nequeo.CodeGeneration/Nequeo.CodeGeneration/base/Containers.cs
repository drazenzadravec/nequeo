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
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Nequeo.CodeGeneration
{
    /// <summary>
    /// Data model container.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class DataModelContainer
    {
        #region Private Fields
        private string _ClassName = null;
        private string _Database = null;
        private string _NamespaceCompanyName = null;
        private string[] _PropertyName = null;
        private string[] _PropertyType = null;
        private bool[] _PropertyIsNullable = null;
        private string[] _PropertyDefaultValue = null;
        private string _NamespaceExtendedName = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
        public string NamespaceExtendedName
        {
            get { return _NamespaceExtendedName; }
            set { _NamespaceExtendedName = value; }
        }

        /// <summary>
        /// Gets sets, the class name.
        /// </summary>
        [XmlElement(ElementName = "ClassName", IsNullable = false)]
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        /// <summary>
        /// Gets sets, the database name.
        /// </summary>
        [XmlElement(ElementName = "Database", IsNullable = false)]
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceCompanyName", IsNullable = false)]
        public string NamespaceCompanyName
        {
            get { return _NamespaceCompanyName; }
            set { _NamespaceCompanyName = value; }
        }

        /// <summary>
        /// Gets sets, the name of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyName", IsNullable = false)]
        public string[] PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        /// <summary>
        /// Gets sets, the type of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyType", IsNullable = false)]
        public string[] PropertyType
        {
            get { return _PropertyType; }
            set { _PropertyType = value; }
        }

        /// <summary>
        /// Gets sets, the property null indicator.
        /// </summary>
        [XmlArray(ElementName = "PropertyIsNullable", IsNullable = false)]
        public bool[] PropertyIsNullable
        {
            get { return _PropertyIsNullable; }
            set { _PropertyIsNullable = value; }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDefaultValue", IsNullable = true)]
        public string[] PropertyDefaultValue
        {
            get { return _PropertyDefaultValue; }
            set { _PropertyDefaultValue = value; }
        }
        #endregion
    }

    /// <summary>
    /// Linq model container.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class LinqToSqlModelContainer
    {
        #region Private Fields
        private string _ClassName = null;
        private string _Database = null;
        private string _NamespaceCompanyName = null;
        private string[] _PropertyName = null;
        private string[] _PropertyType = null;
        private string[] _PropertyDatabaseType = null;
        private bool[] _PropertyIsNullable = null;
        private string[] _PropertyDefaultValue = null;
        private string[] _PropertyDatabaseColumnName = null;
        private string _NamespaceExtendedName = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
        public string NamespaceExtendedName
        {
            get { return _NamespaceExtendedName; }
            set { _NamespaceExtendedName = value; }
        }

        /// <summary>
        /// Gets sets, the name of the class.
        /// </summary>
        [XmlElement(ElementName = "ClassName", IsNullable = false)]
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        /// <summary>
        /// Gets sets, the database name.
        /// </summary>
        [XmlElement(ElementName = "Database", IsNullable = false)]
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceCompanyName", IsNullable = false)]
        public string NamespaceCompanyName
        {
            get { return _NamespaceCompanyName; }
            set { _NamespaceCompanyName = value; }
        }

        /// <summary>
        /// Gets sets, the property name.
        /// </summary>
        [XmlArray(ElementName = "PropertyName", IsNullable = false)]
        public string[] PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [XmlArray(ElementName = "PropertyType", IsNullable = false)]
        public string[] PropertyType
        {
            get { return _PropertyType; }
            set { _PropertyType = value; }
        }

        /// <summary>
        /// Gets sets the property database type.
        /// </summary>
        [XmlArray(ElementName = "PropertyDatabaseType", IsNullable = false)]
        public string[] PropertyDatabaseType
        {
            get { return _PropertyDatabaseType; }
            set { _PropertyDatabaseType = value; }
        }

        /// <summary>
        /// Gets sets, the property null indicator.
        /// </summary>
        [XmlArray(ElementName = "PropertyIsNullable", IsNullable = false)]
        public bool[] PropertyIsNullable
        {
            get { return _PropertyIsNullable; }
            set { _PropertyIsNullable = value; }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDefaultValue", IsNullable = false)]
        public string[] PropertyDefaultValue
        {
            get { return _PropertyDefaultValue; }
            set { _PropertyDefaultValue = value; }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDatabaseColumnName", IsNullable = false)]
        public string[] PropertyDatabaseColumnName
        {
            get { return _PropertyDatabaseColumnName; }
            set { _PropertyDatabaseColumnName = value; }
        }
        #endregion
    }

    /// <summary>
    /// Mvc model container.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class MvcModelContainer
    {
        #region Private Fields
        private string _ClassName = null;
        private string _Database = null;
        private string _NamespaceCompanyName = null;
        private string[] _PropertyName = null;
        private string[] _PropertyType = null;
        private bool[] _PropertyIsNullable = null;
        private string[] _PropertyDefaultValue = null;
        private string _NamespaceExtendedName = null;
        private string[] _LabelText = null;
        private List<System.ComponentModel.DataAnnotations.DataType?> _DataType = null;
        private List<Attribute[]> _Attributes = null;
        private Type _MetadataTypeExtension = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the Metadata Type Extension.
        /// </summary>
        [XmlElement(ElementName = "MetadataTypeExtension", IsNullable = true)]
        public Type MetadataTypeExtension
        {
            get { return _MetadataTypeExtension; }
            set { _MetadataTypeExtension = value; }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
        public string NamespaceExtendedName
        {
            get { return _NamespaceExtendedName; }
            set { _NamespaceExtendedName = value; }
        }

        /// <summary>
        /// Gets sets, the class name.
        /// </summary>
        [XmlElement(ElementName = "ClassName", IsNullable = false)]
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        /// <summary>
        /// Gets sets, the database name.
        /// </summary>
        [XmlElement(ElementName = "Database", IsNullable = false)]
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceCompanyName", IsNullable = false)]
        public string NamespaceCompanyName
        {
            get { return _NamespaceCompanyName; }
            set { _NamespaceCompanyName = value; }
        }

        /// <summary>
        /// Gets sets, the name of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyName", IsNullable = false)]
        public string[] PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        /// <summary>
        /// Gets sets, the type of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyType", IsNullable = false)]
        public string[] PropertyType
        {
            get { return _PropertyType; }
            set { _PropertyType = value; }
        }

        /// <summary>
        /// Gets sets, the property null indicator.
        /// </summary>
        [XmlArray(ElementName = "PropertyIsNullable", IsNullable = false)]
        public bool[] PropertyIsNullable
        {
            get { return _PropertyIsNullable; }
            set { _PropertyIsNullable = value; }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDefaultValue", IsNullable = true)]
        public string[] PropertyDefaultValue
        {
            get { return _PropertyDefaultValue; }
            set { _PropertyDefaultValue = value; }
        }

        /// <summary>
        /// Gets sets, the label text.
        /// </summary>
        [XmlArray(ElementName = "LabelText", IsNullable = false)]
        public string[] LabelText
        {
            get { return _LabelText; }
            set { _LabelText = value; }
        }

        /// <summary>
        /// Gets sets, the data type.
        /// </summary>
        [XmlArray(ElementName = "DataType", IsNullable = true)]
        public List<System.ComponentModel.DataAnnotations.DataType?> DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        /// <summary>
        /// Gets sets, the atributes.
        /// </summary>
        [XmlArray(ElementName = "Attributes", IsNullable = true)]
        public List<Attribute[]> Attributes
        {
            get { return _Attributes; }
            set { _Attributes = value; }
        }
        #endregion
    }
}
