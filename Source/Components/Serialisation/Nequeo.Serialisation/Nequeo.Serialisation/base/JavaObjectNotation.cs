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
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

using Nequeo.Extension;

namespace Nequeo.Serialisation
{
    /// <summary>
    /// JavaScript Object Notation Serialisation.
    /// </summary>
    public sealed class JavaObjectNotation
    {
        #region Public Static Methods
        /// <summary>
        /// Gets the JSON serialized string for an object with the [DataContract] attribute.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="data">The data object the serilaise.</param>
        /// <returns>The string of serialised data.</returns>
        /// <remarks>Encoding type is UTF8</remarks>
        public static string JSonDataContractSerializer<T>(T data)
        {
            MemoryStream stream = null;
            string json = string.Empty;
            
            try
            {
                // Create a new instance of the serialiser
                DataContractJsonSerializer serialise = new DataContractJsonSerializer(typeof(T));

                // Create a new memory stream that is
                // used to serialise the data.
                using (stream = new MemoryStream())
                {
                    serialise.WriteObject(stream, data);
                    json = Encoding.UTF8.GetString(stream.ToArray());
                    stream.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return json;
        }

        /// <summary>
        /// Converts the specified JSON string to an object of type T for an object with the [DataContract] attribute.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="serialisedJSon">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        /// <remarks>Encoding type is UTF8</remarks>
        public static T JSonDataContractDeserializer<T>(string serialisedJSon)
        {
            MemoryStream stream = null;
            T objectData = default(T);

            try
            {
                // Create a new instance of the serialiser
                DataContractJsonSerializer serialise = new DataContractJsonSerializer(typeof(T));

                // Create a new memory stream that is
                // used to deserialise the data.
                using (stream = new MemoryStream(Encoding.UTF8.GetBytes(serialisedJSon)))
                {
                    objectData = (T)serialise.ReadObject(stream);
                    stream.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            // Return the T object type.
            return objectData;
        }

        /// <summary>
        /// Serialises the object to a JSON format.
        /// </summary>
        /// <param name="data">The data to serialise.</param>
        /// <returns>The serialised data.</returns>
        public static string JSonCustomSerializer(object data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            // Create a new string builder.
            StringBuilder builder = new StringBuilder();

            // Append the prefix name.
            builder.Append("{");
            JSonCustomSerializerEx(data, builder);

            // Return the formatted data.
            return builder.ToString().Trim().TrimEnd(',') + "}";
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <typeparam name="T">The type to deserialise the data to.</typeparam>
        /// <param name="serialisedJSon">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public static T JSonCustomDeserializer<T>(string serialisedJSon)
        {
            // Make sure the page reference exists.
            if (serialisedJSon == null) throw new ArgumentNullException("serialisedJSon");

            if (!Nequeo.Invention.Validation.IsJSonData(serialisedJSon))
                throw new Exception("The serialised json data is not valid.");

            string regexSerialisedJSon = Regex.Replace(serialisedJSon, @"]]\s+\,", "]],");
            string regexSerialisedJSon1 = Regex.Replace(regexSerialisedJSon, @"]\s+\,", "],");
            string regexSerialisedJSon2 = Regex.Replace(regexSerialisedJSon1, @"}]\s+\,", "}],");
            string regexSerialisedJSon3 = Regex.Replace(regexSerialisedJSon2, @"}\s+\,", "},");
            string regexSerialisedJSon4 = Regex.Replace(regexSerialisedJSon3, @"}\s+\,\s+{", "},{");

            // Get all the properties within the type.
            // Create a new instance of the type.
            PropertyInfo[] properties = typeof(T).GetProperties();
            T dataObject = Nequeo.Reflection.TypeAccessor.CreateInstance<T>();

            // Split the data into property names.
            string[] splitItems = regexSerialisedJSon4.TrimStart('{').TrimEnd('}').Split(new string[] { ":" }, 2, StringSplitOptions.None);

            // Start the itertive deseriliastion.
            JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), properties, dataObject);

            // Return the object type with the data.
            return dataObject;
        }

        /// <summary>
        /// Formats the two dimensional array of data into the JSON equivalent.
        /// </summary>
        /// <param name="array">The array containg the data.</param>
        /// <param name="prefixName">The prefix name for the JSON format.</param>
        /// <returns>The JSON serilaised formatted data.</returns>
        public static string JSonTwoDimensionDataFormatter(Array array, string prefixName)
        {
            // Make sure the page reference exists.
            if (array == null) throw new ArgumentNullException("array");
            if (prefixName == null) throw new ArgumentNullException("prefixName");

            return JSonTwoDimensionDataFormatter(array, prefixName, array.GetLength(0), array.GetLength(1));
        }

        /// <summary>
        /// Formats the two dimensional array of data into the JSON equivalent.
        /// </summary>
        /// <param name="array">The array containg the data.</param>
        /// <param name="prefixName">The prefix name for the JSON format.</param>
        /// <param name="iTotal">The number of i elements to return from the array.</param>
        /// <param name="jTotal">The number of j elements to return from the array.</param>
        /// <returns>The JSON serilaised formatted data.</returns>
        public static string JSonTwoDimensionDataFormatter(Array array, string prefixName, int iTotal, int jTotal)
        {
            // Make sure the page reference exists.
            if (array == null) throw new ArgumentNullException("array");
            if (prefixName == null) throw new ArgumentNullException("prefixName");

            // Create a new string builder.
            StringBuilder builder = new StringBuilder();

            if (array != null)
            {
                // Get the total deminsion of the array.
                int ieTotal = (array.GetLength(0) <= iTotal ? array.GetLength(0) : iTotal);
                int jeTotal = (array.GetLength(1) <= jTotal ? array.GetLength(1) : jTotal);

                // Append the prefix name.
                builder.Append("\"" + prefixName + "\":[");

                // For each i element
                for (int i = 0; i < ieTotal; i++)
                {
                    // For each j element
                    builder.Append("[");
                    for (int j = 0; j < jeTotal; j++)
                    {
                        // Append the array index data.
                        builder.Append(AppendData(array.GetValue(i, j), array.GetType()));

                        // If it is the last j element.
                        if (j != (jeTotal - 1))
                            builder.Append(",");
                    }
                    builder.Append("]");

                    // If it is the last i element.
                    if (i != (ieTotal - 1))
                        builder.Append(",");
                }
                builder.Append("]");
            }

            // Return the formatted data.
            return builder.ToString();
        }

        /// <summary>
        /// Formats the one dimensional array of data into the JSON equivalent.
        /// </summary>
        /// <param name="array">The array containg the data.</param>
        /// <param name="prefixName">The prefix name for the JSON format.</param>
        /// <returns>The JSON serilaised formatted data.</returns>
        public static string JSonOneDimensionDataFormatter(Array array, string prefixName)
        {
            // Make sure the page reference exists.
            if (array == null) throw new ArgumentNullException("array");
            if (prefixName == null) throw new ArgumentNullException("prefixName");

            return JSonOneDimensionDataFormatter(array, prefixName, array.GetLength(0));
        }

        /// <summary>
        /// Formats the one dimensional array of data into the JSON equivalent.
        /// </summary>
        /// <param name="array">The array containg the data.</param>
        /// <param name="prefixName">The prefix name for the JSON format.</param>
        /// <param name="iTotal">The number of i elements to return from the array.</param>
        /// <returns>The JSON serilaised formatted data.</returns>
        public static string JSonOneDimensionDataFormatter(Array array, string prefixName, int iTotal)
        {
            // Make sure the page reference exists.
            if (array == null) throw new ArgumentNullException("array");
            if (prefixName == null) throw new ArgumentNullException("prefixName");

            // Create a new string builder.
            StringBuilder builder = new StringBuilder();

            if (array != null)
            {
                // Get the total deminsion of the array.
                int ieTotal = (array.GetLength(0) <= iTotal ? array.GetLength(0) : iTotal);

                // Append the prefix name.
                builder.Append("\"" + prefixName + "\":[");

                // For each i element
                for (int i = 0; i < ieTotal; i++)
                {
                    // Append the array index data.
                    builder.Append(AppendData(array.GetValue(i), array.GetType()));

                    // If it is the last i element.
                    if (i != (ieTotal - 1))
                        builder.Append(",");
                }
                builder.Append("]");
            }

            // Return the formatted data.
            return builder.ToString();
        }

        /// <summary>
        /// Gets the JSON serialized string.
        /// </summary>
        /// <param name="data">The data object the serilaise.</param>
        /// <returns>The string of serialised data.</returns>
        public static string JavaScriptSerializer(object data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            // Create a new json Serialiser
            JavaScriptSerializer serialise = new JavaScriptSerializer();
            return serialise.Serialize(data);
        }

        /// <summary>
        /// Gets the JSON serialized string.
        /// </summary>
        /// <param name="data">The data object the serilaise.</param>
        /// <param name="builder">The System.Text.StringBuilder object that is used to write the JSON string.</param>
        public static void JavaScriptSerializer(object data, StringBuilder builder)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");
            if (builder == null) throw new ArgumentNullException("builder");

            // Create a new json Serialiser
            JavaScriptSerializer serialise = new JavaScriptSerializer();
            serialise.Serialize(data, builder);
        }

        /// <summary>
        /// Converts the specified JSON string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="serialisedJSon">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public static T JavaScriptDeserializer<T>(string serialisedJSon)
        {
            // Make sure the page reference exists.
            if (serialisedJSon == null) throw new ArgumentNullException("serialisedJSon");

            // Create a new json Serialiser
            JavaScriptSerializer serialise = new JavaScriptSerializer();
            return serialise.Deserialize<T>(serialisedJSon);
        }

        /// <summary>
        /// Converts the specified JSON string to an object.
        /// </summary>
        /// <param name="serialisedJSon">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public static object JavaScriptDeserializer(string serialisedJSon)
        {
            // Make sure the page reference exists.
            if (serialisedJSon == null) throw new ArgumentNullException("serialisedJSon");

            // Create a new json Serialiser
            JavaScriptSerializer serialise = new JavaScriptSerializer();
            return serialise.DeserializeObject(serialisedJSon);
        }

        /// <summary>
        /// Gets the JSON serialized string.
        /// </summary>
        /// <param name="data">The data object the serilaise.</param>
        /// <returns>The string of serialised data.</returns>
        public static string Serializer(object data)
        {
            // Make sure the page reference exists.
            if (data == null) throw new ArgumentNullException("data");

            // Create a new json Serialiser
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Converts the specified JSON string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="serialisedJSon">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserializer<T>(string serialisedJSon)
        {
            // Make sure the page reference exists.
            if (serialisedJSon == null) throw new ArgumentNullException("serialisedJSon");

            // Create a new json Serialiser
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serialisedJSon);
        }

        /// <summary>
        /// Converts the specified JSON string to an object.
        /// </summary>
        /// <param name="serialisedJSon">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserializer(string serialisedJSon)
        {
            // Make sure the page reference exists.
            if (serialisedJSon == null) throw new ArgumentNullException("serialisedJSon");

            // Create a new json Serialiser
            return Newtonsoft.Json.JsonConvert.DeserializeObject(serialisedJSon);
        }
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Append the type data to the JSON object.
        /// </summary>
        /// <param name="value">The property value.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The equivalent value.</returns>
        private static object AppendData(object value, Type propertyType)
        {
            // Get the JSON value conversion.
            if (value != null)
                return Nequeo.Serialisation.Data.Json.GetJavaObjectNotationValue(propertyType, value);
            else
                return "null";
        }

        /// <summary>
        /// Creates a new object instance or new array instance.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="properties">The properites collection in the object.</param>
        /// <param name="arrayLength1">The length of the first dimension.</param>
        /// <param name="arrayLength2">The length of the second dimension. If zero then only one dimension is created else two dimensions are created.</param>
        /// <param name="createSingleType">Should a single type be created.</param>
        /// <returns>The new object instance.</returns>
        private static object CreateMemberType(string member, PropertyInfo[] properties, int arrayLength1, int arrayLength2, bool createSingleType)
        {
            // Find the name with the property name.
            IEnumerable<PropertyInfo> members = properties.Where(u => u.Name.ToLower() == member.ToLower());
            
            // If data has been found.
            if (member != null)
            {
                if (member.Count() > 0)
                {
                    // Get the first property in the type.
                    PropertyInfo info = members.First();

                    // Create the instance type.
                    if (info.PropertyType.IsArray)
                    {
                        if (createSingleType)
                        {
                            // Get the non array type of the array base.
                            Type singleType = Nequeo.Reflection.TypeAccessor.GetType(
                                info.PropertyType.FullName.Replace(" ", "").TrimEnd(']').TrimEnd(',').TrimEnd('[') + "," +
                                info.PropertyType.Assembly.FullName.Trim());

                            // Create the single type.
                            return Nequeo.Reflection.TypeAccessor.CreateInstance(singleType);
                        }
                        else
                        {
                            // Get the non array type of the array base.
                            Type singleType = Nequeo.Reflection.TypeAccessor.GetType(
                                info.PropertyType.FullName.Replace(" ", "").TrimEnd(']').TrimEnd(',').TrimEnd('[') + "," +
                                info.PropertyType.Assembly.FullName.Trim());

                            // If creating a two dimensional array.
                            if (arrayLength2 > 0)
                                return Nequeo.Reflection.TypeAccessor.CreateArrayInstance(singleType, arrayLength1, arrayLength2);
                            else
                                // Create the one dimensional array.
                                return Nequeo.Reflection.TypeAccessor.CreateArrayInstance(singleType, arrayLength1);
                        }
                    }
                    else
                        // Create a new object type instance.
                        return Nequeo.Reflection.TypeAccessor.CreateInstance(info.PropertyType);
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Formats the one dimensional array of data into the JSON equivalent.
        /// </summary>
        /// <param name="array">The array containg the data.</param>
        /// <param name="memberName">The member name for the JSON format.</param>
        /// <param name="builder">The string builder used to construct the JSON.</param>
        /// <returns>The JSON serilaised formatted data.</returns>
        private static void JSonDataFormatterEx(Array array, string memberName, StringBuilder builder)
        {
            if (array != null)
            {
                // Get the total deminsion of the array.
                int ieTotal = array.GetLength(0);

                // Append the prefix name.
                builder.Append("\"" + memberName + "\":[");

                // For each i element
                for (int i = 0; i < ieTotal; i++)
                {
                    // Get the value type.
                    object returnValue = AppendData(array.GetValue(i), array.GetType());

                    // If value is null then not an primative object type
                    // else the current member is a object.
                    if (returnValue != null)
                    {
                        // Append the array index data.
                        builder.Append(returnValue);
                    }
                    else
                    {
                        builder.Append("{");
                        JSonCustomSerializerEx(array.GetValue(i), builder);

                        // Remove the last ',' comma in the string.
                        builder.Remove(builder.Length - 1, 1);

                        // Close the current object.
                        builder.Append("}");
                    }

                    // If it is the last i element.
                    if (i != (ieTotal - 1))
                        builder.Append(",");
                }

                // Remove the last ',' comma in the string.
                builder.Append("]");
                builder.Append(",");
            }
        }

        /// <summary>
        /// Serialises the object to a JSON format.
        /// </summary>
        /// <param name="data">The data to serialise.</param>
        /// <param name="builder">The string builder used to construct the JSON.</param>
        /// <remarks>This method only serialises members that have 'XmlElementAttribute' or 'XmlArrayAttribute'
        /// attributes any 'XmlArrayAttribute' attribute must be a two dimensional string array. Use
        /// the 'JavaScriptSerializer' methods for all other serialisation.</remarks>
        private static void JSonCustomSerializerEx(object data, StringBuilder builder)
        {
            // For each property member in the current type.
            foreach (PropertyInfo member in (data.GetType().GetProperties()))
            {
                // Get the current member value
                object memberValue = member.GetValue(data, null);

                // Is the current property an array type
                if (member.PropertyType.IsArray)
                {
                    // If two dimensional array 
                    if (member.PropertyType.FullName.Replace(" ", "").EndsWith("[,]"))
                    {
                        builder.Append(JavaObjectNotation.JSonTwoDimensionDataFormatter((Array)memberValue, member.Name));
                        builder.Append(",");
                    }
                    // If one dimensional array
                    else
                    {
                        JavaObjectNotation.JSonDataFormatterEx((Array)memberValue, member.Name, builder);
                        builder.Remove(builder.Length - 1, 1);
                        builder.Append(",");
                    }
                }
                else
                {
                    // Get the value type.
                    object returnValue = AppendData(memberValue, member.PropertyType);

                    // If value is null then not an primative object type
                    // else the current member is a object.
                    if (returnValue != null && returnValue.ToString() != "null")
                    {
                        builder.Append("\"" + member.Name + "\":");
                        builder.Append(returnValue);
                        builder.Append(",");
                    }
                    else
                    {
                        // If the current member is not null.
                        if (memberValue != null)
                        {
                            // Append the prefix name.
                            builder.Append("\"" + member.Name + "\":");
                            builder.Append("{");
                            JSonCustomSerializerEx(memberValue, builder);

                            // Remove the last ',' comma in the string.
                            builder.Remove(builder.Length - 1, 1);

                            // Close the current object.
                            builder.Append("}");
                            builder.Append(",");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assign the property in the data object.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to assign to the data object.</param>
        /// <param name="properties">The collection of properties for the data object.</param>
        /// <param name="dataObject">The data object to assign the data to.</param>
        /// <param name="IsPrimitiveType">Is the type primitive or an object type.</param>
        private static void JSonCustomDeserializerAssignEx(string member, object data, PropertyInfo[] properties, object dataObject, bool IsPrimitiveType)
        {
            // Find the name with the property name.
            IEnumerable<PropertyInfo> members = properties.Where(u => u.Name.ToLower() == member.ToLower());

            // If the data is not null.
            if (data != null && data.ToString().Trim() != "null")
            {
                // If data has been found.
                if (member != null)
                {
                    if (member.Count() > 0)
                    {
                        // Get the first property in the type.
                        PropertyInfo info = members.First();

                        // If the type a primitive type.
                        if (IsPrimitiveType)
                        {
                            // Assign the data type value.
                            object newData = AssignDataType(data, info.PropertyType);

                            // Assign the primitive type.
                            info.SetValue(dataObject, Nequeo.DataType.ConvertType(newData, info.PropertyType), null);
                        }
                        else
                            // Assign the object type.
                            info.SetValue(dataObject, data, null);
                    }
                }
            }
        }

        /// <summary>
        /// Assign the newly formatted data type.
        /// </summary>
        /// <param name="value">The value of the primitive type.</param>
        /// <param name="type">The current primitive type.</param>
        /// <returns>The newly formatted value.</returns>
        private static object AssignDataType(object value, Type type)
        {
            switch (type.Name.ToLower())
            {
                // If date time type.
                case "datetime":
                case "system.datetime":
                    // Remove all the un-nessescary text.
                    string ticksInit = value.ToString().ToLower().
                        Replace("new", "").
                        Replace("date", "").
                        Replace("(", "").
                        Replace(")", "").
                        Replace("\\", "").
                        Replace("/", "").Trim();

                    bool isNegativeTime = false;
                    bool hasUtcOffset = false;
                    string[] timeTickValues = null;

                    // Object that will contain the new date time
                    object dateTimeValue = null;

                    // The starting point +0 date time 1/1/1970.
                    DateTime start = new DateTime(1970, 1, 1);
                    
                    // Is the number of ticks negative
                    // then the time is before the 1/1/1970
                    if (ticksInit.StartsWith("-"))
                        isNegativeTime = true;

                    // Get the number of ticks with offset.
                    string ticks = ticksInit.TrimStart('+').TrimStart('-').Trim();

                    if (ticks.Contains("+"))
                    {
                        // Split the UTC offset.
                        hasUtcOffset = true;
                        timeTickValues = ticks.Split(new char[] { '+' }, StringSplitOptions.None);

                        // Get the UTC offset values.
                        long utcTicks = Convert.ToInt64(timeTickValues[0].Trim());
                        int utcOffsetHours = timeTickValues[1].GetUtcOffsetHours();
                        int utcOffsetMinutes = timeTickValues[1].GetUtcOffsetMinutes();

                        // Add the specfied number of milliseconds and the offset.
                        DateTime newValue = start.AddMilliseconds(Convert.ToDouble(utcTicks)).Add(new TimeSpan(utcOffsetHours, utcOffsetMinutes, 0));

                        // If the date is in the passed less
                        // then 1/1/1970 then subtract the number
                        // of ticks.
                        if (isNegativeTime)
                            newValue = start.Subtract(new TimeSpan(Convert.ToInt64(utcTicks) * 10000000)).Add(new TimeSpan(utcOffsetHours, utcOffsetMinutes, 0));

                        // Assign the new date time.
                        dateTimeValue = newValue;
                    }
                    else if (ticks.Contains("-"))
                    {
                        // Split the UTC offset.
                        hasUtcOffset = true;
                        timeTickValues = ticks.Split(new char[] { '-' }, StringSplitOptions.None);

                        // Get the UTC offset values.
                        long utcTicks = Convert.ToInt64(timeTickValues[0].Trim());
                        int utcOffsetHours = timeTickValues[1].GetUtcOffsetHours();
                        int utcOffsetMinutes = timeTickValues[1].GetUtcOffsetMinutes();

                        // Add the specfied number of milliseconds and the offset.
                        DateTime newValue = start.AddMilliseconds(Convert.ToDouble(utcTicks)).Subtract(new TimeSpan(utcOffsetHours, utcOffsetMinutes, 0));

                        // If the date is in the passed less
                        // then 1/1/1970 then subtract the number
                        // of ticks.
                        if (isNegativeTime)
                            newValue = start.Subtract(new TimeSpan(Convert.ToInt64(utcTicks) * 10000000)).Subtract(new TimeSpan(utcOffsetHours, utcOffsetMinutes, 0));

                        // Assign the new date time.
                        dateTimeValue = newValue;
                    }

                    // If no offset has been included
                    if (!hasUtcOffset)
                    {
                        // Add the specfied number of milliseconds
                        DateTime newValue = start.AddMilliseconds(Convert.ToDouble(ticks));

                        // If the date is in the passed less
                        // then 1/1/1970 then subtract the number
                        // of ticks.
                        if (isNegativeTime)
                            newValue = start.Subtract(new TimeSpan(Convert.ToInt64(ticks) * 10000000));

                        // Assign the new date time.
                        dateTimeValue = newValue;
                    }

                    // Return the converted date time.
                    return dateTimeValue;

                // If time span type.
                case "timespan":
                case "system.timespan":
                    return Nequeo.Invention.Converter.DurationFormatToTimeSpan(value.ToString());

                case "system.nullable`1":
                case "nullable`1":
                    Type[] genericArguments = type.GetGenericArguments();
                    return AssignDataType(value, genericArguments[0]);

                default:
                    // Return the original value.
                    return value;
            }
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to examine.</param>
        /// <param name="properties">The properties for the data object.</param>
        /// <param name="dataObject">The current data object to assign data to.</param>
        private static void JSonCustomDeserializerEx(string member, string data, PropertyInfo[] properties, object dataObject)
        {
            // If the data is a two dementinal array then
            // find the starting array indicator. This is
            // a custom two dimensional string array.
            if (data.Trim().StartsWith("[["))
            {
                JSonCustomDeserializerExArrayArray(member, data, properties, dataObject);
            }
            // If the data is a one dimensional array then
            // find the starting array indicator.
            else if (data.Trim().StartsWith("[{"))
            {
                JSonCustomDeserializerExArrayClass(member, data, properties, dataObject);
            }
            // If the data is a one dimensional array then
            // find the starting array indicator.
            else if (data.Trim().StartsWith("{"))
            {
                JSonCustomDeserializerExClass(member, data, properties, dataObject);
            }
            // If the data is a one dimensional array then
            // find the starting array indicator.
            else if (data.Trim().StartsWith("["))
            {
                JSonCustomDeserializerExArray(member, data, properties, dataObject);
            }
            else
                JSonCustomDeserializerExPrimitive(member, data, properties, dataObject);
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to examine.</param>
        /// <param name="properties">The properties for the data object.</param>
        /// <param name="dataObject">The current data object to assign data to.</param>
        private static void JSonCustomDeserializerExPrimitive(string member, string data, PropertyInfo[] properties, object dataObject)
        {
            // Split the data into property names.
            string[] splitItems = null;

            // Split the data to the menber level.
            string[] splitIntItems = data.Split(new string[] { "," }, 2, StringSplitOptions.None);
            JSonCustomDeserializerAssignEx(member.Replace("\"", ""), splitIntItems[0].Trim().Replace("\"", ""), properties, dataObject, true);

            // If more members exists in the JSON.
            if (splitIntItems.Length > 1)
            {
                splitItems = splitIntItems[1].Split(new string[] { ":" }, 2, StringSplitOptions.None);
                JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), properties, dataObject);
            }
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to examine.</param>
        /// <param name="properties">The properties for the data object.</param>
        /// <param name="dataObject">The current data object to assign data to.</param>
        private static void JSonCustomDeserializerExArrayArray(string member, string data, PropertyInfo[] properties, object dataObject)
        {
            // Split the data into property names.
            string[] splitItems = null;

            // Split the initial data.
            string[] splitIntBBItems = data.Split(new string[] { "]]," }, 2, StringSplitOptions.None);

            // Split the data to the row and column level.
            string dataArray = "[" + splitIntBBItems[0].TrimStart('[').TrimEnd(']').Trim() + "]";
            string[] splitRows = dataArray.Split(new string[] { "]," }, StringSplitOptions.None);
            int columnCount = splitRows[0].TrimStart('[').Split(new string[] { "," }, StringSplitOptions.None).Length;

            // Create a new two dimensional array of
            // the correct size.
            Array dataItems = (Array)CreateMemberType(member.Replace("\"", ""), properties, splitRows.Length, columnCount, false);

            // For each row in the data get each column.
            for (int j = 0; j < splitRows.Length; j++)
            {
                // Get all the current column data.
                string[] splitColumns = splitRows[j].TrimStart('[').TrimEnd(']').Split(new string[] { "," }, StringSplitOptions.None);

                // Interate through each column and
                // assign the data.
                for (int k = 0; k < splitColumns.Length; k++)
                    dataItems.SetValue(splitColumns[k].Replace("\"", ""), j, k);
            }

            // Asiign the data
            JSonCustomDeserializerAssignEx(member.Replace("\"", ""), dataItems, properties, dataObject, false);

            // If more members exists in the JSON.
            if (splitIntBBItems.Length > 1)
            {
                splitItems = splitIntBBItems[1].Split(new string[] { ":" }, 2, StringSplitOptions.None);
                JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), properties, dataObject);
            }
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to examine.</param>
        /// <param name="properties">The properties for the data object.</param>
        /// <param name="dataObject">The current data object to assign data to.</param>
        private static void JSonCustomDeserializerExArrayClass(string member, string data, PropertyInfo[] properties, object dataObject)
        {
            // Split the data into property names.
            string[] splitItems = null;

            // Split the data to the column level.
            string[] splitIntBCItems = data.Split(new string[] { "}]," }, 2, StringSplitOptions.None);

            // Split the data to the row and column level.
            string dataArray = splitIntBCItems[0].TrimStart('[').TrimEnd(']').Trim();
            string[] splitRows = dataArray.Split(new string[] { "},{" }, 2, StringSplitOptions.None);
            Array newObjects = (Array)CreateMemberType(member.Replace("\"", ""), properties, splitRows.Length, 0, false);

            // Interate through each column and
            // assign the data.
            for (int i = 0; i < newObjects.GetLength(0); i++)
            {
                object newObject = CreateMemberType(member.Replace("\"", ""), properties, 0, 0, true);
                newObjects.SetValue(newObject, i);
                PropertyInfo[] newProperties = newObjects.GetValue(i).GetType().GetProperties();

                string[] splitIntItems = splitRows[i].TrimStart('{').TrimEnd('}').Split(new string[] { ":" }, 2, StringSplitOptions.None);
                JSonCustomDeserializerEx(splitIntItems[0].Replace("\"", ""), splitIntItems[1].Trim(), newProperties, newObject);
            }

            // Asiign the data
            JSonCustomDeserializerAssignEx(member.Replace("\"", ""), newObjects, properties, dataObject, false);

            // If more members exists in the JSON.
            if (splitIntBCItems.Length > 1)
            {
                splitItems = splitIntBCItems[1].Split(new string[] { ":" }, 2, StringSplitOptions.None);
                JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), properties, dataObject);
            }
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to examine.</param>
        /// <param name="properties">The properties for the data object.</param>
        /// <param name="dataObject">The current data object to assign data to.</param>
        private static void JSonCustomDeserializerExClass(string member, string data, PropertyInfo[] properties, object dataObject)
        {
            // Split the data into property names.
            string[] splitItems = null;

            // Split the data to the column level.
            string[] splitIntCItems = data.Split(new string[] { "}," }, 2, StringSplitOptions.None);

            // Split the data into property names.
            string newData = splitIntCItems[0].TrimStart('{').TrimEnd('}').Trim();
            object newObjects = CreateMemberType(member.Replace("\"", ""), properties, 0, 0, true);
            PropertyInfo[] newProperties = newObjects.GetType().GetProperties();

            // Split the data into property names.
            splitItems = newData.TrimStart('{').TrimEnd('}').Split(new string[] { ":" }, 2, StringSplitOptions.None);

            // Start the class creation again, the same process
            // as if the begining of the deserialisation.
            JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), newProperties, newObjects);

            // Asiign the data
            JSonCustomDeserializerAssignEx(member.Replace("\"", ""), newObjects, properties, dataObject, false);

            // If more members exists in the JSON.
            if (splitIntCItems.Length > 1)
            {
                splitItems = splitIntCItems[1].Split(new string[] { ":" }, 2, StringSplitOptions.None);
                JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), properties, dataObject);
            }
        }

        /// <summary>
        /// Deserialises the object from a JSON format.
        /// </summary>
        /// <param name="member">The current member name.</param>
        /// <param name="data">The data to examine.</param>
        /// <param name="properties">The properties for the data object.</param>
        /// <param name="dataObject">The current data object to assign data to.</param>
        private static void JSonCustomDeserializerExArray(string member, string data, PropertyInfo[] properties, object dataObject)
        {
            // Split the data into property names.
            string[] splitItems = null;

            // Split the data to the column level.
            string[] splitIntBItems = data.Split(new string[] { "]," }, 2, StringSplitOptions.None);
            string[] splitColumns = splitIntBItems[0].TrimStart('[').TrimEnd(']').Split(new string[] { "," }, StringSplitOptions.None);
            Array dataSingleItems = (Array)CreateMemberType(member.Replace("\"", ""), properties, splitColumns.Length, 0, false);

            // Interate through each column and
            // assign the data.
            for (int y = 0; y < dataSingleItems.GetLength(0); y++)
                dataSingleItems.SetValue(splitColumns[y].Replace("\"", ""), y);

            // Asiign the data
            JSonCustomDeserializerAssignEx(member.Replace("\"", ""), dataSingleItems, properties, dataObject, false);

            // If more members exists in the JSON.
            if (splitIntBItems.Length > 1)
            {
                splitItems = splitIntBItems[1].Split(new string[] { ":" }, 2, StringSplitOptions.None);
                JSonCustomDeserializerEx(splitItems[0].Trim(), splitItems[1].Trim(), properties, dataObject);
            }
        }
        #endregion
    }
}
