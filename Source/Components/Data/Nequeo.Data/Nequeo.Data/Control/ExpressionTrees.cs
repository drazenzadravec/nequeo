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
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Data.Common;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq.Expressions;

using LinqTypes = Nequeo.Data.DataType.ProviderToDataTypes;

using Nequeo.Model;
using Nequeo.Data.DataType;
using Nequeo.Linq;

namespace Nequeo.Data.Control
{
    /// <summary>
    /// Data generic base expression tree analyser.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public sealed class ExpressionTreeDataGeneric<TDataEntity> : Nequeo.Linq.ExpressionVisitor
        where TDataEntity : class, new()
    {
        #region ExpressionTreeDataEntity Class

        #region Private Fields
        private String _sqlStatement = string.Empty;
        private String _sqlMethodStatement = string.Empty;
        private ParameterExpression _param = null;
        private Nequeo.Data.DataType.DataTypeConversion _dataTypeConversion = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlWhereQuery(Expression expression, Nequeo.Data.DataType.DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            LambdaExpression body = (LambdaExpression)base.Visit(expression);
            _param = (ParameterExpression)body.Parameters[0];

            // Start the expression builder.
            StartCreation(body);
            return _sqlStatement;
        }
        #endregion

        #region Expression Tree Analysis Methods
        /// <summary>
        /// Start if the recursive expression analyser.
        /// </summary>
        /// <param name="body">The lambda expression body.</param>
        private void StartCreation(LambdaExpression body)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnNodeType(body.Body))
            {
                // Get the current binary expression.
                BinaryExpression exp = (BinaryExpression)base.Visit(body.Body);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                _sqlStatement += "(";
                LeftExpression(exp);
                _sqlStatement += ValueNodeType(exp);
                RightExpression(exp);
                _sqlStatement += ")";
            }
            else
            {
                switch (body.Body.NodeType)
                {
                    case ExpressionType.Call:
                        // Get the method expression from the expression tree
                        // create the sql statement.
                        MethodCallExpression call = (MethodCallExpression)base.Visit(body.Body);
                        string methodName = call.Method.Name;
                        SqlMethod(call, methodName, null);
                        break;

                    default:
                        // Get the current binary expression.
                        // get the member expression.
                        BinaryExpression exp = (BinaryExpression)base.Visit(body.Body);
                        switch (exp.Left.NodeType)
                        {
                            case ExpressionType.Call:
                                MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
                                string methodNameCall = leftCall.Method.Name;
                                SqlMethod(leftCall, methodNameCall, exp);
                                break;
                            default:
                                MemberExpression left = (MemberExpression)base.Visit(exp.Left);

                                // If the right expression is a convert type.
                                if (exp.Right.NodeType == ExpressionType.Convert)
                                {
                                    // Get the unary expression and
                                    // create the current branch sql statement.
                                    UnaryExpression right = (UnaryExpression)base.Visit(exp.Right);
                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnUnaryOperand(right) + ")";
                                }
                                else if (exp.Right.NodeType == ExpressionType.MemberAccess)
                                {
                                    // Get the member type argument.
                                    MemberExpression expValue = (MemberExpression)base.Visit(exp.Right);
                                    string memberNameValue = expValue.Member.Name;

                                    // Get the constant expression and
                                    // create the current branch sql statement.
                                    ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                                    // Get the current member argument field and
                                    // get the value for the current argument field.
                                    FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                    object objectValue = fieldValue.GetValue(constValue.Value);

                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnConstantValue(objectValue) + ")";
                                }
                                else
                                {
                                    // Get the constant expression and
                                    // create the current branch sql statement.
                                    ConstantExpression right = (ConstantExpression)base.Visit(exp.Right);
                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnConstantValue(right) + ")";
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Left side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void LeftExpression(BinaryExpression expression)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnNodeType(expression.Left))
            {
                BinaryExpression exp = (BinaryExpression)base.Visit(expression.Left);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                _sqlStatement += "(";
                LeftExpression(exp);
                _sqlStatement += ValueNodeType(exp);
                RightExpression(exp);
                _sqlStatement += ")";
            }
            else
            {
                switch (expression.Left.NodeType)
                {
                    case ExpressionType.Call:
                        // Get the method expression from the expression tree
                        // create the sql statement.
                        MethodCallExpression call = (MethodCallExpression)base.Visit(expression.Left);
                        string methodName = call.Method.Name;
                        SqlMethod(call, methodName, null);
                        break;

                    default:
                        // Get the current binary expression.
                        // get the member expression.
                        BinaryExpression exp = (BinaryExpression)base.Visit(expression.Left);
                        switch (exp.Left.NodeType)
                        {
                            case ExpressionType.Call:
                                MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
                                string methodNameCall = leftCall.Method.Name;
                                SqlMethod(leftCall, methodNameCall, exp);
                                break;
                            default:
                                MemberExpression left = (MemberExpression)base.Visit(exp.Left);

                                // If the right expression is a convert type.
                                if (exp.Right.NodeType == ExpressionType.Convert)
                                {
                                    // Get the unary expression and
                                    // create the current branch sql statement.
                                    UnaryExpression right = (UnaryExpression)base.Visit(exp.Right);
                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnUnaryOperand(right) + ")";
                                }
                                else if (exp.Right.NodeType == ExpressionType.MemberAccess)
                                {
                                    // Get the member type argument.
                                    MemberExpression expValue = (MemberExpression)base.Visit(exp.Right);
                                    string memberNameValue = expValue.Member.Name;

                                    // Get the constant expression and
                                    // create the current branch sql statement.
                                    ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                                    // Get the current member argument field and
                                    // get the value for the current argument field.
                                    FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                    object objectValue = fieldValue.GetValue(constValue.Value);

                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnConstantValue(objectValue) + ")";
                                }
                                else
                                {
                                    // Get the constant expression and
                                    // create the current branch sql statement.
                                    ConstantExpression right = (ConstantExpression)base.Visit(exp.Right);
                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnConstantValue(right) + ")";
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Right side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void RightExpression(BinaryExpression expression)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnNodeType(expression.Right))
            {
                BinaryExpression exp = (BinaryExpression)base.Visit(expression.Right);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                _sqlStatement += "(";
                LeftExpression(exp);
                _sqlStatement += ValueNodeType(exp);
                RightExpression(exp);
                _sqlStatement += ")";
            }
            else
            {
                switch (expression.Right.NodeType)
                {
                    case ExpressionType.Call:
                        // Get the method expression from the expression tree
                        // create the sql statement.
                        MethodCallExpression call = (MethodCallExpression)base.Visit(expression.Right);
                        string methodName = call.Method.Name;
                        SqlMethod(call, methodName, null);
                        break;

                    default:
                        // Get the current binary expression.
                        // get the member expression.
                        BinaryExpression exp = (BinaryExpression)base.Visit(expression.Right);
                        switch (exp.Left.NodeType)
                        {
                            case ExpressionType.Call:
                                MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
                                string methodNameCall = leftCall.Method.Name;
                                SqlMethod(leftCall, methodNameCall, exp);
                                break;
                            default:
                                MemberExpression left = (MemberExpression)base.Visit(exp.Left);

                                // If the right expression is a convert type.
                                if (exp.Right.NodeType == ExpressionType.Convert)
                                {
                                    // Get the unary expression and
                                    // create the current branch sql statement.
                                    UnaryExpression right = (UnaryExpression)base.Visit(exp.Right);
                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnUnaryOperand(right) + ")";
                                }
                                else if (exp.Right.NodeType == ExpressionType.MemberAccess)
                                {
                                    // Get the member type argument.
                                    MemberExpression expValue = (MemberExpression)base.Visit(exp.Right);
                                    string memberNameValue = expValue.Member.Name;

                                    // Get the constant expression and
                                    // create the current branch sql statement.
                                    ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                                    // Get the current member argument field and
                                    // get the value for the current argument field.
                                    FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                    object objectValue = fieldValue.GetValue(constValue.Value);

                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnConstantValue(objectValue) + ")";
                                }
                                else
                                {
                                    // Get the constant expression and
                                    // create the current branch sql statement.
                                    ConstantExpression right = (ConstantExpression)base.Visit(exp.Right);
                                    _sqlStatement += "(" + DatabaseColumnName(left) +
                                        ValueNodeType(exp) + DatabaseColumnConstantValue(right) + ")";
                                }
                                break;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Sql Method Expression Tree Analysis Methods
        /// <summary>
        /// Select the appropriate sql method call.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        /// <param name="sqlMethodName">The current sql method.</param>
        /// <param name="exp">The current sql method.</param>
        private void SqlMethod(MethodCallExpression call, string sqlMethodName, Expression exp)
        {
            switch (sqlMethodName.ToLower())
            {
                case "like":
                    SqlLikeMethod(call);
                    break;

                case "notlike":
                    SqlNotLikeMethod(call);
                    break;

                case "likeescape":
                    SqlLikeEscapeMethod(call);
                    break;

                case "notlikeescape":
                    SqlNotLikeEscapeMethod(call);
                    break;

                case "between":
                    SqlBetweenMethod(call);
                    break;

                case "notbetween":
                    SqlNotBetweenMethod(call);
                    break;

                case "isnull":
                    SqlIsNullMethod(call);
                    break;

                case "isnotnull":
                    SqlIsNotNullMethod(call);
                    break;

                case "contains":
                    SqlContainsMethod(call);
                    break;

                case "in":
                    SqlInMethod(call);
                    break;

                case "notin":
                    SqlNotInMethod(call);
                    break;

                case "comparestring":
                    SqlCompareStringMethod(call, exp);
                    break;

                default:
                    DefaultMethod(call, exp);
                    break;
            }
        }

        /// <summary>
        /// Default method call.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        /// <param name="exp">The current sql method.</param>
        private void DefaultMethod(MethodCallExpression call, Expression exp)
        {
            throw new Exception("Sql method is not valid.");
        }

        /// <summary>
        /// Create the 'Like' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlLikeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " LIKE '" +
                _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Create the 'Like' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotLikeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT LIKE '" +
                _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Create the 'Like with escape clause' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlLikeEscapeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            string sqlMethod = _sqlMethodStatement;
            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[2].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[2]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " LIKE '" +
                sqlMethod + "' ESCAPE '" + _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Create the 'Not Like with escape clause' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotLikeEscapeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            string sqlMethod = _sqlMethodStatement;
            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[2].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[2]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT LIKE '" +
                sqlMethod + "' ESCAPE '" + _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Left side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void LeftMethodExpression(BinaryExpression expression)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnSqlMethodNodeType(expression.Left))
            {
                BinaryExpression exp = (BinaryExpression)base.Visit(expression.Left);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                LeftMethodExpression(exp);
                RightMethodExpression(exp);
            }
            else
            {
                if (expression.Left.NodeType == ExpressionType.Constant)
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression left = (ConstantExpression)base.Visit(expression.Left);
                    _sqlMethodStatement += left.Value.ToString();
                }
                else
                {
                    // Get the current binary expression.
                    // get the member expression.
                    MemberExpression member = (MemberExpression)base.Visit(expression.Left);
                    string memberName = member.Member.Name;

                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(member.Expression);

                    // Get the current member argument field and
                    // get the value for the current argument field.
                    FieldInfo field = right.Value.GetType().GetField(memberName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    object memberValue = field.GetValue(right.Value);

                    // Create the current sql method statement.
                    _sqlMethodStatement += memberValue.ToString();
                }
            }
        }

        /// <summary>
        /// Right side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void RightMethodExpression(BinaryExpression expression)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnSqlMethodNodeType(expression.Right))
            {
                BinaryExpression exp = (BinaryExpression)base.Visit(expression.Right);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                LeftMethodExpression(exp);
                RightMethodExpression(exp);
            }
            else
            {
                if (expression.Right.NodeType == ExpressionType.Constant)
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(expression.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
                else
                {
                    // Get the current binary expression.
                    // get the member expression.
                    MemberExpression member = (MemberExpression)base.Visit(expression.Right);
                    string memberName = member.Member.Name;

                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(member.Expression);

                    // Get the current member argument field and
                    // get the value for the current argument field.
                    FieldInfo field = right.Value.GetType().GetField(memberName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    object memberValue = field.GetValue(right.Value);

                    // Create the current sql method statement.
                    _sqlMethodStatement += memberValue.ToString();
                }
            }
        }

        /// <summary>
        /// Create the 'Between' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlBetweenMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;
            object objectFrom = null;
            object objectTo = null;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                objectFrom = constant.Value;
            }
            else
            {
                // Get the member type argument.
                MemberExpression expFrom = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameFrom = expFrom.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constFrom = (ConstantExpression)base.Visit(expFrom.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldFrom = constFrom.Value.GetType().GetField(memberNameFrom,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectFrom = fieldFrom.GetValue(constFrom.Value);
            }

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                objectTo = constant.Value;
            }
            else
            {
                MemberExpression expTo = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameTo = expTo.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constTo = (ConstantExpression)base.Visit(expTo.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldTo = constTo.Value.GetType().GetField(memberNameTo,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectTo = fieldTo.GetValue(constTo.Value);
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " BETWEEN " +
                DatabaseColumnConstantValue(objectFrom) + " AND " + DatabaseColumnConstantValue(objectTo) + ")";
        }

        /// <summary>
        /// Create the 'Between' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotBetweenMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;
            object objectFrom = null;
            object objectTo = null;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                objectFrom = constant.Value.ToString();
            }
            else
            {
                // Get the member type argument.
                MemberExpression expFrom = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameFrom = expFrom.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constFrom = (ConstantExpression)base.Visit(expFrom.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldFrom = constFrom.Value.GetType().GetField(memberNameFrom,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectFrom = fieldFrom.GetValue(constFrom.Value);
            }

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                objectTo = constant.Value.ToString();
            }
            else
            {
                MemberExpression expTo = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameTo = expTo.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constTo = (ConstantExpression)base.Visit(expTo.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldTo = constTo.Value.GetType().GetField(memberNameTo,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectTo = fieldTo.GetValue(constTo.Value);
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT BETWEEN " +
                DatabaseColumnConstantValue(objectFrom) + " AND " + DatabaseColumnConstantValue(objectTo) + ")";
        }

        /// <summary>
        /// Create the 'Is Null' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlIsNullMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " IS NULL " + ")";
        }

        /// <summary>
        /// Create the 'Is Not Null' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlIsNotNullMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " IS NOT NULL " + ")";
        }

        /// <summary>
        /// Create the 'Contains' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlContainsMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Object);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[0].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[0]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[0].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[0]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[0]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(CONTAINS(" + DatabaseColumnName(memberColumn) + ", '" +
                _sqlMethodStatement + "'))";
        }

        /// <summary>
        /// Create the 'In' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlInMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " IN ('" +
                _sqlMethodStatement + "'))";
        }

        /// <summary>
        /// Create the 'Not In' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotInMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT IN ('" +
                _sqlMethodStatement + "'))";
        }

        /// <summary>
        /// Create the 'Compare String' method sql statement.
        /// </summary>
        /// <param name="callMethod">The current method call expression.</param>
        /// <param name="exp">The current method call expression.</param>
        /// <remarks></remarks>
        private void SqlCompareStringMethod(MethodCallExpression callMethod, Expression exp)
        {
            //  Get the two method arguments.
            MemberExpression memberColumn = ((MemberExpression)(base.Visit(callMethod.Arguments[0])));
            _sqlMethodStatement = String.Empty;
            if ((callMethod.Arguments[1].NodeType == ExpressionType.Constant))
            {
                //  Get the constant expression and
                //  create the current branch sql statement.
                ConstantExpression constant = ((ConstantExpression)(base.Visit(callMethod.Arguments[1])));
                _sqlMethodStatement = (_sqlMethodStatement + constant.Value.ToString());
            }
            else if ((callMethod.Arguments[1].NodeType == ExpressionType.MemberAccess))
            {
                //  Get the member type argument.
                MemberExpression expValue = ((MemberExpression)(base.Visit(callMethod.Arguments[1])));
                string memberNameValue = expValue.Member.Name;
                //  Get the constant expression and
                //  create the current branch sql statement.
                ConstantExpression constValue = ((ConstantExpression)(base.Visit(expValue.Expression)));
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue, (BindingFlags.NonPublic
                                | (BindingFlags.Public | BindingFlags.Instance)));
                object objectValue = fieldValue.GetValue(constValue.Value);
                _sqlMethodStatement = (_sqlMethodStatement + objectValue.ToString());
            }
            else
            {
                //  Get the binay type argument.
                BinaryExpression body = ((BinaryExpression)(base.Visit(callMethod.Arguments[1])));
                if (ContinueOnSqlMethodNodeType(body))
                {
                    //  Create the group expression sql,
                    //  add the left, node type and right
                    //  expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    //  Get the constant expression and
                    //  create the current branch sql statement.
                    ConstantExpression right = ((ConstantExpression)(base.Visit(body.Right)));
                    _sqlMethodStatement = (_sqlMethodStatement + right.Value.ToString());
                }
            }
            //  Create the final sql statement.
            _sqlStatement = (_sqlStatement + ("("
                        + (DatabaseColumnName(memberColumn) + (ValueNodeType(exp) + "'"
                        + (_sqlMethodStatement + "')")))));
        }

        /// <summary>
        /// If the current expression has branches then continue
        /// to the next branch.
        /// </summary>
        /// <param name="exp">The current expression to analyse.</param>
        /// <returns>True if the expression tree has more branches.</returns>
        private bool ContinueOnSqlMethodNodeType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Convertion and Validation Methods
        /// <summary>
        /// Gets the database column name for the current property member.
        /// </summary>
        /// <param name="member">The current member expression.</param>
        /// <returns>The database column name.</returns>
        private String DatabaseColumnName(MemberExpression member)
        {
            if (String.IsNullOrEmpty(_param.Name))
            {
                return "[" + GetDbColumnName
                    (member.Member.DeclaringType.GetProperty
                    (member.ToString().Replace(_param.ToString() + ".", ""))) + "]";
            }
            else
            {
                return "[" + GetDbColumnName
                    (member.Member.DeclaringType.GetProperty
                    (member.ToString().Replace(_param.Name + ".", ""))) + "]";
            }
        }

        /// <summary>
        /// Gets the constant sql value.
        /// </summary>
        /// <param name="constant">The constant expression type.</param>
        /// <returns>The column constant value.</returns>
        private String DatabaseColumnConstantValue(ConstantExpression constant)
        {
            return _dataTypeConversion.GetSqlStringValue(constant.Type, constant.Value);
        }

        /// <summary>
        /// Gets the constant sql value.
        /// </summary>
        /// <param name="constant">The constant expression type.</param>
        /// <returns>The column constant value.</returns>
        private String DatabaseColumnConstantValue(object constant)
        {
            return _dataTypeConversion.GetSqlStringValue(constant.GetType(), constant);
        }

        /// <summary>
        /// Gets the constant sql value.
        /// </summary>
        /// <param name="unary">The unary expression type.</param>
        /// <returns>The column unary operand constant value.</returns>
        private String DatabaseColumnUnaryOperand(UnaryExpression unary)
        {
            return DatabaseColumnConstantValue((ConstantExpression)unary.Operand);
        }

        /// <summary>
        /// Get the sql value equivalant expression.
        /// </summary>
        /// <param name="exp">The current expression to analyse.</param>
        /// <returns>The sql equivalant expression.</returns>
        private String ValueNodeType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                    return " + ";
                case ExpressionType.And:
                    return " & ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.Multiply:
                    return " * ";
                case ExpressionType.Not:
                    return " NOT ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                    return " | ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Subtract:
                    return " - ";
                case ExpressionType.ExclusiveOr:
                    return " ^ ";
                default:
                    return "";
            }
        }

        /// <summary>
        /// If the current expression has branches then continue
        /// to the next branch.
        /// </summary>
        /// <param name="exp">The current expression to analyse.</param>
        /// <returns>True if the expression tree has more branches.</returns>
        private bool ContinueOnNodeType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the current data entity column name and schema.
        /// </summary>
        /// <returns>The column name.</returns>
        private string GetDbColumnName(PropertyInfo property)
        {
            // For each attribute for the member
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the nequeo datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // Return the column name.
                    return att.ColumnName.TrimStart('_');
                }
            }

            // Return a null.
            return null;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Exprssion tree evaluator.
    /// </summary>
    public sealed class ExpressionEvaluator : Nequeo.Linq.ExpressionVisitor
    {
        #region Expression Evaluator Class

        #region Private Fields
        private String _sqlStatement = string.Empty;
        private String _sqlMethodStatement = string.Empty;
        private ParameterExpression _param = null;
        private DataTypeConversion _dataTypeConversion = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlWhereQuery(Expression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            LambdaExpression body = (LambdaExpression)base.Visit(expression);
            _param = (ParameterExpression)body.Parameters[0];

            // Start the expression builder.
            StartWhereCreation(body);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The lambda expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlWhereQuery(LambdaExpression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            _param = (ParameterExpression)expression.Parameters[0];

            // Start the expression builder.
            StartWhereCreation(expression);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlSelectQuery(Expression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            LambdaExpression body = (LambdaExpression)base.Visit(expression);
            _param = (ParameterExpression)body.Parameters[0];

            // Start the expression builder.
            StartSelectCreation(body);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The lambda expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlSelectQuery(LambdaExpression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            _param = (ParameterExpression)expression.Parameters[0];

            // Start the expression builder.
            StartSelectCreation(expression);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlOrderByQuery(Expression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            LambdaExpression body = (LambdaExpression)base.Visit(expression);
            _param = (ParameterExpression)body.Parameters[0];

            // Start the expression builder.
            StartOrderByCreation(body);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The lambda expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlOrderByQuery(LambdaExpression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            _param = (ParameterExpression)expression.Parameters[0];

            // Start the expression builder.
            StartOrderByCreation(expression);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlOrderByDescendingQuery(Expression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            LambdaExpression body = (LambdaExpression)base.Visit(expression);
            _param = (ParameterExpression)body.Parameters[0];

            // Start the expression builder.
            StartOrderByDescendingCreation(body);
            return _sqlStatement;
        }

        /// <summary>
        /// Creates the sql query from the expression tree.
        /// </summary>
        /// <param name="expression">The lambda expression tree to analyse.</param>
        /// <param name="dataTypeConversion">Convert data type manager.</param>
        /// <returns>The sql query string.</returns>
        public String CreateSqlOrderByDescendingQuery(LambdaExpression expression, DataTypeConversion dataTypeConversion)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;
            _dataTypeConversion = dataTypeConversion;

            // Get the current body lambad expression.
            _param = (ParameterExpression)expression.Parameters[0];

            // Start the expression builder.
            StartOrderByDescendingCreation(expression);
            return _sqlStatement;
        }
        #endregion

        #region Expression Tree Analysis Methods
        /// <summary>
        /// Start if the recursive expression analyser.
        /// </summary>
        /// <param name="body">The lambda expression body.</param>
        private void StartSelectCreation(LambdaExpression body)
        {
            switch (body.Body.NodeType)
            {
                case ExpressionType.New:
                    NewExpression expNew = ((NewExpression)(base.Visit(body.Body)));
                    System.Collections.ObjectModel.ReadOnlyCollection<Expression> arg = expNew.Arguments;
                    foreach (Expression exp in arg)
                    {
                        MemberExpression expArgMem = ((MemberExpression)(base.Visit(exp)));
                        _sqlStatement += "[" + expArgMem.Member.Name + "], ";
                    }
                    _sqlStatement = _sqlStatement.TrimEnd(',', ' ');
                    break;

                case ExpressionType.MemberAccess:
                    MemberExpression expMem = ((MemberExpression)(base.Visit(body.Body)));
                    _sqlStatement = "[" + expMem.Member.Name + "]";
                    break;
            }
        }

        /// <summary>
        /// Start if the recursive expression analyser.
        /// </summary>
        /// <param name="body">The lambda expression body.</param>
        private void StartOrderByCreation(LambdaExpression body)
        {
            switch (body.Body.NodeType)
            {
                case ExpressionType.New:
                    NewExpression expNew = ((NewExpression)(base.Visit(body.Body)));
                    System.Collections.ObjectModel.ReadOnlyCollection<Expression> arg = expNew.Arguments;
                    foreach (Expression exp in arg)
                    {
                        MemberExpression expArgMem = ((MemberExpression)(base.Visit(exp)));
                        _sqlStatement += "[" + expArgMem.Member.Name + "] ASC, ";
                    }
                    _sqlStatement = _sqlStatement.TrimEnd(',', ' ');
                    break;

                case ExpressionType.MemberAccess:
                    MemberExpression expMem = ((MemberExpression)(base.Visit(body.Body)));
                    _sqlStatement = "[" + expMem.Member.Name + "] ASC";
                    break;
            }
        }

        /// <summary>
        /// Start if the recursive expression analyser.
        /// </summary>
        /// <param name="body">The lambda expression body.</param>
        private void StartOrderByDescendingCreation(LambdaExpression body)
        {
            switch (body.Body.NodeType)
            {
                case ExpressionType.New:
                    NewExpression expNew = ((NewExpression)(base.Visit(body.Body)));
                    System.Collections.ObjectModel.ReadOnlyCollection<Expression> arg = expNew.Arguments;
                    foreach (Expression exp in arg)
                    {
                        MemberExpression expArgMem = ((MemberExpression)(base.Visit(exp)));
                        _sqlStatement += "[" + expArgMem.Member.Name + "] DESC, ";
                    }
                    _sqlStatement = _sqlStatement.TrimEnd(',', ' ');
                    break;

                case ExpressionType.MemberAccess:
                    MemberExpression expMem = ((MemberExpression)(base.Visit(body.Body)));
                    _sqlStatement = "[" + expMem.Member.Name + "] DESC";
                    break;
            }
        }

        /// <summary>
        /// Start if the recursive expression analyser.
        /// </summary>
        /// <param name="body">The lambda expression body.</param>
        private void StartWhereCreation(LambdaExpression body)
        {
            //  If more branches exist then
            //  start recursive analysis.
            if (ContinueOnNodeType(body.Body))
            {
                //  Get the current binary expression.
                BinaryExpression exp = ((BinaryExpression)(base.Visit(body.Body)));
                _sqlStatement += "(";
                LeftExpression(exp);
                _sqlStatement = (_sqlStatement + ValueNodeType(exp));
                RightExpression(exp);
                _sqlStatement += ")";
            }
            else
            {
                switch (body.Body.NodeType)
                {
                    case ExpressionType.Call:
                        //  Get the method expression from the expression tree
                        //  create the sql statement.
                        MethodCallExpression callMeth = ((MethodCallExpression)(base.Visit(body.Body)));
                        string methodName = callMeth.Method.Name;
                        SqlMethod(callMeth, methodName, null);
                        break;
                    default:
                        BinaryExpression exp = ((BinaryExpression)(base.Visit(body.Body)));
                        switch (exp.Left.NodeType)
                        {
                            case ExpressionType.Call:
                                MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
                                string methodNameCall = leftCall.Method.Name;
                                SqlMethod(leftCall, methodNameCall, exp);
                                break;
                            default:
                                MemberExpression left = ((MemberExpression)(base.Visit(exp.Left)));
                                if ((exp.Right.NodeType == ExpressionType.Convert))
                                {
                                    //  Get the unary expression and
                                    //  create the current branch sql statement.
                                    UnaryExpression right = ((UnaryExpression)(base.Visit(exp.Right)));
                                    _sqlStatement = (_sqlStatement
                                                + (("(" + DatabaseColumnName(left))
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnUnaryOperand(right) + ")"))));
                                }
                                else if ((exp.Right.NodeType == ExpressionType.MemberAccess))
                                {
                                    //  Get the member type argument.
                                    MemberExpression expValue = ((MemberExpression)(base.Visit(exp.Right)));
                                    string memberNameValue = expValue.Member.Name;
                                    //  Get the constant expression and
                                    //  create the current branch sql statement.
                                    ConstantExpression constValue = ((ConstantExpression)(base.Visit(expValue.Expression)));
                                    FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue, (BindingFlags.NonPublic
                                                    | (BindingFlags.Public | BindingFlags.Instance)));
                                    object objectValue = fieldValue.GetValue(constValue.Value);
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnConstantValue(objectValue) + ")")))));
                                }
                                else
                                {
                                    //  Get the constant expression and
                                    //  create the current branch sql statement.
                                    ConstantExpression right = ((ConstantExpression)(base.Visit(exp.Right)));
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnConstantValue(right) + ")")))));
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Left side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void LeftExpression(BinaryExpression expression)
        {
            //  If more branches exist then
            //  start recursive analysis.
            if (ContinueOnNodeType(expression.Left))
            {
                BinaryExpression exp = ((BinaryExpression)(base.Visit(expression.Left)));
                _sqlStatement += "(";
                LeftExpression(exp);
                _sqlStatement = (_sqlStatement + ValueNodeType(exp));
                RightExpression(exp);
                _sqlStatement += ")";
            }
            else
            {
                switch (expression.Left.NodeType)
                {
                    case ExpressionType.Call:
                        //  Get the method expression from the expression tree
                        //  create the sql statement.
                        MethodCallExpression callMeth = ((MethodCallExpression)(base.Visit(expression.Left)));
                        string methodName = callMeth.Method.Name;
                        SqlMethod(callMeth, methodName, null);
                        break;
                    default:
                        BinaryExpression exp = ((BinaryExpression)(base.Visit(expression.Left)));
                        switch (exp.Left.NodeType)
                        {
                            case ExpressionType.Call:
                                MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
                                string methodNameCall = leftCall.Method.Name;
                                SqlMethod(leftCall, methodNameCall, exp);
                                break;
                            default:
                                MemberExpression left = ((MemberExpression)(base.Visit(exp.Left)));
                                if ((exp.Right.NodeType == ExpressionType.Convert))
                                {
                                    //  Get the unary expression and
                                    //  create the current branch sql statement.
                                    UnaryExpression right = ((UnaryExpression)(base.Visit(exp.Right)));
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnUnaryOperand(right) + ")")))));
                                }
                                else if ((exp.Right.NodeType == ExpressionType.MemberAccess))
                                {
                                    //  Get the member type argument.
                                    MemberExpression expValue = ((MemberExpression)(base.Visit(exp.Right)));
                                    string memberNameValue = expValue.Member.Name;
                                    //  Get the constant expression and
                                    //  create the current branch sql statement.
                                    ConstantExpression constValue = ((ConstantExpression)(base.Visit(expValue.Expression)));
                                    FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue, (BindingFlags.NonPublic
                                                    | (BindingFlags.Public | BindingFlags.Instance)));
                                    object objectValue = fieldValue.GetValue(constValue.Value);
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnConstantValue(objectValue) + ")")))));
                                }
                                else
                                {
                                    //  Get the constant expression and
                                    //  create the current branch sql statement.
                                    ConstantExpression right = ((ConstantExpression)(base.Visit(exp.Right)));
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnConstantValue(right) + ")")))));
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Right side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void RightExpression(BinaryExpression expression)
        {
            //  If more branches exist then
            //  start recursive analysis.
            if (ContinueOnNodeType(expression.Right))
            {
                BinaryExpression exp = ((BinaryExpression)(base.Visit(expression.Right)));
                _sqlStatement += "(";
                LeftExpression(exp);
                _sqlStatement = (_sqlStatement + ValueNodeType(exp));
                RightExpression(exp);
                _sqlStatement += ")";
            }
            else
            {
                switch (expression.Right.NodeType)
                {
                    case ExpressionType.Call:
                        //  Get the method expression from the expression tree
                        //  create the sql statement.
                        MethodCallExpression callMeth = ((MethodCallExpression)(base.Visit(expression.Right)));
                        string methodName = callMeth.Method.Name;
                        SqlMethod(callMeth, methodName, null);
                        break;
                    default:
                        BinaryExpression exp = ((BinaryExpression)(base.Visit(expression.Right)));
                        switch (exp.Left.NodeType)
                        {
                            case ExpressionType.Call:
                                MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
                                string methodNameCall = leftCall.Method.Name;
                                SqlMethod(leftCall, methodNameCall, exp);
                                break;
                            default:
                                MemberExpression left = ((MemberExpression)(base.Visit(exp.Left)));
                                if ((exp.Right.NodeType == ExpressionType.Convert))
                                {
                                    //  Get the unary expression and
                                    //  create the current branch sql statement.
                                    UnaryExpression right = ((UnaryExpression)(base.Visit(exp.Right)));
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnUnaryOperand(right) + ")")))));
                                }
                                else if ((exp.Right.NodeType == ExpressionType.MemberAccess))
                                {
                                    //  Get the member type argument.
                                    MemberExpression expValue = ((MemberExpression)(base.Visit(exp.Right)));
                                    string memberNameValue = expValue.Member.Name;
                                    //  Get the constant expression and
                                    //  create the current branch sql statement.
                                    ConstantExpression constValue = ((ConstantExpression)(base.Visit(expValue.Expression)));
                                    FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue, (BindingFlags.NonPublic
                                                    | (BindingFlags.Public | BindingFlags.Instance)));
                                    object objectValue = fieldValue.GetValue(constValue.Value);
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnConstantValue(objectValue) + ")")))));
                                }
                                else
                                {
                                    //  Get the constant expression and
                                    //  create the current branch sql statement.
                                    ConstantExpression right = ((ConstantExpression)(base.Visit(exp.Right)));
                                    _sqlStatement = (_sqlStatement + ("("
                                                + (DatabaseColumnName(left)
                                                + (ValueNodeType(exp)
                                                + (DatabaseColumnConstantValue(right) + ")")))));
                                }
                                break;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Sql Method Expression Tree Analysis Methods
        /// <summary>
        /// Select the appropriate sql method call.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        /// <param name="sqlMethodName">The current sql method.</param>
        /// <param name="exp">The current sql method.</param>
        private void SqlMethod(MethodCallExpression call, string sqlMethodName, Expression exp)
        {
            switch (sqlMethodName.ToLower())
            {
                case "like":
                    SqlLikeMethod(call);
                    break;

                case "notlike":
                    SqlNotLikeMethod(call);
                    break;

                case "likeescape":
                    SqlLikeEscapeMethod(call);
                    break;

                case "notlikeescape":
                    SqlNotLikeEscapeMethod(call);
                    break;

                case "between":
                    SqlBetweenMethod(call);
                    break;

                case "notbetween":
                    SqlNotBetweenMethod(call);
                    break;

                case "isnull":
                    SqlIsNullMethod(call);
                    break;

                case "isnotnull":
                    SqlIsNotNullMethod(call);
                    break;

                case "contains":
                    SqlContainsMethod(call);
                    break;

                case "in":
                    SqlInMethod(call);
                    break;

                case "notin":
                    SqlNotInMethod(call);
                    break;

                case "comparestring":
                    SqlCompareStringMethod(call, exp);
                    break;

                default:
                    DefaultMethod(call, exp);
                    break;
            }
        }

        /// <summary>
        /// Default method call.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        /// <param name="exp">The current sql method.</param>
        private void DefaultMethod(MethodCallExpression call, Expression exp)
        {
            throw new Exception("Sql method is not valid.");
        }

        /// <summary>
        /// Create the 'Like' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlLikeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " LIKE '" +
                _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Create the 'Like' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotLikeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT LIKE '" +
                _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Create the 'Like with escape clause' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlLikeEscapeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            string sqlMethod = _sqlMethodStatement;
            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[2].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[2]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " LIKE '" +
                sqlMethod + "' ESCAPE '" + _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Create the 'Not Like with escape clause' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotLikeEscapeMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            string sqlMethod = _sqlMethodStatement;
            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[2].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[2]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT LIKE '" +
                sqlMethod + "' ESCAPE '" + _sqlMethodStatement + "')";
        }

        /// <summary>
        /// Left side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void LeftMethodExpression(BinaryExpression expression)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnSqlMethodNodeType(expression.Left))
            {
                BinaryExpression exp = (BinaryExpression)base.Visit(expression.Left);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                LeftMethodExpression(exp);
                RightMethodExpression(exp);
            }
            else
            {
                if (expression.Left.NodeType == ExpressionType.Constant)
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression left = (ConstantExpression)base.Visit(expression.Left);
                    _sqlMethodStatement += left.Value.ToString();
                }
                else
                {
                    // Get the current binary expression.
                    // get the member expression.
                    MemberExpression member = (MemberExpression)base.Visit(expression.Left);
                    string memberName = member.Member.Name;

                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(member.Expression);

                    // Get the current member argument field and
                    // get the value for the current argument field.
                    FieldInfo field = right.Value.GetType().GetField(memberName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    object memberValue = field.GetValue(right.Value);

                    // Create the current sql method statement.
                    _sqlMethodStatement += memberValue.ToString();
                }
            }
        }

        /// <summary>
        /// Right side expression member recursive method,
        /// this method is call recursively until all tree
        /// branches have be analysed.
        /// </summary>
        /// <param name="expression">The current binary expression.</param>
        private void RightMethodExpression(BinaryExpression expression)
        {
            // If more branches exist then
            // start recursive analysis.
            if (ContinueOnSqlMethodNodeType(expression.Right))
            {
                BinaryExpression exp = (BinaryExpression)base.Visit(expression.Right);

                // Create the group expression sql,
                // add the left, node type and right
                // expression group.
                LeftMethodExpression(exp);
                RightMethodExpression(exp);
            }
            else
            {
                if (expression.Right.NodeType == ExpressionType.Constant)
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(expression.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
                else
                {
                    // Get the current binary expression.
                    // get the member expression.
                    MemberExpression member = (MemberExpression)base.Visit(expression.Right);
                    string memberName = member.Member.Name;

                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(member.Expression);

                    // Get the current member argument field and
                    // get the value for the current argument field.
                    FieldInfo field = right.Value.GetType().GetField(memberName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    object memberValue = field.GetValue(right.Value);

                    // Create the current sql method statement.
                    _sqlMethodStatement += memberValue.ToString();
                }
            }
        }

        /// <summary>
        /// Create the 'Between' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlBetweenMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;
            object objectFrom = null;
            object objectTo = null;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                objectFrom = constant.Value;
            }
            else
            {
                // Get the member type argument.
                MemberExpression expFrom = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameFrom = expFrom.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constFrom = (ConstantExpression)base.Visit(expFrom.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldFrom = constFrom.Value.GetType().GetField(memberNameFrom,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectFrom = fieldFrom.GetValue(constFrom.Value);
            }

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                objectTo = constant.Value;
            }
            else
            {
                MemberExpression expTo = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameTo = expTo.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constTo = (ConstantExpression)base.Visit(expTo.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldTo = constTo.Value.GetType().GetField(memberNameTo,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectTo = fieldTo.GetValue(constTo.Value);
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " BETWEEN " +
                DatabaseColumnConstantValue(objectFrom) + " AND " + DatabaseColumnConstantValue(objectTo) + ")";
        }

        /// <summary>
        /// Create the 'Between' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotBetweenMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;
            object objectFrom = null;
            object objectTo = null;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                objectFrom = constant.Value.ToString();
            }
            else
            {
                // Get the member type argument.
                MemberExpression expFrom = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameFrom = expFrom.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constFrom = (ConstantExpression)base.Visit(expFrom.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldFrom = constFrom.Value.GetType().GetField(memberNameFrom,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectFrom = fieldFrom.GetValue(constFrom.Value);
            }

            if (call.Arguments[2].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[2]);
                objectTo = constant.Value.ToString();
            }
            else
            {
                MemberExpression expTo = (MemberExpression)base.Visit(call.Arguments[2]);
                string memberNameTo = expTo.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constTo = (ConstantExpression)base.Visit(expTo.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldTo = constTo.Value.GetType().GetField(memberNameTo,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                objectTo = fieldTo.GetValue(constTo.Value);
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT BETWEEN " +
                DatabaseColumnConstantValue(objectFrom) + " AND " + DatabaseColumnConstantValue(objectTo) + ")";
        }

        /// <summary>
        /// Create the 'Is Null' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlIsNullMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " IS NULL " + ")";
        }

        /// <summary>
        /// Create the 'Is Not Null' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlIsNotNullMethod(MethodCallExpression call)
        {
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " IS NOT NULL " + ")";
        }

        /// <summary>
        /// Create the 'Contains' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlContainsMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Object);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[0].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[0]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[0].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[0]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(CONTAINS(" + DatabaseColumnName(memberColumn) + ", '" +
                _sqlMethodStatement + "'))";
        }

        /// <summary>
        /// Create the 'In' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlInMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " IN ('" +
                _sqlMethodStatement + "'))";
        }

        /// <summary>
        /// Create the 'Not In' method sql statement.
        /// </summary>
        /// <param name="call">The current method call expression.</param>
        private void SqlNotInMethod(MethodCallExpression call)
        {
            // Get the two method arguments.
            MemberExpression memberColumn = (MemberExpression)base.Visit(call.Arguments[0]);

            // Clear the current sql method statement.
            _sqlMethodStatement = string.Empty;

            if (call.Arguments[1].NodeType == ExpressionType.Constant)
            {
                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constant = (ConstantExpression)base.Visit(call.Arguments[1]);
                _sqlMethodStatement += constant.Value.ToString();
            }
            else if (call.Arguments[1].NodeType == ExpressionType.MemberAccess)
            {
                // Get the member type argument.
                MemberExpression expValue = (MemberExpression)base.Visit(call.Arguments[1]);
                string memberNameValue = expValue.Member.Name;

                // Get the constant expression and
                // create the current branch sql statement.
                ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

                // Get the current member argument field and
                // get the value for the current argument field.
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object objectValue = fieldValue.GetValue(constValue.Value);

                _sqlMethodStatement += objectValue.ToString();
            }
            else
            {
                // Get the binay type argument.
                BinaryExpression body = (BinaryExpression)base.Visit(call.Arguments[1]);

                // If more branches exist then
                // start recursive analysis.
                if (ContinueOnSqlMethodNodeType(body))
                {
                    // Create the group expression sql,
                    // add the left, node type and right
                    // expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    // Get the constant expression and
                    // create the current branch sql statement.
                    ConstantExpression right = (ConstantExpression)base.Visit(body.Right);
                    _sqlMethodStatement += right.Value.ToString();
                }
            }

            // Create the final sql statement.
            _sqlStatement += "(" + DatabaseColumnName(memberColumn) + " NOT IN ('" +
                _sqlMethodStatement + "'))";
        }

        /// <summary>
        /// Create the 'Compare String' method sql statement.
        /// </summary>
        /// <param name="callMethod">The current method call expression.</param>
        /// <param name="exp">The current method call expression.</param>
        /// <remarks></remarks>
        private void SqlCompareStringMethod(MethodCallExpression callMethod, Expression exp)
        {
            //  Get the two method arguments.
            MemberExpression memberColumn = ((MemberExpression)(base.Visit(callMethod.Arguments[0])));
            _sqlMethodStatement = String.Empty;
            if ((callMethod.Arguments[1].NodeType == ExpressionType.Constant))
            {
                //  Get the constant expression and
                //  create the current branch sql statement.
                ConstantExpression constant = ((ConstantExpression)(base.Visit(callMethod.Arguments[1])));
                _sqlMethodStatement = (_sqlMethodStatement + constant.Value.ToString());
            }
            else if ((callMethod.Arguments[1].NodeType == ExpressionType.MemberAccess))
            {
                //  Get the member type argument.
                MemberExpression expValue = ((MemberExpression)(base.Visit(callMethod.Arguments[1])));
                string memberNameValue = expValue.Member.Name;
                //  Get the constant expression and
                //  create the current branch sql statement.
                ConstantExpression constValue = ((ConstantExpression)(base.Visit(expValue.Expression)));
                FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue, (BindingFlags.NonPublic
                                | (BindingFlags.Public | BindingFlags.Instance)));
                object objectValue = fieldValue.GetValue(constValue.Value);
                _sqlMethodStatement = (_sqlMethodStatement + objectValue.ToString());
            }
            else
            {
                //  Get the binay type argument.
                BinaryExpression body = ((BinaryExpression)(base.Visit(callMethod.Arguments[1])));
                if (ContinueOnSqlMethodNodeType(body))
                {
                    //  Create the group expression sql,
                    //  add the left, node type and right
                    //  expression group.
                    LeftMethodExpression(body);
                    RightMethodExpression(body);
                }
                else
                {
                    //  Get the constant expression and
                    //  create the current branch sql statement.
                    ConstantExpression right = ((ConstantExpression)(base.Visit(body.Right)));
                    _sqlMethodStatement = (_sqlMethodStatement + right.Value.ToString());
                }
            }
            //  Create the final sql statement.
            _sqlStatement = (_sqlStatement + ("("
                        + (DatabaseColumnName(memberColumn) + (ValueNodeType(exp) + "'"
                        + (_sqlMethodStatement + "')")))));
        }

        #endregion

        #region Convertion and Validation Methods
        /// <summary>
        /// Gets the database column name for the current property member.
        /// </summary>
        /// <param name="member">The current member expression.</param>
        /// <returns>The database column name.</returns>
        private String DatabaseColumnName(MemberExpression member)
        {
            if (String.IsNullOrEmpty(_param.Name))
            {
                return "[" + member.Member.DeclaringType.
                    GetProperty(member.ToString().
                    Replace((_param.ToString() + "."), "")).Name + "]";
            }
            else
            {
                return "[" + member.Member.DeclaringType.
                    GetProperty(member.ToString().
                    Replace((_param.Name + "."), "")).Name + "]";
            }
        }

        /// <summary>
        /// Gets the constant sql value.
        /// </summary>
        /// <param name="constant">The constant expression type.</param>
        /// <returns>The column constant value.</returns>
        private String DatabaseColumnConstantValue(ConstantExpression constant)
        {
            return _dataTypeConversion.GetSqlStringValue(constant.Type, constant.Value);
        }

        /// <summary>
        /// Gets the constant sql value.
        /// </summary>
        /// <param name="constant">The constant expression type.</param>
        /// <returns>The column constant value.</returns>
        private String DatabaseColumnConstantValue(object constant)
        {
            return _dataTypeConversion.GetSqlStringValue(constant.GetType(), constant);
        }

        /// <summary>
        /// Gets the constant sql value.
        /// </summary>
        /// <param name="unary">The unary expression type.</param>
        /// <returns>The column unary operand constant value.</returns>
        private String DatabaseColumnUnaryOperand(UnaryExpression unary)
        {
            return DatabaseColumnConstantValue((ConstantExpression)unary.Operand);
        }

        /// <summary>
        /// Get the sql value equivalant expression.
        /// </summary>
        /// <param name="exp">The current expression to analyse.</param>
        /// <returns>The sql equivalant expression.</returns>
        private String ValueNodeType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                    return " + ";
                case ExpressionType.And:
                    return " AND ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.Multiply:
                    return " * ";
                case ExpressionType.Not:
                    return " NOT ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                    return " OR ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Subtract:
                    return " - ";
                case ExpressionType.ExclusiveOr:
                    return " ^ ";
                default:
                    return "";
            }
        }

        /// <summary>
        /// If the current expression has branches then continue
        /// to the next branch.
        /// </summary>
        /// <param name="exp">The current expression to analyse.</param>
        /// <returns>True if the expression tree has more branches.</returns>
        private bool ContinueOnNodeType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    return true;
                case ExpressionType.OrElse:
                    return true;
                case ExpressionType.And:
                    return true;
                case ExpressionType.Or:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// If the current expression has branches then continue to the next branch.
        /// </summary>
        /// <param name="exp">The current expression to analyse.</param>
        /// <returns>True if the expression tree has more branches.</returns>
        private bool ContinueOnSqlMethodNodeType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                    return true;
                case ExpressionType.And:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the current data entity column name and schema.
        /// </summary>
        /// <returns>The column name.</returns>
        private string GetDbColumnName(PropertyInfo property)
        {
            // For each attribute for the member
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the Amadeus datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // Return the column name.
                    return att.ColumnName.TrimStart('_');
                }
            }

            // Return a null.
            return null;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Constructs an sql statement.
    /// </summary>
    public sealed class SqlStatementConstructor
    {
        #region Sql Statement Constructor

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dataTypeConversion">A data type conversion.</param>
        public SqlStatementConstructor(DataTypeConversion dataTypeConversion)
        {
            if (dataTypeConversion == null)
                throw new ArgumentNullException("dataTypeConversion");

            _dataTypeConversion = dataTypeConversion;
        }
        #endregion

        #region Fields
        private DataTypeConversion _dataTypeConversion = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the sql query from the queries.
        /// </summary>
        /// <param name="searchQueries">The search query item list.</param>
        /// <param name="columnNames">The column names to match against.</param>
        /// <param name="operand">The operand used to compare each query with.</param>
        /// <returns>The sql query string.</returns>
        public string CreateSqlWhereQuery(string[] searchQueries, string[] columnNames,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            int totalOperands = searchQueries.Length + columnNames.Length;
            int count = 0;

            // For each column
            foreach (string column in columnNames)
            {
                // For each query.
                foreach (string query in searchQueries)
                {
                    count++;
                    sqlBuilder.Append("([" + column + "] LIKE '%" + query + "%')");

                    // Do not add the last operand.
                    if (count < totalOperands)
                        sqlBuilder.Append(" " + Linq.Expressions.GetSqlExpression(operand) + " ");
                }
            }
            
            // Return the query.
            return DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                _dataTypeConversion.ConnectionDataType, sqlBuilder.ToString());
        }

        /// <summary>
        /// Creates the sql query from the queries.
        /// </summary>
        /// <param name="searchQueries">The search query item list.</param>
        /// <param name="operand">The operand used to compare each query with.</param>
        /// <returns>The sql query string.</returns>
        public string CreateSqlWhereQuery(Custom.SearchQueryModel[] searchQueries,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            int count = 0;

            // For each column
            foreach (Custom.SearchQueryModel query in searchQueries)
            {
                count++;
                sqlBuilder.Append(CreateSqlQuery(query));

                // Do not add the last operand.
                if (count < searchQueries.Length)
                    sqlBuilder.Append(" " + Linq.Expressions.GetSqlExpression(operand) + " ");
            }

            // Return the query.
            return DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                _dataTypeConversion.ConnectionDataType, sqlBuilder.ToString());
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <typeparam name="TDataEntity">The data type to create the expression for.</typeparam>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Func<TDataEntity, bool>> CreateLambdaExpression<TDataEntity>(Custom.SearchQueryModel[] searchQueries,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, typeof(TDataEntity), paramExpr, operand);

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <typeparam name="TDataEntity">The data type to create the expression for.</typeparam>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Func<TDataEntity, bool>> CreateLambdaExpression<TDataEntity>(Custom.QueryModel[] searchQueries,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, typeof(TDataEntity), paramExpr, operand);

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="dataEntity">The data type to create the expression for.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression 'Expression[Func[TDataEntity, bool]]'.</returns>
        public object CreateLambdaExpression(Custom.SearchQueryModel[] searchQueries, Type dataEntity,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(dataEntity, "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, dataEntity, paramExpr, operand);

            // Create the func delegate type.
            Type funcType = typeof(Func<,>);
            Type[] funcArgs = { dataEntity, typeof(bool) };
            Type funcMakeType = funcType.MakeGenericType(funcArgs);

            // This expression represents a lambda expression 
            object predicate = Expression.Lambda(funcMakeType, result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="dataEntity">The data type to create the expression for.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression 'Expression[Func[TDataEntity, bool]]'.</returns>
        public object CreateLambdaExpression(Custom.QueryModel[] searchQueries, Type dataEntity,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(dataEntity, "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, dataEntity, paramExpr, operand);

            // Create the func delegate type.
            Type funcType = typeof(Func<,>);
            Type[] funcArgs = { dataEntity, typeof(bool) };
            Type funcMakeType = funcType.MakeGenericType(funcArgs);

            // This expression represents a lambda expression 
            object predicate = Expression.Lambda(funcMakeType, result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <typeparam name="TDataEntity">The data type to create the expression for.</typeparam>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> CreateLambdaExpressionEx<TDataEntity>(Custom.SearchQueryModel[] searchQueries,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, typeof(TDataEntity), paramExpr, operand);

            // This expression represents a lambda expression 
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate =
                Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>>.Lambda<Nequeo.Threading.FunctionHandler<bool, TDataEntity>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <typeparam name="TDataEntity">The data type to create the expression for.</typeparam>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> CreateLambdaExpressionEx<TDataEntity>(Custom.QueryModel[] searchQueries,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, typeof(TDataEntity), paramExpr, operand);

            // This expression represents a lambda expression 
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate =
                Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>>.Lambda<Nequeo.Threading.FunctionHandler<bool, TDataEntity>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="dataEntity">The data type to create the expression for.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression 'Expression[Nequeo.Threading.FunctionHandler[bool, TDataEntity]]'.</returns>
        public object CreateLambdaExpressionEx(Custom.SearchQueryModel[] searchQueries, Type dataEntity,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(dataEntity, "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, dataEntity, paramExpr, operand);

            // Create the func delegate type.
            Type funcType = typeof(Nequeo.Threading.FunctionHandler<,>);
            Type[] funcArgs = { typeof(bool), dataEntity };
            Type funcMakeType = funcType.MakeGenericType(funcArgs);

            // This expression represents a lambda expression 
            object predicate = Expression.Lambda(funcMakeType, result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="dataEntity">The data type to create the expression for.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The lambda expression 'Expression[Nequeo.Threading.FunctionHandler[bool, TDataEntity]]'.</returns>
        public object CreateLambdaExpressionEx(Custom.QueryModel[] searchQueries, Type dataEntity,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(dataEntity, "u");

            // Create the expression.
            Expression result = CreateExpression(searchQueries, dataEntity, paramExpr, operand);

            // Create the func delegate type.
            Type funcType = typeof(Nequeo.Threading.FunctionHandler<,>);
            Type[] funcArgs = { typeof(bool), dataEntity  };
            Type funcMakeType = funcType.MakeGenericType(funcArgs);

            // This expression represents a lambda expression 
            object predicate = Expression.Lambda(funcMakeType, result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="dataEntity">The data type to create the expression for.</param>
        /// <param name="paramExpr">The data entity parameter.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The expression.</returns>
        private Expression CreateExpression(Custom.QueryModel[] searchQueries,
            Type dataEntity, ParameterExpression paramExpr,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // The resulting expression.
            Expression result = null;
            Expression[] created = new Expression[searchQueries.Length];
            Expression swap = null;

            // For each query.
            // For each query create the expression.
            for (int i = 0; i < searchQueries.Length; i++)
            {
                // Create the expression.
                created[i] = CreateExpression(searchQueries[i].Queries, dataEntity, paramExpr, searchQueries[i].Operand);
            }

            // If more than one expression.
            if (searchQueries.Length > 1)
            {
                // Get the first two exressions.
                swap = CreateCombinedExpression(created[0], created[1], operand);

                // For each query created.
                for (int i = 2; i < created.Length; i++)
                {
                    // Get the new swap.
                    swap = CreateCombinedExpression(swap, created[i], operand);
                }

                // Assign the final expression result.
                result = swap;
            }
            else
            {
                // If only one then get the first expression.
                result = created[0];
            }

            // Return the expression.
            return result;
        }

        /// <summary>
        /// Create the lambda expression from the queries.
        /// </summary>
        /// <param name="searchQueries">The collection of search queries.</param>
        /// <param name="dataEntity">The data type to create the expression for.</param>
        /// <param name="paramExpr">The data entity parameter.</param>
        /// <param name="operand">The combining expression operand.</param>
        /// <returns>The expression.</returns>
        private Expression CreateExpression(Custom.SearchQueryModel[] searchQueries, 
            Type dataEntity, ParameterExpression paramExpr,
            Linq.ExpressionOperandType operand = Linq.ExpressionOperandType.OrElse)
        {
            // The resulting expression.
            Expression result = null;
            Expression[] created = new Expression[searchQueries.Length];
            Expression swap = null;

            // Create the initial expression.
            PropertyInfo property = dataEntity.GetProperty(searchQueries[0].ColumnName);
            Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);

            // If more than one expression.
            if (searchQueries.Length > 1)
            {
                // For each query create the expression.
                for (int i = 0; i < searchQueries.Length; i++)
                {
                    // Create the cuurent expression.
                    property = dataEntity.GetProperty(searchQueries[i].ColumnName);
                    propertyMember = Expression.MakeMemberAccess(paramExpr, property);
                    created[i] = CreateExpression(searchQueries[i], propertyMember);
                }

                // Get the first two exressions.
                swap = CreateCombinedExpression(created[0], created[1], operand);

                // For each query created.
                for (int i = 2; i < created.Length; i++)
                {
                    // Get the new swap.
                    swap = CreateCombinedExpression(swap, created[i], operand);
                }

                // Assign the final expression result.
                result = swap;
            }
            else
            {
                // If only one expression.
                result = CreateExpression(searchQueries[0], propertyMember);
            }

            // Return the expression.
            return result;
        }

        /// <summary>
        /// Create the combined expression.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <param name="operand">The joining operand.</param>
        /// <returns>The combined expression.</returns>
        private Expression CreateCombinedExpression(Expression left, Expression right, Linq.ExpressionOperandType operand)
        {
            switch (operand)
            {
                case ExpressionOperandType.Add:
                    return Expression.Add(left, right);

                case ExpressionOperandType.And:
                    return Expression.And(left, right);

                case ExpressionOperandType.AndAlso:
                    return Expression.AndAlso(left, right);

                case ExpressionOperandType.Divide:
                    return Expression.Divide(left, right);

                case ExpressionOperandType.Equal:
                    return Expression.Equal(left, right);

                case ExpressionOperandType.GreaterThan:
                    return Expression.GreaterThan(left, right);

                case ExpressionOperandType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);

                case ExpressionOperandType.IsFalse:
                    return Expression.IsFalse(left);

                case ExpressionOperandType.IsTrue:
                    return Expression.IsTrue(left);

                case ExpressionOperandType.LessThan:
                    return Expression.LessThan(left, right);

                case ExpressionOperandType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);

                case ExpressionOperandType.Multiply:
                    return Expression.Multiply(left, right);

                case ExpressionOperandType.NotEqual:
                    return Expression.NotEqual(left, right);

                case ExpressionOperandType.Or:
                    return Expression.Or(left, right);

                case ExpressionOperandType.OrElse:
                    return Expression.OrElse(left, right);

                case ExpressionOperandType.Subtract:
                    return Expression.Subtract(left, right);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create the expression.
        /// </summary>
        /// <param name="query">The current query.</param>
        /// <param name="propertyMember">The current property member.</param>
        /// <returns>The new expression.</returns>
        private Expression CreateExpression(Custom.SearchQueryModel query, Expression propertyMember)
        {
            switch (query.Operand)
            {
                case ExpressionOperandType.Add:
                    return Expression.Add(Expression.Constant(query.Value, query.ValueType), Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.And:
                    return Expression.And(Expression.Constant(query.Value, query.ValueType), Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.AndAlso:
                    Expression leftAndAlso = Expression.Equal(propertyMember, Expression.Constant(query.Value, query.ValueType));
                    Expression rightAndAlso = Expression.Equal(propertyMember, Expression.Constant(query.Value1, query.ValueType));
                    return Expression.AndAlso(leftAndAlso, rightAndAlso);

                case ExpressionOperandType.Between:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember,
                        Expression.Constant(query.Value, query.ValueType),
                        Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.Divide:
                    return Expression.Divide(Expression.Constant(query.Value, query.ValueType), Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.Equal:
                    return Expression.Equal(propertyMember, Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.GreaterThan:
                    return Expression.GreaterThan(propertyMember, Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(propertyMember, Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.In:
                    if (query.Value1 != null)
                    {
                        return Expression.Call(
                            typeof(Data.TypeExtenders.SqlQueryMethods),
                            Linq.Expressions.GetExpression(query.Operand),
                            null,
                            propertyMember,
                            Expression.Constant(query.Value, query.ValueType),
                            Expression.Constant(query.Value1, query.ValueType));
                    }
                    else
                    {
                        return Expression.Call(
                            typeof(Data.TypeExtenders.SqlQueryMethods),
                            Linq.Expressions.GetExpression(query.Operand),
                            null,
                            propertyMember,
                            Expression.Constant(query.Value, query.ValueType));
                    }

                case ExpressionOperandType.IsFalse:
                    return Expression.IsFalse(propertyMember);

                case ExpressionOperandType.IsNotNull:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember);

                case ExpressionOperandType.IsNull:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember);

                case ExpressionOperandType.IsTrue:
                    return Expression.IsTrue(propertyMember);

                case ExpressionOperandType.LessThan:
                    return Expression.LessThan(propertyMember, Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(propertyMember, Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.Like:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember,
                        Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.LikeEscape:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember,
                        Expression.Constant(query.Value, query.ValueType),
                        Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.Multiply:
                    return Expression.Multiply(Expression.Constant(query.Value, query.ValueType), Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.NotBetween:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember,
                        Expression.Constant(query.Value, query.ValueType),
                        Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.NotEqual:
                    return Expression.NotEqual(propertyMember, Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.NotIn:
                    if (query.Value1 != null)
                    {
                        return Expression.Call(
                            typeof(Data.TypeExtenders.SqlQueryMethods),
                            Linq.Expressions.GetExpression(query.Operand),
                            null,
                            propertyMember,
                            Expression.Constant(query.Value, query.ValueType),
                            Expression.Constant(query.Value1, query.ValueType));
                    }
                    else
                    {
                        return Expression.Call(
                            typeof(Data.TypeExtenders.SqlQueryMethods),
                            Linq.Expressions.GetExpression(query.Operand),
                            null,
                            propertyMember,
                            Expression.Constant(query.Value, query.ValueType));
                    }

                case ExpressionOperandType.NotLike:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember,
                        Expression.Constant(query.Value, query.ValueType));

                case ExpressionOperandType.NotLikeEscape:
                    return Expression.Call(
                        typeof(Data.TypeExtenders.SqlQueryMethods),
                        Linq.Expressions.GetExpression(query.Operand),
                        null,
                        propertyMember,
                        Expression.Constant(query.Value, query.ValueType),
                        Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.Or:
                    return Expression.Or(Expression.Constant(query.Value, query.ValueType), Expression.Constant(query.Value1, query.ValueType));

                case ExpressionOperandType.OrElse:
                    Expression leftOrElse = Expression.Equal(propertyMember, Expression.Constant(query.Value, query.ValueType));
                    Expression rightOrElse = Expression.Equal(propertyMember, Expression.Constant(query.Value1, query.ValueType));
                    return Expression.OrElse(leftOrElse, rightOrElse);

                case ExpressionOperandType.Subtract:
                    return Expression.Subtract(Expression.Constant(query.Value, query.ValueType), Expression.Constant(query.Value1, query.ValueType));

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create the SQL query expression.
        /// </summary>
        /// <param name="query">The search query model.</param>
        /// <returns>The sql expression.</returns>
        private string CreateSqlQuery(Custom.SearchQueryModel query)
        {
            switch (query.Operand)
            {
                case Linq.ExpressionOperandType.Add:
                    return "(" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " + " + 
                                 _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.Between:
                    return "([" + query.ColumnName + "] BETWEEN " + 
                        _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " AND " + 
                        _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.Divide:
                    return "(" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " / " +
                                 _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.Equal:
                    return "([" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.GreaterThan:
                    return "([" + query.ColumnName + "] > " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.GreaterThanOrEqual:
                    return "([" + query.ColumnName + "] >= " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.In:
                    if (query.Value1 != null)
                    {
                        return "([" + query.ColumnName + "] IN (" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ", " + 
                                                                    _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + "))";
                    }
                    else
                    {
                        return "([" + query.ColumnName + "] IN (" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + "))";
                    }
                case Linq.ExpressionOperandType.IsFalse:
                    return "(" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " = FALSE)";
                case Linq.ExpressionOperandType.IsNotNull:
                    return "([" + query.ColumnName + "] IS NOT NULL)";
                case Linq.ExpressionOperandType.IsNull:
                    return "([" + query.ColumnName + "] IS NULL)";
                case Linq.ExpressionOperandType.IsTrue:
                    return "(" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " = TRUE)";
                case Linq.ExpressionOperandType.LessThan:
                    return "([" + query.ColumnName + "] < " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.LessThanOrEqual:
                    return "([" + query.ColumnName + "] <= " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.Like:
                    return "([" + query.ColumnName + "] LIKE " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.LikeEscape:
                    return "([" + query.ColumnName + "] LIKE " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " ESCAPE " + 
                                                                 _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.Multiply:
                    return "(" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " * " +
                                 _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.NotBetween:
                    return "([" + query.ColumnName + "] NOT BETWEEN " +
                        _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " AND " +
                        _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.NotEqual:
                    return "([" + query.ColumnName + "] <> " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.NotIn:
                    if (query.Value1 != null)
                    {
                        return "([" + query.ColumnName + "] NOT IN (" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ", " +
                                                                        _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + "))";
                    }
                    else
                    {
                        return "([" + query.ColumnName + "] IN (" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + "))";
                    }
                case Linq.ExpressionOperandType.NotLike:
                    return "([" + query.ColumnName + "] NOT LIKE " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + ")";
                case Linq.ExpressionOperandType.NotLikeEscape:
                    return "([" + query.ColumnName + "] NOT LIKE " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " ESCAPE " +
                                                                     _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.Subtract:
                    return "(" + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " - " +
                                 _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.And:
                    return "([" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " AND " +
                            "[" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.Or:
                    return "([" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " OR " +
                            "[" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.AndAlso:
                    return "([" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " AND " +
                           "[" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                case Linq.ExpressionOperandType.OrElse:
                    return "([" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value) + " OR " +
                            "[" + query.ColumnName + "] = " + _dataTypeConversion.GetSqlStringValue(query.ValueType, query.Value1) + ")";
                default:
                    return "";
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Constructs an sql statement from a linq expression tree.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type</typeparam>
    public sealed class SqlStatementConstructor<TDataEntity> : Nequeo.Linq.IQueryContext
        where TDataEntity : class, new()
    {
        #region Sql Statement Constructor

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tableName">The table name to target.</param>
        /// <param name="dataTypeConversion">A data type conversion.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public SqlStatementConstructor(string tableName, DataTypeConversion dataTypeConversion, IDataAccess dataAccessProvider)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            if (dataTypeConversion == null)
                throw new ArgumentNullException("dataTypeConversion");

            if (dataAccessProvider == null)
                throw new ArgumentNullException("dataAccessProvider");

            _dataTypeConversion = dataTypeConversion;
            _connectionDataType = dataTypeConversion.ConnectionDataType;
            _dataAccessProvider = dataAccessProvider;
            _tableName = DataTypeConversion.GetSqlConversionDataType(_connectionDataType, tableName.Replace(".", "].["));
        }

        /// <summary>
        /// Default constructor, if the connection type is set then the query is executed.
        /// </summary>
        /// <param name="tableName">The table name to target.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="dataTypeConversion">A data type conversion.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public SqlStatementConstructor(string tableName, ConnectionContext.ConnectionType connectionType,
            DataTypeConversion dataTypeConversion, string connectionString, IDataAccess dataAccessProvider)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            if (dataTypeConversion == null)
                throw new ArgumentNullException("dataTypeConversion");

            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            if (dataAccessProvider == null)
                throw new ArgumentNullException("dataAccessProvider");

            _dataTypeConversion = dataTypeConversion;
            _connectionType = connectionType;
            _connectionDataType = dataTypeConversion.ConnectionDataType;
            _connectionString = connectionString;
            _dataAccessProvider = dataAccessProvider;
            _tableName = DataTypeConversion.GetSqlConversionDataType(_connectionDataType, tableName.Replace(".", "].["));
        }
        #endregion

        #region Fields
        private string _tableName = string.Empty;
        private string _takeSkipSql = string.Empty;
        private string _whereSql = string.Empty;
        private string _firstSql = string.Empty;
        private string _selectSql = string.Empty;
        private string _orderbySql = string.Empty;
        private string _orderbyDescendingSql = string.Empty;
        private string _thenbySql = string.Empty;
        private string _thenbyDescendingSql = string.Empty;
        private string _orderbySqlCombine = string.Empty;
        private string _connectionString = string.Empty;
        private DataTypeConversion _dataTypeConversion = null;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the connection type.
        /// </summary>
        public ConnectionContext.ConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        /// <summary>
        /// Gets sets, connection data type.
        /// </summary>
        public ConnectionContext.ConnectionDataType ConnectionDataType
        {
            get { return _connectionDataType; }
            set
            {
                _connectionDataType = value;
                _dataTypeConversion.ConnectionDataType = value;
            }
        }

        /// <summary>
        /// Gets sets, connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Create a new instance of the current class.
        /// </summary>
        /// <param name="tableName">The name of the table to target.</param>
        /// <param name="dataTypeConversion">A data type conversion object.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        /// <returns>A new instance of the current class.</returns>
        public static SqlStatementConstructor<TDataEntity> CreateInstance(string tableName, 
            DataTypeConversion dataTypeConversion, IDataAccess dataAccessProvider)
        {
            SqlStatementConstructor<TDataEntity> sql =
                new SqlStatementConstructor<TDataEntity>(tableName, dataTypeConversion, dataAccessProvider);
            return sql;
        }

        /// <summary>
        /// Create a new instance of the current class.
        /// </summary>
        /// <param name="tableName">The name of the table to target.</param>
        /// <param name="connectionType">The current connection type.</param>
        /// <param name="dataTypeConversion">A data type conversion object.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        /// <returns></returns>
        public static SqlStatementConstructor<TDataEntity> CreateInstance(string tableName, ConnectionContext.ConnectionType connectionType,
            DataTypeConversion dataTypeConversion, string connectionString, IDataAccess dataAccessProvider)
        {
            SqlStatementConstructor<TDataEntity> sql =
                new SqlStatementConstructor<TDataEntity>(tableName, connectionType, dataTypeConversion, connectionString, dataAccessProvider);
            return sql;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The execute method called by the query provider.
        /// </summary>
        /// <param name="expression">The expression tree to evaluate.</param>
        /// <param name="IsEnumerable">Is the return object an enumerable type.</param>
        /// <returns>The return enumerable object.</returns>
        public object Execute(Expression expression, bool IsEnumerable)
        {
            // The expression must represent a query over the data source.
            //if (!IsQueryOverDataSource(expression))
                //throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Skip() and get the lambda expression predicate.
            InnermostSkipFinder skipFinder = new InnermostSkipFinder();
            MethodCallExpression skipExpression = skipFinder.GetInnermostSkip(expression);

            // Find the call to Take() and get the lambda expression predicate.
            InnermostTakeFinder takeFinder = new InnermostTakeFinder();
            MethodCallExpression takeExpression = takeFinder.GetInnermostTake(expression);

            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);

            // Find the call to First() and get the lambda expression predicate.
            InnermostFirstFinder firstFinder = new InnermostFirstFinder();
            MethodCallExpression firstExpression = firstFinder.GetInnermostFirst(expression);

            // Find the call to Select() and get the lambda expression predicate.
            InnermostSelectFinder selectFinder = new InnermostSelectFinder();
            MethodCallExpression selectExpression = selectFinder.GetInnermostSelect(expression);

            // Find the call to OrderBy() and get the lambda expression predicate.
            InnermostOrderByFinder orderByFinder = new InnermostOrderByFinder();
            MethodCallExpression orderByExpression = orderByFinder.GetInnermostOrderBy(expression);

            // Find the call to OrderByDescending() and get the lambda expression predicate.
            InnermostOrderByDescendingFinder orderByDescendingFinder = new InnermostOrderByDescendingFinder();
            MethodCallExpression orderByDescendingExpression = orderByDescendingFinder.GetInnermostOrderByDescending(expression);

            // Find the call to ThenBy() and get the lambda expression predicate.
            InnermostThenByFinder thenByFinder = new InnermostThenByFinder();
            MethodCallExpression thenByExpression = thenByFinder.GetInnermostThenBy(expression);

            // Find the call to ThenByDescending() and get the lambda expression predicate.
            InnermostThenByDescendingFinder thenByDescendingFinder = new InnermostThenByDescendingFinder();
            MethodCallExpression thenByDescendingExpression = thenByDescendingFinder.GetInnermostThenByDescending(expression);

            // Get the where arguments
            if (whereExpression != null)
            {
                LambdaExpression whereLambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;
                whereLambdaExpression = (LambdaExpression)Evaluator.PartialEval(whereLambdaExpression);
                ExpressionEvaluator evalWhere = new ExpressionEvaluator();
                _whereSql = evalWhere.CreateSqlWhereQuery(whereLambdaExpression, _dataTypeConversion);
            }

            // Get the first arguments
            if (firstExpression != null)
            {
                if (firstExpression.Arguments.Count() > 1)
                {
                    LambdaExpression firstLambdaExpression = (LambdaExpression)((UnaryExpression)(firstExpression.Arguments[1])).Operand;
                    firstLambdaExpression = (LambdaExpression)Evaluator.PartialEval(firstLambdaExpression);
                    ExpressionEvaluator evalFirst = new ExpressionEvaluator();
                    _whereSql = evalFirst.CreateSqlWhereQuery(firstLambdaExpression, _dataTypeConversion);
                }
                _firstSql = "1";
            }

            // Get the select arguments
            if (selectExpression != null)
            {
                // Get the select arguments
                LambdaExpression selectLambdaExpression = (LambdaExpression)((UnaryExpression)(selectExpression.Arguments[1])).Operand;
                selectLambdaExpression = (LambdaExpression)Evaluator.PartialEval(selectLambdaExpression);
                ExpressionEvaluator evalSelect = new ExpressionEvaluator();
                _selectSql = evalSelect.CreateSqlSelectQuery(selectLambdaExpression, _dataTypeConversion);
            }

            // Get the orderby arguments
            if (orderByExpression != null)
            {
                LambdaExpression orderByLambdaExpression = (LambdaExpression)((UnaryExpression)(orderByExpression.Arguments[1])).Operand;
                orderByLambdaExpression = (LambdaExpression)Evaluator.PartialEval(orderByLambdaExpression);
                ExpressionEvaluator evalOrderBy = new ExpressionEvaluator();
                _orderbySql = evalOrderBy.CreateSqlOrderByQuery(orderByLambdaExpression, _dataTypeConversion);
            }

            // Get the orderby descending arguments
            if (orderByDescendingExpression != null)
            {
                LambdaExpression orderByDescendingLambdaExpression = (LambdaExpression)((UnaryExpression)(orderByDescendingExpression.Arguments[1])).Operand;
                orderByDescendingLambdaExpression = (LambdaExpression)Evaluator.PartialEval(orderByDescendingLambdaExpression);
                ExpressionEvaluator evalOrderByDescending = new ExpressionEvaluator();
                _orderbyDescendingSql = evalOrderByDescending.CreateSqlOrderByDescendingQuery(orderByDescendingLambdaExpression, _dataTypeConversion);
            }

            // Get the thenby arguments
            if (thenByExpression != null)
            {
                LambdaExpression thenByLambdaExpression = (LambdaExpression)((UnaryExpression)(thenByExpression.Arguments[1])).Operand;
                thenByLambdaExpression = (LambdaExpression)Evaluator.PartialEval(thenByLambdaExpression);
                ExpressionEvaluator evalThenBy = new ExpressionEvaluator();
                _thenbySql = evalThenBy.CreateSqlOrderByQuery(thenByLambdaExpression, _dataTypeConversion);
            }

            // Get the thenby descending arguments
            if (thenByDescendingExpression != null)
            {
                LambdaExpression thenByDescendingLambdaExpression = (LambdaExpression)((UnaryExpression)(thenByDescendingExpression.Arguments[1])).Operand;
                thenByDescendingLambdaExpression = (LambdaExpression)Evaluator.PartialEval(thenByDescendingLambdaExpression);
                ExpressionEvaluator evalThenByDescending = new ExpressionEvaluator();
                _thenbyDescendingSql = evalThenByDescending.CreateSqlOrderByDescendingQuery(thenByDescendingLambdaExpression, _dataTypeConversion);
            }

            // If any of the order by methods have been found
            // then construct to orderby sql statement.
            if (!String.IsNullOrEmpty(_orderbySql) ||
                !String.IsNullOrEmpty(_orderbyDescendingSql) ||
                !String.IsNullOrEmpty(_thenbySql) ||
                !String.IsNullOrEmpty(_thenbyDescendingSql))
                _orderbySqlCombine = 
                    ((String.IsNullOrEmpty(_orderbySql) ? "" : _orderbySql + ", ") +
                    (String.IsNullOrEmpty(_thenbySql) ? "" : _thenbySql + ", ") +
                    (String.IsNullOrEmpty(_orderbyDescendingSql) ? "" : _orderbyDescendingSql + ", ") +
                    _thenbyDescendingSql).TrimEnd(',', ' ');

            if (!string.IsNullOrEmpty(_whereSql))
                _whereSql = TransformStatement(_whereSql);
            if (!string.IsNullOrEmpty(_selectSql))
                _selectSql = TransformStatement(_selectSql);
            if (!string.IsNullOrEmpty(_orderbySqlCombine))
                _orderbySqlCombine = TransformStatement(_orderbySqlCombine);

            // Construct the take skip sql.
            ConstructTakeSkipSql(skipExpression, takeExpression);

            // Create a new type collection instance.
            TDataEntity[] data = null;
            if (_connectionType != ConnectionContext.ConnectionType.None)
                data = ExecuteQuery(GetSqlStatement());
            else
                data = new TDataEntity[] { new TDataEntity() };

            // Copy the IEnumerable data type to an IQueryable.
            IQueryable<TDataEntity> queryable = data.AsQueryable<TDataEntity>();
            
            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            ExpressionTreeBuilderModifier<TDataEntity> treeCopier = new ExpressionTreeBuilderModifier<TDataEntity>(queryable);
            Expression newExpressionTree = treeCopier.CopyAndModify(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (IsEnumerable)
                return queryable.Provider.CreateQuery(newExpressionTree);
            else
                return queryable.Provider.Execute(newExpressionTree);
        }

        /// <summary>
        /// Get the sql from the expression tree.
        /// </summary>
        /// <returns>The sql statement.</returns>/returns>
        public string GetSqlStatement()
        {
            if (String.IsNullOrEmpty(_takeSkipSql))
                return (
                    (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + " " +
                    "FROM " + _tableName + " " +
                    (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                    (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine)
                    ).Trim();
            else
                return _takeSkipSql;
        }

        /// <summary>
        /// Execute the sql query.
        /// </summary>
        /// <param name="queryText">The query to execute.</param>
        /// <returns>The collection data enity objects.</returns>
        public TDataEntity[] ExecuteQuery(string queryText)
        {
            DataTable table = null;

            // Create a connection for the type.
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.SqliteConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, _connectionDataType, null);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, _connectionDataType, null);
                    break;

                default:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, _connectionString, true, _connectionDataType, null);
                    break;
            }

            // Has data been found.
            if (table != null)
            {
                // Translate the data table to the entity type.
                AnonymousTypeFunction type = new AnonymousTypeFunction();
                return type.Translator<TDataEntity>(table);
            }
            else
                return new TDataEntity[] { new TDataEntity() };
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Is the query over a data source.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>True if over a data source else false.</returns>
        private bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance,
            // expression is of type ConstantExpression, not MethodCallExpression.
            return (expression is MethodCallExpression);
        }

        /// <summary>
        /// Construct the take skip sql query.
        /// </summary>
        /// <param name="skipExpression">The skip expression.</param>
        /// <param name="takeExpression">The take expression.</param>
        private void ConstructTakeSkipSql(MethodCallExpression skipExpression, MethodCallExpression takeExpression)
        {
            // Search for a Take() and Skip() method call.
            if ((skipExpression != null) && (takeExpression != null))
            {
                // Make sure an order by clause has been specified.
                if (String.IsNullOrEmpty(_orderbySqlCombine))
                    throw new Exception("No ORDER BY clause have been specified.");

                string skipSQL = skipExpression.Arguments[1].ToString();
                string takeSQL = takeExpression.Arguments[1].ToString();
                ConstructTakeSkipSql(skipSQL, takeSQL);
            }
            else if (skipExpression != null)
            {
                // Make sure an order by clause has been specified.
                if (String.IsNullOrEmpty(_orderbySqlCombine))
                    throw new Exception("No ORDER BY clause have been specified.");

                string skipSQL = skipExpression.Arguments[1].ToString();
                ConstructTakeSkipSql(skipSQL, string.Empty);
            }
            else if (takeExpression != null)
            {
                string takeSQL = takeExpression.Arguments[1].ToString();
                ConstructTakeSkipSql(string.Empty, takeSQL);
            }
            else
            {
                if (_firstSql == "1")
                    ConstructTakeSkipSql(string.Empty, "1");
                else
                    ConstructTakeSkipSql(string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// Construct the take skip sql query.
        /// </summary>
        /// <param name="skipSQL">The skip query</param>
        /// <param name="takeSQL">The take query</param>
        private void ConstructTakeSkipSql(string skipSQL, string takeSQL)
        {
            // Search for a Take() and Skip() method call.
            if ((!String.IsNullOrEmpty(skipSQL)) && (!String.IsNullOrEmpty(takeSQL)))
            {
                if (_firstSql == "1")
                    takeSQL = "1";

                // Create a query for the data type.
                switch (_connectionDataType)
                {
                    case ConnectionContext.ConnectionDataType.SqlDataType:
                    case ConnectionContext.ConnectionDataType.OracleDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT t0.*" : "SELECT t0." + _selectSql.Replace(", ", ", t0.")) + " " +
                            "FROM (" +
                                (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + ", ROW_NUMBER() OVER(" + (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + ") AS ROW_NUM" + " " +
                                "FROM " + _tableName + " " +
                                (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            ") AS t0" + " " +
                            "WHERE t0.ROW_NUM BETWEEN (" + skipSQL + " + 1) AND (" + skipSQL + " + " + takeSQL + ")" + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY t0." + _orderbySqlCombine.Replace(", ", ", t0."))
                            ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                    case ConnectionContext.ConnectionDataType.MySqlDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + " " +
                            "LIMIT " + takeSQL + " OFFSET " + skipSQL
                            ).Trim();
                        break;

                    default:
                         _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT t0.*" : "SELECT t0." + _selectSql.Replace(", ", ", t0.")) + " " +
                            "FROM (" +
                                (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + ", ROW_NUMBER() OVER(" + (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + ") AS ROW_NUM" + " " +
                                "FROM " + _tableName + " " +
                                (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            ") AS t0" + " " +
                            "WHERE t0.ROW_NUM BETWEEN (" + skipSQL + " + 1) AND (" + skipSQL + " + " + takeSQL + ")" + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY t0." + _orderbySqlCombine.Replace(", ", ", t0."))
                            ).Trim();
                        break;
                }
            }
            else if (!String.IsNullOrEmpty(skipSQL))
            {
                takeSQL = _firstSql;

                // Create a query for the data type.
                switch (_connectionDataType)
                {
                    case ConnectionContext.ConnectionDataType.SqlDataType:
                    case ConnectionContext.ConnectionDataType.OracleDataType:
                        _takeSkipSql = (
                           (String.IsNullOrEmpty(_selectSql) ? "SELECT t0.*" : "SELECT t0." + _selectSql.Replace(", ", ", t0.")) + " " +
                           "FROM (" +
                               (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + ", ROW_NUMBER() OVER(" + (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + ") AS ROW_NUM" + " " +
                               "FROM " + _tableName + " " +
                               (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                           ") AS t0" + " " +
                           "WHERE " + (String.IsNullOrEmpty(takeSQL) ? "(t0.ROW_NUM > " + skipSQL + ")" : "t0.ROW_NUM BETWEEN (" + skipSQL + " + 1) AND (" + skipSQL + " + " + takeSQL + ")") + " " +
                           (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY t0." + _orderbySqlCombine.Replace(", ", ", t0."))
                           ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                    case ConnectionContext.ConnectionDataType.MySqlDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + " " +
                            "LIMIT ALL OFFSET " + skipSQL
                            ).Trim();
                        break;

                    default:
                        _takeSkipSql = (
                           (String.IsNullOrEmpty(_selectSql) ? "SELECT t0.*" : "SELECT t0." + _selectSql.Replace(", ", ", t0.")) + " " +
                           "FROM (" +
                               (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + ", ROW_NUMBER() OVER(" + (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + ") AS ROW_NUM" + " " +
                               "FROM " + _tableName + " " +
                               (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                           ") AS t0" + " " +
                           "WHERE " + (String.IsNullOrEmpty(takeSQL) ? "(t0.ROW_NUM > " + skipSQL + ")" : "t0.ROW_NUM BETWEEN (" + skipSQL + " + 1) AND (" + skipSQL + " + " + takeSQL + ")") + " " +
                           (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY t0." + _orderbySqlCombine.Replace(", ", ", t0."))
                           ).Trim();
                        break;
                }   
            }
            else if (!String.IsNullOrEmpty(takeSQL))
            {
                if (_firstSql == "1")
                    takeSQL = "1";

                // Create a query for the data type.
                switch (_connectionDataType)
                {
                    case ConnectionContext.ConnectionDataType.SqlDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT TOP(" + takeSQL + ") *" : "SELECT TOP(" + takeSQL + ") " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine)
                            ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.OracleDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "WHERE (ROWNUM <= " + takeSQL + ")" : "WHERE " + _whereSql + " AND (ROWNUM <= " + takeSQL + ")") + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine)
                            ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + " " +
                            "LIMIT " + takeSQL
                            ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.MySqlDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT *" : "SELECT " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine) + " " +
                            "LIMIT " + takeSQL
                            ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.AccessDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT TOP " + takeSQL + " *" : "SELECT TOP " + takeSQL + " " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine)
                            ).Trim();
                        break;

                    case ConnectionContext.ConnectionDataType.ScxDataType:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT TOP (" + takeSQL + ") *" : "SELECT TOP (" + takeSQL + ") " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine)
                            ).Trim();
                        break;

                    default:
                        _takeSkipSql = (
                            (String.IsNullOrEmpty(_selectSql) ? "SELECT (" + takeSQL + ") *" : "SELECT (" + takeSQL + ") " + _selectSql) + " " +
                            "FROM " + _tableName + " " +
                            (String.IsNullOrEmpty(_whereSql) ? "" : "WHERE " + _whereSql) + " " +
                            (String.IsNullOrEmpty(_orderbySqlCombine) ? "" : "ORDER BY " + _orderbySqlCombine)
                            ).Trim();
                        break;
                }
            }
            else
                _takeSkipSql = string.Empty;
        }

        /// <summary>
        /// Transforms an sql column statement to the default structure.
        /// </summary>
        /// <param name="statement">The current statement to transform</param>
        /// <returns>The transform statement</returns>
        private string TransformStatement(string statement)
        {
            return DataTypeConversion.GetSqlConversionDataTypeNoContainer(_connectionDataType, statement);
        }
        #endregion

        #endregion

        /// <summary>
        /// Expression tree modifier.
        /// </summary>
        /// <typeparam name="TData">The return data type.</typeparam>
        private sealed class ExpressionTreeBuilderModifier<TData> : Nequeo.Linq.ExpressionVisitor
        {
            #region Expression Tree Builder Modifier

            #region Constructors
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="query">The query expression.</param>
            public ExpressionTreeBuilderModifier(IQueryable<TData> query)
            {
                this._query = query;
            }
            #endregion

            #region Fields
            private IQueryable<TData> _query;
            #endregion

            #region Methods
            /// <summary>
            /// Copy and modify the expression.
            /// </summary>
            /// <param name="expression">The expression tree.</param>
            /// <returns>The new expression.</returns>
            public Expression CopyAndModify(Expression expression)
            {
                return this.Visit(expression);
            }

            /// <summary>
            /// Protected override visit contant expression.
            /// </summary>
            /// <param name="constant">The constant expression.</param>
            /// <returns>The new expression.</returns>
            protected override Expression VisitConstant(ConstantExpression constant)
            {
                // Replace the constant QueryableTerraServerData arg with the queryable Place collection.
                if (constant.Type == typeof(QueryableProvider<TData>))
                    return Expression.Constant(this._query);
                else
                    return constant;
            }

            /// <summary>
            /// The override visit method call.
            /// </summary>
            /// <param name="m">The mehtod call expression</param>
            /// <returns>The new expression.</returns>
            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                // If the Skip method is encounted then
                // replace the constant skip value to zero
                if (m.Method.Name == "Skip")
                {
                    Expression obj = this.Visit(m.Object);
                    IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
                    Expression[] argsCol = args.ToArray();
                    Expression argsNew = Expression.Constant(0);
                    if (obj != m.Object || args != m.Arguments)
                    {
                        return Expression.Call(obj, m.Method, argsCol[0], argsNew);
                    }
                    return m;
                }
                else
                {
                    Expression obj = this.Visit(m.Object);
                    IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
                    if (obj != m.Object || args != m.Arguments)
                    {
                        return Expression.Call(obj, m.Method, args);
                    }
                    return m;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <param name="queryable"></param>
            private void Builder(Expression expression, IQueryable<TDataEntity> queryable)
            {
                IQueryable queryableNew = queryable;

                // Find the call to Where() and get the lambda expression predicate.
                InnermostWhereFinder whereFinder = new InnermostWhereFinder();
                MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);

                if (whereExpression != null)
                {
                    LambdaExpression whereLambdaExpressionNew = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;
                    queryableNew = queryableNew.Provider.CreateQuery(
                        Expression.Call(
                            typeof(Queryable), "Where",
                            new Type[] { queryableNew.ElementType },
                            queryableNew.Expression, Expression.Quote(whereLambdaExpressionNew)));
                }

                // Creating a parameter for an expression tree.
                queryableNew = queryableNew.Provider.CreateQuery(
                    Expression.Call(
                            typeof(Queryable), "Skip",
                            new Type[] { queryableNew.ElementType },
                            queryableNew.Expression, Expression.Constant(0)));
            }
            #endregion

            #endregion
        }
    }
}
