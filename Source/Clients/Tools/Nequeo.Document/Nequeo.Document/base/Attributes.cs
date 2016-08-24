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
using System.Collections.ObjectModel;

namespace Nequeo.Document
{
    /// <summary>
    /// Custom excel row column attribute.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ExcelRowColumnAttribute : System.Attribute
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="column">The column index</param>
        public ExcelRowColumnAttribute(int row, int column)
        {
            _row = row;
            _column = column;
        }
        #endregion

        #region Private Fields
        private string _name = string.Empty;
        private int _row = 1;
        private int _column = 1;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the column name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets sets, the row index.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        /// <summary>
        /// Gets sets, the column index.
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }
        #endregion
    }

    /// <summary>
    /// Custom word column attribute.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class WordColumnAttribute : System.Attribute
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">The column name.</param>
        public WordColumnAttribute(string name)
        {
            _name = name;
        }
        #endregion

        #region Private Fields
        private string _name = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the column name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
    }
}
