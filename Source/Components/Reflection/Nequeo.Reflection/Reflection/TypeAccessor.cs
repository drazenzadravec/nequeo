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
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Linq.Expressions;

using Nequeo.Runtime;
using Nequeo.Extension;

namespace Nequeo.Reflection
{
    /// <summary>
    /// Represents type declarations.
    /// </summary>
    public sealed class TypeAccessor
    {
        /// <summary>
        /// Create an instance of the type.
        /// </summary>
        /// <param name="properties">The dynamic property array.</param>
        /// <returns>The instantiated type.</returns>
        public static object CreateInstance(params DynamicProperty[] properties)
        {
            // Make sure the page reference exists.
            if (properties == null) throw new ArgumentNullException("properties");

            return new DynamicTypeBuilder("ModuleImplementation").Create("TypeImplementation", properties);
        }

        /// <summary>
        /// Create an instance of the type.
        /// </summary>
        /// <param name="properties">The dynamic property array.</param>
        /// <returns>The instantiated type.</returns>
        public static object CreateInstance(IEnumerable<DynamicProperty> properties)
        {
            // Make sure the page reference exists.
            if (properties == null) throw new ArgumentNullException("properties");

            return new DynamicTypeBuilder("ModuleImplementation").Create("TypeImplementation", properties);
        }

        /// <summary>
        /// Create an instance of the type.
        /// </summary>
        /// <param name="type">The type to create an instance of.</param>
        /// <param name="parameters">The constructor parameters of the type.</param>
        /// <returns>The instantiated type.</returns>
        public static object CreateInstance(Type type, params Object[] parameters)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");

            return Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Create an instance of the type.
        /// </summary>
        /// <param name="assemblyName">The assembly given the long form of its name.</param>
        /// <param name="typeName">The namespace and type.</param>
        /// <returns>The instantiated type.</returns>
        public static object CreateInstance(string assemblyName, string typeName)
        {
            // Make sure the page reference exists.
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            if (typeName == null) throw new ArgumentNullException("typeName");

            ObjectHandle handle = Activator.CreateInstance(assemblyName, typeName);

            // Unwrap the new domain instance into a reference in 
            // this domain and use it to execute the untrusted code.
            return handle.Unwrap();
        }

        /// <summary>
        /// Create an instance of the type.
        /// </summary>
        /// <param name="fullyQualifiedName">The string representing the fully qualified name and path to this module.</param>
        /// <param name="fullName">The fully qualified name of the System.Type, including the namespace of the System.Type but not the assembly.</param>
        /// <returns>The instantiated type.</returns>
        public static object CreateInstanceFrom(string fullyQualifiedName, string fullName)
        {
            // Make sure the page reference exists.
            if (fullyQualifiedName == null) throw new ArgumentNullException("fullyQualifiedName");
            if (fullName == null) throw new ArgumentNullException("fullName");

            ObjectHandle handle = Activator.CreateInstanceFrom(fullyQualifiedName, fullName);

            // Unwrap the new domain instance into a reference in 
            // this domain and use it to execute the untrusted code.
            return handle.Unwrap();
        }

        /// <summary>
        /// Create an instance of the type.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <returns>The instantiated type.</returns>
        public static T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Create a one dimensional array.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="length">The length of the array.</param>
        /// <returns>The instantiated array type.</returns>
        public static Array CreateArrayInstance(Type type, int length)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");

            return Array.CreateInstance(type, length);
        }

        /// <summary>
        /// Create a two dimensional array.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="length1">The size of the first dimension of the System.Array to create.</param>
        /// <param name="length2">The size of the second dimension of the System.Array to create.</param>
        /// <returns>The instantiated array type.</returns>
        public static Array CreateArrayInstance(Type type, int length1, int length2)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");

            return Array.CreateInstance(type, length1, length2);
        }

        /// <summary>
        /// Gets the System.Type with the specified name, performing a case-sensitive search.
        /// </summary>
        /// <param name="typeName">The assembly-qualified name of the type to get or the type name
        /// to search for in all the referenced assemblies for the current calling assembly.</param>
        /// <returns>The System.Type with the specified name, if found; otherwise, null.</returns>
        public static Type GetType(string typeName)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");

            return TypeAccessor.GetType(typeName, true, false);
        }

        /// <summary>
        /// Gets the System.Type with the specified name, performing a case-sensitive
        /// search and specifying whether to throw an exception if the type is not found.
        /// </summary>
        /// <param name="typeName">The assembly-qualified name of the type to get or the type name
        /// to search for in all the referenced assemblies for the current calling assembly.</param>
        /// <param name="throwOnError">true to throw an exception if the type cannot be found; false to return null.</param>
        /// <returns>The System.Type with the specified name, if found; otherwise, null.</returns>
        public static Type GetType(string typeName, bool throwOnError)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");

            return TypeAccessor.GetType(typeName, throwOnError, false);
        }

        /// <summary>
        /// Gets the System.Type with the specified name, specifying whether to perform
        /// a case-sensitive search and whether to throw an exception if the type is not found.
        /// </summary>
        /// <param name="typeName">The assembly-qualified name of the type to get or the type name
        /// to search for in all the referenced assemblies for the current calling assembly.</param>
        /// <param name="throwOnError">true to throw an exception if the type cannot be found; false to return null.</param>
        /// <param name="ignoreCase">true to perform a case-insensitive search for typeName, false to perform
        /// a case-sensitive search for typeName.</param>
        /// <returns>The System.Type with the specified name, if found; otherwise, null.</returns>
        public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");

            Type type = null;
            string[] qualifiedName = typeName.Split(new char[] {','}, StringSplitOptions.None);

            // If the type name is a fully qualified name then get
            // the type directly.
            if (qualifiedName.Count() > 1)
                type = Type.GetType(typeName, false, ignoreCase);
            else
            {
                // Get the current calling assembly and return all
                // the referenced assemblies by the calling assembly.
                Assembly executingAssembly = Assembly.GetCallingAssembly();
                AssemblyName[] assemblyNames = executingAssembly.GetReferencedAssemblies();

                // For each assembly name of the referenced assembly.
                foreach (AssemblyName name in assemblyNames)
                {
                    try
                    {
                        // Find the assembly from the assembly name
                        // and the type name. Get the type from the
                        // constructed assembly name and type name.
                        Assembly assembly = Assembly.GetAssembly(Type.GetType(Assembly.CreateQualifiedName(name.Name, typeName), false, ignoreCase));
                        type = Type.GetType(Assembly.CreateQualifiedName(name.Name, typeName), false, ignoreCase);
                        break;
                    }
                    catch { }
                }
            }

            // If the type is null, not found, and throw
            // on error is set the throw an error indicating
            // that the type can not be found.
            if(type == null)
                if (throwOnError)
                    throw new System.Exception("Type '" + typeName + "' can not be found.");

            // Return the type else null.
            return type;
        }

        /// <summary>
        /// Get all the types within the current assembly for the type.
        /// </summary>
        /// <param name="type">The type to return the collection of types within the assembly for.</param>
        /// <returns>The collection of types else null.</returns>
        public static Type[] GetTypes(Type type)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");

            Assembly assembly = type.Assembly;
            return assembly.GetTypes();
        }

        /// <summary>
        /// Get all the types within the current assembly for the type, performing a case-sensitive search.
        /// </summary>
        /// <param name="typeName">The assembly-qualified name of the type to get or the type name
        /// to search for in all the referenced assemblies for the current calling assembly.</param>
        /// <returns>The collection of types else null.</returns>
        public static Type[] GetTypes(string typeName)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");

            return GetTypes(TypeAccessor.GetType(typeName, true, false));
        }

        /// <summary>
        /// Get all the types within the current assembly for the type, performing a case-sensitive
        /// search and specifying whether to throw an exception if the type is not found.
        /// </summary>
        /// <param name="typeName">The assembly-qualified name of the type to get or the type name
        /// to search for in all the referenced assemblies for the current calling assembly.</param>
        /// <param name="throwOnError">true to throw an exception if the type cannot be found; false to return null.</param>
        /// <returns>The collection of types else null.</returns>
        public static Type[] GetTypes(string typeName, bool throwOnError)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");

            return GetTypes(TypeAccessor.GetType(typeName, throwOnError, false));
        }

        /// <summary>
        /// Get all the types within the current assembly for the type, specifying whether to perform
        /// a case-sensitive search and whether to throw an exception if the type is not found.
        /// </summary>
        /// <param name="typeName">The assembly-qualified name of the type to get or the type name
        /// to search for in all the referenced assemblies for the current calling assembly.</param>
        /// <param name="throwOnError">true to throw an exception if the type cannot be found; false to return null.</param>
        /// <param name="ignoreCase">>true to perform a case-insensitive search for typeName, false to perform
        /// a case-sensitive search for typeName.</param>
        /// <returns>The collection of types else null.</returns>
        public static Type[] GetTypes(string typeName, bool throwOnError, bool ignoreCase)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");

            return GetTypes(TypeAccessor.GetType(typeName, throwOnError, ignoreCase));
        }

        /// <summary>
        /// Get all attributes for the object type.
        /// </summary>
        /// <param name="type">The object to return the attributes for.</param>
        /// <returns>The collection of attributes.</returns>
        public static Object[] GetAttributes(object type)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");

            return type.GetType().GetCustomAttributes(false);
        }

        /// <summary>
        /// Get all properties for the data entity type.
        /// </summary>
        /// <param name="type">The object to return the properties for.</param>
        /// <returns>The collection of properties.</returns>
        public static PropertyInfo[] GetProperties(object type)
        {
            // Make sure the page reference exists.
            if (type == null) throw new ArgumentNullException("type");

            // Create a new property collection.
            PropertyInfo[] properties = type.GetType().GetProperties();

            // Return the collection of properties.
            return properties;
        }

        /// <summary>
        /// Get all properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        public static PropertyInfo[] GetProperties<T>()
        {
            // Create a new property collection.
            PropertyInfo[] properties = (typeof(T)).GetProperties();

            // Return the collection of properties.
            return properties;
        }

        /// <summary>
        /// Get all methods for the data entity type.
        /// </summary>
        /// <param name="type">The object to return the methods for.</param>
        /// <returns>The collection of methods.</returns>
        public static MethodInfo[] GetMethods(object type)
        {
            // Create a new property collection.
            MethodInfo[] methods = type.GetType().GetMethods();

            // Return the collection of properties.
            return methods;
        }

        /// <summary>
        /// Get all methods for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of methods.</returns>
        public static MethodInfo[] GetMethods<T>()
        {
            // Create a new property collection.
            MethodInfo[] methods = (typeof(T)).GetMethods();

            // Return the collection of properties.
            return methods;
        }

        /// <summary>
        /// Get the name of the property within the data model type.
        /// </summary>
        /// <typeparam name="T">The data model type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The name of the property.</returns>
        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression == null)
                        throw new NotImplementedException();
                }
                else
                    throw new NotImplementedException();
            }

            // Extract the name of the property from the expression.
            var propertyName = memberExpression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Get the name of the property within the data model type.
        /// </summary>
        /// <typeparam name="T">The data model type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The name of the property.</returns>
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }

            var property = body.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }

            return property.Name;
        }
    }
}
