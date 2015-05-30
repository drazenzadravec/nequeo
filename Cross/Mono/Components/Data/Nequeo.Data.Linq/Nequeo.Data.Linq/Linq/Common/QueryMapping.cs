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

namespace Nequeo.Data.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class QueryMapping
    {
        QueryLanguage language;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        protected QueryMapping(QueryLanguage language)
        {
            this.language = language;
        }

        /// <summary>
        /// The language related to the mapping
        /// </summary>
        public QueryLanguage Language
        {
            get { return this.language; }
        }

        /// <summary>
        /// Determines if a give CLR type is mapped as a database entity
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool IsEntity(Type type);

        /// <summary>
        /// Deterimines is a property is mapped onto a column or relationship
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public abstract bool IsMapped(MemberInfo member);

        /// <summary>
        /// Determines if a property is mapped onto a column
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public abstract bool IsColumn(MemberInfo member);

        /// <summary>
        /// Determines if a property represents or is part of the entities unique identity (often primary key)
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public abstract bool IsIdentity(MemberInfo member);

        /// <summary>
        /// Determines if a property is mapped as a relationship
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public abstract bool IsRelationship(MemberInfo member);

        /// <summary>
        /// The type of the entity on the other side of the relationship
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public abstract Type GetRelatedType(MemberInfo member);

        /// <summary>
        /// The name of the corresponding database table
        /// </summary>
        /// <param name="rowType"></param>
        /// <returns></returns>
        public abstract string GetTableName(Type rowType);

        /// <summary>
        /// The name of the corresponding table column
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public abstract string GetColumnName(MemberInfo member);

        /// <summary>
        /// A sequence of all the mapped members
        /// </summary>
        /// <param name="rowType"></param>
        /// <returns></returns>
        public virtual IEnumerable<MemberInfo> GetMappedMembers(Type rowType)
        {
            HashSet<MemberInfo> members = new HashSet<MemberInfo>(rowType.GetFields().Cast<MemberInfo>().Where(m => this.IsMapped(m)));
            members.UnionWith(rowType.GetProperties().Cast<MemberInfo>().Where(m => this.IsMapped(m)));
            return members.OrderBy(m => m.Name);
        }

        /// <summary>
        /// Determines if a relationship property refers to a single optional entity (as opposed to a collection.)
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual bool IsSingletonRelationship(MemberInfo member)
        {
            if (!IsRelationship(member))
                return false;
            Type ieType = TypeHelper.FindIEnumerable(TypeHelper.GetMemberType(member));
            return ieType == null;
        }

        /// <summary>
        /// Get a query expression that selects all entities from a table
        /// </summary>
        /// <param name="rowType"></param>
        /// <returns></returns>
        public virtual ProjectionExpression GetTableQuery(Type rowType)
        {
            var tableAlias = new TableAlias();
            var selectAlias = new TableAlias();
            var table = new TableExpression(tableAlias, this.GetTableName(rowType));

            Expression projector = this.GetTypeProjection(table, rowType);
            var pc = ColumnProjector.ProjectColumns(this.Language.CanBeColumn, projector, null, selectAlias, tableAlias);

            return new ProjectionExpression(
                new SelectExpression(selectAlias, pc.Columns, table, null),
                pc.Projector
                );
        }

        /// <summary>
        /// Gets an expression that constructs an entity instance relative to a root.
        /// The root is most often a TableExpression, but may be any other experssion such as
        /// a ConstantExpression.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual Expression GetTypeProjection(Expression root, Type type)
        {
            // must be some complex type constructed from multiple columns
            List<MemberBinding> bindings = new List<MemberBinding>();
            foreach (MemberInfo mi in this.GetMappedMembers(type))
            {
                if (!GetColumnName(mi).Equals("UniqueBaseId") && !GetColumnName(mi).Equals("ExtensionData"))
                {
                    if (!this.IsRelationship(mi))
                    {
                        Expression me = this.GetMemberExpression(root, mi);
                        if (me != null)
                        {
                            bindings.Add(Expression.Bind(mi, me));
                        }
                    }
                }
            }
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        /// <summary>
        /// Get the members for the key properities to be joined in an association relationship
        /// </summary>
        /// <param name="association"></param>
        /// <param name="declaredTypeMembers"></param>
        /// <param name="associatedMembers"></param>
        public abstract void GetAssociationKeys(MemberInfo association, out List<MemberInfo> declaredTypeMembers, out List<MemberInfo> associatedMembers);

        /// <summary>
        /// Get an expression for a mapped property relative to a root expression. 
        /// The root is either a TableExpression or an expression defining an entity instance.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual Expression GetMemberExpression(Expression root, MemberInfo member)
        {
            if (this.IsRelationship(member))
            {
                Type rowType = this.GetRelatedType(member);
                ProjectionExpression projection = this.GetTableQuery(rowType);

                // make where clause for joining back to 'root'
                List<MemberInfo> declaredTypeMembers;
                List<MemberInfo> associatedMembers;
                this.GetAssociationKeys(member, out declaredTypeMembers, out associatedMembers);

                Expression where = null;
                for (int i = 0, n = associatedMembers.Count; i < n; i++)
                {
                    Expression equal = Expression.Equal(
                        this.GetMemberExpression(projection.Projector, associatedMembers[i]),
                        this.GetMemberExpression(root, declaredTypeMembers[i])
                        );
                    where = (where != null) ? Expression.And(where, equal) : equal;
                }

                TableAlias newAlias = new TableAlias();
                var pc = ColumnProjector.ProjectColumns(this.Language.CanBeColumn, projection.Projector, null, newAlias, projection.Source.Alias);

                LambdaExpression aggregator = this.GetAggregator(TypeHelper.GetMemberType(member), typeof(IEnumerable<>).MakeGenericType(pc.Projector.Type));
                return new ProjectionExpression(
                    new SelectExpression(newAlias, pc.Columns, projection.Source, where),
                    pc.Projector, aggregator
                    );
            }
            else
            {
                TableExpression table = root as TableExpression;
                if (table != null)
                {
                    if (this.IsColumn(member))
                    {
                        return new ColumnExpression(TypeHelper.GetMemberType(member), table.Alias, this.GetColumnName(member));
                    }
                    else
                    {
                        return this.GetTypeProjection(root, TypeHelper.GetMemberType(member));
                    }
                }
                else
                {
                    return QueryBinder.BindMember(root, member);
                }
            }
        }

        /// <summary>
        /// Get a function that coerces an a sequence of one type into another type.
        /// This is primarily used for aggregators stored in ProjectionExpression's, which are used to represent the 
        /// final transformation of the entire result set of a query.
        /// </summary>
        /// <param name="expectedType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        public virtual LambdaExpression GetAggregator(Type expectedType, Type actualType)
        {
            //Type actualType = typeof(IEnumerable<>).MakeGenericType(elementType);
            Type actualElementType = TypeHelper.GetElementType(actualType);
            if (!expectedType.IsAssignableFrom(actualType))
            {
                Type expectedElementType = TypeHelper.GetElementType(expectedType);
                ParameterExpression p = Expression.Parameter(actualType, "p");
                Expression body = null;
                if (expectedType.IsAssignableFrom(actualElementType))
                {
                    body = Expression.Call(typeof(Enumerable), "SingleOrDefault", new Type[] { actualElementType }, p);
                }
                else if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    body = Expression.Call(typeof(Queryable), "AsQueryable", new Type[] { expectedElementType }, CoerceElement(expectedElementType, p));
                }
                else if (expectedType.IsArray && expectedType.GetArrayRank() == 1)
                {
                    body = Expression.Call(typeof(Enumerable), "ToArray", new Type[] { expectedElementType }, CoerceElement(expectedElementType, p));
                }
                else if (expectedType.IsAssignableFrom(typeof(List<>).MakeGenericType(actualElementType)))
                {
                    // List<T> can be assigned to expectedType
                    body = Expression.Call(typeof(Enumerable), "ToList", new Type[] { expectedElementType }, CoerceElement(expectedElementType, p));
                }
                else
                {
                    // some other collection type that has a constructor that takes IEnumerable<T>
                    ConstructorInfo ci = expectedType.GetConstructor(new Type[] { actualType });
                    if (ci != null)
                    {
                        body = Expression.New(ci, p);
                    }
                }
                if (body != null)
                {
                    return Expression.Lambda(body, p);
                }
            }
            return null;
        }

        private Expression CoerceElement(Type expectedElementType, Expression expression)
        {
            Type elementType = TypeHelper.GetElementType(expression.Type);
            if (expectedElementType != elementType && (expectedElementType.IsAssignableFrom(elementType) || elementType.IsAssignableFrom(expectedElementType)))
            {
                return Expression.Call(typeof(Enumerable), "Cast", new Type[] { expectedElementType }, expression);
            }
            return expression;
        }

        /// <summary>
        /// Apply mapping translations to this expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Translate(Expression expression)
        {
            // convert references to LINQ operators into query specific nodes
            expression = QueryBinder.Bind(this, expression);

            // move aggregate computations so they occur in same select as group-by
            expression = AggregateRewriter.Rewrite(expression);

            // do reduction so duplicate association's are likely to be clumped together
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            // convert references to association properties into correlated queries
            expression = RelationshipBinder.Bind(this, expression);

            // clean up after ourselves! (multiple references to same association property)
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            return expression;
        }
    }
}
