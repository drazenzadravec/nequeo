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
    /// Generic type descriptor extender.
    /// </summary>
    /// <typeparam name="T">The type to examine.</typeparam>
    public abstract class TypeDescriptorExtender<T> : TypeDescriptorExtender
        where T : class, new()
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected TypeDescriptorExtender()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">The instance type object.</param>
        protected TypeDescriptorExtender(T type)
            : base(type)
        {
        }
    }

    /// <summary>
    /// Generic type interface descriptor extender.
    /// </summary>
    /// <typeparam name="T">The type to examine.</typeparam>
    public abstract class TypeDescriptorMockExtender<T> : TypeDescriptorExtender
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected TypeDescriptorMockExtender()
            : base(typeof(T))
        {
        }
    }

    /// <summary>
    /// Type descriptor extender.
    /// </summary>
    public abstract class TypeDescriptorExtender : TypeDescriptionProvider, ICustomTypeDescriptor
    {
        /// <summary>
        /// Component Type constructor.
        /// </summary>
        /// <param name="componentType">The component type to describle.</param>
        protected TypeDescriptorExtender(Type componentType)
        {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            if(componentType.IsClass)
                _component = Activator.CreateInstance(componentType);

            _componentType = componentType;
        }

        /// <summary>
        /// Instance Component constructor.
        /// </summary>
        /// <param name="component">The component type instance to describle.</param>
        protected TypeDescriptorExtender(object component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            _component = component;
            _componentType = component.GetType();
        }

        private readonly object _component;
        private readonly Type _componentType;

        /// <summary>
        /// Gets, the component type instance.
        /// </summary>
        public object Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets, the component type.
        /// </summary>
        public Type ComponentType
        {
            get { return _componentType; }
        }

        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>An System.ComponentModel.AttributeCollection containing the attributes for
        /// this object.</returns>
        public AttributeCollection GetAttributes()
        {
            if(_component != null)
                return TypeDescriptor.GetAttributes(_component);
            else
                return TypeDescriptor.GetAttributes(_componentType);
        }

        /// <summary>
        /// Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>The class name of the object, or null if the class does not have a name.</returns>
        public string GetClassName()
        {
            if (_component != null)
                return TypeDescriptor.GetClassName(_component);
            else
                return TypeDescriptor.GetClassName(_componentType);
        }

        /// <summary>
        /// Returns the name of this instance of a component.
        /// </summary>
        /// <returns>The name of the object, or null if the object does not have a name.</returns>
        public string GetComponentName()
        {
            if (_component != null)
                return TypeDescriptor.GetComponentName(_component);
            else
                return TypeDescriptor.GetComponentName(_componentType);
        }

        /// <summary>
        /// Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>A System.ComponentModel.TypeConverter that is the converter for this object,
        /// or null if there is no System.ComponentModel.TypeConverter for this object.</returns>
        public TypeConverter GetConverter()
        {
            if (_component != null)
                return TypeDescriptor.GetConverter(_component);
            else
                return TypeDescriptor.GetConverter(_componentType);
        }

        /// <summary>
        /// Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>An System.ComponentModel.EventDescriptor that represents the default event
        /// for this object, or null if this object does not have events.</returns>
        public EventDescriptor GetDefaultEvent()
        {
            if (_component != null)
                return TypeDescriptor.GetDefaultEvent(_component);
            else
                return TypeDescriptor.GetDefaultEvent(_componentType);
        }

        /// <summary>
        /// Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>A System.ComponentModel.PropertyDescriptor that represents the default property
        /// for this object, or null if this object does not have properties.</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            if (_component != null)
                return TypeDescriptor.GetDefaultProperty(_component);
            else
                return TypeDescriptor.GetDefaultProperty(_componentType);
        }

        /// <summary>
        /// Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A System.Type that represents the editor for this object.</param>
        /// <returns>An System.Object of the specified type that is the editor for this object,
        /// or null if the editor cannot be found.</returns>
        public object GetEditor(Type editorBaseType)
        {
            if (_component != null)
                return TypeDescriptor.GetEditor(_component, editorBaseType);
            else
                return TypeDescriptor.GetEditor(_componentType, editorBaseType);
        }

        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute
        /// array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type System.Attribute that is used as a filter.</param>
        /// <returns>An System.ComponentModel.EventDescriptorCollection that represents the filtered
        /// events for this component instance.</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            if (_component != null)
                return TypeDescriptor.GetEvents(_component, attributes);
            else
                return TypeDescriptor.GetEvents(_componentType, attributes);
        }

        /// <summary>
        /// Returns the events for this instance of a component.
        /// </summary>
        /// <returns>An System.ComponentModel.EventDescriptorCollection that represents the events
        /// for this component instance.</returns>
        public EventDescriptorCollection GetEvents()
        {
            if (_component != null)
                return TypeDescriptor.GetEvents(_component);
            else
                return TypeDescriptor.GetEvents(_componentType);
        }

        /// <summary>
        /// Returns the properties for this instance of a component using the attribute
        /// array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type System.Attribute that is used as a filter.</param>
        /// <returns>A System.ComponentModel.PropertyDescriptorCollection that represents the
        /// filtered properties for this component instance.</returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_component != null)
                return TypeDescriptor.GetProperties(_component, attributes);
            else
                return TypeDescriptor.GetProperties(_componentType, attributes);
        }

        /// <summary>
        /// Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>A System.ComponentModel.PropertyDescriptorCollection that represents the
        /// properties for this component instance.</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            if (_component != null)
                return TypeDescriptor.GetProperties(_component);
            else
                return TypeDescriptor.GetProperties(_componentType);
        }

        /// <summary>
        /// Get all methods for the type.
        /// </summary>
        /// <returns>The collection of methods.</returns>
        public MethodInfo[] GetInfoMethods()
        {
            return _componentType.GetMethods();
        }

        /// <summary>
        /// Get all public methods for the type.
        /// </summary>
        /// <returns>The collection of methods.</returns>
        public MethodInfo[] GetPublicInfoMethods()
        {
            MethodInfo[] publicOnlyMethods = _componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            return publicOnlyMethods.Where(m => ((!m.Name.ToLower().StartsWith("get_")) && (!m.Name.ToLower().StartsWith("set_")))).ToArray();
        }

        /// <summary>
        /// Get all properties for the type.
        /// </summary>
        /// <returns>The collection of methods.</returns>
        public PropertyInfo[] GetInfoProperties()
        {
            return _componentType.GetProperties();
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property
        /// descriptor.
        /// </summary>
        /// <param name="pd">A System.ComponentModel.PropertyDescriptor that represents the property whose
        /// owner is to be found.</param>
        /// <returns>An System.Object that represents the owner of the specified property.</returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
}
