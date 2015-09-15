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
    /// Passes a generic value (Content) to a recipient.
    /// </summary>
    /// <typeparam name="T">The type of the Content property.</typeparam>
    public class GenericMessage<T> : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="content">The message content.</param>
        public GenericMessage(T content)
        {
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="content">The message content.</param>
        public GenericMessage(object sender, T content)
            : base(sender)
        {
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="content">The message content.</param>
        public GenericMessage(object sender, object target, T content)
            : base(sender, target)
        {
            Content = content;
        }

        /// <summary>
        /// Gets or sets the message's content.
        /// </summary>
        public T Content
        {
            get;
            protected set;
        }
    }
}