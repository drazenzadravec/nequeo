/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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

namespace Nequeo.Net.Http2.Protocol
{
    /// <summary>
    /// Represents the frame type of the Http/2 frame as defined in section 11.2 of the Http/2 protocol spec.
    /// </summary>
    internal enum OpCodeFrame : byte
    {
        /// <summary>
        /// Data frame.
        /// </summary>
        Data = 0x0,
        /// <summary>
        /// Headers frame is used to open a stream (Section 5.1), and additionally carries a header block fragment.
        /// </summary>
        Headers = 0x1,
        /// <summary>
        /// The Priority frame specifies the sender-advised priority
        /// of a stream (Section 5.3).  It can be sent at any time for any
        /// stream, including idle or closed streams.
        /// </summary>
        Priority = 0x2,
        /// <summary>
        /// The RST_STREAM frame allows for immediate termination of a
        /// stream.  RST_STREAM is sent to request cancellation of a stream, or
        /// to indicate that an error condition has occurred.
        /// </summary>
        Reset_Stream = 0x3,
        /// <summary>
        /// The Settings frame conveys configuration parameters that
        /// affect how endpoints communicate, such as preferences and constraints
        /// on peer behavior.  The SETTINGS frame is also used to acknowledge the
        /// receipt of those parameters.  Individually, a SETTINGS parameter can
        /// also be referred to as a "setting".
        /// </summary>
        Settings = 0x4,
        /// <summary>
        /// The PUSH_PROMISE frame is used to notify the peer endpoint
        /// in advance of streams the sender intends to initiate.
        /// </summary>
        Push_Promise = 0x5,
        /// <summary>
        /// The PING frame is a mechanism for measuring a minimal
        /// round trip time from the sender, as well as determining whether an
        /// idle connection is still functional.  PING frames can be sent from
        /// any endpoint.
        /// </summary>
        Ping = 0x6,
        /// <summary>
        /// The Goaway frame informs the remote peer to stop creating
        /// streams on this connection.  GOAWAY can be sent by either the client
        /// or the server.
        /// </summary>
        Go_Away = 0x7,
        /// <summary>
        /// The WINDOW_UPDATE frame is used to implement flow control;
        /// see Section 5.2 for an overview.
        /// Flow control operates at two levels: on each individual stream and on
        /// the entire connection.
        /// </summary>
        Window_Update = 0x8,
        /// <summary>
        /// The CONTINUATION frame is used to continue a sequence of
        /// header block fragments (Section 4.3).  Any number of CONTINUATION
        /// frames can be sent, as long as the preceding frame is on the same
        /// stream and is a HEADERS, PUSH_PROMISE or CONTINUATION frame without
        /// the END_HEADERS flag set.
        /// </summary>
        Continuation = 0x9,
    }

    /// <summary>
    /// This document establishes a registry for HTTP/2 settings.  The
    /// "HTTP/2 Settings" registry manages a 16-bit space.  The "HTTP/2
    /// Settings" registry operates under the "Expert Review" policy
    /// [RFC5226] for values in the range from 0x0000 to 0xefff, with values
    /// between and 0xf000 and 0xffff being reserved for experimental use.
    /// </summary>
    public enum SettingsRegistry : byte
    {
        /// <summary>
        /// Initial Value : 4096
        /// </summary>
        Header_Table_Size = 0x1,
        /// <summary>
        /// Initial Value : 1
        /// </summary>
        Enable_Push = 0x2,
        /// <summary>
        /// Initial Value : Infinite
        /// </summary>
        Max_Concurrent_Streams = 0x3,
        /// <summary>
        /// Initial Value : 65535
        /// </summary>
        Initial_Window_Size = 0x4,
        /// <summary>
        /// Initial Value : 16383
        /// </summary>
        Max_Frame_Size = 0x5,
        /// <summary>
        /// Initial Value : Infinite
        /// </summary>
        Max_Header_List_Size = 0x6,
    }

    /// <summary>
    /// This document establishes a registry for HTTP/2 error codes.  The
    /// "HTTP/2 Error Code" registry manages a 32-bit space.  The "HTTP/2
    /// Error Code" registry operates under the "Expert Review" policy
    /// [RFC5226].
    /// </summary>
    internal enum ErrorCodeRegistry : byte
    {
        /// <summary>
        /// Graceful shutdown.
        /// </summary>
        No_Error = 0x0,
        /// <summary>
        /// Protocol error detected.
        /// </summary>
        Protocol_Error = 0x1,
        /// <summary>
        /// Implementation fault.
        /// </summary>
        Internal_Error = 0x2,
        /// <summary>
        /// Flow control limits exceeded.
        /// </summary>
        Flow_Control_Error = 0x3,
        /// <summary>
        /// Settings not acknowledged.
        /// </summary>
        Settings_Timeout = 0x4,
        /// <summary>
        /// Frame received for closed stream.
        /// </summary>
        Stream_Closed = 0x5,
        /// <summary>
        /// Frame size incorrect.
        /// </summary>
        Frame_Size_Error = 0x6,
        /// <summary>
        /// Stream not processed.
        /// </summary>
        Refused_Stream = 0x7,
        /// <summary>
        /// Stream cancelled.
        /// </summary>
        Cancel = 0x8,
        /// <summary>
        /// Compression state not updated.
        /// </summary>
        Compression_Error = 0x9,
        /// <summary>
        /// TCP connection error for CONNECT method.
        /// </summary>
        Connect_Error = 0xa,
        /// <summary>
        /// Processing capacity exceeded.
        /// </summary>
        Enhance_Your_Calm = 0xb,
        /// <summary>
        /// Negotiated TLS parameters not acceptable.
        /// </summary>
        Inadequate_Security = 0xc,
        /// <summary>
        /// Use HTTP/1.1 for the request.
        /// </summary>
        Http_1_1_Required = 0xd,
    }

    /// <summary>
    /// Frame flags.
    /// </summary>
    [Flags]
    internal enum FrameFlags
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Acknownledge.
        /// </summary>
        Ack = 0x01,
        /// <summary>
        /// End Stream.
        /// </summary>
        EndStream = 0x01,
        /// <summary>
        /// End Header.
        /// </summary>
        EndHeaders = 0x04,
        /// <summary>
        /// Has padding.
        /// </summary>
        Padded = 0x08,
        /// <summary>
        /// Compressed.
        /// </summary>
        Compressed = 0x16,
        /// <summary>
        /// Priority.
        /// </summary>
        Priority = 0x20,
    }

    /// <summary>
    /// Stream state flags.
    /// </summary>
    [Flags]
    internal enum StreamState : ushort
    {
        /// <summary>
        /// Idle.
        /// </summary>
        Idle = 0x00,
        /// <summary>
        /// Half closed remote.
        /// </summary>
        HalfClosedRemote = 0x01,
        /// <summary>
        /// Half closed local.
        /// </summary>
        HalfClosedLocal = 0x02,
        /// <summary>
        /// Opened.
        /// </summary>
        Opened = 0x04,
        /// <summary>
        /// Closed.
        /// </summary>
        Closed = 0x08,
        /// <summary>
        /// Reserved local.
        /// </summary>
        ReservedLocal = 0x10,
        /// <summary>
        /// Reserved remote.
        /// </summary>
        ReservedRemote = 0x20,
    }
}
