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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Nequeo.MathML.Entities;

namespace Nequeo.MathML
{
    /// <summary>
    /// Represents runtime data that is kept while the code is being
    /// assembled from the model.
    /// </summary>
    public sealed class BuildContext
    {
        private readonly IList<String> errors = new List<String>();
        private readonly List<Tuple<ISum, char>> sums = new List<Tuple<ISum, char>>();
        private readonly IList tokens = new ArrayList();
        private readonly ICollection<String> vars = new SortedSet<String>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContext"/> class.
        /// </summary>
        public BuildContext() : this(new BuildContextOptions()) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContext"/> class.
        /// </summary>
        /// <param name="options">Build options.</param>
        internal BuildContext(BuildContextOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Options used for building the code.
        /// </summary>
        internal BuildContextOptions Options { get; private set; }

        /// <summary>
        /// Errors encountered during build.
        /// </summary>
        public IList<string> Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// Variables that have been defined during build.
        /// </summary>
        public ICollection<string> Vars
        {
            get { return vars; }
        }

        /// <summary>
        /// Tokens that have been met during build.
        /// </summary>
        public IList Tokens
        {
            get { return tokens; }
        }

        /// <summary>
        /// Returns the last token encountered, or <c>null</c> if there are none.
        /// </summary>
        public Object LastToken
        {
            get
            {
                return tokens.Count > 0 ? tokens[tokens.Count - 1] : null;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if last token suggests the use of times before identifier.
        /// </summary>
        public bool LastTokenRequiresTimes
        {
            get
            {
                if (LastToken == null) return false;

                // times is required in all cases, except when
                // - last token was an operator and not a closing brace
                object t = LastToken;
                bool isMo = t is Mo;
                bool isClosing = false;
                if (isMo)
                    isClosing = ((Mo)t).IsClosingBrace;

                if (isClosing)
                {
                    return true;
                }
                if (isMo)
                {
                    return false;
                }

                if (t is Msup | t is Mrow) return false;

                return true;
            }
        }

        /// <summary>
        /// Indicates whether we are on the right-hand side (after =) of the equation.
        /// </summary>
        public bool OnRhs
        {
            get
            {
                // note: completely wrong. = can appear in, e.g., sum subscripts
                foreach (var v in tokens)
                    if (v is Mo && (v as Mo).IsEquals)
                        return true;
                return false;
            }
        }


        public IList<Tuple<ISum, char>> Sums
        {
            get
            {
                return sums;
            }
        }

        /// <summary>
        /// Adds a sum.
        /// </summary>
        /// <param name="sum">The sum.</param>
        public void AddSum(ISum sum)
        {
            for (char c = 'a'; c < 'z'; ++c)
            {
                char c1 = c;
                if (!vars.Contains(c1.ToString()) &&
                  sums.FindIndex(i => i.Item2 == c1) == -1)
                {
                    sums.Add(new Tuple<ISum, char>(sum, c));
                    vars.Add(c.ToString());
                    return;
                }
            }
            // todo: make this more civil later on
            throw new Exception("Out of variables!");
        }

        public char GetSumIdentifier(ISum sum)
        {
            int idx = sums.FindIndex(i => i.Item1 == sum);
            if (idx != -1)
                return sums[idx].Item2;
            else return '?';
        }
    }

    internal class BuildContextSum
    {
        private IBuildable[] initTokens;
        private IBuildable[] limitTokens;
        private IBuildable[] statement;
    }

    public enum EquationDataType
    {
        Float,
        Double,
        Decimal
    }
}
