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
using System.IO;
using System.Text;
using System.Security;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Policy;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Remoting;
using System.Reflection.Emit;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Remoting.Activation;

namespace Nequeo.Reflection
{
    /// <summary>
    /// Application domain marshal.
    /// </summary>
    public sealed class AppDomianMarshal : MarshalByRefObject
    {
        /// <summary>
        /// Application domain marshal.
        /// </summary>
        public AppDomianMarshal() { }

        /// <summary>
        /// Get the type.
        /// </summary>
        /// <param name="assemblyName">The assembly given the long form of its name.</param>
        /// <param name="typeName">The namespace and type.</param>
        /// <returns>The type.</returns>
        private Type GetType(string assemblyName, string typeName)
        {
            // Get the class type.
            return Type.GetType(typeName + "," + assemblyName);
        }

        /// <summary>
        /// Create a new instance of the type.
        /// </summary>
        /// <param name="type">The type to create,</param>
        /// <param name="constructorParameters">The constructor parameters of the type.</param>
        /// <returns>The instance.</returns>
        private object CreateInstance(Type type, params Object[] constructorParameters)
        {
            return Nequeo.Reflection.TypeAccessor.CreateInstance(type, constructorParameters);
        }

        /// <summary>
        /// Create a new instance of the type.
        /// </summary>
        /// <param name="assemblyName">The assembly given the long form of its name.</param>
        /// <param name="typeName">The namespace and type.</param>
        /// <param name="constructorParameters">The constructor parameters of the type.</param>
        /// <returns>The instance.</returns>
        private object CreateInstance(string assemblyName, string typeName, params Object[] constructorParameters)
        {
            return CreateInstance(GetType(assemblyName, typeName), constructorParameters);
        }

        /// <summary>
        /// Execute the code within the host.
        /// </summary>
        /// <param name="assemblyName">The assembly given the long form of its name.</param>
        /// <param name="typeName">The namespace and type.</param>
        /// <param name="entryPoint">The member name to execute.</param>
        /// <param name="parameters">The parameters of the member.</param>
        /// <param name="constructorParameters">The constructor parameters.</param>
        public void ExecuteMethod(string assemblyName, string typeName, string entryPoint, Object[] parameters, params Object[] constructorParameters)
        {
            try
            {
                // Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
                // you can use Assembly.EntryPoint to get to the main function in an executable.
                MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);

                // If is static
                if (target.IsStatic)
                {
                    // Now invoke the method.
                    target.Invoke(null, parameters);
                }
                else
                {
                    // Now invoke the method.
                    target.Invoke(CreateInstance(GetType(assemblyName, typeName), constructorParameters), parameters);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute the code within the host.
        /// </summary>
        /// <typeparam name="T">The entry point member return type.</typeparam>
        /// <param name="assemblyName">The assembly given the long form of its name.</param>
        /// <param name="typeName">The namespace and type.</param>
        /// <param name="entryPoint">The member name to execute.</param>
        /// <param name="parameters">The parameters of the member.</param>
        /// <param name="constructorParameters">The constructor parameters.</param>
        /// <returns>The return value.</returns>
        public T ExecuteMethod<T>(string assemblyName, string typeName, string entryPoint, Object[] parameters, params Object[] constructorParameters)
        {
            try
            {
                // Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
                // you can use Assembly.EntryPoint to get to the main function in an executable.
                MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);

                // If is static
                if (target.IsStatic)
                {
                    // Now invoke the method.
                    T retVal = (T)target.Invoke(null, parameters);
                    return retVal;
                }
                else
                {
                    // Now invoke the method.
                    T retVal = (T)target.Invoke(CreateInstance(GetType(assemblyName, typeName), constructorParameters), parameters);
                    return retVal;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
