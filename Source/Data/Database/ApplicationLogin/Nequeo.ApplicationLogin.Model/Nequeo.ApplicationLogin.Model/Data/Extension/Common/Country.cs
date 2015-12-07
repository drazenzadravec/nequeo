using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.SqlClient;
using System.ServiceModel;

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

namespace Nequeo.DataAccess.ApplicationLogin.Data.Extension
{
    /// <summary>
    /// The Country data member extension.
    /// </summary>
    partial class Country
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
                case "GetCountryList":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Extended.CountryList[]>> callbackGetCountryList = (Action<Nequeo.Threading.AsyncOperationResult<Data.Extended.CountryList[]>>)_callback[e2];
                    callbackGetCountryList(new Nequeo.Threading.AsyncOperationResult<Data.Extended.CountryList[]>(((Data.Extended.CountryList[])e1), _state[e2], e2));
                    break;

                case "GetCountryAutoComplete":
                    Action<Nequeo.Threading.AsyncOperationResult<string[]>> callbackGetCountryAutoComplete = (Action<Nequeo.Threading.AsyncOperationResult<string[]>>)_callback[e2];
                    callbackGetCountryAutoComplete(new Nequeo.Threading.AsyncOperationResult<string[]>(((string[])e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Gets the country list.
        /// </summary>
        /// <returns>The country list.</returns>
        public virtual Data.Extended.CountryList[] GetCountryList()
        {
            return
                Select.
                SelectIQueryableItems(c => c.CountryVisible == true).
                OrderBy(c => c.GroupOrder).
                Select(c => new
                {
                    CountryID = c.CountryID,
                    CountryName = c.CountryName,
                    CountryCode = c.CountryCode
                }).ToTypeArray<Data.Extended.CountryList>();
        }

        /// <summary>
        /// Gets the country list.
        /// </summary>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetCountryList(Action<Nequeo.Threading.AsyncOperationResult<Data.Extended.CountryList[]>> callback, object state = null)
        {
            string keyName = "GetCountryList";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Extended.CountryList[]>(u => u.GetCountryList(), keyName);
        }

        /// <summary>
        /// Gets the auto complete country data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of country.</returns>
        public virtual string[] GetCountryAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(c => (SqlQueryMethods.Like(c.CountryName, (prefixText + "%"))) ||
                            (SqlQueryMethods.Like(c.CountryCode, (prefixText + "%")))).
                        Select(c => new
                        {
                            CountryName = (string)c.CountryName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.CountryName;

            return ret;
        }

        /// <summary>
        /// Gets the auto complete country data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetCountryAutoComplete(string prefixText, Int32 numberToReturn, Action<Nequeo.Threading.AsyncOperationResult<string[]>> callback, object state = null)
        {
            string keyName = "GetCountryAutoComplete";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<string[]>(u => u.GetCountryAutoComplete(prefixText, numberToReturn), keyName);
        }
    }
}