/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

using Nequeo.Net.OAuth2;

namespace Nequeo.Net.OAuth.Client.Version2
{
    /// <summary>
    /// Facebook OAuth 2.0 client graph.
    /// </summary>
    [DataContract]
    public class FacebookGraph
    {
        private static DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(FacebookGraph));

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        [DataMember(Name = "link")]
        public Uri Link { get; set; }

        /// <summary>
        /// Gets or sets the birthday.
        /// </summary>
        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// Deserialize the JSON data.
        /// </summary>
        /// <param name="json">The JSON data to deserialise.</param>
        /// <returns>The facebook graph.</returns>
        public static FacebookGraph Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json");

            return Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }

        /// <summary>
        /// Deserialize the JSON stream data.
        /// </summary>
        /// <param name="jsonStream">The JSON stream.</param>
        /// <returns>The facebook graph.</returns>
        public static FacebookGraph Deserialize(Stream jsonStream)
        {
            if (jsonStream == null)
                throw new ArgumentNullException("jsonStream");

            return (FacebookGraph)jsonSerializer.ReadObject(jsonStream);
        }
    }
}
