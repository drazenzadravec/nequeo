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
    /// Page content loaded
    /// </summary>
    public partial class HttpPageContent
    {
        /// <summary>
        /// Get the html Upload Completed.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html001()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>Upload Completed</title>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>Upload Completed.</h2>" +
                                    "<h3>The upload completed.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html No Files Found.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html002()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>Upload File List</title>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>No Files Found.</h2>" +
                                    "<h3>No files have been found.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html Upload file size too large
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html003(long maxFileSize)
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>Upload File Too Large</title>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>Upload File Too Large.</h2>" +
                                    "<h3>The file is too large to be upload; maximum : " + String.Format("{0:#,#} Bytes", maxFileSize) + "</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 401 - Unauthorized: Access is denied due to invalid credentials.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html401()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>401 - Unauthorized: Access is denied due to invalid credentials.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>401 - Unauthorized: Access is denied due to invalid credentials.</h2>" +
                                    "<h3>You do not have permission to view this directory or page using the credentials that you supplied.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 403 - Forbidden: Access is denied.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html403()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>403 - Forbidden: Access is denied.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>403 - Forbidden: Access is denied.</h2>" +
                                    "<h3>You do not have permission to view this directory or page using the credentials that you supplied.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 404 - File or directory not found.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html404()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>404 - File or directory not found.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>404 - File or directory not found.</h2>" +
                                    "<h3>The resource you are looking for might have been removed, had its name changed, or is temporarily unavailable.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 405 - HTTP verb used to access this page is not allowed.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html405()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>405 - HTTP verb used to access this page is not allowed.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>405 - HTTP verb used to access this page is not allowed.</h2>" +
                                    "<h3>The page you are looking for cannot be displayed because an invalid method (HTTP verb) was used to attempt access.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 406 - Client browser does not accept the MIME type of the requested page.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html406()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>406 - Client browser does not accept the MIME type of the requested page.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>406 - Client browser does not accept the MIME type of the requested page.</h2>" +
                                    "<h3>The page you are looking for cannot be opened by your browser because it has a file name extension that your browser does not accept.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 412 - Precondition set by the client failed when evaluated on the Web server.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html412()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>412 - Precondition set by the client failed when evaluated on the Web server.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>412 - Precondition set by the client failed when evaluated on the Web server.</h2>" +
                                    "<h3>The request was not completed due to preconditions that are set in the request header. Preconditions prevent the requested method from being applied to a resource other than the one intended. An example of a precondition is testing for expired content in the page cache of the client.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 500 - Internal server error.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html500()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>500 - Internal server error.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>500 - Internal server error.</h2>" +
                                    "<h3>There is a problem with the resource you are looking for, and it cannot be displayed.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 501 - Header values specify a method that is not implemented.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html501()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>501 - Header values specify a method that is not implemented.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>501 - Header values specify a method that is not implemented.</h2>" +
                                    "<h3>The page you are looking for cannot be displayed because a header value in the request does not match certain configuration settings on the Web server. For example, a request header might specify a POST to a static file that cannot be posted to, or specify a Transfer-Encoding value that cannot make use of compression.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 502 - Web server received an invalid response while acting as a gateway or proxy server.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html502()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>502 - Web server received an invalid response while acting as a gateway or proxy server.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>502 - Web server received an invalid response while acting as a gateway or proxy server.</h2>" +
                                    "<h3>There is a problem with the page you are looking for, and it cannot be displayed. When the Web server (while acting as a gateway or proxy) contacted the upstream content server, it received an invalid response from the content server.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 701 - Request Invalid: The request xml is invalid.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html701()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>400 - Request Invalid: The request xml is invalid.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>400 - Request Invalid: The request xml is invalid.</h2>" +
                                    "<h3>The request xml passed is not valid, or a request xml has not been supplied.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }

        /// <summary>
        /// Get the html 702 - Request Invalid: The request json is invalid.
        /// </summary>
        /// <returns>The formatted content.</returns>
        public static string Html702()
        {
            return
                "<html>" +
                    "<head>" +
                        "<title>400 - Request Invalid: The request json is invalid.</title>" +
                        "<style type=\"text/css\">" +
                            "body{margin:0;font-size:.7em;font-family:Verdana, Arial, Helvetica, sans-serif;background:#EEEEEE;}" +
                            "fieldset{padding:0 15px 10px 15px;}" +
                            "h1{font-size:2.4em;margin:0;color:#FFF;}" +
                            "h2{font-size:1.7em;margin:0;color:#CC0000;}" +
                            "h3{font-size:1.2em;margin:10px 0 0 0;color:#000000;}" +
                            "#header{width:96%;margin:0 0 0 0;padding:6px 2% 6px 2%;font-family:\"trebuchet MS\", Verdana, sans-serif;color:#FFF;" +
                            "background-color:#555555;}" +
                            "#content{margin:0 0 0 2%;position:relative;}" +
                            ".content-container{background:#FFF;width:96%;margin-top:8px;padding:10px;position:relative;}" +
                        "</style>" +
                    "</head>" +
                    "<body>" +
                        "<div id=\"header\"><h1>Server Error</h1></div>" +
                        "<div id=\"content\">" +
                            "<div class=\"content-container\">" +
                                "<fieldset>" +
                                    "<h2>400 - Request Invalid: The request json is invalid.</h2>" +
                                    "<h3>The request json passed is not valid, or a request json has not been supplied.</h3>" +
                                "</fieldset>" +
                            "</div>" +
                        "</div>" +
                    "</body>" +
                "</html>";
        }
    }
}
