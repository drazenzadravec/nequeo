/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nequeo.CodeGeneration.Json
{
    /// <summary>
    /// JSON model code generator.
    /// </summary>
    public sealed class JsonModel
    {
        #region Private Fields
        private CodeCompileUnit _targetUnit;
        private CodeNamespace _samples;
        private CodeTypeDeclaration _targetClass;
        private string _nameSpace = "Nequeo.Json";
        private string _rootClassName = "Root";
        private JsonModelContainer _data = null;
        private IList<JsonType> _typeList = null;
        #endregion

        #region Code Generation
        /// <summary>
        /// Generate the code.
        /// </summary>
        /// <param name="json">The JSON data.</param>
        /// <param name="data">The JSON model container.</param>
        /// <returns>The code unit.</returns>
        public CodeCompileUnit Generate(string json, JsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");
            if (json == null) throw new ArgumentNullException("json");

            // If not null.
            if (data != null)
            {
                _nameSpace = data.Namespace;
                _rootClassName = data.RootClassName;

                Json.JsonGenerator jsonGenerator = new JsonGenerator(json);
                jsonGenerator.Namespace = _nameSpace;
                jsonGenerator.RootClassName = _rootClassName;
                _typeList = jsonGenerator.Extract();

                // Create the namespace.
                InitialiseNamespace();
            }

            // If not null.
            if (data != null)
            {
                _data = data;
                AddClasses();
            }

            // Return the complie unit.
            _targetUnit.Namespaces.Add(_samples);
            return _targetUnit;
        }

        /// <summary>
        /// Generate the code.
        /// </summary>
        /// <param name="stream">The JSON data.</param>
        /// <param name="data">The JSON model container.</param>
        /// <returns>The code unit.</returns>
        public CodeCompileUnit Generate(StreamReader stream, JsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");
            if (stream == null) throw new ArgumentNullException("json");

            // If not null.
            if (data != null)
            {
                _nameSpace = data.Namespace;
                _rootClassName = data.RootClassName;

                Json.JsonGenerator jsonGenerator = new JsonGenerator(stream);
                jsonGenerator.Namespace = _nameSpace;
                jsonGenerator.RootClassName = _rootClassName;
                _typeList = jsonGenerator.Extract();

                // Create the namespace.
                InitialiseNamespace();
            }

            // If not null.
            if (data != null)
            {
                _data = data;
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
        /// <param name="json">The JSON data.</param>
        /// <param name="data">The data model container.</param>
        public static void CreateFile(string fileName, string json, JsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (data == null) throw new ArgumentNullException("data");

            JsonModel model = new JsonModel();
            CodeCompileUnit code = model.Generate(json, data);
            model.CreateCodeFile(fileName, code);
        }

        /// <summary>
        /// Create the code file specified in the data model object.
        /// </summary>
        /// <param name="fileName">The full path where the file is to be created.</param>
        /// <param name="stream">The JSON data.</param>
        /// <param name="data">The data model container.</param>
        public static void CreateFile(string fileName, StreamReader stream, JsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (data == null) throw new ArgumentNullException("data");

            JsonModel model = new JsonModel();
            CodeCompileUnit code = model.Generate(stream, data);
            model.CreateCodeFile(fileName, code);
        }

        /// <summary>
        /// Map all the json data into classes, properties and references.
        /// </summary>
        /// <param name="json">The JSON data.</param>
        /// <param name="data">The data model container.</param>
        /// <returns>The json document.</returns>
        public static JsonDocument ClassMapper(string json, JsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            Json.JsonGenerator jsonGenerator = new JsonGenerator(json);
            jsonGenerator.Namespace = data.Namespace;
            jsonGenerator.RootClassName = data.RootClassName;
            JsonClass[] classess = GetClasses(jsonGenerator);
            return new JsonDocument()
            {
                Namespace = data.Namespace,
                RootClassName = data.RootClassName,
                JsonClasses = classess
            };
        }

        /// <summary>
        /// Map all the json data into classes, properties and references.
        /// </summary>
        /// <param name="stream">The JSON data.</param>
        /// <param name="data">The data model container.</param>
        /// <returns>The json document.</returns>
        public static JsonDocument ClassMapper(StreamReader stream, JsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            Json.JsonGenerator jsonGenerator = new JsonGenerator(stream);
            jsonGenerator.Namespace = data.Namespace;
            jsonGenerator.RootClassName = data.RootClassName;
            JsonClass[] classess = GetClasses(jsonGenerator);
            return new JsonDocument()
            {
                Namespace = data.Namespace,
                RootClassName = data.RootClassName,
                JsonClasses = classess
            };
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
            _samples = new CodeNamespace(_nameSpace);

            // Add each namespace reference.
            _samples.Imports.Add(new CodeNamespaceImport("System"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Threading"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Collections"));
            _samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
        }
        #endregion

        #region Create Classes
        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            // For each type.
            foreach (JsonType classType in _typeList)
            {
                // Create the class and add base inheritance type.
                _targetClass = new CodeTypeDeclaration(classType.AssignedName);
                _targetClass.IsClass = true;
                _targetClass.IsPartial = true;
                _targetClass.TypeAttributes = TypeAttributes.Public;

                // Create a custom region.
                CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, classType.AssignedName + " Data Type");
                _targetClass.StartDirectives.Add(startRegion);

                // Create the comments on the class.
                _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
                _targetClass.Comments.Add(new CodeCommentStatement("The " + classType.AssignedName.ToLower() + " data object class.", true));
                _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

                // Add the class members.
                AddMembers(classType, classType.Fields);

                // Create a custom endregion.
                CodeRegionDirective endRegion = new CodeRegionDirective(CodeRegionMode.End, "");
                _targetClass.EndDirectives.Add(endRegion);

                // Add the class to the namespace
                // and add the namespace to the unit.
                _samples.Types.Add(_targetClass);
            }
        }

        /// <summary>
        /// Add the class members.
        /// </summary>
        private void AddMembers(JsonType classType, IList<JsonFieldInfo> fields)
        {
            AddConstructor();
            AddExtensibilityMethods();
            AddFields(classType, fields);
            AddProperties(classType, fields);
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
        private void AddFields(JsonType classType, IList<JsonFieldInfo> fields)
        {
            // For each column found in the table
            // iterate through the list and create
            // each field.
            for (int i = 0; i < fields.Count; i++)
            {
                // Create a new field.
                CodeMemberField valueField = new CodeMemberField();

                // Assign the name and the accessor attribute.
                valueField.Attributes = MemberAttributes.Private;
                valueField.Name = "_" + fields[i].MemberName;

                bool isNullable = JsonType.IsNullable(fields[i].Type.Type);

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!JsonType.GetSystemType(fields[i].Type.Type).IsArray) &&
                    !JsonType.GetSystemType(fields[i].Type.Type).IsClass))
                {
                    // Assign the data type for the field if
                    // the data type is not a reference type
                    // then create a nullable type field.
                    valueField.Type = new CodeTypeReference(
                        "System.Nullable<" + JsonType.GetSystemType(fields[i].Type.Type).ToString() + ">");
                }
                else
                {
                    // Is array type.
                    if (fields[i].Type.Type == JsonTypeEnum.Array)
                    {
                        if (fields[i].Type.InternalType != null)
                        {
                            if (String.IsNullOrEmpty(fields[i].Type.InternalType.AssignedName))
                            {
                                // Make the type an array type.
                                valueField.Type = new CodeTypeReference(JsonType.GetSystemType(fields[i].Type.InternalType.Type) + "[]");
                            }
                            else
                                // Make the type an array type.
                                valueField.Type = new CodeTypeReference(fields[i].Type.InternalType.AssignedName + "[]");
                        }
                        else
                            valueField.Type = new CodeTypeReference(JsonType.GetSystemType(fields[i].Type.Type));
                    }
                    else
                    {
                        if (fields[i].Type.InternalType != null)
                            // Assign the field type for the field.
                            // Get the data type of the field from
                            // the sql data type.
                            valueField.Type = new CodeTypeReference(fields[i].Type.InternalType.AssignedName);
                        else
                            valueField.Type = new CodeTypeReference(JsonType.GetSystemType(fields[i].Type.Type));
                    }
                }

                // Add the field to the class.
                _targetClass.Members.Add(valueField);
            }
        }
        #endregion

        #region Create Properties
        /// <summary>
        /// Add the properties to the class.
        /// </summary>
        private void AddProperties(JsonType classType, IList<JsonFieldInfo> fields)
        {
            // For each column found in the table
            // iterate through the list and create
            // each property.
            for (int i = 0; i < fields.Count; i++)
            {
                // Create a new property member
                // and the accessor type.
                CodeMemberProperty valueProperty = new CodeMemberProperty();
                valueProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                // Assign the name and get and set indictors.
                valueProperty.Name = fields[i].MemberName;
                valueProperty.HasGet = true;
                valueProperty.HasSet = true;

                // Add the comments to the property.
                valueProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                valueProperty.Comments.Add(new CodeCommentStatement("Gets or sets, the " + fields[i].MemberName.ToLower() + " property for the object.", true));
                valueProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

                bool isNullable = JsonType.IsNullable(fields[i].Type.Type);

                // If the table column is nullable and
                // the data type is not a reference type
                // then apply the nullable generic base.
                if (isNullable && ((!JsonType.GetSystemType(fields[i].Type.Type).IsArray) &&
                    !JsonType.GetSystemType(fields[i].Type.Type).IsClass))
                {
                    // Assign the data type for the property if
                    // the data type is not a reference type
                    // then create a nullable type property.
                    valueProperty.Type = new CodeTypeReference(
                        "System.Nullable<" + JsonType.GetSystemType(fields[i].Type.Type).ToString() + ">");
                }
                else
                {
                    // Is array type.
                    if (fields[i].Type.Type == JsonTypeEnum.Array)
                    {
                        if (fields[i].Type.InternalType != null)
                        {
                            if (String.IsNullOrEmpty(fields[i].Type.InternalType.AssignedName))
                            {
                                // Make the type an array type.
                                valueProperty.Type = new CodeTypeReference(JsonType.GetSystemType(fields[i].Type.InternalType.Type) + "[]");
                            }
                            else
                                // Make the type an array type.
                                valueProperty.Type = new CodeTypeReference(fields[i].Type.InternalType.AssignedName + "[]");
                        }
                        else
                            valueProperty.Type = new CodeTypeReference(JsonType.GetSystemType(fields[i].Type.Type));
                    }
                    else
                    {
                        if (fields[i].Type.InternalType != null)
                            // Assign the field type for the field.
                            // Get the data type of the field from
                            // the sql data type.
                            valueProperty.Type = new CodeTypeReference(fields[i].Type.InternalType.AssignedName);
                        else
                            valueProperty.Type = new CodeTypeReference(JsonType.GetSystemType(fields[i].Type.Type));
                    }
                }

                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "_" + fields[i].MemberName)));

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression("this." + "_" + fields[i].MemberName + " = value")));

                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }
        #endregion

        #region Create Property Model
        /// <summary>
        /// Get all the classes.
        /// </summary>
        /// <param name="jsonGenerator">The json generator.</param>
        /// <returns>The list of classes.</returns>
        private static JsonClass[] GetClasses(Json.JsonGenerator jsonGenerator)
        {
            List<JsonClass> jsonClasses = new List<JsonClass>();
            IList<JsonType> typeList = jsonGenerator.Extract();
            string nameSpace = jsonGenerator.Namespace.TrimEnd('.') + ".";

            // For each type.
            foreach (JsonType classType in typeList)
            {
                JsonClass jsonClass = new JsonClass();
                jsonClass.ClassName = classType.AssignedName;

                IList<JsonFieldInfo> fields = classType.Fields;
                List<Nequeo.Model.PropertyStringModel> properties = new List<Model.PropertyStringModel>();

                // For each column found in the table
                // iterate through the list and create
                // each field.
                for (int i = 0; i < fields.Count; i++)
                {
                    Nequeo.Model.PropertyStringModel property = new Model.PropertyStringModel()
                    {
                        PropertyName = fields[i].MemberName,
                        PropertyType = GetPropertyType(nameSpace, fields[i])
                    };

                    // Add the property.
                    properties.Add(property);
                }

                // Assign the properties.
                jsonClass.Properties = properties.ToArray();

                // Add the class.
                jsonClasses.Add(jsonClass);
            }

            // Return the list of classes.
            return jsonClasses.ToArray();
        }

        /// <summary>
        /// Get the property type.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="jsonFieldInfo">The json field info.</param>
        /// <returns>The string value of the type.</returns>
        private static string GetPropertyType(string nameSpace, JsonFieldInfo jsonFieldInfo)
        {
            string propertyType = "";
            bool isNullable = JsonType.IsNullable(jsonFieldInfo.Type.Type);

            // If the table column is nullable and
            // the data type is not a reference type
            // then apply the nullable generic base.
            if (isNullable && ((!JsonType.GetSystemType(jsonFieldInfo.Type.Type).IsArray) &&
                !JsonType.GetSystemType(jsonFieldInfo.Type.Type).IsClass))
            {
                // Assign the data type for the field if
                // the data type is not a reference type
                // then create a nullable type field.
                propertyType = "System.Nullable<" + JsonType.GetSystemTypeString(jsonFieldInfo.Type.Type).ToString() + ">";
            }
            else
            {
                // Is array type.
                if (jsonFieldInfo.Type.Type == JsonTypeEnum.Array)
                {
                    if (jsonFieldInfo.Type.InternalType != null)
                    {
                        if (String.IsNullOrEmpty(jsonFieldInfo.Type.InternalType.AssignedName))
                        {
                            // Make the type an array type.
                            propertyType = JsonType.GetSystemType(jsonFieldInfo.Type.InternalType.Type).ToString() + "[]";
                        }
                        else
                            // Make the type an array type.
                            propertyType = nameSpace + jsonFieldInfo.Type.InternalType.AssignedName + "[]";
                    }
                    else
                        propertyType = JsonType.GetSystemType(jsonFieldInfo.Type.Type).ToString();
                }
                else
                {
                    if (jsonFieldInfo.Type.InternalType != null)
                        // Assign the field type for the field.
                        // Get the data type of the field from
                        // the sql data type.
                        propertyType = nameSpace + jsonFieldInfo.Type.InternalType.AssignedName;
                    else
                        propertyType = JsonType.GetSystemType(jsonFieldInfo.Type.Type).ToString();
                }
            }

            // Return the property type.
            return propertyType;
        }
        #endregion
    }
}
