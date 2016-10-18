/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Linq.Expressions;

using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;

namespace Nequeo.DataAccess.NequeoCompany.Data.Extension
{
    /// <summary>
    /// The companies data member extension.
    /// </summary>
    public partial class Companies
    {
        /// <summary>
        /// Async complete action handler
        /// </summary>
        /// <param name="sender">The current object handler</param>
        /// <param name="e1">The action execution result</param>
        /// <param name="e2">The unique action name.</param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "GetCompany":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Companies>> callbackGetAccount = (Action<Nequeo.Threading.AsyncOperationResult<Data.Companies>>)_callback[e2];
                    callbackGetAccount(new Nequeo.Threading.AsyncOperationResult<Data.Companies>(((Data.Companies)e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Get the company information.
        /// </summary>
        /// <param name="companyID">The company id.</param>
        /// <returns>The company data.</returns>
        public virtual Data.Companies GetCompany(int companyID)
        {
            return Select.SelectDataEntity(
                u => u.CompanyID == companyID);
        }

        /// <summary>
        /// Get the company information.
        /// </summary>
        /// <param name="companyID">The company id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetCompany(int companyID, Action<Nequeo.Threading.AsyncOperationResult<Data.Companies>> callback, object state = null)
        {
            string keyName = "GetCompany";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Companies>(u => u.GetCompany(companyID), keyName);
        }

        /// <summary>
        /// Search for companies according to search criteria
        /// </summary>
        /// <param name="queries">The criteria to search for.</param>
        /// <returns>The collection of companies that match the criteria.</returns>
        public virtual Data.Companies[] SearchForCompanies(string[] queries)
        {
            // Create the expression helper.
            DataTypeConversion dataTypeConversion = new DataTypeConversion(_connectionDataType);
            SqlStatementConstructor statement = new SqlStatementConstructor(dataTypeConversion);

            // Create the query list.
            Data.Companies[] companies = null;
            List<QueryModel> queryModels = new List<QueryModel>();

            // For each query.
            foreach (string query in queries)
            {
                // Add the serach model.
                SearchQueryModel[] searchModel = new SearchQueryModel[]
                {
                    new SearchQueryModel() 
                    { 
                        ColumnName = "CompanyID", 
                        Operand = Linq.ExpressionOperandType.Like, 
                        Value = query, 
                        ValueType = typeof(string)
                    },
                    new SearchQueryModel() 
                    { 
                        ColumnName = "CompanyName", 
                        Operand = Linq.ExpressionOperandType.Like, 
                        Value = query, 
                        ValueType = typeof(string)
                    },
                    new SearchQueryModel() 
                    { 
                        ColumnName = "Address", 
                        Operand = Linq.ExpressionOperandType.Like, 
                        Value = query, 
                        ValueType = typeof(string)
                    },
                    new SearchQueryModel() 
                    { 
                        ColumnName = "ABN", 
                        Operand = Linq.ExpressionOperandType.Like, 
                        Value = query, 
                        ValueType = typeof(string)
                    },
                };

                // Add the query model item.
                QueryModel model = new QueryModel() { Queries = searchModel, Operand = Linq.ExpressionOperandType.OrElse };
                queryModels.Add(model);
            }

            // Create the company expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, Data.Companies>> predicate =
                statement.CreateLambdaExpressionEx<Data.Companies>(queryModels.ToArray(), Linq.ExpressionOperandType.OrElse);

            // Execute the query.
            companies = Select.SelectIQueryableItems(predicate).ToArray();

            // Return the list found.
            return companies;
        }
    }
}
