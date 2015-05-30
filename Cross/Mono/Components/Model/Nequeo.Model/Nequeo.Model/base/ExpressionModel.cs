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
    /// Expression model.
    /// </summary>
    [DataContract]
    [Serializable()]
	public class ExpressionModel
	{
        private string[] _any = null;
        private string[] _count = null;
        private string[] _where = null;
        private string[] _first = null;
        private string[] _select = null;
        private string[] _orderby = null;
        private string[] _orderbyDescending = null;
        private string[] _thenby = null;
        private string[] _thenbyDescending = null;
        private int? _skipInt32 = null;
        private int? _takeInt32 = null;
        private long? _skipInt64 = null;
        private long? _takeInt64 = null;

        /// <summary>
        /// Gets or sets the split the any expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] Any
        {
            get { return _any; }
            set { _any = value; }
        }

        /// <summary>
        /// Gets or sets the split the count expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// Gets or sets the split the where expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] Where
        {
            get { return _where; }
            set { _where = value; }
        }

        /// <summary>
        /// Gets or sets the split the first expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] First
        {
            get { return _first; }
            set { _first = value; }
        }

        /// <summary>
        /// Gets or sets the split the select expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] Select
        {
            get { return _select; }
            set { _select = value; }
        }

        /// <summary>
        /// Gets or sets the split the orderby expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] Orderby
        {
            get { return _orderby; }
            set { _orderby = value; }
        }

        /// <summary>
        /// Gets or sets the split the orderby descending expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] OrderbyDescending
        {
            get { return _orderbyDescending; }
            set { _orderbyDescending = value; }
        }

        /// <summary>
        /// Gets or sets the split the thenby expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] Thenby
        {
            get { return _thenby; }
            set { _thenby = value; }
        }

        /// <summary>
        /// Gets or sets the split the thenby descending expression into individual expressions.
        /// </summary>
        [DataMember]
        [XmlElement]
        public string[] ThenbyDescending
        {
            get { return _thenbyDescending; }
            set { _thenbyDescending = value; }
        }

        /// <summary>
        /// Gest or sets the integer type of the skip value.
        /// </summary>
        [DataMember]
        [XmlElement]
        public int? SkipInt32
        {
            get { return _skipInt32; }
            set { _skipInt32 = value; }
        }

        /// <summary>
        /// Gest or sets the integer type of the take value.
        /// </summary>
        [DataMember]
        [XmlElement]
        public int? TakeInt32
        {
            get { return _takeInt32; }
            set { _takeInt32 = value; }
        }

        /// <summary>
        /// Gest or sets the long integer type of the skip value.
        /// </summary>
        [DataMember]
        [XmlElement]
        public long? SkipInt64
        {
            get { return _skipInt64; }
            set { _skipInt64 = value; }
        }

        /// <summary>
        /// Gest or sets the long integer type of the take value.
        /// </summary>
        [DataMember]
        [XmlElement]
        public long? TakeInt64
        {
            get { return _takeInt64; }
            set { _takeInt64 = value; }
        }
	}
}
