/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using Nequeo.Data.DataType;
using Nequeo.Extension;

namespace Nequeo.CodeGeneration
{
    /// <summary>
    /// Data model code generator.
    /// </summary>
    public sealed class DataModel
    {
        #region Private Fields
        private CodeCompileUnit _targetUnit;
        private CodeNamespace _samples;
        private CodeTypeDeclaration _targetClass;
        private string _dataBaseName = "Database";
        private string _companyName = "Company";
        private string _extendedName = "";
        private string _className = "Class1";
        private DataModelContainer _data = null;
        #endregion

        #region Code Generation
        /// <summary>
        /// Generate the code.
        /// </summary>
        /// <param name="data">The data model container.</param>
        /// <returns>The code unit.</returns>
        public CodeCompileUnit Generate(DataModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

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
            _targetUnit.Namespaces.Add(_samples);
            return _targetUnit;
        }

        /// <summary>
        /// Create the code file specified in the compile unit
        /// </summary>
        /// <param name="fileName">The full path where the file is to be created.</param>
        /// <param name="codeCompileUnit">The code compile unit containing the code data.</param>
        public void CreateCodeFile(string fileName, CodeCompileUnit codeCompileUnit)
        {
            // Make sure the page reference exists.
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (codeCompileUnit == null) throw new ArgumentNullException("codeCompileUnit");

            // If the director does not
            // exits create the directory.
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            // Create a new code dom provider in
            // C# code format. Add new options
            // to the code in 'C' format.
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            // Write the code to the file
            // through the stream writer.
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(codeCompileUnit, sourceWriter, options);
            }
        }

        /// <summary>
        /// Create the code file specified in the data model object.
        /// </summary>
        /// <param name="fileName">The full path where the file is to be created.</param>
        /// <param name="data">The data model container.</param>
        public static void CreateFile(string fileName, DataModelContainer data)
        {
            // Make sure the page reference exists.
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (data == null) throw new ArgumentNullException("data");

            DataModel model = new DataModel();
            CodeCompileUnit code = model.Generate(data);
            model.CreateCodeFile(fileName, code);
        }

        /// <summary>
        /// Create a new instance of the model containing the property items.
        /// </summary>
        /// <param name="data">The data model container.</param>
        /// <returns>The instantiated object containing the property items</returns>
        public static object CreateInstance(DataModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            // If not null.
            if (data != null)
            {
                List<Nequeo.Reflection.DynamicProperty> prop = new List<Nequeo.Reflection.DynamicProperty>();

                // For each column found in the table
                // iterate through the list and create
                // each property.
                for (int i = 0; i < data.PropertyName.Count(); i++)
                {
                    Type propertyType;
                    bool isNullable = false;
                    if (data.PropertyIsNullable != null)
                        isNullable = data.PropertyIsNullable[i];

                    // If the table column is nullable and
                    // the data type is not a reference type
                    // then apply the nullable generic base.
                    if (isNullable && ((!Nequeo.DataType.GetSystemType(data.PropertyType[i]).IsArray) &&
                        !Nequeo.DataType.GetSystemType(data.PropertyType[i]).IsClass))
                    {
                        // Assign the data type for the property if
                        // the data type is not a reference type
                        // then create a nullable type property.
                        Type propertyIntType = Nequeo.DataType.GetSystemType(data.PropertyType[i]);
                        Type propertyGenericType = typeof(System.Nullable<>);
                        Type[] typeArgs = { propertyIntType };
                        Type nullableTypeConstructor = propertyGenericType.MakeGenericType(typeArgs);
                        propertyType = nullableTypeConstructor;
                    }
                    else
                        // Create the type.
                        propertyType = Nequeo.DataType.GetSystemType(data.PropertyType[i]);
                    
                    // Add the type to the collection.
                    prop.Add(new Nequeo.Reflection.DynamicProperty(data.PropertyName[i], propertyType));
                }

                // Return the new instance of the object.
                object instance = Nequeo.Reflection.TypeAccessor.CreateInstance(prop);
                PropertyInfo[] infos = instance.GetType().GetProperties();

                // Assign each type value
                foreach (PropertyInfo info in infos)
                    info.SetValue(instance, data.PropertyName.First(u => u == info.Name), null);

                // Return the instance with values.
                return instance;
            }

            // No instance
            return null;
        }
        #endregion

        #region Create Namespaces
        /// <summary>
        /// Create the namespace and import namespaces.
        /// </summary>
        private void InitialiseNamespace()
        {
            // Create a new base compile unit module.
            _targetUnit = new CodeCompileUnit();

            // Create a new namespace.
            _samples = new CodeNamespace(_companyName + ".DataAccess." + _dataBaseName + ".Data" +
                (!String.IsNullOrEmpty(_extendedName) ? "." + _extendedName : "") + ".Extended");

            // Add each namespace reference.
            _samples.Imports.Add(new CodeNamespaceImport("System"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Linq"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Text"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Data"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Threading"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Data.OleDb"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Data.Odbc"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Collections"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            _samples.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));

            _samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data"));
            _samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Control"));
            _samples.Imports.Add(new CodeNamespaceImport("Nequeo.Data.Custom"));
        }
        #endregion

        #region Create Classes
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
            _targetClass.BaseTypes.Add(new CodeTypeReference("DataBase"));

            // Create a custom region.
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, _className + " Data Type");
            _targetClass.StartDirectives.Add(startRegion);

            // Create the attributes on the class.
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("DataContractAttribute",
                new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + _className + "\", IsReference = true"))));
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("SerializableAttribute"));
            _targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("KnownTypeAttribute",
                new CodeAttributeArgument(new CodeSnippetExpression("typeof(DataBase)"))));

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + _className.ToLower() + " data object class.", true));
            _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the class members.
            AddMembers();

            // Create a custom endregion.
            CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
            _targetClass.EndDirectives.Add(endRegion);

            // Add the class to the namespace
            // and add the namespace to the unit.
            _samples.Types.Add(_targetClass);
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
        #endregion

        #region Create Constructors
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
        #endregion

        #region Create Extensibility Methods
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
        #endregion

        #region Create Fields
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
                valueField.Name = "_" + _data.PropertyName[i];

                if (_data.PropertyDefaultValue != null)
                    valueField.InitExpression = new CodeSnippetExpression(_data.PropertyDefaultValue[i]);

                bool isNullable = false;
                if (_data.PropertyIsNullable != null)
                    isNullable = _data.PropertyIsNullable[i];

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!Nequeo.DataType.GetSystemType(_data.PropertyType[i]).IsArray) &&
                    !Nequeo.DataType.GetSystemType(_data.PropertyType[i]).IsClass))
                {
                    // Assign the data type for the field if
                    // the data type is not a reference type
                    // then create a nullable type field.
                    valueField.Type = new CodeTypeReference(
                        "System.Nullable<" + Nequeo.DataType.GetSystemType(_data.PropertyType[i]).ToString() + ">");
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
        #endregion

        #region Create Properties
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
                valueProperty.Name = _data.PropertyName[i];
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
                if (isNullable && ((!Nequeo.DataType.GetSystemType(_data.PropertyType[i]).IsArray) &&
                    !Nequeo.DataType.GetSystemType(_data.PropertyType[i]).IsClass))
                {
                    // Assign the data type for the property if
                    // the data type is not a reference type
                    // then create a nullable type property.
                    valueProperty.Type = new CodeTypeReference(
                        "System.Nullable<" + Nequeo.DataType.GetSystemType(_data.PropertyType[i]).ToString() + ">");
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
                    new CodeThisReferenceExpression(), "_" + _data.PropertyName[i])));

                // Create a new code condition statement.
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    new CodeVariableReferenceExpression("(this." + "_" + _data.PropertyName[i] + " != value)"),
                    new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression("this." + "_" + _data.PropertyName[i] + " = value")) });

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
                    new CodeAttributeArgument(new CodeSnippetExpression("Name = \"" + _data.PropertyName[i] + "\""))));

                // If the type is an array the add the 
                // array attribute else add the element attribute.
                if ((Nequeo.DataType.GetSystemType(_data.PropertyType[i]).IsArray) ||
                    (_data.PropertyType[i].EndsWith("[]")))
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlArrayAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + _data.PropertyName[i] + "\", IsNullable = " + isNullable.ToString().ToLower()))));
                else
                    valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("XmlElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("ElementName = \"" + _data.PropertyName[i] + "\", IsNullable = " + isNullable.ToString().ToLower()))));

                valueProperty.CustomAttributes.Add(new CodeAttributeDeclaration("SoapElementAttribute",
                        new CodeAttributeArgument(new CodeSnippetExpression("IsNullable = " + isNullable.ToString().ToLower()))));

                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }
        #endregion

        #region Create Methods
        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        private void AddMethodsMore()
        {
            // Declaring a create data method
            CodeMemberMethod createObjectMethodData = new CodeMemberMethod();
            createObjectMethodData.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            createObjectMethodData.Name = "Create" + _className;
            createObjectMethodData.ReturnType = new CodeTypeReference(
                (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + "Extended." + _className);

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
                    // Add each parameter.
                    createObjectMethodData.Parameters.Add(
                        new CodeParameterDeclarationExpression(_data.PropertyType[i], _data.PropertyName[i].ToLowerFirstLetter()));
                    createObjectMethodData.Comments.Add(
                        new CodeCommentStatement("<param name=\"" + _data.PropertyName[i].ToLowerFirstLetter() + "\">Initial value of " + 
                            _data.PropertyName[i] + ".</param>", true));
                }
            }

            // Return comments.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<returns>The " +
                (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + "Extended." + 
                _className + " entity.</returns>", true));

            // Add the create code statement.
            createObjectMethodData.Statements.Add(
                new CodeSnippetExpression(
                    (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + "Extended." + 
                    _className + " " + _className.ToLowerFirstLetter() + " = new " +
                    (!String.IsNullOrEmpty(_extendedName) ? "Data." + _extendedName + "." : "Data.") + "Extended." + _className + "()"));

            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < _data.PropertyName.Count(); i++)
            {
                // If the column is not null.
                if (!_data.PropertyIsNullable[i])
                {
                    // Add each parameter assignment.
                    createObjectMethodData.Statements.Add(
                        new CodeSnippetExpression(_className.ToLowerFirstLetter() + "." + _data.PropertyName[i] + " = " + 
                            _data.PropertyName[i].ToLowerFirstLetter()));
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
        #endregion
    }
}
