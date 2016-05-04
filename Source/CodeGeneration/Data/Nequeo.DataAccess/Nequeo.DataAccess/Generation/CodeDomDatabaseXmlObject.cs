using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;

using Nequeo.CustomTool.CodeGenerator.Common;

namespace Nequeo.CustomTool.CodeGenerator.Generation
{
    /// <summary>
    /// Database table xml object code generator.
    /// </summary>
    public class CodeDomDatabaseXmlObject
    {
        private string _dataBase = "Database";
        private string _tableName = "Table";
        private string _owner = "Owner";
        private string _companyName = "Company";
        private string _extendedName = "";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private string _dataAccessProvider = "";
        private bool _tableListExclusion = true;
        private string _databaseConnection = "Connection";
        private string _dataBaseConnect = "username/password";
        private string _dataBaseOwner = "owner";
        private DataBaseObjectContainer _data = null;
        private DatabaseModel _databaseModel = null;

        IEnumerable<TablesResult> _tables = null;
        IEnumerable<TablesResult> _views = null;
        IEnumerable<TableColumnsResult> _columns = null;
        IEnumerable<PrimaryKeyColumnsResult> _tablePKs = null;
        IEnumerable<ForeignKeyTableResult> _tableFKs = null;
        IEnumerable<ForeignKeyTableResult> _tableRefs = null;

        private Table _tableEntity = null;

        /// <summary>
        /// Generates the table objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public MemoryStream GenerateCode(DataBaseObjectContainer data)
        {
            _dataBase = data.Database;
            _companyName = data.NamespaceCompanyName;
            _databaseConnection = data.DatabaseConnection;
            _connectionType = data.ConnectionType;
            _connectionDataType = data.ConnectionDataType;
            _tableListExclusion = data.TableListExclusion;
            _dataBaseConnect = data.DataBaseConnect;
            _dataBaseOwner = data.DataBaseOwner;
            _extendedName = data.NamespaceExtendedName;
            _dataAccessProvider = data.DataAccessProvider;
            _data = ConvertToUpperCase(data);

            // Get the database tables.
            if (GetDatabaseTables())
                // Create the object.
                CreateXmlModelObject();

            // Return the xml document.
            return SerialiseXmlModelObject(_databaseModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataBaseObjectContainer ConvertToUpperCase(DataBaseObjectContainer data)
        {
            DataBaseObjectContainer items = data;
            for (int i = 0; i < items.TableList.Count(); i++)
                items.TableList[i] = items.TableList[i].ToUpper();

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseModel"></param>
        /// <returns></returns>
        internal MemoryStream SerialiseXmlModelObject(DatabaseModel databaseModel)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be serialised.
            XmlSerializer serialiser = new XmlSerializer(typeof(DatabaseModel));

            // Create a new string writer, this
            // is where the xml object will be
            // written to.
            MemoryStream memoryStream = new MemoryStream();

            // Uses the serialise method to
            // create the xml document.
            serialiser.Serialize(memoryStream, databaseModel);
            memoryStream.Close();

            // Return the data.
            return memoryStream;
        }

        /// <summary>
        /// Gets the tables from the database.
        /// </summary>
        private bool GetDatabaseTables()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<TablesResult> list = access.GetTables(_databaseConnection, null,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);

            if (list != null && list.Count > 0)
            {
                _tables = list.Distinct(new UniqueTableNameComparer());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the views from the database.
        /// </summary>
        private bool GetDatabaseViews()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<TablesResult> list = access.GetViews(_databaseConnection, null,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);

            if (list != null && list.Count > 0)
            {
                _views = list.Distinct(new UniqueTableNameComparer());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the table columns from the database.
        /// </summary>
        private bool GetDatabaseTableColumns()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<TableColumnsResult> list = access.GetDatabaseTableColumns(
                _databaseConnection, null, _tableName, _owner,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);

            if (list != null && list.Count > 0)
            {
                _columns = list.Distinct(new UniqueColumnNameComparer());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the primary key columns from the database.
        /// </summary>
        private bool GetDatabasePrimaryKeys()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<PrimaryKeyColumnsResult> list = access.GetDatabasePrimaryKeys(
                _databaseConnection, null, _tableName,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);

            if (list != null && list.Count > 0)
            {
                _tablePKs = list.Distinct(new UniquePrimaryKeyNameComparer());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the foreign key columns from the database.
        /// </summary>
        private bool GetDatabaseForeignKeyColumns()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<ForeignKeyTableResult> list = access.GetDatabaseForeignKey(
                _databaseConnection, null, _tableName,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);

            if (list != null && list.Count > 0)
            {
                _tableFKs = list.Distinct(new UniqueColumnNameComparerFk());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the foreign key columns from the database.
        /// </summary>
        private bool GetDatabaseReferenceKeyColumns()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<ForeignKeyTableResult> list = access.GetDatabaseReferenceKey(
                _databaseConnection, null, _tableName,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);

            if (list != null && list.Count > 0)
            {
                _tableRefs = list.Distinct(new UniqueTableNameComparerRef());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Creates the model object.
        /// </summary>
        private void CreateXmlModelObject()
        {
            // Create a new database model object.
            _databaseModel = new DatabaseModel();
            _databaseModel.CompanyNameSpace = _companyName;
            _databaseModel.NamespaceExtendedName = _extendedName;
            _databaseModel.Database = _dataBase;
            _databaseModel.ConnectionType = _connectionType;
            _databaseModel.ConnectionDataType = _connectionDataType;
            _databaseModel.DataAccessProvider = _dataAccessProvider;

            // Create a new list collection.
            List<Table> tableEntities = new List<Table>();

            // For each table found in the list.
            foreach (var table in _tables)
            {
                // If the table item is not on the exclusion list.
                if (_data.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                {
                    // create a new table entity.
                    _tableEntity = new Table();
                    _tableEntity.TableName = table.TableName;
                    _tableEntity.TableOwner = table.TableOwner;

                    _tableName = table.TableName;
                    _owner = table.TableOwner;

                    // Add the table members.
                    CreateDataColumns();
                    CreateForeignKeyColumns();
                    CreateReferenceColumns();

                    // Add the table to the collection.
                    tableEntities.Add(_tableEntity);
                }
            }

            // Get the database views.
            if (GetDatabaseViews())
            {
                // For each view found in the list.
                foreach (var view in _views)
                {
                    // If the view item is not on the exclusion list.
                    if (_data.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                    {
                        // create a new table entity.
                        _tableEntity = new Table();
                        _tableEntity.TableName = view.TableName;
                        _tableEntity.TableOwner = view.TableOwner;

                        _tableName = view.TableName;
                        _owner = view.TableOwner;

                        // Add the table members.
                        CreateDataColumns();
                        CreateForeignKeyColumns();
                        CreateReferenceColumns();

                        // Add the table to the collection.
                        tableEntities.Add(_tableEntity);
                    }
                }
            }

            // The array of tables to the database model.
            _databaseModel.TableEntity = tableEntities.ToArray();
        }

        /// <summary>
        /// Creates the data column.
        /// </summary>
        private void CreateDataColumns()
        {
            // Get all data columns from the database.
            if (GetDatabaseTableColumns())
            {
                // Get all primary keys.
                bool primaryKeysExist = GetDatabasePrimaryKeys();

                int i = 0;
                DataColumn[] array = new DataColumn[_columns.Count()];

                // For each column found in the table
                // iterate through the list and create
                // each field.
                foreach (var result in _columns)
                {
                    DataColumn tableEntityDataColumn = new DataColumn();
                    tableEntityDataColumn.IsAutoGenerated = result.IsComputed;
                    tableEntityDataColumn.Name = result.ColumnName;

                    if (result.Precision != null && result.Precision > 0)
                    {
                        if ((result.Length <= result.Precision))
                            tableEntityDataColumn.Length = result.Length;
                        else
                            tableEntityDataColumn.Length = (long)result.Precision;
                    }
                    else
                    {
                        if (Common.LinqToDataTypes.GetLinqNullableType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                            tableEntityDataColumn.Length = result.Length;
                        else
                            tableEntityDataColumn.Length = LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType));
                    }

                    tableEntityDataColumn.IsNullable = result.ColumnNullable;
                    tableEntityDataColumn.DbType = result.ColumnType;
                    tableEntityDataColumn.DbColumnName = result.ColumnName;

                    // If primary keys exist.
                    if(primaryKeysExist)
                    {
                        // If the current column is a primary key column.
                        if(_tablePKs.Where(k => k.PrimaryKeyName == result.ColumnName).Count() > 0)
                            tableEntityDataColumn.IsPrimaryKey = true;
                        else
                            tableEntityDataColumn.IsPrimaryKey = false;
                    }

                    // Is the primary key is seeded.
                    if (result.PrimaryKeySeed)
                        tableEntityDataColumn.IsSeeded = true;
                    else
                        tableEntityDataColumn.IsSeeded = false;

                    // If the data type is row version.
                    if (result.ColumnType.ToLower() == "timestamp")
                        tableEntityDataColumn.IsRowVersion = true;
                    else
                        tableEntityDataColumn.IsRowVersion = false;

                    // Assign the current data columns.
                    array[i++] = tableEntityDataColumn;
                }

                // Add all the data columns.
                _tableEntity.TableEntityDataColumn = array;
            }
        }

        /// <summary>
        /// Creates the foreign key columns.
        /// </summary>
        private void CreateForeignKeyColumns()
        {
            // Get all foreign key columns from the database.
            if (GetDatabaseForeignKeyColumns())
            {
                List<ForeignKey> array = new List<ForeignKey>();

                // For each column found in the table
                // iterate through the list and create
                // each field.
                foreach (var result in _tableFKs)
                {
                    // If the table item is not on the exclusion list.
                    if (_data.TableList.Contains(result.ForeignKeyTable.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                    {
                        ForeignKey tableEntityForeignKey = new ForeignKey();
                        tableEntityForeignKey.ColumnName = result.ColumnName;
                        tableEntityForeignKey.ColumnType = result.ColumnType;
                        tableEntityForeignKey.Name = result.ForeignKeyTable;
                        tableEntityForeignKey.ForeignKeyTable = result.ForeignKeyTable;
                        tableEntityForeignKey.IsNullable = result.IsNullable;
                        tableEntityForeignKey.ForeignKeyColumnName = result.ForeignKeyColumnName;
                        tableEntityForeignKey.ForeignKeyOwner = result.ForeignKeyOwner;

                        if (result.Precision != null && result.Precision > 0)
                        {
                            if ((result.Length <= result.Precision))
                                tableEntityForeignKey.Length = result.Length;
                            else
                                tableEntityForeignKey.Length = (long)result.Precision;
                        }
                        else
                        {
                            if (Common.LinqToDataTypes.GetLinqNullableType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                tableEntityForeignKey.Length = result.Length;
                            else
                                tableEntityForeignKey.Length = LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType));
                        }

                        // Assign the current reference.
                        array.Add(tableEntityForeignKey);
                    }
                }

                // Add all the foreign keys.
                _tableEntity.TableEntityForeignKey = array.ToArray();
            }
        }

        /// <summary>
        /// Creates the reference columns.
        /// </summary>
        private void CreateReferenceColumns()
        {
            // Get all reference columns from the database.
            if (GetDatabaseReferenceKeyColumns())
            {
                List<Reference> array = new List<Reference>();

                // For each column found in the table
                // iterate through the list and create
                // each field.
                foreach (var result in _tableRefs)
                {
                    // If the table item is not on the exclusion list.
                    if (_data.TableList.Contains(result.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                    {
                        Reference tableEntityReference = new Reference();
                        tableEntityReference.ColumnName = result.ColumnName;
                        tableEntityReference.ColumnType = result.ColumnType;
                        tableEntityReference.Name = result.TableName;
                        tableEntityReference.IsNullable = result.IsNullable;
                        tableEntityReference.Owner = result.ForeignKeyOwner;

                        if (result.Precision != null && result.Precision > 0)
                        {
                            if ((result.Length <= result.Precision))
                                tableEntityReference.Length = result.Length;
                            else
                                tableEntityReference.Length = (long)result.Precision;
                        }
                        else
                        {
                            if (Common.LinqToDataTypes.GetLinqNullableType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                tableEntityReference.Length = result.Length;
                            else
                                tableEntityReference.Length = LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType));
                        }

                        // Assign the current reference.
                        array.Add(tableEntityReference);
                    }
                }

                // Add all the reference keys.
                _tableEntity.TableEntityReference = array.ToArray();
            }
        }
    }
}
