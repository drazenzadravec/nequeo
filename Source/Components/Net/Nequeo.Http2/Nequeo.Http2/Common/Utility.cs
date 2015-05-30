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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Security.Principal;

using Nequeo.Net;
using Nequeo.Extension;
using Nequeo.Model;
using Nequeo.Model.Message;
using Nequeo.Collections;
using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Net utility provider
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Get a new resources with the supplied request.
        /// </summary>
        /// <param name="request">The request header.</param>
        /// <returns>The request resource.</returns>
        public static RequestResource GetRequestResource(string request)
        {
            RequestResource resource = new RequestResource();
            resource.ProtocolVersion = "HTTP/2";

            // Split the request
            string[] requestItems = request.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            // For each resources.
            foreach (string item in requestItems)
            {
                // Get the name value pair.
                string[] resourceNameValue = item.Split(new string[] { "=" }, StringSplitOptions.None);

                // Get the method.
                if (resourceNameValue[0].Trim().ToLower().Equals("method"))
                    resource.Method = resourceNameValue[1].Trim();

                // Get the scheme.
                if (resourceNameValue[0].Trim().ToLower().Equals("scheme"))
                    resource.Scheme = resourceNameValue[1].Trim();

                // Get the path.
                if (resourceNameValue[0].Trim().ToLower().Equals("path"))
                    resource.Path = resourceNameValue[1].Trim();

                // Get the authority.
                if (resourceNameValue[0].Trim().ToLower().Equals("authority"))
                    resource.Authority = resourceNameValue[1].Trim();
            }

            // Return the resource.
            return resource;
        }

        /// <summary>
        /// Get a new resources with the supplied request.
        /// </summary>
        /// <param name="headers">The header list to search in.</param>
        /// <returns>The request resource.</returns>
        public static RequestResource GetRequestResource(List<NameValue> headers)
        {
            RequestResource resource = new RequestResource();
            resource.ProtocolVersion = "HTTP/2";

            // For each header.
            foreach (NameValue item in headers)
            {
                // Get the method.
                if ((item.Name.Trim().ToLower().Equals("method")) || (item.Name.Trim().ToLower().Equals(":method")))
                    resource.Method = item.Value.Trim();

                // Get the scheme.
                if ((item.Name.Trim().ToLower().Equals("scheme")) || (item.Name.Trim().ToLower().Equals(":scheme")))
                    resource.Scheme = item.Value.Trim();

                // Get the path.
                if ((item.Name.Trim().ToLower().Equals("path")) || (item.Name.Trim().ToLower().Equals(":path")))
                    resource.Path = item.Value.Trim();

                // Get the authority.
                if ((item.Name.Trim().ToLower().Equals("authority")) || (item.Name.Trim().ToLower().Equals(":authority")))
                    resource.Authority = item.Value.Trim();
            }

            // Return the resource.
            return resource;
        }

        /// <summary>
        /// Get a new resources with the supplied response.
        /// </summary>
        /// <param name="response">The response header.</param>
        /// <returns>The response resource.</returns>
        public static ResponseResource GetResponseResource(string response)
        {
            ResponseResource resource = new ResponseResource();
            resource.ProtocolVersion = "HTTP/2";

            // Split the request
            string[] requestItems = response.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            // For each resources.
            foreach (string item in requestItems)
            {
                // Get the name value pair.
                string[] resourceNameValue = item.Split(new string[] { "=" }, StringSplitOptions.None);

                // Get the status.
                if (resourceNameValue[0].Trim().ToLower().Equals("status"))
                {
                    // Try get code and sub code.
                    string[] codes = requestItems[1].Trim().Split(new char[] { '.' }, StringSplitOptions.None);
                    resource.Code = Int32.Parse(codes[0].Trim());

                    // Looking for subcode.
                    if (codes.Length > 1)
                        resource.Subcode = Int32.Parse(codes[1].Trim());
                }

                // Get the description.
                if (resourceNameValue[0].Trim().ToLower().Equals("description"))
                    resource.Description = resourceNameValue[1].Trim();
            }

            // Return the resource.
            return resource;
        }

        /// <summary>
        /// Get a new resources with the supplied response.
        /// </summary>
        /// <param name="headers">The header list to search in.</param>
        /// <returns>The response resource.</returns>
        public static ResponseResource GetResponseResource(List<NameValue> headers)
        {
            ResponseResource resource = new ResponseResource();
            resource.ProtocolVersion = "HTTP/2";

            // For each header.
            foreach (NameValue item in headers)
            {
                // Get the status.
                if ((item.Name.Trim().ToLower().Equals("status")) || (item.Name.Trim().ToLower().Equals(":status")))
                {
                    // Try get code and sub code.
                    string[] codes = item.Value.Trim().Split(new char[] { '.' }, StringSplitOptions.None);
                    resource.Code = Int32.Parse(codes[0].Trim());

                    // Looking for subcode.
                    if (codes.Length > 1)
                        resource.Subcode = Int32.Parse(codes[1].Trim());
                }

                // Get the path.
                if ((item.Name.Trim().ToLower().Equals("description")) || (item.Name.Trim().ToLower().Equals(":description")))
                    resource.Description = item.Value.Trim();
            }

            // Return the resource.
            return resource;
        }

        /// <summary>
        /// Process frame requests from the input stream within the current http context.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="canPassContext">True if the current frame processed is a data frame and can be send to the client context.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <param name="requestBufferStore">The request buffer store stream.</param>
        /// <exception cref="System.Exception"></exception>
        /// <returns>True if the processing of frames was successful; else false.</returns>
        public static bool ProcessFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, out bool canPassContext,
            long timeout = -1, int maxReadLength = 0, System.IO.Stream requestBufferStore = null)
        {
            bool ret = true;
            canPassContext = false;
            ContextStream stream = null;

            // Http context is null.
            if (httpContext == null)
                return false;

            // Http request context stream is null.
            if (httpContext.Input == null)
                return false;

            // Read the current frame.
            FrameReader frameReader = null;

            // If not using the buffer store.
            if (requestBufferStore == null)
            {
                // We need to wait until we get all the header
                // data then send the context to the server.
                frameReader = new FrameReader(httpContext.Input);
            }
            else
            {
                // We need to wait until we get all the header
                // data then send the context to the server.
                frameReader = new FrameReader(httpContext.RequestBufferStore);
            }

            try
            {
                // Attempt to read the current frame.
                Frame http2Frame = frameReader.ReadFrame(timeout);

                // If ma frame payload size.
                if (http2Frame.PayloadLength > Constants.MaxFramePayloadSize)
                {
                    // Throw exception.
                    throw new ProtocolError(ErrorCodeRegistry.Frame_Size_Error,
                        String.Format("Frame too large: Type: {0} {1}", http2Frame.FrameType, http2Frame.PayloadLength));
                }

                // Select the frame type.
                switch (http2Frame.FrameType)
                {
                    case OpCodeFrame.Continuation:
                        ProcessContinuationFrameRequest(httpContext, http2Frame as ContinuationFrame, out stream, out canPassContext);
                        break;

                    case OpCodeFrame.Data:
                        ProcessDataFrameRequest(httpContext, http2Frame as DataFrame, out stream, out canPassContext);
                        break;

                    case OpCodeFrame.Go_Away:
                        ProcessGoAwayFrameRequest(httpContext, http2Frame as GoAwayFrame);
                        break;

                    case OpCodeFrame.Headers:
                        ProcessHeaderFrameRequest(httpContext, http2Frame as HeadersFrame, out stream, out canPassContext);
                        break;

                    case OpCodeFrame.Ping:
                        ProcessPingFrameRequest(httpContext, http2Frame as PingFrame);
                        break;

                    case OpCodeFrame.Priority:
                        ProcessPriorityFrameRequest(httpContext, http2Frame as PriorityFrame, out stream);
                        break;

                    case OpCodeFrame.Push_Promise:
                        ProcessPushPromiseFrameRequest(httpContext, http2Frame as PushPromiseFrame, out stream);
                        break;

                    case OpCodeFrame.Reset_Stream:
                        ProcessResetStreamFrameRequest(httpContext, http2Frame as RstStreamFrame, out stream);
                        break;

                    case OpCodeFrame.Settings:
                        ProcessSettingFrameRequest(httpContext, http2Frame as SettingsFrame);
                        break;

                    case OpCodeFrame.Window_Update:
                        ProcessWindowUpdateFrameRequest(httpContext, http2Frame as WindowUpdateFrame, out stream);
                        break;

                    default:
                        /*  Implementations MUST treat the receipt of an unknown frame type
                            (any frame types not defined in this document) as a connection
                            error of type PROTOCOL_ERROR. */
                        throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Unknown frame type detected.");
                }

                // If the frame is type end stream frame and end of stream is true.
                if (http2Frame is IEndStreamFrame && ((IEndStreamFrame)http2Frame).IsEndStream)
                {
                    if (stream != null)
                    {
                        // Half closed remote state.
                        stream.HalfClosedRemote = true;
                    }

                    // Promised resource has been pushed.
                    if (httpContext.PromisedResources.ContainsKey(stream.StreamId))
                        httpContext.PromisedResources.Remove(stream.StreamId);
                }

                if (stream != null)
                {
                    // Increment the frame received count.
                    stream.FramesReceived++;
                }

                // Set the return value to true
                // att this point no errors have
                // been head.
                ret = true;
            }
            catch (StreamNotFoundException snfex)
            {
                /*  5.1.  Stream States
                    An endpoint MUST NOT send frames on a closed stream.  An endpoint
                    that receives a frame after receiving a RST_STREAM [RST_STREAM] or
                    a frame containing a END_STREAM flag on that stream MUST treat
                    that as a stream error (Section 5.4.2) of type STREAM_CLOSED [STREAM_CLOSED].*/
                ProcessStreamNotFoundFrame(httpContext, snfex.StreamId);
                if (stream != null) stream.WasResetSent = true;
                ret = true;
            }
            catch (ProtocolError pex)
            {
                // Close the connection.
                ProcessCloseFrame(httpContext, pex.Code);
                ret = true;
            }
            catch (MaxConcurrentStreamsLimitException)
            {
                // Close the connection.
                ProcessCloseFrame(httpContext, ErrorCodeRegistry.No_Error);
                ret = true;
            }
            catch (Exception)
            {
                // Close the connection.
                ProcessCloseFrame(httpContext, ErrorCodeRegistry.Internal_Error);
                ret = false;
            }
            
            // Return the result.
            return ret;
        }

        /// <summary>
        /// Has all the initial frame data been read.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="output">The stream to store the current data read from the input stream.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseInitialFramePreamble(System.IO.Stream input, System.IO.Stream output = null, int maxReadLength = 0)
        {
            int bytesRead = 0;
            int readOutputBytes = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            
            // Copy the data to the stream.
            if (output != null)
            {
                // If data exists.
                if (output.Length > 0)
                {
                    // Load the current output data into the store.
                    store = new byte[(int)output.Length];
                    readOutputBytes = output.Read(store, 0, store.Length);
                }
            }

            // While the end of the header data
            // has not been found.
            while (!foundEndOfData)
            {
                // Read a single byte from the stream
                // add the data to the store and re-assign.
                bytesRead = input.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Each time data is read reset the timeout.
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length >= Constants.FramePreambleSize)
                    {
                        // The end of the header data has been found.
                        foundEndOfData = true;
                        break;
                    }
                }
                else break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // Copy the data to the stream.
            if (output != null)
            {
                // If data exists.
                if (output.Length > 0)
                {
                    // Write the current data.
                    output.Write(store, readOutputBytes, (store.Length - readOutputBytes));
                }
                else
                {
                    // Write the current data.
                    output.Write(store, 0, store.Length);
                }
            }

            // If the end of headers has been found.
            if (foundEndOfData)
            {
                // Return the data found.
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Process close connection frame.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="errorCode">Error code registry.</param>
        private static void ProcessCloseFrame(Nequeo.Net.Http2.HttpContext httpContext, ErrorCodeRegistry errorCode)
        {
            // Dispose of all session streams
            foreach (var stream in httpContext.ContextStreams.Values)
            {
                // Cancel the stream.
                ProcessClose(httpContext, ErrorCodeRegistry.No_Error, stream);
            }

            // Create the GoAway frame.
            // If there is only the control stream zero then use that
            // else try get the last good stream that was used.
            ProcessCloseGoAwayFrame(httpContext, errorCode, httpContext.ContextStreams.Count > 0 ? httpContext.StreamId : 0);

            // Wait for a while, to let all data to be sent.
            using (var goAwayDelay = new ManualResetEvent(false))
            {
                goAwayDelay.WaitOne(500);
            }
        }

        /// <summary>
        /// Process stream not found frame.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="streamID">The current stream id.</param>
        private static void ProcessStreamNotFoundFrame(Nequeo.Net.Http2.HttpContext httpContext, int streamID)
        {
            // Create the reset stream frame response.
            RstStreamFrame frame = new RstStreamFrame(streamID, ErrorCodeRegistry.Stream_Closed);

            // Write the frame.
            httpContext.ResponseWrite(frame.Buffer);
        }

        /// <summary>
        /// Process close connection frame.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="errorCode">Error code registry.</param>
        /// <param name="stream">The current stream.</param>
        internal static void ProcessClose(Nequeo.Net.Http2.HttpContext httpContext, ErrorCodeRegistry errorCode, ContextStream stream)
        {
            // If the stream has been cancelled of an internal error.
            if (errorCode == ErrorCodeRegistry.Cancel || errorCode == ErrorCodeRegistry.Internal_Error)
                ProcessCloseRstStreamFrame(httpContext, errorCode, stream.StreamId);

            // Close flow control manager.
            stream.FlowControlManager.StreamClosedHandler(stream);

            // Indicate the stream is closed.
            stream.Closed = true;
            stream.Opened = false;

            // Remove the stream from this list.
            httpContext.RemoveStream(stream.StreamId);
        }

        /// <summary>
        /// Proess close goaway frame.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="errorCode">Error code registry.</param>
        /// <param name="lastStreamID">The last good stream id.</param>
        private static void ProcessCloseGoAwayFrame(Nequeo.Net.Http2.HttpContext httpContext, ErrorCodeRegistry errorCode, int lastStreamID)
        {
            // Create the goaway frame response.
            GoAwayFrame frame = new GoAwayFrame(lastStreamID, errorCode);

            // Write the frame.
            httpContext.ResponseWrite(frame.Buffer);
        }

        /// <summary>
        /// Proess close rest stream frame.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="errorCode">Error code registry.</param>
        /// <param name="streamID">The current stream id.</param>
        internal static void ProcessCloseRstStreamFrame(Nequeo.Net.Http2.HttpContext httpContext, ErrorCodeRegistry errorCode, int streamID)
        {
            // Create the reset stream frame response.
            RstStreamFrame frame = new RstStreamFrame(streamID, errorCode);

            // Write the frame.
            httpContext.ResponseWrite(frame.Buffer);
        }

        /// <summary>
        /// Process a ping frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="pingFrame">The ping frame.</param>
        private static void ProcessPingFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, PingFrame pingFrame)
        {
            /*  PING frames are not associated with any individual stream.  If a PING
                frame is received with a stream identifier field value other than
                0x0, the recipient MUST respond with a connection error of type PROTOCOL_ERROR. */
            if (pingFrame.StreamId != 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Incoming ping frame with stream id not equal to 0.");

            // If the payload length is incorrect.
            if (pingFrame.PayloadLength != PingFrame.DefPayloadLength)
                throw new ProtocolError(ErrorCodeRegistry.Frame_Size_Error, "Ping payload size is not equal to " + PingFrame.DefPayloadLength.ToString());

            // Create the ping frame response.
            var frame = new PingFrame(true, pingFrame.Payload.ToArray());

            // Write the frame.
            httpContext.ResponseWrite(frame.Buffer);
        }

        /// <summary>
        /// Process a push promise frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="pushPromiseFrame">The push promise frame.</param>
        /// <param name="stream">The selected stream context.</param>
        private static void ProcessPushPromiseFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, PushPromiseFrame pushPromiseFrame, out ContextStream stream)
        {
            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(pushPromiseFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();
        }

        /// <summary>
        /// Process a window update frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="windowUpdateFrame">The window update frame.</param>
        /// <param name="stream">The selected stream context.</param>
        private static void ProcessWindowUpdateFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, WindowUpdateFrame windowUpdateFrame, out ContextStream stream)
        {
            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(windowUpdateFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();
        }

        /// <summary>
        /// Process a data frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="dataFrame">The data frame.</param>
        /// <param name="stream">The selected stream context.</param>
        /// <param name="canPassContext">Can the data be sent to the client context.</param>
        private static void ProcessDataFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, DataFrame dataFrame, out ContextStream stream, out bool canPassContext)
        {
            // The data can be sent to the client
            // through the http context session.
            canPassContext = false;

            /*  DATA frames MUST be associated with a stream. If a DATA frame is
                received whose stream identifier field is 0x0, the recipient MUST 
                respond with a connection error of type PROTOCOL_ERROR. */
            if (dataFrame.StreamId == 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Incoming continuation frame with stream id is equal to 0.");

            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(dataFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();

            // Set the current stream id.
            httpContext.StreamId = stream.StreamId;

            // Is the data compressed.
            stream.HttpRequest.IsCompressed = dataFrame.IsCompressed;
            stream.HttpRequest.IsEndOfData = dataFrame.IsEndStream;

            // Write the data to the buffer.
            stream.HttpRequest.Input.Write(dataFrame.Data.ToArray(), 0, dataFrame.Data.Count);
            canPassContext = true;
        }

        /// <summary>
        /// Process a priority frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="priorityFrame">The priority frame.</param>
        /// <param name="stream">The selected stream context.</param>
        private static void ProcessPriorityFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, PriorityFrame priorityFrame, out ContextStream stream)
        {
            /*  The PRIORITY frame is associated with an existing stream. If a
                PRIORITY frame is received with a stream identifier of 0x0, the
                recipient MUST respond with a connection error of type PROTOCOL_ERROR. */
            if (priorityFrame.StreamId == 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Incoming priority frame with stream id equal to 0.");

            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(priorityFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();

            /*  The PRIORITY frame can be sent on a stream in any of the "reserved
                (remote)", "open", "half-closed (local)", or "half closed (remote)"
                states, though it cannot be sent between consecutive frames that
                comprise a single header block. */

            if (stream.Closed)
                throw new StreamNotFoundException(priorityFrame.StreamId);

            if (!(stream.Opened || stream.ReservedRemote || stream.HalfClosedLocal))
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Priority for non opened or reserved stream.");

            // If using priorities.
            if (httpContext.UsePriorities)
            {
                // Set the priority weight.
                stream.Priority = priorityFrame.Weight;
            }
        }

        /// <summary>
        /// Process a header frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="headerFrame">The header frame.</param>
        /// <param name="stream">The selected stream context.</param>
        /// <param name="canPassContext">Can the data be sent to the client context.</param>
        private static void ProcessHeaderFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, 
            HeadersFrame headerFrame, out ContextStream stream, out bool canPassContext)
        {
            // The data can be sent to the client
            // through the http context session.
            canPassContext = false;

            /*  HEADERS frames MUST be associated with a stream.  If a HEADERS frame
                is received whose stream identifier field is 0x0, the recipient MUST
                respond with a connection error of type PROTOCOL_ERROR. */
            if (headerFrame.StreamId == 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Incoming headers frame with stream id is equal to 0.");

            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(headerFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();

            // Get the number of compressed headers.
            var serializedHeaders = new byte[headerFrame.CompressedHeaders.Count];

            // Copy the compressed frame.
            Buffer.BlockCopy(headerFrame.CompressedHeaders.Array, headerFrame.CompressedHeaders.Offset, serializedHeaders, 0, serializedHeaders.Length);

            // Decompress the compressed headers.
            HeadersList decompressedHeaders = new HeaderCompression().Decompress(serializedHeaders);
            HeadersList headers = new HeadersList(decompressedHeaders);

            // Add the list of headers.
            foreach (KeyValuePair<string, string> header in headers)
                stream.HttpRequest.OriginalHeaders.Add(new NameValue() { Name = header.Key, Value = header.Value });

            // Determine if all headers have been found.
            if (headerFrame.IsEndHeaders)
            {
                // The end of the headers has been found.
                stream.HttpRequest.HeadersFound = true;
            }

            bool wasValidated = false;

            // Check the current stream state.
            if (stream.Idle)
            {
                // Validate all the headers.
                httpContext.ValidateHeaders(stream);
                wasValidated = true;
            }
            else if (stream.ReservedRemote)
            {
                // Validate all the headers.
                stream.HalfClosedLocal = true;
                httpContext.ValidateHeaders(stream);
                wasValidated = true;
            }
            else if (stream.Opened || stream.HalfClosedLocal)
            {
                // Validate all the headers.
                httpContext.ValidateHeaders(stream);
                wasValidated = true;
            }
            else if (stream.HalfClosedRemote)
            {
                // Half closed remote stream.
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Header for half closed remote stream.");
            }
            else
            {
                // Stream has no state error.
                throw new StreamNotFoundException(headerFrame.StreamId);
            }

            // If the headers where validated.
            if (wasValidated)
            {
                // If headers have been found.
                if (stream.HttpRequest.HeadersFound)
                {
                    // Set the current stream id
                    httpContext.StreamId = stream.StreamId;
                    stream.HttpRequest.IsEndOfData = headerFrame.IsEndStream;

                    // Get the request resources.
                    RequestResource resource = Nequeo.Net.Http2.Utility.GetRequestResource(stream.HttpRequest.OriginalHeaders);

                    // Assign the http request content.
                    stream.HttpRequest.ReadRequestHeaders(stream.HttpRequest.OriginalHeaders, resource);

                    // If this is the last bit of data
                    // that has been sent then sent
                    // the data to the client context.
                    if (headerFrame.IsEndStream)
                        canPassContext = true;
                }
            }
        }

        /// <summary>
        /// Process a continuation frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="continuationFrame">The continuation frame.</param>
        /// <param name="stream">The selected stream context.</param>
        /// <param name="canPassContext">Can the data be sent to the client context.</param>
        private static void ProcessContinuationFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, 
            ContinuationFrame continuationFrame, out ContextStream stream, out bool canPassContext)
        {
            // The data can be sent to the client
            // through the http context session.
            canPassContext = false;

            /*  CONTINUATION frames MUST be associated with a stream. If a CONTINUATION 
                frame is received whose stream identifier field is 0x0, the recipient MUST
                respond with a connection error of type PROTOCOL_ERROR. */
            if (continuationFrame.StreamId == 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Incoming continuation frame with stream id is equal to 0.");

            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(continuationFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();

            // Get the number of compressed headers.
            var serializedHeaders = new byte[continuationFrame.CompressedHeaders.Count];

            // Copy the compressed frame.
            Buffer.BlockCopy(continuationFrame.CompressedHeaders.Array, continuationFrame.CompressedHeaders.Offset, serializedHeaders, 0, serializedHeaders.Length);

            // Decompress the compressed headers.
            HeadersList decompressedHeaders = new HeaderCompression().Decompress(serializedHeaders);
            HeadersList headers = new HeadersList(decompressedHeaders);

            // Add the list of headers.
            foreach (KeyValuePair<string, string> header in headers)
                stream.HttpRequest.OriginalHeaders.Add(new NameValue() { Name = header.Key, Value = header.Value });
            
            // Determine if all headers have been found.
            if (continuationFrame.IsEndHeaders)
            {
                // The end of the headers has been found.
                stream.HttpRequest.HeadersFound = true;
            }

            bool wasValidated = false;

            // Check the current stream state.
            if (stream.Idle || stream.ReservedRemote)
            {
                // Validate all the headers.
                httpContext.ValidateHeaders(stream);
                wasValidated = true;
            }
            else if (stream.Opened || stream.HalfClosedLocal)
            {
                // Validate all the headers.
                httpContext.ValidateHeaders(stream);
                wasValidated = true;
            }
            else if (stream.HalfClosedRemote)
            {
                // Half closed remote stream.
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Continuation for half closed remote stream.");
            }
            else
            {
                // Stream has no state error.
                throw new StreamNotFoundException(continuationFrame.StreamId);
            }

            // If the headers where validated.
            if (wasValidated)
            {
                // If headers have been found.
                if (stream.HttpRequest.HeadersFound)
                {
                    // Set the current stream id
                    httpContext.StreamId = stream.StreamId;
                    stream.HttpRequest.IsEndOfData = continuationFrame.IsEndStream;

                    // Get the request resources.
                    RequestResource resource = Nequeo.Net.Http2.Utility.GetRequestResource(stream.HttpRequest.OriginalHeaders);

                    // Assign the http request content.
                    stream.HttpRequest.ReadRequestHeaders(stream.HttpRequest.OriginalHeaders, resource);

                    // If this is the last bit of data
                    // that has been sent then sent
                    // the data to the client context.
                    if (continuationFrame.IsEndStream)
                        canPassContext = true;
                }
            }
        }

        /// <summary>
        /// Process a setting frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="settingFrame">The setting frame.</param>
        private static void ProcessSettingFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, SettingsFrame settingFrame)
        {
            bool isAck = false;

            /*  If an endpoint receives a SETTINGS frame whose stream identifier 
                field is other than 0x0, the endpoint MUST respond with a connection
                error of type PROTOCOL_ERROR. */
            if (settingFrame.StreamId != 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Settings frame stream id is not equal to 0.");

            /*  Receipt of a SETTINGS frame with the ACK flag set and a length
                field value other than 0 MUST be treated as a connection error
                of type FRAME_SIZE_ERROR. */
            if (settingFrame.IsAck)
            {
                if (settingFrame.PayloadLength != 0)
                    throw new ProtocolError(ErrorCodeRegistry.Frame_Size_Error,
                        "Settings frame with ACK flag set and non-zero payload.");

                isAck = true;
            }

            // If is not acknowledged.
            if (!isAck)
            {
                // For the settings data received.
                for (int i = 0; i < settingFrame.EntryCount; i++)
                {
                    SettingsPair setting = settingFrame[i];
                    switch (setting.Id)
                    {
                        case SettingsRegistry.Enable_Push:
                            httpContext.SetSettingsPair(SettingsRegistry.Enable_Push, setting.Value);
                            break;

                        case SettingsRegistry.Header_Table_Size:
                            httpContext.SetSettingsPair(SettingsRegistry.Header_Table_Size, setting.Value);
                            break;

                        case SettingsRegistry.Initial_Window_Size:
                            httpContext.SetSettingsPair(SettingsRegistry.Initial_Window_Size, setting.Value);
                            break;

                        case SettingsRegistry.Max_Concurrent_Streams:
                            httpContext.SetSettingsPair(SettingsRegistry.Max_Concurrent_Streams, setting.Value);
                            break;

                        case SettingsRegistry.Max_Frame_Size:
                            httpContext.SetSettingsPair(SettingsRegistry.Max_Frame_Size, setting.Value);
                            break;

                        case SettingsRegistry.Max_Header_List_Size:
                            httpContext.SetSettingsPair(SettingsRegistry.Max_Header_List_Size, setting.Value);
                            break;

                        default:
                            /*  An endpoint that receives a SETTINGS frame with any other identifier
                                MUST treat this as a connection error of type PROTOCOL_ERROR. */
                            throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Unknown setting identifier.");
                    }
                }
            }

            // If is not acknowledged.
            if (!settingFrame.IsAck)
            {
                // Sending ACK settings
                SettingsFrame frame = new SettingsFrame(new List<SettingsPair>(new SettingsPair[0]), true);

                // Write the frame.
                httpContext.ResponseWrite(frame.Buffer);
            }
        }

        /// <summary>
        /// Process a goaway frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="goAwayFrame">The goaway frame.</param>
        private static void ProcessGoAwayFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, GoAwayFrame goAwayFrame)
        {
            if (goAwayFrame.StreamId != 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "GoAway frame stream id is not equal to 0.");

            // Close the connection.
            throw new ProtocolError(ErrorCodeRegistry.No_Error, "Close the session and connection.");
        }

        /// <summary>
        /// Process a reset stream frame request.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="resetFrame">The reset stream frame.</param>
        /// <param name="stream">The selected stream context.</param>
        private static void ProcessResetStreamFrameRequest(Nequeo.Net.Http2.HttpContext httpContext, RstStreamFrame resetFrame, out ContextStream stream)
        {
            /*  RST_STREAM frames MUST be associated with a stream.  If a RST_STREAM
                frame is received with a stream identifier of 0x0, the recipient MUST
                treat this as a connection error of type PROTOCOL_ERROR. */
            if (resetFrame.StreamId == 0)
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Reset stream frame with stream id is equal to 0.");

            // Attempt to get the sepcific stream.
            stream = httpContext.GetStream(resetFrame.StreamId);
            if (stream == null)
                throw new MaxConcurrentStreamsLimitException();

            // If the stream is closed.
            if (stream.Closed)
            {
                /* 12 -> 5.4.2
                An endpoint MUST NOT send a RST_STREAM in response to an RST_STREAM
                frame, to avoid looping. */
                if (!stream.WasResetSent)
                {
                    throw new StreamNotFoundException(resetFrame.StreamId);
                }
                return;
            }

            if (!(stream.ReservedRemote || stream.Opened || stream.HalfClosedLocal))
                throw new ProtocolError(ErrorCodeRegistry.Protocol_Error, "Reset stream for non opened or reserved stream.");

            // Close the current stream.
            ProcessClose(httpContext, ErrorCodeRegistry.No_Error, stream);
        }
    }
}
