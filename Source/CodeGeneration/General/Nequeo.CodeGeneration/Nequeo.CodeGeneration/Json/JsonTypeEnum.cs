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
    /// Json type.
    /// </summary>
    internal enum JsonTypeEnum
    {
        /// <summary>
        /// Anything.
        /// </summary>
        Anything,
        /// <summary>
        /// String.
        /// </summary>
        String,
        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean,
        /// <summary>
        /// Integer.
        /// </summary>
        Integer,
        /// <summary>
        /// Long.
        /// </summary>
        Long,
        /// <summary>
        /// Float.
        /// </summary>
        Float,
        /// <summary>
        /// Date.
        /// </summary>
        Date,
        /// <summary>
        /// Nullable integer.
        /// </summary>
        NullableInteger,
        /// <summary>
        /// Nullable long.
        /// </summary>
        NullableLong,
        /// <summary>
        /// Nullable float.
        /// </summary>
        NullableFloat,
        /// <summary>
        /// Nullable boolean.
        /// </summary>
        NullableBoolean,
        /// <summary>
        /// Nullable date.
        /// </summary>
        NullableDate,
        /// <summary>
        /// Object.
        /// </summary>
        Object,
        /// <summary>
        /// Array.
        /// </summary>
        Array,
        /// <summary>
        /// Dictionary.
        /// </summary>
        Dictionary,
        /// <summary>
        /// Nullable something.
        /// </summary>
        NullableSomething,
        /// <summary>
        /// None.
        /// </summary>
        NonConstrained
    }
}
