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

namespace Nequeo.Data.Linq.Common.Expressions
{
    /// <summary>
    /// Extended node types for custom expressions
    /// </summary>
    public enum DbExpressionType
    {
        /// <summary>
        /// 
        /// </summary>
        Table = 1000, // make sure these don't overlap with ExpressionType
        /// <summary>
        /// 
        /// </summary>
        ClientJoin,
        /// <summary>
        /// 
        /// </summary>
        Column,
        /// <summary>
        /// 
        /// </summary>
        Select,
        /// <summary>
        /// 
        /// </summary>
        Projection,
        /// <summary>
        /// 
        /// </summary>
        Join,
        /// <summary>
        /// 
        /// </summary>
        Aggregate,
        /// <summary>
        /// 
        /// </summary>
        Scalar,
        /// <summary>
        /// 
        /// </summary>
        Exists,
        /// <summary>
        /// 
        /// </summary>
        In,
        /// <summary>
        /// 
        /// </summary>
        Grouping,
        /// <summary>
        /// 
        /// </summary>
        AggregateSubquery,
        /// <summary>
        /// 
        /// </summary>
        IsNull,
        /// <summary>
        /// 
        /// </summary>
        Between,
        /// <summary>
        /// 
        /// </summary>
        RowCount,
        /// <summary>
        /// 
        /// </summary>
        NamedValue,
        /// <summary>
        /// 
        /// </summary>
        OuterJoined
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DbExpressionTypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="et"></param>
        /// <returns></returns>
        public static bool IsDbExpression(this ExpressionType et)
        {
            return ((int)et) >= 1000;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DbExpression : Expression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="type"></param>
        protected DbExpression(DbExpressionType eType, Type type)
        {
            _type = type;
            _nodeType = (ExpressionType)eType;
        }

        private ExpressionType _nodeType;
        private Type _type;

        /// <summary>
        /// 
        /// </summary>
        public override ExpressionType NodeType
        {
            get
            {
                return _nodeType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Type Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AliasedExpression : DbExpression
    {
        TableAlias alias;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="type"></param>
        /// <param name="alias"></param>
        protected AliasedExpression(DbExpressionType nodeType, Type type, TableAlias alias)
            : base(nodeType, type)
        {
            this.alias = alias;
        }

        /// <summary>
        /// 
        /// </summary>
        public TableAlias Alias
        {
            get { return this.alias; }
        }
    }

    /// <summary>
    /// A custom expression node that represents a table reference in a SQL query
    /// </summary>
    public class TableExpression : AliasedExpression
    {
        string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        public TableExpression(TableAlias alias, string name)
            : base(DbExpressionType.Table, typeof(void), alias)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "T(" + this.name + ")";
        }
    }

    /// <summary>
    /// A custom expression node that represents a reference to a column in a SQL query
    /// </summary>
    public class ColumnExpression : DbExpression, IEquatable<ColumnExpression>
    {
        TableAlias alias;
        string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        public ColumnExpression(Type type, TableAlias alias, string name)
            : base(DbExpressionType.Column, type)
        {
            this.alias = alias;
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public TableAlias Alias
        {
            get { return this.alias; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Alias.ToString() + ".C(" + this.name + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return alias.GetHashCode() + name.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ColumnExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ColumnExpression other)
        {
            return other != null
                && ((object)this) == (object)other
                 || (alias == other.alias && name == other.Name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableAlias
    {
        /// <summary>
        /// 
        /// </summary>
        public TableAlias()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "A:" + this.GetHashCode();
        }
    }

    /// <summary>
    /// A declaration of a column in a SQL SELECT expression
    /// </summary>
    public class ColumnDeclaration
    {
        string name;
        Expression expression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        public ColumnDeclaration(string name, Expression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    /// <summary>
    /// An SQL OrderBy order type 
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 
        /// </summary>
        Ascending,
        /// <summary>
        /// 
        /// </summary>
        Descending
    }

    /// <summary>
    /// A pairing of an expression and an order type for use in a SQL Order By clause
    /// </summary>
    public class OrderExpression
    {
        OrderType orderType;
        Expression expression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="expression"></param>
        public OrderExpression(OrderType orderType, Expression expression)
        {
            this.orderType = orderType;
            this.expression = expression;
        }

        /// <summary>
        /// 
        /// </summary>
        public OrderType OrderType
        {
            get { return this.orderType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    /// <summary>
    /// A custom expression node used to represent a SQL SELECT expression
    /// </summary>
    public class SelectExpression : AliasedExpression
    {
        ReadOnlyCollection<ColumnDeclaration> columns;
        bool isDistinct;
        Expression from;
        Expression where;
        ReadOnlyCollection<OrderExpression> orderBy;
        ReadOnlyCollection<Expression> groupBy;
        Expression take;
        Expression skip;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="columns"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="groupBy"></param>
        /// <param name="isDistinct"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        public SelectExpression(
            TableAlias alias,
            IEnumerable<ColumnDeclaration> columns,
            Expression from,
            Expression where,
            IEnumerable<OrderExpression> orderBy,
            IEnumerable<Expression> groupBy,
            bool isDistinct,
            Expression skip,
            Expression take)
            : base(DbExpressionType.Select, typeof(void), alias)
        {
            this.columns = columns as ReadOnlyCollection<ColumnDeclaration>;
            if (this.columns == null)
            {
                this.columns = new List<ColumnDeclaration>(columns).AsReadOnly();
            }
            this.isDistinct = isDistinct;
            this.from = from;
            this.where = where;
            this.orderBy = orderBy as ReadOnlyCollection<OrderExpression>;
            if (this.orderBy == null && orderBy != null)
            {
                this.orderBy = new List<OrderExpression>(orderBy).AsReadOnly();
            }
            this.groupBy = groupBy as ReadOnlyCollection<Expression>;
            if (this.groupBy == null && groupBy != null)
            {
                this.groupBy = new List<Expression>(groupBy).AsReadOnly();
            }
            this.take = take;
            this.skip = skip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="columns"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="groupBy"></param>
        public SelectExpression(
            TableAlias alias,
            IEnumerable<ColumnDeclaration> columns,
            Expression from,
            Expression where,
            IEnumerable<OrderExpression> orderBy,
            IEnumerable<Expression> groupBy
            )
            : this(alias, columns, from, where, orderBy, groupBy, false, null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="columns"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        public SelectExpression(
            TableAlias alias, IEnumerable<ColumnDeclaration> columns,
            Expression from, Expression where
            )
            : this(alias, columns, from, where, null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<ColumnDeclaration> Columns
        {
            get { return this.columns; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression From
        {
            get { return this.from; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Where
        {
            get { return this.where; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get { return this.orderBy; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<Expression> GroupBy
        {
            get { return this.groupBy; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDistinct
        {
            get { return this.isDistinct; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Skip
        {
            get { return this.skip; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Take
        {
            get { return this.take; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string QueryText
        {
            get { return TSqlFormatter.Format(this); }
        }
    }

    /// <summary>
    /// A kind of SQL join
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// 
        /// </summary>
        CrossJoin,
        /// <summary>
        /// 
        /// </summary>
        InnerJoin,
        /// <summary>
        /// 
        /// </summary>
        CrossApply,
        /// <summary>
        /// 
        /// </summary>
        OuterApply,
        /// <summary>
        /// 
        /// </summary>
        LeftOuter
    }

    /// <summary>
    /// A custom expression node representing a SQL join clause
    /// </summary>
    public class JoinExpression : DbExpression
    {
        JoinType joinType;
        Expression left;
        Expression right;
        Expression condition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="condition"></param>
        public JoinExpression(JoinType joinType, Expression left, Expression right, Expression condition)
            : base(DbExpressionType.Join, typeof(void))
        {
            this.joinType = joinType;
            this.left = left;
            this.right = right;
            this.condition = condition;
        }

        /// <summary>
        /// 
        /// </summary>
        public JoinType Join
        {
            get { return this.joinType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Left
        {
            get { return this.left; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Right
        {
            get { return this.right; }
        }

        /// <summary>
        /// 
        /// </summary>
        public new Expression Condition
        {
            get { return this.condition; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OuterJoinedExpression : DbExpression
    {
        Expression test;
        Expression expression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        /// <param name="expression"></param>
        public OuterJoinedExpression(Expression test, Expression expression)
            : base(DbExpressionType.OuterJoined, expression.Type)
        {
            this.test = test;
            this.expression = expression;
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Test
        {
            get { return this.test; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class SubqueryExpression : DbExpression
    {
        SelectExpression select;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="type"></param>
        /// <param name="select"></param>
        protected SubqueryExpression(DbExpressionType eType, Type type, SelectExpression select)
            : base(eType, type)
        {
            System.Diagnostics.Debug.Assert(eType == DbExpressionType.Scalar || eType == DbExpressionType.Exists || eType == DbExpressionType.In);
            this.select = select;
        }

        /// <summary>
        /// 
        /// </summary>
        public SelectExpression Select
        {
            get { return this.select; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScalarExpression : SubqueryExpression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="select"></param>
        public ScalarExpression(Type type, SelectExpression select)
            : base(DbExpressionType.Scalar, type, select)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExistsExpression : SubqueryExpression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        public ExistsExpression(SelectExpression select)
            : base(DbExpressionType.Exists, typeof(bool), select)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InExpression : SubqueryExpression
    {
        Expression expression;
        ReadOnlyCollection<Expression> values;  // either select or expressions are assigned

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="select"></param>
        public InExpression(Expression expression, SelectExpression select)
            : base(DbExpressionType.In, typeof(bool), select)
        {
            this.expression = expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="values"></param>
        public InExpression(Expression expression, IEnumerable<Expression> values)
            : base(DbExpressionType.In, typeof(bool), null)
        {
            this.expression = expression;
            this.values = values as ReadOnlyCollection<Expression>;
            if (this.values == null && values != null)
            {
                this.values = new List<Expression>(values).AsReadOnly();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<Expression> Values
        {
            get { return this.values; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum AggregateType
    {
        /// <summary>
        /// 
        /// </summary>
        Count,
        /// <summary>
        /// 
        /// </summary>
        Min,
        /// <summary>
        /// 
        /// </summary>
        Max,
        /// <summary>
        /// 
        /// </summary>
        Sum,
        /// <summary>
        /// 
        /// </summary>
        Average
    }

    /// <summary>
    /// 
    /// </summary>
    public class AggregateExpression : DbExpression
    {
        AggregateType aggType;
        Expression argument;
        bool isDistinct;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="aggType"></param>
        /// <param name="argument"></param>
        /// <param name="isDistinct"></param>
        public AggregateExpression(Type type, AggregateType aggType, Expression argument, bool isDistinct)
            : base(DbExpressionType.Aggregate, type)
        {
            this.aggType = aggType;
            this.argument = argument;
            this.isDistinct = isDistinct;
        }

        /// <summary>
        /// 
        /// </summary>
        public AggregateType AggregateType
        {
            get { return this.aggType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Argument
        {
            get { return this.argument; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDistinct
        {
            get { return this.isDistinct; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AggregateSubqueryExpression : DbExpression
    {
        TableAlias groupByAlias;
        Expression aggregateInGroupSelect;
        ScalarExpression aggregateAsSubquery;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupByAlias"></param>
        /// <param name="aggregateInGroupSelect"></param>
        /// <param name="aggregateAsSubquery"></param>
        public AggregateSubqueryExpression(TableAlias groupByAlias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base(DbExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            this.aggregateInGroupSelect = aggregateInGroupSelect;
            this.groupByAlias = groupByAlias;
            this.aggregateAsSubquery = aggregateAsSubquery;
        }

        /// <summary>
        /// 
        /// </summary>
        public TableAlias GroupByAlias { get { return this.groupByAlias; } }

        /// <summary>
        /// 
        /// </summary>
        public Expression AggregateInGroupSelect { get { return this.aggregateInGroupSelect; } }

        /// <summary>
        /// 
        /// </summary>
        public ScalarExpression AggregateAsSubquery { get { return this.aggregateAsSubquery; } }
    }

    /// <summary>
    /// Allows is-null tests against value-types like int and float
    /// </summary>
    public class IsNullExpression : DbExpression
    {
        Expression expression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public IsNullExpression(Expression expression)
            : base(DbExpressionType.IsNull, typeof(bool))
        {
            this.expression = expression;
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BetweenExpression : DbExpression
    {
        Expression expression;
        Expression lower;
        Expression upper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public BetweenExpression(Expression expression, Expression lower, Expression upper)
            : base(DbExpressionType.Between, expression.Type)
        {
            this.expression = expression;
            this.lower = lower;
            this.upper = upper;
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Lower
        {
            get { return this.lower; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Upper
        {
            get { return this.upper; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RowNumberExpression : DbExpression
    {
        ReadOnlyCollection<OrderExpression> orderBy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        public RowNumberExpression(IEnumerable<OrderExpression> orderBy)
            : base(DbExpressionType.RowCount, typeof(int))
        {
            this.orderBy = orderBy as ReadOnlyCollection<OrderExpression>;
            if (this.orderBy == null && orderBy != null)
            {
                this.orderBy = new List<OrderExpression>(orderBy).AsReadOnly();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get { return this.orderBy; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NamedValueExpression : DbExpression
    {
        string name;
        Expression value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NamedValueExpression(string name, Expression value)
            : base(DbExpressionType.NamedValue, value.Type)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Value
        {
            get { return this.value; }
        }
    }

    /// <summary>
    /// A custom expression representing the construction of one or more result objects from a 
    /// SQL select expression
    /// </summary>
    public class ProjectionExpression : DbExpression
    {
        SelectExpression source;
        Expression projector;
        LambdaExpression aggregator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="projector"></param>
        public ProjectionExpression(SelectExpression source, Expression projector)
            : this(source, projector, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="projector"></param>
        /// <param name="aggregator"></param>
        public ProjectionExpression( SelectExpression source, Expression projector, LambdaExpression aggregator)
            : base(DbExpressionType.Projection, aggregator != null ? aggregator.Body.Type : typeof(IEnumerable<>).MakeGenericType(projector.Type))
        {
            this.source = source;
            this.projector = projector;
            this.aggregator = aggregator;
        }

        /// <summary>
        /// 
        /// </summary>
        public SelectExpression Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Projector
        {
            get { return this.projector; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaExpression Aggregator
        {
            get { return this.aggregator; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSingleton
        {
            get { return this.aggregator != null && this.aggregator.Body.Type == projector.Type; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public string QueryText
        {
            get { return TSqlFormatter.Format(source); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ClientJoinExpression : DbExpression
    {
        ReadOnlyCollection<Expression> outerKey;
        ReadOnlyCollection<Expression> innerKey;
        ProjectionExpression projection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="outerKey"></param>
        /// <param name="innerKey"></param>
        public ClientJoinExpression(ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
            : base(DbExpressionType.ClientJoin, projection.Type)
        {
            this.outerKey = outerKey as ReadOnlyCollection<Expression>;
            if (this.outerKey == null)
            {
                this.outerKey = new List<Expression>(outerKey).AsReadOnly();
            }
            this.innerKey = innerKey as ReadOnlyCollection<Expression>;
            if (this.innerKey == null)
            {
                this.innerKey = new List<Expression>(innerKey).AsReadOnly();
            }
            this.projection = projection;
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<Expression> OuterKey
        {
            get { return this.outerKey; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<Expression> InnerKey
        {
            get { return this.innerKey; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProjectionExpression Projection
        {
            get { return this.projection; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DbExpressionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static SelectExpression SetColumns(this SelectExpression select, IEnumerable<ColumnDeclaration> columns)
        {
            return new SelectExpression(select.Alias, columns.OrderBy(c => c.Name), select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SelectExpression AddColumn(this SelectExpression select, ColumnDeclaration column)
        {
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>(select.Columns);
            columns.Add(column);
            return select.SetColumns(columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SelectExpression RemoveColumn(this SelectExpression select, ColumnDeclaration column)
        {
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>(select.Columns);
            columns.Remove(column);
            return select.SetColumns(columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public static string GetAvailableColumnName(this SelectExpression select, string baseName)
        {
            string name = baseName;
            int n = 0;
            while (!IsUniqueName(select, name))
            {
                name = baseName + (n++);
            }
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool IsUniqueName(SelectExpression select, string name)
        {
            foreach (var col in select.Columns)
            {
                if (col.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static ProjectionExpression AddOuterJoinTest(this ProjectionExpression proj)
        {
            string colName = proj.Source.GetAvailableColumnName("Test");
            SelectExpression newSource = proj.Source.AddColumn(new ColumnDeclaration(colName, Expression.Constant(1, typeof(int?))));
            Expression newProjector = 
                new OuterJoinedExpression(
                    new ColumnExpression(typeof(int?), newSource.Alias, colName),
                    proj.Projector
                    );
            return new ProjectionExpression(newSource, newProjector, proj.Aggregator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="isDistinct"></param>
        /// <returns></returns>
        public static SelectExpression SetDistinct(this SelectExpression select, bool isDistinct)
        {
            if (select.IsDistinct != isDistinct)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, isDistinct, select.Skip, select.Take);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static SelectExpression SetWhere(this SelectExpression select, Expression where)
        {
            if (where != select.Where)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static SelectExpression SetOrderBy(this SelectExpression select, IEnumerable<OrderExpression> orderBy)
        {
            return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, orderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public static SelectExpression AddOrderExpression(this SelectExpression select, OrderExpression ordering)
        {
            List<OrderExpression> orderby = new List<OrderExpression>();
            if (select.OrderBy != null)
                orderby.AddRange(select.OrderBy);
            orderby.Add(ordering);
            return select.SetOrderBy(orderby);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public static SelectExpression RemoveOrderExpression(this SelectExpression select, OrderExpression ordering)
        {
            if (select.OrderBy != null && select.OrderBy.Count > 0)
            {
                List<OrderExpression> orderby = new List<OrderExpression>(select.OrderBy);
                orderby.Remove(ordering);
                return select.SetOrderBy(orderby);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        public static SelectExpression SetGroupBy(this SelectExpression select, IEnumerable<Expression> groupBy)
        {
            return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, groupBy, select.IsDistinct, select.Skip, select.Take);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SelectExpression AddGroupExpression(this SelectExpression select, Expression expression)
        {
            List<Expression> groupby = new List<Expression>();
            if (select.GroupBy != null)
                groupby.AddRange(select.GroupBy);
            groupby.Add(expression);
            return select.SetGroupBy(groupby);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SelectExpression RemoveGroupExpression(this SelectExpression select, Expression expression)
        {
            if (select.GroupBy != null && select.GroupBy.Count > 0)
            {
                List<Expression> groupby = new List<Expression>(select.GroupBy);
                groupby.Remove(expression);
                return select.SetGroupBy(groupby);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static SelectExpression SetSkip(this SelectExpression select, Expression skip)
        {
            if (skip != select.Skip)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, skip, select.Take);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static SelectExpression SetTake(this SelectExpression select, Expression take)
        {
            if (take != select.Take)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, take);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="newAlias"></param>
        /// <returns></returns>
        public static SelectExpression AddRedundantSelect(this SelectExpression select, TableAlias newAlias)
        {
            var newColumns = select.Columns.Select(d => new ColumnDeclaration(d.Name, new ColumnExpression(d.Expression.Type, newAlias, d.Name)));
            var newFrom = new SelectExpression(newAlias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            return new SelectExpression(select.Alias, newColumns, newFrom, null, null, null, false, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public static SelectExpression RemoveRedundantFrom(this SelectExpression select)
        {
            SelectExpression fromSelect = select.From as SelectExpression;
            if (fromSelect != null)
            {
                return SubqueryRemover.Remove(select, fromSelect);
            }
            return select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static SelectExpression SetFrom(this SelectExpression select, Expression from)
        {
            if (select.From != from)
            {
                return new SelectExpression(select.Alias, select.Columns, from, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            }
            return select;
        }
    }
}
