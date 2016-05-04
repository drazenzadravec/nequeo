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
    /// Database table xml model object code generator.
    /// </summary>
    public class CodeDomDatabaseXmlModelObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
        private string _dataBase = "Database";
        private string _tableName = "Table";
        private string _owner = "Owner";
        private string _companyName = "Company";
        private string _extendedName = "";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private string _dataAccessProvider = "";
        private DatabaseModel _data = null;

        /// <summary>
        /// Generates the table objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public CodeCompileUnit GenerateCode(DatabaseModel data)
        {
            _data = data;
            _companyName = data.CompanyNameSpace;
            _dataBase = data.Database;
            _connectionType = data.ConnectionType;
            _connectionDataType = data.ConnectionDataType;
            _extendedName = data.NamespaceExtendedName;
            _dataAccessProvider = data.DataAccessProvider;

            // Create the namespace.
            InitialiseNamespace();

            // Add the classes.
            AddClasses();

            // Return the complie unit.
            _targetUnit.Namespaces.Add(samples);
            return _targetUnit;
        }

        /// <summary>
        /// Create the namespace and import namespaces.
        /// </summary>
        private void InitialiseNamespace()
        {
            // Create a new base compile unit module.
            _targetUnit = new CodeCompileUnit();

            // Create a new namespace.
            samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBase + ".Data" + 
                (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : ""));

            // Add each namespace reference.
            samples.Imports.Add(new CodeNamespaceImport("System"));
            samples.Imports.Add(new CodeNamespaceImport("System.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("System.Text"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data"));
            samples.Imports.Add(new CodeNamespaceImport("System.Threading"));
            samples.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            samples.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.OleDb"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.Odbc"));
            samples.Imports.Add(new CodeNamespaceImport("System.Collections"));
            samples.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            samples.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            samples.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            samples.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            samples.Imports.Add(new CodeNamespaceImport("System.Drawing"));
            samples.Imports.Add(new CodeNamespaceImport("System.Drawing.Design"));

            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataType"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Control"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Custom"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.LinqToSql"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataSet"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Edm"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.ComponentModel"));
        }

        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            if (_data.TableEntity != null && _data.TableEntity.Count() > 0)
            {
                foreach (var table in _data.TableEntity)
                {
                    // Create the class and add base inheritance type.
                    _targetClass = new CodeTypeDeclaration(table.TableName);
                    _targetClass.IsClass = true;
                    _targetClass.IsPartial = true;
                    _targetClass.TypeAttributes = TypeAttributes.Public;
                    _targetClass.BaseTypes.Add(new CodeTypeReference("DataBase"));
                    //_targetClass.BaseTypes.Add(new CodeTypeReference("INotifyPropertyChanged"));
                    //_targetClass.BaseTypes.Add(new CodeTypeReference("INotifyPropertyChanging"));

                    // Create a custom region.
                    CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, table.TableName + " Data Entity Type");
                    _targetClass.StartDirectives.Add(startRegion);

                    // Create the attributes on the class.
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + table.TableName + "\", IsReference = true"))));
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataTableAttribute",
                        new CodeAttributeArgument(new CodePrimitiveExpression(
                            (String.IsNullOrEmpty(table.TableOwner) ? table.TableName : table.TableOwner + "." + table.TableName)))));
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DatabaseAttribute",
                        new CodeAttributeArgument(new CodePrimitiveExpression(_dataBase))));
                    _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("KnownTypeAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("typeof(DataBase)"))));

                    // Create the comments on the class.
                    _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
                    _targetClass.Comments.Add(new CodeCommentStatement("The " + table.TableName.ToLower() + " data object class.", true));
                    _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

                    _tableName = table.TableName;
                    _owner = table.TableOwner;

                    // Add the class members.
                    AddMembers();

                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    _targetClass.EndDirectives.Add(endRegion);

                    // Add the class to the namespace
                    // and add the namespace to the unit.
                    samples.Types.Add(_targetClass);
                }
            }
        }

        /// <summary>
        /// Add the class members.
        /// </summary>
        private void AddMembers()
        {
            AddConstructors();
            //AddConstants();
            AddExtensibilityMethods();

            AddProperties();
            AddPropertiesFk();
            AddPropertiesRef();
            //AddPropertiesExtended();

            //AddEvents();
            //AddMethods();
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
            changedPropertyField.InitExpression = new CodeSnippetExpression("new System.Collections.Generic.List<string>()");
            changedPropertyField.Type = new CodeTypeReference("System.Collections.Generic.List<string>");

            // Add the field to the class.
            _targetClass.Members.Add(changedPropertyField);
        }

        /// <summary>
        /// Add the extensibility method to the class.
        /// </summary>
        private void AddExtensibilityMethods()
        {
            // Create a new field.
            CodeSnippetTypeMember onCreateMethod = new CodeSnippetTypeMember("\t\tpartial void OnCreated();\r\n");

            // Create the comments on the constructor.
            onCreateMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            onCreateMethod.Comments.Add(new CodeCommentStatement("On create data entity.", true));
            onCreateMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Create a new field.
            CodeSnippetTypeMember onLoadMethod = new CodeSnippetTypeMember("\t\tpartial void OnLoaded();\r\n");

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

            if (_data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn != null &&
                _data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn.Count() > 0)
            {
                int j = 0;
                // For each column found in the table
                // iterate through the list and create
                // each field.
                foreach (var result in _data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn)
                {
                    string name = (result.Name).ReplaceKeyOperands().ReplaceNumbers();

                    // If a duplicate name is found.
                    if ((_data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn.Count(u => u.Name == (result.Name).ReplaceKeyOperands()) > 1) ||
                        (result.Name.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        name = (result.Name + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                    }

                    // Create a new field.
                    CodeSnippetTypeMember onColumnChangedMethod = new CodeSnippetTypeMember("\t\tpartial void On" + name + "Changed();\r\n");

                    // Create the comments on the constructor.
                    onColumnChangedMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                    onColumnChangedMethod.Comments.Add(new CodeCommentStatement("On " + name + " column data entity changed.", true));
                    onColumnChangedMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                    _targetClass.Members.Add(onColumnChangedMethod);

                    // Create a new field.
                    CodeSnippetTypeMember onColumnChangingMethod = new CodeSnippetTypeMember("\t\tpartial void On" + name + "Changing();\r\n");

                    // Create the comments on the constructor.
                    onColumnChangingMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                    onColumnChangingMethod.Comments.Add(new CodeCommentStatement("On " + name + " column data entity changing.", true));
                    onColumnChangingMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                    _targetClass.Members.Add(onColumnChangingMethod);
                }
            }

            // Create a custome endregion.
            CodeSnippetTypeMember endRegion = new CodeSnippetTypeMember("\t\t#endregion");

            // Add the constructor to the class.
            _targetClass.Members.Add(endRegion);
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties()
        {
            if (_data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn != null &&
                _data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn.Count() > 0)
            {
                int j = 0;
                int propertyCount = 0;
                CodeMemberProperty endProperty = null;

                // For each column found in the table
                // iterate through the list and create
                // each property.
                foreach (var result in _data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn)
                {
                    // Create a new field.
                    CodeMemberField valueField = new CodeMemberField();

                    string fieldName = ("_" + result.Name).ReplaceKeyOperands();
                    string name = (result.Name).ReplaceKeyOperands().ReplaceNumbers();

                    // If a duplicate name is found.
                    if ((_data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn.Count(u => u.Name == (result.Name).ReplaceKeyOperands()) > 1) ||
                        (result.Name.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + result.Name + (j).ToString()).ReplaceKeyOperands();
                        name = (result.Name + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();    
                    }

                    // Assign the name and the accessor attribute.
                    valueField.Attributes = MemberAttributes.Private;
                    valueField.Name = fieldName;

                    // If the table column is nullable and
                    // the data type is not a reference type
                    // then apply the nullable generic base.
                    if (result.IsNullable && Common.LinqToDataTypes.GetLinqNullableType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                    {
                        // Assign the data type for the field if
                        // the data type is not a reference type
                        // then create a nullable type field.
                        valueField.Type = new CodeTypeReference("System.Nullable<" +
                            Common.LinqToDataTypes.GetLinqDataType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">");
                    }
                    else
                        // Assign the field type for the field.
                        // Get the data type of the field from
                        // the sql data type.
                        valueField.Type = new CodeTypeReference(Common.LinqToDataTypes.GetLinqDataType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)));

                    // Add the field to the class.
                    _targetClass.Members.Add(valueField);

                    // Create a new property member
                    // and the accessor type.
                    CodeMemberProperty valueProperty = new CodeMemberProperty();
                    valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    // Add the region directive if at the beginning
                    if (propertyCount == 0)
                    {
                        // Create a custom region.
                        CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Column Properties");
                        valueProperty.StartDirectives.Add(startRegion);

                        // Increment the count.
                        propertyCount++;
                    }
                    
                    // Assign the name and get and set indictors.
                    valueProperty.Name = name;
                    valueProperty.HasGet = true;
                    valueProperty.HasSet = true;

                    // Add the comments to the property.
                    valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                    valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + result.Name.ToLower() + " property for the object.", true));
                    valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                    // If the table column is nullable and
                    // the data type is not a reference type
                    // then apply the nullable generic base.
                    if (result.IsNullable && Common.LinqToDataTypes.GetLinqNullableType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                    {
                        // Assign the data type for the property if
                        // the data type is not a reference type
                        // then create a nullable type property.
                        valueProperty.Type = new CodeTypeReference("System.Nullable<" +
                            Common.LinqToDataTypes.GetLinqDataType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">");
                    }
                    else
                        // Assign the property type for the property.
                        // Get the data type of the property from
                        // the sql data type.
                        valueProperty.Type = new CodeTypeReference(Common.LinqToDataTypes.GetLinqDataType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)));

                    // Add the code to the
                    // get section of the property.
                    valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), fieldName)));

                    // Create a new code condition statement.
                    CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                        new CodeVariableReferenceExpression("(this." + fieldName + " != value)"),
                        new CodeStatement[] { 
                            new CodeExpressionStatement(new CodeSnippetExpression("this.On" + name + "Changing()")),
                            new CodeExpressionStatement(new CodeSnippetExpression("this.SendPropertyChanging(\"" + name + "\")")),
                            new CodeExpressionStatement(new CodeSnippetExpression("this." + fieldName + " = value")),
                            new CodeExpressionStatement(new CodeSnippetExpression("this.SendPropertyChanged(\"" + name + "\")")),
                            new CodeExpressionStatement(new CodeSnippetExpression("this.On" + name + "Changed()"))});

                    // Add the code to the
                    // set section of the property.
                    valueProperty.SetStatements.Add(conditionalStatement);
                    //valueProperty.SetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.SendPropertyChanged(\"" + name + "\")")));
                    //valueProperty.SetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.On" + name + "Changed()")));

                    //// Add the attributes to the property.
                    //valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CategoryAttribute",
                    //    new CodeAttributeArgument(new CodeSnippetExpression("\"Column\""))));

                    //// Add the attributes to the property.
                    //valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DescriptionAttribute",
                    //    new CodeAttributeArgument(new CodeSnippetExpression("\"Gets sets, the " + result.Name.ToLower() + " property for the object.\""))));

                    // Add the attributes to the property.
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\""))));

                    // If the type is an array the add the 
                    // array attribute else add the element attribute.
                    if (Common.LinqToDataTypes.GetLinqDataType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).IsArray)
                        valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                            new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = " + result.IsNullable.ToString().ToLower()))));
                    else
                        valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                            new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = " + result.IsNullable.ToString().ToLower()))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("SoapElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("IsNullable = " + result.IsNullable.ToString().ToLower()))));

                    // Assign the initial values.
                    string isPrimaryKey = "";
                    string isAutoGenerated = "";
                    string isRowVersion = "";

                    // If the column is a time stamp then
                    // it is a row version.
                    if (result.DbType.ToLower() == "timestamp")
                    {
                        isRowVersion = "IsRowVersion = true, ";
                        isAutoGenerated = "IsAutoGenerated = true, ";
                    }

                    // If the column is a primary key.
                    if (Convert.ToBoolean(result.IsPrimaryKey))
                        isPrimaryKey = "IsPrimaryKey = true, ";

                    // If the column is auto generated or computed.
                    if (Convert.ToBoolean(result.IsAutoGenerated))
                        isAutoGenerated = "IsAutoGenerated = true, ";

                    // If the column is a primary key and
                    // the column is auto generated with a seed.
                    if (Convert.ToBoolean(result.IsPrimaryKey) && Convert.ToBoolean(result.IsSeeded))
                    {
                        isPrimaryKey = "IsPrimaryKey = true, ";
                        isAutoGenerated = "IsAutoGenerated = true, ";
                    }

                    // Create the attribite string.
                    string dataColumnAttribute = "\"" + result.DbColumnName + "\", " +
                        isPrimaryKey +
                        isRowVersion +
                        isAutoGenerated +
                        "DbType = \"" + result.DbType + "\", " +
                        "Length = " + result.Length.ToString() + ", " +
                        "IsNullable = " + result.IsNullable.ToString().ToLower();

                    // Att the attribite.
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataColumnAttribute",
                            new CodeAttributeArgument(new CodeSnippetExpression(dataColumnAttribute))));

                    // Assign each property until the end.
                    endProperty = valueProperty;

                    // Add the property to the class.
                    _targetClass.Members.Add(valueProperty);
                }

                if (endProperty != null)
                {
                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    endProperty.EndDirectives.Add(endRegion);
                }
            }
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddPropertiesFk()
        {
            if (_data.TableEntity.First(t => t.TableName == _tableName).TableEntityForeignKey != null &&
                _data.TableEntity.First(t => t.TableName == _tableName).TableEntityForeignKey.Count() > 0)
            {
                int j = 0;
                int propertyCount = 0;
                CodeMemberProperty endProperty = null;

                // For each column found in the table
                // iterate through the list and create
                // each property.
                foreach (var result in _data.TableEntity.First(t => t.TableName == _tableName).TableEntityForeignKey)
                {
                    // Create a new field.
                    CodeMemberField valueField = new CodeMemberField();

                    string fieldName = ("_" + result.Name).ReplaceKeyOperands();
                    string name = (result.Name).ReplaceKeyOperands().ReplaceNumbers();

                    // If a duplicate name is found.
                    if ((_data.TableEntity.First(t => t.TableName == _tableName).TableEntityForeignKey.Count(u => u.Name == (result.Name).ReplaceKeyOperands()) > 1) ||
                        (result.Name.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + result.Name + (j).ToString()).ReplaceKeyOperands();
                        name = (result.Name + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
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

                    // Add the region directive if at the beginning
                    if (propertyCount == 0)
                    {
                        // Create a custom region.
                        CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Foreign Key Properties");
                        valueProperty.StartDirectives.Add(startRegion);

                        // Increment the count.
                        propertyCount++;
                    }

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
                        new CodeVariableReferenceExpression("(this." + fieldName + " != value)"),
                        new CodeStatement[] { 
                        new CodeExpressionStatement(new CodeSnippetExpression("this." + fieldName + " = value"))});

                    // Add the code to the
                    // set section of the property.
                    valueProperty.SetStatements.Add(conditionalStatement);

                    //// Add the attributes to the property.
                    //valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CategoryAttribute",
                    //    new CodeAttributeArgument(new CodeSnippetExpression("\"Reference\""))));

                    //// Add the attributes to the property.
                    //valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DescriptionAttribute",
                    //    new CodeAttributeArgument(new CodeSnippetExpression("\"Gets sets, the " + result.ColumnName.ToLower() + " foreign key property for the object.\""))));

                    // Add the attributes to the property.
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("EditorAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("typeof(DataObjectEditor), typeof(UITypeEditor)"))));

                    // Add the attributes to the property.
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\", IsRequired = false"))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = true"))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("SoapElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("IsNullable = true"))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataColumnForeignKeyAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression(
                            "\"" + (!String.IsNullOrEmpty(result.ForeignKeyOwner) ? result.ForeignKeyOwner + "." : "") + result.ForeignKeyTable +
                            "\", ReferenceColumnName = \"" + result.ForeignKeyColumnName +
                            "\", ColumnName = \"" + result.ColumnName + "\", ColumnType = \"" + result.ColumnType + "\", Length = " +
                            result.Length.ToString() + ", IsNullable = true, IsReference = true"))));

                    // Assign each property until the end.
                    endProperty = valueProperty;

                    // Add the property to the class.
                    _targetClass.Members.Add(valueProperty);
                }

                if (endProperty != null)
                {
                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    endProperty.EndDirectives.Add(endRegion);
                }
            }
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddPropertiesRef()
        {
            if (_data.TableEntity.First(t => t.TableName == _tableName).TableEntityReference != null &&
                _data.TableEntity.First(t => t.TableName == _tableName).TableEntityReference.Count() > 0)
            {
                int j = 0;
                int propertyCount = 0;
                CodeMemberProperty endProperty = null;

                // For each column found in the table
                // iterate through the list and create
                // each field.
                foreach (var result in _data.TableEntity.First(t => t.TableName == _tableName).TableEntityReference)
                {
                    // Create a new field.
                    CodeMemberField valueField = new CodeMemberField();

                    string name = (result.Name + "Collection").ReplaceKeyOperands().ReplaceNumbers();
                    string fieldName = ("_" + (result.Name + "Collection")).ReplaceKeyOperands();

                    // If a duplicate name is found.
                    if ((_data.TableEntity.First(t => t.TableName == _tableName).TableEntityReference.Count(u => u.Name == (result.Name).ReplaceKeyOperands()) > 1) ||
                        (result.Name.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + (result.Name + "Collection") + (j).ToString()).ReplaceKeyOperands();
                        name = ((result.Name + "Collection") + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                    }

                    // Assign the name and the accessor attribute.
                    valueField.Attributes = MemberAttributes.Private;
                    valueField.Name = fieldName;

                    valueField.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + result.Name + "[]");

                    // Add the field to the class.
                    _targetClass.Members.Add(valueField);

                    // Create a new property member
                    // and the accessor type.
                    CodeMemberProperty valueProperty = new CodeMemberProperty();
                    valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    // Add the region directive if at the beginning
                    if (propertyCount == 0)
                    {
                        // Create a custom region.
                        CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Reference Properties");
                        valueProperty.StartDirectives.Add(startRegion);

                        // Increment the count.
                        propertyCount++;
                    }

                    // Assign the name and get and set indictors.
                    valueProperty.Name = name;
                    valueProperty.HasGet = true;
                    valueProperty.HasSet = true;

                    // Add the comments to the property.
                    valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                    valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + result.Name.ToLower() + " reference property for the object.", true));
                    valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                    valueProperty.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + result.Name + "[]");

                    // Add the code to the
                    // get section of the property.
                    valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), fieldName)));

                    // Create a new code condition statement.
                    CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                        new CodeVariableReferenceExpression("(this." + fieldName + " != value)"),
                        new CodeStatement[] { 
                        new CodeExpressionStatement(new CodeSnippetExpression("this." + fieldName + " = value"))});

                    // Add the code to the
                    // set section of the property.
                    valueProperty.SetStatements.Add(conditionalStatement);

                    //// Add the attributes to the property.
                    //valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CategoryAttribute",
                    //    new CodeAttributeArgument(new CodeSnippetExpression("\"Collection\""))));

                    //// Add the attributes to the property.
                    //valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DescriptionAttribute",
                    //    new CodeAttributeArgument(new CodeSnippetExpression("\"Gets sets, the " + result.Name.ToLower() + " reference property for the object.\""))));

                    // Add the attributes to the property.
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\", IsRequired = false"))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + name + "\", IsNullable = true"))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("SoapElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("IsNullable = true"))));

                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataColumnForeignKeyAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression(
                            "\"" + (!String.IsNullOrEmpty(result.Owner) ? result.Owner + "." : "") + result.Name +
                            "\", ColumnName = \"" + result.ColumnName + "\", ColumnType = \"" + result.ColumnType +
                            "\", Length = " + result.Length.ToString() + ", IsNullable = true, IsReference = false"))));

                    // Assign each property until the end.
                    endProperty = valueProperty;

                    // Add the property to the class.
                    _targetClass.Members.Add(valueProperty);
                }

                if (endProperty != null)
                {
                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    endProperty.EndDirectives.Add(endRegion);
                }
            }
        }

        /// <summary>
        /// Add the properties extended to the class.
        /// </summary>
        private void AddPropertiesExtended()
        {
            // Create a new field.
            CodeMemberField lazyLoadingField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            lazyLoadingField.Attributes = MemberAttributes.Private;
            lazyLoadingField.Name = "_referenceLazyLoading";

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty lazyLoadingProperty = new CodeMemberProperty();
            lazyLoadingProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            lazyLoadingField.Type = new CodeTypeReference(typeof(Boolean));
            lazyLoadingField.InitExpression = new CodeSnippetExpression("false");

            // Add the field to the class.
            _targetClass.Members.Add(lazyLoadingField);

            // Assign the name and get and set indictors.
            lazyLoadingProperty.Name = "ReferenceLazyLoading";
            lazyLoadingProperty.HasGet = true;
            lazyLoadingProperty.HasSet = true;

            // Add the comments to the property.
            lazyLoadingProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            lazyLoadingProperty.Comments.Add(new CodeCommentStatement("Gets sets, the reference lazy loading property for the object.", true));
            lazyLoadingProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            lazyLoadingProperty.Type = new CodeTypeReference(typeof(Boolean));

            // Add the code to the
            // get section of the property.
            lazyLoadingProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), "_referenceLazyLoading")));

            // Create a new code condition statement.
            CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                new CodeVariableReferenceExpression("(this._referenceLazyLoading != value)"),
                new CodeStatement[] { 
                        new CodeExpressionStatement(
                            new CodeSnippetExpression("this._referenceLazyLoading = value"))});

            // Add the code to the
            // set section of the property.
            lazyLoadingProperty.SetStatements.Add(conditionalStatement);
            lazyLoadingProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlIgnoreAttribute"));

            // Add the property to the class.
            _targetClass.Members.Add(lazyLoadingProperty);
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
                new CodeVariableReferenceExpression("(this.PropertyChanged != null)"),
                new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName))")) });

            // Create a new code condition statement.
            CodeConditionStatement changedConditionalStatementName = new CodeConditionStatement(
                new CodeVariableReferenceExpression("(this._changedPropertyNames == null)"),
                new CodeStatement[] { 
                    new CodeExpressionStatement(
                        new CodeSnippetExpression("this._changedPropertyNames = new System.Collections.Generic.List<string>()")) });

            // Create a new code condition statement.
            CodeConditionStatement changedConditionalStatement = new CodeConditionStatement(
                new CodeVariableReferenceExpression("(!this._changedPropertyNames.Contains(propertyName))"),
                new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this._changedPropertyNames.Add(propertyName)")) });

            // Add the code statement.
            sendPropertyChangedMethod.Statements.Add(conditionalStatement);
            sendPropertyChangedMethod.Statements.Add(changedConditionalStatementName);
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
            foreach (var result in _data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn)
            {
                // If the column is not null.
                if (!result.IsNullable)
                {
                    string fieldName = ("_" + result.Name).ReplaceKeyOperands();
                    string name = (result.Name).ReplaceKeyOperands().ReplaceNumbers();

                    // If a duplicate name is found.
                    if ((_data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn.Count(u => u.Name == (result.Name).ReplaceKeyOperands()) > 1) ||
                        (result.Name.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + result.Name + (j).ToString()).ReplaceKeyOperands();
                        name = (result.Name + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                    }

                    // Add each parameter.
                    createObjectMethodData.Parameters.Add(
                        new CodeParameterDeclarationExpression(Common.LinqToDataTypes.GetLinqDataType(result.DbType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)), name.ToLowerFirstLetter()));
                    createObjectMethodData.Comments.Add(
                        new CodeCommentStatement("<param name=\"" + name.ToLowerFirstLetter() + "\">Initial value of " + name + ".</param>", true));
                }
            }

            // Return comments.
            createObjectMethodData.Comments.Add(
                new CodeCommentStatement("<returns>The " + 
                    (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + 
                    _tableName + " entity.</returns>", true));

            string tableName = _tableName;
            // If the name is a keyword.
            if (LinqToDataTypes.KeyWordList.Contains(tableName))
                tableName = tableName + "Item";

            // Add the create code statement.
            createObjectMethodData.Statements.Add(
                new CodeSnippetExpression(
                    (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + 
                    _tableName + " " + tableName.ToLowerFirstLetter() + " = new " + 
                    (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _tableName + "()"));

            j = 0;
            // For each column found.
            foreach (var result in _data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn)
            {
                // If the column is not null.
                if (!result.IsNullable)
                {
                    string fieldName = ("_" + result.Name).ReplaceKeyOperands();
                    string name = (result.Name).ReplaceKeyOperands().ReplaceNumbers();

                    // If a duplicate name is found.
                    if ((_data.TableEntity.First(t => t.TableName == _tableName).TableEntityDataColumn.Count(u => u.Name == (result.Name).ReplaceKeyOperands()) > 1) ||
                        (result.Name.ToLower() == _tableName.ToLower()) ||
                        (LinqToDataTypes.KeyWordList.Contains(name)))
                    {
                        j++;
                        fieldName = ("_" + result.Name + (j).ToString()).ReplaceKeyOperands();
                        name = (result.Name + (j).ToString()).ReplaceKeyOperands().ReplaceNumbers();
                    }

                    // Add each parameter assignment.
                    createObjectMethodData.Statements.Add(
                        new CodeSnippetExpression(tableName.ToLowerFirstLetter() + "." + name + " = " + name.ToLowerFirstLetter()));
                }
            }

            // Assign each entity property
            // to from the parameter.
            createObjectMethodData.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeArgumentReferenceExpression(tableName.ToLowerFirstLetter())));


            // Add the property to the class.
            _targetClass.Members.Add(createObjectMethodData);
        }
    }
}
