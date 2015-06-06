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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Nequeo.Linq
{
    /// <summary>
    /// Expression model.
    /// </summary>
    public class ExpressionModel
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the property type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the operand.
        /// </summary>
        public Nequeo.Linq.ExpressionOperandBaseType Operand { get; set; }
    }

    /// <summary>
    /// Expression provider.
    /// </summary>
    public class Expressions
    {
        /// <summary>
        /// Expression provider.
        /// </summary>
        public Expressions()
        { }

        /// <summary>
        /// Create the lambda expression.
        /// </summary>
        /// <typeparam name="T">The expression parameter type.</typeparam>
        /// <param name="expressions">The expressions to match against the entity.</param>
        /// <param name="operands">The operands used to seperate each property expression.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Func<T, bool>> CreateLambdaExpression<T>(ExpressionModel[] expressions, Nequeo.Linq.ExpressionOperandBaseType[] operands)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(T), "u");

            // Create the expression.
            Expression result = CreateExpression<T>(expressions, operands, paramExpr);

            // This expression represents a lambda expression 
            Expression<Func<T, bool>> predicate =
                Expression<Func<T, bool>>.Lambda<Func<T, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create and compile the lambda expression.
        /// </summary>
        /// <typeparam name="T">The expression parameter type.</typeparam>
        /// <param name="expressions">The expressions to match against the entity.</param>
        /// <param name="operands">The operands used to seperate each property expression.</param>
        /// <returns>The lambda expression.</returns>
        public Func<T, bool> CompileLambdaExpression<T>(ExpressionModel[] expressions, Nequeo.Linq.ExpressionOperandBaseType[] operands)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(T), "u");

            // Create the expression.
            Expression result = CreateExpression<T>(expressions, operands, paramExpr);

            // This expression represents a lambda expression 
            Expression<Func<T, bool>> predicate =
                Expression<Func<T, bool>>.Lambda<Func<T, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate.Compile();
        }

        /// <summary>
        /// Create the property member access expression.
        /// </summary>
        /// <typeparam name="T">The type the property belongs to.</typeparam>
        /// <param name="propertyName">The property name of the property within the type 'T'.</param>
        /// <returns>The property member access expression.</returns>
        public Expression CreatePropertyAccessExpression<T>(string propertyName)
        {
            // A parameter for the lambda expression.
            ParameterExpression param = Expression.Parameter(typeof(T), "u");

            // Get the property info
            PropertyInfo property = typeof(T).GetProperty(propertyName);

            // Make the member access expression.
            Expression expression = Expression.MakeMemberAccess(param, property);
            return expression;
        }

        /// <summary>
        /// Create the method member call expression.
        /// </summary>
        /// <typeparam name="T">The type the method belongs to.</typeparam>
        /// <param name="methodName">The name of the method within the type 'T'.</param>
        /// <param name="arguments">An array of System.Linq.Expressions.Expression objects to use to populate the System.Linq.Expressions.MethodCallExpression.Arguments collection.</param>
        /// <returns>The method call expression.</returns>
        public Expression CreateMethodCallExpression<T>(string methodName, params Expression[] arguments)
        {
            // Make the call expresion.
            Expression expression = Expression.Call(typeof(T), methodName, null, arguments);
            return expression;
        }

        /// <summary>
        /// Create the method lambda expression from the property access and the method info.
        /// </summary>
        /// <typeparam name="T">The delegate type used in the expression.</typeparam>
        /// <typeparam name="P">The type the property access expression belongs to.</typeparam>
        /// <param name="propertyAccessExpression">The property member access expression.</param>
        /// <param name="methodInfo">The method info for the method to call associated with the property type within the 'P' type.</param>
        /// <param name="arguments">An array of System.Linq.Expressions.Expression objects to use to populate the System.Linq.Expressions.MethodCallExpression.Arguments collection.</param>
        /// <returns>The lambda expression.</returns>
        /// <remarks>
        /// The type 'T' must be a System.Deleate type, type 'P' is the type the property belongs to; e.g. Func &lt; P, bool &gt; where the first parameter
        /// is the type the property belongs to and the last parameter type is the method return type.
        /// </remarks>
        public Expression<T> CreateMethodLambdaExpression<T, P>(Expression propertyAccessExpression, MethodInfo methodInfo, params Expression[] arguments)
        {
            // A parameter for the lambda expression.
            ParameterExpression param = Expression.Parameter(typeof(P), "u");

            // The method call on the property.
            MethodCallExpression methodCall = Expression.Call(propertyAccessExpression, methodInfo, arguments);

            // This expression represents a lambda expression 
            Expression<T> predicate = Expression<T>.Lambda<T>(methodCall, param);
            return predicate;
        }

        /// <summary>
        /// Create a lambda expression from the expression body and parameters.
        /// </summary>
        /// <typeparam name="T">The delegate type used in the expression.</typeparam>
        /// <param name="body">An System.Linq.Expressions.Expression to set the System.Linq.Expressions.LambdaExpression.Body property equal to.</param>
        /// <param name="parameters">An array of System.Linq.Expressions.ParameterExpression objects to use to populate the System.Linq.Expressions.LambdaExpression.Parameters collection.</param>
        /// <returns>An System.Linq.Expressions.Expression&lt;TDelegate&gt; that has the System.Linq.Expressions.Expression.NodeType
        /// property equal to System.Linq.Expressions.ExpressionType.Lambda and the System.Linq.Expressions.LambdaExpression.Body
        /// and System.Linq.Expressions.LambdaExpression.Parameters properties set to the specified values.</returns>
        public Expression<T> CreateLambdaExpression<T>(Expression body, params ParameterExpression[] parameters)
        {
            // This expression represents a lambda expression 
            Expression<T> predicate = Expression<T>.Lambda<T>(body, parameters);
            return predicate;
        }
        
        /// <summary>
        /// Create a lambda expression from the method and constructor.
        /// </summary>
        /// <typeparam name="T">The delegate type used in the expression.</typeparam>
        /// <param name="methodInfo">The method info used to construct the expression.</param>
        /// <param name="constructorInfo">The constructor info used when the method is an instance method; else if null then the method must be a static method.</param>
        /// <returns>The lambda expression.</returns>
        /// <remarks>
        /// If the method is an instance method then type 'T' must be a System.Deleate type; e.g. Func &lt; int, int, long, long, bool &gt; where the first
        /// two parameters are the constructor parameters the next two parameters are the method parameters and the last parameter is the method return type.
        /// if the method is a static method then type 'T' must be a System.Deleate type; e.g. Func &lt; long, long, bool &gt; where the first two parameters are
        /// the method parameters and the last parameter is the method return type.
        /// </remarks>
        public Expression<T> CreateLambdaExpression<T>(MethodInfo methodInfo, ConstructorInfo constructorInfo = null)
        {
            ParameterExpression[] parmsConst = null;
            ParameterExpression[] parmsMethod = null;

            // Get method parameters.
            ParameterInfo[] methodParms = methodInfo.GetParameters();

            // Create the instance lambda.
            if (methodParms != null && methodParms.Length > 0)
            {
                // Create the constructor parameters.
                parmsMethod = new ParameterExpression[methodParms.Length];
                for (int i = 0; i < methodParms.Length; i++)
                {
                    // Assign the parms.
                    parmsMethod[i] = Expression.Parameter(methodParms[i].ParameterType, "mp" + i.ToString());
                }
            }

            // Constructor exist.
            if (constructorInfo != null)
            {
                // Get method parameters.
                ParameterInfo[] constParms = constructorInfo.GetParameters();

                // Create the instance lambda.
                if (constParms != null && constParms.Length > 0)
                {
                    // Create the constructor parameters.
                    parmsConst = new ParameterExpression[constParms.Length];
                    for (int i = 0; i < constParms.Length; i++)
                    {
                        // Assign the parms.
                        parmsConst[i] = Expression.Parameter(constParms[i].ParameterType, "cp" + i.ToString());
                    }
                }

                // Parmaters exist.
                if (parmsConst != null)
                {
                    // Parmaters exist.
                    if (parmsMethod != null)
                    {
                        // Combine the parameters.
                        IEnumerable<ParameterExpression> parms = parmsConst.Concat(parmsMethod);

                        // Create the method call.
                        MethodCallExpression call = Expression.Call(Expression.New(constructorInfo, parmsConst), methodInfo, parmsMethod);

                        // This expression represents a lambda expression 
                        Expression<T> predicate = Expression<T>.Lambda<T>(call, parms);
                        return predicate;
                    }
                    else
                    {
                        // Create the method call.
                        MethodCallExpression call = Expression.Call(Expression.New(constructorInfo, parmsConst), methodInfo);

                        // This expression represents a lambda expression 
                        Expression<T> predicate = Expression<T>.Lambda<T>(call, parmsConst);
                        return predicate;
                    }
                }
                else
                {
                    // Parmaters exist.
                    if (parmsMethod != null)
                    {
                        // Create the method call.
                        MethodCallExpression call = Expression.Call(Expression.New(constructorInfo), methodInfo, parmsMethod);

                        // This expression represents a lambda expression 
                        Expression<T> predicate = Expression<T>.Lambda<T>(call, parmsMethod);
                        return predicate;
                    }
                    else
                    {
                        // Create the method call.
                        MethodCallExpression call = Expression.Call(Expression.New(constructorInfo), methodInfo);

                        // This expression represents a lambda expression 
                        Expression<T> predicate = Expression<T>.Lambda<T>(call);
                        return predicate;
                    }
                }
            }
            else
            {
                // Parmaters exist.
                if (parmsMethod != null)
                {
                    // Create the method call.
                    MethodCallExpression call = Expression.Call(null, methodInfo, parmsMethod);

                    // This expression represents a lambda expression 
                    Expression<T> predicate = Expression<T>.Lambda<T>(call, parmsMethod);
                    return predicate;
                }
                else
                {
                    // Create the method call.
                    MethodCallExpression call = Expression.Call(null, methodInfo);

                    // This expression represents a lambda expression 
                    Expression<T> predicate = Expression<T>.Lambda<T>(call);
                    return predicate;
                }
            }
        }

        /// <summary>
        /// Get the sql expression operand string.
        /// </summary>
        /// <param name="exp">The expression to return.</param>
        /// <returns>The string expression.</returns>
        public static string GetSqlExpression(Nequeo.Linq.ExpressionOperandType exp)
        {
            switch (exp)
            {
                case Nequeo.Linq.ExpressionOperandType.Add:
                    return "+";
                case Nequeo.Linq.ExpressionOperandType.Between:
                    return "BETWEEN";
                case Nequeo.Linq.ExpressionOperandType.Divide:
                    return "/";
                case Nequeo.Linq.ExpressionOperandType.Equal:
                    return "=";
                case Nequeo.Linq.ExpressionOperandType.GreaterThan:
                    return ">";
                case Nequeo.Linq.ExpressionOperandType.GreaterThanOrEqual:
                    return ">=";
                case Nequeo.Linq.ExpressionOperandType.In:
                    return "IN";
                case Nequeo.Linq.ExpressionOperandType.IsFalse:
                    return "FALSE";
                case Nequeo.Linq.ExpressionOperandType.IsNotNull:
                    return "IS NOT NULL";
                case Nequeo.Linq.ExpressionOperandType.IsNull:
                    return "IS NULL";
                case Nequeo.Linq.ExpressionOperandType.IsTrue:
                    return "TRUE";
                case Nequeo.Linq.ExpressionOperandType.LessThan:
                    return "<";
                case Nequeo.Linq.ExpressionOperandType.LessThanOrEqual:
                    return "<=";
                case Nequeo.Linq.ExpressionOperandType.Like:
                    return "LIKE";
                case Nequeo.Linq.ExpressionOperandType.LikeEscape:
                    return "LIKE ESCAPE";
                case Nequeo.Linq.ExpressionOperandType.Multiply:
                    return "*";
                case Nequeo.Linq.ExpressionOperandType.NotBetween:
                    return "NOT BETWEEN";
                case Nequeo.Linq.ExpressionOperandType.NotEqual:
                    return "<>";
                case Nequeo.Linq.ExpressionOperandType.NotIn:
                    return "NOT IN";
                case Nequeo.Linq.ExpressionOperandType.NotLike:
                    return "NOT LIKE";
                case Nequeo.Linq.ExpressionOperandType.NotLikeEscape:
                    return "NOT LIKE ESCAPE";
                case Nequeo.Linq.ExpressionOperandType.Subtract:
                    return "-";
                case Nequeo.Linq.ExpressionOperandType.And:
                    return "AND";
                case Nequeo.Linq.ExpressionOperandType.Or:
                    return "OR";
                case Nequeo.Linq.ExpressionOperandType.AndAlso:
                    return "AND";
                case Nequeo.Linq.ExpressionOperandType.OrElse:
                    return "OR";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get the expression operand string.
        /// </summary>
        /// <param name="exp">The expression to return.</param>
        /// <returns>The string expression.</returns>
        public static string GetExpression(Nequeo.Linq.ExpressionOperandType exp)
        {
            switch (exp)
            {
                case Nequeo.Linq.ExpressionOperandType.Add:
                    return "+";
                case Nequeo.Linq.ExpressionOperandType.Between:
                    return "Between";
                case Nequeo.Linq.ExpressionOperandType.Divide:
                    return "/";
                case Nequeo.Linq.ExpressionOperandType.Equal:
                    return "==";
                case Nequeo.Linq.ExpressionOperandType.GreaterThan:
                    return ">";
                case Nequeo.Linq.ExpressionOperandType.GreaterThanOrEqual:
                    return ">=";
                case Nequeo.Linq.ExpressionOperandType.In:
                    return "In";
                case Nequeo.Linq.ExpressionOperandType.IsFalse:
                    return "false";
                case Nequeo.Linq.ExpressionOperandType.IsNotNull:
                    return "IsNotNull";
                case Nequeo.Linq.ExpressionOperandType.IsNull:
                    return "IsNull";
                case Nequeo.Linq.ExpressionOperandType.IsTrue:
                    return "true";
                case Nequeo.Linq.ExpressionOperandType.LessThan:
                    return "<";
                case Nequeo.Linq.ExpressionOperandType.LessThanOrEqual:
                    return "<=";
                case Nequeo.Linq.ExpressionOperandType.Like:
                    return "Like";
                case Nequeo.Linq.ExpressionOperandType.LikeEscape:
                    return "LikeEscape";
                case Nequeo.Linq.ExpressionOperandType.Multiply:
                    return "*";
                case Nequeo.Linq.ExpressionOperandType.NotBetween:
                    return "NotBetween";
                case Nequeo.Linq.ExpressionOperandType.NotEqual:
                    return "!=";
                case Nequeo.Linq.ExpressionOperandType.NotIn:
                    return "NotIn";
                case Nequeo.Linq.ExpressionOperandType.NotLike:
                    return "NotLike";
                case Nequeo.Linq.ExpressionOperandType.NotLikeEscape:
                    return "NotLikeEscape";
                case Nequeo.Linq.ExpressionOperandType.Subtract:
                    return "-";
                case Nequeo.Linq.ExpressionOperandType.And:
                    return "&";
                case Nequeo.Linq.ExpressionOperandType.Or:
                    return "|";
                case Nequeo.Linq.ExpressionOperandType.AndAlso:
                    return "&&";
                case Nequeo.Linq.ExpressionOperandType.OrElse:
                    return "||";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get the expression type.
        /// </summary>
        /// <param name="exp">The expression to return.</param>
        /// <returns>The expression type.</returns>
        public static ExpressionType GetExpressionType(Nequeo.Linq.ExpressionOperandType exp)
        {
            switch (exp)
            {
                case Nequeo.Linq.ExpressionOperandType.Add:
                    return ExpressionType.Add;
                case Nequeo.Linq.ExpressionOperandType.Divide:
                    return ExpressionType.Divide;
                case Nequeo.Linq.ExpressionOperandType.Equal:
                    return ExpressionType.Equal;
                case Nequeo.Linq.ExpressionOperandType.GreaterThan:
                    return ExpressionType.GreaterThan;
                case Nequeo.Linq.ExpressionOperandType.GreaterThanOrEqual:
                    return ExpressionType.GreaterThanOrEqual;
                case Nequeo.Linq.ExpressionOperandType.IsFalse:
                    return ExpressionType.IsFalse;
                case Nequeo.Linq.ExpressionOperandType.IsTrue:
                    return ExpressionType.IsTrue;
                case Nequeo.Linq.ExpressionOperandType.LessThan:
                    return ExpressionType.LessThan;
                case Nequeo.Linq.ExpressionOperandType.LessThanOrEqual:
                    return ExpressionType.LessThanOrEqual;
                case Nequeo.Linq.ExpressionOperandType.Multiply:
                    return ExpressionType.Multiply;
                case Nequeo.Linq.ExpressionOperandType.NotEqual:
                    return ExpressionType.NotEqual;
                case Nequeo.Linq.ExpressionOperandType.Subtract:
                    return ExpressionType.Subtract;
                case Nequeo.Linq.ExpressionOperandType.And:
                    return ExpressionType.And;
                case Nequeo.Linq.ExpressionOperandType.Or:
                    return ExpressionType.Or;
                case Nequeo.Linq.ExpressionOperandType.AndAlso:
                    return ExpressionType.AndAlso;
                case Nequeo.Linq.ExpressionOperandType.OrElse:
                    return ExpressionType.OrElse;
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.ToString()));
            }
        }

        /// <summary>
        /// Get the expression type.
        /// </summary>
        /// <param name="exp">The expression to return.</param>
        /// <returns>The expression type.</returns>
        public static ExpressionType GetExpressionType(Nequeo.Linq.ExpressionOperandBaseType exp)
        {
            switch (exp)
            {
                case Nequeo.Linq.ExpressionOperandBaseType.Add:
                    return ExpressionType.Add;
                case Nequeo.Linq.ExpressionOperandBaseType.Divide:
                    return ExpressionType.Divide;
                case Nequeo.Linq.ExpressionOperandBaseType.Equal:
                    return ExpressionType.Equal;
                case Nequeo.Linq.ExpressionOperandBaseType.GreaterThan:
                    return ExpressionType.GreaterThan;
                case Nequeo.Linq.ExpressionOperandBaseType.GreaterThanOrEqual:
                    return ExpressionType.GreaterThanOrEqual;
                case Nequeo.Linq.ExpressionOperandBaseType.IsFalse:
                    return ExpressionType.IsFalse;
                case Nequeo.Linq.ExpressionOperandBaseType.IsTrue:
                    return ExpressionType.IsTrue;
                case Nequeo.Linq.ExpressionOperandBaseType.LessThan:
                    return ExpressionType.LessThan;
                case Nequeo.Linq.ExpressionOperandBaseType.LessThanOrEqual:
                    return ExpressionType.LessThanOrEqual;
                case Nequeo.Linq.ExpressionOperandBaseType.Multiply:
                    return ExpressionType.Multiply;
                case Nequeo.Linq.ExpressionOperandBaseType.NotEqual:
                    return ExpressionType.NotEqual;
                case Nequeo.Linq.ExpressionOperandBaseType.Subtract:
                    return ExpressionType.Subtract;
                case Nequeo.Linq.ExpressionOperandBaseType.And:
                    return ExpressionType.And;
                case Nequeo.Linq.ExpressionOperandBaseType.Or:
                    return ExpressionType.Or;
                case Nequeo.Linq.ExpressionOperandBaseType.AndAlso:
                    return ExpressionType.AndAlso;
                case Nequeo.Linq.ExpressionOperandBaseType.OrElse:
                    return ExpressionType.OrElse;
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.ToString()));
            }
        }

        /// <summary>
        /// Get the expression operand string.
        /// </summary>
        /// <param name="exp">The expression to return.</param>
        /// <returns>The string expression.</returns>
        public static string GetExpression(Nequeo.Linq.ExpressionOperandBaseType exp)
        {
            switch (exp)
            {
                case Nequeo.Linq.ExpressionOperandBaseType.Add:
                    return "+";
                case Nequeo.Linq.ExpressionOperandBaseType.Divide:
                    return "/";
                case Nequeo.Linq.ExpressionOperandBaseType.Equal:
                    return "==";
                case Nequeo.Linq.ExpressionOperandBaseType.GreaterThan:
                    return ">";
                case Nequeo.Linq.ExpressionOperandBaseType.GreaterThanOrEqual:
                    return ">=";
                case Nequeo.Linq.ExpressionOperandBaseType.IsFalse:
                    return "false";
                case Nequeo.Linq.ExpressionOperandBaseType.IsTrue:
                    return "true";
                case Nequeo.Linq.ExpressionOperandBaseType.LessThan:
                    return "<";
                case Nequeo.Linq.ExpressionOperandBaseType.LessThanOrEqual:
                    return "<=";
                case Nequeo.Linq.ExpressionOperandBaseType.Multiply:
                    return "*";
                case Nequeo.Linq.ExpressionOperandBaseType.NotEqual:
                    return "!=";
                case Nequeo.Linq.ExpressionOperandBaseType.Subtract:
                    return "-";
                case Nequeo.Linq.ExpressionOperandBaseType.And:
                    return "&";
                case Nequeo.Linq.ExpressionOperandBaseType.Or:
                    return "|";
                case Nequeo.Linq.ExpressionOperandBaseType.AndAlso:
                    return "&&";
                case Nequeo.Linq.ExpressionOperandBaseType.OrElse:
                    return "||";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Create the expression.
        /// </summary>
        /// <typeparam name="T">The expression parameter type.</typeparam>
        /// <param name="expressions">The expressions to match against the entity.</param>
        /// <param name="operands">The operands used to seperate each property expression.</param>
        /// <param name="paramExpr">A parameter for the lambda expression.</param>
        /// <returns>The expression.</returns>
        private Expression CreateExpression<T>(ExpressionModel[] expressions, Nequeo.Linq.ExpressionOperandBaseType[] operands, ParameterExpression paramExpr)
        {
            Type entityType = typeof(T);

            // The resulting expression.
            Expression result = null;
            Expression[] created = new Expression[expressions.Length];
            Expression swap = null;

            // If more than one expression.
            if (expressions.Length > 1)
            {
                // For each query create the expression.
                for (int i = 0; i < expressions.Length; i++)
                {
                    // Create the initial expression.
                    PropertyInfo property = entityType.GetProperty(expressions[i].Name);
                    Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);
                    created[i] = CreateExpression(expressions[i], propertyMember, expressions[i].Value, expressions[i].Type);
                }

                // Get the first two exressions.
                swap = CreateCombinedExpression(created[0], created[1], operands[0]);

                // For each query created.
                for (int i = 2; i < created.Length; i++)
                {
                    // Get the new swap.
                    swap = CreateCombinedExpression(swap, created[i], operands[i - 1]);
                }

                // Assign the final expression result.
                result = swap;
            }
            else
            {
                // Create the initial expression.
                PropertyInfo property = entityType.GetProperty(expressions[0].Name);
                Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);
                result = CreateExpression(expressions[0], propertyMember, expressions[0].Value, expressions[0].Type);
            }

            // Return the expression.
            return result;
        }

        /// <summary>
        /// Create the expression.
        /// </summary>
        /// <param name="expression">The current expression.</param>
        /// <param name="propertyMember">The current property member.</param>
        /// <param name="value">The value of the property member.</param>
        /// <param name="valueType">The type of the property member.</param>
        /// <returns>The new expression.</returns>
        private Expression CreateExpression(ExpressionModel expression, Expression propertyMember, object value, Type valueType)
        {
            switch (expression.Operand)
            {
                case ExpressionOperandBaseType.AndAlso:
                    Expression leftAndAlso = Expression.Equal(propertyMember, Expression.Constant(value, valueType));
                    Expression rightAndAlso = Expression.Equal(propertyMember, Expression.Constant(value, valueType));
                    return Expression.AndAlso(leftAndAlso, rightAndAlso);

                case ExpressionOperandBaseType.Equal:
                    return Expression.Equal(propertyMember, Expression.Constant(value, valueType));

                case ExpressionOperandBaseType.GreaterThan:
                    return Expression.GreaterThan(propertyMember, Expression.Constant(value, valueType));

                case ExpressionOperandBaseType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(propertyMember, Expression.Constant(value, valueType));

                case ExpressionOperandBaseType.IsFalse:
                    return Expression.IsFalse(propertyMember);

                case ExpressionOperandBaseType.IsTrue:
                    return Expression.IsTrue(propertyMember);

                case ExpressionOperandBaseType.LessThan:
                    return Expression.LessThan(propertyMember, Expression.Constant(value, valueType));

                case ExpressionOperandBaseType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(propertyMember, Expression.Constant(value, valueType));

                case ExpressionOperandBaseType.NotEqual:
                    return Expression.NotEqual(propertyMember, Expression.Constant(value, valueType));

                case ExpressionOperandBaseType.OrElse:
                    Expression leftOrElse = Expression.Equal(propertyMember, Expression.Constant(value, valueType));
                    Expression rightOrElse = Expression.Equal(propertyMember, Expression.Constant(value, valueType));
                    return Expression.OrElse(leftOrElse, rightOrElse);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create the combined expression.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <param name="operand">The joining operand.</param>
        /// <returns>The combined expression.</returns>
        private Expression CreateCombinedExpression(Expression left, Expression right, Nequeo.Linq.ExpressionOperandBaseType operand)
        {
            switch (operand)
            {
                case ExpressionOperandBaseType.Add:
                    return Expression.Add(left, right);

                case ExpressionOperandBaseType.And:
                    return Expression.And(left, right);

                case ExpressionOperandBaseType.AndAlso:
                    return Expression.AndAlso(left, right);

                case ExpressionOperandBaseType.Divide:
                    return Expression.Divide(left, right);

                case ExpressionOperandBaseType.Equal:
                    return Expression.Equal(left, right);

                case ExpressionOperandBaseType.GreaterThan:
                    return Expression.GreaterThan(left, right);

                case ExpressionOperandBaseType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);

                case ExpressionOperandBaseType.IsFalse:
                    return Expression.IsFalse(left);

                case ExpressionOperandBaseType.IsTrue:
                    return Expression.IsTrue(left);

                case ExpressionOperandBaseType.LessThan:
                    return Expression.LessThan(left, right);

                case ExpressionOperandBaseType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);

                case ExpressionOperandBaseType.Multiply:
                    return Expression.Multiply(left, right);

                case ExpressionOperandBaseType.NotEqual:
                    return Expression.NotEqual(left, right);

                case ExpressionOperandBaseType.Or:
                    return Expression.Or(left, right);

                case ExpressionOperandBaseType.OrElse:
                    return Expression.OrElse(left, right);

                case ExpressionOperandBaseType.Subtract:
                    return Expression.Subtract(left, right);

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Expression opreand type.
    /// </summary>
    public enum ExpressionOperandType : int
    {
        /// <summary>
        /// An addition operation, such as a + b, without overflow checking, for numeric operands.
        /// </summary>
        Add = 0,
        /// <summary>
        /// A division operation, such as (a / b), for numeric operands.
        /// </summary>
        Divide = 1,
        /// <summary>
        /// A node that represents an equality comparison, such as (a == b).
        /// </summary>
        Equal = 2,
        /// <summary>
        /// A "greater than" comparison, such as (a > b).
        /// </summary>
        GreaterThan = 3,
        /// <summary>
        /// A "greater than or equal to" comparison, such as (a >= b).
        /// </summary>
        GreaterThanOrEqual = 4,
        /// <summary>
        /// A "less than" comparison, such as (a <![CDATA[<]]> b).
        /// </summary>
        LessThan = 5,
        /// <summary>
        /// A "less than or equal to" comparison, such as (a <![CDATA[<=]]> b).
        /// </summary>
        LessThanOrEqual = 6,
        /// <summary>
        /// A multiplication operation, such as (a * b), without overflow checking, for numeric operands.
        /// </summary>
        Multiply = 7,
        /// <summary>
        /// An inequality comparison, such as (a != b).
        /// </summary>
        NotEqual = 8,
        /// <summary>
        /// A subtraction operation, such as (a - b), without overflow checking, for numeric operands.
        /// </summary>
        Subtract = 9,
        /// <summary>
        /// A true condition value.
        /// </summary>
        IsTrue = 10,
        /// <summary>
        /// A false condition value.
        /// </summary>
        IsFalse = 11,
        /// <summary>
        /// A like operand, such as [ColumnName] Like '%data%'.
        /// </summary>
        Like = 12,
        /// <summary>
        /// A like operand, such as [ColumnName] Not Like '%data%'.
        /// </summary>
        NotLike = 13,
        /// <summary>
        /// A like operand, such as [ColumnName] Escape Like '%data%'.
        /// </summary>
        LikeEscape = 14,
        /// <summary>
        /// A like operand, such as [ColumnName] Not Escape Like '%data%'.
        /// </summary>
        NotLikeEscape = 15,
        /// <summary>
        /// A between operand, [ColumnName] Between 'data' And 'data'.
        /// </summary>
        Between = 16,
        /// <summary>
        /// A between operand, [ColumnName] Not Between 'data' And 'data'.
        /// </summary>
        NotBetween = 17,
        /// <summary>
        /// A in operand, [ColumnName] In ('data').
        /// </summary>
        In = 18,
        /// <summary>
        /// A in operand, [ColumnName] Not In ('data').
        /// </summary>
        NotIn = 19,
        /// <summary>
        /// A is null operand, [ColumnName] Is Null.
        /// </summary>
        IsNull = 20,
        /// <summary>
        /// A is null operand, [ColumnName] Is Not Null.
        /// </summary>
        IsNotNull = 21,
        /// <summary>
        /// A bitwise or logical AND operation, such as (a <![CDATA[&]]> b).
        /// </summary>
        And = 22,
        /// <summary>
        /// A bitwise or logical OR operation, such as (a | b).
        /// </summary>
        Or = 23,
        /// <summary>
        /// A conditional AND operation that evaluates the second operand only if the
        /// first operand evaluates to true. It corresponds to (a <![CDATA[&&]]> b)
        /// </summary>
        AndAlso = 24,
        /// <summary>
        /// A short-circuiting conditional OR operation, such as (a || b)
        /// </summary>
        OrElse = 25,
    }

    /// <summary>
    /// Expression opreand base type.
    /// </summary>
    public enum ExpressionOperandBaseType : int
    {
        /// <summary>
        /// An addition operation, such as a + b, without overflow checking, for numeric operands.
        /// </summary>
        Add = 0,
        /// <summary>
        /// A division operation, such as (a / b), for numeric operands.
        /// </summary>
        Divide = 1,
        /// <summary>
        /// A node that represents an equality comparison, such as (a == b).
        /// </summary>
        Equal = 2,
        /// <summary>
        /// A "greater than" comparison, such as (a > b).
        /// </summary>
        GreaterThan = 3,
        /// <summary>
        /// A "greater than or equal to" comparison, such as (a >= b).
        /// </summary>
        GreaterThanOrEqual = 4,
        /// <summary>
        /// A "less than" comparison, such as (a <![CDATA[<]]> b).
        /// </summary>
        LessThan = 5,
        /// <summary>
        /// A "less than or equal to" comparison, such as (a <![CDATA[<=]]> b).
        /// </summary>
        LessThanOrEqual = 6,
        /// <summary>
        /// A multiplication operation, such as (a * b), without overflow checking, for numeric operands.
        /// </summary>
        Multiply = 7,
        /// <summary>
        /// An inequality comparison, such as (a != b).
        /// </summary>
        NotEqual = 8,
        /// <summary>
        /// A subtraction operation, such as (a - b), without overflow checking, for numeric operands.
        /// </summary>
        Subtract = 9,
        /// <summary>
        /// A true condition value.
        /// </summary>
        IsTrue = 10,
        /// <summary>
        /// A false condition value.
        /// </summary>
        IsFalse = 11,
        /// <summary>
        /// A bitwise or logical AND operation, such as (a <![CDATA[&]]> b).
        /// </summary>
        And = 12,
        /// <summary>
        /// A bitwise or logical OR operation, such as (a | b).
        /// </summary>
        Or = 13,
        /// <summary>
        /// A conditional AND operation that evaluates the second operand only if the
        /// first operand evaluates to true. It corresponds to (a <![CDATA[&&]]> b)
        /// </summary>
        AndAlso = 14,
        /// <summary>
        /// A short-circuiting conditional OR operation, such as (a || b)
        /// </summary>
        OrElse = 15,
    }
}
