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
using System.Data;
using System.Security.Permissions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.Compilation;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Nequeo.Data;
using Nequeo.Data.DataType;
using Nequeo.Handler;
using Nequeo.Data.Configuration;
using Nequeo.Web.Configuration;

namespace Nequeo.Web.HttpHandler
{
    /// <summary>
    /// Generic data http handler allows any data object type data to be returned
    /// from the database for a specified query string passed.
    /// </summary>
    public class GenericData : IHttpHandler
    {
        /// <summary>
        /// Set which http handler should return data.
        /// </summary>
        public object HttpHandlerType = null;

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the IHttpHandler interface.
        /// </summary>
        /// <param name="context">An HttpContext object that provides references to the 
        /// intrinsic server objects (for example, Request, Response, Session, and Server)
        /// used to service HTTP requests.</param>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Get the collection of http handler configuration extensions
                HttpHandlerDataExtensionElement[] httpCollection = HttpHandlerDataConfigurationManager.HttpHandlerExtensionElements();
                if (httpCollection != null)
                {
                    // If http extensions exist
                    if (httpCollection.Count() > 0)
                    {
                        // For each configuration
                        foreach (HttpHandlerDataExtensionElement item in httpCollection)
                        {
                            // Get the current http handler type
                            // and create a instance of the type.
                            Type httpHandlerType = BuildManager.GetType(item.HttpHandlerTypeName, true, true);
                            object httpHandlerTypeInstance = Activator.CreateInstance(httpHandlerType);

                            if (HttpHandlerType == null)
                                HttpHandlerType = this;

                            if (HttpHandlerType != null)
                            {
                                if (HttpHandlerType.GetType().FullName.ToLower() == httpHandlerTypeInstance.GetType().FullName.ToLower())
                                {
                                    // Get the query string associated with this http handler
                                    string value = context.Request.QueryString[item.UrlQueryTextName];

                                    // If a query string is assosicated with
                                    // this http handler then return the
                                    // data from the database and place this
                                    // data in the current session object.
                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        Type dataAccessProviderType = BuildManager.GetType(item.DataAccessProvider, true, true);
                                        ConnectionContext.ConnectionType connectionType = ConnectionContext.ConnectionTypeConverter.GetConnectionType(item.ConnectionType);
                                        ConnectionContext.ConnectionDataType connectionDataType = ConnectionContext.ConnectionTypeConverter.GetConnectionDataType(item.ConnectionDataType);

                                        // Build the current data object type and
                                        // the  select data model generic type.
                                        Type dataType = BuildManager.GetType(item.DataObjectTypeName, true, true);
                                        Type listGenericType = typeof(SelectDataGenericBase<>);

                                        // Create the generic type parameters
                                        // and create the genric type.
                                        Type[] typeArgs = { dataType };
                                        Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);

                                        // Create an instance of the data access provider
                                        Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                                        // Add the genric tyoe contructor parameters
                                        // and create the generic type instance.
                                        object[] parameters = new object[] { item.ConnectionName, connectionType, connectionDataType, dataAccess };
                                        object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                                        PropertyInfo propertyInfo = dataType.GetProperty(item.DataObjectPropertyName);
                                        object valueType = Convert.ChangeType(value, propertyInfo.PropertyType);

                                        // Get the current object.
                                        Object[] args = new Object[] { 
                                            (item.DataObjectPropertyName + " == @0"),
                                            item.ReferenceLazyLoading,
                                            new object[] { valueType } };

                                        // Add the current data row to the
                                        // business object collection.
                                        object ret = listGeneric.GetType().InvokeMember("SelectDataEntitiesPredicate",
                                            BindingFlags.DeclaredOnly | BindingFlags.Public |
                                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                                            null, listGeneric, args);

                                        // Assign the generic collection data
                                        // to the current session sate with
                                        // the unquie configuration name as the key.
                                        Nequeo.Caching.RuntimeCache.Set(item.Name, ret, (double)600.0);
                                    }
                                }
                            }
                            else break;
                        }

                        if (HttpHandlerType != null)
                            // Redirect to the current request.
                            context.Server.Execute(httpCollection[0].ChildPageGroupExecution);
                    }
                }
            }
            catch (Exception ex) 
            { 
                context.AddError(ex);
                LogHandler.WriteTypeMessage(ex.Message, typeof(GenericData).GetMethod("ProcessRequest"));
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the IHttpHandler instance.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
