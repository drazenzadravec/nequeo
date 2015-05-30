/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Nequeo.Xml.CodeGeneration
{
    /// <summary>
    /// Automatically convert valid XML to the corresponding LINQ to XML statements.
    /// </summary>
    public sealed class XmlToLinqConverter : IDisposable
    {
        private XElement _root;
        private List<XNamespace> _namespaces;
        private List<string> _prefixes;
        private StringBuilder _sb;
        private CodeDomProvider _cp;
        private int _pos;
        private int _indent;

        /// <summary>
        /// Can the xml be converted.
        /// </summary>
        /// <param name="xml">The xml document to examine.</param>
        /// <returns>True if the xml is valide.</returns>
        public static bool CanConvert(string xml)
        {
            bool result = false;
            try
            {
                XElement.Parse(xml);
                result = true;
            }
            catch { }
            
            return result;
        }

        /// <summary>
        /// Convert the xml to the equivalent linq to xml code.
        /// </summary>
        /// <param name="xml">The xml document to convert.</param>
        /// <returns>The string equivalent linq to xml code.</returns>
        public static string Convert(string xml)
        {
            return new XmlToLinqConverter().GetSourceCode(xml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        string GetSourceCode(string xml)
        {
            _root = XElement.Parse(xml);
            _namespaces = new List<XNamespace>();
            _prefixes = new List<string>();
            _sb = new StringBuilder();
            _cp = new CSharpCodeProvider();
            FindNamespaces();
            WriteXNamespaces();
            WriteXElement();
            return _sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns"></param>
        void AddNamespace(XNamespace ns)
        {
            if (ns != XNamespace.None &&
                ns != XNamespace.Xml &&
                ns != XNamespace.Xmlns &&
                !_namespaces.Contains(ns))
            {
                _namespaces.Add(ns);
                _prefixes.Add(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void FindNamespaces()
        {
            foreach (XElement e in _root.DescendantsAndSelf())
            {
                AddNamespace(e.Name.Namespace);
                foreach (XAttribute a in e.Attributes())
                {
                    AddNamespace(a.Name.Namespace);
                }
            }
            foreach (XAttribute a in _root.DescendantsAndSelf().Attributes())
            {
                if (a.IsNamespaceDeclaration && a.Name.LocalName != "xmlns")
                {
                    int i = _namespaces.IndexOf(a.Value);
                    if (i >= 0 && _prefixes[i] == null)
                    {
                        string s = a.Name.LocalName.Replace('.', '_').Replace('-', '_');
                        if (!_cp.IsValidIdentifier(s))
                        {
                            s = '_' + s;
                            if (!_cp.IsValidIdentifier(s)) s = null;
                        }
                        _prefixes[i] = s;
                    }
                }
            }
            for (int i = 0; i < _prefixes.Count; i++)
                if (_prefixes[i] == null) _prefixes[i] = "ns";
            for (int i = 0; i < _prefixes.Count - 1; i++)
            {
                string s = _prefixes[i];
                for (int j = i + 1; j < _prefixes.Count; j++)
                {
                    if (s == _prefixes[j])
                    {
                        int n = 1;
                        for (int k = i; k < _prefixes.Count; k++)
                        {
                            if (s == _prefixes[k]) _prefixes[k] = s + n++;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        static bool IsSingleLineText(XElement e)
        {
            if (e.HasAttributes) return false;
            foreach (XNode node in e.Nodes())
                if (!(node is XText) || ((XText)node).Value.Contains("\n")) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        void Write(char ch)
        {
            if (_pos < _indent)
            {
                _sb.Append(' ', _indent - _pos);
                _pos = _indent;
            }
            _sb.Append(ch);
            _pos++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        void Write(string s)
        {
            if (_pos < _indent)
            {
                _sb.Append(' ', _indent - _pos);
                _pos = _indent;
            }
            _sb.Append(s);
            _pos += s.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void WriteName(XName name)
        {
            if (name.Namespace != XNamespace.None)
            {
                if (name.Namespace == XNamespace.Xmlns)
                {
                    if (name.LocalName != "xmlns")
                        Write("XNamespace.Xmlns + ");
                }
                else if (name.Namespace == XNamespace.Xml)
                {
                    Write("XNamespace.Xml + ");
                }
                else
                {
                    Write(_prefixes[_namespaces.IndexOf(name.Namespace)]);
                    Write(" + ");
                }
            }
            WriteStringLiteral(name.LocalName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void WriteNewElement(XElement e)
        {
            Write("new XElement(");
            WriteName(e.Name);
            if (!e.IsEmpty || e.HasAttributes)
            {
                if (IsSingleLineText(e))
                {
                    Write(", ");
                    WriteStringLiteral(e.Value);
                }
                else
                {
                    _indent += 4;
                    foreach (XAttribute a in e.Attributes())
                    {
                        Write(",");
                        WriteNewLine();
                        Write("new XAttribute(");
                        WriteName(a.Name);
                        Write(", ");
                        int i = a.IsNamespaceDeclaration ? _namespaces.IndexOf(a.Value) : -1;
                        if (i >= 0)
                        {
                            Write(_prefixes[i]);
                        }
                        else
                        {
                            WriteStringLiteral(a.Value);
                        }
                        Write(")");
                    }
                    foreach (XNode node in e.Nodes())
                    {
                        Write(",");
                        WriteNewLine();
                        if (node is XText)
                        {
                            WriteStringLiteral(((XText)node).Value);
                        }
                        else if (node is XElement)
                        {
                            WriteNewElement((XElement)node);
                        }
                        else if (node is XComment)
                        {
                            WriteNewObject("XComment", null, ((XComment)node).Value);
                        }
                        else if (node is XProcessingInstruction)
                        {
                            XProcessingInstruction pi = (XProcessingInstruction)node;
                            WriteNewObject("XProcessingInstruction", pi.Target, pi.Data);
                        }
                    }
                    WriteNewLine();
                    _indent -= 4;
                }
            }
            Write(")");
        }

        /// <summary>
        /// 
        /// </summary>
        void WriteNewLine()
        {
            _sb.Append("\r\n");
            _pos = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void WriteNewObject(string type, string name, string value)
        {
            Write("new ");
            Write(type);
            Write("(");
            if (name != null) WriteStringLiteral(name);
            if (!value.Contains("\n"))
            {
                if (name != null) Write(", ");
                WriteStringLiteral(value);
            }
            else
            {
                if (name != null) Write(",");
                WriteNewLine();
                _indent += 4;
                WriteStringLiteral(value);
                WriteNewLine();
                _indent -= 4;
            }
            Write(")");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        void WriteStringLiteral(string s)
        {
            Write('"');
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if (ch >= ' ')
                {
                    if (ch == '\\' || ch == '"') Write('\\');
                    Write(ch);
                }
                else if (ch == '\t')
                {
                    Write("\\t");
                }
                else if (ch == '\r')
                {
                    Write("\\r");
                }
                else if (ch == '\n')
                {
                    Write("\\n");
                    if (s.Length - i > 1)
                    {
                        Write("\" +");
                        WriteNewLine();
                        Write('"');
                    }
                }
            }
            Write('"');
        }

        /// <summary>
        /// 
        /// </summary>
        void WriteXElement()
        {
            Write("XElement xml = ");
            WriteNewElement(_root);
            Write(";");
            WriteNewLine();
        }

        /// <summary>
        /// 
        /// </summary>
        void WriteXNamespaces()
        {
            for (int i = 0; i < _namespaces.Count; i++)
            {
                Write("XNamespace ");
                Write(_prefixes[i]);
                Write(" = ");
                WriteStringLiteral(_namespaces[i].NamespaceName);
                Write(";");
                WriteNewLine();
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(_cp != null)
                        _cp.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _cp = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~XmlToLinqConverter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
