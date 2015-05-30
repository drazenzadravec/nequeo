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

namespace Nequeo.Reflection
{
    /// <summary>
    /// Allows for mocking of types from the interface type
    /// </summary>
    /// <typeparam name="IT">The interface type to mock.</typeparam>
    public sealed class Mock<IT> : TypeDescriptorMockExtender<IT>, IMock<IT>
    {
        private AssemblyName _assemblyName;
        private AssemblyBuilder _asssemblyBuilder;

        private ModuleBuilder _moduleBuilder;
        private Dictionary<MockSignatureBuilder, Type> _classes;

        private ReaderWriterLock _rwLock;
        private TypeBuilder _typeBuilder;

        private Object _instance = null;

        /// <summary>
        /// DEfault constructor.
        /// </summary>
        public Mock()
        {
            if (!typeof(IT).IsInterface)
                throw new System.Exception("The generic type is not and interface.");

            // Create the new assembly
            _assemblyName = new AssemblyName(typeof(IT).Name + "Module");
            _asssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);

            // Create only one module, therefor the
            // modile name is the assembly name.
            _moduleBuilder = _asssemblyBuilder.DefineDynamicModule(_assemblyName.Name);

            // Get the class unique signature.
            _classes = new Dictionary<MockSignatureBuilder, Type>();
            _rwLock = new ReaderWriterLock();

            // Create a new instance.
            CreateInstance();
        }

        /// <summary>
        /// Gets the new instance implementation from the interface.
        /// </summary>
        public IT Instance
        {
            get { return (IT)_instance; }
        }

        /// <summary>
        /// Setup the return value for the specified method.
        /// </summary>
        /// <param name="methodName">The method name in the interface.</param>
        /// <returns>The method action implementation.</returns>
        public IMockMethodAction Setup(string methodName)
        {
            // Make sure the instance exists.
            if (_instance != null)
            {
                // Return the new instance.
                MockMethodAction action = new MockMethodAction(_instance, methodName);
                return action;
            }
            else
                throw new System.Exception("Create the instance first.");
        }

        /// <summary>
        /// Create a new instance of the implemtation type.
        /// </summary>
        private void CreateInstance()
        {
            // Create the dynamic class.
            Type type = GetDynamicClass(this.GetInfoProperties(), this.GetPublicInfoMethods());

            // Get the current instance.
            _instance = TypeAccessor.CreateInstance(type);
        }

        /// <summary>
        /// Get the interface type details.
        /// </summary>
        /// <param name="properties">The collection of properties to create.</param>
        /// <param name="methods">The collection of methods to create.</param>
        /// <returns>The mocking type from the interface.</returns>
        private Type GetDynamicClass(IEnumerable<PropertyInfo> properties, IEnumerable<MethodInfo> methods)
        {
            // Get the read write lock.
            _rwLock.AcquireReaderLock(Timeout.Infinite);

            try
            {
                // Create a new mock unique signature of the current type.
                MockSignatureBuilder signature = new MockSignatureBuilder(properties, methods);
                Type type;

                // If the current type is not in the collection of
                // types then create the new type.
                if (!_classes.TryGetValue(signature, out type))
                {
                    // Create the type and add to the unique
                    // collection of types created.
                    type = CreateDynamicClass(signature.properties, signature.methods);
                    _classes.Add(signature, type);
                }

                // Return the crerated type.
                return type;
            }
            finally
            {
                // Release the read write lock.
                _rwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Create the dynamic type from the interface.
        /// </summary>
        /// <param name="properties">The collection of properties to create.</param>
        /// <param name="methods">The collection of methods to create.</param>
        /// <returns>The mocking type from the interface.</returns>
        private Type CreateDynamicClass(PropertyInfo[] properties, MethodInfo[] methods)
        {
            // Lock the read write lock.
            LockCookie cookie = _rwLock.UpgradeToWriterLock(Timeout.Infinite);

            try
            {
                // Name the current type.
                string typeName = typeof(IT).Name + "Implementation";
                
                try
                {
                    // Create a new type builder.
                    _typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Class |
                        TypeAttributes.Public, typeof(DynamicClass));

                    // Add the interface inplemenation
                    // of the current interface to mock.
                    _typeBuilder.AddInterfaceImplementation(typeof(IT));

                    // Create the constructor, properties
                    // and the get hash code method and equality method.
                    GenerateConstructor(_typeBuilder);
                    FieldInfo[] fields = GenerateProperties(_typeBuilder, properties);
                    GenerateEquals(_typeBuilder, fields);
                    GenerateGetHashCode(_typeBuilder, fields);

                    // If methods exist then create
                    // each method in the type.
                    if (methods != null)
                        GenerateMethods(_typeBuilder, methods);

                    // Create the type, return the type.
                    Type result = _typeBuilder.CreateType();
                    return result;
                }
                finally { }
            }
            finally
            {
                // Unlock the read write lock.
                _rwLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        /// <summary>
        /// Generate the constructor.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        private void GenerateConstructor(TypeBuilder typeBuilder)
        {
            // Create the default constructor.
            ConstructorInfo baseConstructorInfo = typeof(object).GetConstructor(new Type[0]);
            ConstructorBuilder constructorBuilder = 
                typeBuilder.DefineConstructor(
                           MethodAttributes.Public,
                           CallingConventions.Standard,
                           Type.EmptyTypes);

            // Create the base call operations.
            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseConstructorInfo);
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate the collection of methods.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <param name="methods">The collection of methods to create.</param>
        private void GenerateMethods(TypeBuilder typeBuilder, MethodInfo[] methods)
        {
            // Create each method.
            for (int i = 0; i < methods.Length; i++)
            {
                // Create a new method builder.
                MethodInfo methodInfo = methods[i];
                MethodBuilder methodBuilder;
                ILGenerator mdMethod;

                List<Type> parameters = new List<Type>();
                foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                    parameters.Add(parameterInfo.ParameterType);

                if (methodInfo.ReturnType != typeof(void) && parameters.Count > 0)
                {
                    // Create a new method implementation.
                    methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public |
                        MethodAttributes.Virtual, methodInfo.ReturnType, parameters.ToArray());

                    // Get the method call return information
                    // and the private field where the return
                    // value will be stored.
                    MethodInfo returnTypeCall = CallReturnMethodInfo(typeBuilder);
                    FieldBuilder returnFieldBuilder = GenerateReturnTypeMethodProperties(typeBuilder, methodInfo.ReturnType, methodInfo.Name);
                    MethodInfo exceptionCall = CallExceptionMethodInfo(typeBuilder);
                    FieldBuilder exceptionFieldBuilder = GenerateExceptionMethodProperties(typeBuilder, methodInfo.Name);

                    // Create a new operation code generator.
                    // Load the instance it belongs to (argument zero is the instance)
                    mdMethod = methodBuilder.GetILGenerator();
                    Label exceptionLabel = mdMethod.DefineLabel();
                    Label returnTypeLabel = mdMethod.DefineLabel();
                    LocalBuilder localBuilder = mdMethod.DeclareLocal(methodInfo.ReturnType);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.EmitCall(OpCodes.Call, exceptionCall, new Type[] { typeof(System.String) });
                    mdMethod.Emit(OpCodes.Brtrue_S, exceptionLabel);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, returnFieldBuilder);
                    mdMethod.EmitCall(OpCodes.Call, returnTypeCall, new Type[] { typeof(object) });
                    mdMethod.Emit(OpCodes.Br_S, returnTypeLabel);
                    mdMethod.MarkLabel(exceptionLabel);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.Emit(OpCodes.Newobj, typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }));
                    mdMethod.Emit(OpCodes.Throw);
                    mdMethod.MarkLabel(returnTypeLabel);
                    mdMethod.Emit(OpCodes.Ret);
                }
                else if (methodInfo.ReturnType != typeof(void) && parameters.Count < 1)
                {
                    // Create a new method implementation.
                    methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public |
                        MethodAttributes.Virtual, methodInfo.ReturnType, parameters.ToArray());

                    // Get the method call return information
                    // and the private field where the return
                    // value will be stored.
                    MethodInfo returnTypeCall = CallReturnMethodInfo(typeBuilder);
                    FieldBuilder returnFieldBuilder = GenerateReturnTypeMethodProperties(typeBuilder, methodInfo.ReturnType, methodInfo.Name);
                    MethodInfo exceptionCall = CallExceptionMethodInfo(typeBuilder);
                    FieldBuilder exceptionFieldBuilder = GenerateExceptionMethodProperties(typeBuilder, methodInfo.Name);

                    // Create a new operation code generator.
                    // Load the instance it belongs to (argument zero is the instance)
                    mdMethod = methodBuilder.GetILGenerator();
                    Label exceptionLabel = mdMethod.DefineLabel();
                    Label returnTypeLabel = mdMethod.DefineLabel();
                    LocalBuilder localBuilder = mdMethod.DeclareLocal(methodInfo.ReturnType);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.EmitCall(OpCodes.Call, exceptionCall, new Type[] { typeof(System.String) });
                    mdMethod.Emit(OpCodes.Brtrue_S, exceptionLabel);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, returnFieldBuilder);
                    mdMethod.EmitCall(OpCodes.Call, returnTypeCall, new Type[] { typeof(object) });
                    mdMethod.Emit(OpCodes.Br_S, returnTypeLabel);
                    mdMethod.MarkLabel(exceptionLabel);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.Emit(OpCodes.Newobj, typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }));
                    mdMethod.Emit(OpCodes.Throw);
                    mdMethod.MarkLabel(returnTypeLabel);
                    mdMethod.Emit(OpCodes.Ret);
                }
                else if (methodInfo.ReturnType == typeof(void) && parameters.Count > 0)
                {
                    // Create a new method implementation.
                    methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public |
                        MethodAttributes.Virtual, typeof(void), parameters.ToArray());

                    // Get the method call return information
                    // and the private field where the return
                    // value will be stored.
                    MethodInfo exceptionCall = CallExceptionMethodInfo(typeBuilder);
                    FieldBuilder exceptionFieldBuilder = GenerateExceptionMethodProperties(typeBuilder, methodInfo.Name);

                    // Create a new operation code generator.
                    // Load the instance it belongs to (argument zero is the instance)
                    mdMethod = methodBuilder.GetILGenerator();
                    Label exceptionLabel = mdMethod.DefineLabel();
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.EmitCall(OpCodes.Call, exceptionCall, new Type[] { typeof(System.String) });
                    mdMethod.Emit(OpCodes.Brfalse_S, exceptionLabel);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.Emit(OpCodes.Newobj, typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }));
                    mdMethod.Emit(OpCodes.Throw);
                    mdMethod.MarkLabel(exceptionLabel);
                    mdMethod.Emit(OpCodes.Ret);
                }
                else
                {
                    // Create a new method implementation.
                    methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public |
                        MethodAttributes.Virtual, typeof(void), parameters.ToArray());

                    // Get the method call return information
                    // and the private field where the return
                    // value will be stored.
                    MethodInfo exceptionCall = CallExceptionMethodInfo(typeBuilder);
                    FieldBuilder exceptionFieldBuilder = GenerateExceptionMethodProperties(typeBuilder, methodInfo.Name);

                    // Create a new operation code generator.
                    // Load the instance it belongs to (argument zero is the instance)
                    mdMethod = methodBuilder.GetILGenerator();
                    Label exceptionLabel = mdMethod.DefineLabel();
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.EmitCall(OpCodes.Call, exceptionCall, new Type[] { typeof(System.String) });
                    mdMethod.Emit(OpCodes.Brfalse_S, exceptionLabel);
                    mdMethod.Emit(OpCodes.Ldarg_0);
                    mdMethod.Emit(OpCodes.Ldfld, exceptionFieldBuilder);
                    mdMethod.Emit(OpCodes.Newobj, typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }));
                    mdMethod.Emit(OpCodes.Throw);
                    mdMethod.MarkLabel(exceptionLabel);
                    mdMethod.Emit(OpCodes.Ret);
                }

                // We need to associate our new type's method 
                // with the method in the interface
                typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            }
        }

        /// <summary>
        /// Generate the collection of properties.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <param name="properties">The collection of properties</param>
        /// <returns>The collection of fields in the type.</returns>
        private FieldInfo[] GenerateProperties(TypeBuilder typeBuilder, PropertyInfo[] properties)
        {
            FieldInfo[] fields = new FieldBuilder[properties.Length];

            // For each property add the the type.
            for (int i = 0; i < properties.Length; i++)
            {
                // Create a new property builder.
                PropertyInfo propertyInfo = properties[i];
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyInfo.Name, propertyInfo.PropertyType, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyInfo.PropertyType, null);

                // If there is a getter in the interface, create a getter in the new type
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                if (getMethod != null)
                {
                    // Create a new getter method.
                    MethodBuilder mbGet = typeBuilder.DefineMethod("get_" + propertyInfo.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        propertyInfo.PropertyType, Type.EmptyTypes);

                    // Create a new operation code generator.
                    ILGenerator genGet = mbGet.GetILGenerator();
                    genGet.Emit(OpCodes.Ldarg_0);
                    genGet.Emit(OpCodes.Ldfld, fieldBuilder);
                    genGet.Emit(OpCodes.Ret);
                    propertyBuilder.SetGetMethod(mbGet);

                    // We need to associate our new type's method with the 
                    // getter method in the interface
                    typeBuilder.DefineMethodOverride(mbGet, getMethod);
                }

                // If there is a setter in the interface, create a setter in the new type
                MethodInfo setMethod = propertyInfo.GetSetMethod();
                if (setMethod != null)
                {
                    // Create a new setter method.
                    MethodBuilder mbSet = typeBuilder.DefineMethod("set_" + propertyInfo.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        null, new Type[] { propertyInfo.PropertyType });

                    // Create a new operation code generator.
                    ILGenerator genSet = mbSet.GetILGenerator();
                    genSet.Emit(OpCodes.Ldarg_0);
                    genSet.Emit(OpCodes.Ldarg_1);
                    genSet.Emit(OpCodes.Stfld, fieldBuilder);
                    genSet.Emit(OpCodes.Ret);
                    propertyBuilder.SetSetMethod(mbSet);

                    // We need to associate our new type's method with the 
                    // getter method in the interface
                    typeBuilder.DefineMethodOverride(mbSet, setMethod);
                }

                // Assign the current field;
                fields[i] = fieldBuilder;
            }

            // Return the collection of fields.
            return fields;
        }

        /// <summary>
        /// Generate the property that will contain the return value.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <param name="returnType">The return type.</param>
        /// <param name="methodName">The method name for the return type.</param>
        /// <returns>The field that will contain the return value.</returns>
        private FieldBuilder GenerateReturnTypeMethodProperties(TypeBuilder typeBuilder, Type returnType, string methodName)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_return" + methodName, returnType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Return" + methodName, PropertyAttributes.HasDefault, returnType, null);

            // Create a new setter method.
            MethodBuilder mbSet = typeBuilder.DefineMethod("set_Return" + methodName,
                    MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { returnType });

            // Create a new operation code generator.
            ILGenerator genSet = mbSet.GetILGenerator();
            genSet.Emit(OpCodes.Ldarg_0);
            genSet.Emit(OpCodes.Ldarg_1);
            genSet.Emit(OpCodes.Stfld, fieldBuilder);
            genSet.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(mbSet);

            return fieldBuilder;
        }

        /// <summary>
        /// Generate the property that will contain the exception value.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <param name="methodName">The method name for the return type.</param>
        /// <returns>The field that will contain the return value.</returns>
        private FieldBuilder GenerateExceptionMethodProperties(TypeBuilder typeBuilder, string methodName)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_exception" + methodName, typeof(System.String), FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Exception" + methodName, PropertyAttributes.HasDefault, typeof(System.String), null);

            // Create a new setter method.
            MethodBuilder mbSet = typeBuilder.DefineMethod("set_Exception" + methodName,
                    MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { typeof(System.String) });

            // Create a new operation code generator.
            ILGenerator genSet = mbSet.GetILGenerator();
            genSet.Emit(OpCodes.Ldarg_0);
            genSet.Emit(OpCodes.Ldarg_1);
            genSet.Emit(OpCodes.Stfld, fieldBuilder);
            genSet.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(mbSet);

            return fieldBuilder;
        }

        /// <summary>
        /// Generate the equals method for each property and the type.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <param name="fields">The collection of fields.</param>
        private void GenerateEquals(TypeBuilder typeBuilder, FieldInfo[] fields)
        {
            // Create the equals method for the type.
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new Type[] { typeof(object) });

            // Create a new operation code generator.
            ILGenerator gen = methodBuilder.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(typeBuilder);
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, typeBuilder);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }

            // Return the result.
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate get hash code for each property and the type.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <param name="fields">The collection of fields.</param>
        private void GenerateGetHashCode(TypeBuilder typeBuilder, FieldInfo[] fields)
        {
            // Create the get hash code method for the type.
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);

            // Create a new operation code generator.
            ILGenerator generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                generator.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
                generator.Emit(OpCodes.Xor);
            }

            // Return the result.
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Gets the method information for the calling method return value.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <returns>The method information</returns>
        private MethodInfo CallReturnMethodInfo(TypeBuilder typeBuilder)
        {
            MethodInfo methodInfo = typeof(MockMethodCall).GetMethod("CallMethodReturn", new Type[] { typeof(object) });
            return methodInfo;
        }

        /// <summary>
        /// Gets the method information for the calling method exception.
        /// </summary>
        /// <param name="typeBuilder">The type builder</param>
        /// <returns>The method information</returns>
        private MethodInfo CallExceptionMethodInfo(TypeBuilder typeBuilder)
        {
            MethodInfo methodInfo = typeof(MockMethodCall).GetMethod("CallMethodException", new Type[] { typeof(System.String) });
            return methodInfo;
        }
    }

    /// <summary>
    /// Mock return value method.
    /// </summary>
    public sealed class MockMethodCall
    {
        /// <summary>
        /// The member called to return the value set.
        /// </summary>
        /// <param name="returnValue">The vaue to return.</param>
        /// <returns>The converted type.</returns>
        public static object CallMethodReturn(object returnValue)
        {
            // Return the value.
            return returnValue;
        }

        /// <summary>
        /// The member called to test for an exception instance.
        /// </summary>
        /// <param name="exception">The exception to throw.</param>
        /// <returns>True if an exception instance has been set else false.</returns>
        public static bool CallMethodException(System.String exception)
        {
            // Return true if exception is null
            // else false
            return (exception != null);
        }
    }

    /// <summary>
    /// Mocking dynamic class signature.
    /// </summary>
    internal class MockSignatureBuilder : IEquatable<MockSignatureBuilder>
    {
        public MethodInfo[] methods;
        public PropertyInfo[] properties;
        public int hashCode;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="properties">The collection of properties</param>
        /// <param name="methods">The collection of methods</param>
        public MockSignatureBuilder(IEnumerable<PropertyInfo> properties, IEnumerable<MethodInfo> methods)
        {
            // Assign the array of properties.
            this.properties = properties.ToArray();

            // Assign the array of methods.
            if (methods != null)
                this.methods = methods.ToArray();

            hashCode = 0;

            // For each property create the new has code for the class.
            foreach (PropertyInfo p in properties)
            {
                hashCode ^= p.Name.GetHashCode() ^ p.PropertyType.GetHashCode();
            }
        }

        /// <summary>
        /// Get the class hash code.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return hashCode;
        }

        /// <summary>
        /// The equality operation
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if this object is the same as the comparing object.</returns>
        public override bool Equals(object obj)
        {
            return obj is MockSignatureBuilder ? Equals((MockSignatureBuilder)obj) : false;
        }

        /// <summary>
        /// The equality operation
        /// </summary>
        /// <param name="other">The mocking object to compare.</param>
        /// <returns>True if the object is the same as the comparing object.</returns>
        public bool Equals(MockSignatureBuilder other)
        {
            if (properties.Length != other.properties.Length) return false;

            // Find the mocking type equality.
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name != other.properties[i].Name ||
                    properties[i].PropertyType != other.properties[i].PropertyType) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Mock method action.
    /// </summary>
    public sealed class MockMethodAction : IMockMethodAction
    {
        private string _methodName = null;
        private Object _instance = null;
       
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="instance">The instance of the mock object.</param>
        /// <param name="methodName">The method name to assign.</param>
        internal MockMethodAction(Object instance, string methodName)
        {
            _methodName = methodName;
            _instance = instance;
        }

        /// <summary>
        /// Set the value that is to be returned when the method is invoked.
        /// </summary>
        /// <param name="value">The value to return.</param>
        public void Return(Object value)
        {
            if (value is System.Exception)
            {
                // Set the exception.
                PropertyInfo propertyInfo = _instance.GetType().GetProperty("Exception" + _methodName);

                if (propertyInfo != null)
                    propertyInfo.SetValue(_instance, ((System.Exception)value).Message, null);
                else
                    throw new System.Exception("The specified member may not exist.");
            }
            else
            {
                // Set the original value.
                PropertyInfo propertyInfo = _instance.GetType().GetProperty("Return" + _methodName);
                if (propertyInfo != null)
                {
                    if(value.GetType() == propertyInfo.PropertyType)
                        propertyInfo.SetValue(_instance, value, null);
                    else
                        throw new System.Exception("The member return type : '" + propertyInfo.PropertyType.FullName + "' " +
                            "is not equal to the object type : '" + value.GetType().FullName + "'");
                }
                else
                    throw new System.Exception("The specified member may not exist.");

                // Set the exception.
                PropertyInfo propertyInfoException = _instance.GetType().GetProperty("Exception" + _methodName);
                if (propertyInfoException != null)
                    propertyInfoException.SetValue(_instance, null, null);
                else
                    throw new System.Exception("The specified member may not exist.");
            }
        }
    }

    /// <summary>
    /// Allows for mocking of types from the interface type
    /// </summary>
    /// <typeparam name="IT">The interface type to mock.</typeparam>
    public interface IMock<IT>
    {
        /// <summary>
        /// Create the new instance type from the interface
        /// </summary>
        /// <returns>The instance of the new mocked type.</returns>
        IT Instance { get; }

        /// <summary>
        /// Setup the return value for the specified method.
        /// </summary>
        /// <param name="methodName">The method name in the interface.</param>
        /// <returns>The method action implementation.</returns>
        IMockMethodAction Setup(string methodName);
    }

    /// <summary>
    /// Mock method action.
    /// </summary>
    public interface IMockMethodAction
    {
        /// <summary>
        /// Set the value that is to be returned when the method is invoked.
        /// </summary>
        /// <param name="value">The value to return.</param>
        void Return(Object value);
    }
}
