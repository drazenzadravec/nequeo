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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Nequeo.Data.MongoDb.CodeDom
{
    /// <summary>
    /// Data model container.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("Context", IsNullable = true)]
    public class BsonModelContainer
    {
        #region Private Fields
        private string _ClassName = null;
        private string _Namespace = null;
        private string[] _PropertyName = null;
        private string[] _PropertyType = null;
        private BsonDocument _BsonDocument = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the class name.
        /// </summary>
        [XmlElement(ElementName = "ClassName", IsNullable = false)]
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        /// <summary>
        /// Gets sets, the namespace.
        /// </summary>
        [XmlElement(ElementName = "Namespace", IsNullable = false)]
        public string Namespace
        {
            get { return _Namespace; }
            set { _Namespace = value; }
        }

        /// <summary>
        /// Gets sets, the Bson document.
        /// </summary>
        [XmlArray(ElementName = "BsonDocument", IsNullable = false)]
        public BsonDocument BsonDocument
        {
            get { return _BsonDocument; }
            set { _BsonDocument = value; }
        }

        /// <summary>
        /// Gets sets, the name of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyName", IsNullable = false)]
        public string[] PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        /// <summary>
        /// Gets sets, the type of the property.
        /// </summary>
        [XmlArray(ElementName = "PropertyType", IsNullable = false)]
        public string[] PropertyType
        {
            get { return _PropertyType; }
            set { _PropertyType = value; }
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Assign the properties.
        /// </summary>
        public void AssignProperties()
        {
            // Get the current row.
            Dictionary<string, object> row = _BsonDocument.ToDictionary();

            // Get all document property names.
            List<string> propertyName = new List<string>();
            List<string> propertyType = new List<string>();

            int index = 0;

            // For each property.
            foreach (KeyValuePair<string, object> id in row)
            {
                int current = 0;

                // Get the name.
                string name = id.Key;
                if (name.ToLower().Contains("_id"))
                    name = "Id";

                // Add the name.
                propertyName.Add(name);

                // Get the document values.
                IEnumerable<BsonValue> values = _BsonDocument.Values;

                // For each Bson value.
                foreach (BsonValue bsonValue in values)
                {
                    // If the global and current index match.
                    if (index == current)
                    {
                        // Map the Bson type tp .Net type.
                        object netValue = BsonTypeMapper.MapToDotNetValue(bsonValue);
                        propertyType.Add(netValue.GetType().ToString());
                    }

                    // Increment the current index.
                    current++;
                }

                // Crement global index.
                index++;
            }

            // Assign the values.
            _PropertyName = propertyName.ToArray();
            _PropertyType = propertyType.ToArray();
        }
        #endregion
    }
}
