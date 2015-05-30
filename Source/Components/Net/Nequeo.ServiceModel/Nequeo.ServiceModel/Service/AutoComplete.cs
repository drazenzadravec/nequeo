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
using System.Web;
using System.Web.Services;
using System.Web.Compilation;
using System.Reflection;
using System.Threading;
using System.Linq.Expressions;

using Nequeo.Data;
using Nequeo.Net.ServiceModel.Configuration;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.Configuration;
using Nequeo.Data.DataType;
using Nequeo.Handler.Global;
using Nequeo.Handler;

namespace Nequeo.Net.ServiceModel.Service
{
    /// <summary>
    /// Auto complete web service interface.
    /// </summary>
    [WebService(Name = "AutoComplete", Description = "Auto complete web service")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AutoComplete : System.Web.Services.WebService
    {
        /// <summary>
        /// Set which web service return data.
        /// </summary>
        public object WebServiceType = null;

        /// <summary>
        /// Gets the auto complete company name list from the database.
        /// </summary>
        /// <param name="prefixText">The prefix text to search for.</param>
        /// <param name="count">The number of records to return.</param>
        /// <returns>The string array of company names.</returns>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        [WebMethod]
        public string[] GetList(string prefixText, int count)
        {
            List<string> list = null;

            try
            {
                ConnectionStringExtensionElement[] items = ConnectionStringExtensionConfigurationManager.ConnectionStringExtensionElements();
                if (items != null)
                {
                    if (items.Count() > 0)
                    {
                        // For each service host  configuration find
                        // the corresponding service type.
                        foreach (ConnectionStringExtensionElement item in items)
                        {
                            // Get the current type name
                            // and create a instance of the type.
                            Type typeName = Type.GetType(item.TypeName, true, true);
                            object typeNameInstance = Activator.CreateInstance(typeName);

                            if (WebServiceType == null)
                                WebServiceType = this;

                            if (WebServiceType != null)
                            {
                                if (WebServiceType.GetType().FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                                {
                                    Type dataAccessProviderType = Type.GetType(item.DataAccessProvider, true, true);
                                    ConnectionContext.ConnectionType connectionType = ConnectionContext.ConnectionTypeConverter.GetConnectionType(item.ConnectionType);
                                    ConnectionContext.ConnectionDataType connectionDataType = ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(item.ConnectionDataType);

                                    // Build the current data object type and
                                    // the select data model generic type.
                                    Type dataType = Type.GetType(item.DataObjectTypeName, true, true);
                                    Type listGenericType = typeof(SelectDataGenericBase<>);

                                    // Create the generic type parameters
                                    // and create the genric type.
                                    Type[] typeArgs = { dataType };
                                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                                    // Create an instance of the data access provider
                                    Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                                    // Add the genric type contructor parameters
                                    // and create the generic type instance.
                                    object[] parameters = new object[] { item.ConnectionName, connectionType, connectionDataType, dataAccess };
                                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                                    PropertyInfo propertyInfo = dataType.GetProperty(item.DataObjectPropertyName);
                                    string name = propertyInfo.Name;

                                    // Get the current object.
                                    Object[] args = new Object[] 
                                    { 
                                        0, 
                                        count,
                                        name + " ASC",
                                        "(SqlQueryMethods.Like(" + name + ", \"" + prefixText + "%\"))"
                                    };

                                    // Add the current data row to the
                                    // business object collection.
                                    object ret = listGeneric.GetType().InvokeMember("SelectData",
                                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                                        null, listGeneric, args);

                                    // Cast the data object type as an enumerable object,
                                    // get the enumerator.
                                    System.Collections.IEnumerable itemsRet = (System.Collections.IEnumerable)ret;
                                    System.Collections.IEnumerator dataObjects = itemsRet.GetEnumerator();

                                    // Iterate through the collection.
                                    while (dataObjects.MoveNext())
                                    {
                                        // If the current index equals the
                                        // selected index then return
                                        // the data object type.
                                        // Get the property.
                                        PropertyInfo property = dataObjects.Current.GetType().GetProperty(name);

                                        // Get the current value of the property
                                        string v = property.GetValue(dataObjects.Current, null).ToString();
                                        list.Add(v);
                                    }
                                    dataObjects.Reset();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                string errorMessage = ex.Message;
                LogHandler.WriteTypeMessage(errorMessage, typeof(AutoComplete).GetMethod("GetList"));
            }
            return (list != null ? list.ToArray() : new string[0]);
        }
    }
}
