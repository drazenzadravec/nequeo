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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Nequeo.MathML.Entities
{
    /// <summary>
    /// Subscript and superscript one one variable.
    /// </summary>
    public class Msubsup : WithBuildableContents
    {
        public Msubsup(IBuildable[] contents)
        { // handle star superscript
            List<IBuildable> localCopy = new List<IBuildable>(contents);
            if (localCopy.Count == 3)
            {
                Mo mo = localCopy[2] as Mo;
                if (mo != null && mo.IsTimesOrStar)
                {
                    Mi subscript = localCopy[1] as Mi;
                    if (subscript != null)
                    {
                        subscript.Content += Semantics.starPrefix;
                        localCopy.RemoveAt(2);
                    }
                    else
                    {
                        // maybe the subscript is an mrow
                        Mrow row = localCopy[1] as Mrow;
                        if (row != null && row.LastElement != null && row.LastElement is WithTextContent)
                        {
                            WithTextContent lastElem = (WithTextContent)row.LastElement;
                            lastElem.Content += Semantics.starPrefix;
                            localCopy.RemoveAt(2);
                        }
                    }
                }
            }
            base.contents = localCopy.ToArray();
        }
        public override void Visit(StringBuilder sb, BuildContext context)
        {
            Msub sub = new Msub(contents[0], contents[1]);
            switch (contents.Length)
            {
                case 2:
                    sub.Visit(sb, context);
                    break;
                case 3:
                    Msup sup = new Msup(sub, contents[2]);
                    sup.Visit(sb, context);
                    break;
                default:
                    throw new ApplicationException("Incorrect number of arguments in Msubsup");
            }
        }
    }
}