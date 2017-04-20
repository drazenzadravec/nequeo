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

using System;
using System.Text;

namespace Nequeo.MathML.Entities
{
    /// <summary>
    /// Superscript. Taken to mean 'power of' unless the superscript parses into a *.
    /// </summary>
    public class Msup : WithBinaryContent
    {
        public Msup(IBuildable first, IBuildable second) : base(first, second) { }

        public Tuple<IBuildable, IBuildable> Values
        {
            get
            {
                return new Tuple<IBuildable, IBuildable>(first, second);
            }
        }

        public override void Visit(StringBuilder sb, BuildContext bc)
        {
            if (bc.Options.SubscriptMode)
            {
                // separate by double underscore to differentiate from Msub and prevent clashes
                first.Visit(sb, bc);
                sb.Append("__");
                second.Visit(sb, bc);
            }
            else if (second is Mo && (second as Mo).IsTimesOrStar && first is Mi)
            {
                // this isn't really a superscript - it's part of the variable
                Mi mi = (Mi)first;
                Mi newMi = new Mi(mi.Content + Semantics.starPrefix);
                newMi.Visit(sb, bc);
            }
            else
            {
                if (bc.LastTokenRequiresTimes)
                    sb.Append("*");

                // determine whether power must be inlined
                bool firstIsMrowMi = (first is Mrow) && ((first as Mrow).ContainsSingleMi);
                bool secondIsIntegralPower = (second is Mrow) && ((second as Mrow).ContainsSingleMn) &&
                                             ((second as Mrow).LastElement as Mn).IsIntegerGreaterThan1;
                int power = ((second as Mrow).LastElement as Mn).IntegerValue;
                bool mustInlinePower = power <= bc.Options.MaxInlinePower;
                if (mustInlinePower)
                {
                    for (int i = 0; i < power; ++i)
                    {
                        first.Visit(sb, bc); // * sign appended automatically
                    }
                }
                else
                {
                    sb.Append("Math.Pow(");
                    first.Visit(sb, bc);
                    sb.Append(", ");
                    second.Visit(sb, bc);
                    sb.Append(")");
                }
            }

            bc.Tokens.Add(this);
        }
    }
}