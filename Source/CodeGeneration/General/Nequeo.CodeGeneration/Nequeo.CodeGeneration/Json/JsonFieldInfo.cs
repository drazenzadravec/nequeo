/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nequeo.CodeGeneration.Json
{
    /// <summary>
    /// JSON field info.
    /// </summary>
    internal class JsonFieldInfo
    {
        /// <summary>
        /// JSON field info.
        /// </summary>
        /// <param name="generator">The genrator.</param>
        /// <param name="jsonMemberName">The json memeber name.</param>
        /// <param name="type">The json type.</param>
        /// <param name="usePascalCase">Use pacal case.</param>
        /// <param name="jobjects">The json objects.</param>
        public JsonFieldInfo(IJsonConfig generator, string jsonMemberName, JsonType type, bool usePascalCase, IList<object> jobjects)
        {
            this.generator = generator;
            this.JsonMemberName = jsonMemberName;
            this.MemberName = jsonMemberName;
            if (usePascalCase) MemberName = JsonGenerator.ToTitleCase(MemberName);
            this.Type = type;
            this.Jobjects = jobjects;
        }

        private IJsonConfig generator;

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Gets the json member name.
        /// </summary>
        public string JsonMemberName { get; private set; }

        /// <summary>
        /// Gets the json type.
        /// </summary>
        public JsonType Type { get; private set; }

        /// <summary>
        /// Gets the jobjects.
        /// </summary>
        public IList<object> Jobjects { get; private set; }

    }
}
