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
using System.Threading;
using System.Diagnostics;

namespace Nequeo.Net.Http.Common
{
    /// <summary>
    /// Http status code model.
    /// </summary>
    public class HttpStatusCode
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public int Code { get; set;}

        /// <summary>
        /// Gets or sets the sub code.
        /// </summary>
        public int SubCode { get; set; }

        /// <summary>
        /// Gets or sets the code description.
        /// </summary>
        public string Description { get; set;}

        /// <summary>
        /// Gets or sets the code details.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the code type.
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// Gets or sets the complete html.
        /// </summary>
        public string Html { get; set;}

        /// <summary>
        /// Gets or sets the html head.
        /// </summary>
        public string HtmlHead { get; set;}

        /// <summary>
        /// Gets or sets the html body.
        /// </summary>
        public string HtmlBody { get; set;}
    }

    /// <summary>
    /// Http ststus codes.
    /// </summary>
    public partial class HttpCodes
    {
        /// <summary>
        /// Get the page html header.
        /// </summary>
        /// <param name="title">The page title.</param>
        /// <returns>The html head section.</returns>
        private static string HtmlHead(string title)
        {
            return
                "<head>" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"/>" +
                "<title>" + title + "</title>" +
                "<style type=\"text/css\">" +
                "<!--" +
                "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                "fieldset{padding:0 15px 10px 15px;} " +
                "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                "h2{font-size:1.7em;margin:0;color:#CC0000;} " +
                "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;} " +
                "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                "background-color:#555555;}" +
                "#content{margin:0 0 0 2%;position:relative;}" +
                ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                "-->" +
                "</style>" +
                "</head>";
        }

        /// <summary>
        /// Get the page body.
        /// </summary>
        /// <param name="title">The page title.</param>
        /// <param name="heading">The page heading.</param>
        /// <param name="text">The page text.</param>
        /// <returns>The html body section.</returns>
        private static string HtmlBody(string title, string heading, string text)
        {
            return
                "<body>" +
                "<div id=\"header\"><h1>" + heading + "</h1></div>" +
                "<div id=\"content\">" +
                    "<div class=\"content-container\"><fieldset>" +
                    "<h2>" + title + "</h2>" +
                    "<h3>" + text + "</h3>" +
                    "</fieldset></div>" +
                "</div>" +
                "</body>";
        }

        /// <summary>
        /// Get the list of status codes.
        /// </summary>
        /// <returns>The list of status codes.</returns>
        public static HttpStatusCode[] GetStatusCodes()
        {
            HttpStatusCode[] statusCodes = new HttpStatusCode[]
            {
                Code_100_0(),
                Code_101_0(),
                Code_102_0(),
                Code_200_0(),
                Code_201_0(),
                Code_202_0(),
                Code_203_0(),
                Code_204_0(),
                Code_205_0(),
                Code_206_0(),
                Code_207_0(),
                Code_208_0(),
                Code_300_0(),
                Code_301_0(),
                Code_302_0(),
                Code_303_0(),
                Code_304_0(),
                Code_305_0(),
                Code_306_0(),
                Code_307_0(),
                Code_308_0(),
                Code_400_0(),
                Code_401_0(),
                Code_401_1(),
                Code_401_2(),
                Code_401_3(),
                Code_401_4(),
                Code_401_5(),
                Code_402_0(),
                Code_403_0(),
                Code_403_1(),
                Code_403_2(),
                Code_403_3(),
                Code_403_4(),
                Code_403_5(),
                Code_403_6(),
                Code_403_7(),
                Code_403_8(),
                Code_403_9(),
                Code_403_10(),
                Code_403_11(),
                Code_403_12(),
                Code_403_13(),
                Code_403_14(),
                Code_403_15(),
                Code_403_16(),
                Code_403_17(),
                Code_403_18(),
                Code_403_19(),
                Code_404_0(),
                Code_404_1(),
                Code_404_2(),
                Code_404_3(),
                Code_404_4(),
                Code_404_5(),
                Code_404_6(),
                Code_404_7(),
                Code_404_8(),
                Code_404_9(),
                Code_404_10(),
                Code_404_11(),
                Code_404_12(),
                Code_404_13(),
                Code_404_14(),
                Code_404_15(),
                Code_405_0(),
                Code_406_0(),
                Code_407_0(),
                Code_408_0(),
                Code_409_0(),
                Code_410_0(),
                Code_411_0(),
                Code_412_0(),
                Code_413_0(),
                Code_414_0(),
                Code_415_0(),
                Code_416_0(),
                Code_417_0(),
                Code_422_0(),
                Code_423_0(),
                Code_424_0(),
                Code_426_0(),
                Code_428_0(),
                Code_429_0(),
                Code_431_0(),
                Code_440_0(),
                Code_500_0(),
                Code_500_13(),
                Code_500_14(),
                Code_500_15(),
                Code_500_16(),
                Code_500_17(),
                Code_500_18(),
                Code_500_19(),
                Code_501_0(),
                Code_502_0(),
                Code_503_0(),
                Code_504_0(),
                Code_505_0(),
                Code_506_0(),
                Code_507_0(),
                Code_508_0(),
                Code_510_0(),
                Code_511_0(),
                Code_522_0(),
            };
            return statusCodes;
        }

        /// <summary>
        /// 100 - Continue.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_100_0()
        {
            string codeType = "Informational";
            string details = "This means that the server has received the request headers, and that the client should proceed to send the request body.";
            string htmlHead = HtmlHead("100 - Continue.");
            string htmlBody = HtmlBody("100 - Continue.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 100,
                SubCode = 0,
                Description = "Continue",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 101 - Switching Protocols.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_101_0()
        {
            string codeType = "Informational";
            string details = "This means the requester has asked the server to switch protocols and the server is acknowledging that it will do so.";
            string htmlHead = HtmlHead("101 - Switching Protocols.");
            string htmlBody = HtmlBody("101 - Switching Protocols.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 101,
                SubCode = 0,
                Description = "Switching Protocols",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 102 - Processing.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_102_0()
        {
            string codeType = "Informational";
            string details = "As a WebDAV request may contain many sub-requests involving file operations, it may take a long time to complete the request. This code indicates that the server has received and is processing the request, but no response is available yet.[3] This prevents the client from timing out and assuming the request was lost.";
            string htmlHead = HtmlHead("102 - Processing.");
            string htmlBody = HtmlBody("102 - Processing.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 102,
                SubCode = 0,
                Description = "Processing",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 200 - OK.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_200_0()
        {
            string codeType = "Success";
            string details = "Standard response for successful HTTP requests.";
            string htmlHead = HtmlHead("200 - OK.");
            string htmlBody = HtmlBody("200 - OK.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 200,
                SubCode = 0,
                Description = "OK",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 201 - Created.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_201_0()
        {
            string codeType = "Success";
            string details = "The request has been fulfilled and resulted in a new resource being created.";
            string htmlHead = HtmlHead("201 - Created.");
            string htmlBody = HtmlBody("201 - Created.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 201,
                SubCode = 0,
                Description = "Created",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 202 - Accepted.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_202_0()
        {
            string codeType = "Success";
            string details = "The request has been accepted for processing, but the processing has not been completed. The request might or might not eventually be acted upon, as it might be disallowed when processing actually takes place.";
            string htmlHead = HtmlHead("202 - Accepted.");
            string htmlBody = HtmlBody("202 - Accepted.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 202,
                SubCode = 0,
                Description = "Accepted",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 203 - Non-Authoritative Information.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_203_0()
        {
            string codeType = "Success";
            string details = "The server successfully processed the request, but is returning information that may be from another source.";
            string htmlHead = HtmlHead("203 - Non-Authoritative Information.");
            string htmlBody = HtmlBody("203 - Non-Authoritative Information.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 203,
                SubCode = 0,
                Description = "Non-Authoritative Information",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 204 - No Content.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_204_0()
        {
            string codeType = "Success";
            string details = "The server successfully processed the request, but is not returning any content.[2] Usually used as a response to a successful delete request.";
            string htmlHead = HtmlHead("204 - No Content.");
            string htmlBody = HtmlBody("204 - No Content.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 204,
                SubCode = 0,
                Description = "No Content",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 205 - Reset Content.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_205_0()
        {
            string codeType = "Success";
            string details = "The server successfully processed the request, but is not returning any content. Unlike a 204 response, this response requires that the requester reset the document view.";
            string htmlHead = HtmlHead("205 - Reset Content.");
            string htmlBody = HtmlBody("205 - Reset Content.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 205,
                SubCode = 0,
                Description = "Reset Content",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 206 - Partial Content.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_206_0()
        {
            string codeType = "Success";
            string details = "The server is delivering only part of the resource due to a range header sent by the client. The range header is used by tools like wget to enable resuming of interrupted downloads, or split a download into multiple simultaneous streams.";
            string htmlHead = HtmlHead("206 - Partial Content.");
            string htmlBody = HtmlBody("206 - Partial Content.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 206,
                SubCode = 0,
                Description = "Partial Content",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 207 - Multi-Status.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_207_0()
        {
            string codeType = "Success";
            string details = "The message body that follows is an XML message and can contain a number of separate response codes, depending on how many sub-requests were made.";
            string htmlHead = HtmlHead("207 - Multi-Status.");
            string htmlBody = HtmlBody("207 - Multi-Status.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 207,
                SubCode = 0,
                Description = "Multi-Status",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 208 - Already Reported.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_208_0()
        {
            string codeType = "Success";
            string details = "The members of a DAV binding have already been enumerated in a previous reply to this request, and are not being included again.";
            string htmlHead = HtmlHead("208 - Already Reported.");
            string htmlBody = HtmlBody("208 - Already Reported.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 208,
                SubCode = 0,
                Description = "Already Reported",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 300 - Multiple Choices.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_300_0()
        {
            string codeType = "Redirection";
            string details = "Indicates multiple options for the resource that the client may follow. It, for instance, could be used to present different format options for video, list files with different extensions, or word sense disambiguation.";
            string htmlHead = HtmlHead("300 - Multiple Choices.");
            string htmlBody = HtmlBody("300 - Multiple Choices.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 300,
                SubCode = 0,
                Description = "Multiple Choices",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 301 - Moved Permanently.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_301_0()
        {
            string codeType = "Redirection";
            string details = "This and all future requests should be directed to the given URI.";
            string htmlHead = HtmlHead("301 - Moved Permanently.");
            string htmlBody = HtmlBody("301 - Moved Permanently.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 301,
                SubCode = 0,
                Description = "Moved Permanently",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 302 - Found.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_302_0()
        {
            string codeType = "Redirection";
            string details = "Required the client to perform a temporary redirect.";
            string htmlHead = HtmlHead("302 - Found.");
            string htmlBody = HtmlBody("302 - Found.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 302,
                SubCode = 0,
                Description = "Found",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 303 - See Other.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_303_0()
        {
            string codeType = "Redirection";
            string details = "The response to the request can be found under another URI using a GET method. When received in response to a POST (or PUT/DELETE), it should be assumed that the server has received the data and the redirect should be issued with a separate GET message.";
            string htmlHead = HtmlHead("303 - See Other.");
            string htmlBody = HtmlBody("303 - See Other.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 303,
                SubCode = 0,
                Description = "See Other",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 304 - Not Modified.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_304_0()
        {
            string codeType = "Redirection";
            string details = "Indicates that the resource has not been modified since the version specified by the request headers If-Modified-Since or If-Match.[2] This means that there is no need to retransmit the resource, since the client still has a previously-downloaded copy.";
            string htmlHead = HtmlHead("304 - Not Modified.");
            string htmlBody = HtmlBody("304 - Not Modified.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 304,
                SubCode = 0,
                Description = "Not Modified",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 305 - Use Proxy.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_305_0()
        {
            string codeType = "Redirection";
            string details = "The requested resource is only available through a proxy, whose address is provided in the response.[2] Many HTTP clients (such as Mozilla[9] and Internet Explorer) do not correctly handle responses with this status code, primarily for security reasons.";
            string htmlHead = HtmlHead("305 - Use Proxy.");
            string htmlBody = HtmlBody("305 - Use Proxy.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 305,
                SubCode = 0,
                Description = "Use Proxy",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 306 - Switch Proxy.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_306_0()
        {
            string codeType = "Redirection";
            string details = "No longer used.[2] Originally meant Subsequent requests should use the specified proxy.";
            string htmlHead = HtmlHead("306 - Switch Proxy.");
            string htmlBody = HtmlBody("306 - Switch Proxy.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 306,
                SubCode = 0,
                Description = "Switch Proxy",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 307 - Temporary Redirect.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_307_0()
        {
            string codeType = "Redirection";
            string details = "In this case, the request should be repeated with another URI; however, future requests should still use the original URI.[2] In contrast to how 302 was historically implemented, the request method is not allowed to be changed when reissuing the original request. For instance, a POST request should be repeated using another POST request.";
            string htmlHead = HtmlHead("307 - Temporary Redirect.");
            string htmlBody = HtmlBody("307 - Temporary Redirect.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 307,
                SubCode = 0,
                Description = "Temporary Redirect",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 308 - Permanent Redirect.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_308_0()
        {
            string codeType = "Redirection";
            string details = "The request, and all future requests should be repeated using another URI. 307 and 308 (as proposed) parallel the behaviours of 302 and 301, but do not allow the HTTP method to change. So, for example, submitting a form to a permanently redirected resource may continue smoothly.";
            string htmlHead = HtmlHead("308 - Permanent Redirect.");
            string htmlBody = HtmlBody("308 - Permanent Redirect.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 308,
                SubCode = 0,
                Description = "Permanent Redirect",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 400 - Bad Request.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_400_0()
        {
            string codeType = "Client Error";
            string details = "The request cannot be fulfilled due to bad syntax.";
            string htmlHead = HtmlHead("400 - Bad Request.");
            string htmlBody = HtmlBody("400 - Bad Request.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 400,
                SubCode = 0,
                Description = "Bad Request",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 401 - Unauthorized: Access is denied due to invalid credentials.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_401_0()
        {
            string codeType = "Client Error";
            string details = "You do not have permission to view this directory or page using the credentials that you supplied.";
            string htmlHead = HtmlHead("401 - Unauthorized: Access is denied due to invalid credentials.");
            string htmlBody = HtmlBody("401 - Unauthorized: Access is denied due to invalid credentials.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 401,
                SubCode = 0,
                Description = "Unauthorized: Access is denied due to invalid credentials",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 401.1 - Unauthorized: Access is denied due to invalid credentials.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_401_1()
        {
            string codeType = "Client Error";
            string details = "You do not have permission to view this directory or page using the credentials that you supplied.";
            string htmlHead = HtmlHead("401.1 - Unauthorized: Access is denied due to invalid credentials.");
            string htmlBody = HtmlBody("401.1 - Unauthorized: Access is denied due to invalid credentials.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 401,
                SubCode = 1,
                Description = "Unauthorized: Access is denied due to invalid credentials",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 401.2 - Unauthorized: Access is denied due to server configuration.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_401_2()
        {
            string codeType = "Client Error";
            string details = "You do not have permission to view this directory or page using the credentials that you supplied because your Web browser is sending a WWW-Authenticate header field that the Web server is not configured to accept.";
            string htmlHead = HtmlHead("401.2 - Unauthorized: Access is denied due to server configuration.");
            string htmlBody = HtmlBody("401.2 - Unauthorized: Access is denied due to server configuration.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 401,
                SubCode = 2,
                Description = "Unauthorized: Access is denied due to server configuration",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 401.3 - Unauthorized: Access is denied due to an ACL set on the requested resource.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_401_3()
        {
            string codeType = "Client Error";
            string details = "You do not have permission to view this directory or page due to the access control list (ACL) that is configured for this resource on the Web server.";
            string htmlHead = HtmlHead("401.3 - Unauthorized: Access is denied due to an ACL set on the requested resource.");
            string htmlBody = HtmlBody("401.3 - Unauthorized: Access is denied due to an ACL set on the requested resource.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 401,
                SubCode = 3,
                Description = "Unauthorized: Access is denied due to an ACL set on the requested resource",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 401.4 - Unauthorized: Authorization failed by filter installed on the Web server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_401_4()
        {
            string codeType = "Client Error";
            string details = "You might not have permission to view this directory or page using the credentials that you supplied. The Web server has a filter installed to verify users connecting to the server and it failed to authenticate your credentials.";
            string htmlHead = HtmlHead("401.4 - Unauthorized: Authorization failed by filter installed on the Web server.");
            string htmlBody = HtmlBody("401.4 - Unauthorized: Authorization failed by filter installed on the Web server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 401,
                SubCode = 4,
                Description = "Unauthorized: Authorization failed by filter installed on the Web server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 401.5 - Unauthorized: Authorization failed by an ISAPI/CGI application.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_401_5()
        {
            string codeType = "Client Error";
            string details = "The URL you attempted to reach has an ISAPI or CGI application installed that verifies user credentials before proceeding. This application cannot verify your credentials.";
            string htmlHead = HtmlHead("401.5 - Unauthorized: Authorization failed by an ISAPI/CGI application.");
            string htmlBody = HtmlBody("401.5 - Unauthorized: Authorization failed by an ISAPI/CGI application.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 401,
                SubCode = 5,
                Description = "Unauthorized: Authorization failed by an ISAPI/CGI application",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 402 - Payment Required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_402_0()
        {
            string codeType = "Client Error";
            string details = "Reserved for future use.";
            string htmlHead = HtmlHead("402 - Payment Required.");
            string htmlBody = HtmlBody("402 - Payment Required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 402,
                SubCode = 0,
                Description = "Payment Required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403 - Forbidden: Access is denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_0()
        {
            string codeType = "Client Error";
            string details = "You do not have permission to view this directory or page using the credentials that you supplied.";
            string htmlHead = HtmlHead("403 - Forbidden: Access is denied.");
            string htmlBody = HtmlBody("403 - Forbidden: Access is denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 0,
                Description = "Forbidden: Access is denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.1 - Forbidden: Execute access is denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_1()
        {
            string codeType = "Client Error";
            string details = "You have attempted to execute a CGI, ISAPI, or other executable program from a directory that does not allow programs to be executed.";
            string htmlHead = HtmlHead("403.1 - Forbidden: Execute access is denied.");
            string htmlBody = HtmlBody("403.1 - Forbidden: Execute access is denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 1,
                Description = "Forbidden: Execute access is denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.2 - Forbidden: Read access is denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_2()
        {
            string codeType = "Client Error";
            string details = "There is a problem with the page you are looking for and it cannot be displayed. This error can occur if you are trying to display an HTML page that resides in a directory that is configured to allow Execute or Script permissions only.";
            string htmlHead = HtmlHead("403.2 - Forbidden: Read access is denied.");
            string htmlBody = HtmlBody("403.2 - Forbidden: Read access is denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 2,
                Description = "Forbidden: Read access is denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.3 - Forbidden: Write access is denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_3()
        {
            string codeType = "Client Error";
            string details = "There is a problem saving the page to the Web site. This error can occur if you attempt to upload a file or modify a file in a directory that does not allow Write access.";
            string htmlHead = HtmlHead("403.3 - Forbidden: Write access is denied.");
            string htmlBody = HtmlBody("403.3 - Forbidden: Write access is denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 3,
                Description = "Forbidden: Write access is denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.4 - Forbidden: SSL is required to view this resource.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_4()
        {
            string codeType = "Client Error";
            string details = "The page you are trying to access is secured with Secure Sockets Layer (SSL).";
            string htmlHead = HtmlHead("403.4 - Forbidden: SSL is required to view this resource.");
            string htmlBody = HtmlBody("403.4 - Forbidden: SSL is required to view this resource.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 4,
                Description = "Forbidden: SSL is required to view this resource",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.5 - Forbidden: SSL 128 is required to view this resource.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_5()
        {
            string codeType = "Client Error";
            string details = "The resource you are trying to access is secured with a 128-bit version of Secure Sockets Layer (SSL). In order to view this resource, you need a browser that supports this version of SSL.";
            string htmlHead = HtmlHead("403.5 - Forbidden: SSL 128 is required to view this resource.");
            string htmlBody = HtmlBody("403.5 - Forbidden: SSL 128 is required to view this resource.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 5,
                Description = "Forbidden: SSL 128 is required to view this resource",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.6 - Forbidden: IP address of the client has been rejected.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_6()
        {
            string codeType = "Client Error";
            string details = "The Web server you are attempting to reach has a list of IP addresses that are not allowed to access the Web site, and the IP address of your browsing computer is on this list.";
            string htmlHead = HtmlHead("403.6 - Forbidden: IP address of the client has been rejected.");
            string htmlBody = HtmlBody("403.6 - Forbidden: IP address of the client has been rejected.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 6,
                Description = "Forbidden: IP address of the client has been rejected",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.7 - Forbidden: SSL client certificate is required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_7()
        {
            string codeType = "Client Error";
            string details = "The page you are attempting to access requires your browser to have a Secure Sockets Layer (SSL) client certificate that the Web server will recognize. The client certificate is used for identifying you as a valid user of the resource.";
            string htmlHead = HtmlHead("403.7 - Forbidden: SSL client certificate is required.");
            string htmlBody = HtmlBody("403.7 - Forbidden: SSL client certificate is required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 7,
                Description = "Forbidden: SSL client certificate is required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.8 - Forbidden: DNS name of the client is rejected.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_8()
        {
            string codeType = "Client Error";
            string details = "The Web server you are attempting to reach has a list of DNS names that are not allowed to access this Web site, and the DNS name of your browsing computer is on this list.";
            string htmlHead = HtmlHead("403.8 - Forbidden: DNS name of the client is rejected.");
            string htmlBody = HtmlBody("403.8 - Forbidden: DNS name of the client is rejected.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 8,
                Description = "Forbidden: DNS name of the client is rejected",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.9 - Forbidden: Too many clients are trying to connect to the Web server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_9()
        {
            string codeType = "Client Error";
            string details = "The Web server is too busy to process your request at this time.";
            string htmlHead = HtmlHead("403.9 - Forbidden: Too many clients are trying to connect to the Web server.");
            string htmlBody = HtmlBody("403.9 - Forbidden: Too many clients are trying to connect to the Web server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 9,
                Description = "Forbidden: Too many clients are trying to connect to the Web server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.10 - Forbidden: Web server is configured to deny Execute access.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_10()
        {
            string codeType = "Client Error";
            string details = "You have attempted to execute a CGI, ISAPI, or other executable program from a directory that does not allow programs to be executed.";
            string htmlHead = HtmlHead("403.10 - Forbidden: Web server is configured to deny Execute access.");
            string htmlBody = HtmlBody("403.10 - Forbidden: Web server is configured to deny Execute access.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 10,
                Description = "Forbidden: Web server is configured to deny Execute access",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.11 - Forbidden: Password has been changed.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_11()
        {
            string codeType = "Client Error";
            string details = "You do not have permission to view this directory or page using the credentials that you supplied.";
            string htmlHead = HtmlHead("403.11 - Forbidden: Password has been changed.");
            string htmlBody = HtmlBody("403.11 - Forbidden: Password has been changed.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 11,
                Description = "Forbidden: Password has been changed",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.12 - Forbidden: Client certificate is denied access by the server certificate mapper.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_12()
        {
            string codeType = "Client Error";
            string details = "The account to which your client certificate is mapped on the Web server has been denied access to this Web site. A Secure Sockets Layer (SSL) client certificate is used for identifying you as a valid user of the resource.";
            string htmlHead = HtmlHead("403.12 - Forbidden: Client certificate is denied access by the server certificate mapper.");
            string htmlBody = HtmlBody("403.12 - Forbidden: Client certificate is denied access by the server certificate mapper.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 12,
                Description = "Forbidden: Client certificate is denied access by the server certificate mapper",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.13 - Forbidden: Client certificate has been revoked on the Web server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_13()
        {
            string codeType = "Client Error";
            string details = "Your client certificate was revoked, or the revocation server could not be contacted. A Secure Sockets Layer (SSL) client certificate is used for identifying you as a valid user of the resource.";
            string htmlHead = HtmlHead("403.13 - Forbidden: Client certificate has been revoked on the Web server.");
            string htmlBody = HtmlBody("403.13 - Forbidden: Client certificate has been revoked on the Web server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 13,
                Description = "Forbidden: Client certificate has been revoked on the Web server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.14 - Forbidden: Directory listing denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_14()
        {
            string codeType = "Client Error";
            string details = "The Web server is configured not to display a list of the contents of this directory.";
            string htmlHead = HtmlHead("403.14 - Forbidden: Directory listing denied.");
            string htmlBody = HtmlBody("403.14 - Forbidden: Directory listing denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 14,
                Description = "Forbidden: Directory listing denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.15 - Forbidden: Client access licenses have exceeded limits on the Web server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_15()
        {
            string codeType = "Client Error";
            string details = "There are too many people accessing the Web site at this time. The Web server has exceeded its Client Access License limit.";
            string htmlHead = HtmlHead("403.15 - Forbidden: Client access licenses have exceeded limits on the Web server.");
            string htmlBody = HtmlBody("403.15 - Forbidden: Client access licenses have exceeded limits on the Web server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 15,
                Description = "Forbidden: Client access licenses have exceeded limits on the Web server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.16 - Forbidden: Client certificate is ill-formed or is not trusted by the Web server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_16()
        {
            string codeType = "Client Error";
            string details = "Your client certificate is untrusted or invalid. A Secure Sockets Layer (SSL) client certificate is used for identifying you as a valid user of the resource.";
            string htmlHead = HtmlHead("403.16 - Forbidden: Client certificate is ill-formed or is not trusted by the Web server.");
            string htmlBody = HtmlBody("403.16 - Forbidden: Client certificate is ill-formed or is not trusted by the Web server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 16,
                Description = "Forbidden: Client certificate is ill-formed or is not trusted by the Web server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.17 - Forbidden: Client certificate has expired or is not yet valid.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_17()
        {
            string codeType = "Client Error";
            string details = "Your client certificate has expired or is not yet valid. A Secure Sockets Layer (SSL) client certificate is used for identifying you as a valid user of the resource.";
            string htmlHead = HtmlHead("403.17 - Forbidden: Client certificate has expired or is not yet valid.");
            string htmlBody = HtmlBody("403.17 - Forbidden: Client certificate has expired or is not yet valid.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 17,
                Description = "Forbidden: Client certificate has expired or is not yet valid",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.18 - Forbidden: Cannot execute requested URL in the current application pool.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_18()
        {
            string codeType = "Client Error";
            string details = "The specified request cannot be executed in the application pool that is configured for this resource on the Web server.";
            string htmlHead = HtmlHead("403.18 - Forbidden: Cannot execute requested URL in the current application pool.");
            string htmlBody = HtmlBody("403.18 - Forbidden: Cannot execute requested URL in the current application pool.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 18,
                Description = "Forbidden: Cannot execute requested URL in the current application pool",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 403.19 - Forbidden: Cannot execute CGIs for the client in this application pool.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_403_19()
        {
            string codeType = "Client Error";
            string details = "The configured user for this application pool does not have sufficient privileges to execute CGI applications.";
            string htmlHead = HtmlHead("403.19 - Forbidden: Cannot execute CGIs for the client in this application pool.");
            string htmlBody = HtmlBody("403.19 - Forbidden: Cannot execute CGIs for the client in this application pool.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 403,
                SubCode = 19,
                Description = "Forbidden: Cannot execute CGIs for the client in this application pool",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404 - File or directory not found.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_0()
        {
            string codeType = "Client Error";
            string details = "The resource you are looking for might have been removed, had its name changed, or is temporarily unavailable.";
            string htmlHead = HtmlHead("404 - File or directory not found.");
            string htmlBody = HtmlBody("404 - File or directory not found.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 0,
                Description = "File or directory not found",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.1 - File or directory not found: Web site not accessible on the requested port.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_1()
        {
            string codeType = "Client Error";
            string details = "The Web site you are trying to access has an IP address that is configured not to accept requests that specify a port number.";
            string htmlHead = HtmlHead("404.1 - File or directory not found: Web site not accessible on the requested port.");
            string htmlBody = HtmlBody("404.1 - File or directory not found: Web site not accessible on the requested port.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 1,
                Description = "File or directory not found: Web site not accessible on the requested port",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.2 - File or directory not found: Lockdown policy prevents this request.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_2()
        {
            string codeType = "Client Error";
            string details = "The page you are requesting cannot be served due to the Web service extensions that are configured on the Web server.";
            string htmlHead = HtmlHead("404.2 - File or directory not found: Lockdown policy prevents this request.");
            string htmlBody = HtmlBody("404.2 - File or directory not found: Lockdown policy prevents this request.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 2,
                Description = "File or directory not found: Lockdown policy prevents this request",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.3 - File or directory not found: MIME map policy prevents this request.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_3()
        {
            string codeType = "Client Error";
            string details = "The page you are requesting cannot be served due to the Multipurpose Internet Mail Extensions (MIME) map policy that is configured on the Web server. The page you requested has a file name extension that is not recognised, and is therefore not allowed.";
            string htmlHead = HtmlHead("404.3 - File or directory not found: MIME map policy prevents this request.");
            string htmlBody = HtmlBody("404.3 - File or directory not found: MIME map policy prevents this request.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 3,
                Description = "File or directory not found: MIME map policy prevents this request",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.4 - File or directory not found: No module handler is registered to handle the request.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_4()
        {
            string codeType = "Client Error";
            string details = "The resource you are looking for does not have a module or handler associated with it. It cannot be handled and served.";
            string htmlHead = HtmlHead("404.4 - File or directory not found: No module handler is registered to handle the request.");
            string htmlBody = HtmlBody("404.4 - File or directory not found: No module handler is registered to handle the request.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 4,
                Description = "File or directory not found: No module handler is registered to handle the request",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.5 - URL sequence denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_5()
        {
            string codeType = "Client Error";
            string details = "The specified URL sequence is not accepted by the server.";
            string htmlHead = HtmlHead("404.5 - URL sequence denied.");
            string htmlBody = HtmlBody("404.5 - URL sequence denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 5,
                Description = "URL sequence denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.6 - HTTP verb denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_6()
        {
            string codeType = "Client Error";
            string details = "The specified HTTP verb is not accepted by the server.";
            string htmlHead = HtmlHead("404.6 - HTTP verb denied.");
            string htmlBody = HtmlBody("404.6 - HTTP verb denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 6,
                Description = "HTTP verb denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.7 - File extension denied.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_7()
        {
            string codeType = "Client Error";
            string details = "The specified file extension of the resource is not accepted by the server.";
            string htmlHead = HtmlHead("404.7 - File extension denied.");
            string htmlBody = HtmlBody("404.7 - File extension denied.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 7,
                Description = "File extension denied",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.8 - URL namespace hidden.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_8()
        {
            string codeType = "Client Error";
            string details = "The namespace of the specified URL is hidden by configuration.";
            string htmlHead = HtmlHead("404.8 - URL namespace hidden.");
            string htmlBody = HtmlBody("404.8 - URL namespace hidden.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 8,
                Description = "URL namespace hidden",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.9 - File attribute hidden.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_9()
        {
            string codeType = "Client Error";
            string details = "The requested file has a hidden attribute which prevents it from being served.";
            string htmlHead = HtmlHead("404.9 - File attribute hidden.");
            string htmlBody = HtmlBody("404.9 - File attribute hidden.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 9,
                Description = "File attribute hidden",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.10 - Request header too long.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_10()
        {
            string codeType = "Client Error";
            string details = "One of the request headers is longer than the specified limit configured in the server.";
            string htmlHead = HtmlHead("404.10 - Request header too long.");
            string htmlBody = HtmlBody("404.10 - Request header too long.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 10,
                Description = "Request header too long",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.11 - URL is double-escaped.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_11()
        {
            string codeType = "Client Error";
            string details = "This URL is denied because it is susceptible to double-escaping attacks.";
            string htmlHead = HtmlHead("404.11 - URL is double-escaped.");
            string htmlBody = HtmlBody("404.11 - URL is double-escaped.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 11,
                Description = "URL is double-escaped",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.12 - URL has high bit characters.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_12()
        {
            string codeType = "Client Error";
            string details = "This URL is denied because it has high-bit characters.";
            string htmlHead = HtmlHead("404.12 - URL has high bit characters.");
            string htmlBody = HtmlBody("404.12 - URL has high bit characters.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 12,
                Description = "URL has high bit characters",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.13 - Content-Length too large.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_13()
        {
            string codeType = "Client Error";
            string details = "This URL is denied because the Content-Length set is longer than specified by configuration.";
            string htmlHead = HtmlHead("404.13 - Content-Length too large.");
            string htmlBody = HtmlBody("404.13 - Content-Length too large.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 13,
                Description = "Content-Length too large",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.14 - URL too long.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_14()
        {
            string codeType = "Client Error";
            string details = "This URL is denied because its length is longer than specified by configuration.";
            string htmlHead = HtmlHead("404.14 - URL too long.");
            string htmlBody = HtmlBody("404.14 - URL too long.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 14,
                Description = "URL too long",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 404.15 - Query-String too long.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_404_15()
        {
            string codeType = "Client Error";
            string details = "This URL is denied because its Query-String is longer than specified by configuration.";
            string htmlHead = HtmlHead("404.15 - Query-String too long.");
            string htmlBody = HtmlBody("404.15 - Query-String too long.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 404,
                SubCode = 15,
                Description = "Query-String too long",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 405 - HTTP verb used to access this page is not allowed.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_405_0()
        {
            string codeType = "Client Error";
            string details = "The page you are looking for cannot be displayed because an invalid method (HTTP verb) was used to attempt access.";
            string htmlHead = HtmlHead("405 - HTTP verb used to access this page is not allowed.");
            string htmlBody = HtmlBody("405 - HTTP verb used to access this page is not allowed.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 405,
                SubCode = 0,
                Description = "HTTP verb used to access this page is not allowed",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 406 - Client browser does not accept the MIME type of the requested page.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_406_0()
        {
            string codeType = "Client Error";
            string details = "The page you are looking for cannot be opened by your browser because it has a file name extension that your browser does not accept.";
            string htmlHead = HtmlHead("406 - Client browser does not accept the MIME type of the requested page.");
            string htmlBody = HtmlBody("406 - Client browser does not accept the MIME type of the requested page.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 406,
                SubCode = 0,
                Description = "Client browser does not accept the MIME type of the requested page",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 407 - Proxy Authentication Required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_407_0()
        {
            string codeType = "Client Error";
            string details = "The client must first authenticate itself with the proxy.";
            string htmlHead = HtmlHead("407 - Proxy Authentication Required.");
            string htmlBody = HtmlBody("407 - Proxy Authentication Required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 407,
                SubCode = 0,
                Description = "Proxy Authentication Required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 408 - Request Timeout.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_408_0()
        {
            string codeType = "Client Error";
            string details = "The server timed out waiting for the request.According to W3 HTTP specifications: The client did not produce a request within the time that the server was prepared to wait. The client MAY repeat the request without modifications at any later time.";
            string htmlHead = HtmlHead("408 - Request Timeout.");
            string htmlBody = HtmlBody("408 - Request Timeout.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 408,
                SubCode = 0,
                Description = "Request Timeout",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 409 - Conflict.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_409_0()
        {
            string codeType = "Client Error";
            string details = "Indicates that the request could not be processed because of conflict in the request, such as an edit conflict in the case of multiple updates.";
            string htmlHead = HtmlHead("409 - Conflict.");
            string htmlBody = HtmlBody("409 - Conflict.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 409,
                SubCode = 0,
                Description = "Conflict",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 410 - Gone.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_410_0()
        {
            string codeType = "Client Error";
            string details = "Indicates that the resource requested is no longer available and will not be available again.[2] This should be used when a resource has been intentionally removed and the resource should be purged. Upon receiving a 410 status code, the client should not request the resource again in the future. Clients such as search engines should remove the resource from their indices. Most use cases do not require clients and search engines to purge the resource, and a 404 Not Found may be used instead.";
            string htmlHead = HtmlHead("410 - Gone.");
            string htmlBody = HtmlBody("410 - Gone.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 410,
                SubCode = 0,
                Description = "Gone",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 411 - Length Required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_411_0()
        {
            string codeType = "Client Error";
            string details = "The request did not specify the length of its content, which is required by the requested resource.";
            string htmlHead = HtmlHead("411 - Length Required.");
            string htmlBody = HtmlBody("411 - Length Required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 411,
                SubCode = 0,
                Description = "Length Required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 412 - Precondition set by the client failed when evaluated on the Web server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_412_0()
        {
            string codeType = "Client Error";
            string details = "The request was not completed due to preconditions that are set in the request header. Preconditions prevent the requested method from being applied to a resource other than the one intended. An example of a precondition is testing for expired content in the page cache of the client.";
            string htmlHead = HtmlHead("412 - Precondition set by the client failed when evaluated on the Web server.");
            string htmlBody = HtmlBody("412 - Precondition set by the client failed when evaluated on the Web server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 412,
                SubCode = 0,
                Description = "Precondition set by the client failed when evaluated on the Web server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 413 - Request Entity Too Large.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_413_0()
        {
            string codeType = "Client Error";
            string details = "The request is larger than the server is willing or able to process.";
            string htmlHead = HtmlHead("413 - Request Entity Too Large.");
            string htmlBody = HtmlBody("413 - Request Entity Too Large.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 413,
                SubCode = 0,
                Description = "Request Entity Too Large",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 414 - Request-URI Too Long.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_414_0()
        {
            string codeType = "Client Error";
            string details = "The URI provided was too long for the server to process.[2] Often the result of too much data being encoded as a query-string of a GET request, in which case it should be converted to a POST request.";
            string htmlHead = HtmlHead("414 - Request-URI Too Long.");
            string htmlBody = HtmlBody("414 - Request-URI Too Long.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 414,
                SubCode = 0,
                Description = "Request-URI Too Long",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 415 - Unsupported Media Type.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_415_0()
        {
            string codeType = "Client Error";
            string details = "The request entity has a media type which the server or resource does not support.[2] For example, the client uploads an image as image/svg+xml, but the server requires that images use a different format.";
            string htmlHead = HtmlHead("415 - Unsupported Media Type.");
            string htmlBody = HtmlBody("415 - Unsupported Media Type.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 415,
                SubCode = 0,
                Description = "Unsupported Media Type",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 416 - Requested Range Not Satisfiable.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_416_0()
        {
            string codeType = "Client Error";
            string details = "The client has asked for a portion of the file, but the server cannot supply that portion.[2] For example, if the client asked for a part of the file that lies beyond the end of the file.";
            string htmlHead = HtmlHead("416 - Requested Range Not Satisfiable.");
            string htmlBody = HtmlBody("416 - Requested Range Not Satisfiable.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 416,
                SubCode = 0,
                Description = "Request Entity Too Large",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 417 - Expectation Failed.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_417_0()
        {
            string codeType = "Client Error";
            string details = "The server cannot meet the requirements of the Expect request-header field.";
            string htmlHead = HtmlHead("417 - Expectation Failed.");
            string htmlBody = HtmlBody("417 - Expectation Failed.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 417,
                SubCode = 0,
                Description = "Expectation Failed",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 422 - Unprocessable Entity.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_422_0()
        {
            string codeType = "Client Error";
            string details = "The request was well-formed but was unable to be followed due to semantic errors.";
            string htmlHead = HtmlHead("422 - Unprocessable Entity.");
            string htmlBody = HtmlBody("422 - Unprocessable Entity.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 422,
                SubCode = 0,
                Description = "Unprocessable Entity",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 423 - Locked.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_423_0()
        {
            string codeType = "Client Error";
            string details = "The resource that is being accessed is locked.";
            string htmlHead = HtmlHead("423 - Locked.");
            string htmlBody = HtmlBody("423 - Locked.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 423,
                SubCode = 0,
                Description = "Locked",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 424 - Failed Dependency.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_424_0()
        {
            string codeType = "Client Error";
            string details = "Indicates the method was not executed on a particular resource within its scope because some part of the method's execution failed causing the entire method to be aborted.";
            string htmlHead = HtmlHead("424 - Failed Dependency.");
            string htmlBody = HtmlBody("424 - Failed Dependency.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 424,
                SubCode = 0,
                Description = "Failed Dependency",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 426 - Upgrade Required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_426_0()
        {
            string codeType = "Client Error";
            string details = "The client should switch to a different protocol such as TLS/1.0.";
            string htmlHead = HtmlHead("426 - Upgrade Required.");
            string htmlBody = HtmlBody("426 - Upgrade Required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 426,
                SubCode = 0,
                Description = "Upgrade Required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 428 - Precondition Required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_428_0()
        {
            string codeType = "Client Error";
            string details = "The origin server requires the request to be conditional. Intended to prevent the 'lost update' problem, where a client GETs a resource's state, modifies it, and PUTs it back to the server, when meanwhile a third party has modified the state on the server, leading to a conflict.";
            string htmlHead = HtmlHead("428 - Precondition Required.");
            string htmlBody = HtmlBody("428 - Precondition Required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 428,
                SubCode = 0,
                Description = "Precondition Required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 429 - Too Many Requests.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_429_0()
        {
            string codeType = "Client Error";
            string details = "The user has sent too many requests in a given amount of time. Intended for use with rate limiting schemes.";
            string htmlHead = HtmlHead("429 - Too Many Requests.");
            string htmlBody = HtmlBody("429 - Too Many Requests.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 429,
                SubCode = 0,
                Description = "Too Many Requests",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 431 - Request Header Fields Too Large.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_431_0()
        {
            string codeType = "Client Error";
            string details = "The server is unwilling to process the request because either an individual header field, or all the header fields collectively, are too large.";
            string htmlHead = HtmlHead("431 - Request Header Fields Too Large.");
            string htmlBody = HtmlBody("431 - Request Header Fields Too Large.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 431,
                SubCode = 0,
                Description = "Request Header Fields Too Large",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 440 - Login Timeout.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_440_0()
        {
            string codeType = "Client Error";
            string details = "Indicates that your session has expired.";
            string htmlHead = HtmlHead("440 - Login Timeout.");
            string htmlBody = HtmlBody("440 - Login Timeout.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 440,
                SubCode = 0,
                Description = "Login Timeout",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500 - Internal server error.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_0()
        {
            string codeType = "Server Error";
            string details = "There is a problem with the resource you are looking for, and it cannot be displayed.";
            string htmlHead = HtmlHead("500 - Internal server error.");
            string htmlBody = HtmlBody("500 - Internal server error.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 0,
                Description = "Internal server error",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.13 - Server error: Web server is too busy.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_13()
        {
            string codeType = "Server Error";
            string details = "The request cannot be processed at this time. The amount of traffic exceeds the Web site's configured capacity.";
            string htmlHead = HtmlHead("500.13 - Server error: Web server is too busy.");
            string htmlBody = HtmlBody("500.13 - Server error: Web server is too busy.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 13,
                Description = "Server error: Web server is too busy",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.14 - Server error: Invalid application configuration on the server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_14()
        {
            string codeType = "Server Error";
            string details = "The request cannot be processed due to application configuration errors on the Web server.";
            string htmlHead = HtmlHead("500.14 - Server error: Invalid application configuration on the server.");
            string htmlBody = HtmlBody("500.14 - Server error: Invalid application configuration on the server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 14,
                Description = "Server error: Invalid application configuration on the server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.15 - Server error: Direct requests for GLOBAL.ASA are not allowed.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_15()
        {
            string codeType = "Server Error";
            string details = "GLOBAL.ASA is a special file that cannot be accessed directly by your browser.";
            string htmlHead = HtmlHead("500.15 - Server error: Direct requests for GLOBAL.ASA are not allowed.");
            string htmlBody = HtmlBody("500.15 - Server error: Direct requests for GLOBAL.ASA are not allowed.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 15,
                Description = "Server error: Direct requests for GLOBAL.ASA are not allowed",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.16 - Server error: UNC authorization credentials incorrect.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_16()
        {
            string codeType = "Server Error";
            string details = "The page you are requesting cannot be accessed due to UNC authorization settings that are configured incorrectly on the Web server.";
            string htmlHead = HtmlHead("500.16 - Server error: UNC authorization credentials incorrect.");
            string htmlBody = HtmlBody("500.16 - Server error: UNC authorization credentials incorrect.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 16,
                Description = "Server error: UNC authorization credentials incorrect",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.17 - Server error: URL authorization store cannot be found.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_17()
        {
            string codeType = "Server Error";
            string details = "The URL Authorization store for the page you requested cannot be found on the Web server, therefore your credentials cannot be verified.";
            string htmlHead = HtmlHead("500.17 - Server error: URL authorization store cannot be found.");
            string htmlBody = HtmlBody("500.17 - Server error: URL authorization store cannot be found.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 17,
                Description = "Server error: URL authorization store cannot be found",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.18 - Server error: URL authorization store cannot be opened.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_18()
        {
            string codeType = "Server Error";
            string details = "The URL Authorization store for the page you requested cannot be opened on the Web server, therefore your credentials cannot be verified.";
            string htmlHead = HtmlHead("500.18 - Server error: URL authorization store cannot be opened.");
            string htmlBody = HtmlBody("500.18 - Server error: URL authorization store cannot be opened.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 18,
                Description = "Server error: URL authorization store cannot be opened",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 500.19 - Server error: Data for this file is configured improperly.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_500_19()
        {
            string codeType = "Server Error";
            string details = "The requested page cannot be accessed because of a configuration error.";
            string htmlHead = HtmlHead("500.19 - Server error: Data for this file is configured improperly.");
            string htmlBody = HtmlBody("500.19 - Server error: Data for this file is configured improperly.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 500,
                SubCode = 19,
                Description = "Server error: Data for this file is configured improperly",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 501 - Header values specify a method that is not implemented.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_501_0()
        {
            string codeType = "Server Error";
            string details = "The page you are looking for cannot be displayed because a header value in the request does not match certain configuration settings on the Web server. For example, a request header might specify a POST to a static file that cannot be posted to, or specify a Transfer-Encoding value that cannot make use of compression.";
            string htmlHead = HtmlHead("501 - Header values specify a method that is not implemented.");
            string htmlBody = HtmlBody("501 - Header values specify a method that is not implemented.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 501,
                SubCode = 0,
                Description = "Header values specify a method that is not implemented",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 502 - Web server received an invalid response while acting as a gateway or proxy server.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_502_0()
        {
            string codeType = "Server Error";
            string details = "There is a problem with the page you are looking for, and it cannot be displayed. When the Web server (while acting as a gateway or proxy) contacted the upstream content server, it received an invalid response from the content server.";
            string htmlHead = HtmlHead("502 - Web server received an invalid response while acting as a gateway or proxy server.");
            string htmlBody = HtmlBody("502 - Web server received an invalid response while acting as a gateway or proxy server.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 502,
                SubCode = 0,
                Description = "Web server received an invalid response while acting as a gateway or proxy server",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 503 - Service Unavailable.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_503_0()
        {
            string codeType = "Server Error";
            string details = "The server is currently unavailable (because it is overloaded or down for maintenance).[2] Generally, this is a temporary state. Sometimes, this can be permanent as well on test servers.";
            string htmlHead = HtmlHead("503 - Service Unavailable.");
            string htmlBody = HtmlBody("503 - Service Unavailable.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 503,
                SubCode = 0,
                Description = "Service Unavailable",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 504 - Gateway Timeout.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_504_0()
        {
            string codeType = "Server Error";
            string details = "The server was acting as a gateway or proxy and did not receive a timely response from the upstream server.";
            string htmlHead = HtmlHead("504 - Gateway Timeout.");
            string htmlBody = HtmlBody("504 - Gateway Timeout.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 504,
                SubCode = 0,
                Description = "Gateway Timeout",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 505 - HTTP Version Not Supported.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_505_0()
        {
            string codeType = "Server Error";
            string details = "The server does not support the HTTP protocol version used in the request.";
            string htmlHead = HtmlHead("505 - HTTP Version Not Supported.");
            string htmlBody = HtmlBody("505 - HTTP Version Not Supported.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 505,
                SubCode = 0,
                Description = "HTTP Version Not Supported",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 506 - Variant Also Negotiates.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_506_0()
        {
            string codeType = "Server Error";
            string details = "Transparent content negotiation for the request results in a circular reference.";
            string htmlHead = HtmlHead("506 - Variant Also Negotiates.");
            string htmlBody = HtmlBody("506 - Variant Also Negotiates.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 506,
                SubCode = 0,
                Description = "Variant Also Negotiates",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 507 - Insufficient Storage.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_507_0()
        {
            string codeType = "Server Error";
            string details = "The server is unable to store the representation needed to complete the request.";
            string htmlHead = HtmlHead("507 - Insufficient Storage.");
            string htmlBody = HtmlBody("507 - Insufficient Storage.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 507,
                SubCode = 0,
                Description = "Insufficient Storage",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 508 - Loop Detected.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_508_0()
        {
            string codeType = "Server Error";
            string details = "The server detected an infinite loop while processing the request.";
            string htmlHead = HtmlHead("508 - Loop Detected.");
            string htmlBody = HtmlBody("508 - Loop Detected.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 508,
                SubCode = 0,
                Description = "Loop Detected",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 510 - Not Extended.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_510_0()
        {
            string codeType = "Server Error";
            string details = "The server detected an infinite loop while processing the request.";
            string htmlHead = HtmlHead("510 - Not Extended.");
            string htmlBody = HtmlBody("510 - Not Extended.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 510,
                SubCode = 0,
                Description = "Not Extended",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 511 - Network Authentication Required.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_511_0()
        {
            string codeType = "Server Error";
            string details = "The client needs to authenticate to gain network access. Intended for use by intercepting proxies used to control access to the network.";
            string htmlHead = HtmlHead("511 - Network Authentication Required.");
            string htmlBody = HtmlBody("511 - Network Authentication Required.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 511,
                SubCode = 0,
                Description = "Network Authentication Required",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }

        /// <summary>
        /// 522 - Connection Timed Out.
        /// </summary>
        /// <returns>Status code details.</returns>
        public static HttpStatusCode Code_522_0()
        {
            string codeType = "Server Error";
            string details = "The server connection timed out.";
            string htmlHead = HtmlHead("522 - Connection Timed Out.");
            string htmlBody = HtmlBody("522 - Connection Timed Out.", codeType, details);
            string html = "<html>" + htmlHead + htmlBody + "</html>";

            // Return the status code.
            return new HttpStatusCode()
            {
                Code = 522,
                SubCode = 0,
                Description = "Connection Timed Out",
                Details = details,
                CodeType = codeType,
                HtmlHead = htmlHead,
                HtmlBody = htmlBody,
                Html = html
            };
        }
    }
}
