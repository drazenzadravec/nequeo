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
    /// Fraction.
    /// </summary>
    // note: it might be worthwhile somehow differentiating between fraction and
    // ordinary division, e.g., having fraction parts evaluate as temporary variables.
    public class Mfrac : WithBinaryContent
    {
        public Mfrac(IBuildable first, IBuildable second) : base(first, second) { }

        public override void Visit(StringBuilder sb, BuildContext bc)
        {
            bc.Tokens.Add(this);

            bool needReduce = false;
            // note: the use of double is a judgement call here
            double result = 0.0;
            if (bc.Options.ReduceFractions)
            {
                Mrow row1 = first as Mrow;
                Mrow row2 = second as Mrow;
                if (row1 != null && row2 != null && row1.Contents.Length > 0 && row2.Contents.Length > 0)
                {
                    Mn mn1 = row1.Contents[0] as Mn;
                    Mn mn2 = row2.Contents[0] as Mn;
                    if (mn1 != null && mn2 != null)
                    {
                        try
                        {
                            double _1, _2;
                            if (double.TryParse(mn1.Content, out _1) &&
                                double.TryParse(mn2.Content, out _2) &&
                                _2 != 0.0)
                            {
                                result = _1 / _2;
                                needReduce = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            if (needReduce)
            {
                sb.Append(result);
            }
            else
            {
                sb.Append("((");
                first.Visit(sb, bc);
                sb.Append(") / (");
                second.Visit(sb, bc);
                sb.Append("))");
            }
        }
    }
}