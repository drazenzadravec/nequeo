using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.CodeDom.Compiler;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Xml.Linq;
using System.CodeDom;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Nequeo.CustomTool.CodeGenerator
{
    /// <summary>
    /// Global code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("9CD551E2-5FF2-402B-A478-5687312C3E71")]
    public class GlobalCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Code.CompleteDataObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomLinqObject codeDom = new Generation.CodeDomLinqObject();
                CodeCompileUnit targetUnit = null; // = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Code.CompleteDataObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Code.CompleteDataObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Code.CompleteDataObjectContainer data = (Code.CompleteDataObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Linq object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("7F7CEEDF-FD9E-4272-BD45-B3C72A18A362")]
    public class LinqObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.LinqObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomLinqObject codeDom = new Generation.CodeDomLinqObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.LinqObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.LinqObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.LinqObjectContainer data = (Common.LinqObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Data object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("A064EA11-6E21-4EA0-9BAA-F12DA619CB7C")]
    public class DataObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.DataObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDataObject codeDom = new Generation.CodeDomDataObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.DataObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.DataObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.DataObjectContainer data = (Common.DataObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Database object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("B37D29E6-E18A-47F4-A5B9-AA5F5A59E1D6")]
    public class DatabaseObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.DataBaseObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDatabaseObject codeDom = new Generation.CodeDomDatabaseObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.DataBaseObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.DataBaseObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.DataBaseObjectContainer data = (Common.DataBaseObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Replica object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("BA74DB1C-17CD-43D0-83C3-5721D31C935C")]
    public class ReplicaObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                XElement context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                string tableName = context.Element("TableName").Value;
                string linqDataContextName = context.Element("LinqDataContextName").Value;
                string database = context.Element("Database").Value;
                string namespaceCompanyName = context.Element("NamespaceCompanyName").Value;
                string configKeyDatabaseConnection = context.Element("ConfigKeyDatabaseConnection").Value;
                bool useAnonymousDataEntity = Convert.ToBoolean(context.Element("UseAnonymousDataEntity").Value);
                int connectionType = Convert.ToInt32(context.Element("ConnectionType").Value);
                int connectionDataType = Convert.ToInt32(context.Element("ConnectionDataType").Value);
                string extendedName = context.Element("NamespaceExtendedName").Value;
                string dataAccessProvider = context.Element("DataAccessProvider").Value;

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomReplicaObject codeDom = new Generation.CodeDomReplicaObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(tableName, linqDataContextName,
                    database, namespaceCompanyName, configKeyDatabaseConnection, useAnonymousDataEntity,
                    connectionType, extendedName, connectionDataType, dataAccessProvider);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        private XElement ExtractContext(string inputFileContent)
        {
            XElement context = null;

            // Load in the xml file.
            XDocument xmlDoc = XDocument.Load(new StringReader(inputFileContent));
            if (xmlDoc != null)
            {
                // Gets the context node element.
                XElement contextNode = xmlDoc.Element("Context");
                if (contextNode != null)
                {
                    context = contextNode;
                }
            }

            // Return the context element.
            return context;
        }
    }

    /// <summary>
    /// Model object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("2B6B8968-16D5-43A4-91E3-59875AA77C34")]
    public class ModelObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                XElement context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                string tableName = context.Element("TableName").Value;
                string edmDataContextName = context.Element("EdmDataContextName").Value;
                string database = context.Element("Database").Value;
                string namespaceCompanyName = context.Element("NamespaceCompanyName").Value;
                string configKeyDatabaseConnection = context.Element("ConfigKeyDatabaseConnection").Value;
                bool useAnonymousDataEntity = Convert.ToBoolean(context.Element("UseAnonymousDataEntity").Value);
                int connectionType = Convert.ToInt32(context.Element("ConnectionType").Value);
                int connectionDataType = Convert.ToInt32(context.Element("ConnectionDataType").Value);
                string extendedName = context.Element("NamespaceExtendedName").Value;
                string dataAccessProvider = context.Element("DataAccessProvider").Value;

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomModelObject codeDom = new Generation.CodeDomModelObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(tableName, edmDataContextName,
                    database, namespaceCompanyName, configKeyDatabaseConnection, useAnonymousDataEntity,
                    connectionType, extendedName, connectionDataType, dataAccessProvider);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        private XElement ExtractContext(string inputFileContent)
        {
            XElement context = null;

            // Load in the xml file.
            XDocument xmlDoc = XDocument.Load(new StringReader(inputFileContent));
            if (xmlDoc != null)
            {
                // Gets the context node element.
                XElement contextNode = xmlDoc.Element("Context");
                if (contextNode != null)
                {
                    context = contextNode;
                }
            }

            // Return the context element.
            return context;
        }
    }

    /// <summary>
    /// Schema object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("1D7F3018-6DBA-44B7-9715-09AE31DB0612")]
    public class SchemaObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                XElement context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                string tableName = context.Element("TableName").Value;
                string dataSetDataContextName = context.Element("DataSetDataContextName").Value;
                string database = context.Element("Database").Value;
                string namespaceCompanyName = context.Element("NamespaceCompanyName").Value;
                string configKeyDatabaseConnection = context.Element("ConfigKeyDatabaseConnection").Value;
                bool useAnonymousDataEntity = Convert.ToBoolean(context.Element("UseAnonymousDataEntity").Value);
                int connectionType = Convert.ToInt32(context.Element("ConnectionType").Value);
                int connectionDataType = Convert.ToInt32(context.Element("ConnectionDataType").Value);
                string extendedName = context.Element("NamespaceExtendedName").Value;
                string dataAccessProvider = context.Element("DataAccessProvider").Value;

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomSchemaObject codeDom = new Generation.CodeDomSchemaObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(tableName, dataSetDataContextName,
                    database, namespaceCompanyName, configKeyDatabaseConnection, useAnonymousDataEntity,
                    connectionType, extendedName, connectionDataType, dataAccessProvider);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        private XElement ExtractContext(string inputFileContent)
        {
            XElement context = null;

            // Load in the xml file.
            XDocument xmlDoc = XDocument.Load(new StringReader(inputFileContent));
            if (xmlDoc != null)
            {
                // Gets the context node element.
                XElement contextNode = xmlDoc.Element("Context");
                if (contextNode != null)
                {
                    context = contextNode;
                }
            }

            // Return the context element.
            return context;
        }
    }

    /// <summary>
    /// Model data context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("ED8B457B-223D-45D7-9816-6F48BE36BC3E")]
    public class ModelDataContextObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.ModelDataContextObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomModelDataContext codeDom = new Generation.CodeDomModelDataContext();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.ModelDataContextObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.ModelDataContextObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.ModelDataContextObjectContainer data = (Common.ModelDataContextObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Replica data context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("D83F8AAE-A037-41B0-A22F-2960599EA3BC")]
    public class ReplicaDataContextObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.ReplicaDataContextObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomReplicaDataContext codeDom = new Generation.CodeDomReplicaDataContext();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.ReplicaDataContextObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.ReplicaDataContextObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.ReplicaDataContextObjectContainer data = (Common.ReplicaDataContextObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Schema data context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("F7E3BB20-69D7-40CA-8B13-AB9A51FCB8F7")]
    public class SchemaDataContextObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.SchemaDataContextObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomSchemaDataContext codeDom = new Generation.CodeDomSchemaDataContext();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.SchemaDataContextObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.SchemaDataContextObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.SchemaDataContextObjectContainer data = (Common.SchemaDataContextObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Enumeration object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("C8D7628C-C68B-44F9-B920-D11C6E57A01C")]
    public class EnumObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.EnumObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomEnumObject codeDom = new Generation.CodeDomEnumObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.EnumObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.EnumObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.EnumObjectContainer data = (Common.EnumObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Database xml object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("167A9436-4BCD-49EE-9571-63A54458EE39")]
    public class DatabaseXmlObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Schema.xml"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Schema.xml");
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.DataBaseObjectContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDatabaseXmlObject codeDom = new Generation.CodeDomDatabaseXmlObject();
                MemoryStream memoryStream = codeDom.GenerateCode(context);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = memoryStream.ToArray();

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.DataBaseObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.DataBaseObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.DataBaseObjectContainer data = (Common.DataBaseObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Database xml model object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("314C4EE6-3DB3-4822-B683-E4393F5E6948")]
    public class DatabaseXmlModelObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Schema.xml"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.DatabaseModel context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDatabaseXmlModelObject codeDom = new Generation.CodeDomDatabaseXmlModelObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.DatabaseModel ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.DatabaseModel));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.DatabaseModel data = (Common.DatabaseModel)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Database context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("514C178B-695E-43AF-8162-E596FA8C9DFA")]
    public class DatabaseContextCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.DataBaseContextContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDatabaseContext codeDom = new Generation.CodeDomDatabaseContext();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.DataBaseContextContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.DataBaseContextContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.DataBaseContextContainer data = (Common.DataBaseContextContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Data extension object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("92B68E96-694B-4ED1-8B46-8B3AD732FFFD")]
    public class DataExtensionObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                XElement context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                string tableName = context.Element("TableName").Value;
                string dataContextName = context.Element("DataContextName").Value;
                string database = context.Element("Database").Value;
                string namespaceCompanyName = context.Element("NamespaceCompanyName").Value;
                string configKeyDatabaseConnection = context.Element("ConfigKeyDatabaseConnection").Value;
                int connectionType = Convert.ToInt32(context.Element("ConnectionType").Value);
                int connectionDataType = Convert.ToInt32(context.Element("ConnectionDataType").Value);
                string extendedName = context.Element("NamespaceExtendedName").Value;
                string dataAccessProvider = context.Element("DataAccessProvider").Value;

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDataExtensionObject codeDom = new Generation.CodeDomDataExtensionObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(tableName, dataContextName, database,
                    namespaceCompanyName, configKeyDatabaseConnection, connectionType, extendedName,
                    connectionDataType, dataAccessProvider);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        private XElement ExtractContext(string inputFileContent)
        {
            XElement context = null;

            // Load in the xml file.
            XDocument xmlDoc = XDocument.Load(new StringReader(inputFileContent));
            if (xmlDoc != null)
            {
                // Gets the context node element.
                XElement contextNode = xmlDoc.Element("Context");
                if (contextNode != null)
                {
                    context = contextNode;
                }
            }

            // Return the context element.
            return context;
        }
    }

    /// <summary>
    /// Data extension context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("3BECF995-5A4D-420A-9CB1-5D5D7E7D96DA")]
    public class DataExtensionContextObjectCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.DataContextExtensionContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomDataExtensionContext codeDom = new Generation.CodeDomDataExtensionContext();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(stringWriter.ToString());

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.DataContextExtensionContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.DataContextExtensionContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.DataContextExtensionContainer data = (Common.DataContextExtensionContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Procedure extension context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("4EDE3A37-1831-4C6F-9FE5-6B1367B20D6F")]
    public class ProcedureExtensionCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.ProcedureExtensionContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomProcedureObject codeDom = new Generation.CodeDomProcedureObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.ProcedureExtensionContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.ProcedureExtensionContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.ProcedureExtensionContainer data =
                (Common.ProcedureExtensionContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Function extension context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("ADDB1117-8CD8-4110-8B17-09BA8DEC2CBE")]
    public class FunctionTableExtensionCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.FunctionExtensionContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomFunctionTableObject codeDom = new Generation.CodeDomFunctionTableObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.FunctionExtensionContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.FunctionExtensionContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.FunctionExtensionContainer data =
                (Common.FunctionExtensionContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }

    /// <summary>
    /// Function extension context object code generator.
    /// </summary>
    [ComVisible(true)]
    [Guid("B3C42FBC-DA78-4C7E-99D6-82A01E7170C1")]
    public class FunctionScalarExtensionCodeGenerator : Common.BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Make the generated (a.k.a. code-behind) file have the file extension ".Designer.cs" or ".Designer.vb"
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return (".Designer" + base.GetDefaultExtension());
        }

        /// <summary>
        /// Generates the code at this point.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The byte of code generated data.</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            byte[] generatedCodeAsBytes = null;

            try
            {
                // Check for data context information.
                Common.FunctionExtensionContainer context = ExtractContext(inputFileContent);
                if (context == null)
                    throw new InvalidOperationException("No context data exists in the xml file.");

                // Assign the options when
                // generating code.
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";

                // Create the code dom internal class
                // and start generating the code
                Generation.CodeDomFunctionScalarObject codeDom = new Generation.CodeDomFunctionScalarObject();
                CodeCompileUnit targetUnit = codeDom.GenerateCode(context);

                // Create a new string writer, this
                // is where the generated code will be
                // written to.
                TextWriter stringWriter = new StringWriter();

                // Write the output to the string writer.
                base.GetCodeProvider().GenerateCodeFromCompileUnit(targetUnit, stringWriter, options);

                // Convert generated code into bytes to return to Visual Studio
                string complete =
                    "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                    "#pragma warning disable 169\r\n" +
                    stringWriter.ToString() + "\r\n" +
                    "#pragma warning restore 169\r\n";

                // Convert generated code into bytes to return to Visual Studio
                generatedCodeAsBytes = Encoding.ASCII.GetBytes(complete);

                // Return the bytes.
                return generatedCodeAsBytes;
            }
            catch (Exception e)
            {
                base.GeneratorError(4, e.Message, 1, 1);

                // Returning null signifies that generation has failed.
                return null;
            }
        }

        /// <summary>
        /// Gets the xml context node elements.
        /// </summary>
        /// <param name="inputFileContent">Content of the xml file.</param>
        /// <returns>The xml element.</returns>
        public Common.FunctionExtensionContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(Common.FunctionExtensionContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            Common.FunctionExtensionContainer data =
                (Common.FunctionExtensionContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }
    }
}
