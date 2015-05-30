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

namespace Nequeo.Data.Linq.Provider
{
    /// <summary>
    /// Translate a query expression to the equivalant sql query.
    /// </summary>
    public class QueryTranslater : IQueryText
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="policy">The sql policy provider.</param>
        public QueryTranslater(QueryPolicy policy)
        {
            this.policy = policy;
            this.mapping = policy.Mapping;
            this.language = mapping.Language;
        }

        QueryPolicy policy;
        QueryMapping mapping;
        QueryLanguage language;

        /// <summary>
        /// Gets, the query mapping.
        /// </summary>
        public QueryMapping Mapping
        {
            get { return this.mapping; }
        }

        /// <summary>
        /// Gets, the query langauge.
        /// </summary>
        public QueryLanguage Language
        {
            get { return this.language; }
        }

        /// <summary>
        /// Gets, the sql policy.
        /// </summary>
        public QueryPolicy Policy
        {
            get { return this.policy; }
        }

        /// <summary>
        /// Get the sql text from the query expression.
        /// </summary>
        /// <param name="expression">The expression to translate.</param>
        /// <returns>The sql query expression.</returns>
        public string GetQueryText(Expression expression)
        {
            Expression translated = this.Translate(expression);
            var selects = SelectGatherer.Gather(translated).Select(s => this.language.Format(s));
            return string.Join("\n\n", selects.ToArray()); throw new NotImplementedException();
        }

        /// <summary>
        /// Do all query translations execpt building the execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual ProjectionExpression Translate(Expression expression)
        {
            // pre-evaluate local sub-trees
            expression = PartialEvaluator.Eval(expression, this.CanBeEvaluatedLocally);

            // apply mapping (binds LINQ operators too)
            expression = this.mapping.Translate(expression);

            // any policy specific translations or validations
            expression = this.policy.Translate(expression);

            // any language specific translations or validations
            expression = this.language.Translate(expression);

            // do final reduction
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);

            return (ProjectionExpression)expression;
        }

        /// <summary>
        /// Determines whether a given expression can be executed locally. 
        /// (It contains no parts that should be translated to the target environment.)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual bool CanBeEvaluatedLocally(Expression expression)
        {
            // any operation on a query can't be done locally
            ConstantExpression cex = expression as ConstantExpression;
            if (cex != null)
            {
                IQueryable query = cex.Value as IQueryable;
                if (query != null && query.Provider == this)
                    return false;
            }
            MethodCallExpression mc = expression as MethodCallExpression;
            if (mc != null &&
                (mc.Method.DeclaringType == typeof(Enumerable) ||
                 mc.Method.DeclaringType == typeof(Queryable)))
            {
                return false;
            }
            if (expression.NodeType == ExpressionType.Convert &&
                expression.Type == typeof(object))
                return true;
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }
    }
}
