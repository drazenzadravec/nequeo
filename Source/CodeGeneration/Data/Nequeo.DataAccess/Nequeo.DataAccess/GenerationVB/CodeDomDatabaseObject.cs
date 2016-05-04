using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;

using Nequeo.CustomTool.CodeGenerator.Common;

namespace Nequeo.CustomTool.CodeGenerator.GenerationVB
{
    /// <summary>
    /// Database table object code generator.
    /// </summary>
    public class CodeDomDatabaseObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
        private string _dataBase = "Database";
        private string _tableName = "Table";
        private string _owner = "Owner";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _databaseConnection = "Connection";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private bool _tableListExclusion = true;
        private string _dataBaseConnect = "username/password";
        private string _dataBaseOwner = "owner";
        private DataBaseObjectContainer _data = null;

        IEnumerable<TablesResult> _tables = null;
        IEnumerable<TablesResult> _views = null;
        IEnumerable<TableColumnsResult> _columns = null;
        IEnumerable<PrimaryKeyColumnsResult> _tablePKs = null;
        IEnumerable<ForeignKeyTableResult> _tableFKs = null;
        IEnumerable<ForeignKeyTableResult> _tableRefs = null;

        /// <summary>
        /// Generates the table objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public CodeCompileUnit GenerateCode(DataBaseObjectContainer data)
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
            _data = ConvertToUpperCase(data);

            // Create the namespace.
            InitialiseNamespace();

            // Get the database tables.
            if(GetDatabaseTables())
                // Add the classes.
                AddClasses();

            // Return the complie unit.
            _targetUnit.Namespaces.Add(samples);
            return _targetUnit;
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
        /// Create the namespace and import namespaces.
        /// </summary>
        private void InitialiseNamespace()
        {
            // Create a new base compile unit module.
            _targetUnit = new CodeCompileUnit();

            // Create a new namespace.
            samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBase + ".Data" + (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : ""));

            // Add each namespace reference.
            samples.Imports.Add(new CodeNamespaceImport("System"));
            samples.Imports.Add(new CodeNamespaceImport("System.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("System.Text"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data"));
            samples.Imports.Add(new CodeNamespaceImport("System.Threading"));
            samples.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            samples.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
            samples.Imports.Add(new CodeNamespaceImport("System.Collections"));
            samples.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            samples.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            samples.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            samples.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));

            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Base.Exception"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Access.Control.Generic"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Access.Control.Generic.Data"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Enumeration.Exception"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Controller.Custom"));
        }

        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            foreach (var table in _tables)
            {
                if (_data.TableList.Contains(table.TableName.ToUpper()) == !_tableListExclusion)
                {
                    // Create the class and add base inheritance type.
                    _targetClass = new CodeTypeDeclaration(table.TableName);
                    _targetClass.IsClass = true;
                    _targetClass.IsPartial = true;
                    _targetClass.TypeAttributes = TypeAttributes.Public;
                    _targetClass.BaseTypes.Add(new CodeTypeReference("DataBase"));
                    _targetClass.BaseTypes.Add(new CodeTypeReference("INotifyPropertyChanged"));
                    _targetClass.BaseTypes.Add(new CodeTypeReference("INotifyPropertyChanging"));

                    // Create the attributes on the class.
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + table.TableName + "\""))));
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataTableAttribute",
                        new CodeAttributeArgument(new CodePrimitiveExpression(table.TableOwner + "." + table.TableName))));

                    // Create the comments on the class.
                    _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
                    _targetClass.Comments.Add(new CodeCommentStatement("The " + table.TableName.ToLower() + " data object class.", true));
                    _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

                    _tableName = table.TableName;
                    _owner = table.TableOwner;

                    // Add the class members.
                    AddMembers();

                    // Add the class to the namespace
                    // and add the namespace to the unit.
                    samples.Types.Add(_targetClass);
                }
            }

            // if a views exist.
            if (GetDatabaseViews())
            {
                foreach (var view in _views)
                {
                    if (_data.TableList.Contains(view.TableName.ToUpper()) == !_tableListExclusion)
                    {
                        // Create the class and add base inheritance type.
                        _targetClass = new CodeTypeDeclaration(view.TableName);
                        _targetClass.IsClass = true;
                        _targetClass.IsPartial = true;
                        _targetClass.TypeAttributes = TypeAttributes.Public;
                        _targetClass.BaseTypes.Add(new CodeTypeReference("DataBase"));
                        _targetClass.BaseTypes.Add(new CodeTypeReference("INotifyPropertyChanged"));

                        // Create the attributes on the class.
                        _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute",
                            new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + view.TableName + "\""))));
                        _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));
                        _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataTableAttribute",
                            new CodeAttributeArgument(new CodePrimitiveExpression(view.TableOwner + "." + view.TableName))));

                        // Create the comments on the class.
                        _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
                        _targetClass.Comments.Add(new CodeCommentStatement("The " + view.TableName.ToLower() + " data object class.", true));
                        _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

                        _tableName = view.TableName;
                        _owner = view.TableOwner;

                        // Add the class members.
                        AddMembers();

                        // Add the class to the namespace
                        // and add the namespace to the unit.
                        samples.Types.Add(_targetClass);
                    }
                }
            }
        }

        /// <summary>
        /// Add the class members.
        /// </summary>
        private void AddMembers()
        {
            AddConstructors();
            AddConstants();

            bool ret = GetDatabaseTableColumns();

            if (ret)
            {
                AddExtensibilityMethods();
            }

            if (ret)
            {
                AddProperties();
            }

            if (GetDatabaseForeignKeyColumns())
            {   
                AddPropertiesFk();
            }

            if (GetDatabaseReferenceKeyColumns())
            {
                AddPropertiesRef();
            }

            AddEvents();
            AddMethods();
            AddMethodsMore();
        }

        /// <summary>
        /// Add the constrctor to the class.
        /// </summary>
        private void AddConstructors()
        {
            // Declare the constructor.
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            constructor.Statements.Add(new CodeSnippetExpression("OnCreated()"));

            // Create the comments on the constructor.
            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the constructor to the class.
            _targetClass.Members.Add(constructor);
        }

        /// <summary>
        /// Add the constants to the class.
        /// </summary>
        private void AddConstants()
        {
            // Create a new field.
            CodeMemberField changedPropertyField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            changedPropertyField.Attributes = MemberAttributes.Private;
            changedPropertyField.Name = ("_changedPropertyNames");
            changedPropertyField.InitExpression = new CodeSnippetExpression("New System.Collections.Generic.List(Of String)()");
            changedPropertyField.Type = new CodeTypeReference("System.Collections.Generic.List(Of String)");

            // Add the field to the class.
            _targetClass.Members.Add(changedPropertyField);
        }

        /// <summary>
        /// Add the extensibility method to the class.
        /// </summary>
        private void AddExtensibilityMethods()
        {
            // Create a new field.
            CodeSnippetTypeMember onCreateMethod = new CodeSnippetTypeMember("\t\tPartial Sub OnCreated();\r\n");

            // Create the comments on the constructor.
            onCreateMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            onCreateMethod.Comments.Add(new CodeCommentStatement("On create data entity.", true));
            onCreateMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Create a new field.
            CodeSnippetTypeMember onLoadMethod = new CodeSnippetTypeMember("\t\tPartial Sub OnLoaded();\r\n");

            // Create the comments on the constructor.
            onLoadMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            onLoadMethod.Comments.Add(new CodeCommentStatement("On load data entity.", true));
            onLoadMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Create a start and end region directive
            // on the entire class.
            onCreateMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start,
                "Extensibility Method Definitions"));

            _targetClass.Members.Add(onCreateMethod);
            _targetClass.Members.Add(onLoadMethod);

            int j = 0;
            foreach (var result in _columns)
            {
                string name = (result.ColumnName).ReplaceKeyOperands().ReplaceNumbers();
                string fieldName = ("_" + result.ColumnName).ReplaceKeyOperands();

                // If a duplicate name is found.
                if ((_columns.Count(u => u.ColumnName == (result.ColumnName).ReplaceKeyOperands()) > 1) ||
                    (result.ColumnName.ToLower() == _tableName.ToLower()) ||
                    (LinqToDataTypes.KeyWordList.Contains(name)))
                {
                    j++;
                    fieldName = ("_" + result.ColumnName + (j).ToString()).ReplaceKeyOperands();
                    name = (result.ColumnName + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                }

                // Create a new field.
                CodeSnippetTypeMember onColumnChangedMethod = new CodeSnippetTypeMember("\t\tPartial Sub On" + name + "Changed();\r\n");

                // Create the comments on the constructor.
                onColumnChangedMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                onColumnChangedMethod.Comments.Add(new CodeCommentStatement("On " + name + " column data entity changed.", true));
                onColumnChangedMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                _targetClass.Members.Add(onColumnChangedMethod);

                // Create a new field.
                CodeSnippetTypeMember onColumnChangingMethod = new CodeSnippetTypeMember("\t\tPartial Sub On" + name + "Changing();\r\n");

                // Create the comments on the constructor.
                onColumnChangingMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                onColumnChangingMethod.Comments.Add(new CodeCommentStatement("On " + name + " column data entity changing.", true));
                onColumnChangingMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                _targetClass.Members.Add(onColumnChangingMethod);
            }

            // Create a custome endregion.
            CodeSnippetTypeMember endRegion = new CodeSnippetTypeMember("\t\t#End Region");

            // Add the constructor to the class.
            _targetClass.Members.Add(endRegion);
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties()
        {
            int j = 0;

            // Get all primary keys.
            bool primaryKeysExist = GetDatabasePrimaryKeys();

            // For each column found in the table
            // iterate through the list and create
            // each property.
            foreach (var result in _columns)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                string name = (result.ColumnName).ReplaceKeyOperands().ReplaceNumbers();
                string fieldName = ("_" + result.ColumnName).ReplaceKeyOperands();

                // If a duplicate name is found.
                if ((_columns.Count(u => u.ColumnName == (result.ColumnName).ReplaceKeyOperands()) > 1) ||
                    (result.ColumnName.ToLower() == _tableName.ToLower()) ||
                    (LinqToDataTypes.KeyWordList.Contains(name)))
                {
                    j++;
                    fieldName = ("_" + result.ColumnName + (j).ToString()).ReplaceKeyOperands();
                    name = (result.ColumnName + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                }

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Private;
                valueField.Name = fieldName;

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (result.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                {
                    // Assign the data type for the field if
                    // the data type is not a reference type
                    // then create a nullable type field.
                    valueField.Type = new CodeTypeReference("System.Nullable(Of " +
                        Common.LinqToDataTypes.GetLinqDataType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ")");
                }
                else
                    // Assign the field type for the field.
                    // Get the data type of the field from
                    // the sql data type.
                    valueField.Type = new CodeTypeReference(Common.LinqToDataTypes.GetLinqDataType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)));

                // Add the field to the class.
                _targetClass.Members.Add(valueField);

                // Create a new property member
                // and the accessor type.
                CodeMemberProperty valueProperty = new CodeMemberProperty();
                valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                // Assign the name and get and set indictors.
                valueProperty.Name = name;
                valueProperty.HasGet = true;
                valueProperty.HasSet = true;

                // Add the comments to the property.
                valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + result.ColumnName.ToLower() + " property for the object.", true));
                valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (result.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                {
                    // Assign the data type for the property if
                    // the data type is not a reference type
                    // then create a nullable type property.
                    valueProperty.Type = new CodeTypeReference("System.Nullable(Of " +
                        Common.LinqToDataTypes.GetLinqDataType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ")");
                }
                else
                    // Assign the property type for the property.
                    // Get the data type of the property from
                    // the sql data type.
                    valueProperty.Type = new CodeTypeReference(Common.LinqToDataTypes.GetLinqDataType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)));

                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName)));

                // Create a new code condition statement.
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    new CodeVariableReferenceExpression("(this." + fieldName + " != value)"),
                    new CodeStatement[] { 
                        new CodeExpressionStatement(new CodeSnippetExpression("Me.On" + name + "Changing()")),
                        new CodeExpressionStatement(new CodeSnippetExpression("Me.SendPropertyChanging(\"" + name + "\")")),
                        new CodeExpressionStatement(new CodeSnippetExpression("Me." + fieldName + " = value"))});

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(conditionalStatement);
                valueProperty.SetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("Me.SendPropertyChanged(\"" + name + "\")")));
                valueProperty.SetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("Me.On" + name + "Changed()")));

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\", IsRequired = " + (!result.ColumnNullable).ToString().ToLower()))));

                // If the type is an array the add the 
                // array attribute else add the element attribute.
                if (Common.LinqToDataTypes.GetLinqDataType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).IsArray)
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = " + result.ColumnNullable.ToString().ToLower()))));
                else
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = " + result.ColumnNullable.ToString().ToLower()))));

                bool isPrimaryKeyName = false;
                // If primary keys exist.
                if (primaryKeysExist)
                {
                    // If the current column is a primary key column.
                    if (_tablePKs.Where(k => k.PrimaryKeyName == result.ColumnName).Count() > 0)
                        isPrimaryKeyName = true;
                }

                // Assign the initial values.
                string isPrimaryKey = "";
                string isAutoGenerated = "";
                string isRowVersion = "";

                // If the column is a time stamp then
                // it is a row version.
                if (result.ColumnType.ToLower() == "timestamp")
                {
                    isRowVersion = "IsRowVersion = true, ";
                    isAutoGenerated = "IsAutoGenerated = true, ";
                }

                // If the column is a primary key.
                if (isPrimaryKeyName)
                    isPrimaryKey = "IsPrimaryKey = true, ";

                // If the column is auto generated or computed.
                if (Convert.ToBoolean(result.IsComputed))
                    isAutoGenerated = "IsAutoGenerated = true, ";

                // If the column is a primary key and
                // the column is auto generated with a seed.
                if (isPrimaryKeyName && Convert.ToBoolean(result.PrimaryKeySeed))
                {
                    isPrimaryKey = "IsPrimaryKey = true, ";
                    isAutoGenerated = "IsAutoGenerated = true, ";
                }

                // Get the default length value.
                int defaultLengthValue = LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType));

                // Create the attribite string.
                string dataColumnAttribute = "\"" + result.ColumnName + "\", " +
                    isPrimaryKey +
                    isRowVersion +
                    isAutoGenerated +
                    "DbType = \"" + result.ColumnType + "\", " +
                    "Length = " + ((result.Precision != null && result.Precision > 0) ? 
                        ((result.Length <= result.Precision) ? result.Length.ToString() : result.Precision.ToString()) :
                        ((Common.LinqToDataTypes.GetLinqNullableType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                        (result.Length.ToString()) : (defaultLengthValue).ToString())) + ", " +
                    "IsNullable = " + result.ColumnNullable.ToString().ToLower();

                // Att the attribite.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataColumnAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression(dataColumnAttribute))));

                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddPropertiesFk()
        {
            int j = 0;
            // For each column found in the table
            // iterate through the list and create
            // each property.
            foreach (var result in _tableFKs)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                string fieldName = ("_" + result.ForeignKeyTable).ReplaceKeyOperands();
                string name = (result.ForeignKeyTable).ReplaceKeyOperands().ReplaceNumbers();

                // If a duplicate name is found.
                if ((_tableFKs.Count(u => u.ColumnName == (result.ColumnName).ReplaceKeyOperands()) > 1) ||
                    (result.TableName.ToLower() == _tableName.ToLower()) ||
                    (LinqToDataTypes.KeyWordList.Contains(name)))
                {
                    j++;
                    fieldName = ("_" + result.ForeignKeyTable + (j).ToString()).ReplaceKeyOperands();
                    name = (result.ForeignKeyTable + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                }

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Private;
                valueField.Name = fieldName;

                valueField.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + result.ForeignKeyTable);

                // Add the field to the class.
                _targetClass.Members.Add(valueField);

                // Create a new property member
                // and the accessor type.
                CodeMemberProperty valueProperty = new CodeMemberProperty();
                valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                // Assign the name and get and set indictors.
                valueProperty.Name = name;
                valueProperty.HasGet = true;
                valueProperty.HasSet = true;

                // Add the comments to the property.
                valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + result.ColumnName.ToLower() + " foreign key property for the object.", true));
                valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                valueProperty.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + result.ForeignKeyTable);

                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName)));

                // Create a new code condition statement.
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    new CodeVariableReferenceExpression("(Me." + fieldName + " != value)"),
                    new CodeStatement[] { 
                        new CodeExpressionStatement(new CodeSnippetExpression("Me." + fieldName + " = value"))});

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(conditionalStatement);

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\", IsRequired = false"))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = true"))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataColumnForeignKeyAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("\"" + result.ForeignKeyTable +
                        "\", ColumnName = \"" + result.ColumnName + "\", ColumnType = \"" + result.ColumnType + "\", Length = " +
                        result.Length.ToString() + ", IsNullable = true, IsReference = true"))));


                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddPropertiesRef()
        {
            int j = 0;
            // For each column found in the table
            // iterate through the list and create
            // each field.
            foreach (var result in _tableRefs)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                string name = (result.TableName + "Collection").ReplaceKeyOperands().ReplaceNumbers();
                string fieldName = ("_" + (result.TableName + "Collection")).ReplaceKeyOperands();

                // If a duplicate name is found.
                if ((_tableRefs.Count(u => u.ColumnName == (result.ColumnName).ReplaceKeyOperands()) > 1) ||
                    (result.TableName.ToLower() == _tableName.ToLower()) ||
                    (LinqToDataTypes.KeyWordList.Contains(name)))
                {
                    j++;
                    fieldName = ("_" + (result.TableName + "Collection") + (j).ToString()).ReplaceKeyOperands();
                    name = ((result.TableName + "Collection") + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                }

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Private;
                valueField.Name = fieldName;

                valueField.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + result.TableName + "[]");

                // Add the field to the class.
                _targetClass.Members.Add(valueField);

                // Create a new property member
                // and the accessor type.
                CodeMemberProperty valueProperty = new CodeMemberProperty();
                valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                // Assign the name and get and set indictors.
                valueProperty.Name = name;
                valueProperty.HasGet = true;
                valueProperty.HasSet = true;

                // Add the comments to the property.
                valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + result.TableName.ToLower() + " reference property for the object.", true));
                valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                valueProperty.Type = new CodeTypeReference("Data" + (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName + "." : "Data.") + result.TableName + "[]");

                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName)));

                // Create a new code condition statement.
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    new CodeVariableReferenceExpression("(Me." + fieldName + " != value)"),
                    new CodeStatement[] { 
                        new CodeExpressionStatement(new CodeSnippetExpression("Me." + fieldName + " = value"))});

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(conditionalStatement);

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\", IsRequired = false"))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = true"))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataColumnForeignKeyAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("\"" + result.TableName +
                        "\", ColumnName = \"" + result.ColumnName + "\", ColumnType = \"" + result.ColumnType +
                        "\", Length = " + result.Length.ToString() + ", IsNullable = true, IsReference = false"))));

                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }

        /// <summary>
        /// Add the events to the class.
        /// </summary>
        private void AddEvents()
        {
            // Create a new event.
            CodeMemberEvent propertyChangedEvent = new CodeMemberEvent();

            // Assign the type data.
            propertyChangedEvent.Name = "PropertyChanged";
            propertyChangedEvent.Attributes = MemberAttributes.Public;
            propertyChangedEvent.Type = new CodeTypeReference(typeof(PropertyChangedEventHandler));

            // Add the comments to the event.
            propertyChangedEvent.Comments.Add(new CodeCommentStatement("<summary>", true));
            propertyChangedEvent.Comments.Add(new CodeCommentStatement("Property change event, triggered when a property changes.", true));
            propertyChangedEvent.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Create a new event.
            CodeMemberEvent propertyChangingEvent = new CodeMemberEvent();

            // Assign the type data.
            propertyChangingEvent.Name = "PropertyChanging";
            propertyChangingEvent.Attributes = MemberAttributes.Public;
            propertyChangingEvent.Type = new CodeTypeReference(typeof(PropertyChangingEventHandler));

            // Add the comments to the event.
            propertyChangingEvent.Comments.Add(new CodeCommentStatement("<summary>", true));
            propertyChangingEvent.Comments.Add(new CodeCommentStatement("Property changing event, triggered when a property is changing.", true));
            propertyChangingEvent.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the constructor to the class.
            _targetClass.Members.Add(propertyChangingEvent);
            _targetClass.Members.Add(propertyChangedEvent);
        }

        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        private void AddMethods()
        {
            // Create a new method.
            CodeMemberMethod sendPropertyChangedMethod = new CodeMemberMethod();

            // Assign the type data.
            sendPropertyChangedMethod.Name = "SendPropertyChanged";
            sendPropertyChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "propertyName"));
            sendPropertyChangedMethod.Attributes = MemberAttributes.Public;

            // Create a new code condition statement.
            CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                new CodeVariableReferenceExpression("(Me.PropertyChanged != null)"),
                new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName))")) });

            // Create a new code condition statement.
            CodeConditionStatement changedConditionalStatement = new CodeConditionStatement(
                new CodeVariableReferenceExpression("(!this._changedPropertyNames.Contains(propertyName))"),
                new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this._changedPropertyNames.Add(propertyName)")) });

            // Add the code statement.
            sendPropertyChangedMethod.Statements.Add(conditionalStatement);
            sendPropertyChangedMethod.Statements.Add(changedConditionalStatement);

            // Add the comments to the method.
            sendPropertyChangedMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            sendPropertyChangedMethod.Comments.Add(new CodeCommentStatement("Executes the property change event handle for the attached event.", true));
            sendPropertyChangedMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Create a new method.
            CodeMemberMethod sendPropertyChangingMethod = new CodeMemberMethod();

            // Assign the type data.
            sendPropertyChangingMethod.Name = "SendPropertyChanging";
            sendPropertyChangingMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "propertyName"));
            sendPropertyChangingMethod.Attributes = MemberAttributes.Public;

            // Create a new code condition statement.
            CodeConditionStatement conditionalStatement1 = new CodeConditionStatement(
                new CodeVariableReferenceExpression("(this.PropertyChanging != null)"),
                new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName))")) });

            // Add the code statement.
            sendPropertyChangingMethod.Statements.Add(conditionalStatement1);

            // Add the comments to the method.
            sendPropertyChangingMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            sendPropertyChangingMethod.Comments.Add(new CodeCommentStatement("Executes the property changing event handle for the attached event.", true));
            sendPropertyChangingMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the constructor to the class.
            _targetClass.Members.Add(sendPropertyChangingMethod);
            _targetClass.Members.Add(sendPropertyChangedMethod);
        }

        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        public void AddMethodsMore()
        {
            // Declaring a create data method
            CodeMemberMethod createObjectMethodData = new CodeMemberMethod();
            createObjectMethodData.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            createObjectMethodData.Name = "Create" + _tableName;
            createObjectMethodData.ReturnType = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _tableName);

            // Add the comments to the property.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<summary>", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("Create a new " + _tableName.ToLower() + " data entity.", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("</summary>", true));

            int j = 0;
            // For each column found.
            foreach (var result in _columns)
            {
                // If the column is not null.
                if (!result.ColumnNullable)
                {
                    string name = (result.ColumnName).ReplaceKeyOperands().ReplaceNumbers();
                    string fieldName = ("_" + result.ColumnName).ReplaceKeyOperands();

                    // If a duplicate name is found.
                    if ((_columns.Count(u => u.ColumnName == (result.ColumnName).ReplaceKeyOperands()) > 1) ||
                        (result.ColumnName.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + result.ColumnName + (j).ToString()).ReplaceKeyOperands();
                        name = (result.ColumnName + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                    }

                    // Add each parameter.
                    createObjectMethodData.Parameters.Add(new CodeParameterDeclarationExpression(Common.LinqToDataTypes.GetLinqDataType(result.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)), name.ToLowerFirstLetter()));
                    createObjectMethodData.Comments.Add(new CodeCommentStatement("<param name=\"" + name.ToLowerFirstLetter() + "\">Initial value of " + name + ".</param>", true));
                }
            }

            // Return comments.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<returns>The " + (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _tableName + " entity.</returns>", true));

            string tableName = _tableName;
            // If the name is a keyword.
            if (LinqToDataTypes.KeyWordList.Contains(tableName))
                tableName = tableName + "Item";

            // Add the create code statement.
            createObjectMethodData.Statements.Add(new CodeSnippetExpression((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _tableName + " " + tableName.ToLowerFirstLetter() + " = new " + (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _tableName + "()"));

            j = 0;
            // For each column found.
            foreach (var result in _columns)
            {
                // If the column is not null.
                if (!result.ColumnNullable)
                {
                    string name = (result.ColumnName).ReplaceKeyOperands().ReplaceNumbers();
                    string fieldName = ("_" + result.ColumnName).ReplaceKeyOperands();

                    // If a duplicate name is found.
                    if ((_columns.Count(u => u.ColumnName == (result.ColumnName).ReplaceKeyOperands()) > 1) ||
                        (result.ColumnName.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + result.ColumnName + (j).ToString()).ReplaceKeyOperands();
                        name = (result.ColumnName + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                    }

                    // Add each parameter assignment.
                    createObjectMethodData.Statements.Add(new CodeSnippetExpression(tableName.ToLowerFirstLetter() + "." + name + " = " + name.ToLowerFirstLetter()));
                }
            }

            // Assign each entity property
            // to from the parameter.
            createObjectMethodData.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression(tableName.ToLowerFirstLetter())));


            // Add the property to the class.
            _targetClass.Members.Add(createObjectMethodData);
        }
    }
}
