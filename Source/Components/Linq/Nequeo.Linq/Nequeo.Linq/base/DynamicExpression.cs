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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

using Nequeo.Data.TypeExtenders;

namespace Nequeo.Linq
{
    /// <summary>
    /// Dynamic expression builder.
    /// </summary>
    public static class DynamicExpression
    {
        /// <summary>
        /// Parse string to expression.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        /// <param name="expression">The string expression.</param>
        /// <param name="values">The values.</param>
        /// <returns>The expression.</returns>
        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        /// <summary>
        /// Parse string lambda expression.
        /// </summary>
        /// <param name="itType">Initial type.</param>
        /// <param name="resultType">Result ype.</param>
        /// <param name="expression">The string expression.</param>
        /// <param name="values">The values.</param>
        /// <returns>The lambda expression.</returns>
        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, values);
        }

        /// <summary>
        /// Parse string lambda expression.
        /// </summary>
        /// <param name="parameters">The parameter expressions.</param>
        /// <param name="resultType">The result ype.</param>
        /// <param name="expression">The string expression.</param>
        /// <param name="values">The values.</param>
        /// <returns>The lambda expression.</returns>
        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        /// <summary>
        /// The string to lambda expression.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <typeparam name="S">The result type.</typeparam>
        /// <param name="expression">The string expression.</param>
        /// <param name="values">The values.</param>
        /// <returns>The lambda expression.</returns>
        public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
        {
            return (Expression<Func<T, S>>)ParseLambda(typeof(T), typeof(S), expression, values);
        }

        /// <summary>
        /// Create dynamic class type.
        /// </summary>
        /// <param name="properties">The dynamic properties.</param>
        /// <returns>The dynamic class type.</returns>
        public static Type CreateClass(params Nequeo.Reflection.DynamicProperty[] properties)
        {
            return Nequeo.Reflection.DynamicClassBuilder.Instance.GetDynamicClass(properties);
        }

        /// <summary>
        /// Create dynamic class type.
        /// </summary>
        /// <param name="properties">The dynamic properties.</param>
        /// <returns>The dynamic class type.</returns>
        public static Type CreateClass(IEnumerable<Nequeo.Reflection.DynamicProperty> properties)
        {
            return Nequeo.Reflection.DynamicClassBuilder.Instance.GetDynamicClass(properties);
        }
    }

    /// <summary>
    /// Dynamic ordering.
    /// </summary>
    internal class DynamicOrdering
    {
        /// <summary>
        /// The slector expression.
        /// </summary>
        public Expression Selector;

        /// <summary>
        /// Is ascending.
        /// </summary>
        public bool Ascending;
    }

    /// <summary>
    /// Parse exception.
    /// </summary>
    public sealed class ParseException : Exception
    {
        int position;

        /// <summary>
        /// Parse exception.
        /// </summary>
        /// <param name="message">THe message.</param>
        /// <param name="position">The position.</param>
        public ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public int Position
        {
            get { return position; }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>The message.</returns>
        public override string ToString()
        {
            return string.Format(Res.ParseExceptionFormat, Message, position);
        }
    }

    /// <summary>
    /// Expression parser.
    /// </summary>
    internal class ExpressionParser
    {
        /// <summary>
        /// Token structure.
        /// </summary>
        struct Token
        {
            /// <summary>
            /// Token id.
            /// </summary>
            public TokenId id;

            /// <summary>
            /// The text.
            /// </summary>
            public string text;

            /// <summary>
            /// The position.
            /// </summary>
            public int pos;
        }

        /// <summary>
        /// Token id.
        /// </summary>
        enum TokenId
        {
            Unknown,
            End,
            Identifier,
            StringLiteral,
            IntegerLiteral,
            RealLiteral,
            Exclamation,
            Percent,
            Amphersand,
            OpenParen,
            CloseParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Dot,
            Slash,
            Colon,
            LessThan,
            Equal,
            GreaterThan,
            Question,
            OpenBracket,
            CloseBracket,
            Bar,
            ExclamationEqual,
            DoubleAmphersand,
            LessThanEqual,
            LessGreater,
            DoubleEqual,
            GreaterThanEqual,
            DoubleBar
        }

        /// <summary>
        /// Logical signatures interface.
        /// </summary>
        interface ILogicalSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(bool x, bool y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(bool? x, bool? y);
        }

        /// <summary>
        /// Arithmetic signatures interface.
        /// </summary>
        interface IArithmeticSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(int x, int y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(uint x, uint y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(long x, long y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(ulong x, ulong y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(float x, float y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(double x, double y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(decimal x, decimal y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(int? x, int? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(uint? x, uint? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(long? x, long? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(ulong? x, ulong? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(float? x, float? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(double? x, double? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(decimal? x, decimal? y);
        }

        /// <summary>
        /// Relational signatures interface.
        /// </summary>
        interface IRelationalSignatures : IArithmeticSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(string x, string y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(char x, char y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(DateTime x, DateTime y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(TimeSpan x, TimeSpan y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(char? x, char? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(DateTime? x, DateTime? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(TimeSpan? x, TimeSpan? y);
        }

        /// <summary>
        /// Equality signatures interface.
        /// </summary>
        interface IEqualitySignatures : IRelationalSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(bool x, bool y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(bool? x, bool? y);
        }

        /// <summary>
        /// Add signatures interface.
        /// </summary>
        interface IAddSignatures : IArithmeticSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(DateTime x, TimeSpan y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(TimeSpan x, TimeSpan y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(DateTime? x, TimeSpan? y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(TimeSpan? x, TimeSpan? y);
        }

        /// <summary>
        /// Subtract signatures interface.
        /// </summary>
        interface ISubtractSignatures : IAddSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(DateTime x, DateTime y);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            void F(DateTime? x, DateTime? y);
        }

        /// <summary>
        /// Negation signatures interace.
        /// </summary>
        interface INegationSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(int x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(long x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(float x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(double x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(decimal x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(int? x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(long? x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(float? x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(double? x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(decimal? x);
        }

        /// <summary>
        /// Not signatures interface.
        /// </summary>
        interface INotSignatures
        {
            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(bool x);

            /// <summary>
            /// Compare.
            /// </summary>
            /// <param name="x">The x value.</param>
            void F(bool? x);
        }

        /// <summary>
        /// Enumerable signatures interface.
        /// </summary>
        interface IEnumerableSignatures
        {
            /// <summary>
            /// Where
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            void Where(bool predicate);

            /// <summary>
            /// Any
            /// </summary>
            void Any();

            /// <summary>
            /// Any
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            void Any(bool predicate);

            /// <summary>
            /// All
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            void All(bool predicate);

            /// <summary>
            /// Count
            /// </summary>
            void Count();

            /// <summary>
            /// Count
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            void Count(bool predicate);

            /// <summary>
            /// Min
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Min(object selector);

            /// <summary>
            /// Max
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Max(object selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(int selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(int? selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(long selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(long? selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(float selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(float? selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(double selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(double? selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(decimal selector);

            /// <summary>
            /// Sum
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Sum(decimal? selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(int selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(int? selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(long selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(long? selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(float selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(float? selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(double selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(double? selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(decimal selector);

            /// <summary>
            /// Average
            /// </summary>
            /// <param name="selector">Selector.</param>
            void Average(decimal? selector);
        }

        /// <summary>
        /// Predefined types.
        /// </summary>
        static readonly Type[] predefinedTypes = {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert),
            typeof(SqlQueryMethods)
        };

        static readonly Expression trueLiteral = Expression.Constant(true);
        static readonly Expression falseLiteral = Expression.Constant(false);
        static readonly Expression nullLiteral = Expression.Constant(null);

        static readonly string keywordIt = "it";
        static readonly string keywordIif = "iif";
        static readonly string keywordNew = "new";

        static Dictionary<string, object> keywords;

        Dictionary<string, object> symbols;
        IDictionary<string, object> externals;
        Dictionary<Expression, string> literals;
        ParameterExpression it;

        string text;
        int textPos;
        int textLen;
        char ch;
        Token token;

        /// <summary>
        /// Expression Parser.
        /// </summary>
        /// <param name="parameters">The expression parameters.</param>
        /// <param name="expression">The string expression.</param>
        /// <param name="values">The values.</param>
        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (keywords == null) keywords = CreateKeywords();
            symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            literals = new Dictionary<Expression, string>();
            if (parameters != null) ProcessParameters(parameters);
            if (values != null) ProcessValues(values);
            text = expression;
            textLen = text.Length;
            SetTextPos(0);
            NextToken();
        }

        /// <summary>
        /// Process parameters.
        /// </summary>
        /// <param name="parameters">The expression parameters.</param>
        void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression pe in parameters)
                if (!String.IsNullOrEmpty(pe.Name))
                    AddSymbol(pe.Name, pe);
            if (parameters.Length == 1 && String.IsNullOrEmpty(parameters[0].Name))
                it = parameters[0];
        }

        /// <summary>
        /// Process values.
        /// </summary>
        /// <param name="values">The values.</param>
        void ProcessValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object value = values[i];
                if (i == values.Length - 1 && value is IDictionary<string, object>)
                {
                    externals = (IDictionary<string, object>)value;
                }
                else
                {
                    AddSymbol("@" + i.ToString(System.Globalization.CultureInfo.InvariantCulture), value);
                }
            }
        }

        /// <summary>
        /// Add symbol
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddSymbol(string name, object value)
        {
            if (symbols.ContainsKey(name))
                throw ParseError(Res.DuplicateIdentifier, name);
            symbols.Add(name, value);
        }

        /// <summary>
        /// Parse.
        /// </summary>
        /// <param name="resultType">Result type.</param>
        /// <returns>The expression.</returns>
        public Expression Parse(Type resultType)
        {
            int exprPos = token.pos;
            Expression expr = ParseExpression();
            if (resultType != null)
                if ((expr = PromoteExpression(expr, resultType, true)) == null)
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType));
            ValidateToken(TokenId.End, Res.SyntaxError);
            return expr;
        }

#pragma warning disable 0219
        /// <summary>
        /// Parse ordering.
        /// </summary>
        /// <returns>Dynamic ordering list.</returns>
        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            List<DynamicOrdering> orderings = new List<DynamicOrdering>();
            while (true)
            {
                Expression expr = ParseExpression();
                bool ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    NextToken();
                    ascending = false;
                }
                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending });
                if (token.id != TokenId.Comma) break;
                NextToken();
            }
            ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }
#pragma warning restore 0219

        /// <summary>
        /// Parse expression.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseExpression()
        {
            int errorPos = token.pos;
            Expression expr = ParseLogicalOr();
            if (token.id == TokenId.Question)
            {
                NextToken();
                Expression expr1 = ParseExpression();
                ValidateToken(TokenId.Colon, Res.ColonExpected);
                NextToken();
                Expression expr2 = ParseExpression();
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        /// <summary>
        /// Parse logical Or.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseLogicalOr()
        {
            Expression left = ParseLogicalAnd();
            while (token.id == TokenId.DoubleBar || TokenIdentifierIs("or"))
            {
                Token op = token;
                NextToken();
                Expression right = ParseLogicalAnd();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parse logical And.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseLogicalAnd()
        {
            //&&, and operator
            Expression left = ParseComparison();
            while (token.id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
            {
                Token op = token;
                NextToken();
                Expression right = ParseComparison();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parse comparison.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseComparison()
        {
            // =, ==, !=, <>, >, >=, <, <= operators
            Expression left = ParseAdditive();
            while (token.id == TokenId.Equal || token.id == TokenId.DoubleEqual ||
                token.id == TokenId.ExclamationEqual || token.id == TokenId.LessGreater ||
                token.id == TokenId.GreaterThan || token.id == TokenId.GreaterThanEqual ||
                token.id == TokenId.LessThan || token.id == TokenId.LessThanEqual)
            {
                Token op = token;
                NextToken();
                Expression right = ParseAdditive();
                bool isEquality = op.id == TokenId.Equal || op.id == TokenId.DoubleEqual ||
                    op.id == TokenId.ExclamationEqual || op.id == TokenId.LessGreater;
                if (isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
                {
                    if (left.Type != right.Type)
                    {
                        if (left.Type.IsAssignableFrom(right.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (right.Type.IsAssignableFrom(left.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression e;
                        if ((e = PromoteExpression(right, left.Type, true)) != null)
                        {
                            right = e;
                        }
                        else if ((e = PromoteExpression(left, right.Type, true)) != null)
                        {
                            left = e;
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                    }
                }
                else
                {
                    CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures),
                        op.text, ref left, ref right, op.pos);
                }
                switch (op.id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = GenerateLessThanEqual(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parse additive.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseAdditive()
        {
            // +, -, & operators
            Expression left = ParseMultiplicative();
            while (token.id == TokenId.Plus || token.id == TokenId.Minus ||
                token.id == TokenId.Amphersand)
            {
                Token op = token;
                NextToken();
                Expression right = ParseMultiplicative();
                switch (op.id)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                            goto case TokenId.Amphersand;
                        CheckAndPromoteOperands(typeof(IAddSignatures), op.text, ref left, ref right, op.pos);
                        left = GenerateAdd(left, right);
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.text, ref left, ref right, op.pos);
                        left = GenerateSubtract(left, right);
                        break;
                    case TokenId.Amphersand:
                        left = GenerateStringConcat(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parse multiplicative.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseMultiplicative()
        {
            Expression left = ParseUnary();
            while (token.id == TokenId.Asterisk || token.id == TokenId.Slash ||
                token.id == TokenId.Percent || TokenIdentifierIs("mod"))
            {
                Token op = token;
                NextToken();
                Expression right = ParseUnary();
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
                switch (op.id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Percent:
                    case TokenId.Identifier:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parse -, !, not unary operators
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseUnary()
        {
            if (token.id == TokenId.Minus || token.id == TokenId.Exclamation ||
                TokenIdentifierIs("not"))
            {
                Token op = token;
                NextToken();
                if (op.id == TokenId.Minus && (token.id == TokenId.IntegerLiteral ||
                    token.id == TokenId.RealLiteral))
                {
                    token.text = "-" + token.text;
                    token.pos = op.pos;
                    return ParsePrimary();
                }
                Expression expr = ParseUnary();
                if (op.id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Not(expr);
                }
                return expr;
            }
            return ParsePrimary();
        }

        /// <summary>
        /// Parse primary.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParsePrimary()
        {
            Expression expr = ParsePrimaryStart();
            while (true)
            {
                if (token.id == TokenId.Dot)
                {
                    NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (token.id == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        /// <summary>
        /// Parse primary start.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParsePrimaryStart()
        {
            switch (token.id)
            {
                case TokenId.Identifier:
                    return ParseIdentifier();
                case TokenId.StringLiteral:
                    return ParseStringLiteral();
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral();
                case TokenId.RealLiteral:
                    return ParseRealLiteral();
                case TokenId.OpenParen:
                    return ParseParenExpression();
                default:
                    throw ParseError(Res.ExpressionExpected);
            }
        }

        /// <summary>
        /// Parse string literal.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseStringLiteral()
        {
            ValidateToken(TokenId.StringLiteral);
            char quote = token.text[0];
            string s = token.text.Substring(1, token.text.Length - 2);
            int start = 0;
            while (true)
            {
                int i = s.IndexOf(quote, start);
                if (i < 0) break;
                s = s.Remove(i, 1);
                start = i + 1;
            }
            if (quote == '\'')
            {
                if (s.Length != 1)
                    throw ParseError(Res.InvalidCharacterLiteral);
                NextToken();
                return CreateLiteral(s[0], s);
            }
            NextToken();
            return CreateLiteral(s, s);
        }

        /// <summary>
        /// Parse integer literal.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseIntegerLiteral()
        {
            ValidateToken(TokenId.IntegerLiteral);
            string text = token.text;
            if (text[0] != '-')
            {
                ulong value;
                if (!UInt64.TryParse(text, out value))
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                NextToken();
                if (value <= (ulong)Int32.MaxValue) return CreateLiteral((int)value, text);
                if (value <= (ulong)UInt32.MaxValue) return CreateLiteral((uint)value, text);
                if (value <= (ulong)Int64.MaxValue) return CreateLiteral((long)value, text);
                return CreateLiteral(value, text);
            }
            else
            {
                long value;
                if (!Int64.TryParse(text, out value))
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                NextToken();
                if (value >= Int32.MinValue && value <= Int32.MaxValue)
                    return CreateLiteral((int)value, text);
                return CreateLiteral(value, text);
            }
        }

        /// <summary>
        /// Parse real literal.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseRealLiteral()
        {
            ValidateToken(TokenId.RealLiteral);
            string text = token.text;
            object value = null;
            char last = text[text.Length - 1];
            if (last == 'F' || last == 'f')
            {
                float f;
                if (Single.TryParse(text.Substring(0, text.Length - 1), out f)) value = f;
            }
            else
            {
                double d;
                if (Double.TryParse(text, out d)) value = d;
            }
            if (value == null) throw ParseError(Res.InvalidRealLiteral, text);
            NextToken();
            return CreateLiteral(value, text);
        }

        /// <summary>
        /// Create literal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="text">The text.</param>
        /// <returns>The expression.</returns>
        Expression CreateLiteral(object value, string text)
        {
            ConstantExpression expr = Expression.Constant(value);
            literals.Add(expr, text);
            return expr;
        }

        /// <summary>
        /// Parse paren expression.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseParenExpression()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            Expression e = ParseExpression();
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            NextToken();
            return e;
        }

        /// <summary>
        /// Parse identifier.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseIdentifier()
        {
            ValidateToken(TokenId.Identifier);
            object value;
            if (keywords.TryGetValue(token.text, out value))
            {
                if (value is Type) return ParseTypeAccess((Type)value);
                if (value == (object)keywordIt) return ParseIt();
                if (value == (object)keywordIif) return ParseIif();
                if (value == (object)keywordNew) return ParseNew();
                NextToken();
                return (Expression)value;
            }
            if (symbols.TryGetValue(token.text, out value) ||
                externals != null && externals.TryGetValue(token.text, out value))
            {
                Expression expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    LambdaExpression lambda = expr as LambdaExpression;
                    if (lambda != null) return ParseLambdaInvocation(lambda);
                }
                NextToken();
                return expr;
            }
            if (it != null) return ParseMemberAccess(null, it);
            throw ParseError(Res.UnknownIdentifier, token.text);
        }

        /// <summary>
        /// Parse It.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseIt()
        {
            if (it == null)
                throw ParseError(Res.NoItInScope);
            NextToken();
            return it;
        }

        /// <summary>
        /// Parse Iif.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseIif()
        {
            int errorPos = token.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 3)
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        /// <summary>
        /// Generate conditional.
        /// </summary>
        /// <param name="test">The test expression.</param>
        /// <param name="expr1">Expression</param>
        /// <param name="expr2">Expression</param>
        /// <param name="errorPos">Error position.</param>
        /// <returns>The expression.</returns>
        Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            if (expr1.Type != expr2.Type)
            {
                Expression expr1as2 = expr2 != nullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
                Expression expr2as1 = expr1 != nullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
                if (expr1as2 != null && expr2as1 == null)
                {
                    expr1 = expr1as2;
                }
                else if (expr2as1 != null && expr1as2 == null)
                {
                    expr2 = expr2as1;
                }
                else
                {
                    string type1 = expr1 != nullLiteral ? expr1.Type.Name : "null";
                    string type2 = expr2 != nullLiteral ? expr2.Type.Name : "null";
                    if (expr1as2 != null && expr2as1 != null)
                        throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
                }
            }
            return Expression.Condition(test, expr1, expr2);
        }

        /// <summary>
        /// Parse new.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression ParseNew()
        {
            NextToken();
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            List<Nequeo.Reflection.DynamicProperty> properties = new List<Nequeo.Reflection.DynamicProperty>();
            List<Expression> expressions = new List<Expression>();
            while (true)
            {
                int exprPos = token.pos;
                Expression expr = ParseExpression();
                string propName;
                if (TokenIdentifierIs("as"))
                {
                    NextToken();
                    propName = GetIdentifier();
                    NextToken();
                }
                else
                {
                    MemberExpression me = expr as MemberExpression;
                    if (me == null) throw ParseError(exprPos, Res.MissingAsClause);
                    propName = me.Member.Name;
                }
                expressions.Add(expr);
                properties.Add(new Nequeo.Reflection.DynamicProperty(propName, expr.Type));
                if (token.id != TokenId.Comma) break;
                NextToken();
            }
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            Type type = DynamicExpression.CreateClass(properties);
            MemberBinding[] bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < bindings.Length; i++)
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        /// <summary>
        /// Parse lambda invocation.
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The expression.</returns>
        Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            int errorPos = token.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            MethodBase method;
            if (FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            return Expression.Invoke(lambda, args);
        }

        /// <summary>
        /// Parse type access.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The expression.</returns>
        Expression ParseTypeAccess(Type type)
        {
            int errorPos = token.pos;
            NextToken();
            if (token.id == TokenId.Question)
            {
                if (!type.IsValueType || IsNullableType(type))
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type));
                type = typeof(Nullable<>).MakeGenericType(type);
                NextToken();
            }
            if (token.id == TokenId.OpenParen)
            {
                Expression[] args = ParseArgumentList();
                MethodBase method;
                switch (FindBestMethod(type.GetConstructors(), args, out method))
                {
                    case 0:
                        if (args.Length == 1)
                            return GenerateConversion(args[0], type, errorPos);
                        throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type));
                    case 1:
                        return Expression.New((ConstructorInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type));
                }
            }
            ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected);
            NextToken();
            return ParseMemberAccess(type, null);
        }

        /// <summary>
        /// Generate conversion.
        /// </summary>
        /// <param name="expr">Expression.</param>
        /// <param name="type">The type.</param>
        /// <param name="errorPos">Error position.</param>
        /// <returns>The expression.</returns>
        Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            Type exprType = expr.Type;
            if (exprType == type) return expr;
            if (exprType.IsValueType && type.IsValueType)
            {
                if ((IsNullableType(exprType) || IsNullableType(type)) &&
                    GetNonNullableType(exprType) == GetNonNullableType(type))
                    return Expression.Convert(expr, type);
                if ((IsNumericType(exprType) || IsEnumType(exprType)) &&
                    (IsNumericType(type)) || IsEnumType(type))
                    return Expression.ConvertChecked(expr, type);
            }
            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) ||
                exprType.IsInterface || type.IsInterface)
                return Expression.Convert(expr, type);
            throw ParseError(errorPos, Res.CannotConvertValue,
                GetTypeName(exprType), GetTypeName(type));
        }

        /// <summary>
        /// Parse member access.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">Expression.</param>
        /// <returns>The expression.</returns>
        Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null) type = instance.Type;
            int errorPos = token.pos;
            string id = GetIdentifier();
            NextToken();
            if (token.id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    Type enumerableType = FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != null)
                    {
                        Type elementType = enumerableType.GetGenericArguments()[0];
                        return ParseAggregate(instance, elementType, id, errorPos);
                    }
                }
                Expression[] args = ParseArgumentList();
                MethodBase mb;
                switch (FindMethod(type, id, instance == null, args, out mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableMethod,
                            id, GetTypeName(type));
                    case 1:
                        MethodInfo method = (MethodInfo)mb;
                        if (!IsPredefinedType(method.DeclaringType))
                            throw ParseError(errorPos, Res.MethodsAreInaccessible, GetTypeName(method.DeclaringType));
                        if (method.ReturnType == typeof(void))
                            throw ParseError(errorPos, Res.MethodIsVoid,
                                id, GetTypeName(method.DeclaringType));
                        return Expression.Call(instance, (MethodInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousMethodInvocation,
                            id, GetTypeName(type));
                }
            }
            else
            {
                MemberInfo member = FindPropertyOrField(type, id, instance == null);
                if (member == null)
                    throw ParseError(errorPos, Res.UnknownPropertyOrField,
                        id, GetTypeName(type));
                return member is PropertyInfo ?
                    Expression.Property(instance, (PropertyInfo)member) :
                    Expression.Field(instance, (FieldInfo)member);
            }
        }

        /// <summary>
        /// Find generic type.
        /// </summary>
        /// <param name="generic">The generic type.</param>
        /// <param name="type">The type.</param>
        /// <returns>The type.</returns>
        static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic) return type;
                if (generic.IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = FindGenericType(generic, intfType);
                        if (found != null) return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Parse aggregate.
        /// </summary>
        /// <param name="instance">Expression.</param>
        /// <param name="elementType">The element ype.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="errorPos">Error position.</param>
        /// <returns>The expression.</returns>
        Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            ParameterExpression outerIt = it;
            ParameterExpression innerIt = Expression.Parameter(elementType, "");
            it = innerIt;
            Expression[] args = ParseArgumentList();
            it = outerIt;
            MethodBase signature;
            if (FindMethod(typeof(IEnumerableSignatures), methodName, false, args, out signature) != 1)
                throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);
            Type[] typeArgs;
            if (signature.Name == "Min" || signature.Name == "Max")
            {
                typeArgs = new Type[] { elementType, args[0].Type };
            }
            else
            {
                typeArgs = new Type[] { elementType };
            }
            if (args.Length == 0)
            {
                args = new Expression[] { instance };
            }
            else
            {
                args = new Expression[] { instance, Expression.Lambda(args[0], innerIt) };
            }
            return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args);
        }

        /// <summary>
        /// Parse argument list.
        /// </summary>
        /// <returns>The expressions.</returns>
        Expression[] ParseArgumentList()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            Expression[] args = token.id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            return args;
        }

        /// <summary>
        /// Parse arguments.
        /// </summary>
        /// <returns>The expressions.</returns>
        Expression[] ParseArguments()
        {
            List<Expression> argList = new List<Expression>();
            while (true)
            {
                argList.Add(ParseExpression());
                if (token.id != TokenId.Comma) break;
                NextToken();
            }
            return argList.ToArray();
        }

        /// <summary>
        /// Parse element access.
        /// </summary>
        /// <param name="expr">Expression.</param>
        /// <returns>The expression.</returns>
        Expression ParseElementAccess(Expression expr)
        {
            int errorPos = token.pos;
            ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            NextToken();
            Expression[] args = ParseArguments();
            ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            NextToken();
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                Expression index = PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                    throw ParseError(errorPos, Res.InvalidIndex);
                return Expression.ArrayIndex(expr, index);
            }
            else
            {
                MethodBase mb;
                switch (FindIndexer(expr.Type, args, out mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableIndexer,
                            GetTypeName(expr.Type));
                    case 1:
                        return Expression.Call(expr, (MethodInfo)mb, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousIndexerInvocation,
                            GetTypeName(expr.Type));
                }
            }
        }

        /// <summary>
        /// Is predefined type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if predfined type; else false.</returns>
        static bool IsPredefinedType(Type type)
        {
            foreach (Type t in predefinedTypes) if (t == type) return true;
            return false;
        }

        /// <summary>
        /// Is nullable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if nullable type; else false.</returns>
        static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Get non nullable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type.</returns>
        static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Get type name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type name.</returns>
        static string GetTypeName(Type type)
        {
            Type baseType = GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

        /// <summary>
        /// Is numeric type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if numeric type; else false.</returns>
        static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        /// <summary>
        /// Is signed integral type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if signed integral type; else false.</returns>
        static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        /// <summary>
        /// Is unsigned integral type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if unsigned integral type; else false.</returns>
        static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        /// <summary>
        /// Get numeric type kind.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The numeric kind value.</returns>
        static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Is enum type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if enum type; else false.</returns>
        static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        /// <summary>
        /// Check and promote operand.
        /// </summary>
        /// <param name="signatures">The signatures type.</param>
        /// <param name="opName">OP name.</param>
        /// <param name="expr">Expression.</param>
        /// <param name="errorPos">Error position.</param>
        void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            Expression[] args = new Expression[] { expr };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
                throw ParseError(errorPos, Res.IncompatibleOperand,
                    opName, GetTypeName(args[0].Type));
            expr = args[0];
        }

        /// <summary>
        /// Check and promote operands.
        /// </summary>
        /// <param name="signatures">The signatures type.</param>
        /// <param name="opName">OP name.</param>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <param name="errorPos">Error position.</param>
        void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            Expression[] args = new Expression[] { left, right };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            left = args[0];
            right = args[1];
        }

        /// <summary>
        /// Incompatible operands error.
        /// </summary>
        /// <param name="opName">OP name.</param>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <param name="pos">The position.</param>
        /// <returns>The exception.</returns>
        Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
        {
            return ParseError(pos, Res.IncompatibleOperands,
                opName, GetTypeName(left.Type), GetTypeName(right.Type));
        }

        /// <summary>
        /// Find property or field.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="memberName">Member name.</param>
        /// <param name="staticAccess">Static access.</param>
        /// <returns>The member info.</returns>
        MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field,
                    flags, Type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0) return members[0];
            }
            return null;
        }

        /// <summary>
        /// Find method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="staticAccess">Static access.</param>
        /// <param name="args">Expression arguments.</param>
        /// <param name="method">Method base.</param>
        /// <returns>The method index.</returns>
        int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Method,
                    flags, Type.FilterNameIgnoreCase, methodName);
                int count = FindBestMethod(members.Cast<MethodBase>(), args, out method);
                if (count != 0) return count;
            }
            method = null;
            return 0;
        }

        /// <summary>
        /// Find indexer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">Expression arguments.</param>
        /// <param name="method">Method base.</param>
        /// <returns>The indexer.</returns>
        int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.GetDefaultMembers();
                if (members.Length != 0)
                {
                    IEnumerable<MethodBase> methods = members.
                        OfType<PropertyInfo>().
                        Select(p => (MethodBase)p.GetGetMethod()).
                        Where(m => m != null);
                    int count = FindBestMethod(methods, args, out method);
                    if (count != 0) return count;
                }
            }
            method = null;
            return 0;
        }

        /// <summary>
        /// Self and base types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Array of types.</returns>
        static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        /// <summary>
        /// Self and base classes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Array of types.</returns>
        static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        /// <summary>
        /// Add interface.
        /// </summary>
        /// <param name="types">The array of types.</param>
        /// <param name="type">The type.</param>
        static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type t in type.GetInterfaces()) AddInterface(types, t);
            }
        }

        /// <summary>
        /// Method data.
        /// </summary>
        class MethodData
        {
            /// <summary>
            /// Mathod base.
            /// </summary>
            public MethodBase MethodBase;

            /// <summary>
            /// Parameters.
            /// </summary>
            public ParameterInfo[] Parameters;

            /// <summary>
            /// Expression arguments.
            /// </summary>
            public Expression[] Args;
        }

        /// <summary>
        /// Find best method.
        /// </summary>
        /// <param name="methods">Methods.</param>
        /// <param name="args">Expression arguments.</param>
        /// <param name="method">Method base.</param>
        /// <returns>The method index.</returns>
        int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            MethodData[] applicable = methods.
                Select(m => new MethodData { MethodBase = m, Parameters = m.GetParameters() }).
                Where(m => IsApplicable(m, args)).
                ToArray();
            if (applicable.Length > 1)
            {
                applicable = applicable.
                    Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).
                    ToArray();
            }
            if (applicable.Length == 1)
            {
                MethodData md = applicable[0];
                for (int i = 0; i < args.Length; i++) args[i] = md.Args[i];
                method = md.MethodBase;
            }
            else
            {
                method = null;
            }
            return applicable.Length;
        }

        /// <summary>
        /// Is applicable.
        /// </summary>
        /// <param name="method">Method data.</param>
        /// <param name="args">Expression arguments.</param>
        /// <returns>True if applicable; else false.</returns>
        bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length) return false;
            Expression[] promotedArgs = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo pi = method.Parameters[i];
                if (pi.IsOut) return false;
                Expression promoted = PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null) return false;
                promotedArgs[i] = promoted;
            }
            method.Args = promotedArgs;
            return true;
        }

        /// <summary>
        /// Promote expression.
        /// </summary>
        /// <param name="expr">Expression</param>
        /// <param name="type">The type.</param>
        /// <param name="exact">Exact.</param>
        /// <returns>The expression.</returns>
        Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type == type) return expr;
            if (expr is ConstantExpression)
            {
                ConstantExpression ce = (ConstantExpression)expr;
                if (ce == nullLiteral)
                {
                    if (!type.IsValueType || IsNullableType(type))
                        return Expression.Constant(null, type);
                }
                else
                {
                    string text;
                    if (literals.TryGetValue(ce, out text))
                    {
                        Type target = GetNonNullableType(type);
                        Object value = null;
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);
                                break;
                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = ParseNumber(text, target);
                                break;
                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }
                        if (value != null)
                            return Expression.Constant(value, type);
                    }
                }
            }
            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact) return Expression.Convert(expr, type);
                return expr;
            }
            return null;
        }

        /// <summary>
        /// Parse number.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <returns>The object value.</returns>
        static object ParseNumber(string text, Type type)
        {
            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb)) return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s)) return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us)) return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui)) return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul)) return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f)) return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d)) return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e)) return e;
                    break;
            }
            return null;
        }

        /// <summary>
        /// Parse enum.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>The object value.</returns>
        static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0) return ((FieldInfo)memberInfos[0]).GetValue(null);
            }
            return null;
        }

        /// <summary>
        /// Is compatible with.
        /// </summary>
        /// <param name="source">The source type.</param>
        /// <param name="target">The target type.</param>
        /// <returns>True if compatible with; else false.</returns>
        static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Is better than.
        /// </summary>
        /// <param name="args">Expression arguments.</param>
        /// <param name="m1">Method data.</param>
        /// <param name="m2">Method data.</param>
        /// <returns>True if is better than; else false.</returns>
        static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            bool better = false;
            for (int i = 0; i < args.Length; i++)
            {
                int c = CompareConversions(args[i].Type,
                    m1.Parameters[i].ParameterType,
                    m2.Parameters[i].ParameterType);
                if (c < 0) return false;
                if (c > 0) better = true;
            }
            return better;
        }

        /// <summary>
        /// Return 1 if s -> t1 is a better conversion than s -> t2
        /// Return -1 if s -> t2 is a better conversion than s -> t1
        /// Return 0 if neither conversion is better
        /// </summary>
        /// <param name="s">The s type.</param>
        /// <param name="t1">The type.</param>
        /// <param name="t2">The type.</param>
        /// <returns>The conversion index.</returns>
        static int CompareConversions(Type s, Type t1, Type t2)
        {
            if (t1 == t2) return 0;
            if (s == t1) return 1;
            if (s == t2) return -1;
            bool t1t2 = IsCompatibleWith(t1, t2);
            bool t2t1 = IsCompatibleWith(t2, t1);
            if (t1t2 && !t2t1) return 1;
            if (t2t1 && !t1t2) return -1;
            if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2)) return 1;
            if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1)) return -1;
            return 0;
        }

        /// <summary>
        /// Generate equal.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateEqual(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        /// <summary>
        /// Generate not equal.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateNotEqual(Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        /// <summary>
        /// Generate greater than.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.GreaterThan(left, right);
        }

        /// <summary>
        /// Generate greater than equal.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.GreaterThanOrEqual(left, right);
        }

        /// <summary>
        /// Generate less than.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.LessThan(left, right);
        }

        /// <summary>
        /// Generate less than equal.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.LessThanOrEqual(left, right);
        }

        /// <summary>
        /// Generate add.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateAdd(Expression left, Expression right)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }

        /// <summary>
        /// Generate subtract.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        /// <summary>
        /// Generate string concat.
        /// </summary>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateStringConcat(Expression left, Expression right)
        {
            return Expression.Call(
                null,
                typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }),
                new[] { left, right });
        }

        /// <summary>
        /// Get static method.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        /// <summary>
        /// Generate static method call.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        /// <param name="left">Expression</param>
        /// <param name="right">Expression</param>
        /// <returns>The expression.</returns>
        Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        /// <summary>
        /// Set text position.
        /// </summary>
        /// <param name="pos">The position.</param>
        void SetTextPos(int pos)
        {
            textPos = pos;
            ch = textPos < textLen ? text[textPos] : '\0';
        }

        /// <summary>
        /// Next char.
        /// </summary>
        void NextChar()
        {
            if (textPos < textLen) textPos++;
            ch = textPos < textLen ? text[textPos] : '\0';
        }

        /// <summary>
        /// Next token.
        /// </summary>
        void NextToken()
        {
            while (Char.IsWhiteSpace(ch)) NextChar();
            TokenId t;
            int tokenPos = textPos;
            switch (ch)
            {
                case '!':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (ch == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (ch == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    t = TokenId.Question;
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (ch == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    char quote = ch;
                    do
                    {
                        NextChar();
                        while (textPos < textLen && ch != quote) NextChar();
                        if (textPos == textLen)
                            throw ParseError(textPos, Res.UnterminatedStringLiteral);
                        NextChar();
                    } while (ch == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (Char.IsLetter(ch) || ch == '@' || ch == '_')
                    {
                        do
                        {
                            NextChar();
                        } while (Char.IsLetterOrDigit(ch) || ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }
                    if (Char.IsDigit(ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (Char.IsDigit(ch));
                        if (ch == '.')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(ch));
                        }
                        if (ch == 'E' || ch == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (ch == '+' || ch == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(ch));
                        }
                        if (ch == 'F' || ch == 'f') NextChar();
                        break;
                    }
                    if (textPos == textLen)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(textPos, Res.InvalidCharacter, ch);
            }
            token.id = t;
            token.text = text.Substring(tokenPos, textPos - tokenPos);
            token.pos = tokenPos;
        }

        /// <summary>
        /// Token identifier is.
        /// </summary>
        /// <param name="id">The identifier id.</param>
        /// <returns>True if token identifier is; else false.</returns>
        bool TokenIdentifierIs(string id)
        {
            return token.id == TokenId.Identifier && String.Equals(id, token.text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        string GetIdentifier()
        {
            ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            string id = token.text;
            if (id.Length > 1 && id[0] == '@') id = id.Substring(1);
            return id;
        }

        /// <summary>
        /// Validate digit.
        /// </summary>
        void ValidateDigit()
        {
            if (!Char.IsDigit(ch)) throw ParseError(textPos, Res.DigitExpected);
        }

        /// <summary>
        /// Validate token.
        /// </summary>
        /// <param name="t">The token id.</param>
        /// <param name="errorMessage">Error message.</param>
        void ValidateToken(TokenId t, string errorMessage)
        {
            if (token.id != t) throw ParseError(errorMessage);
        }

        /// <summary>
        /// Validate token.
        /// </summary>
        /// <param name="t">The token id.</param>
        void ValidateToken(TokenId t)
        {
            if (token.id != t) throw ParseError(Res.SyntaxError);
        }

        /// <summary>
        /// Parse error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Exception</returns>
        Exception ParseError(string format, params object[] args)
        {
            return ParseError(token.pos, format, args);
        }

        /// <summary>
        /// Parse error.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Exception</returns>
        Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args), pos);
        }

        /// <summary>
        /// Create keywords.
        /// </summary>
        /// <returns>The list of keywords.</returns>
        static Dictionary<string, object> CreateKeywords()
        {
            Dictionary<string, object> d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            d.Add("true", trueLiteral);
            d.Add("false", falseLiteral);
            d.Add("null", nullLiteral);
            d.Add(keywordIt, keywordIt);
            d.Add(keywordIif, keywordIif);
            d.Add(keywordNew, keywordNew);
            foreach (Type type in predefinedTypes) d.Add(type.Name, type);
            return d;
        }
    }

    /// <summary>
    /// REsource string.
    /// </summary>
    internal static class Res
    {
        /// <summary>
        /// DuplicateIdentifier
        /// </summary>
        public const string DuplicateIdentifier = "The identifier '{0}' was defined more than once";
        /// <summary>
        /// ExpressionTypeMismatch
        /// </summary>
        public const string ExpressionTypeMismatch = "Expression of type '{0}' expected";
        /// <summary>
        /// ExpressionExpected
        /// </summary>
        public const string ExpressionExpected = "Expression expected";
        /// <summary>
        /// InvalidCharacterLiteral
        /// </summary>
        public const string InvalidCharacterLiteral = "Character literal must contain exactly one character";
        /// <summary>
        /// InvalidIntegerLiteral
        /// </summary>
        public const string InvalidIntegerLiteral = "Invalid integer literal '{0}'";
        /// <summary>
        /// InvalidRealLiteral
        /// </summary>
        public const string InvalidRealLiteral = "Invalid real literal '{0}'";
        /// <summary>
        /// UnknownIdentifier
        /// </summary>
        public const string UnknownIdentifier = "Unknown identifier '{0}'";
        /// <summary>
        /// NoItInScope
        /// </summary>
        public const string NoItInScope = "No 'it' is in scope";
        /// <summary>
        /// IifRequiresThreeArgs
        /// </summary>
        public const string IifRequiresThreeArgs = "The 'iif' function requires three arguments";
        /// <summary>
        /// FirstExprMustBeBool
        /// </summary>
        public const string FirstExprMustBeBool = "The first expression must be of type 'Boolean'";
        /// <summary>
        /// BothTypesConvertToOther
        /// </summary>
        public const string BothTypesConvertToOther = "Both of the types '{0}' and '{1}' convert to the other";
        /// <summary>
        /// NeitherTypeConvertsToOther
        /// </summary>
        public const string NeitherTypeConvertsToOther = "Neither of the types '{0}' and '{1}' converts to the other";
        /// <summary>
        /// MissingAsClause
        /// </summary>
        public const string MissingAsClause = "Expression is missing an 'as' clause";
        /// <summary>
        /// ArgsIncompatibleWithLambda
        /// </summary>
        public const string ArgsIncompatibleWithLambda = "Argument list incompatible with lambda expression";
        /// <summary>
        /// TypeHasNoNullableForm
        /// </summary>
        public const string TypeHasNoNullableForm = "Type '{0}' has no nullable form";
        /// <summary>
        /// NoMatchingConstructor
        /// </summary>
        public const string NoMatchingConstructor = "No matching constructor in type '{0}'";
        /// <summary>
        /// AmbiguousConstructorInvocation
        /// </summary>
        public const string AmbiguousConstructorInvocation = "Ambiguous invocation of '{0}' constructor";
        /// <summary>
        /// CannotConvertValue
        /// </summary>
        public const string CannotConvertValue = "A value of type '{0}' cannot be converted to type '{1}'";
        /// <summary>
        /// NoApplicableMethod
        /// </summary>
        public const string NoApplicableMethod = "No applicable method '{0}' exists in type '{1}'";
        /// <summary>
        /// MethodsAreInaccessible
        /// </summary>
        public const string MethodsAreInaccessible = "Methods on type '{0}' are not accessible";
        /// <summary>
        /// MethodIsVoid
        /// </summary>
        public const string MethodIsVoid = "Method '{0}' in type '{1}' does not return a value";
        /// <summary>
        /// AmbiguousMethodInvocation
        /// </summary>
        public const string AmbiguousMethodInvocation = "Ambiguous invocation of method '{0}' in type '{1}'";
        /// <summary>
        /// UnknownPropertyOrField
        /// </summary>
        public const string UnknownPropertyOrField = "No property or field '{0}' exists in type '{1}'";
        /// <summary>
        /// NoApplicableAggregate
        /// </summary>
        public const string NoApplicableAggregate = "No applicable aggregate method '{0}' exists";
        /// <summary>
        /// CannotIndexMultiDimArray
        /// </summary>
        public const string CannotIndexMultiDimArray = "Indexing of multi-dimensional arrays is not supported";
        /// <summary>
        /// InvalidIndex
        /// </summary>
        public const string InvalidIndex = "Array index must be an integer expression";
        /// <summary>
        /// NoApplicableIndexer
        /// </summary>
        public const string NoApplicableIndexer = "No applicable indexer exists in type '{0}'";
        /// <summary>
        /// AmbiguousIndexerInvocation
        /// </summary>
        public const string AmbiguousIndexerInvocation = "Ambiguous invocation of indexer in type '{0}'";
        /// <summary>
        /// IncompatibleOperand
        /// </summary>
        public const string IncompatibleOperand = "Operator '{0}' incompatible with operand type '{1}'";
        /// <summary>
        /// IncompatibleOperands
        /// </summary>
        public const string IncompatibleOperands = "Operator '{0}' incompatible with operand types '{1}' and '{2}'";
        /// <summary>
        /// UnterminatedStringLiteral
        /// </summary>
        public const string UnterminatedStringLiteral = "Unterminated string literal";
        /// <summary>
        /// InvalidCharacter
        /// </summary>
        public const string InvalidCharacter = "Syntax error '{0}'";
        /// <summary>
        /// DigitExpected
        /// </summary>
        public const string DigitExpected = "Digit expected";
        /// <summary>
        /// SyntaxError
        /// </summary>
        public const string SyntaxError = "Syntax error";
        /// <summary>
        /// TokenExpected
        /// </summary>
        public const string TokenExpected = "{0} expected";
        /// <summary>
        /// ParseExceptionFormat
        /// </summary>
        public const string ParseExceptionFormat = "{0} (at index {1})";
        /// <summary>
        /// ColonExpected
        /// </summary>
        public const string ColonExpected = "':' expected";
        /// <summary>
        /// OpenParenExpected
        /// </summary>
        public const string OpenParenExpected = "'(' expected";
        /// <summary>
        /// CloseParenOrOperatorExpected
        /// </summary>
        public const string CloseParenOrOperatorExpected = "')' or operator expected";
        /// <summary>
        /// CloseParenOrCommaExpected
        /// </summary>
        public const string CloseParenOrCommaExpected = "')' or ',' expected";
        /// <summary>
        /// DotOrOpenParenExpected
        /// </summary>
        public const string DotOrOpenParenExpected = "'.' or '(' expected";
        /// <summary>
        /// OpenBracketExpected
        /// </summary>
        public const string OpenBracketExpected = "'[' expected";
        /// <summary>
        /// CloseBracketOrCommaExpected
        /// </summary>
        public const string CloseBracketOrCommaExpected = "']' or ',' expected";
        /// <summary>
        /// IdentifierExpected
        /// </summary>
        public const string IdentifierExpected = "Identifier expected";
    }
}
