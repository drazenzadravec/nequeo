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
    /// Root element in MathML. Not to be confused with <c>System.Math</c>.
    /// </summary>
    public class Math : WithBuildableContents
    {
        public Math(string content) : base(new IBuildable[] { }) { /* just in case */ }
        public Math(IBuildable content) : base(new[] { content }) { }
        public Math(IBuildable[] contents) : base(contents) { }
        public override void Visit(StringBuilder sb, BuildContext context)
        {
            base.Visit(sb, context);

            // todo: this assumes that there is only one statement, which may not be the case
            if (sb.ToString().Length > 0)
                sb.Append(";");

            // sums
            int j = context.Sums.Count;
            if (j > 0)
            {
                var builder = new StringBuilder();
                foreach (var v in context.Sums)
                {
                    builder.AppendLine(v.Item1.Expression(context));
                }
                sb.Insert(0, builder.ToString());
            }

            // variables
            int i = context.Vars.Count;
            if (i > 0)
            {
                var builder = new StringBuilder();
                foreach (var v in context.Vars)
                {
                    builder.Append(Enum.GetName(typeof(EquationDataType), context.Options.EqnDataType).ToLower());
                    builder.Append(" ");
                    builder.Append(v);
                    builder.Append(" = 0.0"); // this is a *must*
                    if (context.Options.NumberPostfix)
                        builder.Append(Semantics.postfixForDataType(context.Options.EqnDataType));
                    builder.AppendLine(";");
                }
                sb.Insert(0, builder.ToString());
            }
        }
    }
}