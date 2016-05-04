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
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nequeo.CodeGeneration.Json
{
    /// <summary>
    /// JSON type builder.
    /// </summary>
    internal class JsonGenerator : IJsonConfig
    {
        /// <summary>
        /// JSON type builder.
        /// </summary>
        /// <param name="json">The json data.</param>
        public JsonGenerator(string json)
        {
            try
            {
                // Load the data into the json reader.
                using (var sr = new StringReader(json))
                using (var reader = new JsonTextReader(sr))
                {
                    JToken jtoken = JToken.ReadFrom(reader);
                    if (jtoken is JArray)
                    {
                        _jobjects = ((JArray)jtoken).Cast<JObject>().ToArray();
                    }
                    else if (jtoken is JObject)
                    {
                        _jobjects = new[] { (JObject)jtoken };
                    }
                    else
                    {
                        throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// JSON type builder.
        /// </summary>
        /// <param name="stream">The stream containing the json data.</param>
        public JsonGenerator(StreamReader stream)
        {
            try
            {
                // Load the data into the json reader.
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jtoken = JToken.ReadFrom(reader);
                    if (jtoken is JArray)
                    {
                        _jobjects = ((JArray)jtoken).Cast<JObject>().ToArray();
                    }
                    else if (jtoken is JObject)
                    {
                        _jobjects = new[] { (JObject)jtoken };
                    }
                    else
                    {
                        throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IList<JsonType> _types = null;
        private JObject[] _jobjects = null;
        private JsonType _rootType = null;
        private HashSet<string> _names = null;

        /// <summary>
        /// Gets or sets the base namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the root class name.
        /// </summary>
        public string RootClassName { get; set; }

        /// <summary>
        /// Extract tthe types from the json data.
        /// </summary>
        /// <returns>The array of json types extracted fron the json data.</returns>
        public IList<JsonType> Extract()
        {
            _types = null;
            _names = null;

            // Create and assign the root class.
            _types = new List<JsonType>();
            _names = new HashSet<string>();

            _names.Add(RootClassName);
            _rootType = new JsonType(this, _jobjects[0]);
            _rootType.IsRoot = true;
            _rootType.AssignName(RootClassName);

            // Generate the class from the json.
            GenerateClass(_jobjects, _rootType);
            return _types;
        }

        /// <summary>
        /// Start entracting the json type information.
        /// </summary>
        /// <param name="jobjects">The json objects.</param>
        /// <param name="type">The json type.</param>
        private void GenerateClass(JObject[] jobjects, JsonType type)
        {
            var jsonFields = new Dictionary<string, JsonType>();
            var fieldExamples = new Dictionary<string, IList<object>>();

            var first = true;

            foreach (var obj in jobjects)
            {
                foreach (var prop in obj.Properties())
                {
                    JsonType fieldType;
                    var currentType = new JsonType(this, prop.Value);
                    var propName = prop.Name;

                    // Try get property.
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {

                        var commonType = fieldType.GetCommonType(currentType);

                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        var commonType = currentType;
                        if (first) commonType = commonType.MaybeMakeNullable(this);
                        else commonType = commonType.GetCommonType(JsonType.GetNull(this));
                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }
                    var fe = fieldExamples[propName];
                    var val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        var v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                            fe.Add(v);
                    }
                }
                first = false;
            }

            //if (UseNestedClasses)
            //{
            //    foreach (var field in jsonFields)
            //    {
            //        _names.Add(field.Key.ToLower());
            //    }
            //}

            foreach (var field in jsonFields)
            {
                var fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(jobjects.Length);
                    foreach (var obj in jobjects)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                subexamples.Add((JObject)value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key));
                    GenerateClass(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(jobjects.Length);
                    foreach (var obj in jobjects)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                foreach (var item in (JArray)value)
                                {
                                    if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    subexamples.Add((JObject)item);
                                }

                            }
                            else if (value.Type == JTokenType.Object)
                            {
                                foreach (var item in (JObject)value)
                                {
                                    if (!(item.Value is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");

                                    subexamples.Add((JObject)item.Value);
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignName(CreateUniqueClassNameFromPlural(field.Key));
                    GenerateClass(subexamples.ToArray(), field.Value.InternalType);
                }

                if(fieldType.InternalType == null)
                {
                    if (fieldType.Type == JsonTypeEnum.Object)
                    {
                        // Assign an internal type.
                        fieldType.SetInternalType(this, fieldType.AssignedName);
                    }
                }
            }

            // Select the field info
            type.Fields = jsonFields.Select(x => new JsonFieldInfo(this, x.Key, x.Value, true, fieldExamples[x.Key])).ToArray();

            // Add the type.
            _types.Add(type);

        }

        /// <summary>
        /// Create a unique class name.
        /// </summary>
        /// <param name="name">The current name.</param>
        /// <returns>The unique name.</returns>
        private string CreateUniqueClassName(string name)
        {
            name = ToTitleCase(name);

            var finalName = name;
            var i = 2;
            while (_names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = name + i.ToString();
                i++;
            }

            _names.Add(finalName);
            return finalName;
        }

        /// <summary>
        /// Create a unique member name.
        /// </summary>
        /// <param name="memberNames">The current member list.</param>
        /// <param name="name">The current name.</param>
        /// <returns>The unique name.</returns>
        private string CreateUniqueMemberName(HashSet<string> memberNames, string name)
        {
            name = ToTitleCase(name);

            var finalName = name;
            var i = 2;
            while (memberNames.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = name + i.ToString();
                i++;
            }

            memberNames.Add(finalName);
            return finalName;
        }

        /// <summary>
        /// Pluralise the word.
        /// </summary>
        /// <param name="plural">The string tp pluralise.</param>
        /// <returns>The pluralised string.</returns>
        private string CreateUniqueClassNameFromPlural(string plural)
        {
            plural = ToTitleCase(plural);
            return CreateUniqueClassName(Plural(plural));
        }

        /// <summary>
        /// To title case.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The new title case.</returns>
        internal static string ToTitleCase(string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts the word from singular to plural.
        /// </summary>
        /// <param name="name">The word to convert.</param>
        /// <returns>The converted word as plural.</returns>
        private string Plural(string name)
        {
            if (name.EndsWith("x", StringComparison.InvariantCultureIgnoreCase)
                || name.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)
                || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return name + "es";
            }
            else if (name.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
            {
                return name.Substring(0, name.Length - 1) + "ies";
            }
            else if (!name.EndsWith("s"))
            {
                return name + "s";
            }
            return name;
        }
    }
}
