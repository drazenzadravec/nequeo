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

namespace Nequeo.Data.Linq.Common.Translation
{
    /// <summary>
    /// Adds relationship to query results depending on policy
    /// </summary>
    public class RelationshipIncluder : DbExpressionVisitor
    {
        QueryPolicy policy;
        QueryMapping mapping;
        ScopedDictionary<MemberInfo, bool> includeScope = new ScopedDictionary<MemberInfo, bool>(null);

        private RelationshipIncluder(QueryPolicy policy)
        {
            this.policy = policy;
            this.mapping = policy.Mapping;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression Include(QueryPolicy policy, Expression expression)
        {
            return new RelationshipIncluder(policy).Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            Expression projector = this.Visit(proj.Projector);
            if (projector != proj.Projector)
            {
                return new ProjectionExpression(proj.Source, projector, proj.Aggregator);
            }
            return proj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            if (this.mapping.IsEntity(init.Type))
            {
                var save = this.includeScope;
                this.includeScope = new ScopedDictionary<MemberInfo,bool>(this.includeScope);

                Dictionary<MemberInfo, MemberBinding> existing = init.Bindings.ToDictionary(b => b.Member);
                List<MemberBinding> newBindings = null;
                foreach (var mi in this.mapping.GetMappedMembers(init.Type))
                {
                    if (!existing.ContainsKey(mi) && this.mapping.IsRelationship(mi) && this.policy.IsIncluded(mi))
                    {
                        if (this.includeScope.ContainsKey(mi))
                        {
                            throw new NotSupportedException(string.Format("Cannot include '{0}.{1}' recursively.", mi.DeclaringType.Name, mi.Name));
                        }
                        Expression me = this.mapping.GetMemberExpression(init, mi);
                        if (newBindings == null)
                        {
                            newBindings = new List<MemberBinding>(init.Bindings);
                        }
                        newBindings.Add(Expression.Bind(mi, me));
                    }
                }
                if (newBindings != null)
                {
                    init = Expression.MemberInit(init.NewExpression, newBindings);
                }

                this.includeScope = save;
            }
            return base.VisitMemberInit(init);
        }
    }
}
