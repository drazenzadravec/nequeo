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
using System.Threading.Tasks;

namespace Nequeo.Data.Linq
{
    /// <summary>
    /// Search query model.
    /// </summary>
    [DataContract]
    [Serializable()]
    public class SearchQueryModel
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the value one if required; else only value is used.
        /// </summary>
        [DataMember]
        public object Value1 { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the operand.
        /// </summary>
        [DataMember]
        public Nequeo.Linq.ExpressionOperandType Operand { get; set; }

        /// <summary>
        /// Gets or sets the value type.
        /// </summary>
        [DataMember]
        public Type ValueType { get; set; }

        /// <summary>
        /// Gets or sets include in search indicator.
        /// </summary>
        [DataMember]
        public bool IncludeInSearch { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [DataMember]
        public int Index { get; set; }

    }

    /// <summary>
    /// Query Model.
    /// </summary>
    [DataContract]
    [Serializable()]
    public class QueryModel
    {
        /// <summary>
        /// Gets or sets the search queries.
        /// </summary>
        [DataMember]
        public SearchQueryModel[] Queries { get; set; }

        /// <summary>
        /// Gets or sets the expression operand type.
        /// </summary>
        [DataMember]
        public Nequeo.Linq.ExpressionOperandType Operand { get; set; }

        /// <summary>
        /// Change the value according to operand.
        /// </summary>
        public virtual void ValueToOperandMatch()
        {
            // For each query.
            foreach (SearchQueryModel query in Queries)
            {
                try
                {
                    // Select the operand.
                    switch (Operand)
                    {
                        case Nequeo.Linq.ExpressionOperandType.Like:
                            // If is string.
                            if (query.ValueType == typeof(string))
                            {
                                if (query.Value != null) query.Value = "%" + query.Value.ToString() + "%";
                                if (query.Value1 != null) query.Value1 = "%" + query.Value1.ToString() + "%";
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch { }
            }
        }
    }
}
