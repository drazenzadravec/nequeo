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

namespace Nequeo.Model
{
    /// <summary>
    /// The query type enum.
    /// </summary>
    [SerializableAttribute()]
    [DataContractAttribute()]
    public enum QueryType : int
    {
        /// <summary>
        /// The letter value.
        /// </summary>
        [EnumMemberAttribute()]
        [XmlEnumAttribute()]
        [SoapEnumAttribute()]
        Default = 0,

        /// <summary>
        /// The number value.
        /// </summary>
        [EnumMemberAttribute()]
        [XmlEnumAttribute()]
        [SoapEnumAttribute()]
        Number = 1,

        /// <summary>
        /// The special chars value.
        /// </summary>
        [EnumMemberAttribute()]
        [XmlEnumAttribute()]
        [SoapEnumAttribute()]
        Special = 2,

        /// <summary>
        /// The phrase value.
        /// </summary>
        [EnumMemberAttribute()]
        [XmlEnumAttribute()]
        [SoapEnumAttribute()]
        Phrase = 3,
    }

    /// <summary>
    /// Query model.
    /// </summary>
    [DataContract]
    [Serializable()]
    public class QueryModel
    {
        /// <summary>
        /// Gets or sets the is query type. 
        /// If phrase (e.g. 'The brown fox'), 
        /// if special (e.g. '/\()%# .. etc), 
        /// if number (e.g. '1, 2, 3, ... etc),
        /// if default the based on the first letter of the word (e.g. 'Agf, Jddf, ... etc);
        /// </summary>
        [DataMember]
        public QueryType QueryType { get; set; }

        /// <summary>
        /// Gets or sets the query text.
        /// </summary>
        [DataMember]
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the match query text. If false then matches the complete 
        /// query text; else true attempts to match part of the query text.
        /// </summary>
        [DataMember]
        public bool MatchPartialQuery { get; set; }
    }
}
