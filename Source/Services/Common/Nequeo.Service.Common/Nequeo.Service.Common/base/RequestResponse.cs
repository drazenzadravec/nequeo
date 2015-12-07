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
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Text;

using Nequeo.Xml;
using Nequeo.Model;
using Nequeo.Model.Message;
using Nequeo.Serialisation;
using Nequeo.Serialisation.Extension;

namespace Nequeo.Service
{
    /// <summary>
    /// Request response communication provider.
    /// </summary>
    /// <remarks>
    /// Processing types: query string key: dataformat
    /// query:  The data is within the body and is in the standard query form.
    /// json:   The data is within the body and is in the standard JSON form.
    /// xml:    The data is within the body and is in the standard XML form.
    /// </remarks>
    public class RequestResponse
    {
        /// <summary>
        /// The data format query string key.
        /// </summary>
        public static string DataFormatQueryStringKey = "dataformat";

        /// <summary>
        /// Create the default web response.
        /// </summary>
        /// <returns>The default response.</returns>
        public static WebResponse CreateDefaultWebResponse()
        {
            // Create a new web response.
            WebResponse response = new WebResponse();
            response.Name = "";
            response.ReturnCode = 0;
            response.ErrorMessage = "";
            response.Body = "";
            return response;
        }

        /// <summary>
        /// Create the default response.
        /// </summary>
        /// <param name="response">The response data.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="data">The default data.</param>
        /// <param name="exception">Should the exception response be processed.</param>
        /// <returns>The response.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string CreateDefaultResponse(WebResponse response, NameValueCollection queryString, string data = "", Exception exception = null)
        {
            // Create the default response.
            DefaultModel defaultModel = new DefaultModel() { Data = data };

            // Assign the body.
            response.Body = ResponseBody<DefaultModel>(defaultModel, queryString);

            // If no exception.
            if (exception == null)
            {
                // Return the default.
                return Response<DefaultModel>(response, defaultModel, queryString);
            }
            else
            {
                // Assign the error.
                response.Name = "error";
                response.ReturnCode = -1;
                response.ErrorMessage = "Data format error. " + exception.Message;

                // Return the default.
                return Response<DefaultModel>(response, defaultModel, queryString);
            }
        }

        /// <summary>
        /// Create authenticate response.
        /// </summary>
        /// <param name="response">The response data.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="isAuthenticated">Is the client authenticated.</param>
        /// <param name="token">The communication token.</param>
        /// <returns>The response.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string CreateAuthenticateResponse(WebResponse response, NameValueCollection queryString, bool isAuthenticated, string token)
        {
            // Assign the name.
            response.Name = "authenticate";

            // Create the response type
            Authenticate authenticate = new Authenticate() { Authenticated = isAuthenticated, Token = token };
            response.Body = ResponseBody<Authenticate>(authenticate, queryString);

            // Return the response.
            return Response<Authenticate>(response, authenticate, queryString);
        }

        /// <summary>
        /// Parse the data according to the format.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="queryBody">The query body.</param>
        /// <param name="bodyString">The body string.</param>
        /// <returns>The web request.</returns>
        /// <exception cref="System.Exception"></exception>
        public static WebRequest Request(NameValueCollection queryString, NameValueCollection queryBody, string bodyString)
        {
            WebRequest request = null;

            // If the data format key query exists.
            if (queryString[DataFormatQueryStringKey] != null)
            {
                // Select the data format.
                switch (queryString[DataFormatQueryStringKey].ToLower())
                {
                    case "query":
                        // Query data format.
                        string requestName = (queryBody["name"] != null ? queryBody["name"] : "");
                        request = new WebRequest() { Name = requestName, Body = bodyString };
                        break;

                    case "xml":
                        // Deserialise the data.
                        XDocument xml = Document.LoadDocumentParseXmlString(bodyString);
                        string name = xml.Element("WebRequest").Element("Name").Value;
                        request = new WebRequest() { Name = name };
                        break;

                    case "json":
                        // Json data format.
                        request = JavaObjectNotation.JavaScriptDeserializer<WebRequest>(bodyString);
                        break;

                    default:
                        // Create an empty request.
                        request = new WebRequest();
                        break;
                }
            }
            else
            {
                // Create an empty request.
                request = new WebRequest();
            }

            // Return the request.
            return request;
        }

        /// <summary>
        /// Create the response.
        /// </summary>
        /// <typeparam name="T">The body type.</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="data">The body type data.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>The string response.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string Response<T>(WebResponse response, T data, NameValueCollection queryString)
        {
            string responseData = "";

            // If the data format key query exists.
            if (queryString[DataFormatQueryStringKey] != null)
            {
                // Select the data format.
                switch (queryString[DataFormatQueryStringKey].ToLower())
                {
                    case "xml":
                        // Serialise the data.
                        WebResponse<T> webResponse = new WebResponse<T>();
                        webResponse.ExplicitAssignment(response, data);

                        // Serialise the response data.
                        responseData = ResponseSerialiseAction<WebResponse<T>>(webResponse, queryString);
                        break;

                    case "json":
                        // Json data format.
                        responseData = JavaObjectNotation.JavaScriptSerializer(response);
                        break;

                    case "query":
                    default:
                        // Query data format.
                        NameValueCollection col = Nequeo.Serialisation.NameValue.Serializer<WebResponse>(response);
                        responseData = Nequeo.Web.WebManager.CreateQueryString(col).Replace("body=", "");
                        break;
                }
            }
            else
            {
                // Query data format.
                NameValueCollection col = Nequeo.Serialisation.NameValue.Serializer<WebResponse>(response);
                responseData = Nequeo.Web.WebManager.CreateQueryString(col).Replace("body=", "");
            }

            // Return the body data.
            return responseData;
        }

        /// <summary>
        /// Get the request body.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="body">The body containg the data.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="queryBody">The query body.</param>
        /// <param name="bodyOriginal">The original body.</param>
        /// <returns>The new type.</returns>
        /// <exception cref="System.Exception"></exception>
        public static T RequestBody<T>(string body, NameValueCollection queryString, NameValueCollection queryBody, byte[] bodyOriginal)
        {
            T requestBody = default(T);

            // If the data format key query exists.
            if (queryString[DataFormatQueryStringKey] != null)
            {
                // Select the data format.
                switch (queryString[DataFormatQueryStringKey].ToLower())
                {
                    case "query":
                        // Query data format.
                        requestBody = Nequeo.Serialisation.NameValue.Deserializer<T>(queryBody);
                        break;

                    case "xml":
                        // Xml data format.
                        requestBody = RequestDeserialiseAction<T>(queryString, bodyOriginal).Body;
                        break;

                    case "json":
                        // Json data format.
                        requestBody = JavaObjectNotation.JavaScriptDeserializer<T>(body);
                        break;

                    default:
                        break;
                }
            }

            // Return the body data.
            return requestBody;
        }

        /// <summary>
        /// Get the response body.
        /// </summary>
        /// <typeparam name="T">The body type.</typeparam>
        /// <param name="data">The body data.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>The body</returns>
        /// <exception cref="System.Exception"></exception>
        public static string ResponseBody<T>(T data, NameValueCollection queryString)
        {
            string responseBody = "";

            // If the data format key query exists.
            if (queryString[DataFormatQueryStringKey] != null)
            {
                // Select the data format.
                switch (queryString[DataFormatQueryStringKey].ToLower())
                {
                    case "xml":
                        // Serialise the data.
                        responseBody = ResponseSerialiseAction<T>(data, queryString);
                        break;

                    case "json":
                        // Json data format.
                        responseBody = JavaObjectNotation.JavaScriptSerializer(data);
                        break;

                    case "query":
                    default:
                        // Query data format.
                        NameValueCollection col = Nequeo.Serialisation.NameValue.Serializer<T>(data);
                        responseBody = Nequeo.Web.WebManager.CreateQueryString(col);
                        break;
                }
            }
            else
            {
                // Query data format.
                NameValueCollection col = Nequeo.Serialisation.NameValue.Serializer<T>(data);
                responseBody = Nequeo.Web.WebManager.CreateQueryString(col);
            }

            // Return the body data.
            return responseBody;
        }

        /// <summary>
        /// Request deserialise action.
        /// </summary>
        /// <typeparam name="T">The body type.</typeparam>
        /// <param name="queryString">The query string.</param>
        /// <param name="body">The original body.</param>
        /// <returns>The web request.</returns>
        /// <exception cref="System.Exception"></exception>
        public static WebRequest<T> RequestDeserialiseAction<T>(NameValueCollection queryString, byte[] body)
        {
            WebRequest<T> request = null;

            // If the data format key query exists.
            if (queryString[DataFormatQueryStringKey] != null)
            {
                // Select the data format.
                switch (queryString[DataFormatQueryStringKey].ToLower())
                {
                    case "xml":
                        // Get the xml data.
                        request = body.DeserialiseXml<WebRequest<T>>();
                        break;

                    default:
                        break;
                }
            }

            // Return request.
            return request;
        }

        /// <summary>
        /// Response serialise action.
        /// </summary>
        /// <typeparam name="T">The body type.</typeparam>
        /// <param name="data">The body data.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>The data.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string ResponseSerialiseAction<T>(T data, NameValueCollection queryString)
        {
            string webResponse = "";

            // If the data format key query exists.
            if (queryString[DataFormatQueryStringKey] != null)
            {
                // Select the data format.
                switch (queryString[DataFormatQueryStringKey].ToLower())
                {
                    case "xml":
                        // Get the xml data.
                        byte[] xml = data.SerialiseXml<T>();
                        webResponse = Encoding.Default.GetString(xml);
                        break;

                    default:
                        break;
                }
            }

            // Return response.
            return webResponse;
        }
    }
}
