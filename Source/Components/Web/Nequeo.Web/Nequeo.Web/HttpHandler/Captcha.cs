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
using System.Data;
using System.Security.Permissions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.Compilation;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading;

using Nequeo.Handler;

namespace Nequeo.Web.HttpHandler
{
    /// <summary>
    /// Captcha image creation control http handler.
    /// </summary>
    public class Captcha : IHttpHandler
    {
        /// <summary>
        /// Set which http handler should return data.
        /// </summary>
        public object HttpHandlerType = null;

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the IHttpHandler interface.
        /// </summary>
        /// <param name="context">An HttpContext object that provides references to the 
        /// intrinsic server objects (for example, Request, Response, Session, and Server)
        /// used to service HTTP requests.</param>
        [Logging(Nequeo.Handler.WriteTo.EventLog, Nequeo.Handler.LogType.Error)]
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (HttpHandlerType != null)
                {
                    // If the configuration data is assosicated with
                    // this http handler
                    if (HttpHandlerType is Nequeo.Web.HttpHandler.Captcha)
                    {
                        // Get the query string from the captcha key
                        NameValueCollection queryString = context.Request.QueryString;
                        string text = queryString[Nequeo.Web.UI.Control.Captcha.KEY];

                        // Responed to the http request by writting to
                        // the stream the captcha image for the source request
                        HttpResponse response = context.Response;
                        Bitmap bitmap = WebManager.GenerateImage(text);

                        // Write the new captcha image to the request stream.
                        if (bitmap != null)
                            bitmap.Save(response.OutputStream, ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex) 
            { 
                context.AddError(ex);
                LogHandler.WriteTypeMessage(ex.Message, typeof(Captcha).GetMethod("ProcessRequest"));
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the IHttpHandler instance.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
