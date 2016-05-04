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
    /// Function code generator.
    /// </summary>
    public class CodeDomFunctionTableObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeNamespace samplesExtended;
        private CodeTypeDeclaration _targetClass;
        private CodeTypeDeclaration _resultClass;
        private string _contextName = "DataContext";
        private string _dataBase = "Database";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _databaseConnection = "Connection";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private bool _functionHandler = true;
        private bool _procedureListExclusion = true;
        private string _dataAccessProvider = "";
        private string _procedureName = "Procedure";
        private string _procedureOwner = "Owner";
        private string _packageName = "package";
        private string _dataBaseConnect = "username/password";
        private string _dataBaseOwner = "owner";
        private FunctionExtensionContainer _data = null;
        private DataObjectContainer _dataObject = null;

        IEnumerable<FunctionResult> _procedures = null;
        IEnumerable<FunctionColumnsResult> _columns = null;

        /// <summary>
        /// Generates the procedure objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public CodeCompileUnit GenerateCode(FunctionExtensionContainer data)
        {
            _dataBase = data.Database;
            _companyName = data.NamespaceCompanyName;
            _databaseConnection = data.DatabaseConnection;
            _connectionType = data.ConnectionType;
            _connectionDataType = data.ConnectionDataType;
            _contextName = data.ExtensionClassName;
            _procedureListExclusion = data.FunctionListExclusion;
            _functionHandler = data.FunctionHandler;
            _dataBaseConnect = data.DataBaseConnect;
            _dataBaseOwner = data.DataBaseOwner;
            _extendedName = data.NamespaceExtendedName;
            _dataAccessProvider = data.DataAccessProvider;
            _data = ConvertToUpperCase(data);

            // Create the namespace.
            InitialiseNamespace();
            InitialiseNamespaceExtended();

            // Get the database procedures.
            if (GetDatabaseFunctions())
                // Add the classes.
                AddClasses();

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
        private FunctionExtensionContainer ConvertToUpperCase(FunctionExtensionContainer data)
        {
            FunctionExtensionContainer items = data;
            for (int i = 0; i < items.FunctionList.Count(); i++)
                items.FunctionList[i] = items.FunctionList[i].ToUpper();

            return items;
        }

        /// <summary>
        /// Gets the functions from the database.
        /// </summary>
        private bool GetDatabaseFunctions()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<FunctionResult> list = access.GetFunctions(_databaseConnection, null,
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner, 0);

            if (list != null && list.Count > 0)
            {
                _procedures = list.Distinct(new UniqueFunctionOverloadNameComparer());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the function columns from the database.
        /// </summary>
        /// <param name="procedureName">The procedure name.</param>
        /// <param name="owner">The schema (owner) of the object.</param>
        private bool GetDatabaseFunctionColumns(string procedureName, string owner, string overloadName)
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<FunctionColumnsResult> list = access.GetDatabaseFunctionColumns(
                _databaseConnection, null, procedureName, owner,
                Common.ConnectionProvider.GetConnectionType(_connectionType),
                null, _dataBaseOwner, overloadName, 0);

            if (list != null && list.Count > 0)
            {
                _columns = list.Distinct(new UniqueFunctionColumnNameComparer());
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

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + _contextName.ToLower() + " object class.", true));
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
            if (!_functionHandler)
                AddMethods();
            else
                AddMethodsFunctionHandler();
        }

        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        private void AddMethodsFunctionHandler()
        {
            int propertyCount = 0;
            CodeMemberMethod endProperty = null;

            foreach (var procedure in _procedures)
            {
                if (_data.FunctionList.Contains(procedure.FunctionName.ToUpper(), new ToUpperComparer()) == !_procedureListExclusion)
                {
                    // Declaring a create data method
                    CodeMemberMethod createProcedureMethod = new CodeMemberMethod();
                    createProcedureMethod.Attributes = MemberAttributes.Public;
                    createProcedureMethod.Name = ((procedure.FunctionRealName != null) ? (procedure.FunctionRealName) : (procedure.FunctionName));

                    // Add the region directive if at the beginning
                    if (propertyCount == 0)
                    {
                        // Create a custom region.
                        CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, "Public Function Methods");
                        createProcedureMethod.StartDirectives.Add(startRegion);

                        // Increment the count.
                        propertyCount++;
                    }

                    // Add the comments to the property.
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("Execute the " + procedure.FunctionName.ToLower() + " routine.", true));
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                    createProcedureMethod.CustomAttributes.Add(new CodeAttributeDeclaration("FunctionRoutineAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("\"" +
                            (!String.IsNullOrEmpty(procedure.FunctionOwner) ?
                                procedure.FunctionOwner + (!String.IsNullOrEmpty(procedure.PackageName) ? 
                                    "." + procedure.PackageName : "") + "." + procedure.FunctionName : procedure.FunctionName) + 
                                    "\", FunctionRoutineType.TableFunction"))));

                    bool ret = GetDatabaseFunctionColumns(procedure.FunctionName, procedure.FunctionOwner,
                        (!String.IsNullOrEmpty(procedure.OverloadName) ? procedure.OverloadName : ""));

                    // Start a new query string.
                    string query = "IExecuteFunctionResult result = Common.ExecuteFunction(Common, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ";
                    List<int> paramCount = new List<int>();
                    int i = -1;

                    // if a procedure column exist.
                    if (ret)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                            if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                if (!column.IsOutParameter)
                                {
                                    i++;
                                    paramCount.Add(i);
                                    query += column.ColumnName.Replace("@", "").ToLowerFirstLetter() + ", ";
                                }

                        foreach (var column in _columns)
                            if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                if (column.IsOutParameter)
                                {
                                    i++;
                                    paramCount.Add(i);
                                    query += column.ColumnName.Replace("@", "").ToLowerFirstLetter() + ", ";
                                }
                    }

                    query = query.TrimEnd(' ', ',') + ")";

                    // Add the query string parameters
                    createProcedureMethod.Statements.Add(new CodeSnippetExpression(query));

                    string parameterList = string.Empty;
                    _procedureName = procedure.FunctionName;
                    _procedureOwner = procedure.FunctionOwner;
                    _packageName = (procedure.PackageName != null) ? procedure.PackageName : "";

                    // if a procedure column exist.
                    if (ret)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (!column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    // If the table column is nullable and
                                    // the data type is not a reference type
                                    // then apply the nullable generic base.
                                    if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Nullable<" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)) + ">", column.ColumnName.Replace("@", "").ToLowerFirstLetter()));

                                        createProcedureMethod.Parameters[createProcedureMethod.Parameters.Count - 1].CustomAttributes.Add(
                                            new CodeAttributeDeclaration("FunctionParameterAttribute",
                                                new CodeAttributeArgument(new CodeSnippetExpression("\"" + column.ColumnName + "\", \"" + column.ColumnType + "\", " +
                                                    ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                        column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                                                            (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) +
                                                            ", " + ((column.IsOutParameter == true) ? "ParameterDirection.Output" : "ParameterDirection.Input") + ", " + 
                                                            column.ColumnNullable.ToString().ToLower()))));

                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                    else
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)),
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter()));

                                        createProcedureMethod.Parameters[createProcedureMethod.Parameters.Count - 1].CustomAttributes.Add(
                                            new CodeAttributeDeclaration("FunctionParameterAttribute",
                                                new CodeAttributeArgument(new CodeSnippetExpression("\"" + column.ColumnName + "\", \"" + column.ColumnType + "\", " +
                                                    ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                        column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                                                            (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) +
                                                            ", " + ((column.IsOutParameter == true) ? "ParameterDirection.Output" : "ParameterDirection.Input") + ", " + 
                                                            column.ColumnNullable.ToString().ToLower()))));

                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                }
                            }
                        }

                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    // If the table column is nullable and
                                    // the data type is not a reference type
                                    // then apply the nullable generic base.
                                    if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression("ref System.Nullable<" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)) + ">", column.ColumnName.Replace("@", "").ToLowerFirstLetter()));

                                        createProcedureMethod.Parameters[createProcedureMethod.Parameters.Count - 1].CustomAttributes.Add(
                                            new CodeAttributeDeclaration("FunctionParameterAttribute",
                                                new CodeAttributeArgument(new CodeSnippetExpression("\"" + column.ColumnName + "\", \"" + column.ColumnType + "\", " +
                                                ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                    column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                                                        (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) +
                                                        ", " + ((column.IsOutParameter == true) ? "ParameterDirection.Output" : "ParameterDirection.Input") + ", " + 
                                                        column.ColumnNullable.ToString().ToLower()))));

                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                    else
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression("ref " +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)),
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter()));

                                        createProcedureMethod.Parameters[createProcedureMethod.Parameters.Count - 1].CustomAttributes.Add(
                                            new CodeAttributeDeclaration("FunctionParameterAttribute",
                                                new CodeAttributeArgument(new CodeSnippetExpression("\"" + column.ColumnName + "\", \"" + column.ColumnType + "\", " +
                                                ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                    column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                                                        (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) +
                                                        ", " + ((column.IsOutParameter == true) ? "ParameterDirection.Output" : "ParameterDirection.Input") + ", " + 
                                                        column.ColumnNullable.ToString().ToLower()))));

                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                }
                            }
                        }
                    }

                    // Return comments.
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("<returns>The execution result.</returns>", true));
                    i = -1;

                    // if a procedure column exist.
                    if (ret)
                    {
                        foreach (var column in _columns)
                            if (!column.IsOutParameter)
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                    i++;

                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    i++;

                                    // If the table column is nullable and
                                    // the data type is not a reference type
                                    // then apply the nullable generic base.
                                    if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                    {
                                        createProcedureMethod.Statements.Add(new CodeSnippetExpression(
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + " = ((System.Nullable<" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">)" +
                                            "(result.GetParameterValue(" + paramCount[i].ToString() + ")))"));
                                    }
                                    else
                                    {
                                        createProcedureMethod.Statements.Add(new CodeSnippetExpression(
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + " = ((" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ")" +
                                            "(result.GetParameterValue(" + paramCount[i].ToString() + ")))"));
                                    }
                                }
                            }
                        }
                    }

                    // Get the real name of the routine.
                    string functionRealNameResult = ((procedure.FunctionRealName != null) ? (procedure.FunctionRealName) : (procedure.FunctionName));
                    functionRealNameResult = (!String.IsNullOrEmpty(procedure.OverloadName) ? 
                            procedure.OverloadName.Replace("_", "") : functionRealNameResult);

                    if (GetTableSchema(functionRealNameResult + "Result", ret))
                    {
                        createProcedureMethod.ReturnType = new CodeTypeReference("List<" +
                            (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") +
                            ".Extended." + functionRealNameResult + "Result" + ">");
                       
                        // Assign each entity property
                        // to from the parameter.
                        createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                            new CodeArgumentReferenceExpression(
                                "((List<" + (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + 
                                ".Extended." + functionRealNameResult + "Result>)(result.ReturnValue))")));
                    }
                    else
                    {
                        createProcedureMethod.ReturnType = new CodeTypeReference(typeof(System.Object));

                        // Assign each entity property
                        // to from the parameter.
                        createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                            new CodeArgumentReferenceExpression("((object)(result.ReturnValue))")));
                    }

                    // Assign each property until the end.
                    endProperty = createProcedureMethod;

                    // Add the method to the class.
                    _targetClass.Members.Add(createProcedureMethod);
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
        /// Add the methods to the class.
        /// </summary>
        private void AddMethods()
        {
            foreach (var procedure in _procedures)
            {
                if (_data.FunctionList.Contains(procedure.FunctionName.ToUpper(), new ToUpperComparer()) == !_procedureListExclusion)
                {
                    // Declaring a create data method
                    CodeMemberMethod createProcedureMethod = new CodeMemberMethod();
                    createProcedureMethod.Attributes = MemberAttributes.Public;
                    createProcedureMethod.Name = ((procedure.FunctionRealName != null) ? (procedure.FunctionRealName) : (procedure.FunctionName));

                    // Add the comments to the property.
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("Execute the " + procedure.FunctionName.ToLower() + " routine.", true));
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                    createProcedureMethod.CustomAttributes.Add(new CodeAttributeDeclaration("FunctionAttribute",
                        new CodeAttributeArgument(
                            new CodeSnippetExpression("Name = \"" + procedure.FunctionOwner + (procedure.PackageName != null ? "." + 
                                procedure.PackageName : "") + "." + procedure.FunctionName + "\", IsComposable = true"))));

                    // Add the data table expression.
                    createProcedureMethod.Statements.Add(new CodeSnippetExpression("DataTable dataTable = null"));

                    bool ret = GetDatabaseFunctionColumns(procedure.FunctionName, procedure.FunctionOwner, 
                        (!String.IsNullOrEmpty(procedure.OverloadName) ? procedure.OverloadName : ""));

                    string parameterList = string.Empty;
                    _procedureName = procedure.FunctionName;
                    _procedureOwner = procedure.FunctionOwner;
                    _packageName = (procedure.PackageName != null) ? procedure.PackageName : "";

                    // if a procedure column exist.
                    if (ret)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (!column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    // If the table column is nullable and
                                    // the data type is not a reference type
                                    // then apply the nullable generic base.
                                    if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Nullable<" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)) + ">", column.ColumnName.Replace("@", "").ToLowerFirstLetter()));
                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" + 
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                    else
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)), column.ColumnName.Replace("@", "").ToLowerFirstLetter()));
                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" + 
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                }
                            }
                        }

                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    // If the table column is nullable and
                                    // the data type is not a reference type
                                    // then apply the nullable generic base.
                                    if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression("ref System.Nullable<" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">", column.ColumnName.Replace("@", "").ToLowerFirstLetter()));
                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" + 
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Return value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                    else
                                    {
                                        createProcedureMethod.Parameters.Add(new CodeParameterDeclarationExpression("ref " +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString(), column.ColumnName.Replace("@", "").ToLowerFirstLetter()));
                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" + 
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Return value of " + column.ColumnName.Replace("@", "") + ".</param>", true));
                                        parameterList += column.ColumnName + ", ";
                                    }
                                }
                            }
                        }
                    }

                    // Return comments.
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("<returns>The execution result.</returns>", true));

                    // Start a new query string.
                    string query = string.Empty;

                    // Get the currrent connection type.
                    switch (Common.ConnectionProvider.GetConnectionType(_connectionType))
                    {
                        case ConnectionProvider.ConnectionType.SqlConnection:
                            query = "\r\n\t\t\tSystem.Data.Common.DbCommand ret = Common.ExecuteQuery(" +
                                "\r\n\t\t\t\tref dataTable, \"SELECT * FROM [" + procedure.FunctionOwner + "].[" + procedure.FunctionName + "](" + 
                                parameterList.TrimEnd(' ', ',') + ")\", CommandType.Text, true, " +
                                "\r\n\t\t\t\t\tnew SqlParameter(\"@RETURN_VALUE\", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, " +
                                "\r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", DataRowVersion.Current, null), ";

                            // if a procedure column exist.
                            if (ret)
                            {
                                // Iterate through the columns
                                foreach (var column in _columns)
                                {
                                    if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                    {
                                        query += "\r\n\t\t\t\t\tnew SqlParameter(\"" + column.ColumnName + "\", SqlDbType." +
                                            Common.LinqToDataTypes.GetSqlDbType(column.ColumnType) + ", " +
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                                                    (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) + ", ";

                                        if (column.IsOutParameter)
                                            query += "ParameterDirection.Output, true, \r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", " +
                                                "DataRowVersion.Current, null), ";
                                        else
                                            query += "ParameterDirection.Input, " + column.ColumnNullable.ToString().ToLower() + ", \r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", " +
                                                "DataRowVersion.Current, " + column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "), ";
                                    }
                                }
                            }

                            query = query.TrimEnd(' ', ',') + ")";
                            break;

                        case ConnectionProvider.ConnectionType.OleDbConnection:
                            query = "\r\n\t\t\tSystem.Data.Common.DbCommand ret = Common.ExecuteQuery(" +
                                "\r\n\t\t\t\tref dataTable, \"" + (!String.IsNullOrEmpty(procedure.FunctionOwner) ? (procedure.FunctionOwner + "." + 
                                procedure.FunctionName) : procedure.FunctionName) + "\", CommandType.StoredProcedure, true, " +
                                "\r\n\t\t\t\t\tnew OleDbParameter(\"@RETURN_VALUE\", OleDbType.Integer, 4, ParameterDirection.ReturnValue, false, " +
                                "\r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", DataRowVersion.Current, null), ";

                            // if a procedure column exist.
                            if (ret)
                            {
                                // Iterate through the columns
                                foreach (var column in _columns)
                                {
                                    if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                    {
                                        query += "\r\n\t\t\t\t\tnew OleDbParameter(\"" + column.ColumnName + "\", OleDbType." +
                                            Common.LinqToDataTypes.GetOleDbDbType(column.ColumnType) + ", " +
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? 
                                                    (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) + ", ";

                                        if (column.IsOutParameter)
                                            query += "ParameterDirection.Output, true, \r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", " +
                                                "DataRowVersion.Current, null), ";
                                        else
                                            query += "ParameterDirection.Input, " + column.ColumnNullable.ToString().ToLower() + ", \r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", " +
                                                "DataRowVersion.Current, " + column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "), ";
                                    }
                                }
                            }

                            query = query.TrimEnd(' ', ',') + ")";
                            break;

                        case ConnectionProvider.ConnectionType.OracleClientConnection:
                            query = "\r\n\t\t\tSystem.Data.Common.DbCommand ret = Common.ExecuteQuery(" +
                                "\r\n\t\t\t\tref dataTable, \"SELECT * FROM \"" + _procedureOwner + (_packageName == "" ? "" : "\".\"" + _packageName) + "\".\"" + _procedureName + "\"(" +
                                parameterList.TrimEnd(' ', ',') + ")\", CommandType.Text, true, " +
                                "\r\n\t\t\t\t\tnew Oracle.DataAccess.Client.OracleParameter(\"@RETURN_VALUE\", Oracle.DataAccess.Client.OracleDbType.Int32, 4, ParameterDirection.ReturnValue, false, " +
                                "\r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", DataRowVersion.Current, null), ";

                            // if a procedure column exist.
                            if (ret)
                            {
                                // Iterate through the columns
                                foreach (var column in _columns)
                                {
                                    if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                    {
                                        query += "\r\n\t\t\t\t\tnew Oracle.DataAccess.Client..OracleParameter(\"" + column.ColumnName + "\", Oracle.DataAccess.Client.OracleDbType." +
                                            Common.LinqToDataTypes.GetOracleClientDbType(column.ColumnType) + ", " +
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                column.Length : column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ?
                                                    (column.Length) : LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) + ", ";

                                        if (column.IsOutParameter)
                                            query += "ParameterDirection.Output, true, \r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", " +
                                                "DataRowVersion.Current, null), ";
                                        else
                                            query += "ParameterDirection.Input, " + column.ColumnNullable.ToString().ToLower() + ", \r\n\t\t\t\t\t\t((Byte)(0)), ((Byte)(0)), \"\", " +
                                                "DataRowVersion.Current, " + column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "), ";
                                    }
                                }
                            }

                            query = query.TrimEnd(' ', ',') + ")";
                            break;
                    }

                    // Add the query string parameters
                    createProcedureMethod.Statements.Add(new CodeSnippetExpression(query));

                    // if a procedure column exist.
                    if (ret)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    // If the table column is nullable and
                                    // the data type is not a reference type
                                    // then apply the nullable generic base.
                                    if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                                    {
                                        createProcedureMethod.Statements.Add(new CodeSnippetExpression("\r\n\t\t\t" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + " = ((System.Nullable<" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">)" +
                                            "(ret.Parameters[\"" + column.ColumnName + "\"].Value))"));
                                    }
                                    else
                                    {
                                        createProcedureMethod.Statements.Add(new CodeSnippetExpression("\r\n\t\t\t" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + " = ((" +
                                            Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ")" +
                                            "(ret.Parameters[\"" + column.ColumnName + "\"].Value))"));
                                    }
                                }
                            }
                        }
                    }

                    // Get the real name of the routine.
                    string functionRealNameResult = ((procedure.FunctionRealName != null) ? (procedure.FunctionRealName) : (procedure.FunctionName));
                    functionRealNameResult = (!String.IsNullOrEmpty(procedure.OverloadName) ? 
                            procedure.OverloadName.Replace("_", "") : functionRealNameResult);

                    if (GetTableSchema(functionRealNameResult + "Result", ret))
                    {
                        createProcedureMethod.ReturnType = new CodeTypeReference("List<" +
                            (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") +
                            ".Extended." + functionRealNameResult + "Result" + ">");

                        createProcedureMethod.Statements.Add(new CodeSnippetExpression(
                            "\r\n\t\t\tNequeo.Data.Access.Control.AnonymousTypeFunction typeConversion = \r\n\t\t\t\tnew " +
                            "Nequeo.Data.Access.Control.AnonymousTypeFunction()"));

                        createProcedureMethod.Statements.Add(new CodeSnippetExpression(
                            "\r\n\t\t\tList<" + (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + 
                            ".Extended." + functionRealNameResult + "Result" + "> access = \r\n\t\t\t\ttypeConversion.GetListCollection<" +
                            (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + ".Extended." +
                            functionRealNameResult + "Result" + ">(dataTable)"));

                        // Assign each entity property
                        // to from the parameter.
                        createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                            new CodeArgumentReferenceExpression("(access.Count() > 0) ? access : null")));
                    }
                    else
                    {
                        createProcedureMethod.ReturnType = new CodeTypeReference(typeof(System.Object));

                        // Assign each entity property
                        // to from the parameter.
                        createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                            new CodeArgumentReferenceExpression("((object)(ret.Parameters[0].Value))")));
                    }

                    // Add the method to the class.
                    _targetClass.Members.Add(createProcedureMethod);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containsPrameters"></param>
        /// <returns></returns>
        private bool GetTableSchema(string name, bool containsPrameters)
        {
            DatabaseAccess access = new DatabaseAccess();
            System.Data.DataTable dataTable = null;
            string parameterList = string.Empty;

            // Get the currrent connection type.
            switch (Common.ConnectionProvider.GetConnectionType(_connectionType))
            {
                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    List<Npgsql.NpgsqlParameter> pgParameters = new List<Npgsql.NpgsqlParameter>();

                    if (containsPrameters)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    pgParameters.Add(new Npgsql.NpgsqlParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetPostgreSqlDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))), "",
                                                        System.Data.ParameterDirection.Output, true, ((Byte)(0)), ((Byte)(0)), 
                                                        System.Data.DataRowVersion.Current, null));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                            else
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    pgParameters.Add(new Npgsql.NpgsqlParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetPostgreSqlDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))), "",
                                                        System.Data.ParameterDirection.Input, true, ((Byte)(0)), ((Byte)(0)), 
                                                        System.Data.DataRowVersion.Current,
                                                        Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName)));

                                    // Add the parameter.
                                    //parameterList += column.ColumnName + ", ";
                                    parameterList += Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName) + ", ";
                                }
                            }
                        }
                    }

                    try
                    {
                        access.ExecutePostgreSqlFunctionQuerySchema(ref dataTable, "SELECT * FROM \"" + _procedureOwner + "\".\"" + _procedureName + "\"(" +
                            parameterList.TrimEnd(' ', ',') + ")", System.Data.CommandType.Text, _databaseConnection, pgParameters.ToArray());
                    }
                    catch { }

                    if (dataTable != null)
                        return CreateResultDataObject(dataTable, name);
                    else
                        return false;

                case ConnectionProvider.ConnectionType.SqlConnection:
                    List<System.Data.SqlClient.SqlParameter> parameters = new List<System.Data.SqlClient.SqlParameter>();

                    if (containsPrameters)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    parameters.Add(new System.Data.SqlClient.SqlParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetSqlDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ? 
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) : 
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Output, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current, null));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                            else
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    parameters.Add(new System.Data.SqlClient.SqlParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetSqlDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ? 
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) : 
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Input, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current,
                                                        Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName)));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                        }
                    }

                    try
                    {
                        access.ExecuteFunctionTableQuerySchema(ref dataTable, "SELECT * FROM [" + _procedureOwner + "].[" + _procedureName + "](" + 
                            parameterList.TrimEnd(' ', ',') + ")", System.Data.CommandType.Text, _databaseConnection, parameters.ToArray());
                    }
                    catch { }

                    if (dataTable != null)
                        return CreateResultDataObject(dataTable, name);
                    else
                        return false;

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    List<System.Data.OleDb.OleDbParameter> eParameters = new List<System.Data.OleDb.OleDbParameter>();

                    if (containsPrameters)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    eParameters.Add(new System.Data.OleDb.OleDbParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetOleDbDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ? 
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) : 
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Output, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current, null));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                            else
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    eParameters.Add(new System.Data.OleDb.OleDbParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetOleDbDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ? 
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) : 
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Input, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current,
                                                        Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName)));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                        }
                    }

                    try
                    {
                        access.ExecuteOleDbQuerySchema(ref dataTable, "" + (!String.IsNullOrEmpty(_procedureOwner) ? _procedureOwner + "." + _procedureName + "" : _procedureName),
                            System.Data.CommandType.StoredProcedure, _databaseConnection, eParameters.ToArray());
                    }
                    catch { }

                    if (dataTable != null)
                        return CreateResultDataObject(dataTable, name);
                    else
                        return false;

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    List<Oracle.ManagedDataAccess.Client.OracleParameter> oParametersClient = new List<Oracle.ManagedDataAccess.Client.OracleParameter>();

                    if (containsPrameters)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    oParametersClient.Add(new Oracle.ManagedDataAccess.Client.OracleParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetOracleClientDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Output, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current, null));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                            else
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    oParametersClient.Add(new Oracle.ManagedDataAccess.Client.OracleParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetOracleClientDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Input, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current,
                                                        Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName)));

                                    // Add the parameter.
                                    //parameterList += column.ColumnName + ", ";
                                    parameterList += Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName) + ", ";
                                }
                            }
                        }
                    }

                    try
                    {
                        access.ExecuteOracleClientFunctionQuerySchema(ref dataTable, "SELECT * FROM TABLE(\"" + _procedureOwner + (_packageName == "" ? "" : "\".\"" + _packageName) + "\".\"" + _procedureName + "\"(" +
                            parameterList.TrimEnd(' ', ',') + "))", System.Data.CommandType.Text, _databaseConnection, oParametersClient.ToArray());
                    }
                    catch { }

                    if (dataTable != null)
                        return CreateResultDataObject(dataTable, name);
                    else
                        return false;

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    List<MySql.Data.MySqlClient.MySqlParameter> myParameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                    if (containsPrameters)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (column.IsOutParameter)
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    myParameters.Add(new MySql.Data.MySqlClient.MySqlParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetMySqlDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Output, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current, null));

                                    // Add the parameter.
                                    parameterList += column.ColumnName + ", ";
                                }
                            }
                            else
                            {
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                {
                                    myParameters.Add(new MySql.Data.MySqlClient.MySqlParameter(column.ColumnName,
                                        Common.LinqToDataTypes.GetMySqlDbType(column.ColumnType),
                                            ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                                Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                    ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                        Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))),
                                                        System.Data.ParameterDirection.Input, true, ((Byte)(0)), ((Byte)(0)), "",
                                                        System.Data.DataRowVersion.Current,
                                                        Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName)));

                                    // Add the parameter.
                                    //parameterList += column.ColumnName + ", ";
                                    parameterList += Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName) + ", ";
                                }
                            }
                        }
                    }

                    try
                    {
                        access.ExecuteMySqlFunctionQuerySchema(ref dataTable, "SELECT * FROM \"" + _procedureOwner + "\".\"" + _procedureName + "\"(" +
                            parameterList.TrimEnd(' ', ',') + ")", System.Data.CommandType.Text, _databaseConnection, myParameters.ToArray());
                    }
                    catch { }

                    if (dataTable != null)
                        return CreateResultDataObject(dataTable, name);
                    else
                        return false;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CreateResultDataObject(System.Data.DataTable dataTable, string name)
        {
            if (dataTable != null)
            {
                if (dataTable.Columns.Count > 0)
                {
                    // Create the class and add base inheritance type.
                    _resultClass = new CodeTypeDeclaration(name);
                    _resultClass.IsClass = true;
                    _resultClass.IsPartial = true;
                    _resultClass.TypeAttributes = TypeAttributes.Public;
                    _resultClass.BaseTypes.Add(new CodeTypeReference("DataBase"));

                    // Create a custom region.
                    CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, name + " Data Entity Type");
                    _resultClass.StartDirectives.Add(startRegion);

                    // Create the attributes on the class.
                    _resultClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + name + "\", IsReference = true"))));
                    _resultClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));
                    _resultClass.CustomAttributes.Add(new CodeAttributeDeclaration("KnownTypeAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("typeof(DataBase)"))));

                    // Create the comments on the class.
                    _resultClass.Comments.Add(new CodeCommentStatement("<summary>", true));
                    _resultClass.Comments.Add(new CodeCommentStatement("The " + name.ToLower() + " data object class.", true));
                    _resultClass.Comments.Add(new CodeCommentStatement("</summary>", true));


                    _dataObject = new DataObjectContainer();
                    _dataObject.ClassName = name;
                    _dataObject.Database = _dataBase;
                    _dataObject.NamespaceCompanyName = _companyName;
                    _dataObject.PropertyIsNullable = new bool[dataTable.Columns.Count];
                    _dataObject.PropertyName = new string[dataTable.Columns.Count];
                    _dataObject.PropertyType = new string[dataTable.Columns.Count];

                    int i = -1;
                    foreach (System.Data.DataColumn column in dataTable.Columns)
                    {
                        i++;
                        _dataObject.PropertyName[i] = column.ColumnName;
                        _dataObject.PropertyType[i] = column.DataType.FullName;
                        _dataObject.PropertyIsNullable[i] = column.AllowDBNull;
                    }

                    // Add the class members.
                    AddMemberResults(name);

                    // Create a custom endregion.
                    CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                    _resultClass.EndDirectives.Add(endRegion);

                    // Add the class to the namespace
                    // and add the namespace to the unit.
                    samplesExtended.Types.Add(_resultClass);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add the class members.
        /// </summary>
        private void AddMemberResults(string name)
        {
            AddConstructor();
            AddExtensibilityMethods();
            AddFields();
            AddProperties();
            AddMethodsMore(name);
        }

        /// <summary>
        /// Create the namespace and import namespaces.
        /// </summary>
        private void InitialiseNamespaceExtended()
        {
            // Create a new namespace.
            samplesExtended = new CodeNamespace(_companyName + ".DataAccess." + _dataBase + ".Data" +
                (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : "") + ".Extended");

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
        /// Add the constrctor to the class.
        /// </summary>
        private void AddConstructor()
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
            _resultClass.Members.Add(constructor);
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
            _resultClass.Members.Add(onCreateMethod);
            _resultClass.Members.Add(onLoadMethod);
            _resultClass.Members.Add(endRegion);
        }

        /// <summary>
        /// Add the fields to the class.
        /// </summary>
        private void AddFields()
        {
            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _dataObject.PropertyName.Count(); i++)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Private;
                valueField.Name = "_" + _dataObject.PropertyName[i];

                if (_dataObject.PropertyDefaultValue != null)
                    valueField.InitExpression = new CodeSnippetExpression(_dataObject.PropertyDefaultValue[i]);

                bool isNullable = false;
                if (_dataObject.PropertyIsNullable != null)
                    isNullable = _dataObject.PropertyIsNullable[i];

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).IsArray) &&
                    !Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).IsClass))
                {
                    // Assign the data type for the field if
                    // the data type is not a reference type
                    // then create a nullable type field.
                    valueField.Type = new CodeTypeReference(
                        "System.Nullable<" + Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).ToString() + ">");
                }
                else
                    // Assign the field type for the field.
                    // Get the data type of the field from
                    // the sql data type.
                    valueField.Type = new CodeTypeReference(_dataObject.PropertyType[i]);

                // Add the field to the class.
                _resultClass.Members.Add(valueField);
            }
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties()
        {
            int propertyCount = 0;
            CodeMemberProperty endProperty = null;

            // For each column found in the table
            // iterate through the list and create
            // each property.
            for (int i = 0; i < _dataObject.PropertyName.Count(); i++)
            {
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
                valueProperty.Name = _dataObject.PropertyName[i];
                valueProperty.HasGet = true;
                valueProperty.HasSet = true;

                // Add the comments to the property.
                valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + _dataObject.PropertyName[i].ToLower() + " property for the object.", true));
                valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                bool isNullable = false;
                if (_dataObject.PropertyIsNullable != null)
                    isNullable = _dataObject.PropertyIsNullable[i];

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).IsArray) &&
                    !Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).IsClass))
                {
                    // Assign the data type for the property if
                    // the data type is not a reference type
                    // then create a nullable type property.
                    valueProperty.Type = new CodeTypeReference(
                        "System.Nullable<" + Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).ToString() + ">");
                }
                else
                    // Assign the property type for the property.
                    // Get the data type of the property from
                    // the sql data type.
                    valueProperty.Type = new CodeTypeReference(_dataObject.PropertyType[i]);

                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "_" + _dataObject.PropertyName[i])));

                // Create a new code condition statement.
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    new CodeVariableReferenceExpression("(this." + "_" + _dataObject.PropertyName[i] + " != value)"),
                    new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this." + "_" + _dataObject.PropertyName[i] + " = value")) });

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(conditionalStatement);

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CategoryAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("\"Column\""))));

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DescriptionAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("\"Gets sets, the " + _dataObject.PropertyName[i].ToLower() + " property for the object.\""))));

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + _dataObject.PropertyName[i] + "\""))));

                // If the type is an array the add the 
                // array attribute else add the element attribute.
                if ((Common.LinqToDataTypes.GetSystemType(_dataObject.PropertyType[i]).IsArray) ||
                    (_dataObject.PropertyType[i].EndsWith("[]")))
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + _dataObject.PropertyName[i] + "\", IsNullable = " + isNullable.ToString().ToLower()))));
                else
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + _dataObject.PropertyName[i] + "\", IsNullable = " + isNullable.ToString().ToLower()))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("SoapElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("IsNullable = " + isNullable.ToString().ToLower()))));

                // Assign each property until the end.
                endProperty = valueProperty;

                // Add the property to the class.
                _resultClass.Members.Add(valueProperty);
            }

            if (endProperty != null)
            {
                // Create a custom endregion.
                CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                endProperty.EndDirectives.Add(endRegion);
            }
        }

        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        private void AddMethodsMore(string name)
        {
            // Declaring a create data method
            CodeMemberMethod createObjectMethodData = new CodeMemberMethod();
            createObjectMethodData.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            createObjectMethodData.Name = "Create" + name;
            createObjectMethodData.ReturnType = new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + ".Extended." + name);

            // Add the comments to the property.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<summary>", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("Create a new " + name.ToLower() + " data entity.", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("</summary>", true));

            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _dataObject.PropertyName.Count(); i++)
            {
                // If the column is not null.
                if (!_dataObject.PropertyIsNullable[i])
                {
                    // Add each parameter.
                    createObjectMethodData.Parameters.Add(
                        new CodeParameterDeclarationExpression(_dataObject.PropertyType[i], _dataObject.PropertyName[i].ToLowerFirstLetter()));
                    createObjectMethodData.Comments.Add(
                        new CodeCommentStatement("<param name=\"" + _dataObject.PropertyName[i].ToLowerFirstLetter() + "\">Initial value of " + 
                            _dataObject.PropertyName[i] + ".</param>", true));
                }
            }

            // Return comments.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<returns>The " + 
                (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + ".Extended." + name + " entity.</returns>", true));

            // Add the create code statement.
            createObjectMethodData.Statements.Add(new CodeSnippetExpression((!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + 
                ".Extended." + name + " " + name.ToLowerFirstLetter() + " = new " +
                (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName : "Data") + ".Extended." + name + "()"));

            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _dataObject.PropertyName.Count(); i++)
            {
                // If the column is not null.
                if (!_dataObject.PropertyIsNullable[i])
                {
                    // Add each parameter assignment.
                    createObjectMethodData.Statements.Add(
                        new CodeSnippetExpression(name.ToLowerFirstLetter() + "." + _dataObject.PropertyName[i] + " = " + 
                            _dataObject.PropertyName[i].ToLowerFirstLetter()));
                }
            }

            // Assign each entity property
            // to from the parameter.
            createObjectMethodData.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeArgumentReferenceExpression(name.ToLowerFirstLetter())));

            // Add the property to the class.
            _resultClass.Members.Add(createObjectMethodData);
        }
    }
}
