/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Nequeo.Data.MongoDb.CodeDom
{
    /// <summary>
    /// Bson document model code generator.
    /// </summary>
    public sealed class BsonDocumentModel
    {
        #region Private Fields
        private CodeCompileUnit _targetUnit;
        private CodeNamespace _samples;
        private CodeTypeDeclaration _targetClass;
        private string _namespace = "MongoDb";
        private string _className = "MongoDbBsonModel";
        private BsonModelContainer _data = null;
        #endregion

        #region Code Generation
        /// <summary>
        /// Generate the code and the code file.
        /// </summary>
        /// <param name="database">The existing instance of MongoDatabase.</param>
        /// <param name="directory">The directoty to store the code files.</param>
        /// <param name="globalNamespace">The code file namespace to use.</param>
        /// <returns>The number of code files created.</returns>
        public int Generate(MongoDatabase database, string directory, string globalNamespace)
        {
            // Make sure the page reference exists.
            if (database == null) throw new ArgumentNullException("database");
            if (String.IsNullOrEmpty(directory)) throw new ArgumentNullException("directory");
            if (String.IsNullOrEmpty(globalNamespace)) throw new ArgumentNullException("globalNamespace");

            int ret = 0;

            // Get the collections names within the database.
            IEnumerable<string> collections = database.GetCollectionNames();

            // Make sure collection exist.
            if (collections != null && collections.Count() > 0)
            {
                // For each collection in the database.
                foreach (string collectionName in collections)
                {
                    try
                    {
                        // Get the document collection.
                        MongoCollection<BsonDocument> collection = database.GetCollection(collectionName);
                        BsonDocument[] bsonDocuments = collection.FindAll().SetLimit(1).ToArray();

                        // At least one document should be returned.
                        if (bsonDocuments != null && bsonDocuments.Length > 0)
                        {
                            // Create the bson model container.
                            Nequeo.Data.MongoDb.CodeDom.BsonModelContainer model = new Nequeo.Data.MongoDb.CodeDom.BsonModelContainer();
                            model.ClassName = collectionName;
                            model.Namespace = globalNamespace;
                            model.BsonDocument = bsonDocuments[0];
                            model.AssignProperties();

                            // Generate the code files.
                            CodeCompileUnit unit = Generate(model);
                            CreateCodeFile(directory.TrimEnd('\\') + "\\" + collectionName + ".cs", unit);

                            // Increment by one.
                            ret++;
                        }
                    }
                    catch { }
                }
            }

            // Return the number of code files created.
            return ret;
        }

        /// <summary>
        /// Generate the code.
        /// </summary>
        /// <param name="data">The data model container.</param>
        /// <returns>The code unit.</returns>
        public CodeCompileUnit Generate(BsonModelContainer data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            // If not null.
            if (data != null)
            {
                _namespace = data.Namespace;
                _className = data.ClassName;

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
            _samples = new CodeNamespace(_namespace);

            // Add each namespace reference.
            _samples.Imports.Add(new CodeNamespaceImport("System"));

            _samples.Imports.Add(new CodeNamespaceImport("MongoDB.Bson"));
            _samples.Imports.Add(new CodeNamespaceImport("MongoDB.Driver"));
            _samples.Imports.Add(new CodeNamespaceImport("MongoDB.Driver.Builders"));
            _samples.Imports.Add(new CodeNamespaceImport("MongoDB.Driver.GridFS"));
            _samples.Imports.Add(new CodeNamespaceImport("MongoDB.Driver.Linq"));
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

            // Create a custom region.
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.Start, _className + " Data Type");
            _targetClass.StartDirectives.Add(startRegion);

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
            AddFields();
            AddProperties();
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

                // Assign the property type for the property.
                // Get the data type of the property from
                // the sql data type.
                valueProperty.Type = new CodeTypeReference(_data.PropertyType[i]);
                    
                // Add the code to the
                // get section of the property.
                valueProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "_" + _data.PropertyName[i])));

                // Add the code to the
                // set section of the property.
                valueProperty.SetStatements.Add(new CodeExpressionStatement(
                    new CodeSnippetExpression("this." + "_" + _data.PropertyName[i] + " = value")));

                // Add the property to the class.
                _targetClass.Members.Add(valueProperty);
            }
        }
        #endregion
    }
}
