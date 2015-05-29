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
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

namespace Nequeo.Serialisation
{
    /// <summary>
    /// Custom Xml serialiser. Provides custom formatting for XML 
    /// serialization and deserialization through the IXmlSerializable interface.
    /// </summary>
    [Serializable]
    public abstract class XmlCustomSerialiser : IXmlSerializable
	{
        /// <summary>
        /// Custom Xml serialiser. Provides custom formatting for XML 
        /// serialization and deserialization through the IXmlSerializable interface.
        /// </summary>
        protected XmlCustomSerialiser() {}

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable
        /// interface, you should return null (Nothing in Visual Basic) from this method,
        /// and instead, if specifying a custom schema is required, apply the System.Xml.Serialization.XmlSchemaProviderAttribute
        /// to the class.
        /// </summary>
        /// <returns>Null; not implemented.</returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader stream from which the object is deserialized.</param>
        public abstract void ReadXml(XmlReader reader);

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public abstract void WriteXml(XmlWriter writer);
    }
}
