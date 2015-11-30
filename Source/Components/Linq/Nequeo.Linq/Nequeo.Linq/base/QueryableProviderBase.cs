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
using System.Collections.ObjectModel;

namespace Nequeo.Linq
{
	/// <summary>
	/// Generic queryable provider expression inspector.
	/// </summary>
	/// <typeparam name="T">The expression inspector type</typeparam>
	public abstract class QueryableProviderBase<T> : Nequeo.Linq.ExpressionVisitor, Nequeo.Linq.IQueryContext where T : class
	{
		#region Expression Evaluator Class

		/// <summary>
		/// Generic expression inspector for the Queryable Provider
		/// </summary>
		protected QueryableProviderBase()
		{
		}

		#region Private Fields
		private string _whereSql = string.Empty;
		private string _firstSql = string.Empty;
		private string _selectSql = string.Empty;
		private string _orderbySql = string.Empty;
		private string _orderbyDescendingSql = string.Empty;
		private string _thenbySql = string.Empty;
		private string _thenbyDescendingSql = string.Empty;
		private string _skipSql = string.Empty;
		private string _takeSql = string.Empty;
		private string _countSql = string.Empty;
		private string _anySql = string.Empty;
		private string _allSql = string.Empty;
		private string _lastSql = string.Empty;
		private string _singleSql = string.Empty;

		private String _sqlStatement = string.Empty;
		private String _sqlMethodStatement = string.Empty;
		private ParameterExpression _param = null;
		#endregion

		#region Public Methods
		/// <summary>
		/// Execution action handler, results method.
		/// </summary>
		/// <param name="expressionTreeModel">The expression inspectors tree model.</param>
		/// <returns>The queryable type result of the execution.</returns>
		protected abstract T[] ExecuteActionHandler(Nequeo.Model.ExpressionTreeModel expressionTreeModel);

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

			// Find the call to Any() and get the lambda expression predicate.
			InnermostAnyFinder anyFinder = new InnermostAnyFinder();
			MethodCallExpression anyExpression = anyFinder.GetInnermostAny(expression);

			// Find the call to All() and get the lambda expression predicate.
			InnermostAllFinder allFinder = new InnermostAllFinder();
			MethodCallExpression allExpression = allFinder.GetInnermostAll(expression);

			// Find the call to Count() and get the lambda expression predicate.
			InnermostCountFinder countFinder = new InnermostCountFinder();
			MethodCallExpression countExpression = countFinder.GetInnermostCount(expression);

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

			// Find the call to Last() and get the lambda expression predicate.
			InnermostLastFinder lastFinder = new InnermostLastFinder();
			MethodCallExpression lastExpression = lastFinder.GetInnermostLast(expression);

			// Find the call to Single() and get the lambda expression predicate.
			InnermostSingleFinder singleFinder = new InnermostSingleFinder();
			MethodCallExpression singleExpression = singleFinder.GetInnermostSingle(expression);

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

			// Get the any argument
			if (anyExpression != null)
			{
				_anySql = CreateAnyString(anyExpression);
			}

			// Get the all argument
			if (allExpression != null)
			{
				_allSql = CreateAllString(allExpression);
			}

			// Get the count argument
			if (countExpression != null)
			{
				_countSql = CreateCountString(countExpression);
			}

			// Get the skip argument
			if (skipExpression != null)
			{
				_skipSql = CreateSkipString(skipExpression);
			}

			// Get the take argument
			if (takeExpression != null)
			{
				_takeSql = CreateTakeString(takeExpression);
			}

			// Get the where arguments
			if (whereExpression != null)
			{
				LambdaExpression whereLambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;
				whereLambdaExpression = (LambdaExpression)Evaluator.PartialEval(whereLambdaExpression);
				_whereSql = CreateWhereString(whereLambdaExpression);
			}

			// Get the first arguments
			if (firstExpression != null)
			{
				if (firstExpression.Arguments.Count() > 1)
				{
					LambdaExpression firstLambdaExpression = (LambdaExpression)((UnaryExpression)(firstExpression.Arguments[1])).Operand;
					firstLambdaExpression = (LambdaExpression)Evaluator.PartialEval(firstLambdaExpression);
					_firstSql = CreateWhereString(firstLambdaExpression);
				}
			}

			// Get the last arguments
			if (lastExpression != null)
			{
				if (lastExpression.Arguments.Count() > 1)
				{
					LambdaExpression lastLambdaExpression = (LambdaExpression)((UnaryExpression)(lastExpression.Arguments[1])).Operand;
					lastLambdaExpression = (LambdaExpression)Evaluator.PartialEval(lastLambdaExpression);
					_lastSql = CreateWhereString(lastLambdaExpression);
				}
			}

			// Get the single arguments
			if (singleExpression != null)
			{
				if (singleExpression.Arguments.Count() > 1)
				{
					LambdaExpression singleLambdaExpression = (LambdaExpression)((UnaryExpression)(singleExpression.Arguments[1])).Operand;
					singleLambdaExpression = (LambdaExpression)Evaluator.PartialEval(singleLambdaExpression);
					_singleSql = CreateWhereString(singleLambdaExpression);
				}
			}

			// Get the select arguments
			if (selectExpression != null)
			{
				// Get the select arguments
				LambdaExpression selectLambdaExpression = (LambdaExpression)((UnaryExpression)(selectExpression.Arguments[1])).Operand;
				selectLambdaExpression = (LambdaExpression)Evaluator.PartialEval(selectLambdaExpression);
				_selectSql = CreateSelectString(selectLambdaExpression);
			}

			// Get the orderby arguments
			if (orderByExpression != null)
			{
				LambdaExpression orderByLambdaExpression = (LambdaExpression)((UnaryExpression)(orderByExpression.Arguments[1])).Operand;
				orderByLambdaExpression = (LambdaExpression)Evaluator.PartialEval(orderByLambdaExpression);
				_orderbySql = CreateOrderByString(orderByLambdaExpression);
			}

			// Get the orderby descending arguments
			if (orderByDescendingExpression != null)
			{
				LambdaExpression orderByDescendingLambdaExpression = (LambdaExpression)((UnaryExpression)(orderByDescendingExpression.Arguments[1])).Operand;
				orderByDescendingLambdaExpression = (LambdaExpression)Evaluator.PartialEval(orderByDescendingLambdaExpression);
				_orderbyDescendingSql = CreateOrderByDescendingString(orderByDescendingLambdaExpression);
			}

			// Get the thenby arguments
			if (thenByExpression != null)
			{
				LambdaExpression thenByLambdaExpression = (LambdaExpression)((UnaryExpression)(thenByExpression.Arguments[1])).Operand;
				thenByLambdaExpression = (LambdaExpression)Evaluator.PartialEval(thenByLambdaExpression);
				_thenbySql = CreateOrderByString(thenByLambdaExpression);
			}

			// Get the thenby descending arguments
			if (thenByDescendingExpression != null)
			{
				LambdaExpression thenByDescendingLambdaExpression = (LambdaExpression)((UnaryExpression)(thenByDescendingExpression.Arguments[1])).Operand;
				thenByDescendingLambdaExpression = (LambdaExpression)Evaluator.PartialEval(thenByDescendingLambdaExpression);
				_thenbyDescendingSql = CreateOrderByDescendingString(thenByDescendingLambdaExpression);
			}

			// Create the expression tree model.
			Model.ExpressionTreeModel expressionTreeModel = new Model.ExpressionTreeModel()
			{
				Take = _takeSql,
				Skip = _skipSql,
				Where = _whereSql,
				First = _firstSql,
				Select = _selectSql,
				Orderby = _orderbySql,
				OrderbyDescending = _orderbyDescendingSql,
				Thenby = _thenbySql,
				ThenbyDescending = _thenbyDescendingSql,
				Count = _countSql,
				Any = _anySql,
				Last = _lastSql,
				Single = _singleSql,
				All = _allSql,
			};

			// Execute the action and return the result, pass the 
			// expression inspector resultes to the client.
			T[] data = ExecuteActionHandler(expressionTreeModel);
			if (data == null)
			{
				throw new ArgumentNullException("The result data collection 'T[]' can not be null.", new Exception("A non empty array must be returned."));
			}

			// Copy the IEnumerable data type to an IQueryable.
			IQueryable<T> queryable = data.AsQueryable<T>();

			// Copy the expression tree that was passed in, changing only the first
			// argument of the innermost MethodCallExpression.
			ExpressionTreeBuilderModifier<T> treeCopier = new ExpressionTreeBuilderModifier<T>(queryable);
			Expression newExpressionTree = treeCopier.CopyAndModify(expression);

			// This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
			if (IsEnumerable)
				return queryable.Provider.CreateQuery(newExpressionTree);
			else
				return queryable.Provider.Execute(newExpressionTree);
		}

		/// <summary>
		/// Creates the where string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateWhereString(Expression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			LambdaExpression body = (LambdaExpression)base.Visit(expression);
			_param = (ParameterExpression)body.Parameters[0];

			// Start the expression builder.
			StartWhereCreation(body);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the where string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The lambda expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateWhereString(LambdaExpression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			_param = (ParameterExpression)expression.Parameters[0];

			// Start the expression builder.
			StartWhereCreation(expression);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the select string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateSelectString(Expression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			LambdaExpression body = (LambdaExpression)base.Visit(expression);
			_param = (ParameterExpression)body.Parameters[0];

			// Start the expression builder.
			StartSelectCreation(body);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the select string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateSelectString(LambdaExpression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			_param = (ParameterExpression)expression.Parameters[0];

			// Start the expression builder.
			StartSelectCreation(expression);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the orderby string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateOrderByString(Expression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			LambdaExpression body = (LambdaExpression)base.Visit(expression);
			_param = (ParameterExpression)body.Parameters[0];

			// Start the expression builder.
			StartOrderByCreation(body);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the orderby string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateOrderByString(LambdaExpression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			_param = (ParameterExpression)expression.Parameters[0];

			// Start the expression builder.
			StartOrderByCreation(expression);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the OrderBy Descending string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateOrderByDescendingString(Expression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			LambdaExpression body = (LambdaExpression)base.Visit(expression);
			_param = (ParameterExpression)body.Parameters[0];

			// Start the expression builder.
			StartOrderByDescendingCreation(body);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the OrderBy Descending string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateOrderByDescendingString(LambdaExpression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			// Get the current body lambad expression.
			_param = (ParameterExpression)expression.Parameters[0];

			// Start the expression builder.
			StartOrderByDescendingCreation(expression);
			return _sqlStatement;
		}

		/// <summary>
		/// Creates the skip string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private string CreateSkipString(MethodCallExpression expression)
		{
			return expression.Arguments[1].ToString();
		}

		/// <summary>
		/// Creates the take string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private string CreateTakeString(MethodCallExpression expression)
		{
			return expression.Arguments[1].ToString();
		}

		/// <summary>
		/// Creates the count string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateCountString(MethodCallExpression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			if (expression.Arguments.Count > 1)
			{
				// Get the current unary expression.
				UnaryExpression exp = ((UnaryExpression)(base.Visit(expression.Arguments[1])));
				Expression express = exp.Operand;

				// Get the current body lambad expression.
				LambdaExpression body = (LambdaExpression)base.Visit(express);
				_param = (ParameterExpression)body.Parameters[0];

				// Start the expression builder.
				StartWhereCreation(body);
				return _sqlStatement;
			}
			else
				return null;
		}

		/// <summary>
		/// Creates the any string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateAnyString(MethodCallExpression expression)
		{
			_sqlStatement = string.Empty;
			_sqlMethodStatement = string.Empty;

			if (expression.Arguments.Count > 1)
			{
				// Get the current unary expression.
				UnaryExpression exp = ((UnaryExpression)(base.Visit(expression.Arguments[1])));
				Expression express = exp.Operand;

				// Get the current body lambad expression.
				LambdaExpression body = (LambdaExpression)base.Visit(express);
				_param = (ParameterExpression)body.Parameters[0];

				// Start the expression builder.
				StartWhereCreation(body);
				return _sqlStatement;
			}
			else
				return null;
		}

        /// <summary>
		/// Creates the all string from the query within the expression tree.
		/// </summary>
		/// <param name="expression">The expression tree to analyse.</param>
		/// <returns>The query as a string.</returns>
		private String CreateAllString(MethodCallExpression expression)
        {
            _sqlStatement = string.Empty;
            _sqlMethodStatement = string.Empty;

            if (expression.Arguments.Count > 1)
            {
                // Get the current unary expression.
                UnaryExpression exp = ((UnaryExpression)(base.Visit(expression.Arguments[1])));
                Expression express = exp.Operand;

                // Get the current body lambad expression.
                LambdaExpression body = (LambdaExpression)base.Visit(express);
                _param = (ParameterExpression)body.Parameters[0];

                // Start the expression builder.
                StartWhereCreation(body);
                return _sqlStatement;
            }
            else
                return null;
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

				case ExpressionType.MemberInit:
					MemberInitExpression expNewInt = ((MemberInitExpression)(base.Visit(body.Body)));
					NewExpression expNew2 = expNewInt.NewExpression;
					System.Collections.ObjectModel.ReadOnlyCollection<MemberBinding> bindings = expNewInt.Bindings;
					foreach (MemberBinding exp in bindings)
					{
						MemberInfo expArgMem = exp.Member;
						_sqlStatement += "[" + expArgMem.Name + "], ";
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
						MethodCall(callMeth, methodName, null);
						break;
					default:
						BinaryExpression exp = ((BinaryExpression)(base.Visit(body.Body)));
						switch (exp.Left.NodeType)
						{
							case ExpressionType.Call:
								MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
								string methodNameCall = leftCall.Method.Name;
								MethodCall(leftCall, methodNameCall, exp);
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
						MethodCall(callMeth, methodName, null);
						break;
					default:
						BinaryExpression exp = ((BinaryExpression)(base.Visit(expression.Left)));
						switch (exp.Left.NodeType)
						{
							case ExpressionType.Call:
								MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
								string methodNameCall = leftCall.Method.Name;
								MethodCall(leftCall, methodNameCall, exp);
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
						MethodCall(callMeth, methodName, null);
						break;
					default:
						BinaryExpression exp = ((BinaryExpression)(base.Visit(expression.Right)));
						switch (exp.Left.NodeType)
						{
							case ExpressionType.Call:
								MethodCallExpression leftCall = ((MethodCallExpression)(base.Visit(exp.Left)));
								string methodNameCall = leftCall.Method.Name;
								MethodCall(leftCall, methodNameCall, exp);
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
			return "\"" + constant.Value.ToString() + "\"";
		}

		/// <summary>
		/// Gets the constant sql value.
		/// </summary>
		/// <param name="constant">The constant expression type.</param>
		/// <returns>The column constant value.</returns>
		private String DatabaseColumnConstantValue(object constant)
		{
			return "\"" + constant.ToString() + "\"";
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
					return " && ";
				case ExpressionType.Divide:
					return " / ";
				case ExpressionType.Equal:
					return " == ";
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
					return " ! ";
				case ExpressionType.NotEqual:
					return " != ";
				case ExpressionType.Or:
					return " | ";
				case ExpressionType.OrElse:
					return " || ";
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
		#endregion

		#region Sql Method Expression Tree Analysis Methods
		/// <summary>
		/// Select the appropriate sql method call.
		/// </summary>
		/// <param name="call">The current method call expression.</param>
		/// <param name="sqlMethodName">The current sql method.</param>
		/// <param name="exp">The current sql method.</param>
		private void MethodCall(MethodCallExpression call, string sqlMethodName, Expression exp)
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
				ReadOnlyCollection<Expression> arguments = call.Arguments;
				foreach (Expression expr in arguments)
				{
					// Get the member type argument.
					MemberExpression expValue = (MemberExpression)base.Visit(expr);
					string memberNameValue = expValue.Member.Name;

					// Get the constant expression and
					// create the current branch sql statement.
					ConstantExpression constValue = (ConstantExpression)base.Visit(expValue.Expression);

					// Get the current member argument field and
					// get the value for the current argument field.
					FieldInfo fieldValue = constValue.Value.GetType().GetField(memberNameValue,
						BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					object objectValue = fieldValue.GetValue(constValue.Value);

					_sqlMethodStatement += objectValue.ToString() + ",";
				}
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
			_sqlStatement += "(" + DatabaseColumnName(memberColumn) + "(" +
				_sqlMethodStatement.TrimEnd(',') + "))";
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
			private void Builder(Expression expression, IQueryable<T> queryable)
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

	/// <summary>
	/// Generic QueryableProvider implementation.
	/// </summary>
	/// <typeparam name="T">The data type to examine</typeparam>
	public sealed class GenericQueryableProviderInspector<T> : QueryableProviderBase<T> where T : class
	{
		/// <summary>
		/// Generic QueryableProvider implementation.
		/// </summary>
		public GenericQueryableProviderInspector()
		{
		}

		/// <summary>
		/// Generic QueryableProvider implementation.
		/// </summary>
		/// <param name="executeAction">The execution action handler, which when executed will return a result.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public GenericQueryableProviderInspector(Func<Model.ExpressionTreeModel, T[]> executeAction)
		{
			if (executeAction == null) throw new ArgumentNullException("executeAction");
			_executeAction = executeAction;
		}

		private Func<Model.ExpressionTreeModel, T[]> _executeAction = null;

		/// <summary>
		/// Gets or sets the execution action handler, which when executed will return a result.
		/// </summary>
		public Func<Model.ExpressionTreeModel, T[]> ExecuteAction
		{
			get { return _executeAction; }
			set { _executeAction = value; }
		}

		/// <summary>
		/// Execution action handler, results method.
		/// </summary>
		/// <param name="expressionTreeModel">The expression inspectors tree model.</param>
		/// <returns>The queryable type result of the execution.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		protected override T[] ExecuteActionHandler(Model.ExpressionTreeModel expressionTreeModel)
		{
			if (_executeAction == null) throw new ArgumentNullException("executeAction");
			return _executeAction(expressionTreeModel);
		}

		/// <summary>
		/// Get the queryable provider for the current type (T).
		/// </summary>
		/// <returns>A new instance of the queryable provider.</returns>
		public QueryableProvider<T> QueryableProvider()
		{
			return new QueryableProvider<T>(this);
		}

		/// <summary>
		/// Get the queryable provider for the current type (T).
		/// </summary>
		/// <param name="executeAction">The execution action handler, which when executed will return a result.</param>
		/// <returns>A new instance of the queryable provider.</returns>
		public static QueryableProvider<T> QueryableProviderEx(Func<Model.ExpressionTreeModel, T[]> executeAction)
		{
			if (executeAction == null) throw new ArgumentNullException("executeAction");

			// Create the Generic Queryable Provider Inspector and
			// Assign the execution function handler.
			Linq.GenericQueryableProviderInspector<T> query = new Linq.GenericQueryableProviderInspector<T>();
			query.ExecuteAction = executeAction;

			// Return the Queryable Provider.
			return query.QueryableProvider();
		}
	}
}
