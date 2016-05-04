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
    /// The databasemodel data object class.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public partial class DatabaseModel
    {

        private string _CompanyNameSpace;

        private string _Database;

        private Table[] _TableEntity;

        private string _NamespaceExtendedName = null;

        private int _ConnectionType = 0;

        private int _ConnectionDataType = 0;

        private string _DataAccessProvider = null;

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
        /// Default constructor.
        /// </summary>
        public DatabaseModel()
        {
        }

        /// <summary>
        /// Gets sets, the companynamespace property for the object.
        /// </summary>
        [XmlElement(ElementName = "CompanyNameSpace", IsNullable = false)]
        public string CompanyNameSpace
        {
            get
            {
                return this._CompanyNameSpace;
            }
            set
            {
                if ((this._CompanyNameSpace != value))
                {
                    this._CompanyNameSpace = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the database property for the object.
        /// </summary>
        [XmlElement(ElementName = "Database", IsNullable = false)]
        public string Database
        {
            get
            {
                return this._Database;
            }
            set
            {
                if ((this._Database != value))
                {
                    this._Database = value;
                }
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
        /// Gets sets, the tableentity property for the object.
        /// </summary>
        [XmlArray(ElementName = "Tables", IsNullable = true)]
        public Table[] TableEntity
        {
            get
            {
                return this._TableEntity;
            }
            set
            {
                if ((this._TableEntity != value))
                {
                    this._TableEntity = value;
                }
            }
        }
    }

    /// <summary>
    /// The tableentity data object class.
    /// </summary>
    [Serializable()]
    public partial class Table
    {

        private string _TableName;

        private string _TableOwner;

        private DataColumn[] _TableEntityDataColumn;

        private ForeignKey[] _TableEntityForeignKey;

        private Reference[] _TableEntityReference;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Gets sets, the tablename property for the object.
        /// </summary>
        [XmlElement(ElementName = "TableName", IsNullable = false)]
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
        [XmlElement(ElementName = "TableOwner", IsNullable = false)]
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

        /// <summary>
        /// Gets sets, the tableentitydatacolumn property for the object.
        /// </summary>
        [XmlArray(ElementName = "DataColumns", IsNullable = true)]
        public DataColumn[] TableEntityDataColumn
        {
            get
            {
                return this._TableEntityDataColumn;
            }
            set
            {
                if ((this._TableEntityDataColumn != value))
                {
                    this._TableEntityDataColumn = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the tableentityforeignkey property for the object.
        /// </summary>
        [XmlArray(ElementName = "ForeignKeys", IsNullable = true)]
        public ForeignKey[] TableEntityForeignKey
        {
            get
            {
                return this._TableEntityForeignKey;
            }
            set
            {
                if ((this._TableEntityForeignKey != value))
                {
                    this._TableEntityForeignKey = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the tableentityreference property for the object.
        /// </summary>
        [XmlArray(ElementName = "References", IsNullable = true)]
        public Reference[] TableEntityReference
        {
            get
            {
                return this._TableEntityReference;
            }
            set
            {
                if ((this._TableEntityReference != value))
                {
                    this._TableEntityReference = value;
                }
            }
        }
    }

    /// <summary>
    /// The tableentitydatacolumn data object class.
    /// </summary>
    [Serializable()]
    public partial class DataColumn
    {
        private string _DbColumnName;

        private string _Name;

        private System.Nullable<System.Boolean> _IsPrimaryKey;

        private System.Nullable<System.Boolean> _IsAutoGenerated;

        private bool _IsNullable;

        private System.Nullable<System.Boolean> _IsRowVersion;

        private string _DbType;

        private long _Length;

        private System.Nullable<System.Boolean> _IsSeeded;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataColumn()
        {
        }

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "DbColumnName", IsNullable = false)]
        public string DbColumnName
        {
            get
            {
                return this._DbColumnName;
            }
            set
            {
                if ((this._DbColumnName != value))
                {
                    this._DbColumnName = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "Name", IsNullable = false)]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if ((this._Name != value))
                {
                    this._Name = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isseeded property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsSeeded", IsNullable = true)]
        public System.Nullable<System.Boolean> IsSeeded
        {
            get
            {
                return this._IsSeeded;
            }
            set
            {
                if ((this._IsSeeded != value))
                {
                    this._IsSeeded = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isprimarykey property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsPrimaryKey", IsNullable = true)]
        public System.Nullable<System.Boolean> IsPrimaryKey
        {
            get
            {
                return this._IsPrimaryKey;
            }
            set
            {
                if ((this._IsPrimaryKey != value))
                {
                    this._IsPrimaryKey = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isautogenerated property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsAutoGenerated", IsNullable = true)]
        public System.Nullable<System.Boolean> IsAutoGenerated
        {
            get
            {
                return this._IsAutoGenerated;
            }
            set
            {
                if ((this._IsAutoGenerated != value))
                {
                    this._IsAutoGenerated = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the isnullable property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsNullable", IsNullable = false)]
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
        /// Gets sets, the isrowversion property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsRowVersion", IsNullable = true)]
        public System.Nullable<System.Boolean> IsRowVersion
        {
            get
            {
                return this._IsRowVersion;
            }
            set
            {
                if ((this._IsRowVersion != value))
                {
                    this._IsRowVersion = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the dbtype property for the object.
        /// </summary>
        [XmlElement(ElementName = "DbType", IsNullable = false)]
        public string DbType
        {
            get
            {
                return this._DbType;
            }
            set
            {
                if ((this._DbType != value))
                {
                    this._DbType = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the length property for the object.
        /// </summary>
        [XmlElement(ElementName = "Length", IsNullable = false)]
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
    }

    /// <summary>
    /// The tableentityforeignkey data object class.
    /// </summary>
    [Serializable()]
    public partial class ForeignKey
    {

        private string _Name;

        private string _ForeignKeyTable;

        private string _ForeignKeyColumnName;

        private string _ForeignKeyOwner;

        private string _ColumnName;

        private string _ColumnType;

        private long _Length;

        private bool _IsNullable;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForeignKey()
        {
        }

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "Name", IsNullable = false)]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if ((this._Name != value))
                {
                    this._Name = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "ReferenceTable", IsNullable = false)]
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
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "ReferenceColumnName", IsNullable = false)]
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
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "ReferenceOwner", IsNullable = false)]
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

        /// <summary>
        /// Gets sets, the columnname property for the object.
        /// </summary>
        [XmlElement(ElementName = "ColumnName", IsNullable = false)]
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
        [XmlElement(ElementName = "ColumnType", IsNullable = false)]
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
        /// Gets sets, the length property for the object.
        /// </summary>
        [XmlElement(ElementName = "Length", IsNullable = false)]
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
        /// Gets sets, the isnullable property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsNullable", IsNullable = false)]
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
    }

    /// <summary>
    /// The tableentityreference data object class.
    /// </summary>
    [Serializable()]
    public partial class Reference
    {

        private string _Name;

        private string _ColumnName;

        private string _ColumnType;

        private long _Length;

        private bool _IsNullable;

        private string _Owner;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Reference()
        {
        }

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "Owner", IsNullable = false)]
        public string Owner
        {
            get
            {
                return this._Owner;
            }
            set
            {
                if ((this._Owner != value))
                {
                    this._Owner = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        [XmlElement(ElementName = "Name", IsNullable = false)]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if ((this._Name != value))
                {
                    this._Name = value;
                }
            }
        }

        /// <summary>
        /// Gets sets, the columnname property for the object.
        /// </summary>
        [XmlElement(ElementName = "ColumnName", IsNullable = false)]
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
        [XmlElement(ElementName = "ColumnType", IsNullable = false)]
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
        /// Gets sets, the length property for the object.
        /// </summary>
        [XmlElement(ElementName = "Length", IsNullable = false)]
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
        /// Gets sets, the isnullable property for the object.
        /// </summary>
        [XmlElement(ElementName = "IsNullable", IsNullable = false)]
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
    }
}
