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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Nequeo.Net.Http.Extension
{
    /// <summary>Extension methods for working with Ping asynchronously.</summary>
    public static class PingExtensions
    {
        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">An IPAddress that identifies the computer that is the destination for the ICMP echo message.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        /// A String that identifies the computer that is the destination for the ICMP echo message. 
        /// The value specified for this parameter can be a host name or a string representation of an IP address.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">An IPAddress that identifies the computer that is the destination for the ICMP echo message.</param>
        /// <param name="timeout">
        /// An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) 
        /// to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, int timeout, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, timeout, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        /// A String that identifies the computer that is the destination for the ICMP echo message. 
        /// The value specified for this parameter can be a host name or a string representation of an IP address.
        /// </param>
        /// <param name="timeout">
        /// An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) 
        /// to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, int timeout, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, timeout, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">An IPAddress that identifies the computer that is the destination for the ICMP echo message.</param>
        /// <param name="timeout">
        /// An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) 
        /// to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        /// A Byte array that contains data to be sent with the ICMP echo message and returned 
        /// in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, int timeout, byte[] buffer, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, timeout, buffer, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        /// A String that identifies the computer that is the destination for the ICMP echo message. 
        /// The value specified for this parameter can be a host name or a string representation of an IP address.
        /// </param>
        /// <param name="timeout">
        /// An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) 
        /// to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        /// A Byte array that contains data to be sent with the ICMP echo message and returned 
        /// in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, timeout, buffer, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">An IPAddress that identifies the computer that is the destination for the ICMP echo message.</param>
        /// <param name="timeout">
        /// An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) 
        /// to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        /// A Byte array that contains data to be sent with the ICMP echo message and returned 
        /// in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="options">A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, timeout, buffer, options, tcs));
        }

        /// <summary>
        /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        /// A String that identifies the computer that is the destination for the ICMP echo message. 
        /// The value specified for this parameter can be a host name or a string representation of an IP address.
        /// </param>
        /// <param name="timeout">
        /// An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) 
        /// to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        /// A Byte array that contains data to be sent with the ICMP echo message and returned 
        /// in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="options">A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
        {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, timeout, buffer, options, tcs));
        }

        /// <summary>The core implementation of SendTask.</summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <param name="sendAsync">
        /// A delegate that initiates the asynchronous send.
        /// The provided TaskCompletionSource must be passed as the user-supplied state to the actual Ping.SendAsync method.
        /// </param>
        /// <returns></returns>
        private static Task<PingReply> SendTaskCore(Ping ping, object userToken, Action<TaskCompletionSource<PingReply>> sendAsync)
        {
            // Validate we're being used with a real smtpClient.  The rest of the arg validation
            // will happen in the call to sendAsync.
            if (ping == null) throw new ArgumentNullException("ping");

            // Create a TaskCompletionSource to represent the operation
            var tcs = new TaskCompletionSource<PingReply>(userToken);

            // Register a handler that will transfer completion results to the TCS Task
            PingCompletedEventHandler handler = null;
            handler = (sender, e) => Nequeo.Threading.Common.EAPCommon.HandleCompletion(tcs, e, () => e.Reply, () => ping.PingCompleted -= handler);
            ping.PingCompleted += handler;

            // Try to start the async operation.  If starting it fails (due to parameter validation)
            // unregister the handler before allowing the exception to propagate.
            try
            {
                sendAsync(tcs);
            }
            catch(Exception exc)
            {
                ping.PingCompleted -= handler;
                tcs.TrySetException(exc);
            }

            // Return the task to represent the asynchronous operation
            return tcs.Task;
        }
    }
}
