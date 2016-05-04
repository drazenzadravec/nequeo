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
    /// Replica data context code generator.
    /// </summary>
    public class CodeDomReplicaDataContext
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeNamespace samplesExtended;
        private CodeTypeDeclaration _targetClass;
        private CodeTypeDeclaration _targetContextClass;
        private string _contextName = "DataContext";
        private string _dataContextName = "DataContextName";
        private string _dataBase = "Database";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _databaseConnection = "Connection";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private bool _tableListExclusion = true;
        private string _dataBaseConnect = "username/password";
        private string _dataBaseOwner = "owner";
        private ReplicaDataContextObjectContainer _data = null;

        IEnumerable<TablesResult> _tables = null;
        IEnumerable<TablesResult> _views = null;

        /// <summary>
        /// Generates the table objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public CodeCompileUnit GenerateCode(ReplicaDataContextObjectContainer data)
        {
            _dataBase = data.Database;
            _companyName = data.NamespaceCompanyName;
            _databaseConnection = data.DatabaseConnection;
            _connectionType = data.ConnectionType;
            _connectionDataType = data.ConnectionDataType;
            _contextName = data.ContextName;
            _tableListExclusion = data.TableListExclusion;
            _dataBaseConnect = data.DataBaseConnect;
            _dataBaseOwner = data.DataBaseOwner;
            _extendedName = data.NamespaceExtendedName;
            _dataContextName = data.DataContextName;
            _data = ConvertToUpperCase(data);

            // Create the namespace.
            InitialiseNamespace();
            InitialiseNamespaceExtended();

            // Get the database tables.
            if (GetDatabaseTables())
            {
                // Add the classes.
                AddClasses();
                AddClassesExtended();
            }

            // Return the complie unit.
            _targetUnit.Namespaces.Add(samples);
            _targetUnit.Namespaces.Add(samplesExtended);
            return _targetUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ReplicaDataContextObjectContainer ConvertToUpperCase(ReplicaDataContextObjectContainer data)
        {
            ReplicaDataContextObjectContainer items = data;
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
        /// Gets the tables from the database.
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
            samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBase + ".LinqToSql" +
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

            samples.Imports.Add(new CodeNamespaceImport("LinqToSql = " + _companyName + ".DataAccess." + _dataBase + ".LinqToSql"));
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
            _targetClass.BaseTypes.Add(new CodeTypeReference("Disposable"));
            _targetClass.BaseTypes.Add(new CodeTypeReference("IDisposable"));

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The data access context replica object class.", true));
            _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the class members.
            AddMembers();

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
            AddFields();
            AddProperties();
        }

        /// <summary>
        /// Add the constrctor to the class.
        /// </summary>
        private void AddConstructors()
        {
            // Declare the constructor.
            CodeConstructor constructor1 = new CodeConstructor();
            constructor1.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            // Create the comments on the constructor.
            constructor1.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor1.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor1.Comments.Add(new CodeCommentStatement("</summary>", true));

            //CodeConstructor constructor2 = new CodeConstructor();
            //constructor2.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            //constructor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "specificPath"));

            //// Create the comments on the constructor.
            //constructor2.Comments.Add(new CodeCommentStatement("<summary>", true));
            //constructor2.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            //constructor2.Comments.Add(new CodeCommentStatement("</summary>", true));
            //constructor2.Comments.Add(new CodeCommentStatement("<param name=\"specificPath\">The specific path of the config file, used for web applications.</param>", true));

            //// Add the initial statement value.
            //constructor2.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("_specificPath = specificPath")));

            // Add the constructor to the class.
            _targetClass.Members.Add(constructor1);
            //_targetClass.Members.Add(constructor2);
        }

        /// <summary>
        /// Add the fields to the class.
        /// </summary>
        private void AddFields()
        {
            //CodeMemberField specificPathField = new CodeMemberField();
            //specificPathField.Attributes = MemberAttributes.Private;
            //specificPathField.Name = "_specificPath";

            //specificPathField.Type = new CodeTypeReference(typeof(String));
            //specificPathField.InitExpression = new CodeSnippetExpression("String.Empty");

            //// Add the field to the class.
            //_targetClass.Members.Add(specificPathField);
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties()
        {
            int propertyCount = 0;
            CodeMemberProperty endProperty = null;

            // For each table in the database.
            foreach (var table in _tables)
            {
                if (_data.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                {
                    // Create a new property member
                    // and the accessor type.
                    CodeMemberProperty tableProperty = new CodeMemberProperty();
                    tableProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    // Add the region directive if at the beginning
                    if (propertyCount == 0)
                    {
                        // Create a custom region.
                        CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Context Extension Properties");
                        tableProperty.StartDirectives.Add(startRegion);

                        // Increment the count.
                        propertyCount++;
                    }

                    // Assign the name and get and set indictors.
                    tableProperty.Name = table.TableName;
                    tableProperty.HasGet = true;

                    // Add the comments to the property.
                    tableProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                    tableProperty.Comments.Add(new CodeCommentStatement("Gets, the " + table.TableName.ToLower() + " replica property for the object.", true));
                    tableProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                    // Assign the return type.
                    tableProperty.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + table.TableName);

                    //// Create a new code condition statement.
                    //CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    //    new CodeVariableReferenceExpression("(String.IsNullOrEmpty(_specificPath))"),
                    //    new CodeStatement[] { 
                    //        new CodeExpressionStatement(
                    //            new CodeSnippetExpression("return new " + 
                    //                (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + table.TableName + "()")) },
                    //    new CodeStatement[] { 
                    //        new CodeExpressionStatement(
                    //            new CodeSnippetExpression("return new " + 
                    //                (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + table.TableName + "(_specificPath)")) });

                    CodeExpressionStatement conditionalStatement = new CodeExpressionStatement(
                                new CodeSnippetExpression("return new " +
                                    (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + table.TableName + "()"));

                    // Add the condition statement.
                    tableProperty.GetStatements.Add(conditionalStatement);

                    // Assign each property until the end.
                    endProperty = tableProperty;

                    // Add the property to the class.
                    _targetClass.Members.Add(tableProperty);
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
                // For each table in the database.
                foreach (var view in _views)
                {
                    if (_data.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !_tableListExclusion)
                    {
                        // Create a new property member
                        // and the accessor type.
                        CodeMemberProperty tableProperty = new CodeMemberProperty();
                        tableProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                        // Add the region directive if at the beginning
                        if (propertyCount == 0)
                        {
                            // Create a custom region.
                            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Context Extension Properties");
                            tableProperty.StartDirectives.Add(startRegion);

                            // Increment the count.
                            propertyCount++;
                        }

                        // Assign the name and get and set indictors.
                        tableProperty.Name = view.TableName;
                        tableProperty.HasGet = true;

                        // Add the comments to the property.
                        tableProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                        tableProperty.Comments.Add(new CodeCommentStatement("Gets, the " + view.TableName.ToLower() + " replica property for the object.", true));
                        tableProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                        // Assign the return type.
                        tableProperty.Type = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + view.TableName);

                        //// Create a new code condition statement.
                        //CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                        //    new CodeVariableReferenceExpression("(String.IsNullOrEmpty(_specificPath))"),
                        //    new CodeStatement[] { 
                        //    new CodeExpressionStatement(
                        //        new CodeSnippetExpression("return new " + 
                        //            (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + view.TableName + "()")) },
                        //    new CodeStatement[] { 
                        //    new CodeExpressionStatement(
                        //        new CodeSnippetExpression("return new " + 
                        //            (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + view.TableName + "(_specificPath)")) });

                        CodeExpressionStatement conditionalStatement = new CodeExpressionStatement(
                                new CodeSnippetExpression("return new " +
                                    (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Replica." + view.TableName + "()"));

                        // Add the condition statement.
                        tableProperty.GetStatements.Add(conditionalStatement);

                        // Assign each property until the end.
                        endProperty = tableProperty;

                        // Add the property to the class.
                        _targetClass.Members.Add(tableProperty);
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

        /// <summary>
        /// Create the namespace and import namespaces.
        /// </summary>
        private void InitialiseNamespaceExtended()
        {
            // Create a new namespace.
            samplesExtended = new CodeNamespace(_companyName + ".DataAccess." + _dataBase + ".LinqToSql" +
                (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : ""));

            // Add each namespace reference.
            samplesExtended.Imports.Add(new CodeNamespaceImport("System"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Text"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Threading"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data.OleDb"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data.Odbc"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Collections"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Linq"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Linq.Expressions"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data.Linq"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data.Linq.SqlClient"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("System.Data.Linq.Mapping"));

            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataType"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Linq"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Control"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Custom"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.LinqToSql"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.DataSet"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Edm"));
            samplesExtended.Imports.Add(new CodeNamespaceImport("Nequeo.ComponentModel"));
        }

        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClassesExtended()
        {
            // Create the class and add base inheritance type.
            _targetContextClass = new CodeTypeDeclaration(_dataContextName);
            _targetContextClass.IsClass = true;
            _targetContextClass.IsPartial = true;
            _targetContextClass.TypeAttributes = TypeAttributes.Public;

            // Create the comments on the class.
            _targetContextClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetContextClass.Comments.Add(new CodeCommentStatement("The data access context object class.", true));
            _targetContextClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the class members.
            AddMembersExtended();

            // Add the class to the namespace
            // and add the namespace to the unit.
            samplesExtended.Types.Add(_targetContextClass);
        }

        /// <summary>
        /// Add the class members.
        /// </summary>
        private void AddMembersExtended()
        {
            AddPropertiesExtended();
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddPropertiesExtended()
        {
            // Create a new property member
            // and the accessor type.
            CodeMemberProperty tableProperty = new CodeMemberProperty();
            tableProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            // Assign the name and get and set indictors.
            tableProperty.Name = _contextName;
            tableProperty.HasGet = true;

            // Add the comments to the property.
            tableProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
            tableProperty.Comments.Add(new CodeCommentStatement("Gets, the " + _contextName.ToLower() + " class.", true));
            tableProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Assign the return type.
            // Add the condition statement.
            tableProperty.Type = new CodeTypeReference(
                (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Extension." + _contextName);
            tableProperty.GetStatements.Add(
                new CodeExpressionStatement(
                    new CodeSnippetExpression("return new " +
                        (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Extension." + _contextName + "()")));

            // Add the property to the class.
            _targetContextClass.Members.Add(tableProperty);
        }
    }
}
