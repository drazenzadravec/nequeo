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
    /// Chat http service.
    /// </summary>
    public class HttpChat : Nequeo.Service.Web.HttpHandler
    {
        #region Constructors
        /// <summary>
        /// Chat http service.
        /// </summary>
        public HttpChat()
        {
            OnChatServerInitialise();
            SetMachineName();
        }
        #endregion

        #region Private Fields
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
            base.Name = "Nequeo Chat Http Server";
            base.ServiceName = "ServiceHttpChat";

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
        /// Http context handler.
        /// </summary>
        /// <param name="context">The http context.</param>
        public override void HttpContext(HttpContext context)
        {
            System.Web.HttpRequest request = null;
            System.Web.HttpResponse response = null;
            bool isServerError = false;
            bool keepAlive = true;
            bool isError = true;

            try
            {
                request = context.Request;
                response = context.Response;

                // If headers exist.
                if (request.Headers != null)
                {
                    // Get the execution member.
                    if (!String.IsNullOrEmpty(request.Headers["Member"]))
                    {
                        // Get the execution member.
                        // Set the calling member.
                        string executionMember = request.Headers["Member"].Trim();
                        response.AddHeader("Member", executionMember);
                        response.AddHeader("ActionName", request.Headers["ActionName"]);

                        // Create the chat state.
                        Chat.ChatHttptState chatState = new Chat.ChatHttptState()
                        {
                            HttpContext = context,
                            Member = new Net.Http.HttpContextMember(context),
                            ExecutionMember = executionMember,
                            ErrorCode = new Exceptions.ErrorCodeException("OK", 200)
                        };

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
                        CreateResponse(response, true, chatState.ErrorCode.ErrorCode, statusDescription: chatState.ErrorCode.Message);
                        isError = false;
                    }
                }
                else
                {
                    // No headers have been found.
                    keepAlive = false;
                    throw new Exception("No headers have been found.");
                }

                // If error has occured.
                if (isError)
                {
                    // Send an error response.
                    response.StatusCode = 400;
                    response.StatusDescription = "Bad Request";
                    response.AddHeader("Content-Length", (0).ToString());
                    response.Flush();
                }
            }
            catch (Exception) { isServerError = true; }

            // If a server error has occured.
            if (isServerError)
            {
                // Make sure the response exists.
                if (response != null)
                {
                    try
                    {
                        // Send an error response.
                        response.StatusCode = 500;
                        response.StatusDescription = "Internal server error";
                        response.AddHeader("Content-Length", (0).ToString());
                        response.Flush();
                    }
                    catch { keepAlive = false; }
                }
            }

            // If do not keep alive.
            if (!keepAlive)
            {
                // Close the connection.
                if (response != null)
                {
                    try
                    {
                        // Close the connection.
                        response.Close();
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
        private void Action(Chat.ChatHttptState chatState)
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
        private bool SendMessage(Chat.ChatHttptState chatState)
        {
            System.Web.HttpRequest request = chatState.HttpContext.Request;
            System.Web.HttpResponse response = chatState.HttpContext.Response;

            // Get the header data.
            string[] sendTo = null;
            string serviceName = request.Headers["FSN"];

            // If header send to.
            if (!String.IsNullOrEmpty(request.Headers["SENDTO"]))
                sendTo = request.Headers["SENDTO"].Split(new char[] { ';' }, StringSplitOptions.None);

            // If sending from a query.
            if (request.QueryString != null)
            {
                // If header from service name.
                if (!String.IsNullOrEmpty(request.QueryString["FSN"]))
                    serviceName = request.QueryString["FSN"];

                // If query send to.
                if (!String.IsNullOrEmpty(request.QueryString["SENDTO"]))
                {
                    // Get the send to query from base64 encoding.
                    byte[] sendToBytes = Convert.FromBase64String(request.QueryString["SENDTO"]);
                    string sendToString = Encoding.Default.GetString(sendToBytes);
                    sendTo = sendToString.Split(new char[] { ';' }, StringSplitOptions.None);
                }
            }

            // Return the result.
            return SendMessageEx(chatState, serviceName, sendTo);
        }

        /// <summary>
        /// SendMessageToClients
        /// </summary>
        /// <param name="chatState">The current chat state.</param>
        /// <param name="serviceNameSendTo">The service name the unique identifiers are connected to.</param>
        /// <param name="sendTo">Send the data to.</param>
        /// <returns>True if error; else false.</returns>
        private bool SendMessageEx(Chat.ChatHttptState chatState, string serviceNameSendTo, string[] sendTo = null)
        {
            bool isError = true;

            System.Web.HttpRequest request = chatState.HttpContext.Request;
            System.Web.HttpResponse response = chatState.HttpContext.Response;

            // If content exists.
            if (request.ContentLength > 0)
            {
                MemoryStream sendBuffer = null;
                try
                {
                    // Read the request stream and write to the send buffer.
                    sendBuffer = new MemoryStream();
                    bool copied = Nequeo.IO.Stream.Operation.CopyStream(request.InputStream, sendBuffer, request.ContentLength, base.RequestTimeout);

                    // If all the data has been copied.
                    if (copied)
                    {
                        // Send the data to client(s) on this machine
                        // and another machine if not on this machine.
                        Common.Helper.SendToReceivers(chatState.Member.UniqueIdentifier, base.ServiceName, serviceNameSendTo, sendTo, sendBuffer.ToArray());
                        isError = false;
                    }
                }
                catch (Exception ex)
                {
                    // Internal error.
                    isError = true;
                    chatState.ErrorCode = new Exceptions.ErrorCodeException(ex.Message, 500);
                }
                finally
                {
                    // Dispose of the buffer.
                    if (sendBuffer != null)
                        sendBuffer.Dispose();
                }
            }

            // True if error else false.
            return isError;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Validate the current user token.
        /// </summary>
        /// <param name="chatState">The current chat state.</param>
        /// <returns>True if valid; else false.</returns>
        private bool ValidateToken(Chat.ChatHttptState chatState)
        {
            string uniqueIdentifier = null;
            string token = null;

            try
            {
                uniqueIdentifier = chatState.HttpContext.Request.Headers["FUI"];
                token = chatState.HttpContext.Request.Headers["TOKEN"];

                // If sending from a query.
                if (chatState.HttpContext.Request.QueryString != null)
                {
                    // If header from unique identifier.
                    if (!String.IsNullOrEmpty(chatState.HttpContext.Request.QueryString["FUI"]))
                        uniqueIdentifier = chatState.HttpContext.Request.QueryString["FUI"];

                    // If header from token.
                    if (!String.IsNullOrEmpty(chatState.HttpContext.Request.QueryString["TOKEN"]))
                        token = chatState.HttpContext.Request.QueryString["TOKEN"];
                }
            }
            catch (Exception)
            {
                // Bad request exception.
                throw new Exceptions.ErrorCodeException("Invalid Request.", 400);
            }

            // If the unique identifier
            // has not been set then the
            // token has not been verified.
            if (String.IsNullOrEmpty(chatState.Member.UniqueIdentifier))
            {
                // Is token valid.
                _token.IsValid(uniqueIdentifier, base.ServiceName, token, (result, permission, state) =>
                {
                    Chat.ChatHttptState stateChat = null;

                    try
                    {
                        // Get the state chat.
                        stateChat = (Chat.ChatHttptState)state;

                        // If the token is valid.
                        if (result)
                        {
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
        /// Create OK headers.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateOkHeaders(System.Web.HttpResponse response, long contentLength = 0)
        {
            // Send the response.
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.AddHeader("Content-Length", contentLength.ToString());
            response.Flush();
        }

        /// <summary>
        /// Create Bad Request headers.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateBadRequestHeaders(System.Web.HttpResponse response, long contentLength = 0)
        {
            // Send the response.
            response.StatusCode = 400;
            response.StatusDescription = "Bad Request";
            response.AddHeader("Content-Length", contentLength.ToString());
            response.Flush();
        }

        /// <summary>
        /// Create Internal Server Error headers.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateInternalServerErrorHeaders(System.Web.HttpResponse response, long contentLength = 0)
        {
            // Send the response.
            response.StatusCode = 500;
            response.StatusDescription = "Internal Server Error";
            response.AddHeader("Content-Length", contentLength.ToString());
            response.Flush();
        }

        /// <summary>
        /// Create the confirmation received response.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <param name="memberResult">The member result; true if success; else false.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <param name="contentLength">The content length.</param>
        private void CreateResponse(System.Web.HttpResponse response, bool memberResult, int errorCode,
            int statusCode = 200, string statusDescription = "OK", long contentLength = 0)
        {
            // Get the error code.
            switch (errorCode)
            {
                case 500:
                    // Internal error.
                    response.StatusCode = 500;
                    response.StatusDescription = statusDescription;
                    response.AddHeader("MemberResult", false.ToString());
                    break;
                case 400:
                    // Bad request.
                    response.StatusCode = 400;
                    response.StatusDescription = statusDescription;
                    response.AddHeader("MemberResult", false.ToString());
                    break;
                default:
                    // Default response.
                    response.StatusCode = statusCode;
                    response.StatusDescription = statusDescription;
                    response.AddHeader("MemberResult", memberResult.ToString());
                    break;
            }

            // Send the response.
            response.AddHeader("Content-Length", contentLength.ToString());
            response.Flush();
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