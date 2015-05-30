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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Nequeo.Model
{
    /// <summary>
    /// Expression tree model.
    /// </summary>
    [DataContract]
    [Serializable()]
	public class ExpressionTreeModel
	{
        private string _where = string.Empty;
        private string _first = string.Empty;
        private string _select = string.Empty;
        private string _orderby = string.Empty;
        private string _orderbyDescending = string.Empty;
        private string _thenby = string.Empty;
        private string _thenbyDescending = string.Empty;
        private string _skip = string.Empty;
        private string _take = string.Empty;
        private string _count = string.Empty;
        private string _any = string.Empty;

        private bool _removeDataPadding = false;

        /// <summary>
        /// Gets or sets the where string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Where
        {
            get { return _where; }
            set { _where = value; }
        }

        /// <summary>
        /// Gets or sets the first string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string First
        {
            get { return _first; }
            set { _first = value; }
        }

        /// <summary>
        /// Gets or sets the select string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Select
        {
            get { return _select; }
            set { _select = value; }
        }

        /// <summary>
        /// Gets or sets the orderby string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Orderby
        {
            get { return _orderby; }
            set { _orderby = value; }
        }

        /// <summary>
        /// Gets or sets the orderby descending string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string OrderbyDescending
        {
            get { return _orderbyDescending; }
            set { _orderbyDescending = value; }
        }

        /// <summary>
        /// Gets or sets the thenby string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Thenby
        {
            get { return _thenby; }
            set { _thenby = value; }
        }

        /// <summary>
        /// Gets or sets the thenby descending string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string ThenbyDescending
        {
            get { return _thenbyDescending; }
            set { _thenbyDescending = value; }
        }

        /// <summary>
        /// Gets or sets the skip string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Skip
        {
            get { return _skip; }
            set { _skip = value; }
        }

        /// <summary>
        /// Gets or sets the take string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Take
        {
            get { return _take; }
            set { _take = value; }
        }

        /// <summary>
        /// Gets or sets the count string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// Gets or sets the any string from the query within the expression tree.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string Any
        {
            get { return _any; }
            set { _any = value; }
        }

        /// <summary>
        /// Get the split and converted expression model data.
        /// </summary>
        /// <param name="removeDataPadding">Remove the database like padding from the expressions.</param>
        /// <returns>The expression model.</returns>
        public ExpressionModel GetExpression(bool removeDataPadding = false)
        {
            _removeDataPadding = removeDataPadding;

            // Return the expression model.
            ExpressionModel model = new ExpressionModel()
            {
                Where = GetWhereExpression(),
                First = GetFirstExpression(),
                Orderby = GetOrderbyExpression(),
                OrderbyDescending = GetOrderbyDescendingExpression(),
                Select = GetSelectExpression(),
                SkipInt32 = GetSkipInt32(),
                SkipInt64 = GetSkipInt64(),
                TakeInt32 = GetTakeInt32(),
                TakeInt64 = GetTakeInt64(),
                Thenby = GetThenbyExpression(),
                ThenbyDescending = GetThenbyDescendingExpression(),
                Count = GetCountExpression(),
                Any = GetAnyExpression()
            };

            _removeDataPadding = false;
            return model;
        }

        /// <summary>
        /// Split the where expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetWhereExpression()
        {
            if (!string.IsNullOrEmpty(_where))
            {
                // Split the data.
                string[] data = _where.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if(_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }
                    
                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the first expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetFirstExpression()
        {
            if (!string.IsNullOrEmpty(_first))
            {
                // Split the data.
                string[] data = _first.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the select expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetSelectExpression()
        {
            if (!string.IsNullOrEmpty(_select))
            {
                // Split the data.
                string[] data = _select.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the orderby expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetOrderbyExpression()
        {
            if (!string.IsNullOrEmpty(_orderby))
            {
                // Split the data.
                string[] data = _orderby.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the orderbyDescending expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetOrderbyDescendingExpression()
        {
            if (!string.IsNullOrEmpty(_orderbyDescending))
            {
                // Split the data.
                string[] data = _orderbyDescending.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the thenby expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetThenbyExpression()
        {
            if (!string.IsNullOrEmpty(_thenby))
            {
                // Split the data.
                string[] data = _thenby.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the thenbyDescending expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetThenbyDescendingExpression()
        {
            if (!string.IsNullOrEmpty(_thenbyDescending))
            {
                // Split the data.
                string[] data = _thenbyDescending.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the count expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetCountExpression()
        {
            if (!string.IsNullOrEmpty(_count))
            {
                // Split the data.
                string[] data = _count.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Split the any expression into individual expressions.
        /// </summary>
        /// <returns>Each where string expression; otherwise null.</returns>
        public string[] GetAnyExpression()
        {
            if (!string.IsNullOrEmpty(_any))
            {
                // Split the data.
                string[] data = _any.
                    Replace("(", "").Replace("{", "").
                    Replace(")", "").Replace("}", "").
                    Split(new string[] { "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);

                // Remove all empty padding
                List<string> result = new List<string>(data.Length);
                foreach (string item in data)
                {
                    if (_removeDataPadding)
                        result.Add(item.Trim().Replace("[", "").Replace("]", ""));
                    else
                        result.Add(item.Trim());
                }

                // Return the collection.
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Get the integer type of the skip value.
        /// </summary>
        /// <returns>The integer skip value; otherwise null.</returns>
        public int? GetSkipInt32()
        {
            if (!string.IsNullOrEmpty(_skip))
            {
                // Return the collection.
                return Int32.Parse(_skip);
            }
            else
                return null;
        }

        /// <summary>
        /// Get the long integer type of the skip value.
        /// </summary>
        /// <returns>The long integer skip value; otherwise null.</returns>
        public long? GetSkipInt64()
        {
            if (!string.IsNullOrEmpty(_skip))
            {
                // Return the collection.
                return Int64.Parse(_skip);
            }
            else
                return null;
        }

        /// <summary>
        /// Get the integer type of the take value.
        /// </summary>
        /// <returns>The integer take value; otherwise null.</returns>
        public int? GetTakeInt32()
        {
            if (!string.IsNullOrEmpty(_take))
            {
                // Return the collection.
                return Int32.Parse(_take);
            }
            else
                return null;
        }

        /// <summary>
        /// Get the long integer type of the take value.
        /// </summary>
        /// <returns>The long integer take value; otherwise null.</returns>
        public long? GetTakeInt64()
        {
            if (!string.IsNullOrEmpty(_take))
            {
                // Return the collection.
                return Int64.Parse(_take);
            }
            else
                return null;
        }
	}
}
