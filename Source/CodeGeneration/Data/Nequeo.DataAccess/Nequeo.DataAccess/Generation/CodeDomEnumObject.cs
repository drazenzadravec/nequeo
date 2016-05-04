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
    /// Enumeration code generator.
    /// </summary>
    public class CodeDomEnumObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
        private string _enumName = "Enum";
        private string _dataBase = "Database";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _databaseConnection = "Connection";
        private int _connectionType = 0;
        private int _connectionDataType = 0;
        private string _dataBaseConnect = "username/password";
        private string _dataBaseOwner = "owner";
        private EnumObjectContainer _data = null;

        IEnumerable<ColumnValuesResult> _columnValues = null;

        /// <summary>
        /// Generates the table objects.
        /// </summary>
        /// <param name="data">The data context.</param>
        /// <returns>The code unit</returns>
        public CodeCompileUnit GenerateCode(EnumObjectContainer data)
        {
            _enumName = data.EnumName;
            _dataBase = data.Database;
            _companyName = data.NamespaceCompanyName;
            _databaseConnection = data.DatabaseConnection;
            _connectionType = data.ConnectionType;
            _connectionDataType = data.ConnectionDataType;
            _dataBaseConnect = data.DataBaseConnect;
            _dataBaseOwner = data.DataBaseOwner;
            _extendedName = data.NamespaceExtendedName;
            _data = data;

            // Create the namespace.
            InitialiseNamespace();

            // Get the database tables.
            if (GetTableColumnValues())
                // Add the classes.
                AddClasses();

            // Return the complie unit.
            _targetUnit.Namespaces.Add(samples);
            return _targetUnit;
        }

        /// <summary>
        /// Gets the tables from the database.
        /// </summary>
        private bool GetTableColumnValues()
        {
            // Get the table results from the database
            // and apply the distinct method on the table
            // data only return unique column names results.
            DatabaseAccess access = new DatabaseAccess();
            List<ColumnValuesResult> list = null;
            if (String.IsNullOrEmpty(_data.DataFilter))
                list = access.GetColumnValue(_data.DatabaseConnection, null, 
                    _data.TableName, _data.ValueColumnName, _data.IndicatorColumnName,
                    Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);
            else
                list = access.GetColumnValue(_data.DatabaseConnection, null, 
                    _data.TableName, _data.ValueColumnName, _data.IndicatorColumnName, _data.DataFilter,
                    Common.ConnectionProvider.GetConnectionType(_connectionType), null, _dataBaseOwner);
            
            if (list != null && list.Count > 0)
            {
                _columnValues = list.Distinct(new UniqueColumnIndictaorComparer());
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
            samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBase + ".Enum" +
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
        }

        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            // Create the class and add base inheritance type.
            _targetClass = new CodeTypeDeclaration(_enumName);
            _targetClass.IsEnum = true;
            _targetClass.TypeAttributes = TypeAttributes.Public;
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute"));

            // Create a custom region.
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, _enumName + " Enum Type");
            _targetClass.StartDirectives.Add(startRegion);

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + "enumerator " + _enumName.ToLower() + " object.", true));
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
            AddEnum();
        }

        /// <summary>
        /// Add the enumerator to the class.
        /// </summary>
        private void AddEnum()
        {
            // For each column found in the table
            // iterate through the list and create
            // each field.
            foreach (var columnValue in _columnValues)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Final;
                valueField.Name = columnValue.ColumnIndicator.ToUpperFirstLetterInEachWord().
                    Replace(" ", "").Replace(".", "").Replace("/", "").Replace("\\", "").
                    Replace("&", "And").Replace("|", "").Replace(",", "").Replace(";", "").
                    Replace(":", "").Replace("-", "_");

                valueField.CustomAttributes.Add(new CodeAttributeDeclaration("EnumMemberAttribute"));
                valueField.CustomAttributes.Add(new CodeAttributeDeclaration("XmlEnumAttribute"));
                valueField.CustomAttributes.Add(new CodeAttributeDeclaration("SoapEnumAttribute"));
                valueField.InitExpression = new CodeSnippetExpression(columnValue.ColumnValue);

                // Add the comments to the property.
                valueField.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueField.Comments.Add(new CodeCommentStatement("The " + columnValue.ColumnIndicator + " enum value", true));
                valueField.Comments.Add(new CodeCommentStatement("</summary>", true));

                // Add the field to the class.
                _targetClass.Members.Add(valueField);
            }
        }
    }
}
