using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace Nequeo.CustomTool.CodeGenerator.Code
{
    /// <summary>
    /// Validation class.
    /// </summary>
    public class Validation
    {
        #region Public Static Methods
        /// <summary>
        /// Validate the xml file with the xsd file.
        /// </summary>
        /// <param name="xsdFile">The xsd file containing the schema.</param>
        /// <param name="xmlFile">The xml file to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlFileValid(string xsdFile, string xmlFile, out string errorMessage)
        {
            // Xml text reader.
            XmlTextReader xsdReader = null;
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Validate the filter xml file.
                // Load the xsd file into to xml reader.
                xsdReader = new XmlTextReader(xsdFile);

                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdReader, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlFile);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xsdReader != null)
                    xsdReader.Close();

                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Validate the xml string with the sxd string.
        /// </summary>
        /// <param name="xsdStream">The xsd memory stream containing the schema.</param>
        /// <param name="xmlStream">The xml memory stream to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlValid(MemoryStream xsdStream, MemoryStream xmlStream, out string errorMessage)
        {
            // Xml text reader.
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdStream, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlStream);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Validate the xml file with the xsd file.
        /// </summary>
        /// <param name="xsdString">The xsd string containing the schema.</param>
        /// <param name="xmlFile">The xml file to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlValid(string xsdString, string xmlFile, out string errorMessage)
        {
            // Xml text reader.
            MemoryStream xsdMemReader = null;
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Validate the filter xml file.
                // Load the xsd file into to xml reader.
                xsdMemReader = new MemoryStream(Encoding.ASCII.GetBytes(xsdString));

                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdMemReader, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlFile);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xsdMemReader != null)
                    xsdMemReader.Close();

                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Validate the xml string with the sxd string.
        /// </summary>
        /// <param name="xsdString">The xsd string containing the schema.</param>
        /// <param name="xmlString">The xml string to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlStringValid(string xsdString, string xmlString, out string errorMessage)
        {
            // Xml text reader.
            MemoryStream xsdMemReader = null;
            MemoryStream xmlMemReader = null;
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Validate the filter xml file.
                // Load the xsd file into to xml reader.
                xsdMemReader = new MemoryStream(Encoding.ASCII.GetBytes(xsdString));
                xmlMemReader = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));

                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdMemReader, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlMemReader);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xsdMemReader != null)
                    xsdMemReader.Close();

                if (xmlMemReader != null)
                    xmlMemReader.Close();

                if (xmlReader != null)
                    xmlReader.Close();
            }
        }
        #endregion
    }
}