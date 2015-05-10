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

namespace Nequeo.Model.Conversion
{
    /// <summary>
    /// Export batch model
    /// </summary>
    public class ExportBatchModel
    {
        /// <summary>
        /// Gets sets, the unique name of the item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets sets, the level 1 key identifer for the value.
        /// </summary>
        public long Level1ID { get; set; }

        /// <summary>
        /// Gets sets, the level 2 key identifer for the value.
        /// </summary>
        public long Level2ID { get; set; }

        /// <summary>
        /// Gets sets, the level 3 key identifer for the value.
        /// </summary>
        public long Level3ID { get; set; }

        /// <summary>
        /// Gets sets, the export file name.
        /// </summary>
        public string ExportName { get; set; }

        /// <summary>
        /// Gets sets, the data value;
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets sets, the data value;
        /// </summary>
        public object Value { get; set; }
    }
}
