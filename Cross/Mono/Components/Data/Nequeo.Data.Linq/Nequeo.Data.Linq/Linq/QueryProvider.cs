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
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;

using LinqTypes = Nequeo.Data.DataType.ProviderToDataTypes;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

using Nequeo.Data.DataType;

namespace Nequeo.Data.Linq
{
    /// <summary>
    /// A LINQ IQueryable query provider that executes database queries over a DbConnection
    /// </summary>
    public class DataQueryProvider : Nequeo.Data.Linq.Provider.QueryProvider
    {
        DbConnection connection;
        QueryPolicy policy;
        QueryMapping mapping;
        QueryLanguage language;
        TextWriter log;
        Type dataObjectType;
        IDataContextBase dataContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="policy"></param>
        /// <param name="log"></param>
        /// <param name="dataObjectType"></param>
        /// <param name="dataContext"></param>
        public DataQueryProvider(
            DbConnection connection, QueryPolicy policy, TextWriter log,
            Type dataObjectType, IDataContextBase dataContext)
        {
            this.connection = connection;
            this.policy = policy;
            this.mapping = policy.Mapping;
            this.language = mapping.Language;
            this.log = log;
            this.dataObjectType = dataObjectType;
            this.dataContext = dataContext;
        }

        /// <summary>
        /// 
        /// </summary>
        public DbConnection Connection
        {
            get { return this.connection; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextWriter Log
        {
            get { return this.log; }
        }

        /// <summary>
        /// 
        /// </summary>
        public QueryPolicy Policy
        {
            get { return this.policy; }
        }

        /// <summary>
        /// 
        /// </summary>
        public QueryMapping Mapping
        {
            get { return this.mapping; }
        }

        /// <summary>
        /// 
        /// </summary>
        public QueryLanguage Language
        {
            get { return this.language; }
        }

        /// <summary>
        /// Converts the query expression into text that corresponds to the command that would be executed.
        /// Useful for debugging.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override string GetQueryText(Expression expression)
        {
            Expression translated = this.Translate(expression);
            var selects = SelectGatherer.Gather(translated).Select(s => this.language.Format(s));
            return string.Join("\n\n", selects.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string GetQueryPlan(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);
            return DbExpressionWriter.WriteToString(plan);
        }

        /// <summary>
        /// Execute the query expression (does translation, etc.)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override object Execute(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);

            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null)
            {
                // compile & return the execution plan so it can be used multiple times
                LambdaExpression fn = Expression.Lambda(lambda.Type, plan, lambda.Parameters);
                return fn.Compile();
            }
            else
            {
                // compile the execution plan and invoke it
                Expression<Func<object>> efn = Expression.Lambda<Func<object>>(Expression.Convert(plan, typeof(object)));
                Func<object> fn = efn.Compile();
                return fn();
            }
        }

        /// <summary>
        /// Convert the query expression into an execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual Expression GetExecutionPlan(Expression expression)
        {
            // strip off lambda for now
            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null)
                expression = lambda.Body;

            // translate query into client & server parts
            ProjectionExpression projection = this.Translate(expression);

            Expression rootQueryable = Nequeo.Data.Linq.Provider.RootQueryableFinder.Find(expression);
            Expression provider = Expression.Convert(
                Expression.Property(rootQueryable, typeof(IQueryable).GetProperty("Provider")),
                typeof(DataQueryProvider)
                );

            return this.policy.BuildExecutionPlan(projection, provider);
        }

        /// <summary>
        /// Do all query translations execpt building the execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual ProjectionExpression Translate(Expression expression)
        {
            // pre-evaluate local sub-trees
            expression = Nequeo.Data.Linq.Provider.PartialEvaluator.Eval(expression, this.CanBeEvaluatedLocally);

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

        /// <summary>
        /// Execute an actual query specified in the target language using the sADO connection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Execute<T>(QueryCommand<T> query, object[] paramValues)
        {
            DbCommand cmd = this.GetCommand(query.CommandText, query.ParameterNames, paramValues);
            this.LogCommand(cmd);
            DbDataReader reader = cmd.ExecuteReader();
            return Project(reader, query.Projector);
        }

        /// <summary>
        /// Converts a data reader into a sequence of objects using a projector function on each row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fnProjector"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Project<T>(DbDataReader reader, Func<DbDataReader, T> fnProjector)
        {
            List<T> data = new List<T>();
            while (reader.Read())
            {
                data.Add(fnProjector(reader));
            }
            reader.Close();

            if (dataObjectType == typeof(T)){
                if (dataContext.LazyLoading){
                    foreach (var dataObject in data){
                        TranslateLazyLoading(dataObject);
                    }
                }
            }

            foreach (var result in data)
                yield return result;
        }

        /// <summary>
        /// Get an IEnumerable that will execute the specified query when enumerated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> ExecuteDeferred<T>(QueryCommand<T> query, object[] paramValues)
        {
            DbCommand cmd = this.GetCommand(query.CommandText, query.ParameterNames, paramValues);
            this.LogCommand(cmd);
            DbDataReader reader = cmd.ExecuteReader();
            List<T> data = new List<T>();
            while (reader.Read())
            {
                data.Add(query.Projector(reader));
            }
            reader.Close();

            foreach (var result in data)
                yield return result;
        }

        /// <summary>
        /// Get an ADO command object initialized with the command-text and parameters
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual DbCommand GetCommand(string commandText, IList<string> paramNames, object[] paramValues)
        {
            // create command object (and fill in parameters)
            DbCommand cmd = this.connection.CreateCommand();
            cmd.CommandText = commandText;
            for (int i = 0, n = paramNames.Count; i < n; i++)
            {
                DbParameter p = cmd.CreateParameter();
                p.ParameterName = paramNames[i];
                p.Value = paramValues[i] ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
            return cmd;
        }

        /// <summary>
        /// Write a command to the log
        /// </summary>
        /// <param name="command"></param>
        protected virtual void LogCommand(DbCommand command)
        {
            if (this.log != null)
            {
                this.log.WriteLine(command.CommandText);
                foreach(DbParameter p in command.Parameters)
                {
                    if (p.Value == null || p.Value == DBNull.Value)
                    {
                        this.log.WriteLine("-- @{0} = NULL", p.ParameterName);
                    }
                    else
                    {
                        this.log.WriteLine("-- @{0} = [{1}]", p.ParameterName, p.Value);
                    }
                }
                this.log.WriteLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        private void TranslateLazyLoading(object dataObject)
        {
            List<PropertyInfo> propertiesRef = GetAllForeignKey(dataObject);
            List<PropertyInfo> propertiesData = GetAllColumnData(dataObject);

            foreach (PropertyInfo property in propertiesRef)
            {
                // Get the current foreign key attribute for the property
                Nequeo.Data.Custom.DataColumnForeignKeyAttribute data = GetForeignKeyAttribute(property);

                PropertyInfo propertyInfo = null;
                try
                {
                    propertyInfo = propertiesData.First(p => p.Name.ToLower() == data.ColumnName.ToLower());
                }
                catch { }
                if (propertyInfo != null)
                {
                    // Get the current referenced coulm in the current data object.
                    object value = propertyInfo.GetValue(dataObject, null);

                    // Set the reference type lazy loading data.
                    if (value != null)
                    {
                        // Set the current reference object back to
                        // the parent type
                        property.SetValue(dataObject, SetForeginKeyReferenceData(property, value, data), null);

                        // Get the new refrence type object instance.
                        object newRefObject = property.GetValue(dataObject, null);

                        // Has the current reference column type contain
                        // reference columns, if true then apply recursion
                        List<PropertyInfo> propertiesRefValue = GetAllForeignKey(newRefObject);
                        if (propertiesRefValue.Count > 0)
                            TranslateLazyLoading(newRefObject);
                    }
                }
            }
        }

        /// <summary>
        /// Get the DataColumnForeignKeyAttribute data for the current property information.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>The data column foreign key attribute data else null if attribute does not exist.</returns>
        private Nequeo.Data.Custom.DataColumnForeignKeyAttribute GetForeignKeyAttribute(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                        (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;
                    return att;
                }
            }

            // Return null.
            return null;
        }

        /// <summary>
        /// Get all foreign key properties for the data entity type.
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        private List<PropertyInfo> GetAllForeignKey(object dataEntity)
        {
            // Create a new property collection.
            List<PropertyInfo> foreignKey = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in dataEntity.GetType().GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                            (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;

                        // If the property attribute
                        // is a foreign key it is
                        // not a reference.
                        if (att.IsReference)
                            foreignKey.Add(member);
                    }
                }
            }

            // Return the collection of
            // foreign key properties.
            return foreignKey;
        }

        /// <summary>
        /// Get all table column properties for the data entity type.
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public List<PropertyInfo> GetAllColumnData(object dataEntity)
        {
            // Create a new property collection.
            List<PropertyInfo> column = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in dataEntity.GetType().GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // Add the table column.
                        column.Add(member);
                    }
                }
            }

            // Return the collection of
            // columns properties.
            return column;
        }

        /// <summary>
        /// Get the translated foreign key reference data.
        /// </summary>
        /// <param name="property">The current property information</param>
        /// <param name="foreignKeyValue">The foreign key value for the referenced data entity.</param>
        /// <param name="data">The data column foreign key reference attribute data.</param>
        /// <returns>The translated data entity object.</returns>
        private object SetForeginKeyReferenceData(PropertyInfo property, object foreignKeyValue, Nequeo.Data.Custom.DataColumnForeignKeyAttribute data)
        {
            // Get the get method of the property.
            MethodInfo method = property.GetGetMethod();

            string tableName = DataTypeConversion.GetSqlConversionDataType(dataContext.ProviderConnectionDataType, data.Name.TrimStart('_').Replace(".", "].["));
            string colunmName = DataTypeConversion.GetSqlConversionDataType(dataContext.ProviderConnectionDataType, data.ReferenceColumnName.TrimStart('_'));
            DataTypeConversion dataTypeConversion = new DataTypeConversion(dataContext.ProviderConnectionDataType);

            // Execute the queryable provider and return the constructed
            // sql statement and return the data.
            string statement = dataTypeConversion.GetSqlStringValue(LinqTypes.GetDataType(data.ColumnType, dataContext.ProviderConnectionDataType), foreignKeyValue);
            DataTable table = dataContext.ExecuteQuery("SELECT * FROM " + tableName + " WHERE " + colunmName + " = " + statement);

            // Get the anonymous type translator from datarow
            // to the foreign key reference property return type.
            Nequeo.Data.Control.AnonymousTypeFunction typeFunction = new Nequeo.Data.Control.AnonymousTypeFunction();
            return ((table.Rows.Count > 0) ? typeFunction.TypeTranslator(table.Rows[0], method.ReturnType) : null);
        }
    }
}
