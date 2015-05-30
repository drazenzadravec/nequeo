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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

using Nequeo.Data.DataType;

namespace Nequeo.Data.Linq.Common
{
    /// <summary>
    /// Contains mapping control static members
    /// </summary>
    internal class MappingController
    {
        /// <summary>
        /// 
        /// </summary>
        public static ConnectionContext.ConnectionType ConnectionType = ConnectionContext.ConnectionType.None;
        /// <summary>
        /// 
        /// </summary>
        public static ConnectionContext.ConnectionDataType ConnectionDataType = ConnectionContext.ConnectionDataType.None;
    }

    /// <summary>
    /// A simple query mapping that attempts to infer mapping from naming conventions
    /// </summary>
    public class ImplicitMapping : QueryMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <param name="connectionType"></param>
        /// <param name="connectionDataType"></param>
        public ImplicitMapping(QueryLanguage language,
            ConnectionContext.ConnectionType connectionType, ConnectionContext.ConnectionDataType connectionDataType)
            : base(language)
        {
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            MappingController.ConnectionType = connectionType;
            MappingController.ConnectionDataType = connectionDataType;
        }

        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsEntity(Type type)
        {
            // everything is an entity except scalar primitives
            return !this.Language.IsScalar(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override bool IsMapped(MemberInfo member)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override bool IsColumn(MemberInfo member)
        {
            return this.Language.IsScalar(TypeHelper.GetMemberType(member));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override bool IsIdentity(MemberInfo member)
        {
            // Customers has CustomerID, Orders has OrderID, etc
            if (this.IsColumn(member)) 
            {
                string name = NameWithoutTrailingDigits(member.Name);
                return member.Name.EndsWith("ID") && member.DeclaringType.Name.StartsWith(member.Name.Substring(0, member.Name.Length - 2)); 
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string NameWithoutTrailingDigits(string name)
        {
            int n = name.Length - 1;
            while (n >= 0 && char.IsDigit(name[n]))
            {
                n--;
            }
            if (n < name.Length - 1)
            {
                return name.Substring(0, n);
            }
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override bool IsRelationship(MemberInfo member)
        {
            if (!IsColumn(member))
            {
                Type otherType = TypeHelper.GetElementType(TypeHelper.GetMemberType(member));
                if (IsEntity(otherType))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override Type GetRelatedType(MemberInfo member)
        {
            return TypeHelper.GetElementType(TypeHelper.GetMemberType(member));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowType"></param>
        /// <returns></returns>
        public override string GetTableName(Type rowType)
        {
            // Get the current member.
            MemberInfo member = rowType;

            // For each attribute for the member
            foreach (object attribute in member.GetCustomAttributes(true))
            {
                // If the attribute is the nequeo datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DataTableAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataTableAttribute att =
                        (Nequeo.Data.Custom.DataTableAttribute)attribute;
                    return DataTypeConversion.GetSqlConversionDataType(_connectionDataType, att.TableName.TrimStart('_').Replace(".", "].["));
                }
            }

            // Return a null.
            return this.Language.Quote(rowType.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override string GetColumnName(MemberInfo member)
        {
            // For each attribute for the member
            foreach (object attribute in member.DeclaringType.GetProperty(member.Name).GetCustomAttributes(true))
            {
                // If the attribute is the nequeo datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;
                    return DataTypeConversion.GetSqlConversionDataType(_connectionDataType, att.ColumnName.TrimStart('_'));
                }
            }

            return member.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="members1"></param>
        /// <param name="members2"></param>
        public override void GetAssociationKeys(MemberInfo member, out List<MemberInfo> members1, out List<MemberInfo> members2)
        {
            Type type1 = member.DeclaringType;
            Type type2 = GetRelatedType(member);

            // find all members in common (same name)
            var map1 = this.GetMappedMembers(type1).Where(m => this.IsColumn(m)).ToDictionary(m => m.Name);
            var map2 = this.GetMappedMembers(type2).Where(m => this.IsColumn(m)).ToDictionary(m => m.Name);
            var commonNames = map1.Keys.Intersect(map2.Keys).OrderBy(k => k);
            members1 = new List<MemberInfo>();
            members2 = new List<MemberInfo>();
            foreach (string name in commonNames)
            {
                members1.Add(map1[name]);
                members2.Add(map2[name]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SplitWords(string name)
        {
            StringBuilder sb = null;
            bool lastIsLower = char.IsLower(name[0]);
            for (int i = 0, n = name.Length; i < n; i++)
            {
                bool thisIsLower = char.IsLower(name[i]);
                if (lastIsLower && !thisIsLower)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                        sb.Append(name, 0, i);
                    }
                    sb.Append(" ");
                }
                if (sb != null)
                {
                    sb.Append(name[i]);
                }
                lastIsLower = thisIsLower;
            }
            if (sb != null)
            {
                return sb.ToString();
            }
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Plural(string name)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Singular(string name)
        {
            if (name.EndsWith("es", StringComparison.InvariantCultureIgnoreCase))
            {
                string rest = name.Substring(0, name.Length - 2);
                if (rest.EndsWith("x", StringComparison.InvariantCultureIgnoreCase)
                    || name.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)
                    || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
                {
                    return rest;
                }
            }
            if (name.EndsWith("ies", StringComparison.InvariantCultureIgnoreCase))
            {
                return name.Substring(0, name.Length - 3) + "y";
            }
            else if (name.EndsWith("s", StringComparison.InvariantCultureIgnoreCase)
                && !name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return name.Substring(0, name.Length - 1);
            }
            return name;
        }
    }
}
