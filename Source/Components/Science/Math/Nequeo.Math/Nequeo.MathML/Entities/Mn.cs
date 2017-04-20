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
    /// Number.
    /// </summary>
    public class Mn : WithTextContent
    {
        public Mn(string content) : base(content) { }

        /// <summary>
        /// Returns <c>true</c> if the content is an integer greater than 1 (i.e., 2, 3, etc.)
        /// </summary>
        public bool IsIntegerGreaterThan1
        {
            get
            {
                try
                {
                    double d = double.Parse(content);
                    if (System.Math.Floor(d) == d)
                        if (d > 1.0)
                            return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public int IntegerValue
        {
            get
            {
                return int.Parse(content);
            }
        }

        public override void Visit(StringBuilder sb, BuildContext bc)
        {
            base.Visit(sb, bc);
            if (bc.Options.NumberPostfix && !bc.Options.SubscriptMode)
                sb.Append(Semantics.postfixForDataType(bc.Options.EqnDataType));
        }
    }
}