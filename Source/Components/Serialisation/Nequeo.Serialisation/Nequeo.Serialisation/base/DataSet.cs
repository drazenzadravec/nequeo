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
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.Composition;

namespace Nequeo.Serialisation
{
    #region Public Delegate
    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when dataset Serialisation process occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void DataSetSerialisationHandler(object sender, DataSetSerialisationArgs e);

    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when dataset Serialisation error occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void DataSetSerialisationErrorHandler(object sender, DataSetSerialisationErrorArgs e);
    #endregion

    /// <summary>
    /// Class that Serialises or deserialises a dataset.
    /// </summary>
    [Export(typeof(IDataSetSerialisation))]
    public class DataSetSerialisation : Nequeo.Runtime.DisposableBase, IDisposable, IDataSetSerialisation
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly DataSetSerialisation Instance = new DataSetSerialisation();

        /// <summary>
        /// Static constructor
        /// </summary>
        static DataSetSerialisation() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public DataSetSerialisation()
        {
        }
        #endregion

        #region Public Events
        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to Serialise a dataset.
        /// </summary>
        public event DataSetSerialisationErrorHandler OnSerialisationError;

        /// <summary>
        /// This event occurs when Serialisation of the dataset
        /// has completed through Serialisation.
        /// </summary>
        public event DataSetSerialisationHandler OnSerialisationComplete;

        /// <summary>
        /// This event occurs when deSerialisation of the dataset
        /// has completed through Serialisation.
        /// </summary>
        public event DataSetSerialisationHandler OnDeserialisationComplete;
        #endregion

        #region Public Methods
        /// <summary>
        /// Deserialises data from the specified byte array.
        /// </summary>
        /// <param name="serialData">The serialised data to deserialise.</param>
        /// <returns>The deserialised dataset.</returns>
        public virtual System.Data.DataSet Deserialise(Byte[] serialData)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deSerialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(System.Data.DataSet));

            // If the data has been altered with unknown 
            // nodes or attributes, handles them with the 
            // UnknownNode, UnknownAttribute and UnknownElement events.
            deserialiser.UnknownNode += new XmlNodeEventHandler(deserialiser_UnknownNode);
            deserialiser.UnknownAttribute += new XmlAttributeEventHandler(deserialiser_UnknownAttribute);
            deserialiser.UnknownElement += new XmlElementEventHandler(deserialiser_UnknownElement);

            // A memory stream is needed.
            MemoryStream memoryStream = null;

            // Declares an object variable of the 
            // type to be deserialised.
            System.Data.DataSet dataSet = null;

            try
            {
                // Create a new instance of the memory stream.
                using (memoryStream = new MemoryStream(serialData))
                {
                    // Uses the deserialise method to restore the object's state 
                    // with data from the byte array.
                    dataSet = (System.Data.DataSet)deserialiser.Deserialize(memoryStream);

                    // Close the memory stream before disposing.
                    memoryStream.Close();

                    // Create a new event handler for
                    // the current event, send to the client.
                    this.CompletionEventHandler(dataSet, "Deserialise",
                        string.Empty, Nequeo.Serialisation.EventType.Deserialise);

                    // Return the deserialised byte array into
                    // the specified object.
                    return dataSet;
                }
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the Serialisation objects.
                this.ExceptionHandler(ex, "Deserialise",
                    "Deserialise from the byte array.\n" + ex.Message, string.Empty);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up.
                if (memoryStream != null)
                    memoryStream.Close();
            }
        }

        /// <summary>
        /// Deserialises data from the specified file.
        /// </summary>
        /// <param name="xmlFile">The full file and path of the file that contains
        /// the Serialised data.</param>
        /// <returns>The deserialised dataset.</returns>
        public virtual System.Data.DataSet Deserialise(string xmlFile)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deSerialised.
            XmlSerializer deserialiser = new XmlSerializer(typeof(System.Data.DataSet));

            // If the XML document has been altered with unknown 
            // nodes or attributes, handles them with the 
            // UnknownNode, UnknownAttribute and UnknownElement events.
            deserialiser.UnknownNode += new XmlNodeEventHandler(deserialiser_UnknownNode);
            deserialiser.UnknownAttribute += new XmlAttributeEventHandler(deserialiser_UnknownAttribute);
            deserialiser.UnknownElement += new XmlElementEventHandler(deserialiser_UnknownElement);

            // A file stream is needed to read the XML document.
            FileStream fileStream = null;

            // Declares an object variable of the 
            // type to be deserialised.
            System.Data.DataSet dataSet = null;

            try
            {
                // Extract the directory path.
                string sFolderPath = System.IO.Path.GetDirectoryName(xmlFile);

                // If the directory does not exists create it.
                if (!Directory.Exists(sFolderPath))
                    Directory.CreateDirectory(sFolderPath);

                // Create a new instance of the file stream.
                using (fileStream = new FileStream(xmlFile, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite))
                {
                    // Uses the DeSerialise method to restore the object's state 
                    // with data from the XML document.
                    dataSet = (System.Data.DataSet)deserialiser.Deserialize(fileStream);

                    // Close the file before disposing.
                    fileStream.Close();

                    // Create a new event handler for
                    // the current event, send to the client.
                    this.CompletionEventHandler(dataSet, "Deserialise",
                        xmlFile, Nequeo.Serialisation.EventType.Deserialise);

                    // Return the deserialised xml into
                    // the specified dataset.
                    return dataSet;
                }
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the Serialisation objects.
                this.ExceptionHandler(ex, "Deserialise",
                    "Deserialise from the XML document.\n" + ex.Message, xmlFile);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up.
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Serialises data to the specified byte array.
        /// </summary>
        /// <param name="dataSet">The dataset to serilaize.</param>
        /// <returns>True if the dataset was Serialised else false.</returns>
        public virtual Byte[] Serialise(System.Data.DataSet dataSet)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deserialised.
            XmlSerializer serialiser = new XmlSerializer(typeof(System.Data.DataSet));

            // If the XML document has been altered with unknown 
            // nodes or attributes, handles them with the 
            // UnknownNode, UnknownAttribute and UnknownElement events.
            serialiser.UnknownNode += new XmlNodeEventHandler(serialiser_UnknownNode);
            serialiser.UnknownAttribute += new XmlAttributeEventHandler(serialiser_UnknownAttribute);
            serialiser.UnknownElement += new XmlElementEventHandler(serialiser_UnknownElement);

            // A memory stream is needed.
            MemoryStream memoryStream = null;

            // Will contain the serialised data
            // from the memory stream.
            Byte[] serialisedData = null;

            try
            {
                // Create a new instance of the memory stream.
                using (memoryStream = new MemoryStream())
                {
                    // Uses the serialise method to restore the object's state 
                    // with data from the byte array.
                    serialiser.Serialize(memoryStream, dataSet);

                    // Get an array of bytes from the MemoryStream 
                    // that holds the serialised data.
                    serialisedData = memoryStream.ToArray();

                    // Close the file before disposing.
                    memoryStream.Close();

                    // Create a new event handler for
                    // the current event, send to the client.
                    this.CompletionEventHandler(dataSet, "Serialise",
                        string.Empty, Nequeo.Serialisation.EventType.Serialise);

                    // True if serialised.
                    return serialisedData;
                }
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the serialisation objects.
                this.ExceptionHandler(ex, "Serialise",
                    "Serialise to the byte array.\n" + ex.Message, string.Empty);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up.
                if (memoryStream != null)
                    memoryStream.Close();
            }
        }

        /// <summary>
        /// Serialises data to the specified file.
        /// </summary>
        /// <param name="dataSet">The dataset to serilaize.</param>
        /// <param name="xmlFile">The full file and path where the Serialised data
        /// dhould be written to.</param>
        /// <returns>True if the dataset was Serialised else false.</returns>
        public virtual bool Serialise(System.Data.DataSet dataSet, string xmlFile)
        {
            // Creates an instance of the XmlSerialiser class;
            // specifies the type of object to be deSerialised.
            XmlSerializer serialiser = new XmlSerializer(typeof(System.Data.DataSet));

            // If the XML document has been altered with unknown 
            // nodes or attributes, handles them with the 
            // UnknownNode, UnknownAttribute and UnknownElement events.
            serialiser.UnknownNode += new XmlNodeEventHandler(serialiser_UnknownNode);
            serialiser.UnknownAttribute += new XmlAttributeEventHandler(serialiser_UnknownAttribute);
            serialiser.UnknownElement += new XmlElementEventHandler(serialiser_UnknownElement);

            // A file stream is needed to read the XML document.
            FileStream fileStream = null;

            try
            {
                // Extract the directory path.
                string sFolderPath = System.IO.Path.GetDirectoryName(xmlFile);

                // If the directory does not exists create it.
                if (!Directory.Exists(sFolderPath))
                    Directory.CreateDirectory(sFolderPath);

                // Create a new instance of the file stream.
                using (fileStream = new FileStream(xmlFile, FileMode.Create,
                    FileAccess.Write, FileShare.ReadWrite))
                {
                    // Uses the Serialise method to restore the object's state 
                    // with data from the XML document.
                    serialiser.Serialize(fileStream, dataSet);

                    // Close the file before disposing.
                    fileStream.Close();

                    // Create a new event handler for
                    // the current event, send to the client.
                    this.CompletionEventHandler(dataSet, "Serialise",
                        xmlFile, Nequeo.Serialisation.EventType.Serialise);

                    // True if serialised.
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the Serialisation objects.
                this.ExceptionHandler(ex, "Serialise",
                    "Serialise to the XML document.\n" + ex.Message, xmlFile);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up.
                if (fileStream != null)
                    fileStream.Close();
            }
        }
        #endregion

        #region Private Error Handler Methods
        /// <summary>
        /// The general exception handler.
        /// </summary>
        /// <param name="e">The current eception object.</param>
        /// <param name="serialMethod">The current Serialisation process.</param>
        /// <param name="serialCustomError">The current custom exception.</param>
        /// <param name="xmlFile">The file that contains serialised data 
        /// or where serialised data is to be placed.</param>
        private void ExceptionHandler(System.Exception e,
            string serialMethod, string serialCustomError, string xmlFile)
        {
            // Detect a thread abort exception.
            if (e is ThreadAbortException)
                Thread.ResetAbort();

            // Make sure than a receiver instance of the
            // event delegate handler was created.
            if (OnSerialisationError != null)
                OnSerialisationError(this, new DataSetSerialisationErrorArgs(
                    serialCustomError, serialMethod, xmlFile));
        }
        #endregion

        #region Private Event Handler Methods
        /// <summary>
        /// The general event completion handler.
        /// </summary>
        /// <param name="dataSet">The current dataset containing the data.</param>
        /// <param name="serialMethod">The current Serialisation process.</param>
        /// <param name="xmlFile">The file that contains serialised data 
        /// or where serialised data is to be placed.</param>
        /// <param name="eventType">The event type to create.</param>
        private void CompletionEventHandler(System.Data.DataSet dataSet,
            string serialMethod, string xmlFile, Nequeo.Serialisation.EventType eventType)
        {
            switch (eventType)
            {
                case Nequeo.Serialisation.EventType.Deserialise:
                    // Make sure than a receiver instance of the
                    // event delegate handler was created.
                    if (OnDeserialisationComplete != null)
                        OnDeserialisationComplete(this,
                            new DataSetSerialisationArgs(dataSet, serialMethod, xmlFile));
                    break;
                case Nequeo.Serialisation.EventType.Serialise:
                    // Make sure than a receiver instance of the
                    // event delegate handler was created.
                    if (OnSerialisationComplete != null)
                        OnSerialisationComplete(this,
                            new DataSetSerialisationArgs(dataSet, serialMethod, xmlFile));
                    break;
            }
        }
        #endregion

        #region Serialisation Events
        /// <summary>
        /// This event is thrown if the element is not known.
        /// </summary>
        /// <param name="sender">The current object sender.</param>
        /// <param name="e">Element event argument.</param>
        void deserialiser_UnknownElement(object sender, XmlElementEventArgs e)
        {
            System.Xml.XmlElement xmlElement = e.Element;

            // Throw a general exception.
            throw new System.Exception(
                "Unknown Element : " + xmlElement.Name + " = " + xmlElement.Value);
        }

        /// <summary>
        /// This event is thrown if the attribute is not known.
        /// </summary>
        /// <param name="sender">The current object sender.</param>
        /// <param name="e">Attribute event argument.</param>
        void deserialiser_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute xmlAttribute = e.Attr;

            // Throw a general exception.
            throw new System.Exception(
                "Unknown Attribute " + xmlAttribute.Name + " = " + xmlAttribute.Value);
        }

        /// <summary>
        /// This event is thrown if the node is not known.
        /// </summary>
        /// <param name="sender">The current object sender.</param>
        /// <param name="e">Node event argument.</param>
        void deserialiser_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            // Throw a general exception.
            throw new System.Exception(
                "Unknown Node : " + e.Name + " Text : " + e.Text +
                "Line Number : " + e.LineNumber.ToString() +
                " Line Position : " + e.LinePosition.ToString());
        }

        /// <summary>
        /// This event is thrown if the element is not known.
        /// </summary>
        /// <param name="sender">The current object sender.</param>
        /// <param name="e">Element event argument.</param>
        void serialiser_UnknownElement(object sender, XmlElementEventArgs e)
        {
            System.Xml.XmlElement xmlElement = e.Element;

            // Throw a general exception.
            throw new System.Exception(
                "Unknown Element : " + xmlElement.Name + " = " + xmlElement.Value);
        }

        /// <summary>
        /// This event is thrown if the attribute is not known.
        /// </summary>
        /// <param name="sender">The current object sender.</param>
        /// <param name="e">Attribute event argument.</param>
        void serialiser_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute xmlAttribute = e.Attr;

            // Throw a general exception.
            throw new System.Exception(
                "Unknown Attribute " + xmlAttribute.Name + " = " + xmlAttribute.Value);
        }

        /// <summary>
        /// This event is thrown if the node is not known.
        /// </summary>
        /// <param name="sender">The current object sender.</param>
        /// <param name="e">Node event argument.</param>
        void serialiser_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            // Throw a general exception.
            throw new System.Exception(
                "Unknown Node : " + e.Name + " Text : " + e.Text +
                "Line Number : " + e.LineNumber.ToString() +
                " Line Position : " + e.LinePosition.ToString());
        }
        #endregion
    }

    /// <summary>
    /// Interface that Serialises or deserialises a dataset.
    /// </summary>
    public interface IDataSetSerialisation
    {
        #region Public Methods
        /// <summary>
        /// Deserialises data from the specified byte array.
        /// </summary>
        /// <param name="serialData">The serialised data to deserialise.</param>
        /// <returns>The deserialised dataset.</returns>
        System.Data.DataSet Deserialise(Byte[] serialData);

        /// <summary>
        /// Deserialises data from the specified file.
        /// </summary>
        /// <param name="xmlFile">The full file and path of the file that contains
        /// the Serialised data.</param>
        /// <returns>The deserialised dataset.</returns>
        System.Data.DataSet Deserialise(string xmlFile);

        /// <summary>
        /// Serialises data to the specified byte array.
        /// </summary>
        /// <param name="dataSet">The dataset to serilaize.</param>
        /// <returns>True if the dataset was Serialised else false.</returns>
        Byte[] Serialise(System.Data.DataSet dataSet);

        /// <summary>
        /// Serialises data to the specified file.
        /// </summary>
        /// <param name="dataSet">The dataset to serilaize.</param>
        /// <param name="xmlFile">The full file and path where the Serialised data
        /// dhould be written to.</param>
        /// <returns>True if the dataset was Serialised else false.</returns>
        bool Serialise(System.Data.DataSet dataSet, string xmlFile);

        #endregion
    }
}