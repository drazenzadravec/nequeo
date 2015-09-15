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
    /// Base class for all messages broadcasted by the Messenger.
    /// You can create your own message types by extending this class.
    /// </summary>
    public class MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        public MessageBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        public MessageBase(object sender)
        {
            Sender = sender;
        }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        public MessageBase(object sender, object target)
            : this(sender)
        {
            Target = target;
        }

        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public object Sender
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the message's intended target. This property can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.
        /// </summary>
        public object Target
        {
            get;
            protected set;
        }
    }
}