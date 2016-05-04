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
    /// Json type converted.
    /// </summary>
    internal class JsonType
    {
        /// <summary>
        /// Json type converted.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        private JsonType(IJsonConfig generator)
        {
            this.generator = generator;
        }

        /// <summary>
        /// Json type converted.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <param name="assignedName">The assigned name.</param>
        private JsonType(IJsonConfig generator, string assignedName)
        {
            this.generator = generator;
            AssignedName = assignedName;
            Type = JsonTypeEnum.Object;
        }

        /// <summary>
        /// Json type converted.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <param name="token">The json token.</param>
        public JsonType(IJsonConfig generator, JToken token)
            : this(generator)
        {

            Type = GetFirstTypeEnum(token);

            if (Type == JsonTypeEnum.Array)
            {
                var array = (JArray)token;
                InternalType = GetCommonType(generator, array.ToArray());
            }
        }

        /// <summary>
        /// Json type converted.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <param name="type">The json type.</param>
        internal JsonType(IJsonConfig generator, JsonTypeEnum type)
            : this(generator)
        {
            this.Type = type;
        }

        private IJsonConfig generator;

        /// <summary>
        /// Get nullables.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <returns>The null able type.</returns>
        internal static JsonType GetNull(IJsonConfig generator)
        {
            return new JsonType(generator, JsonTypeEnum.NullableSomething);
        }

        /// <summary>
        /// Get common type
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <param name="tokens">The json tokens.</param>
        /// <returns>The json type.</returns>
        public static JsonType GetCommonType(IJsonConfig generator, JToken[] tokens)
        {

            if (tokens.Length == 0) return new JsonType(generator, JsonTypeEnum.NonConstrained);

            var common = new JsonType(generator, tokens[0]).MaybeMakeNullable(generator);

            for (int i = 1; i < tokens.Length; i++)
            {
                var current = new JsonType(generator, tokens[i]);
                common = common.GetCommonType(current);
            }

            return common;

        }

        /// <summary>
        /// Make nullable.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <returns>The json type.</returns>
        internal JsonType MaybeMakeNullable(IJsonConfig generator)
        {
            return this.GetCommonType(JsonType.GetNull(generator));
        }

        /// <summary>
        /// Gets the json type.
        /// </summary>
        public JsonTypeEnum Type { get; private set; }

        /// <summary>
        /// Gets the internal type.
        /// </summary>
        public JsonType InternalType { get; private set; }

        /// <summary>
        /// Gets the assigned name.
        /// </summary>
        public string AssignedName { get; private set; }

        /// <summary>
        /// Set the internal type null value and only assign the assignment name.
        /// </summary>
        /// <param name="generator">The generator config.</param>
        /// <param name="assignedName">The assigned name.</param>
        internal void SetInternalType(IJsonConfig generator, string assignedName)
        {
            InternalType = new JsonType(generator, assignedName);
        }

        /// <summary>
        /// Assign the name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AssignName(string name)
        {
            AssignedName = name;
        }

        /// <summary>
        /// Must cache.
        /// </summary>
        public bool MustCache
        {
            get
            {
                switch (Type)
                {
                    case JsonTypeEnum.Array: return true;
                    case JsonTypeEnum.Object: return true;
                    case JsonTypeEnum.Anything: return true;
                    case JsonTypeEnum.Dictionary: return true;
                    case JsonTypeEnum.NonConstrained: return true;
                    default: return false;
                }
            }
        }

        /// <summary>
        /// Get inner most type.
        /// </summary>
        /// <returns>The json type.</returns>
        public JsonType GetInnermostType()
        {
            if (Type != JsonTypeEnum.Array) throw new InvalidOperationException();
            if (InternalType.Type != JsonTypeEnum.Array) return InternalType;
            return InternalType.GetInnermostType();
        }

        /// <summary>
        /// Get json token type.
        /// </summary>
        /// <returns>The token type value.</returns>
        public string GetJTokenType()
        {
            switch (Type)
            {
                case JsonTypeEnum.Boolean:
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.Long:
                case JsonTypeEnum.Float:
                case JsonTypeEnum.Date:
                case JsonTypeEnum.NullableBoolean:
                case JsonTypeEnum.NullableInteger:
                case JsonTypeEnum.NullableLong:
                case JsonTypeEnum.NullableFloat:
                case JsonTypeEnum.NullableDate:
                case JsonTypeEnum.String:
                    return "JValue";
                case JsonTypeEnum.Array:
                    return "JArray";
                case JsonTypeEnum.Dictionary:
                    return "JObject";
                case JsonTypeEnum.Object:
                    return "JObject";
                default:
                    return "JToken";

            }
        }

        /// <summary>
        /// Get the common type.
        /// </summary>
        /// <param name="type2">The json type.</param>
        /// <returns>The json type.</returns>
        public JsonType GetCommonType(JsonType type2)
        {
            var commonType = GetCommonTypeEnum(this.Type, type2.Type);

            if (commonType == JsonTypeEnum.Array)
            {
                if (type2.Type == JsonTypeEnum.NullableSomething) return this;
                if (this.Type == JsonTypeEnum.NullableSomething) return type2;
                var commonInternalType = InternalType.GetCommonType(type2.InternalType).MaybeMakeNullable(generator);
                if (commonInternalType != InternalType) return new JsonType(generator, JsonTypeEnum.Array) { InternalType = commonInternalType };
            }

            if (this.Type == commonType) return this;
            return new JsonType(generator, commonType).MaybeMakeNullable(generator);
        }

        /// <summary>
        /// Is null.
        /// </summary>
        /// <param name="type">The json type.</param>
        /// <returns>True if boolean; else false.</returns>
        private static bool IsNull(JsonTypeEnum type)
        {
            return type == JsonTypeEnum.NullableSomething;
        }

        /// <summary>
        /// Get common type enum.
        /// </summary>
        /// <param name="type1">The json type.</param>
        /// <param name="type2">The json type.</param>
        /// <returns>The json type.</returns>
        private JsonTypeEnum GetCommonTypeEnum(JsonTypeEnum type1, JsonTypeEnum type2)
        {
            if (type1 == JsonTypeEnum.NonConstrained) return type2;
            if (type2 == JsonTypeEnum.NonConstrained) return type1;

            switch (type1)
            {
                case JsonTypeEnum.Boolean:
                    if (IsNull(type2)) return JsonTypeEnum.NullableBoolean;
                    if (type2 == JsonTypeEnum.Boolean) return type1;
                    break;
                case JsonTypeEnum.NullableBoolean:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Boolean) return type1;
                    break;
                case JsonTypeEnum.Integer:
                    if (IsNull(type2)) return JsonTypeEnum.NullableInteger;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.Float;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.Long;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.NullableInteger:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.Float:
                    if (IsNull(type2)) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Float) return type1;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.NullableFloat:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return type1;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.Long:
                    if (IsNull(type2)) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.Float;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.NullableLong:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.Date:
                    if (IsNull(type2)) return JsonTypeEnum.NullableDate;
                    if (type2 == JsonTypeEnum.Date) return JsonTypeEnum.Date;
                    break;
                case JsonTypeEnum.NullableDate:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Date) return type1;
                    break;
                case JsonTypeEnum.NullableSomething:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.String) return JsonTypeEnum.String;
                    if (type2 == JsonTypeEnum.Integer) return JsonTypeEnum.NullableInteger;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Boolean) return JsonTypeEnum.NullableBoolean;
                    if (type2 == JsonTypeEnum.Date) return JsonTypeEnum.NullableDate;
                    if (type2 == JsonTypeEnum.Array) return JsonTypeEnum.Array;
                    if (type2 == JsonTypeEnum.Object) return JsonTypeEnum.Object;
                    break;
                case JsonTypeEnum.Object:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Object) return type1;
                    if (type2 == JsonTypeEnum.Dictionary) throw new ArgumentException();
                    break;
                case JsonTypeEnum.Dictionary:
                    throw new ArgumentException();
                //if (IsNull(type2)) return type1;
                //if (type2 == JsonTypeEnum.Object) return type1;
                //if (type2 == JsonTypeEnum.Dictionary) return type1;
                //  break;
                case JsonTypeEnum.Array:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Array) return type1;
                    break;
                case JsonTypeEnum.String:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.String) return type1;
                    break;
            }

            return JsonTypeEnum.Anything;

        }

        /// <summary>
        /// Is null
        /// </summary>
        /// <param name="type">The json type.</param>
        /// <returns>True if null; else false.</returns>
        private static bool IsNull(JTokenType type)
        {
            return type == JTokenType.Null || type == JTokenType.Undefined;
        }

        /// <summary>
        /// Get first json type.
        /// </summary>
        /// <param name="token">The json token.</param>
        /// <returns>The json type.</returns>
        private static JsonTypeEnum GetFirstTypeEnum(JToken token)
        {
            var type = token.Type;
            if (type == JTokenType.Integer)
            {
                if ((long)((JValue)token).Value < int.MaxValue) return JsonTypeEnum.Integer;
                else return JsonTypeEnum.Long;

            }
            switch (type)
            {
                case JTokenType.Array: return JsonTypeEnum.Array;
                case JTokenType.Boolean: return JsonTypeEnum.Boolean;
                case JTokenType.Float: return JsonTypeEnum.Float;
                case JTokenType.Null: return JsonTypeEnum.NullableSomething;
                case JTokenType.Undefined: return JsonTypeEnum.NullableSomething;
                case JTokenType.String: return JsonTypeEnum.String;
                case JTokenType.Object: return JsonTypeEnum.Object;
                case JTokenType.Date: return JsonTypeEnum.Date;
                default: return JsonTypeEnum.Anything;

            }
        }

        /// <summary>
        /// Is nullable type.
        /// </summary>
        /// <param name="jsonTypeEnum">The json type.</param>
        /// <returns>True if nullable; else false.</returns>
        public static bool IsNullable(JsonTypeEnum jsonTypeEnum)
        {
            switch (jsonTypeEnum)
            {
                case JsonTypeEnum.NullableBoolean:
                case JsonTypeEnum.NullableInteger:
                case JsonTypeEnum.NullableLong:
                case JsonTypeEnum.NullableFloat:
                case JsonTypeEnum.NullableDate:
                case JsonTypeEnum.NullableSomething:
                    return true;
                case JsonTypeEnum.Boolean:
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.Long:
                case JsonTypeEnum.Float:
                case JsonTypeEnum.Date:
                case JsonTypeEnum.String:
                case JsonTypeEnum.Array:
                case JsonTypeEnum.Dictionary:
                case JsonTypeEnum.Object:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the system type.
        /// </summary>
        /// <param name="jsonTypeEnum">The json type.</param>
        /// <returns>The system type.</returns>
        public static Type GetSystemType(JsonTypeEnum jsonTypeEnum)
        {
            switch (jsonTypeEnum)
            {
                case JsonTypeEnum.Boolean:
                case JsonTypeEnum.NullableBoolean:
                    return typeof(System.Boolean);
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.NullableInteger:
                    return typeof(System.Int32);
                case JsonTypeEnum.Long:
                case JsonTypeEnum.NullableLong:
                    return typeof(System.Int64);
                case JsonTypeEnum.Float:
                case JsonTypeEnum.NullableFloat:
                    return typeof(System.Double);
                case JsonTypeEnum.Date:
                case JsonTypeEnum.NullableDate:
                    return typeof(System.DateTime);
                case JsonTypeEnum.String:
                    return typeof(System.String);
                case JsonTypeEnum.Array:
                    return typeof(System.Object[]);
                case JsonTypeEnum.Dictionary:
                    return typeof(System.Collections.IDictionary);
                case JsonTypeEnum.Object:
                case JsonTypeEnum.NullableSomething:
                default:
                    return typeof(System.Object);
            }
        }

        /// <summary>
        /// Get the system type.
        /// </summary>
        /// <param name="jsonTypeEnum">The json type.</param>
        /// <returns>The system type.</returns>
        public static string GetSystemTypeString(JsonTypeEnum jsonTypeEnum)
        {
            switch (jsonTypeEnum)
            {
                case JsonTypeEnum.Boolean:
                case JsonTypeEnum.NullableBoolean:
                    return typeof(System.Boolean).ToString();
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.NullableInteger:
                    return typeof(System.Int32).ToString();
                case JsonTypeEnum.Long:
                case JsonTypeEnum.NullableLong:
                    return typeof(System.Int64).ToString();
                case JsonTypeEnum.Float:
                case JsonTypeEnum.NullableFloat:
                    return typeof(System.Double).ToString();
                case JsonTypeEnum.Date:
                case JsonTypeEnum.NullableDate:
                    return typeof(System.DateTime).ToString();
                case JsonTypeEnum.String:
                    return typeof(System.String).ToString();
                case JsonTypeEnum.Array:
                    return typeof(System.Object[]).ToString();
                case JsonTypeEnum.Dictionary:
                    return typeof(System.Collections.IDictionary).ToString();
                case JsonTypeEnum.Object:
                case JsonTypeEnum.NullableSomething:
                default:
                    return typeof(System.Object).ToString();
            }
        }

        /// <summary>
        /// Gets the field list.
        /// </summary>
        public IList<JsonFieldInfo> Fields { get; internal set; }

        /// <summary>
        /// Gets the is root indicator.
        /// </summary>
        public bool IsRoot { get; internal set; }
    }
}
