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
using System.IO;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

namespace Nequeo.Data.Linq.Provider
{
    /// <summary>
    /// Writes out an expression tree in a C#-ish syntax
    /// </summary>
    public class ExpressionWriter : ExpressionVisitor
    {
        TextWriter writer;
        int indent = 2;
        int depth;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected ExpressionWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="expression"></param>
        public static void Write(TextWriter writer, Expression expression)
        {
            new ExpressionWriter(writer).Visit(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string WriteToString(Expression expression)
        {
            StringWriter sw = new StringWriter();
            Write(sw, expression);
            return sw.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        protected enum Indentation
        {
            /// <summary>
            /// 
            /// </summary>
            Same,
            /// <summary>
            /// 
            /// </summary>
            Inner,
            /// <summary>
            /// 
            /// </summary>
            Outer
        }

        /// <summary>
        /// 
        /// </summary>
        protected int IndentationWidth
        {
            get { return this.indent; }
            set { this.indent = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        protected void WriteLine(Indentation style)
        {
            this.writer.WriteLine();
            this.Indent(style);
            for (int i = 0, n = this.depth * this.indent; i < n; i++)
            {
                this.writer.Write(" ");
            }
        }

        private static readonly char[] splitters = new char[] { '\n', '\r' };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        protected void Write(string text)
        {
            if (text.IndexOf('\n') >= 0)
            {
                string[] lines = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, n = lines.Length; i < n; i++)
                {
                    this.Write(lines[i]);
                    if (i < n - 1)
                    {
                        this.WriteLine(Indentation.Same);
                    }
                }
            }
            else
            {
                this.writer.Write(text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        protected void Indent(Indentation style)
        {
            if (style == Indentation.Inner)
            {
                this.depth++;
            }
            else if (style == Indentation.Outer)
            {
                this.depth--;
                System.Diagnostics.Debug.Assert(this.depth >= 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Not:
                    return "!";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.RightShift:
                    return ">>";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.ExclusiveOr:
                    return "^";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    this.Visit(b.Left);
                    this.Write("[");
                    this.Visit(b.Right);
                    this.Write("]");
                    break;
                case ExpressionType.Power:
                    this.Write("POW(");
                    this.Visit(b.Left);
                    this.Write(", ");
                    this.Visit(b.Right);
                    this.Write(")");
                    break;
                default:
                    this.Visit(b.Left);
                    this.Write(" ");
                    this.Write(GetOperator(b.NodeType));
                    this.Write(" ");
                    this.Visit(b.Right);
                    break;
            }
            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    this.Write("((");
                    this.Write(this.GetTypeName(u.Type));
                    this.Write(")");
                    this.Visit(u.Operand);
                    this.Write(")");
                    break;
                case ExpressionType.ArrayLength:
                    this.Visit(u.Operand);
                    this.Write(".Length");
                    break;
                case ExpressionType.Quote:
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.TypeAs:
                    this.Visit(u.Operand);
                    this.Write(" as ");
                    this.Write(this.GetTypeName(u.Type));
                    break;
                case ExpressionType.UnaryPlus:
                    this.Visit(u.Operand);
                    break;
                default:
                    this.Write(this.GetOperator(u.NodeType));
                    this.Visit(u.Operand);
                    break;
            }
            return u;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetTypeName(Type type)
        {
            string name = type.Name;
            name = name.Replace('+', '.');
            int iGeneneric = name.IndexOf('`');
            if (iGeneneric > 0)
            {
                name = name.Substring(0, iGeneneric);
            }
            if (type.IsGenericType || type.IsGenericTypeDefinition)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(name);
                sb.Append("<");
                var args = type.GetGenericArguments();
                for (int i = 0, n = args.Length; i < n; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                    if (type.IsGenericType)
                    {
                        sb.Append(this.GetTypeName(args[i]));
                    }
                }
                sb.Append(">");
                name = sb.ToString();
            }
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConditional(ConditionalExpression c)
        {
            this.Visit(c.Test);
            this.WriteLine(Indentation.Inner);
            this.Write("? ");
            this.Visit(c.IfTrue);
            this.WriteLine(Indentation.Same);
            this.Write(": ");
            this.Visit(c.IfFalse);
            this.Indent(Indentation.Outer);
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected override IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.VisitBinding(original[i]);
                if (i < n - 1)
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
            }
            return original;
        }

        private static readonly char[] special = new char[] { '\n', '\n', '\\' };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null) 
            {
                this.Write("null");
            }
            else if (c.Type == typeof(string))
            {
                string value = c.Value.ToString();
                if (value.IndexOfAny(special) >= 0)
                    this.Write("@");
                this.Write("\"");
                this.Write(c.Value.ToString());
                this.Write("\"");
            }
            else if (c.Type == typeof(DateTime))
            {
                this.Write("new DataTime(\"");
                this.Write(c.Value.ToString());
                this.Write("\")");
            }
            else if (c.Type.IsArray)
            {
                Type elementType = c.Type.GetElementType();
                this.VisitNewArray(
                    Expression.NewArrayInit(
                        elementType,
                        ((IEnumerable)c.Value).OfType<object>().Select(v => (Expression)Expression.Constant(v, elementType))
                        ));
            }
            else
            {
                this.Write(c.Value.ToString());
            }
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        protected override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            if (initializer.Arguments.Count > 1)
            {
                this.Write("{");
                for (int i = 0, n = initializer.Arguments.Count; i < n; i++)
                {
                    this.Visit(initializer.Arguments[i]);
                    if (i < n - 1)
                    {
                        this.Write(", ");
                    }
                }
                this.Write("}");
            }
            else
            {
                this.Visit(initializer.Arguments[0]);
            }
            return initializer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected override IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.VisitElementInitializer(original[i]);
                if (i < n - 1)
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
            }
            return original;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.Visit(original[i]);
                if (i < n - 1)
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
            }
            return original;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            this.Write("Invoke(");
            this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(iv.Arguments);
            this.Write(", ");
            this.WriteLine(Indentation.Same);
            this.Visit(iv.Expression);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return iv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            if (lambda.Parameters.Count > 1)
            {
                this.Write("(");
                for (int i = 0, n = lambda.Parameters.Count; i < n; i++)
                {
                    this.Write(lambda.Parameters[i].Name);
                    if (i < n - 1)
                    {
                        this.Write(", ");
                    }
                }
                this.Write(")");
            }
            else
            {
                this.Write(lambda.Parameters[0].Name);
            }
            this.Write(" => ");
            this.Visit(lambda.Body);
            return lambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected override Expression VisitListInit(ListInitExpression init)
        {
            this.Visit(init.NewExpression);
            this.Write(" {");
            this.WriteLine(Indentation.Inner);
            this.VisitElementInitializerList(init.Initializers);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return init;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            this.Visit(m.Expression);
            this.Write(".");
            this.Write(m.Member.Name);
            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            this.Write(assignment.Member.Name);
            this.Write(" = ");
            this.Visit(assignment.Expression);
            return assignment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            this.Visit(init.NewExpression);
            this.Write(" {");
            this.WriteLine(Indentation.Inner);
            this.VisitBindingList(init.Bindings);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return init;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            this.Write(binding.Member.Name);
            this.Write(" = {");
            this.WriteLine(Indentation.Inner);
            this.VisitElementInitializerList(binding.Initializers);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return binding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            this.Write(binding.Member.Name);
            this.Write(" = {");
            this.WriteLine(Indentation.Inner);
            this.VisitBindingList(binding.Bindings);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return binding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object != null)
            {
                this.Visit(m.Object);
            }
            else
            {
                this.Write(this.GetTypeName(m.Method.DeclaringType));
            }
            this.Write(".");
            this.Write(m.Method.Name);
            this.Write("(");
            if (m.Arguments.Count > 1)
                this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(m.Arguments);
            if (m.Arguments.Count > 1)
                this.WriteLine(Indentation.Outer);
            this.Write(")");
            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nex"></param>
        /// <returns></returns>
        protected override NewExpression VisitNew(NewExpression nex)
        {
            this.Write("new ");
            this.Write(this.GetTypeName(nex.Constructor.DeclaringType));
            this.Write("(");
            if (nex.Arguments.Count > 1)
                this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(nex.Arguments);
            if (nex.Arguments.Count > 1)
                this.WriteLine(Indentation.Outer);
            this.Write(")");
            return nex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="na"></param>
        /// <returns></returns>
        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            this.Write("new ");
            this.Write(this.GetTypeName(TypeHelper.GetElementType(na.Type)));
            this.Write("[] {");
            if (na.Expressions.Count > 1)
                this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(na.Expressions);
            if (na.Expressions.Count > 1)
                this.WriteLine(Indentation.Outer);
            this.Write("}");
            return na;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            this.Write(p.Name);
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            this.Visit(b.Expression);
            this.Write(" is ");
            this.Write(this.GetTypeName(b.TypeOperand));
            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitUnknown(Expression expression)
        {
            this.Write(expression.ToString());
            return expression;
        }
    }
}