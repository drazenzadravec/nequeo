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

using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Nequeo.MathML.Entities
{
    /// <summary>
    /// Operator.
    /// </summary>
    public class Mo : WithTextContent
    {
        private static readonly StringDictionary rep;
        static Mo()
        {
            // replacements in lieu of the actual operator
            rep = new StringDictionary
      {
        {"!", ".Factorial()"},
        {"-", "-"}, // this one is *really* nasty :)
        {"−", "-"}, // this one is also annoying (thanks MathType)
        {"÷", "/"},
        {"×", "*"},
        {"∗", "*"}, // << unpleasant too!
        {"[", "("},
        {"]", ")"},
        {"{", "("},
        {"}", ")"}
      };
        }

        public Mo(string content) : base(content) { }

        /// <summary>
        /// Returns <c>true</c> if this is an equals operator.
        /// </summary>
        /// <remarks>
        /// Useful for determining whether we are on the RHS or LHS of an assignment.
        /// </remarks>
        public bool IsEquals
        {
            get
            {
                return content == "=";
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this is a closing brace.
        /// </summary>
        public bool IsClosingBrace
        {
            get
            {
                return content == ")";
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this is an opening brace.
        /// </summary>
        public bool IsOpeningBrace
        {
            get
            {
                return content == "(";
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this is a printable operator.
        /// </summary>
        public bool IsPrintableOperator
        {
            get
            {
                return (content != "\u2061" && content != "\u2062" && content != "\u2063");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the content is uppercase sigma.
        /// </summary>
        /// <value><c>true</c> if the content is sigma; otherwise, <c>false</c>.</value>
        public bool IsSigma
        {
            get
            {
                return content == "∑";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the content is uppercase delta.
        /// </summary>
        /// <value><c>true</c> if the content is delta; otherwise, <c>false</c>.</value>
        public bool IsDelta
        {
            get
            {
                return content == "∆";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is times or star.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is times or star; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimesOrStar
        {
            get
            {
                return "*∗".Contains(content);
            }
        }

        public override void Visit(StringBuilder sb, BuildContext bc)
        {
            bc.Tokens.Add(this);

            // if we are in subscript mode, adding an operator is not necessary, but neither can we ignore it
            if (bc.Options.SubscriptMode)
            {
                sb.Append("_");
            }
            else if (rep.ContainsKey(content))
                sb.Append(rep[content]);
            else
            {
                if (IsPrintableOperator)
                {
                    if (IsOpeningBrace && bc.LastTokenRequiresTimes)
                        sb.Append("*");

                    sb.Append(content);
                }
            }
        }
    }
}