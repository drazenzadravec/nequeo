/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Dynamic;

namespace Nequeo.Runtime
{
    /// <summary>
    /// Dynamic runtime base class.
    /// </summary>
    public class DynamicObjectBase : DynamicObject { }

    /// <summary>
    /// Dynamic runtime provider base abstract class.
    /// </summary>
    public abstract class DynamicProviderBase : DynamicObject
    {
        /// <summary>
        /// Invoke a member.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="args">The method arguments.</param>
        /// <returns>The task.</returns>
        protected abstract Task InvokeMember(string method, params object[] args);

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the DynamicObject 
        /// class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides 
        /// the name of the member on which the dynamic operation is performed. For example, for the statement 
        /// sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the 
        /// DynamicObject class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies 
        /// whether the member name is case-sensitive.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation. 
        /// For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the 
        /// DynamicObject class, args[0] is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>True if the operation is successful; otherwise, false. If this method returns false, 
        /// the run-time binder of the language determines the behavior. (In most cases, a language-specific 
        /// run-time exception is thrown.)</returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = InvokeMember(binder.Name, args);
            return true;
        }
    }

    /// <summary>
    /// Dynamic runtime provider abstract class.
    /// </summary>
    public abstract class DynamicProvider : DynamicProviderBase
    {
        // The inner dictionary.
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        /// <summary>
        /// Gets the number of properties in the collection.
        /// </summary>
        public int Count
        {
            get { return dictionary.Count; }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the 
        /// DynamicObject class can override this method to specify dynamic behavior for operations such 
        /// as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called 
        /// for a property, you can assign the property value to result.</param>
        /// <returns>True if the operation is successful; otherwise, false. If this method returns false, 
        /// the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the 
        /// DynamicObject class can override this method to specify dynamic behavior for operations such 
        /// as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", 
        /// where sampleObject is an instance of the class derived from the DynamicObject class, the value is "Test".</param>
        /// <returns>True if the operation is successful; otherwise, false. If this method returns false, the 
        /// run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Add the property name.
            dictionary[binder.Name] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}
