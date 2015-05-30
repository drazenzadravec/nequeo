/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;

namespace Nequeo.Management
{
    /// <summary>
    /// Python automation provider.
    /// </summary>
    public sealed class PythonEx
    {
        /// <summary>
        /// Python automation provider.
        /// </summary>
        public PythonEx() 
        {
            _pythonEngine = Python.CreateEngine();
            _pythonScope = _pythonEngine.CreateScope();
        }

        private ScriptEngine _pythonEngine = null;
        private ScriptScope _pythonScope = null;

        /// <summary>
        /// Gets or sets the python engine.
        /// </summary>
        public ScriptEngine PythonEngine
        {
            get { return _pythonEngine; }
            set { _pythonEngine = value; }
        }

        /// <summary>
        /// Gets or sets the python scope
        /// </summary>
        public ScriptScope PythonScope
        {
            get { return _pythonScope; }
            set { _pythonScope = value; }
        }

        /// <summary>
        /// Create a reference to a python delegate.
        /// </summary>
        /// <typeparam name="D">The delegate type.</typeparam>
        /// <param name="delegateName">The delegate name.</param>
        /// <returns>The reference to the delegate.</returns>
        /// <exception cref="System.ArgumentNullException">Delegate name can not be null, python engine can not be null, python scope can not be null.</exception>
        public D ReferenceDelegate<D>(string delegateName)
        {
            if (String.IsNullOrEmpty(delegateName)) throw new ArgumentNullException("delegateName");

            // Get the reference to the python delegate.
            D pyDelegate = _pythonScope.GetVariable<D>(delegateName);
            return pyDelegate;
        }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        /// <typeparam name="T">The class type to create.</typeparam>
        /// <param name="className">The name of the class to create (e.g. Class()).</param>
        /// <returns>The new instance of the class.</returns>
        /// <exception cref="System.ArgumentNullException">Class name can not be null, python engine can not be null, python scope can not be null.</exception>
        public T CreateClass<T>(string className)
        {
            if (String.IsNullOrEmpty(className)) throw new ArgumentNullException("className");

            // Create the instance of the class.
            T instance = (T)_pythonEngine.Execute(className, _pythonScope);
            return instance;
        }

        /// <summary>
        /// Execute the member within the instance.
        /// </summary>
        /// <typeparam name="C">The class type instance.</typeparam>
        /// <param name="instance">The class instance.</param>
        /// <param name="memberName">The member name to execute.</param>
        /// <param name="parameters">The parameters for the member.</param>
        /// <returns>The returned value after execution.</returns>
        /// <exception cref="System.ArgumentNullException">Instance can not be null, member name can not be null, python engine can not be null, python scope can not be null.</exception>
        public void ExecuteMember<C>(C instance, string memberName, params object[] parameters)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (String.IsNullOrEmpty(memberName)) throw new ArgumentNullException("memberName");

            // Exacute the member.
            var ops = _pythonEngine.CreateOperations(_pythonScope);
            object result = ops.InvokeMember(instance, memberName, parameters);
        }

        /// <summary>
        /// Execute the member within the instance.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <typeparam name="C">The class type instance.</typeparam>
        /// <param name="instance">The class instance.</param>
        /// <param name="memberName">The member name to execute.</param>
        /// <param name="parameters">The parameters for the member.</param>
        /// <returns>The returned value after execution.</returns>
        /// <exception cref="System.ArgumentNullException">Instance can not be null, member name can not be null, python engine can not be null, python scope can not be null.</exception>
        public T ExecuteMember<T, C>(C instance, string memberName, params object[] parameters)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (String.IsNullOrEmpty(memberName)) throw new ArgumentNullException("memberName");

            // Exacute the member.
            var ops = _pythonEngine.CreateOperations(_pythonScope);
            T result = (T)ops.InvokeMember(instance, memberName, parameters);
            return result;
        }

        /// <summary>
        /// Compile and execute the script source.
        /// </summary>
        /// <param name="source">The script source to compile and execute.</param>
        /// <exception cref="System.ArgumentNullException">Source can not be null, python scope can not be null.</exception>
        public void CompileAndExecute(ScriptSource source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (_pythonScope == null) throw new ArgumentNullException("PythonScope");

            // Compile the source code.
            CompiledCode compiled = source.Compile();

            // Executes in the scope of Python
            compiled.Execute(_pythonScope);
        }

        /// <summary>
        /// Compile and execute the source code.
        /// </summary>
        /// <param name="sourceCode">The source code to compile and execute.</param>
        /// <exception cref="System.ArgumentNullException">Source can not be null, python scope can not be null.</exception>
        public void CompileAndExecute(string sourceCode)
        {
            if (String.IsNullOrEmpty(sourceCode)) throw new ArgumentNullException("sourceCode");
            if (_pythonScope == null) throw new ArgumentNullException("PythonScope");

            // Compile the source code.
            ScriptSource source = _pythonEngine.CreateScriptSourceFromString(sourceCode, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile();

            // Executes in the scope of Python
            compiled.Execute(_pythonScope);
        }

        /// <summary>
        /// Compile and execute the source code within a file.
        /// </summary>
        /// <param name="filename">The path and filename of the file that contains the script.</param>
        /// <exception cref="System.ArgumentNullException">Source can not be null, python scope can not be null.</exception>
        public void CompileAndExecuteFile(string filename)
        {
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename");
            if (_pythonScope == null) throw new ArgumentNullException("PythonScope");

            FileStream fileStream = null;
            StreamReader streamReader = null;
            string sourceCode = null;

            try
            {
                // Open the file stream.
                using (fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (streamReader = new StreamReader(fileStream))
                {
                    // Read all the script data in the file.
                    sourceCode = streamReader.ReadToEnd();

                    // Close the streams.
                    streamReader.Close();
                    fileStream.Close();
                }

                // Compile the source code.
                ScriptSource source = _pythonEngine.CreateScriptSourceFromString(sourceCode, SourceCodeKind.Statements);
                CompiledCode compiled = source.Compile();

                // Executes in the scope of Python
                compiled.Execute(_pythonScope);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Dispose();

                if (fileStream != null)
                    fileStream.Dispose();
            }
        }
    }
}
