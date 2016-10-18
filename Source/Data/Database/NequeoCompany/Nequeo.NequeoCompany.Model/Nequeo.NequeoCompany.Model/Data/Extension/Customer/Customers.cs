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
    /// The Customer data member extension.
    /// </summary>
    public partial class Customers
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
                case "GetCustomer":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Customers>> callbackGetAccount = (Action<Nequeo.Threading.AsyncOperationResult<Data.Customers>>)_callback[e2];
                    callbackGetAccount(new Nequeo.Threading.AsyncOperationResult<Data.Customers>(((Data.Customers)e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Get the customer information.
        /// </summary>
        /// <param name="customerID">The customer id.</param>
        /// <returns>The customer data.</returns>
        public virtual Data.Customers GetCustomer(int customerID)
        {
            return Select.SelectDataEntity(
                u => u.CustomerID == customerID);
        }

        /// <summary>
        /// Get the customer information.
        /// </summary>
        /// <param name="customerID">The customer id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetCustomer(int customerID, Action<Nequeo.Threading.AsyncOperationResult<Data.Customers>> callback, object state = null)
        {
            string keyName = "GetCustomer";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Customers>(u => u.GetCustomer(customerID), keyName);
        }

        /// <summary>
        /// Search for customers according to search criteria
        /// </summary>
        /// <param name="queries">The criteria to search for.</param>
        /// <returns>The collection of customers that match the criteria.</returns>
        public virtual Data.Customers[] SearchForCustomers(string[] queries)
        {
            // Create the expression helper.
            DataTypeConversion dataTypeConversion = new DataTypeConversion(_connectionDataType);
            SqlStatementConstructor statement = new SqlStatementConstructor(dataTypeConversion);

            // Create the query list.
            Data.Customers[] customers = null;
            List<QueryModel> queryModels = new List<QueryModel>();

            // For each query.
            foreach (string query in queries)
            {
                // Add the serach model.
                SearchQueryModel[] searchModel = new SearchQueryModel[]
                {
                    new SearchQueryModel() 
                    { 
                        ColumnName = "CustomerID", 
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
                        ColumnName = "Suburb", 
                        Operand = Linq.ExpressionOperandType.Like, 
                        Value = query, 
                        ValueType = typeof(string)
                    },
                    new SearchQueryModel() 
                    { 
                        ColumnName = "PostCode", 
                        Operand = Linq.ExpressionOperandType.Like, 
                        Value = query, 
                        ValueType = typeof(string)
                    },
                    new SearchQueryModel() 
                    { 
                        ColumnName = "EmailAddress", 
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
            Expression<Nequeo.Threading.FunctionHandler<bool, Data.Customers>> predicate =
                statement.CreateLambdaExpressionEx<Data.Customers>(queryModels.ToArray(), Linq.ExpressionOperandType.OrElse);

            // Execute the query.
            customers = Select.SelectIQueryableItems(predicate).ToArray();

            // Return the list found.
            return customers;
        }
    }
}
