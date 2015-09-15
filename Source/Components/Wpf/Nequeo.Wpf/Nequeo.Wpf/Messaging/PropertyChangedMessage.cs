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

namespace Nequeo.Wpf.Messaging
{
    /// <summary>
    /// Passes a string property name (PropertyName) and a generic value
    /// (<see cref="OldValue" /> and <see cref="NewValue" />) to a recipient.
    /// This message type can be used to propagate a PropertyChanged event to
    /// a recipient using the messenging system.
    /// </summary>
    /// <typeparam name="T">The type of the OldValue and NewValue property.</typeparam>
    public class PropertyChangedMessage<T> : PropertyChangedMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedMessage{T}" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedMessage(object sender, T oldValue, T newValue, string propertyName)
            : base(sender, propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedMessage{T}" /> class.
        /// </summary>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedMessage(T oldValue, T newValue, string propertyName)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedMessage{T}" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedMessage(object sender, object target, T oldValue, T newValue, string propertyName)
            : base(sender, target, propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the value that the property has after the change.
        /// </summary>
        public T NewValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value that the property had before the change.
        /// </summary>
        public T OldValue
        {
            get;
            private set;
        }
    }
}