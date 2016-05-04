using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// Database data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class DataBaseContextContainer
    {
        private string _ContextName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private string _ConfigKeyDatabaseConnection = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        private bool _IncludeContextItems = true;


        /// <summary>
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "IncludeContextItems", IsNullable = false)]
        public bool IncludeContextItems
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

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
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
        /// Gets sets, the connection type.
        /// </summary>
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = false)]
        public bool TableListExclusion
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Database data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class DataBaseObjectContainer
    {
        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = false)]
        public bool TableListExclusion
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Data context data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class DataContextExtensionContainer
    {
        private string _ContextName = null;

        private string _DataContextName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
        /// </summary>
        [XmlElement(ElementName = "DataContextName", IsNullable = false)]
        public string DataContextName
        {
            get
            {
                return _DataContextName;
            }
            set
            {
                _DataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the context name.
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
        /// 
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = false)]
        public bool TableListExclusion
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Data object.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class DataObjectContainer
    {
        private string _ClassName = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private string[] _PropertyName = null;

        private string[] _PropertyType = null;

        private bool[] _PropertyIsNullable = null;

        private string[] _PropertyDefaultValue = null;

        private string _NamespaceExtendedName = null;

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the class name.
        /// </summary>
        [XmlElement(ElementName = "ClassName", IsNullable = false)]
        public string ClassName
        {
            get
            {
                return _ClassName;
            }
            set
            {
                _ClassName = value;
            }
        }

        /// <summary>
        /// Gets sets, the database name.
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
        /// Gets sets, the name of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyName", IsNullable = false)]
        public string[] PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                _PropertyName = value;
            }
        }

        /// <summary>
        /// Gets sets, the type of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyType", IsNullable = false)]
        public string[] PropertyType
        {
            get
            {
                return _PropertyType;
            }
            set
            {
                _PropertyType = value;
            }
        }

        /// <summary>
        /// Gets sets, the property null indicator.
        /// </summary>
        [XmlArray(ElementName = "PropertyIsNullable", IsNullable = false)]
        public bool[] PropertyIsNullable
        {
            get
            {
                return _PropertyIsNullable;
            }
            set
            {
                _PropertyIsNullable = value;
            }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDefaultValue", IsNullable = true)]
        public string[] PropertyDefaultValue
        {
            get
            {
                return _PropertyDefaultValue;
            }
            set
            {
                _PropertyDefaultValue = value;
            }
        }
    }

    /// <summary>
    /// Enumeration data object.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class EnumObjectContainer
    {
        private string _EnumName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _TableName = null;

        private string _NamespaceCompanyName = null;

        private string _ValueColumnName = null;

        private string _IndicatorColumnName = null;

        private string _DataFilter = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private string _NamespaceExtendedName = null;

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the enumeration name.
        /// </summary>
        [XmlElement(ElementName = "EnumName", IsNullable = false)]
        public string EnumName
        {
            get
            {
                return _EnumName;
            }
            set
            {
                _EnumName = value;
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
        /// Gets sets, the database name.
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
        /// Gets sets, the name of the table.
        /// </summary>
        [XmlElement(ElementName = "TableName", IsNullable = false)]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
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
        /// Gets sets, the column name of the value.
        /// </summary>
        [XmlElement(ElementName = "ValueColumnName", IsNullable = false)]
        public string ValueColumnName
        {
            get
            {
                return _ValueColumnName;
            }
            set
            {
                _ValueColumnName = value;
            }
        }

        /// <summary>
        /// Gets sets, the column name of the indicator.
        /// </summary>
        [XmlElement(ElementName = "IndicatorColumnName", IsNullable = false)]
        public string IndicatorColumnName
        {
            get
            {
                return _IndicatorColumnName;
            }
            set
            {
                _IndicatorColumnName = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "DataFilter", IsNullable = true)]
        public string DataFilter
        {
            get
            {
                return _DataFilter;
            }
            set
            {
                _DataFilter = value;
            }
        }

        /// <summary>
        /// Gets sets, the connection type.
        /// </summary>
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// ROWSET data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("ROWSET", IsNullable = true)]
    public class XmlObjectContainer
    {
        private ROW _RowEntity;

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "ROW", IsNullable = true)]
        public ROW RowEntity
        {
            get
            {
                return this._RowEntity;
            }
            set
            {
                if ((this._RowEntity != value))
                {
                    this._RowEntity = value;
                }
            }
        }
    }

    /// <summary>
    /// The RowEntity data object class.
    /// </summary>
    [Serializable()]
    public partial class ROW
    {
        private PACKAGE_T _PackageEntity;

        private FUNCTION_T _FunctionEntity;

        private PROCEDURE_T _ProcedureEntity;

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "PACKAGE_T", IsNullable = true)]
        public PACKAGE_T PackageEntity
        {
            get
            {
                return this._PackageEntity;
            }
            set
            {
                if ((this._PackageEntity != value))
                {
                    this._PackageEntity = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "FUNCTION_T", IsNullable = true)]
        public FUNCTION_T FunctionEntity
        {
            get
            {
                return this._FunctionEntity;
            }
            set
            {
                if ((this._FunctionEntity != value))
                {
                    this._FunctionEntity = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "PROCEDURE_T", IsNullable = true)]
        public PROCEDURE_T ProcedureEntity
        {
            get
            {
                return this._ProcedureEntity;
            }
            set
            {
                if ((this._ProcedureEntity != value))
                {
                    this._ProcedureEntity = value;
                }
            }
        }
    }

    /// <summary>
    /// The RowEntity data object class.
    /// </summary>
    [Serializable()]
    public partial class PACKAGE_T
    {
        private SOURCE_LINES _SourceLines;

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "SOURCE_LINES", IsNullable = true)]
        public SOURCE_LINES SourceLines
        {
            get
            {
                return this._SourceLines;
            }
            set
            {
                if ((this._SourceLines != value))
                {
                    this._SourceLines = value;
                }
            }
        }
    }

    /// <summary>
    /// The RowEntity data object class.
    /// </summary>
    [Serializable()]
    public partial class FUNCTION_T
    {
        private SOURCE_LINES _SourceLines;

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "SOURCE_LINES", IsNullable = true)]
        public SOURCE_LINES SourceLines
        {
            get
            {
                return this._SourceLines;
            }
            set
            {
                if ((this._SourceLines != value))
                {
                    this._SourceLines = value;
                }
            }
        }
    }

    /// <summary>
    /// The RowEntity data object class.
    /// </summary>
    [Serializable()]
    public partial class PROCEDURE_T
    {
        private SOURCE_LINES _SourceLines;

        /// <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "SOURCE_LINES", IsNullable = true)]
        public SOURCE_LINES SourceLines
        {
            get
            {
                return this._SourceLines;
            }
            set
            {
                if ((this._SourceLines != value))
                {
                    this._SourceLines = value;
                }
            }
        }
    }

    /// <summary>
    /// The RowEntity data object class.
    /// </summary>
    [Serializable()]
    public partial class SOURCE_LINES
    {
        private SOURCE_LINES_ITEM[] _SourceLinesItem;

        // <summary>
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlElement(ElementName = "SOURCE_LINES_ITEM", IsNullable = true)]
        public SOURCE_LINES_ITEM[] SourceLinesItem
        {
            get
            {
                return this._SourceLinesItem;
            }
            set
            {
                if ((this._SourceLinesItem != value))
                {
                    this._SourceLinesItem = value;
                }
            }
        }
    }

    /// <summary>
    /// The RowEntity data object class.
    /// </summary>
    [Serializable()]
    public partial class SOURCE_LINES_ITEM
    {
        private string _Source;

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "SOURCE", IsNullable = false)]
        public string Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                if ((this._Source != value))
                {
                    this._Source = value;
                }
            }
        }
    }

    /// <summary>
    /// Data context data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class FunctionExtensionContainer
    {
        private string _ExtensionClassName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _FunctionHandler = true;

        private bool _FunctionListExclusion = true;

        private string[] _FunctionList = null;

        private string _NamespaceExtendedName = null;

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
        /// </summary>
        [XmlElement(ElementName = "ExtensionClassName", IsNullable = false)]
        public string ExtensionClassName
        {
            get
            {
                return _ExtensionClassName;
            }
            set
            {
                _ExtensionClassName = value;
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
        /// 
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        /// Gets sets, the Function List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionListExclusion", IsNullable = false)]
        public bool FunctionListExclusion
        {
            get
            {
                return _FunctionListExclusion;
            }
            set
            {
                _FunctionListExclusion = value;
            }
        }

        /// <summary>
        /// Gets sets, the Function List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionHandler", IsNullable = false)]
        public bool FunctionHandler
        {
            get
            {
                return _FunctionHandler;
            }
            set
            {
                _FunctionHandler = value;
            }
        }

        /// <summary>
        /// Gets sets, the function list.
        /// </summary>
        [XmlArray(ElementName = "FunctionList", IsNullable = false)]
        public string[] FunctionList
        {
            get
            {
                return _FunctionList;
            }
            set
            {
                _FunctionList = value;
            }
        }

        /// <summary>
        /// Gets sets, the enumeration data filter.
        /// </summary>
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Linq object data object.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class LinqObjectContainer
    {
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

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the name of the class.
        /// </summary>
        [XmlElement(ElementName = "ClassName", IsNullable = false)]
        public string ClassName
        {
            get
            {
                return _ClassName;
            }
            set
            {
                _ClassName = value;
            }
        }

        /// <summary>
        /// Gets sets, the database name.
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
        /// Gets sets, the property name.
        /// </summary>
        [XmlArray(ElementName = "PropertyName", IsNullable = false)]
        public string[] PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                _PropertyName = value;
            }
        }

        /// <summary>
        /// Gets sets, the property type.
        /// </summary>
        [XmlArray(ElementName = "PropertyType", IsNullable = false)]
        public string[] PropertyType
        {
            get
            {
                return _PropertyType;
            }
            set
            {
                _PropertyType = value;
            }
        }

        /// <summary>
        /// Gets sets the property database type.
        /// </summary>
        [XmlArray(ElementName = "PropertyDatabaseType", IsNullable = false)]
        public string[] PropertyDatabaseType
        {
            get
            {
                return _PropertyDatabaseType;
            }
            set
            {
                _PropertyDatabaseType = value;
            }
        }

        /// <summary>
        /// Gets sets, the property null indicator.
        /// </summary>
        [XmlArray(ElementName = "PropertyIsNullable", IsNullable = false)]
        public bool[] PropertyIsNullable
        {
            get
            {
                return _PropertyIsNullable;
            }
            set
            {
                _PropertyIsNullable = value;
            }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDefaultValue", IsNullable = false)]
        public string[] PropertyDefaultValue
        {
            get
            {
                return _PropertyDefaultValue;
            }
            set
            {
                _PropertyDefaultValue = value;
            }
        }

        /// <summary>
        /// Gets sets, the property default value.
        /// </summary>
        [XmlArray(ElementName = "PropertyDatabaseColumnName", IsNullable = false)]
        public string[] PropertyDatabaseColumnName
        {
            get
            {
                return _PropertyDatabaseColumnName;
            }
            set
            {
                _PropertyDatabaseColumnName = value;
            }
        }
    }

    /// <summary>
    /// Model data context data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class ModelDataContextObjectContainer
    {
        private string _ContextName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        private string _DataContextName = null;

        /// <summary>
        /// Gets sets, the context name.
        /// </summary>
        [XmlElement(ElementName = "DataContextName", IsNullable = false)]
        public string DataContextName
        {
            get
            {
                return _DataContextName;
            }
            set
            {
                _DataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
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
        /// 
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = false)]
        public bool TableListExclusion
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Data context data object
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class ProcedureExtensionContainer
    {
        private string _ExtensionClassName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _FunctionHandler = true;

        private bool _ProcedureListExclusion = true;

        private string[] _ProcedureList = null;

        private string _NamespaceExtendedName = null;

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
        /// </summary>
        [XmlElement(ElementName = "ExtensionClassName", IsNullable = false)]
        public string ExtensionClassName
        {
            get
            {
                return _ExtensionClassName;
            }
            set
            {
                _ExtensionClassName = value;
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
        /// 
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Procedure List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "ProcedureListExclusion", IsNullable = false)]
        public bool ProcedureListExclusion
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
        /// Gets sets, the Function List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "FunctionHandler", IsNullable = false)]
        public bool FunctionHandler
        {
            get
            {
                return _FunctionHandler;
            }
            set
            {
                _FunctionHandler = value;
            }
        }

        /// <summary>
        /// Gets sets, the procedure list.
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Replica data context data object.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class ReplicaDataContextObjectContainer
    {
        private string _ContextName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        private string _DataContextName = null;

        /// <summary>
        /// Gets sets, the context name.
        /// </summary>
        [XmlElement(ElementName = "DataContextName", IsNullable = false)]
        public string DataContextName
        {
            get
            {
                return _DataContextName;
            }
            set
            {
                _DataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
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
        /// Gets sets, the database name.
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = false)]
        public bool TableListExclusion
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }

    /// <summary>
    /// Schema data context data object.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class SchemaDataContextObjectContainer
    {
        private string _ContextName = null;

        private string _DatabaseConnection = null;

        private string _Database = null;

        private string _NamespaceCompanyName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

        private string _DataBaseConnect = null;

        private string _DataBaseOwner = null;

        private bool _TableListExclusion = true;

        private string[] _TableList = null;

        private string _NamespaceExtendedName = null;

        private string _DataContextName = null;

        /// <summary>
        /// Gets sets, the context name.
        /// </summary>
        [XmlElement(ElementName = "DataContextName", IsNullable = false)]
        public string DataContextName
        {
            get
            {
                return _DataContextName;
            }
            set
            {
                _DataContextName = value;
            }
        }

        /// <summary>
        /// Gets sets, the extended namespace.
        /// </summary>
        [XmlElement(ElementName = "NamespaceExtendedName", IsNullable = true)]
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
        /// Gets sets, the context name.
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
        /// Gets sets, the database name.
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
        [XmlElement(ElementName = "ConnectionType", IsNullable = false)]
        public int ConnectionType
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
        [XmlElement(ElementName = "ConnectionDataType", IsNullable = false)]
        public int ConnectionDataType
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
        [XmlElement(ElementName = "DataAccessProvider", IsNullable = false)]
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
        /// Gets sets, the Table List Exclusion.
        /// </summary>
        [XmlElement(ElementName = "TableListExclusion", IsNullable = false)]
        public bool TableListExclusion
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
        [XmlElement(ElementName = "DataBaseConnect", IsNullable = true)]
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
        [XmlElement(ElementName = "DataBaseOwner", IsNullable = true)]
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
    }
}
