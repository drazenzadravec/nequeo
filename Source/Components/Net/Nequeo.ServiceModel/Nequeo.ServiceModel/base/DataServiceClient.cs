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
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// WCF Data service client context.
    /// </summary>
    public abstract class DataServiceClient
    {
        /// <summary>
        /// WCF Data service client context.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        protected DataServiceClient(Uri serviceRoot)
        {
            _serviceRoot = serviceRoot;
            _context = new DataServiceContext(serviceRoot);
        }

        private Uri _serviceRoot = null;
        private System.Data.Services.Client.DataServiceContext _context = null;

        private static char[] _dataPrefix = { 'd', ':' };
        private static char[] _memberPrefix = { 'm', ':' };
        private static string _typePrefix = "Edm.";
        private static string _entryElementName = "entry";
        private static string _contentElementName = "content";
        private static string _memberPropertiesElementName = "m:properties";
        private static string _memberTypePropertiesElementName = "m:type";

        /// <summary>
        /// Member and data name spaces.
        /// </summary>
        private static Nequeo.Model.NameValue[] _namespaces = new Nequeo.Model.NameValue[]
            {
                new Nequeo.Model.NameValue() { Name ="d", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices" },
                new Nequeo.Model.NameValue() { Name ="m", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" },
            };

        /// <summary>
        /// Gets the service root Uri.
        /// </summary>
        public Uri ServiceRoot
        {
            get { return _serviceRoot; }
        }

        /// <summary>
        /// Gets or sets the service credentials.
        /// </summary>
        public NetworkCredential Credentials
        {
            get
            {
                if (_context.Credentials != null)
                    return (NetworkCredential)_context.Credentials;
                else
                    return null;
            }
            set { _context.Credentials = value; }
        }

        /// <summary>
        /// Converts a string to its escaped representation.
        /// </summary>
        /// <param name="stringToEscape">The string to escape.</param>
        /// <returns>A System.String that contains the escaped representation of stringToEscape.</returns>
        protected string EscapeDataString(string stringToEscape)
        {
            return System.Uri.EscapeDataString(stringToEscape);
        }

        /// <summary>
        /// Creates a data service query for data of a specified generic type.
        /// </summary>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        /// <param name="entitySetName">A string that resolves to a URI.</param>
        /// <returns>A new System.Data.Services.Client.DataServiceQuery`1 instance that represents a data service query.</returns>
        protected DataServiceQuery<T> CreateQuery<T>(string entitySetName)
        {
            return _context.CreateQuery<T>(entitySetName);
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>Return the constructed Uri.</returns>
        protected Uri CreateUri(string serviceEntityName)
        {
            return CreateUri(serviceEntityName, null);
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>Return the constructed Uri.</returns>
        protected Uri CreateUri(string serviceEntityName, Nequeo.Model.NameObject[] queries)
        {
            string query = "";

            // If queries exist.
            if (queries != null && queries.Length > 0)
            {
                // Create the query.
                query = Nequeo.Net.Utility.CreateQueryString(queries);
            }

            // Return the URI
            return new Uri(_serviceRoot.AbsoluteUri.TrimEnd('/') + "/" + serviceEntityName + "()" + (String.IsNullOrEmpty(query) ? "" : query));
        }

        /// <summary>
        /// Request data from the service entity name.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>The array of data.</returns>
        public virtual T[] Request<T>(string serviceEntityName)
        {
            return Request<T>(serviceEntityName, null);
        }

        /// <summary>
        /// Request data from the service entity name.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>The array of data.</returns>
        public virtual T[] Request<T>(string serviceEntityName, Nequeo.Model.NameObject[] queries)
        {
            // Create the entity name query.
            DataServiceQuery<T> query;
            query = _context.CreateQuery<T>(serviceEntityName);

            // If queries exist.
            if (queries != null && queries.Length > 0)
            {
                // For each query.
                foreach(Nequeo.Model.NameObject item in queries)
                {
                    // Add the query.
                    query = query.AddQueryOption(item.Name, item.Value);
                }
            }

            // Return the results.
            return query.Execute().ToArray();
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <typeparam name="T">The type to translate the xml data to.</typeparam>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public virtual T[] Response<T>(byte[] xmlData)
        {
            return DataServiceClient.ProcessResponse<T>(xmlData);
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public virtual List<Nequeo.Model.PropertyInfoModel[]> Response(byte[] xmlData)
        {
            return DataServiceClient.ProcessResponse(xmlData);
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public virtual object[] ResponseEx(byte[] xmlData)
        {
            return DataServiceClient.ProcessResponseEx(xmlData);
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <typeparam name="T">The type to translate the xml data to.</typeparam>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public static T[] ProcessResponse<T>(byte[] xmlData)
        {
            List<T> dataList = new List<T>();

            // Get all the properties within the type.
            PropertyInfo[] properties = typeof(T).GetProperties();

            // Crate the document.
            XmlDocument xmlDoc = Nequeo.Xml.Element.Document(xmlData);

            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            foreach (Nequeo.Model.NameValue item in _namespaces)
            {
                // Add the namespace.
                nsmgr.AddNamespace(item.Name, item.Value);
            }

            // Get all the entry nodes from the xml document.
            XmlNodeList propertyNodes = xmlDoc.DocumentElement.GetElementsByTagName(_memberPropertiesElementName);
            foreach (System.Xml.XmlNode propertyNode in propertyNodes)
            {
                // Create a new instance of the type.
                T data = Nequeo.Reflection.TypeAccessor.CreateInstance<T>();

                // If properties exist.
                if (propertyNode.HasChildNodes)
                {
                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo property in properties)
                    {
                        // Find the property in the property node.
                        XmlNode propertyName = propertyNode.SelectSingleNode("d:" + property.Name, nsmgr);
                        if (propertyName != null)
                        {
                            // Get the type.
                            XmlNode propertyTypeNode = propertyName.Attributes.GetNamedItem(_memberTypePropertiesElementName);
                            string typeName = propertyTypeNode.Value;
                            Type type = Nequeo.DataType.GetSystemType(typeName.ToLower().Replace(_typePrefix.ToLower(), ""));

                            // Set the value of the property.
                            property.SetValue(data, propertyName.InnerText);
                        }
                    }
                }

                // Add the type to the collection.
                dataList.Add(data);
            }

            // Return the array.
            return dataList.ToArray();
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public static List<Nequeo.Model.PropertyInfoModel[]> ProcessResponse(byte[] xmlData)
        {
            List<Nequeo.Model.PropertyInfoModel[]> dataList = new List<Nequeo.Model.PropertyInfoModel[]>();

            // Crate the document.
            XmlDocument xmlDoc = Nequeo.Xml.Element.Document(xmlData);

            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            foreach (Nequeo.Model.NameValue item in _namespaces)
            {
                // Add the namespace.
                nsmgr.AddNamespace(item.Name, item.Value);
            }

            // Get all the entry nodes from the xml document.
            XmlNodeList propertyNodes = xmlDoc.DocumentElement.GetElementsByTagName(_memberPropertiesElementName);
            foreach (System.Xml.XmlNode propertyNode in propertyNodes)
            {
                // Create a new instance of the type.
                List<Nequeo.Model.PropertyInfoModel> dataArray = new List<Nequeo.Model.PropertyInfoModel>();

                // If properties exist.
                if (propertyNode.HasChildNodes)
                {
                    // Get all the child nodes, these are all the individual properties.
                    XmlNodeList properties = propertyNode.ChildNodes;
                    foreach (System.Xml.XmlNode property in properties)
                    {
                        // Find the property in the property node.
                        XmlNode propertyName = propertyNode.SelectSingleNode(property.Name, nsmgr);
                        if (propertyName != null)
                        {
                            // Create a new instance of the type.
                            Nequeo.Model.PropertyInfoModel data = new Nequeo.Model.PropertyInfoModel();

                            // Get the type.
                            XmlNode propertyTypeNode = propertyName.Attributes.GetNamedItem(_memberTypePropertiesElementName);
                            string typeName = propertyTypeNode.Value;
                            Type type = Nequeo.DataType.GetSystemType(typeName.ToLower().Replace(_typePrefix.ToLower(), ""));

                            // Set the value of the property.
                            data.PropertyName = property.Name.TrimStart(_dataPrefix);
                            data.PropertyType = type;
                            data.PropertyValue = Convert.ChangeType(propertyName.InnerText, type);

                            // Add to the array.
                            dataArray.Add(data);
                        }
                    }
                }

                // Add the type to the collection.
                dataList.Add(dataArray.ToArray());
            }

            // Return the array.
            return dataList;
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public static object[] ProcessResponseEx(byte[] xmlData)
        {
            int i = 0;
            List<object> dataList = new List<object>();
            Nequeo.Reflection.DynamicTypeBuilder builder = new Reflection.DynamicTypeBuilder("dynamicModule");

            // Crate the document.
            XmlDocument xmlDoc = Nequeo.Xml.Element.Document(xmlData);

            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            foreach (Nequeo.Model.NameValue item in _namespaces)
            {
                // Add the namespace.
                nsmgr.AddNamespace(item.Name, item.Value);
            }

            // Get all the entry nodes from the xml document.
            XmlNodeList propertyNodes = xmlDoc.DocumentElement.GetElementsByTagName(_memberPropertiesElementName);
            foreach (System.Xml.XmlNode propertyNode in propertyNodes)
            {
                // Increment the dymanic object count.
                i++;

                // Create the dynamic types.
                List<Nequeo.Reflection.DynamicPropertyValue> dynamicTypes = new List<Reflection.DynamicPropertyValue>();

                // If properties exist.
                if (propertyNode.HasChildNodes)
                {
                    // Get all the child nodes, these are all the individual properties.
                    XmlNodeList properties = propertyNode.ChildNodes;
                    foreach (System.Xml.XmlNode property in properties)
                    {
                        // Find the property in the property node.
                        XmlNode propertyName = propertyNode.SelectSingleNode(property.Name, nsmgr);
                        if (propertyName != null)
                        {
                            // Get the type.
                            XmlNode propertyTypeNode = propertyName.Attributes.GetNamedItem(_memberTypePropertiesElementName);
                            string typeName = propertyTypeNode.Value;
                            Type type = Nequeo.DataType.GetSystemType(typeName.ToLower().Replace(_typePrefix.ToLower(), ""));

                            // Create a new property type.
                            Nequeo.Reflection.DynamicPropertyValue dynamicProperty =
                                new Reflection.DynamicPropertyValue(property.Name.TrimStart(_dataPrefix), type, Convert.ChangeType(propertyName.InnerText, type));

                            // Add to the array.
                            dynamicTypes.Add(dynamicProperty);
                        }
                    }
                }

                // Create the dynamic type.
                // Create a new instance of the type.
                object data = builder.Create("" + i.ToString(), dynamicTypes);

                // Add the type to the collection.
                dataList.Add(data);
            }

            // Return the array.
            return dataList.ToArray();
        }
    }
}
