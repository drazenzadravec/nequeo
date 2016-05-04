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
    /// Json model container
    /// </summary>
    public sealed class JsonModelContainer
    {
        private string _Namespace = null;
        private string _RootClassName = null;

        /// <summary>
        /// Gets or sets the base namespace.
        /// </summary>
        public string Namespace
        {
            get { return _Namespace; }
            set { _Namespace = value; }
        }

        /// <summary>
        /// Gets or sets the root class name.
        /// </summary>
        public string RootClassName
        {
            get { return _RootClassName; }
            set { _RootClassName = value; }
        }
    }

    /// <summary>
    /// A complete json document.
    /// </summary>
    public sealed class JsonDocument
    {
        /// <summary>
        /// Gets or sets the base namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the root class name.
        /// </summary>
        public string RootClassName { get; set; }

        /// <summary>
        /// Gets or sets the json classes within the document.
        /// </summary>
        public JsonClass[] JsonClasses { get; set; }
    }

    /// <summary>
    /// A json class and type container.
    /// </summary>
    public sealed class JsonClass
    {
        /// <summary>
        /// Gets or sets the class name.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the property array for the class.
        /// </summary>
        public Nequeo.Model.PropertyStringModel[] Properties { get; set; }
    }
}
