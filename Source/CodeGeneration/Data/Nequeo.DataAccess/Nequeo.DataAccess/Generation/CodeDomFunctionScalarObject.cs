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
    public class CodeDomFunctionScalarObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
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
        private System.Data.DataTable _dataTable = null;
        private FunctionExtensionContainer _data = null;

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

            // Get the database procedures.
            if (GetDatabaseFunctions())
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
                Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner, 1);

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
                null, _dataBaseOwner, overloadName, 1);

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
            if(!_functionHandler)
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
                            procedure.FunctionOwner + (!String.IsNullOrEmpty(procedure.PackageName) ? "." + procedure.PackageName : "") + "." + 
                            procedure.FunctionName : procedure.FunctionName) + 
                            "\", FunctionRoutineType.ScalarFunction"))));

                    bool ret = GetDatabaseFunctionColumns(procedure.FunctionName, procedure.FunctionOwner,
                        (!String.IsNullOrEmpty(procedure.OverloadName) ? procedure.OverloadName : ""));

                    // Start a new query string.
                    string query = "IExecuteFunctionResult result = Common.ExecuteFunction(Common, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ";

                    // if a procedure column exist.
                    if (ret)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                            if (!column.IsOutParameter)
                                if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                                    query += column.ColumnName.Replace("@", "").ToLowerFirstLetter() + ", ";
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
                                                    ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ? column.Length :
                                                        column.Precision) : ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? (column.Length) : 
                                                            LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType)))) +
                                                                ", " + ((column.IsOutParameter == true) ? "ParameterDirection.Output" : "ParameterDirection.Input") + ", " + 
                                                                column.ColumnNullable.ToString().ToLower()))));
 
                                        createProcedureMethod.Comments.Add(new CodeCommentStatement("<param name=\"" +
                                            column.ColumnName.Replace("@", "").ToLowerFirstLetter() + "\">Initial value of " + 
                                            column.ColumnName.Replace("@", "") + ".</param>", true));
                                            parameterList += column.ColumnName + ", ";
                                    }
                                }
                            }
                        }
                    }

                    // Return comments.
                    createProcedureMethod.Comments.Add(new CodeCommentStatement("<returns>The execution result.</returns>", true));

                    // Iterate through the columns
                    foreach (var column in _columns)
                    {
                        if (column.IsOutParameter)
                        {
                            // If the table column is nullable and
                            // the data type is not a reference type
                            // then apply the nullable generic base.
                            if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                            {
                                // Assign the current return type.
                                createProcedureMethod.ReturnType = new CodeTypeReference("System.Nullable<" +
                                    Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">");

                                // Assign each entity property
                                // to from the parameter.
                                createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                                    new CodeArgumentReferenceExpression("((System.Nullable<" +
                                        Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">)" +
                                        "(result.ReturnValue))")));
                            }
                            else
                            {
                                // Assign the current return type.
                                createProcedureMethod.ReturnType = new CodeTypeReference(
                                    Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString());

                                // Assign each entity property
                                // to from the parameter.
                                createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                                    new CodeArgumentReferenceExpression("((" +
                                        Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ")" +
                                        "(result.ReturnValue))")));
                            }
                        }
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
                        new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + 
                            procedure.FunctionOwner + (procedure.PackageName != null ? "." + procedure.PackageName : "") + "." + 
                            procedure.FunctionName + "\", IsComposable = true"))));

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
                                "\r\n\t\t\t\tref dataTable, \"SELECT [" + procedure.FunctionOwner + "].[" + procedure.FunctionName + "](" + 
                                parameterList.TrimEnd(' ', ',') + ") AS 'FuncResult'\", CommandType.Text, true, " +
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
                                "\r\n\t\t\t\tref dataTable, \"SELECT \"" + _procedureOwner + (_packageName == "" ? "" : "\".\"" + _packageName) + "\".\"" + _procedureName + "\"(" +
                                parameterList.TrimEnd(' ', ',') + ") AS \"FuncResult\" FROM DUAL, CommandType.Text, true, " +
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
                                        query += "\r\n\t\t\t\t\tnew Oracle.DataAccess.Client.OracleParameter(\"" + column.ColumnName + "\", Oracle.DataAccess.Client.OracleDbType." +
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

                    // Iterate through the columns
                    foreach (var column in _columns)
                    {
                        if (column.IsOutParameter)
                        {
                            // If the table column is nullable and
                            // the data type is not a reference type
                            // then apply the nullable generic base.
                            if (column.ColumnNullable && Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)))
                            {
                                // Assign the current return type.
                                createProcedureMethod.ReturnType = new CodeTypeReference("System.Nullable<" +
                                    Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">");

                                // Assign each entity property
                                // to from the parameter.
                                createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                                    new CodeArgumentReferenceExpression("((System.Nullable<" +
                                        Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ">)" +
                                        "(dataTable.Rows[0][\"FuncResult\"]))")));
                            }
                            else
                            {
                                // Assign the current return type.
                                createProcedureMethod.ReturnType = new CodeTypeReference(
                                    Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString());

                                // Assign each entity property
                                // to from the parameter.
                                createProcedureMethod.Statements.Add(new CodeMethodReturnStatement(
                                    new CodeArgumentReferenceExpression("((" +
                                        Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).ToString() + ")" +
                                        "(dataTable.Rows[0][\"FuncResult\"]))")));
                            }
                        }
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
        /// <param name="overloadName"></param>
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
                    List<Npgsql.NpgsqlParameter> parametersPg = new List<Npgsql.NpgsqlParameter>();

                    if (containsPrameters)
                    {
                        // Iterate through the columns
                        foreach (var column in _columns)
                        {
                            if (LinqToDataTypes.ValidateColumnName(column.ColumnName, Common.ConnectionProvider.GetConnectionType(_connectionType)))
                            {
                                parametersPg.Add(new Npgsql.NpgsqlParameter(column.ColumnName,
                                    Common.LinqToDataTypes.GetPostgreSqlDbType(column.ColumnType),
                                        ((column.Precision != null && column.Precision > 0) ? (((column.Length <= column.Precision)) ?
                                            Convert.ToInt32(column.Length) : Convert.ToInt32(column.Precision)) :
                                                ((Common.LinqToDataTypes.GetLinqNullableType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType))) ? Convert.ToInt32(column.Length) :
                                                    Convert.ToInt32(LinqToDataTypes.DefaultLengthValue(ConnectionProvider.GetConnectionType(_connectionType))))), "",
                                                    System.Data.ParameterDirection.Input, true, ((Byte)(0)), ((Byte)(0)),
                                                    System.Data.DataRowVersion.Current,
                                                    Common.LinqToDataTypes.GetSqlStringValue(Common.LinqToDataTypes.GetLinqDataType(column.ColumnType, Common.ConnectionProvider.GetConnectionDataType(_connectionDataType)).FullName)));

                                // Add the parameter.
                                parameterList += "\"" + column.ColumnName + "\", ";
                            }
                        }
                    }

                    try
                    {
                        access.ExecutePostgreSqlFunctionQuerySchema(ref dataTable, "SELECT \"" + _procedureOwner + "\".\"" + _procedureName + "\"(" +
                            parameterList.TrimEnd(' ', ',') + ") AS \"FuncResult\"", System.Data.CommandType.Text, _databaseConnection, parametersPg.ToArray());
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

                    try
                    {
                        access.ExecuteFunctionTableQuerySchema(ref dataTable, "SELECT [" + _procedureOwner + "].[" + _procedureName + "](" +
                            parameterList.TrimEnd(' ', ',') + ") AS 'FuncResult'", System.Data.CommandType.Text, _databaseConnection, parameters.ToArray());
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

                    try
                    {
                        access.ExecuteOleDbQuerySchema(ref dataTable, "" + (!String.IsNullOrEmpty(_procedureOwner) ?_procedureOwner + "." + _procedureName + "" : _procedureName),
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

                    try
                    {
                        access.ExecuteOracleClientFunctionQuerySchema(ref dataTable, "SELECT \"" + _procedureOwner + (_packageName == "" ? "" : "\".\"" + _packageName) + "\".\"" + _procedureName + "\"(" +
                            parameterList.TrimEnd(' ', ',') + ") AS \"FuncResult\" FROM DUAL", System.Data.CommandType.Text, _databaseConnection, oParametersClient.ToArray());
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
                    _dataTable = dataTable;
                    return true;
                }
                return true;
            }
            return false;
        }
    }
}
