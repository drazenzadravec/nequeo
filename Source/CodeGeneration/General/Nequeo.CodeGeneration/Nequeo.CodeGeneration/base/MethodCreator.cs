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
using System.ComponentModel.DataAnnotations;
using System.IO;

using Nequeo.Data.DataType;
using Nequeo.Extension;

namespace Nequeo.CodeGeneration
{
    /// <summary>
    /// Method code generator.
    /// </summary>
    public sealed class MethodCreator
    {
        #region Private Fields
        private CodeCompileUnit _targetUnit;
        private CodeNamespace _samples;
        private CodeTypeDeclaration _targetClass;
        private Type _model;
        #endregion

        #region Code Generation
        /// <summary>
        /// Generate the code.
        /// </summary>
        /// <param name="model">The data model type.</param>
        /// <returns>The code unit.</returns>
        public CodeCompileUnit Generate(Type model)
        {
            // Make sure the page reference exists.
            if (model == null) throw new ArgumentNullException("model");

            // If not null.
            if (model != null)
            {
                _model = model;

                // Create the namespace.
                InitialiseNamespace();
            }

            // If not null.
            if (model != null)
            {
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
            _samples = new CodeNamespace();

            // Add each namespace reference.
            _samples.Imports.Add(new CodeNamespaceImport("System"));
        }
        #endregion

        #region Create Classes
        /// <summary>
        /// Add the classes.
        /// </summary>
        private void AddClasses()
        {
            // Create the class and add base inheritance type.
            _targetClass = new CodeTypeDeclaration(_model.Name);
            _targetClass.IsClass = true;
            _targetClass.IsPartial = true;
            _targetClass.TypeAttributes = TypeAttributes.Public;

            // Create the comments on the class.
            _targetClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            _targetClass.Comments.Add(new CodeCommentStatement("The " + _model.Name + " class.", true));
            _targetClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the class members.
            AddMembers();

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
            AddMethod();
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

            // Create the comments on the constructor.
            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement("Default constructor.", true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Add the constructor to the class.
            _targetClass.Members.Add(constructor);
        }
        #endregion

        #region Create Methods
        /// <summary>
        /// Add the methods to the class.
        /// </summary>
        private void AddMethod()
        {
            // Declaring a create data method
            CodeMemberMethod createObjectMethodData = new CodeMemberMethod();
            createObjectMethodData.Attributes = MemberAttributes.Public;

            createObjectMethodData.Name = "Create" + _model.Name;
            //createObjectMethodData.ReturnType = new CodeTypeReference("void");

            // Add the comments to the property.
            createObjectMethodData.Comments.Add(new CodeCommentStatement("<summary>", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("Create a new " + _model.Name + " data entity.", true));
            createObjectMethodData.Comments.Add(new CodeCommentStatement("</summary>", true));

            // Get all the properites within the type.
            PropertyInfo[] properties = _model.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // For each column found.
            foreach (PropertyInfo result in properties)
            {
                // Add each parameter.
                createObjectMethodData.Parameters.Add(
                    new CodeParameterDeclarationExpression(result.PropertyType, result.Name.ToLowerFirstLetter()));
                createObjectMethodData.Comments.Add(
                    new CodeCommentStatement("<param name=\"" + result.Name.ToLowerFirstLetter() + "\">Initial value of " + result.Name + ".</param>", true));
            }

            // Add the create code statement.
            createObjectMethodData.Statements.Add(
                new CodeSnippetExpression(_model.Name + " " + _model.Name.ToLower() + " = new " + _model.Name + "()"));

            // For each column found.
            foreach (PropertyInfo result in properties)
            {
                // Add each parameter assignment.
                createObjectMethodData.Statements.Add(
                    new CodeSnippetExpression(_model.Name.ToLower() + "." + result.Name + " = " + result.Name.ToLowerFirstLetter()));
            }

            // Add the property to the class.
            _targetClass.Members.Add(createObjectMethodData);
        }
        #endregion
    }
}
