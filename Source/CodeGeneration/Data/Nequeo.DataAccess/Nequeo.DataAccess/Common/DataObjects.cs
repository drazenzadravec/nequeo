using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// The tablecolumnsresult linq object class.
    /// </summary>
    [DataContract(Name = "TableColumnsResult")]
    [Serializable()]
    public partial class TableColumnsResult
    {

        private string _ColumnName;

        private string _ColumnType;

        private bool _ColumnNullable;

        private bool _IsComputed;

        private System.Nullable<System.Int32> _ColumnOrder;

        private long _Length;

        private System.Nullable<System.Int64> _Precision;

        private bool _PrimaryKeySeed;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableColumnsResult()
        {
        }

        /// <summary>
        /// Gets sets, the columnname property for the object.
        /// </summary>
        [DataMember(Name = "ColumnName", IsRequired = true)]
        [XmlElement(ElementName = "ColumnName", IsNullable = false)]
        [Column(Storage = "_ColumnName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnName
        {
            get
            {
                return this._ColumnName;
            }
            set
            {
                if ((this._ColumnName != value))
                {
                    this._ColumnName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columntype property for the object.
        /// </summary>
        [DataMember(Name = "ColumnType", IsRequired = true)]
        [XmlElement(ElementName = "ColumnType", IsNullable = false)]
        [Column(Storage = "_ColumnType", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnType
        {
            get
            {
                return this._ColumnType;
            }
            set
            {
                if ((this._ColumnType != value))
                {
                    this._ColumnType = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnnullable property for the object.
        /// </summary>
        [DataMember(Name = "ColumnNullable", IsRequired = true)]
        [XmlElement(ElementName = "ColumnNullable", IsNullable = false)]
        [Column(Storage = "_ColumnNullable", DbType = "Bit", CanBeNull = false)]
        public bool ColumnNullable
        {
            get
            {
                return this._ColumnNullable;
            }
            set
            {
                if ((this._ColumnNullable != value))
                {
                    this._ColumnNullable = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the iscomputed property for the object.
        /// </summary>
        [DataMember(Name = "IsComputed", IsRequired = true)]
        [XmlElement(ElementName = "IsComputed", IsNullable = false)]
        [Column(Storage = "_IsComputed", DbType = "Bit", CanBeNull = false)]
        public bool IsComputed
        {
            get
            {
                return this._IsComputed;
            }
            set
            {
                if ((this._IsComputed != value))
                {
                    this._IsComputed = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnorder property for the object.
        /// </summary>
        [DataMember(Name = "ColumnOrder", IsRequired = false)]
        [XmlElement(ElementName = "ColumnOrder", IsNullable = true)]
        [Column(Storage = "_ColumnOrder", DbType = "Int", CanBeNull = true)]
        public System.Nullable<System.Int32> ColumnOrder
        {
            get
            {
                return this._ColumnOrder;
            }
            set
            {
                if ((this._ColumnOrder != value))
                {
                    this._ColumnOrder = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the length property for the object.
        /// </summary>
        [DataMember(Name = "Length", IsRequired = true)]
        [XmlElement(ElementName = "Length", IsNullable = false)]
        [Column(Storage = "_Length", DbType = "BigInt", CanBeNull = false)]
        public long Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                if ((this._Length != value))
                {
                    this._Length = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the Precision property for the object.
        /// </summary>
        [DataMember(Name = "Precision", IsRequired = false)]
        [XmlElement(ElementName = "Precision", IsNullable = true)]
        [Column(Storage = "Precision", DbType = "BigInt", CanBeNull = true)]
        public System.Nullable<System.Int64> Precision
        {
            get
            {
                return this._Precision;
            }
            set
            {
                if ((this._Precision != value))
                {
                    this._Precision = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the primarykeyseed property for the object.
        /// </summary>
        [DataMember(Name = "PrimaryKeySeed", IsRequired = false)]
        [XmlElement(ElementName = "PrimaryKeySeed", IsNullable = true)]
        [Column(Storage = "_PrimaryKeySeed", DbType = "Bit", CanBeNull = true)]
        public bool PrimaryKeySeed
        {
            get
            {
                return this._PrimaryKeySeed;
            }
            set
            {
                if ((this._PrimaryKeySeed != value))
                {
                    this._PrimaryKeySeed = value;
                }
            }
        }
    }

    /// <summary>
    /// The primarykeycolumnsresult linq object class.
    /// </summary>
    [DataContract(Name = "PrimaryKeyColumnsResult")]
    [Serializable()]
    public partial class PrimaryKeyColumnsResult
    {

        private string _PrimaryKeyName;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PrimaryKeyColumnsResult()
        {
        }

        /// <summary>
        /// Gets sets, the primarykeyname property for the object.
        /// </summary>
        [DataMember(Name = "PrimaryKeyName", IsRequired = true)]
        [XmlElement(ElementName = "PrimaryKeyName", IsNullable = false)]
        [Column(Storage = "_PrimaryKeyName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string PrimaryKeyName
        {
            get
            {
                return this._PrimaryKeyName;
            }
            set
            {
                if ((this._PrimaryKeyName != value))
                {
                    this._PrimaryKeyName = value;
                }
            }
        }
    }

    /// <summary>
    /// The foreignkeytableresult linq object class.
    /// </summary>
    [DataContract(Name = "ForeignKeyTableResult")]
    [Serializable()]
    public partial class ForeignKeyTableResult
    {

        private string _TableName;

        private string _ColumnName;

        private string _ColumnType;

        private bool _IsNullable;

        private long _Length;

        private System.Nullable<System.Int64> _Precision;

        private string _ForeignKeyTable;

        private string _ForeignKeyColumnName;

        private string _ForeignKeyOwner;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForeignKeyTableResult()
        {
        }

        /// <summary>
        /// Gets sets, the tablename property for the object.
        /// </summary>
        [DataMember(Name = "TableName", IsRequired = true)]
        [XmlElement(ElementName = "TableName", IsNullable = false)]
        [Column(Storage = "_TableName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string TableName
        {
            get
            {
                return this._TableName;
            }
            set
            {
                if ((this._TableName != value))
                {
                    this._TableName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnname property for the object.
        /// </summary>
        [DataMember(Name = "ColumnName", IsRequired = true)]
        [XmlElement(ElementName = "ColumnName", IsNullable = false)]
        [Column(Storage = "_ColumnName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnName
        {
            get
            {
                return this._ColumnName;
            }
            set
            {
                if ((this._ColumnName != value))
                {
                    this._ColumnName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columntype property for the object.
        /// </summary>
        [DataMember(Name = "ColumnType", IsRequired = true)]
        [XmlElement(ElementName = "ColumnType", IsNullable = false)]
        [Column(Storage = "_ColumnType", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnType
        {
            get
            {
                return this._ColumnType;
            }
            set
            {
                if ((this._ColumnType != value))
                {
                    this._ColumnType = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isnullable property for the object.
        /// </summary>
        [DataMember(Name = "IsNullable", IsRequired = true)]
        [XmlElement(ElementName = "IsNullable", IsNullable = false)]
        [Column(Storage = "_IsNullable", DbType = "Bit", CanBeNull = false)]
        public bool IsNullable
        {
            get
            {
                return this._IsNullable;
            }
            set
            {
                if ((this._IsNullable != value))
                {
                    this._IsNullable = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the length property for the object.
        /// </summary>
        [DataMember(Name = "Length", IsRequired = true)]
        [XmlElement(ElementName = "Length", IsNullable = false)]
        [Column(Storage = "_Length", DbType = "BigInt", CanBeNull = false)]
        public long Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                if ((this._Length != value))
                {
                    this._Length = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the Precision property for the object.
        /// </summary>
        [DataMember(Name = "Precision", IsRequired = false)]
        [XmlElement(ElementName = "Precision", IsNullable = true)]
        [Column(Storage = "Precision", DbType = "BigInt", CanBeNull = true)]
        public System.Nullable<System.Int64> Precision
        {
            get
            {
                return this._Precision;
            }
            set
            {
                if ((this._Precision != value))
                {
                    this._Precision = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the foreignkeytable property for the object.
        /// </summary>
        [DataMember(Name = "ForeignKeyTable", IsRequired = true)]
        [XmlElement(ElementName = "ForeignKeyTable", IsNullable = false)]
        [Column(Storage = "_ForeignKeyTable", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ForeignKeyTable
        {
            get
            {
                return this._ForeignKeyTable;
            }
            set
            {
                if ((this._ForeignKeyTable != value))
                {
                    this._ForeignKeyTable = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the foreignkeytable property for the object.
        /// </summary>
        [DataMember(Name = "ForeignKeyColumnName", IsRequired = true)]
        [XmlElement(ElementName = "ForeignKeyColumnName", IsNullable = false)]
        [Column(Storage = "_ForeignKeyColumnName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ForeignKeyColumnName
        {
            get
            {
                return this._ForeignKeyColumnName;
            }
            set
            {
                if ((this._ForeignKeyColumnName != value))
                {
                    this._ForeignKeyColumnName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the foreignkeytable property for the object.
        /// </summary>
        [DataMember(Name = "ForeignKeyOwner", IsRequired = true)]
        [XmlElement(ElementName = "ForeignKeyOwner", IsNullable = false)]
        [Column(Storage = "_ForeignKeyOwner", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ForeignKeyOwner
        {
            get
            {
                return this._ForeignKeyOwner;
            }
            set
            {
                if ((this._ForeignKeyOwner != value))
                {
                    this._ForeignKeyOwner = value;
                }
            }
        }
    }

    /// <summary>
    /// The tablesresult linq object class.
    /// </summary>
    [DataContract(Name = "TablesResult")]
    [Serializable()]
    public partial class TablesResult
    {

        private string _TableName;

        private string _TableOwner;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TablesResult()
        {
        }

        /// <summary>
        /// Gets sets, the tablename property for the object.
        /// </summary>
        [DataMember(Name = "TableName", IsRequired = false)]
        [XmlElement(ElementName = "TableName", IsNullable = true)]
        [Column(Storage = "_TableName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string TableName
        {
            get
            {
                return this._TableName;
            }
            set
            {
                if ((this._TableName != value))
                {
                    this._TableName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the tableowner property for the object.
        /// </summary>
        [DataMember(Name = "TableOwner", IsRequired = false)]
        [XmlElement(ElementName = "TableOwner", IsNullable = true)]
        [Column(Storage = "_TableOwner", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string TableOwner
        {
            get
            {
                return this._TableOwner;
            }
            set
            {
                if ((this._TableOwner != value))
                {
                    this._TableOwner = value;
                }
            }
        }
    }

    /// <summary>
    /// The proceduresresult linq object class.
    /// </summary>
    [DataContract(Name = "ProceduresResult")]
    [Serializable()]
    public partial class ProceduresResult
    {

        private string _ProcedureName;

        private string _ProcedureOwner;

        private string _PackageName;

        private string _FunctionRealName;

        #region Extensibility Method Definitions
        /// <summary>
        /// On create data entity.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// On load data entity.
        /// </summary>
        partial void OnLoaded();

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProceduresResult()
        {
            OnCreated();
        }

        /// <summary>
        /// Gets sets, the procedurename property for the object.
        /// </summary>
        [DataMember(Name = "ProcedureName", IsRequired = false)]
        [XmlElement(ElementName = "ProcedureName", IsNullable = true)]
        [Column(Storage = "_ProcedureName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string ProcedureName
        {
            get
            {
                return this._ProcedureName;
            }
            set
            {
                if ((this._ProcedureName != value))
                {
                    this._ProcedureName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the procedureowner property for the object.
        /// </summary>
        [DataMember(Name = "ProcedureOwner", IsRequired = false)]
        [XmlElement(ElementName = "ProcedureOwner", IsNullable = true)]
        [Column(Storage = "_ProcedureOwner", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string ProcedureOwner
        {
            get
            {
                return this._ProcedureOwner;
            }
            set
            {
                if ((this._ProcedureOwner != value))
                {
                    this._ProcedureOwner = value;
                }
            }
        }

        // <summary>
        /// Gets sets, the FunctionOwner property for the object.
        /// </summary>
        [DataMember(Name = "PackageName", IsRequired = false)]
        [XmlElement(ElementName = "PackageName", IsNullable = true)]
        [Column(Storage = "PackageName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string PackageName
        {
            get
            {
                return this._PackageName;
            }
            set
            {
                if ((this._PackageName != value))
                {
                    this._PackageName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the FunctionOwner property for the object.
        /// </summary>
        [DataMember(Name = "FunctionRealName", IsRequired = false)]
        [XmlElement(ElementName = "FunctionRealName", IsNullable = true)]
        [Column(Storage = "FunctionRealName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string FunctionRealName
        {
            get
            {
                return this._FunctionRealName;
            }
            set
            {
                if ((this._FunctionRealName != value))
                {
                    this._FunctionRealName = value;
                }
            }
        }
    }

    /// <summary>
    /// The packagesresult linq object class.
    /// </summary>
    [DataContract(Name = "PackagesResult")]
    [Serializable()]
    public partial class PackagesResult
    {

        private string _PackageName;

        private string _PackageOwner;

        #region Extensibility Method Definitions
        /// <summary>
        /// On create data entity.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// On load data entity.
        /// </summary>
        partial void OnLoaded();

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PackagesResult()
        {
            OnCreated();
        }

        /// <summary>
        /// Gets sets, the procedurename property for the object.
        /// </summary>
        [DataMember(Name = "PackageName", IsRequired = false)]
        [XmlElement(ElementName = "PackageName", IsNullable = true)]
        [Column(Storage = "_PackageName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string PackageName
        {
            get
            {
                return this._PackageName;
            }
            set
            {
                if ((this._PackageName != value))
                {
                    this._PackageName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the procedureowner property for the object.
        /// </summary>
        [DataMember(Name = "PackageOwner", IsRequired = false)]
        [XmlElement(ElementName = "PackageOwner", IsNullable = true)]
        [Column(Storage = "_PackageOwner", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string PackageOwner
        {
            get
            {
                return this._PackageOwner;
            }
            set
            {
                if ((this._PackageOwner != value))
                {
                    this._PackageOwner = value;
                }
            }
        }
    }

    /// <summary>
    /// The FunctionResult linq object class.
    /// </summary>
    [DataContract(Name = "FunctionResult")]
    [Serializable()]
    public partial class FunctionResult
    {

        private string _FunctionName;

        private string _FunctionOwner;

        private string _PackageName;

        private string _FunctionRealName;

        private string _OverloadName;

        #region Extensibility Method Definitions
        /// <summary>
        /// On create data entity.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// On load data entity.
        /// </summary>
        partial void OnLoaded();

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FunctionResult()
        {
            OnCreated();
        }

        /// <summary>
        /// Gets sets, the FunctionName property for the object.
        /// </summary>
        [DataMember(Name = "FunctionName", IsRequired = false)]
        [XmlElement(ElementName = "FunctionName", IsNullable = true)]
        [Column(Storage = "FunctionName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string FunctionName
        {
            get
            {
                return this._FunctionName;
            }
            set
            {
                if ((this._FunctionName != value))
                {
                    this._FunctionName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the FunctionOwner property for the object.
        /// </summary>
        [DataMember(Name = "FunctionOwner", IsRequired = false)]
        [XmlElement(ElementName = "FunctionOwner", IsNullable = true)]
        [Column(Storage = "FunctionOwner", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string FunctionOwner
        {
            get
            {
                return this._FunctionOwner;
            }
            set
            {
                if ((this._FunctionOwner != value))
                {
                    this._FunctionOwner = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the FunctionOwner property for the object.
        /// </summary>
        [DataMember(Name = "PackageName", IsRequired = false)]
        [XmlElement(ElementName = "PackageName", IsNullable = true)]
        [Column(Storage = "PackageName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string PackageName
        {
            get
            {
                return this._PackageName;
            }
            set
            {
                if ((this._PackageName != value))
                {
                    this._PackageName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the FunctionOwner property for the object.
        /// </summary>
        [DataMember(Name = "FunctionRealName", IsRequired = false)]
        [XmlElement(ElementName = "FunctionRealName", IsNullable = true)]
        [Column(Storage = "FunctionRealName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string FunctionRealName
        {
            get
            {
                return this._FunctionRealName;
            }
            set
            {
                if ((this._FunctionRealName != value))
                {
                    this._FunctionRealName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the overloadname property for the object.
        /// </summary>
        [DataMember(Name = "OverloadName", IsRequired = false)]
        [XmlElement(ElementName = "OverloadName", IsNullable = true)]
        [Column(Storage = "_OverloadName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string OverloadName
        {
            get
            {
                return this._OverloadName;
            }
            set
            {
                if ((this._OverloadName != value))
                {
                    this._OverloadName = value;
                }
            }
        }
    }

    /// <summary>
    /// The columnvaluesresult linq object class.
    /// </summary>
    [DataContract(Name = "ColumnValuesResult")]
    [Serializable()]
    public partial class ColumnValuesResult
    {

        private string _ColumnValue;

        private string _ColumnIndicator;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnValuesResult()
        {
        }

        /// <summary>
        /// Gets sets, the columnvalue property for the object.
        /// </summary>
        [DataMember(Name = "ColumnValue", IsRequired = false)]
        [XmlElement(ElementName = "ColumnValue", IsNullable = true)]
        [Column(Storage = "_ColumnValue", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string ColumnValue
        {
            get
            {
                return this._ColumnValue;
            }
            set
            {
                if ((this._ColumnValue != value))
                {
                    this._ColumnValue = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnindicator property for the object.
        /// </summary>
        [DataMember(Name = "ColumnIndicator", IsRequired = false)]
        [XmlElement(ElementName = "ColumnIndicator", IsNullable = true)]
        [Column(Storage = "_ColumnIndicator", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string ColumnIndicator
        {
            get
            {
                return this._ColumnIndicator;
            }
            set
            {
                if ((this._ColumnIndicator != value))
                {
                    this._ColumnIndicator = value;
                }
            }
        }
    }

    /// <summary>
    /// The procedurecolumnsresult linq object class.
    /// </summary>
    [DataContract(Name = "ProcedureColumnsResult")]
    [Serializable()]
    public partial class ProcedureColumnsResult
    {

        private string _ColumnName;

        private string _ColumnType;

        private bool _ColumnNullable;

        private int _ColumnOrder;

        private long _Length;

        private System.Nullable<System.Int64> _Precision;

        private bool _IsOutParameter;

        #region Extensibility Method Definitions
        /// <summary>
        /// On create data entity.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// On load data entity.
        /// </summary>
        partial void OnLoaded();

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProcedureColumnsResult()
        {
            OnCreated();
        }

        /// <summary>
        /// Gets sets, the columnname property for the object.
        /// </summary>
        [DataMember(Name = "ColumnName", IsRequired = true)]
        [XmlElement(ElementName = "ColumnName", IsNullable = false)]
        [Column(Storage = "_ColumnName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnName
        {
            get
            {
                return this._ColumnName;
            }
            set
            {
                if ((this._ColumnName != value))
                {
                    this._ColumnName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columntype property for the object.
        /// </summary>
        [DataMember(Name = "ColumnType", IsRequired = true)]
        [XmlElement(ElementName = "ColumnType", IsNullable = false)]
        [Column(Storage = "_ColumnType", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnType
        {
            get
            {
                return this._ColumnType;
            }
            set
            {
                if ((this._ColumnType != value))
                {
                    this._ColumnType = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnnullable property for the object.
        /// </summary>
        [DataMember(Name = "ColumnNullable", IsRequired = true)]
        [XmlElement(ElementName = "ColumnNullable", IsNullable = false)]
        [Column(Storage = "_ColumnNullable", DbType = "Bit", CanBeNull = false)]
        public bool ColumnNullable
        {
            get
            {
                return this._ColumnNullable;
            }
            set
            {
                if ((this._ColumnNullable != value))
                {
                    this._ColumnNullable = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnorder property for the object.
        /// </summary>
        [DataMember(Name = "ColumnOrder", IsRequired = true)]
        [XmlElement(ElementName = "ColumnOrder", IsNullable = false)]
        [Column(Storage = "_ColumnOrder", DbType = "Int", CanBeNull = false)]
        public int ColumnOrder
        {
            get
            {
                return this._ColumnOrder;
            }
            set
            {
                if ((this._ColumnOrder != value))
                {
                    this._ColumnOrder = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the length property for the object.
        /// </summary>
        [DataMember(Name = "Length", IsRequired = true)]
        [XmlElement(ElementName = "Length", IsNullable = false)]
        [Column(Storage = "_Length", DbType = "BigInt", CanBeNull = false)]
        public long Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                if ((this._Length != value))
                {
                    this._Length = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the Precision property for the object.
        /// </summary>
        [DataMember(Name = "Precision", IsRequired = true)]
        [XmlElement(ElementName = "Precision", IsNullable = false)]
        [Column(Storage = "_Precision", DbType = "BigInt", CanBeNull = false)]
        public System.Nullable<System.Int64> Precision
        {
            get
            {
                return this._Precision;
            }
            set
            {
                if ((this._Precision != value))
                {
                    this._Precision = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isoutparameter property for the object.
        /// </summary>
        [DataMember(Name = "IsOutParameter", IsRequired = true)]
        [XmlElement(ElementName = "IsOutParameter", IsNullable = false)]
        [Column(Storage = "_IsOutParameter", DbType = "Bit", CanBeNull = false)]
        public bool IsOutParameter
        {
            get
            {
                return this._IsOutParameter;
            }
            set
            {
                if ((this._IsOutParameter != value))
                {
                    this._IsOutParameter = value;
                }
            }
        }
    }

    /// <summary>
    /// The FunctionColumnsResult linq object class.
    /// </summary>
    [DataContract(Name = "FunctionColumnsResult")]
    [Serializable()]
    public partial class FunctionColumnsResult
    {

        private string _ColumnName;

        private string _ColumnType;

        private bool _ColumnNullable;

        private int _ColumnOrder;

        private long _Length;

        private System.Nullable<System.Int64> _Precision;

        private bool _IsOutParameter;

        private string _OverloadName;

        #region Extensibility Method Definitions
        /// <summary>
        /// On create data entity.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// On load data entity.
        /// </summary>
        partial void OnLoaded();

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FunctionColumnsResult()
        {
            OnCreated();
        }

        /// <summary>
        /// Gets sets, the columnname property for the object.
        /// </summary>
        [DataMember(Name = "ColumnName", IsRequired = true)]
        [XmlElement(ElementName = "ColumnName", IsNullable = false)]
        [Column(Storage = "_ColumnName", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnName
        {
            get
            {
                return this._ColumnName;
            }
            set
            {
                if ((this._ColumnName != value))
                {
                    this._ColumnName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columntype property for the object.
        /// </summary>
        [DataMember(Name = "ColumnType", IsRequired = true)]
        [XmlElement(ElementName = "ColumnType", IsNullable = false)]
        [Column(Storage = "_ColumnType", DbType = "VarChar(MAX)", CanBeNull = false)]
        public string ColumnType
        {
            get
            {
                return this._ColumnType;
            }
            set
            {
                if ((this._ColumnType != value))
                {
                    this._ColumnType = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnnullable property for the object.
        /// </summary>
        [DataMember(Name = "ColumnNullable", IsRequired = true)]
        [XmlElement(ElementName = "ColumnNullable", IsNullable = false)]
        [Column(Storage = "_ColumnNullable", DbType = "Bit", CanBeNull = false)]
        public bool ColumnNullable
        {
            get
            {
                return this._ColumnNullable;
            }
            set
            {
                if ((this._ColumnNullable != value))
                {
                    this._ColumnNullable = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnorder property for the object.
        /// </summary>
        [DataMember(Name = "ColumnOrder", IsRequired = true)]
        [XmlElement(ElementName = "ColumnOrder", IsNullable = false)]
        [Column(Storage = "_ColumnOrder", DbType = "Int", CanBeNull = false)]
        public int ColumnOrder
        {
            get
            {
                return this._ColumnOrder;
            }
            set
            {
                if ((this._ColumnOrder != value))
                {
                    this._ColumnOrder = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the length property for the object.
        /// </summary>
        [DataMember(Name = "Length", IsRequired = true)]
        [XmlElement(ElementName = "Length", IsNullable = false)]
        [Column(Storage = "_Length", DbType = "BigInt", CanBeNull = false)]
        public long Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                if ((this._Length != value))
                {
                    this._Length = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the Precision property for the object.
        /// </summary>
        [DataMember(Name = "Precision", IsRequired = true)]
        [XmlElement(ElementName = "Precision", IsNullable = false)]
        [Column(Storage = "_Precision", DbType = "BigInt", CanBeNull = false)]
        public System.Nullable<System.Int64> Precision
        {
            get
            {
                return this._Precision;
            }
            set
            {
                if ((this._Precision != value))
                {
                    this._Precision = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isoutparameter property for the object.
        /// </summary>
        [DataMember(Name = "IsOutParameter", IsRequired = true)]
        [XmlElement(ElementName = "IsOutParameter", IsNullable = false)]
        [Column(Storage = "_IsOutParameter", DbType = "Bit", CanBeNull = false)]
        public bool IsOutParameter
        {
            get
            {
                return this._IsOutParameter;
            }
            set
            {
                if ((this._IsOutParameter != value))
                {
                    this._IsOutParameter = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the overloadname property for the object.
        /// </summary>
        [DataMember(Name = "OverloadName", IsRequired = false)]
        [XmlElement(ElementName = "OverloadName", IsNullable = true)]
        [Column(Storage = "_OverloadName", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string OverloadName
        {
            get
            {
                return this._OverloadName;
            }
            set
            {
                if ((this._OverloadName != value))
                {
                    this._OverloadName = value;
                }
            }
        }
    }

    /// <summary>
    /// The columnvaluesresult linq object class.
    /// </summary>
    [DataContract(Name = "XmlObjectResult")]
    [Serializable()]
    public partial class XmlObjectResult
    {

        private string _ObjectXml;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public XmlObjectResult()
        {
        }

        /// <summary>
        /// Gets sets, the columnvalue property for the object.
        /// </summary>
        [DataMember(Name = "ObjectXml", IsRequired = false)]
        [XmlElement(ElementName = "ObjectXml", IsNullable = true)]
        [Column(Storage = "_ObjectXml", DbType = "VarChar(MAX)", CanBeNull = true)]
        public string ObjectXml
        {
            get
            {
                return this._ObjectXml;
            }
            set
            {
                if ((this._ObjectXml != value))
                {
                    this._ObjectXml = value;
                }
            }
        }
    }
}
