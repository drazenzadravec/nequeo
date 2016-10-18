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

using Nequeo.Extension;
using Nequeo.Model;
using Nequeo.Model.Message;

namespace Nequeo.Net
{
    /// <summary>
    /// Net utility provider
    /// </summary>
	public class Utility
    {
        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceRoot">The service root.</param>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>Return the constructed Uri.</returns>
        public static Uri CreateUri(string serviceRoot, string serviceEntityName)
        {
            // Return the URI
            return new Uri(serviceRoot.TrimEnd('/') + "/" + serviceEntityName);
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceRoot">The service root.</param>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>Return the constructed Uri.</returns>
        public static Uri CreateUri(string serviceRoot, string serviceEntityName, Nequeo.Model.NameObject[] queries)
        {
            string query = "";

            // If queries exist.
            if (queries != null && queries.Length > 0)
            {
                // Create the query.
                query = Nequeo.Net.Utility.CreateQueryString(queries);
            }

            // Return the URI
            return new Uri(serviceRoot.TrimEnd('/') + "/" + serviceEntityName + (String.IsNullOrEmpty(query) ? "" : query));
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceRoot">The service root.</param>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>Return the constructed Uri.</returns>
        public static Uri CreateUri(string serviceRoot, string serviceEntityName, Nequeo.Model.NameValue[] queries)
        {
            string query = "";

            // If queries exist.
            if (queries != null && queries.Length > 0)
            {
                // Create the query.
                query = Nequeo.Net.Utility.CreateQueryString(queries);
            }

            // Return the URI
            return new Uri(serviceRoot.TrimEnd('/') + "/" + serviceEntityName + (String.IsNullOrEmpty(query) ? "" : query));
        }

        /// <summary>
        /// Create the URI from the parameters.
        /// </summary>
        /// <param name="serviceRoot">The service root.</param>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>Return the constructed Uri.</returns>
        public static Uri CreateUri(string serviceRoot, string serviceEntityName, NameValueCollection queries)
        {
            string query = "";

            // If queries exist.
            if (queries != null && queries.Count > 0)
            {
                // Create the query.
                query = Nequeo.Net.Utility.CreateQueryString(queries);
            }

            // Return the URI
            return new Uri(serviceRoot.TrimEnd('/') + "/" + serviceEntityName + (String.IsNullOrEmpty(query) ? "" : query));
        }

        /// <summary>
        /// Create the query string from the collection.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <returns>The query string.</returns>
        public static string CreateQueryString(NameValueCollection collection)
        {
            string query = "";

            // Iterate through the collection.
            foreach (string item in collection.AllKeys)
            {
                query += item + "=" + collection[item] + "&";
            }

            // Return the query string.
            return "?" + query.TrimEnd('&');
        }

        /// <summary>
        /// Create the query string from the collection.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <returns>The query string.</returns>
        public static string CreateQueryString(Nequeo.Model.NameValue[] collection)
        {
            string query = "";

            // Iterate through the collection.
            for(int i = 0; i < collection.Length; i++)
            {
                query += collection[i].Name + "=" + collection[i].Value + "&";
            }

            // Return the query string.
            return "?" + query.TrimEnd('&');
        }

        /// <summary>
        /// Create the query string from the collection.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <returns>The query string.</returns>
        public static string CreateQueryString(Nequeo.Model.NameObject[] collection)
        {
            string query = "";

            // Iterate through the collection.
            for (int i = 0; i < collection.Length; i++)
            {
                query += collection[i].Name + "=" + collection[i].Value.ToString() + "&";
            }

            // Return the query string.
            return "?" + query.TrimEnd('&');
        }

        /// <summary>
        /// Parse the query string.
        /// </summary>
        /// <param name="query">The data to parse.</param>
        /// <returns>The name value collection.</returns>
        public static NameValueCollection ParseQueryString(string query)
        {
            NameValueCollection nameValue = new NameValueCollection();

            // If data exists.
            if (!String.IsNullOrEmpty(query))
            {
                string[] split = query.Split('&');
                foreach (string item in split)
                {
                    string[] equ = item.Split('=');
                    nameValue.Add(equ[0], equ[1]);
                }
            }
            return nameValue;
        }

        /// <summary>
        /// Parse the query string.
        /// </summary>
        /// <param name="query">The data to parse.</param>
        /// <returns>The name value collection.</returns>
        public static Nequeo.Model.NameValue[] ParseQueryStringEx(string query)
        {
            List<Nequeo.Model.NameValue> nameValue = new List<NameValue>();

            // If data exists.
            if (!String.IsNullOrEmpty(query))
            {
                string[] split = query.Split('&');
                foreach (string item in split)
                {
                    string[] equ = item.Split('=');
                    nameValue.Add(new Nequeo.Model.NameValue() { Name = equ[0], Value = equ[1] });
                }
            }
            return nameValue.ToArray();
        }

        /// <summary>
        /// Get a new resources with the supplied request.
        /// </summary>
        /// <param name="request">The request header.</param>
        /// <returns>The request resource.</returns>
        public static RequestResource GetRequestResource(string request)
        {
            RequestResource resource = new RequestResource();

            // Split the request
            string[] requestItems = request.Split(new string[] { " " }, StringSplitOptions.None);

            // Assign the resource data.
            resource.Method = requestItems[0].Trim().TrimStart(new char[] { '\0' }).ToUpper();

            if(requestItems.Length > 1)
                resource.Path = requestItems[1].Trim();
            
            if (requestItems.Length > 2)
                resource.ProtocolVersion = requestItems[2].Trim();

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

            // Split the request
            string[] requestItems = response.Split(new string[] { " " }, StringSplitOptions.None);

            // Assign the resource data.
            resource.ProtocolVersion = requestItems[0].Trim();

            if (requestItems.Length > 1)
            {
                // Try get code and sub code.
                string[] codes = requestItems[1].Trim().Split(new char[] { '.' }, StringSplitOptions.None);
                resource.Code = Int32.Parse(codes[0].Trim());

                // Looking for subcode.
                if(codes.Length > 1)
                    resource.Subcode = Int32.Parse(codes[1].Trim());
            }

            if (requestItems.Length > 2)
            {
                string description = "";
                for (int i = 2; i < requestItems.Length; i++)
                    description += requestItems[i] + " ";

                resource.Description = description.Trim();
            }
            
            // Return the resource.
            return resource;
        }

        /// <summary>
        /// Create the web response headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="protocolVersion">The protocol version.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <param name="statusSubcode">The status subcode.</param>
        /// <returns>The string that represents the headers.</returns>
        public static string CreateWebResponseHeaders(List<NameValue> headers,
            string protocolVersion = "HTTP/1.1", int statusCode = 200, string statusDescription = "OK", int statusSubcode = 0)
        {
            StringBuilder builder = new StringBuilder();

            // Create the top level header.
            builder.Append(protocolVersion + " " + statusCode.ToString() + (statusSubcode > 0 ? "." + statusSubcode.ToString() : "") + " " + statusDescription + "\r\n");

            // For each header found.
            foreach (NameValue header in headers)
            {
                // Add each header.
                builder.Append(header.Name + ":" + header.Value + "\r\n");
            }

            // Add the ending CRLF.
            builder.Append("\r\n");

            // Return the string
            return builder.ToString();
        }

        /// <summary>
        /// Create the web response headers only (no version included).
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>The string that represents the headers.</returns>
        public static string CreateWebResponseHeadersOnly(List<NameValue> headers)
        {
            StringBuilder builder = new StringBuilder();

            // For each header found.
            foreach (NameValue header in headers)
            {
                // Add each header.
                builder.Append(header.Name + ":" + header.Value + "\r\n");
            }

            // Add the ending CRLF.
            builder.Append("\r\n");

            // Return the string
            return builder.ToString();
        }

        /// <summary>
        /// Assign the user principal with the context.
        /// </summary>
        /// <param name="context">The current web context.</param>
        /// <returns>True if principal was assigned; else false.</returns>
        public static bool AssignUserPrincipal(Nequeo.Net.WebContext context)
        {
            bool ret = false;
            
            // If context exists.
            if (context != null)
            {
                // If web request exists.
                if (context.WebRequest != null)
                {
                    // Set the user principle if credentials
                    // have been passed.
                    if (context.WebRequest.Credentials != null)
                    {
                        // Add the credentials.
                        Nequeo.Security.IdentityMember identity =
                            new Nequeo.Security.IdentityMember(
                                context.WebRequest.Credentials.UserName,
                                context.WebRequest.Credentials.Password,
                                context.WebRequest.Credentials.Domain);

                        Nequeo.Security.AuthenticationType authType = Nequeo.Security.AuthenticationType.None;
                        try
                        {
                            // Attempt to get the authentication type.
                            authType = (Nequeo.Security.AuthenticationType)
                                Enum.Parse(typeof(Nequeo.Security.AuthenticationType), context.WebRequest.AuthorizationType);
                        }
                        catch { }

                        // Set the cuurent authentication schema.
                        identity.AuthenticationSchemes = authType;

                        // Create the principal.
                        Nequeo.Security.PrincipalMember principal = new Nequeo.Security.PrincipalMember(identity, null);

                        // Assign the principal
                        context.User = principal;
                        ret = true;
                    }
                }
            }

            // Return
            return ret;
        }

        /// <summary>
        /// Get the principal member from the user.
        /// </summary>
        /// <param name="context">The current context containing the user principal.</param>
        /// <returns>The user principal member; else null.</returns>
        public static Nequeo.Security.PrincipalMember GetPrincipal(Nequeo.Net.WebContext context)
        {
            Nequeo.Security.PrincipalMember ret = null;

            if (context != null)
            {
                // Set the principal.
                ret = (Nequeo.Security.PrincipalMember)context.User;
            }

            // Return the principal.
            return ret;
        }

        /// <summary>
        /// Get the identity member from the principal.
        /// </summary>
        /// <param name="principal">The current principal member.</param>
        /// <returns>The user identity member; else null.</returns>
        public static Nequeo.Security.IdentityMember GetIdentity(Nequeo.Security.PrincipalMember principal)
        {
            Nequeo.Security.IdentityMember ret = null;

            if (principal != null)
            {
                // Set the identity.
                ret = (Nequeo.Security.IdentityMember)principal.Identity;
            }

            // Return the identity.
            return ret;
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">Get the request or response with the supplied data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeaders(byte[] input, out string resource, long timeout = -1, int maxReadLength = 0)
        {
            // Header has not been found at this point.
            string resourceData = "";
            byte[] rawData = null;
            resource = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            List<NameValue> headers = Nequeo.Net.Utility.
                ParseHeaders(input, out resourceData, ref rawData, timeout, maxReadLength);

            // If headers exists.
            if (headers != null)
            {
                // Assign the request or response resource.
                resource = resourceData;
            }

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">Get the request or response with the supplied data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeaders(System.IO.Stream input, out string resource, long timeout = -1, int maxReadLength = 0)
        {
            // Header has not been found at this point.
            string resourceData = "";
            byte[] rawData = null;
            resource = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            List<NameValue> headers = Nequeo.Net.Utility.
                ParseHeaders(input, out resourceData, ref rawData, timeout, maxReadLength);

            // If headers exists.
            if (headers != null)
            {
                // Assign the request or response resource.
                resource = resourceData;
            }

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">The request or response header resource.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeaders(byte[] input, out string resource, ref byte[] rawData, long timeout = -1, int maxReadLength = 0)
        {
            System.IO.MemoryStream buffer = null;
            List<NameValue> headers = null;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = ParseHeaders(buffer, out resource, ref rawData, timeout, maxReadLength);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">The request or response header resource.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeaders(System.IO.Stream input, out string resource, ref byte[] rawData, long timeout = -1, int maxReadLength = 0)
        {
            int bytesRead = 0;
            bool foundEndOfHeaders = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;
            List<NameValue> headers = null;
            resource = "";

            // Start a new timeout clock.
            Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

            // If raw data exists.
            if (rawData != null)
            {
                // Add the current raw data to the collection.
                byte[] temp = store.CombineParallel(rawData);
                store = temp;
                position = store.Length;
            }

            // While the end of the header data
            // has not been found.
            while (!foundEndOfHeaders)
            {
                // Read a single byte from the stream
                // add the data to the store and re-assign.
                bytesRead = input.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Each time data is read reset the timeout.
                    timeoutClock.Reset();
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfHeaders = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else
                    SpinWaitHandler(input, timeoutClock);

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfHeaders)
            {
                // Get the header store minus the ending bytes,
                // split the headers into a collection.
                string headersStore = System.Text.Encoding.Default.GetString(store, 0, store.Length - 4);
                string[] headerCol = headersStore.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                resource = headerCol[0];

                // Create a header name value collection.
                headers = new List<NameValue>();

                // For each header
                for (int i = 1; i < headerCol.Length; i++)
                {
                    // Split the name and value header pair.
                    string[] nameValue = headerCol[i].Split(new string[] { ":" }, StringSplitOptions.None);

                    // Get the values
                    string values = "";
                    for (int j = 1; j < nameValue.Length; j++)
                        values += nameValue[j] + ":";

                    // Add the header.
                    NameValue nameValuePair = new NameValue()
                    {
                         Name = nameValue[0].Trim(),
                         Value = values.TrimEnd(new char[] { ':' }).Trim()
                    };
                    headers.Add(nameValuePair);
                }

                // Headers have been found no need to return raw data.
                rawData = null;
            }
            else
                // Return the raw data if not found.
                rawData = store;

            // Return the header collection.
            return headers;
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeadersOnly(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            // Header has not been found at this point.
            byte[] rawData = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            List<NameValue> headers = Nequeo.Net.Utility.
                ParseHeadersOnly(input, ref rawData, timeout, maxReadLength);

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeadersOnly(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            // Header has not been found at this point.
            byte[] rawData = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            List<NameValue> headers = Nequeo.Net.Utility.
                ParseHeadersOnly(input, ref rawData, timeout, maxReadLength);

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeadersOnly(byte[] input, ref byte[] rawData, long timeout = -1, int maxReadLength = 0)
        {
            System.IO.MemoryStream buffer = null;
            List<NameValue> headers = null;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = ParseHeadersOnly(buffer, ref rawData, timeout, maxReadLength);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        public static List<NameValue> ParseHeadersOnly(System.IO.Stream input, ref byte[] rawData, long timeout = -1, int maxReadLength = 0)
        {
            int bytesRead = 0;
            bool foundEndOfHeaders = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;
            List<NameValue> headers = null;

            // Start a new timeout clock.
            Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

            // If raw data exists.
            if (rawData != null)
            {
                // Add the current raw data to the collection.
                byte[] temp = store.CombineParallel(rawData);
                store = temp;
                position = store.Length;
            }

            // While the end of the header data
            // has not been found.
            while (!foundEndOfHeaders)
            {
                // Read a single byte from the stream
                // add the data to the store and re-assign.
                bytesRead = input.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Each time data is read reset the timeout.
                    timeoutClock.Reset();
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfHeaders = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else
                    SpinWaitHandler(input, timeoutClock);

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfHeaders)
            {
                // Get the header store minus the ending bytes,
                // split the headers into a collection.
                string headersStore = System.Text.Encoding.Default.GetString(store, 0, store.Length - 4);
                string[] headerCol = headersStore.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                // Create a header name value collection.
                headers = new List<NameValue>();

                // For each header
                for (int i = 0; i < headerCol.Length; i++)
                {
                    // Split the name and value header pair.
                    string[] nameValue = headerCol[i].Split(new string[] { ":" }, StringSplitOptions.None);

                    // Get the values
                    string values = "";
                    for (int j = 1; j < nameValue.Length; j++)
                        values += nameValue[j] + ":";

                    // Add the header.
                    NameValue nameValuePair = new NameValue()
                    {
                        Name = nameValue[0].Trim(),
                        Value = values.TrimEnd(new char[] { ':' }).Trim()
                    };
                    headers.Add(nameValuePair);
                }

                // Headers have been found no need to return raw data.
                rawData = null;
            }
            else
                // Return the raw data if not found.
                rawData = store;

            // Return the header collection.
            return headers;
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        public static byte[] ParseCRLF(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            System.IO.MemoryStream buffer = null;
            byte[] headers = null;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = ParseCRLF(buffer, timeout, maxReadLength);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        public static byte[] ParseCRLF(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            int bytesRead = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

            // Start a new timeout clock.
            Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

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
                    timeoutClock.Reset();
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > 1)
                    {
                        // If the end of the header data has been found
                        // \r\n (13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else
                    SpinWaitHandler(input, timeoutClock);

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfData)
            {
                // Return the data found.
                return store;
            }
            else
                return null;
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        public static byte[] Parse2CRLF(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            System.IO.MemoryStream buffer = null;
            byte[] headers = null;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = Parse2CRLF(buffer, timeout, maxReadLength);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        public static byte[] Parse2CRLF(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            int bytesRead = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

            // Start a new timeout clock.
            Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

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
                    timeoutClock.Reset();
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else
                    SpinWaitHandler(input, timeoutClock);

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfData)
            {
                // Return the data found.
                return store;
            }
            else
                return null;
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseCRLF(byte[] input)
        {
            System.IO.MemoryStream buffer = null;
            bool headers = false;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = IsParseCRLF(buffer);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseCRLF(System.IO.Stream input)
        {
            int bytesRead = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

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
                    if (store.Length > 1)
                    {
                        // If the end of the header data has been found
                        // \r\n (13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else break;
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
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParse2CRLF(byte[] input)
        {
            System.IO.MemoryStream buffer = null;
            bool headers = false;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = IsParse2CRLF(buffer);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParse2CRLF(System.IO.Stream input)
        {
            int bytesRead = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

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
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else break;
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
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="output">The stream to store the current data read from the input stream.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParse2CRLF(System.IO.Stream input, System.IO.Stream output = null, int maxReadLength = 0)
        {
            int bytesRead = 0;
            int readOutputBytes = 0;
            bool foundEndOfData = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

            // Copy the data to the stream.
            if (output != null)
            {
                // If data exists.
                if (output.Length > 0)
                {
                    // Load the current output data into the store.
                    store = new byte[(int)output.Length];
                    readOutputBytes = output.Read(store, 0, store.Length);
                    position = readOutputBytes;
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
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfData = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
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
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeadersOnly(byte[] input)
        {
            // Header has not been found at this point.
            byte[] rawData = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            bool headers = Nequeo.Net.Utility.
                IsParseHeadersOnly(input, ref rawData);

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeadersOnly(System.IO.Stream input)
        {
            // Header has not been found at this point.
            byte[] rawData = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            bool headers = Nequeo.Net.Utility.
                IsParseHeadersOnly(input, ref rawData);

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeadersOnly(byte[] input, ref byte[] rawData)
        {
            System.IO.MemoryStream buffer = null;
            bool headers = false;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = IsParseHeadersOnly(buffer, ref rawData);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeadersOnly(System.IO.Stream input, ref byte[] rawData)
        {
            int bytesRead = 0;
            bool foundEndOfHeaders = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

            // If raw data exists.
            if (rawData != null)
            {
                // Add the current raw data to the collection.
                byte[] temp = store.CombineParallel(rawData);
                store = temp;
                position = store.Length;
            }

            // While the end of the header data
            // has not been found.
            while (!foundEndOfHeaders)
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
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfHeaders = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else break;
            }

            // If the end of headers has been found.
            if (foundEndOfHeaders)
            {
                // Return the data found.
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeaders(byte[] input)
        {
            // Header has not been found at this point.
            byte[] rawData = null;
            
            // We need to wait until we get all the header
            // data then send the context to the server.
            bool headers = Nequeo.Net.Utility.
                IsParseHeaders(input, ref rawData);

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeaders(System.IO.Stream input)
        {
            // Header has not been found at this point.
            byte[] rawData = null;

            // We need to wait until we get all the header
            // data then send the context to the server.
            bool headers = Nequeo.Net.Utility.
                IsParseHeaders(input, ref rawData);

            // Return the headers.
            return headers;
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeaders(byte[] input, ref byte[] rawData)
        {
            System.IO.MemoryStream buffer = null;
            bool headers = false;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(input))
                {
                    // Get the headers
                    headers = IsParseHeaders(buffer, ref rawData);
                }

                // Return the headers.
                return headers;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();
            }
        }

        /// <summary>
        /// Has all the data been found that ends with (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="rawData">The current request header raw data if end of headers not found.</param>
        /// <returns>True if the end has been found; else false.</returns>
        public static bool IsParseHeaders(System.IO.Stream input, ref byte[] rawData)
        {
            int bytesRead = 0;
            bool foundEndOfHeaders = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;
           
            // If raw data exists.
            if (rawData != null)
            {
                // Add the current raw data to the collection.
                byte[] temp = store.CombineParallel(rawData);
                store = temp;
                position = store.Length;
            }

            // While the end of the header data
            // has not been found.
            while (!foundEndOfHeaders)
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
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfHeaders = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else break;
            }

            // If the end of headers has been found.
            if (foundEndOfHeaders)
            {
                // Return the data found.
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Read chunked data from the stream.
        /// </summary>
        /// <param name="input">The stream containing the chunked data.</param>
        /// <param name="destination">The stream to write the chunked data to.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the chunked data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        public static void ReadChunkedData(System.IO.Stream input, System.IO.Stream destination, long timeout = -1, int maxReadLength = 0)
        {
            // Get the first chunked data size.
            byte[] chunckLengthData = ParseCRLF(input, timeout, maxReadLength);
            string chunkedLengthHex = Encoding.Default.GetString(chunckLengthData).Trim();
            long chunckLength = Nequeo.Conversion.Context.HexStringToLong(chunkedLengthHex);

            // Write the chunked data to the stream.
            bool ret = Nequeo.IO.Stream.Operation.CopyStream(input, destination, chunckLength, timeout);

            byte[] buffer = null;

            while (chunckLength > 0 && ret)
            {
                // Read CRLF data at the end of the chunk segment.
                buffer = ParseCRLF(input, timeout, 2);

                // Get the chunked data size.
                chunckLengthData = ParseCRLF(input, timeout, maxReadLength);
                chunkedLengthHex = Encoding.Default.GetString(chunckLengthData).Trim();
                chunckLength = Nequeo.Conversion.Context.HexStringToLong(chunkedLengthHex);

                // Write the chunked data to the stream.
                if (chunckLength > 0)
                {
                    // Write the chunked data to the stream.
                    ret = Nequeo.IO.Stream.Operation.CopyStream(input, destination, chunckLength, timeout);
                }
                else
                {
                    // Remove the last \r\n.
                    buffer = ParseCRLF(input, timeout, 2);
                }
            }
        }

        /// <summary>
        /// Write chunked data to the stream
        /// </summary>
        /// <param name="source">The stream containing the data to chunk.</param>
        /// <param name="output">The stream to write chunked data to.</param>
        /// <param name="maxChunkSize">The maximum chunk size.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the chunked data; -1 wait indefinitely.</param>
        public static void WriteChunkedData(System.IO.Stream source, System.IO.Stream output, int maxChunkSize = 8192, long timeout = -1)
        {
            // Write first chunk
            long chunckLength = (long)maxChunkSize;
            string chunkedLengthHex = Nequeo.Conversion.Context.LongToHexString(chunckLength);

            // If less data exists in the source then maximun chunk size.
            if ((source.Length - source.Position) < chunckLength)
            {
                // Get the new chunk size.
                chunckLength = (source.Length - source.Position);
                chunkedLengthHex = Nequeo.Conversion.Context.LongToHexString(chunckLength);
            }

            // Write the chunk size and the CRLF.
            byte[] crlf = new byte[] { 13, 10 };
            byte[] chunkedLengthHexByte = Nequeo.Conversion.Context.HexStringToByteArray(chunkedLengthHex);
            byte[] buffer = chunkedLengthHexByte.Combine(crlf);

            // Write the size of the chunk first.
            output.Write(buffer, 0, buffer.Length);

            // Write the chunked data to the stream.
            Nequeo.IO.Stream.Operation.CopyStream(source, output, chunckLength, timeout);

            // Write the ending CRLF after the chunk.
            output.Write(crlf, 0, crlf.Length);

            // If more data exists.
            if ((source.Length - source.Position) > 0)
            {
                // Write the next chunk of data.
                WriteChunkedData(source, output, maxChunkSize, timeout);
            }
            else
            {
                // End the chunk indication.
                chunckLength = 0;
                chunkedLengthHex = Nequeo.Conversion.Context.LongToHexString(chunckLength);

                chunkedLengthHexByte = Nequeo.Conversion.Context.HexStringToByteArray(chunkedLengthHex);
                buffer = chunkedLengthHexByte.Combine(crlf);

                // Write the ending chunk 0 hex and CRLF.
                output.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Spin wait until data is avaiable or timed out.
        /// </summary>
        /// <param name="source">The source stream to check.</param>
        /// <param name="timeoutClock">The time to check.</param>
        private static void SpinWaitHandler(System.IO.Stream source, Custom.TimeoutClock timeoutClock)
        {
            bool exitIndicator = false;

            // Create the tasks.
            Task[] tasks = new Task[1];

            // Poller task.
            Task poller = Task.Factory.StartNew(() =>
            {
                // Create a new spin wait.
                SpinWait sw = new SpinWait();

                // Action to perform.
                while (!exitIndicator)
                {
                    // The NextSpinWillYield property returns true if 
                    // calling sw.SpinOnce() will result in yielding the 
                    // processor instead of simply spinning. 
                    if (sw.NextSpinWillYield)
                    {
                        if (timeoutClock.IsComplete() || source.Length > 0) exitIndicator = true;
                    }
                    sw.SpinOnce();
                }
            });

            // Assign the listener task.
            tasks[0] = poller;

            // Wait for all tasks to complete.
            Task.WaitAll(tasks);

            // For each task.
            foreach (Task item in tasks)
            {
                try
                {
                    // Release the resources.
                    item.Dispose();
                }
                catch { }
            }
            tasks = null;
        }
    }
}
