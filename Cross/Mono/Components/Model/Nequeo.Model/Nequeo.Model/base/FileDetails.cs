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
    /// File details.
    /// </summary>
    [DataContract]
    [Serializable()]
    public class FileDetails
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the size, in bytes, of the current file.
        /// </summary>
        [DataMember]
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        [DataMember]
        public System.IO.FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the current file or directory.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.
        /// </summary>
        [DataMember]
        public DateTime CreationTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file or directory exists.
        /// </summary>
        [DataMember]
        public bool Exists { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the current file is read only.
        /// </summary>
        [DataMember]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the string representing the extension part of the file.
        /// </summary>
        [DataMember]
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the full path of the directory or file.
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets an instance of the parent directory.
        /// </summary>
        [DataMember]
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets a string representing the directory's full path.
        /// </summary>
        [DataMember]
        public string DirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the time the current file or directory was last accessed.
        /// </summary>
        [DataMember]
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.
        /// </summary>
        [DataMember]
        public DateTime LastAccessTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the time when the current file or directory was last written to.
        /// </summary>
        [DataMember]
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.
        /// </summary>
        [DataMember]
        public DateTime LastWriteTimeUtc { get; set; }
    }
}
