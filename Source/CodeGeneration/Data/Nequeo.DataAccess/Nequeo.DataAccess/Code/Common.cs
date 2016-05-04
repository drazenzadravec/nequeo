using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.CSharp;

using Nequeo.CustomTool.CodeGenerator.Common;

using Generate = Nequeo.CustomTool.CodeGenerator.Generation;
using GenerateComm = Nequeo.CustomTool.CodeGenerator.Common;

namespace Nequeo.CustomTool.CodeGenerator.Code
{
    /// <summary>
    /// Internal code generation interface class.
    /// </summary>
    public class Common
    {
        #region Static Generation Methods
        /// <summary>
        /// Generate the code contained in the code complie unit object.
        /// </summary>
        /// <param name="directory">The directory where the code is placed</param>
        /// <param name="fileName">The file name the code is created in.</param>
        /// <param name="targetUnit">The code complie unit containing the code.</param>
        /// <param name="start">The text to place at the begining.</param>
        /// <param name="end">The text tgo place at the end</param>
        public static void GenerateCodeFile(string directory, string fileName, 
            CodeCompileUnit targetUnit, string start, string end)
        {
            // Create a new code dom provider in
            // C# code format. Add new options
            // to the code in 'C' format.
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Write the code to the file
            // through the stream writer.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\" + fileName))
            {
                sourceWriter.WriteLine(start);
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
                sourceWriter.WriteLine(end);
            }
        }

        /// <summary>
        /// Deserialise the xml data to the data object.
        /// </summary>
        /// <param name="inputFileContent">The xml string to deserialise.</param>
        /// <returns>The deserialised data contained in the data object.</returns>
        public static CompleteDataObjectContainer ExtractContext(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(CompleteDataObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            CompleteDataObjectContainer data = (CompleteDataObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }

        /// <summary>
        /// Deserialise the xml data to the data object.
        /// </summary>
        /// <param name="inputFileContent">The xml string to deserialise.</param>
        /// <returns>The deserialised data contained in the data object.</returns>
        public static GenerateComm.DatabaseModel ExtractContextDatabaseModel(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(GenerateComm.DatabaseModel));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            GenerateComm.DatabaseModel data = (GenerateComm.DatabaseModel)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }

        /// <summary>
        /// Deserialise the xml data to the data object.
        /// </summary>
        /// <param name="inputFileContent">The xml string to deserialise.</param>
        /// <returns>The deserialised data contained in the data object.</returns>
        public static GenerateComm.DataObjectContainer ExtractDataObjectItem(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(GenerateComm.DataObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            GenerateComm.DataObjectContainer data = (GenerateComm.DataObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }

        /// <summary>
        /// Deserialise the xml data to the data object.
        /// </summary>
        /// <param name="inputFileContent">The xml string to deserialise.</param>
        /// <returns>The deserialised data contained in the data object.</returns>
        public static GenerateComm.LinqObjectContainer ExtractLinqObjectItem(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(GenerateComm.LinqObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            GenerateComm.LinqObjectContainer data = (GenerateComm.LinqObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }

        /// <summary>
        /// Deserialise the xml data to the data object.
        /// </summary>
        /// <param name="inputFileContent">The xml string to deserialise.</param>
        /// <returns>The deserialised data contained in the data object.</returns>
        public static GenerateComm.EnumObjectContainer ExtractEnumObject(string inputFileContent)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(GenerateComm.EnumObjectContainer));

            // Create a new string writer, this
            // is where the generated code will be
            // written to.
            TextReader stringReader = new StringReader(inputFileContent);

            // Uses the Deserialise method to restore the object's state 
            // with data from the XML document.
            GenerateComm.EnumObjectContainer data = (GenerateComm.EnumObjectContainer)deserialiser.Deserialize(stringReader);

            // Return the data.
            return data;
        }

        /// <summary>
        /// Generate the database object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateDataBaseObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // If database objects are to be split.
            if ((long)dataObject.TableListMaxSize > 0)
            {
                // Create a new instance of the data object
                // code generation class and assign the configuration data.
                Generate.CodeDomDatabaseObject codeDomDatabaseObject = new Generate.CodeDomDatabaseObject();

                // Get all the tables and views in the current database.
                IEnumerable<GenerateComm.TablesResult> tables =
                    codeDomDatabaseObject.GetDatabaseTables(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);
                IEnumerable<GenerateComm.TablesResult> views =
                    codeDomDatabaseObject.GetDatabaseViews(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);

                // Joins the tables and views into one object collection.
                List<string> listItems = new List<string>();
                IEnumerable<GenerateComm.TablesResult> dataItemsCol = null;
                if (views != null)
                    dataItemsCol = tables.Concat(views);
                else
                    dataItemsCol = tables;

                // For each table found add the object.
                foreach (var table in dataItemsCol)
                {
                    // If the table is in the list and needs
                    // to be created then generate the table object.
                    if (dataObject.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                        listItems.Add(table.TableName);
                }

                string[] tableListItems = listItems.ToArray();
                bool addRepeat = false;
                long totalObjects = tableListItems.Count();
                long maxListCount = (long)dataObject.TableListMaxSize;
                long repeats = (long)(((decimal)totalObjects) / ((decimal)maxListCount));

                // If a remained exits then add one more count.
                if ((totalObjects % maxListCount) > 0)
                    addRepeat = true;

                if (addRepeat)
                    repeats += 1;

                // For each group of objects create a
                // new database object file.
                for (int i = 0; i < repeats; i++)
                    // Create one database object file.
                    CreateDataBaseObjectContainerItems(directory, xDoc, dataObject,
                        tableListItems.Skip(i * Convert.ToInt32(maxListCount)).Take(Convert.ToInt32(maxListCount)).ToArray(),
                        false, (i + 1).ToString());
            }
            else
                // Create one database object file.
                CreateDataBaseObjectContainerItems(directory, xDoc, dataObject,
                    dataObject.TableList, (bool)dataObject.TableListExclusion, string.Empty);
        }

        /// <summary>
        /// Generate the database object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        /// <param name="tableListItems">The list of data table list items.</param>
        /// <param name="tableListExclusionValue">The list of data table list exclusion.</param>
        /// <param name="fileNumber">The current file number</param>
        private static void CreateDataBaseObjectContainerItems(string directory, System.Xml.Linq.XDocument xDoc,
            CompleteDataObjectContainer dataObject, string[] tableListItems, bool tableListExclusionValue, string fileNumber)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDatabaseXmlObject codeDomDatabaseXmlObject = new Generate.CodeDomDatabaseXmlObject();
            Generate.CodeDomDatabaseXmlModelObject codeDomDatabaseXmlModelObject = new Generate.CodeDomDatabaseXmlModelObject();
            GenerateComm.DataBaseObjectContainer data = new Nequeo.CustomTool.CodeGenerator.Common.DataBaseObjectContainer()
            {
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                TableListExclusion = tableListExclusionValue,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                TableList = tableListItems
            };

            MemoryStream stream = codeDomDatabaseXmlObject.GenerateCode(data);
            byte[] generatedCodeAsBytes = stream.ToArray();

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data"))
                Directory.CreateDirectory(directory + "\\Data");

            // Write the code to the file
            // through the stream writer.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Data\\Database" + fileNumber + ".Schema.xml"))
                sourceWriter.Write(Encoding.ASCII.GetString(generatedCodeAsBytes));

            // Generate the code from the returned
            // generate complie code unit.
            GenerateComm.DatabaseModel databaseModel = ExtractContextDatabaseModel(Encoding.ASCII.GetString(generatedCodeAsBytes));
            GenerateCodeFile(directory + "\\Data", "Database" + fileNumber + ".Schema.Designer.cs", 
                codeDomDatabaseXmlModelObject.GenerateCode(databaseModel), string.Empty, string.Empty);

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Data\\Database" + fileNumber + ".xsd"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("TableListExclusion", xDoc.Element("Context").Element("TableListExclusion").Value),
                    new XElement("TableList", xDoc.Element("Context").Element("TableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }

            if (stream != null)
                stream.Close();
        }

        /// <summary>
        /// Generate the context object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateContextObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDatabaseContext codeDomDatabaseContext = new Generate.CodeDomDatabaseContext();
            GenerateComm.DataBaseContextContainer data = new Nequeo.CustomTool.CodeGenerator.Common.DataBaseContextContainer()
            {
                ConfigKeyDatabaseConnection = dataObject.ConfigKeyDatabaseConnection,
                ContextName = dataObject.ContextName,
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                TableListExclusion = (bool)dataObject.TableListExclusion,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                IncludeContextItems = (bool)dataObject.IncludeContextItems,
                TableList = dataObject.TableList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data"))
                Directory.CreateDirectory(directory + "\\Data");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Data", "DatabaseContext.Designer.cs",
                codeDomDatabaseContext.GenerateCode(data), string.Empty, string.Empty);

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Data\\DatabaseContext.xsd"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                    new XElement("ContextName", xDoc.Element("Context").Element("ContextName").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("IncludeContextItems", xDoc.Element("Context").Element("IncludeContextItems").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("TableListExclusion", xDoc.Element("Context").Element("TableListExclusion").Value),
                    new XElement("TableList", xDoc.Element("Context").Element("TableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the extension context object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateExtensionContextContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDataExtensionContext codeDomDataExtensionContext = new Generate.CodeDomDataExtensionContext();
            GenerateComm.DataContextExtensionContainer data = new Nequeo.CustomTool.CodeGenerator.Common.DataContextExtensionContainer()
            {
                DataContextName = dataObject.ContextName,
                ContextName = dataObject.ExtensionContextName,
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                TableListExclusion = (bool)dataObject.TableListExclusion,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                TableList = dataObject.TableList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data\\Extension"))
                Directory.CreateDirectory(directory + "\\Data\\Extension");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Data\\Extension", dataObject.ExtensionContextName +
                ".Designer.cs", codeDomDataExtensionContext.GenerateCode(data), string.Empty, string.Empty);

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Data\\Extension\\" + dataObject.ExtensionContextName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("DataContextName", xDoc.Element("Context").Element("ContextName").Value),
                    new XElement("ContextName", xDoc.Element("Context").Element("ExtensionContextName").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("TableListExclusion", xDoc.Element("Context").Element("TableListExclusion").Value),
                    new XElement("TableList", xDoc.Element("Context").Element("TableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the database extension object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateDatabaseObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDatabaseObject codeDomDatabaseObject = new Generate.CodeDomDatabaseObject();
            Generate.CodeDomDataExtensionObject codeDomDataExtensionObject = new Generate.CodeDomDataExtensionObject();

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data\\Extension\\Designer"))
                Directory.CreateDirectory(directory + "\\Data\\Extension\\Designer");

            // Get all the tables and views in the current database.
            IEnumerable<GenerateComm.TablesResult> tables =
                codeDomDatabaseObject.GetDatabaseTables(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);
            IEnumerable<GenerateComm.TablesResult> views =
                codeDomDatabaseObject.GetDatabaseViews(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);

            // If tables exist then create the data objects
            if (tables != null)
            {
                // For each table found add the object.
                foreach (var table in tables)
                {
                    // If the table is in the list and needs
                    // to be created then generate the table object.
                    if (dataObject.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\Data\\Extension\\Designer",
                            table.TableName + ".Designer.cs",
                            codeDomDataExtensionObject.GenerateCode(
                                table.TableName,
                                dataObject.ContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Data\\Extension\\Designer\\" + table.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", table.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("DataContextName", xDoc.Element("Context").Element("ContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }

            // If views exist then create the data objects
            if (views != null)
            {
                // For each view found add the object.
                foreach (var view in views)
                {
                    // If the view is in the list and needs
                    // to be created then generate the view object.
                    if (dataObject.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\Data\\Extension\\Designer",
                            view.TableName + ".Designer.cs",
                            codeDomDataExtensionObject.GenerateCode(
                                view.TableName,
                                dataObject.ContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Data\\Extension\\Designer\\" + view.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", view.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("DataContextName", xDoc.Element("Context").Element("ContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate the stored procedure object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateStoredProcedureContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomProcedureObject codeDomProcedureObject = new Generate.CodeDomProcedureObject();
            GenerateComm.ProcedureExtensionContainer data = new Nequeo.CustomTool.CodeGenerator.Common.ProcedureExtensionContainer()
            {
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                ExtensionClassName = dataObject.ProcedureExtensionClassName,
                FunctionHandler = (bool)dataObject.ProcedureFunctionHandler,
                ProcedureListExclusion = (bool)dataObject.ProcedureListExclusion,
                ProcedureList = dataObject.ProcedureList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data\\Extension\\Designer\\Procedure"))
                Directory.CreateDirectory(directory + "\\Data\\Extension\\Designer\\Procedure");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Data\\Extension\\Designer\\Procedure",
                dataObject.ProcedureExtensionClassName + ".Designer.cs", codeDomProcedureObject.GenerateCode(data),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(
                directory + "\\Data\\Extension\\Designer\\Procedure\\" + dataObject.ProcedureExtensionClassName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("ExtensionClassName", xDoc.Element("Context").Element("ProcedureExtensionClassName").Value),
                    new XElement("FunctionHandler", xDoc.Element("Context").Element("ProcedureFunctionHandler").Value),
                    new XElement("ProcedureListExclusion", xDoc.Element("Context").Element("ProcedureListExclusion").Value),
                    new XElement("ProcedureList", xDoc.Element("Context").Element("ProcedureList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the scalar function object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateFunctionScalarContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomFunctionScalarObject codeDomFunctionScalarObject = new Generate.CodeDomFunctionScalarObject();
            GenerateComm.FunctionExtensionContainer data = new Nequeo.CustomTool.CodeGenerator.Common.FunctionExtensionContainer()
            {
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                ExtensionClassName = dataObject.FunctionScalarExtensionClassName,
                FunctionHandler = (bool)dataObject.FunctionScalarFunctionHandler,
                FunctionListExclusion = (bool)dataObject.FunctionScalarListExclusion,
                FunctionList = dataObject.FunctionScalarList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data\\Extension\\Designer\\Function\\Scalar"))
                Directory.CreateDirectory(directory + "\\Data\\Extension\\Designer\\Function\\Scalar");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Data\\Extension\\Designer\\Function\\Scalar",
                dataObject.FunctionScalarExtensionClassName + ".Designer.cs", 
                codeDomFunctionScalarObject.GenerateCode(data),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(
                directory + "\\Data\\Extension\\Designer\\Function\\Scalar\\" + dataObject.FunctionScalarExtensionClassName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("ExtensionClassName", xDoc.Element("Context").Element("FunctionScalarExtensionClassName").Value),
                    new XElement("FunctionHandler", xDoc.Element("Context").Element("FunctionScalarFunctionHandler").Value),
                    new XElement("FunctionListExclusion", xDoc.Element("Context").Element("FunctionScalarListExclusion").Value),
                    new XElement("FunctionList", xDoc.Element("Context").Element("FunctionScalarList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the table function object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateFunctionTableContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomFunctionTableObject codeDomFunctionTableObject = new Generate.CodeDomFunctionTableObject();
            GenerateComm.FunctionExtensionContainer data = new Nequeo.CustomTool.CodeGenerator.Common.FunctionExtensionContainer()
            {
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                ExtensionClassName = dataObject.FunctionTableExtensionClassName,
                FunctionHandler = (bool)dataObject.FunctionTableFunctionHandler,
                FunctionListExclusion = (bool)dataObject.FunctionTableListExclusion,
                FunctionList = dataObject.FunctionTableList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data\\Extension\\Designer\\Function\\Table"))
                Directory.CreateDirectory(directory + "\\Data\\Extension\\Designer\\Function\\Table");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Data\\Extension\\Designer\\Function\\Table",
                dataObject.FunctionTableExtensionClassName + ".Designer.cs", 
                codeDomFunctionTableObject.GenerateCode(data),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(
                directory + "\\Data\\Extension\\Designer\\Function\\Table\\" + dataObject.FunctionTableExtensionClassName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("ExtensionClassName", xDoc.Element("Context").Element("FunctionTableExtensionClassName").Value),
                    new XElement("FunctionHandler", xDoc.Element("Context").Element("FunctionTableFunctionHandler").Value),
                    new XElement("FunctionListExclusion", xDoc.Element("Context").Element("FunctionTableListExclusion").Value),
                    new XElement("FunctionList", xDoc.Element("Context").Element("FunctionTableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the data object item code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The data object item deserialised xml file data</param>
        public static void CreateDataObjectItemContainer(string directory, System.Xml.Linq.XDocument xDoc, GenerateComm.DataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class.
            Generate.CodeDomDataObject codeDomDataObject = new Generate.CodeDomDataObject();

            // If the director does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Data\\Extended\\Designer"))
                Directory.CreateDirectory(directory + "\\Data\\Extended\\Designer");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Data\\Extended\\Designer",
                dataObject.ClassName + ".Designer.cs", codeDomDataObject.GenerateCode(dataObject), string.Empty, string.Empty);
        }

        /// <summary>
        /// Generate the data object item code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The data object item deserialised xml file data</param>
        public static void CreateLinqObjectItemContainer(string directory, System.Xml.Linq.XDocument xDoc, GenerateComm.LinqObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class.
            Generate.CodeDomLinqObject codeDomLinqObject = new Generate.CodeDomLinqObject();

            // If the director does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Linq\\Extended\\Designer"))
                Directory.CreateDirectory(directory + "\\Linq\\Extended\\Designer");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Linq\\Extended\\Designer",
                dataObject.ClassName + ".Designer.cs", codeDomLinqObject.GenerateCode(dataObject), string.Empty, string.Empty);
        }

        /// <summary>
        /// Generate the enum object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The enum object deserialised xml file data</param>
        public static void CreateEnumObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, GenerateComm.EnumObjectContainer dataObject)
        {
            // Create a new instance of the enum object
            // code generation class.
            Generate.CodeDomEnumObject codeDomEnumObject = new Generate.CodeDomEnumObject();

            // If the director does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Enum"))
                Directory.CreateDirectory(directory + "\\Enum");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Enum",
                dataObject.EnumName + ".Designer.cs", codeDomEnumObject.GenerateCode(dataObject), string.Empty, string.Empty);
        }

        /// <summary>
        /// Generate the extension context object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateDataSetExtensionContextContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomSchemaDataContext codeDomSchemaDataContext = new Generate.CodeDomSchemaDataContext();
            GenerateComm.SchemaDataContextObjectContainer data = new Nequeo.CustomTool.CodeGenerator.Common.SchemaDataContextObjectContainer()
            {
                DataContextName = dataObject.DataSetDataContextName,
                ContextName = dataObject.ExtensionContextName,
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                TableListExclusion = (bool)dataObject.TableListExclusion,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                TableList = dataObject.TableList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\DataSet\\Extension"))
                Directory.CreateDirectory(directory + "\\DataSet\\Extension");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\DataSet\\Extension", dataObject.ExtensionContextName +
                ".Designer.cs", codeDomSchemaDataContext.GenerateCode(data), string.Empty, string.Empty);

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\DataSet\\Extension\\" + dataObject.ExtensionContextName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("DataContextName", xDoc.Element("Context").Element("DataSetDataContextName").Value),
                    new XElement("ContextName", xDoc.Element("Context").Element("ExtensionContextName").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("TableListExclusion", xDoc.Element("Context").Element("TableListExclusion").Value),
                    new XElement("TableList", xDoc.Element("Context").Element("TableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the extension context object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateEdmExtensionContextContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomModelDataContext codeDomModelDataContext = new Generate.CodeDomModelDataContext();
            GenerateComm.ModelDataContextObjectContainer data = new Nequeo.CustomTool.CodeGenerator.Common.ModelDataContextObjectContainer()
            {
                DataContextName = dataObject.EdmDataContextName,
                ContextName = dataObject.ExtensionContextName,
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                TableListExclusion = (bool)dataObject.TableListExclusion,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                TableList = dataObject.TableList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Edm\\Extension"))
                Directory.CreateDirectory(directory + "\\Edm\\Extension");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Edm\\Extension", dataObject.ExtensionContextName +
                ".Designer.cs", codeDomModelDataContext.GenerateCode(data), string.Empty, string.Empty);

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Edm\\Extension\\" + dataObject.ExtensionContextName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("DataContextName", xDoc.Element("Context").Element("EdmDataContextName").Value),
                    new XElement("ContextName", xDoc.Element("Context").Element("ExtensionContextName").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("TableListExclusion", xDoc.Element("Context").Element("TableListExclusion").Value),
                    new XElement("TableList", xDoc.Element("Context").Element("TableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the extension context object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateLinqExtensionContextContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomReplicaDataContext codeDomReplicaDataContext = new Generate.CodeDomReplicaDataContext();
            GenerateComm.ReplicaDataContextObjectContainer data = new Nequeo.CustomTool.CodeGenerator.Common.ReplicaDataContextObjectContainer()
            {
                DataContextName = dataObject.LinqDataContextName,
                ContextName = dataObject.ExtensionContextName,
                ConnectionDataType = (int)dataObject.ConnectionDataType,
                ConnectionType = (int)dataObject.ConnectionType,
                DataAccessProvider = dataObject.DataAccessProvider,
                TableListExclusion = (bool)dataObject.TableListExclusion,
                Database = dataObject.Database,
                DataBaseConnect = dataObject.DataBaseConnect,
                DatabaseConnection = dataObject.DatabaseConnection,
                DataBaseOwner = dataObject.DataBaseOwner,
                NamespaceCompanyName = dataObject.NamespaceCompanyName,
                NamespaceExtendedName = dataObject.NamespaceExtendedName,
                TableList = dataObject.TableList
            };

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Linq\\Extension"))
                Directory.CreateDirectory(directory + "\\Linq\\Extension");

            // Generate the code from the returned
            // generate complie code unit.
            GenerateCodeFile(directory + "\\Linq\\Extension", dataObject.ExtensionContextName +
                ".Designer.cs", codeDomReplicaDataContext.GenerateCode(data), string.Empty, string.Empty);

            // Create a stream writer, writer the xml configuration file.
            using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Linq\\Extension\\" + dataObject.ExtensionContextName + ".xml"))
            {
                // Create the xml configuration file.
                XElement xsd = new XElement("Context",
                    new XElement("DatabaseConnection", xDoc.Element("Context").Element("DatabaseConnection").Value),
                    new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                    new XElement("DataContextName", xDoc.Element("Context").Element("LinqDataContextName").Value),
                    new XElement("ContextName", xDoc.Element("Context").Element("ExtensionContextName").Value),
                    new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                    new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                    new XElement("DataBaseConnect", xDoc.Element("Context").Element("DataBaseConnect").Value),
                    new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                    new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                    new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                    new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                    new XElement("TableListExclusion", xDoc.Element("Context").Element("TableListExclusion").Value),
                    new XElement("TableList", xDoc.Element("Context").Element("TableList").Elements("string")));

                // Write the xml data to the specified file.
                sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
            }
        }

        /// <summary>
        /// Generate the database extension object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateDataSetDatabaseObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDatabaseObject codeDomDatabaseObject = new Generate.CodeDomDatabaseObject();
            Generate.CodeDomSchemaObject codeDomSchemaObject = new Generate.CodeDomSchemaObject();

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\DataSet\\Schema\\Designer"))
                Directory.CreateDirectory(directory + "\\DataSet\\Schema\\Designer");

            // Get all the tables and views in the current database.
            IEnumerable<GenerateComm.TablesResult> tables =
                codeDomDatabaseObject.GetDatabaseTables(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);
            IEnumerable<GenerateComm.TablesResult> views =
                codeDomDatabaseObject.GetDatabaseViews(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);

            // If tables exist then create the data objects
            if (tables != null)
            {
                // For each table found add the object.
                foreach (var table in tables)
                {
                    // If the table is in the list and needs
                    // to be created then generate the table object.
                    if (dataObject.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\DataSet\\Schema\\Designer",
                            table.TableName + ".Designer.cs",
                            codeDomSchemaObject.GenerateCode(
                                table.TableName,
                                dataObject.DataSetDataContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (bool)dataObject.DataSetUseAnonymousDataEntity,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\DataSet\\Schema\\Designer\\" + table.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", table.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("DataSetDataContextName", xDoc.Element("Context").Element("DataSetDataContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                                new XElement("UseAnonymousDataEntity", xDoc.Element("Context").Element("DataSetUseAnonymousDataEntity").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }

            // If views exist then create the data objects
            if (views != null)
            {
                // For each view found add the object.
                foreach (var view in views)
                {
                    // If the view is in the list and needs
                    // to be created then generate the view object.
                    if (dataObject.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\DataSet\\Schema\\Designer",
                            view.TableName + ".Designer.cs",
                            codeDomSchemaObject.GenerateCode(
                                view.TableName,
                                dataObject.DataSetDataContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (bool)dataObject.DataSetUseAnonymousDataEntity,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\DataSet\\Schema\\Designer\\" + view.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", view.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("DataSetDataContextName", xDoc.Element("Context").Element("DataSetDataContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                                new XElement("UseAnonymousDataEntity", xDoc.Element("Context").Element("DataSetUseAnonymousDataEntity").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate the database extension object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateEdmDatabaseObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDatabaseObject codeDomDatabaseObject = new Generate.CodeDomDatabaseObject();
            Generate.CodeDomModelObject codeDomModelObject = new Generate.CodeDomModelObject();

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Edm\\Model\\Designer"))
                Directory.CreateDirectory(directory + "\\Edm\\Model\\Designer");

            // Get all the tables and views in the current database.
            IEnumerable<GenerateComm.TablesResult> tables =
                codeDomDatabaseObject.GetDatabaseTables(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);
            IEnumerable<GenerateComm.TablesResult> views =
                codeDomDatabaseObject.GetDatabaseViews(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);

            // If tables exist then create the data objects
            if (tables != null)
            {
                // For each table found add the object.
                foreach (var table in tables)
                {
                    // If the table is in the list and needs
                    // to be created then generate the table object.
                    if (dataObject.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\Edm\\Model\\Designer",
                            table.TableName + ".Designer.cs",
                            codeDomModelObject.GenerateCode(
                                table.TableName,
                                dataObject.EdmDataContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (bool)dataObject.EdmUseAnonymousDataEntity,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Edm\\Model\\Designer\\" + table.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", table.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("EdmDataContextName", xDoc.Element("Context").Element("EdmDataContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                                new XElement("UseAnonymousDataEntity", xDoc.Element("Context").Element("EdmUseAnonymousDataEntity").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }

            // If views exist then create the data objects
            if (views != null)
            {
                // For each view found add the object.
                foreach (var view in views)
                {
                    // If the view is in the list and needs
                    // to be created then generate the view object.
                    if (dataObject.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\Edm\\Model\\Designer",
                            view.TableName + ".Designer.cs",
                            codeDomModelObject.GenerateCode(
                                view.TableName,
                                dataObject.EdmDataContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (bool)dataObject.EdmUseAnonymousDataEntity,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Edm\\Model\\Designer\\" + view.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", view.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("EdmDataContextName", xDoc.Element("Context").Element("EdmDataContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                                new XElement("UseAnonymousDataEntity", xDoc.Element("Context").Element("EdmUseAnonymousDataEntity").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate the database extension object code.
        /// </summary>
        /// <param name="directory">The directory where the code is place.</param>
        /// <param name="xDoc">The ling xdocument containing the xml data.</param>
        /// <param name="dataObject">The complete data object deserialised xml file data</param>
        public static void CreateLinqDatabaseObjectContainer(string directory, System.Xml.Linq.XDocument xDoc, CompleteDataObjectContainer dataObject)
        {
            // Create a new instance of the data object
            // code generation class and assign the configuration data.
            Generate.CodeDomDatabaseObject codeDomDatabaseObject = new Generate.CodeDomDatabaseObject();
            Generate.CodeDomReplicaObject codeDomReplicaObject = new Generate.CodeDomReplicaObject();

            // If the directory does not
            // exits create the directory.
            if (!Directory.Exists(directory + "\\Linq\\Replica\\Designer"))
                Directory.CreateDirectory(directory + "\\Linq\\Replica\\Designer");

            // Get all the tables and views in the current database.
            IEnumerable<GenerateComm.TablesResult> tables =
                codeDomDatabaseObject.GetDatabaseTables(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);
            IEnumerable<GenerateComm.TablesResult> views =
                codeDomDatabaseObject.GetDatabaseViews(dataObject.DatabaseConnection, (int)dataObject.ConnectionType, dataObject.DataBaseOwner);

            // If tables exist then create the data objects
            if (tables != null)
            {
                // For each table found add the object.
                foreach (var table in tables)
                {
                    // If the table is in the list and needs
                    // to be created then generate the table object.
                    if (dataObject.TableList.Contains(table.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\Linq\\Replica\\Designer",
                            table.TableName + ".Designer.cs",
                            codeDomReplicaObject.GenerateCode(
                                table.TableName,
                                dataObject.LinqDataContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (bool)dataObject.LinqUseAnonymousDataEntity,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Linq\\Replica\\Designer\\" + table.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", table.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("LinqDataContextName", xDoc.Element("Context").Element("LinqDataContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                                new XElement("UseAnonymousDataEntity", xDoc.Element("Context").Element("LinqUseAnonymousDataEntity").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }

            // If views exist then create the data objects
            if (views != null)
            {
                // For each view found add the object.
                foreach (var view in views)
                {
                    // If the view is in the list and needs
                    // to be created then generate the view object.
                    if (dataObject.TableList.Contains(view.TableName.ToUpper(), new ToUpperComparer()) == !dataObject.TableListExclusion)
                    {
                        // Generate the code from the returned
                        // generate complie code unit.
                        GenerateCodeFile(directory + "\\Linq\\Replica\\Designer",
                            view.TableName + ".Designer.cs",
                            codeDomReplicaObject.GenerateCode(
                                view.TableName,
                                dataObject.LinqDataContextName,
                                dataObject.Database,
                                dataObject.NamespaceCompanyName,
                                dataObject.ConfigKeyDatabaseConnection,
                                (bool)dataObject.LinqUseAnonymousDataEntity,
                                (int)dataObject.ConnectionType,
                                dataObject.NamespaceExtendedName,
                                (int)dataObject.ConnectionDataType,
                                dataObject.DataAccessProvider),
                                "// Warning 169 (Disables the 'Never used' warning)\r\n" +
                                "#pragma warning disable 169\r\n",
                                "#pragma warning restore 169\r\n");

                        // Create a stream writer, writer the xml configuration file.
                        using (StreamWriter sourceWriter = new StreamWriter(directory + "\\Linq\\Replica\\Designer\\" + view.TableName + ".xml"))
                        {
                            // Create the xml configuration file.
                            XElement xsd = new XElement("Context",
                                new XElement("TableName", view.TableName),
                                new XElement("Database", xDoc.Element("Context").Element("Database").Value),
                                new XElement("LinqDataContextName", xDoc.Element("Context").Element("LinqDataContextName").Value),
                                new XElement("NamespaceCompanyName", xDoc.Element("Context").Element("NamespaceCompanyName").Value),
                                new XElement("NamespaceExtendedName", xDoc.Element("Context").Element("NamespaceExtendedName").Value),
                                new XElement("DataBaseOwner", xDoc.Element("Context").Element("DataBaseOwner").Value),
                                new XElement("ConnectionType", xDoc.Element("Context").Element("ConnectionType").Value),
                                new XElement("ConnectionDataType", xDoc.Element("Context").Element("ConnectionDataType").Value),
                                new XElement("DataAccessProvider", xDoc.Element("Context").Element("DataAccessProvider").Value),
                                new XElement("ConfigKeyDatabaseConnection", xDoc.Element("Context").Element("ConfigKeyDatabaseConnection").Value),
                                new XElement("UseAnonymousDataEntity", xDoc.Element("Context").Element("LinqUseAnonymousDataEntity").Value));

                            // Write the xml data to the specified file.
                            sourceWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + xsd.ToString());
                        }
                    }
                }
            }
        }
        #endregion
    }
}
