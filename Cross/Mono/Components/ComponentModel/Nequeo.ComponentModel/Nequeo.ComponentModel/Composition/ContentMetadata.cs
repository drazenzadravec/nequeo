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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Nequeo.ComponentModel.Composition
{
    /// <summary>
    /// Content base meta-data attribute
    /// </summary>
    [MetadataAttribute]
    public sealed class ContentMetadataAttribute : Attribute
    {
        /// <summary>
        /// The name of the export.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the export.
        /// </summary>
        public string Description { get; set; }  

        /// <summary>
        /// The index of the export.
        /// </summary>
        public int Index { get; set; }    

        /// <summary>
        /// Is the export hidden.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// The value of the export.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentMetadataAttribute()
        {
            Name = "";
            Description = "";
            Index = 0;
            Hidden = false;
            Value = null;
        }
    }

    /// <summary>
    /// Content base meta-data interface
    /// </summary>
    public interface IContentMetadata
    {
        /// <summary>
        /// The name of the export.
        /// </summary>
        [DefaultValue("")]
        string Name { get; }

        /// <summary>
        /// The description of the export.
        /// </summary>
        [DefaultValue("")]
        string Description { get; }

        /// <summary>
        /// The index of the export.
        /// </summary>
        [DefaultValue(0)]
        int Index { get; }

        /// <summary>
        /// Is the export hidden.
        /// </summary>
        [DefaultValue(false)]
        bool Hidden { get; }

        /// <summary>
        /// The value of the export.
        /// </summary>
        [DefaultValue(null)]
        object Value { get; }
    }
}
