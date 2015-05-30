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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.Compilation;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Xml.Serialization;

namespace Nequeo.Web.Common
{
    /// <summary>
    /// The positioning choices for the Calendar popup
    /// </summary>
    public enum CalendarPosition
    {
        /// <summary>
        /// 
        /// </summary>
        BottomLeft = 0,
        /// <summary>
        /// 
        /// </summary>
        BottomRight = 1,
        /// <summary>
        /// 
        /// </summary>
        TopLeft = 2,
        /// <summary>
        /// 
        /// </summary>
        TopRight = 3,
        /// <summary>
        /// 
        /// </summary>
        Right = 4,
        /// <summary>
        /// 
        /// </summary>
        Left = 5
    }

    /// <summary>
    /// Describes an object that can be used to resolve references to a control by its ID
    /// </summary>
    public interface IControlResolver
    {
        /// <summary>
        /// Resolves a reference to a control by its ID
        /// </summary>
        /// <param name="controlID">The control id to resolve</param>
        /// <returns>The web control</returns>
        System.Web.UI.Control ResolveControl(string controlID);
    }

    /// <summary>
    /// The JSON Data Table Service object.
    /// </summary>
    [Serializable()]
    public class JSonDataTableService
    {
        #region JSon DataTable Service
        /// <summary>
        /// Gets sets, information for datatables to use for rendering.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string sEcho
        {
            get; set;
        }

        /// <summary>
        /// Gets sets, total records
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int iTotalRecords
        {
            get; set;
        }

        /// <summary>
        /// Gets sets, total records after filtering
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int iTotalDisplayRecords
        {
            get; set;
        }

        /// <summary>
        /// Gets sets, the two dimensional array of data.
        /// </summary>
        [XmlArray(IsNullable = true)]
        public string[,] aaData
        {
            get; set;
        }
        #endregion
    }
}
