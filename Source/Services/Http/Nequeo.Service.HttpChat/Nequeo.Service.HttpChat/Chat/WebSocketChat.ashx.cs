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
using System.Linq;
using System.Web;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

using Nequeo.Extension;

namespace Nequeo.Service.Chat
{
    /// <summary>
    /// Chat web socket service.
    /// </summary>
    public class WebSocketChat : Nequeo.Web.WebSocketHandler
    {
        #region Constructors
        /// <summary>
        /// Chat web socket service.
        /// </summary>
        public WebSocketChat() 
        {
            OnChatServerInitialise();
            SetMachineName();
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;

        private Nequeo.Data.Provider.IToken _token = null;
        private Nequeo.Data.Provider.ICommunication _communication = null;

        private string _machineName = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the token provider.
        /// </summary>
        public Nequeo.Data.Provider.IToken TokenProvider
        {
            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// Gets or sets the communication provider.
        /// </summary>
        public Nequeo.Data.Provider.ICommunication CommunicationProvider
        {
            get { return _communication; }
            set { _communication = value; }
        }
        #endregion

        #region Private Base Server Initialiser Members
        /// <summary>
        /// Initialise the chat server.
        /// </summary>
        private void OnChatServerInitialise()
        {
            base.Timeout = 60;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.Name = "Nequeo Chat Web Socket Server";
            base.ServiceName = "ServiceWebSocketChat";

            if (CommunicationProvider == null)
                CommunicationProvider = Common.Helper.Communication;

            if (TokenProvider == null)
                TokenProvider = Common.Helper.Token;

            if (IntegrationContext == null)
                IntegrationContext = Common.Helper.IntegrationContext;

            if (MemberContextManager == null)
                MemberContextManager = Common.Helper.MemberContextManager;

            if (TimeoutManager == null)
                TimeoutManager = Common.Helper.TimeoutManager;
        }

        /// <summary>
        /// On web socket context.
        /// </summary>
        /// <param name="webSocketContext">The asp net web socket context.</param>
        public override void WebSocketContext(System.Web.WebSockets.AspNetWebSocketContext webSocketContext)
        {
            OnChatWebSocketContext(webSocketContext);
        }
        #endregion

        #region Private Base Server Members
        /// <summary>
        /// On web socket context.
        /// </summary>
        /// <param name="webSocketContext">The asp net web socket context.</param>
        private async void OnChatWebSocketContext(System.Web.WebSockets.AspNetWebSocketContext context)
        {
            WebSocket webSocket = null;
            Nequeo.Net.WebSockets.WebSocketMember member = null;
            Nequeo.Collections.CircularBuffer<byte> requestBuffer = null;
            Nequeo.IO.Stream.StreamBufferBase requestStream = null;

            try
            {
                // Get the current web socket.
                webSocket = context.WebSocket;

                // Create the web socket member and
                // add to the member collection.
                member = new Nequeo.Net.WebSockets.WebSocketMember(webSocket);
                AddMember(member);

                // Holds the receive data.
                bool hasBeenFound = false;
                byte[] store = new byte[0];
                byte[] receiveBuffer = new byte[READ_BUFFER_SIZE];

                // Create the stream buffers.
                requestBuffer = new Collections.CircularBuffer<byte>(base.RequestBufferCapacity);
                requestStream = new Nequeo.IO.Stream.StreamBufferBase(requestBuffer);
                requestBuffer.RemoveItemsWritten = true;

                // Create the current chat state.
                Chat.ChatWebSocketState chatState = new Chat.ChatWebSocketState() { Member = member, RequestStream = requestStream, WebSocket = webSocket };
                CancellationTokenSource receiveCancelToken = new CancellationTokenSource();

                // While the WebSocket connection remains open run a 
                // simple loop that receives data and sends it back.
                while (webSocket.State == WebSocketState.Open)
                {
                    // Receive the next set of data.
                    ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(receiveBuffer);
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(arrayBuffer, receiveCancelToken.Token);

                    // Assign the member properties.
                    member.ReceiveResult = receiveResult;
                    member.TimeoutTime = DateTime.Now;
                    requestStream.Write(receiveBuffer, 0, receiveResult.Count);

                    // If the connection has been closed.
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        // Close the connection.
                        member.Close();
                        break;
                    }
                    else
                    {
                        // Store the data.
                        byte[] temp = null;
                        if (!hasBeenFound)
                        {
                            temp = store.CombineParallel(receiveBuffer);

                            // Find the end of the data.
                            hasBeenFound = Nequeo.Net.Utility.IsParse2CRLF(temp);

                            // Store the data until the end.
                            store = temp;
                            temp = null;
                        }
                        else
                        {
                            // If this is the end of the message.
                            if (receiveResult.EndOfMessage)
                            {
                                // Clear the store.
                                store = null;
                                store = new byte[0];
                                hasBeenFound = false;
                                string resource = "";

                                // Get the request headers.
                                List<Nequeo.Model.NameValue> headers = base.ParseHeaders(requestStream, out resource, base.HeaderTimeout);

                                // All headers have been found.
                                if (headers != null)
                                {
                                    // Get the execution member.
                                    // Set the calling member.
                                    string executionMember = headers.First(m => m.Name.ToUpper().Contains("MEMBER")).Value;
                                    string actionName = headers.First(m => m.Name.ToUpper().Contains("ACTIONNAME")).Value;

                                    // Assign the values.
                                    chatState.Headers = headers;
                                    chatState.ExecutionMember = executionMember;
                                    chatState.ErrorCode = new Exceptions.ErrorCodeException("OK", 200);

                                    try
                                    {
                                        // Validate the current user token.
                                        bool isTokenValid = ValidateToken(chatState);
                                    }
                                    catch (Exceptions.ErrorCodeException exc)
                                    {
                                        // Get the error code.
                                        chatState.ErrorCode = exc;
                                    }
                                    catch (Exception ex)
                                    {
                                        // Internal error.
                                        chatState.ErrorCode = new Exceptions.ErrorCodeException(ex.Message, 500);
                                    }

                                    // Send a message back to the client indicating that
                                    // the message was recivied and was sent.
                                    await webSocket.SendAsync(new ArraySegment<byte>(
                                        CreateResponse(chatState.ErrorCode.ErrorCode, true, executionMember, actionName, chatState.ErrorCode.Message)),
                                        WebSocketMessageType.Binary, true, CancellationToken.None);
                                }
                            }
                        }
                    }
                }

                // Cancel the receive request.
                if (webSocket.State != WebSocketState.Open)
                    receiveCancelToken.Cancel();
            }
            catch { }
            finally
            {
                // If a member context exists.
                if (member != null)
                {
                    try
                    {
                        // Remove the member context
                        // from the collection.
                        RemoveMember(member);
                    }
                    catch { }
                    member = null;
                }

                // Clean up by disposing the WebSocket.
                if (webSocket != null)
                    webSocket.Dispose();

                if (requestBuffer != null)
                    requestBuffer.Dispose();

                if (requestStream != null)
                    requestStream.Dispose();

                if (_communication != null)
                {
                    try
                    {
                        // Remove the client from the communication service.
                        _communication.RemoveClient(member.UniqueIdentifier, base.ServiceName, _machineName, null, actionName: member.UniqueIdentifier);
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Private Action Members
        /// <summary>
        /// The action to perform.
        /// </summary>
        /// <param name="chatState">The current chat state.</param>
        private void Action(Chat.ChatWebSocketState chatState)
        {
            try
            {
                // Select the member to execute.
                switch (chatState.ExecutionMember.ToUpper())
                {
                    case "DATA":
                        // SendMessage
                        SendMessage(chatState);
                        break;

                    default:
                        throw new Exception("Command not recognised.");
                }
            }
            catch (Exceptions.ErrorCodeException exc)
            {
                // Get the error code.
                chatState.ErrorCode = exc;
            }
            catch (Exception ex)
            {
                // Internal error.
                chatState.ErrorCode = new Exceptions.ErrorCodeException(ex.Message, 500);
            }
        }

        /// <summary>
        /// Send the message to the client.
        /// </summary>
        /// <param name="chatState">The current chat state.</param>
        /// <returns>True if error; else false.</returns>
        private bool SendMessage(Chat.ChatWebSocketState chatState)
        {
            bool isError = true;

            // If the headers do not exist.
            if ((chatState.Headers.Count(m => m.Name.ToUpper().Contains("FSN")) < 1) ||
                (chatState.Headers.Count(m => m.Name.ToUpper().Contains("SENDTO")) < 1))
            {
                // Bad request exception.
                throw new Exceptions.ErrorCodeException("Invalid Request.", 400);
            }

            // Get the header data.
            string[] sendTo = null;
            string serviceName = chatState.Headers.First(m => m.Name.ToUpper().Contains("FSN")).Value;

            // If header send to.
            if (!String.IsNullOrEmpty(chatState.Headers.First(m => m.Name.ToUpper().Contains("SENDTO")).Value))
                sendTo = chatState.Headers.First(m => m.Name.ToUpper().Contains("SENDTO")).Value.Split(new char[] { ';' }, StringSplitOptions.None);

            // Create the buffer
            byte[] buffer = new byte[chatState.RequestStream.Length];

            // Read the current set of bytes.
            int bytesRead = chatState.RequestStream.Read(buffer, 0, buffer.Length);

            // Send to clients
            Common.Helper.SendToReceivers(chatState.Member.UniqueIdentifier, base.ServiceName, serviceName, sendTo, buffer);
            isError = false;

            // Return the result.
            return isError;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Validate the current user token.
        /// </summary>
        /// <param name="chatState">The current chat state.</param>
        /// <returns>True if valid; else false.</returns>
        private bool ValidateToken(Chat.ChatWebSocketState chatState)
        {
            // If the headers do not exist.
            if ((chatState.Headers.Count(m => m.Name.ToUpper().Contains("FUI")) < 1) ||
                (chatState.Headers.Count(m => m.Name.ToUpper().Contains("TOKEN")) < 1))
            {
                // Bad request exception.
                throw new Exceptions.ErrorCodeException("Invalid Request.", 400);
            }

            // Get the header data.
            string uniqueIdentifier = chatState.Headers.First(m => m.Name.ToUpper().Contains("FUI")).Value;
            string token = chatState.Headers.First(m => m.Name.ToUpper().Contains("TOKEN")).Value;

            // If the unique identifier
            // has not been set then the
            // token has not been verified.
            if (String.IsNullOrEmpty(chatState.Member.UniqueIdentifier))
            {
                // Is token valid.
                _token.IsValid(uniqueIdentifier, base.ServiceName, token, (result, permission, state) =>
                {
                    Chat.ChatWebSocketState stateChat = null;

                    try
                    {
                        // Get the state chat.
                        stateChat = (Chat.ChatWebSocketState)state;

                        // If the token is valid.
                        if (result)
                        {
                            // Add this client to the communication store.
                            _communication.AddClient(stateChat.Member.UniqueIdentifier, base.ServiceName, _machineName,
                                null, true, token, stateChat.Member.UniqueIdentifier);

                            // Set the current unique identifier
                            // and the service name.
                            stateChat.Member.ServiceName = base.ServiceName;
                            stateChat.Member.UniqueIdentifier = uniqueIdentifier;

                            // Perform the action.
                            Action(stateChat);
                        }
                        else
                        {
                            // Bad request.
                            stateChat.ErrorCode = new Exceptions.ErrorCodeException("The token is not valid.", 400);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Internal error.
                        stateChat.ErrorCode = new Exceptions.ErrorCodeException(ex.Message, 500);
                    }

                }, uniqueIdentifier, chatState);

                // Return false not valid.
                return false;
            }
            else
            {
                // Perform the action.
                Action(chatState);

                // Token is valid.
                return true;
            }
        }

        /// <summary>
        /// Create the confirmation received response.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="memberResult">The member result; true if success; else false.</param>
        /// <param name="executionMember">The execution member.</param>
        /// <param name="actionName">The member action name.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <returns>The result.</returns>
        private byte[] CreateResponse(int errorCode, bool memberResult, string executionMember, string actionName, string statusDescription)
        {
            // Get the error code.
            switch (errorCode)
            {
                case 500:
                    // Internal error.
                    return CreateInternalServerErrorHeaders(false, executionMember, actionName, statusDescription);
                case 400:
                    // Bad request.
                    return CreateBadRequestHeaders(false, executionMember, actionName, statusDescription);
                default:
                    // Default response.
                    return CreateReceivedResponse(memberResult, executionMember, actionName, statusDescription);
            }
        }

        /// <summary>
        /// Create the confirmation received response.
        /// </summary>
        /// <param name="memberResult">The member result; true if success; else false.</param>
        /// <param name="executionMember">The execution member.</param>
        /// <param name="actionName">The member action name.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <returns>The result.</returns>
        private byte[] CreateReceivedResponse(bool memberResult, string executionMember, string actionName, string statusDescription)
        {
            // Construct the message headers.
            List<Nequeo.Model.NameValue> headers = new List<Model.NameValue>()
            {
                new Model.NameValue() { Name = "Member", Value = executionMember},
                new Model.NameValue() { Name = "ActionName", Value = actionName},
                new Model.NameValue() { Name = "MemberResult", Value = memberResult.ToString()},
                new Model.NameValue() { Name = "Content-Length", Value = "0".ToString()},
            };

            // Create the header list.
            string headerList = Nequeo.Net.Utility.CreateWebResponseHeaders(headers);
            byte[] headersData = Encoding.Default.GetBytes(headerList);

            // Return the recive result.
            return headersData;
        }

        /// <summary>
        /// Create Bad Request headers.
        /// </summary>
        /// <param name="memberResult">The member result; true if success; else false.</param>
        /// <param name="executionMember">The execution member.</param>
        /// <param name="actionName">The member action name.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <returns>The result.</returns>
        private byte[] CreateBadRequestHeaders(bool memberResult, string executionMember, string actionName, string statusDescription)
        {
            // Construct the message headers.
            List<Nequeo.Model.NameValue> headers = new List<Model.NameValue>()
            {
                new Model.NameValue() { Name = "Member", Value = executionMember},
                new Model.NameValue() { Name = "ActionName", Value = actionName},
                new Model.NameValue() { Name = "MemberResult", Value = memberResult.ToString()},
                new Model.NameValue() { Name = "Content-Length", Value = "0".ToString()},
            };

            // Create the header list.
            string headerList = Nequeo.Net.Utility.CreateWebResponseHeaders(headers, statusCode: 400, statusDescription: statusDescription);
            byte[] headersData = Encoding.Default.GetBytes(headerList);

            // Return the recive result.
            return headersData;
        }

        /// <summary>
        /// Create Internal Server Error headers.
        /// </summary>
        /// <param name="memberResult">The member result; true if success; else false.</param>
        /// <param name="executionMember">The execution member.</param>
        /// <param name="actionName">The member action name.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <returns>The result.</returns>
        private byte[] CreateInternalServerErrorHeaders(bool memberResult, string executionMember, string actionName, string statusDescription)
        {
            // Construct the message headers.
            List<Nequeo.Model.NameValue> headers = new List<Model.NameValue>()
            {
                new Model.NameValue() { Name = "Member", Value = executionMember},
                new Model.NameValue() { Name = "ActionName", Value = actionName},
                new Model.NameValue() { Name = "MemberResult", Value = memberResult.ToString()},
                new Model.NameValue() { Name = "Content-Length", Value = "0".ToString()},
            };

            // Create the header list.
            string headerList = Nequeo.Net.Utility.CreateWebResponseHeaders(headers, statusCode: 500, statusDescription: statusDescription);
            byte[] headersData = Encoding.Default.GetBytes(headerList);

            // Return the recive result.
            return headersData;
        }

        /// <summary>
        /// Set the current machine name.
        /// </summary>
        private void SetMachineName()
        {
            try
            {
                // Attempt to get the local machine name.
                _machineName = Environment.MachineName;
            }
            catch { }
        }
        #endregion
    }
}