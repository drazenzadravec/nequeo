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

namespace Nequeo.Wpf.Messaging
{
    /// <summary>
    /// Provides a message class with a built-in callback. When the recipient
    /// is done processing the message, it can execute the callback to
    /// notify the sender that it is done. Use the <see cref="Execute" />
    /// method to execute the callback. The callback method has one parameter.
    /// <seealso cref="NotificationMessageAction"/> and
    /// <seealso cref="NotificationMessageAction&lt;TCallbackParameter&gt;"/>.
    /// </summary>
    ////[ClassInfo(typeof(Messenger))]
    public class NotificationMessageWithCallback : NotificationMessage
    {
        private readonly Delegate _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessageWithCallback" /> class.
        /// </summary>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageWithCallback(string notification, Delegate callback)
            : base(notification)
        {
            CheckCallback(callback);
            _callback = callback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessageWithCallback" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageWithCallback(object sender, string notification, Delegate callback)
            : base(sender, notification)
        {
            CheckCallback(callback);
            _callback = callback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessageWithCallback" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageWithCallback(object sender, object target, string notification, Delegate callback)
            : base(sender, target, notification)
        {
            CheckCallback(callback);
            _callback = callback;
        }

        /// <summary>
        /// Executes the callback that was provided with the message with an
        /// arbitrary number of parameters.
        /// </summary>
        /// <param name="arguments">A  number of parameters that will
        /// be passed to the callback method.</param>
        /// <returns>The object returned by the callback method.</returns>
        public virtual object Execute(params object[] arguments)
        {
            return _callback.DynamicInvoke(arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        private static void CheckCallback(Delegate callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback", "Callback may not be null");
            }
        }
    }
}