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

namespace Nequeo.CustomTool.CodeGenerator.Generation
{
    /// <summary>
    /// Database table context object code generator.
    /// </summary>
    public class CodeDomDatabaseContext
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
        private string _dataBase = "Database";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _databaseConnection = "Connection";
        private string _contextName = "DataContext";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private bool _tableListExclusion = true;
        private bool _includeContextItems = true;
        private string _dataAccessProvider = "";
        private string _configKeyDatabaseConnection = "DatabaseConnection";
        private string _dataBaseConnect = "username/password";
        private string _dataBaseOwner = "owner";
        private DataBaseContextContainer _data = null;

        IEnumerable<TablesResult> _tables = null;
        IEnumerable<TablesResult> _views = null;

        /// <summary>
        /// Generates the table objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public CodeCompileUnit GenerateCode(DataBaseContextContainer data)
        {
            _dataBase = data.Database;
            _companyName = data.NamespaceCompanyName;
            _databaseConnection = data.DatabaseConnection;
            _configKeyDatabaseConnection = data.ConfigKeyDatabaseConnection;
            _contextName = data.ContextName;
            _connectionType = data.ConnectionType;
            _connectionDataType = data.ConnectionDataType;
            _tableListExclusion = data.TableListExclusion;
            _dataBaseConnect = data.DataBaseConnect;
            _dataBaseOwner = data.DataBaseOwner;
            _extendedName = data.NamespaceExtendedName;
            _includeContextItems = data.IncludeContextItems;
            _dataAccessProvider = data.DataAccessProvider;
            _data = ConvertToUpperCase(data);

            // Create the namespace.
            InitialiseNamespace();

            // Get the database tables.
            if (GetDatabaseTables())
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
        private DataBaseContextContainer ConvertToUpperCase(DataBaseContextContainer data)
        {
            DataBaseContextContainer items = data;
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

            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataType"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Control"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Custom"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.LinqToSql"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataSet"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Edm"));
            samples.Imports.Add(new CodeNamespaceImport("Nequeo.ComponentModel"));
            
            if(_includeContextItems)
                samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Linq"));
        }

        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            // Create the class and add base inheritance type.
            _targetClass = new CodeTypeDeclaration(_contextName);
            _targetClass.IsClass = true;
            _targetClass.IsPartial = true;
            _targetClass.TypeAttributes = TypeAttributes.Public;
            _targetClass.BaseTypes.Add(new CodeTypeReference("DataContextBase"));

            // Create a custom region.
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, _contextName + " Context Ling Type");
            _targetClass.StartDirectives.Add(startRegion);

            // Create the attributes on the class.
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DatabaseAttribute",
                new CodeAttributeArgument(new CodeSnippetExpression("\"" + _dataBase + "\""))));

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + _contextName.ToLower() + " data context object class.", true));
            _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            AddConstructors();
            AddConstants();
            AddExtensibilityMethods();
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
            AddProperties();
        }

        /// <summary>
        /// Add the constrctor to the class.
        /// </summary>
        private void AddConstructors()
        {
            string connectionType = Common.ConnectionProvider.GetConnectionTypeValue(_connectionType);
            string connectionDataType = Common.ConnectionProvider.GetConnectionDataTypeValue(_connectionDataType);

            // Declare the constructor.
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            constructor.BaseConstructorArgs.Add(new CodeSnippetExpression(
                "defaultDatabaseConnectionConfigurationKey, " +
                "Nequeo.Data.DataType.ConnectionContext.ConnectionType." + connectionType + ", " +
                "Nequeo.Data.DataType.ConnectionContext.ConnectionDataType." + connectionDataType + ", " +
                "new " + _dataAccessProvider + "()"));
            constructor.Statements.Add(new CodeSnippetExpression("OnCreated()"));

            // Create the comments on the constructor.
            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));

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

            // Create the comments on the constructor.
            constructor2.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor2.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor2.Comments.Add(new CodeCommentStatement("</summary>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"connectionConfigKey\">The database connection configuration key.</param>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"connectionType\">The connection type.</param>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"connectionDataType\">The connection data type.</param>", true));
            constructor2.Comments.Add(new CodeCommentStatement("<param name=\"dataAccessProvider\">The data access provider.</param>", true));

            // Add the constructor to the class.
            _targetClass.Members.Add(constructor);
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
            CodeSnippetTypeMember endRegion = new CodeSnippetTypeMember("\t\t#endregion");

            // Add the constructor to the class.
            _targetClass.Members.Add(onCreateMethod);
            _targetClass.Members.Add(onLoadMethod);
            _targetClass.Members.Add(endRegion);
        }


        /// <summary>
        /// Add each query property.
        /// </summary>
        private void AddProperties()
        {
            if (_includeContextItems)
            {
                int propertyCount = 0;
                CodeMemberProperty endProperty = null;

                foreach (var table in _tables)
                {
                    if (_data.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                    {
                        // Create a new property member
                        // and the accessor type.
                        CodeMemberProperty valueProperty = new CodeMemberProperty();
                        valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                        // Add the region directive if at the beginning
                        if (propertyCount == 0)
                        {
                            // Create a custom region.
                            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Ling Properties");
                            valueProperty.StartDirectives.Add(startRegion);

                            // Increment the count.
                            propertyCount++;
                        }

                        // Assign the name and get and set indictors.
                        valueProperty.Name = LinqToDataTypes.Plural(table.TableName);
                        valueProperty.HasGet = true;

                        // Add the comments to the property.
                        valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                        valueProperty.Comments.Add(new CodeCommentStatement("Gets, the " + table.TableName.ToLower() + " queryable provider property for the object.", true));
                        valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                        // Assign the property type for the property.
                        // Get the data type of the property from
                        // the sql data type.
                        valueProperty.Type = new CodeTypeReference("Nequeo.Data.Linq.Query<" +
                            (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + table.TableName + ">");

                        // Add the code to the
                        // get section of the property.
                        valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), "GetTable<" +
                                    (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + table.TableName + ">()")));

                        // Assign each property until the end.
                        endProperty = valueProperty;

                        // Add the property to the class.
                        _targetClass.Members.Add(valueProperty);
                    }
                }

                if (endProperty != null)
                {
                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    endProperty.EndDirectives.Add(endRegion);
                }

                propertyCount = 0;
                endProperty = null;

                // if a views exist.
                if (GetDatabaseViews())
                {
                    foreach (var view in _views)
                    {
                        if (_data.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                        {
                            // Create a new property member
                            // and the accessor type.
                            CodeMemberProperty valueProperty = new CodeMemberProperty();
                            valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                            // Add the region directive if at the beginning
                            if (propertyCount == 0)
                            {
                                // Create a custom region.
                                CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Ling Properties");
                                valueProperty.StartDirectives.Add(startRegion);

                                // Increment the count.
                                propertyCount++;
                            }

                            // Assign the name and get and set indictors.
                            valueProperty.Name = LinqToDataTypes.Plural(view.TableName);
                            valueProperty.HasGet = true;

                            // Add the comments to the property.
                            valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                            valueProperty.Comments.Add(new CodeCommentStatement("Gets, the " + view.TableName.ToLower() + " queryable provider property for the object.", true));
                            valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                            // Assign the property type for the property.
                            // Get the data type of the property from
                            // the sql data type.
                            valueProperty.Type = new CodeTypeReference("Nequeo.Data.Linq.Query<" +
                                (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + view.TableName + ">");

                            // Add the code to the
                            // get section of the property.
                            valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), "GetTable<" +
                                        (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + view.TableName + ">()")));

                            // Assign each property until the end.
                            endProperty = valueProperty;

                            // Add the property to the class.
                            _targetClass.Members.Add(valueProperty);
                        }
                    }
                }

                if (endProperty != null)
                {
                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    endProperty.EndDirectives.Add(endRegion);
                }
            }
        }
    }
}
