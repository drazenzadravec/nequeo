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
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace Nequeo.Net.OAuth
{
    /// <summary>
    /// OAuth execption provider.
    /// </summary>
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", 
        Justification = "All instance members are exposed through properties")]
    public class OAuthErrorException : Exception
    {
        /// <summary>
        /// Create a new OAuthErrorException if an error response was received anytime during consent flow.
        /// See OAuth 2.0 section 4.1.1.
        /// </summary>
        /// <param name="error">The error parameter from the response.</param>
        /// <param name="errorUri">The error_uri parameter from the response.</param>
        /// <param name="errorDescription">The error_description parameter from the response.</param>
        /// <param name="state">The current state.</param>
        public OAuthErrorException(string error, string errorUri, string errorDescription, string state) :
            base(errorDescription)
        {
            Error = error;
            ErrorUri = errorUri;
            State = state;
        }

        /// <summary>
        /// Create a new OAuthErrorException if an error response was received anytime during access token request or refresh.
        /// See OAuth 2.0 section 4.1.2.1, 4.2.2.1, and 5.2
        /// </summary>
        /// <param name="error">The error parameter from the response.</param>
        /// <param name="errorUri">The error_uri parameter from the response.</param>
        /// <param name="errorDescription">The error_description parameter from the response.</param>
        /// <param name="state">The current state.</param>
        /// <param name="innerEx">The inner web exception.</param>
        public OAuthErrorException(string error, string errorUri, string errorDescription, string state, WebException innerEx) :
            base(errorDescription, innerEx)
        {
            Error = error;
            ErrorUri = errorUri;
            State = state;
        }

        private class OAuthErrorJson
        {
            public string error { get; set; }
            public string error_uri { get; set; }
            public string error_description { get; set; }
            public string state { get; set; }
        }

        /// <summary>
        /// Attempt to construct an OAuthErrorException from a WebResponse
        /// </summary>
        /// <param name="ex">WebException which contains the response stream to read OAuth error codes from.</param>
        /// <returns>New OAuthErrorException</returns>
        public static OAuthErrorException FromWebException(WebException ex)
        {
            if (ex == null)
            {
                return null;
            }

            OAuthErrorException result = null;
            try
            {
                using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    var errorJson = ser.Deserialize<OAuthErrorJson>(sr.ReadToEnd());

                    if (errorJson != null)
                    {
                        result = new OAuthErrorException(errorJson.error, errorJson.error_uri, errorJson.error_description,
                            errorJson.state, ex);
                    }
                }
            }
            // Ignore any JSON deserialization exceptions, as we'll return null if it's not an OAuth error response
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }

            return result;
        }

        /// <summary>
        /// OAuth Error type
        /// </summary>
        public string Error { get; protected set; }

        /// <summary>
        /// A URI identifying a human-readable web page with information about the error
        /// </summary>
        public string ErrorUri { get; protected set; }

        /// <summary>
        /// User-defined state object, if any
        /// </summary>
        public string State { get; protected set; }

        /// <summary>
        /// Human-readable error description
        /// </summary>
        public string ErrorDescription
        {
            get { return this.Message; }
        }
    }
}
