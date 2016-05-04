using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using Nequeo.CustomTool.CodeGenerator.Common;

namespace Nequeo.CustomTool.CodeGenerator.Generation
{
    /// <summary>
    /// Data extension object code generator.
    /// </summary>
    public class CodeDomDataExtensionObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
        private string _className = "Schema";
        private string _dataBaseName = "DataBase";
        private string _dataContextName = "DataContext";
        private string _companyName = "Company";
        private string _extendedName = "";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private string _dataAccessProvider = "";
        private string _configKeyDatabaseConnection = "DatabaseConnection";
        private string _inheritanceCodeTypeReference = string.Empty;
        private string _dataEntityClassName = ".Access.Control.AnonymousType";

        /// <summary>
        /// Generate the code
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="dataContextName">The data contect name</param>
        /// <param name="dataBase">The database name</param>
        /// <param name="companyName">The company top level namespace name</param>
        /// <param name="configKeyDatabaseConnection">The configuration of connection string.</param>
        /// <param name="connectionType">The connection type.</param>
        /// <param name="extendedName">The bottom level namespace name.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        /// <returns>The code complie unit, the generated code.</returns>
        public CodeCompileUnit GenerateCode(string tableName,
            string dataContextName, string dataBase, string companyName,
            string configKeyDatabaseConnection, int connectionType,
            string extendedName, int connectionDataType, string dataAccessProvider)
        {
            _dataBaseName = dataBase;
            _companyName = companyName;
            _className = tableName;
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            _dataContextName = dataContextName;
            _configKeyDatabaseConnection = configKeyDatabaseConnection;
            _extendedName = extendedName;
            _dataAccessProvider = dataAccessProvider;

            _dataEntityClassName = (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _className;
            _inheritanceCodeTypeReference = "DataBase<" + (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _className + ">";

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
            samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBaseName + ".Data" +
                (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : "") + ".Extension");

            // Add each namespace reference.
            samples.Imports.Add(new CodeNamespaceImport("System"));
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
            samples.Imports.Add(new CodeNamespaceImport("System.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("System.Linq.Expressions"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.Linq.SqlClient"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.Linq.Mapping"));

            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataType"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Control"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Custom"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.LinqToSql"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataSet"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Edm"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.ComponentModel"));

            samples.Imports.Add(new CodeNamespaceImport("Data = " + _companyName + ".DataAccess." + _dataBaseName + ".Data"));
        }

        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            // Create the class and add base inheritance type.
            _targetClass = new CodeTypeDeclaration(_className);
            _targetClass.IsClass = true;
            _targetClass.IsPartial = true;
            _targetClass.TypeAttributes = TypeAttributes.Public;
            _targetClass.BaseTypes.Add(new CodeTypeReference(_inheritanceCodeTypeReference));
            _targetClass.BaseTypes.Add(new CodeTypeReference("IDisposable"));

            // Create a custom region.
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, _className + " Data Extension Type");
            _targetClass.StartDirectives.Add(startRegion);

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + _className.ToLower() + " replica object class.", true));
            _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the class members.
            AddMembers();

            // Create a custom endregion.
            CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
            _targetClass.EndDirectives.Add(endRegion);

            // Add the class to the namespace
            // and add the namespace to the unit.
            samples.Types.Add(_targetClass);
        }

        /// <summary>
        /// Add the class members.
        /// </summary>
        private void AddMembers()
        {
            AddConstructors();
            AddConstants();
            AddFieldMore();
            AddPropertiesMore();
            AddProperties();
            AddExtensibilityMethods();
            AddMethods();
        }

        /// <summary>
        /// Add the constrctor to the class.
        /// </summary>
        private void AddConstructors()
        {
            string connectionType = Common.ConnectionProvider.GetConnectionTypeValue(_connectionType);
            string connectionDataType = Common.ConnectionProvider.GetConnectionDataTypeValue(_connectionDataType);

            // Declare the constructor.
            CodeConstructor constructor1 = new CodeConstructor();
            constructor1.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            constructor1.BaseConstructorArgs.Add(new CodeSnippetExpression(
                "defaultDatabaseConnectionConfigurationKey, " +
                "Nequeo.Data.DataType.ConnectionContext.ConnectionType." + connectionType + ", " +
                "Nequeo.Data.DataType.ConnectionContext.ConnectionDataType." + connectionDataType + ", " +
                "new " + _dataAccessProvider + "()"));
            constructor1.Statements.Add(new CodeSnippetExpression("OnCreated()"));
            constructor1.Statements.Add(new CodeSnippetExpression("_connectionConfigKey = defaultDatabaseConnectionConfigurationKey"));
            constructor1.Statements.Add(new CodeSnippetExpression("_connectionType = Nequeo.Data.DataType.ConnectionContext.ConnectionType." + connectionType));
            constructor1.Statements.Add(new CodeSnippetExpression("_connectionDataType = Nequeo.Data.DataType.ConnectionContext.ConnectionDataType." + connectionDataType));

            // Create the comments on the constructor.
            constructor1.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor1.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor1.Comments.Add(new CodeCommentStatement("</summary>", true));

            CodeConstructor constructor2 = new CodeConstructor();
            constructor2.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            constructor2.Statements.Add(new CodeSnippetExpression("OnCreated()"));
            constructor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connectionConfigKey"));
            constructor2.Parameters.Add(new CodeParameterDeclarationExpression("Nequeo.Data.DataType.ConnectionContext.ConnectionType", "connectionType"));
            constructor2.Parameters.Add(new CodeParameterDeclarationExpression("Nequeo.Data.DataType.ConnectionContext.ConnectionDataType", "connectionDataType"));
            constructor2.Parameters.Add(new CodeParameterDeclarationExpression("Nequeo.Data.DataType.IDataAccess", "dataAccessProvider"));
            constructor2.BaseConstructorArgs.Add(new CodeSnippetExpression("connectionConfigKey"));
            constructor2.BaseConstructorArgs.Add(new CodeSnippetExpression("connectionType"));
            constructor2.BaseConstructorArgs.Add(new CodeSnippetExpression("connectionDataType"));
            constructor2.BaseConstructorArgs.Add(new CodeSnippetExpression("dataAccessProvider"));
            constructor2.Statements.Add(new CodeSnippetExpression("_connectionConfigKey = connectionConfigKey"));
            constructor2.Statements.Add(new CodeSnippetExpression("_connectionType = connectionType"));
            constructor2.Statements.Add(new CodeSnippetExpression("_connectionDataType = connectionDataType"));

            // Create the comments on the constructor.
            constructor2.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor2.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor2.Comments.Add(new CodeCommentStatement("</summary>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"connectionConfigKey\">The database connection configuration key.</param>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"connectionType\">The connection type.</param>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"connectionDataType\">The connection data type.</param>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"dataAccessProvider\">The data access provider.</param>", true));

            //CodeSnippetTypeMember destructor = new CodeSnippetTypeMember("~" + _className + "() { base.DisposeDataContextInstance(); }");
            //destructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            //destructor.Comments.Add(new CodeCommentStatement("Destructor.", true));
            //destructor.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the constructor to the class.
            _targetClass.Members.Add(constructor1);
            _targetClass.Members.Add(constructor2);
        }

        /// <summary>
        /// Add the constants to the class.
        /// </summary>
        private void AddConstants()
        {
            CodeMemberField connectionDefaultConstant = new CodeMemberField();
            connectionDefaultConstant.Attributes = MemberAttributes.Const | MemberAttributes.Private;
            connectionDefaultConstant.Name = "defaultDatabaseConnectionConfigurationKey";

            connectionDefaultConstant.Type = new CodeTypeReference(typeof(String));
            connectionDefaultConstant.InitExpression = new CodeSnippetExpression("\"" + _configKeyDatabaseConnection + "\"");

            // Add the field to the class.
            _targetClass.Members.Add(connectionDefaultConstant);
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

            // Create a start and end region directive
            // on the entire class.
            onCreateMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start,
                "Extensibility Method Definitions"));
            CodeSnippetTypeMember endRegion = new CodeSnippetTypeMember("\t\t#endregion");

            // Add the constructor to the class.
            _targetClass.Members.Add(onCreateMethod);
            _targetClass.Members.Add(endRegion);
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties()
        {
            string connectionType = Common.ConnectionProvider.GetConnectionTypeValue(_connectionType);
            string connectionDataType = Common.ConnectionProvider.GetConnectionDataTypeValue(_connectionDataType);

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty tableProperty = new CodeMemberProperty();
            tableProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            tableProperty.Name = "DataContext";
            tableProperty.HasGet = true;

            // Add the comments to the property.
            tableProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            tableProperty.Comments.Add(new CodeCommentStatement("Gets, the " + _className.ToLower() + " entity table property for the object.", true));
            tableProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            tableProperty.Type = new CodeTypeReference(
                (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _dataContextName);

            // Add the code to the
            // get section of the property.
            tableProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeSnippetExpression("new " + (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") +
                    _dataContextName + "(_connectionConfigKey, _connectionType, _connectionDataType, new " + _dataAccessProvider + "())")));

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty lazyLoadingProperty = new CodeMemberProperty();
            lazyLoadingProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            lazyLoadingProperty.Name = "LazyLoading";
            lazyLoadingProperty.HasGet = true;
            lazyLoadingProperty.HasSet = true;

            // Add the comments to the property.
            lazyLoadingProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            lazyLoadingProperty.Comments.Add(new CodeCommentStatement("Gets sets, the reference lazy loading indicator.", true));
            lazyLoadingProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            lazyLoadingProperty.Type = new CodeTypeReference(typeof(System.Boolean));

            // Add the code to the
            // get section of the property.
            lazyLoadingProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_referenceLazyLoading")));

            // Add the code to the
            // set section of the property.
            lazyLoadingProperty.SetStatements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_referenceLazyLoading"), 
                    new CodePropertySetValueReferenceExpression()));

            // Add the property to the class.
            _targetClass.Members.Add(tableProperty);
            _targetClass.Members.Add(lazyLoadingProperty);
        }

        /// <summary>
        /// Add the fields to the class.
        /// </summary>
        private void AddFieldMore()
        {
            string connectionType = Common.ConnectionProvider.GetConnectionTypeValue(_connectionType);
            string connectionDataType = Common.ConnectionProvider.GetConnectionDataTypeValue(_connectionDataType);

            // Create a new field.
            CodeMemberField connectionTypeField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            connectionTypeField.Attributes = MemberAttributes.Private;
            connectionTypeField.Name = "_connectionType";
            connectionTypeField.Type = new CodeTypeReference("Nequeo.Data.DataType.ConnectionContext.ConnectionType");
            connectionTypeField.InitExpression = new CodeSnippetExpression("Nequeo.Data.DataType.ConnectionContext.ConnectionType." + connectionType);

            // Create a new field.
            CodeMemberField connectionDataTypeField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            connectionDataTypeField.Attributes = MemberAttributes.Private;
            connectionDataTypeField.Name = "_connectionDataType";
            connectionDataTypeField.Type = new CodeTypeReference("Nequeo.Data.DataType.ConnectionContext.ConnectionDataType");
            connectionDataTypeField.InitExpression = new CodeSnippetExpression("Nequeo.Data.DataType.ConnectionContext.ConnectionDataType." + connectionDataType);

            // Create a new field.
            CodeMemberField connectionConfigKeyField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            connectionConfigKeyField.Attributes = MemberAttributes.Private;
            connectionConfigKeyField.Name = "_connectionConfigKey";
            connectionConfigKeyField.Type = new CodeTypeReference(typeof(string));
            connectionConfigKeyField.InitExpression = new CodeSnippetExpression("\"" + _configKeyDatabaseConnection + "\"");

            // Create a new field.
            CodeMemberField selectField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            selectField.Attributes = MemberAttributes.Private;
            selectField.Name = "SelectContext";
            selectField.Type = new CodeTypeReference("ISelectDataGenericBase<" + _dataEntityClassName + ">");

            // Create a new field.
            CodeMemberField deleteField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            deleteField.Attributes = MemberAttributes.Private;
            deleteField.Name = "DeleteContext";
            deleteField.Type = new CodeTypeReference("IDeleteDataGenericBase<" + _dataEntityClassName + ">");

            // Create a new field.
            CodeMemberField insertField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            insertField.Attributes = MemberAttributes.Private;
            insertField.Name = "InsertContext";
            insertField.Type = new CodeTypeReference("IInsertDataGenericBase<" + _dataEntityClassName + ">");

            // Create a new field.
            CodeMemberField updateField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            updateField.Attributes = MemberAttributes.Private;
            updateField.Name = "UpdateContext";
            updateField.Type = new CodeTypeReference("IUpdateDataGenericBase<" + _dataEntityClassName + ">");

            // Create a new field.
            CodeMemberField linqDataField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            linqDataField.Attributes = MemberAttributes.Private;
            linqDataField.Name = "CommonContext";
            linqDataField.Type = new CodeTypeReference("ICommonDataGenericBase<" + _dataEntityClassName + ">");

            // Create a new field.
            CodeMemberField contextField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            contextField.Attributes = MemberAttributes.Private;
            contextField.Name = "Context";
            contextField.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + _dataContextName);

            // Create a new field.
            CodeMemberField lazyLoadingeField = new CodeMemberField();

            // Assign the name and the accessor attribute.
            lazyLoadingeField.Attributes = MemberAttributes.Private;
            lazyLoadingeField.Name = "_referenceLazyLoading";
            lazyLoadingeField.Type = new CodeTypeReference(typeof(System.Boolean));
            lazyLoadingeField.InitExpression = new CodeSnippetExpression("false");

            // Add the field to the class.
            _targetClass.Members.Add(connectionConfigKeyField);
            _targetClass.Members.Add(connectionTypeField);
            _targetClass.Members.Add(connectionDataTypeField);
            _targetClass.Members.Add(selectField);
            _targetClass.Members.Add(deleteField);
            _targetClass.Members.Add(insertField);
            _targetClass.Members.Add(updateField);
            _targetClass.Members.Add(linqDataField);
            _targetClass.Members.Add(contextField);
            _targetClass.Members.Add(lazyLoadingeField);
        }

        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        public void AddMethods()
        {
            string connectionType = Common.ConnectionProvider.GetConnectionTypeValue(_connectionType);
            string connectionDataType = Common.ConnectionProvider.GetConnectionDataTypeValue(_connectionDataType);

            // Declaring a create data method
            CodeMemberMethod defaultConnectionDBMethod = new CodeMemberMethod();
            defaultConnectionDBMethod.Attributes = MemberAttributes.Public;

            defaultConnectionDBMethod.Name = "DefaultDatabaseConnection";
            defaultConnectionDBMethod.ReturnType = new CodeTypeReference("String");

            // Add the comments to the property.
            defaultConnectionDBMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            defaultConnectionDBMethod.Comments.Add(new CodeCommentStatement("Get the default database connection string.", true));
            defaultConnectionDBMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Return statement
            defaultConnectionDBMethod.Statements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeBaseReferenceExpression(), "Common.DefaultConnection(_connectionConfigKey)")));

            // Return comments.
            defaultConnectionDBMethod.Comments.Add(new CodeCommentStatement("<returns>The default connection string.</returns>", true));

            // Declaring a queryable provider method
            CodeMemberMethod queryableProviderMethod = new CodeMemberMethod();
            queryableProviderMethod.Attributes = MemberAttributes.Public;

            queryableProviderMethod.Name = "GetQueryableProvider<T>";
            queryableProviderMethod.ReturnType =
                new CodeTypeReference("Nequeo.Linq.QueryableProvider<T>");

            // Add the comments to the property.
            queryableProviderMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            queryableProviderMethod.Comments.Add(new CodeCommentStatement("Gets the T queryable provider.", true));
            queryableProviderMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            queryableProviderMethod.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression(
                    "DataTypeConversion dtc = new DataTypeConversion(DataBaseContext.ConnectionDataType." + connectionDataType + ")")));
            queryableProviderMethod.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression(
                    "SqlStatementConstructor<T> sql = SqlStatementConstructor<T>." +
                    "CreateInstance(\"" + _className + "\", DataBaseContext.ConnectionType." + connectionType + ", " +
                    "dtc, base.Common.DefaultConnection(_connectionConfigKey))")));
            queryableProviderMethod.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression(
                    "Nequeo.Linq.QueryableProvider<T> " +
                        "queryableProvider = new Nequeo.Linq.QueryableProvider<T>(sql)")));

            // Return statement
            queryableProviderMethod.Statements.Add(new CodeMethodReturnStatement(
                new CodeSnippetExpression("queryableProvider")));

            // Return comments.
            queryableProviderMethod.Comments.Add(new CodeCommentStatement("<returns>The object queryable provider.</returns>", true));

            // Declaring a get queryable provider method
            CodeMemberMethod staticGetQueryableProviderMethod = new CodeMemberMethod();
            staticGetQueryableProviderMethod.Attributes = MemberAttributes.Static | MemberAttributes.Public;

            staticGetQueryableProviderMethod.Name = "QueryableProvider";
            staticGetQueryableProviderMethod.ReturnType =
                new CodeTypeReference("Nequeo.Linq.QueryableProvider<" + _dataEntityClassName + ">");

            // Add the comments to the property.
            staticGetQueryableProviderMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            staticGetQueryableProviderMethod.Comments.Add(new CodeCommentStatement("Gets the " + _dataEntityClassName + " queryable provider.", true));
            staticGetQueryableProviderMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Return statement

            staticGetQueryableProviderMethod.Statements.Add(new CodeMethodReturnStatement(
                new CodeSnippetExpression("new " + _className + "().Select.QueryableProvider()")));

            // Return comments.
            staticGetQueryableProviderMethod.Comments.Add(new CodeCommentStatement("<returns>The object queryable provider.</returns>", true));

            // Add the property to the class.
            _targetClass.Members.Add(defaultConnectionDBMethod);
            //_targetClass.Members.Add(queryableProviderMethod);
            _targetClass.Members.Add(staticGetQueryableProviderMethod);
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddPropertiesMore()
        {
            // Create a new property member
            // and the accessor type.
            CodeMemberProperty selectProperty = new CodeMemberProperty();
            selectProperty.Attributes = MemberAttributes.New | MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            selectProperty.Name = "Select";
            selectProperty.HasGet = true;

            // Add the comments to the property.
            selectProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            selectProperty.Comments.Add(new CodeCommentStatement("Gets, the select property override.", true));
            selectProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            selectProperty.Type = new CodeTypeReference("ISelectDataGenericBase<" + _dataEntityClassName + ">");

            // Add the code to the
            // get section of the property.
            selectProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.SelectContext = base.Select")));
            selectProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.SelectContext.LazyLoading = _referenceLazyLoading")));
            selectProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "SelectContext")));

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty deletetProperty = new CodeMemberProperty();
            deletetProperty.Attributes = MemberAttributes.New | MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            deletetProperty.Name = "Delete";
            deletetProperty.HasGet = true;

            // Add the comments to the property.
            deletetProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            deletetProperty.Comments.Add(new CodeCommentStatement("Gets, the delete property override.", true));
            deletetProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            deletetProperty.Type = new CodeTypeReference("IDeleteDataGenericBase<" + _dataEntityClassName + ">");

            // Add the code to the
            // get section of the property.
            deletetProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.DeleteContext = base.Delete")));
            deletetProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), "DeleteContext")));

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty inserttProperty = new CodeMemberProperty();
            inserttProperty.Attributes = MemberAttributes.New | MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            inserttProperty.Name = "Insert";
            inserttProperty.HasGet = true;

            // Add the comments to the property.
            inserttProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            inserttProperty.Comments.Add(new CodeCommentStatement("Gets, the insert property override.", true));
            inserttProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            inserttProperty.Type = new CodeTypeReference("IInsertDataGenericBase<" + _dataEntityClassName + ">");

            // Add the code to the
            // get section of the property.
            inserttProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.InsertContext = base.Insert")));
            inserttProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), "InsertContext")));

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty updateProperty = new CodeMemberProperty();
            updateProperty.Attributes = MemberAttributes.New | MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            updateProperty.Name = "Update";
            updateProperty.HasGet = true;

            // Add the comments to the property.
            updateProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            updateProperty.Comments.Add(new CodeCommentStatement("Gets, the update property override.", true));
            updateProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            updateProperty.Type = new CodeTypeReference("IUpdateDataGenericBase<" + _dataEntityClassName + ">");

            // Add the code to the
            // get section of the property.
            updateProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.UpdateContext = base.Update")));
            updateProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), "UpdateContext")));

            // Create a new property member
            // and the accessor type.
            CodeMemberProperty setDataField = new CodeMemberProperty();
            setDataField.Attributes = MemberAttributes.New | MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            setDataField.Name = "Common";
            setDataField.HasGet = true;

            // Add the comments to the property.
            setDataField.Comments.Add(new CodeCommentStatement("<summary>", true));
            setDataField.Comments.Add(new CodeCommentStatement("Gets, the common property override.", true));
            setDataField.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            setDataField.Type = new CodeTypeReference("ICommonDataGenericBase<" + _dataEntityClassName + ">");

            // Add the code to the
            // get section of the property.
            setDataField.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this.CommonContext = base.Common")));
            setDataField.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), "CommonContext")));

            // Add the property to the class.
            _targetClass.Members.Add(selectProperty);
            _targetClass.Members.Add(deletetProperty);
            _targetClass.Members.Add(inserttProperty);
            _targetClass.Members.Add(updateProperty);
            _targetClass.Members.Add(setDataField);
        }
    }
}
