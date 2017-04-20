/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
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

using System.Text;

namespace Nequeo.MathML.Entities
{
    /// <summary>
    /// A fairly generic element that does not mean anything.
    /// </summary>
    public class Mrow : WithBuildableContents
    {
        public Mrow(IBuildable[] content) : base(content)
        {

        }

        public Mrow(IBuildable first, IBuildable second) : this(new[] { first, second })
        {
        }

        public Mrow(IBuildable content) : this(new[] { content })
        {
        }

        public IBuildable[] Contents
        {
            get { return contents; }
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="Mrow"/> contains a single <see cref="Mi"/>.
        /// </summary>
        public bool ContainsSingleMi
        {
            get
            {
                return contents.Length == 1 && contents[0] is Mi;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Mrow contains a single Mn.
        /// </summary>
        /// <value><c>true</c> if this row contains a single Mn; otherwise, <c>false</c>.</value>
        public bool ContainsSingleMn
        {
            get
            {
                return contents.Length == 1 && contents[0] is Mn;
            }
        }

        public override void Visit(StringBuilder sb, BuildContext context)
        {
            base.Visit(sb, context);
        }
    }
}