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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Nequeo.Reflection
{
    /// <summary>
    /// Type converter.
    /// </summary>
    public sealed class TypeConverterExtender
    {
        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataObject">The object array containing the data.</param>
        /// <returns>The array of the type of object.</returns>
        public static T[] GetTypeDataArray<T>(object[] dataObject)
            where T : new()
        {
            int i = 0;
            T[] dataArrary = new T[dataObject.Length];

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // If properties exist within the data type.
            if (properties.Count > 0)
            {
                // Add the type to the list collection.
                foreach (Object item in dataObject)
                {
                    try
                    {
                        // Create a new instance of the
                        // object type.
                        T data = new T();

                        // Get all the properties within the type.
                        PropertyInfo[] propertyTypes = item.GetType().GetProperties();

                        // For each property found add the
                        // property information.
                        foreach (PropertyInfo property in propertyTypes)
                        {
                            try
                            {
                                // Find in the property collection the current property that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                PropertyInfo propertyInfo = null;
                                try
                                {
                                    propertyInfo = properties.First(p => p.Name.ToLower() == property.Name.ToLower());
                                }
                                catch { }
                                if (propertyInfo != null)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (propertyInfo.Name.ToLower() == property.Name.ToLower())
                                    {
                                        // Set the object type property
                                        // with the property found in the
                                        // current object collection.
                                        propertyInfo.SetValue(data, property.GetValue(item, null), null);
                                    }
                                }
                            }
                            catch { }
                        }

                        dataArrary[i++] = data;
                    }
                    catch (System.Exception ex)
                    {
                        // Throw a general exception.
                        throw new System.Exception(ex.Message, ex.InnerException);
                    }
                }
            }

            // Return the collection.
            return dataArrary;
        }

        /// <summary>
        /// Convert all the object data into a list collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataObject">The object array containing the data.</param>
        /// <returns>The list collection of the type of object.</returns>
        public static List<T> GetListTypeData<T>(object[] dataObject)
            where T : new()
        {
            // Create a new instance for the generic data type.
            List<T> dataObjectCollection = new List<T>(dataObject.Length);

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // If properties exist within the data type.
            if (properties.Count > 0)
            {
                // Add the type to the list collection.
                foreach (Object item in dataObject)
                {
                    try
                    {
                        // Create a new instance of the
                        // object type.
                        T data = new T();

                        // Get all the properties within the type.
                        PropertyInfo[] propertyTypes = item.GetType().GetProperties();

                        // For each property found add the
                        // property information.
                        foreach (PropertyInfo property in propertyTypes)
                        {
                            try
                            {
                                // Find in the property collection the current property that matches
                                // the current column. Use the Predicate delegate object to
                                // initiate a search for the specified match.
                                PropertyInfo propertyInfo = null;
                                try
                                {
                                    propertyInfo = properties.First(p => p.Name.ToLower() == property.Name.ToLower());
                                }
                                catch { }
                                if (propertyInfo != null)
                                {
                                    // If the current property within the property collection
                                    // is the current column within the data collection.
                                    if (propertyInfo.Name.ToLower() == property.Name.ToLower())
                                    {
                                        // Set the object type property
                                        // with the property found in the
                                        // current object collection.
                                        propertyInfo.SetValue(data, property.GetValue(item, null), null);
                                    }
                                }
                            }
                            catch { }
                        }

                        // Add the object type to the collection.
                        dataObjectCollection.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        // Throw a general exception.
                        throw new System.Exception(ex.Message, ex.InnerException);
                    }
                }
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Get all public properties within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive properties within.</param>
        /// <returns>The collection of all proerties within the type.</returns>
        private static List<PropertyInfo> GetPublicProperties(Type t)
        {
            // Create a new instance of the property collection.
            List<PropertyInfo> properties = new List<PropertyInfo>();

            // Get the base type and the derived type.
            Type type = t;

            // Add the complete property range.
            properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            // Return all the properties within
            // the type.
            return properties;
        }
    }
}
