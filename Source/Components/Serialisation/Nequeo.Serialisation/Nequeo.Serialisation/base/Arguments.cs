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

namespace Nequeo.Serialisation
{
    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class DataSetSerialisationErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the Serialisation class.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public DataSetSerialisationErrorArgs(string exceptionMessage,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.exceptionMessage = exceptionMessage;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The error message within the Serialisation class.
        private string exceptionMessage = string.Empty;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the Serialisation class.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class DataSetSerialisationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="dataSet">The dataset that was Serialised or deSerialised.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public DataSetSerialisationArgs(System.Data.DataSet dataSet,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.dataSet = dataSet;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The dataset that was Serialised or deSerialised.
        private System.Data.DataSet dataSet = null;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the dataset that was Serialised or deserialised.
        /// </summary>
        public System.Data.DataSet DataSet
        {
            get { return dataSet; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class DataTableSerialisationErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the Serialisation class.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public DataTableSerialisationErrorArgs(string exceptionMessage,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.exceptionMessage = exceptionMessage;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The error message within the Serialisation class.
        private string exceptionMessage = string.Empty;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the Serialisation class.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class DataTableSerialisationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="dataTable">The datatable that was Serialised or deSerialised.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public DataTableSerialisationArgs(System.Data.DataTable dataTable,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.dataTable = dataTable;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The datatable that was Serialised or deSerialised.
        private System.Data.DataTable dataTable = null;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the datatable that was Serialised or deSerialised.
        /// </summary>
        public System.Data.DataTable DataTable
        {
            get { return dataTable; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class SerialisationErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the Serialisation class.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public SerialisationErrorArgs(string exceptionMessage,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.exceptionMessage = exceptionMessage;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The error message within the Serialisation class.
        private string exceptionMessage = string.Empty;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the Serialisation class.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class SerialisationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="data">The data that was Serialised or deserialised.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public SerialisationArgs(object data,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.data = data;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The data that was Serialised or deserialised.
        private object data = null;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the datatable that was Serialised or deserialised.
        /// </summary>
        public object Data
        {
            get { return data; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class GenericSerialisationErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the Serialisation class.</param>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public GenericSerialisationErrorArgs(string exceptionMessage,
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.exceptionMessage = exceptionMessage;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The error message within the Serialisation class.
        private string exceptionMessage = string.Empty;
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the Serialisation class.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }

    /// <summary>
    /// Serialisation argument class containing event handler
    /// information for the Serialisation process delegate.
    /// </summary>
    public class GenericSerialisationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the Serialisation process event argument.
        /// </summary>
        /// <param name="serialisationProcess">The original command that was to be processed.</param>
        /// <param name="xmlFile">The file that contains serialised data or where serialised data
        /// is to be placed.</param>
        public GenericSerialisationArgs(
            string serialisationProcess, string xmlFile)
        {
            this.xmlFile = xmlFile;
            this.serialisationProcess = serialisationProcess;
        }
        #endregion

        #region Private Fields
        // The original command that was to be processed.
        private string serialisationProcess = string.Empty;
        // The file that contains serialised data or 
        // where serialised data is to be placed.
        private string xmlFile = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the original command that was to be processed.
        /// </summary>
        public string SerialisationProcess
        {
            get { return serialisationProcess; }
        }

        /// <summary>
        /// Contains the original file that contains serialised data or 
        /// where serialised data is to be placed.
        /// </summary>
        public string XmlFile
        {
            get { return xmlFile; }
        }
        #endregion
    }
}
