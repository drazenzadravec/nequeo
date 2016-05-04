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
    /// Linq object code generator.
    /// </summary>
    public class CodeDomLinqObject
    {
        private CodeCompileUnit _targetUnit;
        private CodeNamespace samples;
        private CodeTypeDeclaration _targetClass;
        private string _dataBaseName = "Database";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _className = "Class1";
        private LinqObjectContainer _data = null;

        /// <summary>
        /// Generate the code.
        /// </summary>
        /// <param name="data">The data collection.</param>
        /// <returns>The code unit.</returns>
        public CodeCompileUnit GenerateCode(LinqObjectContainer data)
        {
            // If not null.
            if (data != null)
            {
                _dataBaseName = data.Database;
                _companyName = data.NamespaceCompanyName;
                _className = data.ClassName;
                _extendedName = data.NamespaceExtendedName;

                // Create the namespace.
                InitialiseNamespace();
            }

            // If not null.
            if (data != null)
            {
                _data = data;

                // Add the classes.
                AddClasses();
            }

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
            samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBaseName + ".LinqToSql" + 
                (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : "") + ".Extended");

            // Add each namespace reference.
            samples.Imports.Add(new CodeNamespaceImport("System"));
            samples.Imports.Add(new CodeNamespaceImport("System.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.Linq"));
            samples.Imports.Add(new CodeNamespaceImport("System.Data.Linq.Mapping"));
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

            // Create a custom region.
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, _className + " LinqToSql Type");
            _targetClass.StartDirectives.Add(startRegion);

            // Create the attributes on the class.
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute",
                new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + _className + "\", IsReference = true"))));
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + _className.ToLower() + " LinqToSql object class.", true));
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
            AddConstructor();
            AddExtensibilityMethods();
            AddFields();
            AddProperties();
            AddMethodsMore();
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
            _targetClass.Members.Add(constructor);
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
        /// Add the fields to the class.
        /// </summary>
        private void AddFields()
        {
            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _data.PropertyName.Count(); i++)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Private;
                valueField.Name = ("_" + _data.PropertyName[i]).ReplaceKeyOperands();

                if (_data.PropertyDefaultValue != null)
                    valueField.InitExpression = new CodeSnippetExpression(_data.PropertyDefaultValue[i]);

                bool isNullable = false;
                if (_data.PropertyIsNullable != null)
                    isNullable = _data.PropertyIsNullable[i];

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).IsArray) &&
                    !Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).IsClass))
                {
                    // Assign the data type for the field if
                    // the data type is not a reference type
                    // then create a nullable type field.
                    valueField.Type = new CodeTypeReference(
                        "System.Nullable<" + Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).ToString() + ">");
                }
                else
                    // Assign the field type for the field.
                    // Get the data type of the field from
                    // the sql data type.
                    valueField.Type = new CodeTypeReference(_data.PropertyType[i]);

                // Add the field to the class.
                _targetClass.Members.Add(valueField);
            }
        }

        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties()
        {
            // For each column found in the table
            // iterate through the list and create
            // each property.
            for (int i = 0; i < _data.PropertyName.Count(); i++)
            {
                // Create a new property member
                // and the accessor type.
                CodeMemberProperty valueProperty = new CodeMemberProperty();
                valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                // Assign the name and get and set indictors.
                valueProperty.Name = (_data.PropertyName[i]).ReplaceKeyOperands().ReplaceNumbers();
                valueProperty.HasGet = true;
                valueProperty.HasSet = true;

                // Add the comments to the property.
                valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueProperty.Comments.Add(new CodeCommentStatement("Gets sets, the " + _data.PropertyName[i].ToLower() + " property for the object.", true));
                valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                bool isNullable = false;
                if (_data.PropertyIsNullable != null)
                    isNullable = _data.PropertyIsNullable[i];

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).IsArray) &&
                    !Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).IsClass))
                {
                    // Assign the data type for the property if
                    // the data type is not a reference type
                    // then create a nullable type property.
                    valueProperty.Type = new CodeTypeReference(
                        "System.Nullable<" + Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).ToString() + ">");
                }
                else
                    // Assign the property type for the property.
                    // Get the data type of the property from
                    // the sql data type.
                    valueProperty.Type = new CodeTypeReference(_data.PropertyType[i]);

                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), ("_" + _data.PropertyName[i]).ReplaceKeyOperands())));

                // Create a new code condition statement.
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    new CodeVariableReferenceExpression("(this." + ("_" + _data.PropertyName[i]).ReplaceKeyOperands() + " != value)"),
                    new CodeStatement[] { 
                        new CodeExpressionStatement(
                            new CodeSnippetExpression("this." + ("_" + _data.PropertyName[i]).ReplaceKeyOperands() + " = value")) });

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(conditionalStatement);

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CategoryAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("\"Column\""))));

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DescriptionAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("\"Gets sets, the " + _data.PropertyName[i].ToLower() + " property for the object.\""))));

                // Add the attributes to the property.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("DataMemberAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + _data.PropertyDatabaseColumnName[i] + "\""))));

                // If the type is an array the add the 
                // array attribute else add the element attribute.
                if (Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]).IsArray)
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + _data.PropertyDatabaseColumnName[i] + 
                            "\", IsNullable = " + isNullable.ToString().ToLower()))));
                else
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + _data.PropertyDatabaseColumnName[i] + 
                            "\", IsNullable = " + isNullable.ToString().ToLower()))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("SoapElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("IsNullable = " + isNullable.ToString().ToLower()))));

                // Assign the column attribute.
                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("ColumnAttribute",
                    new CodeAttributeArgument(new CodeSnippetExpression("Storage = \"" + "_" + _data.PropertyDatabaseColumnName[i] + "\"" + ", DbType = \"" +
                         _data.PropertyDatabaseType[i] + "\", CanBeNull = " + isNullable.ToString().ToLower()))));

                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }

        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        public void AddMethodsMore()
        {
            // Declaring a create data method
            CodeMemberMethod createObjectMethodData = new CodeMemberMethod();
            createObjectMethodData.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            createObjectMethodData.Name = "Create" + _className;
            createObjectMethodData.ReturnType =
                new CodeTypeReference((!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Extended." + _className);

            // Add the comments to the property.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<summary>", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("Create a new " + _className.ToLower() + " data entity.", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("</summary>", true));

            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _data.PropertyName.Count(); i++)
            {
                // If the column is not null.
                if (!_data.PropertyIsNullable[i])
                {
                    string name = (_data.PropertyName[i]).ReplaceKeyOperands().ReplaceNumbers();
                    
                        // Add each parameter.
                    createObjectMethodData.Parameters.Add(
                        new CodeParameterDeclarationExpression(
                            Common.LinqToDataTypes.GetSystemType(_data.PropertyType[i]), name.ToLowerFirstLetter()));
                    createObjectMethodData.Comments.Add(
                        new CodeCommentStatement("<param name=\"" + name.ToLowerFirstLetter() + "\">Initial value of " + name + ".</param>", true));
                }
            }

            // Return comments.
            createObjectMethodData.Comments.Add(
                new CodeCommentStatement("<returns>The " +
                    (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + 
                    "Extended." + _className + " entity.</returns>", true));

            // Add the create code statement.
            createObjectMethodData.Statements.Add(
                new CodeSnippetExpression((!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + 
                    "Extended." + _className + " " + _className.ToLowerFirstLetter() + " = new " +
                    (!String.IsNullOrEmpty(_extendedName) ? "LinqToSql." + _extendedName + "." : "LinqToSql.") + "Extended." + _className + "()"));

            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _data.PropertyName.Count(); i++)
            {
                // If the column is not null.
                if (!_data.PropertyIsNullable[i])
                {
                    string name = (_data.PropertyName[i]).ReplaceKeyOperands().ReplaceNumbers();

                    // Add each parameter assignment.
                    createObjectMethodData.Statements.Add(
                        new CodeSnippetExpression(_className.ToLowerFirstLetter() + "." + name + " = " + name.ToLowerFirstLetter()));
                }
            }

            // Assign each entity property
            // to from the parameter.
            createObjectMethodData.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeArgumentReferenceExpression(_className.ToLowerFirstLetter())));


            // Add the property to the class.
            _targetClass.Members.Add(createObjectMethodData);
        }
    }
}
