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
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net.Sockets;
using Nequeo.Security;
using Nequeo.Threading;
using Nequeo.Data.Enum;
using Nequeo.Data.Provider;

namespace Nequeo.Client.Chat
{
    /// <summary>
    /// Chat sorted binary search manager. Stores all chat clients and
    /// managers the searching of context items within the current machine.
    /// </summary>
    public class ChatManager : Manager
    {
        /// <summary>
        /// Chat sorted binary search manager. Stores all chat clients and
        /// managers the searching of context items within the current machine.
        /// </summary>
        public ChatManager()
        {
        }

        /// <summary>
        /// Get the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier (key).</param>
        /// <returns>The client model; else null.</returns>
        public virtual ChatModel GetClient(string uniqueIdentifier)
        {
            // Attempt to find the client.
            Nequeo.Client.Chat.IChat client = null;

            // Atempt to get the messages.
            Tuple<string, string, MessageType, byte[]>[]  messages = null;

            // Attempt to get the token.
            string token = null;

            // Invoke the finds in parallel
            Parallel.Invoke
            (
                () => client = Find(uniqueIdentifier),
                () => messages = FindMessages(uniqueIdentifier),
                () => token = FindToken(uniqueIdentifier)
            );

            // The client; else null.
            return new ChatModel() { Client = client, Messages = messages, Token = token };
        }

        /// <summary>
        /// Create and authorise a new client and connect to the server..
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier (key).</param>
        /// <param name="createAction">If not null then the action is executed to create a client.</param>
        /// <param name="tokenAction">If not null then the action is executed to create a token.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="token">The token.</param>
        /// <returns>True if the client has been authorised; else false.</returns>
        public virtual bool CreateClient(string uniqueIdentifier, 
            Func<Nequeo.Client.Chat.IChat> createAction, 
            Func<string> tokenAction,
            Nequeo.Model.Credentials credentials,
            out string token)
        {
            // Attempt to find the client.
            Nequeo.Client.Chat.IChat client = null;
            string tokenInternal = null;

            // Invoke the finds in parallel
            Parallel.Invoke
            (
                () => client = Find(uniqueIdentifier),
                () => tokenInternal = FindToken(uniqueIdentifier)
            );

            // If not found.
            if (client == null)
            {
                // Create the client if does not exist.
                if (createAction != null)
                {
                    // Create the client.
                    client = createAction();
                }

                // If created.
                if (client != null)
                {
                    // Initialisation, connect and authorise.
                    client.Initialisation();
                    client.Connect();

                    // Only add the client if authorised.
                    if (client.IsAuthorised)
                    {
                        // If token action exists.
                        if (tokenAction != null)
                        {
                            // Execute the token action.
                            tokenInternal = tokenAction();
                        }
                        else
                        {
                            // Create a defalt token.
                            tokenInternal = Nequeo.Invention.TokenGenerator.Instance.Random(15);
                        }

                        // Add the client to the list.
                        CreateEx(uniqueIdentifier, client, tokenInternal);
                    }
                }
            }
            else
            {
                // If not connected then connect.
                if(!client.Connected)
                    client.Connect();

                // If not authorised then authorise.
                if(!client.IsAuthorised)
                    client.Authorise(credentials.Username, credentials.Password, credentials.Domain);
            }

            // The token.
            token = tokenInternal;

            // Has the client been authorised.
            return client.IsAuthorised;
        }

        /// <summary>
        /// Remove all client, token and message data.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier (key).</param>
        public virtual void Remove(string uniqueIdentifier)
        {
            RemoveClient(uniqueIdentifier);
            RemoveMessages(uniqueIdentifier);
            RemoveToken(uniqueIdentifier);
        }
        
        /// <summary>
        /// Add the client item.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier (key).</param>
        /// <param name="client">The client connected to the machine.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void Add(string uniqueIdentifier, Nequeo.Client.Chat.IChat client)
        {
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (client == null) throw new ArgumentNullException("client");

            // Add the item.
            base.AddClient(uniqueIdentifier, client);
        }

        /// <summary>
        /// Find the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier (key).</param>
        /// <returns>The client; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Nequeo.Client.Chat.IChat Find(string uniqueIdentifier)
        {
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            // Attempt to find the client.
            return (Nequeo.Client.Chat.IChat)base.FindClient(uniqueIdentifier);
        }

        /// <summary>
        /// Find all the clients.
        /// </summary>
        /// <param name="uniqueIdentifiers">The unique client identifiers (keys).</param>
        /// <returns>The collection of clients; else null.</returns>
        public virtual Nequeo.Client.Chat.IChat[] Find(string[] uniqueIdentifiers)
        {
            // Attempt to find the client.
            return (Nequeo.Client.Chat.IChat[])base.FindClients(uniqueIdentifiers);
        }

        /// <summary>
        /// Decrypt the token.
        /// </summary>
        /// <param name="token">The hex token to decrypt.</param>
        /// <returns>The decrypted token.</returns>
        public virtual string DecryptToken(string token)
        {
            // Create a byte array of the hex token and
            // then decrypted the hex token.
            byte[] hexToken = Nequeo.Conversion.Context.HexStringToByteArray(token);
            byte[] decryptedTokenBytes = Nequeo.Cryptography.AdvancedAES.Instance.DecryptFromMemory(hexToken);
            string decryptedToken = Encoding.Default.GetString(decryptedTokenBytes).Replace("\0", "");

            // Return the decrypted hex token.
            return decryptedToken;
        }

        /// <summary>
        /// Encrypt the token.
        /// </summary>
        /// <param name="token">The token to encrypt.</param>
        /// <returns>The encrypted token as a hex string.</returns>
        public virtual string EncryptToken(string token)
        {
            // Encrypted the token and create a hex string of
            // the encrypted token.
            byte[] tokenBytes = Encoding.Default.GetBytes(token);
            byte[] encryptedToken = Nequeo.Cryptography.AdvancedAES.Instance.EncryptToMemory(tokenBytes);
            string hexToken = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedToken);

            // Return the encrypted hex token.
            return hexToken;
        }

        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier (key).</param>
        /// <param name="client">The client connected to the machine.</param>
        /// <param name="token">The client token to add.</param>
        /// <returns>The communication token; else null.</returns>
        private void CreateEx(string uniqueIdentifier, Nequeo.Client.Chat.IChat client, string token)
        {
            // Create the connection.
            client.OnServerMessage += client_OnServerMessage;
            client.OnValidCommand += client_OnValidCommand;
            client.OnDisconnected += client_OnDisconnected;
            client.OnError += client_OnError;
            client.UniqueIdentifier = uniqueIdentifier;

            // Add the new client.
            Add(uniqueIdentifier, client);
            AddToken(uniqueIdentifier, token);
        }

        /// <summary>
        /// On disconnected.
        /// </summary>
        /// <param name="sender">The client that received the message.</param>
        /// <param name="e">The event arguments.</param>
        private void client_OnDisconnected(object sender, EventArgs e)
        {
            string uniqueIdentifier = ((Nequeo.Client.Chat.IChat)sender).UniqueIdentifier;
            RemoveClient(uniqueIdentifier);
            RemoveMessages(uniqueIdentifier);
            RemoveToken(uniqueIdentifier);
        }

        /// <summary>
        /// On error.
        /// </summary>
        /// <param name="sender">The client that received the message.</param>
        /// <param name="e1">The command data.</param>
        /// <param name="e2">The command code.</param>
        /// <param name="e3">The error message.</param>
        private void client_OnError(object sender, string e1, string e2, string e3)
        {
            // Add the new message for the client.
            string uniqueIdentifier = ((Nequeo.Client.Chat.IChat)sender).UniqueIdentifier;
            AddMessage(uniqueIdentifier, new Tuple<string, string, MessageType, byte[]>("ERRO", e1 + " " + e2, MessageType.A00, Encoding.Default.GetBytes(e3)));
        }

        /// <summary>
        /// On valid command.
        /// </summary>
        /// <param name="sender">The client that received the message.</param>
        /// <param name="e1">The command data.</param>
        /// <param name="e2">The command code.</param>
        /// <param name="e3">The error message.</param>
        private void client_OnValidCommand(object sender, string e1, string e2, string e3)
        {
            // Add the new message for the client.
            string uniqueIdentifier = ((Nequeo.Client.Chat.IChat)sender).UniqueIdentifier;
            AddMessage(uniqueIdentifier, new Tuple<string, string, MessageType, byte[]>("INFO", e1 + " " + e2, MessageType.A01, Encoding.Default.GetBytes(e3)));
        }

        /// <summary>
        /// A message from the server.
        /// </summary>
        /// <param name="sender">The client that received the message.</param>
        /// <param name="e1">The unique identifier of who sent the message.</param>
        /// <param name="e2">The description.</param>
        /// <param name="e3">The message type.</param>
        /// <param name="e4">The message.</param>
        private void client_OnServerMessage(object sender, string e1, string e2, MessageType e3, byte[] e4)
        {
            // Add the new message for the client.
            string uniqueIdentifier = ((Nequeo.Client.Chat.IChat)sender).UniqueIdentifier;
            AddMessage(uniqueIdentifier, new Tuple<string, string, MessageType, byte[]>(e1, e2, e3, e4));
        }
    }
}