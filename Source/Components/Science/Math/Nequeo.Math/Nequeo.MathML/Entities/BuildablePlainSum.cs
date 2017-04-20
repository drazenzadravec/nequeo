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
    public class BuildablePlainSum : ISum
    {
        private readonly IBuildable expression;
        public BuildablePlainSum(IBuildable expression)
        {
            this.expression = expression;
        }

        #region ISum Members

        public void Visit(StringBuilder sb, BuildContext bc)
        {
            sb.Append(bc.GetSumIdentifier(this));
        }

        public string Expression(BuildContext context)
        {
            // get the target
            StringBuilder target = new StringBuilder();
            expression.Visit(target, context);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("for (int i{0} = 0; i{0} < {1}.Length; ++i{0})",
                            context.GetSumIdentifier(this), target);
            sb.AppendLine();
            sb.Append("  ");
            sb.AppendFormat("{0} += {1}[i{0}];", context.GetSumIdentifier(this), target);
            return sb.ToString();
        }

        #endregion
    }
}
