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
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Data;

namespace Nequeo.Data.LinqToXml
{
    /// <summary>
    /// Data set xml reader and writer.
    /// </summary>
	public class XmlDataSet
	{
        /// <summary>
        /// Read the xml document into a data set
        /// </summary>
        /// <param name="xmlFile">The full path and file of the xml document to read.</param>
        /// <returns>The data table containing the data.</returns>
        public static DataSet ReadXml(string xmlFile)
        {
            // Load data from XML file
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(xmlFile);
            return dataSet;
        }

        /// <summary>
        /// Write the data table to the xml document.
        /// </summary>
        /// <param name="dataTable">The data table that contains the data.</param>
        /// <param name="xmlFile">The full path and file name of the xml document to write.</param>
        public static void WriteXml(DataTable dataTable, string xmlFile)
        {
            // Load data from XML file
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            dataSet.WriteXml(xmlFile);
        }

        /// <summary>
        /// Write the data set to the xml document.
        /// </summary>
        /// <param name="dataSet">The data table that contains the data.</param>
        /// <param name="xmlFile">The full path and file name of the xml document to write.</param>
        public static void WriteXml(DataSet dataSet, string xmlFile)
        {
            // Load data from XML file
            dataSet.WriteXml(xmlFile);
        }
	}
}
