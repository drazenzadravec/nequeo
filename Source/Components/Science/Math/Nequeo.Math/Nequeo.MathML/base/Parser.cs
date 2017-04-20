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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Nequeo.MathML.Entities;
using Math = Nequeo.MathML.Entities.Math;

namespace Nequeo.MathML
{
    public static class Parser
    {
        /// <summary>
        /// Capitalizes the initial letter of an identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The modified identifier.</returns>
        internal static string camel(string id)
        {
            return "" + char.ToUpper(id[0], CultureInfo.InvariantCulture) + id.Substring(1);
        }

        /// <summary>
        /// Parses the XML tree and returns the root <c>Math</c> element.
        /// </summary>
        /// <param name="rootElement">The root <c>XElement</c></param>
        /// <returns>A parsed tree if everything went ok; <c>null</c> otherwise.</returns>
        public static Math Parse(XElement rootElement)
        {
            return GenericParse(rootElement) as Math;
        }

        /// <summary>
        /// Parse the math ML xml data.
        /// </summary>
        /// <param name="mathML">The math ML xml data.</param>
        /// <returns>The math expression; else null;</returns>
        public static string Parse(string mathML)
        {
            // try parsing the root
            XElement root;
            string s = null;

            try
            {
                root = XElement.Parse(mathML);
                BuildContextOptions buildOptions = new BuildContextOptions();

                var m = Parser.Parse(root);
                var sb = new StringBuilder();
                m.Visit(sb, new BuildContext(buildOptions));
                s = sb.ToString();

                bool useHacks = true;

                if (useHacks)
                {

                    // hack: evil hacks until problems are resolved
                    s = s.Replace("(*", "(")
                         .Replace(")Math", ")*Math")
                         .Replace(")(", ")*(")
                         .Replace("**", "*");

                    // hack: even nastier hack here
                    Regex re = new Regex(@"(\d)(\()");
                    foreach (Match match in re.Matches(s))
                    {
                        string toReplace = match.Value;
                        string toReplaceWith = match.Groups[1].Value + "*" + match.Groups[2].Value;
                        s = s.Replace(toReplace, toReplaceWith);
                    }

                }
            }
            catch { s = null; }
            return s;
        }

        /// <summary>
        /// Parses the element and creates a parse tree
        /// </summary>
        /// <param name="element">The element to parse from.</param>
        /// <returns>The root of the parse tree.</returns>
        public static IBuildable GenericParse(XElement element)
        {
            // make children recursively depending on the number of contained elements
            var elems = element.Elements();
            var c = camel(element.Name.LocalName);
            var t = Type.GetType("Nequeo.MathML.Entities." + c);
            if (t == null)
            {
                return null;
            }
            switch (elems.Count())
            {
                case 0: // return text
                    return Activator.CreateInstance(t, element.Value) as IBuildable;

                case 1:
                    var first = GenericParse(elems.First());
                    return Activator.CreateInstance(t, first) as IBuildable;

                case 2:
                    var pair = elems.Select(e => GenericParse(e)).ToArray();
                    IBuildable result;
                    // some cases require a binary constructor; other cases requre an N-ary one instead
                    try
                    {
                        result = Activator.CreateInstance(t, pair[0], pair[1]) as IBuildable;
                    }
                    catch
                    {
                        result = Activator.CreateInstance(t, new[] { pair }) as IBuildable;
                    }
                    return result;

                default: // has a single item
                    var pars = elems.Select(e => GenericParse(e)).ToArray();
                    return Activator.CreateInstance(t, new[] { pars }) as IBuildable;
            }
        }
    }
}
