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

namespace Nequeo.Model.Message
{
    /// <summary>
    /// Web request model.
    /// </summary>
    [DataContract]
    [Serializable()]
    public class WebRequest
    {
        /// <summary>
        /// Gets sets the request name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets sets the token.
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// Gets sets the request body.
        /// </summary>
        [DataMember]
        public string Body { get; set; }

    }

    /// <summary>
    /// Web request model.
    /// </summary>
    /// <typeparam name="T">The body type.</typeparam>
    [DataContract]
    [Serializable()]
    [XmlRoot("WebRequest")]
    public class WebRequest<T>
    {
        /// <summary>
        /// Gets sets the request name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets sets the token.
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// Gets sets the request body.
        /// </summary>
        [DataMember]
        public T Body { get; set; }

        /// <summary>
        /// Explicit assignment.
        /// </summary>
        /// <param name="request">The request type.</param>
        /// <param name="body">The body data.</param>
        public void ExplicitAssignment(WebRequest request, T body)
        {
            Name = request.Name;
            Token = request.Token;
            Body = body;
        }
    }

    /// <summary>
    /// Web request body model.
    /// </summary>
    /// <typeparam name="T">The body type</typeparam>
    public class WebRequestBody<T>
    {
        /// <summary>
        /// Gets sets the request body.
        /// </summary>
        public T Body { get; set; }

    }
}
