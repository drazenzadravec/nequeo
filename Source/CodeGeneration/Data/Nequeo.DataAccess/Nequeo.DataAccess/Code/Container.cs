using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nequeo.CustomTool.CodeGenerator.Code
{
    /// <summary>
    /// Unique table name comparer.
    /// </summary>
    internal class ToUpperComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x.ToUpper() == y.ToUpper();
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Unique table name comparer.
    /// </summary>
    internal class ToLowerComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x.ToLower() == y.ToLower();
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Database data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class CompleteDataObjectContainer
    {
        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private string _ConfigKeyDatabaseConnection = null;

        private string _ContextName = null;

        private string _ExtensionContextName = null;

        private int? _ConnectionType = 0;

        private int? _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private long? _TableListMaxSize = 0;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool? _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        private string _ProcedureExtensionClassName = null;

        private bool? _ProcedureFunctionHandler = true;

        private bool? _ProcedureListExclusion = true;

        private string[] _ProcedureList = null;

        private string _FunctionScalarExtensionClassName = null;

        private bool? _FunctionScalarFunctionHandler = true;

        private bool? _FunctionScalarListExclusion = true;

        private string[] _FunctionScalarList = null;

        private string _FunctionTableExtensionClassName = null;

        private bool? _FunctionTableFunctionHandler = true;

        private bool? _FunctionTableListExclusion = true;

        private string[] _FunctionTableList = null;

        private string _DataSetDataContextName = null;

        private bool? _DataSetUseAnonymousDataEntity = false;

        private string _EdmDataContextName = null;

        private bool? _EdmUseAnonymousDataEntity = false;

        private string _LinqDataContextName = null;

        private bool? _LinqUseAnonymousDataEntity = false;

        private bool? _IncludeContextItems = true;


        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = false)]
        public string NamespaceExtendedName
        {
            get
            {
                return _NamespaceExtendedName;
            }
            set
            {
                _NamespaceExtendedName = value;
            }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "ConfigKeyDatabaseConnection", IsNullable = false)]
        public string ConfigKeyDatabaseConnection
        {
            get
            {
                return _ConfigKeyDatabaseConnection;
            }
            set
            {
                _ConfigKeyDatabaseConnection = value;
            }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "ContextName", IsNullable = false)]
        public string ContextName
        {
            get
            {
                return _ContextName;
            }
            set
            {
                _ContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "ExtensionContextName", IsNullable = false)]
        public string ExtensionContextName
        {
            get
            {
                return _ExtensionContextName;
            }
            set
            {
                _ExtensionContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the database connection.
        /// </summary>
        [XmlElement(ElementName = "DatabaseConnection", IsNullable = false)]
        public string DatabaseConnection
        {
            get
            {
                return _DatabaseConnection;
            }
            set
            {
                _DatabaseConnection = value;
            }
        }

        /// <summary>
        /// Gets sets, the database.
        /// </summary>
        [XmlElement(ElementName = "Database", IsNullable = false)]
        public string Database
        {
            get
            {
                return _Database;
            }
            set
            {
                _Database = value;
            }
        }

        /// <summary>
        /// Gets sets, the company namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceCompanyName", IsNullable = false)]
        public string NamespaceCompanyName
        {
            get
            {
                return _NamespaceCompanyName;
            }
            set
            {
                _NamespaceCompanyName = value;
            }
        }

        /// <summary>
        /// Gets sets, the connection type.
        /// </summary>
        [XmlElement(ElementName = "ConnectionType", IsNullable = true)]
        public int? ConnectionType
        {
            get
            {
                return _ConnectionType;
            }
            set
            {
                _ConnectionType = value;
            }
        }

        /// <summary>
        /// Gets sets, the connection data type.
        /// </summary>
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = true)]
        public int? ConnectionDataType
        {
            get
            {
                return _ConnectionDataType;
            }
            set
            {
                _ConnectionDataType = value;
            }
        }

        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = true)]
        public string DataAccessProvider
        {
            get
            {
                return _DataAccessProvider;
            }
            set
            {
                _DataAccessProvider = value;
            }
        }

        /// <summary>
        /// Gets sets, the table list maximun size.
        /// </summary>
        [XmlElement(ElementName = "TableListMaxSize", IsNullable = true)]
        public long? TableListMaxSize
        {
            get
            {
                return _TableListMaxSize;
            }
            set
            {
                _TableListMaxSize = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = true)]
        public bool? TableListExclusion
        {
            get
            {
                return _TableListExclusion;
            }
            set
            {
                _TableListExclusion = value;
            }
        }

        /// <summary>
        /// Gets sets, the table list.
        /// </summary>
        [XmlArray(ElementName = "TableList", IsNullable = false)]
        public string[] TableList
        {
            get
            {
                return _TableList;
            }
            set
            {
                _TableList = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = false)]
        public string DataBaseConnect
        {
            get
            {
                return _DataBaseConnect;
            }
            set
            {
                _DataBaseConnect = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = false)]
        public string DataBaseOwner
        {
            get
            {
                return _DataBaseOwner;
            }
            set
            {
                _DataBaseOwner = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "ProcedureExtensionClassName", IsNullable = false)]
        public string ProcedureExtensionClassName
        {
            get
            {
                return _ProcedureExtensionClassName;
            }
            set
            {
                _ProcedureExtensionClassName = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "ProcedureFunctionHandler", IsNullable = true)]
        public bool? ProcedureFunctionHandler
        {
            get
            {
                return _ProcedureFunctionHandler;
            }
            set
            {
                _ProcedureFunctionHandler = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "ProcedureListExclusion", IsNullable = true)]
        public bool? ProcedureListExclusion
        {
            get
            {
                return _ProcedureListExclusion;
            }
            set
            {
                _ProcedureListExclusion = value;
            }
        }

        /// <summary>
        /// Gets sets, the table list.
        /// </summary>
        [XmlArray(ElementName = "ProcedureList", IsNullable = false)]
        public string[] ProcedureList
        {
            get
            {
                return _ProcedureList;
            }
            set
            {
                _ProcedureList = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "FunctionScalarExtensionClassName", IsNullable = false)]
        public string FunctionScalarExtensionClassName
        {
            get
            {
                return _FunctionScalarExtensionClassName;
            }
            set
            {
                _FunctionScalarExtensionClassName = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionScalarFunctionHandler", IsNullable = true)]
        public bool? FunctionScalarFunctionHandler
        {
            get
            {
                return _FunctionScalarFunctionHandler;
            }
            set
            {
                _FunctionScalarFunctionHandler = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionScalarListExclusion", IsNullable = true)]
        public bool? FunctionScalarListExclusion
        {
            get
            {
                return _FunctionScalarListExclusion;
            }
            set
            {
                _FunctionScalarListExclusion = value;
            }
        }

        /// <summary>
        /// Gets sets, the table list.
        /// </summary>
        [XmlArray(ElementName = "FunctionScalarList", IsNullable = false)]
        public string[] FunctionScalarList
        {
            get
            {
                return _FunctionScalarList;
            }
            set
            {
                _FunctionScalarList = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "FunctionTableExtensionClassName", IsNullable = false)]
        public string FunctionTableExtensionClassName
        {
            get
            {
                return _FunctionTableExtensionClassName;
            }
            set
            {
                _FunctionTableExtensionClassName = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionTableFunctionHandler", IsNullable = true)]
        public bool? FunctionTableFunctionHandler
        {
            get
            {
                return _FunctionTableFunctionHandler;
            }
            set
            {
                _FunctionTableFunctionHandler = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionTableListExclusion", IsNullable = true)]
        public bool? FunctionTableListExclusion
        {
            get
            {
                return _FunctionTableListExclusion;
            }
            set
            {
                _FunctionTableListExclusion = value;
            }
        }

        /// <summary>
        /// Gets sets, the table list.
        /// </summary>
        [XmlArray(ElementName = "FunctionTableList", IsNullable = false)]
        public string[] FunctionTableList
        {
            get
            {
                return _FunctionTableList;
            }
            set
            {
                _FunctionTableList = value;
            }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "DataSetDataContextName", IsNullable = true)]
        public string DataSetDataContextName
        {
            get
            {
                return _DataSetDataContextName;
            }
            set
            {
                _DataSetDataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "DataSetUseAnonymousDataEntity", IsNullable = true)]
        public bool? DataSetUseAnonymousDataEntity
        {
            get
            {
                return _DataSetUseAnonymousDataEntity;
            }
            set
            {
                _DataSetUseAnonymousDataEntity = value;
            }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "EdmDataContextName", IsNullable = true)]
        public string EdmDataContextName
        {
            get
            {
                return _EdmDataContextName;
            }
            set
            {
                _EdmDataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "EdmUseAnonymousDataEntity", IsNullable = true)]
        public bool? EdmUseAnonymousDataEntity
        {
            get
            {
                return _EdmUseAnonymousDataEntity;
            }
            set
            {
                _EdmUseAnonymousDataEntity = value;
            }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "LinqDataContextName", IsNullable = true)]
        public string LinqDataContextName
        {
            get
            {
                return _LinqDataContextName;
            }
            set
            {
                _LinqDataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "LinqUseAnonymousDataEntity", IsNullable = true)]
        public bool? LinqUseAnonymousDataEntity
        {
            get
            {
                return _LinqUseAnonymousDataEntity;
            }
            set
            {
                _LinqUseAnonymousDataEntity = value;
            }
        }

        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "IncludeContextItems", IsNullable = true)]
        public bool? IncludeContextItems
        {
            get
            {
                return _IncludeContextItems;
            }
            set
            {
                _IncludeContextItems = value;
            }
        }
    }
}
